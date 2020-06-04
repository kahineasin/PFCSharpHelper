namespace PerfectHelperTestUI
{
    partial class saveToUtf8Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.selectFileBtn = new System.Windows.Forms.Button();
            this.encodeCBox = new System.Windows.Forms.ComboBox();
            this.fileDGView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.startConvertBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileEncode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.fileDGView)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectFileBtn
            // 
            this.selectFileBtn.Location = new System.Drawing.Point(12, 12);
            this.selectFileBtn.Name = "selectFileBtn";
            this.selectFileBtn.Size = new System.Drawing.Size(75, 23);
            this.selectFileBtn.TabIndex = 0;
            this.selectFileBtn.Text = "selectFile";
            this.selectFileBtn.UseVisualStyleBackColor = true;
            this.selectFileBtn.Click += new System.EventHandler(this.selectFileBtn_Click);
            // 
            // encodeCBox
            // 
            this.encodeCBox.FormattingEnabled = true;
            this.encodeCBox.Location = new System.Drawing.Point(52, 9);
            this.encodeCBox.Name = "encodeCBox";
            this.encodeCBox.Size = new System.Drawing.Size(121, 20);
            this.encodeCBox.TabIndex = 1;
            // 
            // fileDGView
            // 
            this.fileDGView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileDGView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileDGView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName,
            this.FileEncode});
            this.fileDGView.Location = new System.Drawing.Point(12, 41);
            this.fileDGView.Name = "fileDGView";
            this.fileDGView.RowTemplate.Height = 23;
            this.fileDGView.Size = new System.Drawing.Size(636, 331);
            this.fileDGView.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "转换为";
            // 
            // startConvertBtn
            // 
            this.startConvertBtn.Location = new System.Drawing.Point(179, 8);
            this.startConvertBtn.Name = "startConvertBtn";
            this.startConvertBtn.Size = new System.Drawing.Size(94, 23);
            this.startConvertBtn.TabIndex = 4;
            this.startConvertBtn.Text = "startConvert";
            this.startConvertBtn.UseVisualStyleBackColor = true;
            this.startConvertBtn.Click += new System.EventHandler(this.startConvertBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.encodeCBox);
            this.panel1.Controls.Add(this.startConvertBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(13, 378);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(635, 37);
            this.panel1.TabIndex = 5;
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.FileName.HeaderText = "FileName";
            this.FileName.Name = "FileName";
            this.FileName.Width = 78;
            // 
            // FileEncode
            // 
            this.FileEncode.HeaderText = "FileEncode";
            this.FileEncode.Name = "FileEncode";
            // 
            // saveToUtf8Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 427);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.fileDGView);
            this.Controls.Add(this.selectFileBtn);
            this.Name = "saveToUtf8Form";
            this.Text = "saveToUtf8Form";
            ((System.ComponentModel.ISupportInitialize)(this.fileDGView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Button selectFileBtn;
        private System.Windows.Forms.ComboBox encodeCBox;
        private System.Windows.Forms.DataGridView fileDGView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button startConvertBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileEncode;
    }
}