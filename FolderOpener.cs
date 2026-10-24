using System.Diagnostics;

namespace MP3toMP4
{
    /// <summary>
    /// Opens a folder in SweWolf File Explorer when it can be found, else in the Windows Explorer.
    /// The EXE is looked for in the order of D:\Dev\CLAUDE.md ("Finding a sibling app"):
    ///   1. next to the running EXE,
    ///   2. ..\SwewolfFileExplorer\SwewolfFileExplorer.exe relative to the running EXE,
    ///   3. not found: the program Windows has for folders (no question, no path saved).
    ///
    /// Shared with the sibling apps by copying this whole file. When copying, change the
    /// namespace. When changing this file, add a line to the history below, so the copies in
    /// the other apps can be compared with it.
    ///
    /// History:
    ///   2026-10-23  Created: Open Folder goes to SweWolf File Explorer if it is found.
    ///   2026-10-23  Open(folder, fileToSelect): the file's path is sent so the explorer selects it.
    /// </summary>
    internal static class FolderOpener
    {
        private const string AppName = "SwewolfFileExplorer";

        /// <summary>The explorer's EXE, or null when it is not found.</summary>
        public static string? FindExplorer()
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
        /// Opens the folder, with <paramref name="fileToSelect"/> selected when it exists. SweWolf File Explorer gets the file's
        /// path (it opens the file's folder and selects the file); the Windows Explorer gets /select.
        /// </summary>
        public static void Open(string folder, string? fileToSelect = null)
        {
            bool hasFile = !string.IsNullOrEmpty(fileToSelect) && File.Exists(fileToSelect);
            string? exe = FindExplorer();
            try
            {
                if (exe != null)
                {
                    var info = new ProcessStartInfo { FileName = exe, UseShellExecute = false };
                    info.ArgumentList.Add(hasFile ? fileToSelect! : folder);
                    Process.Start(info);
                    return;
                }
            }
            catch (Exception)
            {
                // could not be started: the Windows Explorer below
            }
            var explorer = new ProcessStartInfo("explorer.exe");
            if (hasFile) explorer.ArgumentList.Add($"/select,{fileToSelect}");
            else explorer.ArgumentList.Add(folder);
            Process.Start(explorer);
        }
    }
}
