namespace FieldSetsManager
{
    partial class AddForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbText = new System.Windows.Forms.TextBox();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbDataSet = new System.Windows.Forms.Label();
            this.cbHaveWidth = new System.Windows.Forms.CheckBox();
            this.cbVisible = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nudSqlLength = new System.Windows.Forms.NumericUpDown();
            this.cbHaveSqlLength = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSqlLength)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Text:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Type:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Width:";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(73, 49);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(100, 21);
            this.tbName.TabIndex = 4;
            // 
            // tbText
            // 
            this.tbText.Location = new System.Drawing.Point(73, 76);
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(100, 21);
            this.tbText.TabIndex = 5;
            // 
            // cbType
            // 
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "string",
            "decimal",
            "int",
            "DateTime",
            "date",
            "month",
            "bool",
            "percent"});
            this.cbType.Location = new System.Drawing.Point(73, 104);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(121, 20);
            this.cbType.TabIndex = 8;
            // 
            // nudWidth
            // 
            this.nudWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudWidth.Location = new System.Drawing.Point(73, 157);
            this.nudWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWidth.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(120, 21);
            this.nudWidth.TabIndex = 9;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(73, 223);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lbDataSet
            // 
            this.lbDataSet.AutoSize = true;
            this.lbDataSet.Location = new System.Drawing.Point(15, 13);
            this.lbDataSet.Name = "lbDataSet";
            this.lbDataSet.Size = new System.Drawing.Size(41, 12);
            this.lbDataSet.TabIndex = 11;
            this.lbDataSet.Text = "label5";
            // 
            // cbHaveWidth
            // 
            this.cbHaveWidth.AutoSize = true;
            this.cbHaveWidth.Location = new System.Drawing.Point(52, 160);
            this.cbHaveWidth.Name = "cbHaveWidth";
            this.cbHaveWidth.Size = new System.Drawing.Size(15, 14);
            this.cbHaveWidth.TabIndex = 12;
            this.cbHaveWidth.UseVisualStyleBackColor = true;
            this.cbHaveWidth.CheckedChanged += new System.EventHandler(this.cbHaveWidth_CheckedChanged);
            // 
            // cbVisible
            // 
            this.cbVisible.AutoSize = true;
            this.cbVisible.Location = new System.Drawing.Point(179, 53);
            this.cbVisible.Name = "cbVisible";
            this.cbVisible.Size = new System.Drawing.Size(66, 16);
            this.cbVisible.TabIndex = 14;
            this.cbVisible.Text = "Visible";
            this.cbVisible.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "SqlLen:";
            // 
            // nudSqlLength
            // 
            this.nudSqlLength.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSqlLength.Location = new System.Drawing.Point(74, 130);
            this.nudSqlLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudSqlLength.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudSqlLength.Name = "nudSqlLength";
            this.nudSqlLength.Size = new System.Drawing.Size(120, 21);
            this.nudSqlLength.TabIndex = 9;
            // 
            // cbHaveSqlLength
            // 
            this.cbHaveSqlLength.AutoSize = true;
            this.cbHaveSqlLength.Location = new System.Drawing.Point(53, 133);
            this.cbHaveSqlLength.Name = "cbHaveSqlLength";
            this.cbHaveSqlLength.Size = new System.Drawing.Size(15, 14);
            this.cbHaveSqlLength.TabIndex = 12;
            this.cbHaveSqlLength.UseVisualStyleBackColor = true;
            this.cbHaveSqlLength.CheckedChanged += new System.EventHandler(this.cbHaveSqlLength_CheckedChanged);
            // 
            // AddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.cbVisible);
            this.Controls.Add(this.cbHaveSqlLength);
            this.Controls.Add(this.cbHaveWidth);
            this.Controls.Add(this.lbDataSet);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.nudSqlLength);
            this.Controls.Add(this.nudWidth);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.tbText);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AddForm";
            this.Text = "AddForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSqlLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbText;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lbDataSet;
        private System.Windows.Forms.CheckBox cbHaveWidth;
        private System.Windows.Forms.CheckBox cbVisible;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudSqlLength;
        private System.Windows.Forms.CheckBox cbHaveSqlLength;
    }
}