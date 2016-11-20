using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFXEngine.AudioEngine {
    public class SFXEngineProperties {
        // Default maximum cache limit to 5 minute tracks
        public static uint MaxCachedSoundSize { get; set; } = 5 * 60 * AudioPlaybackEngine.DefaultSampleRate * AudioPlaybackEngine.DefaultChannelCount;
        // SoundFile cutoff cach size (will select BufferedSound once file passes either this or the MaxCachedSoundSize limit)
        public static TimeSpan MaxCachedSoundFile { get; set; } = new TimeSpan(0, 3, 0);    // 3-minutes
        // Default buffer size to 30 seconds (60 seconds of buffer in both main and secondary buffers)
        public static uint AudioBufferSize { get; set; } = 30 * AudioPlaybackEngine.DefaultSampleRate * AudioPlaybackEngine.DefaultChannelCount;
        // Maximum volume increase amount 
        public static double MaxVolume { get; set; } = 3;   // limit to no more than triple original volume
        
        public static SFXProperties ShowProperties { private get; set; }

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
