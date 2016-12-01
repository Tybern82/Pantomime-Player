using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace SFXEngine.AudioEngine {
    /**
     * Interface to a Fadeable sound FX source. Provides the structure of methods necessary to manipulate the volume, and control
     * fading in and out of the source.
     */
    public interface FadeableSFX : ISampleProvider {
        
        /**
         * The base volume applied to the effect. This volume modifies the original source and is used as the basis of the fade methods.
         */
        double Volume { get; set; }


        /**
         * Determine whether this source will automatically fade out after a preset length of time (as specified in AutoFadeOutAt). This
         * parameter exists to allow the fade-out to be triggered immediately on the start of the track.
         */
        bool hasAutoFade { get; set; }

        /**
         * Duration of the fade-in over the beginning of this track. May be TimeSpan.Zero to specify that the source should start at
         * full volume (as modified by Volume).
         */
        TimeSpan FadeInDuration { get; set; }
        /**
         * Duration of the fade-out at the end of the track. May be TimeSpan.Zero to specify that the track will immediately terminate when
         * asked to fade-out. Equivalent of FadeInDuration for the end of the track.
         */
        TimeSpan FadeOutDuration { get; set; }
        /**
         * Point in the track at which to automatically trigger a fade-out. This parameter will only be applied if 'hasAutoFade' is also 
         * set to true, otherwise this parameter should be silently ignored.
         */
        TimeSpan AutoFadeOutAt { get; set; }


        /**
         * Trigger the start of a fade-in of this source.
         */
        void beginFadeIn();

        /**
         * Trigger the start of the fade-out of this source. When the source has completely faded, the output will terminate.
         */
        void beginFadeOut();
    }
}
