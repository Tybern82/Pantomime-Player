using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFXEngine {
    public enum EffectState {
        LOADING,
        READY,
        RUNNING,
        PAUSED,
        STOPPED
    }

    public interface Effect {
        EffectState state { get; }
        TimeSpan length { get; }

        Effect cache();

        bool makeReady();
        Effect dup();

        bool play(PlaybackDevice dev);

        EventRegister onStart { get; }
        EventRegister onStop { get; }
    }
}
