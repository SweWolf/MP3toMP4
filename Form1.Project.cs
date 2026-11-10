namespace MP3toMP4
{
    // The File menu: projects (New, Open, Save, Save As, Undo All Changes, Autosave, backups).
    // A project is everything in the main window that describes the video to make, saved as one
    // JSON file (see ProjectData.cs for the contents and ProjectFiles.cs for the shared parts).
    public partial class Form1
    {
        /// <summary>The open project file (full path), or null when the work has no file.</summary>
        private string? _projectPath;

        // The window's contents as JSON (see CaptureState) at two moments. Comparing with the
        // current contents tells whether there is something to save / to undo, without having to
        // know which control was changed, or whether a change was later typed back.
        private string _savedState = "";    // when the project file was last written or read
        private string _openedState = "";   // when the project was opened (or first saved with Save As)

        /// <summary>The project as it was opened: what Undo All Changes goes back to.</summary>
        private ProjectData? _openedData;

        /// <summary>
        /// True from opening a project until its file is written for the first time: just before
        /// that, the file as it was is copied to the backup folder.
        /// </summary>
        private bool _backupPending;

        private bool _applyingProject;
        private bool _autosaveErrorShown;
        private string? _startupProject;

        // The draft of the work in the window (see the Drafts section): its file, when it was
        // started (part of the file name) and what was written last. All null while the work
        // needs no draft, because it is empty or safely in its project file.
        private string? _draftPath;
        private DateTime? _draftStarted;
        private string? _draftJson;
        private readonly ProjectFiles.DraftSession _draftSession = new();

        // Restarted by every change; when it fires, the window has been left alone for a moment
        private readonly System.Windows.Forms.Timer _projectTimer = new() { Interval = 1000 };

        private static bool AutosaveOn => AppSettings.AutosaveProjects == "Yes";

        /// <summary>A conversion is running: Convert is disabled from its start to its end.</summary>
        private bool IsBusy => !btnConvert.Enabled;

        private static string AppVersionText =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "";

        private void InitProjects()
        {
            _projectTimer.Tick += (_, _) =>
            {
                _projectTimer.Stop();
                CheckProjectChanges();
            };

            // Everything that is saved in a project. A control that is missing here is still
            // saved; its change is just not noticed until something else changes.
            void Changed(object? sender, EventArgs e) => ProjectChanged();
            txtMP3File.TextChanged += Changed;
            chkTrimToRange.CheckedChanged += Changed;
            txtInputStart.TextChanged += Changed;
            txtInputEnd.TextChanged += Changed;
            txtFadeInLength.TextChanged += Changed;
            txtFadeOutLength.TextChanged += Changed;
            tabImage.SelectedIndexChanged += Changed;
            radFile.CheckedChanged += Changed;
            radBlack.CheckedChanged += Changed;
            radText.CheckedChanged += Changed;
            cboImageFile.TextChanged += Changed;
            chkUseImageFileFromMp3File.CheckedChanged += Changed;
            txtVideoText.TextChanged += Changed;
            grdFiles.CellValueChanged += (_, _) => ProjectChanged();
            grdFiles.CellEndEdit += (_, _) => ProjectChanged();
            grdFiles.RowsAdded += (_, _) => ProjectChanged();
            grdFiles.RowsRemoved += (_, _) => ProjectChanged();
            cboTransition.SelectedIndexChanged += Changed;
            cboTransitionVariant.SelectedIndexChanged += Changed;
            numTransitionSeconds.ValueChanged += Changed;
            chkSlowZoom.CheckedChanged += Changed;
            chkLyrics.CheckedChanged += Changed;
            txtMP4File.TextChanged += Changed;
            cboVideoQuality.SelectedIndexChanged += Changed;

            menuFile.DropDownOpening += (_, _) => UpdateFileMenu();
            // A disabled menu item also loses its shortcut key, and the items are only updated when
            // the menu opens: enable them again afterwards (the commands check IsBusy themselves)
            menuFile.DropDownClosed += (_, _) =>
            {
                menuNewProject.Enabled = true;
                menuOpenProject.Enabled = true;
            };
            menuAutosave.Checked = AutosaveOn;

            ProjectFiles.DeleteOldBackupsInBackground();
            ProjectFiles.DeleteOldDraftsInBackground();
        }

        private void UpdateFileMenu()
        {
            bool idle = !IsBusy;
            menuNewProject.Enabled = idle;
            menuOpenProject.Enabled = idle;
            menuUndoAllChanges.Enabled = idle && _projectPath != null && _openedData != null
                                         && CaptureState() != _openedState;
            menuAutosave.Checked = AutosaveOn;
            UpdateRecentProjectsMenu();
            // The draft of the work that is in the window now is not listed
            ProjectFiles.FillDraftMenu(menuOpenDraft, enabled: idle, exclude: _draftPath, OpenDraft, OpenDraftFolder);
        }

        /// <summary>
        /// Fills File > Recent Projects with the projects opened or saved last, newest first, as
        /// many as Settings > Recent Projects to Show says (0 hides the menu item). The list
        /// itself always holds the last nine, so a higher number shows older ones again.
        /// </summary>
        private void UpdateRecentProjectsMenu() =>
            ProjectFiles.FillRecentMenu(menuRecentProjects, AppSettings.RecentProjectsToShow,
                enabled: !IsBusy, OpenRecentProject);

        private void OpenRecentProject(string path)
        {
            if (IsBusy) return;

            if (!File.Exists(path))
            {
                if (MessageBox.Show(this,
                        $"The project \"{path}\" was not found. It may have been moved, renamed or deleted.\n\n" +
                        "Do you want to remove it from the Recent Projects list?",
                        AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    ProjectFiles.RemoveRecentProject(path);
                return;
            }

            if (!ConfirmLeaveProject()) return;
            UpdateTitle();   // the changes were just saved
            LoadProjectFile(path);
        }

        // ----- Noticing changes, autosave and the title bar -----

        private void ProjectChanged()
        {
            if (_applyingProject) return;
            _projectTimer.Stop();
            _projectTimer.Start();
        }

        /// <summary>
        /// With Autosave on, writes the project file if the window no longer matches it.
        /// What is then still not in a project file goes to the draft. Last, the state is shown
        /// in the title bar.
        /// </summary>
        private void CheckProjectChanges()
        {
            // A cell that is being edited isn't part of the list yet: CellEndEdit comes back here
            if (_projectPath != null && AutosaveOn && !grdFiles.IsCurrentCellInEditMode
                && CaptureState() != _savedState)
                SaveProjectTo(_projectPath, automatic: true);
            UpdateDraft();
            UpdateTitle();
        }

        // ----- Drafts -----
        // A draft is written by the program itself, a moment after every change, for work that is
        // not safely in a project file: work that was never saved as a project, and a project
        // with unsaved changes (Autosave off, or the project file could not be written). It is a
        // normal project file in ProjectFiles.DraftsFolder.
        //  - Work without a project keeps its draft when the window is closed or emptied, so it
        //    can be continued later with File > Open Draft. Saving it as a project deletes it.
        //  - A draft of a project's unsaved changes only outlives a crash: it is deleted when the
        //    changes are saved or deliberately dropped ("No" to "Do you want to save the changes?").
        //  - After a crash, the next start offers the draft that was in use (AskToRestoreCrashedDraft).
        // Problems with drafts are never shown: a draft is a safety net, not the user's own file.

        /// <summary>There is something in the window that would be work to fill in again.</summary>
        private bool HasWork =>
            txtMP3File.Text.Trim().Length > 0
            || cboImageFile.Text.Trim().Length > 0
            || GetGridDataRows().Count > 0
            || (radText.Checked && txtVideoText.Text.Trim().Length > 0);

        /// <summary>Writes the draft if the window changed since it was written; deletes it when none is needed.</summary>
        private void UpdateDraft()
        {
            if (_applyingProject) return;
            try
            {
                bool needed = _projectPath == null ? HasWork : CaptureState() != _savedState;
                if (!needed)
                {
                    DeleteDraft();
                    return;
                }

                var data = CaptureProject(Path.GetDirectoryName(_projectPath));
                data.DraftOf = _projectPath;
                string json = ProjectFiles.Serialize(data);
                if (json == _draftJson && _draftPath != null && File.Exists(_draftPath)) return;

                // Named after the project or the audio file; the name follows if another audio file is chosen
                _draftStarted ??= DateTime.Now;
                string name = _projectPath != null
                    ? ProjectFiles.ProjectName(_projectPath, AppSettings.ProjectFileExtension)
                    : SafeFileNameWithoutExtension(txtMP3File.Text.Trim());
                string path = ProjectFiles.DraftPath(name, _draftStarted.Value, _draftPath);

                Directory.CreateDirectory(ProjectFiles.DraftsFolder);
                ProjectFiles.WriteAtomic(path, json);
                if (_draftPath != null && !ProjectFiles.SamePath(path, _draftPath))
                    try { File.Delete(_draftPath); } catch { }

                _draftPath = path;
                _draftJson = json;
                _draftSession.SetDraft(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
                                           or NotSupportedException or System.Security.SecurityException)
            {
                // No draft this time; the next change tries again
            }
        }

        private void DeleteDraft()
        {
            if (_draftPath != null)
                try { File.Delete(_draftPath); } catch { }
            ForgetDraft();
        }

        private void ForgetDraft()
        {
            _draftPath = null;
            _draftStarted = null;
            _draftJson = null;
            _draftSession.SetDraft(null);
        }

        /// <summary>
        /// Called just before the work in the window is replaced (New, Open) or the window closes.
        /// Work without a project: its draft gets the latest changes and stays behind. A project:
        /// it was just saved or its changes were dropped, so its draft goes.
        /// </summary>
        private void LeaveDraft()
        {
            if (_projectPath != null)
            {
                DeleteDraft();
                return;
            }
            UpdateDraft();
            ForgetDraft();
        }

        /// <summary>File > Open Draft: one of the listed drafts.</summary>
        private void OpenDraft(string path)
        {
            if (IsBusy) return;
            if (!File.Exists(path))
            {
                MessageBox.Show(this, $"The draft \"{Path.GetFileName(path)}\" was not found.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ConfirmLeaveProject()) return;
            UpdateTitle();   // the changes were just saved
            OpenDraftFile(path);
        }

        /// <summary>
        /// Puts a draft back in the window and goes on working in that same draft. A draft of
        /// work without a project comes back as work without a project. A draft of a project's
        /// unsaved changes opens that project with the changes on top, still unsaved.
        /// </summary>
        private bool OpenDraftFile(string path)
        {
            var data = ProjectFiles.Load<ProjectData>(path, out string error);
            if (data == null)
            {
                MessageBox.Show(this, error, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string? project = data.DraftOf;
            if (!string.IsNullOrWhiteSpace(project) && File.Exists(project) && !ProjectFiles.IsDraft(project))
            {
                if (!LoadProjectFile(project, showMissingFiles: false)) return false;
                ApplyProject(data, Path.GetDirectoryName(project));
            }
            else
            {
                LeaveDraft();
                _projectTimer.Stop();
                _projectPath = null;
                _openedData = null;
                _backupPending = false;
                ApplyProject(data, null);
            }

            _draftPath = path;
            _draftStarted = ProjectFiles.DraftStarted(path);
            _draftJson = null;
            _draftSession.SetDraft(path);

            CheckProjectChanges();
            ShowMissingFiles();
            return true;
        }

        private void OpenDraftFolder()
        {
            try
            {
                // Created if missing, so the right folder opens (empty before the first draft)
                Directory.CreateDirectory(ProjectFiles.DraftsFolder);
                FolderOpener.Open(ProjectFiles.DraftsFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Could not open the folder.\n\n{ex.Message}", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// At start-up, without a start argument: if the program was not closed properly the
        /// last time (a crash, a power failure), offers the draft that was in use then. Asked
        /// once; "No" leaves the draft under File > Open Draft. Settings > Projects can turn
        /// the question off.
        /// </summary>
        private void AskToRestoreCrashedDraft()
        {
            var crashed = ProjectFiles.TakeCrashedDrafts();
            if (crashed.Count == 0 || AppSettings.RestoreDraftAfterCrash != "Yes") return;

            string draft = crashed[0];
            if (MessageBox.Show(this,
                    $"{AppTitle} was not closed properly the last time.\n\n" +
                    $"Do you want to continue with the draft \"{Path.GetFileNameWithoutExtension(draft)}\"?\n\n" +
                    "If you choose No, you can still open it later with File > Open Draft.",
                    AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                OpenDraftFile(draft);
        }

        /// <summary>The window is closing normally: the draft is finished and nothing is left for the next start to ask about.</summary>
        private void EndDraftSession()
        {
            LeaveDraft();
            _draftSession.Dispose();
        }

        /// <summary>
        /// "MP3 to MP4 - Song (Autosave)" with Autosave on, "MP3 to MP4 - Song*" with unsaved
        /// changes, and just "MP3 to MP4" when no project is open. The app's name comes first
        /// and the project's name has no extension (the user's choices, 2026-11-10).
        /// </summary>
        private void UpdateTitle()
        {
            string title = AppTitle;
            if (_projectPath != null)
            {
                string unsaved = CaptureState() != _savedState ? "*" : "";
                string autosave = AutosaveOn ? " (Autosave)" : "";
                // Without the extension (.json, .mp3tomp4.json, ...): it says nothing here
                string name = ProjectFiles.ProjectName(_projectPath, AppSettings.ProjectFileExtension);
                title = $"{AppTitle} - {name}{unsaved}{autosave}";
            }
            if (Text != title) Text = title;
        }

        // ----- Window <-> project -----

        /// <summary>The window's contents as project JSON, for comparing two moments.</summary>
        private string CaptureState() =>
            ProjectFiles.Serialize(CaptureProject(Path.GetDirectoryName(_projectPath)));

        /// <summary>
        /// The window's contents as a project. <paramref name="projectFolder"/> is where the project
        /// file is (or will be): the files are also saved relative to it.
        /// </summary>
        private ProjectData CaptureProject(string? projectFolder)
        {
            var data = new ProjectData
            {
                App = ProjectFiles.AppTag,
                FormatVersion = ProjectFiles.FormatVersion,
                AppVersion = AppVersionText,
                AudioFile = ProjectFileRef.From(txtMP3File.Text, projectFolder),
                Trim = new ProjectTrim
                {
                    Enabled = chkTrimToRange.Checked,
                    Start = txtInputStart.Text.Trim(),
                    End = txtInputEnd.Text.Trim(),
                    FadeIn = txtFadeInLength.Text.Trim(),
                    FadeOut = txtFadeOutLength.Text.Trim(),
                },
                Tab = tabImage.SelectedTab == tabMultiple ? "Multiple" : "Single",
                Single = new ProjectSingle
                {
                    Source = radBlack.Checked ? "Black" : radText.Checked ? "Text" : "File",
                    ImageFile = ProjectFileRef.From(cboImageFile.Text, projectFolder),
                    UseImageFromAudioFile = chkUseImageFileFromMp3File.Checked,
                    Text = txtVideoText.Text,
                },
                Effects = new ProjectEffects
                {
                    Transition = GetTransitionId(),
                    TransitionSeconds = numTransitionSeconds.Value,
                    SlowZoom = chkSlowZoom.Checked,
                },
                IncludeLyrics = chkLyrics.Checked,
                Output = new ProjectOutput
                {
                    File = ProjectFileRef.From(txtMP4File.Text, projectFolder),
                    VideoQuality = HighQuality ? "High" : "Standard",
                },
            };

            var rows = GetGridDataRows();
            for (int i = 0; i < rows.Count; i++)
            {
                var durationCell = rows[i].Cells[colDuration.Index];
                data.Rows.Add(new ProjectRow
                {
                    File = ProjectFileRef.From(rows[i].Cells[colFile.Index].Value?.ToString(), projectFolder),
                    // The same rules as RecalculateDurations, so the result doesn't depend on whether
                    // it has run yet: the first row starts at 0:00, and the last row lasts to the
                    // end. Only a typed Duration is saved; the others are worked out again.
                    StartTime = i == 0 ? "0:00" : rows[i].Cells[colStart.Index].Value?.ToString()?.Trim() ?? "",
                    Duration = i < rows.Count - 1 && IsTypedDuration(durationCell)
                        ? durationCell.Value?.ToString()?.Trim() ?? ""
                        : "",
                });
            }
            return data;
        }

        /// <summary>
        /// Fills the window from a project. A file that is not where the project says is looked
        /// for relative to <paramref name="projectFolder"/>, the folder of the project file.
        /// </summary>
        private void ApplyProject(ProjectData data, string? projectFolder)
        {
            _applyingProject = true;
            try
            {
                ResetFields();

                // First the audio file: choosing one fills in the suggested image, text and output file
                txtMP3File.Text = (data.AudioFile ?? new()).Resolve(projectFolder);
                UpdateMp3Length();

                var trim = data.Trim ?? new();
                chkTrimToRange.Checked = trim.Enabled;
                txtInputStart.Text = trim.Start ?? "";
                txtInputEnd.Text = trim.End ?? "";
                txtFadeInLength.Text = string.IsNullOrWhiteSpace(trim.FadeIn) ? "0" : trim.FadeIn;
                txtFadeOutLength.Text = string.IsNullOrWhiteSpace(trim.FadeOut) ? "0" : trim.FadeOut;

                var single = data.Single ?? new();
                cboImageFile.Text = (single.ImageFile ?? new()).Resolve(projectFolder);
                txtVideoText.Text = single.Text ?? "";
                // Only possible while the audio file has a picture; it extracts and selects it
                if (single.UseImageFromAudioFile && _mp3HasArtwork)
                    chkUseImageFileFromMp3File.Checked = true;
                switch (single.Source)
                {
                    case "Black": radBlack.Checked = true; break;
                    case "Text": radText.Checked = true; break;
                    default: radFile.Checked = true; break;
                }

                foreach (var row in data.Rows ?? [])
                {
                    if (row == null) continue;
                    int index = grdFiles.Rows.Add(
                        (row.File ?? new()).Resolve(projectFolder), row.StartTime ?? "", row.Duration ?? "");
                    if (!string.IsNullOrWhiteSpace(row.Duration))
                        grdFiles.Rows[index].Cells[colDuration.Index].Tag = "typed";
                }
                // After the rows: opening the tab with an empty list copies the single image into it
                tabImage.SelectedTab = data.Tab == "Multiple" ? tabMultiple : tabSingle;

                var effects = data.Effects ?? new();
                SelectTransition(effects.Transition ?? "none");
                numTransitionSeconds.Value = Math.Clamp(effects.TransitionSeconds,
                    numTransitionSeconds.Minimum, numTransitionSeconds.Maximum);
                chkSlowZoom.Checked = effects.SlowZoom;

                // Only possible while the audio file has lyrics
                chkLyrics.Checked = data.IncludeLyrics && chkLyrics.Enabled;

                var output = data.Output ?? new();
                string outputFile = (output.File ?? new()).Resolve(projectFolder, isFolderEnough: true);
                if (outputFile.Length > 0)   // else the file suggested for the audio file stays
                    txtMP4File.Text = outputFile;
                cboVideoQuality.SelectedIndex = output.VideoQuality == "High" ? 1 : 0;
            }
            finally
            {
                _applyingProject = false;
            }

            if (IsHandleCreated)
            {
                RecalculateDurations();
                if (grdFiles.Rows.Count > 0)
                    grdFiles.CurrentCell = grdFiles.Rows[0].Cells[0];
                RefreshMultipleState();
            }
        }

        // ----- Reading and writing the file -----

        /// <summary>
        /// Opens a project file in the window. Returns false, after telling why, when the file
        /// is not a project of this app or can't be read; the window is then left as it was.
        /// </summary>
        private bool LoadProjectFile(string path, bool showMissingFiles = true)
        {
            try { path = Path.GetFullPath(path); } catch { }

            // A draft (e.g. picked in Open Project, or given as the start argument) is opened as a
            // draft, not as a project that lives in the drafts folder
            if (ProjectFiles.IsDraft(path)) return OpenDraftFile(path);

            var data = ProjectFiles.Load<ProjectData>(path, out string error);
            if (data == null)
            {
                MessageBox.Show(this, error, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (data.FormatVersion > ProjectFiles.FormatVersion)
                MessageBox.Show(this,
                    $"This project was saved by a newer version of {AppTitle}. Settings that this version " +
                    "does not know are ignored, and they are removed from the file when it is saved here.",
                    AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

            LeaveDraft();
            _projectTimer.Stop();
            _projectPath = path;
            _openedData = data;
            _backupPending = true;
            _autosaveErrorShown = false;
            ApplyProject(data, Path.GetDirectoryName(path));
            _savedState = _openedState = CaptureState();
            ProjectFiles.AddRecentProject(path);
            UpdateTitle();
            if (showMissingFiles) ShowMissingFiles();
            return true;
        }

        /// <summary>Tells which files of the open project don't exist (of the tab that is used).</summary>
        private void ShowMissingFiles()
        {
            var missing = new List<string>();
            void Check(string? path)
            {
                path = path?.Trim();
                if (!string.IsNullOrEmpty(path) && !File.Exists(path)
                    && !missing.Contains(path, StringComparer.OrdinalIgnoreCase))
                    missing.Add(path);
            }

            Check(txtMP3File.Text);
            if (tabImage.SelectedTab == tabMultiple)
                foreach (var row in GetGridDataRows())
                    Check(row.Cells[colFile.Index].Value?.ToString());
            else if (radFile.Checked && !chkUseImageFileFromMp3File.Checked)
                Check(cboImageFile.Text);

            if (missing.Count == 0) return;

            const int shown = 15;
            string list = string.Join("\n", missing.Take(shown));
            if (missing.Count > shown) list += $"\n... and {missing.Count - shown} more";
            string message = missing.Count == 1
                ? $"This file was not found:\n\n{list}\n\nIt may have been moved, renamed or deleted."
                : $"These files were not found:\n\n{list}\n\nThey may have been moved, renamed or deleted.";
            MessageBox.Show(this, message, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Writes the window's contents to <paramref name="path"/>, which then is the open project.
        /// The first time an opened project is written, the file as it was is first copied to the
        /// backup folder. <paramref name="automatic"/>: an autosave, which reports a problem only
        /// once and doesn't write without that backup copy.
        /// </summary>
        private bool SaveProjectTo(string path, bool automatic)
        {
            try
            {
                if (_backupPending && ProjectFiles.SamePath(path, _projectPath) && File.Exists(path))
                {
                    if (ProjectFiles.CreateBackup(path) == null && automatic)
                        throw new IOException("The backup copy of the original file could not be created.");
                    _backupPending = false;
                }
                ProjectFiles.WriteAtomic(path,
                    ProjectFiles.Serialize(CaptureProject(Path.GetDirectoryName(path))));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                                           or NotSupportedException or System.Security.SecurityException)
            {
                if (!automatic)
                    MessageBox.Show(this, $"The project could not be saved to \"{path}\".\n\n{ex.Message}",
                        AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (!_autosaveErrorShown)
                {
                    _autosaveErrorShown = true;
                    MessageBox.Show(this,
                        $"The project could not be saved automatically to \"{path}\".\n\n{ex.Message}\n\n" +
                        "Your changes are still in the window. To keep them, save the project to " +
                        "another file with File > Save Project As.",
                        AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }

            _projectPath = path;
            _savedState = CaptureState();
            _autosaveErrorShown = false;
            UpdateDraft();   // the work is in its project file now: the draft goes
            return true;
        }

        /// <summary>
        /// Before the open project is left (New, Open, closing the window): saves its changes with
        /// Autosave on, else asks. Returns false when the user wants to stay. Work that has no
        /// project file is never asked about.
        /// </summary>
        private bool ConfirmLeaveProject()
        {
            if (_projectPath == null) return true;
            _projectTimer.Stop();
            grdFiles.EndEdit();
            if (CaptureState() == _savedState) return true;

            string name = Path.GetFileName(_projectPath);
            if (AutosaveOn)
            {
                // Without its own message: the question below says what went wrong
                _autosaveErrorShown = true;
                if (SaveProjectTo(_projectPath, automatic: true)) return true;
                UpdateTitle();
                return MessageBox.Show(this,
                    $"The changes to \"{name}\" could not be saved.\n\nDo you want to continue without saving them?",
                    AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.OK;
            }

            return MessageBox.Show(this, $"Do you want to save the changes to \"{name}\"?", AppTitle,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) switch
            {
                DialogResult.Yes => SaveProjectTo(_projectPath, automatic: false),
                DialogResult.No => true,
                _ => false
            };
        }

        // ----- The commands of the File menu -----

        /// <summary>File > New Project, and the Clear button: closes the project and empties the window.</summary>
        private void NewProject()
        {
            if (IsBusy || !ConfirmLeaveProject()) return;

            LeaveDraft();   // work without a project stays behind as a draft (File > Open Draft)
            _projectTimer.Stop();
            _projectPath = null;
            _openedData = null;
            _backupPending = false;
            ResetFields();
            UpdateTitle();
        }

        private void OpenProject()
        {
            if (IsBusy || !ConfirmLeaveProject()) return;
            UpdateTitle();   // the changes were just saved

            using var dlg = new OpenFileDialog
            {
                Title = "Open Project",
                Filter = ProjectOpenFilter(),
                InitialDirectory = ProjectBrowseFolder()
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            LoadProjectFile(dlg.FileName);
        }

        private void SaveProject()
        {
            if (_projectPath == null)
            {
                SaveProjectAs();
                return;
            }
            _projectTimer.Stop();
            grdFiles.EndEdit();
            SaveProjectTo(_projectPath, automatic: false);
            UpdateTitle();
        }

        private bool SaveProjectAs()
        {
            grdFiles.EndEdit();

            string extension = AppSettings.ProjectFileExtension;
            string audio = txtMP3File.Text.Trim();
            string name = _projectPath != null ? Path.GetFileName(_projectPath)
                : (audio.Length > 0 ? SafeFileNameWithoutExtension(audio) : "Untitled") + extension;

            using var dlg = new SaveFileDialog
            {
                Title = "Save Project As",
                Filter = $"Project files (*{extension})|*{extension}|All files (*.*)|*.*",
                // Added below instead: the dialog can't add an extension like ".mp3tomp4.json"
                AddExtension = false,
                OverwritePrompt = true,
                InitialDirectory = ProjectBrowseFolder(),
                FileName = name
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return false;

            string path = dlg.FileName;
            if (dlg.FilterIndex == 1 && !HasProjectExtension(path, extension))
            {
                path += extension;
                // The dialog asked about the name as typed, not about this one
                if (File.Exists(path) && MessageBox.Show(this,
                        $"\"{path}\" already exists.\n\nDo you want to replace it?", AppTitle,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                    return false;
            }

            if (IsFileOfProject(path))
            {
                MessageBox.Show(this, "The project file cannot be the same as one of the files it uses.",
                    AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            _projectTimer.Stop();
            bool sameFile = ProjectFiles.SamePath(path, _projectPath);
            // Another file that is replaced is kept as a backup copy, too
            if (!sameFile && File.Exists(path)) ProjectFiles.CreateBackup(path);

            if (!SaveProjectTo(path, automatic: false)) return false;
            ProjectFiles.AddRecentProject(_projectPath ?? path);

            if (!sameFile)
            {
                // A new file: this moment is what Undo All Changes goes back to
                _backupPending = false;
                _openedData = CaptureProject(Path.GetDirectoryName(path));
                _openedState = _savedState;
            }
            UpdateTitle();
            return true;
        }

        private void UndoAllChanges()
        {
            if (IsBusy || _projectPath == null || _openedData == null) return;

            if (MessageBox.Show(this,
                    $"This restores \"{Path.GetFileName(_projectPath)}\" to what it was when you opened it. " +
                    "All changes made since then are lost.\n\nDo you want to continue?",
                    AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;

            _projectTimer.Stop();
            grdFiles.CancelEdit();
            ApplyProject(_openedData, Path.GetDirectoryName(_projectPath));
            CheckProjectChanges();   // with Autosave on, the file is restored as well
        }

        private void SetAutosave(bool on)
        {
            AppSettings.AutosaveProjects = on ? "Yes" : "No";
            menuAutosave.Checked = on;
            CheckProjectChanges();   // switched on: changes that were waiting are saved now
        }

        private void OpenBackupFolder()
        {
            try
            {
                // Created if missing, so the right folder opens (empty before the first backup copy)
                Directory.CreateDirectory(ProjectFiles.BackupFolder);
                FolderOpener.Open(ProjectFiles.BackupFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Could not open the folder.\n\n{ex.Message}", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuNewProject_Click(object? sender, EventArgs e) => NewProject();
        private void menuOpenProject_Click(object? sender, EventArgs e) => OpenProject();
        private void menuSaveProject_Click(object? sender, EventArgs e) => SaveProject();
        private void menuSaveProjectAs_Click(object? sender, EventArgs e) => SaveProjectAs();
        private void menuUndoAllChanges_Click(object? sender, EventArgs e) => UndoAllChanges();
        private void menuAutosave_Click(object? sender, EventArgs e) => SetAutosave(!AutosaveOn);
        private void menuOpenBackupFolder_Click(object? sender, EventArgs e) => OpenBackupFolder();
        private void menuExit_Click(object? sender, EventArgs e) => Close();

        // ----- Helpers for the Open and Save dialogs -----

        /// <summary>
        /// Where the project dialogs start: the open project's folder, else Settings > Default
        /// Project Folder, else the audio file's folder ("" leaves it to Windows).
        /// </summary>
        private string ProjectBrowseFolder()
        {
            if (_projectPath != null && GetBrowseFolder(_projectPath) is { } projectFolder)
                return projectFolder;

            string folder = AppSettings.DefaultProjectFolder;
            if (folder.Length > 0 && Directory.Exists(folder)) return folder;

            return GetBrowseFolder(txtMP3File.Text) ?? "";
        }

        /// <summary>*.json, the suggested extensions and the one chosen in Settings.</summary>
        private static string ProjectOpenFilter()
        {
            var patterns = new List<string>();
            foreach (string extension in ProjectFiles.SuggestedExtensions.Append(AppSettings.ProjectFileExtension))
            {
                // "*.json" already lists "*.mp3tomp4.json"
                bool covered = patterns.Any(p => extension.EndsWith(p[1..], StringComparison.OrdinalIgnoreCase));
                if (!covered) patterns.Add("*" + extension);
            }
            string all = string.Join(";", patterns);
            return $"Project files ({all})|{all}|All files (*.*)|*.*";
        }

        private static bool HasProjectExtension(string path, string extension) =>
            ProjectFiles.SuggestedExtensions.Append(extension)
                .Any(e => path.EndsWith(e, StringComparison.OrdinalIgnoreCase));

        private static string SafeFileNameWithoutExtension(string path)
        {
            try
            {
                string name = Path.GetFileNameWithoutExtension(path);
                return name.Length > 0 && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 ? name : "Untitled";
            }
            catch (ArgumentException)
            {
                return "Untitled";
            }
        }

        /// <summary>True when <paramref name="path"/> is the audio, image, video or output file of the window.</summary>
        private bool IsFileOfProject(string path) =>
            ProjectFiles.SamePath(path, txtMP3File.Text)
            || ProjectFiles.SamePath(path, txtMP4File.Text)
            || ProjectFiles.SamePath(path, cboImageFile.Text)
            || GetGridDataRows().Any(r => ProjectFiles.SamePath(path, r.Cells[colFile.Index].Value?.ToString()));
    }
}
