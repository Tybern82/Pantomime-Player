using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using SFXEngine.AudioEngine.Adapters;
using SFXEngine.AudioEngine.Effects;

namespace SFXEngine.AudioEngine {
    public class SFXUtilities {

        public static TimeSpan TimeInSeconds(int seconds) {
            return new TimeSpan(0, 0, seconds);
        }

        public static Silence GenerateSilence(TimeSpan ts) {
            return new Silence(ts, AudioPlaybackEngine.Instance.WaveFormat);
        }

        public static Silence GenerateSilence(int seconds) {
            return GenerateSilence(new TimeSpan(0, 0, seconds));
        }

        public static Silence GenerateSilence(int mins, int secs) {
            return GenerateSilence(new TimeSpan(0, mins, secs));
        }

        public static ISampleProvider ConvertSampleFormat(ISampleProvider source, WaveFormat dFormat) {
            // resample, if required
            if (source.WaveFormat.SampleRate != dFormat.SampleRate) source = new WdlResamplingSampleProvider(source, dFormat.SampleRate);
            // adjust channel count, if required
            if (source.WaveFormat.Channels != dFormat.Channels) source = SFXUtilities.AdjustChannelCount(source, (uint)dFormat.Channels);
            return source;
        }

        public static ISampleProvider AdjustChannelCount(ISampleProvider snd, UInt32 AudioChannelCount) {
            if (AudioChannelCount == 1) {
                if (snd.WaveFormat.Channels == 1) return snd;
                return new MultiChannelToMonoSampleProvider(snd);
            } else if (AudioChannelCount == 2) {
                if (snd.WaveFormat.Channels == 2) return snd;
                if (snd.WaveFormat.Channels == 1) return new MonoToStereoSampleProvider(snd);
                var sInput = new MultiplexingSampleProvider(new ISampleProvider[] { snd }, 2);
                if (snd.WaveFormat.Channels == 6) {
                    // 5.1 sound
                    sInput.ConnectInputToOutput(0, 0);  // FRONT-LEFT -> LEFT
                    sInput.ConnectInputToOutput(1, 1);  // FRONT-RIGHT -> RIGHT
                    sInput.ConnectInputToOutput(2, 0);  // CENTRE -> LEFT
                    sInput.ConnectInputToOutput(2, 1);  // CENTRE -> RIGHT
                                                        // sInput.ConnectInputToOutput(3, ?);   // LFE -> ???
                    sInput.ConnectInputToOutput(3, 0);  // LOW-FREQ -> LEFT
                    sInput.ConnectInputToOutput(3, 1);  // LOW-FREQ -> RIGHT
                    sInput.ConnectInputToOutput(4, 0);  // SURROUND-LEFT -> LEFT
                    sInput.ConnectInputToOutput(5, 1);  // SURROUND-RIGHT -> RIGHT
                } else if (snd.WaveFormat.Channels == 8) {
                    // 7.1 sound (assuming standard order, but alternative still goes to same stereo channel)
                    // L, R, C, LFE, RL, RR, SL, SR
                    sInput.ConnectInputToOutput(0, 0);  // LEFT
                    sInput.ConnectInputToOutput(1, 1);  // RIGHT
                    sInput.ConnectInputToOutput(2, 0);  // CENTRE
                    sInput.ConnectInputToOutput(2, 1);
                    sInput.ConnectInputToOutput(3, 0);  // LOW-FREQ
                    sInput.ConnectInputToOutput(3, 1);
                    sInput.ConnectInputToOutput(4, 0);  // REAR-LEFT
                    sInput.ConnectInputToOutput(5, 1);  // REAR-RIGHT
                    sInput.ConnectInputToOutput(6, 0);  // SIDE-LEFT
                    sInput.ConnectInputToOutput(7, 1);  // SIDE-RIGHT
                } else throw new UnsupportedAudioException("Unable to convert audio file channels for output.");
                return sInput;
            } else if (AudioChannelCount == 6) {
                if (snd.WaveFormat.Channels == 6) return snd;
                if (snd.WaveFormat.Channels == 1) return new MonoToMultiChannelSampleProvider(snd, 6);
                var sInput = new MultiplexingSampleProvider(new ISampleProvider[] { snd }, 6);
                if (snd.WaveFormat.Channels == 8) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(2, 2);
                    sInput.ConnectInputToOutput(3, 3);
                    sInput.ConnectInputToOutput(4, 4);
                    sInput.ConnectInputToOutput(5, 5);
                    sInput.ConnectInputToOutput(6, 4);
                    sInput.ConnectInputToOutput(7, 5);
                } else if (snd.WaveFormat.Channels == 2) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(0, 2);
                    sInput.ConnectInputToOutput(1, 2);
                    sInput.ConnectInputToOutput(0, 3);
                    sInput.ConnectInputToOutput(1, 3);
                    sInput.ConnectInputToOutput(0, 4);
                    sInput.ConnectInputToOutput(1, 5);
                } else throw new UnsupportedAudioException("Unable to convert audio file channels for output.");
                return sInput;
            } else if (AudioChannelCount == 8) {
                if (snd.WaveFormat.Channels == 8) return snd;
                if (snd.WaveFormat.Channels == 1) return new MonoToMultiChannelSampleProvider(snd, 8);
                var sInput = new MultiplexingSampleProvider(new ISampleProvider[] { snd }, 8);
                if (snd.WaveFormat.Channels == 6) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(2, 2);
                    sInput.ConnectInputToOutput(3, 3);
                    sInput.ConnectInputToOutput(4, 4);
                    sInput.ConnectInputToOutput(5, 5);
                    sInput.ConnectInputToOutput(4, 6);
                    sInput.ConnectInputToOutput(5, 7);
                } else if (snd.WaveFormat.Channels == 2) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(0, 2);
                    sInput.ConnectInputToOutput(1, 2);
                    sInput.ConnectInputToOutput(0, 3);
                    sInput.ConnectInputToOutput(1, 3);
                    sInput.ConnectInputToOutput(0, 4);
                    sInput.ConnectInputToOutput(1, 5);
                    sInput.ConnectInputToOutput(0, 6);
                    sInput.ConnectInputToOutput(0, 7);
                } else throw new UnsupportedAudioException("Unable to convert audio file channels for output.");
                return sInput;
            } else throw new UnsupportedAudioException("Unable to convert audio file channels for output.");
        }
    }
}
