using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace AudioPlaybackEngine {
    class CachedSound {
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

        private CachedSound(float[] aData, WaveFormat fmt, TimeSpan length) {
            this.AudioData = aData;
            this.WaveFormat = fmt;
            this.AudioLength = length;
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
                curr = monoToStereo(curr);
                snd = monoToStereo(snd);
            }

            List<float> nAudio = new List<float>(curr.AudioData);
            nAudio.AddRange(snd.AudioData);
            return new CachedSound(nAudio.ToArray(), curr.WaveFormat, curr.AudioLength.Add(snd.AudioLength));
        }
    }
}