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
    /// 日期组件--wxj
    /// </summary>
    public class PFDateBox : PFTextBox
    {
        private string _format = "yyyy-MM-dd";
        private DateTime? _minDate = null;
        private DateTime? _maxDate = null;
        public PFDateBox() : base()
        {
            RemoveClass("txt_333_12");
            AddClass("Wdate");
            SetHtmlAttribute("autocomplete", "off");//浏览器默认的下拉显示历史记录框,会挡住Wdate的控件窗--benjamin20200303
        }
        public override void SetReadonly(bool ro = true)
        {
            base.SetReadonly(ro);
            if (ro) { RemoveClass("Wdate"); }
        }
        public virtual void SetFormat(string format)
        {
            _format = format;
        }
        public void SetMinDate(DateTime date)
        {
            _minDate = date;
        }
        public void SetMaxDate(DateTime date)
        {
            _maxDate = date;
        }
        protected override Dictionary<string, object> GetHtmlAttributes()
        {
            var attrs = base.GetHtmlAttributes();
            if (!Readonly)
            {
                var opts = new List<string>();
                if (!string.IsNullOrWhiteSpace(_format)) { opts.Add("dateFmt:'" + _format + "'"); }
                if (_minDate.HasValue) { opts.Add("minDate:'" + _minDate.Value.ToString(_format) + "'"); }
                if (_maxDate.HasValue) { opts.Add("maxDate:'" + _maxDate.Value.ToString(_format) + "'"); }
                //if (Readonly) { opts.Add("readOnly:true,isShowClear:false,$wdate:false"); }
                attrs.Add("onclick", string.Format("WdatePicker({{{0}}})", string.Join(",", opts)));
            }
            return attrs;
        }
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            //var value = ValueExtensions.ValueFor(htmlHelper, expression);
            var value = PFMVCHelper.ExpressionValueFor(htmlHelper, expression);
            return this.Html(htmlHelper, propertyName, value);
        }
        public override MvcHtmlString Html(HtmlHelper htmlHelper, string name, object value)
        {
            var htmlAttributes = GetHtmlAttributes();
            var result = "";
            if (!Readonly)
            {
                if (htmlHelper.ViewData["hasDataBoxJs"] == null)
                {
                    var configMapper = PFDataHelper.GetConfigMapper();
                    var pathConfig = configMapper.GetPathConfig();
                    result = string.Format("<script language='javascript' type='text/javascript' src='/{0}'></script>", pathConfig.DataBoxJsPath);
                    htmlHelper.ViewData["hasDataBoxJs"] = true;
                }
            }
            return new MvcHtmlString(result + htmlHelper.TextBox(name, PFDataHelper.ObjectToDateString(value, _format), htmlAttributes).ToString());
        }
    }
}
