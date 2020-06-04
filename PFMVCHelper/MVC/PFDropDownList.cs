
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace Perfect.MVC
{
    /// <summary>
    /// 下拉组件--wxj
    /// </summary>
    public class PFDropDownList : PFTextBox
    {
        private SelectList _selectList;
        private string _optionLabel;
        private string _selectedValue { get; set; }
        private string _selectedText { get; set; }
        public PFDropDownList()
            : base()
        {
        }
        public override void SetPropertyFor<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            base.SetPropertyFor(htmlHelper, expression);
            //var t = typeof(TProperty);
            var t = PFDataHelper.GetNonnullType(typeof(TProperty));
            var v = PFMVCHelper.ExpressionValueFor(htmlHelper, expression);
            if (v != null)
            {
                if (t.IsEnum)
                {
                    _selectedValue = ((int)v).ToString();
                }
                else
                {
                    _selectedValue = v.ToString();
                }
            }
            if (t.IsEnum)
            {
                SetSelectList(PFDataHelper.EnumToKVList(t));
            }

            //_selectedValue = (PFMVCHelper.ExpressionValueFor(htmlHelper,expression)??"").ToString();
            //_selectedText = (PFMVCHelper.ExpressionValueFor(htmlHelper, expression) ?? "").ToString();//这里用text,是因为即便是enum,ToString后肯定是需要显示的值
        }
        public void SetSelectList(SelectList selectList)
        {
            _selectList = selectList;
        }
        public void SetSelectList<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> list)
        {
            var selectList = new SelectList(list, "Key", "Value");
            _selectList = selectList;
        }
        public void SetOptionLabel(string optionLabel)
        {
            _optionLabel = optionLabel;
        }
        public virtual void SetSelectedValue(string selectedValue)
        {
            _selectedValue = selectedValue;
        }
        public virtual void SetSelectedText(string selectedText)
        {
            _selectedText = selectedText;
        }
        public void SetDisabled()
        {
            SetHtmlAttribute("disabled", "disabled");
        }
        /// <summary>
        /// 自扩展的可编辑下拉
        /// </summary>
        public void SetEditable()
        {
            SetHtmlAttribute("editable", "editable");
        }
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var selectedList = GetSelectedList();
            if (Readonly)//这种ReadOnly不好,暂使用前端的$pf.disableField()
            {
                //return base.Html(htmlHelper,expression);//只读时如果直接返回textbox,就不能实现text映射value的方式了
                var name = ExpressionHelper.GetExpressionText(expression);
                var selectedItem = selectedList == null ? null : selectedList.FirstOrDefault(a => a.Selected);
                var text = selectedItem == null ? "" : selectedItem.Text;
                var value = selectedItem == null ? "" : selectedItem.Value;
                string s = htmlHelper.Hidden(name, value).ToString() + base.Html(htmlHelper, name + "_txt", text).ToString();
                return MvcHtmlString.Create(s);

                ////return base.Html(htmlHelper,expression);//只读时如果直接返回textbox,就不能实现text映射value的方式了
                //var name=ExpressionHelper.GetExpressionText(expression);
                //var text= (PFMVCHelper.ExpressionValueFor(htmlHelper, expression) ?? "").ToString();//这里用text,是因为即便是enum,ToString后肯定是需要显示的值
                ////var text = selectedList.Where(a => a.Selected).Select(a => a.Text).FirstOrDefault();
                ////var value= selectedList.Where(a => a.Selected).Select(a => a.Value).FirstOrDefault();
                //var value = _selectList==null?"": _selectList.Where(a => a.Text==text).Select(a => a.Value).FirstOrDefault();
                //string s = htmlHelper.Hidden(name, value).ToString()+base.Html(htmlHelper,name+"_txt", text).ToString();
                //return MvcHtmlString.Create(s);
            }
            else
            {
                //var config = GetModelPropertyConfig(htmlHelper, expression);
                //SetByModelPropertyConfig(config);
                var htmlAttributes = GetHtmlAttributes();
                if (selectedList == null)//为空时用DropDownListFor会报错--benjamin20190927
                {
                    return htmlHelper.DropDownListFor(expression, new List<SelectListItem>(), _optionLabel, htmlAttributes);
                }
                return htmlHelper.DropDownListFor(expression, selectedList, _optionLabel, htmlAttributes);
            }
        }
        public override MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            if (Readonly) { return base.Html(htmlHelper, name, value); }
            var htmlAttributes = GetHtmlAttributes();
            var sAttr = "";
            foreach (var attr in htmlAttributes)
            {
                sAttr += string.Format(" {0}='{1}'", attr.Key, attr.Value);
            }
            var result = string.Format("<select id='{0}' name='{1}' {2} >", GetId() ?? name, name, sAttr);
            var selectedList = GetSelectedList();
            if (selectedList != null)
            {
                foreach (var selectItem in selectedList)
                {
                    result += string.Format("<option value='{0}' {2} >{1}</option>", selectItem.Value, selectItem.Text, selectItem.Selected ? "selected='selected'" : "");
                }
            }
            result += "</select>";
            return MvcHtmlString.Create(result);
            //return htmlHelper.DropDownList(name, GetSelectedList(), (string)value, htmlAttributes);//有bug,无论如果设置不了已选值
        }
        protected SelectList GetSelectedList()
        {
            if ((_selectedValue != null || _selectedText != null) && _selectList != null)
            {
                if (_selectedValue != null)
                {
                    return NewSelectList(_selectedValue);
                }
                else if (_selectedText != null)
                {
                    foreach (var i in _selectList)
                    {
                        if (i.Text == _selectedText)
                        {
                            return NewSelectList(i.Value);
                        }
                    }
                }

            }
            return _selectList;
        }
        private SelectList NewSelectList(string selectValue)
        {
            //return new SelectList(_selectList, "Value", "Text", i.Value);//这样的SelectList多了一个层级
            var tList = _selectList.Select(a => new KeyValuePair<object, object>(a.Value, a.Text)).ToList();
            return new SelectList(tList, "Key", "Value", selectValue);
        }

        #region Protected
        protected override void SetByModelPropertyConfig(PFModelConfig config)
        {
            base.SetByModelPropertyConfig(config);

            if (config != null)
            {
                if (!string.IsNullOrWhiteSpace(config.FieldWidth))
                {
                    var width = int.Parse(config.FieldWidth.Replace("px", "")) + 20;//下拉控件后有个下健头,所以要宽一点
                    SetStyle("width:" + width + "px");
                }
            }
        }
        #endregion
    }
}
