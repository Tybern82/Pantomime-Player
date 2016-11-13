using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Threading;

namespace SFXEngine {
    public class BufferedSound : AbstractSoundFX {

        private Single[] MainAudioBuffer;
        private Single[] SecondaryAudioBuffer;
        private readonly object MainAudioBuffer_lock = new object();
        private readonly object SecondaryAudioBuffer_lock = new object();

        private ISampleProvider source;

        public BufferedSound(SoundFX source) : this(source.toSampleProvider()) { }

        public BufferedSound(ISampleProvider source) {
            this.source = source;
            this.WaveFormat = source.WaveFormat;
            this.state = EffectState.LOADING;
            if (source is Effect) {
                this.length = ((Effect)source).length;
            } else {
                // No way to determine length from just the ISampleProvider, default to '0'
                this.length = new TimeSpan();
            }
        }

        public static void buffer(object obj) {
            if (obj is BufferedSound) {
                BufferedSound snd = (BufferedSound)obj;
                lock (snd.SecondaryAudioBuffer_lock) {
                    if ((snd.SecondaryAudioBuffer == null) || (snd.SecondaryAudioBuffer.Length == 0)) {
                        // only buffer if it doesn't already exist
                        var buffer = new List<Single>();
                        var readBuffer = new float[snd.source.WaveFormat.SampleRate * snd.source.WaveFormat.Channels];
                        int samplesRead;
                        long totalSamples = 0;
                        while ((samplesRead = snd.source.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                            buffer.AddRange(readBuffer.Take(samplesRead));
                            totalSamples += samplesRead;
                            if (totalSamples >= SFXEngineProperties.AudioBufferSize) break;
                        }
                        snd.SecondaryAudioBuffer = buffer.ToArray();
                    }
                }
            }
        }

        private void loadMainBuffer() {
            lock (MainAudioBuffer_lock) {
                lock (SecondaryAudioBuffer_lock) {
                    // Ensure the secondary buffer is filled
                    buffer(this);
                    // Move the secondary buffer to the main buffer
                    MainAudioBuffer = SecondaryAudioBuffer;
                    // and reset the secondary buffer to be empty
                    SecondaryAudioBuffer = null;
                    // Move to the start position in the new main buffer
                    position = 0;
                    // Start a thread to preload the secondary buffer
                    ThreadPool.QueueUserWorkItem(buffer, this);
                    // Thread t = new Thread(buffer);
                    // t.Start(this);
                }
            }
        }

        public override SoundFX cache() {
            lock (MainAudioBuffer_lock) {
                // If the buffer is not already filled, prepare to fill it
                if (MainAudioBuffer == null) loadMainBuffer();                
            }
            return this;
        }

        public override SoundFX dup() {
            if (source is SoundFX) {
                SoundFX src = (SoundFX)source;
                return new BufferedSound(src.dup().toSampleProvider());
            }
            // We can't duplicate the underlying sample source, therefore we can't duplicate the buffer
            return null;
        }

        public override Boolean ResetPlayback() {
            if (this.state == EffectState.RUNNING) return false;
            if (this.state == EffectState.LOADING) return true; // initial start, already reset
            if (this.state == EffectState.READY) return true;   // initial start, already reset
            if (source is SoundFX) {
                this.state = EffectState.LOADING;
                bool result = ((SoundFX)source).ResetPlayback();
                if (!result) return false;  // if we can't reset the underlying stream, we can't reset the buffer
                lock (MainAudioBuffer_lock) {
                    lock (SecondaryAudioBuffer_lock) {
                        MainAudioBuffer = null;
                        SecondaryAudioBuffer = null;
                        position = 0;
                    }
                }
                return true;
            } else return false;
        }

        private long position = 0;

        public override Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            if (this.state != EffectState.RUNNING) return 0;
            lock (MainAudioBuffer_lock) {
                if (MainAudioBuffer == null) loadMainBuffer();  // preload the first buffer
                if ((MainAudioBuffer == null) || (MainAudioBuffer.Length == 0)) {
                    // no samples in the buffer.... end of stream
                    this.state = EffectState.STOPPED;
                    return 0;
                }
                long samplesCopied = 0;
                while ((state == EffectState.RUNNING) && (samplesCopied < count)) {
                    var availableSamples = MainAudioBuffer.Length - position;
                    if (availableSamples == 0) {
                        loadMainBuffer();
                        availableSamples = MainAudioBuffer.Length - position;
                    }
                    if (availableSamples == 0) this.state = EffectState.STOPPED;
                    var samplesToCopy = Math.Min(availableSamples, count);
                    Array.Copy(MainAudioBuffer, position, buffer, offset + samplesCopied, samplesToCopy);
                    position += samplesToCopy;
                    samplesCopied += samplesToCopy;
                }
                return (int)samplesCopied;
            }
        }
    }
}