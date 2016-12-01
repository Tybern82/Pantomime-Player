using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Groups;
using SFXEngine.Events;

namespace SFXPlayer.Cues {

    /**
     * Enumeration of options for grouping cues.
     * 
     * Currently supports:
     *     SEQUENCE   - Items are played in sequence, with each effect played one at a time.
     *     COLLECTION - Items are all played at once, started simultaneously.
     */
    public enum GroupElementType {
        SEQUENCE, COLLECTION
    }

    /**
     * Helper class used for storing the CueGroup items into the DB structure.
     */
    public class CueGroupElement {
        /**
         * Unique idendifier for the element (necessary only because the SQLite implementation does not currently support multi-column
         * PrimaryKey elements). 'Real' PrimaryKey should be actually [CueID:ItemSequence]. Class supports 'null' in ElementID to allow
         * the DB to automatically allocate a new ID.
         */
        [PrimaryKey, AutoIncrement] public uint? ElementID { get; set; }

        /**
         * Identifier of the CueGroup which this element is a member.
         */
        public uint CueID { get; set; }

        /**
         * This provides the ordering of the cues in the sequence. Cue exists in the group at this position.
         */
        public uint ItemSequence { get; set; }

        /**
         * Identifier of the Cue which is the member of this CueGroup (which may itself be another grouping element).
         */
        public uint ItemID { get; set; }
    }

    /**
     * Cue containing a group of other cue elements. Used for creating groups of simultaneous or sequential cues.
     */
    public class CueGroup : SFXCue {

        #region Local Data
        /**
         * Internal storage for the items in this group.
         */
        private SortedDictionary<uint, SFXCue> cueItems = new SortedDictionary<uint, SFXCue>();
        #endregion

        #region Events
        public PropertyEventRegister onAddChild { get; } = new PropertyEventRegister();
        public PropertyEventRegister onRemoveChild { get; } = new PropertyEventRegister();
        #endregion

        #region Property Definitions
        /**
         * Thread Locks
         */
        private object _CueGroup_lock = new object();
        private object _cueItems_lock = new object();

        /**
         * Override the Name element to automatically select an option based on which type of grouping element we are.
         * If an unknown/invalid grouping type is used, this will automatically retrieve the underlying base Name. All
         * changes are propagated into the underlying base Name element.
         */
        public override string Name {
            get {
                lock (_CueGroup_lock) {
                    switch (Type) {
                        case GroupElementType.COLLECTION: return "Collection";  // Identify as a simultaneous collection of cues
                        case GroupElementType.SEQUENCE: return "Sequence";      // Identify as a sequence of cues
                        default: return base.Name;                              // Invalid collection of cues - use the base Name
                    }
                }
            }

            set {
                lock (_CueGroup_lock) {
                    base.Name = value;  // Base update will trigger the NotifyPropertyChanged event.
                }
            }
        }

        /**
         * The Grouping type for this CueGroup - either a sequence of cues, or a collection of simultaneous items.
         */
        private GroupElementType _Type = GroupElementType.SEQUENCE;
        public GroupElementType Type {
            get { lock (_CueGroup_lock) { return _Type; } }
            set {
                lock (_CueGroup_lock) {
                    logger.Debug("Setting Type of cue group <" + CueID + ">: [" +
                    System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToString().ToLower()) +
                    "]");
                    _Type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Override the Length to retrieve either the longest cue in a collection of simultaneous elements, or the sum of all
         * the cues in a sequence of cues.
         */
        public override TimeSpan Length {
            get {
                lock (_CueGroup_lock) {
                    TimeSpan _result = TimeSpan.Zero;       // start with a length of '0'
                    switch (Type) {
                        case GroupElementType.COLLECTION:
                            // For a collection of cues, iterate over all the cues and identify the longest cue in the collection
                            // which will become the length of the entire collection.
                            lock (_cueItems_lock) {
                                foreach (var item in cueItems.Values) {
                                    if (item != null)                   // Not looking at a null item (just in case....)...
                                        if (_result < item.Length)      // ... and it's longer than we have seen already...
                                            _result = item.Length;      // ... so update our longest element to the new item.
                                }
                            }
                            break;

                        case GroupElementType.SEQUENCE:
                            // For a sequence of cues, iterate over all the cues and add their lengths together to come up with
                            // the total length of the entire sequence.
                            lock (_cueItems_lock) {
                                foreach (SFXCue c in cueItems.Values) {
                                    if (c != null)              // Not looking at a null item (just in case....)...
                                        _result += c.Length;    // ... add in it's length to the running total.
                                }
                            }
                            break;
                    }
                    return _result;
                }
            }

            set { lock (_CueGroup_lock) { base.Length = value; } }
        }

        /**
         * Retrieve the current list of child elements stored in this cue group.
         */
        private List<SFXCue> _children = null;
        private PropertyEventCallback clearChildren = null;
        private object _children_lock = new object();
        [Ignore] public List<SFXCue> children {
            get {
                lock (_cueItems_lock) {
                    List<SFXCue> _result = new List<SFXCue>();
                    foreach (var item in cueItems) {
                        _result.Add(item.Value);
                    }
                    return _result;
                }

                lock (_CueGroup_lock) {
                    if (clearChildren == null) clearChildren = delegate (string prop, object nValue) {
                        lock (_children_lock) {
                            _children = null;
                            if (clearChildren != null) onAddChild.removeEventTrigger(clearChildren);
                            if (clearChildren != null) onRemoveChild.removeEventTrigger(clearChildren);
                        }
                    };
                    lock (_cueItems_lock) {
                        lock (_children_lock) {
                            if (_children == null) {
                                _children = new List<SFXCue>();
                                foreach (var item in cueItems) {
                                    _children.Add(item.Value);
                                }
                                onAddChild.addEventTrigger(clearChildren);
                                onRemoveChild.addEventTrigger(clearChildren);
                            }
                            return _children;
                        }
                    }
                }
            }
        }

        #endregion

        #region Class Methods

        /**
         * Add the given CueGroupElement into this Cue - helper method to make it easier for loading from the DB. Method will
         * currently treat invalid structure in this element as a soft-error - just logged and the item will be ignored.
         */
        public void addElement(CueGroupElement e) {
            if (e == null) {
                logger.Error("Adding null element into cue group <" + CueID + ">");
                throw new ArgumentNullException("CueGroup.addElement<CueGroupElement>.e");
            }
            if (e.CueID != CueID) {
                logger.Warn("Adding cue element into cue group <" + CueID + "> meant for group <" + e.CueID + ">");
                // TODO: Use the global option for triggering exceptions from 'soft' errors (which are currently just logged)
            }
            if (currentShow == null) {
                logger.Warn("Attempting to load cue element into group <" + CueID + "> but missing current show details.");
                // TODO: Soft-Error
            } else {
                SFXCue itemCue = currentShow.getCue(e.ItemID);
                if (itemCue == null) {
                    logger.Warn("Unable to add a cue to group <" + CueID + "> - cannot find a cue under <" + e.ItemID + "> in the current show.");
                    // TODO: Soft-Error
                } else {
                    insertAt(e.ItemSequence, itemCue);
                }
            }
        }

        /**
         * Add the given effect into the end of the group.
         */
        public uint addElement(SFXCue SFXCue) {
            lock (_cueItems_lock) {
                uint nValue = getNextCue();     // get the next cue position after existing elements
                insertAt(nValue, SFXCue);     // store the given cue into this position
                return nValue;                  // return the location to the caller
            }
        }

        /**
         * Helper method to determine the direct parent of a given cue, provided it falls within the current group structure. This method
         * is normally called on the root cue list to find a parent for the current element. Will return 'null' if we cannot find the 
         * given cue within our structure.
         */
        public CueGroup getDirectParent(SFXCue cue) {
            if (cue == null) return null;       // null cue is always null parent
            logger.Debug("Searching for parent of cue <" + cue.CueID + "> in cue group <" + CueID + ">");
            CueGroup _result = null;
            if (cue != this) {  // if we are the requested cue, we will never find them inside our structure, return immediately
                lock (_cueItems_lock) {
                    // Check all our child items
                    foreach (SFXCue item in children) {
                        if (item != null) {
                            // There is an item....
                            if (item == cue) {
                                logger.Debug("Cue found in group <" + CueID + ">");
                                return this;       // if the item is the requested cue, we are their parent, return
                            } else if (item is CueGroup) {        // otherwise, if they are also a group, check if they are the parent
                                _result = (item as CueGroup).getDirectParent(cue);
                                if (_result != null) return _result;    // if they have found the parent, return immediately
                            }
                        }
                    }
                }
            }
            logger.Debug("Cue not found in group <" + CueID + ">");
            return _result; // only reached if no parent was found - we're returning the original 'null' value
        }

        /**
         * Identify the location of the given cue in the current list of cues, or -1 if the cue is not a member of this group (direct member check only).
         */
        public long sequenceOf(SFXCue c) {
            lock (_cueItems_lock) {
                if (contains(c)) {                              // cue is in the list... now to identify it's location
                    foreach (var item in cueItems) {            // check each item in the group...
                        if (item.Value == c) return item.Key;   // ... and return as soon as we've found the correct item.
                    }
                }
                return -1;      // item was not found in the list...
            }
        }

        /**
         * Base method to actually perform an insert at a specific position in the sequence.
         */
        public void insertAt(uint nPos, SFXCue c) {
            lock (_cueItems_lock) {
                if (c == null) {
                    SFXCue oItem = cueItems[nPos];
                    cueItems.Remove(nPos);      // remove the cue at the given position
                    logger.Debug("Removing cue item <" + ((oItem != null) ? oItem.CueID.ToString() : "null") + "> from cue group <" + CueID + ">");
                    structureChanged();
                    oItem.clearParent();
                    onRemoveChild.triggerEvent(nPos.ToString(), oItem);
                    return;
                }
                logger.Debug("Updating cue item <" + nPos + "> from cue group <" + CueID + "> to <" + ((c != null) ? (""+c.CueID) : "null") + ">");
                uint maxCue = getNextCue();
                if (nPos < maxCue) {
                    // need to move existing elements up to make room for the new item
                    for (uint x = maxCue; x > nPos; x--) {     // check over all the possible key locations (starting at the top)
                        if (cueItems.Keys.Contains(x)) {        // if the item location exists
                            cueItems[x + 1] = cueItems[x];      // move it up one position
                            cueItems.Remove(x);                 // and remove it's old position
                        }
                    }
                    if (cueItems.Keys.Contains(nPos)) {
                        cueItems[nPos + 1] = cueItems[nPos];
                        cueItems.Remove(nPos);
                    }
                }
                cueItems[nPos] = c;         // now we have a free space, insert the new item
                structureChanged();
                onAddChild.triggerEvent(nPos.ToString(), c);
            }
        }

        /**
         * Helper method to insert a new cue immediately following the given cue in this sequence. Inserts to the end if the given cue
         * is not part of this sequence.
         */
        public void insertAfter(SFXCue item, SFXCue nCue) {
            lock (_cueItems_lock) {
                long pos = sequenceOf(item);
                uint nPos = (pos == -1) ? getNextCue() : (uint)pos + 1;
                insertAt(nPos, nCue);
            }
        }

        /**
         * Helper method to insert a new cue immediately preceding the given cue in this sequence. Inserts to the beginning if the given
         * cue is not part of this sequence.
         */
        public void insertBefore(SFXCue item, SFXCue nCue) {
            lock (_cueItems_lock) {
                long pos = sequenceOf(item);
                uint nPos = (pos == -1) ? 0 : (uint)pos;
                insertAt(nPos, nCue);
            }
        }

        /**
         * Remove all copies of the given cue from this group.
         */
        public void removeElement(SFXCue c) {
            lock (_cueItems_lock) {
                foreach (uint i in cueItems.Keys) {     // check against every key
                    if (cueItems[i] == c) {             // we have found a copy of the current item...
                        insertAt(i, null);              // ... trigger the removal of this entry
                    }
                }
            }
        }

        /**
         * Check whether the given cue is a direct member of this group. (Does not check whether the cue is a grand-child (etc) of this element).
         */
        public bool contains(SFXCue c) {
            return children.Contains(c);
        }

        /**
         * Fast clear of all children of this group.
         */
        public void clear() {
            lock (_cueItems_lock) {
                cueItems.Clear();
                structureChanged();
                onRemoveChild.triggerEvent("", null);
            }
        }

        /**
         * Switch the grouping mode of this CueGroup - acts as a toggle between Collection and Sequence. Unknown types will
         * not be modified.
         */
        public void switchMode() {
            switch (_Type) {
                case GroupElementType.COLLECTION:
                    Type = GroupElementType.SEQUENCE;
                    break;

                case GroupElementType.SEQUENCE:
                    Type = GroupElementType.COLLECTION;
                    break;
            }
        }

        #endregion

        #region Helper Methods

        /**
         * Helper method to identify the largest possible cue currently known for this group.
         */
        private uint getNextCue() {
            lock (_cueItems_lock) {
                return (cueItems.Count == 0) ? 0 : cueItems.Keys.Max() + 1;
            }
        }

        /**
         * Helper method to update the stored version of the cue, and trigger a notification for update of the UI.
         */
         private void structureChanged() {
            if (currentShow != null)
                currentShow.updateTable(this);      // update the DB structure of the table
            NotifyPropertyChanged("Length");        // changes to the list of cues can potentially change the Length of the cue, notify the UI
        }

        /**
         * Build the grouping structure for this cue group.
         */
        protected override SoundFX buildCue() {
            List<SoundFX> items = new List<SoundFX>();
            foreach (SFXCue cue in children) {
                if (cue == null) {
                    logger.Error("Null item found in cue group <" + CueID + ">");
                } else {
                    SoundFX fx = cue.source;
                    if (fx == null) {
                        logger.Error("Unable to load SoundFX from cue <" + cue.CueID + "> in group <" + CueID + ">");
                    } else {
                        items.Add(fx);
                    }
                }
            }
            switch (Type) {
                case GroupElementType.COLLECTION:
                    return new SoundFXCollection(items);
                case GroupElementType.SEQUENCE:
                    return new SoundFXSequence(items);
                default:
                    logger.Error("Unknown grouping type detected when building CueGroup SoundFX element.");
                    throw new UnsupportedAudioException("Unknown SFXCue grouping type.");
            }
        }

        #endregion
    }
}
