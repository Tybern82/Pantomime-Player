using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SQLite;
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;

namespace SFXPlayer {
    public class SFXShowFile {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SFXShowFile));

        private static readonly string ShowDBFilename = "show.db";
        private static readonly string ShowSoundsDir = "sounds";
        private static readonly string ShowAnnounceDir = "announce";

        private SFXShowProperties _showDetails;
        public SFXShowProperties showDetails {
            get { return _showDetails; }
            set {
                _showDetails.update(value);
            }
        }

        private SQLiteConnection conn;
        private object conn_lock = new object();

        public DirectoryInfo baseDirectory { get; private set; }
        public FileInfo dbFile { get; private set; }
        public DirectoryInfo soundsDirectory { get; private set; }
        public DirectoryInfo announceDirectory { get; private set; }

        private SFXShowFileAnnouncements _announcements;
        public SFXShowFileAnnouncements Announcements {
            get {
                // Lazy construct the announcements.
                if (_announcements == null) _announcements = new SFXShowFileAnnouncements(this);
                return _announcements;
            }
        }

        public SFXShowFile(string fname) {
            this._showDetails = new SFXShowProperties(this);
            baseDirectory = new DirectoryInfo(fname);
            if (!baseDirectory.Exists) baseDirectory.Create();
            soundsDirectory = new DirectoryInfo(Path.Combine(fname, ShowSoundsDir));
            if (!soundsDirectory.Exists) soundsDirectory.Create();
            announceDirectory = new DirectoryInfo(Path.Combine(fname, ShowAnnounceDir));
            if (!announceDirectory.Exists) announceDirectory.Create();
            dbFile = new FileInfo(Path.Combine(fname, ShowDBFilename));
            if (!dbFile.Exists) {
                conn = new SQLiteConnection(dbFile.FullName, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                createDBStructure();
            } else {
                conn = new SQLiteConnection(dbFile.FullName);
                loadDBStructure();
            }
        }

        public void updateProperties(string name, SFXShowProperties props) {
            lock (conn_lock) {
                if (name != props.Name) {
                    // we're modifying the primary key, need to delete the old entry
                    conn.Delete<SFXShowProperties>(name);
                }
                conn.InsertOrReplace(props);
                this._showDetails = props;
            }
        }

        public void updateProperties(SFXShowProperties props) {
            updateProperties(props.Name, props);
        }

        public static bool verifyShowFile(DirectoryInfo dInfo) {
            logger.Debug("Validating [" + dInfo.FullName + "]");
            if (!dInfo.Exists) {
                logger.Debug("Attempt to open show file which does not exist at all.");
                return false;
            }
            FileInfo showFile = new FileInfo(Path.Combine(dInfo.FullName, ShowDBFilename));
            if (!showFile.Exists) {
                logger.Debug("Attempt to open show file with no database.");
                return false;
            }
            SQLiteConnection conn = new SQLiteConnection(showFile.FullName);
            var tInfo = conn.GetTableInfo("SFXShowProperties");
            if (tInfo.Count == 0) {
                logger.Debug("Missing basic properties from show database. Corrupt DB?");
                return false;
            }
            tInfo = conn.GetTableInfo("RegisteredEffect");
            if (tInfo.Count == 0) {
                logger.Debug("Missing registry of sound effects in database. Corrupt DB?");
                return false;
            }
            // TODO: Continue implementing the structure check - in concert with createDBStructure()
            logger.Debug("Validate show file successfully.");
            return true;
        }

        private void createDBStructure() {
            lock (conn_lock) {
                conn.CreateTable<SFXShowProperties>();
                conn.Insert(showDetails);
                conn.CreateTable<RegisteredEffect>();
                // TODO: Continue implementing the basic database structure
            }
        }

        private void loadDBStructure() {
            lock (conn_lock) {
                SFXShowProperties props = conn.Table<SFXShowProperties>().First();
                if (props == null) conn.Insert(showDetails);
                else showDetails.load(props);
                lock (registeredEffects_lock) {
                    foreach (RegisteredEffect e in conn.Table<RegisteredEffect>()) {
                        registeredEffects[e.SourceID] = e;
                    }
                }
                // TODO: Continue loading the datbase structure
            }
        }

        private SortedDictionary<uint, RegisteredEffect> registeredEffects = new SortedDictionary<uint, RegisteredEffect>();
        private object registeredEffects_lock = new object();

        public bool isRegisteredEffect(uint regID) {
            lock (registeredEffects_lock) {
                return registeredEffects.ContainsKey(regID);
            }
        }

        public RegisteredEffect getRegisteredEffect(uint regID) {
            lock (registeredEffects_lock) {
                return registeredEffects[regID];
            }
        }

        public List<RegisteredEffect> getRegisteredEffects() {
            lock (registeredEffects_lock) {
                return registeredEffects.Values.ToList();
            }
        }

        private uint _nextEffect = 0;
        public long registerEffect(SoundFile fx) {
            lock (registeredEffects_lock) {
                while (isRegisteredEffect(_nextEffect)) _nextEffect++;
                updateEffect(_nextEffect, fx);
                uint regID = _nextEffect;
                _nextEffect++;
                return regID;
            }
        }

        public bool updateEffect(uint reqIndex, SoundFile fx) {
            lock (registeredEffects_lock) {
                if (isRegisteredEffect(reqIndex)) {
                    if (fx == null) {
                        registeredEffects.Remove(reqIndex);
                        conn.Delete<RegisteredEffect>(reqIndex);
                    } else {
                        RegisteredEffect row = registeredEffects[reqIndex];
                        row.fx = fx;
                        registeredEffects[reqIndex] = row;
                        conn.InsertOrReplace(row);
                    }
                    return true;
                } else {
                    if (fx == null) return false;
                    RegisteredEffect row = new RegisteredEffect(reqIndex, fx);
                    registeredEffects[reqIndex] = row;
                    conn.InsertOrReplace(row);
                    return false;
                }
            }
        }

        public void close() {
            lock (registeredEffects_lock) {
                foreach (RegisteredEffect row in registeredEffects.Values) {
                    row.fx.Dispose();
                }
            }
            // TODO: Cleanup the cue lists
        }
    }
}
