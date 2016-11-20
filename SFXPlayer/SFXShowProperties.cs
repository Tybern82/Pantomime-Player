using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFXEngine.AudioEngine;
using SFXEngine.Events;
using SQLite;

namespace SFXPlayer {
    public class SFXShowProperties : SFXProperties {

        private SFXShowFile showFile;

        public SFXShowProperties(SFXShowFile show) {
            this.showFile = show;
        }

        public SFXShowProperties() : this(null) { }

        [Ignore]
        public PropertyEventRegister onChange { get; private set; } = new PropertyEventRegister();

        [PrimaryKey]
        public string Name {
            get { return _Name; }
            set {
                string oName = _Name;
                _Name = value;
                if (showFile != null) {
                    showFile.updateProperties(oName, this);
                }
                onChange.triggerEvent("Name", value);
            }
        }
        private string _Name = "New Show";

        private DateTime _Revision = DateTime.Now;
        public DateTime Revision {
            get { return _Revision; }
            set { _Revision = value; if (showFile != null) showFile.updateProperties(this); onChange.triggerEvent("Revision", value); }
        }

        private string _FXDesign = "Unknown";
        public string FXDesign {
            get { return _FXDesign; }
            set { _FXDesign = value;  if (showFile != null) showFile.updateProperties(this); onChange.triggerEvent("FXDesign", value); }
        }

        private string _Organisation = "Unknown";
        public string Organisation {
            get { return _Organisation; }
            set { _Organisation = value;  if (showFile != null) showFile.updateProperties(this); onChange.triggerEvent("Organisation", value); }
        }

        // TODO: Update these to ensure that set values don't go outside the SFXEngineProperties ranges
        private uint _MaxCachedSoundSize = SFXEngineProperties.MaxCachedSoundSize;
        public uint MaxCachedSoundSize {
            get { return _MaxCachedSoundSize; }
            set {
                _MaxCachedSoundSize = (value > SFXEngineProperties.MaxCachedSoundSize) ? SFXEngineProperties.MaxCachedSoundSize : value;
                if (showFile != null) showFile.updateProperties(this);
                onChange.triggerEvent("MaxCachedSoundSize", value);
            }
        }

        private TimeSpan _MaxCachedSoundFile = SFXEngineProperties.MaxCachedSoundFile;
        public TimeSpan MaxCachedSoundFile {
            get { return _MaxCachedSoundFile; }
            set {
                _MaxCachedSoundFile = (value > SFXEngineProperties.MaxCachedSoundFile) ? SFXEngineProperties.MaxCachedSoundFile : value;
                if (showFile != null) showFile.updateProperties(this);
                onChange.triggerEvent("MaxCachedSoundFile", value);
            }
        }

        private uint _AudioBufferSize = SFXEngineProperties.AudioBufferSize;
        public uint AudioBufferSize {
            get { return _AudioBufferSize; }
            set {
                _AudioBufferSize = (value > SFXEngineProperties.AudioBufferSize) ? SFXEngineProperties.AudioBufferSize : value;
                if (showFile != null) showFile.updateProperties(this);
                onChange.triggerEvent("AudioBufferSize", value);
            }
        }

        private double _MaxVolume = SFXEngineProperties.MaxVolume;
        public double MaxVolume {
            get { return _MaxVolume; }
            set {
                _MaxVolume = (value > SFXEngineProperties.MaxVolume) ? SFXEngineProperties.MaxVolume : value;
                if (showFile != null) showFile.updateProperties(this);
                onChange.triggerEvent("MaxVolume", value);
            }
        }

        public void load(SFXShowProperties props) {
            this._Name = props.Name;
            this._Revision = props.Revision;
            this._FXDesign = props.FXDesign;
            this._Organisation = props.Organisation;
            this._MaxCachedSoundSize = props.MaxCachedSoundSize;
            this._MaxCachedSoundFile = props.MaxCachedSoundFile;
            this._AudioBufferSize = props.AudioBufferSize;
            this._MaxVolume = props.MaxVolume;
        }

        public void update(SFXShowProperties props) {
            string oName = _Name;
            this._Name = props.Name;
            this._Revision = props.Revision;
            this._FXDesign = props.FXDesign;
            this._Organisation = props.Organisation;
            this._MaxCachedSoundSize = props.MaxCachedSoundSize;
            this._MaxCachedSoundFile = props.MaxCachedSoundFile;
            this._AudioBufferSize = props.AudioBufferSize;
            this._MaxVolume = props.MaxVolume;
            if (showFile != null) showFile.updateProperties(oName, this);
            onChange.triggerEvent("", this);
        }
    }
}
