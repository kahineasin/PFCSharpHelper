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
    /// 组件基类(注意:在此增加代码要慎重)--wxj
    /// </summary>
    public abstract class PFComponent
    {
        private string _id;
        private Dictionary<string, string> _style = new Dictionary<string, string>();
        private Dictionary<string, object> _htmlAttributes = new Dictionary<string, object>();
        private List<string> _classes = new List<string>();
        public PFComponent()
        {
        }
        public void SetId(string id){
            _id = id;
        }
        public string GetId()
        {
            return _id;
        }
        public void SetStyle(string style)
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
            }
        }
        public void AutoWidth()
        {
            SetStyle("width:auto");
        }
        public void SetHtmlAttributes(object htmlAttributes)
        {
            var list = htmlAttributes.GetType().GetProperties().Where(p => p.PropertyType.IsPublic && p.CanRead
            //&& p.CanWrite 
            && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)));
            foreach (var p in list)
            {
                if(_htmlAttributes.ContainsKey(p.Name)){
                    _htmlAttributes.Remove(p.Name);
                }
                _htmlAttributes.Add(p.Name,p.GetValue(htmlAttributes,null));
            }
        }
        public void AddClass(string sClass)
        {
            _classes.Add(sClass);
        }
        public void RemoveClass(string sClass)
        {
            _classes.Remove(sClass);
        }
        public void SetClass(string classes)
        {
            _classes = classes.Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        #region Protected
        protected void SetHtmlAttribute(string key, object value)
        {
            if (_htmlAttributes.ContainsKey(key)) { _htmlAttributes.Remove(key); }
            _htmlAttributes.Add(key, value);
        }
        protected void RemoveHtmlAttribute(string key)
        {
            if (_htmlAttributes.ContainsKey(key)) { _htmlAttributes.Remove(key); }
        }
        protected virtual Dictionary<string, object> GetHtmlAttributes()
        {
            if (_style.Count > 0) { SetHtmlAttribute("style", string.Join(";", _style.Select(a=>a.Key+":"+a.Value))); }
            if (_classes.Count > 0) { SetHtmlAttribute("class", string.Join(" ", _classes)); }
            if (!string.IsNullOrWhiteSpace(_id)) { SetHtmlAttribute("id",_id ); }
            return _htmlAttributes;
        }
        protected PFModelConfig GetModelPropertyConfig<TModel, TProperty>(HtmlHelper htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var modelType = typeof(TModel);
            //var modelConfig = PFDataHelper.GetModelConfig(modelType.Name, modelType.FullName);
            //if (modelConfig == null)
            //{
            //    modelConfig = htmlHelper.ViewData["modelConfig"] as PFModelConfigCollection;
            //}
            var modelConfig = PFDataHelper.MergeModelConfig(
                PFDataHelper.GetModelConfig(modelType.Name, modelType.FullName),
                htmlHelper.ViewData["modelConfig"] as PFModelConfigCollection
                );
            if (modelConfig == null) { return null; }
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var config = modelConfig[propertyName];
            return config;
        }

        protected PFModelConfig GetModelPropertyConfig(HtmlHelper htmlHelper, string expression)
        {
            PFModelConfigCollection modelConfig = htmlHelper.ViewData["modelConfig"] as PFModelConfigCollection;
            if (modelConfig == null) { return null; }
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var config = modelConfig[propertyName];
            return config;
        }
        #endregion
    }
}
