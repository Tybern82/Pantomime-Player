using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine {
    public abstract class AbstractSoundFX : SoundFX {
        public TimeSpan length { get; protected set; }
        public WaveFormat WaveFormat { get; protected set; }
        public EventRegister onStart { get; } = new EventRegister();
        public EventRegister onStop { get; } = new EventRegister();
        public EventRegister onSeek { get; } = new EventRegister();

        private EffectState _state;
        public EffectState state {
            get {
                return _state;
            }
            set {
                if (value == EffectState.RUNNING) onStart.triggerEvent(this);
                if (value == EffectState.STOPPED) onStop.triggerEvent(this);
                _state = value;
            }
        }

        public abstract SoundFX cache();
        public abstract SoundFX dup();
        public abstract Int32 Read(Single[] buffer, Int32 offset, Int32 count);
        public abstract Boolean ResetPlayback();

        public bool play(PlaybackDevice dev) {
            if (state != EffectState.READY) return false;
            if (dev is AudioPlaybackEngine) {
                var audio = (AudioPlaybackEngine)dev;
                toSampleProvider(); // prepare the cachedProvider
                if (cachedProvider.WaveFormat.SampleRate != audio.AudioSampleRate)
                    cachedProvider = new WdlResamplingSampleProvider(cachedProvider, audio.AudioSampleRate);
                cachedProvider = AudioPlaybackEngine.AdjustChannelCount(cachedProvider, audio.AudioChannelCount);
            }
            var result = dev.Play(this);
            if (result) {
                state = EffectState.RUNNING;
            } else {
                state = EffectState.STOPPED;
            }
            return result;
        }

        public Boolean StopPlayback() {
            if (state != EffectState.RUNNING) return false;
            AudioPlaybackEngine.Instance.Stop(toSampleProvider());
            state = EffectState.STOPPED;
            return true;
        }

        public bool playBuffer() {
            if (state != EffectState.READY) return false;
            state = EffectState.RUNNING;
            return true;
        }

        public Boolean makeReady() {
            if ((state == EffectState.RUNNING) || (state == EffectState.PAUSED)) return false;
            ResetPlayback();
            state = EffectState.READY;
            return true;
        }

        private ISampleProvider cachedProvider = null;
        public ISampleProvider toSampleProvider() {
            if (cachedProvider == null) {
                cachedProvider = this;
                // prepare for sample playback
                makeReady();
                playBuffer();
            }
            return cachedProvider;
        }

        public bool SeekForward(TimeSpan length) {
            long sampleLength = (long)Math.Round(length.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
            return SeekForward(sampleLength);
        }

        public bool SeekForward(long sampleLength) {
            if ((state == EffectState.RUNNING) || (state == EffectState.READY)) {
                EffectState currState = state;
                state = EffectState.RUNNING;    // necessary to allow Read to actually access samples
                var readBuffer = new float[WaveFormat.SampleRate * WaveFormat.Channels];
                int samplesRead;
                while (sampleLength > 0) {
                    samplesRead = Read(readBuffer, 0, (int)Math.Min((long)readBuffer.Length, sampleLength));
                    sampleLength -= samplesRead;
                }
                state = currState;  // restore the previous state
                onSeek.triggerEvent(this);
                return true;
            } else {
                return false;
            }
        }

        Effect Effect.cache() {
            return cache();
        }

        Effect Effect.dup() {
            return dup();
        }
    }
}
