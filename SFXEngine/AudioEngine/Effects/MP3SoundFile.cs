using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Effects {
    public class MP3SoundFile : SFXEngine.AudioEngine.SoundFX, IDisposable {

        private Mp3FileReader source;
        private ISampleProvider readerSample;
        private byte[] mp3Data;

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

        public MP3SoundFile(Mp3FileReader reader) {
            if (reader == null) throw new ArgumentNullException();
            this.source = reader;
            this.readerSample = source.ToSampleProvider();
            this.length = source.TotalTime;
            this.canSeek = true;
            this.canDuplicate = false;
        }

        public MP3SoundFile(byte[] mp3Data) : this (new Mp3FileReader(new MemoryStream(mp3Data))) {
            this.mp3Data = mp3Data;
            this.canDuplicate = true;   // with the original source data, we can duplicate the stream
        }

        ~MP3SoundFile() {
            _Dispose();
        }

        public override Boolean stop() {
            bool _result = base.stop();
            if (_result) {
                if (source != null) {
                    ((IDisposable)source).Dispose();
                    source = null;
                    readerSample = null;
                }
            }
            return _result;
        }

        public override SoundFX dup() {
            if (canDuplicate) {
                MP3SoundFile _result = new MP3SoundFile(mp3Data);
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
            if (source != null) ((IDisposable)source).Dispose();
        }
    }
}
