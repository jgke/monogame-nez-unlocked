using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameImpl.Support {
    public class Savefile {
        public static readonly string gamedir = "GameName";
        public static string GetSaveDirectory() {
            PlatformID platform = Environment.OSVersion.Platform;
            switch (platform) {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyDocuments
                        ),
                        "SavedGames",
                        gamedir
                    );
                default:
                    string osConfigDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                    if (String.IsNullOrEmpty(osConfigDir)) {
                        osConfigDir = Environment.GetEnvironmentVariable("HOME");
                        if (String.IsNullOrEmpty(osConfigDir)) {
                            return "."; // Oh well.
                        }
                        osConfigDir += "/.local/share";
                    }
                    return Path.Combine(osConfigDir, gamedir);
            }
        }
    }
}
