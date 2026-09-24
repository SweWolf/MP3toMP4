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
    }
}
