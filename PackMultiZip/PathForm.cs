using System;
using System.Windows.Forms;
using Perfect;
using System.IO;
using System.Xml;
using System.Configuration;

namespace PackMultiZip
{
    public partial class PathForm : Form
    {
        private XmlNodeList _syses;
        private XmlDocument _xml;
        private XmlNode _dataSets;
        private XmlDocument _fieldSetsXml;
        private string _xmlPath;
        public PathForm()
        {
            InitializeComponent();

            string xmlfile = Path.Combine(PFDataHelper.BaseDirectory, "XmlConfig\\PathConfig.xml");
            _xml = new XmlDocument();
            _xml.Load(xmlfile);
            _syses = _xml.SelectSingleNode("Paths").ChildNodes;

            RefreshGrid();
        }
        private void RefreshGrid()
        {
            foreach (XmlNode sys in _syses)
            {
                var path = sys.InnerText;
                var fileName = Path.GetFileName(path);
                var i = pathDGView.Rows.Add();
                pathDGView.Rows[i].Cells["FolderName"].Value = fileName;
                pathDGView.Rows[i].Cells["FolderPath"].Value = path;
                if (sys.Attributes != null && sys.Attributes["excludeFolder"] != null)
                {
                    pathDGView.Rows[i].Cells["excludeFolder"].Value = sys.Attributes["excludeFolder"].Value;
                }

            }

        }

        private void packZipBtn_Click(object sender, EventArgs e)
        {
            PFWinFormHelper.GetGridSelectedRows(pathDGView, rows => {
                //var targetPath = "D:\\wxj\\pf_project";
                var targetPath = ConfigurationManager.AppSettings["ZipTargetPath"];
                PFDataHelper.CreateDirectory(targetPath);

                        PFWinFormHelper.MultiFuncLoadingFlag = true;//flag 为false时候，退出执行耗时操作
                
                        MultiFuncLoading loadingfrm = new MultiFuncLoading(this);
                        // 将Loaing窗口，注入到 SplashScreenManager 来管理
                        SplashScreenManager loading = new SplashScreenManager(loadingfrm);
                        loading.ShowLoading();
                        // 设置loadingfrm操作必须在调用ShowLoading之后执行
                        loadingfrm.SetTxt("多功能Loaidng界面", "拼命加载中...客官耐心等待", "Please Waitting...");
                
                        // try catch 包起来，防止出错
                        try
                        {
                            int i = 0;
                            int total = rows.Count;
                            foreach (var row in rows)
                            {
                                i++;
                                ZipUtility zip = new ZipUtility();
                                var path = row.Cells["FolderPath"].Value.ToString();
                                var fileName = row.Cells["FolderName"].Value.ToString();
                                var excludeFolderArr = row.Cells["excludeFolder"].Value == null ? null : row.Cells["excludeFolder"].Value.ToString().Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries);
                                loadingfrm.SetJD("当前：正在打包", "当前进度：" + fileName + "(" + i+"/"+ total + ")");

                                zip.ZipFileFromDirectory(path, Path.Combine(targetPath, fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ConfigurationManager.AppSettings["FromWhere"] + ".zip"), 4, excludeFolderArr);
                            }
                
                        }
                        catch (Exception) { /*可选处理异常*/ }
                        finally { loading.CloseWaitForm(); }


                System.Diagnostics.Process.Start(targetPath); //如果是本地访问就直接打开文件夹
            }); 
        }
    }
}
