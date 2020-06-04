using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Dynamic;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Net;
using System.Web.Caching;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Perfect
{
    /// <summary>
    /// 这些方法由于net版本问题,不能用于这个系统:
    /// 直销员系统,
    /// </summary>
    public partial class PFDataHelper
    {
        public static Expression<Func<T, T1>> FuncToExpression<T, T1>(Func<T, T1> call)
        {
            MethodCallExpression methodCall = call.Target == null
                ? Expression.Call(call.Method, Expression.Parameter(typeof(T)))
                : Expression.Call(Expression.Constant(call.Target), call.Method, Expression.Parameter(typeof(T)));

            return Expression.Lambda<Func<T, T1>>(methodCall);
        }
        #region 反射
        public static bool IsDynamicType(Type type)
        {
            return type.Equals(typeof(ExpandoObject)) || type.Equals(typeof(object));
        }
        public static void EachListHeader(object list, Action<int, string, Type> handle)
        {
            var index = 0;
            var dict = GetListProperties(list);
            foreach (var item in dict)
                handle(index++, item.Key, item.Value);
        }
        public static Dictionary<string, Type> GetListProperties(dynamic list)
        {
            var type = GetGenericType(list);
            var names = new Dictionary<string, Type>();

            if (IsDynamicType(type))
            {
                if (list.Count > 0)
                    foreach (var item in GetIDictionaryValues(list[0]))
                        names.Add(item.Key, (item.Value ?? string.Empty).GetType());
            }
            else
            {
                foreach (var p in GetProperties(type))
                    names.Add(p.Value.Name, p.Value.PropertyType);
            }

            return names;
        }
        public static void EachListRow(object list, Action<int, object> handle)
        {
            var index = 0;
            IEnumerator enumerator = ((dynamic)list).GetEnumerator();
            while (enumerator.MoveNext())
                handle(index++, enumerator.Current);
        }
        public static IDictionary<string, object> GetIDictionaryValues(object item)
        {
            if (item is IDictionary<string, object>)
            {
                return item as IDictionary<string, object>;
            }
            if (IsDynamicType(item.GetType()))
                return item as IDictionary<string, object>;

            var expando = (IDictionary<string, object>)new ExpandoObject();
            var properties = GetProperties(item.GetType());
            foreach (var p in properties)
                expando.Add(p.Value.Name, p.Value.GetValue(item, null));
            return expando;
        }
        public static void EachObjectProperty(object row, Action<int, string, object> handle)
        {
            var index = 0;
            var dict = GetIDictionaryValues(row);
            foreach (var item in dict)
                handle(index++, item.Key, item.Value);
        }
        #endregion 反射
        #region 浏览器
        public static RequestHostInfo GetRequestHostInfo(HttpRequestBase request)
        {
            var result = new RequestHostInfo
            {
                OSVersion = PFDataHelper.GetOSVersion(request),//ok
                Browser = PFDataHelper.GetBrowser(request)
                //,//,ok
                //IPAddress = PFDataHelper.GetIPAddress(request),
                ,//,ok
                IPAddress = request.UserHostAddress,

            };

            ////跨域访问时,这段代码报错:不知道这样的主机
            //string HostName = string.Empty;
            //string ip = string.Empty;
            //string ipv4 = String.Empty;

            //if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_VIA"]))
            //    ip = Convert.ToString(request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            //if (string.IsNullOrEmpty(ip))
            //    ip = request.UserHostAddress;

            //// 利用 Dns.GetHostEntry 方法，由获取的 IPv6 位址反查 DNS 纪录，<br> // 再逐一判断何者为 IPv4 协议，即可转为 IPv4 位址。
            //foreach (IPAddress ipAddr in Dns.GetHostEntry(ip).AddressList)
            //{
            //    if (ipAddr.AddressFamily.ToString() == "InterNetwork")
            //    {
            //        ipv4 = ipAddr.ToString();
            //    }
            //}
            //result.HostName = Dns.GetHostEntry(ip).HostName;

            return result;
            //HostName = "主机名: " + Dns.GetHostEntry(ip).HostName + " IP: " + ipv4;
        } /// <summary> 
          /// 获取操作系统版本号 
          /// </summary> 
          /// <returns></returns>

        public static string GetOSVersion(HttpRequestBase request)
        {
            //UserAgent 
            var userAgent = request.ServerVariables["HTTP_USER_AGENT"];

            var osVersion = "未知";
            if (userAgent.Contains("NT 10.0"))
            {
                osVersion = "Windows 10";
            }
            else if (userAgent.Contains("NT 6.3"))
            {
                osVersion = "Windows 8.1";
            }
            else if (userAgent.Contains("NT 6.2"))
            {
                osVersion = "Windows 8";
            }

            else if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                osVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                osVersion = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osVersion = "SunOS";
            }
            return osVersion;
        }
        ///// <summary> 
        ///// 获取IP地址
        ///// </summary> 
        ///// <returns></returns>

        public static string GetIPAddress(HttpRequestBase request)
        {
            string ipv4 = String.Empty;
            foreach (IPAddress IPA in Dns.GetHostAddresses(request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = IPA.ToString();
                    break;
                }
            }
            if (ipv4 != String.Empty)
            {
                return ipv4;
            }
            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = IPA.ToString();
                    break;
                }
            }
            return ipv4;
        }
        /// <summary> 
        /// 获取浏览器版本号 
        /// </summary> 
        /// <returns></returns> 
        public static string GetBrowser(HttpRequestBase request)
        {
            HttpBrowserCapabilitiesBase bc = request.Browser;
            return bc.Browser + bc.Version;
        }
        #endregion 浏览器
    }

    /// <summary>
    /// 使用时报错
    /// 复制对象
    /// 使用方法：StudentSecond ss= TransExpV2<Student, StudentSecond>.Trans(s);
    /// 由于会缓存，所以效率比反射高
    /// https://www.cnblogs.com/lsgsanxiao/p/8205096.html
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public static class TransExpV2<TIn, TOut>
    {

        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }

    }
}
