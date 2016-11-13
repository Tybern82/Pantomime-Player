using System;
using NAudio.Wave;

namespace SFXEngine {
    public interface SoundFX : Effect, ISampleProvider {
        
        ISampleProvider toSampleProvider();

        bool StopPlayback();
        bool ResetPlayback();
        bool playBuffer();

        new SoundFX cache();
        new SoundFX dup();

        EventRegister onSeek { get; }

        bool SeekForward(TimeSpan length);
        bool SeekForward(long sampleLength);
        
        // TODO: Seek(TimeSpan index);
        // TODO: Seek(long sampleIndex);
        // TODO: Trim(TimeSpan length);
        // TODO: Trim(long sampleLength);
        // TODO: Trim(TimeSpan start, TimeSpan length);
        // TODO: Trim(long startIndex, long sampleLength);
    }
}
