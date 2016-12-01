using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

using SQLite;
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;

namespace SFXPlayer {
    /**
     * Class storing a single effect file loaded into this show.
     */
    public class RegisteredEffect : INotifyPropertyChanged {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(RegisteredEffect));
        
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

        #region Property definitions

        private object _RegisteredEffect_lock = new object();

        /**
         * DB Identifier for this effect.
         */
        private uint _SourceID;
        [PrimaryKey, AutoIncrement] public uint SourceID {
            get { lock (_RegisteredEffect_lock) { return _SourceID; } }
            set {
                lock (_RegisteredEffect_lock) {
                    logger.Debug("Setting SourceID to [" + value + "]");
                    _SourceID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /**
         * Records the stored filename to the effect. This is stored relative to the base of the show file, meaning that 
         * (provided you use the default of storing the effects into the show folder) you can move the entire show file
         * to a new location and it should require no modification of the effects. Filename is generated from the underlying
         * effect only when first required, not immediately on loading.
         */
        private string _Filename = null;
        [Indexed] public string Filename {
            get {
                lock (_RegisteredEffect_lock) {
                    // We are currently unloaded, and there is an effect available to load details from...
                    if ((_Filename == null) && (fx != null))
                        _Filename = getRelativePath(fx.filename);   // Retrieve the relative path to the file from the current base...
                    return (_Filename != null) ? _Filename : "";    // We can still be 'null' if there is no current effect, just return "" in this case
                }
            }
            set {
                lock (_RegisteredEffect_lock) {
                    logger.Debug("Setting effect via Filename for <" + _SourceID + ">: [" + value + "]");
                    if (value != null) {
                        FileInfo nFile = new FileInfo(getAbsolutePath(value));          // Loaded effect contains the absolute path to the effect
                        try {
                            this.fx = nFile.Exists ? new SoundFile(nFile) : null;       // Only try to open the file if it actually exists...
                        } catch (UnsupportedAudioException) {
                            this.fx = null;                                             // The requested file is an unsupported format/not a sound file.
                        }
                        if (this.fx == null) logger.Warn("Loaded effect cannot be found...");
                        this._GroupingKey = (nFile.Name.Length != 0) ? ("" + nFile.Name[0]).ToUpper() : ""; // preload the GroupingKey since we already have a FileInfo with the required details.
                    } else {
                        this._fx = null;             // setting a null value for the filename - clear the effect and grouping key as well
                    }
                    // ??? - May need to trigger to update the 'Filename' property - a notification is already sent to update the fx
                    // NotifyPropertyChanged();
                }
                // Note - we don't actually set the _Filename here.... =) It will be cleared to 'null' by setting the fx element, 
                //        and will be automatically constructed in the correct format from the fx element the first time it is used.
            }
        }

        /**
         * Helper property used to assist in grouping effect names (by first letter of the name) in the GUI. This property is constructed (normally) 
         * the first time it is requested (meaning we don't needlessly calculate it unless required), except when setting via Filename (where we already
         * have the required value already). GroupingKey is specified as upper-case to ensure that 'A' and 'a' files map into the same grouping.
         * DB - Not stored in the database, since it can be recalculated from the filename.
         */
        private string _GroupingKey = "";
        [Ignore] public string GroupingKey {
            get {
                lock (_RegisteredEffect_lock) {
                    if ((_GroupingKey == null) && (fx != null) && (fx.filename != null)) {
                        // we need to calculate the key since it's not already here
                        FileInfo nFile = new FileInfo(fx.filename);     // load the information on the file (let the IO system work out what is just the name)
                        _GroupingKey = (nFile.Name.Length != 0)         // If the name in the effect is not empty.....
                            ? ("" + nFile.Name[0]).ToUpper()            // ... retrieve the first letter and convert to uppercase
                            : "";                                       // ... otherwise just use an empty string (will group all like together)
                    }
                    return (_GroupingKey != null) ? _GroupingKey : "";  // we may still be empty (if effect couldn't be detected)
                }
            }
        }

        /**
         * Helper property to retrieve the Length of the effect. Included to allow the GUI to retrieve all the required properties to display
         * from this class and allow it to setup suitable default values.
         * Default: 0:00:00
         */
        [Ignore] public TimeSpan Length { get { lock (_RegisteredEffect_lock) { return (fx == null) ? TimeSpan.Zero : fx.length; } } }

        /**
         * Helper property to retrieve a suitable display value for the caching mode.
         */
        [Ignore] public string CacheMode { get { lock (_RegisteredEffect_lock) { return (fx == null) ? "" : (fx.isCachable ? "Cached" : "Buffered"); } } }

        /**
         * Stores the current effect. Note that this property will not be stored directly, only indirectly via the filename.
         * Setting the effect automatically clears the Filename and GroupingKey backing to allow these to be automatically
         * recalculated from the new effect when next retrieved.
         */
        private SoundFile _fx;
        [Ignore] public SoundFile fx {
            get { lock (_RegisteredEffect_lock) { return _fx; } }
            set {
                lock (_RegisteredEffect_lock) {
                    logger.Debug("Setting new effect for <" + _SourceID + ">: [" + value + "]");
                    _fx = value;                // assign the new value to the effect...
                    _Filename = null;           // ... and clear the automatically generated items
                    _GroupingKey = null;
                    NotifyPropertyChanged();
                    // ??? - May need to trigger these as well as these change with the automatic generation.
                    // NotifyPropertyChanged("Length");
                    // NotifyPropertyChanged("CacheMode");
                    // NotifyPropertyChanged("GroupingKey");
                }
            }
        }

        #endregion

        #region Constructors

        /**
         * Just record the ID and effect - this will autogenerate other properties as required.
         */
        public RegisteredEffect(uint id, SoundFile fx) {
            this.SourceID = id;
            this.fx = fx;
        }

        /**
         * Default constructor sets an ID of 0 and no effect file.
         */
        public RegisteredEffect() : this(0, null) {}

        #endregion

        #region Class Methods

        /**
         * Generate a display string for this effect (suitable for use in logger) containing the only non-generated
         * information - the ID and the effect file currently loaded.
         */
        public override String ToString() {
            return SourceID + " - " + Filename;
        }

        /**
         * Deprecated method used by original system to store the data into a row in the standard UI list. This method has been
         * overriden by the use of ObjectListView and INotifyPropertyChanged to trigger the updates.
         */
        public object[] toRow() {
            return new object[] {
                SourceID, Filename, Length, CacheMode
            };
        }

        #endregion

        #region Static Methods

        /**
         * Retrieve the absolute path to the given filename - assuming a current base directory retrieved from the 
         * property in the engine. This property is updated when (or rather, just before) loading a new show file
         * to ensure all references are relative to the base of the current show. Where there is no base specified,
         * we assume that the filename is already an absolute filename.
         */
        public static string getAbsolutePath(string filename) {
            logger.Debug("Select absolute path for: [" + filename + "]");
            if (filename == null) return null;                      // if we have no filename, we can't do anything meaningful with it
            if (SFXEngineProperties.RelativeBase != null) {         // there is a recorded base to use for the relative calculation
                Uri fName = new Uri(filename, UriKind.Relative);    // get the filename (in relative format)
                if (fName.IsAbsoluteUri) {                              // we already have an absolute name, just return this
                    logger.Debug("Absolute Path: <" + filename + ">");
                    return filename;
                } else {                                                // we have to calculate the absolute name from the given base...
                    // Path.GetFullPath will resolve the relative references, after we combine the relative portion with the base path
                    string _result = Path.GetFullPath(Path.Combine(SFXEngineProperties.RelativeBase.FullName, filename));
                    logger.Debug("Absolute Path: <" + _result + ">");
                    return _result;
                }
            } else {
                return filename;        // IF there is no base path, we just return the original (assumed to be absolute)
                // Option: we could perform the same calculation as we would with a base path, using the current directory
            }
        }

        /**
         * Companion method to getAbsolutePath - retrieves the relative path, given a current base directory retrieved from
         * the property in the engine. IF we are unable to determine an appropriate base path, just return the absolute form
         * of the path to the caller.
         */
        public static string getRelativePath(string filename) {
            logger.Debug("Select relative path for: [" + filename + "]");
            if (filename == null) return null;                      // if there is no path, we can't do anything meaningful with it
            if (SFXEngineProperties.RelativeBase != null) {         // there is a recorded base to use in the relative calculation
                FileInfo fInfo = new FileInfo(filename);            // get a FileInfo of the current file (to ensure we get the full path information)
                // Get URI for both base dir and file
                Uri fName = new Uri(fInfo.FullName, UriKind.Absolute);  // generate an Absolute reference to the original...
                Uri bName = new Uri(SFXEngineProperties.RelativeBase.FullName + Path.DirectorySeparatorChar, UriKind.Absolute); // ... and the base path
                // Construct the relative URI
                Uri _result = bName.MakeRelativeUri(fName);
                // And return the (unescaped) relative path
                return Uri.UnescapeDataString(_result.ToString());  // we have to unescape the result (since we are going to display this to the user).
            } else {
                return filename;            // IF there is no base path, we just return the original (assumed to be absolute)
            }
        }

        #endregion
    }
}
