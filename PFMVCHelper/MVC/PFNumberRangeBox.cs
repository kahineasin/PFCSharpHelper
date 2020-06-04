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
    /// 数字范围字段
    /// </summary>
    public class PFNumberRangeBox : PFComponent
    {
        private PFNumberBox _smonBox;
        private PFNumberBox _emonBox;
        public PFNumberRangeBox()
        {
            _smonBox = new PFNumberBox();
            //_smonBox.SetStyle("width:30px;padding-right:0px");
            _smonBox.SetStyle("padding-right:0px");
            //_smonBox.SetRequired();

            _emonBox = new PFNumberBox();
            //_emonBox.SetStyle("width:30px;padding-right:0px");
            _emonBox.SetStyle("padding-right:0px");
            //_emonBox.SetRequired();
        }

        public MvcHtmlString Html(HtmlHelper htmlHelper,
            string smonName, object smonValue,
            string emonName, object emonValue
            )
        {
            _emonBox.SetHtmlAttributes(new { isLargerThen = "#" + smonName });

            var result = string.Format("{0}-{1}",
                _smonBox.Html(htmlHelper, smonName, smonValue),
                _emonBox.Html(htmlHelper, emonName, emonValue)
                );
            return MvcHtmlString.Create(result);
        }

        public PFNumberRangeBox SetSNumBox(Action<PFNumberBox> smonBoxAction)
        {
            smonBoxAction(_smonBox);
            return this;
        }
        public PFNumberRangeBox SetENumBox(Action<PFNumberBox> emonBoxAction)
        {
            emonBoxAction(_emonBox);
            return this;
        }

    }
}
