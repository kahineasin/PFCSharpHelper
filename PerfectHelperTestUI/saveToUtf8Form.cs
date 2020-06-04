using Perfect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectHelperTestUI
{
    public partial class saveToUtf8Form : Form
    {
        public saveToUtf8Form()
        {
            InitializeComponent();

            string[] dayTables = Enum.GetNames(typeof(TxtFileEncoder.PFEncoding));
            foreach (string i in dayTables)
            {
                encodeCBox.Items.Add(i);
            }
            //encodeCBox.Items.Add("utf-8");
            //encodeCBox.Items.Add("utf-8 no BOM");
        }

        private void selectFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            //fileDialog.Filter = "所有文件(*xls*)|*.xls*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                PFWinFormHelper.GridClear(fileDGView);
                foreach (var path in fileDialog.FileNames)
                {
                    var fileName = Path.GetFileName(path);
                    var i = fileDGView.Rows.Add();
                    TxtFileEncoder.PFEncoding encode = TxtFileEncoder.GetPFEncoding((Stream)new FileStream(path, FileMode.Open));
                    fileDGView.Rows[i].Cells["FileName"].Value = path;
                    fileDGView.Rows[i].Cells["FileEncode"].Value = encode;

                    //TxtFileEncoder.GetEncoding(i);
                    ////Encoding encode = TxtFileEncoder.GetEncoding(i);
                    //Encoding encode = TxtFileEncoder.GetEncoding((Stream)new FileStream(i, FileMode.Open));
                    //string s = PFDataHelper.ReadFileToString(i, encode);
                    //PFDataHelper.SaveStringToFile(s,Path.Combine(PFDataHelper.BaseDirectory,"OutFile",Path.GetFileName(i)));
                }
                //System.Diagnostics.Process.Start(Path.Combine(PFDataHelper.BaseDirectory, "OutFile"));
            }
        }

        private void startConvertBtn_Click(object sender, EventArgs e)
        {
            if (fileDGView.Rows.Count < 1)
            {
                MessageBox.Show("无待转换文件");
                return;
            }
            foreach(DataGridViewRow row in fileDGView.Rows)
            {
                if (!row.IsNewRow)
                {
                    var path = row.Cells["FileName"].Value.ToString();
                    //Encoding encode = TxtFileEncoder.GetEncoding(i);
                    Encoding encode = TxtFileEncoder.GetEncoding((Stream)new FileStream(path, FileMode.Open));
                    string s = PFDataHelper.ReadFileToString(path, encode);
                    PFDataHelper.SaveStringToFile(s, Path.Combine(PFDataHelper.BaseDirectory, "OutFile", Path.GetFileName(path)));
                }
            }
            System.Diagnostics.Process.Start(Path.Combine(PFDataHelper.BaseDirectory, "OutFile"));
        }
    }
}
