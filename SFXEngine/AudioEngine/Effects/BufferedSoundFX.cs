﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using System.Threading;

namespace SFXEngine.AudioEngine.Effects {
    public class BufferedSoundFX : SFXEngine.AudioEngine.SoundFX {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(BufferedSoundFX));

        static BufferedSoundFX() {
            // TODO: Move this to application level
            log4net.Config.BasicConfigurator.Configure();
        }

        private float[] mainBuffer;
        private float[] secondaryBuffer;
        private object mainBuffer_lock = new object();
        private object secondaryBuffer_lock = new object();

        private ISampleProvider source;
        private int position = 0;
        private long totalPosition = 0;

        public override WaveFormat WaveFormat {
            get {
                return source.WaveFormat;
            }
        }

        public override Int64 currentSample {
            get {
                return totalPosition;
            }
        }

        public override TimeSpan currentTime {
            get {
                long ticks = (long)Math.Round((double)(currentSample * TimeSpan.TicksPerSecond / (source.WaveFormat.SampleRate * source.WaveFormat.Channels)), MidpointRounding.ToEven);
                return new TimeSpan(ticks);
            }
        }

        public BufferedSoundFX(SoundFX source) : this((ISampleProvider)source) {}

        public BufferedSoundFX(ISampleProvider source) {
            if (source is SoundFX) {
                this.length = ((SoundFX)source).length;
                this.canDuplicate = ((SoundFX)source).canDuplicate;
                this.canSeek = ((SoundFX)source).canSeek;
            } else {
                this.length = TimeSpan.Zero;
                this.canDuplicate = false;
                this.canSeek = false;
            }
            this.source = source;
        }

        public static void buffer(object obj) {
            if (obj is BufferedSoundFX) {
                BufferedSoundFX snd = (BufferedSoundFX)obj;
                lock (snd.secondaryBuffer_lock) {
                    if ((snd.secondaryBuffer == null) || (snd.secondaryBuffer.Length == 0)) {
                        log.Debug("Loading secondary buffer");
                        // only buffer if not already buffered
                        var buffer = new List<float>();
                        var readBuffer = new float[snd.WaveFormat.SampleRate * snd.WaveFormat.Channels];
                        int samplesRead;
                        int totalSamples = 0;
                        while ((samplesRead = snd.source.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                            buffer.AddRange(readBuffer.Take(samplesRead));
                            totalSamples += samplesRead;
                            if (totalSamples >= SFXEngineProperties.AudioBufferSize) break;
                        }
                        snd.secondaryBuffer = buffer.ToArray();
                    }
                }
            }
        }

        private void loadMainBuffer() {
            lock (mainBuffer_lock) {
                lock (secondaryBuffer_lock) {
                    log.Debug("Switching main buffer");
                    buffer(this);                   // ensure the secondary buffer is filled
                    mainBuffer = secondaryBuffer;   // shift the buffered data into the main buffer
                    secondaryBuffer = null;         // clear the secondary buffer
                    position = 0;                   // move to start of new buffer
                    ThreadPool.QueueUserWorkItem(buffer, this); // start thread to preload the secondary buffer
                }
            }
        }

        public override SoundFX cache() {
            lock (mainBuffer_lock) {
                if (mainBuffer == null) loadMainBuffer();
            }
            return this;
        }

        public override SoundFX dup() {
            if ((canDuplicate) && (source is SoundFX)) {
                SoundFX fx = ((SoundFX)source).dup();
                if (fx != null) return new BufferedSoundFX(fx);
            }
            return null;
        }

        public override Boolean seekTo(TimeSpan index) {
            if ((canSeek) && (source is SoundFX)) {
                lock (mainBuffer_lock) {
                    lock (secondaryBuffer) {
                        bool _result = ((SoundFX)source).seekTo(index);
                        if (_result) {
                            onSeek.triggerEvent(this);
                            position = 0;
                            totalPosition = (int)Math.Round(index.TotalSeconds * WaveFormat.SampleRate * WaveFormat.Channels, MidpointRounding.ToEven);
                            mainBuffer = null;
                            secondaryBuffer = null;
                            ThreadPool.QueueUserWorkItem(buffer, this);
                        }
                        return _result;
                    }
                }
            }
            return false;
        }

        public override Boolean seekTo(Int64 sampleIndex) {
            if ((canSeek) && (source is SoundFX)) {
                lock (mainBuffer_lock) {
                    lock (secondaryBuffer) {
                        bool _result = ((SoundFX)source).seekTo(sampleIndex);
                        if (_result) {
                            onSeek.triggerEvent(this);
                            position = 0;
                            totalPosition = sampleIndex;
                            mainBuffer = null;
                            secondaryBuffer = null;
                            ThreadPool.QueueUserWorkItem(buffer, this);
                        }
                        return _result;
                    }
                }
            }
            return false;
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
                lock (mainBuffer_lock) {
                    if (mainBuffer == null) loadMainBuffer();       // preload the first buffer
                    if ((mainBuffer == null) || (mainBuffer.Length == 0)) {
                        // no samples from the source... end of stream
                        stop();
                        return 0;
                    }
                    int samplesCopied = 0;
                    while (isPlaying && (samplesCopied < count)) {
                        var availableSamples = mainBuffer.Length - position;
                        if (availableSamples == 0) {
                            // first try to load a new buffer
                            loadMainBuffer();
                            availableSamples = mainBuffer.Length - position;
                        }
                        if (availableSamples == 0) stop();  // we've loaded a new buffer, and still no samples, must be end of source stream
                        var samplesToCopy = Math.Min(availableSamples, count);
                        Array.Copy(mainBuffer, position, buffer, offset + samplesCopied, samplesToCopy);
                        position += samplesToCopy;
                        totalPosition += samplesToCopy;
                        samplesCopied += samplesToCopy;
                    }
                    return samplesCopied;
                }
            }
        }
    }
}