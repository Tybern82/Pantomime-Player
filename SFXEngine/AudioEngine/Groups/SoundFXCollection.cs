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

        private int position;
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

        public SoundFXCollection() : this(WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.Instance.AudioSampleRate, AudioPlaybackEngine.Instance.AudioChannelCount)) { }

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
            if (fmt == null) fmt = WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.Instance.AudioSampleRate, AudioPlaybackEngine.Instance.AudioChannelCount);
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
                    if (fx.length > length) length = fx.length;
                    registerMixerInput(fx);
                }
            }
        }

        public void removeSoundFX(SoundFX fx) {
            lock (_play_lock) {
                if (effects.Contains(fx)) {
                    effects.Remove(fx);
                    unregisterMixerInput(fx);
                    if ((!fx.canDuplicate) || (!fx.canSeek) || (fx.length == length)) updateInfo();
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
                    if (fx.length > length) length = fx.length;
                }
            }
        }

        public override Boolean seekForward(Int64 sampleLength) {
            lock (_play_lock) {
                bool _result = true;
                foreach (SoundFX fx in effects) {
                    _result &= fx.seekForward(sampleLength);
                }
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

        public override Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            lock (_play_lock) {
                if (isPaused) {
                    return readSilence(buffer, offset, count);
                } else if (isStopped) return 0;
                if (!isPlaying) {
                    isPlaying = true;
                    onPlay.triggerEvent(this);
                }
                var samplesRead = mixer.Read(buffer, offset, count);
                if (samplesRead == 0) stop();
                else onSample.triggerEvent(this);
                position += samplesRead;
                return samplesRead;
            }
        }
    }
}
