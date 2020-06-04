namespace PackMultiZip
{
    partial class PathForm
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
            this.pathDGView = new System.Windows.Forms.DataGridView();
            this.packZipBtn = new System.Windows.Forms.Button();
            this.FolderName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FolderPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excludeFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pathDGView)).BeginInit();
            this.SuspendLayout();
            // 
            // pathDGView
            // 
            this.pathDGView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pathDGView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FolderName,
            this.FolderPath,
            this.excludeFolder});
            this.pathDGView.Location = new System.Drawing.Point(13, 13);
            this.pathDGView.Name = "pathDGView";
            this.pathDGView.RowTemplate.Height = 23;
            this.pathDGView.Size = new System.Drawing.Size(564, 318);
            this.pathDGView.TabIndex = 0;
            // 
            // packZipBtn
            // 
            this.packZipBtn.Location = new System.Drawing.Point(13, 338);
            this.packZipBtn.Name = "packZipBtn";
            this.packZipBtn.Size = new System.Drawing.Size(75, 23);
            this.packZipBtn.TabIndex = 1;
            this.packZipBtn.Text = "打包zip";
            this.packZipBtn.UseVisualStyleBackColor = true;
            this.packZipBtn.Click += new System.EventHandler(this.packZipBtn_Click);
            // 
            // FolderName
            // 
            this.FolderName.HeaderText = "文件夹名";
            this.FolderName.Name = "FolderName";
            this.FolderName.Width = 150;
            // 
            // FolderPath
            // 
            this.FolderPath.HeaderText = "文件夹路径";
            this.FolderPath.Name = "FolderPath";
            this.FolderPath.Width = 300;
            // 
            // excludeFolder
            // 
            this.excludeFolder.HeaderText = "排除文件夹";
            this.excludeFolder.Name = "excludeFolder";
            // 
            // PathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 437);
            this.Controls.Add(this.packZipBtn);
            this.Controls.Add(this.pathDGView);
            this.Name = "PathForm";
            this.Text = "PathForm";
            ((System.ComponentModel.ISupportInitialize)(this.pathDGView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView pathDGView;
        private System.Windows.Forms.Button packZipBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FolderName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FolderPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn excludeFolder;
    }
}