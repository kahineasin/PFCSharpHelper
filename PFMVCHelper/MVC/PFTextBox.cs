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
    /// Text组件--wxj
    /// </summary>
    public class PFTextBox:PFComponent
    {
        private bool _required;
        private bool _readonly;
        protected PFModelConfig _modelConfig;

        protected bool Required { get { return _required; } }
        protected bool Readonly { get { return _readonly; } }
        public PFTextBox() :base()
        {
            AddClass("txt_333_12");
        }
        public virtual void SetPropertyFor<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            _modelConfig = GetModelPropertyConfig(htmlHelper,expression);
            if (_modelConfig != null)
            {
                SetByModelPropertyConfig(_modelConfig);
            }
        }
        public virtual void SetPropertyFor(HtmlHelper htmlHelper, string expression)
        {
            _modelConfig = GetModelPropertyConfig(htmlHelper, expression);
            if (_modelConfig != null)
            {
                SetByModelPropertyConfig(_modelConfig);
            }
        }
        public virtual void SetReadonly(bool ro=true)
        {
            _readonly = ro;
            if (ro) { SetHtmlAttribute("readonly", "readonly"); }
            else { RemoveHtmlAttribute("readonly"); }
        }
        public void SetRequired(bool required = true)
        {
            _required = required;
            if (required) { SetHtmlAttribute("required", "required"); }
            else { RemoveHtmlAttribute("required"); }
        }
        public void SetMinLength(int length)
        {
            SetHtmlAttribute("minlength", length);
        }
        public void SetMaxLength(int length)
        {
            SetHtmlAttribute("maxlength", length);
        }
        public void SetPlaceholder(string placeholder)
        {
            SetHtmlAttribute("placeholder", placeholder);
        }

        public virtual MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            //var config = GetModelPropertyConfig(expression);
            //SetByModelPropertyConfig(config);
            var htmlAttributes = GetHtmlAttributes();
            return htmlHelper.TextBoxFor(expression, htmlAttributes);
        }
        public virtual MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            var htmlAttributes = GetHtmlAttributes();
            return htmlHelper.TextBox(name, value, htmlAttributes);
        }

        #region Protected
        protected virtual void SetByModelPropertyConfig(PFModelConfig config)
        {
            if (config != null)
            {
                if (config.Required) { SetRequired(); }
                //if (!string.IsNullOrWhiteSpace(config.FieldWidth)) { SetStyle("width:"+ config.FieldWidth+"pt"); }
                if (!string.IsNullOrWhiteSpace(config.FieldWidth)) { SetStyle("width:" + config.FieldWidth); }
            }
        } 
        #endregion
    }
}
