using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SFXEngine.AudioEngine.Adapters {
    public class MonoToMultiChannelSampleProvider : ISampleProvider {

        private readonly ISampleProvider source;
        private readonly int Channels;

        private WaveFormat _WaveFormat;
        public WaveFormat WaveFormat {
            get {
                if (_WaveFormat == null) {
                    WaveFormat _result = source.WaveFormat;
                    _WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_result.SampleRate, Channels);
                        }
                return _WaveFormat;
            }
        }

        public MonoToMultiChannelSampleProvider(ISampleProvider source, int channels) {
            this.source = source;
            this.Channels = channels;
        }

        private float currentValue;
        private int channelsToSend = 0;

        public int Read(float[] buffer, int offset, int count) {
            int sentCount = 0;
            if (channelsToSend > 0) {
                for (int x = 0; (x < count) && (channelsToSend > 0); x++, channelsToSend--, sentCount++) {
                    buffer[offset + x] = currentValue;
                }
            }
            if (sentCount < count) {
                float[] iBuffer = new float[(count - sentCount) / Channels];
                if (iBuffer.Length != 0) {
                    int read = source.Read(iBuffer, 0, WaveFormat.Channels);
                    if (read == 0) return sentCount;
                    int totalRead = read;
                    while (totalRead < iBuffer.Length) {
                        read = source.Read(iBuffer, totalRead, iBuffer.Length - totalRead);
                        if (read == 0) break;
                        else totalRead += read;
                    }
                    for (int x = 0; x < totalRead; x++) {
                        for (int i = 0; i < Channels; i++) {
                            buffer[offset + sentCount] = iBuffer[x];
                            sentCount++;
                        }
                    }
                }
                // now we've sent a whole number of channels, send through the remaining partial channels
                if (sentCount < count) {
                    iBuffer = new float[1];
                    int read = source.Read(iBuffer, 0, 1);
                    if (read == 0) return sentCount;
                    currentValue = iBuffer[0];
                    channelsToSend = Channels;
                    for (int x = 0; (sentCount < count) && (channelsToSend > 0); x++, channelsToSend--, sentCount++) {
                        buffer[offset + sentCount] = currentValue;
                    }
                }
            }
            return sentCount;
        }
    }
}
