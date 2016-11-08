using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine {
    public class SoundFile : SoundFX { 

        public TimeSpan length { get; private set; }
        public EffectState state { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        private string filename;
        private AudioFileReader reader;

        public SoundFile(string filename) {
            if (!File.Exists(filename)) throw new ArgumentException("Attempt to access a sound file which does not exist.", "filename");
            this.filename = filename;
            this.state = EffectState.LOADING;
            this.reader = new AudioFileReader(filename);
            this.length = reader.TotalTime;
            this.WaveFormat = reader.WaveFormat;
        }

        public SoundFX cache() {
            try {
                return new CachedSound(this);
            } catch (ArgumentOutOfRangeException) {
                // TODO: replace with creation of BufferedSound
                return this;
            }
        }

        public Effect dup() {
            return new SoundFile(filename);
        }

        public Boolean makeReady() {
            if ((state == EffectState.RUNNING) || (state == EffectState.PAUSED)) return false;
            ResetPlayback();
            state = EffectState.READY;
            return true;
        }

        public Boolean play(PlaybackDevice dev) {
            if (state != EffectState.READY) return false;
            if (dev is AudioPlaybackEngine) {
                var audio = (AudioPlaybackEngine)dev;
                if (cachedProvider.WaveFormat.SampleRate != audio.AudioSampleRate)
                    cachedProvider = new WdlResamplingSampleProvider(cachedProvider, audio.AudioSampleRate);
                cachedProvider = AudioPlaybackEngine.AdjustChannelCount(cachedProvider, audio.AudioChannelCount);
            }
            var result = dev.Play(this);
            if (result) {
                state = EffectState.RUNNING;
                return true;
            } else {
                state = EffectState.STOPPED;
                return false;
            }
        }

        public Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            if ((reader == null) || (state != EffectState.RUNNING))
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0) {
                reader.Dispose();
                reader = null;
                state = EffectState.STOPPED;
            }
            return read;
        }

        public Boolean ResetPlayback() {
            if (this.state == EffectState.RUNNING) return false;
            this.state = EffectState.LOADING;
            if (reader != null) reader.Dispose();
            this.reader = new AudioFileReader(filename);
            return true;
        }

        public Boolean StopPlayback() {
            if (state != EffectState.RUNNING) return false;
            AudioPlaybackEngine.Instance.Stop(toSampleProvider());
            state = EffectState.STOPPED;
            return true;
        }

        private ISampleProvider cachedProvider = null;
        public ISampleProvider toSampleProvider() {
            if (cachedProvider == null) cachedProvider = this;
            return cachedProvider;
        }

        Effect Effect.cache() {
            return cache();
        }
    }
}
