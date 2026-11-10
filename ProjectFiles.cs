using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MP3toMP4
{
    /// <summary>
    /// Project files: the work in the main window saved as one JSON file, so it can be opened again.
    /// This file holds the parts that are the same in every app:
    ///   - the envelope every project file starts with ("app", "formatVersion", "appVersion"),
    ///     so a program can tell which app a JSON file belongs to from its first lines,
    ///   - file references that are stored both absolute and relative to the project file,
    ///   - reading (with the "app" check) and crash-safe writing,
    ///   - backup copies under %APPDATA%\SweWolfSoftware\&lt;app&gt;\Backups\, deleted after 30 days,
    ///   - checking a file extension typed in Settings.
    /// What a project contains is app-specific: a class that derives from ProjectEnvelope
    /// (ProjectData.cs in MP3toMP4).
    ///
    /// Shared with the sibling apps by copying this whole file. When copying, change the
    /// namespace, AppFolderName, AppTag, AppDisplayName and SuggestedExtensions. When changing
    /// this file, add a line to the history below, so the copies in the other apps can be
    /// compared with it.
    ///
    /// History:
    ///   2026-11-09  Created for File > Save Project / Open Project in MP3toMP4.
    /// </summary>
    internal static class ProjectFiles
    {
        private const string AppFolderName = "MP3toMP4";

        /// <summary>The value of "app", the first property of every project file of this app.</summary>
        internal const string AppTag = "SweWolf MP3 to MP4";

        /// <summary>The app's name in messages.</summary>
        private const string AppDisplayName = "MP3 to MP4";

        /// <summary>Raise this when a newer file can no longer be read correctly by an older app.</summary>
        internal const int FormatVersion = 1;

        internal const string DefaultExtension = ".json";

        /// <summary>The choices in Settings > Projects > File Extension (anything else can be typed).</summary>
        internal static readonly string[] SuggestedExtensions = [".json", ".mp3tomp4.json", ".mp3tomp4"];

        private const int BackupDays = 30;

        internal static string BackupFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", AppFolderName, "Backups");

        /// <summary>
        /// Indented, camelCase, and readable: letters like å, ä, ö and the quotes and backslashes
        /// of a path are written as they are, not as \uXXXX codes.
        /// </summary>
        internal static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        internal static string Serialize<T>(T data) where T : ProjectEnvelope =>
            JsonSerializer.Serialize(data, JsonOptions);

        /// <summary>
        /// True when the file starts like a JSON object: its first character that isn't white space
        /// is "{". Used to tell a project file from a media file without trusting the extension.
        /// </summary>
        internal static bool LooksLikeJson(string path)
        {
            try
            {
                using var stream = File.OpenRead(path);
                var buffer = new byte[64];
                int read = stream.Read(buffer, 0, buffer.Length);
                int i = read >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF ? 3 : 0;   // UTF-8 BOM
                while (i < read && buffer[i] is 0x20 or 0x09 or 0x0D or 0x0A) i++;   // space, tab, CR, LF
                return i < read && buffer[i] == (byte)'{';
            }
            catch { return false; }
        }

        /// <summary>The "app" value of a JSON file, or null when it has none or can't be read.</summary>
        internal static string? ReadAppTag(string path)
        {
            try
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions
                {
                    CommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                });
                return doc.RootElement.ValueKind == JsonValueKind.Object
                    && doc.RootElement.TryGetProperty("app", out var app)
                    && app.ValueKind == JsonValueKind.String
                    ? app.GetString()
                    : null;
            }
            catch { return null; }
        }

        /// <summary>
        /// Reads a project file. Returns null, with a message for the user in
        /// <paramref name="error"/>, when the file can't be read or belongs to something else.
        /// Properties this version doesn't know are ignored.
        /// </summary>
        internal static T? Load<T>(string path, out string error) where T : ProjectEnvelope
        {
            error = "";
            string text;
            try
            {
                text = File.ReadAllText(path);   // detects a byte order mark, if an editor added one
            }
            catch (Exception ex)
            {
                error = $"The file \"{path}\" could not be read.\n\n{ex.Message}";
                return null;
            }

            try
            {
                var data = JsonSerializer.Deserialize<T>(text, JsonOptions);
                if (data == null || !string.Equals(data.App, AppTag, StringComparison.OrdinalIgnoreCase))
                {
                    error = $"The file \"{path}\" is not a project file of {AppDisplayName}.";
                    return null;
                }
                return data;
            }
            catch (JsonException ex)
            {
                // Not JSON at all, or JSON with a mistake in it (it may have been edited by hand)
                error = ReadAppTag(path) == null && !text.Contains($"\"{AppTag}\"", StringComparison.OrdinalIgnoreCase)
                    ? $"The file \"{path}\" is not a project file of {AppDisplayName}."
                    : $"The project file \"{path}\" could not be read: it contains an error.\n\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Writes the file in one step: first to a temporary file next to it, which then replaces
        /// the old file. A crash or power cut during saving leaves the old file as it was.
        /// </summary>
        internal static void WriteAtomic(string path, string text)
        {
            string temp = path + ".tmp";
            try
            {
                File.WriteAllText(temp, text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                File.Move(temp, path, overwrite: true);
            }
            catch
            {
                try { if (File.Exists(temp)) File.Delete(temp); } catch { }
                throw;
            }
        }

        /// <summary>
        /// Copies a project file to the backup folder as "Name yyyy-MM-dd HH.mm.ext" (the time of
        /// the copy). Returns the copy's path, or null when it could not be made.
        /// </summary>
        internal static string? CreateBackup(string projectPath)
        {
            try
            {
                Directory.CreateDirectory(BackupFolder);
                SplitFileName(Path.GetFileName(projectPath), out string name, out string extension);
                string stamp = DateTime.Now.ToString("yyyy-MM-dd HH.mm");
                string target = Path.Combine(BackupFolder, $"{name} {stamp}{extension}");
                for (int n = 2; File.Exists(target); n++)
                    target = Path.Combine(BackupFolder, $"{name} {stamp} ({n}){extension}");
                File.Copy(projectPath, target);
                return target;
            }
            catch { return null; }
        }

        /// <summary>"Song.mp3tomp4.json" is "Song" + ".mp3tomp4.json": a suggested extension is kept whole.</summary>
        private static void SplitFileName(string fileName, out string name, out string extension)
        {
            extension = SuggestedExtensions
                .OrderByDescending(e => e.Length)
                .FirstOrDefault(e => fileName.Length > e.Length && fileName.EndsWith(e, StringComparison.OrdinalIgnoreCase))
                ?? Path.GetExtension(fileName);
            extension = fileName[^extension.Length..];   // as written in the file name
            name = fileName[..^extension.Length];
        }

        // "... 2026-11-09 14.32.json" and "... 2026-11-09 14.32 (2).json": the names CreateBackup gives
        private static readonly Regex BackupName = new(@" \d{4}-\d{2}-\d{2} \d{2}\.\d{2}( \(\d+\))?\.", RegexOptions.Compiled);

        /// <summary>
        /// Deletes backup copies that were made more than 30 days ago, in the background. Only
        /// files with a name that CreateBackup gives are touched. The age is the time the copy was
        /// made (its creation time): a copy keeps the "modified" time of the original.
        /// </summary>
        internal static void DeleteOldBackupsInBackground()
        {
            Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists(BackupFolder)) return;
                    DateTime limit = DateTime.UtcNow.AddDays(-BackupDays);
                    foreach (string file in Directory.GetFiles(BackupFolder))
                    {
                        try
                        {
                            if (BackupName.IsMatch(Path.GetFileName(file)) && File.GetCreationTimeUtc(file) < limit)
                                File.Delete(file);
                        }
                        catch { }
                    }
                }
                catch { }
            });
        }

        /// <summary>
        /// Checks a file extension typed in Settings and returns it with a leading dot; blank gives
        /// the default (.json). Returns null, with a message in <paramref name="error"/>, when it
        /// can't be used in a file name.
        /// </summary>
        internal static string? NormalizeExtension(string typed, out string error)
        {
            error = "";
            string ext = typed.Trim();
            if (ext.Length == 0) return DefaultExtension;
            if (!ext.StartsWith('.')) ext = "." + ext;

            char[] invalid = Path.GetInvalidFileNameChars();
            if (ext.Length < 2 || ext.EndsWith('.') || ext.Any(c => char.IsWhiteSpace(c) || invalid.Contains(c)))
            {
                error = "The file extension is not valid. It cannot be empty, end with a period, or contain " +
                        "spaces or any of these characters: \\ / : * ? \" < > |";
                return null;
            }
            return ext;
        }

        /// <summary>True when the two paths are the same file (full paths, ignoring case).</summary>
        internal static bool SamePath(string? a, string? b)
        {
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
            try
            {
                return string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }
    }

    /// <summary>
    /// What every project file starts with. "app" comes first, so a program that wants to know
    /// which app a JSON file belongs to only has to read its first lines.
    /// </summary>
    internal class ProjectEnvelope
    {
        [JsonPropertyOrder(-3)] public string App { get; set; } = "";
        [JsonPropertyOrder(-2)] public int FormatVersion { get; set; }
        /// <summary>The version of the app that saved the file (information only).</summary>
        [JsonPropertyOrder(-1)] public string AppVersion { get; set; } = "";
    }

    /// <summary>
    /// A file used by a project: the path as it stands in the window, plus the same path relative
    /// to the project file's folder, so a project still opens after its folder was moved or renamed.
    /// </summary>
    internal sealed class ProjectFileRef
    {
        public string Path { get; set; } = "";
        public string? RelativePath { get; set; }

        internal static ProjectFileRef From(string? path, string? projectFolder)
        {
            var result = new ProjectFileRef { Path = path?.Trim() ?? "" };
            try
            {
                if (result.Path.Length > 0 && !string.IsNullOrEmpty(projectFolder)
                    && System.IO.Path.IsPathFullyQualified(result.Path))
                {
                    string relative = System.IO.Path.GetRelativePath(projectFolder, result.Path);
                    // On another drive there is no relative path: GetRelativePath returns the full one
                    if (!System.IO.Path.IsPathFullyQualified(relative)) result.RelativePath = relative;
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// The path to use: the saved one while it exists, else the relative one when that exists
        /// next to the project file, else the saved one (so the window shows what is missing).
        /// </summary>
        /// <param name="isFolderEnough">For a file that need not exist yet (an output file): its folder is what counts.</param>
        internal string Resolve(string? projectFolder, bool isFolderEnough = false)
        {
            string path = Path?.Trim() ?? "";
            if (path.Length == 0 || string.IsNullOrEmpty(RelativePath) || string.IsNullOrEmpty(projectFolder))
                return path;
            try
            {
                if (Exists(path, isFolderEnough)) return path;
                string moved = System.IO.Path.GetFullPath(RelativePath, projectFolder);
                if (Exists(moved, isFolderEnough)) return moved;
            }
            catch { }
            return path;
        }

        private static bool Exists(string path, bool isFolderEnough) =>
            isFolderEnough
                ? Directory.Exists(System.IO.Path.GetDirectoryName(path))
                : File.Exists(path);
    }
}
