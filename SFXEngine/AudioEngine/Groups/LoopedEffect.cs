using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using SFXEngine.AudioEngine;

namespace SFXEngine.AudioEngine.Groups {
    public class LoopedEffect : SoundFX {

        private SoundFX source;

        public override WaveFormat WaveFormat {
            get {
                return source.WaveFormat;
            }
        }

        public override Int64 currentSample {
            get {
                return source.currentSample;
            }
        }

        public override TimeSpan currentTime {
            get {
                return source.currentTime;
            }
        }

        public override Boolean isCachable {
            get {
                return false;   // cannot cache an infinite source....
            }
        }

        public LoopedEffect(SoundFX src) {
            if (!src.canSeek) throw new UnsupportedAudioException("Unable to loop an audio source which cannot be restarted...");
            this.source = src;
            this.canSeek = source.canSeek;
            this.canDuplicate = source.canDuplicate;
        }

        public override SoundFX dup() {
            if (canDuplicate) {
                return new LoopedEffect(source.dup());
            }
            return null;
        }

        public override Boolean seekTo(Int64 sampleIndex) {
            if (!canSeek) return false;
            lock (_play_lock) {
                long maxSamples = (long)Math.Round(source.length.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
                if (maxSamples != 0) {
                    sampleIndex = sampleIndex % maxSamples;
                }
                bool _result = source.seekTo(sampleIndex);
                if (_result) onSeek.triggerEvent(this);
                return _result;
            }
        }

        public override Boolean seekTo(TimeSpan index) {
            long sampleIndex = (long)Math.Round(index.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
            return seekTo(sampleIndex);
        }

        public override Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            var samplesRead = source.Read(buffer, offset, count);
            // Loop to support the possibility of very short sources (ie requested 100 samples from a source of only 3...)
            while (samplesRead < count) {
                bool justReset = false;
                if (samplesRead == 0) {
                    source.reset();
                    justReset = true;
                }
                samplesRead += source.Read(buffer, offset + samplesRead, count - samplesRead);
                if (justReset && (samplesRead == 0)) break; // should not ever be unable to read immediately following a reset, (no audio in source?)
            }
            return samplesRead;
        }
    }
}
