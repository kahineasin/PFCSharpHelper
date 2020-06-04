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
    /// Check组件--wxj
    /// </summary>
    public class PFRadioButton : PFTextBox
    {
        private string _label;
        private bool _isChecked;
        public PFRadioButton()
            : base()
        {
        }
        public void SetLabel(string label)
        {
            _label = label;
        }
        public void IsChecked()
        {
            _isChecked = true;
        }
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var htmlAttributes = GetHtmlAttributes();
            return htmlHelper.RadioButtonFor(expression as Expression<Func<TModel, bool>>, htmlAttributes);//as 未测试能否成功转换
        }
        public override MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            var htmlAttributes = GetHtmlAttributes();
            var rb = htmlHelper.RadioButton(name, value, _isChecked, htmlAttributes);
            var result = string.Format(@"
                            <span class='choose'>
                                {0}
                                <span class='text'>{1}</span>
                            </span>
            ", rb, _label);
            return MvcHtmlString.Create(result);
        }
    }
}
