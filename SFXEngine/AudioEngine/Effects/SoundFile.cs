using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Effects {
    public class SoundFile : SFXEngine.AudioEngine.SoundFX, IDisposable {

        private AudioFileReader _source;
        private AudioFileReader source {
            get {
                return _source;
            }
            set {
                // Make sure we properly dispose of any existing reader (ignore dispose if not set, or
                // if we are assigning the same reader, so we don't dispose a reader we're still using).
                if ((_source != null) && ((value == null) || (_source != value))) _Dispose();
                _source = value;
            }
        }
        
        private ISampleProvider readerSample;

        public string filename { get; private set; }

        public override WaveFormat WaveFormat {
            get {
                return source.WaveFormat;
            }
        }

        public override TimeSpan currentTime {
            get {
                return source.CurrentTime;
            }
        }

        public SoundFile(AudioFileReader reader) {
            if (reader == null) throw new ArgumentNullException();
            this.source = reader;
            this.readerSample = source.ToSampleProvider();
            this.length = source.TotalTime;
            this.canSeek = true;
            this.canDuplicate = false;
        }

        public SoundFile(string filename) : this (new AudioFileReader(filename)) {
            this.filename = System.IO.Path.GetFullPath(filename);
            this.canDuplicate = true;   // with the original source data, we can duplicate the stream
        }

        public SoundFile(System.IO.FileInfo file) : this(file.FullName) {}

        ~SoundFile() {
            _Dispose();
        }

        public void close() {
            stop();
        }

        public override Boolean stop() {
            bool _result = base.stop();
            if (_result) {
                if (_source != null) {
                    ((IDisposable)_source).Dispose();
                    _source = null;
                }
            }
            return _result;
        }

        public override SoundFX dup() {
            if (canDuplicate) {
                SoundFile _result = new SoundFile(filename);
                _result.source.CurrentTime = this.currentTime;
                return _result;
            } else {
                return null;
            }
        }

        public override Boolean seekForward(Int64 sampleLength) {
            return seekTo(sampleLength + currentSample);
        }

        public override Boolean seekForward(TimeSpan ts) {
            return seekTo(currentTime + ts);
        }

        public override Boolean seekTo(Int64 sampleIndex) {
            long millis = (long)Math.Round((((double)(sampleIndex * 1000)) / WaveFormat.Channels / WaveFormat.SampleRate), MidpointRounding.AwayFromZero);
            long secs = millis / 1000;
            long mins = secs / 60;
            long hours = mins / 60;
            int days = (int)(hours / 24);
            TimeSpan index = new TimeSpan(days, (int)(hours % 24), (int)(mins % 60), (int)(secs % 60), (int)(millis % 1000));
            return seekTo(index);
        }

        public override Boolean seekTo(TimeSpan index) {
            lock (_play_lock) {
                source.CurrentTime = index;
                onSeek.triggerEvent(this);
                return true;
            }
        }

        public override Int32 ReadSamples(Single[] buffer, Int32 offset, Int32 count) {
            return readerSample.Read(buffer, offset, count);
        }

        public void Dispose() {
            _Dispose();
            GC.SuppressFinalize(this);
        }

        private void _Dispose() {
            if (_source != null) ((IDisposable)_source).Dispose();
        }
    }
}
