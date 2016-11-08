using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFXEngine {
    public class SFXEngineProperties {
        // Default maximum cache limit to 5 minute tracks
        public static uint MaxCachedSoundSize { get; set; } = 5 * 60 * AudioPlaybackEngine.DefaultSampleRate * AudioPlaybackEngine.DefaultChannelCount;
    }
}
