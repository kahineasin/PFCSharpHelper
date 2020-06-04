
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
    /// Number组件--wxj
    /// </summary>
    public class PFNumberBox:PFTextBox
    {
        private bool _unboundValue=false;
        public PFNumberBox()
            : base()
        {
            SetHtmlAttribute("type", "number");
        }
        public override void SetPropertyFor<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var t = typeof(TProperty);
            if (t == typeof(decimal) || t == typeof(int)) { SetRequired(); }
            base.SetPropertyFor(htmlHelper, expression);
        }
        public override MvcHtmlString Html<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            if (_unboundValue)
            {
                var propertyName = ExpressionHelper.GetExpressionText(expression);
                return this.Html(htmlHelper, propertyName,"");
            }
            return base.Html(htmlHelper, expression);
        }
        /// <summary>
        /// 设置小数位数(如：3是3位小数；-3表示无小数且step每次加1000)
        /// </summary>
        public void SetDecimalDigits(int number) {
            SetHtmlAttribute("step", Math.Pow(0.1,number));
        }
        public void SetMin(int num)
        {
            SetHtmlAttribute("min", num);
        }
        public void SetMax(int num)
        {
            SetHtmlAttribute("max", num);
        }
        /// <summary>
        ///当新增编辑页共用模型时,decimal在新增页中不想显示0的时候调用此方法
        /// </summary>
        public void UnboundValue()
        {
            _unboundValue = true;
        }
        #region Protected
        protected override void SetByModelPropertyConfig(PFModelConfig config)
        {
            //base.SetByModelPropertyConfig(config);

            if (config != null)
            {
                if (config.Required) { SetRequired(); }
                if (!string.IsNullOrWhiteSpace(config.FieldWidth))
                {
                    var width = int.Parse(config.FieldWidth.Replace("px", ""))+10;//number类型控件后有个上下健头,所以要宽一点
                    SetStyle("width:" + width+"px"); 
                }

                if (config.FieldType == typeof(decimal))
                {
                    SetHtmlAttribute("onfocus", "this.oldvalue = this.value; ");
                    if (config.Precision.HasValue)
                    {
                        SetHtmlAttribute("onchange", "if(!$pf.isDecimal(this.value," + config.Precision + ")){this.value=this.oldvalue;}");
                    }
                    else
                    {
                        SetHtmlAttribute("onchange", "if(!$pf.isDecimal(this.value)){this.value=this.oldvalue;}");
                    }
                }
                else if (config.FieldType == typeof(int))
                {
                    SetHtmlAttribute("onfocus", "this.oldvalue = this.value; ");
                    SetHtmlAttribute("onchange", "if(!$pf.isInt(this.value)){this.value=this.oldvalue;}");
                }
            }
        }
        #endregion
    }
}
