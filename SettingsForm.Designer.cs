namespace MP3toMP4
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            grpUpdate = new GroupBox();
            cboNewVersionCheck = new ComboBox();
            lblNewVersionCheck = new Label();
            grpOutputFolder = new GroupBox();
            btnBrowseDefaultOutputFolder = new Button();
            txtDefaultOutputFolder = new TextBox();
            grpActionWhenFinished = new GroupBox();
            btnBrowseCustomSoundFile = new Button();
            txtCustomSoundFile = new TextBox();
            lblCustomSoundFile = new Label();
            btnPlayFinishedSound = new Button();
            cboFinishedSound = new ComboBox();
            lblFinishedSound = new Label();
            cboActionWhenFinished = new ComboBox();
            lblAction = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            grpUpdate.SuspendLayout();
            grpOutputFolder.SuspendLayout();
            grpActionWhenFinished.SuspendLayout();
            SuspendLayout();
            //
            // grpUpdate
            //
            grpUpdate.Controls.Add(cboNewVersionCheck);
            grpUpdate.Controls.Add(lblNewVersionCheck);
            grpUpdate.Location = new Point(12, 12);
            grpUpdate.Name = "grpUpdate";
            grpUpdate.Size = new Size(360, 90);
            grpUpdate.TabIndex = 0;
            grpUpdate.TabStop = false;
            grpUpdate.Text = "Updates";
            //
            // cboNewVersionCheck
            //
            cboNewVersionCheck.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNewVersionCheck.FormattingEnabled = true;
            cboNewVersionCheck.Items.AddRange(new object[] { "Yes", "No" });
            cboNewVersionCheck.Location = new Point(10, 46);
            cboNewVersionCheck.Name = "cboNewVersionCheck";
            cboNewVersionCheck.Size = new Size(218, 23);
            cboNewVersionCheck.TabIndex = 1;
            //
            // lblNewVersionCheck
            //
            lblNewVersionCheck.AutoSize = true;
            lblNewVersionCheck.Location = new Point(10, 28);
            lblNewVersionCheck.Name = "lblNewVersionCheck";
            lblNewVersionCheck.TabIndex = 0;
            lblNewVersionCheck.Text = "Check for &New Version at Startup";
            //
            // grpOutputFolder
            //
            grpOutputFolder.Controls.Add(btnBrowseDefaultOutputFolder);
            grpOutputFolder.Controls.Add(txtDefaultOutputFolder);
            grpOutputFolder.Location = new Point(12, 108);
            grpOutputFolder.Name = "grpOutputFolder";
            grpOutputFolder.Size = new Size(360, 62);
            grpOutputFolder.TabIndex = 1;
            grpOutputFolder.TabStop = false;
            grpOutputFolder.Text = "Default Output Folder";
            //
            // txtDefaultOutputFolder
            //
            txtDefaultOutputFolder.Location = new Point(10, 24);
            txtDefaultOutputFolder.Name = "txtDefaultOutputFolder";
            txtDefaultOutputFolder.Size = new Size(300, 23);
            txtDefaultOutputFolder.TabIndex = 0;
            //
            // btnBrowseDefaultOutputFolder
            //
            btnBrowseDefaultOutputFolder.Location = new Point(316, 23);
            btnBrowseDefaultOutputFolder.Name = "btnBrowseDefaultOutputFolder";
            btnBrowseDefaultOutputFolder.Size = new Size(34, 25);
            btnBrowseDefaultOutputFolder.TabIndex = 1;
            btnBrowseDefaultOutputFolder.Text = "...";
            btnBrowseDefaultOutputFolder.UseVisualStyleBackColor = true;
            btnBrowseDefaultOutputFolder.Click += btnBrowseDefaultOutputFolder_Click;
            //
            // grpActionWhenFinished
            //
            grpActionWhenFinished.Controls.Add(btnBrowseCustomSoundFile);
            grpActionWhenFinished.Controls.Add(txtCustomSoundFile);
            grpActionWhenFinished.Controls.Add(lblCustomSoundFile);
            grpActionWhenFinished.Controls.Add(btnPlayFinishedSound);
            grpActionWhenFinished.Controls.Add(cboFinishedSound);
            grpActionWhenFinished.Controls.Add(lblFinishedSound);
            grpActionWhenFinished.Controls.Add(cboActionWhenFinished);
            grpActionWhenFinished.Controls.Add(lblAction);
            grpActionWhenFinished.Location = new Point(12, 176);
            grpActionWhenFinished.Name = "grpActionWhenFinished";
            grpActionWhenFinished.Size = new Size(360, 210);
            grpActionWhenFinished.TabIndex = 2;
            grpActionWhenFinished.TabStop = false;
            grpActionWhenFinished.Text = "When the Conversion Is Finished";
            //
            // lblAction
            //
            lblAction.AutoSize = true;
            lblAction.Location = new Point(10, 28);
            lblAction.Name = "lblAction";
            lblAction.TabIndex = 0;
            lblAction.Text = "&Action";
            //
            // cboActionWhenFinished
            //
            cboActionWhenFinished.DropDownStyle = ComboBoxStyle.DropDownList;
            cboActionWhenFinished.FormattingEnabled = true;
            cboActionWhenFinished.Items.AddRange(new object[] { "Play a Sound", "Message Box", "None" });
            cboActionWhenFinished.Location = new Point(10, 46);
            cboActionWhenFinished.Name = "cboActionWhenFinished";
            cboActionWhenFinished.Size = new Size(340, 23);
            cboActionWhenFinished.TabIndex = 1;
            //
            // lblFinishedSound
            //
            lblFinishedSound.AutoSize = true;
            lblFinishedSound.Location = new Point(10, 88);
            lblFinishedSound.Name = "lblFinishedSound";
            lblFinishedSound.TabIndex = 2;
            lblFinishedSound.Text = "&Sound";
            lblFinishedSound.Visible = false;
            //
            // cboFinishedSound
            //
            cboFinishedSound.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFinishedSound.FormattingEnabled = true;
            cboFinishedSound.Location = new Point(10, 106);
            cboFinishedSound.Name = "cboFinishedSound";
            cboFinishedSound.Size = new Size(300, 23);
            cboFinishedSound.TabIndex = 3;
            cboFinishedSound.Visible = false;
            //
            // btnPlayFinishedSound
            //
            btnPlayFinishedSound.Location = new Point(316, 106);
            btnPlayFinishedSound.Name = "btnPlayFinishedSound";
            btnPlayFinishedSound.Size = new Size(34, 23);
            btnPlayFinishedSound.TabIndex = 4;
            btnPlayFinishedSound.Text = "▶";
            btnPlayFinishedSound.UseVisualStyleBackColor = true;
            btnPlayFinishedSound.Visible = false;
            btnPlayFinishedSound.Click += BtnPlayFinishedSound_Click;
            //
            // lblCustomSoundFile
            //
            lblCustomSoundFile.AutoSize = true;
            lblCustomSoundFile.Location = new Point(10, 146);
            lblCustomSoundFile.Name = "lblCustomSoundFile";
            lblCustomSoundFile.TabIndex = 5;
            lblCustomSoundFile.Text = "&Custom Sound File (WAV)";
            lblCustomSoundFile.Visible = false;
            //
            // txtCustomSoundFile
            //
            txtCustomSoundFile.Location = new Point(10, 164);
            txtCustomSoundFile.Name = "txtCustomSoundFile";
            txtCustomSoundFile.Size = new Size(300, 23);
            txtCustomSoundFile.TabIndex = 6;
            txtCustomSoundFile.Visible = false;
            //
            // btnBrowseCustomSoundFile
            //
            btnBrowseCustomSoundFile.Location = new Point(316, 163);
            btnBrowseCustomSoundFile.Name = "btnBrowseCustomSoundFile";
            btnBrowseCustomSoundFile.Size = new Size(34, 25);
            btnBrowseCustomSoundFile.TabIndex = 7;
            btnBrowseCustomSoundFile.Text = "...";
            btnBrowseCustomSoundFile.UseVisualStyleBackColor = true;
            btnBrowseCustomSoundFile.Visible = false;
            btnBrowseCustomSoundFile.Click += BtnBrowseCustomSoundFile_Click;
            //
            // btnOK
            //
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(216, 398);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 27);
            btnOK.TabIndex = 3;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(297, 398);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 27);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // SettingsForm
            //
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(384, 437);
            ControlBox = false;
            Controls.Add(grpUpdate);
            Controls.Add(grpOutputFolder);
            Controls.Add(grpActionWhenFinished);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            Load += SettingsForm_Load;
            grpUpdate.ResumeLayout(false);
            grpUpdate.PerformLayout();
            grpOutputFolder.ResumeLayout(false);
            grpOutputFolder.PerformLayout();
            grpActionWhenFinished.ResumeLayout(false);
            grpActionWhenFinished.PerformLayout();
            ResumeLayout(false);
        }

        private GroupBox grpUpdate;
        private ComboBox cboNewVersionCheck;
        private Label lblNewVersionCheck;
        private GroupBox grpOutputFolder;
        private TextBox txtDefaultOutputFolder;
        private Button btnBrowseDefaultOutputFolder;
        private GroupBox grpActionWhenFinished;
        private Label lblAction;
        private ComboBox cboActionWhenFinished;
        private Label lblFinishedSound;
        private ComboBox cboFinishedSound;
        private Button btnPlayFinishedSound;
        private Label lblCustomSoundFile;
        private TextBox txtCustomSoundFile;
        private Button btnBrowseCustomSoundFile;
        private Button btnOK;
        private Button btnCancel;
    }
}
