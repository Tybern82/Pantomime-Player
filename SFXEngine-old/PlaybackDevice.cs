using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFXEngine {
    public interface PlaybackDevice {
        void Stop();
        bool Play(Effect cue);
    }
}
