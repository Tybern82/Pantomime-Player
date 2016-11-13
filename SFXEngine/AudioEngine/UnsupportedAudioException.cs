using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFXEngine.AudioEngine {
    public class UnsupportedAudioException : Exception {

        public UnsupportedAudioException() : base() {}
        public UnsupportedAudioException(string message) : base(message) {}
        public UnsupportedAudioException(string message, Exception inner) : base(message, inner) {}
    }
}
