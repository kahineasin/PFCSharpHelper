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
    public partial class compareFolderFileForm : Form
    {
        public compareFolderFileForm()
        {
            InitializeComponent();
        }

        private void selectSrcBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择src文件";
            //fileDialog.Filter = "所有文件(*xls*)|*.xls*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                srcTBox.Text = fileDialog.FileName.Replace(Path.GetFileName(fileDialog.FileName),"");
            }
        }

        private void selectDstBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择dst文件";
            //fileDialog.Filter = "所有文件(*xls*)|*.xls*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                dstTBox.Text = fileDialog.FileName.Replace(Path.GetFileName(fileDialog.FileName), "");
            }
        }
        #region 选择文件夹的方式（没有选文件方便，因为选文件的框可以粘路径）
        //private void selectSrcBtn_Click(object sender, EventArgs e)
        //{
        //    FolderBrowserDialog fileDialog = new FolderBrowserDialog();
        //    //fileDialog.Multiselect = true;
        //    fileDialog.Description = "请选择src文件夹";
        //    //fileDialog.Filter = "所有文件(*xls*)|*.xls*"; //设置要选择的文件的类型
        //    if (fileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        srcTBox.Text = fileDialog.SelectedPath;
        //    }
        //}

        //private void selectDstBtn_Click(object sender, EventArgs e)
        //{
        //    FolderBrowserDialog fileDialog = new FolderBrowserDialog();
        //    //fileDialog.Multiselect = true;
        //    fileDialog.Description = "请选择dst文件夹";
        //    //fileDialog.Filter = "所有文件(*xls*)|*.xls*"; //设置要选择的文件的类型
        //    if (fileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        dstTBox.Text = fileDialog.SelectedPath;
        //    }
        //} 
        #endregion

        private void compareBtn_Click(object sender, EventArgs e)
        {
            PFWinFormHelper.GridClear(compareDGView);
            var srcFiles=Directory.GetFiles(srcTBox.Text);
            var dstFiles = Directory.GetFiles(dstTBox.Text);
            var srcList = srcFiles.Select(a => new CompareModel { SrcFileName =Path.GetFileNameWithoutExtension( a) }).ToList();
            var dstList = dstFiles.Select(a => new CompareModel { DstFileName = Path.GetFileNameWithoutExtension(a) }).ToList();
            var leftData = (from first in srcList
                            join last in dstList on first.SrcFileName equals last.DstFileName into temp  //last有可能空
                            from last in temp.DefaultIfEmpty(new CompareModel { SrcFileName=first.SrcFileName, DstFileName = default(string) })  //或 from item in billTypeList.DefaultIfEmpty() //这行的last和第一行的first可在select时取到值
                            select new CompareModel
                            {
                                SrcFileName = first.SrcFileName,
                                DstFileName = last.DstFileName,   //StockQty = item != null ? item.StockQty : 0, //判断空
                            });
            var rightRemainingData = (from r in dstList
                                      where !(from a in leftData select a.SrcFileName).Contains(r.DstFileName)
                                      select new CompareModel
                                      {
                                          SrcFileName = default(string),
                                          DstFileName = r.DstFileName
                                      });
            var fullOuterjoinData = leftData.Concat(rightRemainingData).ToList();
            //var fullOuterjoinData = PFDataHelper.ListFullJoin<CompareModel, CompareModel,string, CompareModel>(srcList,dstList,
            //    l=>l.SrcFileName,r=>r.DstFileName,lr=> lr.SrcFileName,
            //    (l)=> new CompareModel { SrcFileName = l.SrcFileName, DstFileName = default(string) },
            //    //(r) => new CompareModel { SrcFileName = l.SrcFileName, DstFileName = default(string) },
            //    (l,r) => new CompareModel { SrcFileName = l.SrcFileName, DstFileName = r.DstFileName },
            //    (r) => new CompareModel { SrcFileName = default(string), DstFileName = r.DstFileName }
            //    );


            foreach (var i in fullOuterjoinData)
            {
                var row = compareDGView.Rows.Add();
                compareDGView.Rows[row].Cells["SrcFileName"].Value = i.SrcFileName;
                compareDGView.Rows[row].Cells["DstFileName"].Value = i.DstFileName;
            }
        }
        public class CompareModel
        {
            public string SrcFileName { get; set; }
            public string DstFileName { get; set; }
        }
    }
}
