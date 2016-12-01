using SFXEngine.AudioEngine;
using SQLite;
using System;
using System.ComponentModel;

namespace SFXPlayer.Cues {
    /**
     * Base class for SoundFX cues. Provides the underlying structure necessary to support the cues.
     */
    public class SFXCue : SFXEventSource, INotifyPropertyChanged {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SFXCue));

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        public void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "") {
            // logger.Info("Property changed [" + propertyName + "]");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property Definitions

        /**
         * Threading Locks
         */
        private object _SFXCue_lock = new object();     // general lock for accessing properties
        private object _parent_lock = new object();     // specific lock for manipulating the cached parent
        private object _source_lock = new object();     // specific lock for manipulating the playing source

        /**
         * Link to the current show file (necessary to load up certain cues from other data in the show).
         */
        private SFXShowFile _currentShow;
        [Ignore]
        public SFXShowFile currentShow {
            get { lock (_SFXCue_lock) { return _currentShow; } }
            set {
                lock (_SFXCue_lock) {
                    if (value != null) {
                        logger.Debug("Setting Show for cue <" + _CueID + "> to [" + value.showDetails.Name + "]");
                    }
                    _currentShow = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Retrieve the parent element containing this cue, or 'null' if this cue is the root of the 
         * current show. The cached value is removed whenever we are not sure if it may still be valid.
         */
        private CueGroup _parent = null;
        [Ignore]
        public CueGroup parent {
            get {
                lock (_parent_lock) {
                    if (_parent == null) {
                        // first time build of the parent element
                        if (currentShow == null) return null;   // we don't have a current show, we can't find anything
                        _parent = currentShow.rootCues.getDirectParent(this);  // start at the root and locate our direct parent element
                        if (_parent != null) {
                            // Add an event to remove the cached item if we are removed from the parent element (if we found one)
                            _parent.onRemoveChild.addEventTrigger(delegate (string prop, object nValue) {
                                lock (_parent_lock) {
                                    // clear out the cached parent value if either we have been removed, or we are not
                                    // sure what may have been removed (null is triggered when clearing the entire list, etc)
                                    if ((nValue == null) || (nValue == this)) _parent = null;
                                }
                            });
                        }
                    }
                    return _parent;
                }
            }
        }

        /**
         * Identifier for the cue.
         */
        private uint _CueID;
        [PrimaryKey, AutoIncrement]
        public uint CueID {
            get { lock (_SFXCue_lock) { return _CueID; } }
            set {
                lock (_SFXCue_lock) {
                    logger.Debug("Setting CueID to [" + value + "]");
                    _CueID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Identifier for this cue (used to more easily identify the required cues in the player).
         */
        private string _Name;
        [Indexed]
        public virtual string Name {
            get { lock (_SFXCue_lock) { return _Name; } }
            set {
                lock (_SFXCue_lock) {
                    logger.Debug("Setting Name for cue <" + _CueID + ">: [" + value + "]");
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Initial position in the underlying structure to begin playing.
         */
        private TimeSpan _SeekTo = TimeSpan.Zero;
        // []
        public TimeSpan SeekTo {
            get { lock (_SFXCue_lock) { return _SeekTo; } }
            set {
                lock (_SFXCue_lock) {
                    var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                    logger.Debug("Setting Seek Position for cue <" + _CueID + ">: [" + value + "] as [" + nValue + "]");
                    _SeekTo = nValue;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Duration to fade-in the start of the cue.
         */
        private TimeSpan _FadeInDuration = TimeSpan.Zero;
        // []
        public TimeSpan FadeInDuration {
            get { lock (_SFXCue_lock) { return _FadeInDuration; } }
            set {
                lock (_SFXCue_lock) {
                    var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                    logger.Debug("Setting Fade-In Length for cue <" + _CueID + ">: [" + value + "] as [" + nValue + "]");
                    _FadeInDuration = nValue;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Determines whether the automatic fade-out time applies to this cue. Note that this parameter only applies to whether the
         * AutoFadeOutAt element is valid - it does not affect either the FadeInDuration or FadeOutDuration, which will still trigger
         * when required.
         */
        private bool _hasAutoFade = false;
        // []
        public bool hasAutoFade {
            get { lock (_SFXCue_lock) { return _hasAutoFade; } }
            set {
                lock (_SFXCue_lock) {
                    logger.Debug("Setting Automatic Fade-out for cue <" + _CueID + ">: [" + value + "]");
                    _hasAutoFade = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Time to trigger the cue to automatically fade out (or '0:0:0' if no automatic fade-out is to be performed).
         */
        private TimeSpan _AutoFadeOutAt = TimeSpan.Zero;
        // []
        public TimeSpan AutoFadeOutAt {
            get { lock (_SFXCue_lock) { return _AutoFadeOutAt; } }
            set {
                lock (_SFXCue_lock) {
                    var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                    logger.Debug("Setting Automatic Fade-Out Time for cue <" + _CueID + ">: [" + value + "] as [" + nValue + "]");
                    _AutoFadeOutAt = nValue;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Duration to perform the fade-out over (if triggered).
         */
        private TimeSpan _FadeOutDuration = TimeSpan.Zero;
        // []
        public TimeSpan FadeOutDuration {
            get { lock (_SFXCue_lock) { return _FadeOutDuration; } }
            set {
                lock (_SFXCue_lock) {
                    var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                    logger.Debug("Setting Fade-Out Length for cue <" + _CueID + ">: [" + value + "] as [" + nValue + "]");
                    _FadeOutDuration = nValue;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Volume level for this cue.
         */
        private double _Volume = 1.0;
        // []
        public double Volume {
            get { lock (_SFXCue_lock) { return _Volume; } }
            set {
                lock (_SFXCue_lock) {
                    if (value < 0) _Volume = 0;
                    else if (value > SFXEngineProperties.getMaxVolume()) _Volume = SFXEngineProperties.getMaxVolume();
                    else _Volume = value;
                    logger.Debug("Setting Volume for cue <" + _CueID + ">: [" + value + "] as [" + _Volume + "]");
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Length of this cue.
         */
        private TimeSpan _Length = TimeSpan.Zero;
        // []
        public virtual TimeSpan Length {
            get { lock (_SFXCue_lock) { return _Length; } }
            set {
                lock (_SFXCue_lock) {
                    var nValue = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;
                    logger.Debug("Setting Track Length for cue <" + _CueID + ">: [" + value + "] as [" + nValue + "]");
                    _Length = nValue;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Used to retrieve a prepared SoundFX element ready for playing. This will retrieve the current data if one is already
         * in progress/prepared, or create a new effect element if one is not present. Using this element allows the player to
         * lazy-load the required effects as necessary. Since the source only represents the current playing state of the cue,
         * it is not stored in the DB.
         */
        private SoundFX _source = null;
        [Ignore]
        public SoundFX source {
            get {
                lock (_source_lock) {
                    if (_source == null) _source = prepareCue(buildCue());
                    return _source;
                }
            }
        }

        #endregion

        #region Class Methods

        /**
         * Clear out the prepared cue ready to start playing again. Note that we don't actually do anything to the existing sound
         * source, since we may be asked to play the same cue multiple times at once/overlapped. Leave stopping the cue up to the
         * actual player.
         */
        public void clearSource() {
            lock (_source_lock) {   // make sure we have a reference to the lock
                // Called to free the prepared source when the effect has completed playing
                this._source = null;
            }
        }

        /**
         * Helper method to be called by a parent when removing a cue from their list to ensue the cached parent is immediately removed.
         * While not required, this method ensures that an invalid parent cannot be retrieved in the period between when the parent
         * actually removes the child item, and when the automatic trigger is processed. (The automatic trigger exists to ensure the
         * cached value is cleared in the rare situation that the parent does not use this method, or where the parent is unable to
         * determine which cues need to be modified - as in the case of erasing the entire contents of a group).
         */
         public void clearParent() {
            lock (_parent_lock) {
                this._parent = null;
            }
        }

        /**
         * Check to determine whether the cue is currently active (we can't just check the source field, since this will
         * prepare a new cue if it is missing).
         */
        public bool isActive() {
            lock (_source_lock) {
                return (_source != null);
            }
        }

        /**
         * Prepares a SoundFX cue to play - sets the correct start position, caches the data and adjusts the volume.
         */
        protected SoundFX prepareCue(SoundFX cue) {
            lock (_source_lock) {
                // We cannot prepare a non-existant cue, just leave it as is
                if (cue == null) return null;

                // Seek to the correct position in the stream
                if (SeekTo != cue.currentTime) {
                    // we need to seek the underlying stream
                    if (cue.canSeek) {
                        // If the underlying stream can seek by itself, use that (it will potentially be faster
                        cue.seekTo(SeekTo);
                    } else if ((SeekTo - cue.currentTime) > TimeSpan.Zero) {
                        // if we can't seek the underlying stream, try to seek from the current position forward
                        // this is guaranteed to be valid if we are at the start of the underlying stream since
                        // we only have to keep reading forward until we get to the correct position
                        cue.seekForward(SeekTo - cue.currentTime);
                    } else {
                        // We have been asked to seek earlier than the current position (since this is normally the start of the stream,
                        // this should not happen in normal situations), but the stream is incapable of seeking in this fashion - log the
                        // error but don't immediately fail - allow the sound to play from the current position instead. 
                        // TODO: Add a global setting to trigger a fault on this condition instead
                        logger.Error("Unable to complete seek on a stream - stream is unseekable, and the requested time is earlier than the current position.");
                    }
                }

                // Make sure the cue is cached
                SoundFX cachedCue = cue.cache();

                // Add information to trigger the cue
                cachedCue.registerCascade(this);
                cachedCue.onStop.addEventTrigger(delegate (SoundFX source) {
                    clearSource();
                });

                // Add information to adjust the volume and fade-in/fade-out
                cachedCue.AutoFadeOutAt = AutoFadeOutAt;
                cachedCue.FadeInDuration = FadeInDuration;
                cachedCue.FadeOutDuration = FadeOutDuration;
                cachedCue.hasAutoFade = hasAutoFade;            // make sure we set this last to avoid the automatic override when setting AutoFadeOutAt
                cachedCue.Volume = Volume;

                return cachedCue;
            }
        }

        /**
         * Exists solely to be overriden by subclasses. This method is implemented by each class to generate the SoundFX
         * which is used to construct the audio cue for playback.
         */
        protected virtual SoundFX buildCue() { return null; }

        #endregion
    }
}
