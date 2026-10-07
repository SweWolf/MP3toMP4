namespace MP3toMP4
{
    public partial class AboutForm : Form
    {
        private readonly Version _currentVersion;

        public AboutForm()
        {
            InitializeComponent();

            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
            _currentVersion = new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart);
            lblVersion.Text = $"Version {fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";

            lblFfmpegVer.Text = GetFfmpegVersion();
            lnkFfmpeg.LinkClicked += lnkFfmpeg_LinkClicked;
            Shown += AboutForm_Shown;

            try
            {
                // Embedded in the EXE (see MP3toMP4.csproj), so it also works in the standalone release
                using Stream? stream = typeof(AboutForm).Assembly.GetManifestResourceStream("MP3_to_MP4_Converter.ico");
                if (stream != null)
                {
                    using var ico = new Icon(stream, 48, 48);
                    picIcon.Image = ico.ToBitmap();
                }
            }
            catch { }
        }

        private async void AboutForm_Shown(object? sender, EventArgs e)
        {
            var result = await GitHubUpdateChecker.CheckAsync("SweWolf", "MP3toMP4", _currentVersion);

            if (result == null || IsDisposed) return; // network error or form already closed

            if (result.IsUpdateAvailable)
            {
                lblUpdateStatus.Text = $"↑ Version {result.LatestVersion} available";
                lblUpdateStatus.ForeColor = Color.FromArgb(255, 210, 80); // warm yellow
            }
            else
            {
                lblUpdateStatus.Text = "✓ This is the latest version";
                lblUpdateStatus.ForeColor = Color.FromArgb(120, 210, 120); // light green
            }
        }

        private static string GetFfmpegVersion()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-version")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                };
                using var process = System.Diagnostics.Process.Start(psi);
                if (process == null) return "Not found";
                string firstLine = process.StandardOutput.ReadLine() ?? "";
                process.WaitForExit();
                // First line: "ffmpeg version 7.1.1 Copyright (c) ..."
                var match = System.Text.RegularExpressions.Regex.Match(firstLine, @"ffmpeg version (\S+)");
                return match.Success ? match.Groups[1].Value : "Unknown";
            }
            catch { return "Not found"; }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void lnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = "https://github.com/SweWolf/MP3toMP4",
                UseShellExecute = true,
            });
        }

        private void lnkFfmpeg_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = "https://ffmpeg.org",
                UseShellExecute = true,
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
