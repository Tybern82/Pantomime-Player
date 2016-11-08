using System;
using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioPlaybackEngine {
    public class SequenceEffect : SoundEffect {

        private List<SoundEffect> SoundEffects { get; set; }

        private TimeSpan audioDelay;
        public TimeSpan AudioDelay
        {
            get
            {
                return audioDelay;
            }
            set
            {
                cachedAudioData = null;
                audioDelay = value;
            }
        }

        public SequenceEffect(TimeSpan delay) {
            this.SoundEffects = new List<SoundEffect>();
            this.AudioDelay = delay;
        }

        public SequenceEffect() : this(new TimeSpan(0)) { }

        public SoundEffect appendEffect(CachedSound snd) {
            cachedAudioData = null;
            return appendEffect(new BasicSoundEffect(snd));
        }

        public SoundEffect appendEffect(SoundEffect effect) {
            cachedAudioData = null;
            SoundEffects.Add(effect);
            return effect;
        }

        public void removeEffect(SoundEffect effect) {
            cachedAudioData = null;
            SoundEffects.Remove(effect);
        }

        private CachedSound cachedAudioData = null;
        public CachedSound AudioData
        {
            get
            {
                if (cachedAudioData != null) return cachedAudioData;
                var format = (SoundEffects.Count != 0) 
                    ? SoundEffects[0].AudioData.WaveFormat 
                    : WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.DefaultSampleRate, AudioPlaybackEngine.DefaultChannelCount);
                CachedSound result = CachedSound.createSilence(format, AudioDelay);
                for (int x = 0; x < SoundEffects.Count; x++) {
                    result = result.appendSound(SoundEffects[x].AudioData);
                }
                cachedAudioData = result;
                return result;
            }
        }

        public void playback(AudioPlaybackEngine engine) {
            engine.PlaySound(AudioData);
        }
    }

    public class SimultaneousEffect : SoundEffect {

        private TimeSpan audioDelay;
        public TimeSpan AudioDelay
        {
            get
            {
                return audioDelay;
            }
            set
            {
                cachedAudioData = null;
                audioDelay = value;
            }
        }

        private List<SoundEffect> SoundEffects { get; set; }

        public SimultaneousEffect(TimeSpan delay) {
            this.SoundEffects = new List<SoundEffect>();
            this.AudioDelay = delay;
        }

        public SimultaneousEffect() : this(new TimeSpan(0)) { }

        public SoundEffect addEffect(CachedSound snd) {
            cachedAudioData = null;
            return addEffect(new BasicSoundEffect(snd));
        }

        public SoundEffect addEffect(SoundEffect effect) {
            cachedAudioData = null;
            SoundEffects.Add(effect);
            return effect;
        }

        public void removeEffect(SoundEffect effect) {
            cachedAudioData = null;
            SoundEffects.Remove(effect);
        }

        public void playback(AudioPlaybackEngine engine) {
            var format = (SoundEffects.Count != 0)
                    ? SoundEffects[0].AudioData.WaveFormat
                    : WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.DefaultSampleRate, AudioPlaybackEngine.DefaultChannelCount);
            CachedSound delay = CachedSound.createSilence(format, AudioDelay);
            for (int x = 0; x < SoundEffects.Count; x++) {
                engine.PlaySound(delay.appendSound(SoundEffects[x].AudioData));
            }
        }


        private CachedSound cachedAudioData = null;
        public CachedSound AudioData
        {
            get
            {
                if (cachedAudioData == null) {
                    var format = (SoundEffects.Count != 0)
                    ? SoundEffects[0].AudioData.WaveFormat
                    : WaveFormat.CreateIeeeFloatWaveFormat(AudioPlaybackEngine.DefaultSampleRate, AudioPlaybackEngine.DefaultChannelCount);
                    CachedSound delay = CachedSound.createSilence(format, AudioDelay);
                    var mixer = new MixingSampleProvider(format);
                    for (int x = 0; x < SoundEffects.Count; x++) {
                        mixer.AddMixerInput(new CachedSoundSampleProvider(delay.appendSound(SoundEffects[x].AudioData)));
                    }
                    cachedAudioData = CachedSound.cache(mixer);
                }
                return cachedAudioData;
            }
        }
    }

    public class BasicSoundEffect : SoundEffect {
        public CachedSound Data { get; private set; }
        public TimeSpan AudioDelay { get; set; }

        public CachedSound AudioData
        {
            get
            {
                if (AudioDelay.TotalMilliseconds == 0) return Data;
                return CachedSound.createSilence(Data.WaveFormat, AudioDelay).appendSound(Data);
            }
        }

        public BasicSoundEffect(CachedSound source, TimeSpan delay) {
            this.Data = source;
            this.AudioDelay = delay;
        }

        public BasicSoundEffect(CachedSound source) : this(source, new TimeSpan(0)) { }

        public void playback(AudioPlaybackEngine engine) {
            engine.PlaySound(AudioData);
        }
    }

    public interface SoundEffect {
        CachedSound AudioData { get; }
        TimeSpan AudioDelay { get; set; }

        void playback(AudioPlaybackEngine engine);
    }
}
