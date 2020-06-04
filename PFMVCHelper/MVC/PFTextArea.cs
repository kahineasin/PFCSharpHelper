using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace Perfect.MVC
{
    public class PFTextArea:PFTextBox
    {
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var config = GetModelPropertyConfig(htmlHelper,expression);
            //SetByModelPropertyConfig(config);
            var htmlAttributes = GetHtmlAttributes();
            return htmlHelper.TextAreaFor(expression, htmlAttributes);
        }
        public override MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            var htmlAttributes = GetHtmlAttributes();
            return htmlHelper.TextArea(name, (value??"").ToString(), htmlAttributes);
        }
    }
}