using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFXEngine.AudioEngine;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine.AudioEngine.Groups {
    public class SoundFXCollection : SoundFX {

        private List<SoundFX> effects = new List<SoundFX>();
        private Dictionary<SoundFX, ISampleProvider> mixerInputs = new Dictionary<SoundFX, ISampleProvider>();

        public override WaveFormat WaveFormat {
            get {
                return mixer.WaveFormat;
            }
        }

        private long position;
        public override Int64 currentSample {
            get {
                return position;
            }
        }

        public override TimeSpan currentTime {
            get {
                long ticks = (long)Math.Round((double)(currentSample * TimeSpan.TicksPerSecond / (WaveFormat.SampleRate * WaveFormat.Channels)), MidpointRounding.ToEven);
                return new TimeSpan(ticks);
            }
        }

        private MixingSampleProvider mixer;

        public SoundFXCollection() : this(AudioPlaybackEngine.Instance.WaveFormat) { }

        public SoundFXCollection(WaveFormat fmt) {
            this.mixer = new MixingSampleProvider(fmt);
        }

        public SoundFXCollection(IEnumerable<SoundFX> fxList) {
            WaveFormat fmt = null;
            foreach (SoundFX fx in fxList) {
                if (fmt == null) fmt = fx.WaveFormat;
                else if (fmt != fx.WaveFormat) {
                    // found a non-matching wave format, default to standard
                    fmt = null;
                    break;
                }
            }
            if (fmt == null) fmt = AudioPlaybackEngine.Instance.WaveFormat;
            this.mixer = new MixingSampleProvider(fmt);
            foreach (SoundFX fx in fxList) {
                addSoundFX(fx);
            }
        }

        public SoundFXCollection(WaveFormat fmt, IEnumerable<SoundFX> fxList) {
            this.mixer = new MixingSampleProvider(fmt);
            foreach (SoundFX fx in fxList) {
                addSoundFX(fx);
            }
        }

        public void addSoundFX(SoundFX fx) {
            lock (_play_lock) {
                if (!effects.Contains(fx)) {
                    effects.Add(fx);
                    if (!fx.canSeek) canSeek = false;
                    if (!fx.canDuplicate) canDuplicate = false;
                    if (fx.playLengthRemaining > length) length = fx.playLengthRemaining;
                    registerMixerInput(fx);
                }
            }
        }

        public void removeSoundFX(SoundFX fx) {
            lock (_play_lock) {
                if (effects.Contains(fx)) {
                    effects.Remove(fx);
                    unregisterMixerInput(fx);
                    if ((!fx.canDuplicate) || (!fx.canSeek) /*|| (fx.playLengthRemaining == length)*/)
                        updateInfo();
                }
            }
        }

        private void registerMixerInput(SoundFX fx) {
            lock (_play_lock) {
                ISampleProvider sample = fx;
                // adjust to correct format (as required)....
                sample = SFXUtilities.ConvertSampleFormat(sample, mixer.WaveFormat);
                mixer.AddMixerInput(sample);
                mixerInputs[fx] = sample;
            }
        }

        private void unregisterMixerInput(SoundFX fx) {
            lock (_play_lock) {
                if (mixerInputs.ContainsKey(fx)) {
                    ISampleProvider sample = mixerInputs[fx];
                    mixer.RemoveMixerInput(sample);
                    mixerInputs.Remove(fx);
                }
            }
        }

        private void updateInfo() {
            lock (_play_lock) {
                canSeek = true;
                canDuplicate = true;
                length = TimeSpan.Zero;
                foreach (SoundFX fx in effects) {
                    if (!fx.canDuplicate) canDuplicate = false;
                    if (!fx.canSeek) canSeek = false;
                    if (fx.playLengthRemaining > length) length = fx.playLengthRemaining;
                }
            }
        }

        public override Boolean seekForward(Int64 sampleLength) {
            lock (_play_lock) {
                bool _result = true;
                foreach (SoundFX fx in effects) {
                    _result &= fx.seekForward(sampleLength);
                }
                position += sampleLength;
                if (_result) onSeek.triggerEvent(this);
                return _result;
            }
        }

        public override Boolean seekForward(TimeSpan ts) {
            lock (_play_lock) {
                bool _result = true;
                foreach (SoundFX fx in effects) {
                    _result &= fx.seekForward(ts);
                }
                position += (long)Math.Round(ts.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
                if (_result) onSeek.triggerEvent(this);
                return _result;
            }
        }

        public override Boolean seekTo(Int64 sampleIndex) {
            if (canSeek) {
                lock (_play_lock) {
                    bool _result = canSeek;
                    foreach (SoundFX fx in effects) {
                        _result &= fx.seekTo(sampleIndex);
                    }
                    position = sampleIndex;
                    if (_result) onSeek.triggerEvent(this);
                    return _result;
                }
            }
            return canSeek;
        }

        public override Boolean seekTo(TimeSpan index) {
            if (canSeek) {
                lock (_play_lock) {
                    bool _result = canSeek;
                    foreach (SoundFX fx in effects) {
                        _result &= fx.seekTo(index);
                    }
                    position = (long)Math.Round(index.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
                    if (_result) onSeek.triggerEvent(this);
                    return _result;
                }
            }
            return canSeek;
        }

        public override SoundFX dup() {
            if (canDuplicate) {
                List<SoundFX> _dupEffects = new List<SoundFX>();
                foreach (SoundFX fx in effects) {
                    _dupEffects.Add(fx.dup());
                }
                return new SoundFXCollection(WaveFormat, _dupEffects);
            }
            return null;
        }

        public override Int32 ReadSamples(Single[] buffer, Int32 offset, Int32 count) {
            var samplesRead = mixer.Read(buffer, offset, count);
            position += samplesRead;
            return samplesRead;
        }
    }
}
