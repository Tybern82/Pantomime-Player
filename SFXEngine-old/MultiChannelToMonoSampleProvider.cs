using System;
using NAudio.Wave;

namespace SFXEngine {

    public class MultiChannelToMonoSampleProvider : ISampleProvider {
        public WaveFormat WaveFormat { get; private set; }

        private ISampleProvider source;

        public MultiChannelToMonoSampleProvider(ISampleProvider source) {
            WaveFormat = new WaveFormat(source.WaveFormat.SampleRate, source.WaveFormat.BitsPerSample, 1);
            this.source = source;
        }

        public Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            Single[] iBuffer = new Single[source.WaveFormat.Channels];
            for (int x = 0; x < count; x++) {
                int read = source.Read(iBuffer, 0, source.WaveFormat.Channels);
                if (read == 0) return x;
                double mixed = 0.0;
                for (int y = 0; y < read; y++) {
                    mixed += iBuffer[y];
                }
                buffer[offset + x] = (Single)(mixed / source.WaveFormat.Channels);
            }
            return count;
        }
    }
}
