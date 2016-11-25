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

    public class RegisteredEffect {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(RegisteredEffect));

        [PrimaryKey, AutoIncrement]
        public uint SourceID { get; set; }
        [Indexed]
        public string Filename {
            get {
                return (fx == null) ? "" : getRelativePath(fx.filename);
            }
            set {
                FileInfo nFile = new FileInfo(getAbsolutePath(value));
                this.fx = nFile.Exists ? new SoundFile(nFile) : null;
            }
        }

        [Ignore]
        public TimeSpan Length { get { return (fx == null) ? TimeSpan.Zero : fx.length; } }
        [Ignore]
        public string CacheMode { get { return (fx == null) ? "" : (fx.isCachable ? "Cached" : "Buffered"); } }
        [Ignore]
        public SoundFile fx { get; set; }

        public RegisteredEffect(uint id, SoundFile fx) {
            this.SourceID = id;
            this.fx = fx;
        }

        public RegisteredEffect() {
            this.SourceID = 0;
            this.fx = null;
        }

        public object[] toRow() {
            return new object[] {
                SourceID, Filename, Length, CacheMode
            };
        }

        public static string getAbsolutePath(string filename) {
            logger.Debug("Select absolute path for: [" + filename + "]");
            if (SFXEngineProperties.RelativeBase != null) {
                Uri fName = new Uri(filename, UriKind.Relative);
                if (fName.IsAbsoluteUri) {
                    logger.Debug("Absolute Path: <" + filename + ">");
                    return filename;
                } else {
                    string _result = Path.GetFullPath(Path.Combine(SFXEngineProperties.RelativeBase.FullName, filename));
                    logger.Debug("Absolute Path: <" + _result + ">");
                    return _result;
                }
            } else {
                return filename;
            }
        }

        public static string getRelativePath(string filename) {
            logger.Debug("Select relative path for: [" + filename + "]");
            if (SFXEngineProperties.RelativeBase != null) {
                FileInfo fInfo = new FileInfo(filename);
                // Get URI for both base dir and file
                Uri fName = new Uri(fInfo.FullName, UriKind.Absolute);
                Uri bName = new Uri(SFXEngineProperties.RelativeBase.FullName + Path.DirectorySeparatorChar, UriKind.Absolute);
                // Construct the relative URI
                Uri _result = bName.MakeRelativeUri(fName);
                // And return the (unescaped) relative path
                return Uri.UnescapeDataString(_result.ToString());
            } else {
                return filename;
            }
        }
    }
}
