namespace MP3toMP4
{
    /// <summary>
    /// Persists application settings to flat files under
    /// %APPDATA%\SweWolfSoftware\MP3toMP4\.
    ///
    /// All I/O failures are silently swallowed so the host app is never affected.
    /// </summary>
    internal static class AppSettings
    {
        // -------------------------------------------------------------------------
        // Check for updates on startup: "Yes" (default) | "No"
        // -------------------------------------------------------------------------

        private static readonly string UpdateCheckFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "check-for-updates.txt");

        private static string? _cachedUpdateCheck;

        public static string CheckForUpdatesOnStartup
        {
            get
            {
                if (_cachedUpdateCheck != null) return _cachedUpdateCheck;
                try
                {
                    if (File.Exists(UpdateCheckFile))
                    {
                        string v = File.ReadAllText(UpdateCheckFile, System.Text.Encoding.UTF8).Trim();
                        if (v == "Yes" || v == "No")
                        {
                            _cachedUpdateCheck = v;
                            return v;
                        }
                    }
                }
                catch { }
                _cachedUpdateCheck = "Yes";
                return _cachedUpdateCheck;
            }
            set
            {
                _cachedUpdateCheck = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(UpdateCheckFile)!);
                    File.WriteAllText(UpdateCheckFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Default output folder: "" (default, meaning "same folder as the MP3 file")
        // -------------------------------------------------------------------------

        private static readonly string DefaultOutputFolderFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "default-output-folder.txt");

        private static string? _cachedDefaultOutputFolder;

        public static string DefaultOutputFolder
        {
            get
            {
                if (_cachedDefaultOutputFolder != null) return _cachedDefaultOutputFolder;
                try
                {
                    if (File.Exists(DefaultOutputFolderFile))
                    {
                        _cachedDefaultOutputFolder = File.ReadAllText(DefaultOutputFolderFile, System.Text.Encoding.UTF8).Trim();
                        return _cachedDefaultOutputFolder;
                    }
                }
                catch { }
                _cachedDefaultOutputFolder = "";
                return _cachedDefaultOutputFolder;
            }
            set
            {
                _cachedDefaultOutputFolder = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(DefaultOutputFolderFile)!);
                    File.WriteAllText(DefaultOutputFolderFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Default output format, i.e. the extension of the suggested output file:
        // "MP4" (default) | "MKV" | "MKV for Lossless" (MKV only for WAV/FLAC/AIFF sources)
        // -------------------------------------------------------------------------

        private static readonly string DefaultOutputFormatFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "default-output-format.txt");

        private static string? _cachedDefaultOutputFormat;

        public static string DefaultOutputFormat
        {
            get
            {
                if (_cachedDefaultOutputFormat != null) return _cachedDefaultOutputFormat;
                try
                {
                    if (File.Exists(DefaultOutputFormatFile))
                    {
                        string v = File.ReadAllText(DefaultOutputFormatFile, System.Text.Encoding.UTF8).Trim();
                        if (v == "MP4" || v == "MKV" || v == "MKV for Lossless")
                        {
                            _cachedDefaultOutputFormat = v;
                            return v;
                        }
                    }
                }
                catch { }
                _cachedDefaultOutputFormat = "MP4";
                return _cachedDefaultOutputFormat;
            }
            set
            {
                _cachedDefaultOutputFormat = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(DefaultOutputFormatFile)!);
                    File.WriteAllText(DefaultOutputFormatFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // What re-encoded audio is optimized for: "Quality" (default, AAC 384k) |
        // "Smaller File Size" (AAC 192k)
        // -------------------------------------------------------------------------

        private static readonly string OptimizeAudioForFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "optimize-audio-for.txt");

        private static string? _cachedOptimizeAudioFor;

        public static string OptimizeAudioFor
        {
            get
            {
                if (_cachedOptimizeAudioFor != null) return _cachedOptimizeAudioFor;
                try
                {
                    if (File.Exists(OptimizeAudioForFile))
                    {
                        string v = File.ReadAllText(OptimizeAudioForFile, System.Text.Encoding.UTF8).Trim();
                        if (v == "Quality" || v == "Smaller File Size")
                        {
                            _cachedOptimizeAudioFor = v;
                            return v;
                        }
                    }
                }
                catch { }
                _cachedOptimizeAudioFor = "Quality";
                return _cachedOptimizeAudioFor;
            }
            set
            {
                _cachedOptimizeAudioFor = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(OptimizeAudioForFile)!);
                    File.WriteAllText(OptimizeAudioForFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Action to take when a conversion finishes: "Play a Sound" (default, matching
        // FFmpegAssistant and SplitMediaFiles) | "Message Box" | "None"
        // -------------------------------------------------------------------------

        private static readonly string ActionWhenConversionFinishedFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "action-when-conversion-finished.txt");

        private static string? _cachedActionWhenConversionFinished;

        public static string ActionWhenConversionFinished
        {
            get
            {
                if (_cachedActionWhenConversionFinished != null) return _cachedActionWhenConversionFinished;
                try
                {
                    if (File.Exists(ActionWhenConversionFinishedFile))
                    {
                        string v = File.ReadAllText(ActionWhenConversionFinishedFile, System.Text.Encoding.UTF8).Trim();
                        if (v == "Play a Sound" || v == "Message Box" || v == "None")
                        {
                            _cachedActionWhenConversionFinished = v;
                            return v;
                        }
                    }
                }
                catch { }
                _cachedActionWhenConversionFinished = "Play a Sound";
                return _cachedActionWhenConversionFinished;
            }
            set
            {
                _cachedActionWhenConversionFinished = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ActionWhenConversionFinishedFile)!);
                    File.WriteAllText(ActionWhenConversionFinishedFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Sound file (Resources\*.wav, or a full path to a custom file) used when
        // ActionWhenConversionFinished == "Play a Sound"
        // -------------------------------------------------------------------------

        private static readonly string FinishedConversionSoundFileSetting = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "finished-conversion-sound.txt");

        private static string? _cachedFinishedConversionSound;

        public static string FinishedConversionSoundFile
        {
            get
            {
                if (_cachedFinishedConversionSound != null) return _cachedFinishedConversionSound;
                try
                {
                    if (File.Exists(FinishedConversionSoundFileSetting))
                    {
                        _cachedFinishedConversionSound = File.ReadAllText(FinishedConversionSoundFileSetting, System.Text.Encoding.UTF8).Trim();
                        return _cachedFinishedConversionSound;
                    }
                }
                catch { }
                _cachedFinishedConversionSound = "";
                return _cachedFinishedConversionSound;
            }
            set
            {
                _cachedFinishedConversionSound = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FinishedConversionSoundFileSetting)!);
                    File.WriteAllText(FinishedConversionSoundFileSetting, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Projects: the extension Save Project As suggests: ".json" (default), or any
        // other extension, with its leading dot (e.g. ".mp3tomp4.json", ".mp3tomp4")
        // -------------------------------------------------------------------------

        private static readonly string ProjectFileExtensionFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "project-file-extension.txt");

        private static string? _cachedProjectFileExtension;

        public static string ProjectFileExtension
        {
            get
            {
                if (_cachedProjectFileExtension != null) return _cachedProjectFileExtension;
                try
                {
                    if (File.Exists(ProjectFileExtensionFile))
                    {
                        string? v = ProjectFiles.NormalizeExtension(
                            File.ReadAllText(ProjectFileExtensionFile, System.Text.Encoding.UTF8), out _);
                        if (v != null)
                        {
                            _cachedProjectFileExtension = v;
                            return v;
                        }
                    }
                }
                catch { }
                _cachedProjectFileExtension = ProjectFiles.DefaultExtension;
                return _cachedProjectFileExtension;
            }
            set
            {
                _cachedProjectFileExtension = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ProjectFileExtensionFile)!);
                    File.WriteAllText(ProjectFileExtensionFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Projects: the folder Save Project As and Open Project start in:
        // "" (default, meaning "same folder as the audio file")
        // -------------------------------------------------------------------------

        private static readonly string DefaultProjectFolderFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "default-project-folder.txt");

        private static string? _cachedDefaultProjectFolder;

        public static string DefaultProjectFolder
        {
            get
            {
                if (_cachedDefaultProjectFolder != null) return _cachedDefaultProjectFolder;
                try
                {
                    if (File.Exists(DefaultProjectFolderFile))
                    {
                        _cachedDefaultProjectFolder = File.ReadAllText(DefaultProjectFolderFile, System.Text.Encoding.UTF8).Trim();
                        return _cachedDefaultProjectFolder;
                    }
                }
                catch { }
                _cachedDefaultProjectFolder = "";
                return _cachedDefaultProjectFolder;
            }
            set
            {
                _cachedDefaultProjectFolder = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(DefaultProjectFolderFile)!);
                    File.WriteAllText(DefaultProjectFolderFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Projects: save every change to the open project file by itself:
        // "Yes" (default) | "No". Also switched with File > Autosave.
        // -------------------------------------------------------------------------

        private static readonly string AutosaveProjectsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "autosave-projects.txt");

        private static string? _cachedAutosaveProjects;

        public static string AutosaveProjects
        {
            get
            {
                if (_cachedAutosaveProjects != null) return _cachedAutosaveProjects;
                try
                {
                    if (File.Exists(AutosaveProjectsFile))
                    {
                        string v = File.ReadAllText(AutosaveProjectsFile, System.Text.Encoding.UTF8).Trim();
                        if (v == "Yes" || v == "No")
                        {
                            _cachedAutosaveProjects = v;
                            return v;
                        }
                    }
                }
                catch { }
                _cachedAutosaveProjects = "Yes";
                return _cachedAutosaveProjects;
            }
            set
            {
                _cachedAutosaveProjects = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(AutosaveProjectsFile)!);
                    File.WriteAllText(AutosaveProjectsFile, value, System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // -------------------------------------------------------------------------
        // Projects: how many projects File > Recent Projects lists: 0 to 9
        // (default 4). 0 hides the menu item.
        // -------------------------------------------------------------------------

        private static readonly string RecentProjectsToShowFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "recent-projects-to-show.txt");

        private static int? _cachedRecentProjectsToShow;

        public static int RecentProjectsToShow
        {
            get
            {
                if (_cachedRecentProjectsToShow != null) return _cachedRecentProjectsToShow.Value;
                try
                {
                    if (File.Exists(RecentProjectsToShowFile)
                        && int.TryParse(File.ReadAllText(RecentProjectsToShowFile, System.Text.Encoding.UTF8).Trim(), out int v)
                        && v >= 0 && v <= ProjectFiles.MaxRecentProjects)
                    {
                        _cachedRecentProjectsToShow = v;
                        return v;
                    }
                }
                catch { }
                _cachedRecentProjectsToShow = 4;
                return 4;
            }
            set
            {
                value = Math.Clamp(value, 0, ProjectFiles.MaxRecentProjects);
                _cachedRecentProjectsToShow = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(RecentProjectsToShowFile)!);
                    File.WriteAllText(RecentProjectsToShowFile, value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        System.Text.Encoding.UTF8);
                }
                catch { /* never crash the host app */ }
            }
        }

        // The list of recent projects itself is kept by ProjectFiles.cs (recent-projects.txt).
    }
}
