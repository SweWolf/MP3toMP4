using System.Diagnostics;

namespace MP3toMP4
{
    /// <summary>
    /// Plays a video in SweWolf Media Player when it can be found, else in the program Windows has for it.
    /// The EXE is looked for in the order of D:\Dev\CLAUDE.md ("Finding a sibling app"):
    ///   1. next to the running EXE,
    ///   2. ..\SwewolfMediaPlayer\SwewolfMediaPlayer.exe relative to the running EXE,
    ///   3. not found: the program Windows has for the file (no question, no message, no path saved).
    /// Also shows the Windows "Open with" dialog for a file.
    ///
    /// Shared with the sibling apps by copying this whole file. When copying, change the
    /// namespace. When changing this file, add a line to the history below, so the copies in
    /// the other apps can be compared with it.
    ///
    /// History:
    ///   2026-11-08  Created: Open File plays in SweWolf Media Player if it is found (modeled on the
    ///               explorer's Helpers\MediaPlayerLauncher.cs); OpenWithDialog(file) for the Windows "Open with" dialog.
    /// </summary>
    internal static class MediaPlayerLauncher
    {
        private const string AppName = "SwewolfMediaPlayer";

        /// <summary>The player's EXE, or null when it is not found.</summary>
        public static string? FindPlayer()
        {
            string here = AppContext.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(here, AppName + ".exe"),
                Path.GetFullPath(Path.Combine(here, "..", AppName, AppName + ".exe")),
            };
            return candidates.FirstOrDefault(File.Exists);
        }

        /// <summary>
        /// Plays the file: SweWolf Media Player gets the file's path as its argument (<c>--no-resume</c>: a video that
        /// has just been made starts from the beginning, not from a resume point saved for an older file with the same
        /// name). When the player is not found, or cannot be started, the file opens in the program Windows has for it.
        /// </summary>
        public static void Play(string file)
        {
            string? exe = FindPlayer();
            try
            {
                if (exe != null)
                {
                    var info = new ProcessStartInfo { FileName = exe, UseShellExecute = false };
                    info.ArgumentList.Add(file);
                    info.ArgumentList.Add("--no-resume");
                    Process.Start(info);
                    return;
                }
            }
            catch (Exception)
            {
                // could not be started: the program Windows has for the file, below
            }
            Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });
        }

        /// <summary>Shows the Windows "Open with" dialog for the file, where the user picks the program.</summary>
        public static void OpenWithDialog(string file)
        {
            Process.Start(new ProcessStartInfo { FileName = file, Verb = "openas", UseShellExecute = true });
        }
    }
}
