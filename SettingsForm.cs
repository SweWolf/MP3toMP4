namespace MP3toMP4
{
    internal partial class SettingsForm : Form
    {
        private const string CustomSoundSentinel = "Custom Sound File...";

        public SettingsForm()
        {
            InitializeComponent();

            // The Designer sets ClientSize before ControlBox = false / FixedDialog, and changing
            // those keeps the window size, so the client area ends up ~33 px too short and the
            // bottom-anchored OK/Cancel slide up over the last group. Size it from the layout instead.
            ClientSize = new Size(grpActionWhenFinished.Right + 12,
                                  grpActionWhenFinished.Bottom + 12 + btnOK.Height + 12);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            cboNewVersionCheck.SelectedItem = AppSettings.CheckForUpdatesOnStartup;
            txtDefaultOutputFolder.Text = AppSettings.DefaultOutputFolder;
            // Item 0 = "MP4 (Recommended)", 1 = "MKV", 2 = "MKV When Converting from a Lossless Format"
            cboDefaultOutputFormat.SelectedIndex = AppSettings.DefaultOutputFormat switch
            {
                "MKV" => 1,
                "MKV for Lossless" => 2,
                _ => 0
            };
            // Item 0 = "Quality (Recommended)", item 1 = "Smaller File Size"
            cboOptimizeAudioFor.SelectedIndex = AppSettings.OptimizeAudioFor == "Smaller File Size" ? 1 : 0;

            cboFinishedSound.Items.AddRange(SoundLibrary.GetAvailableSounds().ToArray());
            cboFinishedSound.Items.Add(CustomSoundSentinel);

            cboActionWhenFinished.SelectedItem = AppSettings.ActionWhenConversionFinished;

            string savedSound = AppSettings.FinishedConversionSoundFile;
            if (string.IsNullOrEmpty(savedSound))
                savedSound = SoundLibrary.DefaultSoundFileName;

            SoundOption? match = cboFinishedSound.Items.OfType<SoundOption>()
                .FirstOrDefault(o => o.FileName == savedSound);
            if (match != null)
            {
                cboFinishedSound.SelectedItem = match;
            }
            else
            {
                // Not one of the bundled sounds: treat the saved value as a custom file path
                cboFinishedSound.SelectedItem = CustomSoundSentinel;
                txtCustomSoundFile.Text = savedSound;
            }

            cboActionWhenFinished.SelectedIndexChanged += (_, _) => UpdateSoundControlsVisibility();
            cboFinishedSound.SelectedIndexChanged += (_, _) => UpdateSoundControlsVisibility();
            UpdateSoundControlsVisibility();
        }

        private bool IsPlaySoundSelected => cboActionWhenFinished.SelectedItem?.ToString() == "Play a Sound";

        private bool IsCustomSoundSelected => (cboFinishedSound.SelectedItem as string) == CustomSoundSentinel;

        private void UpdateSoundControlsVisibility()
        {
            bool playSound = IsPlaySoundSelected;
            lblFinishedSound.Visible = playSound;
            cboFinishedSound.Visible = playSound;
            btnPlayFinishedSound.Visible = playSound;

            bool customSound = playSound && IsCustomSoundSelected;
            lblCustomSoundFile.Visible = customSound;
            txtCustomSoundFile.Visible = customSound;
            btnBrowseCustomSoundFile.Visible = customSound;
        }

        /// <summary>
        /// Returns the sound to save/play for the current selection: the bundled file name,
        /// or the full path typed/browsed into the custom sound text box.
        /// </summary>
        private string GetSelectedSoundIdentifier()
        {
            if (IsCustomSoundSelected)
                return txtCustomSoundFile.Text.Trim();

            return (cboFinishedSound.SelectedItem as SoundOption)?.FileName ?? string.Empty;
        }

        private void BtnPlayFinishedSound_Click(object? sender, EventArgs e)
        {
            SoundLibrary.Play(GetSelectedSoundIdentifier());
        }

        private void BtnBrowseCustomSoundFile_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Select Custom Sound File",
                Filter = "WAV audio files (*.wav)|*.wav",
                DefaultExt = "wav"
            };

            string current = txtCustomSoundFile.Text.Trim();
            if (File.Exists(current))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(current);
                dlg.FileName = Path.GetFileName(current);
            }

            if (dlg.ShowDialog(this) == DialogResult.OK)
                txtCustomSoundFile.Text = dlg.FileName;
        }

        private void btnBrowseDefaultOutputFolder_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Select Default Output Folder",
                UseDescriptionForTitle = true
            };

            string current = txtDefaultOutputFolder.Text.Trim();
            if (Directory.Exists(current))
                // A trailing separator makes the folder picker open inside this folder
                dlg.SelectedPath = Path.TrimEndingDirectorySeparator(current) + Path.DirectorySeparatorChar;

            if (dlg.ShowDialog(this) == DialogResult.OK)
                txtDefaultOutputFolder.Text = dlg.SelectedPath;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsPlaySoundSelected && IsCustomSoundSelected)
            {
                string customPath = txtCustomSoundFile.Text.Trim();
                if (!string.Equals(Path.GetExtension(customPath), ".wav", StringComparison.OrdinalIgnoreCase)
                    || !File.Exists(customPath))
                {
                    MessageBox.Show("Please select an existing WAV file as the custom sound.",
                        Form1.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomSoundFile.Focus();
                    return;
                }
            }

            AppSettings.CheckForUpdatesOnStartup = cboNewVersionCheck.SelectedItem?.ToString() ?? "Yes";
            AppSettings.DefaultOutputFolder = txtDefaultOutputFolder.Text.Trim();
            AppSettings.DefaultOutputFormat = cboDefaultOutputFormat.SelectedIndex switch
            {
                1 => "MKV",
                2 => "MKV for Lossless",
                _ => "MP4"
            };
            AppSettings.OptimizeAudioFor = cboOptimizeAudioFor.SelectedIndex == 1 ? "Smaller File Size" : "Quality";
            AppSettings.ActionWhenConversionFinished = cboActionWhenFinished.SelectedItem?.ToString() ?? "Play a Sound";
            AppSettings.FinishedConversionSoundFile = GetSelectedSoundIdentifier();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
