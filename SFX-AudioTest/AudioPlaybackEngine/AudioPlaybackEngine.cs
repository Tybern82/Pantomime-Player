using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioPlaybackEngine {
    public class AudioPlaybackEngine : IDisposable {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public int AudioSampleRate { get; private set; }
        public int AudioChannelCount { get; private set; }

        public AudioPlaybackEngine(int sampleRate = DefaultSampleRate, int channelCount = DefaultChannelCount) {
            AudioSampleRate = sampleRate;
            AudioChannelCount = channelCount;
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void PlaySound(string fileName) {
            if (fileName == null) return;
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input) {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels) {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2) {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(CachedSound sound) {
            if (sound == null) return;
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        public void PlaySound(ISampleProvider sound) {
            if (sound == null) return;
            AddMixerInput(sound);
        }

        public void Stop() {
            mixer.RemoveAllMixerInputs();
        }

        private void AddMixerInput(ISampleProvider input) {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        public void Dispose() {
            outputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(DefaultSampleRate, DefaultChannelCount);
        public const int DefaultSampleRate = 44100;
        public const int DefaultChannelCount = 2;
    }
}
