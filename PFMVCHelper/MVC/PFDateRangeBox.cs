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
    /// 月份范围选择字段,格式如 xx年 x-x月,目的是为了限制只能选择一年之内
    /// 一般用这个组件时,月份范围都是必需的,所以默认设置为required
    /// </summary>
    public class PFDateRangeBox : PFDateBox
    {
        //private PFNumberBox _yearBox;
        //private PFDateBox _smonBox;
        private PFDateBox _emonBox;
        public PFDateRangeBox()
        {
            //_yearBox = new PFNumberBox();
            //_yearBox.SetStyle("width:50px;padding-right:0px");
            //_yearBox.SetRequired();
            //_yearBox.SetHtmlAttributes(new { isYear = true });//自定义jquery validate

            var _smonBox = this;
            //_smonBox = new PFDateBox();
            //_smonBox.SetFormat(PFDataHelper.MonthFormat);
            _smonBox.SetStyle("padding-right:0px");
            //_smonBox.SetRequired();
            //_smonBox.SetHtmlAttributes(new { isMonth = true });//自定义jquery validate

            _emonBox = new PFDateBox();
            //_emonBox.SetFormat(PFDataHelper.MonthFormat);
            _emonBox.SetStyle("padding-right:0px");
            //_emonBox.SetRequired();
            //_emonBox.SetHtmlAttributes(new { isMonth = true });//自定义jquery validate
        }

        public MvcHtmlString Html(HtmlHelper htmlHelper,
            string smonName,object smonValue,
            string emonName, object emonValue
            )
        {
            _emonBox.SetHtmlAttributes(new { isLargerThen = "#"+ smonName });
            
            var result = string.Format("{0}-{1}",
                base.Html(htmlHelper, smonName, smonValue),
                _emonBox.Html(htmlHelper, emonName, emonValue)
                );
            return MvcHtmlString.Create(result);
        }

        public override void SetFormat(string format)
        {
            base.SetFormat(format);
            _emonBox.SetFormat(format);
        }

        //public PFMonthRangeInOneYearBox SetYearBox(Action<PFNumberBox> yearBoxAction) {
        //    yearBoxAction(_yearBox);
        //    return this;
        //}
        //public PFMonthRangeInOneYearBox SetSMonBox(Action<PFNumberBox> smonBoxAction)
        //{
        //    smonBoxAction(_smonBox);
        //    return this;
        //}
        //public PFMonthRangeInOneYearBox SetEMonBox(Action<PFNumberBox> emonBoxAction)
        //{
        //    emonBoxAction(_emonBox);
        //    return this;
        //}

        ///// <summary>
        ///// 便于controller中把前端的值提交过来
        ///// </summary>
        ///// <param name="year">2014</param>
        ///// <param name="smon">1</param>
        ///// <param name="emon">2</param>
        //public static void GetCMonthRange(string year,string smon,string emon,out string sCMonth,out string eCMonth)
        //{
        //    int yearInt = PFDataHelper.ObjectToInt(year) ?? 0;
        //    sCMonth = PFDataHelper.DateToCMonth(new DateTime(yearInt, PFDataHelper.ObjectToInt(smon) ?? 0, 1));
        //    eCMonth = PFDataHelper.DateToCMonth(new DateTime(yearInt, PFDataHelper.ObjectToInt(emon) ?? 0, 1));
        //}
    }
}
