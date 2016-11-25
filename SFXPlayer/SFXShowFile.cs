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

        public CueGroup rootCues { get; private set; }

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
            tInfo = conn.GetTableInfo("RegisteredCue");
            if (tInfo.Count == 0) {
                logger.Debug("Missing registered cues in database. Corrupt DB?");
                return false;
            }
            CueGroup g = null;
            try {
                g = conn.Get<CueGroup>(0);
            } catch (Exception) {}
            if (g == null) {
                logger.Debug("Missing root cue element. Corrupt DB?");
                return false;
            }
            // TODO: Continue implementing the structure check - in concert with createDBStructure()
            logger.Debug("Validate show file successfully.");
            return true;
        }

        private void createDBStructure() {
            lock (conn_lock) {
                // Stores the basic information about the show - this table only contains a single row
                conn.CreateTable<SFXShowProperties>();
                conn.Insert(showDetails);
                // Stores information about sound files used for cues
                conn.CreateTable<RegisteredEffect>();
                // Stores the cue information
                conn.CreateTable<RegisteredCue>();
                conn.CreateTable<SilenceCue>();
                conn.CreateTable<CueGroup>();
                conn.CreateTable<CueGroupElement>();
                // Store the root cue grouping (used to define the ordering over the entire list of cues)
                rootCues = new CueGroup();
                rootCues.CueID = 0;
                rootCues.Name = "Show Cues";
                conn.InsertOrReplace(rootCues);
                // TODO: Continue implementing the basic database structure
            }
        }

        private bool loading = false;
        private void loadDBStructure() {
            lock (conn_lock) {
                loading = true;
                // Load the basic show properties
                SFXShowProperties props = conn.Table<SFXShowProperties>().First();
                if (props == null) conn.Insert(showDetails);
                else showDetails.load(props);

                // Load the registered sound effects
                lock (registeredEffects_lock) {
                    foreach (RegisteredEffect e in conn.Table<RegisteredEffect>()) {
                        registeredEffects[e.SourceID] = e;
                    }
                }

                // Load the cues
                lock (cueTable_lock) {
                    // Load the individual cue structures...
                    foreach (Cue c in conn.Table<RegisteredCue>()) {
                        // Cues linked to RegisteredEffect items
                        c.currentShow = this;
                        cueTable[c.CueID] = c;
                    }
                    foreach (SilenceCue c in conn.Table<SilenceCue>()) {
                        // Cues representing a block of silence (used for timed pauses, wait times, etc)
                        c.currentShow = this;
                        cueTable[c.CueID] = c;
                    }
                    foreach (CueGroup g in conn.Table<CueGroup>()) {
                        // Cues which will be filled to represent collection and sequences of other cues
                        g.currentShow = this;
                        cueTable[g.CueID] = g;
                    }
                    // All the individual cues have been loaded - now load the sequence/collection elements
                    // Since the cue items link to the actual cue objects, these *must* be loaded last
                    foreach (CueGroupElement e in conn.Table<CueGroupElement>()) {
                        Cue c = cueTable[e.CueID];
                        if (c is CueGroup) {
                            CueGroup g = c as CueGroup;
                            g.addElement(e);
                        } else {
                            throw new InvalidOperationException("Sequence item found for non-sequenced cue.");
                        }
                    }
                    rootCues = cueTable[0] as CueGroup;
                }
                // TODO: Continue loading the database structure
                loading = false;
            }
        }

        private SortedDictionary<uint, Cue> cueTable = new SortedDictionary<uint, Cue>();
        private object cueTable_lock = new object();

        public bool isCue(uint cueID) {
            lock (cueTable_lock) {
                return cueTable.ContainsKey(cueID);
            }
        }

        public Cue getCue(uint cueID) {
            lock (cueTable_lock) {
                return isCue(cueID) ? cueTable[cueID] : null;
            }
        }

        public List<Cue> getRootCues() {
            lock (cueTable_lock) {
                return rootCues.cueItems.Values.ToList();
            }
        }

        public CueGroup getParent(Cue cue) {
            if (cue == null) return null;
            if (cue == rootCues) return null;
            foreach (Cue c in cueTable.Values) {
                if ((c is CueGroup) && ((c as CueGroup).contains(cue))) return (c as CueGroup);
            }
            return null;
        }

        public void registerCue(Cue c) {
            if (c == null) return;
            c.currentShow = this;
            cueTable[c.CueID] = c;
            updateTable(c);
        }

        public void removeCue(Cue cue) {
            if (cue.CueID == 0) return; // don't delete the root cues list
            uint maxCue = (cueTable.Count == 0) ? 0 : cueTable.Keys.Max();
            for (uint x = 0; x < maxCue; x++) {
                Cue c = cueTable[x];
                if ((c != null) && (c is CueGroup)) ((CueGroup)c).removeElement(c);
            }
            cueTable.Remove(cue.CueID);
            if (cue is RegisteredCue) {
                conn.Delete<RegisteredCue>(cue.CueID);
            } else if (cue is SilenceCue) {
                conn.Delete<SilenceCue>(cue.CueID);
            } else if (cue is CueGroup) {
                conn.Delete<CueGroup>(cue.CueID);
            }
        }

        public void updateTable(Cue c) {
            if (loading) return;    // don't make updates during the load cycle
            if (c is RegisteredCue) {
                updateRegisteredCue((RegisteredCue)c);
            } else if (c is SilenceCue) {
                updateSilenceCue((SilenceCue)c);
            } else if (c is CueGroup) {
                updateCueGroup((CueGroup)c);
            }
        }

        private void updateRegisteredCue(RegisteredCue cue) {
            conn.InsertOrReplace(cue);
        }

        private void updateSilenceCue(SilenceCue cue) {
            conn.InsertOrReplace(cue);
        }

        private void updateCueGroup(CueGroup cue) {
            conn.InsertOrReplace(cue);
            foreach (var item in cue.cueItems) {
                CueGroupElement elem = new CueGroupElement();
                elem.CueID = cue.CueID;
                elem.ItemSequence = item.Key;
                elem.ItemID = item.Value.CueID;
                long id = -1;
                foreach (var i in conn.Table<CueGroupElement>()) {
                    if ((i.CueID == elem.CueID) && (i.ItemSequence == elem.ItemSequence)) {
                        id = (long)i.ElementID;
                        break;
                    }
                }
                if (id == -1) {
                    elem.ElementID = null;
                    conn.Insert(elem);
                } else {
                    elem.ElementID = (uint)id;
                    conn.InsertOrReplace(elem);
                }
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
                    row.fx.close();
                }
            }
            // TODO: Cleanup the cue lists
        }
    }
}
