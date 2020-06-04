using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Web.Routing;

namespace Perfect.MVC
{
    public class PFProvinceCityBox: PFComponent
    {
        private string _provinceField = "sf";
        private string _cityField = "city";
        public string ProvinceField { get { return _provinceField; } set { _provinceField = value; } }
        public string CityField { get { return _cityField; } set { _cityField = value; } }
        public object ProvinceValue { get; set; }
        public object CityValue { get; set; }
        public object ProvinceList { get; set; }
        public object CityList { get; set; }
        public virtual MvcHtmlString Html(HtmlHelper htmlHelper)
        {
            //var htmlAttributes = GetHtmlAttributes();

            //return System.Web.Mvc.Html.ChildActionExtensions.Action(htmlHelper, "ProvinceCityBox", "Home",
            //    new RouteValueDictionary
            //    {
            //        { "area",null },
            //        { "sf", (ProvinceValue ?? "").ToString() },
            //        { "city",(CityValue ?? "").ToString() },
            //        {"sfField", ProvinceField },
            //        {"cityField", CityField }
            //    });

            //return System.Web.Mvc.Html.ChildActionExtensions.Action(htmlHelper, "ProvinceCityBox", "Home",
            //    new RouteValueDictionary
            //    { area=null,sf=(ProvinceValue??"").ToString(),city=(CityValue??"").ToString(),
            //        sfField =ProvinceField,cityField=CityField});

            return System.Web.Mvc.Html.PartialExtensions.Partial(htmlHelper,
                "~/Views/Shared/Control/sfCity.cshtml",
                this);
        }
    }
}