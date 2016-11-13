using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine {
    public class CachedSound : AbstractSoundFX {
        public Single[] AudioData { get; private set; }

        private CachedSound() {
            this.state = EffectState.LOADING;
        }

        public CachedSound(Single[] audio, WaveFormat fmt, TimeSpan length) : this() { 
            this.AudioData = audio;
            this.WaveFormat = fmt;
            this.length = length;
        }

        public CachedSound(SoundFX source) : this(source.toSampleProvider()) {}

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

        public override SoundFX cache() {
            // cached sound is always cached
            return this;
        }

        public override SoundFX dup() {
            return new CachedSound(this.AudioData, this.WaveFormat, this.length);
        }

        public override bool ResetPlayback() {
            if (this.state == EffectState.RUNNING) return false;
            if (this.state == EffectState.LOADING) return true; // initial start, already reset
            if (this.state == EffectState.READY) return true;   // initial start, already reset
            this.state = EffectState.LOADING;
            this.position = 0;
            return true;
        }

        private long position;

        public override int Read(float[] buffer, int offset, int count) {
            if (this.state != EffectState.RUNNING) return 0;
            var availableSamples = this.AudioData.Length - position;
            if (availableSamples == 0) this.state = EffectState.STOPPED;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(this.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }
    }
}
