using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;

using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;
using NAudio.Wave;

namespace SFXEngine_Test {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        BufferedSoundFX buf;
        CachedSoundFX cache;

        SoundFX sfx1;
        SoundFX sfx2;
        ISampleProvider sample2;

        private void button1_Click(Object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK) {
                label1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(Object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK) {
                label2.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(Object sender, EventArgs e) {
            buf = new BufferedSoundFX(new SoundFile(label1.Text));
            buf.cache();
            AudioPlaybackEngine.Instance.play(buf);
        }

        private void button4_Click(Object sender, EventArgs e) {
            cache = new CachedSoundFX(new SoundFile(label2.Text));
            cache.cache();
            AudioPlaybackEngine.Instance.play(cache);
        }

        private void button5_Click(Object sender, EventArgs e) {
            if (buf != null) buf.stop();
            if (cache != null) cache.stop();
            if (sfx1 != null) sfx1.stop();
            if (sfx2 != null) sfx2.stop();
            if (sample2 != null) AudioPlaybackEngine.Instance.stop(sample2);
            // AudioPlaybackEngine.Instance.Stop();
        }

        private void button6_Click(Object sender, EventArgs e) {
            sfx1 = new SoundFile(label1.Text);
            sfx1 = sfx1.cache();
            if (sfx1 is CachedSoundFX) Console.WriteLine("Automatically selected CachedSound for <" + label1.Text + ">");
            if (sfx1 is BufferedSoundFX) Console.WriteLine("Automatically selected BufferedSound for <" + label1.Text + ">");
            sfx1.seekForward(new TimeSpan(0, 0, 30));
            AudioPlaybackEngine.Instance.play(sfx1);
        }

        private void button7_Click(Object sender, EventArgs e) {
            sfx2 = (new SoundFile(label2.Text)).cache();
            if (sfx2 is CachedSoundFX) Console.WriteLine("Automatically selected CachedSound for <" + label2.Text + ">");
            if (sfx2 is BufferedSoundFX) Console.WriteLine("Automatically selected BufferedSound for <" + label2.Text + ">");
            sample2 = new SFXEngine.AudioEngine.Adapters.FadeSampleProvider(sfx2, SFXUtilities.TimeInSeconds(10), SFXUtilities.TimeInSeconds(30), SFXUtilities.TimeInSeconds(10));
            AudioPlaybackEngine.Instance.play(sample2);
        }

        private void button8_Click(Object sender, EventArgs e) {
            /*
            SoundFXCollection collection = new SoundFXCollection();
            collection.addEffect((new SoundFile(label1.Text)).cache());
            collection.addEffect((new SoundFile(label2.Text)).cache());
            collection.makeReady();
            collection.play(AudioPlaybackEngine.Instance);
            */
        }
    }
}
