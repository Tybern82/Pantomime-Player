using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine {
    public class SoundFile : SoundFX, IDisposable { 

        public TimeSpan length { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public EventRegister onStart { get; } = new EventRegister();
        public EventRegister onStop { get; } = new EventRegister();
        public EventRegister onSeek { get; } = new EventRegister();

        private EffectState _state;
        public EffectState state {
            get {
                return _state;
            }
            set {
                if (value == EffectState.RUNNING) onStart.triggerEvent(this);
                if (value == EffectState.STOPPED) onStop.triggerEvent(this);
                _state = value;
            }
        }

        private string filename;
        private AudioFileReader reader;

        public SoundFile(string filename) {
            if (!File.Exists(filename)) throw new ArgumentException("Attempt to access a sound file which does not exist.", "filename");
            this.filename = filename;
            this.state = EffectState.LOADING;
            try {
                this.reader = new AudioFileReader(filename);
            } catch (System.Runtime.InteropServices.COMException) {
                // Unable to open the file to read
                throw new ArgumentException("Unable to open this file type for reading.");
            }
            this.length = reader.TotalTime;
            this.WaveFormat = reader.WaveFormat;
        }

        public bool SeekForward(TimeSpan length) {
            if ((reader != null) && (reader.CanSeek)) {
                reader.CurrentTime = reader.CurrentTime + length;
                return true;
            } else {
                return false;
            }
        }

        public bool SeekForward(long sampleLength) {
            long millis = (long)Math.Round((((double)(sampleLength * 1000)) / WaveFormat.Channels / WaveFormat.SampleRate), MidpointRounding.AwayFromZero);
            long secs = millis / 1000;
            long mins = secs / 60;
            long hours = mins / 60;
            int days = (int)(hours / 24);
            TimeSpan length = new TimeSpan(days, (int)(hours % 24), (int)(mins % 60), (int)(secs % 60), (int)(millis % 1000));
            return SeekForward(length);
        }

        public SoundFX cache() {
            // Will cache, where possible, otherwise, will return a buffered sound
            bool isCachable = (length.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels) <= SFXEngineProperties.MaxCachedSoundSize;
            isCachable &= (reader.TotalTime - reader.CurrentTime) <= SFXEngineProperties.MaxCachedSoundFile;
            if (!isCachable) {
                return new BufferedSound(this);
            } else {
                long currPosition = reader.Position;
                try {                    
                    return new CachedSound(this);
                } catch (ArgumentOutOfRangeException) {
                    reader.Position = currPosition; // reset to previous position in the stream (restore bytes read into the CachedSound)
                    return new BufferedSound(this);
                }
            }
        }

        public SoundFX dup() {
            return new SoundFile(filename);
        }

        public Boolean makeReady() {
            if ((state == EffectState.RUNNING) || (state == EffectState.PAUSED)) return false;
            ResetPlayback();
            state = EffectState.READY;
            return true;
        }

        public bool playBuffer() {
            if (state != EffectState.READY) return false;
            state = EffectState.RUNNING;
            return true;
        }

        public Boolean play(PlaybackDevice dev) {
            if (state != EffectState.READY) return false;
            if (dev is AudioPlaybackEngine) {
                var audio = (AudioPlaybackEngine)dev;
                toSampleProvider();
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
            if (this.state == EffectState.LOADING) return true; // initial start, already reset
            if (this.state == EffectState.READY) return true;   // initial start, already reset
            this.state = EffectState.LOADING;
            if (reader == null) {
                reader = new AudioFileReader(filename);
            } else {
                reader.Position = 0;
            }
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
            if (cachedProvider == null) {
                cachedProvider = this;
                // prepare for sample playback
                makeReady();
                playBuffer();
            }
            return cachedProvider;
        }

        Effect Effect.cache() {
            return cache();
        }

        Effect Effect.dup() {
            return dup();
        }

        public void Dispose() {
            if (reader != null) ((IDisposable)reader).Dispose();
        }
    }
}
