using System;
using System.IO;

using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;

namespace SFXPlayer {
    public class SFXShowFileAnnouncements {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SFXShowFileAnnouncements));

        public static readonly string fShowBeginning = "Show Beginning";
        public static readonly string fIntervalWarning = "Interval - 5 Minute Warning";
        public static readonly string fPreshow05 = "Preshow - 5 Minute Warning";
        public static readonly string fPreshow10 = "Preshow - 10 Minute Warning";
        public static readonly string fSpecial = "Special Notice";

        public static readonly string fBeginInterval10 = "Interval - 10 Minutes";
        public static readonly string fBeginInterval15 = "Interval - 15 Minutes";
        public static readonly string fBeginInterval20 = "Interval - 20 Minutes";
        public static readonly string fBeginInterval25 = "Interval - 25 Minutes";
        public static readonly string fBeginInterval30 = "Interval - 30 Minutes";
        public static readonly string fBeginInterval35 = "Interval - 35 Minutes";

        public static readonly string fPreshowInterval10 = "Preshow - 10 Minutes";
        public static readonly string fPreshowInterval15 = "Preshow - 15 Minutes";
        public static readonly string fPreshowInterval20 = "Preshow - 20 Minutes";
        public static readonly string fPreshowInterval25 = "Preshow - 25 Minutes";
        public static readonly string fPreshowInterval30 = "Preshow - 30 Minutes";
        public static readonly string fPreshowInterval35 = "Preshow - 35 Minutes";

        private SFXShowFile show;

        public SFXShowFileAnnouncements(SFXShowFile show) {
            this.show = show;
        }

        private SoundFile loadAnnouncement(string name) {
            FileInfo[] cFiles = show.announceDirectory.GetFiles(name + ".*");
            foreach (FileInfo cFile in cFiles) {
                try {
                    // Implicitly allow a file of text type to be included in the override folder.
                    // This file can then be used to store the text of the actual announcement in place
                    // without affecting the override functionality.
                    logger.Debug("Checking [" + cFile.FullName + "] - <" + cFile.Extension + ">");
                    if (cFile.Extension != ".txt") {
                        logger.Info("Custom file detected for [" + name + "] at <" + cFile.FullName + ">");
                        return new SoundFile(cFile.FullName);
                    }
                } catch (Exception e) {
                    // Errors in the override won't crash the app, just write a notice and eventually we'll fallback to the
                    // embedded version.
                    logger.Info("Invalid override announcement for <" + name + "> found at [" + cFile.FullName + "]", e);
                }
            }
            return null;
        }

        public SoundFX ShowBeginning {
            get {
                SoundFX _result = loadAnnouncement(fShowBeginning);
                if (_result == null) _result = Announcements.ShowBeginning;
                return _result;
            }
        }

        public SoundFX Interval_5MinuteWarning {
            get {
                SoundFX _result = loadAnnouncement(fIntervalWarning);
                if (_result == null) _result = Announcements.Interval_5MinuteWarning;
                return _result;
            }
        }

        public SoundFX BeginInterval(IntervalTime length) {
            SoundFX _result = null;
            switch (length) {
                case IntervalTime.I10: _result = loadAnnouncement(fBeginInterval10); break;
                case IntervalTime.I15: _result = loadAnnouncement(fBeginInterval15); break;
                case IntervalTime.I20: _result = loadAnnouncement(fBeginInterval20); break;
                case IntervalTime.I25: _result = loadAnnouncement(fBeginInterval25); break;
                case IntervalTime.I30: _result = loadAnnouncement(fBeginInterval30); break;
                case IntervalTime.I35: _result = loadAnnouncement(fBeginInterval35); break;
            }
            if (_result == null) _result = Announcements.BeginInterval(length);
            return _result;
        }

        public SoundFX PreshowInterval(IntervalTime length) {
            SoundFX _result = null;
            switch (length) {
                case IntervalTime.I10: _result = loadAnnouncement(fPreshowInterval10); break;
                case IntervalTime.I15: _result = loadAnnouncement(fPreshowInterval15); break;
                case IntervalTime.I20: _result = loadAnnouncement(fPreshowInterval20); break;
                case IntervalTime.I25: _result = loadAnnouncement(fPreshowInterval25); break;
                case IntervalTime.I30: _result = loadAnnouncement(fPreshowInterval30); break;
                case IntervalTime.I35: _result = loadAnnouncement(fPreshowInterval35); break;
            }
            if (_result == null) _result = Announcements.PreshowInterval(length);
            return _result;
        }

        public SoundFX PreshowEntry_5 {
            get {
                SoundFX _result = loadAnnouncement(fPreshow05);
                if (_result == null) _result = Announcements.PreshowEntry_5;
                return _result;
            }
        }

        public SoundFX PreshowEntry_10 {
            get {
                SoundFX _result = loadAnnouncement(fPreshow10);
                if (_result == null) _result = Announcements.PreshowEntry_10;
                return _result;
            }
        }

        public SoundFX SpecialNotice {
            get {
                SoundFX _result = loadAnnouncement(fSpecial);
                if (_result == null) _result = Announcements.SpecialNotice;
                return _result;
            }
        }
    }
}
