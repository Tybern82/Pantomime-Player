using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Groups;
using SFXEngine.AudioEngine.Effects;
using SFXEngine.AudioEngine.Adapters;

using NAudio.Wave;

namespace SFXPlayer {
    public class Cue : System.ComponentModel.INotifyPropertyChanged {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Cue));

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        [Ignore]
        public SFXShowFile currentShow { get; set; }

        private uint _CueID;
        [PrimaryKey, AutoIncrement]
        public uint CueID {
            get { return _CueID; }
            set { _CueID = value;  NotifyPropertyChanged(); }
        }

        private string _Name;
        [Indexed]
        public virtual string Name {
            get { return _Name; }
            set { _Name = value;  NotifyPropertyChanged(); }
        }

        private TimeSpan _SeekTo = TimeSpan.Zero;
        public TimeSpan SeekTo {
            get { return _SeekTo; }
            set { _SeekTo = (value < TimeSpan.Zero) ? TimeSpan.Zero : value; NotifyPropertyChanged(); }
        }

        private TimeSpan _FadeInDuration = TimeSpan.Zero;
        public TimeSpan FadeInDuration {
            get { return _FadeInDuration; }
            set { _FadeInDuration = value;  NotifyPropertyChanged(); }
        }

        private TimeSpan _AutoFadeOutTime = TimeSpan.Zero;
        public TimeSpan AutoFadeOutTime {
            get { return _AutoFadeOutTime; }
            set { _AutoFadeOutTime = value;  NotifyPropertyChanged(); }
        }

        private TimeSpan _FadeOutDuration = TimeSpan.Zero;
        public TimeSpan FadeOutDuration {
            get { return _FadeOutDuration; }
            set { _FadeOutDuration = value;  NotifyPropertyChanged(); }
        }

        public double _Volume = 1.0;
        public double Volume {
            get { return _Volume; }
            set {
                if (value < 0) _Volume = 0;
                else if (value > SFXEngineProperties.getMaxVolume()) _Volume = SFXEngineProperties.getMaxVolume();
                else _Volume = value;
                NotifyPropertyChanged();
            }
        }

        private TimeSpan _Length = TimeSpan.Zero;
        public virtual TimeSpan Length {
            get { return _Length; }
            set { _Length = value;  NotifyPropertyChanged(); }
        }

        public virtual SoundFX loadCue() {
            return null;
        }

        [Ignore]
        public FadeSampleProvider source {
            get {
                if (_source == null) _source = prepareCue(loadCue());
                return _source;
            }
        }
        private FadeSampleProvider _source = null;

        protected FadeSampleProvider prepareCue(SoundFX cue) {
            if (cue == null) return null;
            if (SeekTo != cue.currentTime) {
                // we need to seek the underlying stream
                if (cue.canSeek) {
                    cue.seekTo(SeekTo);
                } else if ((SeekTo - cue.currentTime) > TimeSpan.Zero) {
                    // if we can't seek the underlying stream, try to seek from the current position forward
                    // this is guaranteed to be valid if we are at the start of the underlying stream
                    cue.seekForward(SeekTo-cue.currentTime);
                } else {
                    logger.Error("Unable to complete seek on a stream - stream is unseekable, and the requested time is earlier than the current position.");
                }
            }
            ISampleProvider _result = cue;
            if (Volume != 1.0) {
                _result = new VolumeControlProvider(cue, Volume);
            }
            return new FadeSampleProvider(_result, FadeInDuration, AutoFadeOutTime, FadeOutDuration);
        }
    }

    public class SilenceCue : Cue {

        public override SoundFX loadCue() {
            return new Silence(Length);
        }

        public SilenceCue() {
            this.Name = "Silence";
        }
    }

    public class RegisteredCue : Cue {
        private uint _SourceID;
        [Indexed]
        public uint SourceID {
            get { return _SourceID; }
            set { _SourceID = value;  NotifyPropertyChanged(); }
        }

        public override TimeSpan Length {
            get {
                return (currentShow == null) ? base.Length : currentShow.getRegisteredEffect(SourceID).Length;
            }

            set {
                base.Length = value;
            }
        }

        public override SoundFX loadCue() {
            return (currentShow == null) ? null : currentShow.getRegisteredEffect(SourceID).fx.dup();
        }
    }

    public enum GroupElementType {
        SEQUENCE, COLLECTION
    }

    public class CueGroupElement {
        [PrimaryKey]
        public uint? ElementID { get; set; }
        
        public uint CueID { get; set; }
        public uint ItemSequence { get; set; }
        public uint ItemID { get; set; }
    }

    public class CueGroup : Cue {
        [Ignore]
        public SortedDictionary<uint, Cue> cueItems { get; } = new SortedDictionary<uint, Cue>();
        
        public override string Name {
            get {
                switch (Type) {
                    case GroupElementType.COLLECTION: return "Collection";
                    case GroupElementType.SEQUENCE: return "Sequence";
                    default: return base.Name;
                }
            }

            set {
                base.Name = value;
            }
        }

        private GroupElementType _Type;
        public GroupElementType Type {
            get { return _Type; }
            set { _Type = value;  NotifyPropertyChanged(); }
        }

        public override TimeSpan Length {
            get {
                TimeSpan _result = TimeSpan.Zero;
                switch (Type) {
                    case GroupElementType.COLLECTION:
                        foreach (var item in cueItems.Values) {
                            if (_result < item.Length) _result = item.Length;
                        }
                        return _result;

                    case GroupElementType.SEQUENCE:
                        foreach (Cue c in cueItems.Values) {
                            _result += c.Length;
                        }
                        return _result;
                    default:
                        return _result;
                }
            }

            set {
                base.Length = value;
            }
        }

        private uint getMaxCue() {
            return (cueItems.Count == 0) ? 0 : cueItems.Keys.Max() + 1;
        }

        public void addElement(CueGroupElement e) {
            addElement(e.ItemSequence, currentShow.getCue(e.ItemID));
        }

        public void addElement(uint sequence, Cue cue) {
            cueItems.Add(sequence, cue);
            currentShow?.updateTable(this);
            NotifyPropertyChanged("Length");
        }

        public uint addElement(Cue cue) {
            uint nValue = getMaxCue();
            addElement(nValue, cue);
            return nValue;
        }

        public long sequenceOf(Cue c) {
            if (!contains(c)) return -1;
            else {
                foreach (var item in cueItems) {
                    if (item.Value == c) return item.Key;
                }
            }
            return -1;
        }

        public void insertAt(uint nPos, Cue c) {
            uint maxCue = getMaxCue();
            if (nPos <= maxCue) {
                // need to move existing elements up
                for (uint x = maxCue; x > nPos; x--) {
                    cueItems[x + 1] = cueItems[x];
                    cueItems.Remove(x);
                }
            }
            cueItems[nPos] = c;
            currentShow?.updateTable(this);
            NotifyPropertyChanged("Length");
        }

        public void insertAfter(Cue item, Cue nCue) {
            long pos = sequenceOf(item);
            uint nPos = (pos == -1) ? getMaxCue() : (uint)pos + 1;
            insertAt(nPos, nCue);
        }

        public void insertBefore(Cue item, Cue nCue) {
            long pos = sequenceOf(item);
            uint nPos = (pos == -1) ? 0 : (uint)pos;
            insertAt(nPos, nCue);
        }

        public void removeElement(Cue c) {
            foreach (uint i in cueItems.Keys) {
                if (cueItems[i] == c) cueItems.Remove(i);
            }
            currentShow?.updateTable(this);
            NotifyPropertyChanged("Length");
        }

        public bool contains(Cue c) {
            return cueItems.Values.Contains(c);
        }

        public override SoundFX loadCue() {
            List<SoundFX> items = new List<SoundFX>();
            uint max = getMaxCue();
            for (uint x = 0; x < max; x++) {
                Cue c = cueItems[x];
                if (c == null) logger.Error("Missing item from sequence...");
                else {
                    SoundFX fx = c.loadCue();
                    if (fx == null) logger.Error("Loaded null effect from sequence item...");
                    else {
                        items.Add(fx);
                    }
                }
            }
            switch (Type) {
                case GroupElementType.COLLECTION:
                    return new SoundFXCollection(items);
                case GroupElementType.SEQUENCE:
                    return new SoundFXSequence(items);
                default:
                    throw new UnsupportedAudioException("Unknown cue grouping type.");
            }
        }
    }
}
