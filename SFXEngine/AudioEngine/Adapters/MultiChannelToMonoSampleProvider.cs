using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SFXEngine.AudioEngine.Adapters {
    public class MultiChannelToMonoSampleProvider : ISampleProvider {

        private readonly ISampleProvider source;

        public WaveFormat WaveFormat {
            get {
                return source.WaveFormat;
            }
        }

        public MultiChannelToMonoSampleProvider(ISampleProvider source) {
            if (source == null) throw new UnsupportedAudioException("Attempt to convert empty audio source.");
            this.source = source;
        }

        public int Read(float[] buffer, int offset, int count) {
            float[] iBuffer = new float[WaveFormat.Channels];
            for (int x = 0; x < count; x++) {
                int read = source.Read(iBuffer, 0, WaveFormat.Channels);
                if (read == 0) return x;
                int totalRead = read;
                while (totalRead < WaveFormat.Channels) {
                    // Make sure we fully read a complete set of channel signals (unless the stream ends)
                    read = source.Read(iBuffer, totalRead, WaveFormat.Channels - totalRead);
                    if (read == 0) totalRead = WaveFormat.Channels;
                    else totalRead += read;
                }
                double mixed = 0.0;
                for (int y = 0; y < totalRead; y++) {
                    mixed += iBuffer[y];
                }
                buffer[offset + x] = (float)(mixed / WaveFormat.Channels);
            }
            return count;
        }
    }
}