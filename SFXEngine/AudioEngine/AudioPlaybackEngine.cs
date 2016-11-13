using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SFXEngine.AudioEngine {
    public class AudioPlaybackEngine : IDisposable {

        public static AudioPlaybackEngine Instance = new AudioPlaybackEngine();

        public const UInt16 DefaultSampleRate = 44100;
        public const UInt16 DefaultChannelCount = 2;

        public UInt16 AudioSampleRate { get; private set; }
        public UInt16 AudioChannelCount { get; private set; }

        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine() : this(DefaultSampleRate, DefaultChannelCount) { }

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

        public void stop() {
            mixer.RemoveAllMixerInputs();
        }

        public void stop(ISampleProvider snd) {
            mixer.RemoveMixerInput(snd);
        }

        public ISampleProvider play(ISampleProvider source) {
            try {
                // resample, if required
                if (source.WaveFormat.SampleRate != AudioSampleRate) source = new WdlResamplingSampleProvider(source, AudioSampleRate);
                // adjust channel count, if required
                if (source.WaveFormat.Channels != AudioChannelCount) source = SFXUtilities.AdjustChannelCount(source, AudioChannelCount);
                // begin playing
                mixer.AddMixerInput(source);
                return source;
            } catch (UnsupportedAudioException) {
                return null;
            }
        }

        public void Dispose() {
            outputDevice.Dispose();
        }
    }
}
