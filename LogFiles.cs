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
    /// </summary>
    internal static class LogFiles
    {
        private const string AppFolderName = "MP3toMP4";

        internal static string LogFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", AppFolderName, "Logs");

        /// <summary>Creates the log folder if needed and returns a new, time-stamped conversion log path.</summary>
        internal static string NewConversionLogFile()
        {
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
                string commandLine = $"\"{ffmpeg}\" {ffmpegArgs}".Replace("\r", "").Replace("\n", " ");
                File.AppendAllText(Path.Combine(LogFolder, "FFmpeg_calls.log"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t{commandLine}\r\n", System.Text.Encoding.UTF8);
            }
            catch { }
        }
    }
}
