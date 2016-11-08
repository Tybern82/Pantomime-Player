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

namespace PizzazzSound {


    public partial class Form1 : Form {

        string PreWarning10 = "";
        CachedSound sndPreWarning10;

        string PreWarning05 = "";
        CachedSound sndPreWarning05;

        string PreBegin = "";
        CachedSound sndPreBegin;
        
        enum IntervalTime {
            I10 = 10,
            I15 = 15,
            I20 = 20,
            I25 = 25,
            I30 = 30,
            I35 = 35
        }

        IntervalTime ITime = IntervalTime.I30;

        string[] PreInterval = new string[] {
            "",
            "",
            "",
            "",
            "",
            ""
        };
        CachedSound sndPreInterval;

        string[] Interval = new string[] {
            "",
            "",
            "",
            "",
            "",
            ""
        };
        CachedSound sndInterval;

        public Form1() {
            InitializeComponent();

            sndPreWarning10 = new CachedSound(PreWarning10);
            sndPreWarning05 = new CachedSound(PreWarning05);
            sndPreBegin = new CachedSound(PreBegin);
            switch (ITime) {
                case IntervalTime.I10:
                    sndPreInterval = new CachedSound(PreInterval[0]);
                    sndInterval = new CachedSound(Interval[0]);
                    break;


            }
        }
    }
}
