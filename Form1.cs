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

        private static bool IsVideoFile(string path) =>
            VideoExtensions.Contains(Path.GetExtension(path).ToLowerInvariant());

        private static readonly string SettingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SweWolfSoftware", "MP3toMP4", "settings.json");

        private record ImageSegment(string FilePath, TimeSpan Duration);

        private Process? _ffmpegProcess;
        private bool     _cancelRequested;
        private string?  _lastLogFile;

        public Form1()
        {
            InitializeComponent();

            btnBrowseForMp3File.Click += BtnBrowseForMp3File_Click;
            btnBrowsForImageFile.Click += BtnBrowseForImageFile_Click;
            cmdBrowseForMP4File.Click += CmdBrowseForMP4File_Click;
            btnConvert.Click += BtnConvert_Click;
            txtMP3File.TextChanged += TxtMP3File_TextChanged;

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

            picImage.SizeMode    = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            cboImageFile.TextChanged += (_, _) =>
            {
                string t = cboImageFile.Text;
                if (t.Length >= 2 && t[0] == '"' && t[^1] == '"')
                    { cboImageFile.Text = t[1..^1]; return; }
                LoadImagePreview(t.Trim(), picImage);
            };
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

            chkUseImageFileFromMp3File.CheckedChanged  += ChkUseImageFileFromMp3File_CheckedChanged;
            chkUseTheFullMp3File.CheckedChanged        += ChkUseTheFullMp3File_CheckedChanged;

            btnOpenFile.Click   += BtnOpenFile_Click;
            btnOpenFolder.Click += BtnOpenFolder_Click;
            btnOpenLogFile.Click += BtnOpenLogFile_Click;

            btnAdd.Click       += BtnAdd_Click;
            btnMoveUp.Click    += CmdMoveUp_Click;
            btnMoveDown.Click  += CmdMoveDown_Click;
            btnDelete.Click    += CmdDelete_Click;

            grdFiles.SelectionChanged += (_, _) => RefreshMultipleState();
            grdFiles.CellValueChanged += GrdFiles_CellValueChanged;
            grdFiles.CellValidating  += GrdFiles_CellValidating;
            grdFiles.EditingControlShowing += GrdFiles_EditingControlShowing;

            colFile.SortMode     = DataGridViewColumnSortMode.NotSortable;
            colStart.SortMode    = DataGridViewColumnSortMode.NotSortable;
            colDuration.SortMode = DataGridViewColumnSortMode.NotSortable;

            grdFiles.AllowDrop = true;
            grdFiles.DragEnter += GrdFiles_DragEnter;
            grdFiles.DragDrop  += GrdFiles_DragDrop;
            grdFiles.CellFormatting += GrdFiles_CellFormatting;
            grdFiles.CellEndEdit    += GrdFiles_CellEndEdit;
            tabImage.SelectedIndexChanged += TabImage_SelectedIndexChanged;
            UpdateMultipleButtons();

            chkLyrics.Enabled = false;
            LoadSettings();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                txtMP3File.Text = args[1];
                PopulateImageCombo(args[1]);
            }
        }

        private void BtnBrowseForMp3File_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Select MP3 file",
                Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            txtMP3File.Text = dlg.FileName;
            PopulateImageCombo(dlg.FileName);
        }

        private void BtnBrowseForImageFile_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Select image or video file",
                Filter = "Image & video files (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.mp4;*.mov;*.avi;*.mkv;*.webm;*.m4v)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.mp4;*.mov;*.avi;*.mkv;*.webm;*.m4v|All files (*.*)|*.*"
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
                    "MP3toMP4 – drag diagnostic", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                txtMP4File.Text = Path.ChangeExtension(mp3, ".mp4");

            if (File.Exists(mp3))
                PopulateImageCombo(mp3);
            else
            {
                cboImageFile.Items.Clear();
                chkUseImageFileFromMp3File.Enabled = false;
                chkUseImageFileFromMp3File.Checked = false;
                chkLyrics.Enabled = false;
                chkLyrics.Checked = false;
            }
        }

        private void PopulateImageCombo(string mp3Path)
        {
            cboImageFile.Items.Clear();
            chkUseImageFileFromMp3File.Enabled = false;
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
            chkUseImageFileFromMp3File.Enabled = HasEmbeddedArtwork(mp3Path);
            chkLyrics.Enabled = HasLyrics(mp3Path);
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
                    "image/png"  => ".png",
                    "image/gif"  => ".gif",
                    "image/bmp"  => ".bmp",
                    _            => ".jpg"
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
            if (!chkUseImageFileFromMp3File.Checked) return;

            string mp3 = txtMP3File.Text.Trim();
            if (string.IsNullOrEmpty(mp3)) return;

            string? tempFile = ExtractEmbeddedArtwork(mp3);
            if (tempFile == null)
            {
                MessageBox.Show("Could not extract artwork from the MP3 file.",
                    "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkUseImageFileFromMp3File.Checked = false;
                return;
            }

            // Add to combo if not already there, then select it
            if (!cboImageFile.Items.Contains(tempFile))
                cboImageFile.Items.Insert(0, tempFile);

            cboImageFile.SelectedItem = tempFile;
        }

        private void ChkUseTheFullMp3File_CheckedChanged(object? sender, EventArgs e)
        {
            bool show = !chkUseTheFullMp3File.Checked;
            lblInputStart.Visible    = show;
            txtInputStart.Visible    = show;
            label4.Visible           = show;
            txtInputEnd.Visible      = show;
            lblFadeIn.Visible        = show;
            txtFadeInLength.Visible  = show;
            lblFadeOut.Visible       = show;
            txtFadeOutLength.Visible = show;
        }

        private (string InputPrefix, string Codec, string Filter) GetAudioOptions(string mp3Path)
        {
            if (chkUseTheFullMp3File.Checked)
                return ("", "-c:a copy", "");

            bool hasStart = TryParseTime(txtInputStart.Text, out var start);
            bool hasEnd   = TryParseTime(txtInputEnd.Text,   out var end);
            TryParseFade(txtFadeInLength.Text,  out double fadeIn);
            TryParseFade(txtFadeOutLength.Text, out double fadeOut);

            bool hasTrim = hasStart || hasEnd;
            bool hasFade = fadeIn > 0 || fadeOut > 0;

            if (!hasTrim && !hasFade)
                return ("", "-c:a copy", "");

            // Build the audio filter chain entirely in -af so FFmpeg controls the
            // endpoint precisely.  (Input-side -ss/-to combined with -af and AAC
            // encoding can flush 1–2 extra seconds of buffered frames after the
            // intended end point.)
            var filters = new List<string>();

            if (hasTrim)
            {
                string startArg = hasStart ? $"start={start.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)}" : "";
                string endArg   = hasEnd   ? $"end={end.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture)}"     : "";
                string trimArgs = string.Join(":", new[] { startArg, endArg }.Where(s => s.Length > 0));
                filters.Add($"atrim={trimArgs}");
                filters.Add("asetpts=PTS-STARTPTS");
            }

            if (hasFade)
            {
                TimeSpan totalDuration  = GetMp3Duration(mp3Path);
                TimeSpan effectiveStart = hasStart ? start : TimeSpan.Zero;
                TimeSpan effectiveEnd   = hasEnd   ? end   : totalDuration;
                double   effectiveSecs  = Math.Max(0, (effectiveEnd - effectiveStart).TotalSeconds);

                if (fadeIn > 0)
                    filters.Add($"afade=t=in:st=0:d={fadeIn.ToString("F3", CultureInfo.InvariantCulture)}");
                if (fadeOut > 0)
                {
                    double st = Math.Max(0, effectiveSecs - fadeOut);
                    filters.Add($"afade=t=out:st={st.ToString("F3", CultureInfo.InvariantCulture)}:d={fadeOut.ToString("F3", CultureInfo.InvariantCulture)}");
                }
            }

            string filterArg = $"-af \"{string.Join(",", filters)}\"";
            return ("", "-c:a aac -b:a 192k", filterArg);
        }

        private static bool TryParseFade(string? input, out double seconds)
        {
            seconds = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double val)) return false;
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
                MessageBox.Show("Please select a valid MP3 file.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(mp4))
            {
                MessageBox.Show("Please specify an output MP4 file.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!mp4.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("The output file must have the .mp4 extension.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (audioInputPrefix, audioCodec, audioFilter) = GetAudioOptions(mp3);

            // --- Mode-specific validation and arg building ---
            string ffmpegArgs, logInfo;

            if (!multiMode)
            {
                string image = cboImageFile.Text.Trim();
                if (string.IsNullOrEmpty(image) || !File.Exists(image))
                {
                    MessageBox.Show("Please select a valid image or video file.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            else
            {
                TimeSpan mp3Duration = GetMp3Duration(mp3);
                if (!TryBuildSegments(mp3Duration, out var segments, out string segError))
                {
                    MessageBox.Show(segError, "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // --- Overwrite check ---
            if (File.Exists(mp4))
            {
                var answer = MessageBox.Show(
                    $"The output file already exists:\n{mp4}\n\nDo you want to overwrite it?",
                    "File Already Exists",
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
                    "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnConvert.Enabled     = false;
            btnCancel.Enabled      = true;
            btnOpenFile.Enabled    = false;
            btnOpenFolder.Enabled  = false;
            btnOpenLogFile.Enabled = false;
            _cancelRequested       = false;
            progressBar.Value      = 0;
            lblEstimatedRemaining.Text = "Estimated remaining time: —";

            string logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SweWolfSoftware", "MP3toMP4", "Logs");
            Directory.CreateDirectory(logDir);
            _lastLogFile = Path.Combine(logDir, $"convert_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            btnOpenLogFile.Enabled = true;

            TimeSpan totalDuration = GetMp3Duration(mp3);

            try
            {
                await RunFFmpegAsync(ffmpeg, ffmpegArgs, totalDuration, mp4, _lastLogFile, logInfo);

                if (chkLyrics.Checked)
                    WriteLyricsToMp4(mp3, mp4);

                btnOpenFile.Enabled   = File.Exists(mp4);
                btnOpenFolder.Enabled = true;
                progressBar.Value = progressBar.Maximum;
                lblEstimatedRemaining.Text = "Estimated remaining time: Done";
                MessageBox.Show(
                    "Conversion completed successfully!\n\nOutput file:\n" + mp4,
                    "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                progressBar.Value = 0;
                lblEstimatedRemaining.Text = "Estimated remaining time: —";
            }
            catch (Exception ex)
            {
                btnOpenFolder.Enabled = !string.IsNullOrEmpty(Path.GetDirectoryName(mp4));
                MessageBox.Show("Conversion failed: " + ex.Message, "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConvert.Enabled = true;
                btnCancel.Enabled  = false;
                _ffmpegProcess     = null;
            }
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
                        UseShellExecute        = false,
                        CreateNoWindow         = true,
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

        private bool TryBuildSegments(TimeSpan mp3Duration, out List<ImageSegment> segments, out string error)
        {
            segments = [];
            error    = "";

            var dataRows = grdFiles.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            if (dataRows.Count == 0)
            {
                error = "Add at least one image to the Multiple Images list.";
                return false;
            }

            for (int i = 0; i < dataRows.Count; i++)
            {
                string? file = dataRows[i].Cells[colFile.Index].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(file) || !File.Exists(file))
                {
                    error = $"Row {i + 1}: file not found:\n{file}";
                    return false;
                }

                if (!TryParseTime(dataRows[i].Cells[colStart.Index].Value?.ToString(), out var start))
                {
                    error = $"Row {i + 1}: invalid or missing Start Time.";
                    return false;
                }

                TimeSpan duration;
                string? durStr = dataRows[i].Cells[colDuration.Index].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(durStr) && TryParseTime(durStr, out var explicit_))
                {
                    duration = explicit_;
                }
                else if (i < dataRows.Count - 1 &&
                         TryParseTime(dataRows[i + 1].Cells[colStart.Index].Value?.ToString(), out var nextStart))
                {
                    duration = nextStart - start;
                }
                else
                {
                    duration = mp3Duration - start;
                }

                if (duration <= TimeSpan.Zero)
                {
                    error = $"Row {i + 1}: duration must be greater than zero.";
                    return false;
                }

                segments.Add(new ImageSegment(file, duration));
            }

            return true;
        }

        private Task RunFFmpegAsync(string ffmpeg, string ffmpegArgs, TimeSpan totalDuration,
                                     string mp4, string logFile, string logInfo)
        {
            return Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ffmpeg,
                    Arguments = ffmpegArgs,
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
            {
                var result = MessageBox.Show(
                    "Conversion in progress. If you close the application, the output file will be deleted.",
                    "Close Application",
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
                "Cancel Conversion",
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

        private void aboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using var form = new AboutForm();
            form.ShowDialog(this);
        }

        private void GrdFiles_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (grdFiles.CurrentCell?.ColumnIndex != colStart.Index &&
                grdFiles.CurrentCell?.ColumnIndex != colDuration.Index) return;

            if (e.Control is TextBox tb)
            {
                tb.KeyPress -= GridTimeCell_KeyPress;
                tb.KeyPress += GridTimeCell_KeyPress;
            }
        }

        private static void GridTimeCell_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ',')
            {
                e.Handled = true;
                if (sender is TextBox tb)
                {
                    int pos = tb.SelectionStart;
                    tb.Text = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength).Insert(pos, ".");
                    tb.SelectionStart = pos + 1;
                }
            }
        }

        private bool _gridUpdating;

        private void GrdFiles_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != colStart.Index && e.ColumnIndex != colDuration.Index) return;
            string? val = e.FormattedValue?.ToString();
            if (string.IsNullOrWhiteSpace(val)) return;
            if (!TryParseTime(val, out _))
            {
                e.Cancel = true;
                MessageBox.Show("Enter a time as H:MM:SS, M:SS, or plain seconds (e.g. 90).",
                    "Invalid time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GrdFiles_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_gridUpdating || e.RowIndex < 0) return;
            if (e.ColumnIndex != colStart.Index && e.ColumnIndex != colDuration.Index) return;

            _gridUpdating = true;
            try
            {
                var rows = grdFiles.Rows;
                int row  = e.RowIndex;

                if (e.ColumnIndex == colStart.Index)
                {
                    // Start Time changed on row N → update Duration of row N-1
                    if (row > 0 &&
                        TryParseTime(rows[row].Cells[colStart.Index].Value?.ToString(), out var thisStart) &&
                        TryParseTime(rows[row - 1].Cells[colStart.Index].Value?.ToString(), out var prevStart) &&
                        thisStart > prevStart)
                    {
                        rows[row - 1].Cells[colDuration.Index].Value = FormatTime(thisStart - prevStart);
                    }
                }
                else // colDuration
                {
                    // Duration changed on row N → update Start Time of row N+1
                    if (row < rows.Count - 1 &&
                        TryParseTime(rows[row].Cells[colStart.Index].Value?.ToString(), out var start) &&
                        TryParseTime(rows[row].Cells[colDuration.Index].Value?.ToString(), out var duration))
                    {
                        rows[row + 1].Cells[colStart.Index].Value = FormatTime(start + duration);
                    }
                }
            }
            finally
            {
                _gridUpdating = false;
            }
        }

        private static bool TryParseTime(string? input, out TimeSpan result)
        {
            result = TimeSpan.Zero;
            if (string.IsNullOrWhiteSpace(input)) return false;

            var parts = input.Split(':');

            // M:SS  — two parts means minutes:seconds, not hours:minutes
            if (parts.Length == 2 &&
                double.TryParse(parts[0], out double min) &&
                double.TryParse(parts[1], System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double sec))
            {
                result = TimeSpan.FromSeconds(min * 60 + sec);
                return true;
            }

            // H:MM:SS — three parts
            if (parts.Length == 3 &&
                double.TryParse(parts[0], out double hr) &&
                double.TryParse(parts[1], out double mn) &&
                double.TryParse(parts[2], System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double sc))
            {
                result = TimeSpan.FromSeconds(hr * 3600 + mn * 60 + sc);
                return true;
            }

            // Plain seconds
            if (double.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double secs))
            {
                result = TimeSpan.FromSeconds(secs);
                return true;
            }

            return false;
        }

        private static string FormatTime(TimeSpan t)
        {
            string frac = t.Milliseconds > 0
                ? $".{t.Milliseconds / 100}"   // one decimal place, e.g. ".5"
                : "";

            return t.TotalHours >= 1
                ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}{frac}"
                : $"{t.Minutes}:{t.Seconds:D2}{frac}";
        }

        private void GrdFiles_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != colStart.Index || e.RowIndex < 0) return;

            var dataRows = grdFiles.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).ToList();
            if (dataRows.Count < 2) return;

            int editedListIdx = dataRows.FindIndex(r => r.Index == e.RowIndex);
            if (editedListIdx < 0) return;

            // Build sortable snapshot
            var snapshot = dataRows.Select(r => (
                File:     r.Cells[colFile.Index].Value?.ToString() ?? "",
                Start:    r.Cells[colStart.Index].Value?.ToString() ?? "",
                Duration: r.Cells[colDuration.Index].Value?.ToString() ?? "",
                T:        TryParseTime(r.Cells[colStart.Index].Value?.ToString(), out var t) ? t : TimeSpan.MaxValue
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
                    dataRows[i].Cells[colFile.Index].Value     = sorted[i].File;
                    dataRows[i].Cells[colStart.Index].Value    = sorted[i].Start;
                    dataRows[i].Cells[colDuration.Index].Value = sorted[i].Duration;
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
            int idx      = grdFiles.CurrentRow?.Index ?? -1;
            int lastData = grdFiles.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) - 1;
            btnMoveUp.Enabled   = idx > 0 && idx <= lastData;
            btnMoveDown.Enabled = idx >= 0 && idx < lastData;
            btnDelete.Enabled   = idx >= 0 && idx <= lastData;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title       = "Add Image or Video Files",
                Filter      = "Image & video files (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.mp4;*.mov;*.avi;*.mkv;*.webm;*.m4v)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.mp4;*.mov;*.avi;*.mkv;*.webm;*.m4v|All files (*.*)|*.*",
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
            int idx      = grdFiles.CurrentRow?.Index ?? -1;
            int lastData = grdFiles.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) - 1;
            if (idx < 0 || idx >= lastData) return;
            SwapRows(idx, idx + 1);
            grdFiles.CurrentCell = grdFiles.Rows[idx + 1].Cells[0];
            RefreshMultipleState();
        }

        private void SwapRows(int a, int b)
        {
            foreach (DataGridViewColumn col in grdFiles.Columns)
            {
                (grdFiles.Rows[a].Cells[col.Index].Value,
                 grdFiles.Rows[b].Cells[col.Index].Value) =
                (grdFiles.Rows[b].Cells[col.Index].Value,
                 grdFiles.Rows[a].Cells[col.Index].Value);
            }
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
                MessageBox.Show("File not found.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Log file not found.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
