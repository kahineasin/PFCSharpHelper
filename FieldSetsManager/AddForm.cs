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
using System.Xml;

namespace FieldSetsManager
{
    public delegate void AddDelegateHander(string fieldName); //声明一个委托
    public delegate void EditDelegateHander(); //声明一个委托
    public partial class AddForm : Form
    {
        public event AddDelegateHander AddEvent;//声明一个事件
        public event EditDelegateHander EditEvent;//声明一个事件
        private XmlDocument _fieldSetsXml;
        private string _xmlPath;
        private string _fieldName;
        //private PFModelConfig _pfModelConfig=null;
        protected bool IsEdit { get { return !string.IsNullOrWhiteSpace(_fieldName); } }
        public AddForm()
        {
            InitializeComponent();
        }
        public AddForm(XmlDocument fieldSetsXml,string xmlPath, string dataSet,string fieldName=null) :this()
        {
            _fieldSetsXml = fieldSetsXml;
            _xmlPath = xmlPath;
            lbDataSet.Text = dataSet;
            lbDataSet.Refresh();


            _fieldName = fieldName;
            if (IsEdit) {
                LoadField();
                tbName.ReadOnly = true;
                //this.Name = "编辑";
            }else
            {
                //this.Name = "新增";
                cbType.SelectedIndex = 0;
                nudSqlLength.Value = 100;
                cbHaveSqlLength.Checked = false;
                nudSqlLength.ReadOnly = true;
                nudWidth.Value = 100;
                cbHaveWidth.Checked = false;
                nudWidth.ReadOnly = true;
                cbVisible.Checked = true;
            }
        }
        private void LoadField()
        {
            var fieldNode = FieldSetsHelper.FindField(_fieldSetsXml, lbDataSet.Text, _fieldName);
            if (fieldNode != null)
            {
                var config = new PFModelConfig(fieldNode, lbDataSet.Text);
                tbName.Text = config.FieldName;
                tbText.Text = config.FieldText;
                if (config.FieldType != null) { cbType.SelectedItem = PFDataHelper.GetStringByType(config.FieldType); }
                cbVisible.Checked = config.Visible;
                if (config.FieldSqlLength != null)
                {
                    nudSqlLength.Value = config.FieldSqlLength.Value;
                    nudSqlLength.ReadOnly = !(cbHaveSqlLength.Checked = true);
                }
                else
                {
                    nudSqlLength.ReadOnly = !(cbHaveSqlLength.Checked = false);
                }
                if (config.FieldWidth != null)
                {
                    nudWidth.Value = decimal.Parse(config.FieldWidth.Replace("px", ""));
                    nudWidth.ReadOnly=!(cbHaveWidth.Checked = true);
                }else
                {
                    nudWidth.ReadOnly = !(cbHaveWidth.Checked = false);
                }
                //_pfModelConfig = config;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsFormValid()) { return; }
            if (IsEdit) {
                DoEdit();
            }
            else{
                DoAdd();
            }
        }
        private bool IsFormValid() {            
            if (!IsEdit)
            {
                if (string.IsNullOrWhiteSpace(tbName.Text))
                {
                    MessageBox.Show("FieldName不能为空");
                    return false;
                }
            }
            if (string.IsNullOrWhiteSpace(tbText.Text))
            {
                MessageBox.Show("FieldText不能为空");
                return false;
            }
            return true;
        }
        private void DoAdd()
        {
            var old = FieldSetsHelper.FindField(_fieldSetsXml, lbDataSet.Text, tbName.Text);
            if (old != null)
            {
                MessageBox.Show(string.Format("{0}中已有存在{1}",lbDataSet.Text,tbName.Text));
                return;
            }
            var fields = _fieldSetsXml.SelectSingleNode(string.Format(@"DataSets/DataSet[@name='{0}']/Fields", lbDataSet.Text));
            if (fields != null)
            {
                XmlNode fieldNode = _fieldSetsXml.CreateNode("element", "Field", "");
                XmlNode nameNode = _fieldSetsXml.CreateNode("element", "FieldName", "");
                nameNode.InnerText = tbName.Text;
                fieldNode.AppendChild(nameNode);

                UpdateFieldNodeByForm(ref fieldNode);

                #region old
                //XmlNode textNode = _fieldSetsXml.CreateNode("element", "FieldText", "");
                //textNode.InnerText = tbText.Text;
                //XmlNode sqlLengthNode = _fieldSetsXml.CreateNode("element", "FieldSqlLength", "");
                //sqlLengthNode.InnerText = nudSqlLength.Value.ToString();
                //XmlNode widthNode = _fieldSetsXml.CreateNode("element", "FieldWidth", "");
                //widthNode.InnerText = nudWidth.Value.ToString();
                //fieldNode.AppendChild(textNode);
                //if (cbType.SelectedItem != null)
                //{
                //    XmlNode typeNode = _fieldSetsXml.CreateNode("element", "FieldType", "");
                //    typeNode.InnerText = cbType.SelectedItem.ToString();
                //    fieldNode.AppendChild(typeNode);
                //}
                //if (cbHaveSqlLength.Checked) { fieldNode.AppendChild(sqlLengthNode); }
                //if (cbHaveWidth.Checked) { fieldNode.AppendChild(widthNode); }
                //if (!cbVisible.Checked)
                //{
                //    var visibleNode = _fieldSetsXml.CreateNode("element", "Visible", "");
                //    visibleNode.InnerText = "false";
                //    fieldNode.AppendChild(visibleNode);
                //} 
                #endregion

                //添加为根元素的第一层子结点
                fields.AppendChild(fieldNode);
                _fieldSetsXml.Save(_xmlPath);

                MessageBox.Show("新增成功");
                AddEvent(tbName.Text);
                this.Close();
            }
        }
        private void UpdateModelByForm(ref PFModelConfig model)
        {

        }
        private void UpdateFieldNodeByModel(ref XmlNode xmlNode,ref PFModelConfig model)
        {

        }

        /// <summary>
        /// 更新Field节点的属性节点值
        /// </summary>
        /// <param name="fieldNode">Field节点</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">属性值</param>
        /// <param name="saveCondition">保存条件(不满足时移除属性节点)</param>
        private void UpdateFieldNodeProperty(ref XmlNode fieldNode, string propertyName, string value,
            Func<bool> saveCondition)
        {
            var needSave = saveCondition();
            var w = fieldNode.SelectSingleNode(propertyName);
            if (w != null)
            {
                if (needSave)
                {
                    w.InnerText = value;
                }
                else
                {
                    fieldNode.RemoveChild(w);
                }
            }
            else
            {
                if (needSave)
                {
                    XmlNode widthNode = _fieldSetsXml.CreateNode("element", propertyName, "");
                    widthNode.InnerText = value;
                    fieldNode.AppendChild(widthNode);
                }
            }
        }
        private void UpdateFieldNodeByForm(ref XmlNode fieldNode)
        {
            UpdateFieldNodeProperty(ref fieldNode, "FieldText", tbText.Text, () => true);
            UpdateFieldNodeProperty(ref fieldNode, "FieldType",cbType.SelectedItem as string , () => !PFDataHelper.StringIsNullOrWhiteSpace(cbType.SelectedItem as string));
            UpdateFieldNodeProperty(ref fieldNode, "FieldSqlLength", nudSqlLength.Value.ToString(), () => cbHaveSqlLength.Checked);
            UpdateFieldNodeProperty(ref fieldNode, "FieldWidth", nudWidth.Value.ToString(), () => cbHaveWidth.Checked);
            UpdateFieldNodeProperty(ref fieldNode, "Visible", cbVisible.Checked?"true": "false", () => !cbVisible.Checked);//其实,true时没有保存节点
            
            #region old
            //var n = fieldNode.SelectSingleNode("FieldText");
            //if (n != null) { n.InnerText = tbText.Text; }
            //var t = fieldNode.SelectSingleNode("FieldType");
            //if (t != null) { t.InnerText = cbType.SelectedItem.ToString(); }

            //var l = fieldNode.SelectSingleNode("FieldSqlLength");
            //if (l != null)
            //{
            //    if (cbHaveSqlLength.Checked) { l.InnerText = nudSqlLength.Value.ToString(); } else { fieldNode.RemoveChild(l); }
            //}
            //else
            //{
            //    if (cbHaveSqlLength.Checked)
            //    {
            //        XmlNode sqlLengthNode = _fieldSetsXml.CreateNode("element", "FieldSqlLength", "");
            //        sqlLengthNode.InnerText = nudSqlLength.Value.ToString();
            //        fieldNode.AppendChild(sqlLengthNode);
            //    }
            //}

            //var w = fieldNode.SelectSingleNode("FieldWidth");
            //if (w != null)
            //{
            //    if (cbHaveWidth.Checked) { w.InnerText = nudWidth.Value.ToString(); } else { fieldNode.RemoveChild(w); }
            //}
            //else
            //{
            //    if (cbHaveWidth.Checked)
            //    {
            //        XmlNode widthNode = _fieldSetsXml.CreateNode("element", "FieldWidth", "");
            //        widthNode.InnerText = nudWidth.Value.ToString();
            //        fieldNode.AppendChild(widthNode);
            //    }
            //}
            //var v = fieldNode.SelectSingleNode("Visible");
            //if (v != null)
            //{
            //    if (cbVisible.Checked) { fieldNode.RemoveChild(v); }
            //}
            //else
            //{
            //    if (!cbVisible.Checked)
            //    {
            //        XmlNode visibleNode = _fieldSetsXml.CreateNode("element", "Visible", "");
            //        visibleNode.InnerText = "false";
            //        fieldNode.AppendChild(visibleNode);
            //    }
            //} 
            #endregion
        }

        private void DoEdit()//现在这样每个属性单独判断(而不是生成一个Field大节点来替换)有个好外,就是如果某程序需要在xml加一个属性,但没更新本功能时,使用本功能不会影响未纳入管理的属性
        {
            var fieldNode = FieldSetsHelper.FindField(_fieldSetsXml, lbDataSet.Text, _fieldName);
            if (fieldNode != null)
            {
                UpdateFieldNodeByForm(ref fieldNode);

                #region old
                //var n = fieldNode.SelectSingleNode("FieldText");
                //if (n != null) { n.InnerText = tbText.Text; }
                //var t = fieldNode.SelectSingleNode("FieldType");
                //if (t != null) { t.InnerText = cbType.SelectedItem.ToString(); }

                //var l = fieldNode.SelectSingleNode("FieldSqlLength");
                //if (l != null)
                //{
                //    if (cbHaveSqlLength.Checked) { l.InnerText = nudSqlLength.Value.ToString(); } else { fieldNode.RemoveChild(l); }
                //}
                //else
                //{
                //    if (cbHaveSqlLength.Checked)
                //    {
                //        XmlNode sqlLengthNode = _fieldSetsXml.CreateNode("element", "FieldSqlLength", "");
                //        sqlLengthNode.InnerText = nudSqlLength.Value.ToString();
                //        fieldNode.AppendChild(sqlLengthNode);
                //    }
                //}

                //var w = fieldNode.SelectSingleNode("FieldWidth");
                //if (w != null) {
                //    if (cbHaveWidth.Checked) { w.InnerText = nudWidth.Value.ToString(); } else { fieldNode.RemoveChild(w); }
                //}else
                //{
                //    if (cbHaveWidth.Checked)
                //    {
                //        XmlNode widthNode = _fieldSetsXml.CreateNode("element", "FieldWidth", "");
                //        widthNode.InnerText = nudWidth.Value.ToString();
                //        fieldNode.AppendChild(widthNode);
                //    }
                //}
                //var v = fieldNode.SelectSingleNode("Visible");
                //if (v != null)
                //{
                //    if (cbVisible.Checked) { fieldNode.RemoveChild(v); }
                //}
                //else
                //{
                //    if (!cbVisible.Checked)
                //    {
                //        XmlNode visibleNode = _fieldSetsXml.CreateNode("element", "Visible", "");
                //        visibleNode.InnerText = "false";
                //        fieldNode.AppendChild(visibleNode);
                //    }
                //} 
                #endregion

                _fieldSetsXml.Save(_xmlPath);
                MessageBox.Show("修改成功");
                EditEvent();
                this.Close();
            }
        }

        private void cbHaveSqlLength_CheckedChanged(object sender, EventArgs e)
        {
            nudSqlLength.ReadOnly = !cbHaveSqlLength.Checked;
        }
        private void cbHaveWidth_CheckedChanged(object sender, EventArgs e)
        {
            nudWidth.ReadOnly = !cbHaveWidth.Checked;
        }
    }
}
