using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Perfect
{
    public interface IDataGetter
    {
        //object GetData(HttpContext context);
        object GetData(IController controller, HttpContext context);
    }
}
