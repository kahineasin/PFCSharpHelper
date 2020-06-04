namespace FieldSetsManager
{
    partial class SearchForm
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
            this.tbField = new System.Windows.Forms.TextBox();
            this.cbSys = new System.Windows.Forms.ComboBox();
            this.dgvFieldSet = new System.Windows.Forms.DataGridView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.DataSet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hideDataSet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.tbLose = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFieldSet)).BeginInit();
            this.SuspendLayout();
            // 
            // tbField
            // 
            this.tbField.Location = new System.Drawing.Point(92, 29);
            this.tbField.Name = "tbField";
            this.tbField.Size = new System.Drawing.Size(307, 21);
            this.tbField.TabIndex = 0;
            // 
            // cbSys
            // 
            this.cbSys.FormattingEnabled = true;
            this.cbSys.Location = new System.Drawing.Point(23, 5);
            this.cbSys.Name = "cbSys";
            this.cbSys.Size = new System.Drawing.Size(121, 20);
            this.cbSys.TabIndex = 1;
            // 
            // dgvFieldSet
            // 
            this.dgvFieldSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFieldSet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataSet,
            this.FieldName,
            this.FieldText,
            this.hideDataSet});
            this.dgvFieldSet.Location = new System.Drawing.Point(23, 153);
            this.dgvFieldSet.Name = "dgvFieldSet";
            this.dgvFieldSet.RowTemplate.Height = 23;
            this.dgvFieldSet.Size = new System.Drawing.Size(477, 276);
            this.dgvFieldSet.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(416, 28);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "结果";
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(59, 59);
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.Size = new System.Drawing.Size(352, 21);
            this.tbResult.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "所需字段：";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(23, 124);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(105, 124);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 8;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(187, 124);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // DataSet
            // 
            this.DataSet.HeaderText = "节点";
            this.DataSet.Name = "DataSet";
            // 
            // FieldName
            // 
            this.FieldName.HeaderText = "字段";
            this.FieldName.Name = "FieldName";
            // 
            // FieldText
            // 
            this.FieldText.HeaderText = "中文";
            this.FieldText.Name = "FieldText";
            // 
            // hideDataSet
            // 
            this.hideDataSet.HeaderText = "hideDataSet";
            this.hideDataSet.Name = "hideDataSet";
            this.hideDataSet.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "仍缺字段：";
            // 
            // tbLose
            // 
            this.tbLose.Location = new System.Drawing.Point(92, 86);
            this.tbLose.Name = "tbLose";
            this.tbLose.ReadOnly = true;
            this.tbLose.Size = new System.Drawing.Size(352, 21);
            this.tbLose.TabIndex = 5;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 441);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbLose);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.dgvFieldSet);
            this.Controls.Add(this.cbSys);
            this.Controls.Add(this.tbField);
            this.Name = "SearchForm";
            this.Text = "SearchForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFieldSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbField;
        private System.Windows.Forms.ComboBox cbSys;
        private System.Windows.Forms.DataGridView dgvFieldSet;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldText;
        private System.Windows.Forms.DataGridViewTextBoxColumn hideDataSet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbLose;
    }
}