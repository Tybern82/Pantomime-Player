using SFXEngine.AudioEngine;
using SQLite;
using System;

namespace SFXPlayer.Cues {
    /**
     * Cue used to access a registered sound effect. 
     */
    public class RegisteredEffectCue : SFXCue {

        #region Property Definitions

        /**
         * Thread Locks
         */
        private object _RegisteredEffectCue_lock = new object();

        /**
         * The identifier of the RegisteredEffect accessed by this cue.
         */
        private uint _SourceID;
        [Indexed] public uint SourceID {
            get { lock (_RegisteredEffectCue_lock) { return _SourceID; } }
            set {
                lock (_RegisteredEffectCue_lock) {
                    logger.Debug("Setting SourceID for cue <" + CueID + ">: [" + value + "]");
                    _SourceID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Used to access the actual RegisteredEffect being used in this cue. Will return 'null' if the cue is invalid
         * the current show has not yet been set.
         */
        [Ignore] public RegisteredEffect Effect {
            get { lock (_RegisteredEffectCue_lock) { return (currentShow == null) ? null : currentShow.getRegisteredEffect(_SourceID); } }
            set {
                lock (_RegisteredEffectCue_lock) {
                    if (value == null) {
                        string msg = "Attempting to set new Effect in cue <" + CueID + "> to a null value.";
                        logger.Error(msg);
                        throw new ArgumentNullException("RegisteredEffectCue.Effect", msg);
                    }
                    logger.Debug("Setting source directly for cue <" + CueID + ">");
                    this.SourceID = value.SourceID;
                    // Notification is triggered by setting the SourceID.
                    // NotifyPropertyChanged();
                }
            }
        } 

        /**
         * Override the Length to retrieve the details from the underlying RegisteredEffect. If the cue is currently invalid
         * (show not set, or cue not found in the current show) this will access the underlying stored Length. Changes will
         * also be made to this underlying length but can be retrieved only when the cue becomes invalid.
         * Base set will call the NotifyPropertyChanged event.
         */
        public override TimeSpan Length {
            get {
                lock (_RegisteredEffectCue_lock) {
                    RegisteredEffect e = Effect;
                    return (e != null) ? e.Length : base.Length;
                }
            }

            set { lock (_RegisteredEffectCue_lock) { base.Length = value; } }
        }

        /**
         * Override the Name to retrieve the Filename of the associated effect (unless the name has been overriden by a direct
         * setting).
         */
        public override String Name {
            get {
                lock (_RegisteredEffectCue_lock) {
                    string _result = base.Name;
                    if (_result == null) {
                        RegisteredEffect eff = Effect;
                        _result = (eff != null) ? eff.Filename : "";
                    }
                    return _result;
                }
            }

            set { lock (_RegisteredEffectCue_lock) { base.Name = value; } }
        }

        #endregion

        #region Class Methods
        /**
         * Retrieve the cue from the RegisteredEffect.
         */
        protected override SoundFX buildCue() {
            RegisteredEffect _result = Effect;
            if ((_result != null) && (_result.fx != null)) return _result.fx.dup();
            return null;
        }
        #endregion
    }
}
