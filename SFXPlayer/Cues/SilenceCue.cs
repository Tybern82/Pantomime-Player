
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;

namespace SFXPlayer.Cues {
    /**
     * Cue used to provide a buffer in cue sequences. Used to both provide a break between cues, as well as to align sequences of 
     * cues to other effects.
     */
    public class SilenceCue : SFXCue {

        #region Property Definitions
        /**
         * Override the name to provide a default of "Silence" for this type of track.
         */
        public override string Name {
            get {
                string _result = base.Name;
                if (_result == null) _result = "Silence";
                return _result;
            }

            set { base.Name = value; }
        }
        #endregion

        #region Class Methods
        /**
         * Generate a track of silence of the correct length for this cue.
         */
        protected override SoundFX buildCue() {
            return new Silence(Length);
        }
        #endregion
    }
}
