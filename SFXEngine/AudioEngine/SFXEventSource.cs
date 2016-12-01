using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFXEngine.Events;
using SQLite;

namespace SFXEngine.AudioEngine {
    public class SFXEventSource {
        [Ignore] public SoundEventRegister onPlay { get; } = new SoundEventRegister();
        [Ignore] public SoundEventRegister onStop { get; } = new SoundEventRegister();
        [Ignore] public SoundEventRegister onSample { get; } = new SoundEventRegister();

        [Ignore] public SoundEventRegister onPause { get; } = new SoundEventRegister();
        [Ignore] public SoundEventRegister onResume { get; } = new SoundEventRegister();

        [Ignore] public SoundEventRegister onReset { get; } = new SoundEventRegister();
        [Ignore] public SoundEventRegister onSeek { get; } = new SoundEventRegister();

        public void registerCascade(SFXEventSource reg) {
            onPlay.addEventTrigger(delegate (SoundFX source) {
                reg.onPlay.triggerEvent(source);
            });
            onStop.addEventTrigger(delegate (SoundFX source) {
                reg.onStop.triggerEvent(source);
            });
            onSample.addEventTrigger(delegate (SoundFX source) {
                reg.onSample.triggerEvent(source);
            });

            onPause.addEventTrigger(delegate (SoundFX source) {
                reg.onPause.triggerEvent(source);
            });
            onResume.addEventTrigger(delegate (SoundFX source) {
                reg.onResume.triggerEvent(source);
            });

            onReset.addEventTrigger(delegate (SoundFX source) {
                reg.onReset.triggerEvent(source);
            });
            onSeek.addEventTrigger(delegate (SoundFX source) {
                reg.onSeek.triggerEvent(source);
            });
        }
    }
}
