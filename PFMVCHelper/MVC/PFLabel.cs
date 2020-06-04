using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Perfect.MVC
{
    public class PFLabel : PFComponent
    {
        private bool _required;
        protected PFModelConfig _modelConfig;
        public void SetRequired(bool required=true)
        {
            _required = required;
        }
        public virtual void SetPropertyFor<TModel, TProperty>(HtmlHelper htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            _modelConfig = GetModelPropertyConfig(htmlHelper,expression);
            if (_modelConfig != null)
            {
                _required = _modelConfig.Required;
            }
        }
        public virtual void SetPropertyFor(HtmlHelper htmlHelper, string expression)
        {
            _modelConfig = GetModelPropertyConfig(htmlHelper, expression);
            if (_modelConfig != null)
            {
                _required = _modelConfig.Required;
            }
        }
        public virtual MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var config = _modelConfig;
            if (config != null)
            {
                var result = htmlHelper.LabelFor(expression, config.FieldText);
                var h = GetHtmlByModelConfig(result.ToHtmlString(), config);
                var htmlAttributes = GetHtmlAttributes();
                h = AppendAttr(h, htmlAttributes);
                return MvcHtmlString.Create(h);
            }
            return htmlHelper.LabelFor(expression);
        }
        public virtual MvcHtmlString Html(HtmlHelper htmlHelper, string expression)
        {
            var config = _modelConfig;
            var label = expression;
            if (config != null) { label = config.FieldText; }
            var h = htmlHelper.Label(label).ToHtmlString();
            h = GetHtmlByModelConfig(h, config);
            var htmlAttributes = GetHtmlAttributes();
            h = AppendAttr(h, htmlAttributes);
            return MvcHtmlString.Create(h);         
        }

        #region Private
        private string AppendToLast(string html,  string s)//追加内容
        {
            return Regex.Replace(html, "(<.*>)(.*)(<\\/.*>)", "$1$2" + s + "$3");
        }
        private string AppendAttr(string html, Dictionary<string, object> attributes)//追加内容
        {
            var s = "";
            foreach(var attr in attributes)
            {
                s += " " + attr.Key + "=" + attr.Value;
            }
            return Regex.Replace(html, "^(<)([^>]+)(>.*)", "$1$2" + s + "$3");
        }
        private string GetHtmlByModelConfig(string html, PFModelConfig config)
        {
            if (config != null)
            {
                var ex = "";
                if (_required) { ex += "<span style='color:red'>*</span>"; }
                ex += "：";
                return AppendToLast(html, ex);
            }
            return html;
        }
        #endregion
    }
}
