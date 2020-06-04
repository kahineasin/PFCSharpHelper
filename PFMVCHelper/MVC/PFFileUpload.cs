using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace Perfect.MVC
{
    public class PFFileUpload: PFTextBox
    {
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name= ExpressionHelper.GetExpressionText(expression);
            //var value = ValueExtensions.ValueFor(htmlHelper,expression);
            var value = htmlHelper.DisplayForModel(expression);
            return Html(htmlHelper, name, value);
        }
        public override MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            AddClass("file pf-fileupload");
            var htmlAttributes = GetHtmlAttributes();
            var sAttr = "";
            foreach (var attr in htmlAttributes)
            {
                sAttr += string.Format(" {0}='{1}'", attr.Key, attr.Value);
            }
            var sRequired = Required ? " required='required' ":"";
            var result = string.Format("<div {1} ><span>{2}</span><input id='{0}' type='file' name='{0}' {3} /><input type='button' value='上传' /></div>", name, sAttr,value,sRequired);
            return MvcHtmlString.Create(result);
        }
    }
}