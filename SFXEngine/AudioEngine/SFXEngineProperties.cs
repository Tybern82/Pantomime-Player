using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SFXEngine.AudioEngine {
    public class SFXEngineProperties {
        // Default maximum cache limit to 3 minute tracks
        public static uint MaxCachedSoundSize { get; set; } = 3 * 60 * AudioPlaybackEngine.DefaultSampleRate * AudioPlaybackEngine.DefaultChannelCount;
        // SoundFile cutoff cach size (will select BufferedSound once file passes either this or the MaxCachedSoundSize limit)
        public static TimeSpan MaxCachedSoundFile { get; set; } = new TimeSpan(0, 1, 0);    // 3-minutes
        // Default buffer size to 10 seconds (20 seconds of buffer in both main and secondary buffers)
        public static uint AudioBufferSize { get; set; } = 10 * AudioPlaybackEngine.DefaultSampleRate * AudioPlaybackEngine.DefaultChannelCount;
        // Maximum volume increase amount 
        public static double MaxVolume { get; set; } = 3;   // limit to no more than triple original volume
        
        public static SFXProperties ShowProperties { private get; set; }
        public static DirectoryInfo RelativeBase { get; set; }

        public static uint getMaxCachedSoundSize() {
            return (ShowProperties != null) ? ShowProperties.MaxCachedSoundSize : MaxCachedSoundSize;
        }

        public static TimeSpan getMaxCachedSoundFile() {
            return (ShowProperties != null) ? ShowProperties.MaxCachedSoundFile : MaxCachedSoundFile;
        }

        public static uint getAudioBufferSize() {
            return (ShowProperties != null) ? ShowProperties.AudioBufferSize : AudioBufferSize;
        }

        public static double getMaxVolume() {
            return (ShowProperties != null) ? ShowProperties.MaxVolume : MaxVolume;
        }
    }

    public interface SFXProperties {
        uint MaxCachedSoundSize { get; set; }
        TimeSpan MaxCachedSoundFile { get; set; }
        uint AudioBufferSize { get; set; }
        double MaxVolume { get; set; }
    }
}
