using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MP3toMP4
{
    public partial class Form1 : Form
    {
        private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp"];
        private static readonly string[] VideoExtensions = [".mp4", ".mov", ".avi", ".mkv", ".webm", ".m4v"];

        // Open-dialog filter for images/videos, built from the lists above so it can't drift from them
        private static readonly string ImageVideoFilter = BuildImageVideoFilter();

        private static string BuildImageVideoFilter()
        {
            static string Entry(string name, IEnumerable<string> exts)
            {
                string patterns = string.Join(";", exts.Select(e => "*" + e));
                return $"{name} ({patterns})|{patterns}";
            }
            return string.Join("|",
                Entry("Image or video files", ImageExtensions.Concat(VideoExtensions)),
                Entry("Image files", ImageExtensions),
                Entry("Video files", VideoExtensions),
                "All files (*.*)|*.*");
        }

        private static bool IsVideoFile(string path) =>
            VideoExtensions.Contains(Path.GetExtension(path).ToLowerInvariant());

        private static readonly string SettingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "settings.json");

        private record ImageSegment(string FilePath, TimeSpan Duration);

        // Caption for the app's message boxes (same as the window title)
        internal const string AppTitle = "MP3 to MP4";

        private Process? _ffmpegProcess;
        private bool _cancelRequested;
        private string? _lastLogFile;
        private TimeSpan _mp3Length;
        private bool _mp3HasArtwork;
        // The Artist/"Title" text last filled in automatically; while the text box still holds
        // exactly this, the user hasn't edited it and it may be refreshed for a new MP3.
        private string _autoVideoText = "";

        public Form1()
        {
            InitializeComponent();

            btnBrowseForMp3File.Click += BtnBrowseForMp3File_Click;
            btnBrowsForImageFile.Click += BtnBrowseForImageFile_Click;
            cmdBrowseForMP4File.Click += CmdBrowseForMP4File_Click;
            btnConvert.Click += BtnConvert_Click;
            btnClear.Click += BtnClear_Click;
            txtMP3File.TextChanged += TxtMP3File_TextChanged;
            txtMP3File.Leave += (_, _) => UpdateMp3Length();

            txtMP3File.AllowDrop = true;
            cboImageFile.AllowDrop = true;
            txtMP4File.AllowDrop = true;

            txtMP3File.DragEnter += OnDragEnter;
            cboImageFile.DragEnter += OnDragEnter;
            txtMP4File.DragEnter += OnDragEnter;

            txtMP3File.DragDrop += TxtMP3File_DragDrop;
            cboImageFile.DragDrop += CboImageFile_DragDrop;
            txtMP4File.DragDrop += TxtMP4File_DragDrop;

            picImage.AllowDrop = true;
            picImage.DragEnter += OnDragEnter;
            picImage.DragDrop += CboImageFile_DragDrop;

            picImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            cboImageFile.TextChanged += (_, _) =>
            {
                string t = cboImageFile.Text;
                if (t.Length >= 2 && t[0] == '"' && t[^1] == '"')
                { cboImageFile.Text = t[1..^1]; return; }
                UpdateSinglePreview();
            };

            radFile.CheckedChanged += RadImageSource_CheckedChanged;
            radBlack.CheckedChanged += RadImageSource_CheckedChanged;
            radText.CheckedChanged += RadImageSource_CheckedChanged;
            label2.Click += (_, _) => radFile.Checked = true;   // radFile has no text of its own
            txtVideoText.TextChanged += (_, _) => UpdateSinglePreview();
            // WinForms quirk: resizing an editable ComboBox selects all its text. Anchored
            // Left|Right, it resizes with the window, so undo that unless the user is in it.
            cboImageFile.SizeChanged += (_, _) => BeginInvoke(() =>
            {
                if (!cboImageFile.Focused) cboImageFile.SelectionLength = 0;
            });
            txtMP4File.TextChanged += (_, _) =>
            {
                string t = txtMP4File.Text;
                if (t.Length >= 2 && t[0] == '"' && t[^1] == '"')
                    txtMP4File.Text = t[1..^1];
            };
            cboImageFile.KeyDown += CboImageFile_KeyDown;

            var pasteItem = new ToolStripMenuItem("Paste");
            pasteItem.Click += (_, _) => PasteClipboardImage();
            var ctxMenu = new ContextMenuStrip();
            ctxMenu.Items.Add(pasteItem);
            ctxMenu.Opening += (_, _) => pasteItem.Enabled = Clipboard.ContainsImage() || Clipboard.ContainsText();
            cboImageFile.ContextMenuStrip = ctxMenu;

            chkUseImageFileFromMp3File.CheckedChanged += ChkUseImageFileFromMp3File_CheckedChanged;
            chkTrimToRange.CheckedChanged += ChkTrimToRange_CheckedChanged;
            toolTip.SetToolTip(chkTrimToRange,
                "Unchecked: use the complete file.\n" +
                "Checked: shows Start at/End at (and Fade in/out) fields to trim the file to a specific range.");
            toolTip.SetToolTip(chkLyrics,
                "Copies the lyrics stored in the MP3 file's tags into the MP4 file, so players that " +
                "support it can display them.\n" +
                "The lyrics are not shown in the video itself.\n" +
                "Available only when the MP3 file contains the lyrics tag.");
            ShowToolTipWhenDisabled(chkLyrics);
            string startEndTooltip =
                "Optional - leave blank for the start/end of the file.\n" +
                "Time format: h:mm:ss.f, m:ss, or plain seconds (e.g. 1:23:45, 2:07, 90, 7.5) - '.' or ',' both work for the fraction.\n" +
                "Or a percentage of the MP3 file's length (e.g. 45%).";
            toolTip.SetToolTip(txtInputStart, startEndTooltip);
            toolTip.SetToolTip(txtInputEnd, startEndTooltip);

            btnOpenFile.Click += BtnOpenFile_Click;
            btnOpenFolder.Click += BtnOpenFolder_Click;
            btnOpenLogFile.Click += BtnOpenLogFile_Click;

            btnAdd.Click += BtnAdd_Click;
            btnMoveUp.Click += CmdMoveUp_Click;
            btnMoveDown.Click += CmdMoveDown_Click;
            btnDelete.Click += CmdDelete_Click;

            grdFiles.SelectionChanged += (_, _) => RefreshMultipleState();
            grdFiles.CellValidating += GrdFiles_CellValidating;
            grdFiles.RowsAdded += (_, _) => ScheduleRecalculateDurations();
            grdFiles.RowsRemoved += (_, _) => ScheduleRecalculateDurations();

            colFile.SortMode = DataGridViewColumnSortMode.NotSortable;
            colStart.SortMode = DataGridViewColumnSortMode.NotSortable;
            colDuration.SortMode = DataGridViewColumnSortMode.NotSortable;
            colDuration.DefaultCellStyle.ForeColor = SystemColors.GrayText;

            // The last image runs until the end of the audio, so its duration depends on the trim range too
            txtInputStart.TextChanged += (_, _) => ScheduleRecalculateDurations();
            txtInputEnd.TextChanged += (_, _) => ScheduleRecalculateDurations();
            chkTrimToRange.CheckedChanged += (_, _) => ScheduleRecalculateDurations();

            grdFiles.AllowDrop = true;
            grdFiles.DragEnter += GrdFiles_DragEnter;
            grdFiles.DragDrop += GrdFiles_DragDrop;
            grdFiles.CellFormatting += GrdFiles_CellFormatting;
            grdFiles.CellParsing += GrdFiles_CellParsing;
            grdFiles.CellEndEdit += GrdFiles_CellEndEdit;
            tabImage.SelectedIndexChanged += TabImage_SelectedIndexChanged;
            UpdateMultipleButtons();

            chkLyrics.Enabled = false;
            LoadSettings();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                txtMP3File.Text = args[1];
                PopulateImageCombo(args[1]);
                UpdateMp3Length();
            }
        }

        /// <summary>Resets every field to how it looks when the window is first opened.</summary>
        private void BtnClear_Click(object? sender, EventArgs e)
        {
            // Clearing the MP3 path also clears the image combo items and the image/lyrics checkboxes
            txtMP3File.Text = "";
            lblLength.Text = "";
            cboImageFile.Text = "";
            radFile.Checked = true;
            txtVideoText.Text = "";
            _autoVideoText = "";

            chkTrimToRange.Checked = false;
            txtInputStart.Text = "";
            txtInputEnd.Text = "";
            txtFadeInLength.Text = "0";
            txtFadeOutLength.Text = "0";

            grdFiles.CancelEdit();
            grdFiles.Rows.Clear();
            RefreshMultipleState();
            tabImage.SelectedIndex = 0;

            txtMP4File.Text = "";

            progressBar.Value = 0;
            lblEstimatedRemaining.Text = "Estimated remaining time: —";

            btnOpenFile.Enabled = false;
            btnOpenFolder.Enabled = false;
            btnOpenLogFile.Enabled = false;

            _lastLogFile = null;
        }

        private void BtnBrowseForMp3File_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Select MP3 or other audio file",
                Filter = "MP3 files (*.mp3)|*.mp3|" +
                         "Audio files (*.mp3;*.wav;*.flac;*.m4a;*.aac;*.ogg;*.opus;*.wma;*.aiff;*.aif)|*.mp3;*.wav;*.flac;*.m4a;*.aac;*.ogg;*.opus;*.wma;*.aiff;*.aif|" +
                         "All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            txtMP3File.Text = dlg.FileName;
            PopulateImageCombo(dlg.FileName);
            UpdateMp3Length();
        }

        private void BtnBrowseForImageFile_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Select image or video file",
                Filter = ImageVideoFilter
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            cboImageFile.Text = dlg.FileName;
        }

        private void CmdBrowseForMP4File_Click(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Title = "Save MP4 file as",
                Filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*",
                DefaultExt = "mp4",
                OverwritePrompt = false
            };

            if (!string.IsNullOrWhiteSpace(txtMP4File.Text))
                dlg.FileName = txtMP4File.Text;

            if (dlg.ShowDialog() != DialogResult.OK) return;

            txtMP4File.Text = dlg.FileName;
        }

        private static void OnDragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true ||
                e.Data?.GetDataPresent("UniformResourceLocatorW") == true ||
                e.Data?.GetDataPresent("UniformResourceLocator") == true)
                e.Effect = DragDropEffects.Copy;
        }

        private void TxtMP3File_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files || files.Length == 0) return;
            txtMP3File.Text = files[0];
            PopulateImageCombo(files[0]);
            UpdateMp3Length();
        }

        private async void CboImageFile_DragDrop(object? sender, DragEventArgs e)
        {
            var data = e.Data;
            if (data == null) return;

            // 1. File dragged from Explorer
            if (data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                cboImageFile.Text = files[0];
                return;
            }

            // Extract all data synchronously BEFORE any await — IDataObject can
            // become invalid once the drop operation unwinds from the call stack
            string? url = null;

            // 2. Direct URL formats (Chrome sets these when dragging an image)
            foreach (string fmt in new[] { "UniformResourceLocatorW", "UniformResourceLocator" })
            {
                if (data.GetData(fmt) is string s)
                {
                    url = s.Trim('\0').Trim();
                    if (!string.IsNullOrEmpty(url)) break;
                }
            }

            // 3. Fallback: parse src from the HTML Chrome puts on the drag object
            if (string.IsNullOrEmpty(url))
            {
                foreach (string fmt in new[] { "HTML Format", "text/html" })
                {
                    if (data.GetData(fmt) is string html)
                    {
                        var m = Regex.Match(html, @"src=""([^""]+\.(jpg|jpeg|png|gif|webp|bmp))", RegexOptions.IgnoreCase);
                        if (m.Success) { url = m.Groups[1].Value; break; }
                    }
                }
            }

            // 4. Nothing recognised — show diagnostic so we can identify the format
            if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                string available = string.Join("\n", data.GetFormats());
                MessageBox.Show($"No image URL found in drag data.\n\nAvailable formats:\n{available}",
                    AppTitle + " – drag diagnostic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await DownloadImageToTempAsync(url);
        }

        private async Task DownloadImageToTempAsync(string url)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                var bytes = await client.GetByteArrayAsync(url);

                string ext = Path.GetExtension(new Uri(url).LocalPath).ToLowerInvariant();
                if (!ImageExtensions.Contains(ext)) ext = ".jpg";

                string tempDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SweWolfSoftware", "MP3toMP4", "Temp");
                Directory.CreateDirectory(tempDir);

                string tempFile = Path.Combine(tempDir, $"drag_{DateTime.Now:yyyyMMdd_HHmmss}{ext}");
                await File.WriteAllBytesAsync(tempFile, bytes);

                cboImageFile.Text = tempFile;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not download image:\n{ex.Message}",
                    AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtMP4File_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files || files.Length == 0) return;
            txtMP4File.Text = files[0];
        }

        private void CboImageFile_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.V || !Clipboard.ContainsImage()) return;

            e.Handled = true;
            e.SuppressKeyPress = true;
            PasteClipboardImage();
        }

        private void PasteClipboardImage()
        {
            if (Clipboard.ContainsImage())
            {
                using var img = Clipboard.GetImage();
                if (img == null) return;

                string tempDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SweWolfSoftware", "MP3toMP4", "Temp");
                Directory.CreateDirectory(tempDir);

                string tempFile = Path.Combine(tempDir, $"paste_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                img.Save(tempFile, System.Drawing.Imaging.ImageFormat.Png);
                cboImageFile.Text = tempFile;
            }
            else if (Clipboard.ContainsText())
            {
                cboImageFile.Text = Clipboard.GetText();
            }
        }

        private void LoadImagePreview(string path, PictureBox target)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                target.Image = null;
                return;
            }
            try
            {
                target.Image?.Dispose();

                if (path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                {
                    // GDI+ doesn't support WebP; use WPF's WIC decoder which does
                    var bi = new System.Windows.Media.Imaging.BitmapImage(new Uri(path));
                    var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bi));
                    using var ms = new MemoryStream();
                    encoder.Save(ms);
                    ms.Position = 0;
                    target.Image = new Bitmap(ms);
                }
                else
                {
                    using var ms = new MemoryStream(File.ReadAllBytes(path));
                    target.Image = Image.FromStream(ms);
                }
            }
            catch
            {
                target.Image = null;
            }
        }

        /// <summary>
        /// The suggested output file for an MP3: same name with .mp4, in the default output
        /// folder from Settings, or next to the MP3 if no default folder is set.
        /// </summary>
        private static string DefaultMp4Path(string mp3)
        {
            string folder = AppSettings.DefaultOutputFolder;
            if (string.IsNullOrEmpty(folder))
                return Path.ChangeExtension(mp3, ".mp4");

            return Path.Combine(folder, Path.GetFileNameWithoutExtension(mp3) + ".mp4");
        }

        private void TxtMP3File_TextChanged(object? sender, EventArgs e)
        {
            // Strip surrounding quotes (Windows Explorer copies paths with quotes)
            string text = txtMP3File.Text;
            if (text.Length >= 2 && text[0] == '"' && text[^1] == '"')
            {
                txtMP3File.Text = text[1..^1]; // fires TextChanged again; let that call do the work
                return;
            }

            string mp3 = text.Trim();
            if (!string.IsNullOrEmpty(mp3))
                txtMP4File.Text = DefaultMp4Path(mp3);

            if (File.Exists(mp3))
                PopulateImageCombo(mp3);
            else
            {
                cboImageFile.Items.Clear();
                _mp3HasArtwork = false;
                chkUseImageFileFromMp3File.Checked = false;
                chkLyrics.Enabled = false;
                chkLyrics.Checked = false;
            }
            UpdateImageSourceState();
            RefreshDefaultVideoText();
        }

        private void PopulateImageCombo(string mp3Path)
        {
            cboImageFile.Items.Clear();
            _mp3HasArtwork = false;
            chkUseImageFileFromMp3File.Checked = false;
            chkLyrics.Enabled = false;
            chkLyrics.Checked = false;

            var dir = Path.GetDirectoryName(mp3Path);
            var baseName = Path.GetFileNameWithoutExtension(mp3Path);

            if (string.IsNullOrEmpty(dir)) return;

            var matches = Directory.GetFiles(dir)
                .Where(f => Path.GetFileNameWithoutExtension(f).Equals(baseName, StringComparison.OrdinalIgnoreCase)
                         && ImageExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                .ToArray();

            foreach (var match in matches)
                cboImageFile.Items.Add(match);

            if (matches.Length > 0)
                cboImageFile.SelectedIndex = 0;

            // Enable checkboxes based on what the MP3 contains
            _mp3HasArtwork = HasEmbeddedArtwork(mp3Path);
            chkLyrics.Enabled = HasLyrics(mp3Path);
            UpdateImageSourceState();
        }

        private static bool HasEmbeddedArtwork(string mp3Path)
        {
            try
            {
                using var file = TagLib.File.Create(mp3Path);
                return file.Tag.Pictures.Length > 0;
            }
            catch { return false; }
        }

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsFile)) return;
                var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(SettingsFile));
                if (doc.RootElement.TryGetProperty("includeLyrics", out var v))
                    chkLyrics.Checked = v.GetBoolean();
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile)!);
                File.WriteAllText(SettingsFile,
                    System.Text.Json.JsonSerializer.Serialize(new { includeLyrics = chkLyrics.Checked }));
            }
            catch { }
        }

        /// <summary>
        /// WinForms never shows a ToolTip on a disabled control; its mouse moves go to the parent
        /// instead. So show the control's tooltip from the parent while the pointer is over it.
        /// </summary>
        private void ShowToolTipWhenDisabled(Control control)
        {
            Control parent = control.Parent!;
            bool shown = false;
            parent.MouseMove += (_, e) =>
            {
                bool over = !control.Enabled && control.Bounds.Contains(e.Location);
                if (over && !shown)
                {
                    toolTip.Show(toolTip.GetToolTip(control), parent, control.Left, control.Bottom + 4);
                    shown = true;
                }
                else if (!over && shown)
                {
                    toolTip.Hide(parent);
                    shown = false;
                }
            };
            parent.MouseLeave += (_, _) =>
            {
                if (shown) { toolTip.Hide(parent); shown = false; }
            };
        }

        private static bool HasLyrics(string mp3Path)
        {
            try
            {
                using var file = TagLib.File.Create(mp3Path);
                return !string.IsNullOrWhiteSpace(file.Tag.Lyrics);
            }
            catch { return false; }
        }

        private static void WriteLyricsToMp4(string mp3Path, string mp4Path)
        {
            try
            {
                using var src = TagLib.File.Create(mp3Path);
                string lyrics = src.Tag.Lyrics;
                if (string.IsNullOrWhiteSpace(lyrics)) return;

                using var dst = TagLib.File.Create(mp4Path);
                dst.Tag.Lyrics = lyrics;
                dst.Save();
            }
            catch { }
        }

        private string? ExtractEmbeddedArtwork(string mp3Path)
        {
            try
            {
                using var file = TagLib.File.Create(mp3Path);
                var pic = file.Tag.Pictures.FirstOrDefault();
                if (pic == null) return null;

                string ext = pic.MimeType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/gif" => ".gif",
                    "image/bmp" => ".bmp",
                    _ => ".jpg"
                };

                string tempDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SweWolfSoftware", "MP3toMP4", "Temp");
                Directory.CreateDirectory(tempDir);

                string baseName = Path.GetFileNameWithoutExtension(mp3Path);
                string tempFile = Path.Combine(tempDir, $"embedded_{baseName}{ext}");
                File.WriteAllBytes(tempFile, pic.Data.Data);
                return tempFile;
            }
            catch { return null; }
        }

        private void ChkUseImageFileFromMp3File_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateImageSourceState();

            if (!chkUseImageFileFromMp3File.Checked) return;

            string mp3 = txtMP3File.Text.Trim();
            if (string.IsNullOrEmpty(mp3)) return;

            string? tempFile = ExtractEmbeddedArtwork(mp3);
            if (tempFile == null)
            {
                MessageBox.Show("Could not extract artwork from the MP3 file.",
                    AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkUseImageFileFromMp3File.Checked = false;
                return;
            }

            // Add to combo if not already there, then select it
            if (!cboImageFile.Items.Contains(tempFile))
                cboImageFile.Items.Insert(0, tempFile);

            cboImageFile.SelectedItem = tempFile;
        }

        // ----- Single Image tab: Image File / Black Screen / Text -----

        private void RadImageSource_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is not RadioButton { Checked: true }) return;   // each switch fires twice

            if (radText.Checked && txtVideoText.Text.Trim().Length == 0)
                RefreshDefaultVideoText(force: true);
            UpdateImageSourceState();
        }

        /// <summary>
        /// Enables the controls that belong to the chosen image source. While the MP3's own
        /// artwork is used, every other way of choosing an image is locked too (typing, Browse,
        /// and dropping onto the preview).
        /// </summary>
        private void UpdateImageSourceState()
        {
            bool fileMode = radFile.Checked;
            bool manualImage = fileMode && !chkUseImageFileFromMp3File.Checked;
            cboImageFile.Enabled = manualImage;
            btnBrowsForImageFile.Enabled = manualImage;
            picImage.AllowDrop = manualImage;
            chkUseImageFileFromMp3File.Enabled = fileMode && _mp3HasArtwork;
            txtVideoText.Visible = radText.Checked;
            UpdateSinglePreview();
        }

        private void UpdateSinglePreview()
        {
            var old = picImage.Image;
            if (radFile.Checked)
                LoadImagePreview(cboImageFile.Text.Trim(), picImage);
            else
                picImage.Image = RenderVideoTextPreview(radText.Checked ? GetVideoTextLines() : []);
            if (!ReferenceEquals(old, picImage.Image)) old?.Dispose();
        }

        /// <summary>
        /// Default text for Text mode: contributing artist (album artist if there is none),
        /// then the title in quotes on the next line. The file name stands in for a missing title.
        /// </summary>
        private void RefreshDefaultVideoText(bool force = false)
        {
            string mp3 = txtMP3File.Text.Trim();
            string def = "";
            if (File.Exists(mp3))
            {
                string artist = "", title = "";
                try
                {
                    using var f = TagLib.File.Create(mp3);
                    artist = f.Tag.JoinedPerformers?.Trim() ?? "";   // "Contributing artists" in Explorer
                    if (artist.Length == 0) artist = f.Tag.JoinedAlbumArtists?.Trim() ?? "";
                    title = f.Tag.Title?.Trim() ?? "";
                }
                catch { }
                if (title.Length == 0) title = Path.GetFileNameWithoutExtension(mp3);
                def = artist.Length > 0 ? $"{artist}\r\n\"{title}\"" : $"\"{title}\"";
            }

            // Only replace text the user hasn't edited
            if (force || txtVideoText.Text == _autoVideoText) txtVideoText.Text = def;
            _autoVideoText = def;
        }

        /// <summary>The text box's lines, trimmed, without leading/trailing blank lines.</summary>
        private string[] GetVideoTextLines()
        {
            var lines = txtVideoText.Text.Replace("\r", "").Split('\n').Select(l => l.Trim()).ToList();
            while (lines.Count > 0 && lines[0].Length == 0) lines.RemoveAt(0);
            while (lines.Count > 0 && lines[^1].Length == 0) lines.RemoveAt(lines.Count - 1);
            return [.. lines];
        }

        private const float VideoTextLineSpacing = 1.3f;

        // Segoe UI ships with every Windows since Vista; Arial is the fallback
        private static readonly (string File, string Family) VideoTextFont =
            File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "segoeui.ttf"))
                ? (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "segoeui.ttf"), "Segoe UI")
                : (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"), "Arial");

        /// <summary>
        /// Font size and vertical placement shared by the FFmpeg filter and the preview:
        /// the largest size (max 64 px) at which the widest line and the whole block fit
        /// within 90% of the video.
        /// </summary>
        private static (int FontPx, float LineHeight, float Top) LayoutVideoText(string[] lines)
        {
            using var bmp = new Bitmap(1, 1);
            using var g = Graphics.FromImage(bmp);
            int size = 64;
            for (; size > 16; size -= 2)
            {
                using var font = new Font(VideoTextFont.Family, size, GraphicsUnit.Pixel);
                float widest = lines.Max(l => l.Length == 0 ? 0 :
                    g.MeasureString(l, font, PointF.Empty, StringFormat.GenericTypographic).Width);
                if (widest <= VideoWidth * 0.9f && size * VideoTextLineSpacing * lines.Length <= VideoHeight * 0.9f)
                    break;
            }
            float lineHeight = size * VideoTextLineSpacing;
            return (size, lineHeight, (VideoHeight - lineHeight * lines.Length) / 2);
        }

        private static float VideoTextLineY(int line, (int FontPx, float LineHeight, float Top) layout) =>
            layout.Top + line * layout.LineHeight + (layout.LineHeight - layout.FontPx) / 2;

        /// <summary>
        /// One drawtext per line so every line is centred on its own. The text is read from
        /// line{i}.txt files (see <see cref="WriteVideoTextFiles"/>) in FFmpeg's working folder,
        /// which avoids escaping quotes, colons and backslashes in the filter; expansion=none
        /// keeps '%' literal.
        /// </summary>
        private static string BuildDrawTextFilter(string[] lines)
        {
            var layout = LayoutVideoText(lines);
            string font = VideoTextFont.File.Replace('\\', '/').Replace(":", "\\:");
            var filters = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0) continue;
                int y = (int)Math.Round(VideoTextLineY(i, layout));
                filters.Add($"drawtext=fontfile='{font}':textfile='line{i}.txt':expansion=none:" +
                            $"fontcolor=white:fontsize={layout.FontPx}:x=(w-text_w)/2:y={y}");
            }
            return string.Join(",", filters);
        }

        private static string WriteVideoTextFiles(string[] lines)
        {
            string dir = Path.Combine(Path.GetTempPath(), "MP3toMP4", "text_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            var utf8NoBom = new System.Text.UTF8Encoding(false);   // a BOM would be drawn as a glyph
            for (int i = 0; i < lines.Length; i++)
                if (lines[i].Length > 0)
                    File.WriteAllText(Path.Combine(dir, $"line{i}.txt"), lines[i], utf8NoBom);
            return dir;
        }

        /// <summary>What the Black Screen / Text video will look like, for the preview box.</summary>
        private static Bitmap RenderVideoTextPreview(string[] lines)
        {
            var bmp = new Bitmap(VideoWidth, VideoHeight);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);
            if (lines.Length == 0) return bmp;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            var layout = LayoutVideoText(lines);
            using var font = new Font(VideoTextFont.Family, layout.FontPx, GraphicsUnit.Pixel);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0) continue;
                float w = g.MeasureString(lines[i], font, PointF.Empty, StringFormat.GenericTypographic).Width;
                g.DrawString(lines[i], font, Brushes.White, (VideoWidth - w) / 2, VideoTextLineY(i, layout),
                             StringFormat.GenericTypographic);
            }
            return bmp;
        }

        private void ChkTrimToRange_CheckedChanged(object? sender, EventArgs e)
        {
            bool show = chkTrimToRange.Checked;
            lblInputStart.Visible = show;
            txtInputStart.Visible = show;
            label4.Visible = show;
            txtInputEnd.Visible = show;
            lblFadeIn.Visible = show;
            txtFadeInLength.Visible = show;
            lblFadeOut.Visible = show;
            txtFadeOutLength.Visible = show;
        }

        // Settings > Optimize Audio For. Quality = 384k, YouTube's recommended stereo upload bitrate:
        // YouTube re-encodes the audio anyway, so every generation of lossy compression before that
        // should lose as little as possible. Smaller File Size = 192k, about half the audio size.
        private static string AacCodec =>
            AppSettings.OptimizeAudioFor == "Smaller File Size" ? "-c:a aac -b:a 192k" : "-c:a aac -b:a 384k";

        /// <summary>
        /// MP3 and AAC play everywhere inside an MP4, so they're copied untouched. Anything else
        /// (WAV, FLAC, OGG, WMA, ...) is re-encoded to AAC: FFmpeg either refuses it in MP4 (WMA)
        /// or produces files many players can't handle (PCM, Vorbis), and WAV would be ~10x bigger.
        /// </summary>
        private static string UntrimmedAudioCodec(string audioPath) =>
            Path.GetExtension(audioPath).ToLowerInvariant() is ".mp3" or ".m4a" or ".aac"
                ? "-c:a copy"
                : AacCodec;

        private (string InputPrefix, string Codec, string Filter) GetAudioOptions(string mp3Path)
        {
            if (!chkTrimToRange.Checked)
                return ("", UntrimmedAudioCodec(mp3Path), "");

            TimeSpan totalDuration = GetMp3Duration(mp3Path);
            bool hasStart = TryParseRangeValue(txtInputStart.Text, totalDuration, out var start);
            bool hasEnd = TryParseRangeValue(txtInputEnd.Text, totalDuration, out var end);
            TryParseFade(txtFadeInLength.Text, out double fadeIn);
            TryParseFade(txtFadeOutLength.Text, out double fadeOut);

            bool hasTrim = hasStart || hasEnd;
            bool hasFade = fadeIn > 0 || fadeOut > 0;

            if (!hasTrim && !hasFade)
                return ("", UntrimmedAudioCodec(mp3Path), "");

            // Build the audio filter chain entirely in -af so FFmpeg controls the
            // endpoint precisely.  (Input-side -ss/-to combined with -af and AAC
            // encoding can flush 1–2 extra seconds of buffered frames after the
            // intended end point.)
            var filters = new List<string>();

            if (hasTrim)
            {
                string startArg = hasStart ? $"start={start.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)}" : "";
                string endArg = hasEnd ? $"end={end.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)}" : "";
                string trimArgs = string.Join(":", new[] { startArg, endArg }.Where(s => s.Length > 0));
                filters.Add($"atrim={trimArgs}");
                filters.Add("asetpts=PTS-STARTPTS");
            }

            if (hasFade)
            {
                TimeSpan effectiveStart = hasStart ? start : TimeSpan.Zero;
                TimeSpan effectiveEnd = hasEnd ? end : totalDuration;
                double effectiveSecs = Math.Max(0, (effectiveEnd - effectiveStart).TotalSeconds);

                if (fadeIn > 0)
                    filters.Add($"afade=t=in:st=0:d={fadeIn.ToString("F3", CultureInfo.InvariantCulture)}");
                if (fadeOut > 0)
                {
                    double st = Math.Max(0, effectiveSecs - fadeOut);
                    filters.Add($"afade=t=out:st={st.ToString("F3", CultureInfo.InvariantCulture)}:d={fadeOut.ToString("F3", CultureInfo.InvariantCulture)}");
                }
            }

            string filterArg = $"-af \"{string.Join(",", filters)}\"";
            return ("", AacCodec, filterArg);
        }

        private static bool TryParseFade(string? input, out double seconds)
        {
            seconds = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            string normalized = input.Trim().Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double val)) return false;
            if (val <= 0) return false;
            seconds = val;
            return true;
        }

        private async void BtnConvert_Click(object? sender, EventArgs e)
        {
            SaveSettings();

            string mp3 = txtMP3File.Text.Trim();
            string mp4 = txtMP4File.Text.Trim();
            bool multiMode = tabImage.SelectedTab == tabMultiple;

            // --- Common validation ---
            if (string.IsNullOrEmpty(mp3) || !File.Exists(mp3))
            {
                MessageBox.Show("Please select a valid MP3 file.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(mp4))
            {
                MessageBox.Show("Please specify an output MP4 file.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!mp4.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("The output file must have the .mp4 extension.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (GetInvalidPathCharError(mp4) is string pathError)
            {
                MessageBox.Show(pathError, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMP4File.Focus();
                return;
            }

            var (audioInputPrefix, audioCodec, audioFilter) = GetAudioOptions(mp3);

            // --- Mode-specific validation and arg building ---
            string ffmpegArgs, logInfo;
            string[]? videoTextLines = null;   // Text mode: written to temp files just before FFmpeg runs

            if (!multiMode)
            {
                string image = cboImageFile.Text.Trim();
                if (radBlack.Checked)
                {
                    ffmpegArgs = BuildBlackBackgroundArgs(mp3, mp4, audioInputPrefix, audioCodec, audioFilter);
                    logInfo = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP3   : {mp3}\n" +
                              $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Image : (black background, {VideoWidth}x{VideoHeight})\n" +
                              $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP4   : {mp4}\n";
                }
                else if (radText.Checked)
                {
                    string[] lines = GetVideoTextLines();
                    if (lines.Length == 0)
                    {
                        MessageBox.Show("Please enter the text to show in the video.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtVideoText.Focus();
                        return;
                    }
                    videoTextLines = lines;
                    ffmpegArgs = BuildBlackBackgroundArgs(mp3, mp4, audioInputPrefix, audioCodec, audioFilter,
                                                          BuildDrawTextFilter(lines));
                    logInfo = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP3   : {mp3}\n" +
                              $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Text  : {string.Join(" / ", lines)}\n" +
                              $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP4   : {mp4}\n";
                }
                else
                {
                    if (string.IsNullOrEmpty(image) || !File.Exists(image))
                    {
                        MessageBox.Show("Please select a valid image or video file.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    bool singleIsVideo = IsVideoFile(image);
                    ffmpegArgs = singleIsVideo
                        ? BuildSingleVideoArgs(image, mp3, mp4, audioInputPrefix, audioCodec, audioFilter)
                        : BuildSingleImageArgs(image, mp3, mp4, audioInputPrefix, audioCodec, audioFilter);
                    string singleLabel = singleIsVideo ? "Video" : "Image";
                    logInfo = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP3   : {mp3}\n" +
                              $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {singleLabel,-5} : {image}\n" +
                              $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP4   : {mp4}\n";
                }
            }
            else
            {
                TimeSpan mp3Duration = GetMp3Duration(mp3);
                if (!TryBuildSegments(GetEffectiveAudioLength(mp3Duration), out var segments, out string segError))
                {
                    MessageBox.Show(segError, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ffmpegArgs = BuildMultiImageArgs(mp3, mp4, segments, audioInputPrefix, audioCodec, audioFilter);
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP3   : {mp3}");
                for (int i = 0; i < segments.Count; i++)
                {
                    string kind = IsVideoFile(segments[i].FilePath) ? "Video" : "Image";
                    sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {kind} {i + 1}: {segments[i].FilePath} ({FormatTime(segments[i].Duration)})");
                }
                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MP4   : {mp4}");
                logInfo = sb.ToString();
            }

            if (!EnsureOutputFolderExists(mp4)) return;

            // --- Overwrite check ---
            if (File.Exists(mp4))
            {
                var answer = MessageBox.Show(
                    $"The output file already exists:\n{mp4}\n\nDo you want to overwrite it?",
                    AppTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (answer != DialogResult.Yes)
                {
                    txtMP4File.Focus();
                    txtMP4File.SelectAll();
                    return;
                }
            }

            string? ffmpeg = FindFFmpeg();
            if (ffmpeg == null)
            {
                MessageBox.Show(
                    "FFmpeg not found. Place ffmpeg.exe next to this application or add it to PATH.",
                    AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnConvert.Enabled = false;
            btnClear.Enabled = false;
            btnCancel.Enabled = true;
            btnOpenFile.Enabled = false;
            btnOpenFolder.Enabled = false;
            btnOpenLogFile.Enabled = false;
            _cancelRequested = false;
            progressBar.Value = 0;
            lblEstimatedRemaining.Text = "Estimated remaining time: —";

            string logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SweWolfSoftware", "MP3toMP4", "Logs");
            Directory.CreateDirectory(logDir);
            _lastLogFile = Path.Combine(logDir, $"convert_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            btnOpenLogFile.Enabled = true;

            TimeSpan totalDuration = GetMp3Duration(mp3);
            string? textWorkDir = null;

            try
            {
                // FFmpeg runs inside this folder so the text filter can use bare file names
                if (videoTextLines != null)
                    textWorkDir = WriteVideoTextFiles(videoTextLines);

                await RunFFmpegAsync(ffmpeg, ffmpegArgs, totalDuration, mp4, _lastLogFile, logInfo, textWorkDir);

                if (chkLyrics.Checked)
                    WriteLyricsToMp4(mp3, mp4);

                btnOpenFile.Enabled = File.Exists(mp4);
                btnOpenFolder.Enabled = true;
                progressBar.Value = progressBar.Maximum;
                lblEstimatedRemaining.Text = "Estimated remaining time: Done";
                NotifyConversionFinished(mp4);
            }
            catch (OperationCanceledException)
            {
                progressBar.Value = 0;
                lblEstimatedRemaining.Text = "Estimated remaining time: —";
            }
            catch (Exception ex)
            {
                btnOpenFolder.Enabled = !string.IsNullOrEmpty(Path.GetDirectoryName(mp4));
                MessageBox.Show("Conversion failed: " + ex.Message, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConvert.Enabled = true;
                btnClear.Enabled = true;
                btnCancel.Enabled = false;
                _ffmpegProcess = null;
                if (textWorkDir != null)
                    try { Directory.Delete(textWorkDir, recursive: true); } catch { }
            }
        }

        /// <summary>
        /// Returns an error message if <paramref name="path"/> contains a character that isn't
        /// allowed in a file or folder name on this OS (on Windows " &lt; &gt; | : * ? and control
        /// characters), otherwise null. FFmpeg only reports a cryptic exit code (-22) for those.
        /// (Same routine in all the SweWolf FFmpeg apps.)
        /// </summary>
        private static string? GetInvalidPathCharError(string path)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string root = Path.GetPathRoot(path) ?? "";
            foreach (string name in path[root.Length..].Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            {
                int i = name.IndexOfAny(invalid);
                if (i < 0) continue;

                string found = char.IsControl(name[i]) ? "an invisible control character" : $"the character \"{name[i]}\"";
                string shown = string.Join(" ", invalid.Where(c => !char.IsControl(c)
                    && c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar));
                return $"\"{name}\" contains {found}, which is not allowed in file and folder names."
                    + (shown.Length > 0 ? $"\n\nThese characters are not allowed: {shown}" : "");
            }
            return null;
        }

        /// <summary>
        /// FFmpeg can't create folders, so offer to create a missing output folder
        /// (same routine as in SplitMediaFiles). False = cancelled or failed.
        /// </summary>
        private static bool EnsureOutputFolderExists(string outputPath)
        {
            string? folder = Path.GetDirectoryName(Path.GetFullPath(outputPath));
            if (string.IsNullOrEmpty(folder) || Directory.Exists(folder)) return true;

            var answer = MessageBox.Show(
                $"The folder \"{folder}\" does not exist.\n\nDo you want to create it now?",
                AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (answer != DialogResult.OK) return false;

            try
            {
                Directory.CreateDirectory(folder); // creates any missing intermediate subfolders too
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not create the folder:\n{folder}\n\n{ex.Message}",
                    AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void UpdateMp3Length()
        {
            string mp3 = txtMP3File.Text.Trim();
            if (string.IsNullOrEmpty(mp3) || !File.Exists(mp3))
            {
                lblLength.Text = "";
                _mp3Length = TimeSpan.Zero;
                ScheduleRecalculateDurations();
                return;
            }

            TimeSpan duration = await Task.Run(() => GetMp3Duration(mp3));

            // Discard result if the path changed while we were reading
            if (txtMP3File.Text.Trim() != mp3) return;

            lblLength.Text = duration > TimeSpan.Zero ? $"Length: {FormatLengthDisplay(duration)}" : "";
            _mp3Length = duration;
            ScheduleRecalculateDurations();
        }

        /// <summary>
        /// The length of the audio that actually ends up in the MP4: the whole MP3, or just the
        /// Start at/End at range when "Trim to a Range" is checked.
        /// </summary>
        private TimeSpan GetEffectiveAudioLength(TimeSpan mp3Length)
        {
            if (!chkTrimToRange.Checked) return mp3Length;

            TimeSpan start = TryParseRangeValue(txtInputStart.Text, mp3Length, out var s) ? s : TimeSpan.Zero;
            TimeSpan end = TryParseRangeValue(txtInputEnd.Text, mp3Length, out var e) && e < mp3Length ? e : mp3Length;
            return end > start ? end - start : TimeSpan.Zero;
        }

        private static string FormatLengthDisplay(TimeSpan t)
        {
            int totalSeconds = (int)Math.Round(t.TotalSeconds, MidpointRounding.AwayFromZero);
            int hours = totalSeconds / 3600;
            int minutes = totalSeconds % 3600 / 60;
            int seconds = totalSeconds % 60;
            if (hours >= 1) return $"{hours}:{minutes:D2}:{seconds:D2}";
            if (minutes >= 1) return $"{minutes}:{seconds:D2}";
            return $"{seconds} s";
        }

        private static TimeSpan GetMp3Duration(string mp3)
        {
            try
            {
                using var f = TagLib.File.Create(mp3);
                return f.Properties.Duration;
            }
            catch { return TimeSpan.Zero; }
        }

        private static string BuildSingleImageArgs(string image, string mp3, string mp4,
            string audioInputPrefix, string audioCodec, string audioFilter)
        {
            var parts = new List<string>
            {
                "-y", "-loop 1",
                $"-i \"{FfmpegPath(image)}\"",
                $"{audioInputPrefix}-i \"{FfmpegPath(mp3)}\"",
                "-c:v libx264", "-tune stillimage", "-vf \"scale=1280:-2\"",
                "-r 30", "-pix_fmt yuv420p",
                audioCodec,
            };
            if (!string.IsNullOrEmpty(audioFilter)) parts.Add(audioFilter);
            parts.AddRange(["-shortest", "-movflags +faststart", $"\"{FfmpegPath(mp4)}\""]);
            return string.Join(" ", parts);
        }

        // Size of the generated video for Black Screen and Text (16:9)
        private const int VideoWidth = 1280;
        private const int VideoHeight = 720;

        /// <summary>
        /// Black Screen and Text modes: FFmpeg generates a solid black 16:9 video itself.
        /// <paramref name="videoFilter"/> draws on top of it (the text).
        /// </summary>
        private static string BuildBlackBackgroundArgs(string mp3, string mp4,
            string audioInputPrefix, string audioCodec, string audioFilter, string? videoFilter = null)
        {
            var parts = new List<string>
            {
                "-y",
                $"-f lavfi -i \"color=c=black:s={VideoWidth}x{VideoHeight}:r=30\"",
                // Full paths: in Text mode FFmpeg runs in a temp folder
                $"{audioInputPrefix}-i \"{FfmpegPath(Path.GetFullPath(mp3))}\"",
            };
            if (!string.IsNullOrEmpty(videoFilter)) parts.Add($"-vf \"{videoFilter}\"");
            parts.AddRange(
            [
                "-map 0:v", "-map 1:a",
                "-c:v libx264", "-tune stillimage", "-pix_fmt yuv420p",
                audioCodec,
            ]);
            if (!string.IsNullOrEmpty(audioFilter)) parts.Add(audioFilter);
            parts.AddRange(["-shortest", "-movflags +faststart", $"\"{FfmpegPath(Path.GetFullPath(mp4))}\""]);
            return string.Join(" ", parts);
        }

        private static string BuildSingleVideoArgs(string video, string mp3, string mp4,
            string audioInputPrefix, string audioCodec, string audioFilter)
        {
            var parts = new List<string>
            {
                "-y",
                $"-i \"{FfmpegPath(video)}\"",
                $"{audioInputPrefix}-i \"{FfmpegPath(mp3)}\"",
                // loop=-1 loops indefinitely; size=32767 is the frame buffer (~36 min at 30 fps)
                "-filter_complex \"[0:v]loop=loop=-1:size=32767[v]\"",
                "-map \"[v]\"", "-map 1:a",
                "-c:v libx264", "-r 30", "-pix_fmt yuv420p",
                audioCodec,
            };
            if (!string.IsNullOrEmpty(audioFilter)) parts.Add(audioFilter);
            parts.AddRange(["-shortest", "-movflags +faststart", $"\"{FfmpegPath(mp4)}\""]);
            return string.Join(" ", parts);
        }

        private static (int W, int H) GetImagePixelSize(string path)
        {
            if (IsVideoFile(path))
            {
                try
                {
                    // Derive ffprobe path from same folder as ffmpeg, else rely on PATH
                    string ffprobe = "ffprobe";
                    string? ffmpeg = FindFFmpeg();
                    if (ffmpeg != null)
                    {
                        string candidate = Path.Combine(Path.GetDirectoryName(ffmpeg)!, "ffprobe.exe");
                        if (File.Exists(candidate)) ffprobe = candidate;
                    }

                    var psi = new System.Diagnostics.ProcessStartInfo(ffprobe,
                        $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=s=x:p=0 \"{FfmpegPath(path)}\"")
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    using var proc = System.Diagnostics.Process.Start(psi);
                    if (proc != null)
                    {
                        string line = proc.StandardOutput.ReadLine() ?? "";
                        proc.WaitForExit();
                        var parts = line.Split('x');
                        if (parts.Length == 2 &&
                            int.TryParse(parts[0], out int w) &&
                            int.TryParse(parts[1], out int h))
                            return (w, h);
                    }
                }
                catch { }
                return (1280, 720);
            }

            try
            {
                using var img = System.Drawing.Image.FromFile(path);
                return (img.Width, img.Height);
            }
            catch
            {
                try
                {
                    var bmp = new System.Windows.Media.Imaging.BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(path);
                    bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    return (bmp.PixelWidth, bmp.PixelHeight);
                }
                catch { return (1280, 720); }
            }
        }

        private static string BuildMultiImageArgs(string mp3, string mp4, List<ImageSegment> segments,
            string audioInputPrefix, string audioCodec, string audioFilter)
        {
            // Derive target size from the first image; make both dimensions even (libx264 requirement)
            var (tw, th) = GetImagePixelSize(segments[0].FilePath);
            tw = (tw / 2) * 2;
            th = (th / 2) * 2;
            string sf = $"scale={tw}:{th}:force_original_aspect_ratio=decrease," +
                        $"pad={tw}:{th}:(ow-iw)/2:(oh-ih)/2,setsar=1";

            var sb = new System.Text.StringBuilder("-y ");

            foreach (var seg in segments)
            {
                if (IsVideoFile(seg.FilePath))
                    // Video: just open it — the loop filter in filter_complex handles looping + trim
                    sb.Append($"-i \"{FfmpegPath(seg.FilePath)}\" ");
                else
                    sb.Append($"-loop 1 -t {seg.Duration.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)} -i \"{FfmpegPath(seg.FilePath)}\" ");
            }

            sb.Append($"{audioInputPrefix}-i \"{FfmpegPath(mp3)}\" ");

            sb.Append("-filter_complex \"");
            for (int i = 0; i < segments.Count; i++)
            {
                if (IsVideoFile(segments[i].FilePath))
                {
                    string dur = segments[i].Duration.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture);
                    // loop=-1 loops indefinitely; trim cuts at the segment duration; setpts resets timestamps
                    sb.Append($"[{i}:v]loop=loop=-1:size=32767,trim=0:{dur},setpts=PTS-STARTPTS,{sf}[v{i}];");
                }
                else
                {
                    sb.Append($"[{i}:v]{sf}[v{i}];");
                }
            }
            for (int i = 0; i < segments.Count; i++)
                sb.Append($"[v{i}]");
            sb.Append($"concat=n={segments.Count}:v=1:a=0[v]\" ");

            sb.Append($"-map \"[v]\" -map \"{segments.Count}:a\" ");
            sb.Append($"-c:v libx264 -tune stillimage -r 30 -pix_fmt yuv420p {audioCodec} ");
            if (!string.IsNullOrEmpty(audioFilter)) sb.Append($"{audioFilter} ");
            sb.Append("-shortest -movflags +faststart ");
            sb.Append($"\"{mp4}\"");

            return sb.ToString();
        }

        /// <summary>
        /// Start Time is the single source of truth: each image lasts until the next row's Start
        /// Time, and the last one until the end of the audio. The Duration column is display-only.
        /// The images together therefore always cover exactly <paramref name="audioLength"/>, so
        /// -shortest never cuts the audio.
        /// </summary>
        private bool TryBuildSegments(TimeSpan audioLength, out List<ImageSegment> segments, out string error)
        {
            segments = [];
            error = "";

            var dataRows = GetGridDataRows();

            if (dataRows.Count == 0)
            {
                error = "Add at least one image or video to the Multiple Images/Videos list.";
                return false;
            }

            if (audioLength <= TimeSpan.Zero)
            {
                error = "Could not determine the length of the audio. Check the MP3 file and the Start at/End at values.";
                return false;
            }

            var starts = new List<TimeSpan>();
            for (int i = 0; i < dataRows.Count; i++)
            {
                string? file = dataRows[i].Cells[colFile.Index].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(file) || !File.Exists(file))
                {
                    error = $"Row {i + 1}: file not found:\n{file}";
                    return false;
                }

                // The first image always starts at the very beginning
                TimeSpan start = TimeSpan.Zero;
                if (i > 0)
                {
                    if (!TryParseTime(dataRows[i].Cells[colStart.Index].Value?.ToString(), out start))
                    {
                        error = $"Row {i + 1}: invalid or missing Start Time.";
                        return false;
                    }
                    if (start <= starts[i - 1])
                    {
                        error = $"Row {i + 1}: the Start Time must be later than the Start Time of row {i}.";
                        return false;
                    }
                }
                if (start >= audioLength)
                {
                    error = $"Row {i + 1}: the Start Time is at or after the end of the audio ({FormatTime(audioLength)}).";
                    return false;
                }
                starts.Add(start);
            }

            for (int i = 0; i < dataRows.Count; i++)
            {
                TimeSpan end = i < dataRows.Count - 1 ? starts[i + 1] : audioLength;
                string file = dataRows[i].Cells[colFile.Index].Value!.ToString()!.Trim();
                segments.Add(new ImageSegment(file, end - starts[i]));
            }

            return true;
        }

        private Task RunFFmpegAsync(string ffmpeg, string ffmpegArgs, TimeSpan totalDuration,
                                     string mp4, string logFile, string logInfo, string? workingDir = null)
        {
            return Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ffmpeg,
                    Arguments = ffmpegArgs,
                    WorkingDirectory = workingDir ?? "",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    StandardErrorEncoding = System.Text.Encoding.UTF8,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                _ffmpegProcess = process;

                var startTime = DateTime.UtcNow;

                process.Start();

                using var log = new StreamWriter(logFile, append: false, System.Text.Encoding.UTF8);
                log.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] FFmpeg: {ffmpeg}");
                log.Write(logInfo);
                log.WriteLine();

                string? line;
                while ((line = process.StandardError.ReadLine()) != null)
                {
                    log.WriteLine(line);
                    // Track encoding position via "time=HH:MM:SS.ms"
                    var tm = Regex.Match(line, @"time=(\d+):(\d+):(\d+\.\d+)");
                    if (tm.Success && totalDuration > TimeSpan.Zero)
                    {
                        double current = int.Parse(tm.Groups[1].Value) * 3600
                                       + int.Parse(tm.Groups[2].Value) * 60
                                       + double.Parse(tm.Groups[3].Value, CultureInfo.InvariantCulture);
                        double percent = Math.Min(current / totalDuration.TotalSeconds, 1.0);
                        int progressValue = (int)(percent * 100);

                        string remaining = "—";
                        if (percent > 0.01)
                        {
                            var elapsed = DateTime.UtcNow - startTime;
                            var left = TimeSpan.FromSeconds(elapsed.TotalSeconds / percent) - elapsed;
                            remaining = left.TotalSeconds < 60
                                ? $"{(int)left.TotalSeconds}s"
                                : $"{(int)left.TotalMinutes}m {left.Seconds}s";
                        }

                        Invoke(() =>
                        {
                            progressBar.Value = progressValue;
                            lblEstimatedRemaining.Text = $"Estimated remaining time: {remaining}";
                        });
                    }
                }

                process.WaitForExit();

                if (_cancelRequested)
                {
                    try { if (File.Exists(mp4)) File.Delete(mp4); } catch { }
                    throw new OperationCanceledException();
                }

                if (process.ExitCode != 0)
                    throw new Exception($"FFmpeg exited with code {process.ExitCode}.");
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // The start-up size is the smallest the layout looks right at. Taken here, after
            // DPI/font scaling, so it holds at any display scale.
            MinimumSize = Size;

            // Check for updates in the background, does not block startup
            if (AppSettings.CheckForUpdatesOnStartup == "Yes")
                _ = CheckForUpdatesAsync();
        }

        /// <summary>
        /// Plays a sound, shows a message box, or does nothing, as chosen in Settings.
        /// </summary>
        private void NotifyConversionFinished(string mp4)
        {
            switch (AppSettings.ActionWhenConversionFinished)
            {
                case "Play a Sound":
                    SoundLibrary.Play(AppSettings.FinishedConversionSoundFile);
                    break;
                case "None":
                    break;
                default: // "Message Box"
                    MessageBox.Show(
                        "Conversion completed successfully!\n\nOutput file:\n" + mp4,
                        AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            var currentVersion = System.Reflection.Assembly
                .GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

            var result = await GitHubUpdateChecker.CheckAsync(
                "SweWolf", "MP3toMP4", currentVersion);

            if (result is { IsUpdateAvailable: true } && !IsDisposed)
            {
                var answer = MessageBox.Show(this,
                    $"A new version is available: {result.LatestVersion}\n\n" +
                    $"You are running version {currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}.\n\n" +
                    $"Do you want to go to the download page?",
                    AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (answer == DialogResult.Yes)
                    Process.Start(new ProcessStartInfo(result.ReleasePageUrl) { UseShellExecute = true });
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Button? button = keyData switch
            {
                Keys.Control | Keys.O => btnOpenFile,
                Keys.Control | Keys.Shift | Keys.O => btnOpenFolder,
                Keys.Control | Keys.E => btnConvert,
                Keys.Alt | Keys.B => btnBrowseForMp3File,
                _ => null
            };

            if (button != null)
            {
                if (button.Enabled) button.PerformClick();
                return true;
            }

            if (keyData == Keys.F6)
            {
                txtMP3File.Focus();
                txtMP3File.SelectAll();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
            {
                var result = MessageBox.Show(
                    "Conversion in progress. If you close the application, the output file will be deleted.",
                    AppTitle,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }

                _cancelRequested = true;
                try { _ffmpegProcess.Kill(entireProcessTree: true); } catch { }

                string mp4 = txtMP4File.Text.Trim();
                try { if (File.Exists(mp4)) File.Delete(mp4); } catch { }
            }

            base.OnFormClosing(e);
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to cancel the conversion?",
                AppTitle,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.OK) return;

            _cancelRequested = true;
            try { _ffmpegProcess?.Kill(entireProcessTree: true); } catch { }
        }

        private void menuCreateShortcut_Click(object? sender, EventArgs e)
        {
            using var form = new CreateShortcutForm(Application.ExecutablePath);
            form.ShowDialog(this);
        }

        private void menuSettings_Click(object? sender, EventArgs e)
        {
            using var form = new SettingsForm();
            form.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using var form = new AboutForm();
            form.ShowDialog(this);
        }

        private bool _gridUpdating;
        private bool _recalcPending;

        private List<DataGridViewRow> GetGridDataRows() =>
            grdFiles.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).ToList();

        private void GrdFiles_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != colStart.Index) return;
            string? val = e.FormattedValue?.ToString();
            if (string.IsNullOrWhiteSpace(val)) return;
            if (!TryParseTime(val, out _))
            {
                e.Cancel = true;
                MessageBox.Show("The time is not valid. Enter it as H:MM:SS, M:SS, or plain seconds (e.g. 1:23:45, 2:07, 90, 7.5). " +
                                "'.' or ',' both work for the fraction.",
                    AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Queues <see cref="RecalculateDurations"/> to run once the current grid event has
        /// finished. Row add/remove events fire in the middle of DataGridView's own processing,
        /// where changing cell values or ReadOnly flags isn't safe.
        /// </summary>
        private void ScheduleRecalculateDurations()
        {
            if (_recalcPending || !IsHandleCreated) return;
            _recalcPending = true;
            BeginInvoke(() =>
            {
                _recalcPending = false;
                RecalculateDurations();
            });
        }

        /// <summary>
        /// Refreshes the display-only Duration column from the Start Times, and locks the first
        /// row's Start Time at 0:00. Uses the same rules as <see cref="TryBuildSegments"/>.
        /// </summary>
        private void RecalculateDurations()
        {
            // Mid-edit: the value being typed isn't committed yet. CellEndEdit schedules another run.
            if (_gridUpdating || grdFiles.IsCurrentCellInEditMode) return;

            var dataRows = GetGridDataRows();
            TimeSpan audioLength = GetEffectiveAudioLength(_mp3Length);

            _gridUpdating = true;
            try
            {
                for (int i = 0; i < dataRows.Count; i++)
                {
                    var startCell = dataRows[i].Cells[colStart.Index];
                    startCell.ReadOnly = i == 0;
                    if (i == 0 && startCell.Value?.ToString() != "0:00")
                        startCell.Value = "0:00";

                    string duration = "";
                    if (TryParseTime(startCell.Value?.ToString(), out var start))
                    {
                        if (i < dataRows.Count - 1)
                        {
                            if (TryParseTime(dataRows[i + 1].Cells[colStart.Index].Value?.ToString(), out var next) && next > start)
                                duration = FormatTime(next - start);
                        }
                        else if (audioLength <= TimeSpan.Zero)
                            duration = "(to the end)";
                        else if (audioLength > start)
                            duration = FormatTime(audioLength - start);
                    }
                    dataRows[i].Cells[colDuration.Index].Value = duration;
                }
            }
            finally
            {
                _gridUpdating = false;
            }
        }

        /// <summary>
        /// Parses "7", "0:07", "0:00:07" and fractional/comma variants ("7.5", "7,5") as a duration.
        /// </summary>
        private static bool TryParseTime(string? input, out TimeSpan result)
        {
            result = TimeSpan.Zero;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string normalized = input.Trim().Replace(',', '.');
            var parts = normalized.Split(':');

            static bool TryNum(string s, out double v) => double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);

            // Plain seconds
            if (parts.Length == 1 && TryNum(parts[0], out double secOnly) && secOnly >= 0)
            {
                result = TimeSpan.FromSeconds(secOnly);
                return true;
            }

            // M:SS  — two parts means minutes:seconds, not hours:minutes
            if (parts.Length == 2 && TryNum(parts[0], out double min) && TryNum(parts[1], out double sec) && min >= 0 && sec >= 0)
            {
                result = TimeSpan.FromSeconds(min * 60 + sec);
                return true;
            }

            // H:MM:SS — three parts
            if (parts.Length == 3 && TryNum(parts[0], out double hr) && TryNum(parts[1], out double mn) && TryNum(parts[2], out double sc) &&
                hr >= 0 && mn >= 0 && sc >= 0)
            {
                result = TimeSpan.FromSeconds(hr * 3600 + mn * 60 + sc);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Same as <see cref="TryParseTime"/>, but also accepts a trailing "%" (e.g. "45%",
        /// "45.5 %") meaning that percentage of <paramref name="totalDuration"/>. Deliberately does
        /// NOT rewrite the textbox to the resolved time - the percentage is meant to stay reusable
        /// across different MP3 files of different lengths, re-evaluated fresh each time.
        /// </summary>
        private static bool TryParseRangeValue(string? input, TimeSpan totalDuration, out TimeSpan result)
        {
            result = TimeSpan.Zero;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string trimmed = input.Trim();
            if (trimmed.EndsWith('%'))
            {
                string normalized = trimmed[..^1].Trim().Replace(',', '.');
                if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double percent) || percent < 0)
                    return false;

                result = TimeSpan.FromSeconds(totalDuration.TotalSeconds * percent / 100.0);
                return true;
            }

            return TryParseTime(trimmed, out result);
        }

        private static string FormatTime(TimeSpan t)
        {
            // Round (not truncate) to one decimal place, e.g. 7.25 → "0:07.3"
            t = TimeSpan.FromMilliseconds(Math.Round(t.TotalMilliseconds / 100, MidpointRounding.AwayFromZero) * 100);
            string frac = t.Milliseconds > 0
                ? $".{t.Milliseconds / 100}"
                : "";

            return t.TotalHours >= 1
                ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}{frac}"
                : $"{t.Minutes}:{t.Seconds:D2}{frac}";
        }

        private void GrdFiles_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            ScheduleRecalculateDurations();
            if (e.ColumnIndex != colStart.Index || e.RowIndex < 0) return;

            var dataRows = GetGridDataRows();
            if (dataRows.Count < 2) return;

            int editedListIdx = dataRows.FindIndex(r => r.Index == e.RowIndex);
            if (editedListIdx < 0) return;

            // Keep rows in Start Time order: the edited image moves together with its new time.
            // Durations aren't carried along - RecalculateDurations rebuilds them afterwards.
            var snapshot = dataRows.Select(r => (
                File: r.Cells[colFile.Index].Value?.ToString() ?? "",
                Start: r.Cells[colStart.Index].Value?.ToString() ?? "",
                T: TryParseTime(r.Cells[colStart.Index].Value?.ToString(), out var t) ? t : TimeSpan.MaxValue
            )).ToList();

            var sortedOrder = Enumerable.Range(0, snapshot.Count).OrderBy(i => snapshot[i].T).ToList();

            // Already sorted — nothing to do
            if (sortedOrder.SequenceEqual(Enumerable.Range(0, snapshot.Count))) return;

            // Rewrite rows in sorted order
            _gridUpdating = true;
            try
            {
                var sorted = sortedOrder.Select(i => snapshot[i]).ToList();
                for (int i = 0; i < sorted.Count; i++)
                {
                    dataRows[i].Cells[colFile.Index].Value = sorted[i].File;
                    dataRows[i].Cells[colStart.Index].Value = sorted[i].Start;
                }

                // Follow the edited row to its new position
                int newIdx = sortedOrder.IndexOf(editedListIdx);
                if (newIdx >= 0)
                {
                    grdFiles.CurrentCell = grdFiles.Rows[newIdx].Cells[0];
                    RefreshMultipleState();
                }
            }
            finally
            {
                _gridUpdating = false;
            }
        }

        private void GrdFiles_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != colFile.Index || e.Value is not string fullPath || string.IsNullOrEmpty(fullPath))
                return;

            // A relative path (typed while no MP3 was chosen) would be resolved against the
            // working directory, not the MP3 folder: show it as typed.
            if (!Path.IsPathFullyQualified(fullPath)) return;

            string? mp3Folder = Path.GetDirectoryName(txtMP3File.Text.Trim());
            if (string.IsNullOrEmpty(mp3Folder)) return;

            try
            {
                string relative = Path.GetRelativePath(mp3Folder, fullPath);
                if (!relative.StartsWith(".."))
                {
                    e.Value = relative;
                    e.FormattingApplied = true;
                }
            }
            catch { }
        }

        /// <summary>
        /// The File column stores full paths and only displays them relative to the MP3 folder
        /// (see <see cref="GrdFiles_CellFormatting"/>). Turns a typed or edited relative path
        /// ("one.jpg", "Images\one.jpg") back into a full path based on the MP3 folder.
        /// </summary>
        private void GrdFiles_CellParsing(object? sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex != colFile.Index || e.Value is not string typed) return;

            // "Copy as path" in Explorer adds quotes
            typed = typed.Trim().Trim('"').Trim();
            e.Value = typed;
            e.ParsingApplied = true;
            if (typed.Length == 0 || Path.IsPathFullyQualified(typed)) return;

            string? mp3Folder = Path.GetDirectoryName(txtMP3File.Text.Trim());
            if (string.IsNullOrEmpty(mp3Folder)) return;

            try
            {
                e.Value = Path.GetFullPath(typed, mp3Folder);
            }
            catch { }
        }

        private static void GrdFiles_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
        }

        private void GrdFiles_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files) return;

            int? firstAdded = null;

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file);
                if (!ImageExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) &&
                    !VideoExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    continue;

                // Find the first row with no file, or add a new one
                DataGridViewRow? emptyRow = grdFiles.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(r => !r.IsNewRow &&
                                        string.IsNullOrWhiteSpace(r.Cells[colFile.Index].Value?.ToString()));

                int rowIdx;
                if (emptyRow != null)
                {
                    emptyRow.Cells[colFile.Index].Value = file;
                    rowIdx = emptyRow.Index;
                }
                else
                {
                    rowIdx = grdFiles.Rows.Add();
                    grdFiles.Rows[rowIdx].Cells[colFile.Index].Value = file;
                }

                firstAdded ??= rowIdx;
            }

            if (firstAdded.HasValue)
            {
                grdFiles.CurrentCell = grdFiles.Rows[firstAdded.Value].Cells[0];
                RefreshMultipleState();
            }
        }

        private void TabImage_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabImage.SelectedTab != tabMultiple) return;

            string imagePath = cboImageFile.Text.Trim();
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath)) return;

            bool hasData = grdFiles.Rows.Cast<DataGridViewRow>().Any(r => !r.IsNewRow);
            if (hasData) return;

            int newRowIdx = grdFiles.Rows.Add(imagePath, "0:00", "");
            grdFiles.CurrentCell = grdFiles.Rows[newRowIdx].Cells[0];
            RefreshMultipleState();
        }

        private void RefreshMultipleState()
        {
            UpdateMultipleButtons();
            string? file = grdFiles.CurrentRow?.Cells[colFile.Index].Value?.ToString();
            LoadImagePreview(file ?? "", pictureBox2);
        }

        private void UpdateMultipleButtons()
        {
            int idx = grdFiles.CurrentRow?.Index ?? -1;
            int lastData = grdFiles.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) - 1;
            btnMoveUp.Enabled = idx > 0 && idx <= lastData;
            btnMoveDown.Enabled = idx >= 0 && idx < lastData;
            btnDelete.Enabled = idx >= 0 && idx <= lastData;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Add Image or Video Files",
                Filter = ImageVideoFilter,
                Multiselect = true,
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            int? firstAdded = null;
            foreach (string file in dlg.FileNames)
            {
                int idx = grdFiles.Rows.Add(file, "", "");
                firstAdded ??= idx;
            }

            if (firstAdded.HasValue)
            {
                grdFiles.CurrentCell = grdFiles.Rows[firstAdded.Value].Cells[0];
                RefreshMultipleState();
            }
        }

        private void CmdMoveUp_Click(object? sender, EventArgs e)
        {
            int idx = grdFiles.CurrentRow?.Index ?? -1;
            if (idx <= 0) return;
            SwapRows(idx, idx - 1);
            grdFiles.CurrentCell = grdFiles.Rows[idx - 1].Cells[0];
            RefreshMultipleState();
        }

        private void CmdMoveDown_Click(object? sender, EventArgs e)
        {
            int idx = grdFiles.CurrentRow?.Index ?? -1;
            int lastData = grdFiles.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) - 1;
            if (idx < 0 || idx >= lastData) return;
            SwapRows(idx, idx + 1);
            grdFiles.CurrentCell = grdFiles.Rows[idx + 1].Cells[0];
            RefreshMultipleState();
        }

        /// <summary>
        /// Swaps only the files: the time slots (Start Time, and so Duration) stay in place, so
        /// moving an image up/down changes which image is shown when, not the timing itself.
        /// </summary>
        private void SwapRows(int a, int b)
        {
            (grdFiles.Rows[a].Cells[colFile.Index].Value,
             grdFiles.Rows[b].Cells[colFile.Index].Value) =
            (grdFiles.Rows[b].Cells[colFile.Index].Value,
             grdFiles.Rows[a].Cells[colFile.Index].Value);
        }

        private void CmdDelete_Click(object? sender, EventArgs e)
        {
            int idx = grdFiles.CurrentRow?.Index ?? -1;
            if (idx < 0) return;
            grdFiles.Rows.RemoveAt(idx);
            // Move selection to the row that slid into this position, or the one above
            int lastData = grdFiles.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) - 1;
            if (lastData >= 0)
                grdFiles.CurrentCell = grdFiles.Rows[Math.Min(idx, lastData)].Cells[0];
            RefreshMultipleState();
        }

        private void BtnOpenFile_Click(object? sender, EventArgs e)
        {
            string mp4 = txtMP4File.Text.Trim();
            if (!File.Exists(mp4))
            {
                MessageBox.Show("File not found.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Process.Start(new ProcessStartInfo(mp4) { UseShellExecute = true });
        }

        private void BtnOpenFolder_Click(object? sender, EventArgs e)
        {
            string mp4 = txtMP4File.Text.Trim();
            if (!string.IsNullOrEmpty(mp4) && File.Exists(mp4))
            {
                Process.Start("explorer.exe", $"/select,\"{mp4}\"");
                return;
            }
            string? folder = Path.GetDirectoryName(mp4);
            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                Process.Start("explorer.exe", $"\"{folder}\"");
        }

        private void BtnOpenLogFile_Click(object? sender, EventArgs e)
        {
            if (_lastLogFile == null || !File.Exists(_lastLogFile))
            {
                MessageBox.Show("Log file not found.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Process.Start(new ProcessStartInfo(_lastLogFile) { UseShellExecute = true });
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll",
            CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern uint GetShortPathName(string lpszLongPath,
            System.Text.StringBuilder lpszShortPath, uint cchBuffer);

        // Returns the 8.3 short path (always ASCII) so FFmpeg receives no Unicode in arguments.
        // Falls back to the original path if short names are unavailable on the volume.
        private static string FfmpegPath(string path)
        {
            var sb = new System.Text.StringBuilder(512);
            uint n = GetShortPathName(path, sb, (uint)sb.Capacity);
            return (n > 0 && n < sb.Capacity) ? sb.ToString() : path;
        }

        private static string? FindFFmpeg()
        {
            // 1. Next to the executable
            string local = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            if (File.Exists(local)) return local;

            // 2. System PATH
            foreach (string dir in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
            {
                string candidate = Path.Combine(dir.Trim(), "ffmpeg.exe");
                if (File.Exists(candidate)) return candidate;
            }

            // 3. Common install location
            string common = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "FFmpeg", "bin", "ffmpeg.exe");
            if (File.Exists(common)) return common;

            return null;
        }
    }
}
