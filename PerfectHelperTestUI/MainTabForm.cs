using Perfect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectHelperTestUI
{
    public partial class MainTabForm : Form
    {
        public MainTabForm()
        {
            InitializeComponent();
            ProjDataHelper.MainTab = this.tabControl1;
        }
        
        private void backupMenuItem_Click(object sender, EventArgs e)
        {

            var menu = (sender as ToolStripItem);
            if (menu == null) { return; }
            GoCreateDatabase();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void selectDataBaseBtn_Click(object sender, EventArgs e)
        {
            GoSelectDatabase(null);
        }
        private void transDataBtn_Click(object sender, EventArgs e)
        {
            GoTransData();
        }

        private void createDatabaseBtn_Click(object sender, EventArgs e)
        {
            GoCreateDatabase();
        }
        private void GoSelectDatabase(Action afterSelectAction)
        {
            //var f = new SelectBackupDatabaseForm();
            //    f.AfterSelectAction=afterSelectAction;
            //this.tabControl1.AddTab(f);
        }

        private void GoCreateDatabase()
        {
            DoIfSelectedDatabase(()=> {
                //this.tabControl1.AddTab(new Form1());
            });
        }
        private void GoTransData()
        {
            DoIfSelectedDatabase(() => {
                //this.tabControl1.AddTab(new TransferDataForm(ProjDataHelper.CurrentCMonth));
            });
        }

        private void DoIfSelectedDatabase(Action action,Action nullAction=null)
        {
            if (ProjDataHelper.CurrentBackupDatabase == null)
            {
                MessageBox.Show("请先选择备份数据库");
                if (nullAction != null) {
                    nullAction();
                } else
                {
                    GoSelectDatabase(action);
                }
            }
            else
            {
                action();
            }
        }

        private void backupTcMenuItem_Click(object sender, EventArgs e)
        {
            this.tabControl1.AddTab(new saveToUtf8Form());
        }

        private void backupDbReportMenuItem_Click(object sender, EventArgs e)
        {
            var dbs = ProjDataHelper.GetBackupDatabases();
            var s = dbs.First(db => db.Database == "dbreport");
            ProjDataHelper.CurrentBackupDatabase = s;
            //this.tabControl1.AddTab(new TransDataToDbReportAfterMonthBackupForm());
        }

        private void backupDayMenuItem_Click(object sender, EventArgs e)
        {
            this.tabControl1.AddTab(new TestEmailForm());
            //this.tabControl1.AddTab(new TransferDayDataForm());
        }

        private void TestMySqlMenuItem_Click(object sender, EventArgs e)
        {
            
            this.tabControl1.AddTab(new compareFolderFileForm());
            //this.tabControl1.AddTab(new ShowMySqlResultForm());
        }
        private void showTransferDayBtn_Click(object sender, EventArgs e)
        {
            //this.tabControl1.AddTab(new ShowMySqlResultForm());
        }

        private void showTransferTcBtn_Click(object sender, EventArgs e)
        {
            //this.tabControl1.AddTab(new TransferTcDataForm());
        }

        private void testAsyncMenuItem_Click(object sender, EventArgs e)
        {
            this.tabControl1.AddTab(new TestAsyncForm());
        }
    }
}
