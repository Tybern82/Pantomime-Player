using System.Collections.Generic;
using System.Threading;

using SFXEngine.AudioEngine;

namespace SFXEngine.Events {

    public delegate void SoundEventCallback(SoundFX eventSource);

    class SoundEventCallbackItem {
        public SoundEventCallback callback { get; private set; }
        public SFXEngine.AudioEngine.SoundFX triggerSource { get; private set; }

        public SoundEventCallbackItem(SoundEventCallback call, SoundFX trigger) {
            this.callback = call;
            this.triggerSource = trigger;
        }
    }

    public class SoundEventRegister {

        private List<SoundEventCallback> triggers = new List<SoundEventCallback>();

        public bool addEventTrigger(SoundEventCallback callback) {
            if (!triggers.Contains(callback)) {
                triggers.Add(callback);
                return true;
            } else {
                return false;
            }
        }

        public bool removeEventTrigger(SoundEventCallback callback) {
            if (triggers.Contains(callback)) {
                triggers.Remove(callback);
                return true;
            } else {
                return false;
            }
        }

        public void triggerEvent(SoundFX triggerSource) {
            foreach (SoundEventCallback callback in triggers) {
                SoundEventCallbackItem item = new SoundEventCallbackItem(callback, triggerSource);
                ThreadPool.QueueUserWorkItem(eventCallback, item);
            }
        }

        public static void eventCallback(object evt) {
            if (evt is SoundEventCallbackItem) {
                SoundEventCallbackItem item = (SoundEventCallbackItem)evt;
                item.callback(item.triggerSource);
            }
        }
    }
}
