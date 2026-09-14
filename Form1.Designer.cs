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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            txtMP3File = new TextBox();
            label1 = new Label();
            label2 = new Label();
            cboImageFile = new ComboBox();
            btnBrowseForMp3File = new Button();
            btnBrowsForImageFile = new Button();
            grpInput = new GroupBox();
            lblFadeOut = new Label();
            lblFadeIn = new Label();
            txtFadeOutLength = new TextBox();
            txtFadeInLength = new TextBox();
            label4 = new Label();
            lblInputStart = new Label();
            txtInputEnd = new TextBox();
            txtInputStart = new TextBox();
            chkUseTheFullMp3File = new CheckBox();
            tabImage = new TabControl();
            tabSingle = new TabPage();
            chkUseImageFileFromMp3File = new CheckBox();
            picImage = new PictureBox();
            tabMultiple = new TabPage();
            pictureBox2 = new PictureBox();
            btnAdd = new Button();
            btnDelete = new Button();
            btnMoveDown = new Button();
            btnMoveUp = new Button();
            grdFiles = new DataGridView();
            colFile = new DataGridViewTextBoxColumn();
            colStart = new DataGridViewTextBoxColumn();
            colDuration = new DataGridViewTextBoxColumn();
            chkLyrics = new CheckBox();
            groupBox2 = new GroupBox();
            txtMP4File = new TextBox();
            label3 = new Label();
            cmdBrowseForMP4File = new Button();
            btnConvert = new Button();
            lblEstimatedRemaining = new Label();
            progressBar = new TextProgressBar();
            btnCancel = new Button();
            menuStrip = new MenuStrip();
            menuSetup = new ToolStripMenuItem();
            menuCreateShortcut = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
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
            label2.Location = new Point(12, 14);
            label2.Name = "label2";
            label2.Size = new Size(81, 21);
            label2.TabIndex = 2;
            label2.Text = "Image File";
            // 
            // cboImageFile
            // 
            cboImageFile.Font = new Font("Segoe UI", 12F);
            cboImageFile.FormattingEnabled = true;
            cboImageFile.Location = new Point(10, 38);
            cboImageFile.Name = "cboImageFile";
            cboImageFile.Size = new Size(645, 29);
            cboImageFile.TabIndex = 2;
            // 
            // btnBrowseForMp3File
            // 
            btnBrowseForMp3File.Location = new Point(696, 46);
            btnBrowseForMp3File.Name = "btnBrowseForMp3File";
            btnBrowseForMp3File.Size = new Size(37, 29);
            btnBrowseForMp3File.TabIndex = 1;
            btnBrowseForMp3File.Text = "...";
            btnBrowseForMp3File.UseVisualStyleBackColor = true;
            // 
            // btnBrowsForImageFile
            // 
            btnBrowsForImageFile.Location = new Point(686, 38);
            btnBrowsForImageFile.Name = "btnBrowsForImageFile";
            btnBrowsForImageFile.Size = new Size(37, 29);
            btnBrowsForImageFile.TabIndex = 3;
            btnBrowsForImageFile.Text = "...";
            btnBrowsForImageFile.UseVisualStyleBackColor = true;
            // 
            // grpInput
            // 
            grpInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            grpInput.Controls.Add(lblFadeOut);
            grpInput.Controls.Add(lblFadeIn);
            grpInput.Controls.Add(txtFadeOutLength);
            grpInput.Controls.Add(txtFadeInLength);
            grpInput.Controls.Add(label4);
            grpInput.Controls.Add(lblInputStart);
            grpInput.Controls.Add(txtInputEnd);
            grpInput.Controls.Add(txtInputStart);
            grpInput.Controls.Add(chkUseTheFullMp3File);
            grpInput.Controls.Add(tabImage);
            grpInput.Controls.Add(chkLyrics);
            grpInput.Controls.Add(txtMP3File);
            grpInput.Controls.Add(btnBrowseForMp3File);
            grpInput.Controls.Add(label1);
            grpInput.Font = new Font("Segoe UI", 12F);
            grpInput.Location = new Point(12, 27);
            grpInput.Name = "grpInput";
            grpInput.Size = new Size(1042, 466);
            grpInput.TabIndex = 5;
            grpInput.TabStop = false;
            grpInput.Text = "Input";
            // 
            // lblFadeOut
            // 
            lblFadeOut.AutoSize = true;
            lblFadeOut.Location = new Point(845, 82);
            lblFadeOut.Name = "lblFadeOut";
            lblFadeOut.Size = new Size(90, 21);
            lblFadeOut.TabIndex = 31;
            lblFadeOut.Text = "Fade out (s)";
            lblFadeOut.Visible = false;
            // 
            // lblFadeIn
            // 
            lblFadeIn.AutoSize = true;
            lblFadeIn.Location = new Point(657, 82);
            lblFadeIn.Name = "lblFadeIn";
            lblFadeIn.Size = new Size(80, 21);
            lblFadeIn.TabIndex = 30;
            lblFadeIn.Text = "Fade in (s)";
            lblFadeIn.Visible = false;
            // 
            // txtFadeOutLength
            // 
            txtFadeOutLength.Location = new Point(941, 79);
            txtFadeOutLength.Name = "txtFadeOutLength";
            txtFadeOutLength.Size = new Size(82, 29);
            txtFadeOutLength.TabIndex = 6;
            txtFadeOutLength.Text = "0";
            txtFadeOutLength.Visible = false;
            // 
            // txtFadeInLength
            // 
            txtFadeInLength.Location = new Point(743, 79);
            txtFadeInLength.Name = "txtFadeInLength";
            txtFadeInLength.Size = new Size(82, 29);
            txtFadeInLength.TabIndex = 5;
            txtFadeInLength.Text = "0";
            txtFadeInLength.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(461, 82);
            label4.Name = "label4";
            label4.Size = new Size(53, 21);
            label4.TabIndex = 27;
            label4.Text = "End at";
            label4.Visible = false;
            // 
            // lblInputStart
            // 
            lblInputStart.AutoSize = true;
            lblInputStart.Location = new Point(279, 82);
            lblInputStart.Name = "lblInputStart";
            lblInputStart.Size = new Size(59, 21);
            lblInputStart.TabIndex = 26;
            lblInputStart.Text = "Start at";
            lblInputStart.Visible = false;
            // 
            // txtInputEnd
            // 
            txtInputEnd.Location = new Point(520, 79);
            txtInputEnd.Name = "txtInputEnd";
            txtInputEnd.Size = new Size(82, 29);
            txtInputEnd.TabIndex = 4;
            txtInputEnd.Visible = false;
            // 
            // txtInputStart
            // 
            txtInputStart.Location = new Point(344, 79);
            txtInputStart.Name = "txtInputStart";
            txtInputStart.Size = new Size(82, 29);
            txtInputStart.TabIndex = 3;
            txtInputStart.Visible = false;
            // 
            // chkUseTheFullMp3File
            // 
            chkUseTheFullMp3File.AutoSize = true;
            chkUseTheFullMp3File.Checked = true;
            chkUseTheFullMp3File.CheckState = CheckState.Checked;
            chkUseTheFullMp3File.Location = new Point(20, 81);
            chkUseTheFullMp3File.Name = "chkUseTheFullMp3File";
            chkUseTheFullMp3File.Size = new Size(180, 25);
            chkUseTheFullMp3File.TabIndex = 2;
            chkUseTheFullMp3File.Text = "Use the Complete File";
            chkUseTheFullMp3File.UseVisualStyleBackColor = true;
            // 
            // tabImage
            // 
            tabImage.Controls.Add(tabSingle);
            tabImage.Controls.Add(tabMultiple);
            tabImage.Location = new Point(6, 122);
            tabImage.Name = "tabImage";
            tabImage.SelectedIndex = 0;
            tabImage.Size = new Size(1030, 260);
            tabImage.TabIndex = 22;
            // 
            // tabSingle
            // 
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
            tabSingle.Text = "Single Image";
            tabSingle.UseVisualStyleBackColor = true;
            // 
            // chkUseImageFileFromMp3File
            // 
            chkUseImageFileFromMp3File.AutoSize = true;
            chkUseImageFileFromMp3File.Enabled = false;
            chkUseImageFileFromMp3File.Font = new Font("Segoe UI", 12F);
            chkUseImageFileFromMp3File.Location = new Point(12, 87);
            chkUseImageFileFromMp3File.Name = "chkUseImageFileFromMp3File";
            chkUseImageFileFromMp3File.Size = new Size(232, 25);
            chkUseImageFileFromMp3File.TabIndex = 5;
            chkUseImageFileFromMp3File.Text = "Use Image File from MP3 File";
            chkUseImageFileFromMp3File.UseVisualStyleBackColor = true;
            // 
            // picImage
            // 
            picImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            picImage.Location = new Point(12, 134);
            picImage.Name = "picImage";
            picImage.Size = new Size(404, 76);
            picImage.TabIndex = 4;
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
            tabMultiple.Text = "Multiple Images";
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
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Location = new Point(624, 17);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(72, 34);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Add...";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Location = new Point(624, 164);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(72, 34);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveDown.Location = new Point(624, 115);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(72, 34);
            btnMoveDown.TabIndex = 2;
            btnMoveDown.Text = "Down";
            btnMoveDown.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            btnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveUp.Location = new Point(624, 66);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(72, 34);
            btnMoveUp.TabIndex = 1;
            btnMoveUp.Text = "Up";
            btnMoveUp.UseVisualStyleBackColor = true;
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
            colStart.Width = 120;
            // 
            // colDuration
            // 
            colDuration.HeaderText = "Duration";
            colDuration.Name = "colDuration";
            // 
            // chkLyrics
            // 
            chkLyrics.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkLyrics.AutoSize = true;
            chkLyrics.Checked = true;
            chkLyrics.CheckState = CheckState.Checked;
            chkLyrics.Location = new Point(20, 422);
            chkLyrics.Name = "chkLyrics";
            chkLyrics.Size = new Size(200, 25);
            chkLyrics.TabIndex = 6;
            chkLyrics.Text = "Include Lyrics if available";
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
            groupBox2.TabIndex = 6;
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
            label3.Size = new Size(70, 21);
            label3.TabIndex = 1;
            label3.Text = "MP4 File";
            // 
            // cmdBrowseForMP4File
            // 
            cmdBrowseForMP4File.Location = new Point(698, 52);
            cmdBrowseForMP4File.Name = "cmdBrowseForMP4File";
            cmdBrowseForMP4File.Size = new Size(37, 29);
            cmdBrowseForMP4File.TabIndex = 1;
            cmdBrowseForMP4File.Text = "...";
            cmdBrowseForMP4File.UseVisualStyleBackColor = true;
            // 
            // btnConvert
            // 
            btnConvert.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnConvert.Font = new Font("Segoe UI", 12F);
            btnConvert.Location = new Point(14, 635);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(127, 37);
            btnConvert.TabIndex = 2;
            btnConvert.Text = "Convert";
            btnConvert.UseVisualStyleBackColor = true;
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
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
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
            btnCancel.TabIndex = 17;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { menuSetup, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1066, 24);
            menuStrip.TabIndex = 18;
            // 
            // menuSetup
            // 
            menuSetup.DropDownItems.AddRange(new ToolStripItem[] { menuCreateShortcut });
            menuSetup.Name = "menuSetup";
            menuSetup.Size = new Size(49, 20);
            menuSetup.Text = "Setup";
            // 
            // menuCreateShortcut
            // 
            menuCreateShortcut.Name = "menuCreateShortcut";
            menuCreateShortcut.Size = new Size(165, 22);
            menuCreateShortcut.Text = "Create Shortcut...";
            menuCreateShortcut.Click += menuCreateShortcut_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(116, 22);
            aboutToolStripMenuItem.Text = "About...";
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
            btnOpenLogFile.TabIndex = 21;
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
            btnOpenFolder.TabIndex = 20;
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
            btnOpenFile.TabIndex = 19;
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
        private MenuStrip menuStrip;
        private ToolStripMenuItem menuSetup;
        private ToolStripMenuItem menuCreateShortcut;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
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
        private CheckBox chkUseTheFullMp3File;
        private Label label4;
        private Label lblInputStart;
        private TextBox txtInputEnd;
        private TextBox txtInputStart;
        private Label lblFadeOut;
        private Label lblFadeIn;
        private TextBox txtFadeOutLength;
        private TextBox txtFadeInLength;
    }
}
