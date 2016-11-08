using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace AudioPlaybackEngine {
    public class CachedSound {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public TimeSpan AudioLength { get; private set; }

        public CachedSound(string audioFileName) {
            using (var audioFileReader = new AudioFileReader(audioFileName)) {
                int outRate = AudioPlaybackEngine.Instance.AudioSampleRate;
                WaveFormat = audioFileReader.WaveFormat;
                AudioLength = audioFileReader.TotalTime;
                if (WaveFormat.SampleRate != outRate) {
                    // Need to resample the input file
                    var resampler = new WdlResamplingSampleProvider(audioFileReader, outRate);
                    WaveFormat = resampler.WaveFormat;
                    var wholeFile = new List<float>();
                    var readBuffer = new float[resampler.WaveFormat.SampleRate * resampler.WaveFormat.Channels];
                    int samplesRead;
                    while ((samplesRead = resampler.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                        wholeFile.AddRange(readBuffer.Take(samplesRead));
                    }
                    AudioData = wholeFile.ToArray();
                } else {
                    var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                    var readBuffer= new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                    int samplesRead;
                    while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                        wholeFile.AddRange(readBuffer.Take(samplesRead));
                    }
                    AudioData = wholeFile.ToArray();
                }
            }
        }

        public static CachedSound cache(ISampleProvider source) {
            var wholeFile = new List<float>();
            var readBuffer = new float[source.WaveFormat.SampleRate * source.WaveFormat.Channels];
            int samplesRead;
            long totalSamples = 0;
            while ((samplesRead = source.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
                totalSamples += samplesRead;
            } 
            long ticks = (long)Math.Round((double)(totalSamples * TimeSpan.TicksPerSecond / (source.WaveFormat.SampleRate * source.WaveFormat.Channels)), MidpointRounding.ToEven);
            CachedSound snd = new CachedSound(wholeFile.ToArray(), source.WaveFormat, new TimeSpan(ticks));
            return resample(snd, AudioPlaybackEngine.Instance.AudioSampleRate);
        }

        private CachedSound(float[] aData, WaveFormat fmt, TimeSpan length) {
            this.AudioData = aData;
            this.WaveFormat = fmt;
            this.AudioLength = length;
        }

        public static CachedSound createSilence(WaveFormat fmt, double seconds) {
            TimeSpan length = new TimeSpan((long)(seconds * 10000000));
            return createSilence(fmt, length);
        }

        public static CachedSound createSilence(WaveFormat fmt, TimeSpan length) {
            Double samples = ((fmt.SampleRate * fmt.Channels) * length.TotalSeconds);
            var silence = new float[(int)Math.Round(samples, MidpointRounding.AwayFromZero)];
            for (int x = 0; x < silence.Length; x++) silence[x] = 0.0f;
            return new CachedSound(silence, fmt, length);
        }

        public static CachedSound resample(CachedSound snd, int sampleRate) {
            if (snd.WaveFormat.SampleRate == sampleRate) return snd;    // don't resample if not required
            var resampler = new WdlResamplingSampleProvider(new CachedSoundSampleProvider(snd), sampleRate);
            var format = resampler.WaveFormat;
            var wholeFile = new List<float>();
            var readBuffer = new float[resampler.WaveFormat.SampleRate * resampler.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = resampler.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            return new CachedSound(wholeFile.ToArray(), format, snd.AudioLength);
        }

        public static CachedSound monoToStereo(CachedSound snd) {
            if (snd.WaveFormat.Channels == 2) return snd;
            if (snd.WaveFormat.Channels != 1) throw new NotImplementedException("Not yet implemented this channel count conversion");
            var sInput = new MonoToStereoSampleProvider(new CachedSoundSampleProvider(snd));
            var format = sInput.WaveFormat;
            var wholeFile = new List<float>();
            var readBuffer = new float[format.SampleRate * format.Channels];
            int samplesRead;
            while ((samplesRead = sInput.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            return new CachedSound(wholeFile.ToArray(), format, snd.AudioLength);
        }

        public static CachedSound multiChannelToStereo(CachedSound snd) {
            if (snd.WaveFormat.Channels == 2) return snd;
            if (snd.WaveFormat.Channels == 1) return monoToStereo(snd);
            var sInput = new MultiplexingWaveProvider(new IWaveProvider[] {new SampleToWaveProvider(new CachedSoundSampleProvider(snd)) }, 2);
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
            } else {
                throw new NotImplementedException("Not yet implemented this channel count conversion");
            }
            var dInput = new WaveToSampleProvider(sInput);
            var format = dInput.WaveFormat;
            var wholeFile = new List<float>();
            var readBuffer = new float[format.SampleRate * format.Channels];
            int samplesRead;
            while ((samplesRead = dInput.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            return new CachedSound(wholeFile.ToArray(), format, snd.AudioLength);
        }

        public static CachedSound multiChannelToMono(CachedSound snd) {
            if (snd.WaveFormat.Channels == 1) return snd;   // already mono - no change required
            var format = new WaveFormat(snd.WaveFormat.SampleRate, snd.WaveFormat.BitsPerSample, 1);
            var wholeFile = snd.AudioData;
            var nData = new List<float>();
            for (int x = 0; x < snd.AudioData.Length; x += snd.WaveFormat.Channels) {
                double mixed = 0.0;
                for (int y = 0; y < snd.WaveFormat.Channels; y++) {
                    mixed += wholeFile[x + y];
                }
                nData.Add((float)(mixed / snd.WaveFormat.Channels));
            }
            return new CachedSound(nData.ToArray(), format, snd.AudioLength);
        }

        public CachedSound appendSound(CachedSound snd) {
            // if the sound is empty/null, just return the current instance
            if (snd == null) return this;
            CachedSound curr = this;
            // Sound files can only be appended when they are of the same format
            if (WaveFormat.BitsPerSample != snd.WaveFormat.BitsPerSample) return null;
            if (curr.WaveFormat.SampleRate != snd.WaveFormat.SampleRate) {
                // always resample to the higher rate
                if (curr.WaveFormat.SampleRate > snd.WaveFormat.SampleRate) {
                    snd = resample(snd, curr.WaveFormat.SampleRate);
                } else {
                    curr = resample(curr, snd.WaveFormat.SampleRate);
                }
            }
            if (curr.WaveFormat.Channels != snd.WaveFormat.Channels) {
                curr = multiChannelToStereo(curr);
                snd = multiChannelToStereo(snd);
            }

            List<float> nAudio = new List<float>(curr.AudioData);
            nAudio.AddRange(snd.AudioData);
            return new CachedSound(nAudio.ToArray(), curr.WaveFormat, curr.AudioLength.Add(snd.AudioLength));
        }
    }
}