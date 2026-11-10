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
            // The groups are in two columns; Projects is the last group of the right, taller column.
            ClientSize = new Size(grpProjects.Right + 12,
                                  grpProjects.Bottom + 12 + btnOK.Height + 12);
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

            // Projects: the list holds suggestions, any other extension can be typed
            cboProjectExtension.Items.AddRange(ProjectFiles.SuggestedExtensions);
            cboProjectExtension.Text = AppSettings.ProjectFileExtension;
            // The list offers one ready-made folder; any other folder can be typed or browsed for
            cboDefaultProjectFolder.Items.Add(ProjectFiles.ProjectsFolder);
            cboDefaultProjectFolder.DropDownWidth = Math.Max(cboDefaultProjectFolder.Width,
                TextRenderer.MeasureText(ProjectFiles.ProjectsFolder, cboDefaultProjectFolder.Font).Width
                + SystemInformation.VerticalScrollBarWidth);
            cboDefaultProjectFolder.Text = AppSettings.DefaultProjectFolder;
            chkAutosaveProjects.Checked = AppSettings.AutosaveProjects == "Yes";
            numRecentProjects.Value = AppSettings.RecentProjectsToShow;
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

        private void btnBrowseDefaultOutputFolder_Click(object sender, EventArgs e) =>
            BrowseForFolder(txtDefaultOutputFolder, "Select Default Output Folder");

        private void btnBrowseDefaultProjectFolder_Click(object? sender, EventArgs e) =>
            BrowseForFolder(cboDefaultProjectFolder, "Select Default Project Folder");

        /// <summary>Shows the folder picker, starting in the folder that is in the box.</summary>
        private void BrowseForFolder(Control target, string title)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = title,
                UseDescriptionForTitle = true
            };

            string current = target.Text.Trim();
            if (Directory.Exists(current))
                // A trailing separator makes the folder picker open inside this folder
                dlg.SelectedPath = Path.TrimEndingDirectorySeparator(current) + Path.DirectorySeparatorChar;

            if (dlg.ShowDialog(this) == DialogResult.OK)
                target.Text = dlg.SelectedPath;
        }

        /// <summary>
        /// The Default Project Folder is only used while it exists, so a folder that is missing
        /// (like the one from the list, the first time) is created here, after asking. Empty is
        /// fine: it means the folder of the audio file. Returns false when the user should
        /// correct the field.
        /// </summary>
        private bool EnsureProjectFolderExists(string folder)
        {
            if (folder.Length == 0) return true;

            if (!Path.IsPathFullyQualified(folder))
            {
                MessageBox.Show("Enter the full path of the Default Project Folder, for example C:\\Projects, " +
                                "or leave it empty to use the folder of the audio file.",
                    Form1.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (Directory.Exists(folder)) return true;

            if (MessageBox.Show($"The folder \"{folder}\" does not exist.\n\nDo you want to create it now?",
                    Form1.AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return false;

            try
            {
                Directory.CreateDirectory(folder); // creates any missing intermediate subfolders too
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not create the folder:\n{folder}\n\n{ex.Message}",
                    Form1.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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

            string? projectExtension = ProjectFiles.NormalizeExtension(cboProjectExtension.Text, out string extensionError);
            if (projectExtension == null)
            {
                MessageBox.Show(extensionError, Form1.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboProjectExtension.Focus();
                return;
            }

            // "Copy as path" in Explorer adds quotes
            string projectFolder = cboDefaultProjectFolder.Text.Trim().Trim('"').Trim();
            if (!EnsureProjectFolderExists(projectFolder))
            {
                cboDefaultProjectFolder.Focus();
                return;
            }

            AppSettings.ProjectFileExtension = projectExtension;
            AppSettings.DefaultProjectFolder = projectFolder;
            AppSettings.AutosaveProjects = chkAutosaveProjects.Checked ? "Yes" : "No";
            AppSettings.RecentProjectsToShow = (int)numRecentProjects.Value;
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
