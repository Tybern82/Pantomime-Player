﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine.Effects {
    public class SoundFile : SFXEngine.AudioEngine.SoundFX, IDisposable {

        private AudioFileReader source;
        private ISampleProvider readerSample;
        private string filename;

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
            this.filename = filename;
            this.canDuplicate = true;   // with the original source data, we can duplicate the stream
        }

        public override SoundFX dup() {
            if (canDuplicate) {
                return new SoundFile(filename);
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

        public override Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            lock (_play_lock) {
                if (isPaused) {
                    return readSilence(buffer, offset, count);
                } else if (isStopped) return 0;
                if (!isPlaying) {
                    isPlaying = true;
                    onPlay.triggerEvent(this);
                }
                int _result = readerSample.Read(buffer, offset, count);
                if (_result == 0) stop();
                return _result;
            }
        }

        public void Dispose() {
            ((IDisposable)source).Dispose();
        }
    }
}
