using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SQLite;
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;
using SFXPlayer.Cues;

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

        public SFXShowFile(string fname, SFXLoadingUI loadUI) {
            this._showDetails = new SFXShowProperties(this);
            baseDirectory = new DirectoryInfo(fname);
            if (!baseDirectory.Exists) baseDirectory.Create();
            soundsDirectory = new DirectoryInfo(Path.Combine(fname, ShowSoundsDir));
            if (!soundsDirectory.Exists) soundsDirectory.Create();
            announceDirectory = new DirectoryInfo(Path.Combine(fname, ShowAnnounceDir));
            if (!announceDirectory.Exists) announceDirectory.Create();
            dbFile = new FileInfo(Path.Combine(fname, ShowDBFilename));
            loadUI.updateProgress(0.1);
            if (!dbFile.Exists) {
                conn = new SQLiteConnection(dbFile.FullName, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                createDBStructure(loadUI);
            } else {
                conn = new SQLiteConnection(dbFile.FullName);
                loadDBStructure(loadUI);
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
            tInfo = conn.GetTableInfo("RegisteredEffectCue");
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

        private void createDBStructure(SFXLoadingUI loadUI) {
            lock (conn_lock) {
                // Stores the basic information about the show - this table only contains a single row
                conn.CreateTable<SFXShowProperties>();
                conn.Insert(showDetails);
                loadUI.updateProgress(0.2);

                // Stores information about sound files used for cues
                conn.CreateTable<RegisteredEffect>();
                loadUI.updateProgress(0.4);

                // Stores the cue information
                conn.CreateTable<RegisteredEffectCue>();
                conn.CreateTable<SilenceCue>();
                conn.CreateTable<CueGroup>();
                conn.CreateTable<CueGroupElement>();
                loadUI.updateProgress(0.6);

                // Store the root cue grouping (used to define the ordering over the entire list of cues)
                rootCues = new CueGroup();
                rootCues.CueID = 0;
                rootCues.Name = "Show Cues";
                rootCues.currentShow = this;
                conn.InsertOrReplace(rootCues);
                cueTable[0] = rootCues;
                loadUI.updateProgress(0.8);
                // TODO: Continue implementing the basic database structure
            }
        }

        private bool loading = false;
        private void loadDBStructure(SFXLoadingUI loadUI) {
            lock (conn_lock) {
                loading = true;
                // Load the basic show properties
                SFXShowProperties props = conn.Table<SFXShowProperties>().First();
                if (props == null) conn.Insert(showDetails);
                else showDetails.load(props);
                loadUI.updateProgress(0.2);

                // Load the registered sound effects
                lock (registeredEffects_lock) {
                    foreach (RegisteredEffect e in conn.Table<RegisteredEffect>()) {
                        registeredEffects[e.SourceID] = e;
                    }
                }
                loadUI.updateProgress(0.4);

                // Load the cues
                lock (cueTable_lock) {
                    // Load the individual cue structures...
                    foreach (SFXCue c in conn.Table<RegisteredEffectCue>()) {
                        // Cues linked to RegisteredEffect items
                        c.currentShow = this;
                        cueTable[c.CueID] = c;
                    }
                    loadUI.updateProgress(0.5);

                    foreach (SilenceCue c in conn.Table<SilenceCue>()) {
                        // Cues representing a block of silence (used for timed pauses, wait times, etc)
                        c.currentShow = this;
                        cueTable[c.CueID] = c;
                    }
                    loadUI.updateProgress(0.6);

                    foreach (CueGroup g in conn.Table<CueGroup>()) {
                        // Cues which will be filled to represent collection and sequences of other cues
                        g.currentShow = this;
                        cueTable[g.CueID] = g;
                    }
                    loadUI.updateProgress(0.7);

                    // All the individual cues have been loaded - now load the sequence/collection elements
                    // Since the cue items link to the actual cue objects, these *must* be loaded last
                    foreach (CueGroupElement e in conn.Table<CueGroupElement>()) {
                        SFXCue c = cueTable[e.CueID];
                        if (c is CueGroup) {
                            CueGroup g = c as CueGroup;
                            g.addElement(e);
                        } else {
                            throw new InvalidOperationException("Sequence item found for non-sequenced cue.");
                        }
                    }
                    rootCues = cueTable[0] as CueGroup;
                    loadUI.updateProgress(0.8);
                }
                // TODO: Continue loading the database structure
                loading = false;
            }
        }

        private SortedDictionary<uint, SFXCue> cueTable = new SortedDictionary<uint, SFXCue>();
        private object cueTable_lock = new object();

        public bool isCue(uint cueID) {
            if (cueID == 0) return true;
            lock (cueTable_lock) {
                return cueTable.ContainsKey(cueID);
            }
        }

        public SFXCue getCue(uint cueID) {
            lock (cueTable_lock) {
                return isCue(cueID) ? cueTable[cueID] : null;
            }
        }

        public List<SFXCue> getRootCues() {
            lock (cueTable_lock) {
                return rootCues.children;
            }
        }

        public CueGroup getParent(SFXCue cue) {
            lock (cueTable_lock) {
                if (cue == null) return null;
                if (cue == rootCues) return null;
                foreach (SFXCue c in cueTable.Values) {
                    if ((c is CueGroup) && ((c as CueGroup).contains(cue))) return (c as CueGroup);
                }
                return null;
            }
        }

        public void registerCue(SFXCue c) {
            if (c == null) return;
            c.currentShow = this;
            cueTable[c.CueID] = c;
            updateTable(c);
        }

        public void removeCue(uint cueID) {
            removeCue(getCue(cueID));
        }

        public void removeCue(SFXCue cue) {
            if (cue == null) return;    // if the cue doesn't exist, we can't delete it
            if (cue.CueID == 0) return; // don't delete the root cues list
            foreach (var x in cueTable.Keys) {
                SFXCue c = cueTable[x];
                if ((c != null) && (c is CueGroup)) {
                    ((CueGroup)c).removeElement(c);
                    // updateTable(c);
                    // c.NotifyPropertyChanged("Length");
                }
            }
            cueTable.Remove(cue.CueID);
            if (cue is RegisteredEffectCue) {
                conn.Delete<RegisteredEffectCue>(cue.CueID);
            } else if (cue is SilenceCue) {
                conn.Delete<SilenceCue>(cue.CueID);
            } else if (cue is CueGroup) {
                conn.Delete<CueGroup>(cue.CueID);
                var currentItems = conn.Table<CueGroupElement>().Where(c => c.CueID == cue.CueID).ToList();
                foreach (var item in currentItems) {
                    conn.Delete<CueGroupElement>(item.ElementID);
                    removeCue(item.ItemID);
                }
            }
        }

        public void updateTable(SFXCue c) {
            if (loading) return;    // don't make updates during the load cycle
            if (!isCue(c.CueID)) {
                logger.Error("Attempting to update an unregistered cue <" + c.CueID + ">");
                return;    // prevent updates of cues which are not registered (maybe caused by changes during the delete process)
            }
            if (c is RegisteredEffectCue) {
                updateRegisteredCue((RegisteredEffectCue)c);
            } else if (c is SilenceCue) {
                updateSilenceCue((SilenceCue)c);
            } else if (c is CueGroup) {
                updateCueGroup((CueGroup)c);
            }
        }

        private void updateRegisteredCue(RegisteredEffectCue cue) {
            conn.InsertOrReplace(cue);
        }

        private void updateSilenceCue(SilenceCue cue) {
            conn.InsertOrReplace(cue);
        }

        private CueGroupElement getElement(uint sequence, uint cueID) {
            CueGroupElement elem = conn.Table<CueGroupElement>().Where(c => c.CueID == cueID && c.ItemSequence == sequence).FirstOrDefault<CueGroupElement>();
            if (elem == null) {
                elem = new CueGroupElement();
                elem.CueID = cueID;
                elem.ItemSequence = sequence;
                elem.ElementID = null;
            }
            return elem;
        }

        private void updateCueGroup(CueGroup cue) {
            // Update the cue group itself
            conn.InsertOrReplace(cue);

            // Then update any items which have been changed in the cue elements
            List<uint> validItems = new List<uint>();
            foreach (var item in cue.children) {
                long itemSequence = cue.sequenceOf(item);
                if (itemSequence == -1) {
                    logger.Error("Unable to determine index of item retrieved as a child from the cue group <" + cue.CueID + ">");
                    throw new InvalidOperationException();
                }
                CueGroupElement elem = getElement((uint)itemSequence, cue.CueID);
                if (elem.ItemID != item.CueID) {
                    elem.ItemID = item.CueID;   // need to update the entry
                    if (elem.ElementID == null) {
                        conn.Insert(elem);
                    } else {
                        conn.InsertOrReplace(elem);
                    }
                }
                if (elem.ElementID == null) logger.Error("PrimaryKey has not been assigned for CueGroupElement.");
                else validItems.Add((uint)elem.ElementID);
            }

            // Then check over the cue elements and remove any items which no longer exist in the cue group
            var currentItems = conn.Table<CueGroupElement>().Where(c => c.CueID == cue.CueID).ToList();
            if (currentItems.Count != validItems.Count) {       // only check through the full list of items where there may be some to delete
                foreach (var item in currentItems) {
                    if (!validItems.Contains(item.ItemSequence)) {
                        conn.Delete<CueGroupElement>(item.ElementID);
                    }
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
                return isRegisteredEffect(regID) ? registeredEffects[regID] : null;
            }
        }

        public uint getFirstEffect() {
            lock (registeredEffects_lock) {
                if (registeredEffects.Count == 0) return 0;
                return registeredEffects.Keys.Min();
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
