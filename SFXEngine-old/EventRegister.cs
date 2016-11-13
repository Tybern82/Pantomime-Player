using System;
using System.Collections.Generic;
using System.Threading;

namespace SFXEngine {

    public delegate void EventCallback(Effect eventSource);

    class EventCallbackItem {
        public EventCallback callback { get; private set; }
        public Effect triggerSource { get; private set; }

        public EventCallbackItem(EventCallback call, Effect trigger) {
            this.callback = call;
            this.triggerSource = trigger;
        }
    }

    public class EventRegister {

        private List<EventCallback> triggers = new List<EventCallback>();

        public bool addEventTrigger(EventCallback callback) {
            if (!triggers.Contains(callback)) {
                triggers.Add(callback);
                return true;
            } else {
                return false;
            }
        }

        public bool removeEventTrigger(EventCallback callback) {
            if (triggers.Contains(callback)) {
                triggers.Remove(callback);
                return true;
            } else {
                return false;
            }
        }

        public void triggerEvent(Effect triggerSource) {
            foreach (EventCallback callback in triggers) {
                EventCallbackItem item = new EventCallbackItem(callback, triggerSource);
                ThreadPool.QueueUserWorkItem(eventCallback, item);
            }
        }

        public static void eventCallback(object evt) {
            if (evt is EventCallbackItem) {
                EventCallbackItem item = (EventCallbackItem)evt;
                item.callback(item.triggerSource);
            }
        }
    }
}
