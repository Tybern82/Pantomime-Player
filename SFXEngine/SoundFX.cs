using System;
using NAudio.Wave;

namespace SFXEngine {
    public interface SoundFX : Effect, ISampleProvider {

        ISampleProvider toSampleProvider();

        bool StopPlayback();
        bool ResetPlayback();

        new SoundFX cache();
    }
}
