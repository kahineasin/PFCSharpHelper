namespace PerfectHelperTestUI
{
    partial class MainTabForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testAsyncMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToUtf8MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareFolderFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testEmailMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupDbReportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new Perfect.PFTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.showTransferTcBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.transferDayToMonthBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.showTransferDayBtn = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.transDataBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.createDatabaseBtn = new System.Windows.Forms.Button();
            this.selectDataBaseBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.功能ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(659, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 功能ToolStripMenuItem
            // 
            this.功能ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testAsyncMenuItem,
            this.saveToUtf8MenuItem,
            this.compareFolderFileMenuItem,
            this.testEmailMenuItem,
            this.backupDbReportMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.功能ToolStripMenuItem.Name = "功能ToolStripMenuItem";
            this.功能ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.功能ToolStripMenuItem.Text = "功能";
            // 
            // testAsyncMenuItem
            // 
            this.testAsyncMenuItem.Name = "testAsyncMenuItem";
            this.testAsyncMenuItem.Size = new System.Drawing.Size(184, 22);
            this.testAsyncMenuItem.Text = "testAsync";
            this.testAsyncMenuItem.Click += new System.EventHandler(this.testAsyncMenuItem_Click);
            // 
            // saveToUtf8MenuItem
            // 
            this.saveToUtf8MenuItem.Name = "saveToUtf8MenuItem";
            this.saveToUtf8MenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToUtf8MenuItem.Text = "saveToUtf8";
            this.saveToUtf8MenuItem.Click += new System.EventHandler(this.backupTcMenuItem_Click);
            // 
            // compareFolderFileMenuItem
            // 
            this.compareFolderFileMenuItem.Name = "compareFolderFileMenuItem";
            this.compareFolderFileMenuItem.Size = new System.Drawing.Size(184, 22);
            this.compareFolderFileMenuItem.Text = "compareFolderFile";
            this.compareFolderFileMenuItem.Click += new System.EventHandler(this.TestMySqlMenuItem_Click);
            // 
            // testEmailMenuItem
            // 
            this.testEmailMenuItem.Name = "testEmailMenuItem";
            this.testEmailMenuItem.Size = new System.Drawing.Size(184, 22);
            this.testEmailMenuItem.Text = "测试Email";
            this.testEmailMenuItem.Click += new System.EventHandler(this.backupDayMenuItem_Click);
            // 
            // backupDbReportMenuItem
            // 
            this.backupDbReportMenuItem.Name = "backupDbReportMenuItem";
            this.backupDbReportMenuItem.Size = new System.Drawing.Size(184, 22);
            this.backupDbReportMenuItem.Text = "备份dbreport数据";
            this.backupDbReportMenuItem.Click += new System.EventHandler(this.backupDbReportMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exitMenuItem.Text = "退出";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(10, 5);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(659, 441);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(651, 411);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "首页";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel3.BackColor = System.Drawing.Color.LightPink;
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.showTransferTcBtn);
            this.panel3.Location = new System.Drawing.Point(85, 257);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(493, 89);
            this.panel3.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(130, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 21);
            this.label7.TabIndex = 0;
            this.label7.Text = "每月数据任务";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(39, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "步骤:";
            // 
            // showTransferTcBtn
            // 
            this.showTransferTcBtn.Location = new System.Drawing.Point(93, 36);
            this.showTransferTcBtn.Name = "showTransferTcBtn";
            this.showTransferTcBtn.Size = new System.Drawing.Size(99, 23);
            this.showTransferTcBtn.TabIndex = 2;
            this.showTransferTcBtn.Text = "导入调差数据";
            this.showTransferTcBtn.UseVisualStyleBackColor = true;
            this.showTransferTcBtn.Click += new System.EventHandler(this.showTransferTcBtn_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel2.BackColor = System.Drawing.Color.DodgerBlue;
            this.panel2.Controls.Add(this.transferDayToMonthBtn);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.showTransferDayBtn);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Location = new System.Drawing.Point(85, 162);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(493, 89);
            this.panel2.TabIndex = 7;
            // 
            // transferDayToMonthBtn
            // 
            this.transferDayToMonthBtn.Location = new System.Drawing.Point(221, 36);
            this.transferDayToMonthBtn.Name = "transferDayToMonthBtn";
            this.transferDayToMonthBtn.Size = new System.Drawing.Size(132, 23);
            this.transferDayToMonthBtn.TabIndex = 3;
            this.transferDayToMonthBtn.Text = "转移数据到月数据库";
            this.transferDayToMonthBtn.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(130, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 21);
            this.label5.TabIndex = 0;
            this.label5.Text = "每日数据任务";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(39, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 16);
            this.label6.TabIndex = 1;
            this.label6.Text = "步骤:";
            // 
            // showTransferDayBtn
            // 
            this.showTransferDayBtn.ForeColor = System.Drawing.Color.Blue;
            this.showTransferDayBtn.Location = new System.Drawing.Point(93, 36);
            this.showTransferDayBtn.Name = "showTransferDayBtn";
            this.showTransferDayBtn.Size = new System.Drawing.Size(99, 23);
            this.showTransferDayBtn.TabIndex = 2;
            this.showTransferDayBtn.Text = "导入日结数据";
            this.showTransferDayBtn.UseVisualStyleBackColor = true;
            this.showTransferDayBtn.Click += new System.EventHandler(this.showTransferDayBtn_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(198, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "->";
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.BackColor = System.Drawing.Color.Turquoise;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.transDataBtn);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.createDatabaseBtn);
            this.panel1.Controls.Add(this.selectDataBaseBtn);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(85, 67);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 89);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(130, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "联通直销员数据管理";
            // 
            // transDataBtn
            // 
            this.transDataBtn.ForeColor = System.Drawing.Color.Blue;
            this.transDataBtn.Location = new System.Drawing.Point(347, 36);
            this.transDataBtn.Name = "transDataBtn";
            this.transDataBtn.Size = new System.Drawing.Size(90, 23);
            this.transDataBtn.TabIndex = 5;
            this.transDataBtn.Text = "导入当月数据";
            this.transDataBtn.UseVisualStyleBackColor = true;
            this.transDataBtn.Click += new System.EventHandler(this.transDataBtn_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(39, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "步骤:";
            // 
            // createDatabaseBtn
            // 
            this.createDatabaseBtn.Location = new System.Drawing.Point(221, 36);
            this.createDatabaseBtn.Name = "createDatabaseBtn";
            this.createDatabaseBtn.Size = new System.Drawing.Size(97, 23);
            this.createDatabaseBtn.TabIndex = 4;
            this.createDatabaseBtn.Text = "生成当月数据库";
            this.createDatabaseBtn.UseVisualStyleBackColor = true;
            this.createDatabaseBtn.Click += new System.EventHandler(this.createDatabaseBtn_Click);
            // 
            // selectDataBaseBtn
            // 
            this.selectDataBaseBtn.Location = new System.Drawing.Point(93, 36);
            this.selectDataBaseBtn.Name = "selectDataBaseBtn";
            this.selectDataBaseBtn.Size = new System.Drawing.Size(99, 23);
            this.selectDataBaseBtn.TabIndex = 2;
            this.selectDataBaseBtn.Text = "选择备份数据库";
            this.selectDataBaseBtn.UseVisualStyleBackColor = true;
            this.selectDataBaseBtn.Click += new System.EventHandler(this.selectDataBaseBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "->";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(198, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "->";
            // 
            // MainTabForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 469);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainTabForm";
            this.Text = "主界面";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 功能ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testAsyncMenuItem;
        private Perfect.PFTabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button selectDataBaseBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button createDatabaseBtn;
        private System.Windows.Forms.Button transDataBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem saveToUtf8MenuItem;
        private System.Windows.Forms.ToolStripMenuItem backupDbReportMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testEmailMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareFolderFileMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button showTransferDayBtn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button showTransferTcBtn;
        private System.Windows.Forms.Button transferDayToMonthBtn;
        private System.Windows.Forms.Label label9;
    }
}