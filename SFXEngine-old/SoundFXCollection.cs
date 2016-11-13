using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine {
    public class SoundFXCollection : AbstractSoundFX, EffectCollection {

        private List<SoundFX> effects = new List<SoundFX>();
        private object effects_lock = new object();

        private MixingSampleProvider mixer;

        public SoundFXCollection() {
            this.mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.Instance.AudioSampleRate, AudioPlaybackEngine.Instance.AudioChannelCount));
            this.WaveFormat = mixer.WaveFormat;
        }

        public SoundFXCollection(IEnumerable<SoundFX> effects) : this() {
            this.effects.AddRange(effects);
            TimeSpan _length = new TimeSpan(0);
            foreach (SoundFX fx in effects) {
                if (fx.length > _length) _length = fx.length;
            }
            this.length = _length;
        }

        public bool addEffect(SoundFX fx) {
            lock (effects_lock) {
                if (effects.Contains(fx)) return false;
                effects.Add(fx);
                if (fx.length > length) length = fx.length;
            }
            return true;
        }

        public bool removeEffect(SoundFX fx) {
            lock (effects_lock) {
                if (!effects.Contains(fx)) return false;
                effects.Remove(fx);
                mixer.RemoveMixerInput(fx.toSampleProvider());
                if (fx.length >= length) {  // only need to recalculate if the removed effect may have been the longest in the collection
                    TimeSpan _length = new TimeSpan(0);
                    foreach (SoundFX i in effects) {
                        if (_length < i.length) _length = i.length;
                    }
                    length = _length;
                }
            }
            return true;
        }

        public bool addEffect(Effect e) {
            if (e is SoundFX) return addEffect((SoundFX)e);
            return false;
        }

        public bool removeEffect(Effect e) {
            if (e is SoundFX) return removeEffect((SoundFX)e);
            return false;
        }

        public override Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            return mixer.Read(buffer, offset, count);
        }

        public new bool makeReady() {
            if ((state == EffectState.RUNNING) || (state == EffectState.PAUSED)) return false;
            lock (effects_lock) {
                ResetPlayback();
                state = EffectState.READY;
                foreach (SoundFX fx in effects) {
                    ISampleProvider snd = fx.toSampleProvider(); // automatically calls makeReady and playBuffer on the stream
                                                                 // Resample input, if required
                    if (snd.WaveFormat.SampleRate != mixer.WaveFormat.SampleRate) snd = new WdlResamplingSampleProvider(snd, mixer.WaveFormat.SampleRate);
                    // Then adjust the channel outputs
                    if (snd.WaveFormat.Channels != mixer.WaveFormat.Channels) snd = AudioPlaybackEngine.AdjustChannelCount(snd, (UInt16)mixer.WaveFormat.Channels);
                    // Finally begin playing
                    mixer.AddMixerInput(snd);
                }
            }
            return true;
        }

        public override Boolean ResetPlayback() {
            if (this.state == EffectState.RUNNING) return false;
            if (this.state == EffectState.LOADING) return true; // initial start, already reset
            if (this.state == EffectState.READY) return true;   // initial start, already reset
            bool _result = true;
            lock (effects_lock) {
                mixer.RemoveAllMixerInputs();
                foreach (SoundFX fx in effects) {
                    _result &= fx.ResetPlayback();
                }
            }
            return _result;
        }

        public new Boolean StopPlayback() {
            lock (effects_lock) {
                bool _result = true;
                foreach (SoundFX fx in effects) {
                    _result &= fx.StopPlayback();
                }
                return _result;
            }
        }

        public override SoundFX cache() {
            return new BufferedSound(toSampleProvider());
        }

        public override SoundFX dup() {
            lock (effects_lock) {
                return new SoundFXCollection(effects);
            }
        }
    }
}
