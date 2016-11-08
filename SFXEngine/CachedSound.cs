using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine {
    public class CachedSound : SoundFX {
        public Single[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public TimeSpan length { get; private set; }
        public EffectState state { get; private set; }

        private CachedSound() {
            this.state = EffectState.LOADING;
        }

        private CachedSound(Single[] audio, WaveFormat fmt, TimeSpan length) : this() { 
            this.AudioData = audio;
            this.WaveFormat = fmt;
            this.length = length;
        }

        public CachedSound(ISampleProvider source) : this() {
            var wholeFile = new List<float>();
            var readBuffer = new float[source.WaveFormat.SampleRate * source.WaveFormat.Channels];
            int samplesRead;
            long totalSamples = 0;
            while ((samplesRead = source.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
                totalSamples += samplesRead;
                if (totalSamples > SFXEngineProperties.MaxCachedSoundSize) throw new ArgumentOutOfRangeException("source", "Attempt to cache a sample longer than the maximum permitted.");
            }
            long ticks = (long)Math.Round((double)(totalSamples * TimeSpan.TicksPerSecond / (source.WaveFormat.SampleRate * source.WaveFormat.Channels)), MidpointRounding.ToEven);
            this.AudioData = wholeFile.ToArray();
            this.WaveFormat = source.WaveFormat;
            this.length = new TimeSpan(ticks);
        }

        private ISampleProvider cachedProvider = null;
        public ISampleProvider toSampleProvider() {
            if (cachedProvider == null) cachedProvider = this;
            return cachedProvider;
        }

        public Boolean StopPlayback() {
            if (state != EffectState.RUNNING) return false;
            AudioPlaybackEngine.Instance.Stop(toSampleProvider());
            state = EffectState.STOPPED;
            return true;
        }

        public SoundFX cache() {
            // cached sound is always cached
            return this;
        }

        public Boolean makeReady() {
            if ((state == EffectState.RUNNING) || (state == EffectState.PAUSED)) return false;
            ResetPlayback();
            state = EffectState.READY;
            return true;
        }

        public Effect dup() {
            return new CachedSound(this.AudioData, this.WaveFormat, this.length);
        }

        public bool play(PlaybackDevice dev) {
            if (state != EffectState.READY) return false;
            if (dev is AudioPlaybackEngine) {
                var audio = (AudioPlaybackEngine)dev;
                if (cachedProvider.WaveFormat.SampleRate != audio.AudioSampleRate)
                    cachedProvider = new WdlResamplingSampleProvider(cachedProvider, audio.AudioSampleRate);
                cachedProvider = AudioPlaybackEngine.AdjustChannelCount(cachedProvider, audio.AudioChannelCount);
            }
            var result = dev.Play(this);
            if (result) {
                state = EffectState.RUNNING;
                return true;
            } else {
                state = EffectState.STOPPED;
                return false;
            }
        }

        public bool ResetPlayback() {
            if (this.state == EffectState.RUNNING) return false;
            this.state = EffectState.LOADING;
            this.position = 0;
            return true;
        }

        private long position;

        public int Read(float[] buffer, int offset, int count) {
            if (this.state != EffectState.RUNNING) return 0;
            var availableSamples = this.AudioData.Length - position;
            if (availableSamples == 0) this.state = EffectState.STOPPED;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(this.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        Effect Effect.cache() {
            return cache();
        }
    }
}
