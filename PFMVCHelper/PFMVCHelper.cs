using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Perfect.MVC;
using System.Linq.Expressions;
using System.Collections;
using System.Data;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Perfect;

namespace Perfect
{
    /// <summary>
    /// MVC帮助类(考虑到解耦的问题,最好不引用System.Data;)(一般放到Controller项目)--wxj
    /// </summary>
    public static class PFMVCHelper
    {
        //public static SelectList DataTableToSelectList(DataTable dt)
        //{
        //    //创建一个属性的列表
        //    List<PropertyInfo> prlist = new List<PropertyInfo>();
        //    //获取TResult的类型实例  反射的入口
        //    Type t = typeof(T);
        //    //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表 
        //    Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
        //    //创建返回的集合
        //    List<T> oblist = new List<T>();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        //创建TResult的实例
        //        T ob = new T();
        //        //找到对应的数据  并赋值
        //        prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
        //        //放入到返回的集合中.
        //        oblist.Add(ob);
        //    }
        //    return oblist;
        //}
        #region 表单元素


        #region TextBox
        public static MvcHtmlString BuildTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFTextBox> action = null)
        {
            var component = new PFTextBox();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildTextBox(this HtmlHelper htmlHelper, string name, object value = null, Action<PFTextBox> action = null)
        {
            var component = new PFTextBox();
            component.SetPropertyFor(htmlHelper, name);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }
        #endregion
        #region TextArea
        public static MvcHtmlString BuildTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFTextArea> action = null)
        {
            var component = new PFTextArea();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildTextArea(this HtmlHelper htmlHelper, string name, object value = null, Action<PFTextArea> action = null)
        {
            var component = new PFTextArea();
            component.SetPropertyFor(htmlHelper, name);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }
        #endregion

        #region DropDownList
        public static MvcHtmlString BuildDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFDropDownList> action = null)
        {
            var component = new PFDropDownList();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildDropDownList(this HtmlHelper htmlHelper, string name, object value = null, Action<PFDropDownList> action = null)
        {
            var component = new PFDropDownList();
            component.SetPropertyFor(htmlHelper, name);
            if (value != null) { component.SetSelectedValue(value.ToString()); }
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }

        #endregion
        #region CheckBoxList
        //public static MvcHtmlString BuildCheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFCheckBoxList> action = null)
        //{
        //    var component = new PFCheckBoxList();
        //    component.SetPropertyFor(htmlHelper, expression);
        //    if (action != null) { action(component); }
        //    return component.Html(htmlHelper, expression);
        //}
        public static MvcHtmlString BuildCheckBoxList(this HtmlHelper htmlHelper, string name, object value = null, Action<PFCheckBoxList> action = null)
        {
            var component = new PFCheckBoxList();
            component.SetPropertyFor(htmlHelper, name);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }

        #endregion
        #region RadioButtonList
        public static MvcHtmlString BuildRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFRadioButtonList> action = null)
        {
            var component = new PFRadioButtonList();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildRadioButtonList(this HtmlHelper htmlHelper, string name, object value = null, Action<PFRadioButtonList> action = null)
        {
            var component = new PFRadioButtonList();
            component.SetPropertyFor(htmlHelper, name);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }

        #endregion
        #region DateBox
        public static MvcHtmlString BuildDateBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFDateBox> action = null)
        {
            var component = new PFDateBox();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildDateBox(this HtmlHelper htmlHelper, string name, object value = null, Action<PFDateBox> action = null)
        {
            var component = new PFDateBox();
            component.SetPropertyFor(htmlHelper, name);
            if (action != null) { action(component); }
            var r = component.Html(htmlHelper, name, value);
            return r;
        }
        #endregion

        #region MonthRangeInOneYearBox
        public static MvcHtmlString BuildMonthRangeInOneYearBox(this HtmlHelper htmlHelper,
            string yearName, object yearValue,
            string smonName, object smonValue,
            string emonName, object emonValue,
            Action<PFMonthRangeInOneYearBox> action = null)
        {
            var component = new PFMonthRangeInOneYearBox();
            //component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, yearName, yearValue,
             smonName, smonValue,
             emonName, emonValue);
        }

        #endregion MonthRangeInOneYearBox

        #region NumberBox
        public static MvcHtmlString BuildNumberBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFNumberBox> action = null)
        {
            var component = new PFNumberBox();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildNumberBox(this HtmlHelper htmlHelper, string name, object value = null, Action<PFNumberBox> action = null)
        {
            var component = new PFNumberBox();
            component.SetPropertyFor(htmlHelper, name);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }
        #endregion
        #region CheckBox
        public static MvcHtmlString BuildCheckBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFCheckBox> action = null)
        {
            var component = new PFCheckBox();
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildCheckBox(this HtmlHelper htmlHelper, string name, bool? isChecked = false, Action<PFCheckBox> action = null)
        {
            var component = new PFCheckBox();
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, isChecked);
        }
        #endregion
        #region RadioButton
        public static MvcHtmlString BuildRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFRadioButton> action = null)
        {
            var component = new PFRadioButton();
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        /// <summary>
        /// RadioButton(注意,js的change事件里,值都是字符串类型
        /// 用法:
        ///@Html.BuildRadioButton("UserType", 0, a => a.SetLabel("专卖店"))
        ///@Html.BuildRadioButton("UserType", 1, a => { a.SetLabel("顾客"); a.IsChecked(); })
        public static MvcHtmlString BuildRadioButton(this HtmlHelper htmlHelper, string name, object value, Action<PFRadioButton> action = null)
        {
            var component = new PFRadioButton();
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }
        #endregion

        #region Grid
        public static MvcHtmlString BuildGrid(this HtmlHelper htmlHelper, IList data, Action<PFGrid> action = null)
        {
            var component = new PFGrid();
            component.SetModel(data);
            if (action != null) { action(component); }
            return component.Html(htmlHelper);
        }
        public static MvcHtmlString BuildGrid(this HtmlHelper htmlHelper, DataTable data, Action<PFGrid> action = null)
        {
            var component = new PFGrid();
            component.SetModel(data);
            if (action != null) { action(component); }
            return component.Html(htmlHelper);
        }
        public static MvcHtmlString BuildGrid(this HtmlHelper htmlHelper, Action<PFGrid> action = null)
        {
            var component = new PFGrid();
            if (action != null) { action(component); }
            return component.Html(htmlHelper);
        }
        public static MvcHtmlString BuildGrid(this HtmlHelper htmlHelper, PFJsonData<IList> data, Action<PFGrid> action = null)
        {
            if (data.Result)
            {
                return htmlHelper.BuildGrid(data.Data, action);
            }
            else
            {
                return htmlHelper.BuildGrid(new List<Dictionary<string, string>> { new Dictionary<string, string> { { "colError", data.Message } } }, g =>
                {
                    action(g);//就算这里要action,也要和success的action分开
                    g.ClearColumns();
                    g.ColumnsFor("yjquery", cc =>
                    {
                        cc.Add("colError").SetText("提示");
                    });
                });
            }
        }
        public static MvcHtmlString BuildGrid(this HtmlHelper htmlHelper, PFJsonData<DataTable> data, Action<PFGrid> action = null)
        {
            if (data.Result)
            {
                return htmlHelper.BuildGrid(data.Data, action);
            }
            else
            {
                return htmlHelper.BuildGrid(new List<Dictionary<string, string>> { new Dictionary<string, string> { { "colError", data.Message } } }, g =>
                {
                    action(g);//就算这里要action,也要和success的action分开
                    g.ClearColumns();
                    g.ColumnsFor("yjquery", cc =>
                    {
                        cc.Add("colError").SetText("提示");
                    });
                });
            }
        }
        #endregion

        #region Label
        public static MvcHtmlString BuildLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFLabel> action = null)
        {
            var component = new PFLabel();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildLabel(this HtmlHelper htmlHelper, string expression, Action<PFLabel> action = null)
        {
            var component = new PFLabel();
            component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }

        #endregion
        #region FileUpload
        public static MvcHtmlString BuildFileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Action<PFFileUpload> action = null)
        {
            var component = new PFFileUpload();
            //component.SetPropertyFor(expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, expression);
        }
        public static MvcHtmlString BuildFileUpload(this HtmlHelper htmlHelper, string name, string value = null, Action<PFFileUpload> action = null)
        {
            var component = new PFFileUpload();
            //component.SetPropertyFor(htmlHelper, expression);
            if (action != null) { action(component); }
            return component.Html(htmlHelper, name, value);
        }

        #endregion
        #endregion


        #region Expression
        /// <summary>
        /// 取表达式的值(用于代替MVC5.0的ValueExtensions.ValueFor(htmlHelper, expression)(注意该方法返回MVCString且不为空的,与本方法不完全一样)方法)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static object ExpressionValueFor<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            if (htmlHelper.ViewData.Model == null || string.IsNullOrWhiteSpace(propertyName)) { return null; }
            var t = typeof(TModel);
            var p = t.GetProperty(propertyName);
            if (p != null) { return p.GetValue(htmlHelper.ViewData.Model, null); }
            var f = t.GetField(propertyName);
            if (f != null) { return f.GetValue(htmlHelper.ViewData.Model); }
            return null;
            //return typeof(TModel).GetProperty(propertyName).GetValue(htmlHelper.ViewData.Model, null);
        }

        public static SelectList InitSelectList<TModel, TValue, TText>(this IEnumerable<TModel> list, Expression<Func<TModel, TValue>> value, Expression<Func<TModel, TText>> text)
        {
            var valueName = ExpressionHelper.GetExpressionText(value);
            var textName = ExpressionHelper.GetExpressionText(text);
            return new SelectList(list, valueName, textName);
        }
        #endregion Expression
    }

    public class StoreColumnCollection<T> : StoreColumnCollection
    {
        public StoreColumnCollection(string modelConfig) : base(modelConfig) { }
        public void Add<TProperty>(Expression<Func<T, TProperty>> expression, Action<StoreColumn> action = null)//, bool setWidthByHeaderWord = true)
        {

            var propertyName = ExpressionHelper.GetExpressionText(expression);
            Add(propertyName, c =>
            {
                c.dataType = PFDataHelper.GetStringByType(typeof(TProperty));
                if (action != null) { action(c); }
            });
        }
    }

}
