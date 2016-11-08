using System;
using NAudio.Wave;

namespace SFXEngine {
    public class FadeInOutSampleProvider : ISampleProvider {
        enum FadeState {
            Silence,
            FadingIn,
            FullVolume,
            FadingOut,
        }

        private readonly object lockObject = new object();
        private readonly ISampleProvider source;
        private readonly long autoFadeOutSample;
        private readonly double autoFadeDuration;
        private long totalSamples;
        private int fadeSamplePosition;
        private int fadeSampleCount;
        private FadeState fadeState;

        public FadeInOutSampleProvider(ISampleProvider source) {
            this.source = source;
            this.fadeState = FadeState.FullVolume;
        }

        public FadeInOutSampleProvider(ISampleProvider source, TimeSpan autoFadeOut, TimeSpan fadeDuration) : this(source) {
            int samplesPerSecond = source.WaveFormat.SampleRate * source.WaveFormat.Channels;
            this.autoFadeOutSample = (long)Math.Round(autoFadeOut.TotalSeconds * samplesPerSecond, MidpointRounding.AwayFromZero);
            this.autoFadeDuration = fadeDuration.TotalMilliseconds;
        }

        public void BeginFadeIn(double fadeDurationInMilliseconds) {
            lock (lockObject) {
                fadeSamplePosition = 0;
                fadeSampleCount = (int)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
                fadeState = FadeState.FadingIn;
            }
        }

        public void BeginFadeOut(double fadeDurationInMilliseconds) {
            lock (lockObject) {
                fadeSamplePosition = 0;
                fadeSampleCount = (int)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
                fadeState = FadeState.FadingOut;
            }
        }

        public int Read(float[] buffer, int offset, int count) {
            int sourceSamplesRead = source.Read(buffer, offset, count);
            if ((fadeState == FadeState.FullVolume) && (totalSamples + sourceSamplesRead > autoFadeOutSample)) {
                // need to begin the fadeout
                int unfadedSamples = (int)(autoFadeOutSample - totalSamples);
                if (unfadedSamples > 0) {
                    // need to only fade a portion of the data
                    BeginFadeOut(autoFadeDuration);
                    FadeOut(buffer, offset + unfadedSamples, sourceSamplesRead - unfadedSamples);
                    totalSamples += sourceSamplesRead;
                    return sourceSamplesRead;
                } else {
                    // no lead in - just begin the fade
                    BeginFadeOut(autoFadeDuration);
                }
            }
            totalSamples += sourceSamplesRead;
            lock (lockObject) {
                if (fadeState == FadeState.FadingIn) {
                    FadeIn(buffer, offset, sourceSamplesRead);
                } else if (fadeState == FadeState.FadingOut) {
                    FadeOut(buffer, offset, sourceSamplesRead);
                } else if (fadeState == FadeState.Silence) {
                    ClearBuffer(buffer, offset, count);
                    return 0;
                }
            }
            return sourceSamplesRead;
        }

        private static void ClearBuffer(float[] buffer, int offset, int count) {
            for (int n = 0; n < count; n++) {
                buffer[n + offset] = 0;
            }
        }

        private void FadeOut(float[] buffer, int offset, int sourceSamplesRead) {
            int sample = 0;
            while (sample < sourceSamplesRead) {
                float multiplier = 1.0f - (fadeSamplePosition / (float)fadeSampleCount);
                for (int ch = 0; ch < source.WaveFormat.Channels; ch++) {
                    buffer[offset + sample++] *= multiplier;
                }
                fadeSamplePosition++;
                if (fadeSamplePosition > fadeSampleCount) {
                    fadeState = FadeState.Silence;
                    // clear out the end
                    ClearBuffer(buffer, sample + offset, sourceSamplesRead - sample);
                    break;
                }
            }
        }

        private void FadeIn(float[] buffer, int offset, int sourceSamplesRead) {
            int sample = 0;
            while (sample < sourceSamplesRead) {
                float multiplier = (fadeSamplePosition / (float)fadeSampleCount);
                for (int ch = 0; ch < source.WaveFormat.Channels; ch++) {
                    buffer[offset + sample++] *= multiplier;
                }
                fadeSamplePosition++;
                if (fadeSamplePosition > fadeSampleCount) {
                    fadeState = FadeState.FullVolume;
                    // no need to multiply any more
                    break;
                }
            }
        }

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }
    }
}
