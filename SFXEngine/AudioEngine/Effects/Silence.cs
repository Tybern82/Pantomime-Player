using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Effects {
    public class Silence : SoundFX {

        private long totalSamples;
        private WaveFormat _WaveFormat;
        public override WaveFormat WaveFormat {
            get {
                return _WaveFormat;
            }
        }

        private long totalPosition = 0;
        public override Int64 currentSample {
            get {
                return totalPosition;
            }
        }

        public override TimeSpan currentTime {
            get {
                long ticks = (long)Math.Round((double)(currentSample * TimeSpan.TicksPerSecond / (WaveFormat.SampleRate * WaveFormat.Channels)), MidpointRounding.ToEven);
                return new TimeSpan(ticks);
            }
        }

        public Silence(TimeSpan ts) : this(ts, AudioPlaybackEngine.Instance.WaveFormat) {}

        public Silence(TimeSpan ts, WaveFormat fmt) : this(((long)Math.Round(ts.TotalSeconds * fmt.SampleRate * fmt.Channels, MidpointRounding.ToEven)), fmt) {
            this.length = ts;
        }

        public Silence(long samples, WaveFormat fmt) {
            this._WaveFormat = fmt;
            long ticks = (long)Math.Round((double)(samples * TimeSpan.TicksPerSecond / (WaveFormat.SampleRate * WaveFormat.Channels)), MidpointRounding.ToEven);
            this.length = new TimeSpan(ticks);
            this.totalSamples = samples;
        }

        public override SoundFX dup() {
            lock (_play_lock) {
                Silence _result = new Silence(length, WaveFormat);
                _result.totalPosition = this.totalPosition;
                return _result;
            }
        }

        public override Boolean seekForward(Int64 sampleLength) {
            lock (_play_lock) {
                this.totalPosition += sampleLength;
                if (totalPosition >= totalSamples) stop();
                return true;
            }
        }

        public override Boolean seekTo(Int64 sampleIndex) {
            lock (_play_lock) {
                this.totalPosition = sampleIndex;
                if (totalPosition >= totalSamples) stop();
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
                var samplesAvailable = totalSamples - totalPosition;
                var samplesRead = (int)Math.Min(samplesAvailable, count);
                readSilence(buffer, offset, samplesRead);   // 'read' 0 data
                if (samplesRead == 0) stop();
                else onSample.triggerEvent(this);
                totalPosition += samplesRead;
                return samplesRead;
            }
        }
    }
}
