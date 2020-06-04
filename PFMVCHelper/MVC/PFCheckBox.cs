using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Reflection;

namespace Perfect.MVC
{
    /// <summary>
    /// Check组件--wxj
    /// </summary>
    public class PFCheckBox : PFTextBox
    {
        public PFCheckBox()
            : base()
        {
        }
        public override void SetReadonly(bool ro = true)
        {
            base.SetReadonly(ro);
            if (ro) { SetHtmlAttribute("onclick", "return false"); }
        }
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var htmlAttributes = GetHtmlAttributes();
            var exp = expression as Expression<Func<TModel, bool>>;
            if (exp != null) {
                return htmlHelper.CheckBoxFor(expression as Expression<Func<TModel, bool>>, htmlAttributes);
            }else//上面For方法只支持非空bool型
            {
                var n= ExpressionHelper.GetExpressionText(expression);
                var v=PFMVCHelper.ExpressionValueFor(htmlHelper, expression);
                if (v == null) { v = false; }
                return htmlHelper.CheckBox(n, (bool)v,htmlAttributes);
            }
        }
        public override MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            var htmlAttributes = GetHtmlAttributes();
            bool b = false;
            if (value != null) { b = (bool)value; }
            return htmlHelper.CheckBox(name, b, htmlAttributes);
        }
    }
}
