using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NAudio.Wave;

namespace SFXEngine {
    class AutoRemoveMixerInput {
        public static void RemoveMixerInput(SoundFX fx, AudioPlaybackEngine dev) {
            fx.onStop.addEventTrigger((new AutoRemoveMixerInput(dev)).remove);
        }

        private AudioPlaybackEngine dev;

        AutoRemoveMixerInput(AudioPlaybackEngine dev) {
            this.dev = dev;
        }

        public void remove(Effect e) {
            Thread.Sleep(SFXEngineProperties.AutoStopWaitTime);    // wait make sure the last of the audio data has been processed
            if (e is SoundFX) {
                dev.Stop(((SoundFX)e).toSampleProvider());
            }
        }
    }
}
