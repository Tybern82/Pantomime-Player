using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Adapters {
    public class FadeSampleProvider : ISampleProvider {
        enum FadeState {
            Silence,
            FadeIn,
            FullVolume,
            FadeOut
        }

        private readonly object source_lock = new object();
        private readonly ISampleProvider source;

        private readonly long autoFadeOutSample;
        private readonly TimeSpan autoFadeDuration;

        private long totalSamples;

        private int fadeSamplePosition;
        private int fadeSampleCount;

        private FadeState fadeState;

        public WaveFormat WaveFormat {
            get {
                return source.WaveFormat;
            }
        }

        public FadeSampleProvider(ISampleProvider source) {
            this.source = source;
            this.fadeState = FadeState.FullVolume;
        }

        public FadeSampleProvider(ISampleProvider source, TimeSpan autoFadeTime, TimeSpan autoFadeDuration) : this (source) {
            int samplesPerSecond = WaveFormat.SampleRate * WaveFormat.Channels;
            this.autoFadeOutSample = (long)Math.Round(autoFadeTime.TotalSeconds * samplesPerSecond, MidpointRounding.AwayFromZero);
            this.autoFadeDuration = autoFadeDuration;
        }

        public FadeSampleProvider(ISampleProvider source, TimeSpan fadeInDuration, TimeSpan autoFadeTime, TimeSpan autoFadeDuration) 
            : this(source, autoFadeTime, autoFadeDuration) {
            if (fadeInDuration != TimeSpan.Zero) fadeIn(fadeInDuration.TotalMilliseconds);
        }

        public void fadeIn(double durationInMillis) {
            lock (source_lock) {
                fadeSamplePosition = 0;
                fadeSampleCount = (int)((durationInMillis * WaveFormat.SampleRate) / 1000);
                fadeState = FadeState.FadeIn;
            }
        }

        public void fadeOut() {
            fadeOut(autoFadeDuration.TotalMilliseconds);
        }

        public void fadeOut(double durationInMillis) {
            lock (source_lock) {
                fadeSamplePosition = 0;
                fadeSampleCount = (int)((durationInMillis * WaveFormat.SampleRate) / 1000);
                fadeState = FadeState.FadeOut;
            }
        }

        public int Read(float[] buffer, int offset, int count) {
            int sourceSamplesRead = source.Read(buffer, offset, count);
            if ((fadeState == FadeState.FullVolume) && (totalSamples + sourceSamplesRead > autoFadeOutSample)) {
                // need to begin the fadeout
                int unfadedSamples = (int)(autoFadeOutSample - totalSamples);
                fadeOut();
                if (unfadedSamples > 0) {
                    doFadeOut(buffer, offset + unfadedSamples, sourceSamplesRead - unfadedSamples);
                    totalSamples += sourceSamplesRead;
                    return sourceSamplesRead;
                }
            }
            totalSamples += sourceSamplesRead;
            lock (source_lock) {
                if (fadeState == FadeState.FadeIn) {
                    doFadeIn(buffer, offset, sourceSamplesRead);
                } else if (fadeState == FadeState.FadeOut) {
                    doFadeOut(buffer, offset, sourceSamplesRead);
                } else if (fadeState == FadeState.Silence) {
                    doClearBuffer(buffer, offset, count);
                    return 0;
                }
            }
            return sourceSamplesRead;
        }

        private static void doClearBuffer(float[] buffer, int offset, int count) {
            Array.Clear(buffer, offset, count);
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
                    doClearBuffer(buffer, sample + offset, sourceSamplesRead - sample);
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