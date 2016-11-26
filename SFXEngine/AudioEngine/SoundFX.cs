using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

using SFXEngine.Events;
using SFXEngine.AudioEngine.Effects;

namespace SFXEngine.AudioEngine {
    public abstract class SoundFX : ISampleProvider {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SoundFX));

        public bool isPlaying { get; protected set; }
        public bool isPaused { get; protected set; }
        public bool isStopped { get; protected set; }

        public bool canSeek { get; protected set; } = false;    // seekForward is always possible (just read sufficient samples), this confirms whether indexed seeking is available
        public bool canDuplicate { get; protected set; }        // not all effects may be able to be correctly duplicated

        public SoundEventRegister onPlay { get; } = new SoundEventRegister();
        public SoundEventRegister onStop { get; } = new SoundEventRegister();
        public SoundEventRegister onSample { get; } = new SoundEventRegister();

        public SoundEventRegister onPause { get; } = new SoundEventRegister();
        public SoundEventRegister onResume { get; } = new SoundEventRegister();

        public SoundEventRegister onReset { get; } = new SoundEventRegister();
        public SoundEventRegister onSeek { get; } = new SoundEventRegister();

        public TimeSpan length { get; protected set; } = TimeSpan.Zero; // a '0' length is taken to be unknown, rather than empty

        public virtual WaveFormat WaveFormat { get; }
        public virtual TimeSpan currentTime { get; }
        public virtual long currentSample {
            get {
                return (long)Math.Round(currentTime.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
            }
        }

        public virtual bool isCachable {
            get {
                bool _isCachable = (length.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels) <= SFXEngineProperties.getMaxCachedSoundSize();
                _isCachable &= (length - currentTime) <= SFXEngineProperties.getMaxCachedSoundFile();
                return _isCachable;
            }
        }

        protected object _play_lock = new object();

        public virtual SoundFX cache() {
            if (isCachable) {
                var currPosition = currentTime;
                try {
                    return new CachedSoundFX(this);
                } catch (ArgumentOutOfRangeException) {
                    seekTo(currPosition);
                }
            }
            return new BufferedSoundFX(this);
        }

        public virtual SoundFX dup() {
            return null;
        }

        public virtual bool seekForward(TimeSpan ts) {
            long samplesLength = (long)Math.Round((ts.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels), MidpointRounding.ToEven);
            return seekForward(samplesLength);
        }

        public virtual bool seekForward(long sampleLength) {
            float[] buffer = new float[Math.Min(sampleLength, WaveFormat.SampleRate * WaveFormat.Channels)];
            long samplesRead = 0;
            int read;
            lock (_play_lock) {
                while (samplesRead < sampleLength) {
                    read = Read(buffer, 0, (int)Math.Min(buffer.Length, sampleLength - samplesRead));
                    if (read == 0) return true; // seek as hit the end of input, complete immediately
                    samplesRead += read;
                }
            }
            onSeek.triggerEvent(this);
            return true;
        }

        public virtual bool seekTo(TimeSpan index) {
            long sampleIndex = (long)Math.Round((index.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels), MidpointRounding.ToEven);
            return seekTo(sampleIndex);
        }

        public virtual bool seekTo(long sampleIndex) {
            return false;
        }

        public virtual bool reset() {
            if (canSeek) {
                seekTo(TimeSpan.Zero);
                isStopped = false;
                onReset.triggerEvent(this);
            }
            return canSeek;
        }

        public virtual bool pause() {
            lock (_play_lock) {
                if (isPaused) return false;
                isPaused = true;
                isPlaying = false;
                onPause.triggerEvent(this);
                return true;
            }
        }

        public virtual bool resume() {
            lock (_play_lock) {
                if (isPaused) {
                    isPaused = false;
                    isPlaying = true;
                    onResume.triggerEvent(this);
                    return true;
                } else {
                    return false;
                }
            }
        }

        public virtual bool stop() {
            lock (_play_lock) {
                if (isPlaying) {
                    logger.Debug("Stopping SoundFX");
                    isPlaying = false;
                    isStopped = true;
                    onStop.triggerEvent(this);
                    return true;
                }
                return false;
            }
        }

        public abstract int Read(float[] buffer, int offset, int count);

        public int readSilence(float[] buffer, int offset, int count) {
            Array.Clear(buffer, offset, count);
            return count;
        }
    }
}
