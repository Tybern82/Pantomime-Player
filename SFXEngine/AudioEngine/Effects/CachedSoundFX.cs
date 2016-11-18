using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Effects {
    public class CachedSoundFX : SFXEngine.AudioEngine.SoundFX {

        private float[] audioData;
        private int position = 0;

        private WaveFormat _WaveFormat;
        public override WaveFormat WaveFormat { get { return _WaveFormat; } }
        
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

        public override Boolean isCachable {
            get {
                return true;
            }
        }

        private CachedSoundFX(WaveFormat fmt, TimeSpan length) {
            this.length = length;
            this._WaveFormat = fmt;
            this.canSeek = true;
        }

        private CachedSoundFX(float[] buffer, WaveFormat fmt, TimeSpan length) : this(fmt, length) {
            this.audioData = buffer;
        }

        public CachedSoundFX(SFXEngine.AudioEngine.SoundFX fx):this(fx.WaveFormat, fx.length){
            this.audioData = CachedSoundFX.cacheAudioData(fx);
        }

        public static CachedSoundFX cacheSampleProvider(ISampleProvider source) {
            if (source is CachedSoundFX) {
                return (CachedSoundFX)((CachedSoundFX)source).dup();
            }
            float[] audioData = cacheAudioData(source);
            long ticks = (long)Math.Round((double)(audioData.Length * TimeSpan.TicksPerSecond / (source.WaveFormat.SampleRate * source.WaveFormat.Channels)), MidpointRounding.ToEven);
            return new CachedSoundFX(audioData, source.WaveFormat, new TimeSpan(ticks));
        }

        private static float[] cacheAudioData(ISampleProvider source) {
            var wholeFile = new List<float>();
            var readBuffer = new float[source.WaveFormat.SampleRate * source.WaveFormat.Channels];
            int samplesRead;
            long totalSamples = 0;
            while ((samplesRead = source.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
                totalSamples += samplesRead;
                if (totalSamples > SFXEngineProperties.MaxCachedSoundSize) throw new ArgumentOutOfRangeException("source", "Attempt to cache a sample longer than the maximum permitted.");
            }
            return wholeFile.ToArray();
        }

        public override SoundFX cache() {
            return this;    // already cached
        }

        public override SoundFX dup() {
            CachedSoundFX _result = new CachedSoundFX(audioData, WaveFormat, length);
            _result.position = this.position;
            return _result;
        }

        public override Boolean seekTo(TimeSpan index) {
            long sampleIndex = (long)Math.Round(index.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
            return seekTo(sampleIndex);
        }

        public override Boolean seekTo(Int64 sampleIndex) {
            lock (_play_lock) {
                if ((sampleIndex >= audioData.Length) || (sampleIndex < 0)) return false;
                position = (int)sampleIndex;
                onSeek.triggerEvent(this);
                return true;
            }
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
                var availableSamples = this.audioData.Length - position;
                if (availableSamples == 0) stop();
                var samplesToCopy = Math.Min(availableSamples, count);
                Array.Copy(this.audioData, position, buffer, offset, samplesToCopy);
                position += samplesToCopy;                
                onSample.triggerEvent(this);
                return (int)samplesToCopy;
            }
        }
    }
}
