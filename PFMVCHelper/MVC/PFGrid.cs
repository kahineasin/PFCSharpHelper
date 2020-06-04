using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Collections;
using System.Data;
using Perfect;
using System.Reflection;

namespace Perfect.MVC
{
    /// <summary>
    /// 表格组件--wxj
    /// </summary>
    public class PFGrid : PFComponent
    {
        private List<PFGridColumn> _columns = new List<PFGridColumn>();
        private string _title;
        private IList _model;
        private PFGridMultiHeader _header;
        private string _itemClass = "pfGridItem";
        private string _headClass = "pfGridHead";
        private SelectMode _selectMode;
        private bool _closeTree = false;
        /// <summary>
        /// 支持树型List<TreeListItem>
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(IList model)
        {
            _model = model;
        }
        /// <summary>
        /// DataTable不支持树型
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(DataTable model)
        {
            var result = new List<Dictionary<string, object>>();
            for (var i = 0; i < model.Rows.Count; i++)
            {
                DataRow row = model.Rows[i];
                var arr = row.ItemArray;
                var r = new Dictionary<string, object>();
                for (var j = 0; j < arr.Length; j++)
                {
                    var c = model.Columns[j].ColumnName;
                    r.Add(c, arr[j]);
                }
                result.Add(r);
            }
            _model = result;
        }
        public void SetSelectMode(SelectMode selectMode)
        {
            _selectMode = selectMode;
        }
        public SelectMode GetSelectMode()
        {
            return _selectMode;
        }
        public void SetTitle(string title)
        {
            _title = title;
        }

        public PFGridMultiHeader MultiHeader()
        {
            return _header = new PFGridMultiHeader();
        }
        public string GetHeadClass()
        {
            return _headClass;
        }
        public void CloseTree()
        {
            _closeTree = true;
        }

        public virtual void ColumnsFor<TModel>(Action<PFGridColumnCollection<TModel>> action)
        {
            var cs = new PFGridColumnCollection<TModel> { Grid = this };
            action(cs);
            cs.ForEach(_columns.Add);
        }
        public virtual void ColumnsFor(string modelConfigName, Action<PFGridColumnCollection> action)
        {
            var cs = new PFGridColumnCollection { Grid = this };
            if (!string.IsNullOrWhiteSpace(modelConfigName)) { cs.SetModelConfig(modelConfigName); }
            action(cs);
            cs.ForEach(_columns.Add);
        }
        public virtual void ColumnsForAll(string modelConfigName)
        {
            var cs = new PFGridColumnCollection { Grid = this };
            if (!string.IsNullOrWhiteSpace(modelConfigName)) { cs.SetModelConfig(modelConfigName); }

            if (_model != null && _model.Count > 0)
            {
                if (_model[0] is TreeListItem)
                {
                    PFDataHelper.EachObjectProperty((_model[0] as TreeListItem).Data, (i, name, value) =>
                    {
                        ////旧版只有一句?待验证--benjamin todo 20191014
                        //cs.Add(name);

                        var c = cs.Add(name);
                        if (value != null) { PFGridColumn.SetStyleByDataType(c, value.GetType()); }
                    });
                }
                else if (_model[0] is Dictionary<string, object>)//新版才有这段代码?待验证--benjamin todo 20191014
                {
                    var dict = (_model[0] as Dictionary<string, object>);
                    foreach (var key in dict.Keys)
                    {
                        var c = cs.Add(key);
                        var value = dict[key];
                        if (value != null) { PFGridColumn.SetStyleByDataType(c, value.GetType()); }
                    }
                }
            }
            cs.ForEach(_columns.Add);
        }
        /// <summary>
        /// 生成header和columns
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="generateColumn">是否生成column</param>
        public virtual void Columns(List<StoreColumn> columns, bool generateColumn = true)//,string modelConfigName=null)
        {
            //var modelConfig= PFDataHelper.GetMultiModelConfig(modelConfigName);
            var header = MultiHeader();

            columns.ForEach(a =>
            {
                AppendHeader(header, a, generateColumn);
                //var c = new PFGridColumn(a);
                //if (a.Children.Any())
                //{
                //    var h = new PFGridMultiHeader(a.title);
                //    h.AddChildren(a.Children.Select(b => new PFGridColumn (b) ).ToArray());
                //    header.AddChildren(h);
                //}else
                //{
                //    _columns.Add(c);
                //    header.AddChildren(c);
                //}
            });
            //var root = new TreeListItem { Children = columns.Select(a=>new TreeListItem { Data=a}).ToList() };
            //root.EachChild(a=> {

            //});
            //columns
            //var cs = new PFGridColumnCollection { Grid = this };
            //if (!string.IsNullOrWhiteSpace(modelConfigName)) { cs.SetModelConfig(modelConfigName, modelConfigName); }

            //if (_model != null && _model.Count > 0)
            //{
            //    if (_model[0] is TreeListItem)
            //    {
            //        PFDataHelper.EachObjectProperty((_model[0] as TreeListItem).Data, (i, name, value) => {
            //            cs.Add(name);
            //        });
            //    }
            //}
            //cs.ForEach(_columns.Add);
        }
        public virtual void ClearColumns()
        {
            _columns.Clear();
        }

        private void AppendHeader(PFGridMultiHeader p, StoreColumn a, bool generateColumn = true)
        {
            if (a.Children.Any())
            {
                var h = new PFGridMultiHeader(a.title);
                a.Children.ForEach(b =>
                {
                    AppendHeader(h, b, generateColumn);
                });
                //h.AddChildren(a.Children.Select(b => new PFGridColumn(b)).ToArray());
                p.AddChildren(h);
            }
            else
            {
                var c = new PFGridColumn(a);
                if (generateColumn)
                {
                    _columns.Add(c);
                }
                p.AddChildren(c);
            }
            //cs.ForEach(a => {
            //    var c = new PFGridColumn(a);
            //    p..Add(c);
            //    if (a.Children.Any())
            //    {
            //        AppendChildColumn(c, a.Children);
            //    }
            //});
        }
        private object GetCellText(object row, PFGridColumn c)
        {
            object val = null;
            if (!string.IsNullOrWhiteSpace(c.DataIndex))
            {
                if (row is DataRow)
                {
                    val = ((DataRow)row)[c.DataIndex];
                }
                else if (row is IDictionary)
                {
                    val = (row as IDictionary)[c.DataIndex];
                }
                else
                {
                    //var pi = row.GetType().GetProperty(c.DataIndex);
                    var pi = c.GetOrSetPropertyInfo(() => row.GetType().GetProperty(c.DataIndex));
                    val = pi.GetValue(row, null);
                }
            }
            if (c.Render != null)
            {
                val = c.Render(c, row, val);
            }
            //if (c.Render == null)
            //{
            //    if (row is DataRow)
            //    {
            //        val = ((DataRow)row)[c.DataIndex];
            //    }else if(row is IDictionary)
            //    {
            //        val = (row as IDictionary)[c.DataIndex];
            //    }
            //    else
            //    {
            //        var pi = row.GetType().GetProperty(c.DataIndex);
            //        val = pi.GetValue(row, null);
            //    }
            //}
            //else
            //{
            //    if (row is DataRow)
            //    {
            //       var temval = ((DataRow)row)[c.DataIndex];
            //        val = c.Render(c, row, temval);
            //    }
            //    else
            //    {
            //        var pi = row.GetType().GetProperty(c.DataIndex);
            //        var temval = pi.GetValue(row, null);
            //        val = c.Render(c, row, temval);
            //    }
            //}
            return val;
        }
        private string RowToHtml(object row)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<tr class='{0}' >", _itemClass);
            _columns.ForEach(c =>
            {
                object val;
                val = GetCellText(row, c);
                //sb.AppendFormat("<td {1}>{0}</td>", val, c.GetStyle());

                //这个是新版?待验证--benjamin todo
                var style = "";
                if (c.Visible == false) { style = " style='display:none' "; }//好像有col-h的class代表隐藏--benjamin20191014
                sb.AppendFormat("<td {1} {2}>{0}</td>", val, c.GetClassName(), style);//样式加到表头
                //yjquery2018项目中是这句,应该是旧版?
                //sb.AppendFormat("<td >{0}</td>", val);//样式加到表头

            });
            sb.Append("</tr>");
            return sb.ToString();
        }
        private string GetClassByTreeMatrixNetLine(TreeMatrixNet net)
        {
            if (net.HasFlag(TreeMatrixNet.Up) && net.HasFlag(TreeMatrixNet.Right) && net.HasFlag(TreeMatrixNet.Down))
            {
                //return "tree-tr-linearea-urd";
                return "linearea-urd";
            }
            if (net.HasFlag(TreeMatrixNet.Up) && net.HasFlag(TreeMatrixNet.Down))
            {
                //return "tree-tr-linearea-ud";
                return "linearea-ud";
            }
            if (net.HasFlag(TreeMatrixNet.Up) && net.HasFlag(TreeMatrixNet.Right))
            {
                //return "tree-tr-linearea-ur";
                return "linearea-ur";
            }
            return "";
        }
        private string TreeCellToHtml(TreeListItem row, int rowIdx, int level, PFGridColumn column, TreeMatrix matrix, bool isFirstColumn)
        {
            var sb = new StringBuilder();
            object val = GetCellText(row.Data, column);

            if (isFirstColumn)
            {
                //var css = "tree-tr-hitarea tree-tr-hitarea-expanded";
                var css = _closeTree ? "hitarea hitarea-closed" : "hitarea hitarea-expanded";
                var line = "";
                for (var j = 0; j < level; j++)
                {
                    //line += string.Format("<div class='{0} {1}'></div>", "tree-tr-linearea ", GetClassByTreeMatrixNetLine(matrix.GetNetLine(j, rowIdx)));
                    line += string.Format("<div class='{0} {1}'></div>", "linearea ", GetClassByTreeMatrixNetLine(matrix.GetNetLine(j, rowIdx)));
                }
                val = line + string.Format("<div class='{0}'></div>", css) + val;

                //前面的GetCellText已经有调用render了，这里就不用了--benjamin20190705
                ////sb.AppendFormat("<td {1}  onclick='$pf.expandTree(this)'>{0}</td>", val, column.GetStyle());
                //if (column.Render != null)
                //{
                //    sb.AppendFormat("<td>{0}</td>", column.Render(column, row, (string)val)) ;
                //}
                //else
                //{
                sb.AppendFormat("<td>{0}</td>", val);//样式不要加到tbody上,否则页面太大了,onclick事件改到pfTreeTable的init上
                //}

            }
            else
            {
                //if (column.Render != null)
                //{
                //    //sb.AppendFormat("<td>{0}</td>", column.Render(column, row, (string)val));//样式不要加到tbody上,否则页面太大了
                //    sb.AppendFormat("<td {1}>{0}</td>", column.Render(column, row, (string)val), column.GetClassName());//样式不要加到tbody上,否则页面太大了,加上类名的页面大小多了1/23,但为了数字列右对齐
                //}
                //else
                //{
                //sb.AppendFormat("<td {1}>{0}</td>", val, column.GetStyle());
                sb.AppendFormat("<td {1}>{0}</td>", val, column.GetClassName());//样式不要加到tbody上,否则页面太大了
                //}
            }
            return sb.ToString();
        }
        public string TreeRowToHtml(TreeListItem row, int rowIdx, int depth, TreeMatrix matrix)//, int level = 0)
        {
            var sb = new StringBuilder();
            int level = depth - 1;
            //sb.AppendFormat("<tr class='{0}' expanded='expanded' level='{1}' >", _itemClass, level);
            sb.AppendFormat("<tr class='{0}' level='{1}' {2} {3}>",
                _itemClass,
                level,
                _closeTree ? "" : "expanded='expanded'",
                _closeTree && level > 0 ? "style='display:none'" : "");

            //选择列
            var selectTh = "";
            //var depth = GetDepth() - 1;
            //var selectThRowSpan = depth > 1 ? ("rowspan=" + _rowSpan) : "";
            switch (GetSelectMode())
            {
                case PFGrid.SelectMode.Single:
                case PFGrid.SelectMode.Multi:
                    selectTh = "<td><input type=\"checkbox\" class=\"pf-row-select\"></td>";
                    break;
                default:
                    break;
            }
            sb.Append(selectTh);

            //暂时固定第一列为树型列--
            int i = 0;
            if (_header != null)
            {
                _header.EachLeaf(h =>
                {
                    var column = _columns.First(c => c.Text == h.Text);
                    sb.Append(TreeCellToHtml(row, rowIdx, level, column, matrix, (_header.FirstLeaf(a => (a).Visible)).Text == column.Text));
                    if (h.Visible) { i++; }
                });
            }
            else
            {
                _columns.ForEach(column =>
                {
                    sb.Append(TreeCellToHtml(row, rowIdx, level, column, matrix, column == _columns[0]));
                });
            }
            sb.Append("</tr>");

            return sb.ToString();
        }

        #region For DataTable Or IList
        /// <summary>
        /// 注意:Model为DataTable时没必要写自动生成列的方法,因为在PFGrid.ColumnsFor方法里循环Table生成也是很简单的
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public MvcHtmlString Html(HtmlHelper htmlHelper)
        {

            var sb = new StringBuilder();
            ////特性
            var htmlAttributes = GetHtmlAttributes();
            var sAttributes = "";
            foreach (var a in htmlAttributes)
            {
                sAttributes += string.Format(" {0}='{1}' ", a.Key, a.Value);
            }

            var configMapper = PFDataHelper.GetConfigMapper();
            var pathConfig = configMapper.GetPathConfig();


            if (htmlHelper.ViewData["hasPFGridCss"] == null)
            {
                sb.AppendFormat("<link href=\"/{0}/PFGrid.css\" rel=\"stylesheet\" />", pathConfig.CssPath);
                htmlHelper.ViewData["hasPFGridCss"] = true;
            }
            #region 标题
            //            if (!string.IsNullOrWhiteSpace(_title))
            //            {
            ////                sb.AppendFormat(@"
            ////                            <tr>
            ////                                <td style='' class='txt_86433c_12 pf-grid-head-td' align='left'>
            ////                                    <strong>◎ {0}</strong>
            ////                                </td>
            ////                            </tr>
            ////", _title);

            //            }
            #endregion 标题

            #region 主体table

            sb.AppendFormat(@"<table {0}>", sAttributes);

            bool hasData = _model != null && _model.Count > 0;
            #region 表头
            if (hasData)
            {
                if (_header != null)
                {
                    sb.Append(_header.Html(this));
                }
                else
                {
                    sb.AppendFormat("<thead><tr class='{0}'>", _headClass);
                    _columns.ForEach(c =>
                    {
                        var style = c.GetStyle(false);
                        if (!string.IsNullOrWhiteSpace(c.Width))
                        {
                            style += ";min-width:" + c.Width;
                        }
                        if (!string.IsNullOrWhiteSpace(style))
                        {
                            style = string.Format("style='{0} '", style);
                        }
                        sb.AppendFormat("<th {1} field-name='{2}' >{0}</th>", c.Text, style, c.DataIndex);
                        //var style = "";
                        //if (!c.Visible) { style += "display:none;"; }
                        //if (c.Width != null) { style += "width:" + c.Width + ";"; }
                        //if (!string.IsNullOrWhiteSpace(style)) { style = string.Format("style='{0}'", style); }
                        //sb.AppendFormat("<th {1}  >{0}</th>", c.Text, style);

                    });
                    sb.Append("</tr></thead>");
                }
            }
            #endregion 表头
            sb.Append(@"<tbody>");
            if (hasData)
            {
                #region 行内容
                int i = 0;
                bool isTree = false;
                foreach (var row in _model)
                {
                    if (row is TreeListItem)
                    {
                        isTree = true;
                        if (_columns.Any())
                        {
                            var style = "text-align:left;padding-left:5px;padding-right:5px;white-space:nowrap;";
                            if (_header != null)
                            {
                                _columns.First(c => c.Text == _header.FirstLeaf(a => a.Visible).Text).SetStyle(style);
                            }
                            else
                            {
                                _columns.First(a => a.Visible).SetStyle(style);
                            }
                        }
                        break;
                    }
                }
                if (isTree)
                {
                    var matrix = new TreeMatrix(_model);
                    foreach (var row in _model)
                    {
                        var tRow = row as TreeListItem;
                        sb.Append(TreeRowToHtml(tRow, i, 1, matrix));
                        i++;
                        tRow.EachChild((a, b) =>
                        {
                            //sb.Append(TreeRowToHtml(tRow, i, b, matrix));
                            sb.Append(TreeRowToHtml(a, i, b, matrix));
                            i++;
                        });
                    }
                }
                else
                {
                    foreach (var row in _model)
                    {
                        sb.Append(RowToHtml(row));
                    }
                }

                #endregion 行内容

            }
            else
            {
                sb.AppendFormat("<tr><td>{0}</td></tr>", "无相关数据");
                //sb.AppendFormat("<table style='width: 100%;'><tr class='{0}'><td style='text-align:center;color:red'>无相关数据</td></tr></table>", _itemClass);
            }
            sb.Append(@"</tbody>");
            sb.Append(@"</table>");

            #endregion 主体table


            return MvcHtmlString.Create(sb.ToString());
        }
        #endregion
        public enum SelectMode
        {
            None = 0,
            Single = 1,
            Multi = 2
        }
    }

    /// <summary>
    /// 表格列
    /// </summary>
    public class PFGridColumn
    {

        private Dictionary<string, string> _style = new Dictionary<string, string>();//(注意这里的样式仅对tbody生效,不生成到thead)
        private List<string> _className = new List<string>();
        public string Text { get; set; }
        public string DataIndex { get; set; }

        private string _width;
        //private bool _isFirstColumn;
        public string Width
        {
            get
            {
                return _width;
            }
            set
            {
                decimal num = 0;
                if (decimal.TryParse(value, out num))
                {
                    _width = num.ToString() + "px";
                }
                else
                {
                    _width = value;
                }
                if (!string.IsNullOrWhiteSpace(_width)) { SetStyle("width", _width); }
                else { RemoveStyle("width"); }
            }
        }
        private bool _visible = true;
        private PropertyInfo _propertyInfo = null;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (value)
                {
                    RemoveStyle("display");
                }
                else
                {
                    SetStyle("display", "none");
                }
                _visible = value;
            }
        }
        public PFGridColumn() { }
        public PFGridColumn(StoreColumn c)
        {
            this.DataIndex = c.data;
            this.Text = c.title ?? c.data;
            this.Width = c.width;
            this.Visible = c.visible;
            if (!this.Visible) { SetClassName("col-h"); }
            if (!string.IsNullOrWhiteSpace(c.dataType))
            {
                var dataType = PFDataHelper.GetTypeByString(c.dataType);
                SetStyleByDataType(this, dataType);
                //bool isPercent = dataType == typeof(PFPercent);
                //if (dataType == typeof(decimal) || dataType == typeof(int) || isPercent)
                //{
                //    SetStyle("text-align:right;padding-right: 9px");
                //    Render = (cc, r, v) =>
                //    {
                //        var rr = PFDataHelper.Thousandth(v);
                //        if (isPercent)
                //        {
                //            rr += " %";
                //        }
                //        return rr;
                //    };
                //    SetClassName("col-r");
                //}
            }
        }
        /// <summary>
        /// 为了减少反射的消耗--benjamin20190710
        /// </summary>
        /// <param name="nullAction"></param>
        /// <returns></returns>
        public PropertyInfo GetOrSetPropertyInfo(Func<PropertyInfo> nullAction)
        {
            if (_propertyInfo == null) { _propertyInfo = nullAction(); }
            return _propertyInfo;
        }
        public static void SetStyleByDataType(PFGridColumn c, Type dataType)
        {
            bool isPercent = dataType == typeof(PFPercent);
            if (dataType == typeof(decimal) || dataType == typeof(int) || isPercent)
            {
                c.SetStyle("text-align:right;padding-right: 9px");
                c.Render = (cc, r, v) =>
                {
                    var rr = PFDataHelper.Thousandth(v);
                    if (isPercent)
                    {
                        rr += " %";
                    }
                    return rr;
                };
                c.SetClassName("col-r");
            }
        }
        /// <summary>
        /// 设置列样式
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public PFGridColumn SetStyle(string style)
        {
            var list = style.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var i in list)
            {
                var kv = i.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (_style.ContainsKey(kv[0]))
                {
                    _style.Remove(kv[0]);
                }
                _style.Add(kv[0], kv[1]);
                if (kv[0] == "width")
                {
                    _width = kv[1];
                }
            }
            return this;
        }
        /// <summary>
        /// 设置列class
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public PFGridColumn SetClassName(string className)
        {
            var list = className.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var i in list)
            {
                if (_className.IndexOf(i) > -1)
                {
                    _className.Remove(i);
                }
                _className.Add(i);
            }
            return this;
        }
        public PFGridColumn SetTextAlign(TextAlign ta)
        {
            _className.Remove("col-r");
            switch (ta)
            {
                case TextAlign.Center:
                    SetStyle("text-align:center");
                    break;
                case TextAlign.Right:
                    SetStyle("text-align:right;padding-right: 9px");
                    _className.Add("col-r");
                    break;
                case TextAlign.Left:
                default:
                    SetStyle("text-align:left");
                    break;
            }
            return this;
        }
        public enum TextAlign
        {
            Left = 1,
            Center = 2,
            Right = 3
        }
        private void RemoveStyle(string key)
        {
            if (_style.ContainsKey(key)) { _style.Remove(key); }
        }
        private void SetStyle(string key, string value)
        {
            if (!_style.ContainsKey(key))
            {
                _style.Add(key, value);
            }
            else
            {
                _style[key] = value;
            }
        }
        /// <summary>
        /// 列样式
        /// </summary>
        public string GetStyle(bool withAttr = true)
        {
            var s = _style.Any() ? string.Join(";", _style.Select(a => a.Key + ":" + a.Value)) : "";
            if (string.IsNullOrWhiteSpace(s)) { return ""; }
            return withAttr ? string.Format("style='{0} '", s) : s;
        }
        /// <summary>
        /// 列class
        /// </summary>
        public string GetClassName()
        {
            //if(this.DataIndex== "jsjf")
            //{
            //    var aa = "aa";
            //}
            return _className.Any() ? string.Format("class='{0} '",
                string.Join(" ", _className)
                ) : "";
        }
        public PFGridColumn SetText(string text)
        {
            Text = text;
            return this;
        }
        //public void IsFirstColumn() {
        //    _isFirstColumn = true;
        //}
        //public bool IsFirstColumn { get{ return _isFirstColumn; }set{ _isFirstColumn = value; } }
        /// <summary>
        /// 列,行,值,返回值
        /// </summary>
        public Func<PFGridColumn, object, object, string> Render { get; set; }
    }
    public class PFGridColumnCollection : List<PFGridColumn>
    {
        protected PFModelConfigCollection _modelConfig;
        public PFGrid Grid;
        /// <summary>
        /// 增加列
        /// </summary>
        /// <param name="text">表头文字</param>
        /// <param name="render">渲染方法 参数:列,行(当为TreeListItem时是它的Data),值(当设置了DataIndex);返回:显示值</param>
        /// <returns></returns>
        public PFGridColumn Add(string text, Func<PFGridColumn, object, object, string> render)
        {
            var col = new PFGridColumn { Text = text, Render = render };
            this.Add(col);
            return col;
        }
        public PFGridColumn Add(string dataIndex)
        {
            var col = new PFGridColumn { DataIndex = dataIndex, Text = dataIndex };
            if (_modelConfig != null)
            {
                var config = _modelConfig[col.DataIndex];
                if (config != null)//为了页面可以修改列宽,所以应该像现在这样Add就加上配置
                {
                    col.Text = config.FieldText;

                    if (!string.IsNullOrWhiteSpace(config.FieldWidth)) { col.Width = config.FieldWidth; }
                    if (!config.Visible) { col.Visible = false; }

                    PFGridColumn.SetStyleByDataType(col, config.FieldType);
                    //bool isPercent = config.FieldType == typeof(PFPercent);
                    //if (config.FieldType == typeof(decimal) || config.FieldType == typeof(int) || isPercent)
                    //{
                    //    col.SetStyle("text-align:right;padding-right: 9px");
                    //    col.Render = (c, r,v) =>
                    //    {
                    //        var rr = PFDataHelper.Thousandth(v);
                    //        if (isPercent)
                    //        {
                    //            rr += " %";
                    //        }
                    //        return rr;
                    //    };
                    //    col.SetClassName("col-r");
                    //}
                }
            }
            if (//setWidthByHeaderWord && 
                string.IsNullOrWhiteSpace(col.Width))//设置为中文后进入这里才有意义
            {
                //var w = PFDataHelper.GetWordsWidth(col.Text,null,null,36);
                var w = PFDataHelper.GetWordsWidth(col.Text);
                if (w != null) { col.Width = w; }

            }
            //if (!string.IsNullOrWhiteSpace(col.Width))
            //{
            //    col.Width = (decimal.Parse(col.Width.Replace("px", "")) + 36) + "px";//36是padding左右的值
            //}
            //if (Count < 1)
            //{
            //    col.IsFirstColumn=true;
            //}
            this.Add(col);
            return col;
        }
        public PFGridColumn Add(DataColumn dc, Func<PFGridColumn, object, object, string> render = null)
        {
            var sc = new StoreColumn(dc, _modelConfig[dc.ColumnName]);
            var gc = new PFGridColumn(sc);
            if (render != null) { gc.Render = render; }
            this.Add(gc);
            return gc;
        }
        /// <summary>
        /// 增加操作列
        /// </summary>
        /// <returns></returns>
        public PFGridColumn AddOperateColumn()
        {
            var col = new PFGridColumn
            {
                Text = "操作",
                Render = (c, r, v) =>
                {
                    return string.Format(@"
<input type='button' name='btn_edit' value='编辑' onclick='$pf.fireEvent(""{0}"",""rowEdit"",[{1}])' />
<input type='button' name='btn_delete' value='删除'  onclick='$pf.fireEvent(""{0}"",""rowDelete"",[{1}])' />
", Grid.GetId(), Newtonsoft.Json.JsonConvert.SerializeObject(r));
                }
            };
            this.Add(col);
            return col;
        }
        public PFGridColumn AddOperateColumn(Action<PFGridOperateColumnConfig> configAction)
        {
            var config = new PFGridOperateColumnConfig();
            configAction(config);
            var col = new PFGridColumn
            {
                Text = config.ColumnHead,
                Render = (c, r, v) =>
                {
                    return string.Format(@"
<input type='button' name='{2}' value='{3}' onclick='$pf.fireEvent(""{0}"",""{2}"",[{1}])' />
", Grid.GetId(), Newtonsoft.Json.JsonConvert.SerializeObject(r), config.ColumnName, config.ColumnText);
                }
            };
            this.Add(col);
            return col;
        }

        public void SetModelConfig(string name, string fullName)
        {
            _modelConfig = PFDataHelper.GetModelConfig(name, fullName);
        }
        public void SetModelConfig(string names)
        {
            _modelConfig = PFDataHelper.GetMultiModelConfig(names);
        }
    }
    public class PFGridOperateColumnConfig
    {
        public PFGridOperateColumnConfig()
        {
            ColumnHead = "操作";
        }
        public string ColumnHead { get; set; }
        public string ColumnName { get; set; }
        public string ColumnText { get; set; }
    }
    public class PFGridColumnCollection<TModel> : PFGridColumnCollection//List<PFGridColumn>
    {

        public PFGridColumnCollection()
        {
            var modelType = typeof(TModel);
            SetModelConfig(modelType.Name, modelType.FullName);
        }
        /// <summary>
        /// 增加列
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp">字段表达式</param>
        /// <param name="text">表头文字</param>
        /// <returns></returns>
        public PFGridColumn Add<TProperty>(Expression<Func<TModel, TProperty>> exp, string text)
        {
            var col = Add(exp);
            col.Text = text;
            return col;
        }
        public PFGridColumn Add<TProperty>(Expression<Func<TModel, TProperty>> exp)
        {
            var col = Add(ExpressionHelper.GetExpressionText(exp));
            //字典类型的话,直接拿Key调用另一个重载就好了
            //var sExp = ExpressionHelper.GetExpressionText(exp);
            ////if (typeof(TModel) is IDictionary)//当是字典类型时,sExp型如: [xx]
            //if(typeof(TModel).Equals(typeof(Dictionary<string, object>)))
            //{
            //    if (sExp[0] == '[') { sExp = sExp.Substring(1); }
            //    if (sExp[sExp.Length-1] == ']') { sExp = sExp.Substring(0, sExp.Length - 1); }
            //}
            //var col = Add(sExp);
            return col;

        }

        /// <summary>
        /// 增加列
        /// </summary>
        /// <param name="text">表头文字</param>
        /// <param name="render">渲染方法 参数:列,行,值;返回:显示值</param>
        /// <returns></returns>
        public PFGridColumn Add<TProperty>(Expression<Func<TModel, TProperty>> exp, Func<PFGridColumn, object, object, string> render)
        {
            var col = Add(exp);
            col.Render = render;
            return col;
        }

    }
    /// <summary>
    /// 多表头
    /// </summary>
    public class PFGridMultiHeader : TreeListItem<PFGridMultiHeader>
    {
        #region Field
        private string _text;
        private int? _columnSpan;
        private int? _rowSpan;
        private bool _visible = true;
        private string _width;
        private string _dataIndex;
        //private bool _isFirstColumn;
        #endregion

        #region Property

        public bool Visible { get { return _visible; } private set { _visible = value; } }
        public string Text { get { return _text; } set { _text = value; } }
        #endregion

        #region Ctor
        public PFGridMultiHeader() { }

        /// <summary>
        ///     初始化,同时给表头赋值
        /// </summary>
        /// <param name="text">表头的文字</param>
        public PFGridMultiHeader(string text)
            : this()
        {
            _text = text;
        }
        public PFGridMultiHeader(PFGridColumn c)
            : this()
        {
            _text = c.Text;
            Visible = c.Visible; _width = c.Width; _dataIndex = c.DataIndex;
            // _isFirstColumn = c.IsFirstColumn;
        }
        #endregion

        /// <summary>
        ///     增加子元素
        /// </summary>
        /// <param name="childrens">子元素,支持多个</param>
        /// <returns></returns>
        public PFGridMultiHeader AddChildren(params PFGridMultiHeader[] childrens)
        {
            if (childrens != null)
            {
                foreach (var c in childrens)
                {
                    Children.Add(c);
                }
            }
            return this;
        }

        /// <summary>
        ///     增加子元素
        /// </summary>
        /// <param name="childrens">子元素,支持多个,要求TableCell类型</param>
        /// <returns></returns>
        public PFGridMultiHeader AddChildren(params PFGridColumn[] childrens)
        {
            if (childrens != null)
            {
                foreach (var c in childrens)
                {
                    //Children.Add(new PFGridMultiHeader(c.Text) { Visible = c.Visible , _width =c.Width, _isFirstColumn=c.IsFirstColumn });
                    Children.Add(new PFGridMultiHeader(c));
                }
            }
            return this;
        }

        /// <summary>
        ///     渲染到writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="style">表头样式</param>
        public string Html(PFGrid grid)
        {
            if (Children == null) { return ""; }

            var selectTh = "";
            var depth = GetDepth() - 1;
            var selectThRowSpan = depth > 1 ? ("rowspan=" + depth) : "";
            switch (grid.GetSelectMode())
            {
                case PFGrid.SelectMode.Single:
                    selectTh = string.Format("<th {0}>选择</th>", selectThRowSpan);
                    break;
                case PFGrid.SelectMode.Multi:
                    selectTh = string.Format("<th {0}><input type=\"checkbox\" class=\"pf-row-select-all\">选择</th>", selectThRowSpan);
                    break;
                default:
                    break;
            }
            var html = string.Format("<thead><tr  class='{0}' >{1}",
                grid.GetHeadClass(),
                selectTh
                );
            RenderListToWriter(ref html, grid, Children, depth);//第一层的header是不输出的,所以要减1
            html += "</tr></thead>";
            return html;
        }

        #region Private
        /// <summary>
        ///     渲染List到writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="lists">表头同一层的List</param>
        /// <param name="style">表头样式</param>
        /// <param name="maxDepth">最大深度</param>
        /// <param name="currentDepth">当前深度</param>
        private void RenderListToWriter(ref string writer, PFGrid grid, IList<PFGridMultiHeader> lists, int maxDepth, int currentDepth = 1)
        {
            //因为header内部是td,可以认为首尾已经各有一个tr
            if (lists != null)
            {
                //显示下一层
                var next = new List<PFGridMultiHeader>();
                //显示头
                foreach (var i in lists)
                {
                    i.RenderHeaderToWrite(ref writer, grid, maxDepth, currentDepth);

                    if (i.Children != null) next.AddRange(i.Children);//为了免于遍历两次,放到这里--wxj20180713
                }
                ////显示下一层
                //var next = new List<PFGridMultiHeader>();
                //foreach (var i in lists)
                //{
                //    if (i.Children != null) next.AddRange(i.Children);
                //}
                if (next.Count > 0)
                {
                    writer += "</tr>";
                    writer += string.Format("<tr class='{0}'>", grid.GetHeadClass());
                    RenderListToWriter(ref writer, grid, next, maxDepth, ++currentDepth);
                }
            }
        }



        /// <summary>
        ///     渲染表头td到writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="maxDepth">最大深度</param>
        /// <param name="currentDepth">当前深度</param>
        private void RenderHeaderToWrite(ref string writer, PFGrid grid, int maxDepth, int currentDepth)
        {
            if (string.IsNullOrWhiteSpace(_text)) return;
            //if (Children != null && Children.Count > 1)
            if (Children != null && Children.Count > 0)//大于0才是对的--wxj20180906
            {
                _columnSpan = GetAllLeafCount(a => a.Visible); //GetAllChildrenCount();//
            }
            else if (maxDepth > currentDepth)
            {
                _rowSpan = maxDepth - currentDepth + 1;
            }
            var styles = new List<string>();
            if (!Visible) { styles.Add("display:none"); }
            if (//(!_isFirstColumn)&&
                (!string.IsNullOrWhiteSpace(_width)))
            {
                styles.Add("width:" + _width);
                styles.Add("min-width:" + _width);//th的width是不会撑开table的,但min-width可以
            };
            //if (!string.IsNullOrWhiteSpace(_width)) { styles.Add("width:"+_width); }//现在的树组件,如果只有一列不设置宽度时,那列的样式就会很长,另外,树型列的宽度也不应该设置.所以暂时让TreeGrid自己控制列宽吧
            var style = styles.Any() ? string.Format("style ='{0}' ", string.Join(";", styles)) : "";
            var dataIndex = string.IsNullOrWhiteSpace(_dataIndex) ? "" : ("field-name=" + _dataIndex);
            writer += string.Format("<th {1} {2} {3} {4} >{0}</th>", _text, _columnSpan > 1 ? ("colspan=" + _columnSpan) : "", _rowSpan > 1 ? ("rowspan=" + _rowSpan) : "", style, dataIndex);
        }


        #endregion
    }
}
