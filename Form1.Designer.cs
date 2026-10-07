namespace MP3toMP4
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            txtMP3File = new TextBox();
            toolTip = new ToolTip(components);
            btnBrowseForMp3File = new Button();
            btnBrowsForImageFile = new Button();
            btnAdd = new Button();
            btnDelete = new Button();
            btnMoveDown = new Button();
            btnMoveUp = new Button();
            cmdBrowseForMP4File = new Button();
            btnConvert = new Button();
            lblLength = new Label();
            label1 = new Label();
            label2 = new Label();
            cboImageFile = new ComboBox();
            grpInput = new GroupBox();
            lblFadeOut = new Label();
            lblFadeIn = new Label();
            txtFadeOutLength = new TextBox();
            txtFadeInLength = new TextBox();
            label4 = new Label();
            lblInputStart = new Label();
            txtInputEnd = new TextBox();
            txtInputStart = new TextBox();
            chkTrimToRange = new CheckBox();
            tabImage = new TabControl();
            tabSingle = new TabPage();
            txtVideoText = new TextBox();
            radText = new RadioButton();
            radBlack = new RadioButton();
            radFile = new RadioButton();
            chkUseImageFileFromMp3File = new CheckBox();
            picImage = new PictureBox();
            tabMultiple = new TabPage();
            pictureBox2 = new PictureBox();
            grdFiles = new DataGridView();
            colFile = new DataGridViewTextBoxColumn();
            colStart = new DataGridViewTextBoxColumn();
            colDuration = new DataGridViewTextBoxColumn();
            chkLyrics = new CheckBox();
            groupBox2 = new GroupBox();
            txtMP4File = new TextBox();
            label3 = new Label();
            lblEstimatedRemaining = new Label();
            progressBar = new TextProgressBar();
            btnCancel = new Button();
            btnClear = new Button();
            menuStrip = new MenuStrip();
            menuTools = new ToolStripMenuItem();
            menuCreateShortcut = new ToolStripMenuItem();
            menuSettings = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            menuViewHelp = new ToolStripMenuItem();
            menuViewHelpSeparator = new ToolStripSeparator();
            menuOpenFFmpegCallLog = new ToolStripMenuItem();
            menuOpenLogFolder = new ToolStripMenuItem();
            menuHelpSeparator = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            btnOpenLogFile = new Button();
            btnOpenFolder = new Button();
            btnOpenFile = new Button();
            grpInput.SuspendLayout();
            tabImage.SuspendLayout();
            tabSingle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImage).BeginInit();
            tabMultiple.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grdFiles).BeginInit();
            groupBox2.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // txtMP3File
            // 
            txtMP3File.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtMP3File.Location = new Point(20, 46);
            txtMP3File.Name = "txtMP3File";
            txtMP3File.Size = new Size(645, 29);
            txtMP3File.TabIndex = 0;
            // 
            // toolTip
            // 
            toolTip.AutoPopDelay = 15000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 100;
            // 
            // btnBrowseForMp3File
            // 
            btnBrowseForMp3File.Location = new Point(696, 46);
            btnBrowseForMp3File.Name = "btnBrowseForMp3File";
            btnBrowseForMp3File.Size = new Size(37, 29);
            btnBrowseForMp3File.TabIndex = 1;
            btnBrowseForMp3File.Text = "...";
            toolTip.SetToolTip(btnBrowseForMp3File, "Browse for an MP3 file");
            btnBrowseForMp3File.UseVisualStyleBackColor = true;
            // 
            // btnBrowsForImageFile
            // 
            btnBrowsForImageFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowsForImageFile.Location = new Point(676, 37);
            btnBrowsForImageFile.Name = "btnBrowsForImageFile";
            btnBrowsForImageFile.Size = new Size(37, 29);
            btnBrowsForImageFile.TabIndex = 4;
            btnBrowsForImageFile.Text = "...";
            toolTip.SetToolTip(btnBrowsForImageFile, "Browse for an image or video file");
            btnBrowsForImageFile.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Location = new Point(624, 17);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(72, 34);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "Add...";
            toolTip.SetToolTip(btnAdd, "Add image or video files");
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Location = new Point(624, 164);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(72, 34);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Delete";
            toolTip.SetToolTip(btnDelete, "Delete the selected row");
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveDown.Location = new Point(624, 115);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(72, 34);
            btnMoveDown.TabIndex = 3;
            btnMoveDown.Text = "▼";
            toolTip.SetToolTip(btnMoveDown, "Move the selected row down");
            btnMoveDown.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            btnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveUp.Location = new Point(624, 66);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(72, 34);
            btnMoveUp.TabIndex = 2;
            btnMoveUp.Text = "▲";
            toolTip.SetToolTip(btnMoveUp, "Move the selected row up");
            btnMoveUp.UseVisualStyleBackColor = true;
            // 
            // cmdBrowseForMP4File
            // 
            cmdBrowseForMP4File.Location = new Point(698, 52);
            cmdBrowseForMP4File.Name = "cmdBrowseForMP4File";
            cmdBrowseForMP4File.Size = new Size(37, 29);
            cmdBrowseForMP4File.TabIndex = 1;
            cmdBrowseForMP4File.Text = "...";
            toolTip.SetToolTip(cmdBrowseForMP4File, "Choose where to save the MP4 or MKV file");
            cmdBrowseForMP4File.UseVisualStyleBackColor = true;
            // 
            // btnConvert
            // 
            btnConvert.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnConvert.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConvert.Location = new Point(14, 635);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(127, 37);
            btnConvert.TabIndex = 10;
            btnConvert.Text = "Convert";
            toolTip.SetToolTip(btnConvert, "Start converting the audio file (MP3 or other type) to an MP4 or MKV file");
            btnConvert.UseVisualStyleBackColor = true;
            // 
            // lblLength
            // 
            lblLength.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLength.AutoSize = true;
            lblLength.Location = new Point(750, 53);
            lblLength.Name = "lblLength";
            lblLength.Size = new Size(0, 21);
            lblLength.TabIndex = 33;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(20, 22);
            label1.Name = "label1";
            label1.Size = new Size(70, 21);
            label1.TabIndex = 1;
            label1.Text = "MP3 File";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(31, 14);
            label2.Name = "label2";
            label2.Size = new Size(144, 21);
            label2.TabIndex = 2;
            label2.Text = "Image or Video File";
            // 
            // cboImageFile
            // 
            cboImageFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboImageFile.Font = new Font("Segoe UI", 12F);
            cboImageFile.FormattingEnabled = true;
            cboImageFile.Location = new Point(31, 38);
            cboImageFile.Name = "cboImageFile";
            cboImageFile.Size = new Size(624, 29);
            cboImageFile.TabIndex = 3;
            // 
            // grpInput
            // 
            grpInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpInput.Controls.Add(lblLength);
            grpInput.Controls.Add(lblFadeOut);
            grpInput.Controls.Add(lblFadeIn);
            grpInput.Controls.Add(txtFadeOutLength);
            grpInput.Controls.Add(txtFadeInLength);
            grpInput.Controls.Add(label4);
            grpInput.Controls.Add(lblInputStart);
            grpInput.Controls.Add(txtInputEnd);
            grpInput.Controls.Add(txtInputStart);
            grpInput.Controls.Add(chkTrimToRange);
            grpInput.Controls.Add(tabImage);
            grpInput.Controls.Add(chkLyrics);
            grpInput.Controls.Add(txtMP3File);
            grpInput.Controls.Add(btnBrowseForMp3File);
            grpInput.Controls.Add(label1);
            grpInput.Font = new Font("Segoe UI", 12F);
            grpInput.Location = new Point(12, 27);
            grpInput.Name = "grpInput";
            grpInput.Size = new Size(1042, 466);
            grpInput.TabIndex = 0;
            grpInput.TabStop = false;
            grpInput.Text = "Input";
            // 
            // lblFadeOut
            // 
            lblFadeOut.AutoSize = true;
            lblFadeOut.Location = new Point(845, 92);
            lblFadeOut.Name = "lblFadeOut";
            lblFadeOut.Size = new Size(90, 21);
            lblFadeOut.TabIndex = 31;
            lblFadeOut.Text = "Fade out (s)";
            lblFadeOut.Visible = false;
            // 
            // lblFadeIn
            // 
            lblFadeIn.AutoSize = true;
            lblFadeIn.Location = new Point(657, 92);
            lblFadeIn.Name = "lblFadeIn";
            lblFadeIn.Size = new Size(80, 21);
            lblFadeIn.TabIndex = 30;
            lblFadeIn.Text = "Fade in (s)";
            lblFadeIn.Visible = false;
            // 
            // txtFadeOutLength
            // 
            txtFadeOutLength.Location = new Point(941, 89);
            txtFadeOutLength.Name = "txtFadeOutLength";
            txtFadeOutLength.Size = new Size(82, 29);
            txtFadeOutLength.TabIndex = 6;
            txtFadeOutLength.Text = "0";
            txtFadeOutLength.Visible = false;
            // 
            // txtFadeInLength
            // 
            txtFadeInLength.Location = new Point(743, 89);
            txtFadeInLength.Name = "txtFadeInLength";
            txtFadeInLength.Size = new Size(82, 29);
            txtFadeInLength.TabIndex = 5;
            txtFadeInLength.Text = "0";
            txtFadeInLength.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(461, 92);
            label4.Name = "label4";
            label4.Size = new Size(53, 21);
            label4.TabIndex = 27;
            label4.Text = "End at";
            label4.Visible = false;
            // 
            // lblInputStart
            // 
            lblInputStart.AutoSize = true;
            lblInputStart.Location = new Point(279, 92);
            lblInputStart.Name = "lblInputStart";
            lblInputStart.Size = new Size(59, 21);
            lblInputStart.TabIndex = 26;
            lblInputStart.Text = "Start at";
            lblInputStart.Visible = false;
            // 
            // txtInputEnd
            // 
            txtInputEnd.Location = new Point(520, 89);
            txtInputEnd.Name = "txtInputEnd";
            txtInputEnd.Size = new Size(82, 29);
            txtInputEnd.TabIndex = 4;
            txtInputEnd.Visible = false;
            // 
            // txtInputStart
            // 
            txtInputStart.Location = new Point(344, 89);
            txtInputStart.Name = "txtInputStart";
            txtInputStart.Size = new Size(82, 29);
            txtInputStart.TabIndex = 3;
            txtInputStart.Visible = false;
            // 
            // chkTrimToRange
            // 
            chkTrimToRange.AutoSize = true;
            chkTrimToRange.Location = new Point(20, 91);
            chkTrimToRange.Name = "chkTrimToRange";
            chkTrimToRange.Size = new Size(138, 25);
            chkTrimToRange.TabIndex = 2;
            chkTrimToRange.Text = "Trim to a Range";
            chkTrimToRange.UseVisualStyleBackColor = true;
            // 
            // tabImage
            // 
            tabImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabImage.Controls.Add(tabSingle);
            tabImage.Controls.Add(tabMultiple);
            tabImage.Location = new Point(6, 132);
            tabImage.Name = "tabImage";
            tabImage.SelectedIndex = 0;
            tabImage.Size = new Size(1030, 260);
            tabImage.TabIndex = 7;
            // 
            // tabSingle
            // 
            tabSingle.Controls.Add(txtVideoText);
            tabSingle.Controls.Add(radText);
            tabSingle.Controls.Add(radBlack);
            tabSingle.Controls.Add(radFile);
            tabSingle.Controls.Add(cboImageFile);
            tabSingle.Controls.Add(btnBrowsForImageFile);
            tabSingle.Controls.Add(chkUseImageFileFromMp3File);
            tabSingle.Controls.Add(label2);
            tabSingle.Controls.Add(picImage);
            tabSingle.Location = new Point(4, 30);
            tabSingle.Name = "tabSingle";
            tabSingle.Padding = new Padding(3);
            tabSingle.Size = new Size(1022, 226);
            tabSingle.TabIndex = 0;
            tabSingle.Text = "Single Image/Video";
            tabSingle.UseVisualStyleBackColor = true;
            // 
            // txtVideoText
            // 
            txtVideoText.AcceptsReturn = true;
            txtVideoText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtVideoText.Location = new Point(94, 150);
            txtVideoText.Multiline = true;
            txtVideoText.Name = "txtVideoText";
            txtVideoText.Size = new Size(561, 60);
            txtVideoText.TabIndex = 6;
            txtVideoText.Visible = false;
            // 
            // radText
            // 
            radText.AutoSize = true;
            radText.Location = new Point(7, 156);
            radText.Name = "radText";
            radText.Size = new Size(62, 25);
            radText.TabIndex = 2;
            radText.TabStop = true;
            radText.Text = "  Text";
            radText.UseVisualStyleBackColor = true;
            // 
            // radBlack
            // 
            radBlack.AutoSize = true;
            radBlack.Location = new Point(7, 116);
            radBlack.Name = "radBlack";
            radBlack.Size = new Size(123, 25);
            radBlack.TabIndex = 1;
            radBlack.Text = "  Black Screen";
            radBlack.UseVisualStyleBackColor = true;
            // 
            // radFile
            // 
            radFile.AutoSize = true;
            radFile.Checked = true;
            radFile.Location = new Point(7, 18);
            radFile.Name = "radFile";
            radFile.Size = new Size(14, 13);
            radFile.TabIndex = 0;
            radFile.TabStop = true;
            radFile.UseVisualStyleBackColor = true;
            // 
            // chkUseImageFileFromMp3File
            // 
            chkUseImageFileFromMp3File.AutoSize = true;
            chkUseImageFileFromMp3File.Enabled = false;
            chkUseImageFileFromMp3File.Font = new Font("Segoe UI", 12F);
            chkUseImageFileFromMp3File.Location = new Point(31, 73);
            chkUseImageFileFromMp3File.Name = "chkUseImageFileFromMp3File";
            chkUseImageFileFromMp3File.Size = new Size(230, 25);
            chkUseImageFileFromMp3File.TabIndex = 5;
            chkUseImageFileFromMp3File.Text = "Use Image from the MP3 File";
            chkUseImageFileFromMp3File.UseVisualStyleBackColor = true;
            // 
            // picImage
            // 
            picImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            picImage.Location = new Point(733, 6);
            picImage.Name = "picImage";
            picImage.Size = new Size(280, 214);
            picImage.TabIndex = 7;
            picImage.TabStop = false;
            // 
            // tabMultiple
            // 
            tabMultiple.Controls.Add(pictureBox2);
            tabMultiple.Controls.Add(btnAdd);
            tabMultiple.Controls.Add(btnDelete);
            tabMultiple.Controls.Add(btnMoveDown);
            tabMultiple.Controls.Add(btnMoveUp);
            tabMultiple.Controls.Add(grdFiles);
            tabMultiple.Location = new Point(4, 30);
            tabMultiple.Name = "tabMultiple";
            tabMultiple.Padding = new Padding(3);
            tabMultiple.Size = new Size(1022, 226);
            tabMultiple.TabIndex = 1;
            tabMultiple.Text = "Multiple Images/Videos";
            tabMultiple.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pictureBox2.Location = new Point(719, 17);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(283, 193);
            pictureBox2.TabIndex = 24;
            pictureBox2.TabStop = false;
            // 
            // grdFiles
            // 
            grdFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdFiles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdFiles.Columns.AddRange(new DataGridViewColumn[] { colFile, colStart, colDuration });
            grdFiles.Location = new Point(12, 17);
            grdFiles.Name = "grdFiles";
            grdFiles.Size = new Size(595, 193);
            grdFiles.TabIndex = 0;
            // 
            // colFile
            // 
            colFile.HeaderText = "File";
            colFile.Name = "colFile";
            colFile.Width = 200;
            // 
            // colStart
            // 
            colStart.HeaderText = "Start Time";
            colStart.Name = "colStart";
            colStart.ToolTipText = "When this image or video appears in the finished video. The first one always starts at 0:00. Leave it blank to share the time equally with the other blank rows.";
            colStart.Width = 120;
            // 
            // colDuration
            // 
            colDuration.HeaderText = "Duration";
            colDuration.Name = "colDuration";
            colDuration.ToolTipText = resources.GetString("colDuration.ToolTipText");
            // 
            // chkLyrics
            // 
            chkLyrics.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkLyrics.AutoSize = true;
            chkLyrics.Checked = true;
            chkLyrics.CheckState = CheckState.Checked;
            chkLyrics.Location = new Point(17, 425);
            chkLyrics.Name = "chkLyrics";
            chkLyrics.Size = new Size(149, 25);
            chkLyrics.TabIndex = 8;
            chkLyrics.Text = "Include Lyrics Tag";
            chkLyrics.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox2.Controls.Add(txtMP4File);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(cmdBrowseForMP4File);
            groupBox2.Location = new Point(12, 509);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(767, 103);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Output";
            // 
            // txtMP4File
            // 
            txtMP4File.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtMP4File.Location = new Point(22, 52);
            txtMP4File.Name = "txtMP4File";
            txtMP4File.Size = new Size(645, 29);
            txtMP4File.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(22, 28);
            label3.Name = "label3";
            label3.Size = new Size(126, 21);
            label3.TabIndex = 1;
            label3.Text = "MP4 or MKV File";
            // 
            // lblEstimatedRemaining
            // 
            lblEstimatedRemaining.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblEstimatedRemaining.AutoSize = true;
            lblEstimatedRemaining.Font = new Font("Segoe UI", 10F);
            lblEstimatedRemaining.Location = new Point(12, 696);
            lblEstimatedRemaining.Name = "lblEstimatedRemaining";
            lblEstimatedRemaining.Size = new Size(186, 19);
            lblEstimatedRemaining.TabIndex = 15;
            lblEstimatedRemaining.Text = "Estimated remaining time: —";
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(12, 720);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1042, 23);
            progressBar.TabIndex = 16;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Enabled = false;
            btnCancel.Font = new Font("Segoe UI", 12F);
            btnCancel.Location = new Point(160, 635);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(127, 37);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClear.Font = new Font("Segoe UI", 12F);
            btnClear.Location = new Point(308, 635);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(127, 37);
            btnClear.TabIndex = 12;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { menuTools, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1066, 24);
            menuStrip.TabIndex = 22;
            // 
            // menuTools
            // 
            menuTools.DropDownItems.AddRange(new ToolStripItem[] { menuCreateShortcut, menuSettings });
            menuTools.Name = "menuTools";
            menuTools.Size = new Size(47, 20);
            menuTools.Text = "Tools";
            // 
            // menuCreateShortcut
            // 
            menuCreateShortcut.Name = "menuCreateShortcut";
            menuCreateShortcut.Size = new Size(165, 22);
            menuCreateShortcut.Text = "Create Shortcut...";
            menuCreateShortcut.Click += menuCreateShortcut_Click;
            // 
            // menuSettings
            // 
            menuSettings.Name = "menuSettings";
            menuSettings.Size = new Size(165, 22);
            menuSettings.Text = "Settings";
            menuSettings.Click += menuSettings_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuViewHelp, menuViewHelpSeparator, menuOpenFFmpegCallLog, menuOpenLogFolder, menuHelpSeparator, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // menuViewHelp
            // 
            menuViewHelp.Name = "menuViewHelp";
            menuViewHelp.ShortcutKeys = Keys.F1;
            menuViewHelp.Size = new Size(195, 22);
            menuViewHelp.Text = "MP3 to MP4 Help";
            menuViewHelp.Click += menuViewHelp_Click;
            // 
            // menuViewHelpSeparator
            // 
            menuViewHelpSeparator.Name = "menuViewHelpSeparator";
            menuViewHelpSeparator.Size = new Size(192, 6);
            // 
            // menuOpenFFmpegCallLog
            // 
            menuOpenFFmpegCallLog.Name = "menuOpenFFmpegCallLog";
            menuOpenFFmpegCallLog.Size = new Size(195, 22);
            menuOpenFFmpegCallLog.Text = "Open FFmpeg Call Log";
            menuOpenFFmpegCallLog.Click += menuOpenFFmpegCallLog_Click;
            // 
            // menuOpenLogFolder
            // 
            menuOpenLogFolder.Name = "menuOpenLogFolder";
            menuOpenLogFolder.Size = new Size(195, 22);
            menuOpenLogFolder.Text = "Open Log Folder";
            menuOpenLogFolder.Click += menuOpenLogFolder_Click;
            // 
            // menuHelpSeparator
            // 
            menuHelpSeparator.Name = "menuHelpSeparator";
            menuHelpSeparator.Size = new Size(192, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(195, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // btnOpenLogFile
            // 
            btnOpenLogFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenLogFile.Enabled = false;
            btnOpenLogFile.Font = new Font("Segoe UI", 12F);
            btnOpenLogFile.Location = new Point(363, 761);
            btnOpenLogFile.Name = "btnOpenLogFile";
            btnOpenLogFile.Size = new Size(153, 36);
            btnOpenLogFile.TabIndex = 15;
            btnOpenLogFile.Text = "Open Log File";
            btnOpenLogFile.UseVisualStyleBackColor = true;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenFolder.Enabled = false;
            btnOpenFolder.Font = new Font("Segoe UI", 12F);
            btnOpenFolder.Location = new Point(187, 761);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(153, 36);
            btnOpenFolder.TabIndex = 14;
            btnOpenFolder.Text = "Open Folder";
            btnOpenFolder.UseVisualStyleBackColor = true;
            // 
            // btnOpenFile
            // 
            btnOpenFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenFile.Enabled = false;
            btnOpenFile.Font = new Font("Segoe UI", 12F);
            btnOpenFile.Location = new Point(12, 761);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(153, 36);
            btnOpenFile.TabIndex = 13;
            btnOpenFile.Text = "Open File";
            btnOpenFile.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AcceptButton = btnConvert;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1066, 809);
            Controls.Add(btnOpenLogFile);
            Controls.Add(btnOpenFolder);
            Controls.Add(btnOpenFile);
            Controls.Add(btnClear);
            Controls.Add(btnCancel);
            Controls.Add(progressBar);
            Controls.Add(lblEstimatedRemaining);
            Controls.Add(btnConvert);
            Controls.Add(groupBox2);
            Controls.Add(grpInput);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MP3 to MP4";
            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();
            tabImage.ResumeLayout(false);
            tabSingle.ResumeLayout(false);
            tabSingle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picImage).EndInit();
            tabMultiple.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)grdFiles).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMP3File;
        private Label lblLength;
        private Label label1;
        private Label label2;
        private ComboBox cboImageFile;
        private Button btnBrowseForMp3File;
        private Button btnBrowsForImageFile;
        private GroupBox grpInput;
        private GroupBox groupBox2;
        private TextBox txtMP4File;
        private Label label3;
        private Button cmdBrowseForMP4File;
        private Button btnConvert;
        private Label lblEstimatedRemaining;
        private TextProgressBar progressBar;
        private PictureBox picImage;
        private Button btnCancel;
        private Button btnClear;
        private MenuStrip menuStrip;
        private ToolStripMenuItem menuTools;
        private ToolStripMenuItem menuCreateShortcut;
        private ToolStripMenuItem menuSettings;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem menuOpenFFmpegCallLog;
        private ToolStripMenuItem menuOpenLogFolder;
        private ToolStripSeparator menuHelpSeparator;
        private ToolStripMenuItem menuViewHelp;
        private ToolStripSeparator menuViewHelpSeparator;
        private CheckBox chkUseImageFileFromMp3File;
        private CheckBox chkLyrics;
        private Button btnOpenLogFile;
        private Button btnOpenFolder;
        private Button btnOpenFile;
        private TabControl tabImage;
        private TabPage tabSingle;
        private TabPage tabMultiple;
        private DataGridView grdFiles;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnMoveDown;
        private Button btnMoveUp;
        private DataGridViewTextBoxColumn colFile;
        private DataGridViewTextBoxColumn colStart;
        private DataGridViewTextBoxColumn colDuration;
        private PictureBox pictureBox2;
        private CheckBox chkTrimToRange;
        private ToolTip toolTip;
        private Label label4;
        private Label lblInputStart;
        private TextBox txtInputEnd;
        private TextBox txtInputStart;
        private Label lblFadeOut;
        private Label lblFadeIn;
        private TextBox txtFadeOutLength;
        private TextBox txtFadeInLength;
        private RadioButton radFile;
        private RadioButton radBlack;
        private TextBox txtVideoText;
        private RadioButton radText;
    }
}
