using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;

namespace Perfect
{
    /// <summary>
    /// Net帮助类
    /// </summary>
    public static class PFNetHelper
    {

        /// <summary>
        /// DropDownList控件数据绑定
        /// </summary>
        /// <param name="dt">数据源表,DataTable或List</param>
        /// <param name="ddlctrl">要绑定的控件</param>
        /// <param name="textname">显示字段名</param>
        /// <param name="valuename">值字段名</param>
        public static void DropDownListBind(object dt, ref DropDownList ddlctrl, string textname, string valuename)
        {
            ddlctrl.DataSource = dt;
            ddlctrl.DataTextField = textname;
            ddlctrl.DataValueField = valuename;
            ddlctrl.DataBind();
        }

        public static void DropDownListSelectText(ref DropDownList ddlctrl, string text)
        {
            foreach (ListItem i in ddlctrl.Items)
            {
                if (i.Text == text) { ddlctrl.SelectedValue = i.Value; return; }
            }
        }
        /// <summary>
        /// 控件绑定模型配置
        /// </summary>
        /// <param name="page"></param>
        /// <param name="modelName">实现IPFConfigMapper的类中定义</param>
        /// <param name="list"></param>
        public static void BindModel(string modelName, List<ModelBindConfig> list)
        {
            var modelConfig = PFDataHelper.GetMultiModelConfig(modelName);
            foreach (var i in list)
            {
                PFModelConfig config = null;
                if (modelConfig != null) { config = modelConfig[i.GetPropertyName()]; }
                if (i.GetComponent() is System.Web.UI.WebControls.Label)
                {
                    System.Web.UI.WebControls.Label tmpControl = (System.Web.UI.WebControls.Label)i.GetComponent();
                    LabelBind(tmpControl, config);
                }
                else if (i.GetComponent() is System.Web.UI.WebControls.TextBox)
                {
                    System.Web.UI.WebControls.TextBox tmpControl = (System.Web.UI.WebControls.TextBox)i.GetComponent();
                    BindValidator(tmpControl, config, i.GetValidator());
                    LabelBind(i.GetLabel(), config);
                    TextBoxBind(tmpControl, config);
                }
                else if (i.GetComponent() is System.Web.UI.WebControls.DropDownList)
                {
                    System.Web.UI.WebControls.DropDownList tmpControl = (System.Web.UI.WebControls.DropDownList)i.GetComponent();
                    BindValidator(tmpControl, config, i.GetValidator());
                    LabelBind(i.GetLabel(), config);
                }
            }
        }

        /// <summary>
        /// 控件绑定模型配置
        /// </summary>
        /// <param name="page"></param>
        /// <param name="modelName">实现IPFConfigMapper的类中定义</param>
        /// <param name="list"></param>
        public static void GridBindModel(DataGrid grid, string modelName)
        {
            var modelConfig = PFDataHelper.GetMultiModelConfig(modelName);
            if (modelConfig != null)
            {
                if (grid.Columns != null)
                {
                    foreach (DataGridColumn c in grid.Columns)
                    {
                        if (c is BoundColumn)
                        {
                            BoundColumn tc = c as BoundColumn;
                            PFModelConfig config = null;
                            if (modelConfig != null) { config = modelConfig[tc.DataField]; }
                            if (config != null && (PFDataHelper.StringIsNullOrWhiteSpace(tc.HeaderText) || tc.HeaderText == config.FieldName)) { tc.HeaderText = config.FieldText; }
                        }
                        else if (c is TemplateColumn)
                        {
                            TemplateColumn tc = c as TemplateColumn;
                            PFModelConfig config = null;
                            if (modelConfig != null) { config = modelConfig[tc.HeaderText]; }
                            if (config != null && (PFDataHelper.StringIsNullOrWhiteSpace(tc.HeaderText) || tc.HeaderText == config.FieldName)) { tc.HeaderText = config.FieldText; }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 控件绑定模型配置
        /// </summary>
        /// <param name="page"></param>
        /// <param name="modelName">实现IPFConfigMapper的类中定义</param>
        /// <param name="list"></param>
        public static void GridBindModel(GridView grid, string modelName)
        {
            var modelConfig = PFDataHelper.GetMultiModelConfig(modelName);
            if (modelConfig != null)
            {
                if (grid.Columns != null)
                {
                    foreach (DataGridColumn c in grid.Columns)
                    {
                        if (c is BoundColumn)
                        {
                            BoundColumn tc = c as BoundColumn;
                            PFModelConfig config = null;
                            if (modelConfig != null) { config = modelConfig[tc.DataField]; }
                            if (config != null && (PFDataHelper.StringIsNullOrWhiteSpace(tc.HeaderText) || tc.HeaderText == config.FieldName)) { tc.HeaderText = config.FieldText; }
                        }
                        else if (c is TemplateColumn)
                        {
                            TemplateColumn tc = c as TemplateColumn;
                            PFModelConfig config = null;
                            if (modelConfig != null) { config = modelConfig[tc.HeaderText]; }
                            if (config != null && (PFDataHelper.StringIsNullOrWhiteSpace(tc.HeaderText) || tc.HeaderText == config.FieldName)) { tc.HeaderText = config.FieldText; }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据row设置控件的value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="list"></param>
        public static void SetControlByRow(DataRow row, Dictionary<Control, string> list)
        {
            foreach (var i in list)
            {
                object val = row[i.Value];
                if (i.Key is System.Web.UI.WebControls.TextBox)//现时使用TextBox的有:decimal,int,string,DateTime.(NumberEditor是继承TextBox的)
                {
                    System.Web.UI.WebControls.TextBox tmpControl = (System.Web.UI.WebControls.TextBox)i.Key;
                    if (val != null && val != DBNull.Value)
                    {
                        if (val is DateTime) { tmpControl.Text = PFDataHelper.ObjectToDateString(val, tmpControl.Attributes["dateFmt"]); }
                        if (val is string) { tmpControl.Text = val.ToString(); }
                        if (val is decimal) { tmpControl.Text = val.ToString(); }
                        if (val is int) { tmpControl.Text = val.ToString(); }
                    }
                }
                else if (i.Key is System.Web.UI.WebControls.DropDownList)
                {
                    System.Web.UI.WebControls.DropDownList tmpControl = (System.Web.UI.WebControls.DropDownList)i.Key;
                    tmpControl.SelectedValue = val.ToString();
                }
            }
        }
        /// <summary>
        /// 读取控件的value到object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="list"></param>
        public static void ReadControlToObject(object obj, Dictionary<Control, string> list)
        {
            string property = string.Empty;
            Type ot = obj.GetType();

            try
            {
                foreach (var i in list)
                {
                    var ti = ot.GetProperty(i.Value);
                    property = ti.Name;
                    var type = ti.PropertyType;
                    Type pt = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? type.GetGenericArguments()[0] : type;//获得非空类型

                    if (i.Key is System.Web.UI.WebControls.TextBox)
                    {
                        System.Web.UI.WebControls.TextBox tmpControl = (System.Web.UI.WebControls.TextBox)i.Key;
                        if (pt == typeof(String) || pt == typeof(string))
                        {
                            ti.SetValue(obj, tmpControl.Text, null);
                        }
                        else if (pt == typeof(decimal) || pt == typeof(int))
                        {
                            if (PFDataHelper.StringIsNullOrWhiteSpace(tmpControl.Text))
                            {
                                ti.SetValue(obj, null, null);//当ti为不可空的属性时,设null值不会报错,值为0
                            }
                            else
                            {
                                if (pt == typeof(decimal))
                                {
                                    ti.SetValue(obj, decimal.Parse(tmpControl.Text), null);
                                }
                                else if (pt == typeof(int))
                                {
                                    ti.SetValue(obj, int.Parse(tmpControl.Text), null);
                                }
                            }
                        }
                        else if (pt == typeof(DateTime))
                        {
                            DateTime d = new DateTime();
                            if (DateTime.TryParse(tmpControl.Text, out d))
                            {
                                ti.SetValue(obj, d, null);
                            }
                        }
                    }
                    else if (i.Key is System.Web.UI.WebControls.DropDownList)
                    {
                        System.Web.UI.WebControls.DropDownList tmpControl = (System.Web.UI.WebControls.DropDownList)i.Key;
                        ti.SetValue(obj, tmpControl.SelectedValue, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " Property:" + property);
            }
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isStatic"></param>
        public static void ShowMsgByPage(Page page, string msg)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(page, page.GetType(), "OPEN",
                string.Format("alert('" + msg.Replace('\'', '"').Replace("\r\n", "  ") + "');",
               "ShowMsgByMasterPage"
                ), true);
        }

        public static TableCell GetCell(this DataGridItem row, string headerText)
        {
            var grid = (DataGrid)row.Parent.Parent;
            for (var i = 0; i < grid.Columns.Count; i++)
            {
                var col = grid.Columns[i];
                if (col is BoundColumn)
                {
                    BoundColumn tmpCol = col as BoundColumn;
                    if (tmpCol.DataField == headerText || tmpCol.HeaderText == headerText) { return row.Cells[i]; }
                }
                else
                {
                    if (col.HeaderText == headerText) { return row.Cells[i]; }
                }
            }
            return null;
        }

        public static DataTable GetCacheDataTable(string tablename, Page page)
        {
            object objdt = page.Cache[tablename];
            return objdt == null ? null : (DataTable)objdt;
        }

        public static void SetCacheDataTable(DataTable dt, string tablename, Page page)
        {
            page.Cache[tablename] = dt;
        }

        /// <summary>
        ///     导出excel(中文名需要dataGrid,多页的数据需要table)
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="table"></param>
        /// <param name="page"></param>
        /// <param name="FileName"></param>
        public static void ExportToExcel(DataGrid dataGrid, System.Data.DataTable table, Page page, string FileName)
        {
            dataGrid.AllowPaging = false;
            dataGrid.DataSource = table;
            dataGrid.HeaderStyle.BackColor = System.Drawing.Color.Green;
            dataGrid.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            dataGrid.HeaderStyle.Font.Bold = true;

            dataGrid.Attributes.Add("style", "vnd.ms-excel.numberformat:@");

            dataGrid.DataBind();

            page.Response.Charset = "utf-8";
            page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            FileName = HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8);
            page.Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);// HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8)); //filename="*.xls";

            page.Response.ContentType = "application/ms-excel";
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            dataGrid.RenderControl(oHtmlTextWriter);
            page.Response.Output.Write(oStringWriter.ToString());
            page.Response.End();
        }
        /// <summary>
        ///     导出excel(中文名需要dataGrid,多页的数据需要table)
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="table"></param>
        /// <param name="page"></param>
        /// <param name="FileName"></param>
        public static void ExportToExcel(GridView gridView, System.Data.DataTable table, Page page, string FileName)
        {
            System.Web.UI.WebControls.DataGrid dataGrid = new System.Web.UI.WebControls.DataGrid();
            ExportToExcel(dataGrid, table, page, FileName);
        }
        #region Private
        /// <param name="validator">动态生成RequiredFieldValidator的方法不能实现</param>
        private static void BindValidator<TComponent>(TComponent component, PFModelConfig modelConfig, BaseValidator validator)
            where TComponent : WebControl
        {
            if (modelConfig != null && modelConfig.Required)
            {
                component.Attributes.Add("required", "required");
            }
            if (validator != null)
            {
                validator.ControlToValidate = component.ID;
                validator.Text = "必填";
                validator.Display = ValidatorDisplay.Dynamic;
                if (modelConfig != null) { validator.Enabled = modelConfig.Required; }
            }
        }
        /// <summary>
        /// 绑定label
        /// </summary>
        /// <typeparam name="TComponent">支持textbox</typeparam>
        /// <param name="component"></param>
        /// <param name="modelConfig"></param>
        /// <param name="label"></param>
        private static void LabelBind(Label label, PFModelConfig modelConfig)
        {
            if (modelConfig != null)
            {
                if (PFDataHelper.StringIsNullOrWhiteSpace(label.Text) || label.Text == modelConfig.FieldName
                    || label.Text == "Label"//Label控件的默认Text是Label
                    ) { label.Text = modelConfig.FieldText; }
                if (modelConfig.Required) { label.Text += "<span style='color:red'>*</span>"; }
                label.Text += ":";
            }
        }
        private static void TextBoxBind(TextBox textBox, PFModelConfig modelConfig)
        {
            if (modelConfig != null)
            {
                if (modelConfig.FieldType == typeof(decimal))
                {
                    textBox.Attributes.Add("onfocus", "this.oldvalue = this.value; ");
                    if (modelConfig.Precision.HasValue)
                    {
                        textBox.Attributes.Add("onchange", "if(!$pf.isDecimal(this.value," + modelConfig.Precision + ")){this.value=this.oldvalue;}");
                    }
                    else
                    {
                        textBox.Attributes.Add("onchange", "if(!$pf.isDecimal(this.value)){this.value=this.oldvalue;}");
                    }
                }
                else if (modelConfig.FieldType == typeof(int))
                {
                    textBox.Attributes.Add("onfocus", "this.oldvalue = this.value; ");
                    textBox.Attributes.Add("onchange", "if(!$pf.isInt(this.value)){this.value=this.oldvalue;}");
                }

                if (textBox.Width != null && textBox.Width.IsEmpty &&
                    (!PFDataHelper.StringIsNullOrWhiteSpace(modelConfig.FieldWidth)))
                {
                    textBox.Width = new Unit(modelConfig.FieldWidth);
                }
            }
        }
        #endregion
    }
    #region Class
    public class ModelBindConfig
    {
        private Control _component;
        private string _propertyName;
        private BaseValidator _validator;
        private Label _label;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component">表单控件</param>
        /// <param name="propertyName">模型属性名</param>
        public ModelBindConfig(Control component, string propertyName)
        {
            _component = component;
            _propertyName = propertyName;
        }

        public Control GetComponent()
        {
            return _component;
        }
        public string GetPropertyName()
        {
            return _propertyName;
        }
        public Label GetLabel()
        {
            return _label;
        }
        /// <summary>
        /// 动态生成控件到页面的方法暂时不能实现
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public ModelBindConfig SetLabel(Label component)
        {
            _label = component;
            return this;
        }
        public BaseValidator GetValidator()
        {
            return _validator;
        }
        /// <summary>
        /// 设置验证器(动态生成控件到页面的方法暂时不能实现)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public ModelBindConfig SetValidator(BaseValidator v)
        {
            _validator = v;
            return this;
        }
    }
    public class ModelBindConfigCollection : List<ModelBindConfig>
    {
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="com">表单控件</param>
        /// <param name="propertyName">模型的属性名</param>
        /// <param name="action"></param>
        public void Add(Control com, string propertyName, Action<ModelBindConfig> action = null)
        {
            var item = new ModelBindConfig(com, propertyName);
            if (action != null) { action(item); }
            Add(item);
        }
    }

    /// <summary>
    ///     列表页(含导出) 基类
    /// </summary>
    public abstract class ExportListPageBase : System.Web.UI.Page
    {
        protected string SelfName { get { return this.GetType().Name; } }
        public abstract string GetUserName();//sf.Pager.Session["username"]
        public abstract GridView GetMainGrid();//DataGrid

        /// <summary>
        ///     缓存Key前缀
        /// </summary>
        protected string CacheKeyPrev { get { return "yj_" + GetUserName() + "_" + SelfName; } }

        /// <summary>
        ///     缓存dateTable(用于导出)
        /// </summary>
        protected DataTable dt
        {
            get
            {
                return PFNetHelper.GetCacheDataTable(CacheKeyPrev + "_dt", this);
            }
            set
            {
                PFNetHelper.SetCacheDataTable(value ?? new DataTable(), CacheKeyPrev + "_dt", this);
            }
        }

        protected void databind(MyCustomControl.myPager MyPager1, Button btnExcel, Label lerror, string modelName = null)
        {
            if (dt != null)
            {
                MyPager1.SetDataSource(dt);
                var MyList = GetMainGrid();
                MyList.DataSource = MyPager1.myDataTable;
                //MyList.DataBind();
                if (modelName != null) { PFNetHelper.GridBindModel(MyList, modelName); }
                MyList.DataBind();

                var hasData = dt.Rows.Count > 0;
                MyPager1.Visible = MyList.Visible = hasData;
                if (btnExcel != null) { btnExcel.Visible = hasData; }
                lerror.Visible = !hasData;
                if (!hasData) { lerror.Text = "没有记录显示"; }
            }
        }
        /// <summary>
        ///     获得excel文件名--wxj20171212
        /// </summary>
        /// <returns></returns>
        protected virtual string GetExcelName(string tableName)
        {
            string strYY = "", strMM = "", strDD = "", strDate = "";
            strYY = DateTime.Today.Year.ToString();
            if (DateTime.Today.Month < 9)
                strMM = "0";
            strMM += DateTime.Today.Month.ToString();
            if (DateTime.Today.Day < 10)
                strDD = "0";
            strDD += DateTime.Today.Day.ToString();
            strDate = strYY + strMM + strDD;
            string fileName = tableName + "(" + strDate + ").xls";
            return fileName;
        }
    }
    #endregion

}