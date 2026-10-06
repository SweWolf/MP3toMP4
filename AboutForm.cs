namespace MP3toMP4
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
            lblVersion.Text = $"Version {fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";

            lblFfmpegVer.Text = GetFfmpegVersion();
            lnkFfmpeg.LinkClicked += lnkFfmpeg_LinkClicked;

            try
            {
                string iconPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "MP3_to_MP4_Converter.ico");
                if (File.Exists(iconPath))
                {
                    using var ico = new Icon(iconPath, 48, 48);
                    picIcon.Image = ico.ToBitmap();
                }
            }
            catch { }
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
