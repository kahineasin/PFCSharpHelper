using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace Perfect.MVC
{
    public class PFCheckBoxList : PFDropDownList
    {
        private MultiSelectList _selectList;
        private string[] _selectedValue { get; set; }
        private string[] _selectedText { get; set; }
        protected virtual string InputType { get { return "checkbox"; } }
        public PFCheckBoxList() : base()
        {
        }
        public void SetSelectList(MultiSelectList selectList)
        {
            _selectList = selectList;
        }
        public new void SetSelectList<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> list)
        {
            var selectList = new MultiSelectList(list, "Key", "Value");
            _selectList = selectList;
        }
        public void SetSelectedValue(string[] selectedValue)
        {
            _selectedValue = selectedValue;
        }
        public void SetSelectedText(string[] selectedText)
        {
            _selectedText = selectedText;
        }
        public override void SetSelectedValue(string selectedValue)
        {
            _selectedValue = new string[] { selectedValue };
        }
        public override void SetSelectedText(string selectedText)
        {
            _selectedText = new string[] { selectedText };
        }
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            return Html(htmlHelper, name, null);
        }
        public override MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            var htmlAttributes = GetHtmlAttributes();
            var sAttr = "";
            foreach (var attr in htmlAttributes)
            {
                sAttr += string.Format(" {0}='{1}'", attr.Key, attr.Value);
            }
            //var result = string.Format("<select id='{0}' name='{1}' {2} >", GetId() ?? name, name, sAttr);
            var result = "";
            var selectedList = GetSelectedList();
            if (selectedList != null)
            {
                foreach (var selectItem in selectedList)
                {
                    //var cb = htmlHelper.CheckBox(name, selectItem.Selected, htmlAttributes);
                    //var cb = htmlHelper.CheckBox(name, selectItem.Selected, new { Value=selectItem.Value});//用原生方法生成的input后面多了个hidden，原因不明（造成提交时有很多value为false的值）
                    var cb = string.Format(@"<input type='{3}' name='{0}' {1} value='{2}' {4}>", name, selectItem.Selected ? "checked='checked'" : "", selectItem.Value, InputType, sAttr);
                    result += string.Format(@"
                            <span class='choose'>
                                {0}
                                <span class='text'>{1}</span>
                            </span>
            ", cb, selectItem.Text);
                    //result += string.Format("<option value='{0}' {2} >{1}</option>", selectItem.Value, selectItem.Text, selectItem.Selected ? "selected='selected'" : "");
                }
            }
            //result += "</select>";
            return MvcHtmlString.Create(result);
            //return htmlHelper.DropDownList(name, GetSelectedList(), (string)value, htmlAttributes);//有bug,无论如果设置不了已选值
        }

        protected new MultiSelectList GetSelectedList()
        {
            if ((_selectedValue != null || _selectedText != null) && _selectList != null)
            {
                if (_selectedValue != null)
                {
                    return NewSelectList(_selectedValue);
                }
                else if (_selectedText != null)
                {
                    List<string> values = new List<string>();
                    foreach (var i in _selectList)
                    {
                        //if (i.Text == _selectedText)
                        if (_selectedText.Contains(i.Text))
                        {
                            values.Add(i.Value);
                            //return NewSelectList(i.Value);
                        }
                    }
                    return NewSelectList(values.ToArray());
                }

            }
            return _selectList;
        }
        private MultiSelectList NewSelectList(string[] selectValue)
        {
            //return new SelectList(_selectList, "Value", "Text", i.Value);//这样的SelectList多了一个层级
            var tList = _selectList.Select(a => new KeyValuePair<object, object>(a.Value, a.Text)).ToList();
            return new MultiSelectList(tList, "Key", "Value", selectValue);
        }
    }
}
