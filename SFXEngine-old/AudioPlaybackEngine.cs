using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine {
    public class AudioPlaybackEngine : PlaybackDevice, IDisposable {

        public static AudioPlaybackEngine Instance = new AudioPlaybackEngine(DefaultSampleRate, DefaultChannelCount);

        public const UInt16 DefaultSampleRate = 44100;
        public const UInt16 DefaultChannelCount = 2;

        public UInt16 AudioSampleRate { get; private set; }
        public UInt16 AudioChannelCount { get; private set; }
        
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(UInt16 sampleRate, UInt16 channels) {
            this.AudioSampleRate = sampleRate;
            this.AudioChannelCount = channels;
            switch (channels) {
                case 1:
                case 2:
                case 6:
                case 8:
                    break;
                default:
                    throw new NotImplementedException("Only able to support mono, stereo, 5.1 and 7.1 channel outputs.");
            }
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(AudioSampleRate, AudioChannelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void Stop() {
            mixer.RemoveAllMixerInputs();
        }

        public void Stop(ISampleProvider snd) {
            mixer.RemoveMixerInput(snd);
        }

        public bool Play(Effect cue) {
            if (cue is SoundFX) {
                return Play((SoundFX)cue);
            } else {
                return false;
            }
        }

        public bool Play(SoundFX cue) {
            ISampleProvider source = cue.toSampleProvider();
            AutoRemoveMixerInput.RemoveMixerInput(cue, this);
            return Play(source);
        }

        public bool Play(ISampleProvider snd) {
            try {
                // Resample input, if required
                if (snd.WaveFormat.SampleRate != AudioSampleRate) snd = new WdlResamplingSampleProvider(snd, AudioSampleRate);
                // Then adjust the channel outputs
                if (snd.WaveFormat.Channels != AudioChannelCount) snd = AdjustChannelCount(snd, AudioChannelCount);
                // Finally begin playing
                mixer.AddMixerInput(snd);
                return true;
            } catch (NotImplementedException) {
                return false;
            }
        }

        public static ISampleProvider AdjustChannelCount(ISampleProvider snd, UInt16 AudioChannelCount) {
            if (AudioChannelCount == 2) {
                if (snd.WaveFormat.Channels == 2) return snd;
                if (snd.WaveFormat.Channels == 1) return new MonoToStereoSampleProvider(snd);
                var sInput = new MultiplexingSampleProvider(new ISampleProvider[] { snd }, 2);
                if (snd.WaveFormat.Channels == 6) {
                    // 5.1 sound
                    sInput.ConnectInputToOutput(0, 0);  // FRONT-LEFT -> LEFT
                    sInput.ConnectInputToOutput(1, 1);  // FRONT-RIGHT -> RIGHT
                    sInput.ConnectInputToOutput(2, 0);  // CENTRE -> LEFT
                    sInput.ConnectInputToOutput(2, 1);  // CENTRE -> RIGHT
                                                        // sInput.ConnectInputToOutput(3, ?);   // LFE -> ???
                    sInput.ConnectInputToOutput(3, 0);  // LOW-FREQ -> LEFT
                    sInput.ConnectInputToOutput(3, 1);  // LOW-FREQ -> RIGHT
                    sInput.ConnectInputToOutput(4, 0);  // SURROUND-LEFT -> LEFT
                    sInput.ConnectInputToOutput(5, 1);  // SURROUND-RIGHT -> RIGHT
                } else if (snd.WaveFormat.Channels == 8) {
                    // 7.1 sound (assuming standard order, but alternative still goes to same stereo channel)
                    // L, R, C, LFE, RL, RR, SL, SR
                    sInput.ConnectInputToOutput(0, 0);  // LEFT
                    sInput.ConnectInputToOutput(1, 1);  // RIGHT
                    sInput.ConnectInputToOutput(2, 0);  // CENTRE
                    sInput.ConnectInputToOutput(2, 1);
                    sInput.ConnectInputToOutput(3, 0);  // LOW-FREQ
                    sInput.ConnectInputToOutput(3, 1);
                    sInput.ConnectInputToOutput(4, 0);  // REAR-LEFT
                    sInput.ConnectInputToOutput(5, 1);  // REAR-RIGHT
                    sInput.ConnectInputToOutput(6, 0);  // SIDE-LEFT
                    sInput.ConnectInputToOutput(7, 1);  // SIDE-RIGHT
                } else throw new NotImplementedException("Unsupported channel count detected...");
                return sInput;
            } else if (AudioChannelCount == 1) {
                if (snd.WaveFormat.Channels == 1)
                    return snd;
                else
                    return new MultiChannelToMonoSampleProvider(snd);
            } else if (AudioChannelCount == 6) {
                var sInput = new MultiplexingSampleProvider(new ISampleProvider[] { snd }, 6);
                if (snd.WaveFormat.Channels == 6) return snd;
                else if (snd.WaveFormat.Channels == 1) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(0, 1);
                    sInput.ConnectInputToOutput(0, 2);
                    sInput.ConnectInputToOutput(0, 3);
                    sInput.ConnectInputToOutput(0, 4);
                    sInput.ConnectInputToOutput(0, 5);
                } else if (snd.WaveFormat.Channels == 8) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(2, 2);
                    sInput.ConnectInputToOutput(3, 3);
                    sInput.ConnectInputToOutput(4, 4);
                    sInput.ConnectInputToOutput(5, 5);
                    sInput.ConnectInputToOutput(6, 4);
                    sInput.ConnectInputToOutput(7, 5);
                } else if (snd.WaveFormat.Channels == 2) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(0, 2);
                    sInput.ConnectInputToOutput(1, 2);
                    sInput.ConnectInputToOutput(0, 3);
                    sInput.ConnectInputToOutput(1, 3);
                    sInput.ConnectInputToOutput(0, 4);
                    sInput.ConnectInputToOutput(1, 5);
                } else throw new NotImplementedException("Unsupported channel count detected...");
                return sInput;
            } else if (AudioChannelCount == 8) {
                var sInput = new MultiplexingSampleProvider(new ISampleProvider[] { snd }, 8);
                if (snd.WaveFormat.Channels == 8) return snd;
                else if (snd.WaveFormat.Channels == 1) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(0, 1);
                    sInput.ConnectInputToOutput(0, 2);
                    sInput.ConnectInputToOutput(0, 3);
                    sInput.ConnectInputToOutput(0, 4);
                    sInput.ConnectInputToOutput(0, 5);
                    sInput.ConnectInputToOutput(0, 6);
                    sInput.ConnectInputToOutput(0, 7);
                } else if (snd.WaveFormat.Channels == 6) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(2, 2);
                    sInput.ConnectInputToOutput(3, 3);
                    sInput.ConnectInputToOutput(4, 4);
                    sInput.ConnectInputToOutput(5, 5);
                    sInput.ConnectInputToOutput(4, 6);
                    sInput.ConnectInputToOutput(5, 7);
                } else if (snd.WaveFormat.Channels == 2) {
                    sInput.ConnectInputToOutput(0, 0);
                    sInput.ConnectInputToOutput(1, 1);
                    sInput.ConnectInputToOutput(0, 2);
                    sInput.ConnectInputToOutput(1, 2);
                    sInput.ConnectInputToOutput(0, 3);
                    sInput.ConnectInputToOutput(1, 3);
                    sInput.ConnectInputToOutput(0, 4);
                    sInput.ConnectInputToOutput(1, 5);
                    sInput.ConnectInputToOutput(0, 6);
                    sInput.ConnectInputToOutput(0, 7);
                } else throw new NotImplementedException("Unsupported channel count detected...");
                return sInput;
            } else {
                throw new NotImplementedException("Unsupported channel count detected...");
            }
        }

        public void Dispose() {
            outputDevice.Dispose();
        }
    }
}
