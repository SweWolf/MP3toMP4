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
            txtMP3File = new TextBox();
            label1 = new Label();
            label2 = new Label();
            cboImageFile = new ComboBox();
            btnBrowseForMp3File = new Button();
            btnBrowsForImageFile = new Button();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            txtMP4File = new TextBox();
            label3 = new Label();
            cmdBrowseForMP4File = new Button();
            btnConvert = new Button();
            lblEstimatedRemaining = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
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
            label2.Location = new Point(20, 114);
            label2.Name = "label2";
            label2.Size = new Size(81, 21);
            label2.TabIndex = 2;
            label2.Text = "Image File";
            // 
            // cboImageFile
            // 
            cboImageFile.Font = new Font("Segoe UI", 12F);
            cboImageFile.FormattingEnabled = true;
            cboImageFile.Location = new Point(20, 138);
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
            btnBrowsForImageFile.Location = new Point(696, 138);
            btnBrowsForImageFile.Name = "btnBrowsForImageFile";
            btnBrowsForImageFile.Size = new Size(37, 29);
            btnBrowsForImageFile.TabIndex = 3;
            btnBrowsForImageFile.Text = "...";
            btnBrowsForImageFile.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnBrowsForImageFile);
            groupBox1.Controls.Add(txtMP3File);
            groupBox1.Controls.Add(btnBrowseForMp3File);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(cboImageFile);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(767, 182);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Input";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtMP4File);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(cmdBrowseForMP4File);
            groupBox2.Location = new Point(10, 229);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(772, 103);
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
            btnConvert.Font = new Font("Segoe UI", 12F);
            btnConvert.Location = new Point(32, 352);
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
            lblEstimatedRemaining.Location = new Point(12, 406);
            lblEstimatedRemaining.Name = "lblEstimatedRemaining";
            lblEstimatedRemaining.Size = new Size(186, 19);
            lblEstimatedRemaining.TabIndex = 15;
            lblEstimatedRemaining.Text = "Estimated remaining time: —";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 465);
            Controls.Add(lblEstimatedRemaining);
            Controls.Add(btnConvert);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MP3 to MP4";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
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
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private TextBox txtMP4File;
        private Label label3;
        private Button cmdBrowseForMP4File;
        private Button btnConvert;
        private Label lblEstimatedRemaining;
    }
}
