using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Groups {
    public class SoundFXSequence : SoundFX {

        private List<SoundFX> effects = new List<SoundFX>();
        private List<ISampleProvider> samples = new List<ISampleProvider>();

        private WaveFormat _WaveFormat = AudioPlaybackEngine.Instance.WaveFormat;
        public override WaveFormat WaveFormat {
            get {
                return _WaveFormat;
            }
        }

        private int currentFX = 0;

        private int position = 0;
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

        public SoundFXSequence() : this(WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.Instance.AudioSampleRate, AudioPlaybackEngine.Instance.AudioChannelCount)) { }

        public SoundFXSequence(WaveFormat fmt) {
            this._WaveFormat = fmt;
        }

        public SoundFXSequence(IEnumerable<SoundFX> fxList) {
            WaveFormat fmt = null;
            foreach (SoundFX fx in fxList) {
                if (fmt == null) fmt = fx.WaveFormat;
                else if (fmt != fx.WaveFormat) {
                    // found a non-matching wave format, default to standard
                    fmt = null;
                    break;
                }
            }
            if (fmt == null) fmt = WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.Instance.AudioSampleRate, AudioPlaybackEngine.Instance.AudioChannelCount);
            this._WaveFormat = fmt;
            foreach (SoundFX fx in fxList) {
                addSoundFX(fx);
            }
        }

        public SoundFXSequence(WaveFormat fmt, IEnumerable<SoundFX> fxList) {
            this._WaveFormat = fmt;
            foreach (SoundFX fx in fxList) {
                addSoundFX(fx);
            }
        }

        public void addSoundFX(SoundFX fx) {
            lock (_play_lock) {
                if (!effects.Contains(fx)) {
                    effects.Add(fx);
                    samples.Add(SFXUtilities.ConvertSampleFormat(fx, WaveFormat));
                    if (!fx.canSeek) canSeek = false;
                    if (!fx.canDuplicate) canDuplicate = false;
                    this.length += fx.length;
                }
            }
        }

        public void removeSoundFX(SoundFX fx) {
            lock (_play_lock) {
                if (effects.Contains(fx)) {
                    if (isPlaying) throw new InvalidOperationException("Cannot remove effects from a currently playing sequence.");
                    int index = effects.IndexOf(fx);
                    effects.Remove(fx);
                    samples.RemoveAt(index);
                    if ((!fx.canDuplicate) || (!fx.canSeek)) updateInfo();
                    this.length -= fx.length;
                }
            }
        }

        private void updateInfo() {
            lock (_play_lock) {
                canSeek = true;
                canDuplicate = true;
                foreach (SoundFX fx in effects) {
                    if (!fx.canDuplicate) canDuplicate = false;
                    if (!fx.canSeek) canSeek = false;
                }
            }
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

        public override Boolean seekTo(Int64 sampleIndex) {
            if (!canSeek) return false;
            if (sampleIndex == 0) {
                // reset to start
                lock (_play_lock) {
                    // reset our indexes
                    currentFX = 0;
                    position = 0;
                    bool _result = true;
                    // reset the underlying streams
                    foreach (SoundFX fx in effects) _result &= fx.reset();
                    return _result;
                }
            } else {
                seekTo(0);
                return seekForward(sampleIndex);
            }
        }

        public override Int32 ReadSamples(Single[] buffer, Int32 offset, Int32 count) {
            var samplesRead = 0;        // track how many samples have been loaded into the buffer
            while ((samplesRead < count) && (currentFX < samples.Count)) {  // while we need more samples, and there are still sources available
                var needed = count - samplesRead;   // determine how many samples are still required
                var currRead = samples[currentFX].Read(buffer, offset+samplesRead, needed); // try to read that many samples from the next source
                samplesRead += currRead;    // update the number of produced samples
                if (currRead == 0) currentFX++; // move to the next source if we have exhausted this one
            }
            return samplesRead; // return the number of samples successfully read
        }
    }
}