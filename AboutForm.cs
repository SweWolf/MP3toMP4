using System.Reflection;

namespace MP3toMP4
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            lblVersion.Text = version != null
                ? $"Version {version.Major}.{version.Minor}.{version.Build}"
                : "Version 1.0.0";

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

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void lnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/SweWolf",
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
