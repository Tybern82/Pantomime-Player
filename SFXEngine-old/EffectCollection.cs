using System;

namespace SFXEngine {
    public interface EffectCollection : Effect {
        bool addEffect(Effect e);
        bool removeEffect(Effect e);
    }
}
