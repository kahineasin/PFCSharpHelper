using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace Perfect
{
    public class ApiData :
        IDataGetter
    {

        public object GetData(IController controller,HttpContext context)//控制器一定要传过来,不要用反射获得,否则Session等成员无法处理
        {
            dynamic data = null;
            var url = context.Request.Form["dataAction"];
            JObject param = JsonConvert.DeserializeObject<dynamic>(context.Request.Form["dataParams"]);

            //var route = url.Replace("/api/", "").Split('/'); // route[0]=mms,route[1]=send,route[2]=get
            var route = url.Split('/');
            route = route.Skip(1).ToArray();

            var action = route.Length > 2 ? route[2] : "Get";

            if (action.IndexOf('?') > -1)
            {
                NameValueCollection urlParams = PFDataHelper.GetQueryStringParams(action);
                action = action.Split('?')[0];
                foreach (var i in urlParams.AllKeys)
                {
                    param[i] = urlParams[i];
                }
            }

            var methodInfo = controller.GetType().GetMethod(action);

            var parameters = new object[] { new PagingParameters().SetRequestData(param) };

            data = methodInfo.Invoke(controller, parameters);

            if (data.GetType() == typeof(ExpandoObject))
            {
                if ((data as ExpandoObject).Where(x => x.Key == "rows").Count() > 0)
                    data = data.rows;
            }

            if (data.Data is PagingResult) { return data.Data; }
            return data;
        }
      
    }
}
