using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Adapters {
    public class VolumeControlProvider : ISampleProvider {

        private double _volumeMultiplier;
        public double volumeMultiplier {
            get {
                return _volumeMultiplier;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("volumeMultiplier", value, "Volume cannot be negative.");
                if (value > SFXEngineProperties.MaxVolume)
                    throw new ArgumentOutOfRangeException("volumeMultiplier", value, "Volume cannot be increased to this level.");
                this._volumeMultiplier = value;
            }
        }

        public WaveFormat WaveFormat {
            get {
                return source.WaveFormat;
            }
        }

        private ISampleProvider source;

        public VolumeControlProvider(ISampleProvider source) {
            this.source = source;
            this.volumeMultiplier = 1;
        }

        public Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            int samplesRead = source.Read(buffer, offset, count);
            for (int x = 0; x < samplesRead; x++) {
                buffer[offset + x] = (float)(buffer[offset + x] * volumeMultiplier);
            }
            return samplesRead;
        }
    }
}
