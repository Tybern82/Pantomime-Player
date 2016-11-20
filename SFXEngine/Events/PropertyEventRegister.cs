using System.Collections.Generic;
using System.Threading;

namespace SFXEngine.Events {
    public delegate void PropertyEventCallback(string property, object nValue);

    class PropertyEventCallbackItem {
        public PropertyEventCallback callback { get; private set; }
        public string property { get; private set; }
        public object nValue { get; private set; }

        public PropertyEventCallbackItem(PropertyEventCallback call, string prop, object val) {
            this.callback = call;
            this.property = prop;
            this.nValue = val;
        }
    }

    public class PropertyEventRegister {

        private List<PropertyEventCallback> triggers = new List<PropertyEventCallback>();

        public bool addEventTrigger(PropertyEventCallback callback) {
            if (!triggers.Contains(callback)) {
                triggers.Add(callback);
                return true;
            } else return false;
        }

        public bool removeEventTrigger(PropertyEventCallback callback) {
            if (triggers.Contains(callback)) {
                triggers.Remove(callback);
                return true;
            } else return false;
        }

        public void triggerEvent(string property, object nValue) {
            foreach (PropertyEventCallback callback in triggers) {
                PropertyEventCallbackItem item = new PropertyEventCallbackItem(callback, property, nValue);
                ThreadPool.QueueUserWorkItem(propertyCallback, item);
            }
        }

        public static void propertyCallback(object evt) {
            if (evt is PropertyEventCallbackItem) {
                PropertyEventCallbackItem item = (PropertyEventCallbackItem)evt;
                item.callback(item.property, item.nValue);
            }
        }
    }
}