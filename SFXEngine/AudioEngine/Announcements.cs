using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using NAudio.Wave;
using SFXEngine.AudioEngine.Effects;

namespace SFXEngine.AudioEngine {
    public enum IntervalTime {
        I10 = 10,
        I15 = 15,
        I20 = 20,
        I25 = 25,
        I30 = 30,
        I35 = 35
    }

    public class Announcements {

        public static MP3SoundFile ShowBeginning {
            get {
                return new MP3SoundFile(StandardAnnouncements.ShowBeginning);
            }
        }

        public static MP3SoundFile Interval_5MinuteWarning {
            get {
                return new MP3SoundFile(StandardAnnouncements.Interval_5MinuteWarning);
            }
        }

        public static MP3SoundFile BeginInterval(IntervalTime length) {
            switch (length) {
                case IntervalTime.I10: return new MP3SoundFile(StandardAnnouncements.Interval_Begin10);
                case IntervalTime.I15: return new MP3SoundFile(StandardAnnouncements.Interval_Begin15);
                case IntervalTime.I20: return new MP3SoundFile(StandardAnnouncements.Interval_Begin20);
                case IntervalTime.I25: return new MP3SoundFile(StandardAnnouncements.Interval_Begin25);
                case IntervalTime.I30: return new MP3SoundFile(StandardAnnouncements.Interval_Begin30);
                case IntervalTime.I35: return new MP3SoundFile(StandardAnnouncements.Interval_Begin35);
                default: throw new ArgumentOutOfRangeException("Invalid Interval Time selected");
            }
        }

        public static MP3SoundFile PreshowInterval(IntervalTime length) {
            switch (length) {
                case IntervalTime.I10: return new MP3SoundFile(StandardAnnouncements.Preshow_Interval10);
                case IntervalTime.I15: return new MP3SoundFile(StandardAnnouncements.Preshow_Interval15);
                case IntervalTime.I20: return new MP3SoundFile(StandardAnnouncements.Preshow_Interval20);
                case IntervalTime.I25: return new MP3SoundFile(StandardAnnouncements.Preshow_Interval25);
                case IntervalTime.I30: return new MP3SoundFile(StandardAnnouncements.Preshow_Interval30);
                case IntervalTime.I35: return new MP3SoundFile(StandardAnnouncements.Preshow_Interval35);
                default: throw new ArgumentOutOfRangeException("Invalid Interval Time selected");
            }
        }

        public static MP3SoundFile PreshowEntry_5 {
            get {
                return new MP3SoundFile(StandardAnnouncements.Preshow_Entry05);
            }
        }

        public static MP3SoundFile PreshowEntry_10 {
            get {
                return new MP3SoundFile(StandardAnnouncements.Preshow_Entry10);
            }
        }

        public static MP3SoundFile SpecialNotice_AgathaChristie {
            get {
                return new MP3SoundFile(StandardAnnouncements.SpecialNotice_AgathaChristie);
            }
        }
    }
}
