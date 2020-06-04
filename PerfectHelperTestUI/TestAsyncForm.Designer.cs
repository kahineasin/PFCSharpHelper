namespace PerfectHelperTestUI
{
    partial class TestAsyncForm
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
            this.resultTBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.testAsyncBtn = new System.Windows.Forms.Button();
            this.testTaskBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultTBox
            // 
            this.resultTBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultTBox.Location = new System.Drawing.Point(12, 12);
            this.resultTBox.Multiline = true;
            this.resultTBox.Name = "resultTBox";
            this.resultTBox.Size = new System.Drawing.Size(605, 326);
            this.resultTBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.testTaskBtn);
            this.panel1.Controls.Add(this.testAsyncBtn);
            this.panel1.Location = new System.Drawing.Point(13, 345);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(615, 83);
            this.panel1.TabIndex = 1;
            // 
            // testAsyncBtn
            // 
            this.testAsyncBtn.Location = new System.Drawing.Point(4, 4);
            this.testAsyncBtn.Name = "testAsyncBtn";
            this.testAsyncBtn.Size = new System.Drawing.Size(75, 23);
            this.testAsyncBtn.TabIndex = 0;
            this.testAsyncBtn.Text = "testAsync";
            this.testAsyncBtn.UseVisualStyleBackColor = true;
            this.testAsyncBtn.Click += new System.EventHandler(this.testAsyncBtn_Click);
            // 
            // testTaskBtn
            // 
            this.testTaskBtn.Location = new System.Drawing.Point(86, 3);
            this.testTaskBtn.Name = "testTaskBtn";
            this.testTaskBtn.Size = new System.Drawing.Size(75, 23);
            this.testTaskBtn.TabIndex = 1;
            this.testTaskBtn.Text = "testTask";
            this.testTaskBtn.UseVisualStyleBackColor = true;
            this.testTaskBtn.Click += new System.EventHandler(this.testTaskBtn_Click);
            // 
            // TestAsyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 432);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.resultTBox);
            this.Name = "TestAsyncForm";
            this.Text = "TestAsyncForm";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox resultTBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button testAsyncBtn;
        private System.Windows.Forms.Button testTaskBtn;
    }
}