using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MP3toMP4
{
    public partial class Form1 : Form
    {
        private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp"];

        private Process? _ffmpegProcess;
        private bool     _cancelRequested;

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

            picImage.SizeMode = PictureBoxSizeMode.Zoom;
            cboImageFile.TextChanged += (_, _) => LoadImagePreview(cboImageFile.Text.Trim());
            cboImageFile.KeyDown += CboImageFile_KeyDown;

            var pasteItem = new ToolStripMenuItem("Paste");
            pasteItem.Click += (_, _) => PasteClipboardImage();
            var ctxMenu = new ContextMenuStrip();
            ctxMenu.Items.Add(pasteItem);
            ctxMenu.Opening += (_, _) => pasteItem.Enabled = Clipboard.ContainsImage() || Clipboard.ContainsText();
            cboImageFile.ContextMenuStrip = ctxMenu;

            chkUseImageFileFromMp3File.CheckedChanged += ChkUseImageFileFromMp3File_CheckedChanged;

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
                Title = "Select image file",
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp|All files (*.*)|*.*"
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
                DefaultExt = "mp4"
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

        private void LoadImagePreview(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                picImage.Image = null;
                return;
            }
            try
            {
                // Load via MemoryStream so the file is not locked after loading
                using var ms = new MemoryStream(File.ReadAllBytes(path));
                picImage.Image?.Dispose();
                picImage.Image = Image.FromStream(ms);
            }
            catch
            {
                picImage.Image = null;
            }
        }

        private void TxtMP3File_TextChanged(object? sender, EventArgs e)
        {
            string mp3 = txtMP3File.Text.Trim();
            if (!string.IsNullOrEmpty(mp3))
                txtMP4File.Text = Path.ChangeExtension(mp3, ".mp4");
        }

        private void PopulateImageCombo(string mp3Path)
        {
            cboImageFile.Items.Clear();
            chkUseImageFileFromMp3File.Enabled = false;
            chkUseImageFileFromMp3File.Checked = false;

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

            // Enable checkbox if the MP3 contains embedded artwork
            chkUseImageFileFromMp3File.Enabled = HasEmbeddedArtwork(mp3Path);
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

        private async void BtnConvert_Click(object? sender, EventArgs e)
        {
            string mp3 = txtMP3File.Text.Trim();
            string image = cboImageFile.Text.Trim();
            string mp4 = txtMP4File.Text.Trim();

            if (string.IsNullOrEmpty(mp3) || !File.Exists(mp3))
            {
                MessageBox.Show("Please select a valid MP3 file.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(image) || !File.Exists(image))
            {
                MessageBox.Show("Please select a valid image file.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(mp4))
            {
                MessageBox.Show("Please specify an output MP4 file.", "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string? ffmpeg = FindFFmpeg();
            if (ffmpeg == null)
            {
                MessageBox.Show(
                    "FFmpeg not found. Place ffmpeg.exe next to this application or add it to PATH.",
                    "MP3toMP4", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnConvert.Enabled    = false;
            btnCancel.Enabled     = true;
            _cancelRequested      = false;
            progressBar.Value     = 0;
            lblEstimatedRemaining.Text = "Estimated remaining time: —";

            try
            {
                await RunFFmpegAsync(ffmpeg, mp3, image, mp4);
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

        private Task RunFFmpegAsync(string ffmpeg, string mp3, string image, string mp4)
        {
            return Task.Run(() =>
            {
                string args = string.Join(" ",
                    "-y",
                    "-loop 1",
                    $"-i \"{image}\"",
                    $"-i \"{mp3}\"",
                    "-c:v libx264",
                    "-tune stillimage",
                    "-vf \"scale=1280:-2\"",
                    "-r 30",
                    "-pix_fmt yuv420p",
                    "-c:a copy",
                    "-shortest",
                    "-movflags +faststart",
                    $"\"{mp4}\""
                );

                var psi = new ProcessStartInfo
                {
                    FileName = ffmpeg,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                _ffmpegProcess = process;

                TimeSpan totalDuration = GetMp3Duration(mp3);
                var startTime = DateTime.UtcNow;

                process.Start();

                string? line;
                while ((line = process.StandardError.ReadLine()) != null)
                {
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
