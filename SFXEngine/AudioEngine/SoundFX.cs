using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

using SFXEngine.Events;
using SFXEngine.AudioEngine.Effects;

namespace SFXEngine.AudioEngine {
    public abstract class SoundFX : SFXEventSource, ISampleProvider, FadeableSFX {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SoundFX));

        enum FadeState {
            Silence,
            FadeIn,
            FullVolume,
            FadeOut
        }

        public bool isPlaying { get; protected set; }
        public bool isPaused { get; protected set; }
        public bool isStopped { get; protected set; }

        public bool canSeek { get; protected set; } = false;    // seekForward is always possible (just read sufficient samples), this confirms whether indexed seeking is available
        public bool canDuplicate { get; protected set; }        // not all effects may be able to be correctly duplicated

        public TimeSpan length { get; protected set; } = TimeSpan.Zero; // a '0' length is taken to be unknown, rather than empty

        public TimeSpan playLengthRemaining {
            get {
                return ((hasAutoFade ? AutoFadeOutAt + FadeOutDuration : length) - currentTime);
            }
        }

        public virtual WaveFormat WaveFormat { get; }
        public virtual TimeSpan currentTime { get; }
        public virtual long currentSample {
            get {
                return totalSamples;
            }
            protected set {
                totalSamples = value;
            }
        }

        // Fade Control
        private double _Volume = 1.0;
        public double Volume {
            get { return _Volume; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Volume", value, "Volume cannot be negative.");
                if (value > SFXEngineProperties.getMaxVolume())
                    throw new ArgumentOutOfRangeException("Volume", value, "Volume cannot be increased to this level.");
                this._Volume = value;
            }
        }
        
        private long totalSamples;

        private int fadeSamplePosition;
        private int fadeSampleCount;

        private FadeState fadeState = FadeState.FullVolume;

        public bool hasAutoFade { get; set; } = false;

        private TimeSpan _FadeInDuration = TimeSpan.Zero;
        // []
        public TimeSpan FadeInDuration {
            get { return _FadeInDuration; }
            set {
                var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                _FadeInDuration = nValue;
            }
        }

        private TimeSpan _AutoFadeOutAt = TimeSpan.Zero;
        // []
        public TimeSpan AutoFadeOutAt {
            get { return _AutoFadeOutAt; }
            set {
                var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                if (nValue != TimeSpan.Zero) hasAutoFade = true;
                _AutoFadeOutAt = nValue;
            }
        }

        private long autoFadeOutSample {
            get {
                return (long)Math.Round(_AutoFadeOutAt.TotalSeconds * (WaveFormat.SampleRate * WaveFormat.Channels), MidpointRounding.AwayFromZero);
            }
        }

        private TimeSpan _FadeOutDuration = TimeSpan.Zero;
        // []
        public TimeSpan FadeOutDuration {
            get { return _FadeOutDuration; }
            set {
                var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                _FadeOutDuration = nValue;
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

        public abstract int ReadSamples(float[] buffer, int offset, int count);

        public int Read(float[] buffer, int offset, int count) {
            lock (_play_lock) {
                if (isPaused) {
                    return readSilence(buffer, offset, count);
                } else if (isStopped) return 0;
                if (!isPlaying) {
                    isPlaying = true;
                    onPlay.triggerEvent(this);
                }
                int _result = ReadSamples(buffer, offset, count);
                if (_result == 0) stop();
                else {
                    onSample.triggerEvent(this);
                    // TODO: Implement volume control
                    double volumeMultiplier = Volume;
                    if (volumeMultiplier != 1.0) {
                        // Only adjust volume when it will actually matter
                        for (int x = 0; x < _result; x++) {
                            buffer[offset + x] = (float)(buffer[offset + x] * volumeMultiplier);
                        }
                    }
                    if ((fadeState == FadeState.FullVolume) && (totalSamples + _result > autoFadeOutSample) && hasAutoFade) {
                        // need to begin the fadeout
                        int unfadedSamples = (int)(autoFadeOutSample - totalSamples);
                        beginFadeOut();
                        if (unfadedSamples > 0) {
                            doFadeOut(buffer, offset + unfadedSamples, _result - unfadedSamples);
                            totalSamples += _result;
                            return _result;
                        }
                    }
                    totalSamples += _result;
                    if (fadeState == FadeState.FadeIn) {
                        doFadeIn(buffer, offset, _result);
                    } else if (fadeState == FadeState.FadeOut) {
                        doFadeOut(buffer, offset, _result);
                    } else if (fadeState == FadeState.Silence) {
                        readSilence(buffer, offset, count);
                        stop();
                        return 0;
                    }
                }
                return _result;
            }
        }

        public void beginFadeIn() {
            logger.Info("Beginning fade-in...");
            lock (_play_lock) {
                double durationInMillis = FadeInDuration.TotalMilliseconds;
                fadeSamplePosition = 0;
                fadeSampleCount = (int)((durationInMillis * WaveFormat.SampleRate) / 1000);
                fadeState = FadeState.FadeIn;
            }
        }

        public void beginFadeOut() {
            logger.Info("Beginning fade-out....");
            lock (_play_lock) {
                double durationInMillis = FadeOutDuration.TotalMilliseconds;
                fadeSamplePosition = 0;
                fadeSampleCount = (int)((durationInMillis * WaveFormat.SampleRate) / 1000);
                fadeState = FadeState.FadeOut;
            }
        }

        public int readSilence(float[] buffer, int offset, int count) {
            Array.Clear(buffer, offset, count);
            return count;
        }

        private void doFadeOut(float[] buffer, int offset, int sourceSamplesRead) {
            int sample = 0;
            while (sample < sourceSamplesRead) {
                float multiplier = 1.0f - (fadeSamplePosition / (float)fadeSampleCount);
                for (int ch = 0; ch < WaveFormat.Channels; ch++) {
                    buffer[offset + sample++] *= multiplier;
                }
                fadeSamplePosition++;
                if (fadeSamplePosition > fadeSampleCount) {
                    fadeState = FadeState.Silence;
                    // clear out the end of the buffer
                    readSilence(buffer, sample + offset, sourceSamplesRead - sample);
                    break;
                }
            }
        }

        private void doFadeIn(float[] buffer, int offset, int sourceSamplesRead) {
            int sample = 0;
            while (sample < sourceSamplesRead) {
                float multiplier = (fadeSamplePosition / (float)fadeSampleCount);
                for (int ch = 0; ch < WaveFormat.Channels; ch++) {
                    buffer[offset + sample++] *= multiplier;
                }
                fadeSamplePosition++;
                if (fadeSamplePosition > fadeSampleCount) {
                    fadeState = FadeState.FullVolume;
                    // no need to adjust volume on any more samples
                    break;
                }
            }
        }
    }
}
