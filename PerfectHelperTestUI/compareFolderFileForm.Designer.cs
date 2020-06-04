namespace PerfectHelperTestUI
{
    partial class compareFolderFileForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.srcTBox = new System.Windows.Forms.TextBox();
            this.selectSrcBtn = new System.Windows.Forms.Button();
            this.dstLbl = new System.Windows.Forms.Label();
            this.dstTBox = new System.Windows.Forms.TextBox();
            this.selectDstBtn = new System.Windows.Forms.Button();
            this.compareDGView = new System.Windows.Forms.DataGridView();
            this.srcFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dstFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.compareBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.compareDGView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Src";
            // 
            // srcTBox
            // 
            this.srcTBox.Location = new System.Drawing.Point(57, 11);
            this.srcTBox.Name = "srcTBox";
            this.srcTBox.Size = new System.Drawing.Size(383, 21);
            this.srcTBox.TabIndex = 1;
            // 
            // selectSrcBtn
            // 
            this.selectSrcBtn.Location = new System.Drawing.Point(446, 10);
            this.selectSrcBtn.Name = "selectSrcBtn";
            this.selectSrcBtn.Size = new System.Drawing.Size(75, 23);
            this.selectSrcBtn.TabIndex = 2;
            this.selectSrcBtn.Text = "选择";
            this.selectSrcBtn.UseVisualStyleBackColor = true;
            this.selectSrcBtn.Click += new System.EventHandler(this.selectSrcBtn_Click);
            // 
            // dstLbl
            // 
            this.dstLbl.AutoSize = true;
            this.dstLbl.Location = new System.Drawing.Point(13, 44);
            this.dstLbl.Name = "dstLbl";
            this.dstLbl.Size = new System.Drawing.Size(23, 12);
            this.dstLbl.TabIndex = 0;
            this.dstLbl.Text = "Dst";
            // 
            // dstTBox
            // 
            this.dstTBox.Location = new System.Drawing.Point(57, 40);
            this.dstTBox.Name = "dstTBox";
            this.dstTBox.Size = new System.Drawing.Size(383, 21);
            this.dstTBox.TabIndex = 1;
            // 
            // selectDstBtn
            // 
            this.selectDstBtn.Location = new System.Drawing.Point(446, 39);
            this.selectDstBtn.Name = "selectDstBtn";
            this.selectDstBtn.Size = new System.Drawing.Size(75, 23);
            this.selectDstBtn.TabIndex = 2;
            this.selectDstBtn.Text = "选择";
            this.selectDstBtn.UseVisualStyleBackColor = true;
            this.selectDstBtn.Click += new System.EventHandler(this.selectDstBtn_Click);
            // 
            // compareDGView
            // 
            this.compareDGView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.compareDGView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.srcFileName,
            this.dstFileName});
            this.compareDGView.Location = new System.Drawing.Point(13, 73);
            this.compareDGView.Name = "compareDGView";
            this.compareDGView.RowTemplate.Height = 23;
            this.compareDGView.Size = new System.Drawing.Size(529, 346);
            this.compareDGView.TabIndex = 3;
            // 
            // srcFileName
            // 
            this.srcFileName.HeaderText = "srcFileName";
            this.srcFileName.Name = "srcFileName";
            // 
            // dstFileName
            // 
            this.dstFileName.HeaderText = "dstFileName";
            this.dstFileName.Name = "dstFileName";
            // 
            // compareBtn
            // 
            this.compareBtn.Location = new System.Drawing.Point(15, 426);
            this.compareBtn.Name = "compareBtn";
            this.compareBtn.Size = new System.Drawing.Size(75, 23);
            this.compareBtn.TabIndex = 4;
            this.compareBtn.Text = "compare";
            this.compareBtn.UseVisualStyleBackColor = true;
            this.compareBtn.Click += new System.EventHandler(this.compareBtn_Click);
            // 
            // compareFolderFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 494);
            this.Controls.Add(this.compareBtn);
            this.Controls.Add(this.compareDGView);
            this.Controls.Add(this.selectDstBtn);
            this.Controls.Add(this.dstTBox);
            this.Controls.Add(this.selectSrcBtn);
            this.Controls.Add(this.dstLbl);
            this.Controls.Add(this.srcTBox);
            this.Controls.Add(this.label1);
            this.Name = "compareFolderFileForm";
            this.Text = "比较两个文件夹的文件";
            ((System.ComponentModel.ISupportInitialize)(this.compareDGView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox srcTBox;
        private System.Windows.Forms.Button selectSrcBtn;
        private System.Windows.Forms.Label dstLbl;
        private System.Windows.Forms.TextBox dstTBox;
        private System.Windows.Forms.Button selectDstBtn;
        private System.Windows.Forms.DataGridView compareDGView;
        private System.Windows.Forms.DataGridViewTextBoxColumn srcFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dstFileName;
        private System.Windows.Forms.Button compareBtn;
    }
}