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
        // Default time to wait for the last of the audio data to be processed before auto-removing the mixer input
        public static TimeSpan AutoStopWaitTime { get; set; } = new TimeSpan(0, 0, 5);  // 5-seconds
        // Maximum volume increase amount 
        public static double MaxVolume { get; set; } = 3;   // limit to no more than triple original volume
    }
}
