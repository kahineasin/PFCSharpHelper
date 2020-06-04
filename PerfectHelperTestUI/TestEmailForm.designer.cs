namespace PerfectHelperTestUI
{
    partial class TestEmailForm
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
            this.components = new System.ComponentModel.Container();
            this.emailCntBtn = new System.Windows.Forms.Button();
            this.subjectTBox = new System.Windows.Forms.TextBox();
            this.receiveBtn = new System.Windows.Forms.Button();
            this.connectStatusTBox = new System.Windows.Forms.TextBox();
            this.connectBtn = new System.Windows.Forms.Button();
            this.emailCntNBox = new System.Windows.Forms.NumericUpDown();
            this.emailDateBox = new System.Windows.Forms.DateTimePicker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.disConnectBtn = new System.Windows.Forms.Button();
            this.testListerBtn = new System.Windows.Forms.Button();
            this.bodyTBox = new System.Windows.Forms.TextBox();
            this.emailDateTBox = new System.Windows.Forms.TextBox();
            this.newEmailBtn = new System.Windows.Forms.Button();
            this.checkEachEmailBtn = new System.Windows.Forms.Button();
            this.errorTBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.emailCntNBox)).BeginInit();
            this.SuspendLayout();
            // 
            // emailCntBtn
            // 
            this.emailCntBtn.Location = new System.Drawing.Point(154, 65);
            this.emailCntBtn.Name = "emailCntBtn";
            this.emailCntBtn.Size = new System.Drawing.Size(75, 23);
            this.emailCntBtn.TabIndex = 1;
            this.emailCntBtn.Text = "邮件数量";
            this.emailCntBtn.UseVisualStyleBackColor = true;
            this.emailCntBtn.Click += new System.EventHandler(this.emailCntBtn_Click);
            // 
            // subjectTBox
            // 
            this.subjectTBox.Location = new System.Drawing.Point(12, 94);
            this.subjectTBox.Name = "subjectTBox";
            this.subjectTBox.Size = new System.Drawing.Size(212, 21);
            this.subjectTBox.TabIndex = 2;
            // 
            // receiveBtn
            // 
            this.receiveBtn.Location = new System.Drawing.Point(230, 92);
            this.receiveBtn.Name = "receiveBtn";
            this.receiveBtn.Size = new System.Drawing.Size(75, 23);
            this.receiveBtn.TabIndex = 3;
            this.receiveBtn.Text = "收邮件";
            this.receiveBtn.UseVisualStyleBackColor = true;
            this.receiveBtn.Click += new System.EventHandler(this.receiveBtn_Click);
            // 
            // connectStatusTBox
            // 
            this.connectStatusTBox.Location = new System.Drawing.Point(13, 40);
            this.connectStatusTBox.Name = "connectStatusTBox";
            this.connectStatusTBox.Size = new System.Drawing.Size(100, 21);
            this.connectStatusTBox.TabIndex = 5;
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(120, 37);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 6;
            this.connectBtn.Text = "连接邮箱";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // emailCntNBox
            // 
            this.emailCntNBox.Location = new System.Drawing.Point(12, 65);
            this.emailCntNBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.emailCntNBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.emailCntNBox.Name = "emailCntNBox";
            this.emailCntNBox.Size = new System.Drawing.Size(120, 21);
            this.emailCntNBox.TabIndex = 7;
            // 
            // emailDateBox
            // 
            this.emailDateBox.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.emailDateBox.Location = new System.Drawing.Point(12, 121);
            this.emailDateBox.Name = "emailDateBox";
            this.emailDateBox.Size = new System.Drawing.Size(200, 21);
            this.emailDateBox.TabIndex = 8;
            // 
            // disConnectBtn
            // 
            this.disConnectBtn.Location = new System.Drawing.Point(202, 40);
            this.disConnectBtn.Name = "disConnectBtn";
            this.disConnectBtn.Size = new System.Drawing.Size(75, 23);
            this.disConnectBtn.TabIndex = 9;
            this.disConnectBtn.Text = "断开连接";
            this.disConnectBtn.UseVisualStyleBackColor = true;
            this.disConnectBtn.Click += new System.EventHandler(this.disConnectBtn_Click);
            // 
            // testListerBtn
            // 
            this.testListerBtn.Location = new System.Drawing.Point(11, 149);
            this.testListerBtn.Name = "testListerBtn";
            this.testListerBtn.Size = new System.Drawing.Size(75, 23);
            this.testListerBtn.TabIndex = 10;
            this.testListerBtn.Text = "testListen";
            this.testListerBtn.UseVisualStyleBackColor = true;
            this.testListerBtn.Click += new System.EventHandler(this.testListerBtn_Click);
            // 
            // bodyTBox
            // 
            this.bodyTBox.Location = new System.Drawing.Point(11, 178);
            this.bodyTBox.Multiline = true;
            this.bodyTBox.Name = "bodyTBox";
            this.bodyTBox.Size = new System.Drawing.Size(359, 260);
            this.bodyTBox.TabIndex = 11;
            // 
            // emailDateTBox
            // 
            this.emailDateTBox.Location = new System.Drawing.Point(240, 122);
            this.emailDateTBox.Name = "emailDateTBox";
            this.emailDateTBox.Size = new System.Drawing.Size(243, 21);
            this.emailDateTBox.TabIndex = 12;
            this.emailDateTBox.TextChanged += new System.EventHandler(this.emailDateTBox_TextChanged);
            // 
            // newEmailBtn
            // 
            this.newEmailBtn.Location = new System.Drawing.Point(311, 92);
            this.newEmailBtn.Name = "newEmailBtn";
            this.newEmailBtn.Size = new System.Drawing.Size(75, 23);
            this.newEmailBtn.TabIndex = 13;
            this.newEmailBtn.Text = "新邮件";
            this.newEmailBtn.UseVisualStyleBackColor = true;
            this.newEmailBtn.Click += new System.EventHandler(this.newEmailBtn_Click);
            // 
            // checkEachEmailBtn
            // 
            this.checkEachEmailBtn.Location = new System.Drawing.Point(92, 149);
            this.checkEachEmailBtn.Name = "checkEachEmailBtn";
            this.checkEachEmailBtn.Size = new System.Drawing.Size(103, 23);
            this.checkEachEmailBtn.TabIndex = 14;
            this.checkEachEmailBtn.Text = "checkEachEmail";
            this.checkEachEmailBtn.UseVisualStyleBackColor = true;
            this.checkEachEmailBtn.Click += new System.EventHandler(this.checkEachEmailBtn_Click);
            // 
            // errorTBox
            // 
            this.errorTBox.Location = new System.Drawing.Point(380, 178);
            this.errorTBox.Multiline = true;
            this.errorTBox.Name = "errorTBox";
            this.errorTBox.Size = new System.Drawing.Size(359, 260);
            this.errorTBox.TabIndex = 11;
            // 
            // TestEmailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 450);
            this.Controls.Add(this.checkEachEmailBtn);
            this.Controls.Add(this.newEmailBtn);
            this.Controls.Add(this.emailDateTBox);
            this.Controls.Add(this.errorTBox);
            this.Controls.Add(this.bodyTBox);
            this.Controls.Add(this.testListerBtn);
            this.Controls.Add(this.disConnectBtn);
            this.Controls.Add(this.emailDateBox);
            this.Controls.Add(this.emailCntNBox);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.connectStatusTBox);
            this.Controls.Add(this.receiveBtn);
            this.Controls.Add(this.subjectTBox);
            this.Controls.Add(this.emailCntBtn);
            this.Name = "TestEmailForm";
            this.Text = "TestEmailForm";
            ((System.ComponentModel.ISupportInitialize)(this.emailCntNBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button emailCntBtn;
        private System.Windows.Forms.TextBox subjectTBox;
        private System.Windows.Forms.Button receiveBtn;
        private System.Windows.Forms.TextBox connectStatusTBox;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.NumericUpDown emailCntNBox;
        private System.Windows.Forms.DateTimePicker emailDateBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button disConnectBtn;
        private System.Windows.Forms.Button testListerBtn;
        private System.Windows.Forms.TextBox bodyTBox;
        private System.Windows.Forms.TextBox emailDateTBox;
        private System.Windows.Forms.Button newEmailBtn;
        private System.Windows.Forms.Button checkEachEmailBtn;
        private System.Windows.Forms.TextBox errorTBox;
    }
}