namespace MP3toMP4
{
    /// <summary>
    /// Log files under %APPDATA%\SweWolfSoftware\&lt;app&gt;\Logs\:
    ///   convert_yyyyMMdd_HHmmss.log  one per conversion: run details plus FFmpeg's full output
    ///   FFmpeg_calls.log             one line per conversion: "yyyy-MM-dd HH:mm:ss&lt;TAB&gt;command line"
    ///
    /// Shared with the sibling apps by copying this whole file. When copying, change only the
    /// namespace and AppFolderName. When changing this file, add a line to the history below,
    /// so the copies in the other apps can be compared with it.
    ///
    /// History:
    ///   2026-09-27  Created: conversion log path moved here from Form1; added FFmpeg_calls.log.
    ///   2026-09-27  FFmpeg_calls.log: over 0.5 MB, cut down to its newest 0.25 MB.
    ///   2026-09-27  convert_*.log files older than 30 days are deleted (at start-up and once a day),
    ///               only files whose name exactly matches convert_yyyyMMdd_HHmmss.log.
    ///   2026-09-27  Added FFmpegCallsLogFile (used by Help > Open FFmpeg Call Log);
    ///               LogFolder is also used by Help > Open Log Folder.
    /// </summary>
    internal static class LogFiles
    {
        private const string AppFolderName = "MP3toMP4";

        internal static string LogFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", AppFolderName, "Logs");

        internal static string FFmpegCallsLogFile => Path.Combine(LogFolder, "FFmpeg_calls.log");

        /// <summary>Creates the log folder if needed and returns a new, time-stamped conversion log path.</summary>
        internal static string NewConversionLogFile()
        {
            DeleteOldLogsOncePerDay();
            Directory.CreateDirectory(LogFolder);
            return Path.Combine(LogFolder, $"convert_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        }

        /// <summary>
        /// Appends one line to FFmpeg_calls.log, with the ffmpeg path quoted so the call can be copied
        /// and re-run in a terminal. Logging must never stop a conversion, so errors are ignored.
        /// </summary>
        internal static void AppendFFmpegCall(string ffmpeg, string ffmpegArgs)
        {
            try
            {
                Directory.CreateDirectory(LogFolder);
                string logFile = FFmpegCallsLogFile;
                string commandLine = $"\"{ffmpeg}\" {ffmpegArgs}".Replace("\r", "").Replace("\n", " ");
                File.AppendAllText(logFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t{commandLine}\r\n", System.Text.Encoding.UTF8);
                TruncateToNewestHalf(logFile, FFmpegCallsMaxBytes);
            }
            catch { }
        }

        private const long FFmpegCallsMaxBytes = 512 * 1024;
        private const int ConversionLogMaxAgeDays = 30;

        // Exactly the names NewConversionLogFile creates: convert_yyyyMMdd_HHmmss.log
        private static readonly System.Text.RegularExpressions.Regex ConversionLogNameRegex =
            new(@"^convert_[0-9]{8}_[0-9]{6}\.log$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        private static readonly object CleanupLock = new();
        private static DateTime _lastCleanupDate = DateTime.MinValue;

        /// <summary>
        /// Deletes convert_*.log files older than 30 days, on a background thread. Call it at start-up;
        /// NewConversionLogFile also calls it, so an app that runs for weeks still cleans up, without a
        /// timer. Runs at most once per local calendar day. Errors are ignored.
        /// </summary>
        internal static void DeleteOldLogsOncePerDay()
        {
            lock (CleanupLock)
            {
                if (_lastCleanupDate == DateTime.Today) return;
                _lastCleanupDate = DateTime.Today;
            }

            Task.Run(() =>
            {
                try
                {
                    string folder = Path.GetFullPath(LogFolder);
                    if (!Directory.Exists(folder)) return;
                    DateTime cutoff = DateTime.Now.AddDays(-ConversionLogMaxAgeDays);
                    foreach (string file in Directory.EnumerateFiles(folder, "convert_*.log"))
                    {
                        try
                        {
                            // Double check before deleting: the search pattern also matches longer
                            // extensions (".logx") and 8.3 short names, so accept only the exact name
                            // NewConversionLogFile creates, directly in the log folder.
                            string fullPath = Path.GetFullPath(file);
                            if (!string.Equals(Path.GetDirectoryName(fullPath), folder, StringComparison.OrdinalIgnoreCase))
                                continue;
                            if (!ConversionLogNameRegex.IsMatch(Path.GetFileName(fullPath)))
                                continue;

                            if (File.GetLastWriteTime(fullPath) < cutoff) File.Delete(fullPath);
                        }
                        catch { } // e.g. open in an editor: try again next time
                    }
                }
                catch { }
            });
        }

        /// <summary>
        /// When the file is larger than maxBytes, keeps only the newest whole lines that fit in half of it,
        /// so the file isn't rewritten on every call. Writes a temporary file first and then replaces the
        /// log, so a failure never leaves a half-written log behind.
        /// </summary>
        private static void TruncateToNewestHalf(string file, long maxBytes)
        {
            if (new FileInfo(file).Length <= maxBytes) return;

            string[] lines = File.ReadAllLines(file, System.Text.Encoding.UTF8);
            long budget = maxBytes / 2, size = 0;
            int first = lines.Length;
            while (first > 0)
            {
                size += System.Text.Encoding.UTF8.GetByteCount(lines[first - 1]) + 2; // + CRLF
                if (size > budget) break;
                first--;
            }

            string temp = file + ".tmp";
            File.WriteAllLines(temp, lines[first..], System.Text.Encoding.UTF8);
            File.Move(temp, file, overwrite: true);
        }
    }
}
