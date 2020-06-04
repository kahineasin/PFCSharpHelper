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
using System.Xml;

namespace FieldSetsManager
{
    public partial class SearchForm : Form
    {
        private XmlNodeList _syses;
        private XmlDocument _xml;
        private XmlNode _dataSets;
        private XmlDocument _fieldSetsXml;
        private string _xmlPath;
        public SearchForm()
        {
            //this.IsMdiContainer = true;
            InitializeComponent();

            string xmlfile = Path.Combine(PFDataHelper.BaseDirectory, "XmlConfig\\PathConfig.xml");
            _xml = new XmlDocument();
            _xml.Load(xmlfile);
            _syses = _xml.SelectSingleNode("Systems").ChildNodes;
            var list = new List<KeyValuePair<string, string>>();
            foreach (XmlNode sys in _syses)
            {
                var node = sys.SelectSingleNode("Path");
                if (node != null)
                {
                    list.Add(new KeyValuePair<string, string>(
                        sys.Name,
                        node.InnerText
                        ));
                }
            }
            cbSys.DataSource = list;
            cbSys.ValueMember = "Value";
            cbSys.DisplayMember = "Key";

            tbField.Text = "hybh,hyxm,xx";
        }
        private void RefreshGrid() {
            
                _xmlPath = cbSys.SelectedValue.ToString();
                _fieldSetsXml = new XmlDocument();
            _fieldSetsXml.Load(_xmlPath);

                _dataSets = _fieldSetsXml.ChildNodes[1];

            List<string> fields = (tbField.Text ?? "").Replace(" ","").Split(new char[] { ',','"' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            fields = fields.Select(a => (a??"").ToLower()).ToList();
            var list = new List<PFModelConfig>();
            foreach (XmlNode dataSet in _dataSets.ChildNodes)
            {
                var fieldsNode = dataSet.SelectSingleNode("Fields");//demo节点也读出来了，是null的
                if (fieldsNode != null)
                {
                    foreach (XmlNode i in dataSet.SelectSingleNode("Fields").ChildNodes)
                    {
                        //foreach (string field in fields)
                        //{
                        var config = new PFModelConfig(i, dataSet.Attributes["name"].Value);
                        //if (fields.Any(a => (a??"").ToLower() == config.LowerFieldName))
                        if (fields.Any(a => a == config.LowerFieldName))
                        {
                            list.Add(config);
                        }
                        //}
                    }
                }
            }
            var groups = list.GroupBy(a => a.DataSet).OrderByDescending(a => a.Count());

            var tmpFields = new List<string>(fields.ToArray());
            var result = new List<string>();
            dgvFieldSet.Rows.Clear();
            foreach (var group in groups)
            {
                bool haveAny = false;
                AddDataSetRow(group.Key);
                foreach (PFModelConfig item in group)
                {
                    AddFieldRow(item);
                    if (tmpFields.Contains(item.LowerFieldName))
                    {
                        tmpFields.Remove(item.LowerFieldName);
                        haveAny = true;
                    }
                    //if (tmpFields.Contains(item.FieldName))
                    //{
                    //    tmpFields.Remove(item.FieldName);
                    //    haveAny = true;
                    //}
                }
                if ( haveAny)
                {
                    result.Add(group.Key);
                }
            }
            tbResult.Text = string.Join(",", result);
            tbLose.Text= string.Join(",", tmpFields);
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        private void AddDataSetRow(string name)
        {
            var i = dgvFieldSet.Rows.Add();
            dgvFieldSet.Rows[i].Cells["DataSet"].Value = name;
            dgvFieldSet.Rows[i].Cells["hideDataSet"].Value = name;
        }
        private void AddFieldRow(PFModelConfig config)
        {
            var i = dgvFieldSet.Rows.Add();
            dgvFieldSet.Rows[i].Cells["FieldName"].Value = config.FieldName;
            dgvFieldSet.Rows[i].Cells["FieldText"].Value = config.FieldText;
            dgvFieldSet.Rows[i].Cells["hideDataSet"].Value = config.DataSet;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var selects = dgvFieldSet.SelectedRows;
            if (selects.Count > 0 && (!selects[0].IsNewRow))
            {
                var name = selects[0].Cells["hideDataSet"].Value.ToString();

                AddForm childrenForm = new AddForm(_fieldSetsXml, _xmlPath, name);
                //childrenForm.MdiParent = this;
                childrenForm.AddEvent += new AddDelegateHander(OnAdd);//注册事件
                childrenForm.Show();
                //childrenForm.WindowState = FormWindowState.Maximized;//最大化
            }
        }
        protected void OnAdd(string fieldName)
        {
            var fields = (tbField.Text ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!fields.Contains(fieldName))
            {
                fields.Add(fieldName);
                tbField.Text = string.Join(",", fields);
            }
            RefreshGrid();
        }
        protected void OnEdit()
        {
            RefreshGrid();
        }
        protected override void OnClosed(EventArgs e)
        {
            DisposeFile();
            base.OnClosed(e);
        }
        private void DisposeFile()
        {
            if (_fieldSetsXml != null )
            {
                //_fieldSetsXml.
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selects = dgvFieldSet.SelectedRows;
            if (selects.Count == 1 && (!selects[0].IsNewRow))
            {
                if (selects[0].Cells["FieldName"].Value != null)
                {
                    var fieldNode = FieldSetsHelper.FindField(_fieldSetsXml, selects[0].Cells["hideDataSet"].Value.ToString(), selects[0].Cells["FieldName"].Value.ToString());
                    if (fieldNode != null)
                    {
                        fieldNode.ParentNode.RemoveChild(fieldNode);
                        _fieldSetsXml.Save(_xmlPath);
                        MessageBox.Show("删除成功");
                        RefreshGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show("只能删除一个字段");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selects = dgvFieldSet.SelectedRows;
            if (selects.Count > 0 && (!selects[0].IsNewRow))
            {
                var dataSet = (selects[0].Cells["hideDataSet"].Value??"").ToString();
                var fieldName = (selects[0].Cells["FieldName"].Value??"").ToString();
                if ((!string.IsNullOrWhiteSpace(dataSet)) && (!string.IsNullOrWhiteSpace(fieldName)))
                {
                    AddForm childrenForm = new AddForm(_fieldSetsXml, _xmlPath,dataSet,fieldName);
                    childrenForm.EditEvent += new EditDelegateHander(OnEdit);//注册事件
                    childrenForm.Show();
                }
            }
        }
    }
}
