using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioPlaybackEngine;

namespace SFX_AudioTest {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private CachedSound sound1 = null;
        private CachedSound sound2 = null;
        private CachedSound sound3 = null;

        private void button1_Click(Object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK) {
                CachedSound snd = new CachedSound(openFileDialog1.FileName);
                if (sound1 == null) sound1 = snd;
                else if (sound2 == null) sound2 = snd;
                else sound3 = snd;
                label1.Text = openFileDialog1.FileName;
                button2.Enabled = true;
            }
        }

        private void button2_Click(Object sender, EventArgs e) {
            // CachedSound snd = new CachedSound(label1.Text);
            // AudioPlaybackEngine.Instance.PlaySound(snd);

            SequenceEffect seq = new SequenceEffect(new TimeSpan(0, 0, 3));
            if (sound1 != null) seq.appendEffect(sound1);
            if (sound2 != null) seq.appendEffect(sound2);
            // seq.playback(AudioPlaybackEngine.AudioPlaybackEngine.Instance);
            SimultaneousEffect sim = new SimultaneousEffect();
            sim.addEffect(seq);
            if (sound3 != null) sim.addEffect(sound3);
            sim.playback(AudioPlaybackEngine.AudioPlaybackEngine.Instance);
        }

        private void button3_Click(Object sender, EventArgs e) {
            AudioPlaybackEngine.AudioPlaybackEngine.Instance.Stop();
            // GC.Collect();
        }
    }
}
