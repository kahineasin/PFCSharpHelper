using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Net.Sockets;
//using Aspose.Cells;
//using System.Threading.Tasks;

namespace Perfect
{
    /// <summary>
    /// 数据帮助类(此类希望在.Net中共用,不要引用MVC)(Framework3.5以下兼容)
    /// </summary>
    public static partial class PFDataHelper
    {
        public static IPFConfigMapper _configMapper = null;
        public static string BaseDirectory
        {
            get
            {
                //return Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", ""));
                return PathCombine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", ""));
            }
        }
        /// <summary>
        /// 临时文件目录(作用完立刻删除)
        /// </summary>
        public static string TempFileDirectory
        {
            get
            {
                return PathCombine(BaseDirectory, "TempFile");
            }
        }

        /// <summary>
        /// 读取Web.config文件里的compilation节点的debug属性(发布后的项目里,这个属性应该设置为false)
        /// </summary>
        public static bool IsCompilationDebug
        {
            get
            {
                System.Web.Configuration.CompilationSection configSection = (System.Web.Configuration.CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
                return configSection.Debug;
            }
        }
        public static bool IsDebug
        {
            get
            {
                return ConfigurationManager.AppSettings["PFDebug"] != null && bool.Parse(ConfigurationManager.AppSettings["PFDebug"]);
            }
        }
        /// <summary>
        /// 测试时可以使用之前保存的本地数据
        /// </summary>
        public static bool UseLocalData
        {
            get
            {
                return ConfigurationManager.AppSettings["PFUseLocalData"] != null && bool.Parse(ConfigurationManager.AppSettings["PFUseLocalData"]);
            }
        }
        public static bool AllowGCCollect
        {
            get
            {
                return ConfigurationManager.AppSettings["PFAllowGCCollect"] != null && bool.Parse(ConfigurationManager.AppSettings["PFAllowGCCollect"]);
            }
        }

        /// <summary>
        /// 允许自动登陆(免登陆,分公司跨平台)
        /// </summary>
        public static bool AllowAutoLogin
        {
            get
            {
                return ConfigurationManager.AppSettings["AllowAutoLogin"] != null && bool.Parse(ConfigurationManager.AppSettings["AllowAutoLogin"]);
            }
        }
        /// <summary>
        /// 如果收发邮件不设置超时，发现PFListenEmailTask会因为超时而卡在Connect_Click后面
        /// 默认20,000毫秒
        /// </summary>
        public static int EmailTimeout
        {
            get
            {
                return ObjectToInt(ConfigurationManager.AppSettings["EmailTimeout"]) ?? 20000;
            }
        }

        /// <summary>
        /// 系统邮箱的信息(用于监听邮件和发送系统邮件)
        /// </summary>
        public static string SysEmailHostName = "smtp.perfect99.com";
        public static string SysEmailUserName = "wxj@perfect99.com";
        public static string SysEmailPwd = "shi3KjkE48QZ3SPA";

        /// <summary>
        /// 开发者工号,便于不写本人访问日志,检查权限,等等
        /// </summary>
        public static string DeveloperNumber = "1712002";

        /// <summary>
        /// 生成唯一键,便于异步任务监听结果(如监听邮件发送结果)
        /// </summary>
        public static string NewUniqueHashId { get { return Guid.NewGuid() + DateTime.Now.ToString("yyyyMMddHHmmss"); } }

        public static Encoding _encoding = Encoding.UTF8;
        /// <summary>
        /// 日期格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string DateFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 月份格式
        /// </summary>
        public static string MonthFormat = "yyyy.MM";
        public static string DayFormat = "yyyy-MM-dd";
        public static string YMFormat = "yyyyMM";

        /// <summary>
        /// 系统最大月份(对于数据庞大的系统,有起始数据月份是合理的)
        /// </summary>
        public static PFCmonth SysMaxMonth;
        public static PFCmonth SysMinMonth;

        /// <summary>
        /// 小数精确度默认4位(常用于除法的小数保留位数)
        /// </summary>
        public static int DecimalPrecision = 4;

        /// <summary>
        /// 系统允许的最小时间(在sql的允许范围内),一般为了使Sql插入时不报错,可以把小于这个值的设成此值
        /// </summary>
        public static DateTime MinTime { get { return (new List<DateTime> { DateTime.MinValue, System.Data.SqlTypes.SqlDateTime.MinValue.Value }).Max(); } }
        #region Data

        #region 随机数
        //public static List<T> ListRandomTake<T>(ref Random Rnd,List<T> list, int num)
        //{
        //    if (list == null || list.Count <= num) { return list; }
        //    var result = new List<T>();
        //    //Random Rnd = new Random();
        //    int lc = list.Count;
        //    List<int> exist = new List<int>();
        //    while (result.Count < num)
        //    {
        //        int i = Rnd.Next(0, lc);
        //        if (!exist.Contains(i))
        //        {
        //            exist.Add(i);
        //            result.Add(list[i]);
        //        }
        //    }
        //    //for (int i = 0; i < num; i++) {
        //    //    result.Add(ListRandomTakeOne(list));
        //    //}
        //    return result;
        //}
        public static List<T> ListRandomTake<T>(ref Random Rnd, List<T> list, int num, bool removeFromSrcList = false)
        {
            var result = new List<T>();

            //if (list == null || list.Count <= num) { return list; }
            if (list == null) { return list; }
            if (list.Count <= num)
            {
                if (removeFromSrcList)
                {
                    result = new List<T>(list); ;
                    list.Clear();
                    return result;
                }
                else
                {
                    return list;
                }
            }

            //Random Rnd = new Random();
            int lc = list.Count;
            List<int> exist = new List<int>();
            List<T> tmpList = null;
            if (removeFromSrcList)
            {
                tmpList = list;
            }
            else
            {
                tmpList = new List<T>(list);
            }
            while (result.Count < num)
            {
                int i = Rnd.Next(0, tmpList.Count);
                result.Add(tmpList[i]);
                tmpList.RemoveAt(i);
                //if (!exist.Contains(i))
                //{
                //    exist.Add(i);
                //    result.Add(list[i]);
                //}
            }
            //for (int i = 0; i < num; i++) {
            //    result.Add(ListRandomTakeOne(list));
            //}
            return result;
        }
        //private static T ListRandomTakeOne<T>(List<T> list)
        //{
        //    Random Rnd = new Random();
        //    int i=Rnd.Next(0, list.Count);
        //    return list[i];
        //} 
        #endregion

        #region Date
        /// <summary>
        /// obj转为日期格式的字符串,如果转换不成功返回""
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ObjectToDateString(object obj, string format = "yyyy-MM-dd")
        {
            if (obj == null || obj == DBNull.Value) { return ""; }
            if (obj is DateTime && ((DateTime)obj).Date.CompareTo(DateTime.MaxValue.Date) == 0) { return ""; }

            if (obj is DateTime)
            {
                return ((DateTime)obj).ToString(format);
            }
            else if (obj is string)
            {
                return (string)obj;
            }
            else if (obj is int)//出生年份之类,数据库中有可能保存的是int格式如1988
            {
                DateTime s;

                if (DateTime.TryParseExact(obj.ToString(), "yyyy", null, System.Globalization.DateTimeStyles.None, out s))
                {
                    return s.ToString(format);
                }
            }
            return "";
        }
        /// <summary>
        /// 转为大写日期,如二〇一八年一月五日
        /// </summary>
        /// <returns></returns>
        public static string CMonthToCapitalDate(DateTime date)
        {
            //string result = "";
            StringBuilder sb = new StringBuilder();
            var year = date.Year.ToString();
            var month = date.Month.ToString();
            var day = date.Day.ToString();
            foreach (char i in year)
            {
                sb.Append(CharToChinese(i));
            }
            sb.Append("年");
            sb.Append(NumToChinese(month));
            sb.Append("月");
            sb.Append(NumToChinese(day));
            sb.Append("日");
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmonth">格式如:2019.03</param>
        /// <returns></returns>
        public static void CMonthToDateRange(string cmonth, out DateTime? start, out DateTime? end)
        {
            var year = int.Parse(cmonth.Substring(0, 4));
            var month = int.Parse(cmonth.Substring(5, 2));
            var date = new DateTime(year, month, 1);
            start = date;
            end = date.GetMonthEnd();
        }
        /// <summary>
        /// 验证int是否可以转为CMonth,如:201901
        /// </summary>
        /// <param name="iCMonth"></param>
        /// <returns></returns>
        public static bool IsCMonthInt(int? iCMonth)
        {
            if (iCMonth == null) { return true; }
            string cmonth = iCMonth.ToString();
            if (cmonth.Length != 6) { return false; }

            var year = cmonth.Substring(0, 4);
            var month = cmonth.Substring(4, 2);
            DateTime d = new DateTime();
            if (DateTime.TryParse(year + "-" + month + "-" + "01", out d))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static DateTime CMonthToDate(string cmonth)
        {
            var year = int.Parse(cmonth.Substring(0, 4));
            var month = int.Parse(cmonth.Substring(5, 2));
            return new DateTime(year, month, 1);
        }
        public static DateTime? CMonthToDate(object cmonthObj)
        {
            var cmonth = ObjectToString(cmonthObj);
            if (cmonth.Length < 7) { return null; }
            return CMonthToDate(cmonth);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmonth"></param>
        /// <returns></returns>
        public static string CMonthAddMonths(string cmonth, int months)
        {
            if (cmonth == null) { return null; }
            return DateToCMonth(PFDataHelper.CMonthToDate(cmonth).AddMonths(months));
        }
        //public static DateTime YmToDate(string cmonth)
        //{
        //    var year = int.Parse(cmonth.Substring(0, 4));
        //    var month = int.Parse(cmonth.Substring(4, 2));
        //    return new DateTime(year, month, 1);
        //}
        public static string DateToCMonth(DateTime cmonth)
        {
            return cmonth.ToString(MonthFormat);
        }
        public static string DateToYM(DateTime cmonthDate)
        {
            return cmonthDate.ToString(YMFormat);
        }
        /// <summary>
        /// 最近一年的cmonth列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLastYearCMonthList()
        {
            var cmonthList = new List<string>();
            var now = DateTime.Now;
            for (int i = 0; i < 12; i++)
            {
                cmonthList.Add(now.ToString(MonthFormat));
                now = now.AddMonths(-1);
            }
            return cmonthList;
        }

        public static string NumToChinese(string x, bool isCapitalization = false)
        {
            //数字转换为中文后的数组 //转载请注明来自 http://www.goubangwang.com
            string[] P_array_num = isCapitalization ?
                new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" } :
                new string[] { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            //为数字位数建立一个位数组
            string[] P_array_digit = isCapitalization ?
                new string[] { "", "拾", "佰", "仟" } :
                new string[] { "", "十", "百", "千" };
            //为数字单位建立一个单位数组
            string[] P_array_units = new string[] { "", "万", "亿", "万亿" };
            string P_str_returnValue = ""; //返回值
            int finger = 0; //字符位置指针
            int P_int_m = x.Length % 4; //取模
            int P_int_k = 0;
            if (P_int_m > 0)
                P_int_k = x.Length / 4 + 1;
            else
                P_int_k = x.Length / 4;
            //外层循环,四位一组,每组最后加上单位: ",万亿,",",亿,",",万,"
            for (int i = P_int_k; i > 0; i--)
            {
                int P_int_L = 4;
                if (i == P_int_k && P_int_m != 0)
                    P_int_L = P_int_m;
                //得到一组四位数
                string four = x.Substring(finger, P_int_L);
                int P_int_l = four.Length;
                //内层循环在该组中的每一位数上循环
                for (int j = 0; j < P_int_l; j++)
                {
                    //处理组中的每一位数加上所在的位
                    int n = Convert.ToInt32(four.Substring(j, 1));
                    if (n == 0)
                    {
                        if (j < P_int_l - 1 && Convert.ToInt32(four.Substring(j + 1, 1)) > 0 && !P_str_returnValue.EndsWith(P_array_num[n]))
                            P_str_returnValue += P_array_num[n];
                    }
                    else
                    {
                        if (!(n == 1 && (P_str_returnValue.EndsWith(P_array_num[0]) | P_str_returnValue.Length == 0) && j == P_int_l - 2))
                            P_str_returnValue += P_array_num[n];
                        P_str_returnValue += P_array_digit[P_int_l - j - 1];
                    }
                }
                finger += P_int_L;
                //每组最后加上一个单位:",万,",",亿," 等
                if (i < P_int_k) //如果不是最高位的一组
                {
                    if (Convert.ToInt32(four) != 0)
                        //如果所有4位不全是0则加上单位",万,",",亿,"等
                        P_str_returnValue += P_array_units[i - 1];
                }
                else
                {
                    //处理最高位的一组,最后必须加上单位
                    P_str_returnValue += P_array_units[i - 1];
                }
            }
            return P_str_returnValue;
        }
        private static string CharToChinese(char c)
        {
            string result = "";
            switch (c)
            {
                case '0':
                    result = "〇";
                    break;
                case '1':
                    result = "一";
                    break;
                case '2':
                    result = "二";
                    break;
                case '3':
                    result = "三";
                    break;
                case '4':
                    result = "四";
                    break;
                case '5':
                    result = "五";
                    break;
                case '6':
                    result = "六";
                    break;
                case '7':
                    result = "七";
                    break;
                case '8':
                    result = "八";
                    break;
                case '9':
                    result = "九";
                    break;
                default:
                    result = c.ToString();
                    break;
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 千分位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Thousandth(object value)
        {
            if (value == null) { return ""; }
            string strValue = value.ToString();
            if (!PFDataHelper.StringIsNullOrWhiteSpace(strValue))
            {
                if (strValue.IndexOf('.') > -1)
                {//有小数点时
                    return Regex.Replace(strValue, "(\\d)(?=(\\d{3})+\\.)", "$1,");
                }
                else
                {
                    return Regex.Replace(strValue, "(\\d)(?=(\\d{3})+$)", "$1,");
                }
            }
            return strValue;
        }

        public static bool StringIsNullOrWhiteSpace(object s)
        {
            if (s == null) { return true; }
            if (s == DBNull.Value) { return true; }
            return StringIsNullOrWhiteSpace(s.ToString());
            //var ss = s.ToString();
            //if (ss == "null") { return true; }
            //return string.IsNullOrWhiteSpace(ss);
        }
        public static bool StringIsNullOrWhiteSpace(string s)
        {
            if (s == "null") { return true; }
            if (s == "undefined") { return true; }
            return string.IsNullOrEmpty(s) || s.Replace(" ", "") == "";
        }
        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringFirstCharUpper(ref string s)
        {
            if (s == null) { return s; }
            if (s.Length < 2) { return s.ToUpper(); }
            return s[0].ToString().ToUpper() + s.Substring(1);
        }
        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringFirstCharLower(ref string s)
        {
            if (s == null) { return s; }
            if (s.Length < 2) { return s.ToLower(); }
            return s[0].ToString().ToLower() + s.Substring(1);
        }
        //public static string StringFirstCharUpperNoRef(string s)
        //{
        //    if (s == null) { return s; }
        //    if (s.Length < 2) { return s.ToUpper(); }
        //    return s[0].ToString().ToUpper() + s.Substring(1);
        //}

        /// <summary>
        /// 字符串脱敏
        /// </summary>
        public static string StringDesensitization(string s, params KeyValuePair<int, int>[] starPos)
        {
            if (StringIsNullOrWhiteSpace(s)) { return s; }
            string r = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (starPos.Any(a => a.Key <= i && a.Value >= i))
                {
                    r += "*";
                }
                else
                {
                    r += s[i];
                }
            }
            return r;
        }

        /// <summary>
        /// 字符串脱敏
        /// </summary>
        public static string StringDesensitization(string s, int left, int right)
        {
            if (StringIsNullOrWhiteSpace(s)) { return s; }
            string r = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (i < left || i > s.Length - 1 - right)
                {
                    r += "*";
                }
                else
                {
                    r += s[i];
                }
            }
            return r;
        }

        /// <summary>
        /// 身份证脱敏
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string IdentificationDesensitization(string s)
        {
            return StringDesensitization(s, 3, 4);
        }


        #region Enum方法
        /// <summary>
        /// enum转list(调用方法：PFDataHelper.EnumToKVList(FgsFunc.客单量);)
        /// </summary>
        /// <param name="valueEnum"></param>
        /// <param name="useValue">是否使用Enum的Value作为Key</param>
        /// <returns></returns>
        public static List<KeyValuePair<object, object>> EnumToKVList(Enum valueEnum, bool useValue = false, bool showValueOnName = false)
        {
            var t = valueEnum.GetType();
            return EnumToKVList(t, useValue, showValueOnName);
            //return (from int value in Enum.GetValues(valueEnum.GetType())
            //        select new KeyValuePair<object, object>(
            //        useValue ? value.ToString() : Enum.GetName(valueEnum.GetType(), value),
            //        Enum.GetName(valueEnum.GetType(), value))
            //        ).ToList();
        }
        public static List<KeyValuePair<object, object>> EnumToKVList(Type enumType, bool useValue = false, bool showValueOnName = false)
        {
            Func<int, string> getName = (v) =>
            {
                string r = "";
                r = Enum.GetName(enumType, v);
                if (showValueOnName) { r = v + " " + r; }
                return r;
            };
            return (from int value in Enum.GetValues(enumType)
                    select new KeyValuePair<object, object>(
                    useValue ? value.ToString() : getName(value),
                    getName(value))
                    ).ToList();
        }
        public static bool EnumHasFlag(Enum source, Enum target)
        {
            return ((source.GetHashCode() & target.GetHashCode()) == target.GetHashCode());
        }
        /// <summary>
        /// source中的任意值和target中的任意值相等时,返回true
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EnumAnyFlag(Enum source, Enum target)
        {
            //return
            //    source.GetHashCode() != 0//Default=0与Default=0时,应为true才对(如FuncAuthority里)--benjamin20191202
            //    && (
            //        EnumHasFlag(source, target) || EnumHasFlag(target, source)
            //    );
            //return
            //    (source.GetHashCode() >= 0 && target.GetHashCode() == 0)
            //    || (
            //        source.GetHashCode() != 0
            //        && (
            //            EnumHasFlag(source, target) || EnumHasFlag(target, source)
            //        )
            //    );
            var srcHash = source.GetHashCode();
            var tarHash = target.GetHashCode();
            return
                (srcHash >= 0 && tarHash == 0)
                || (
                    srcHash != 0
                    && (
                        (srcHash & tarHash) != 0
                    )
                );//benjamin20200103
        }
        //public static List<KeyValuePair<object, object>> EnumFlagToArray(Enum valueEnum, bool useValue = false)
        //{
        //    var t = valueEnum.GetType();
        //    foreach (int value in Enum.GetValues(t))
        //    {
        //        if(valueEnum.HasFlag())
        //    }
        //}
        #endregion

        private static int chfrom = Convert.ToInt32("4e00", 16); //中文:范围（0x4e00～0x9fff）转换成int（chfrom～chend）
        private static int chend = Convert.ToInt32("9fff", 16);
        public static PFCharType GetCharType(char c)
        {
            PFCharType result = PFCharType.Default;
            int code = Char.ConvertToUtf32(c.ToString(), 0);
            //var engChars="abcdefghijklmnopq"
            if (code >= chfrom && code <= chend)//中文
            {
                result |= PFCharType.Chinese;
            }
            else if (c == 12288)//全角空格为12288，半角空格为32
            {
                result |= PFCharType.FullChar;
            }
            else if (c > 65280 && c < 65375)//其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
            {
                result |= PFCharType.FullChar;
            }
            else if (c == 32)//半角空格
            {
                result |= PFCharType.HalfChar;
            }
            else if (c > 32 && c < 127)//半角字符,如英文单引号
            {
                result |= PFCharType.HalfChar;
            }
            else if ((c >= 97 && c <= 122)//小写英文
                || (c >= 65 && c <= 90)//大写英文
                )
            {
                result |= PFCharType.English;
            }
            else if (c >= 48 && c <= 57)//数字
            {
                result |= PFCharType.Number;
            }
            else if (c == 9)//字符\t,数据库中占1
            {
                result |= PFCharType.HalfChar;
            }
            else if (c == 127)//特殊字符"方框",数据库中占1
            {
                result |= PFCharType.HalfChar;
            }
            else
            {
            }
            return result;
        }
        //public static int GetWordsCharLengthOut(string words, out int englishCnt, out int cnCnt//, bool checkNull=true
        //    )
        //{
        //    englishCnt = 0;
        //    cnCnt = 0;
        //    if (//checkNull&&
        //        PFDataHelper.StringIsNullOrWhiteSpace(words)) { return 0; }
        //    var english = Regex.Replace(words, "([\u4E00-\u9FA5\uf900-\ufa2d])", "");
        //    englishCnt = english.Length;
        //    cnCnt = words.Length - englishCnt;//余下的是中文数
        //    return englishCnt + (cnCnt * 2);
        //}
        public static int GetWordsCharLengthOut(string words, out int englishCnt, out int cnCnt//, bool checkNull=true
            )
        {
            var result = 0;
            englishCnt = 0;
            cnCnt = 0;

            foreach (var i in words)
            {
                var ct = GetCharType(i);
                if (ct == PFCharType.Chinese)
                {
                    cnCnt += 1;
                    result += 2;
                }
                else if (ct == PFCharType.English)
                {
                    englishCnt += 1;
                    result += 1;
                }
                else if (ct == PFCharType.Number)
                {
                    englishCnt += 1;
                    result += 1;
                }
                else if (ct == PFCharType.FullChar)
                {
                    cnCnt += 1;
                    result += 2;
                }
                else if (ct == PFCharType.HalfChar)
                {
                    englishCnt += 1;
                    result += 1;
                }
                else
                {
                    cnCnt += 1;
                    result += 2;
                }
            }
            return result;
        }
        public static int GetWordsCharLength(string words//, bool checkNull=true
            )
        {
            var englishCnt = 0;
            var cnCnt = 0;
            return GetWordsCharLengthOut(words, out englishCnt, out cnCnt);
        }
        /// <summary>
        /// 设置字符串长度(截取,为了便于测试)
        /// </summary>
        /// <param name="words"></param>
        /// <param name="englishCnt"></param>
        /// <param name="cnCnt"></param>
        /// <returns></returns>
        public static string SetWordsCharLength(string words, int length
            )
        {
            var result = "";
            for (int i = 0; i < words.Length; i++)
            {
                var ct = GetCharType(words[i]);
                if (ct == PFCharType.Chinese)
                {
                    length -= 2;
                }
                else if (ct == PFCharType.English)
                {
                    length -= 1;
                }
                else if (ct == PFCharType.Number)
                {
                    length -= 1;
                }
                else if (ct == PFCharType.FullChar)
                {
                    length -= 2;
                }
                else if (ct == PFCharType.HalfChar)
                {
                    length -= 1;
                }
                else
                {
                    length -= 2;
                }

                if (length < 0)
                {
                    return result;
                }
                else
                {
                    result += words[i];
                }
            }
            return result;
        }
        /// 转全角的函数(SBC case)
        ///
        ///任意字符串
        ///全角字符串
        ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///
        public static String ToSBC(String input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }

        /**/
        // /
        // / 转半角的函数(DBC case)
        // /
        // /任意字符串
        // /半角字符串
        // /
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        public static String ToDBC(String input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }
        /// <summary>
        /// 计算文字句子的宽度(主要用于计算表头宽度)
        /// </summary>
        /// <param name="words"></param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontSpace">估计文字间隔</param>
        /// <param name="paddingLR">默认0是因为有的地方在调用此方法的后面统一计算比较方便</param>
        /// <returns></returns>
        public static string GetWordsWidth(string words, int? fontSizeIn = null, int? fontSpaceIn = null, int? paddingLRIn = null, string fontWeight = null)//fontSpace = 2
        {
            //if (PFDataHelper.StringIsNullOrWhiteSpace(words)) { return null; }
            var fontSize = fontSizeIn == null ? 12 : fontSizeIn.Value;
            var fontSpace = fontSpaceIn == null ? 0 : fontSpaceIn.Value;
            var paddingLR = paddingLRIn == null ? 0 : paddingLRIn.Value;
            int englishCnt = 0;
            int cnCnt = 0;
            GetWordsCharLengthOut(words, out englishCnt, out cnCnt);//, false);
            double weightAddWidth = 0;
            if (fontWeight != null)//粗体字的情况下,bold所增加的px和fontSize成线性关系,k和b的值是通过线性回归得到的
            {
                double k = 0;
                double b = 0;
                if (fontWeight == "bold")
                {
                    k = 0.019530096957208705;
                    b = 0.017247150640138;
                }
                weightAddWidth = fontSize * k + b;
                if (weightAddWidth < 1)
                {
                    weightAddWidth = 1;
                }
                else
                {
                    weightAddWidth = Math.Round(weightAddWidth);
                }
            }
            return (
                    (fontSpace * (englishCnt + cnCnt)) //间隔长度
                    + ((fontSize + weightAddWidth) * cnCnt) //中文长度 
                    + ((fontSize / 2 + weightAddWidth) * englishCnt) //英文长度
                    + paddingLR
                ).ToString("0.##") + "px";

            #region old
            //if (PFDataHelper.StringIsNullOrWhiteSpace(words)) { return null; }
            //var english = Regex.Replace(words, "([\u4E00-\u9FA5\uf900-\ufa2d])", "");
            //decimal englishCnt = english.Length;
            //decimal cnCnt = words.Length - englishCnt;//余下的是中文数
            //return (
            //        (fontSpace * (englishCnt + cnCnt)) //间隔长度
            //        + (fontSize * cnCnt) //中文长度 
            //        + (fontSize * englishCnt / 2) //英文长度
            //        + paddingLR
            //    ).Value.ToString("0.##") + "px"; 
            #endregion
        }
        /// <summary>
        /// 计算年龄(如果要用DateTime参数,就需要区别?可空类型,使用时也很麻烦,直接用object比较方便)
        /// </summary>
        /// <param name="birth">出生日期</param>
        /// <returns></returns>
        public static int? GetAge(object birth)
        {
            if (birth is DateTime)
            {
                var dBirth = (DateTime)birth;
                int age = DateTime.Now.Year - dBirth.Year;
                if (DateTime.Now.Month < dBirth.Month || (DateTime.Now.Month == dBirth.Month && DateTime.Now.Day < dBirth.Day)) age--;
                return age;
            }
            return null;
        }
        #region Object
        public static decimal? ObjectToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) { return null; }
            decimal r = 0;
            if (decimal.TryParse(value.ToString(), out r))
            {
                return r;
            }
            return null;
        }
        public static int? ObjectToInt(object value)
        {
            if (value == null || value == DBNull.Value) { return null; }
            if (value is bool)
            {
                var b = (bool)value;
                return b ? 1 : 0;
            }
            int r = 0;
            if (int.TryParse(value.ToString(), out r))
            {
                return r;
            }
            return null;
        }
        public static long? ObjectToLong(object value)//long就是int64
        {
            if (value == null || value == DBNull.Value) { return null; }
            if (value is bool)
            {
                var b = (bool)value;
                return b ? 1 : 0;
            }
            long r = 0;
            if (value is double)
            {
                if (long.TryParse(((double)value).ToString("0"), out r))//double小数位数过多会报错--benjamin20190927
                {
                    return r;
                }
            }
            if (long.TryParse(value.ToString(), out r))
            {
                return r;
            }
            return null;
        }
        public static double? ObjectToDouble(object value)
        {
            if (value == null || value == DBNull.Value) { return null; }
            if (value is bool)
            {
                var b = (bool)value;
                return b ? 1 : 0;
            }
            double r = 0;
            if (double.TryParse(value.ToString(), out r))
            {
                return r;
            }
            return null;
        }
        public static bool? ObjectToBool(object value)
        {
            if (value == null || value == DBNull.Value) { return null; }
            bool r = false;
            var vs = value.ToString();
            if (vs == "1") { return true; }//"1"的话tryParse是失败的
            if (vs == "0") { return false; }//"1"的话tryParse是失败的
            if (bool.TryParse(vs, out r))
            {
                return r;
            }
            if (value.ToString() == "true,false") { return true; }//mvc的Checkbox跟着一个hidden,如果选中时,传回来的是"true,false"
            return null;
        }
        public static DateTime? ObjectToDateTime(object value)
        {
            if (value == null || value == DBNull.Value) { return null; }
            DateTime r;
            var sv = value.ToString();
            if (sv.IndexOf("(CST)") > -1)//126邮箱的时间格式为: Tue, 21 Jan 2020 09:25:08 +0800 (CST) ,DateTime.TryParse转换不了--benjamin20200121
            {
                sv = sv.Replace("(CST)", "");
            }
            if (DateTime.TryParse(sv, out r))
            {
                return r;
            }
            return null;
        }
        public static DateTime? IDCardToBirthDay(object value)
        {
            if (value == null || value == DBNull.Value) { return null; }
            DateTime r;
            var sv = value.ToString();
            if (sv.Length < 14) { return null; }
            sv = sv.Substring(6, 8);
            sv = sv.Insert(6, "-");
            sv = sv.Insert(4, "-");
            if (DateTime.TryParse(sv, out r))
            {
                if (r > DateTime.Now) { return null; }
                return r;
            }
            return null;
        }
        public static TEnum? ObjectToEnum<TEnum>(object value)
            where TEnum : struct
        {
            if (value == null || value == DBNull.Value) { return null; }
            TEnum r;
            try
            {
                r = (TEnum)Enum.Parse(typeof(TEnum), value.ToString());
                return r;
            }
            catch (Exception)//e)
            {
                return null;
            }
            //if (Enum.TryParse(value.ToString(), out r))
            //{
            //    return r;
            //}
            //return null;
        }
        public static Object ConvertObjectByType(object value, Type srcType, Type dstType)
        {
            //当把DataTable转为LocalJson再读为DataTable时会出现这些情况,如"查网络图"里的ch字段,以后再考虑怎么排除此问题
            if (dstType == typeof(decimal?) || dstType == typeof(decimal)
           )
            {
                return ObjectToDecimal(value);
            }
            //else if ((dstType == typeof(Int32?) || dstType == typeof(Int32))
            //&& srcType == typeof(Int64)
            //)
            //{
            //    return Convert.ToInt32(value);
            //}
            else if (dstType == typeof(Int32?) || dstType == typeof(Int32)
            )
            {
                return PFDataHelper.ObjectToInt(value);
            }
            else if (srcType == typeof(SByte) && (dstType == typeof(bool?) || dstType == typeof(bool))//mysql中读出来的bool类型是SBype的
            )
            {
                var sb = (SByte)value;
                if (sb == 1) { return true; }
                if (sb == 0) { return false; }
                return null;
            }
            else if (srcType == typeof(int) && (dstType == typeof(bool?) || dstType == typeof(bool))//mysql中读出来的bool类型是SBype的
            )
            {
                if (value != null)
                {
                    var sb = value.ToString();
                    if (sb == "1") { return true; }
                    if (sb == "0") { return false; }
                }
                return null;
            }
            else if (srcType == typeof(decimal) && (dstType == typeof(Int64?) || dstType == typeof(Int64))//mysql中读出来的bigint类型是decimal的?如us_user.id列--benjamin20190730
            )
            {
                return ObjectToLong(value);
            }
            else if (//(srcType == typeof(string)|| (srcType == typeof(MySqlDateTime)) && 
                (dstType == typeof(DateTime?) || dstType == typeof(DateTime))//mysql中if(c.card_time<'1753-01-01 00:00:00','1753-01-01 00:00:00',c.card_time) as fjoindate得到的竟然是string格式,原因不明
            )
            {
                return ObjectToDateTime(value);
            }
            else if (srcType == typeof(string) && dstType == typeof(string[]))
            {
                return (value as string).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return value;
            }

        }
        public static bool IsObjectEquals(object o1, object o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }
            else if (o1 == null || o2 == null)
            {
                return false;
            }
            else
            {
                return o1.Equals(o2);
            }
        }

        //public static void DisaposeObject<T>(object o1)
        //    where T: class,IDisposable
        //{
        //    if (o1 == null) { return; }
        //    (o1 as T).Dispose();
        //    o1 = null;
        //}
        public static void DisaposeObject(object o1)
        {
            if (o1 == null) { return; }
            if (o1 is IDisposable)
            {
                (o1 as IDisposable).Dispose();
            }
        }
        #endregion Object
        /// <summary>
        /// 同比
        /// </summary>
        /// <param name="thisYear"></param>
        /// <param name="lastYear"></param>
        /// <returns></returns>
        public static string GetYearOnYear(decimal? thisYear, decimal? lastYear)
        {
            //if(thisYear==null|| lastYear == null) { return null; }
            //if (thisYear == 0 || lastYear == 0) { return 0; }
            if (lastYear == null || lastYear == 0) { return null; }
            var r = (thisYear - lastYear) * 100 / lastYear;
            return r == null ? null : System.Decimal.Round(r.Value, 2).ToString() + " %";
        }
        /// <summary>
        /// 环比
        /// </summary>
        /// <param name="thisYear"></param>
        /// <param name="lastYear"></param>
        /// <returns></returns>
        public static string GetRingRatio(decimal? thisMonth, decimal? lastMonth)
        {
            //if(thisYear==null|| lastYear == null) { return null; }
            //if (thisYear == 0 || lastYear == 0) { return 0; }
            if (lastMonth == null || lastMonth == 0) { return null; }
            var r = (thisMonth - lastMonth) * 100 / lastMonth;
            return r == null ? null : System.Decimal.Round(r.Value, 2).ToString() + " %";
        }

        /// <summary>
        /// 计划完成率
        /// </summary>
        /// <param name="thisYear"></param>
        /// <param name="lastYear"></param>
        /// <returns></returns>
        public static string GetPlanCompletionRate(decimal? actual, decimal? plan)
        {
            if (plan == null || plan == 0) { return null; }
            var r = (actual / plan) * 100;
            return r == null ? null : System.Decimal.Round(r.Value, 2).ToString() + " %";
        }

        /// <summary>
        /// NameValueCollection转T类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T NameValueCollectionToModel<T>(this NameValueCollection c)
            where T : new()
        {
            var model = new T();
            if (c != null)
            {
                var t = typeof(T);
                foreach (var name in c.AllKeys)
                {
                    if (name != "operator")
                    {
                        var v = c[name];
                        if (!PFDataHelper.StringIsNullOrWhiteSpace(v))
                        {
                            try
                            {
                                PropertyInfo p = null;
                                p = t.GetProperty(name);
                                var vObj = Convert.ChangeType(v, GetPropertyType(p));
                                if (p != null) { p.SetValue(model, vObj, null); }
                            }
                            catch (Exception e)//有的字段用了系统关键字会报错,如operator
                            {
                                WriteError(e);
                            }
                        }
                    }
                }
            }
            return model;
        }
        #endregion Data

        #region DataTable
        /// <summary>
        /// KVP比SelectList更容易操作item
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dataValueField"></param>
        /// <param name="dataTextField"></param>
        /// <returns></returns>
        public static List<KeyValuePair<object, object>> DataTableToKVList(this DataTable dt, string dataValueField, string dataTextField)
        {
            //创建返回的集合
            List<KeyValuePair<object, object>> oblist = new List<KeyValuePair<object, object>>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //创建TResult的实例
                    //KeyValuePair<string, string> ob = new KeyValuePair<string, string>((row[dataValueField]??"").ToString(), (row[dataValueField]??"").ToString());
                    KeyValuePair<object, object> ob = new KeyValuePair<object, object>(row[dataValueField], row[dataTextField]);
                    oblist.Add(ob);
                }
            }
            return oblist;
        }

        /// <summary>
        /// DataTable 转换为List 集合
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <param name="eachRow">(o,dr)=></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(this DataTable dt, Action<T, DataRow> eachRow = null
            ) where T : class, new()
        {
            //创建一个属性的列表
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            Type t = typeof(T);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表 
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合
            List<T> oblist = new List<T>();
            string currentColumnName = "";
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    //创建TResult的实例
                    T ob = new T();
                    //找到对应的数据  并赋值
                    prlist.ForEach(p =>
                    {
                        if (row[p.Name] != DBNull.Value && row[p.Name] != null)
                        {
                            currentColumnName = p.Name;
                            var pt = PFDataHelper.GetPropertyType(p);
                            if (pt.IsEnum)
                            {
                                p.SetValue(ob, Enum.Parse(pt, row[p.Name].ToString()), null);
                            }
                            //else if (pt.IsSubclassOf(typeof(PFCustomStringType)))
                            //{
                            //    p.SetValue(ob, row[p.Name].ToString(), null);
                            //}
                            else
                            {
                                //if (pt == typeof(decimal) && dt.Columns[p.Name].DataType == typeof(int))
                                //{
                                //    //由于某些数据库版本不统一的问题会导致,inv表的pv是decimal的，但tc_inv表的pv是int的，于是建model时tc_inv都采用decimal就行了
                                //}

                                p.SetValue(ob, ConvertObjectByType(row[p.Name], dt.Columns[p.Name].DataType, p.PropertyType), null);

                            }
                        }
                    });
                    if (eachRow != null) { eachRow(ob, row); }
                    //放入到返回的集合中.
                    oblist.Add(ob);
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("DataTableToList方法报错,当前字段:{0},错误信息:{1}", currentColumnName, e);
                WriteError(new Exception(msg));
            }
            return oblist;
        }
        /// <summary>
        /// 转为字典(MVC直接返回DataTable不能序列化,可转为字典返回)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> DataTableToDictList(this DataTable dt, bool useStringValue = true)
        {
            //创建返回的集合
            List<Dictionary<string, object>> arrayList = new List<Dictionary<string, object>>();
            var columns = dt.Columns;
            foreach (DataRow dataRow in dt.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (DataColumn dataColumn in columns)
                {
                    dictionary.Add(dataColumn.ColumnName.IndexOf(".") > -1 ? dataColumn.ColumnName.Replace(".", "_") : dataColumn.ColumnName
                        , useStringValue ? dataRow[dataColumn.ColumnName].ToString() : dataRow[dataColumn.ColumnName]);
                }
                arrayList.Add(dictionary);
            }
            return arrayList;
        }

        /// <summary>
        /// 转为字典
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string, object> DataTableToDict(this DataTable dt, string keyField, string valueField)
        {
            //创建返回的集合
            Dictionary<string, object> arrayList = new Dictionary<string, object>();
            //try
            //{
            var key = "";
            var columns = dt.Columns;
            object value;

            foreach (DataRow dataRow in dt.Rows)
            {
                key = (dataRow[keyField] ?? "").ToString();
                value = dataRow[valueField];
                if (!arrayList.ContainsKey(key))
                {
                    arrayList.Add(key, dataRow[valueField]);
                }
                else
                {
                    arrayList[key] += ("," + value);
                }
            }

            return arrayList;
        }
        public static Dictionary<string, Dictionary<string, object>> DataTableToRowDict(this DataTable dt, string keyField, params string[] valueField)
        {
            //创建返回的集合
            Dictionary<string, Dictionary<string, object>> arrayList = new Dictionary<string, Dictionary<string, object>>();
            //try
            //{
            var key = "";
            var columns = dt.Columns;
            object value;

            foreach (DataRow dataRow in dt.Rows)
            {
                key = (dataRow[keyField] ?? "").ToString();
                //value = dataRow;
                if (!arrayList.ContainsKey(key))
                {
                    arrayList.Add(key, new Dictionary<string, object>());
                }
                foreach (string vf in valueField)
                {
                    value = dataRow[vf];
                    if (!arrayList[key].ContainsKey(vf))
                    {
                        arrayList[key].Add(vf, value);
                    }
                    else
                    {
                        arrayList[key][vf] += ("," + value);
                    }
                }
            }

            return arrayList;
        }

        /// <summary>
        /// 转化一个DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(this IEnumerable<T> list)
        {
            //创建属性的集合
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口
            Type type = typeof(T);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列
            //Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            var aa = type.GetProperties().Where(p => p.PropertyType.IsPublic && p.CanRead
            //&& p.CanWrite //CanRead已经够了--benjamin20190802
            ).ToArray();
            Array.ForEach<PropertyInfo>(aa, p => { pList.Add(p); dt.Columns.Add(p.Name, GetPropertyType(p)); });
            foreach (var item in list)
            {
                //创建一个DataRow实例
                DataRow row = dt.NewRow();
                //给row 赋值
                pList.ForEach(p =>
                {
                    var v = p.GetValue(item, null);
                    if (v == null)
                    {
                        row[p.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[p.Name] = v;
                    }
                });
                //加入到DataTable
                dt.Rows.Add(row);
            }
            return dt;
        }
        public static DataTable DictListToDataTable(List<Dictionary<string, object>> list)
        {
            var columnType = new Dictionary<string, Type>();
            DataTable dt = new DataTable();
            ////创建返回的集合
            //List<Dictionary<string, object>> arrayList = new List<Dictionary<string, object>>();
            //var columns = dt.Columns;
            foreach (Dictionary<string, object> dataRow in list)
            {
                //创建一个DataRow实例
                DataRow row = dt.NewRow();
                //Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> cell in dataRow)
                {
                    if (columnType.ContainsKey(cell.Key))
                    {

                    }
                    else if (cell.Value != null && cell.Value != DBNull.Value)
                    {
                        columnType.Add(cell.Key, cell.Value.GetType());
                        dt.Columns.Add(cell.Key, columnType[cell.Key]);
                    }
                    if (columnType.ContainsKey(cell.Key))
                    {
                        row[cell.Key] = cell.Value ?? DBNull.Value;
                    }
                }
                //加入到DataTable
                dt.Rows.Add(row);
            }
            return dt;
        }

        //public static int ColumnTotal(DataTable mydt, string colname)
        //{
        //    int result = 0;
        //    int ColIndex = mydt.Columns[colname].Ordinal;
        //    if (ColIndex >= 0 && ColIndex < mydt.Columns.Count)
        //    {
        //        for (int i = 0; i < mydt.Rows.Count; i++)
        //        {
        //            if (mydt.Rows[i][ColIndex].GetType() != typeof(DBNull))
        //            {
        //                result += Convert.ToInt32(mydt.Rows[i][ColIndex]);
        //            }
        //        }
        //    }
        //    return result;
        //}
        public static decimal ColumnTotal(DataTable mydt, string colname)
        {
            //var t = typeof(T);
            //T result = (T)Activator.CreateInstance(t); ;
            decimal result = 0;
            int ColIndex = mydt.Columns[colname].Ordinal;
            if (ColIndex >= 0 && ColIndex < mydt.Columns.Count)
            {
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    var t = mydt.Rows[i][ColIndex].GetType();
                    if (t != typeof(DBNull))
                    {
                        if (t == typeof(decimal)) { result += Convert.ToDecimal(mydt.Rows[i][ColIndex]); }
                        else if (t == typeof(int)) { result += Convert.ToInt32(mydt.Rows[i][ColIndex]); }
                    }
                }
            }
            return result;
        }

        public static string DataTableToString(DataTable dt, string split)
        {
            StringBuilder sb = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                int colInt = dt.Columns.Count;
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < colInt; i++)
                    {
                        sb.Append(row[i].ToString() + split);
                    }
                }
                string str = sb.ToString();
                return str.Substring(0, str.Length - 1);
            }
            else
            {
                return string.Empty;
            }
        }

        public static DataTable DataTableFilter(this DataTable dt, string filterValue)
        {
            if (StringIsNullOrWhiteSpace(filterValue)) { return dt; }
            filterValue = filterValue.ToLower();
            var arrFillter = filterValue.Split(',');
            DataTable newdt = new DataTable();
            foreach (System.Data.DataColumn dcol in dt.Columns)
            {
                DataColumn dc = new DataColumn();
                dc.DataType = dcol.DataType;
                dc.ColumnName = dcol.ColumnName;
                dc.DefaultValue = dcol.DefaultValue;
                newdt.Columns.Add(dc);
            }
            var rowStart = 0;
            var rowEnd = dt.Rows.Count - 1;
            for (int i = rowStart; i <= rowEnd; i++)
            {
                if (dt.Rows[i].ItemArray != null
                    //&& dt.Rows[i].ItemArray.Any(a =>
                    //a==null?false:a.ToString().ToLower()==filterValue)
                    //&& dt.Rows[i].ItemArray.Any(a =>
                    //a == null ? false : arrFillter.Contains(a.ToString().ToLower()))
                    && IsFilterFitRow(dt.Rows[i], arrFillter)
                    )
                {
                    DataRow dr = newdt.NewRow();
                    dr.ItemArray = dt.Rows[i].ItemArray;
                    newdt.Rows.Add(dr);
                }
            }
            return newdt;
        }
        public static bool IsFilterFitRow(DataRow row, string[] arr)
        {
            if (row.ItemArray == null) { return false; }
            foreach (var i in arr)
            {
                if (StringIsNullOrWhiteSpace(i)) { continue; }
                if (!row.ItemArray.Any(a =>
                     a == null ? false : a.ToString().ToLower() == i))
                {
                    return false;
                }
            }
            return true;
        }
        public static DataTable DataPager(this DataTable dt, int pageIndex, int pageSize)
        {
            int pageCount = 0;
            return DataTablePaging(dt, pageIndex, pageSize, out pageCount);
        }

        private static DataTable DataTablePaging(DataTable dt, int pageIndex, int pageSize, out int pageCount)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                pageCount = -1;
                return null;
            }
            int recordCount = dt.Rows.Count;
            int _pageCount2 = recordCount / pageSize;

            if (pageIndex < 0)
                pageIndex = 0;
            else if (pageIndex > _pageCount2)
                pageIndex = _pageCount2;

            DataTable newdt = new DataTable();
            foreach (System.Data.DataColumn dcol in dt.Columns)
            {
                DataColumn dc = new DataColumn();
                dc.DataType = dcol.DataType;
                dc.ColumnName = dcol.ColumnName;
                dc.DefaultValue = dcol.DefaultValue;
                newdt.Columns.Add(dc);
            }
            int lastPageRecordCount = recordCount % pageSize;
            int rowStart = pageIndex * pageSize;
            int rowEnd;

            if (pageIndex == _pageCount2 && lastPageRecordCount != 0)
            {
                rowEnd = pageIndex * pageSize + lastPageRecordCount - 1;
            }
            else
            {
                rowEnd = (pageIndex + 1) * pageSize - 1;
            }

            for (int i = rowStart; i <= rowEnd; i++)
            {
                DataRow dr = newdt.NewRow();
                dr.ItemArray = dt.Rows[i].ItemArray;
                newdt.Rows.Add(dr);
            }

            if (lastPageRecordCount != 0)
            {
                pageCount = _pageCount2 + 1;
            }
            else
            {
                pageCount = _pageCount2;
            }
            return newdt;
        }

        public static DataTable DataPager(this DataTable dt, int pageIndex, int pageSize, out int pageCount)
        {
            return DataTablePaging(dt, pageIndex, pageSize, out pageCount);
        }
        private static DataTable SetColumnProperty(this DataTable dt, string columnName, string propertyName, object propertyValue)
        {

            if (dt.Columns[columnName] != null)
            {
                dt.Columns[columnName].ExtendedProperties[propertyName] = propertyValue;
            }
            return dt;
        }
        private static DataColumn SetColumnProperty(DataColumn column, string propertyName, object propertyValue)
        {

            if (column != null)
            {
                column.ExtendedProperties[propertyName] = propertyValue;
            }
            return column;
        }
        private static DataTable SetColumnProperty(this DataTable dt, int columnIdx, string propertyName, string propertyValue)
        {
            if (dt.Columns.Count > columnIdx)
            {
                dt.Columns[columnIdx].ExtendedProperties[propertyName] = propertyValue;
            }
            return dt;
        }
        private static DataTable SetColumn(this DataTable dt, string columnName, Action<DataColumn> action)
        {
            var c = dt.Columns[columnName];
            if (c != null)
            {
                action(c);
            }
            return dt;
        }
        private static object GetColumnProperty(this DataTable dt, string columnName, string propertyName)
        {

            if (dt.Columns[columnName] != null)
            {
                return dt.Columns[columnName].ExtendedProperties[propertyName];
            }
            return null;
        }
        /// <summary>
        /// 自用的列中文标题
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static DataTable SetColumnTitle(this DataTable dt, string columnName, string title)
        {
            return SetColumnProperty(dt, columnName, "title", title);
        }
        public static DataColumn SetColumnTitle(this DataColumn column, string title)
        {
            return SetColumnProperty(column, "title", title);
        }
        public static DataTable SetColumnWidth(this DataTable dt, string columnName, string width)
        {
            return SetColumnProperty(dt, columnName, "width", width);
        }
        public static DataTable SetColumnWidth(this DataTable dt, int columnIdx, string width)
        {
            return SetColumnProperty(dt, columnIdx, "width", width);
        }
        public static DataTable SetColumnHasSummary(this DataTable dt, string columnName)
        {
            return SetColumnProperty(dt, columnName, "hasSummary", true);
        }
        public static DataTable SetColumnSummaryType(this DataTable dt, string columnName, SummaryType summaryType)
        {
            return SetColumnProperty(dt, columnName, "summaryType", summaryType.ToString());
        }
        //public static DataTable SetColumnCompute(this DataTable dt, string columnName, string expression)
        //{
        //    SetColumn(dt,columnName,a=> {
        //        a.ExtendedProperties["compute"]="1";
        //        a.Expression = expression;
        //    });
        //    return dt;
        //}
        public static DataColumn SetColumnCompute(this DataColumn col, string expression)
        {
            col.ExtendedProperties["compute"] = true;
            col.Expression = expression;
            return col;
        }
        public static DataTable SetTableExData(this DataTable dt, Object exData)
        {

            if (dt != null)
            {
                dt.ExtendedProperties["exData"] = exData;
            }
            return dt;
        }
        /// <summary>
        /// 是计算列时,groupby不运算
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnIdx"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static bool IsColumnCompute(this DataColumn col)
        {
            var p = col.ExtendedProperties["compute"];
            return p != null && ((bool)p).Equals(true);
        }
        //public static bool IsColumnCompute(this DataTable dt, string columnName)
        //{
        //    var p = GetColumnProperty(dt, columnName, "compute");
        //    return p != null && (p as string).Equals("1");
        //}
        private static void CopyTableColumn(DataColumn srcColumn, DataTable dst)
        {
            var dstColumn = dst.Columns.Add(srcColumn.ColumnName, srcColumn.DataType);
            foreach (var key in srcColumn.ExtendedProperties.Keys)
            {
                dstColumn.ExtendedProperties[key] = srcColumn.ExtendedProperties[key];
            }
            if (srcColumn.Expression != null) { dstColumn.Expression = srcColumn.Expression; }
        }
        public static void CopyTableRow(DataRow srcRow, DataTable dst)
        {
            //var src = rows.Table;
            var dstRow = dst.NewRow();
            foreach (DataColumn column in dst.Columns)
            {
                dstRow[column.ColumnName] = srcRow[column.ColumnName];
            }
            dst.Rows.Add(dstRow);
        }
        /// <summary>
        /// Table汇总方法，如果不提供valueFields，默认对group之外的所有int或decimal字段汇总
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="groupFields"></param>
        /// <param name="valueFields"></param>
        /// <returns></returns>
        public static DataTable DataTableGroupBy(DataTable dt, string[] groupFields, PFKeyValueCollection<SummaryType> valueFields = null)
        {
            try
            {
                var columns = dt.Columns;
                var columnType = new Dictionary<string, Type>();
                //var valueColumnType = new Dictionary<string, Type>();
                bool allValue = valueFields == null;
                if (allValue) { valueFields = new PFKeyValueCollection<SummaryType>(); }
                DataTable result = new DataTable();
                var rowList = dt.Rows.Cast<DataRow>().ToList();
                //加列
                List<string> srcColumnNames = new List<string>();
                foreach (DataColumn dataColumn in columns)
                {
                    if (groupFields.Contains(dataColumn.ColumnName))
                    {
                        CopyTableColumn(dataColumn, result);
                        //result.Columns.Add(dataColumn.ColumnName, dataColumn.DataType).ExtendedProperties=dataColumn.ExtendedProperties;
                    }
                    else if (dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(int)
                        || dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(long))//其实字符串也可以汇总的，如Max之类，但好像没什么意义
                    {
                        if (allValue)
                        {
                            valueFields.Add(dataColumn.ColumnName, SummaryType.Sum);
                        }
                        //if (//allValue||
                        //    valueFields.Keys.Contains(dataColumn.ColumnName))
                        if (//allValue||
                            valueFields.Keys.Contains(dataColumn.ColumnName))
                        {
                            CopyTableColumn(dataColumn, result);
                            //result.Columns.Add(dataColumn.ColumnName, dataColumn.DataType);
                        }
                    }
                    srcColumnNames.Add(dataColumn.ColumnName);
                }
                //因为dt有可能经过其它代码groupby的，就会出现groupField的列不存在于dt的情况
                groupFields = groupFields.Where(a => srcColumnNames.Contains(a)).ToArray();
                valueFields = new PFKeyValueCollection<SummaryType>(valueFields.Where(a => srcColumnNames.Contains(a.Key)).Select(b => b).ToList());

                List<IGrouping<string, DataRow>> group = rowList
                    .GroupBy<DataRow, string>(dr =>
                    {
                        var g = "";
                        foreach (var i in groupFields)
                        {
                            //g += ObjectToString(dr[i]) ?? "";//太慢
                            g += (dr[i] ?? "").ToString();
                        }
                        return g;
                    }).ToList();//按A分组  
                foreach (IGrouping<string, DataRow> ig in group)
                {
                    //创建一个DataRow实例
                    DataRow row = result.NewRow();
                    //分组列
                    var f = ig.First();//用于得到分组
                    foreach (var i in groupFields)
                    {
                        //row[i]=ig.Key[i]//这样写要改上面group的对象为Dictionary
                        row[i] = f[i];
                    }
                    //值列
                    foreach (var i in valueFields)
                    {
                        if (!IsColumnCompute(result.Columns[i.Key]))//不是计算列才设置值
                        {
                            var dataType = columns[i.Key].DataType;
                            if (i.Value == SummaryType.Sum)
                            {
                                if (dataType == typeof(decimal))
                                {
                                    row[i.Key] = ig.Sum(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToDecimal(r[i.Key]);
                                    });
                                }
                                else if (dataType == typeof(int))
                                {
                                    row[i.Key] = ig.Sum(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToInt(r[i.Key]);
                                    });
                                }
                                else if (dataType == typeof(double))
                                {
                                    row[i.Key] = ig.Sum(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToDouble(r[i.Key]);
                                    });
                                }
                                else if (dataType == typeof(long))
                                {
                                    row[i.Key] = ig.Sum(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToLong(r[i.Key]);
                                    });
                                }
                            }
                            if (i.Value == SummaryType.Average)
                            {
                                if (dataType == typeof(decimal))
                                {
                                    row[i.Key] = Math.Round(ig.Average(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToDecimal(r[i.Key]) ?? 0;
                                    }), DecimalPrecision);
                                }
                                else if (dataType == typeof(int))
                                {
                                    row[i.Key] = ig.Average(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToInt(r[i.Key]);
                                    });
                                }
                                else if (dataType == typeof(double))
                                {
                                    row[i.Key] = ig.Average(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToDouble(r[i.Key]);
                                    });
                                }
                                else if (dataType == typeof(long))
                                {
                                    row[i.Key] = ig.Average(delegate (DataRow r)
                                    {
                                        return PFDataHelper.ObjectToLong(r[i.Key]);
                                    });
                                }
                            }
                        }

                    }

                    //加入到DataTable
                    result.Rows.Add(row);
                }
                //ExtProp
                foreach (var key in dt.ExtendedProperties.Keys)
                {
                    result.ExtendedProperties[key] = dt.ExtendedProperties[key];
                }

                return result;
            }
            catch (Exception e)
            {
                WriteError(e);
            }
            return null;
        }

        /// <summary>
        /// 同比的计算列表达式如 (thisYear-lastYear)*100/lastYear
        /// </summary>
        /// <param name="thisYear"></param>
        /// <param name="lastYear"></param>
        /// <returns></returns>
        public static string GetColumnYearOnYear(string thisYear, string lastYear)
        {
            return string.Format("IIF({1}>0,({0}-{1})*100/{1},0)", thisYear, lastYear);
        }
        /// <summary>
        /// 计划完成率表达式
        /// </summary>
        /// <param name="thisYear"></param>
        /// <param name="lastYear"></param>
        /// <returns></returns>
        public static string GetColumnPlanCompletionRate(string actual, string plan)
        {
            return string.Format("IIF({1}>0,({0}/{1})*100,0)", actual, plan);
        }
        /// <summary>
        /// 便于统一改为不执行
        /// </summary>
        public static void GCCollect()
        {
            if (AllowGCCollect)
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// 清理对象
        /// </summary>
        public static void ClearDataTable(DataTable dt)
        {
            while (dt.Rows.Count > 0)
            {
                dt.Rows.RemoveAt(0);
            }
            dt.Clear();
            dt.Dispose();
            dt = null;
            GCCollect();
            //GC.Collect();
        }
        public static void ClearDictList<TKey, TVaue>(List<Dictionary<TKey, TVaue>> dictList)
        {
            foreach (var i in dictList)
            {
                i.Clear();
            }
            dictList.Clear();
            dictList = null;
            GCCollect();
            //GC.Collect();
        }
        public static void ClearArray(Array array)
        {
            array = null;
            GCCollect();
            //GC.Collect();
        }
        #endregion DataTable

        #region Tree
        public static void BuildTreeList(List<TreeListItem> list, List<TreeListItem> parent, Func<List<TreeListItem>, TreeListItem, List<TreeListItem>> childrenAction)
        {
            if (parent != null && parent.Count > 0)
            {
                parent.ForEach(p =>
                {
                    p.Children = childrenAction(list, p);
                    if (p.Children != null && p.Children.Count > 0)
                    {
                        BuildTreeList(list, p.Children, childrenAction);
                    }
                });
            }
        }
        #endregion Tree

        #region List

        public delegate T2 SelectListInvoker<T, T2>(T x);
        /// <summary>
        /// net2.0版本可使用的select方法
        /// </summary>
        public static List<T2> SelectList<T, T2>(List<T> source, SelectListInvoker<T, T2> action)
        {
            var result = new List<T2>();
            source.ForEach(a =>
            {
                result.Add(action(a));
            });
            return result;
        }
        /// <summary>
        /// 注意,这个方法会改变第一个list的值(为了效率)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static List<T> MergeList<T>(params List<T>[] lists)
        {
            List<T> result = null;
            foreach (var i in lists)
            {
                if (i == null) { continue; }
                if (result == null)
                {
                    result = i;
                }
                else
                {
                    result.AddRange(i);
                }
            }
            return result;
        }

        [Obsolete("这样封装用起来还是很麻烦,而且不灵活,还是不用了吧")]
        public static List<TReturn> ListFullJoin<TLeft, TRight, TKey, TReturn>(List<TLeft> left, List<TRight> right,
            Func<TLeft, TKey> lKeyAction, Func<TRight, TKey> rKeyAction, Func<TReturn, TKey> lrKeyAction,
            Func<TLeft, TRight> rEmptyAction,
            Func<TLeft, TRight, TReturn> leftSelectAction, Func<TRight, TReturn> rightSelectAction)
        {
            var leftData = (from first in left
                            join last in right on lKeyAction(first) equals rKeyAction(last) into temp  //last有可能空
                            from last in temp.DefaultIfEmpty(rEmptyAction(first))  //或 from item in billTypeList.DefaultIfEmpty() //这行的last和第一行的first可在select时取到值
                            select leftSelectAction(first, last));
            var rightRemainingData = (from r in right
                                      where !(from a in leftData select lrKeyAction(a)).Contains(rKeyAction(r))
                                      select rightSelectAction(r));
            var fullOuterjoinData = leftData.Concat(rightRemainingData).ToList();
            return fullOuterjoinData;
        }
        #endregion

        #region ReflectionHelper
        public static bool IsNullable(PropertyInfo property)
        {
            return IsNullable(property.PropertyType);
            //if (property.PropertyType.IsGenericType &&
            //    property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            //    return true;

            //return false;
        }
        public static bool IsNullable(Type t)
        {
            if (t.IsGenericType &&
                t.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;

            return false;
        }


        /// <summary>
        /// Includes a work around for getting the actual type of a Nullable type.(非空类型)
        /// </summary>
        public static Type GetPropertyType(PropertyInfo property)
        {
            if (IsNullable(property))
                return property.PropertyType.GetGenericArguments()[0];

            return property.PropertyType;
        }
        /// <summary>
        /// Includes a work around for getting the actual type of a Nullable type.(非空类型)
        /// </summary>
        public static Type GetNonnullType(Type t)
        {
            if (IsNullable(t))
                return t.GetGenericArguments()[0];

            return t;
        }
        #endregion

        #region Linq

        //public static Expression<Func<T, T1>> FuncToExpression<T, T1>(Func<T, T1> call)
        //{
        //    MethodCallExpression methodCall = call.Target == null
        //        ? Expression.Call(call.Method, Expression.Parameter(typeof(T)))
        //        : Expression.Call(Expression.Constant(call.Target), call.Method, Expression.Parameter(typeof(T)));

        //    return Expression.Lambda<Func<T, T1>>(methodCall);
        //}

        #endregion Linq

        public static IPFConfigMapper GetConfigMapper()
        {
            if (_configMapper == null)
            {
                #region 这样的效率似乎很低
                //var startTime = DateTime.Now;
                //var exAssems = new string[] { //.GetTypes会报错的程序集
                //    "Microsoft.Practices.EnterpriseLibrary.PolicyInjection, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null" ,
                //    "Microsoft.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null"
                //};
                //var assem = AppDomain.CurrentDomain.GetAssemblies()
                //                     .Where(a => !exAssems.Contains(a.FullName));
                //var mappers = new List<Type>();
                //foreach (var i in assem)
                //{
                //    try
                //    {
                //        mappers.AddRange(i.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPFConfigMapper))));
                //    }
                //    catch// (Exception e)
                //    {
                //        //var aa = "";
                //        //exTypes1.Add(i.FullName);
                //    }
                //}
                //if (!mappers.Any()) { throw new Exception("必需实现接口IPFConfigMapper"); }
                //_configMapper = ((IPFConfigMapper)Activator.CreateInstance(mappers.First())); 
                #endregion

                //_configMapper = new PFConfigMapper();
                //var endTime = DateTime.Now;
                //#if DEBUG
                //                var timeRange = endTime - startTime;
                //                if (timeRange.TotalSeconds > 3)
                //                {
                //                    throw new Exception(string.Format("反射IPFConfigMapper的时间为[{0}秒],太慢了", timeRange.TotalSeconds));
                //                }
                //#endif
            }
            return _configMapper;
        }
        public static void SetConfigMapper(IPFConfigMapper configMapper)
        {
            _configMapper = configMapper;
        }


        public static PFModelConfigCollection MergeModelConfig(params PFModelConfigCollection[] configs)
        {
            var m = new PFModelConfigCollection();
            var idx = 0;
            foreach (var i in configs)
            {
                if (i != null)
                {
                    if (idx == 0)
                    {
                        m = i;
                    }
                    else
                    {
                        var other = i;
                        foreach (var o in other)
                        {
                            if (!m.ContainsKey(o.Key))
                            {
                                m.Add(o.Key, o.Value);
                            }
                        }
                    }
                    idx++;
                }
            }
            return m;

        }
        /// <summary>
        /// 获得多个Model的类型配置
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static PFModelConfigCollection GetMultiModelConfig(string names)
        {
            var dataSet = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var m = new PFModelConfigCollection();
            var arr = new List<PFModelConfigCollection>();
            foreach (var i in dataSet)
            {
                arr.Add(PFDataHelper.GetModelConfig(i, null));
            }
            return MergeModelConfig(arr.ToArray());
            //var idx = 0;
            //foreach (var i in dataSet)
            //{
            //    if (idx == 0)
            //    {
            //        m = PFDataHelper.GetModelConfig(i, null);
            //    }
            //    else
            //    {
            //        var other = PFDataHelper.GetModelConfig(i, null);
            //        foreach (var o in other)
            //        {
            //            if (!m.ContainsKey(o.Key))
            //            {
            //                m.Add(o.Key, o.Value);
            //            }
            //        }
            //    }
            //    idx++;
            //}
            //return m;
        }
        /// <summary>
        /// 获得Model的类型配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static PFModelConfigCollection GetModelConfig(string name, string fullname)
        {
            var configMapper = GetConfigMapper();
            var mapper = configMapper.GetModelConfigMapper()
                .FirstOrDefault(a => a.ModelName == fullname || a.ModelName == name);//当Model名有重复但命名空间不一样时,这样找是有风险的,定义mapper时最好把fullName的项放在前面

            if (mapper == null) { return null; }

            var pathConfig = configMapper.GetPathConfig();
            //string xmlfile = Path.Combine(HttpRuntime.AppDomainAppPath, pathConfig.ConfigPath + "\\FieldSets.xml");
            string xmlfile = Path.Combine(BaseDirectory, pathConfig.ConfigPath + "\\FieldSets.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfile);

            var dataSets = doc.ChildNodes[1];

            var result = new PFModelConfigCollection();

            XmlNode dataSet = dataSets.SelectSingleNode(string.Format("DataSet[@name='{0}']", mapper.XmlDataSetName));
            if (dataSet != null)
            {
                foreach (XmlNode i in dataSet.SelectSingleNode("Fields").ChildNodes)
                {
                    var config = new PFModelConfig(i, mapper.XmlDataSetName);
                    //if (!result.ContainsKey(config.FieldName))
                    //{
                    //    result.Add(config.FieldName, config);
                    //}
                    if (!result.ContainsKey(config.LowerFieldName))
                    {
                        result.Add(config.LowerFieldName, config);
                    }
                }

            }
            //特殊配置的属性
            if (mapper.OtherXmlDataSetName != null && mapper.OtherXmlDataSetName.Any())
            {
                mapper.OtherXmlDataSetName.ForEach(a =>
                {
                    var otherDataSet = dataSets
                        .SelectSingleNode(string.Format("DataSet[@name='{0}']", a));

                    foreach (XmlNode i in otherDataSet.SelectSingleNode("Fields").ChildNodes)
                    {
                        var fieldName = i.SelectSingleNode("FieldName");
                        //if (!result.Any(b => b.PropertyName == i.SelectSingleNode("FieldName").InnerText))
                        if (!result.ContainsKey(fieldName.InnerText))
                        {
                            result.Add(fieldName.InnerText, new PFModelConfig(i, a));
                        }
                    }

                });
            }
            ////特殊配置的属性
            //if (mapper.ExProperty != null)
            //{
            //    mapper.ExProperty.ForEach(a =>
            //    {
            //        var old = result.FirstOrDefault(b => b.PropertyName == a.PropertyName);
            //        if (old != null) { result.Remove(old); }
            //        var exDataSet = dataSets
            //            .SelectSingleNode(string.Format("DataSet[@name='{0}']", a.XmlDataSetName));
            //        foreach (XmlNode i in exDataSet.SelectSingleNode("Fields").ChildNodes)
            //        {
            //            if (i.SelectSingleNode("FieldName").InnerText == a.XmlFieldName)
            //            {
            //                result.Add(new PFModelConfig(i, a.XmlDataSetName) { PropertyName = a.PropertyName });
            //            }
            //        }
            //        //result.Add(a);

            //    });
            //}
            return result;

        }

        public static PFModelConfigCollection GetModelConfig(Type modelType)
        {
            return PFDataHelper.GetModelConfig(modelType.Name, modelType.FullName);
        }
        public static Type GetTypeByString(string type)
        {
            switch (type.ToLower())
            {
                case "bool":
                    return Type.GetType("System.Boolean", true, true);
                case "byte":
                    return Type.GetType("System.Byte", true, true);
                case "sbyte":
                    return Type.GetType("System.SByte", true, true);
                case "char":
                    return Type.GetType("System.Char", true, true);
                case "decimal":
                    return Type.GetType("System.Decimal", true, true);
                case "double":
                    return Type.GetType("System.Double", true, true);
                case "float":
                    return Type.GetType("System.Single", true, true);
                case "int":
                    return Type.GetType("System.Int32", true, true);
                case "uint":
                    return Type.GetType("System.UInt32", true, true);
                case "long":
                    return Type.GetType("System.Int64", true, true);
                case "ulong":
                    return Type.GetType("System.UInt64", true, true);
                case "object":
                    return Type.GetType("System.Object", true, true);
                case "short":
                    return Type.GetType("System.Int16", true, true);
                case "ushort":
                    return Type.GetType("System.UInt16", true, true);
                case "string":
                    return Type.GetType("System.String", true, true);
                case "datetime":
                    return Type.GetType("System.DateTime", true, true);
                case "guid":
                    return Type.GetType("System.Guid", true, true);
                case "percent":
                    return typeof(PFPercent);
                case "date":
                    return typeof(PFDate);
                default:
                    return Type.GetType(type, true, true);
            }
        }

        public static string GetStringByType(Type type)
        {
            switch (type.ToString())
            {
                case "System.Boolean":
                    return "bool";
                case "System.Byte":
                    return "byte";
                case "System.SByte":
                    return "sbyte";
                case "System.Char":
                    return "char";
                case "System.Decimal":
                    return "decimal";
                case "System.Double":
                    return "double";
                case "System.Single":
                    return "float";
                case "System.Int32":
                    return "int";
                case "System.UInt32":
                    return "uint";
                case "System.Int64":
                    return "long";
                case "System.UInt64":
                    return "ulong";
                case "System.Object":
                    return "object";
                case "System.Int16":
                    return "short";
                case "System.UInt16":
                    return "ushort";
                case "System.String":
                    return "string";
                case "System.DateTime":
                    //return "datetime";
                    return "DateTime";//转string后必需和FieldSet.xml上的对应
                case "System.Guid":
                    return "guid";
                case "Perfect.PFPercent":
                    return "percent";
                case "Perfect.PFDate":
                    return "date";
                default:
                    return type.ToString();
            }
        }

        #region 时间
        public static DateTime GetYearStart(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }
        public static DateTime GetYearEnd(this DateTime date)
        {
            return GetYearStart(date.AddYears(1)).AddSeconds(-1);
        }
        public static DateTime GetMonthStart(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
        public static DateTime GetMonthEnd(this DateTime date)
        {
            return GetMonthStart(date.AddMonths(1)).AddSeconds(-1);
        }
        public static DateTime GetDayStart(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
        public static DateTime GetDayEnd(this DateTime date)
        {
            return GetDayStart(date.AddDays(1)).AddSeconds(-1);
        }
        public static long CountTime(Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
        /// <summary>
        /// Data.getTime()相减之后的值转换为容易处理的对象
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static PFTimeSpan GetTimeSpan(long ElapsedMilliseconds, PFYmd ymd = PFYmd.Hour | PFYmd.Minute | PFYmd.Second)
        {
            var after = ElapsedMilliseconds / 1000;//这里得到是秒
            var result = new PFTimeSpan(ymd);
            if (EnumHasFlag(ymd, PFYmd.Hour))
            {
                result.Hour = ObjectToInt(after / (60 * 60)) ?? 0;//计算整数小时数
                after -= (result.Hour * 60 * 60);//取得算出小时数后剩余的秒数
            };
            if (EnumHasFlag(ymd, PFYmd.Minute))
            {
                result.Minute = ObjectToInt(after / 60) ?? 0;//计算整数小时数
                after -= (result.Minute * 60);//取得算出小时数后剩余的秒数
            };
            if (EnumHasFlag(ymd, PFYmd.Second))
            {
                result.Second = ObjectToInt(after) ?? 0;//计算整数秒数
                after -= result.Second;//取得算出秒数后剩余的毫秒数
            };
            if (EnumHasFlag(ymd, PFYmd.Millisecond))
            {
                result.Millisecond = ObjectToInt(after) ?? 0;
            };
            return result;
        }
        public static PFTimeSpan GetTimeSpan(TimeSpan ts, PFYmd ymd = PFYmd.Hour | PFYmd.Minute | PFYmd.Second)
        {
            return GetTimeSpan(ObjectToLong(ts.TotalMilliseconds) ?? 0, ymd);
        }
        #endregion 时间
        //public static string GetEnMonthByNum(int num)
        //{
        //    switch (num)
        //    {
        //        case 1:
        //            return "Jan";
        //      case 2:
        //            return "Feb";
        //        case 3:
        //            return "Mar";
        //        case 4:
        //            return "Apr";
        //        case 5:
        //            return "May";
        //        case 6:
        //            return "Jun";
        //        case 7:
        //            return "Jul";
        //        case 8:
        //            return "Aug";
        //        case 9:
        //            return "Sep";
        //        case 10:
        //            return "Oct";
        //        case 11:
        //            return "Nov";
        //        case 12:
        //            return "Dec";
        //        default:
        //            return "";
        //    }
        //}


        #region 文件
        /// <summary>
        /// method for getting a files MD5 hash, say for
        /// a checksum operation
        /// </summary>
        /// <param name="file">the file we want the has from</param>
        /// <returns></returns>
        public static string GetHashMD5(Stream stream)
        {
            //MD5 hash provider for computing the hash of the file
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            //calculate the files hash
            md5.ComputeHash(stream);

            //byte array of files hash
            byte[] hash = md5.Hash;

            //string builder to hold the results
            StringBuilder sb = new StringBuilder();

            //loop through each byte in the byte array
            foreach (byte b in hash)
            {
                //format each byte into the proper value and append
                //current value to return value
                sb.Append(string.Format("{0:X2}", b));
            }

            //return the MD5 hash of the file
            return sb.ToString();
        }
        /// <summary>
        /// 创建指定目录
        /// </summary>
        /// <param name="targetDir"></param>
        public static void CreateDirectory(string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(targetDir);
            if (!dir.Exists)
                dir.Create();
        }
        /// <summary>
        /// 删除指定目录
        /// </summary>
        /// <param name="targetDir"></param>
        public static void DeleteDirectory(string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(targetDir);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
        }
        public static void DeleteFile(string targetDir)
        {
            if (File.Exists(targetDir))
            {
                File.Delete(targetDir);
                //FileAttributes attr = File.GetAttributes(targetDir);
                //if (attr == FileAttributes.Directory)
                //{
                //    Directory.Delete(targetDir, true);
                //}
                //else
                //{
                //    File.Delete(targetDir);
                //}
            }
        }

        private static byte[] GetFileHash(FileStream file1)
        {
            //计算第一个文件的哈希值
            var hash = System.Security.Cryptography.HashAlgorithm.Create();
            byte[] hashByte_1 = hash.ComputeHash(file1);
            return hashByte_1;
        }
        /// <summary>
        /// 比较两个文件是不是完全一样
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool IsFileAllTheSame(FileStream file1, FileStream file2)
        {

            if (file1 != null && file2 != null)
            {
                //计算第一个文件的哈希值
                byte[] hashByte_1 = GetFileHash(file1);
                //计算第二个文件的哈希值
                byte[] hashByte_2 = GetFileHash(file2);
                //stream_2.Close();

                //比较两个哈希值
                if (BitConverter.ToString(hashByte_1) == BitConverter.ToString(hashByte_2))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="FileFullPath">下载文件下载的完整路径及名称</param>
        public static void DownLoadFile(string FileFullPath)
        {
            if (!string.IsNullOrEmpty(FileFullPath) && System.IO.File.Exists(FileFullPath))
            {
                FileInfo fi = new FileInfo(FileFullPath);//文件信息
                FileFullPath = HttpUtility.UrlEncode(FileFullPath); //对文件名编码
                FileFullPath = FileFullPath.Replace("+", "%20"); //解决空格被编码为"+"号的问题
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileFullPath);
                HttpContext.Current.Response.AppendHeader("content-length", fi.Length.ToString()); //文件长度
                int chunkSize = 102400;//缓存区大小,可根据服务器性能及网络情况进行修改
                byte[] buffer = new byte[chunkSize]; //缓存区
                using (FileStream fs = fi.Open(FileMode.Open))  //打开一个文件流
                {
                    while (fs.Position >= 0 && HttpContext.Current.Response.IsClientConnected) //如果没到文件尾并且客户在线
                    {
                        int tmp = fs.Read(buffer, 0, chunkSize);//读取一块文件
                        if (tmp <= 0) break; //tmp=0说明文件已经读取完毕,则跳出循环
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, tmp);//向客户端传送一块文件
                        HttpContext.Current.Response.Flush();//保证缓存全部送出
                        Thread.Sleep(10);//主线程休息一下,以释放CPU
                    }
                }
            }
        }
        #region 下载大文件 支持续传、速度限制
        public static bool DownloadFile(HttpContext context, string filePath, long speed)
        {
            string fileName = Path.GetFileName(filePath);
            Stream myFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return DownloadFile(context, myFile, fileName, speed);
        }
        /// <summary>
        /// 下载文件，支持大文件、续传、速度限制。支持续传的响应头Accept-Ranges、ETag，请求头Range 。
        /// Accept-Ranges：响应头，向客户端指明，此进程支持可恢复下载.实现后台智能传输服务（BITS），值为：bytes；
        /// ETag：响应头，用于对客户端的初始（200）响应，以及来自客户端的恢复请求，
        /// 必须为每个文件提供一个唯一的ETag值（可由文件名和文件最后被修改的日期组成），这使客户端软件能够验证它们已经下载的字节块是否仍然是最新的。
        /// Range：续传的起始位置，即已经下载到客户端的字节数，值如：bytes=1474560- 。
        /// 另外：UrlEncode编码后会把文件名中的空格转换中+（+转换为%2b），但是浏览器是不能理解加号为空格的，所以在浏览器下载得到的文件，空格就变成了加号；
        /// 解决办法：UrlEncode 之后, 将 "+" 替换成 "%20"，因为浏览器将%20转换为空格
        /// </summary>
        /// <param name="httpContext">当前请求的HttpContext</param>
        /// <param name="filePath">下载文件的物理路径，含路径、文件名</param>
        /// <param name="speed">下载速度：每秒允许下载的字节数</param>
        /// <returns>true下载成功，false下载失败</returns>
        public static bool DownloadFile(HttpContext context, Stream myFile, string fileName, long speed)
        {
            bool ret = true;
            try
            {
                #region 定义局部变量
                long startBytes = 0;
                int packSize = 1024 * 10; //分块读取，每块10K bytes

                //FileStream myFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                long fileLength = myFile.Length;

                int sleep = (int)Math.Ceiling(1000.0 * packSize / speed);//毫秒数：读取下一数据块的时间间隔
                //string lastUpdateTiemStr = File.GetLastWriteTimeUtc(filePath).ToString("r");
                //string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + lastUpdateTiemStr;//便于恢复下载时提取请求头;
                #endregion

                #region--验证：文件是否太大，是否是续传，且在上次被请求的日期之后是否被修改过--------------
                if (myFile.Length > Int32.MaxValue)
                {//-------文件太大了-------
                    context.Response.StatusCode = 413;//请求实体太大
                    return false;
                }

                //if (context.Request.Headers["If-Range"] != null)//对应响应头ETag：文件名+文件最后修改时间
                //{
                //    //----------上次被请求的日期之后被修改过--------------
                //    if (context.Request.Headers["If-Range"].Replace("\"", "") != eTag)
                //    {//文件修改过
                //        context.Response.StatusCode = 412;//预处理失败
                //        return false;
                //    }
                //}
                #endregion

                try
                {
                    #region -------添加重要响应头、解析请求头、相关验证-------------------
                    context.Response.Clear();
                    context.Response.Buffer = false;
                    context.Response.AddHeader("Content-MD5", GetHashMD5(myFile));//用于验证文件
                    context.Response.AddHeader("Accept-Ranges", "bytes");//重要：续传必须
                    //context.Response.AppendHeader("ETag", "\"" + eTag + "\"");//重要：续传必须
                    //context.Response.AppendHeader("Last-Modified", lastUpdateTiemStr);//把最后修改日期写入响应              
                    context.Response.ContentType = "application/octet-stream";//MIME类型：匹配任意文件类型
                    context.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace("+", "%20"));
                    context.Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    context.Response.AddHeader("Connection", "Keep-Alive");
                    context.Response.ContentEncoding = Encoding.UTF8;
                    if (context.Request.Headers["Range"] != null)
                    {
                        //------如果是续传请求，则获取续传的起始位置，即已经下载到客户端的字节数------
                        context.Response.StatusCode = 206;//重要：续传必须，表示局部范围响应。初始下载时默认为200
                        string[] range = context.Request.Headers["Range"].Split(new char[] { '=', '-' });//"bytes=1474560-"
                        startBytes = Convert.ToInt64(range[1]);//已经下载的字节数，即本次下载的开始位置
                        if (startBytes < 0 || startBytes >= fileLength)
                        {
                            //无效的起始位置
                            return false;
                        }
                    }
                    if (startBytes > 0)
                    {
                        //------如果是续传请求，告诉客户端本次的开始字节数，总长度，以便客户端将续传数据追加到startBytes位置后----------
                        context.Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }
                    #endregion


                    #region -------向客户端发送数据块-------------------
                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize);//分块下载，剩余部分可分成的块数
                    for (int i = 0; i < maxCount && context.Response.IsClientConnected; i++)
                    {//客户端中断连接，则暂停
                        context.Response.BinaryWrite(br.ReadBytes(packSize));
                        context.Response.Flush();
                        if (sleep > 1) Thread.Sleep(sleep);
                    }
                    #endregion
                }
                catch
                {
                    ret = false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        /// <summary>
        /// 下载文件到服务器(通常用于保存临时文件)
        /// </summary>
        /// <param name="FileFullPath">下载文件下载的完整路径及名称</param>
        public static bool DownLoadFileToService(string httpUrl, string savePath)
        {
            try
            {
                EnsureFilePath(savePath);

                //string extension = Path.GetExtension(httpUrl);
                //Random random = new Random();
                //string fileName = DateTime.Now.ToString("yyyyMMddHHmm") + Guid.NewGuid().ToString().Replace("-", "").Substring(9, 16) + random.Next(100000, 999999) + extension;
                //string saveFilePath = Path.Combine(basePath, fileName);
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(httpUrl, savePath);
                }
                if (File.Exists(savePath))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        //public static bool DownloadExcel(HttpContext context, Workbook workbook, string fileName, long speed)
        //{
        //    bool ret = true;
        //    try
        //    {
        //        #region 定义局部变量
        //        long startBytes = 0;
        //        int packSize = 1024 * 10; //分块读取，每块10K bytes

        //        var tmpFileName = Guid.NewGuid().ToString("N") + DateTime.Now.ToString("yyyyMMddHHmmss") + fileName;
        //        var path = Path.Combine(PFDataHelper.BaseDirectory, "TempFile", tmpFileName);
        //        var directoryName = Path.GetDirectoryName(path);
        //        PFDataHelper.CreateDirectory(directoryName);
        //        workbook.Save(path);//注意，当xlsx文件超过65535行时，如果调用workbook.SaveToStream方法，会使行数变少，所以暂时只想到先保存到服务器的办法

        //        var filePath = path;
        //        FileStream myFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //        BinaryReader br = new BinaryReader(myFile);
        //        long fileLength = myFile.Length;

        //        int sleep = (int)Math.Ceiling(1000.0 * packSize / speed);//毫秒数：读取下一数据块的时间间隔
        //        //string lastUpdateTiemStr = File.GetLastWriteTimeUtc(filePath).ToString("r");
        //        //string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + lastUpdateTiemStr;//便于恢复下载时提取请求头;
        //        #endregion

        //        #region--验证：文件是否太大，是否是续传，且在上次被请求的日期之后是否被修改过--------------
        //        if (myFile.Length > Int32.MaxValue)
        //        {//-------文件太大了-------
        //            context.Response.StatusCode = 413;//请求实体太大
        //            return false;
        //        }

        //        //if (context.Request.Headers["If-Range"] != null)//对应响应头ETag：文件名+文件最后修改时间
        //        //{
        //        //    //----------上次被请求的日期之后被修改过--------------
        //        //    if (context.Request.Headers["If-Range"].Replace("\"", "") != eTag)
        //        //    {//文件修改过
        //        //        context.Response.StatusCode = 412;//预处理失败
        //        //        return false;
        //        //    }
        //        //}
        //        #endregion

        //        try
        //        {
        //            #region -------添加重要响应头、解析请求头、相关验证-------------------
        //            context.Response.Clear();
        //            context.Response.Buffer = false;
        //            context.Response.AddHeader("Content-MD5", GetHashMD5(myFile));//用于验证文件
        //            context.Response.AddHeader("Accept-Ranges", "bytes");//重要：续传必须
        //            //context.Response.AppendHeader("ETag", "\"" + eTag + "\"");//重要：续传必须
        //            //context.Response.AppendHeader("Last-Modified", lastUpdateTiemStr);//把最后修改日期写入响应              
        //            context.Response.ContentType = "application/octet-stream";//MIME类型：匹配任意文件类型
        //            context.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace("+", "%20"));
        //            context.Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
        //            context.Response.AddHeader("Connection", "Keep-Alive");
        //            context.Response.ContentEncoding = Encoding.UTF8;
        //            if (context.Request.Headers["Range"] != null)
        //            {
        //                //------如果是续传请求，则获取续传的起始位置，即已经下载到客户端的字节数------
        //                context.Response.StatusCode = 206;//重要：续传必须，表示局部范围响应。初始下载时默认为200
        //                string[] range = context.Request.Headers["Range"].Split(new char[] { '=', '-' });//"bytes=1474560-"
        //                startBytes = Convert.ToInt64(range[1]);//已经下载的字节数，即本次下载的开始位置
        //                if (startBytes < 0 || startBytes >= fileLength)
        //                {
        //                    //无效的起始位置
        //                    return false;
        //                }
        //            }
        //            if (startBytes > 0)
        //            {
        //                //------如果是续传请求，告诉客户端本次的开始字节数，总长度，以便客户端将续传数据追加到startBytes位置后----------
        //                context.Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
        //            }
        //            #endregion


        //            #region -------向客户端发送数据块-------------------
        //            br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
        //            int maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize);//分块下载，剩余部分可分成的块数
        //            for (int i = 0; i < maxCount && context.Response.IsClientConnected; i++)
        //            {//客户端中断连接，则暂停
        //                context.Response.BinaryWrite(br.ReadBytes(packSize));
        //                context.Response.Flush();
        //                if (sleep > 1) Thread.Sleep(sleep);
        //            }
        //            #endregion
        //        }
        //        catch
        //        {
        //            ret = false;
        //        }
        //        finally
        //        {
        //            br.Close();
        //            myFile.Close();
        //            PFDataHelper.DeleteFile(path);
        //        }
        //    }
        //    catch
        //    {
        //        ret = false;
        //    }
        //    return ret;
        //}
        #endregion
        #endregion

        #region 反射
        public static Type GetGenericType(object list)
        {
            return list.GetType().GetGenericArguments()[0];
        }
        //public static bool IsDynamicType(Type type)
        //{
        //    return type.Equals(typeof(ExpandoObject)) || type.Equals(typeof(object));
        //}
        #region dynamic需要用到(Net4.0版本以上)
        //public static void EachListHeader(object list, Action<int, string, Type> handle)
        //{
        //    var index = 0;
        //    var dict = GetListProperties(list);
        //    foreach (var item in dict)
        //        handle(index++, item.Key, item.Value);
        //}
        //public static Dictionary<string, Type> GetListProperties(dynamic list)
        //{
        //    var type = GetGenericType(list);
        //    var names = new Dictionary<string, Type>();

        //    if (IsDynamicType(type))
        //    {
        //        if (list.Count > 0)
        //            foreach (var item in GetIDictionaryValues(list[0]))
        //                names.Add(item.Key, (item.Value ?? string.Empty).GetType());
        //    }
        //    else
        //    {
        //        foreach (var p in GetProperties(type))
        //            names.Add(p.Value.Name, p.Value.PropertyType);
        //    }

        //    return names;
        //}
        //public static void EachListRow(object list, Action<int, object> handle)
        //{
        //    var index = 0;
        //    IEnumerator enumerator = ((dynamic)list).GetEnumerator();
        //    while (enumerator.MoveNext())
        //        handle(index++, enumerator.Current);
        //}
        #endregion
        private static readonly PFThreadDictionary<Type, Dictionary<string, PropertyInfo>> _cachedProperties = new PFThreadDictionary<Type, Dictionary<string, PropertyInfo>>();
        public static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            var properties = _cachedProperties.GetOrAdd(type, BuildPropertyDictionary);

            return properties;
        }
        private static Dictionary<string, PropertyInfo> BuildPropertyDictionary(Type type)
        {
            var result = new Dictionary<string, PropertyInfo>();

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                result.Add(property.Name//.ToLower()//SqlUpdateCollection使用时，如果每次对比时都转小写是非常麻烦的--wxj20181121
                    , property);
            }
            return result;
        }
        //public static IDictionary<string, object> GetIDictionaryValues(object item)
        //{
        //    if (item is IDictionary<string, object>)
        //    {
        //        return item as IDictionary<string, object>;
        //    }
        //    if (IsDynamicType(item.GetType()))
        //        return item as IDictionary<string, object>;

        //    var expando = (IDictionary<string, object>)new ExpandoObject();
        //    var properties = GetProperties(item.GetType());
        //    foreach (var p in properties)
        //        expando.Add(p.Value.Name, p.Value.GetValue(item, null));
        //    return expando;
        //}
        //public static void EachObjectProperty(object row, Action<int, string, object> handle)
        //{
        //    var index = 0;
        //    var dict = GetIDictionaryValues(row);
        //    foreach (var item in dict)
        //        handle(index++, item.Key, item.Value);
        //}
        #endregion

        #region Object

        /// <summary>
        /// 转换为string类型 defult为string.Empty ,模板通用转换方法 对string的处理有bug, 所以用此方法独立处理
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToString(object obj)
        {
            if (obj == null || obj == DBNull.Value
                ) { return ""; }
            return obj.ToString();

            #region 慢,耗时:699毫秒,0秒,0分钟,listCount:200
            ////传入guid形式的会报错，所以扩展为 by wxj
            //Guid result = Guid.Empty;
            //try
            //{
            //    var g = new Guid(obj == null ? "" : obj.ToString());
            //    return ObjectToType<string>(g, obj.ToString());
            //    //if (Guid.TryParse(obj == null ? "" : obj.ToString(), out result))
            //    //{
            //    //    return ObjectToType<string>(obj, obj.ToString());
            //    //}
            //}
            //catch (Exception) { }
            //return ObjectToType<string>(obj, string.Empty); 
            #endregion
        }
        /// <summary>
        /// 转换object为 T 值   
        /// </summary>
        /// <typeparam name="T">T 类型</typeparam>
        /// <param name="obj">要被转换的值</param>
        /// <returns>T 类型值</returns>
        public static T ObjectToType<T>(object obj, T defaultValue)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            else if (obj is T)
            {
                return (T)obj;
            }
            else
            {
                try
                {
                    Type conversionType = typeof(T);
                    object obj2 = null;
                    if (conversionType.Equals(typeof(Guid)))
                        obj2 = new Guid(Convert.ToString(obj));
                    else
                        obj2 = Convert.ChangeType(obj, conversionType);
                    return (T)obj2;
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }
        }
        #endregion

        #region 编码解码
        //private static byte[] EncodeByte(string s,PFEncodeType encodeType) {

        //    if (encodeType == PFEncodeType.Base64)
        //    {
        //        return Convert.FromBase64String(s);
        //    }
        //    else if (encodeType == PFEncodeType.Bit8)
        //    {
        //        return Convert.FromBase64String(s);
        //        //v1 = System.Text.Encoding.Default.GetBytes(v);
        //    }
        //    return null;
        //}
        //private static string FromByte(byte[] b,PFEncodeType encodeType)
        //{            
        //    if (encodeType == PFEncodeType.UTF8)
        //    {
        //        return Encoding.UTF8.GetString(b);
        //    }
        //    else if (encodeType == PFEncodeType.GB18030)
        //    {
        //        return Encoding.Default.GetString(b);
        //    }
        //    return null;
        //}

        /// <summary>
        /// Decodes a QuotedPrintable encoded string
        /// </summary>
        /// <param name="_ToDecode">The encoded string to decode</param>
        /// <returns>Decoded string</returns>
        public static string DecodeQQ(string _ToDecode, string charsetType)
        {
            //remove soft-linebreaks first
            //_ToDecode = _ToDecode.Replace("=\r\n", "");

            char[] chars = _ToDecode.ToCharArray();

            byte[] bytes = new byte[chars.Length];

            int bytesCount = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                // if encoded character found decode it
                if (chars[i] == '=')
                {
                    bytes[bytesCount++] = System.Convert.ToByte(int.Parse(chars[i + 1].ToString() + chars[i + 2].ToString(), System.Globalization.NumberStyles.HexNumber));

                    i += 2;
                }
                else
                {
                    bytes[bytesCount++] = System.Convert.ToByte(chars[i]);
                }
            }
            //return System.Text.Encoding.Default.GetString(bytes, 0, bytesCount);
            return System.Text.Encoding.GetEncoding(charsetType).GetString(bytes, 0, bytesCount);
        }
        public static byte[] QuotedPrintableGetByte(string _ToDecode)
        {
            //remove soft-linebreaks first
            //_ToDecode = _ToDecode.Replace("=\r\n", "");

            char[] chars = _ToDecode.ToCharArray();

            byte[] bytes = new byte[chars.Length];

            int bytesCount = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                //if encoded character found decode it
                if (chars[i] == '=')
                {
                    bytes[bytesCount++] = System.Convert.ToByte(int.Parse(chars[i + 1].ToString() + chars[i + 2].ToString(), System.Globalization.NumberStyles.HexNumber));
                    i += 2;
                }
                //if (chars[i] == '=')
                //{
                //    //这样当等号结尾时会报错(perfect99Email_含附件里有这种情况)(有的工具把结尾的 = 解析为 % ,不知道对不对)--benjamin20200306
                //    try
                //    {
                //        bytes[bytesCount++] = System.Convert.ToByte(int.Parse(chars[i + 1].ToString() + chars[i + 2].ToString(), System.Globalization.NumberStyles.HexNumber));
                //        i += 2;
                //    }
                //    catch (Exception e)
                //    {
                //        bytes[bytesCount++] = System.Convert.ToByte(new char[] { '\r'});
                //    }
                //}
                else
                {
                    bytes[bytesCount++] = System.Convert.ToByte(chars[i]);
                }
            }

            //return System.Text.Encoding.Default.GetString(bytes, 0, bytesCount);
            //return System.Text.Encoding.GetEncoding(charsetType).GetString(bytes, 0, bytesCount);
            //return bytes;
            return bytes.Take(bytesCount).ToArray();
        }
        //public static string DecodeQP(string codeString)
        //{
        //    //编码的字符集
        //    //string mailEncoding = "GB2312";
        //    string mailEncoding = "GBK";

        //    StringBuilder strBud = new StringBuilder();
        //    for (int i = 0; i < codeString.Length; i++)
        //    {
        //        if (codeString[i] == '=')
        //        {
        //            if (Convert.ToInt32((codeString[i + 1] + codeString[i + 2]).ToString(), 16) < 127)
        //            {
        //                strBud.Append(
        //                Encoding.GetEncoding(mailEncoding).GetString(
        //                new byte[] { Convert.ToByte((codeString[i + 1] + codeString[i + 2]).ToString(), 16) }));

        //                i += 2;
        //                continue;
        //            }

        //            if (codeString[i + 3] == '=')
        //            {
        //                strBud.Append(
        //                Encoding.GetEncoding(mailEncoding).GetString(
        //                new byte[] { Convert.ToByte((codeString[i + 1].ToString() + codeString[i + 2].ToString()), 16),
        //         Convert.ToByte((codeString[i + 4].ToString() + codeString[i + 5].ToString()), 16) }));

        //                i += 5;
        //                continue;
        //            }
        //        }
        //        else
        //        {
        //            strBud.Append(codeString[i]);
        //        }
        //    }
        //    return strBud.ToString();
        //}

        //参考自:https://stackoverflow.com/questions/2226554/c-class-for-decoding-quoted-printable-encoding
        //但解这句报错:    =?GBK?Q?=5Ffrom126Mail=5F=BB=E1=D4=B1=D7=CA=C1=CF=B1=ED?=
        //建议使用:  QuotedPrintableGetByte()
        //public static string DecodeQuotedPrintables(string input, string charSet)
        //{
        //    if (string.IsNullOrEmpty(charSet))
        //    {
        //        var charSetOccurences = new Regex(@"=\?.*\?Q\?", RegexOptions.IgnoreCase);
        //        var charSetMatches = charSetOccurences.Matches(input);
        //        foreach (Match match in charSetMatches)
        //        {
        //            charSet = match.Groups[0].Value.Replace("=?", "").Replace("?Q?", "");
        //            input = input.Replace(match.Groups[0].Value, "").Replace("?=", "");
        //        }
        //    }

        //    Encoding enc = new ASCIIEncoding();
        //    if (!string.IsNullOrEmpty(charSet))
        //    {
        //        try
        //        {
        //            enc = Encoding.GetEncoding(charSet);
        //        }
        //        catch
        //        {
        //            enc = new ASCIIEncoding();
        //        }
        //    }

        //    //decode iso-8859-[0-9]
        //    var occurences = new Regex(@"=[0-9A-Z]{2}", RegexOptions.Multiline);
        //    var matches = occurences.Matches(input);
        //    foreach (Match match in matches)
        //    {
        //        try
        //        {
        //            byte[] b = new byte[] { byte.Parse(match.Groups[0].Value.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier) };
        //            char[] hexChar = enc.GetChars(b);
        //            input = input.Replace(match.Groups[0].Value, hexChar[0].ToString());
        //        }
        //        catch { }
        //    }

        //    //decode base64String (utf-8?B?)
        //    occurences = new Regex(@"\?utf-8\?B\?.*\?", RegexOptions.IgnoreCase);
        //    matches = occurences.Matches(input);
        //    foreach (Match match in matches)
        //    {
        //        byte[] b = Convert.FromBase64String(match.Groups[0].Value.Replace("?utf-8?B?", "").Replace("?UTF-8?B?", "").Replace("?", ""));
        //        string temp = Encoding.UTF8.GetString(b);
        //        input = input.Replace(match.Groups[0].Value, temp);
        //    }

        //    input = input.Replace("=\r\n", "");
        //    return input;
        //}

        /// <summary>
        /// 是方法Encoding.GetEncoding(xx)支持的编码名
        /// </summary>
        /// <param name="encodeType"></param>
        /// <returns></returns>
        private static bool IsCSharpSupportEncoding(PFEncodeType encodeType)
        {
            return !(new PFEncodeType[] { PFEncodeType.Base64 }).Contains(encodeType);
        }
        public static string Encode(string v, PFEncodeType byteEncodeType, PFEncodeType encodeType)
        {
            byte[] v1 = null;
            if (encodeType == PFEncodeType.UTF8)
            {
                v1 = Encoding.UTF8.GetBytes(v);
            }
            //else if (encodeType == PFEncodeType.GB18030)
            //{
            //    v1 = Encoding.Default.GetBytes(v);
            //}
            else if (IsCSharpSupportEncoding(encodeType))
            {
                var tmp = GetEncodingString(encodeType);
                if (tmp != null)
                {
                    v1 = Encoding.GetEncoding(tmp).GetBytes(v);
                }
            }

            if (v1 == null) { return null; }

            string v2 = "";
            if (byteEncodeType == PFEncodeType.Base64)
            {
                v2 = Convert.ToBase64String(v1);
            }
            else if (byteEncodeType == PFEncodeType.Bit8)
            {
                v2 = Convert.ToBase64String(v1);
                //v1 = System.Text.Encoding.Default.GetBytes(v);
            }
            return v2;
        }

        /// <summary>
        /// 注意:
        /// 1.
        /// =?GBK?Q?=5Ffrom126Mail=5F=BB=E1=D4=B1=D7=CA=C1=CF=B1=ED?=
        /// 上面这个标题传入时的参数分别为
        /// v:  =5Ffrom126Mail=5F=BB=E1=D4=B1=D7=CA=C1=CF=B1=ED
        /// byteEncodeType:  QuotedPrintable
        /// encodeType:  GBK
        /// 
        /// 2.有的QuotedPrintable编码会用=在行结尾表示换行,一定要去掉=再调用此方法,否则会报错
        /// </summary>
        /// <param name="v"></param>
        /// <param name="byteEncodeType"></param>
        /// <param name="encodeType"></param>
        /// <returns></returns>
        public static string Decode(string v, PFEncodeType byteEncodeType, PFEncodeType encodeType)
        {
            byte[] v1 = null;
            if (byteEncodeType == PFEncodeType.Base64)
            {
                v1 = Convert.FromBase64String(v);
            }
            else if (byteEncodeType == PFEncodeType.Bit8)
            {
                v1 = Convert.FromBase64String(v);
                //v1 = System.Text.Encoding.Default.GetBytes(v);
            }
            else if (byteEncodeType == PFEncodeType.QuotedPrintable)
            {
                v1 = QuotedPrintableGetByte(v);
                //v1 = System.Text.Encoding.Default.GetBytes(v);
            }

            if (v1 == null) { return null; }

            string v2 = "";
            if (encodeType == PFEncodeType.UTF8)
            {
                v2 = Encoding.UTF8.GetString(v1);
            }
            //else if (encodeType == PFEncodeType.GB18030)
            //{
            //    v2 = Encoding.GetEncoding("gb18030").GetString(v1);
            //}
            //else if (encodeType == PFEncodeType.GB2312)
            //{
            //    v2 = Encoding.GetEncoding("gb2312").GetString(v1);
            //}
            //else if (encodeType == PFEncodeType.GBK)
            //{
            //    v2 = Encoding.GetEncoding("GBK").GetString(v1);
            //}
            else if (IsCSharpSupportEncoding(encodeType))
            {
                var tmp = GetEncodingString(encodeType);
                if (tmp != null)
                {
                    v2 = Encoding.GetEncoding(tmp).GetString(v1);
                }
            }
            return v2;
        }
        private static Dictionary<PFEncodeType, List<string>> _encodingDict = new Dictionary<PFEncodeType, List<string>> {
            {PFEncodeType.Base64,new List<string> { "B" } },
            {PFEncodeType.QuotedPrintable,new List<string> { "Q" } },
            {PFEncodeType.GB18030,new List<string> { "gb18030" } },
            {PFEncodeType.GB2312,new List<string> { "gb2312", "GB2312" } },
            {PFEncodeType.UTF8,new List<string> { "utf-8" ,"UTF-8"} },
            {PFEncodeType.GBK,new List<string> { "GBK" } }
        };
        public static PFEncodeType GetEncoding(string s)
        {
            foreach (var i in _encodingDict)
            {
                foreach (var j in i.Value)
                {
                    if (j == s)
                    {
                        return i.Key;
                    }
                }
            }
            #region old
            //if (s == "B")
            //{
            //    return PFEncodeType.Base64;
            //}
            //else if (s == "Q")
            //{
            //    return PFEncodeType.QuotedPrintable;
            //}
            //else if (s == "gb18030")
            //{
            //    return PFEncodeType.GB18030;
            //}
            //else if (s == "gb2312")
            //{
            //    return PFEncodeType.GB2312;
            //}
            //else if (s == "utf-8" || s == "UTF-8")
            //{
            //    return PFEncodeType.UTF8;
            //}
            //else if (s == "GBK")
            //{
            //    return PFEncodeType.GBK;
            //} 
            #endregion
            return PFEncodeType.UTF8;
        }
        public static string GetEncodingString(PFEncodeType encoding)
        {
            if (_encodingDict.ContainsKey(encoding))
            {
                return _encodingDict[encoding].First();
            }
            return null;
            //return "utf-8";
        }
        /// <summary>
        /// 获取字符串编码之后的bytes数组
        /// </summary>
        /// <param name="codeType">编码类型名称</param>
        /// <param name="strCode">将被编码的字符串</param>
        /// <returns></returns>
        public static byte[] GetEncodeBeforeBuffer(string codeType, string strCode)
        {
            //根据编码类型构造该类型编码的编码器的实例
            Encoder encoder = Encoding.GetEncoding(codeType).GetEncoder();

            char[] chars = strCode.ToCharArray();
            //根据获取对字符进行编码所产生的字节数来创建一个byte数组
            byte[] bytes = new byte[encoder.GetByteCount(chars, 0, chars.Length, true)];

            //将字符写入到byte数组中
            encoder.GetBytes(chars, 0, chars.Length, bytes, 0, true);

            return bytes;
        }

        /// <summary>
        ///获取字符串解码之后的字符串
        /// </summary>
        /// <param name="codeType">编码格式</param>
        /// <param name="byteCode">编码的字节数组</param>
        /// <returns></returns>
        public static string GetDecodeBeforeText(string codeType, byte[] byteCode)
        {
            //根据编码类型构造该类型编码的解码器的实例
            Decoder decoder = Encoding.GetEncoding(codeType).GetDecoder();

            //计算对字节序列（从指定字节数组开始）进行解码所产生的字符数
            char[] chars = new char[decoder.GetCharCount(byteCode, 0, byteCode.Length, true)];

            //根据获取的解码所产生的字节数来创建一个char数组
            int charLen = decoder.GetChars(byteCode, 0, byteCode.Length, chars, 0);

            StringBuilder strResult = new StringBuilder();

            foreach (char c in chars)
            {
                strResult = strResult.Append(c.ToString());
            }
            return strResult.ToString();
        }
        #endregion

        #region url
        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryStringParams(string queryString)
        {
            return GetQueryStringParams(queryString, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="encoding"></param>
        /// <param name="isEncoded"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryStringParams(string queryString, Encoding encoding, bool isEncoded)
        {
            var qm = queryString.IndexOf('?');
            if (qm > -1)
            {
                queryString = queryString.Substring(qm + 1);
            }
            //queryString = queryString.Replace("?", "");
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(queryString))
            {
                int count = queryString.Length;
                for (int i = 0; i < count; i++)
                {
                    int startIndex = i;
                    int index = -1;
                    while (i < count)
                    {
                        char item = queryString[i];
                        if (item == '=')
                        {
                            if (index < 0)
                            {
                                index = i;
                            }
                        }
                        else if (item == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value = null;
                    if (index >= 0)
                    {
                        key = queryString.Substring(startIndex, index - startIndex);
                        value = queryString.Substring(index + 1, (i - index) - 1);
                    }
                    else
                    {
                        key = queryString.Substring(startIndex, i - startIndex);
                    }
                    if (isEncoded)
                    {
                        //result[MyUrlDeCode(key, encoding)] = MyUrlDeCode(value, encoding);//这种方法的话，如果前台是checkbox,会提交IsEffective=true&IsEffective=false,最终会丢了前面的--benjamin20200113
                        SetGetQueryStringParamsItem(ref result, MyUrlDeCode(key, encoding), MyUrlDeCode(value, encoding));
                    }
                    else
                    {
                        result[key] = value;
                    }
                    if ((i == (count - 1)) && (queryString[i] == '&'))
                    {
                        result[key] = string.Empty;
                    }
                }
            }
            return result;
        }
        private static void SetGetQueryStringParamsItem(ref NameValueCollection result, string key, string value)
        {
            if (result.AllKeys.Contains(key))
            {
                result[key] = result[key] + "," + value;
            }
            else
            {
                result[key] = value;
            }
        }

        /// <summary>
        /// 解码URL.
        /// </summary>
        /// <param name="encoding">null为自动选择编码</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                Encoding utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                     
                string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }
            return HttpUtility.UrlDecode(str, encoding);
        }
        #endregion url


        #region 浏览器

        //public static RequestHostInfo GetRequestHostInfo(HttpRequestBase request)
        //{
        //    var result = new RequestHostInfo
        //    {
        //        OSVersion = PFDataHelper.GetOSVersion(request),//ok
        //        Browser = PFDataHelper.GetBrowser(request)
        //        //,//,ok
        //        //IPAddress = PFDataHelper.GetIPAddress(request),
        //        ,//,ok
        //        IPAddress = request.UserHostAddress,

        //    };

        //    ////跨域访问时,这段代码报错:不知道这样的主机
        //    //string HostName = string.Empty;
        //    //string ip = string.Empty;
        //    //string ipv4 = String.Empty;

        //    //if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_VIA"]))
        //    //    ip = Convert.ToString(request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
        //    //if (string.IsNullOrEmpty(ip))
        //    //    ip = request.UserHostAddress;

        //    //// 利用 Dns.GetHostEntry 方法，由获取的 IPv6 位址反查 DNS 纪录，<br> // 再逐一判断何者为 IPv4 协议，即可转为 IPv4 位址。
        //    //foreach (IPAddress ipAddr in Dns.GetHostEntry(ip).AddressList)
        //    //{
        //    //    if (ipAddr.AddressFamily.ToString() == "InterNetwork")
        //    //    {
        //    //        ipv4 = ipAddr.ToString();
        //    //    }
        //    //}
        //    //result.HostName = Dns.GetHostEntry(ip).HostName;

        //    return result;
        //    //HostName = "主机名: " + Dns.GetHostEntry(ip).HostName + " IP: " + ipv4;
        //}
        #region 获取操作系统版本号

        ///// <summary> 
        ///// 获取操作系统版本号 
        ///// </summary> 
        ///// <returns></returns>

        //public static string GetOSVersion(HttpRequestBase request)
        //{
        //    //UserAgent 
        //    var userAgent = request.ServerVariables["HTTP_USER_AGENT"];

        //    var osVersion = "未知";
        //    if (userAgent.Contains("NT 10.0"))
        //    {
        //        osVersion = "Windows 10";
        //    }
        //    else if (userAgent.Contains("NT 6.3"))
        //    {
        //        osVersion = "Windows 8.1";
        //    }
        //    else if (userAgent.Contains("NT 6.2"))
        //    {
        //        osVersion = "Windows 8";
        //    }

        //    else if (userAgent.Contains("NT 6.1"))
        //    {
        //        osVersion = "Windows 7";
        //    }
        //    else if (userAgent.Contains("NT 6.0"))
        //    {
        //        osVersion = "Windows Vista/Server 2008";
        //    }
        //    else if (userAgent.Contains("NT 5.2"))
        //    {
        //        osVersion = "Windows Server 2003";
        //    }
        //    else if (userAgent.Contains("NT 5.1"))
        //    {
        //        osVersion = "Windows XP";
        //    }
        //    else if (userAgent.Contains("NT 5"))
        //    {
        //        osVersion = "Windows 2000";
        //    }
        //    else if (userAgent.Contains("NT 4"))
        //    {
        //        osVersion = "Windows NT4";
        //    }
        //    else if (userAgent.Contains("Me"))
        //    {
        //        osVersion = "Windows Me";
        //    }
        //    else if (userAgent.Contains("98"))
        //    {
        //        osVersion = "Windows 98";
        //    }
        //    else if (userAgent.Contains("95"))
        //    {
        //        osVersion = "Windows 95";
        //    }
        //    else if (userAgent.Contains("Mac"))
        //    {
        //        osVersion = "Mac";
        //    }
        //    else if (userAgent.Contains("Unix"))
        //    {
        //        osVersion = "UNIX";
        //    }
        //    else if (userAgent.Contains("Linux"))
        //    {
        //        osVersion = "Linux";
        //    }
        //    else if (userAgent.Contains("SunOS"))
        //    {
        //        osVersion = "SunOS";
        //    }
        //    return osVersion;
        //}
        #endregion
        #region 获取IP地址

        /////// <summary> 
        /////// 获取IP地址
        /////// </summary> 
        /////// <returns></returns>

        //public static string GetIPAddress(HttpRequestBase request)
        //{
        //    string ipv4 = String.Empty;
        //    foreach (IPAddress IPA in Dns.GetHostAddresses(request.UserHostAddress))
        //    {
        //        if (IPA.AddressFamily.ToString() == "InterNetwork")
        //        {
        //            ipv4 = IPA.ToString();
        //            break;
        //        }
        //    }
        //    if (ipv4 != String.Empty)
        //    {
        //        return ipv4;
        //    }
        //    foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
        //    {
        //        if (IPA.AddressFamily.ToString() == "InterNetwork")
        //        {
        //            ipv4 = IPA.ToString();
        //            break;
        //        }
        //    }
        //    return ipv4;
        //}

        #endregion
        #region 获取浏览器版本号

        ///// <summary> 
        ///// 获取浏览器版本号 
        ///// </summary> 
        ///// <returns></returns> 
        //public static string GetBrowser(HttpRequestBase request)
        //{
        //    HttpBrowserCapabilitiesBase bc = request.Browser;
        //    return bc.Browser + bc.Version;
        //}

        #endregion
        #endregion

        #region 文件操作

        ///// <summary>
        ///// 保存文件,如果目录不存在会生成目录
        ///// </summary>
        ///// <param name="FromStream"></param>
        ///// <param name="TargetFile"></param>
        //public static void SaveStreamToFile(Stream FromStream, string TargetFile)
        //{
        //    // FromStream=the stream we wanna save to a file 
        //    //TargetFile = name&path of file to be created to save to 
        //    //i.e"c:\mysong.mp3" 
        //    try
        //    {
        //        var pathWithoutFileName = TargetFile.Replace(Path.GetFileName(TargetFile), "");//如果目录不存在先生成目录,否则会报错--wxj20180713
        //        if (!Directory.Exists(pathWithoutFileName))
        //        {
        //            Directory.CreateDirectory(pathWithoutFileName);
        //        }
        //        //Creat a file to save to
        //        Stream ToStream = File.Create(TargetFile);

        //        //use the binary reader & writer because
        //        //they can work with all formats
        //        //i.e images, text files ,avi,mp3..
        //        BinaryReader br = new BinaryReader(FromStream);
        //        BinaryWriter bw = new BinaryWriter(ToStream);

        //        //copy data from the FromStream to the outStream
        //        //convert from long to int 
        //        bw.Write(br.ReadBytes((int)FromStream.Length));
        //        //save
        //        bw.Flush();
        //        //clean up 
        //        bw.Close();
        //        br.Close();
        //    }

        //    //use Exception e as it can handle any exception 
        //    catch (Exception e)
        //    {
        //        var aa = e;
        //        //code if u like 
        //    }
        //}

        private static void EnsureFilePath(string filePath)
        {
            var pathWithoutFileName = filePath.Replace(Path.GetFileName(filePath), "");//如果目录不存在先生成目录,否则会报错--wxj20180713
            if (!Directory.Exists(pathWithoutFileName))
            {
                Directory.CreateDirectory(pathWithoutFileName);
            }
        }

        /// <summary>
        /// 写字符串到txt(复盖)(多用于保存测试的字符)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="filePath"></param>
        public static void SaveStringToFile(string s, string filePath)
        {
            EnsureFilePath(filePath);
            //FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            //FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            var sw = new StreamWriter(fs);
            sw.Write(s);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void AppendLineToFile(string filePath, params string[] line)
        {
            EnsureFilePath(filePath);
            //FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            var sw = new StreamWriter(fs);
            foreach (var l in line)
            {
                sw.WriteLine(l);
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static string ReadFileToString(string filePath, Encoding encoding = null)
        {
            //EnsureFilePath(filePath);
            if (!File.Exists(filePath)) { return null; }
            string text = string.Empty;
            //System.Text.Encoding code = System.Text.Encoding.GetEncoding("gb2312");
            using (var sr = new StreamReader(filePath, encoding ?? _encoding))
            {
                try
                {
                    text = sr.ReadToEnd(); // 读取文件
                    sr.Close();
                }
                catch { }
            }
            return text;
        }
        public static List<string> ReadFileToCodes(Stream fileStream)
        {
            var hybhs = new List<string>();
            if (fileStream != null)
            {
                //StreamReader sreader = new System.IO.StreamReader(Request.Files[0].InputStream, System.Text.Encoding.GetEncoding("utf-8"));
                StreamReader sreader = new System.IO.StreamReader(fileStream, PFDataHelper._encoding);

                string hybh = string.Empty;
                string tel = string.Empty;
                while (!string.IsNullOrEmpty(hybh = sreader.ReadLine()))
                {
                    hybhs.Add(hybh);
                }
                sreader.Close();
            }
            if (hybhs.Count > 0 && hybhs[0].IndexOf(",") > -1)
            {//如果是一行用逗号分隔的方式
                var newHybhs = new List<string>();
                hybhs.ForEach(a =>
                {
                    newHybhs.AddRange(a.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                });
                hybhs = newHybhs;
                //hybhs = hybhs[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return hybhs.Distinct().ToList();
        }
        ///// <summary>
        ///// 把文件(txt)读为多个字符串，支持逗号分隔或者每行一个（建议每行一个）
        ///// </summary>
        ///// <param name="filePath"></param>
        ///// <returns></returns>
        //public static string[] ReadFileToStringArray(string filePath)
        //{
        //    string text = string.Empty;
        //    //System.Text.Encoding code = System.Text.Encoding.GetEncoding("gb2312");
        //    using (var sr = new StreamReader(filePath, _encoding))
        //    {
        //        try
        //        {
        //            text = sr.ReadToEnd(); // 读取文件
        //            sr.Close();
        //        }
        //        catch { }
        //    }
        //    return text;
        //}
        /// <summary>
        /// 代替4.0的多path重载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PathCombine(params string[] path)
        {
            if (path == null || path.Length < 1) { return null; }
            string r = path[0];
            for (int i = 1; i < path.Length; i++)
            {
                r = Path.Combine(r, path[i]);
            }
            return r;
        }
        public static string ReadLocalJson(string fileName)
        {
            string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Json", fileName);
            return ReadFileToString(filePath);
        }
        public static T ReadLocalJson<T>(string fileName)
        {
            string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Json", fileName);
            return JsonConvert.DeserializeObject<T>(ReadFileToString(filePath));
        }
        public static string ReadLocalTxt(string fileName)
        {
            string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Txt", fileName);
            return ReadFileToString(filePath);
        }
        public static string[] ReadLocalTxtLines(string fileName)
        {
            string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Txt", fileName);
            //string[] result = null;
            if (!File.Exists(filePath)) { return null; }
            //System.Text.Encoding code = System.Text.Encoding.GetEncoding("gb2312");
            List<string> resultList = new List<string>();
            using (var sr = new StreamReader(filePath, _encoding))
            {
                try
                {
                    if (sr.BaseStream != null)
                    {
                        //StreamReader sreader = new System.IO.StreamReader(Request.Files[0].InputStream, System.Text.Encoding.GetEncoding("utf-8"));
                        StreamReader sreader = new System.IO.StreamReader(sr.BaseStream, PFDataHelper._encoding);

                        string hybh = string.Empty;
                        string tel = string.Empty;
                        while (!string.IsNullOrEmpty(hybh = sreader.ReadLine()))
                        {
                            resultList.Add(hybh);
                        }
                        sreader.Close();
                    }
                    //result = ReadFileToCodes(sr.BaseStream).ToArray();
                    sr.Close();
                }
                catch (Exception e)
                {
                    WriteError(e);
                }
            }
            return resultList.ToArray();
        }
        public static T[] ReadLocalTxtLines<T>(string fileName)
        {
            var arr = ReadLocalTxtLines(fileName);
            if (arr == null) { return null; }
            var result = new List<T>();
            foreach (var i in arr)
            {
                try
                {
                    result.Add(JsonConvert.DeserializeObject<T>(i));
                }
                catch (Exception e)
                {

                }
            }
            return result.ToArray();
        }
        public static DataTable ReadLocalDataTable(string fileName)
        {
            var s = ReadLocalJson(fileName);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(s);
            return DictListToDataTable(list);
        }
        public static void WriteLocalTxt(string s, string fileName)
        {
            string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Txt", fileName);
            SaveStringToFile(s, filePath);
        }
        public static void WriteLocalJson(object obj, string fileName)
        {
            string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Json", fileName);
            SaveStringToFile(JsonConvert.SerializeObject(obj), filePath);
        }
        //public static void WriteLocalTxtLine(string s, string fileName)
        //{
        //    string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Txt", fileName);
        //    AppendLineToFile(s, filePath);
        //}
        public static void WriteLocalTxtLine<T>(string fileName, params T[] obj)
        {
            if (obj == null || !obj.Any()) { return; }
            string filePath = PathCombine(PFDataHelper.BaseDirectory, "LocalData", "Txt", fileName);
            AppendLineToFile(filePath, obj.Select(a => JsonConvert.SerializeObject(a)).ToArray());
        }

        //读写锁，当资源处于写入模式时，其他线程写入需要等待本次写入结束之后才能继续写入
        private static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        #region 多线程写日志有风险,暂改为单线程(如果用多线程,至少要加线程计数,如果超出10,就直接返回)
        //private static void DoWriteLog<TMsg>(TMsg e, string filePrev)
        //{//一定要对log目录设置everyone权限

        //    var t = new Thread(new ParameterizedThreadStart((a) =>
        //    {
        //        try
        //        {
        //            //    //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
        //            //    //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
        //            //    //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
        //            //    //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
        //            LogWriteLock.EnterWriteLock();
        //            var server = PFDataHelper.BaseDirectory;// AppDomain.CurrentDomain.BaseDirectory;
        //            var dirPath = Path.Combine(server, "log");
        //            var logPath = Path.Combine(dirPath, string.Format("{0}_{1}.txt", filePrev ?? "pfError", ObjectToDateString(DateTime.Now, "yyyyMMdd")));

        //            if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); }
        //            var sw = new StreamWriter(logPath, true);
        //            //sw.WriteLine(string.Format("message:[{0}],InnerException:[{1}],time:[{2}]", e.Message, e.InnerException, DateTime.Now));//开始写入值
        //            sw.WriteLine(string.Format("\r\ntime:[{0}]\r\n{1}\r\n", DateTime.Now, e));//开始写入值
        //            sw.Flush();
        //            sw.Close();
        //        }
        //        catch (Exception) { }
        //        finally
        //        {
        //            //    //退出写入模式，释放资源占用
        //            //    //注意：一次请求对应一次释放
        //            //    //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
        //            //    //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
        //            //LogWriteLock.ExitWriteLock();
        //            try//防止ExitWriteLock报错导致中断--benjamin20191204
        //            {
        //                LogWriteLock.ExitWriteLock();
        //            }
        //            catch (Exception) { }
        //        }
        //    }));
        //    t.Start();
        //} 
        #endregion
        private static void DoWriteLog<TMsg>(TMsg e, string filePrev)
        {//一定要对log目录设置everyone权限
            try
            {
                //    //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //    //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //    //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //    //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                LogWriteLock.EnterWriteLock();
                var server = PFDataHelper.BaseDirectory;// AppDomain.CurrentDomain.BaseDirectory;
                var dirPath = Path.Combine(server, "log");
                var logPath = Path.Combine(dirPath, string.Format("{0}_{1}.txt", filePrev ?? "pfError", ObjectToDateString(DateTime.Now, "yyyyMMdd")));

                if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); }
                var sw = new StreamWriter(logPath, true);
                //sw.WriteLine(string.Format("message:[{0}],InnerException:[{1}],time:[{2}]", e.Message, e.InnerException, DateTime.Now));//开始写入值
                sw.WriteLine(string.Format("\r\ntime:[{0}]\r\n{1}\r\n", DateTime.Now, e));//开始写入值
                sw.Flush();
                sw.Close();
            }
            catch (Exception) { }
            finally
            {
                //    //退出写入模式，释放资源占用
                //    //注意：一次请求对应一次释放
                //    //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
                //    //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
                //LogWriteLock.ExitWriteLock();
                try//防止ExitWriteLock报错导致中断--benjamin20191204
                {
                    LogWriteLock.ExitWriteLock();
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// e为空也不会报错
        /// </summary>
        /// <param name="e"></param>
        public static void WriteError(Exception e)
        {//一定要对log目录设置everyone权限
            DoWriteLog(e, "pfError");
        }
        public static void WriteLog(string msg)
        {//一定要对log目录设置everyone权限
            DoWriteLog(msg, "pfLog");
        }
        #endregion
        //public static object GetSystemUserData(string userId)
        //{

        //    if (PFDataHelper.StringIsNullOrWhiteSpace(userId)) { return null; }
        //    return Caching.Get(userId);
        //}
        #region old
        ///// <summary>
        ///// 获得功能权限表(如功能A里面有Insert等按钮的功能)--wxj20180907
        ///// </summary>
        ///// <param name="funcNo"></param>
        ///// <param name="purviewIds"></param>
        ///// <returns></returns>
        //public static Dictionary<string, FuncAuthority> GetFuncAuthority(List<string> authorites)
        //{
        //    //var purviewIdsArr = GetUserActions().Actions;
        //    var purviewIdsArr = authorites;
        //    //var purviewIdsArr = purviewIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    var result = new Dictionary<string, FuncAuthority>();
        //    var authType = typeof(FuncAuthority);
        //    var authorities = Enum.GetNames(authType);
        //    foreach (var i in purviewIdsArr)
        //    {
        //        var code = i;
        //        //if (code[0] == '\'') { code = code.Substring(1, code.Length - 1); }
        //        //if (code[code.Length - 1] == '\'') { code = code.Substring(0, code.Length - 1); }

        //        //var code = i.Number;
        //        var authority = GetAuthorityByFuncCode(code, authorities);
        //        if (authority != null)
        //        {
        //            var pureCode = code.Substring(0, code.Length - authority.Length - 1);//去掉功能码后的纯编码
        //            var v = (FuncAuthority)Enum.Parse(authType, authority);
        //            if (!result.ContainsKey(pureCode))
        //            {
        //                result.Add(pureCode, v);
        //            }
        //            else
        //            {
        //                //result[pureCode] = result[pureCode] & (~FuncAuthority.Default);//去掉Default项(如果存在).Default改用0值后,经过|运算会自动去掉
        //                result[pureCode] |= v;
        //            }
        //        }
        //        else if (!result.ContainsKey(code))
        //        {
        //            result.Add(code, FuncAuthority.Default);
        //        }
        //    }
        //    return result;
        //}
        #endregion

        /// <summary>
        /// 获得功能权限表(如功能A里面有Insert等按钮的功能)--wxj20180907
        /// </summary>
        /// <param name="funcNo"></param>
        /// <param name="purviewIds"></param>
        /// <returns></returns>
        public static Dictionary<string, FuncAuthority> GetFuncAuthority(List<string> purviewIdsArr, out Dictionary<string, List<string>> otherAuthorities)
        {
            otherAuthorities = new Dictionary<string, List<string>>();

            var result = new Dictionary<string, FuncAuthority>();
            var authType = typeof(FuncAuthority);
            foreach (var i in purviewIdsArr)
            {
                var code = i;
                var m = Regex.Match(code, "\\.[^\\._]+$");
                if (m.Success)
                {
                    var authority = m.Value.Replace(".", "");
                    var pureCode = code.Substring(0, code.Length - authority.Length - 1);//去掉功能码后的纯编码

                    FuncAuthority? v = FuncAuthority.Default;
                    v = PFDataHelper.ObjectToEnum<FuncAuthority>(authority);
                    if (v != null)
                    {
                        if (!result.ContainsKey(pureCode))
                        {
                            result.Add(pureCode, v.Value);
                        }
                        else
                        {
                            result[pureCode] |= v.Value;
                        }
                    }
                    else
                    {
                        if (!otherAuthorities.ContainsKey(pureCode))
                        {
                            otherAuthorities[pureCode] = new List<string>();
                        }
                        otherAuthorities[pureCode].Add(authority);
                    }
                    //if (Enum.TryParse<FuncAuthority>(authority, out v))
                    //{
                    //    if (!result.ContainsKey(pureCode))
                    //    {
                    //        result.Add(pureCode, v);
                    //    }
                    //    else
                    //    {
                    //        result[pureCode] |= v;
                    //    }
                    //}
                    //else
                    //{
                    //    if (!otherAuthorities.ContainsKey(pureCode))
                    //    {
                    //        otherAuthorities[pureCode] = new List<string>();
                    //    }
                    //    otherAuthorities[pureCode].Add(authority);
                    //}

                }
                else if (!result.ContainsKey(code))
                {
                    result.Add(code, FuncAuthority.Default);
                }
            }
            return result;
        }
        /// <summary>
        /// 根据功能码(如RiskManage.FXSJK.Add)找功能的权限码(如Add)
        /// </summary>
        /// <param name="funcCode"></param>
        /// <param name="authorities"></param>
        /// <returns></returns>
        private static string GetAuthorityByFuncCode(string funcCode, string[] authorities)
        {
            foreach (var i in authorities)
            {

                var idx = funcCode.IndexOf(i);

                if (funcCode.Length > i.Length//有这种特殊情况
                    && idx == funcCode.Length - i.Length
                    && funcCode[idx - 1] == '.'//排除特殊情况"AgentManage.Agent__DataDelete"
                    )
                {
                    return i;
                }

            }
            return null;
        }

        #region old,20190617备份
        //public static string HttpPost(string url, string body, Action<HttpWebRequest> requestAction = null)
        //{
        //    //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        //    Encoding encoding = Encoding.UTF8;
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Method = "POST";
        //    request.Accept = "text/html, application/xhtml+xml, */*";
        //    request.ContentType = "application/json";
        //    if (requestAction != null) { requestAction(request); }

        //    if (!PFDataHelper.StringIsNullOrWhiteSpace(body))
        //    {
        //        byte[] buffer = encoding.GetBytes(body);
        //        request.ContentLength = buffer.Length;
        //        request.GetRequestStream().Write(buffer, 0, buffer.Length);
        //    }
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        //    {
        //        var r = reader.ReadToEnd();
        //        if (response != null)
        //        {
        //            response.Close();
        //        }
        //        if (request != null)
        //        {
        //            request.Abort();
        //        }
        //        return r;
        //    }
        //} 
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body">body = JsonConvert.SerializeObject(new CrmHyzl(a, model.type));</param>
        /// <param name="requestAction"></param>
        /// <param name="keepAlive"></param>
        /// <param name="cookie">当多次post时,keepAlive应为true,且给cookie的值(测试知java开源的springCloud转发方式用此方式很慢,还会无影响)</param>
        /// <returns></returns>
        public static string HttpPost(string url, string body, Action<HttpPostOption> postOptionAction = null)//, Action<HttpWebRequest> requestAction = null, bool keepAlive = true, CookieContainer cookie = null)
        {
            var postOption = new HttpPostOption();
            if (postOptionAction != null) { postOptionAction(postOption); }

            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Accept = "text/html, application/xhtml+xml, */*";
            //request.ContentType = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentType = "application/json;charset=UTF-8";
            //if (requestAction != null) { requestAction(request); }
            if (postOption != null)
            {
                request.KeepAlive = postOption.KeepAlive;
                if (postOption.Cookie != null)
                {
                    request.CookieContainer = postOption.Cookie;
                }
                if (postOption.Header != null)
                {
                    foreach (var head in postOption.Header)
                    {
                        request.Headers.Add(head.Key, head.Value);
                    }
                }
            }
            if (!PFDataHelper.StringIsNullOrWhiteSpace(body))
            {
                byte[] buffer = encoding.GetBytes(body);
                request.ContentLength = buffer.Length;//报错:基础连接已经关闭: 连接被意外关闭。(好像这个错误不是这句引起的)
                Stream newStream = request.GetRequestStream();
                newStream.Write(buffer, 0, buffer.Length);
                newStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (postOption != null && postOption.Cookie != null) { response.Cookies = postOption.Cookie.GetCookies(response.ResponseUri); }
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                var r = reader.ReadToEnd();
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                if (postOption != null && (!postOption.KeepAlive))
                {
                    GCCollect();
                    //System.GC.Collect();
                }
                return r;
            }
        }
        public static string HttpGet(string url)
        {
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// jquery不能用作data的特殊字符
        /// </summary>
        private static List<KeyValuePair<string, string>> _specDataChar = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(".","_"),
                new KeyValuePair<string, string>("[","_lz_"),
                new KeyValuePair<string, string>("]","_rz_")
            };
        private static string EncodeDataChar(string s)
        {
            _specDataChar.ForEach(a =>
            {
                s = s.Replace(a.Key, a.Value);
            });
            return s;
        }
        private static string DecodeDataChar(string s)
        {
            _specDataChar.ForEach(a =>
            {
                s = s.Replace(a.Value, a.Key);
            });
            return s;
        }
        private static object GetColumnSummary(DataTable dt, string columnName, SummaryType summaryType)
        {
            switch (summaryType)
            {
                case SummaryType.Average:
                    //return dt.Rows.Count < 1 ? 0 : (PFDataHelper.ColumnTotal(dt, columnName) / dt.Rows.Count);
                    return dt.Rows.Count < 1 ? 0 : Math.Round((PFDataHelper.ColumnTotal(dt, columnName) / dt.Rows.Count), DecimalPrecision);
                default:
                    return PFDataHelper.ColumnTotal(dt, columnName);
            }
        }

        /// <summary>
        /// var random = new Random();
        /// </summary>
        /// <param name="random"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomNo(Random random, int length)
        {
            var r = "";
            for (int i = 0; i < length; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }

        public static PagingResult PagingStore(DataTable dataTable, PagingParameters p, StoreColumnCollection header = null, bool setWidthByHeaderWord = true, string xmlDataSetName = null)
        {
            if (dataTable == null) { return null; }

            if (!p.PageSize.HasValue) { p.PageSize = dataTable.Rows.Count; }
            var all = dataTable;
            var columns = dataTable.Columns;

            List<string> srcColumnNames = new List<string>();//自定义group之后，目标head中可能有不存在的列--ben20190627
            foreach (DataColumn dataColumn in columns)
            {
                srcColumnNames.Add(dataColumn.ColumnName);
            }

            bool isPageSql = all.Rows.Count > 0 && all.Columns["rowtotal"] != null;
            int total = 0;
            DataTable dt = null;
            if (isPageSql)
            {
                total = (int)all.Rows[0]["rowtotal"];
                dt = all;
            }
            else
            {
                if (p.OnlyNeedOneRow != true)
                {
                    if (!PFDataHelper.StringIsNullOrWhiteSpace(p.Sort))
                    {
                        //all.DefaultView.Sort = p.Sort.Replace(sc[0].Value, sc[0].Key);
                        all.DefaultView.Sort = DecodeDataChar(p.Sort);
                        all = all.DefaultView.ToTable();
                    }
                    if (!PFDataHelper.StringIsNullOrWhiteSpace(p.FilterValue))
                    {
                        all = all.DataTableFilter(p.FilterValue);
                    }
                    total = all.Rows.Count;//一定要过滤完再计算total
                    dt = all.DataPager(p.PageIndex.Value, p.PageSize.Value);//注意分页后的columns丢了ExtendedProperties
                }
                else
                {
                    dt = all.DataPager(p.PageIndex.Value, 1);
                }
            }
            //int total = all.Rows.Count>0&&all.Rows[0]["rowtotal"]!=null? (int)all.Rows[0]["rowtotal"] : all.Rows.Count;
            if (dt != null)
            {
                ArrayList arrayList = new ArrayList();

                foreach (DataRow dataRow in dt.Rows)
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    if (header == null)
                    {
                        foreach (DataColumn dataColumn in columns)
                        {
                            ////dictionary.Add(dataColumn.ColumnName.IndexOf(".") > -1 ? dataColumn.ColumnName.Replace(".", "_") : dataColumn.ColumnName
                            ////    , dataRow[dataColumn.ColumnName].ToString());
                            //dictionary.Add(EncodeDataChar(dataColumn.ColumnName)
                            //    , dataRow[dataColumn.ColumnName].ToString());
                            dictionary.Add(EncodeDataChar(dataColumn.ColumnName)
                                , dataRow[dataColumn.ColumnName]);
                        }
                    }
                    else
                    {
                        (new StoreColumn { Children = header }).EachLeaf(a =>
                        {
                            if (srcColumnNames.Contains(a.data))
                            {
                                dictionary.Add(EncodeDataChar(a.data)
            , dataRow[a.data]);
                            }
                            else
                            {
                                dictionary.Add(EncodeDataChar(a.data)
                , null);
                                a.visible = false;
                            }
                        });
                    }
                    arrayList.Add(dictionary);

                }

                if (header == null)
                {
                    header = new StoreColumnCollection();
                    //header =xmlDataSetName==null?new StoreColumnCollection(): new StoreColumnCollection(xmlDataSetName);
                    //var modelConfig = PFDataHelper.GetMultiModelConfig(xmlDataSetName);
                    var modelConfig = xmlDataSetName == null ? null : PFDataHelper.GetMultiModelConfig(xmlDataSetName);
                    foreach (DataColumn dataColumn in columns)
                    {
                        StoreColumn dictionary = modelConfig == null ? new StoreColumn(dataColumn) : new StoreColumn(dataColumn, modelConfig[dataColumn.ColumnName]);
                        //if (dataColumn.ColumnName.IndexOf(sc[0].Key) > -1)
                        //{
                        //    dictionary.title = dataColumn.ColumnName;//前端jqueryDatables不支持.
                        //    dictionary.data = dataColumn.ColumnName.Replace(sc[0].Key,sc[0].Value);
                        //}
                        //else
                        //{
                        //    dictionary.data = dataColumn.ColumnName;
                        //}
                        if (_specDataChar.Any(a => dataColumn.ColumnName.IndexOf(a.Key) > -1))
                        {
                            dictionary.title = dataColumn.ColumnName;//前端jqueryDatables不支持.
                            dictionary.data = EncodeDataChar(dataColumn.ColumnName);
                        }
                        else
                        {
                            dictionary.data = dataColumn.ColumnName;
                        }

                        //宽度的优先级:setWidthByHeaderWord<modelConfig<ExtendedProperties
                        //PFModelConfig config = modelConfig == null ? null : modelConfig[dataColumn.ColumnName];
                        //dictionary.SetPropertyByModelConfig(config);

                        if (dataColumn.ExtendedProperties.ContainsKey("title")) { dictionary.title = dataColumn.ExtendedProperties["title"].ToString(); }//20180803
                        if (PFDataHelper.StringIsNullOrWhiteSpace(dictionary.title)) { dictionary.title = dataColumn.ColumnName; }
                        if (dataColumn.ExtendedProperties != null && dataColumn.ExtendedProperties.Contains("width"))
                        {
                            dictionary.width = dataColumn.ExtendedProperties["width"].ToString();
                        }
                        else if (setWidthByHeaderWord && PFDataHelper.StringIsNullOrWhiteSpace(dictionary.width))//设置为中文后进入这里才有意义
                        {
                            dictionary.SetWidthByTitleWords();

                        }

                        if (dataColumn.ExtendedProperties != null && dataColumn.ExtendedProperties.Contains("dataType"))
                        {
                            dictionary.dataType = dataColumn.ExtendedProperties["dataType"].ToString();
                        }
                        if (dataColumn.ExtendedProperties != null && dataColumn.ExtendedProperties.Contains("visible"))
                        {
                            dictionary.visible = bool.Parse(dataColumn.ExtendedProperties["visible"].ToString());
                        }
                        if (dataColumn.ExtendedProperties != null)
                        {
                            if (dataColumn.ExtendedProperties.Contains("summary"))
                            {
                                dictionary.summary = dataColumn.ExtendedProperties["summary"].ToString();
                            }
                            else if (dataColumn.ExtendedProperties.Contains("hasSummary") && bool.Parse(dataColumn.ExtendedProperties["hasSummary"].ToString()) == true)
                            {
                                dictionary.summary = GetColumnSummary(all, dataColumn.ColumnName, dictionary.summaryType);
                            }
                        }
                        header.Add(dictionary);
                    }
                    modelConfig.Dispose();
                    modelConfig = null;
                    GCCollect();
                    //GC.Collect();
                }
                else//head不为null时
                {
                    //如果有head的情况下,visible到底以head的为准还是以xml为准?所以xml配置应该在初始化时就尽量加入--wxj20180815
                    //var modelConfig = PFDataHelper.GetMultiModelConfig(xmlDataSetName);

                    var tree = new StoreColumn { Children = header };
                    tree.EachLeaf(column =>
                    {
                        //if (column.data!=null&&column.data.IndexOf(".") > -1)
                        //{
                        //    column.data=column.data.Replace(".", "_");
                        //}
                        if (column.data != null && _specDataChar.Any(a => column.data.IndexOf(a.Key) > -1))
                        {
                            column.data = EncodeDataChar(column.data);
                        }

                        ////宽度的优先级:setWidthByHeaderWord<modelConfig<column.width
                        ////PFModelConfig config = modelConfig == null ? null : modelConfig[column.data];
                        ////column.SetPropertyByModelConfig(config);//先设置了中文才能计算中文字符长度
                        //if (setWidthByHeaderWord && PFDataHelper.StringIsNullOrWhiteSpace(column.width))
                        //{
                        //    column.SetWidthByTitleWords();
                        //}

                        if (column.hasSummary)
                        {
                            //column.summary = PFDataHelper.Thousandth(CommonFun.ColumnTotal(all, column.data));
                            column.summary = GetColumnSummary(all, column.data, column.summaryType);
                        }
                    });
                }

                var r = new PagingResult { data = arrayList, columns = header, total = total };
                if (dataTable.ExtendedProperties.ContainsKey("exData"))
                {
                    r.exData = dataTable.ExtendedProperties["exData"];
                }

                //如果清了dataTable,cache就空了
                //PFDataHelper.ClearDataTable(dataTable);
                //dataTable = null;
                //PFDataHelper.ClearDataTable(all);
                //all = null;


                return r;
                //var jsResult = new PFJsonResult();
                //jsResult.Data = r;
                //jsResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                //return jsResult;
                ////return Json(r, JsonRequestBehavior.AllowGet); //返回一个json字符串

            }
            return null;
        }
        #region MD5函数
        /// <summary>
        /// MD5函数,需引用：using System.Security.Cryptography;
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            //微软md5方法参考return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        #endregion

        private static int WebAndExcelSizeRate = 4;//px和excel尺码的数值比 px/excel=rate
        public static double? WebWidthToExcel(string px)
        {
            if (StringIsNullOrWhiteSpace(px)) { return null; }
            double r = 0;
            if (double.TryParse(px.Replace("px", ""), out r))
            {
                return r / WebAndExcelSizeRate;
            }
            return null;
        }
        public static string ExcelWidthToWeb(double d)
        {
            return (d * 4).ToString("0.0") + "px";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailFrom"></param>
        /// <param name="emailFromPwd"></param>
        /// <param name="smtpHost"></param>
        /// <param name="toEmails"></param>
        /// <param name="mailTitle"></param>
        /// <param name="mailBody"></param>
        /// <param name="mailAction">oMail => {
        ///    oMail.IsBodyHtml = true;
        ///    oMail.Attachments.Add(new Attachment(file.InputStream, file.FileName));//附件
        ///}</param>
        /// <returns></returns>
        public static bool SendEmail(string emailFrom, string emailFromPwd, string smtpHost,
            string[] toEmails, string mailTitle, string mailBody,
            Action<MailMessage> mailAction = null)
        {
            //xuzhiquan@richinfo.cn,wxw<wxw@perfect99.com>,wxj@perfect99.com<wxj@perfect99.com>

            //实例化两个必要的
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            //发送邮箱地址
            mail.From = new MailAddress(emailFrom);

            //收件人(可以群发)
            foreach (var i in toEmails)
            {
                mail.To.Add(new MailAddress(i));//benjamin 
            }

            //是否以HTML格式发送
            mail.IsBodyHtml = true;
            //主题的编码格式
            mail.SubjectEncoding = Encoding.UTF8;
            //邮件的标题
            mail.Subject = mailTitle;
            //内容的编码格式
            mail.BodyEncoding = Encoding.UTF8;
            //邮件的优先级
            mail.Priority = MailPriority.Normal;
            //发送内容,带一个图片标签,用于对方打开之后,回发你填写的地址信息
            //mail.Body = @"获取打开邮件的用户IP，图片由服务器自动生成：<img src='" + Receipt + "'>";
            mail.Body = mailBody;
            //收件人可以在邮件里面
            //mail.Headers.Add("Disposition-Notification-To", "回执信息");//Framework3.5下报错:在头值中找到无效的字符 (这里应该是填回执邮箱的吧)--benjamin20200422
            mail.Headers.Add("Disposition-Notification-To", PFDataHelper.Encode("回执信息", PFEncodeType.Base64, PFEncodeType.UTF8));

            if (mailAction != null)
            {
                mailAction(mail);
            }

            //发件邮箱的服务器地址
            //smtp.Host = "smtp.qq.com";
            smtp.Host = smtpHost;// "smtp.perfect99.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Timeout = 1000000;
            //是否为SSL加密
            smtp.EnableSsl = true;
            //设置端口,如果不设置的话,默认端口为25
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            //验证发件人的凭据
            //smtp.Credentials = new System.Net.NetworkCredential("****@163.com", "这里的密码可以是授权码");
            smtp.Credentials = new System.Net.NetworkCredential(emailFrom, emailFromPwd);
            //加这段之前用公司邮箱发送报错：根据验证过程，远程证书无效
            //加上后解决问题
            ServicePointManager.ServerCertificateValidationCallback =
delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };

            try
            {
                //发送邮件
                smtp.Send(mail);
                //smtp.Dispose();//framework3.5没有Dispose方法--benjamin todo
                return true;
            }
            catch (Exception e1)
            {
                PFDataHelper.WriteError(e1);
                return false;
            }
        }
        public static bool SendEmailAsync(string emailFrom, string emailFromPwd, string smtpHost,
            string[] toEmails, string mailTitle, string mailBody,
            Action<MailMessage> mailAction = null)
        {
            //xuzhiquan@richinfo.cn,wxw<wxw@perfect99.com>,wxj@perfect99.com<wxj@perfect99.com>

            //实例化两个必要的
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            //发送邮箱地址
            mail.From = new MailAddress(emailFrom);

            //收件人(可以群发)
            foreach (var i in toEmails)
            {
                mail.To.Add(new MailAddress(i));//benjamin 
            }

            //是否以HTML格式发送
            mail.IsBodyHtml = true;
            //主题的编码格式
            mail.SubjectEncoding = Encoding.UTF8;
            //邮件的标题
            mail.Subject = mailTitle;
            //内容的编码格式
            mail.BodyEncoding = Encoding.UTF8;
            //邮件的优先级
            mail.Priority = MailPriority.Normal;
            //发送内容,带一个图片标签,用于对方打开之后,回发你填写的地址信息
            //mail.Body = @"获取打开邮件的用户IP，图片由服务器自动生成：<img src='" + Receipt + "'>";
            mail.Body = mailBody;
            //收件人可以在邮件里面
            mail.Headers.Add("Disposition-Notification-To", "回执信息");

            if (mailAction != null)
            {
                mailAction(mail);
            }

            //发件邮箱的服务器地址
            //smtp.Host = "smtp.qq.com";
            smtp.Host = smtpHost;// "smtp.perfect99.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Timeout = 1000000;
            //是否为SSL加密
            smtp.EnableSsl = true;
            //设置端口,如果不设置的话,默认端口为25
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            //验证发件人的凭据
            //smtp.Credentials = new System.Net.NetworkCredential("****@163.com", "这里的密码可以是授权码");
            smtp.Credentials = new System.Net.NetworkCredential(emailFrom, emailFromPwd);
            //加这段之前用公司邮箱发送报错：根据验证过程，远程证书无效
            //加上后解决问题
            ServicePointManager.ServerCertificateValidationCallback =
delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };

            try
            {
                //发送邮件
                smtp.SendAsync(mail, null);
                //smtp.Dispose();
                return true;
            }
            catch (Exception e1)
            {
                PFDataHelper.WriteError(e1);
                return false;
            }
        }

        ///// <summary>
        ///// 发送邮件并监听结果
        ///// </summary>
        ///// <param name="emailFrom"></param>
        ///// <param name="emailFromPwd"></param>
        ///// <param name="smtpHost"></param>
        ///// <param name="toEmails"></param>
        ///// <param name="mailTitle"></param>
        ///// <param name="mailBody"></param>
        ///// <param name="mailAction"></param>
        ///// <returns></returns>
        //public static Task<PFEmail> SendEmailAsync(string emailFrom, string emailFromPwd, string smtpHost,
        //    string[] toEmails, string mailTitle, string mailBody,
        //    //Action<PFEmail> resultAction,
        //    Action<MailMessage> mailAction = null)
        //{
        //    var rt = new Task<PFEmail>(() =>
        //    {
        //        //生产方监听回复
        //        PFEmail result = null;
        //        bool hasGotResult = false;
        //        //var nowStr = DateTime.Now.ToString(PFDataHelper.DateFormat);
        //        var producerListenTask = new PFListenEmailTask(NewUniqueHashId,
        //        new PFEmailManager(smtpHost, emailFrom, emailFromPwd),
        //        email =>
        //        {
        //            result = email;
        //            hasGotResult = true;
        //        },
        //        (email//, task
        //        ) =>
        //        {
        //            //return email.Subject != null && email.Subject.IndexOf("PFListenEmailTask_AutoReply_") == 0;//这里不要用>-1,否则可能把自动回复的邮件也当作是了
        //            //return email.Subject == task.HashId + "_Reply_" + mailTitle;//消费方不知道生成的hashId,所以不能用hashId来匹配
        //            return email.Subject == "PFEmailMq_consumer_" + mailTitle;
        //        });
        //        producerListenTask.Start();

        //        //生产方发邮件
        //        PFDataHelper.SendEmail(emailFrom, emailFromPwd, smtpHost,
        //            toEmails, mailTitle, JsonConvert.SerializeObject(mailBody));

        //        while (!hasGotResult)
        //        {
        //            Thread.Sleep(2000);
        //        }

        //        return result;
        //    });
        //    rt.Start();
        //    return rt;
        //}

        //private static List<PFListenEmailTask> _listenEmailTask;
        //public static void ListenEmail(string hostName, string userName, string pwd,
        //     Action<PFEmail> doAction, Func<PFEmail, bool> subjectMatch)//,bool deleteAfterRead) 
        //{

        //    if (_listenEmailTask == null) { _listenEmailTask = new List<PFListenEmailTask>(); }
        //    var task = new PFListenEmailTask("PFTcBackupChecker",
        //        new PFEmailManager(hostName, userName, pwd),
        //        doAction,
        //        subjectMatch);
        //    _listenEmailTask.Add(task);
        //    task.Start();

        //}

        //        public static string FormatMethodExecStatus(List<string> errors, MethodExecStatus execStatus)
        //        {
        //            if (execStatus == MethodExecStatus.Success) { return "执行成功,没有报错"; }
        //            return string.Format(@"
        //{0}:
        //{1}
        //",
        //execStatus == MethodExecStatus.Error ? "执行报错" : "执行部分报错",
        //string.Join("\r\n", errors)
        //);
        //        }
        public static string FormatMethodExecStatus(List<string> errors, int total, out MethodExecStatus execStatus)
        {
            if (!errors.Any())
            {
                execStatus = MethodExecStatus.Success;
                return "执行成功,没有报错";
            }
            else if (errors.Count < total)
            {
                execStatus = MethodExecStatus.PartError;
            }
            else
            {
                execStatus = MethodExecStatus.Error;
            }
            return string.Format(@"
{0}:
{1}
",
execStatus == MethodExecStatus.Error ? "执行报错" : "执行部分报错",
string.Join("\r\n", errors.ToArray())
);
        }

        /// <summary>
        /// 科学计数法
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ScientificNotation(double num)
        {
            double bef = System.Math.Abs(num);
            int aft = 0;
            while (bef >= 10 || (bef < 1 && bef != 0))
            {
                if (bef >= 10)
                {
                    bef = bef / 10;
                    aft++;
                }
                else
                {
                    bef = bef * 10;
                    aft--;
                }
            }
            return string.Concat(num >= 0 ? "" : "-", ReturnBef(bef), "E", ReturnAft(aft));
        }
        /// <summary>
        /// 有效数字的处理
        /// </summary>
        /// <param name="bef">有效数字</param>
        /// <returns>三位有效数字，不足则补零</returns>
        public static string ReturnBef(double bef)
        {
            if (bef.ToString() != null)
            {
                char[] arr = bef.ToString().ToCharArray();
                switch (arr.Length)
                {
                    case 1:
                    case 2: return string.Concat(arr[0], ".", "00"); break;
                    case 3: return string.Concat(arr[0] + "." + arr[2] + "0"); break;
                    default: return string.Concat(arr[0] + "." + arr[2] + arr[3]); break;
                }
            }
            else
                return "000";
        }
        /// <summary>
        /// 幂的处理
        /// </summary>
        /// <param name="aft">幂数</param>
        /// <returns>三位幂数部分，不足则补零</returns>
        public static string ReturnAft(int aft)
        {
            if (aft.ToString() != null)
            {
                string end;
                char[] arr = System.Math.Abs(aft).ToString().ToCharArray();
                switch (arr.Length)
                {
                    case 1: end = "00" + arr[0]; break;
                    case 2: end = "0" + arr[0] + arr[1]; break;
                    default: end = System.Math.Abs(aft).ToString(); break;
                }
                return string.Concat(aft >= 0 ? "+" : "-", end);
            }
            else
                return "+000";
        }
        public class CFoldPoint
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        #region 统计算法
        //最小二乘法直线拟合,线性回归
        public static bool CalculateLineKB(ref List<CFoldPoint> m_FoldList, ref double k, ref double b)
        {
            //最小二乘法直线拟合
            //m_FoldList为关键点(x,y)的链表
            //拟合直线方程(Y=kX+b)，k和b为返回值

            if (m_FoldList == null) return false;
            long lCount = m_FoldList.Count;
            if (lCount < 2) return false;
            //CFoldPoint pFold;
            double mX, mY, mXX, mXY, n;
            mX = mY = mXX = mXY = 0;
            n = lCount;
            //POSITION pos = m_FoldList->GetHeadPosition();
            foreach (var pFold in m_FoldList)
            {
                //pFold = m_FoldList->GetNext(pos);
                mX += pFold.X;
                mY += pFold.Y;
                mXX += pFold.X * pFold.X;
                mXY += pFold.X * pFold.Y;
            }
            if (mX * mX - mXX * n == 0) return true;
            k = (mY * mX - mXY * n) / (mX * mX - mXX * n);
            b = (mY - mX * k) / n;
            return true;
        }
        #endregion

        public static NameValueCollection GetConnectArray(string connectionString)
        {
            NameValueCollection CurConnectStrings = new NameValueCollection();
            string[] conn1 = connectionString.Split(new char[] { ';' });
            foreach (string conn in conn1)
            {
                string[] conn2 = conn.Split(new char[] { '=' });
                CurConnectStrings.Add(conn2[0], conn2[1]);
            }
            return CurConnectStrings;
        }
        public static string GetConnectionString(string connectionString, string databaseName)
        {
            NameValueCollection CurConnectStrings = GetConnectArray(connectionString);
            CurConnectStrings["Initial Catalog"] = databaseName;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < CurConnectStrings.Count; i++)
            {

                if (i < CurConnectStrings.Count - 1)
                {
                    sb.Append(CurConnectStrings.GetKey(i) + "=" + CurConnectStrings[i] + ";");
                }
                else
                {
                    sb.Append(CurConnectStrings.GetKey(i) + "=" + CurConnectStrings[i]);
                }
            }
            return sb.ToString();
        }
        //PFDataHelper End
    }

    #region 树型结构
    /// <summary>
    /// 树型项目(不编写TreeList<T>的原因是,继承IList需要实现的方法太多了)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeListItem<T>
        where T : TreeListItem<T>
    {
        private List<T> _children;
        public List<T> Children { get { return _children ?? (_children = new List<T>()); } set { _children = value; } }

        /// <summary>
        /// 深度优先递归
        /// </summary>
        /// <param name="action">参数:T子项,int深度</param>
        public void EachChild(Action<T, int> action, int depth = 2)
        {
            Children.ForEach(a => { action(a, depth); a.EachChild(action, depth + 1); });
        }

        /// <summary>
        /// 深度优先递归
        /// </summary>
        /// <param name="action">参数:T子项,int深度,T父节点</param>
        public void EachChild(Action<T, int, TreeListItem<T>> action, int depth = 2)
        {
            Children.ForEach(a => { action(a, depth, this); a.EachChild(action, depth + 1); });
        }

        /// <summary>
        /// 深度优先递归
        /// </summary>
        public void EachChild(Action<T> action)
        {
            Children.ForEach(a => { action(a); a.EachChild(action); });

        }

        /// <summary>
        /// 遍历末级叶节点
        /// </summary>
        /// <param name="action"></param>
        public void EachLeaf(Action<T> action)
        {
            Children.ForEach(a =>
            {
                if (a.Children.Any())
                {
                    a.EachLeaf(action);
                }
                else
                {
                    action(a);
                }
            });
        }
        /// <summary>
        /// 第一个叶节点
        /// </summary>
        public T FirstLeaf(Func<T, bool> condition)
        {
            if (Children.Count < 1) { return condition((T)this) ? (T)this : null; }
            T result;
            foreach (var i in Children)
            {
                result = i.FirstLeaf(condition);
                if (result != null) { return result; }
            }
            return null;
        }
        //public bool HasLeaf(Func<T, bool> condition)
        //{
        //    foreach(var i in Children)
        //    {
        //        var isHas = false;
        //        i.EachLeaf(a=> {
        //            if (condition(a))
        //            {
        //                isHas;
        //            }
        //        });
        //    }
        //    if (Children.Any())
        //    {
        //        return Children.Any(a => a.HasLeaf(condition));
        //    }
        //    else
        //    {
        //        if (condition(this))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        #region 便于过滤所需要的变量,请不要在外部使用
        private TreeListItem<T> _parent = null;
        private bool _fitFilter = false;
        private void SetParent()
        {
            EachChild((child, depth, parent) =>
            {
                //if (child._parent == null)
                //{
                //    child._parent = parent;
                //}
                child._parent = parent;
                child._fitFilter = false;
            });
        }
        private void SetFit()
        {
            this._fitFilter = true;
            if (this._parent != null) { _parent.SetFit(); }
        }
        private void RemoveNotFit()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i]._fitFilter)
                {
                    Children[i].RemoveNotFit();
                }
                else
                {
                    Children.Remove(Children[i]);
                }
            }
        }
        #endregion

        /// <summary>
        /// 根据叶节点来过滤
        /// </summary>
        public TreeListItem<T> FilterByLeaf(Func<T, bool> condition)
        {
            SetParent();
            EachLeaf((a) =>
            {
                if (condition(a))
                {
                    a.SetFit();
                }
            });
            RemoveNotFit();
            return this;
        }

        /// <summary>
        /// 获得最大深度(最小为1)
        /// </summary>
        /// <returns></returns>
        public int GetDepth()
        {
            int max = 1;
            EachChild((a, b) => { if (b > max) { max = b; } });
            return max;
        }


        /// <summary>
        ///     获得所有children的数量,递归查找
        /// </summary>
        /// <returns></returns>
        public int GetAllChildrenCount()
        {
            int total = 0;
            EachChild(a => total++);
            return total;
        }
        ///// <summary>
        /////     获得所有children的数量,递归查找
        ///// </summary>
        ///// <returns></returns>
        //public int GetAllChildrenCount(Func<T, bool> condition )
        //{
        //    int total = 0;
        //    EachChild(a => { if (condition == null || condition(a)) { total++; } });
        //    return total;
        //}
        /// <summary>
        /// 获得所有末级叶Child的数量
        /// </summary>
        /// <returns></returns>
        public int GetAllLeafCount()
        {
            int total = 0;
            EachLeaf(a => total++);
            return total;
        }
        /// <summary>
        /// 获得所有末级叶Child的数量
        /// </summary>
        /// <returns></returns>
        public int GetAllLeafCount(Func<T, bool> condition)
        {
            int total = 0;
            EachLeaf(a => { if (condition == null || condition(a)) { total++; } });
            return total;
        }
    }
    public interface ITreeListItem
    {
        object Data { get; set; }
        IList GetChildren();
    }
    public class TreeListItem : TreeListItem<TreeListItem>, ITreeListItem
    {
        public object Data { get; set; }

        public IList GetChildren()
        {
            return Children;
        }
    }
    #endregion 树型结构

    #region 日期范围
    /// <summary>
    /// 日期范围(多用于报表统计)
    /// </summary>
    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateRange(int year)
        {
            StartDate = new DateTime(year, 1, 1);
            EndDate = new DateTime(year + 1, 1, 1).AddSeconds(-1);
        }
        //public DateRange(DateRangeType rangeType,int num)
        public DateRange(int year, int month)
        {
            StartDate = new DateTime(year, month, 1);
            EndDate = StartDate.AddMonths(1).AddSeconds(-1);
        }
        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
        /// <summary>
        /// 旧版本有问题,当传入2014.01~2014.02时,仅会返回2014.01,但传入2014.01~2014.01时,也是返回2014.01.这样是有矛盾的
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetPerMonthStartOld()
        {
            var currentDate = StartDate;
            do
            {
                yield return currentDate.GetMonthStart();
                currentDate = currentDate.AddMonths(1);
            } while (currentDate < EndDate);
        }
        public IEnumerable<DateTime> GetPerMonthStart()
        {
            var currentDate = StartDate;
            while (currentDate <= EndDate)
            {
                yield return currentDate.GetMonthStart();
                currentDate = currentDate.AddMonths(1);
            }
            #region 这样有问题,当传入2014.01~2014.02时,人会返回2014.01
            //do
            //{
            //    yield return currentDate.GetMonthStart();
            //    currentDate = currentDate.AddMonths(1);
            //} while (currentDate < EndDate); 
            #endregion
        }
        public IEnumerable<DateTime> GetPerDayStart()
        {
            var currentDate = StartDate;
            do
            {
                yield return currentDate.GetDayStart();
                currentDate = currentDate.AddDays(1);
            } while (currentDate < EndDate);
        }
    }
    //public enum DateRangeType {
    //    Year=1,
    //    Month=2
    //}
    #endregion

    /// <summary>
    /// 字段配置(使用时,一个重要原则是:只要本配置里不为null,就会复盖DataColumn原来的配置)
    /// </summary>
    public class PFModelConfig : ICloneable
    {
        /// <summary>
        /// 为了使用复制方法
        /// </summary>
        public PFModelConfig()
        {

        }
        public bool Equals(PFModelConfig obj)
        {
            return this.PropertyName == obj.PropertyName && this.DataSet == obj.DataSet && this.FieldName == obj.FieldName;
        }

        public PFModelConfig TClone()
        {
            #region 不用反射性能更好
            return new PFModelConfig
            {
                PropertyName = this.PropertyName,
                DataSet = this.DataSet,
                FieldId = this.FieldId,
                FieldName = this.FieldName,
                LowerFieldName = this.LowerFieldName,
                FieldText = this.FieldText,
                FieldType = this.FieldType,
                Precision = this.Precision,
                FieldSqlLength = this.FieldSqlLength,
                FieldWidth = this.FieldWidth,
                Visible = this.Visible,
                Required = this.Required
            };
            #endregion
            //return TransExpV2<PFModelConfig, PFModelConfig>.Trans(this);
        }

        public object Clone()
        {
            return TClone();
        }

        public PFModelConfig(XmlNode fieldNode, string dataSet)
        {
            var node = fieldNode.SelectSingleNode("FieldId");
            if (node != null) { FieldId = node.InnerText; }
            node = fieldNode.SelectSingleNode("FieldName");
            if (node != null)
            {
                PropertyName = FieldName =
                    node.InnerText;
            }
            node = fieldNode.SelectSingleNode("FieldText");
            if (node != null) { FieldText = node.InnerText; }
            node = fieldNode.SelectSingleNode("FieldType");
            if (node != null) { FieldType = PFDataHelper.GetTypeByString(node.InnerText); }
            node = fieldNode.SelectSingleNode("Precision");
            if (node != null) { Precision = int.Parse(node.InnerText); }
            node = fieldNode.SelectSingleNode("FieldSqlLength");
            if (node != null) { FieldSqlLength = int.Parse(node.InnerText); }
            node = fieldNode.SelectSingleNode("FieldWidth");
            if (node != null) { FieldWidth = node.InnerText + "px"; }
            node = fieldNode.SelectSingleNode("Visible");
            if (node != null) { Visible = bool.Parse(node.InnerText); }
            node = fieldNode.SelectSingleNode("Required");
            if (node != null) { Required = bool.Parse(node.InnerText); }

            LowerFieldName = (FieldName ?? "").ToLower();
            DataSet = dataSet;
        }

        /// <summary>
        /// Model的属性名(当table里有两个来自不同模块的inv字段时,这两个字段的PropertyName不一样(因为是Model中的属性),但FieldName是一样,所以FieldName对前端其实是无作用.)(现保留这两属性是为了便于以后要区分数据来源)
        /// </summary>
        public string PropertyName { get; set; }

        #region xml里的属性
        public string DataSet { get; set; }//DataSet和FieldName是为了使于日后维护xml的对应节点
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        //public string LowerFieldName { get { return (FieldName??"").ToLower(); } }
        public string LowerFieldName { get; set; }//配置文件改为不区分大小写，这样好像更好，为了过渡，不删除FieldName属性--wxj20181012
        public string FieldText { get; set; }
        public Type FieldType { get; set; }

        /// <summary>
        /// 如果是decimal,可以设置精确度,表示decimal(a,b)中的b
        /// </summary>
        public int? Precision { get; set; }
        /// <summary>
        /// sql中varchar的长度,便于以后做验证;也可表示decimal(a,b)中的a,可以考虑加一个属性来记录b
        /// </summary>
        public int? FieldSqlLength { get; set; }//用字符串是为了适应decimal

        public string FieldWidth { get; set; }//用字符串是为了写单位14px,14dx
        private bool _visible = true;
        public bool Visible { get { return _visible; } set { _visible = value; } }
        public bool Required { get; set; }
        #endregion

        #region Method
        //public bool IsMatchField(string fieldName)
        //{
        //    if (!PFDataHelper.StringIsNullOrWhiteSpace(fieldName)) {
        //        return fieldName.ToLower() == LowerFieldName;
        //    }
        //    return false;
        //}
        #endregion
    }

    public class PFModelConfigCollection : Dictionary<string, PFModelConfig>, IDisposable
    {
        private Dictionary<string, string> _commonPrev = new Dictionary<string, string> {
            { "old_","原" }
        };
        private Dictionary<string, string> _commonWord = new Dictionary<string, string> {
            { "old","原" }
        };
        public new PFModelConfig this[string key]
        {
            get
            {
                var low = key.ToLower();
                if (this.ContainsKey(low))
                {
                    return base[low];
                }
                foreach (var i in _commonPrev)//以old_开头的
                {
                    if (low.IndexOf(i.Key) > -1)
                    {
                        var pureLow = low.Replace(i.Key, "");
                        if (this.ContainsKey(pureLow))
                        {
                            //var pureConfig = TransExpV2<PFModelConfig, PFModelConfig>.Trans(base[pureLow]);
                            var pureConfig = base[pureLow].TClone();
                            pureConfig.FieldText = i.Value + pureConfig.FieldText;
                            return pureConfig;
                        }
                    }
                }
                if (low.IndexOf("_") > -1)//如果是hybh_hpos之类的key,用_分隔来取中文--benjamin20191231
                {
                    var ens = low.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    var cn = "";
                    var allHasCn = true;
                    for (int i = 0; i < ens.Length; i++)
                    {
                        if (this.ContainsKey(ens[i]))
                        {
                            cn += base[ens[i]].FieldText;
                        }
                        else if (_commonWord.ContainsKey(ens[i]))
                        {
                            cn += _commonWord[ens[i]];
                        }
                        else
                        {
                            allHasCn = false;
                            break;
                        }
                    }
                    if (allHasCn)
                    {
                        return new PFModelConfig { FieldText = cn };
                    }
                }
                return null;
            }
            set
            {
                var low = key.ToLower();
                base[low] = value;
            }
        }
        public void Dispose()
        {
            this.Clear();
        }
    }
    public class PFModelConfigMapper
    {
        public string ModelName { get; set; }
        /// <summary>
        /// xml的DataSet节点名
        /// </summary>
        public string XmlDataSetName { get; set; }

        private List<string> _otherXmlDataSetName;
        /// <summary>
        /// 其它xml节点(如果主节点找不到,在这里找)
        /// </summary>
        public List<string> OtherXmlDataSetName { get { return _otherXmlDataSetName ?? (_otherXmlDataSetName = new List<string>()); } set { _otherXmlDataSetName = value; } }

        //private List<PFModelPropertyConfigMapper> _exProperty;
        ///// <summary>
        ///// 例外属性,指定了别的DataSet节点和对应字段
        ///// </summary>
        //public List<PFModelPropertyConfigMapper> ExProperty { get { return _exProperty ?? (_exProperty = new List<PFModelPropertyConfigMapper>()); } set { _exProperty = value; } }

    }
    public class PFModelPropertyConfigMapper
    {
        public string PropertyName { get; set; }
        public string XmlDataSetName { get; set; }
        public string XmlFieldName { get; set; }
        public PFModelPropertyConfigMapper(string propertyName, string xmlDataSetName)
        {
            XmlFieldName = PropertyName = propertyName;
            XmlDataSetName = xmlDataSetName;
        }
        public PFModelPropertyConfigMapper(string propertyName, string xmlDataSetName, string xmlFieldName)
        {
            PropertyName = propertyName;
            XmlDataSetName = xmlDataSetName;
            XmlFieldName = xmlFieldName;
        }
    }
    public class PFPathConfig
    {
        #region Field
        private string _imagePath = "Images/Perfect";
        /// <summary>
        /// 样式的存放目录(/Css)
        /// </summary>
        private string _cssPath = "Css/Perfect";
        /// <summary>
        /// 字段列名等的存放目录
        /// </summary>
        private string _configPath = "Configs/XmlConfig";
        /// <summary>
        /// DataBox脚本路径
        /// </summary>
        private string _dataBoxJsPath = "Content/My97DatePicker/WdatePicker.js";
        #endregion
        /// <summary>
        /// 组件图片等的存放目录(/Images)
        /// </summary>
        public string ImagePath { get { return _imagePath; } set { _imagePath = value; } }
        /// <summary>
        /// 样式的存放目录(/Css)
        /// </summary>
        public string CssPath { get { return _cssPath; } set { _cssPath = value; } }
        /// <summary>
        /// 字段列名等的存放目录
        /// </summary>
        public string ConfigPath { get { return _configPath; } set { _configPath = value; } }//最前面不加/是为了方便AppDomainAppPath拼接的情况

        /// <summary>
        /// 字段列名等的存放目录
        /// </summary>
        public string DataBoxJsPath { get { return _dataBoxJsPath; } set { _dataBoxJsPath = value; } }

    }
    public class PFNetworkConfig
    {
        private int _downloadSpeed = 1024 * 1024 * 10;
        public int DownloadSpeed { get { return _downloadSpeed; } set { _downloadSpeed = value; } }
    }

    public interface IPFConfigMapper
    {
        List<PFModelConfigMapper> GetModelConfigMapper();

        PFPathConfig GetPathConfig();
        PFNetworkConfig GetNetworkConfig();
    }


    #region SqlHelper
    public class SqlCreateTableItem : PFModelConfig
    {

    }
    public class SqlCreateTableCollection : List<SqlCreateTableItem>// Dictionary<string, PFModelConfig>
    {
        private string _charset = "utf-8";
        public string TableName { get; set; }
        public string Charset { get { return _charset; } set { _charset = value; } }
        public virtual string FieldQuotationCharacterL { get { return "`"; } }//[
        public virtual string FieldQuotationCharacterR { get { return "`"; } }//]
        public string[] PrimaryKey { get; set; }
        public string[] TableIndex { get; set; }
        public string ToSql()
        {
            #region tidb上测试通过

            var result = string.Format(@"
            CREATE TABLE `{0}` (
              {1}
               {2}
            );

            ",
TableName,
string.Join(",", this.Select(
    a =>
        string.Format("{0}{1}{2} {3} ",
            FieldQuotationCharacterL,
            a.FieldName,
            FieldQuotationCharacterR,
            GetFieldTypeString(a)
        )
    ).ToArray()
),
    PrimaryKey != null && PrimaryKey.Any() ? string.Format(",PRIMARY KEY({0})", string.Join(",", PrimaryKey)) : ""
);
            if (TableIndex != null && TableIndex.Any())
            {
                foreach (var i in TableIndex)
                {
                    result += string.Format(@"
CREATE INDEX  idx_{2} ON {0} ({1}{2}{3});
", TableName, FieldQuotationCharacterL, i, FieldQuotationCharacterR);
                }
            }
            #endregion

            //            //tidb语法https://blog.csdn.net/u011782423/article/details/81082419
            //            var result = string.Format(@"
            //           CREATE TABLE {0}
            //           (
            //                {1}
            //             );
            //",
            //TableName,
            //string.Join(",", this.Select(
            //    a =>
            //        string.Format("{0} {1}",
            //            a.FieldName,
            //            GetFieldTypeString(a)
            //        )
            //    )
            //),
            // Charset
            //);
            return result;
        }
        public string GetFieldTypeString(PFModelConfig m)
        {
            string r = "";
            if (m.FieldType == typeof(int))
            {
                r = string.Format("int");//int(11) ?后面不知道要不要长度
            }
            else if (m.FieldType == typeof(string))
            {
                r = string.Format("varchar({0})", m.FieldSqlLength ?? 100);//int(11) ?后面不知道要不要长度
            }
            else if (m.FieldType == typeof(decimal))
            {
                r = string.Format("decimal({0},{1})", m.FieldSqlLength ?? 18, m.Precision ?? 2);
            }
            else if (m.FieldType == typeof(DateTime))
            {
                r = "datetime";
            }
            else
            {
                r = "varchar(100)";
            }
            if (PrimaryKey != null && PrimaryKey.Contains(m.FieldName))
            {
                r += " not null";
            }
            return r;
        }

    }
    //public enum SqlExpressionType
    //{
    //}
    public enum SqlExpressionOperator
    {
        Equal = 0,
        /// <summary>
        /// 数据库的值小于输入值
        /// </summary>
        Less = 1,
        /// <summary>
        /// 数据库的值小于或等于输入值
        /// </summary>
        LessOrEqual = 2,
        /// <summary>
        /// 数据库的值大于输入值
        /// </summary>
        Greater = 3,
        /// <summary>
        /// 数据库的值大于或等于输入值
        /// </summary>
        GreaterOrEqual = 4,
        Like = 5,
        IN = 6,
        NotIn = 7,
        //Exists = 8,
        //NotExists = 9,
        NotEqual = 10,
        StartWith = 11,
        EndWith = 12
    }
    public class SqlWhereItem
    {
        public SqlWhereItem(string key, object value, SqlExpressionOperator expressionOperator = SqlExpressionOperator.Equal)
        {
            Key = key;
            Value = value;
            ExpressionOperator = expressionOperator;
        }
        public string Key { get; set; }
        public object Value { get; set; }
        public SqlExpressionOperator ExpressionOperator { get; set; }
    }
    /// <summary>
    /// Sql Where条件拼接器(便于模糊查询,空字符串乎略)(可考虑扩展为TreeListItem,或增加or条件的支持)
    /// 不要自动加object的所有property为条件,因为不知道条件是like还是equal等等(而且同一类型有可能使用此类用于多个dal方法,不应该互相影响)
    /// 返回格式如: where xx=xx and ...
    /// </summary>
    public class SqlWhereCollection : List<SqlWhereItem>
    {
        public virtual string FieldQuotationCharacterL { get { return "["; } }
        public virtual string FieldQuotationCharacterR { get { return "]"; } }
        public enum WhereOrAnd
        {
            Where = 1,
            And = 2
        }
        /// <summary>
        /// 分页参数
        /// </summary>
        public class Pagination
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <param name="sort">注意这个字段即便join表也不需要带前缀,a.id直接写id就行了</param>
            public Pagination(int start, int end, string sort)
            {
                Start = start;
                End = end;
                Sort = sort;
            }
            public int Start { get; set; }
            public int End { get; set; }
            /// <summary>
            /// 分页参数的排序字段必需指定主键，否则会很慢
            /// </summary>
            public string Sort { get; set; }
        }
        public void Add(string name, object value, SqlExpressionOperator expressionOperator = SqlExpressionOperator.Equal)
        {
            base.Add(new SqlWhereItem(name, value, expressionOperator));
        }
        /// <summary>
        /// 加中括号,如 a.typeid 变为 a.[typeid]
        /// </summary>
        private string FormatKey(string key)
        {
            //return Regex.Replace(key, "([^.]+)$", "[$1]");
            return Regex.Replace(key, "([^.]+)$", FieldQuotationCharacterL + "$1" + FieldQuotationCharacterR);
        }
        public string ToSql(WhereOrAnd woa = WhereOrAnd.Where)
        {
            var result = "";
            int count = 0;
            var prev = "";
            foreach (var i in this)
            {
                if (i.Value != null)
                {
                    prev = count == 0 ?
                        (woa == WhereOrAnd.Where ? " where " : " and ")
                        : " and ";

                    var val = i.Value;
                    //if (i.Value is DateTime) { tmpControl.Text = PFDataHelper.ObjectToDateString(val, tmpControl.Attributes["dateFmt"]); }
                    if (val is string && !PFDataHelper.StringIsNullOrWhiteSpace(val.ToString()))
                    {
                        switch (i.ExpressionOperator)
                        {
                            //既然为空时不进入这里,那么isnull应该是无必要的--wxj20181022
                            case SqlExpressionOperator.Like:
                                //result += prev + string.Format(" isnull({0},'') like '%{1}%' ", i.Key, i.Value);
                                result += prev + string.Format(" {0} like '%{1}%' ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.StartWith:
                                //result += prev + string.Format(" isnull({0},'') like '{1}%' ", i.Key, i.Value);
                                result += prev + string.Format(" {0} like '{1}%' ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.EndWith:
                                //result += prev + string.Format(" isnull({0},'') like '%{1}' ", i.Key, i.Value);
                                result += prev + string.Format(" {0} like '%{1}' ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.NotEqual:
                                result += prev + string.Format(" isnull({0},'') <> '{1}' ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.Less:
                                result += prev + string.Format(" isnull({0},'') < '{1}' ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.LessOrEqual:
                                result += prev + string.Format(" isnull({0},'') <= '{1}' ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.Greater:
                                //result += prev + string.Format(" isnull({0},'') >= '{1}' ", FormatKey(i.Key), i.Value);
                                result += prev + string.Format(" {0} > '{1}' ", FormatKey(i.Key), i.Value);//效率高一些--20200507
                                break;
                            case SqlExpressionOperator.GreaterOrEqual:
                                //result += prev + string.Format(" isnull({0},'') >= '{1}' ", FormatKey(i.Key), i.Value);
                                result += prev + string.Format(" {0} >= '{1}' ", FormatKey(i.Key), i.Value);//效率高一些--20200507
                                break;
                            default:
                                //result += prev + string.Format(" isnull({0},'')='{1}' ", i.Key, i.Value);
                                result += prev + string.Format(" {0}='{1}' ", FormatKey(i.Key), i.Value);
                                break;
                        }
                        count++;
                    }
                    else if (val is decimal || val is int)
                    {
                        //result += prev + string.Format(" isnull({0},0)={1} ", FormatKey(i.Key), i.Value);
                        switch (i.ExpressionOperator)
                        {
                            //日期的范围比较有些特别,把临界点包含进来比较适合
                            case SqlExpressionOperator.Less:
                                //result += prev + string.Format(" isnull({0},'') <= '{1}' ", FormatKey(i.Key), i.Value);
                                result += prev + string.Format(" isnull({0},0)<{1} ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.LessOrEqual:
                                //result += prev + string.Format(" isnull({0},'') <= '{1}' ", FormatKey(i.Key), i.Value);
                                result += prev + string.Format(" isnull({0},0)<={1} ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.Greater:
                                //result += prev + string.Format(" isnull({0},'') >= '{1}' ", FormatKey(i.Key), i.Value);
                                //result += prev + string.Format(" isnull({0},0)>{1} ", FormatKey(i.Key), i.Value);
                                result += prev + string.Format(" {0}>{1} ", FormatKey(i.Key), i.Value);
                                break;
                            case SqlExpressionOperator.GreaterOrEqual:
                                //result += prev + string.Format(" isnull({0},'') >= '{1}' ", FormatKey(i.Key), i.Value);
                                //result += prev + string.Format(" isnull({0},0)>={1} ", FormatKey(i.Key), i.Value);
                                result += prev + string.Format(" {0}>={1} ", FormatKey(i.Key), i.Value);
                                break;
                            default:
                                //result += prev + string.Format(" isnull({0},'')='{1}' ", FormatKey(i.Key), i.Value);
                                result += prev + string.Format(" isnull({0},0)={1} ", FormatKey(i.Key), i.Value);
                                break;
                        }
                        count++;
                    }
                    else if (val is DateTime && val != null)
                    {
                        var valDateTime = ((DateTime)val).ToString(PFDataHelper.DateFormat);
                        switch (i.ExpressionOperator)
                        {
                            case SqlExpressionOperator.Less:
                                result += prev + string.Format(" isnull({0},'') < '{1}' ", FormatKey(i.Key), valDateTime);
                                break;
                            //日期的范围比较有些特别,把临界点包含进来比较适合
                            case SqlExpressionOperator.LessOrEqual:
                                result += prev + string.Format(" isnull({0},'') <= '{1}' ", FormatKey(i.Key), valDateTime);
                                break;
                            case SqlExpressionOperator.Greater:
                                result += prev + string.Format(" isnull({0},'') > '{1}' ", FormatKey(i.Key), valDateTime);
                                break;
                            case SqlExpressionOperator.GreaterOrEqual:
                                result += prev + string.Format(" isnull({0},'') >= '{1}' ", FormatKey(i.Key), valDateTime);
                                break;
                            default:
                                result += prev + string.Format(" isnull({0},'')='{1}' ", FormatKey(i.Key), valDateTime);
                                break;
                        }
                        count++;
                    }
                    else if (val is bool && val != null)
                    {
                        var b = (bool)val;
                        result += prev + string.Format(" {0}={1} ", FormatKey(i.Key), b ? 1 : 0);
                        count++;
                    }
                    else if (val is Guid)
                    {
                        result += prev + string.Format(" {0}='{1}' ", FormatKey(i.Key), i.Value);
                        count++;
                    }
                    else if (val is IList<string> && i.ExpressionOperator == SqlExpressionOperator.IN)
                    {
                        var list = val as IList<string>;
                        if (list.Count > 0)
                        {
                            //result += prev + string.Format(" {0} in('{1}') ", FormatKey(i.Key), string.Join("','", list));
                            result += prev + string.Format(" {0} in('{1}') ", FormatKey(i.Key), string.Join("','", list.ToArray()));//为了net2.0版本
                            count++;
                        }
                    }
                    else if (val is IList<string> && i.ExpressionOperator == SqlExpressionOperator.NotIn)
                    {
                        var list = val as IList<string>;
                        if (list.Count > 0)
                        {
                            result += prev + string.Format(" {0} not in('{1}') ", FormatKey(i.Key), string.Join("','", list.ToArray()));
                            count++;
                        }
                    }
                }
            }
            return result;
        }
        public string ToPageSql(Pagination pagination)
        {
            return string.Format(@"
With TTTTT AS
(
    {{0}}
)
select * from
(
    select ROW_NUMBER() OVER (ORDER BY {0}) as rownumber,*,(select count(*) from TTTTT) as rowtotal from TTTTT
) as T
where rownumber between {1} and {2}
", pagination.Sort, pagination.Start, pagination.End);
        }
    }
    public class MySqlWhereCollection : SqlWhereCollection
    {
        public override string FieldQuotationCharacterL
        {
            get
            {
                return "`";
            }
        }
        public override string FieldQuotationCharacterR
        {
            get
            {
                return "`";
            }
        }
    }
    public class SqlUpdateItem
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public Type VType { get; set; }
        public PropertyInfo PInfo { get; set; }
    }
    public abstract class BaseSqlUpdateCollection : Dictionary<string, SqlUpdateItem>// List<KeyValuePair<string, object>>
    {
        private object _model = null;

        public virtual string FieldQuotationCharacterL { get { return "["; } }
        public virtual string FieldQuotationCharacterR { get { return "]"; } }

        //private string _fieldQuotationCharacterL = "[";
        //private string _fieldQuotationCharacterR = "]";
        ///// <summary>
        ///// 字段引用字符:SqlServer中为[]  MySql中为`
        ///// </summary>
        //public string FieldQuotationCharacterL { get { return _fieldQuotationCharacterL; } set { _fieldQuotationCharacterL = value; } }
        //public string FieldQuotationCharacterR { get { return _fieldQuotationCharacterR; } set { _fieldQuotationCharacterR = value; } }
        ////private Dictionary<string, PropertyInfo> _modelProperties = null;
        public new SqlUpdateItem this[string name]
        {
            get
            {
                if (this.ContainsKey(name)) { return base[name]; }
                else if (!PFDataHelper.StringIsNullOrWhiteSpace(name)) { return base[name.ToLower()]; }//初始化时，GetProperties的Key是ToLower之后的，原来就这么用，所以就那方法了
                return null;
            }
            set
            {
                this[name] = value;
            }
        }
        public BaseSqlUpdateCollection()
        {
            //_modelProperties = new Dictionary<string, PropertyInfo>();
        }
        public BaseSqlUpdateCollection(object model, params string[] names
            )
        {
            _model = model;
            //_modelProperties = PFDataHelper.GetProperties(model.GetType());
            var modelProperties = PFDataHelper.GetProperties(model.GetType());
            if (names != null && names.Length > 0)
            {
                foreach (string i in names)
                {
                    //Add(i);
                    base.Add(i, new SqlUpdateItem { Key = i, Value = modelProperties[i].GetValue(_model, null), VType = modelProperties[i].PropertyType, PInfo = modelProperties[i] });
                }
            }
            else
            {
                foreach (var i in modelProperties)
                {
                    //Add(i.Key, i.Value.GetValue(_model, null));
                    base.Add(i.Key, new SqlUpdateItem { Key = i.Key, Value = i.Value.GetValue(_model, null), VType = i.Value.PropertyType, PInfo = i.Value });
                }
            }
        }
        public void Add<VType>(string name, VType value)
        {
            base.Add(name, new SqlUpdateItem { Key = name, Value = value, VType = typeof(VType) });
        }
        public void Add(SqlUpdateItem item)
        {
            base.Add(item.Key, item);
        }
        /// <summary>		
        /// 批量更新时为了减少应用反射的次数,只更新value		
        /// 执行5120次共花费11毫秒,重新构造整个对象的花费是此方法的2倍		
        /// 注意使用此方法的前提是:初始化时提供了model
        /// </summary>		
        public virtual void UpdateModelValue(object model)
        {
            foreach (var i in this)
            {
                i.Value.Value = i.Value.PInfo.GetValue(model, null);
            }
        }
        //protected string GetFormatValue(object val)
        //{
        //    if (val != null&&val.GetType().IsEnum) { return string.Format(" {0} ", (int)val); }
        //    if (val is string) { return string.Format(" '{0}' ", val); }
        //    if (val is decimal || val is int) { return string.Format(" {0} ", val); }
        //    return string.Format(" '{0}' ", val);
        //}
        protected string GetFormatValue(object val, Type vtype)
        {
            if (val == null
                || val == DBNull.Value //benjamin20190910
                )
            {
                if (vtype == typeof(decimal) || vtype == typeof(decimal?) || vtype == typeof(int) || vtype == typeof(int?) || vtype == typeof(DateTime) || vtype == typeof(DateTime?) || vtype == typeof(bool) || vtype == typeof(bool?)
                    || vtype == typeof(double) || vtype == typeof(double?) || vtype == typeof(System.Type)
                    )
                {
                    return " null ";
                }
                else if (vtype == typeof(string))
                {
                    return " '' ";
                }
                else
                {
                    return " '' ";
                }
            }
            var nonnullType = PFDataHelper.GetNonnullType(vtype);
            if (nonnullType.IsEnum) { return string.Format(" {0} ", (int)val); }
            if (val is string)
            {
                //return string.Format(" '{0}' ", val);
                return string.Format(" '{0}' ", (val as string).Replace("'", "''").Replace("\\", "\\\\"));//如果字符串有单引号,会报错--benjamin20200311
            }
            if (val is decimal || val is int) { return string.Format(" {0} ", val); }
            if (val is bool) { return string.Format(" {0} ", PFDataHelper.ObjectToBool(val) == true ? 1 : 0); }
            if (val is IList<string>)//支持string[]的成员
            {
                var list = val as IList<string>;
                return string.Format(" '{0}' ", string.Join(",", list.ToArray()));
            }
            return string.Format(" '{0}' ", val);
        }
    }

    /// <summary>
    /// 生成sql的update语句的(手动指定PrimaryKeyFields可以防止漏了写where导致更新了所有数据)
    /// 
    /// 使用方法:(d支持匿名对象)
    ///var update = new SqlUpdateCollection(d)
    ///    .UpdateFields("hybh", "hyxm", "old_pv", "pv", "tcmonth", "lrmonth",
    ///    "lrman", "lrdate", "moneyflag", "agentno")
    ///    .PrimaryKeyFields("id");
    /// </summary>
    public abstract class SqlUpdateCollection<TWhereCollection> : BaseSqlUpdateCollection//<SqlUpdateCollection>
        where TWhereCollection : SqlWhereCollection, new()
    {
        protected IList<string> _updateFields;
        //protected IList<string> _keyFields;

        private TWhereCollection _where;
        //public TWhereCollection Where { get { return _where; } }
        public Dictionary<string, SqlWhereItem> PrimaryFields { get; set; }
        public SqlUpdateCollection()
            : base()
        {
        }
        public SqlUpdateCollection(object model, params string[] names
            )
            : base(model, names
                  )
        {
        }
        /// <summary>
        /// 更新的字段(用于生成set语句)
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public virtual SqlUpdateCollection<TWhereCollection> UpdateFields(params string[] names)
        {
            _updateFields = names;
            return this;
        }
        /// <summary>
        /// 主键字段(用于生成where语句)
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public virtual SqlUpdateCollection<TWhereCollection> PrimaryKeyFields(params string[] names)
        {
            return PrimaryKeyFields(true, names);
            //_where = new SqlWhereCollection { };
            //foreach (var i in names)
            //{
            //    var v = this[i].Value;
            //    if (PFDataHelper.StringIsNullOrWhiteSpace((v ?? "").ToString())) { throw new Exception("更新时where条件不能为空!"); }
            //    //_where.Add(i, this[i].Value);//这样不保险，因为names有可能是大写的，但this[i].Key有可能是小写的
            //    _where.Add(this[i].Key, this[i].Value);
            //}
            ////_primaryKeyFields = names;
            //if (_updateFields == null)//一般来讲,除去主键之外的字段都应该要更新
            //{
            //    _updateFields = this.Select(a => a.Key).Where(a => !names.Contains(a)).ToList();
            //}
            //return this;
        }
        public SqlUpdateCollection<TWhereCollection> PrimaryKeyFields(bool checkWhereNotNull, params string[] names)
        {
            //_keyFields = new List<string>();
            _where = new TWhereCollection { };
            PrimaryFields = new Dictionary<string, SqlWhereItem>();
            foreach (var i in names)
            {
                var v = this[i].Value;
                if (checkWhereNotNull && PFDataHelper.StringIsNullOrWhiteSpace((v ?? "").ToString())) { throw new Exception("更新时where条件不能为空!"); }
                ////_where.Add(i, this[i].Value);//这样不保险，因为names有可能是大写的，但this[i].Key有可能是小写的
                //_where.Add(this[i].Key, this[i].Value);
                var whereItem = new SqlWhereItem(this[i].Key, this[i].Value);
                _where.Add(whereItem);
                PrimaryFields.Add(this[i].Key, whereItem);
            }
            //_primaryKeyFields = names;
            if (_updateFields == null)//一般来讲,除去主键之外的字段都应该要更新
            {
                _updateFields = this.Select(a => a.Key).Where(a => !names.Contains(a)).ToList();
            }
            return this;
        }
        /// <summary>		
        /// 批量更新时为了减少应用反射的次数,只更新value		
        /// 执行5120次共花费11毫秒,重新构造整个对象的花费是此方法的2倍		
        /// </summary>		
        public void UpdateByDataReader(IDataReader dr)
        {
            foreach (var i in this)
            {
                //i.Value.Value = i.Value.PInfo.GetValue(model, null);
                if (!_where.Any(a => a.Key == i.Key))
                {
                    i.Value.Value = dr[i.Key];
                }
            }
            foreach (var i in _where)
            {
                i.Value = dr[i.Key];
            }
        }

        //public void UpdateByDict(Dictionary<string, object> model)
        //{
        //    foreach (var i in this)
        //    {
        //        if (model.ContainsKey(i.Key))
        //        {
        //            i.Value.Value = model[i.Key];
        //        }
        //    }
        //    foreach (var i in _where)
        //    {
        //        if (model.ContainsKey(i.Key))
        //        {
        //            i.Value = model[i.Key];
        //        }
        //    }
        //}

        public void Set(string key, object value)
        {
            if (PrimaryFields.ContainsKey(key))
            {
                _where.First(a => a.Key == key).Value = value;
            }
            if (this.ContainsKey(key))
            {
                this[key].Value = value;
            }
        }
        /// <summary>
        /// 格式如:name1='value1',name2='value2',name3=time3,...
        /// </summary>
        /// <returns></returns>
        public string ToSetSql()
        {
            int count = 0;
            string s1 = "";
            foreach (var i in _updateFields)
            {
                if (count != 0) { s1 += ","; }
                //s1 += ("[" + i + "]" + "=");
                s1 += string.Format("{0}{1}{2}=", FieldQuotationCharacterL, i, FieldQuotationCharacterR);

                s1 += GetFormatValue(this[i].Value, this[i].VType);
                count++;
            }

            return s1;
        }
        /// <summary>
        /// 返回格式如: where xx=xx and ...
        /// </summary>
        /// <returns></returns>
        public string ToWhereSql(SqlWhereCollection.WhereOrAnd woa = SqlWhereCollection.WhereOrAnd.Where)
        {
            return _where.ToSql(woa);
        }
        public override void UpdateModelValue(object model)
        {
            base.UpdateModelValue(model);
            foreach (var i in _where)
            {
                i.Value = this[i.Key].PInfo.GetValue(model, null);
            }
        }
    }

    public class SqlUpdateCollection : SqlUpdateCollection<SqlWhereCollection>
    {
        public SqlUpdateCollection() : base() { }
        public SqlUpdateCollection(object model, params string[] names
            )
            : base(model, names
                  )
        {
        }
        public new SqlUpdateCollection UpdateFields(params string[] names)
        {
            base.UpdateFields(names);
            return this;
        }
        /// <summary>
        /// 主键字段(用于生成where语句)
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public new SqlUpdateCollection PrimaryKeyFields(params string[] names)
        {
            base.PrimaryKeyFields(names);
            return this;
        }
    }
    public class MySqlUpdateCollection : SqlUpdateCollection<MySqlWhereCollection>
    {
        public override string FieldQuotationCharacterL
        {
            get
            {
                return "`";
            }
        }
        public override string FieldQuotationCharacterR
        {
            get
            {
                return "`";
            }
        }
    }
    /// <summary>
    /// 生成sql的insert语句的(注意,与updateCollection不同的是,insertCollection在初始化时已经决定了生成sql的字段)
    /// </summary>
    public class SqlInsertCollection : BaseSqlUpdateCollection//<SqlInsertCollection>
    {
        //private IList<string> _updateFields;
        public SqlInsertCollection()
            : base()
        {
        }
        public SqlInsertCollection(object model, params string[] names
            )
            : base(model, names
                  )
        {
        }

        /// <summary>
        /// 格式如:name1,name2,...
        /// </summary>
        /// <returns></returns>
        public string ToKeysSql()
        {
            int count = 0;
            string s1 = "";
            foreach (var i in this)
            {
                var key = string.Format("{0}{1}{2}", FieldQuotationCharacterL, i.Key, FieldQuotationCharacterR);
                s1 += count == 0 ? key : ("," + key);
                count++;
            }
            s1 += "";
            return s1;
        }
        /// <summary>
        /// 格式如:'value1','value2',time1,int1
        /// </summary>
        /// <returns></returns>
        public string ToValuesSql()
        {
            int count = 0;
            string s2 = "";
            foreach (var i in this)
            {
                s2 += count == 0 ? "" : ",";

                var v = i.Value;
                s2 += GetFormatValue(v.Value, v.VType);
                count++;
            }
            s2 += "";
            return s2;
        }
    }
    public class MySqlInsertCollection : SqlInsertCollection
    {
        public override string FieldQuotationCharacterL
        {
            get
            {
                return "`";
            }
        }
        public override string FieldQuotationCharacterR
        {
            get
            {
                return "`";
            }
        }
        public MySqlInsertCollection()
            : base()
        {
        }
        public MySqlInsertCollection(object model, params string[] names
            )
            : base(model, names
                  )
        {
        }
    }
    #endregion SqlHelper
    public class RequestHostInfo
    {

        /// <summary>
        /// OSVersion
        /// </summary>
        public String OSVersion { get; set; } //4

        /// <summary>
        /// Browser
        /// </summary>
        public String Browser { get; set; } //5

        /// <summary>
        /// IPAddress
        /// </summary>
        public String IPAddress { get; set; } //6

        /// <summary>
        /// Location
        /// </summary>
        public String Location { get; set; } //7

        public String HostName { get; set; }
    }

    #region Cache

    public static class Caching
    {
        private static readonly string appKey = ConfigurationManager.AppSettings["AppKey"];

        public static object Get(string key)
        {
            //return HttpContext.Current.Cache.Get(appKey + key);//这种方法在async情况下Current为null--benjamin20200316
            return HttpRuntime.Cache.Get(appKey + key);
        }

        public static void Set(string key, object value, CacheDependency dependency)
        {

            if (value == null)//value为null时，Cache.Insert会报错
            {
                Remove(key);
            }
            else
            {
                /*.NET2.0 中 System.Web.Caching.Cache.NoSlidingExpiration相当于.NET1.1中TimeSpan.Zero
                *   但是在.Framework2.o的环境下，沿用1.1的写法，会失效！！！
                 */
                //HttpRuntime.Cache.Insert(appKey + key, value, dependency, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                HttpRuntime.Cache.Insert(appKey + key, value, dependency, DateTime.Now.AddDays(1), TimeSpan.Zero, CacheItemPriority.High, null);
            }
        }

        public static void Remove(string key)
        {
            //if (HttpContext.Current.Cache[appKey + key] != null)
            //{
            //    HttpContext.Current.Cache.Remove(appKey + key);
            //}
            if (HttpRuntime.Cache[appKey + key] != null)
            {
                HttpRuntime.Cache.Remove(appKey + key);
            }
        }
    }
    #endregion
    public class PFPercent { }
    public class PFDate { }
    public class PFMonth { }

    /// <summary>
    /// 功能权限(功能里有哪些按钮)
    /// </summary>
    [Flags]
    public enum FuncAuthority
    {
        /// <summary>
        /// 默认权限,如果没有设置任何Add等,就相当于All(也即权限码无后缀,如RiskManage.FXSJK).或者表示该功能不需要控制权限
        /// 注意，因为0|任何值=任何值,所以Default不会与其它值共存
        /// </summary>
        Default = 0,
        All = 1,
        /// <summary>
        /// 新增
        /// </summary>
        Add = 2,
        /// <summary>
        /// 修改
        /// </summary>
        Edit = 4,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 8,
        Import = 16,
        Export = 32
    }
    /// <summary>
    /// 系统按钮
    /// </summary>
    public class SysBtn
    {
        /// <summary>
        /// 所需权限
        /// </summary>
        public virtual FuncAuthority Authority { get; set; }
        /// <summary>
        /// 名称(英)
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 显示的文字
        /// </summary>
        public virtual string Text { get; set; }
        /// <summary>
        /// 总是显示（包含无数据时）
        /// </summary>
        public virtual bool AlwayShow { get; set; }
    }
    public class SysBtnCollection : List<SysBtn>
    {
        public void Add(string name, string text, FuncAuthority authority = FuncAuthority.Default, bool alwayShow = false)
        {
            Add(new SysBtn { Name = name, Text = text, Authority = authority, AlwayShow = alwayShow });

        }
        public SysBtnCollection FilterByAuthority(FuncAuthority funcAuthority)
        {
            //if (funcAuthority.HasFlag(FuncAuthority.All)) { return this; }
            if (PFDataHelper.EnumHasFlag(funcAuthority, FuncAuthority.All)) { return this; }
            var result = new SysBtnCollection();
            foreach (var i in this)
            {
                //if (funcAuthority.HasFlag(i.Authority)) { result.Add(i); }
                if (PFDataHelper.EnumHasFlag(funcAuthority, i.Authority)) { result.Add(i); }
            }
            return result;
        }
    }
    public partial class PagingParameters
    {
        private int _pageIndex = 0;
        private int? _pageSize = null;
        //private NameValueCollection request { get; set; }
        public Dictionary<string, object> Request { get; set; }//改为public是为了便于在使用PFEmailMq时作为请求参数使用(因为JsonConver不会序列化private)(另外,原来的NameValueCollection格式在JsonConvert后会变为string[],原因不明)

        public int? PageIndex { get { return _pageIndex; } set { if (value.HasValue) { _pageIndex = value.Value; } } }//从0开始
        //public int? PageSize { get { return _pageSize; } set { if (value.HasValue) { _pageSize = value.Value; } } }
        public int? PageSize { get { return _pageSize; } set { _pageSize = value; } }
        public int? PageStart { get { return PageIndex == null || PageSize == null ? null : PageIndex * PageSize; } }//从0开始
        public int? PageEnd { get { return PageStart == null ? null : PageStart + PageSize - 1; } }//这个值有可能超出Table的Rows索引

        public string Sort { get; set; }
        /// <summary>
        /// 模糊查找
        /// </summary>
        public string FilterValue { get; set; }

        ///// <summary>
        ///// 是否导出(除了sql分页的,都导出全部)(其实,sql分页的没必要导出了)
        ///// </summary>
        //public bool IsExport { get; set; }
        /// <summary>
        /// 显示所有用户可见列(因为增加功能用户可设置可见列之后,设置窗口的调用方法需显示全部列)(可空是因为前端不提交绑定会失败)
        /// </summary>
        public bool? ShowAllColumn = false;
        /// <summary>
        /// 不需要数据(对于自定义分组等,只需要列属性就行了,可以提高效率)--benjamin20200408
        /// </summary>
        public bool? OnlyNeedOneRow = false;

        public string this[string name]
        {
            get
            {
                //var field = string.Empty;
                //if (name.IndexOf(".") >= 0) field = name.Split('.')[1];
                //return Request[field] ?? Request[name]
                //    ;

                var field = string.Empty;
                if (name.IndexOf(".") >= 0)
                {
                    field = name.Split('.')[1];
                }
                else
                {
                    field = name;
                }
                //return PFDataHelper.ObjectToString(Request[field]);
                //改为字典后,这样会报错--benjamin 20200215
                if (Request.ContainsKey(field))
                {
                    return PFDataHelper.ObjectToString(Request[field]);
                }
                return null;
            }
            set
            {
                if (name.IndexOf(".") >= 0) { name = name.Split('.')[1]; }


                int ti = 0;
                var lowerName = name.ToLower();
                if (lowerName == "pageindex" && int.TryParse(value, out ti))
                {
                    PageIndex = ti;
                }
                else if (lowerName == "pagesize" && int.TryParse(value, out ti))
                {
                    PageSize = ti;
                }
                else if (lowerName == "sort")
                {
                    Sort = value;
                }
                else if (lowerName == "filtervalue")
                {
                    FilterValue = value;
                }
                //if (Request == null) { Request = new NameValueCollection(); }
                if (Request == null) { Request = new Dictionary<string, object>(); }
                Request[name] = value;
            }
        }
        public PagingParameters(NameValueCollection query)
        {
            this.SetRequestData(query);
        }

        public PagingParameters()
        {
            this.SetRequestData(new NameValueCollection());
        }
        #region old
        //public PagingParameters SetRequestData(NameValueCollection values)
        //{
        //    this.request = values;
        //    foreach (var i in values.AllKeys)
        //    {
        //        int ti = 0;
        //        if (i.ToLower() == "pageindex" && int.TryParse(values[i], out ti))
        //        {
        //            PageIndex = ti;
        //        }
        //        else
        //        if (i.ToLower() == "pagesize" && int.TryParse(values[i], out ti))
        //        {
        //            PageSize = ti;
        //        }
        //        else
        //        if (i.ToLower() == "sort")
        //        {
        //            Sort = values[i];
        //        }
        //        else
        //        if (i.ToLower() == "filtervalue")
        //        {
        //            FilterValue = values[i];
        //        }
        //    }
        //    return this;
        //} 
        #endregion
        public PagingParameters SetRequestData(NameValueCollection values)
        {
            foreach (var i in values.AllKeys)
            {
                this[i] = values[i];
            }
            return this;
        }

        public PagingParameters SetRequestData(JToken values)
        {
            if (values != null)
            {
                foreach (JProperty item in values.Children())
                {
                    if (item != null)
                    {
                        this[item.Name] = item.Value.ToString();
                    }
                }
            }
            return this;
        }
    }
    /// <summary>
    /// 分页查询结果
    /// </summary>
    public class PagingResult : IDisposable
    {
        public IList data { get; set; }
        public object exData { get; set; }//为了减少前端多次请求,便于放其它数据
        public StoreColumnCollection columns { get; set; }
        public int total { get; set; }
        public string TableCacheKey { get; set; }

        public void Dispose()
        {
            //data.Clear();
            data = null;//2329-700 =1662
            exData = null;
            columns = null;
            PFDataHelper.GCCollect();
            //GC.Collect();
            //var aa = "aa";
            //throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 汇总类型
    /// </summary>
    public enum SummaryType
    {
        None = 0,
        Sum = 1,
        Average = 2,
        Count = 4
    }    /// <summary>
         /// 用于前端pfTable和pfTreeTable的列的json格式
         /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class StoreColumn : TreeListItem<StoreColumn>
    {
        private int _rowspan = 1;
        private int _colspan = 1;
        private bool _visible = true;
        private bool _hasSummary = false;
        private SummaryType _summaryType = SummaryType.None;
        public string data { get; set; }
        public string title { get; set; }
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string width { get; set; }//需然FieldSets里面不写px单位,但设到这里时补上
        //[DefaultValue(null)]
        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        //public int? precision { get; set; }
        [JsonIgnore]//现在的excel是后端导出的，前端应该用不着这个属性--benjamin20190704
        public double? excelWidth
        {
            get
            {
                return PFDataHelper.WebWidthToExcel(width);
            }
            set
            {
                if (value == null) { width = null; }
                else
                {
                    width = PFDataHelper.ExcelWidthToWeb(value.Value);
                }
            }
        }//需然FieldSets里面不写px单位,但设到这里时补上
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int rowspan { get { return _rowspan; } set { _rowspan = value; } }
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int colspan { get { return _colspan; } set { _colspan = value; } }
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string dataType { get; set; }
        [DefaultValue(true)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool visible { get { return _visible; } set { _visible = value; } }

        [JsonIgnore]
        public bool hasSummary { get { return _hasSummary; } set { if (value) { _summaryType = SummaryType.Sum; } _hasSummary = value; } }
        [JsonIgnore]
        public SummaryType summaryType { get { return _summaryType; } set { if (value != SummaryType.None) { _hasSummary = true; } _summaryType = value; } }
        [DefaultValue("")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object summary { get; set; }

        /// <summary>
        /// 日期显示格式
        /// </summary>
        [DefaultValue("")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string dateFormat { get; set; }
        public StoreColumn() { }
        public StoreColumn(string fieldName)
        {
            title = data = fieldName;
        }
        public StoreColumn(DataColumn c)
            : this(c.ColumnName)
        {
            dataType = PFDataHelper.GetStringByType(c.DataType);
            if (c.ExtendedProperties.Contains("summaryType"))
            {
                summaryType = PFDataHelper.ObjectToEnum<SummaryType>(c.ExtendedProperties["summaryType"]) ?? SummaryType.None;
            }
        }
        public StoreColumn(DataColumn c, PFModelConfig config)
            //: this(c.ColumnName)
            : this(c)
        {
            //dataType = PFDataHelper.GetStringByType(c.DataType);
            SetPropertyByModelConfig(config);
        }
        public StoreColumn(string fieldName, PFModelConfig config)
            : this(fieldName)
        {
            SetPropertyByModelConfig(config);
        }
        public void SetPropertyByModelConfig(PFModelConfig config)
        {
            if (config != null)
            {
                //data = config.FieldName;
                title = config.FieldText;
                if (!PFDataHelper.StringIsNullOrWhiteSpace(config.FieldWidth)) { width = config.FieldWidth; }
                if (config.FieldType != null) { dataType = PFDataHelper.GetStringByType(config.FieldType); }
                visible = config.Visible;
                //if (config.Precision != null) { precision = config.Precision; }
            }
        }

        public void SetWidthByTitleWords()
        {
            if (PFDataHelper.StringIsNullOrWhiteSpace(title)) { return; }
            width = PFDataHelper.GetWordsWidth(title, null, null, null, "bold");
        }
    }
    public class StoreColumnCollection : List<StoreColumn>
    {
        protected PFModelConfigCollection _modelConfig;
        public StoreColumnCollection() { }
        public StoreColumnCollection(string modelConfig)
        {
            _modelConfig = PFDataHelper.GetMultiModelConfig(modelConfig);
        }
        //public StoreColumn FirstLeaf(Func<StoreColumn, bool> predicate)
        //{
        //    return (new StoreColumn { Children = this }).FirstLeaf(predicate);
        //}
        public void Add(string data, Action<StoreColumn> action = null)//, bool setWidthByHeaderWord = true)
        {
            var config = _modelConfig == null ? null : _modelConfig[data];
            var c = new StoreColumn(data, config);

            if (action != null) { action(c); }//action里有可能改title,所以此句一定在GetWordsWidth之前--wxj20180906

            //if (//setWidthByHeaderWord&&
            //    PFDataHelper.StringIsNullOrWhiteSpace(c.width))//如果xml配置中有宽度,就不用计算字长了
            //{
            //    var w = PFDataHelper.GetWordsWidth(c.title ?? c.data);
            //    if (!PFDataHelper.StringIsNullOrWhiteSpace(w)) { c.width = w; }
            //}

            Add(c);
        }
        public void Add(DataColumn column, Action<StoreColumn> action = null)//, bool setWidthByHeaderWord = true)
        {
            var config = _modelConfig == null ? null : _modelConfig[column.ColumnName];
            var c = new StoreColumn(column, config);

            if (action != null) { action(c); }//action里有可能改title,所以此句一定在GetWordsWidth之前--wxj20180906

            //if (//setWidthByHeaderWord&&
            //    PFDataHelper.StringIsNullOrWhiteSpace(c.width))//如果xml配置中有宽度,就不用计算字长了
            //{
            //    var w = PFDataHelper.GetWordsWidth(c.title ?? c.data);
            //    if (!PFDataHelper.StringIsNullOrWhiteSpace(w)) { c.width = w; }
            //}

            Add(c);
        }
        /// <summary>
        /// 树型转二维数组(现在只有导出excel时用到,所以默认过滤了visible==false的数据
        /// </summary>
        /// <returns></returns>
        public static void StoreColumnTo2DArray(ref List<List<StoreColumn>> target, StoreColumnCollection columns, ref int maxDepth)
        {
            //var result = new List<List<StoreColumn>>();
            var floor = new List<StoreColumn>();
            var next = new StoreColumnCollection();
            var currentDepth = target.Count + 1;
            var rowSpan = maxDepth - currentDepth + 1;
            columns.ForEach(a =>
            {
                if (a.visible)
                {
                    var children = a.Children.Where(b => b.visible);
                    //if (a.Children.Any(b => b.visible))
                    if (children != null && children.Any())
                    {
                        //next.AddRange(a.Children);
                        next.AddRange(children);
                        //a.colspan = a.Children.Count;
                        //a.colspan = a.GetAllLeafCount();
                        a.colspan = a.GetAllLeafCount(b => b.visible);
                    }
                    else
                    {
                        a.rowspan = rowSpan;
                    }
                    //a.Children = null;
                    floor.Add(a);
                }
            });
            target.Add(floor);
            if (next.Any())
            {
                StoreColumnTo2DArray(ref target, next, ref maxDepth);
            }
        }
    }

    /// <summary>
    /// 如果Data用object,当Data是复杂类型时,WebService无法序列化,所以改为泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PFJsonData<T> : PFJsonData
    {
        public T Data { get; set; }
        //public static PFJsonData SetSuccess()
        //{
        //    return new PFJsonData
        //    {
        //        Result = true
        //    };
        //}

        public static PFJsonData<T> SetSuccess(T data)
        {
            return new PFJsonData<T>
            {
                Result = true,
                Data = data
            };
        }
        public new static PFJsonData<T> SetFault(string message)
        {
            return new PFJsonData<T>
            {
                Message = message,
                Result = false
            };
        }
    }
    [Serializable]
    public class PFJsonData
    {
        //public T Data { get; set; }
        public string Message { get; set; }
        public bool Result { get; set; }
        public string Url { get; set; }
        public string HtmlPartial { get; set; }
        public string HtmlPartial2 { get; set; }
        public string HtmlPartial3 { get; set; }
        public static PFJsonData<T> SetSuccess<T>(bool result, T data, string message)
        {
            return new PFJsonData<T>
            {
                Message = message,
                Result = result,
                Data = data
            };
        }
        //public static PFJsonData<T> SetSuccess(string message)
        //{
        //    return new PFJsonData<T>
        //    {
        //        Message = message,
        //        Result = true
        //    };
        //}

        ////public static JsonData SetBizResult(BizResult result)
        ////{
        ////    return new JsonData
        ////    {
        ////        Message = result.Message,
        ////        Result = result.Success,
        ////        Data = result.GetData()
        ////    };
        ////}

        //public static PFJsonData SetResult(bool result)
        //{
        //    return new PFJsonData
        //    {
        //        Result = result
        //    };
        //}

        //public static PFJsonData SetResult(bool result, string msg)
        //{
        //    return new PFJsonData
        //    {
        //        Result = result,
        //        Message = msg
        //    };
        //}

        public static PFJsonData SetSuccess()
        {
            return new PFJsonData
            {
                Result = true
            };
        }

        public static PFJsonData<T> SetSuccess<T>(T data)
        {
            return new PFJsonData<T>
            {
                Result = true,
                Data = data
            };
        }

        //public static PFJsonData SetSuccess(string message, object data)
        //{
        //    return new PFJsonData
        //    {
        //        Data = data,
        //        Message = message,
        //        Result = true
        //    };
        //}

        //public static PFJsonData SetSuccess(string message, string url)
        //{
        //    return new PFJsonData
        //    {
        //        Url = url,
        //        Message = message,
        //        Result = true
        //    };
        //}

        //public static PFJsonData SetSuccess(string message, object data, string url)
        //{
        //    return new PFJsonData
        //    {
        //        Data = data,
        //        Message = message,
        //        Url = url,
        //        Result = true
        //    };
        //}

        public static PFJsonData SetFault(string message)
        {
            return new PFJsonData
            {
                Message = message,
                Result = false
            };
        }

        //public static PFJsonData SetFault()
        //{
        //    return new PFJsonData
        //    {
        //        Result = false
        //    };
        //}

        //public static PFJsonData SetFault(string message, string url)
        //{
        //    return new PFJsonData
        //    {
        //        Url = url,
        //        Message = message,
        //        Result = false
        //    };
        //}
    }

    [Serializable]
    public class PFKeyValue<TValue>
    {
        public PFKeyValue() { }
        public string Key { get; set; }
        public TValue Value { get; set; }
    }
    /// <summary>
    /// 可序列化键值对(便于webService返回类型,Value必需也是可序列化的类型或简单类型)
    /// </summary>
    [Serializable]
    public class PFKeyValueCollection<TValue> : List<PFKeyValue<TValue>>
    {
        public PFKeyValueCollection(IEnumerable<PFKeyValue<TValue>> list)
            : base(list)
        {
        }
        //private string _keys = ",";
        public string[] Keys { get { return this.Select(a => a.Key).ToArray(); } }
        public TValue this[string name]
        {
            get
            {
                var item = this.FirstOrDefault(a => a.Key == name);
                //return item == null ? null : item.Value;
                return item == null ? default(TValue) : item.Value;
            }
            set
            {
                var item = this.FirstOrDefault(a => a.Key == name);
                if (item == null) { this.Add(new PFKeyValue<TValue> { Key = name, Value = value }); }
                else { item.Value = value; }
            }
        }
        public PFKeyValueCollection()
            : base()
        { }
        //public new void Add(PFKeyValue<TValue> item){
        //    base.Add(item);
        //    //AddKey(item.Key);
        //}
        public void Add(string key, TValue value)
        {
            Add(new PFKeyValue<TValue> { Key = key, Value = value });
            //AddKey(key);
        }
        //public bool ContainsKey(string key) {
        //    return _keys.IndexOf("," + key + ",") > -1;
        //}
        //private void AddKey(string key)
        //{
        //    if (!ContainsKey(key))
        //    {
        //        _keys += key + ",";
        //    }
        //}
    }
    #region 自定义类型基类
    #region 这种方式不好用,因为反射时无法和String转换,没找到办法(可能是因为CanRead为false?)
    public class PFCustomTypeJsonConverter : JsonConverter
    {
        //是否开启自定义反序列化，值为true时，反序列化时会走ReadJson方法，值为false时，不走ReadJson方法，而是默认的反序列化
        //public override bool CanRead => false;//net2.0不支持这种写法
        public override bool CanRead { get { return false; } }
        //是否开启自定义序列化，值为true时，序列化时会走WriteJson方法，值为false时，不走WriteJson方法，而是默认的序列化
        //public override bool CanWrite => true;//net2.0不支持这种写法
        public override bool CanWrite { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return typeof(PFCustomStringType) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((PFCustomStringType)value).ToString());
        }
    }
    public class PFCustomTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(object))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string) || destinationType == typeof(object))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new PFCustomStringType(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var shippingOption = value as PFCustomStringType;
                if (shippingOption != null)
                {
                    return shippingOption.ToString();
                }
                else
                {
                    return "";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    /// <summary>
    /// 自定义类型基类
    /// 使用方法:
    ///public class CustomType1 : PFCustomType<string>
    ///{
    ///    public CustomType1(string v) : base(v)
    ///    {
    ///    }
    ///    public static CustomType1 AliceBlue { get { return new CustomType1("AliceBlue"); } }
    ///}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonConverter(typeof(PFCustomTypeJsonConverter))]
    [TypeConverter(typeof(PFCustomTypeConverter))]
    [Obsolete]
    public class PFCustomStringType : IEquatable<PFCustomStringType>, IConvertible
    {
        protected string _value;
        public static readonly PFCustomStringType Empty;
        public PFCustomStringType(string v)
        {
            _value = v;
        }
        //protected static PFCustomStringType FromName(string v)
        //{
        //    var r = new PFCustomStringType(); r._value = v; return r;
        //}
        //protected  PFCustomStringType FromName(string v)
        //{
        //    var r = new PFCustomStringType(); r._value = v; return r;
        //}
        public override bool Equals(object other)
        {
            return ((PFCustomStringType)other)._value == this._value;
        }
        public bool Equals(PFCustomStringType other)
        {
            return other._value == this._value;
        }

        public override string ToString()
        {
            return _value;
        }

        public static bool operator ==(PFCustomStringType left, PFCustomStringType right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(PFCustomStringType left, PFCustomStringType right)
        {
            return left._value != right._value;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.String;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return _value != null;
        }

        public char ToChar(IFormatProvider provider)
        {
            return _value[0];
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return _value;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        //  User-defined conversion from double to Digit
        public static implicit operator PFCustomStringType(String d)
        {
            return new PFCustomStringType(d);
        }
    }
    #endregion
    #region old任何类型
    //public class PFCustomTypeConverter : JsonConverter
    //{
    //    //是否开启自定义反序列化，值为true时，反序列化时会走ReadJson方法，值为false时，不走ReadJson方法，而是默认的反序列化
    //    public override bool CanRead => false;
    //    //是否开启自定义序列化，值为true时，序列化时会走WriteJson方法，值为false时，不走WriteJson方法，而是默认的序列化
    //    public override bool CanWrite => true;

    //    public override bool CanConvert(Type objectType)
    //    {
    //        return typeof(PFCustomType<>) == objectType.GetGenericTypeDefinition();
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue(value.ToString());
    //    }
    //}
    //[JsonConverter(typeof(PFCustomTypeConverter))]
    //public class PFCustomType<T> : IEquatable<PFCustomType<T>>
    //{
    //    private T _value;
    //    public static readonly PFCustomType<T> Empty;
    //    public PFCustomType(T v)
    //    {
    //        _value = v;
    //    }
    //    //protected PFCustomType<T> FromName(string v)
    //    //{
    //    //    var r = new PFCustomType<T>(); r._value = v; return r;
    //    //}
    //    //protected PFCustomType<T> FromName(string v)
    //    //{
    //    //    this._value = v;
    //    //    return this;
    //    //}
    //    public override bool Equals(object other)
    //    {
    //        return other.ToString() == this.ToString();
    //        //return true;
    //        //throw new NotImplementedException();
    //    }
    //    public bool Equals(PFCustomType<T> other)
    //    {
    //        return other.ToString() == this.ToString();
    //        //throw new NotImplementedException();
    //    }

    //    public override string ToString()
    //    {
    //        //if(this.ToString()== MessageReqTypeDto.sms.ToString()) { return "sms"; }
    //        return _value.ToString();
    //    }


    //    //
    //    // 摘要:
    //    //     测试两个指定的 System.Drawing.Color 结构是否等效。
    //    //
    //    // 参数:
    //    //   left:
    //    //     相等运算符左侧的 System.Drawing.Color。
    //    //
    //    //   right:
    //    //     相等运算符右侧的 System.Drawing.Color。
    //    //
    //    // 返回结果:
    //    //     如果两个 System.Drawing.Color 结构相等，为 true；否则为 false。
    //    public static bool operator ==(PFCustomType<T> left, PFCustomType<T> right)
    //    {
    //        return left.ToString() == right.ToString();
    //    }
    //    //
    //    // 摘要:
    //    //     测试两个指定的 System.Drawing.Color 结构是否不同。
    //    //
    //    // 参数:
    //    //   left:
    //    //     不等运算符左侧的 System.Drawing.Color。
    //    //
    //    //   right:
    //    //     不等运算符右侧的 System.Drawing.Color。
    //    //
    //    // 返回结果:
    //    //     如果两个 System.Drawing.Color 结构不同，为 true；否则为 false。
    //    public static bool operator !=(PFCustomType<T> left, PFCustomType<T> right)
    //    {
    //        return left.ToString() != right.ToString();
    //    }
    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }
    //    //public MessageReqTypeDto sms { get { return Empty; } }
    //    //public string email = "email";
    //    //public string app = "app";
    //    //public string inMail = "in-mail";
    //}  
    #endregion
    #endregion 自定义类型基类
    //public class PagedBase
    //{
    //    public int rowtotal;
    //}
    /// <summary>
    /// 生成图片验证码
    /// </summary>
    public partial class PFValidateCode
    {

        public static string GenerateCheckCode()
        {
            //创建整型型变量
            int number;
            //创建字符型变量
            char code;
            //创建字符串变量并初始化为空
            string checkCode = String.Empty;
            //创建Random对象
            Random random = new Random();
            //使用For循环生成4个数字
            for (int i = 0; i < 4; i++)
            {
                //生成一个随机数
                number = random.Next();
                //将数字转换成为字符型
                code = (char)('0' + (char)(number % 10));

                checkCode += code.ToString();
            }

            return checkCode;
        }

        public static byte[] CreateCheckCodeImage(string checkCode)
        {
            //判断字符串不等于空和null
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return null;
            //创建一个位图对象
            Bitmap image = new Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
            //创建Graphics对象
            Graphics g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器
                Random random = new Random();

                //清空图片背景色
                g.Clear(Color.White);

                //画图片的背景噪音线
                for (int i = 0; i < 2; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                }

                System.Drawing.Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的前景噪音点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //将图片输出到页面上
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
    public class HttpPostOption
    {
        private bool _keepAlive = true;
        public bool KeepAlive { get { return _keepAlive; } set { _keepAlive = value; } }

        public CookieContainer Cookie { get; set; }
        public List<KeyValuePair<string, string>> Header { get; set; }
        /// <summary>
        /// GET POST
        /// </summary>
        public string HttpMethod { get; set; }
    }

    /// <summary>
    /// 代替4.0的ConcurrentDictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PFThreadDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        where TValue : new()
    {
        private ReaderWriterLockSlim _addLock = new ReaderWriterLockSlim();
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value = new TValue();
            try
            {
                _addLock.EnterWriteLock();
                if (ContainsKey(key))
                {
                }
                else
                {
                    this[key] = valueFactory(key);
                }
                value = this[key];
            }
            catch (Exception e)
            {
                var msg = string.Format("PFThreadDictionary.GetOrAdd方法报错,当前key:{0},错误信息:{1}", key, e);
                PFDataHelper.WriteError(new Exception(msg));
            }
            finally
            {
                _addLock.ExitWriteLock();
            }
            return value;
        }
    }

    ///// <summary>
    ///// 执行sql的情况
    ///// </summary>
    //public enum SqlExecStatus
    //{
    //    /// <summary>
    //    /// 失败
    //    /// </summary>
    //    Error = 0,
    //    /// <summary>
    //    /// 成功
    //    /// </summary>
    //    Success = 1,
    //    /// <summary>
    //    /// 部分失败
    //    /// </summary>
    //    PartError = 2
    //}
    /// <summary>
    /// 执行sql的情况
    /// </summary>
    public enum MethodExecStatus
    {
        /// <summary>
        /// 失败
        /// </summary>
        Error = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 部分失败
        /// </summary>
        PartError = 2
    }
    public class PFTimeSpan
    {
        private PFYmd _ymd;
        public PFTimeSpan(PFYmd ymd = PFYmd.Hour | PFYmd.Minute | PFYmd.Second)
        {
            _ymd = ymd;
        }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Millisecond { get; set; }
        public override string ToString()
        {
            var s = "";
            if (PFDataHelper.EnumHasFlag(_ymd, PFYmd.Hour))
            {
                s += Hour + "时";
            }
            if (PFDataHelper.EnumHasFlag(_ymd, PFYmd.Minute))
            {
                s += Minute + "分";
            }
            if (PFDataHelper.EnumHasFlag(_ymd, PFYmd.Second))
            {
                s += Second + "秒";
            }
            if (PFDataHelper.EnumHasFlag(_ymd, PFYmd.Millisecond))
            {
                s += Millisecond + "毫秒";
            }
            return s;
            //return string.Format("{0}时{1}分{2}秒{3}毫秒", Hour,Minute,Second,Millisecond);
        }
    }
    [Flags]
    public enum PFYmd
    {
        Hour = 1,
        Minute = 2,
        Second = 4,
        Millisecond = 8
    }

    [Flags]
    public enum PFCharType
    {
        Default = 0,
        Chinese = 1,
        English = 2,
        Number = 4,
        /// <summary>
        /// 全角符号
        /// </summary>
        FullChar = 8,
        /// <summary>
        /// 半角符号
        /// </summary>
        HalfChar = 16
    }
    public enum PFEmailItemType
    {

        Default,
        Subject,
        From,
        Date,
        ContentType,
        ContentTransferEncoding,
        //Boundary,
        XPriority,
        XMailer,
        XCMTRANSID,
        /// <summary>
        /// Mime-Version
        /// </summary>
        MimeVersion,
        Body
    }

    public enum PFEncodeType
    {
        /// <summary>
        /// base64
        /// </summary>
        Base64,
        /// <summary>
        /// B? 或 8Bit
        /// </summary>
        Bit8,
        /// <summary>
        /// Q?
        /// </summary>
        QuotedPrintable,
        UTF8,
        GB18030,
        GB2312,
        GBK
    }
    //public enum PFEmailCharsetType {
    //    UTF8,
    //    GB18030
    //}
    public enum PFEmailTransferEncodeType
    {
        /// <summary>
        /// 8Bit
        /// </summary>
        Bit8,
        /// <summary>
        /// base64
        /// </summary>
        Base64,
        /// <summary>
        /// Q?
        /// </summary>
        QuotedPrintable
    }
    public enum PFEmailContentType
    {
        /// <summary>
        /// text/html
        /// </summary>
        TextHtml,
        /// <summary>
        /// text/plain
        /// </summary>
        TextPlain,
        /// <summary>
        /// multipart/alternative (如qq邮箱)
        /// </summary>
        MultipartAlternative,
        /// <summary>
        /// perfect99邮箱存在这种(有附件时)
        /// </summary>
        MultipartMixed
    }
    public class PFEmailBody : TreeListItem<PFEmailBody>//, ITreeListItem
    {
        public PFEmailContentType ContentType { get; set; }
        public PFEncodeType Charset { get; set; }
        public PFEmailTransferEncodeType ContentTransferEncoding { get; set; }
        /// <summary>
        /// 本条记录的Boundary
        /// </summary>
        public string Boundary { get; set; }
        /// <summary>
        /// 子级记录的Boundary
        /// </summary>
        public string ChildBoundary { get; set; }
        /// <summary>
        /// 多行body要合在一起解码,因为有可能一个中文字是中间拆开到两个line传过来的
        /// </summary>
        public List<string> EncodeBody = new List<string>();
        public string Body { get; set; }


        //public object Data { get; set; }

        //public IList GetChildren()
        //{
        //    return Children;
        //}
    }
    public class PFEmail : IDisposable
    {
        private PFEncodeType _charset = PFEncodeType.UTF8;
        /// <summary>
        /// 邮件编号
        /// </summary>
        public int MailNum { get; set; }
        /// <summary>
        /// 由于读Subject时并不能保存已经读了ContentTransferEncoding,所以最后再解码
        /// </summary>
        private List<string> EncodeSubjects = new List<string>();
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }
        public List<PFEmailBody> MultiBody { get; set; }
        public PFEmailContentType ContentType { get; set; }
        public PFEncodeType Charset { get { return _charset; } set { _charset = value; } }
        public PFEmailTransferEncodeType ContentTransferEncoding { get; set; }
        public string ChildBoundary { get; set; }
        private List<string> Boundarys = new List<string>();
        private List<string> EncodeBody = new List<string>();
        public string Body { get; set; }
        public string From { get; set; }
        public DateTime? Date { get; set; }
        public IList<string> Messages { get; set; }
        private int _idx = 0;
        //public IList<string> IsEmpty { get; set; }
        public PFEmail()
        {
        }
        //旧方法有两个问题:
        //1.中文标题没有解码;
        //2.多行标题只读了1行
        //public PFEmail(int mailNum, IList<string> messages)
        //{
        //    MailNum = mailNum;
        //    Messages = new List<string>();
        //    foreach (var s in messages)
        //    {
        //        if (s != null)
        //        {
        //            //每种邮箱的顺序是不一样的,所以不用考虑顺序问题了
        //            if (s.IndexOf("Subject: ") == 0)
        //            {
        //                Subject = s.Substring(9);
        //            }
        //            else if (s.IndexOf("From: ") == 0)
        //            {
        //                From = PFDataHelper.ObjectToString(s.Substring(6));
        //            }
        //            else if (s.IndexOf("Date: ") == 0)
        //            {
        //                Date = PFDataHelper.ObjectToDateTime(s.Substring(6));
        //            }
        //        }
        //        Messages.Add(s);
        //    }
        //}
        //private  PFEmailItemType GetCurItemTypeByLine(ref PFEmailItemType curItem,string s)
        //{
        //    var i = 0;
        //    i = s.IndexOf("Subject: ");
        //    if (i == 0)//标题有可能是两行的--benjamin todo
        //    {
        //        curItem = PFEmailItemType.Subject;
        //        //Subject+= DecodeSubject(s.Substring(9));
        //        EncodeSubjects.Add(s.Substring(9));
        //    }
        //}
        private PFEmailItemType? GetProperty(string s)
        {
            if (s != null)
            {
                //每种邮箱的顺序是不一样的,所以不用考虑顺序问题了
                if (s.IndexOf("Subject: ") == 0)//标题有可能是两行的--benjamin todo
                {
                    return PFEmailItemType.Subject;
                }
                else if (s.IndexOf("From: ") == 0)
                {
                    return PFEmailItemType.From;
                }
                else if (s.IndexOf("Date: ") == 0)
                {
                    return PFEmailItemType.Date;
                }
                else if (s.IndexOf("Content-Type: ") == 0)
                {
                    return PFEmailItemType.ContentType;
                }
                else if (s.IndexOf("Content-Transfer-Encoding: ") == 0)
                {
                    return PFEmailItemType.ContentTransferEncoding;
                }
                else if (s.IndexOf("Mime-Version: ") == 0)
                {
                    return PFEmailItemType.MimeVersion;
                }
                else if (s.IndexOf("X-Priority: ") == 0)
                {
                    return PFEmailItemType.XPriority;
                }
                else if (s.IndexOf("X-Mailer: ") == 0)
                {
                    return PFEmailItemType.XMailer;
                }
                else if (s.IndexOf("X-CM-TRANSID:") == 0)
                {
                    return PFEmailItemType.XCMTRANSID;

                }
                else if (s == "")//perfect99邮箱的正文内容似乎在""下一个line(图见D:\wxj\工作记录\20200119_CSharp接收邮件研究)
                {
                    return PFEmailItemType.Body;
                }
                else
                {
                }
            }
            return null;
        }
        [Obsolete("旧方法,不准确")]
        public PFEmail(int mailNum, IList<string> messages)
            : this(messages)
        {

        }

        private enum MessageSplitReadStep
        {
            Begin,
            Property,
            PropertyEnd,
            Body,
            BodyEnd
        }
        /// <summary>
        /// 第一次分隔消息
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="property"></param>
        /// <param name="body"></param>
        /// <param name="leftPart"></param>
        private void MessageSplit(IList<string> messages, out List<string> property, out List<string> body, out List<string> leftPart
            ,
            out string boundary, out string childBoundary
            )
        {
            ////boundary = boundary ?? "------=";
            ////string boundary = null;
            boundary = null;
            //outChildBoundary = null;
            childBoundary = null;

            property = null;
            body = null;
            leftPart = new List<string>();
            var cnt = messages.Count();

            var step = MessageSplitReadStep.Begin;

            //Func<string, bool> isBoundaryBegin = s => {
            //    return s.IndexOf("------=") == 0 ||
            //    (boundary != null && s.IndexOf(boundary) > -1);
            //};
            //boundary有个特点,就是以--开头和结束
            Func<string, bool> isBoundaryBegin = s =>
            {
                if (s == null || s.Length < 2) { return false; }
                //return s.Substring(0, 2) == "--";

                //if (boundary == null) { return false; }
                //return s == "--"+ boundary;

                return Boundarys.Any(a => s == "--" + a);
            };
            Func<string, bool> isBoundaryEnd = s =>
            {
                if (s == null || s.Length < 4) { return false; }
                //return s.Substring(0, 2) == "--" && s.Substring(s.Length - 2, 2) == "--";//头尾都是--

                //if (childBoundary == null) { return false; }
                //return s == "--" + childBoundary+"--";

                return Boundarys.Any(a => s == "--" + a + "--");
            };
            Func<string, int, bool> isPartEnd = (message, i) =>
            {
                return message == ""
                        //||
                        //        message.IndexOf("------=") == 0//part的结束位置也是这个开头,注意不一定是------=_
                        //|| i == cnt - 1
                        ;
            };
            //Action<string> findChildBoundary = (message) =>
            //{
            //    if (childBoundary == null)
            //    {
            //        var boundaryIdx = message.IndexOf("boundary");
            //        if (boundaryIdx > -1)
            //        {
            //            childBoundary = message.Substring(boundaryIdx + 10).Replace("\"", "");
            //            if (childBoundary[childBoundary.Length - 1] == ';')
            //            {
            //                childBoundary = childBoundary.Substring(0, childBoundary.Length - 1);
            //            }
            //        }
            //    }
            //};
            for (int i = 1; i < cnt; i++)
            {
                var message = messages[i];
                //findBoundary(message);
                if (step == MessageSplitReadStep.Begin)
                {
                    if (isPartEnd(message, i))
                    {
                        continue;
                    }
                    else if (isBoundaryBegin(message) && (!isBoundaryEnd(message)))
                    {
                        boundary = message.Substring(2);
                        continue;
                    }
                    else if (isBoundaryEnd(message))
                    {
                        continue;
                    }
                    else
                    {
                        step = MessageSplitReadStep.Property;
                    }
                }
                if (step == MessageSplitReadStep.Property)
                {
                    if (isPartEnd(message, i))
                    {
                        step = MessageSplitReadStep.PropertyEnd;
                        //continue;
                    }
                    else
                    {
                        //findBoundary(message);
                        //boundary存在多种情况:
                        //1.	boundary=--boundary_0_4667e9a7-6869-4818-983d-5ee64177c8a4
                        //2.	boundary="----=_001_NextPart818672378862_=----"
                        var boundaryIdx = message.IndexOf("boundary");
                        if (boundaryIdx > -1)
                        {
                            childBoundary = message.Substring(boundaryIdx + 9).Replace("\"", "");
                            if (childBoundary[childBoundary.Length - 1] == ';')
                            {
                                childBoundary = childBoundary.Substring(0, childBoundary.Length - 1);
                            }
                            if (!Boundarys.Contains(childBoundary))
                            {
                                Boundarys.Add(childBoundary);
                            }
                            //outChildBoundary = childBoundary;
                        }

                        if (property == null) { property = new List<string>(); }
                        property.Add(message);
                        continue;
                    }
                }
                if (step == MessageSplitReadStep.PropertyEnd)
                {
                    if (isBoundaryBegin(message))
                    {
                        leftPart = messages.Skip(i).ToList();
                        return;
                    }
                    if (!isPartEnd(message, i))
                    {
                        step = MessageSplitReadStep.Body;
                    }
                    //if(isPartEnd(message, i))
                    //{
                    //    step = MessageSplitReadStep.Body;
                    //}
                    //else
                    //{
                    //    step = MessageSplitReadStep.Body;
                    //    //if (body == null) { body = new List<string>(); }
                    //    //body.Add(message);
                    //    //continue;
                    //}
                }
                if (step == MessageSplitReadStep.Body)
                {
                    if (isBoundaryBegin(message))
                    {
                        leftPart = messages.Skip(i).ToList();
                        return;
                    }
                    if (isBoundaryEnd(message))
                    {
                        leftPart = messages.Skip(i).ToList();
                        return;
                    }
                    if (isPartEnd(message, i))
                    {
                        step = MessageSplitReadStep.BodyEnd;
                        //continue;
                    }
                    else
                    {
                        if (body == null) { body = new List<string>(); }
                        body.Add(message);
                        continue;
                    }
                }
                if (step == MessageSplitReadStep.BodyEnd)
                {
                    if (isBoundaryEnd(message))
                    {
                        leftPart = messages.Skip(i + 1).ToList();
                        return;
                    }
                    leftPart = messages.Skip(i).ToList();
                    return;
                }
                if (isBoundaryEnd(message))
                {
                    leftPart = messages.Skip(i + 1).ToList();
                    return;
                }
                if (isBoundaryBegin(message))
                {
                    leftPart = messages.Skip(i).ToList();
                    return;
                }

            }

        }
        #region 分隔符不准确
        ///// <summary>
        ///// 第一次分隔消息
        ///// </summary>
        ///// <param name="messages"></param>
        ///// <param name="property"></param>
        ///// <param name="body"></param>
        ///// <param name="leftPart"></param>
        //private void MessageSplit(IList<string> messages, out List<string> property, out List<string> body, out List<string> leftPart)
        //{
        //    property = null;
        //    body = null;
        //    leftPart = new List<string>();
        //    var cnt = messages.Count();

        //    for (int i = 1; i < cnt; i++)
        //    {
        //        var message = messages[i];

        //        if (message == "" ||
        //            message.IndexOf("------=") == 0//part的结束位置也是这个开头,注意不一定是------=_
        //            || i == cnt - 1
        //            )
        //        {
        //            int curRowIsContent = message != "" && message.IndexOf("------=") != 0 ? 1 : 0;//本行是否内容
        //            if (property == null)
        //            {
        //                property = messages.Take(i + curRowIsContent).ToList();
        //            }
        //            else if (property != null && body == null)
        //            {
        //                //body = string.Join("", messages.Take(i).Skip(property.Count).ToList());
        //                body = messages.Take(i + curRowIsContent).Skip(property.Count + 1).ToList();
        //            }
        //        }

        //        if (message.IndexOf("------=") == 0 || i == cnt - 1)
        //        {
        //            //if(i == cnt - 1)
        //            //{
        //            //    if (property == null)
        //            //    {
        //            //        property = messages.Take(i).ToList();
        //            //    }
        //            //    else if (property != null && body == null)
        //            //    {
        //            //        //body = string.Join("", messages.Take(i).Skip(property.Count).ToList());
        //            //        body = messages.Take(i).Skip(property.Count+1).ToList();
        //            //    }

        //            //}
        //            leftPart = messages.Skip(i).ToList();
        //            return;
        //        }
        //    }
        //    //if (property == null)
        //    //{
        //    //    property = messages.ToList();
        //    //}
        //    //else if (property != null && body == null)
        //    //{
        //    //    //body = string.Join("", messages.Take(i).Skip(property.Count).ToList());
        //    //    body = messages.ToList().Skip(property.Count).ToList();
        //    //}
        //} 
        #endregion

        //private string GetBoundary(IList<string> inmessages)
        //{          
        //    if(inmessages==null) { return null; }
        //    foreach(var msg in inmessages)
        //    {
        //        var idx = msg.IndexOf("boundary");
        //        if (idx > -1)
        //        {
        //            return msg.Substring(idx+10);
        //        }
        //    }
        //    return null;
        //}
        public PFEmail(IList<string> inmessages)
        {
            //////////PFDataHelper.WriteLocalTxt(JsonConvert.SerializeObject(inmessages), "perfect99Email2.txt");
            //////PFDataHelper.WriteLocalTxt(string.Join("\r\n", inmessages), "perfect99Email3.txt");
            //PFDataHelper.WriteLocalTxt(string.Join("\r\n", inmessages), "xxx.txt");

            if (inmessages == null) { return; }//防止报错--benjamin20200318
            List<string> property = null;
            List<string> sbody = null;
            List<string> leftPart = null;
            string boundary = null;
            string childBoundary = null;

            //var boundary = GetBoundary(inmessages);
            //string boundary = null;
            //MessageSplit(inmessages, out property, out sbody, out leftPart, out boundary);
            //MessageSplit(inmessages, out property, out sbody, out leftPart, null, out childBoundary);
            //MessageSplit(inmessages, out property, out sbody, out leftPart);
            MessageSplit(inmessages, out property, out sbody, out leftPart, out boundary, out childBoundary);
            ChildBoundary = childBoundary;

            //var aa = "aa";
            //return;


            #region 邮件主属性
            Messages = new List<string>();
            PFEmailItemType curItem = PFEmailItemType.Default;
            //int idx = 0;
            if (property != null)
            {
                foreach (var s in property)
                {
                    if (s != null)
                    {
                        //每种邮箱的顺序是不一样的,所以不用考虑顺序问题了
                        if (s.IndexOf("Subject: ") == 0)//标题有可能是两行的--benjamin todo
                        {
                            curItem = PFEmailItemType.Subject;
                            //Subject+= DecodeSubject(s.Substring(9));
                            EncodeSubjects.Add(s.Substring(9));
                        }
                        else if (s.IndexOf("From: ") == 0)
                        {
                            curItem = PFEmailItemType.From;
                            From = PFDataHelper.ObjectToString(s.Substring(6));
                        }
                        else if (s.IndexOf("Date: ") == 0)
                        {
                            curItem = PFEmailItemType.Date;
                            Date = PFDataHelper.ObjectToDateTime(s.Substring(6));
                        }
                        else if (s.IndexOf("Content-Type: ") == 0)
                        {
                            curItem = PFEmailItemType.ContentType;
                            GetContentType(s, a => ContentType = a, a => Charset = a);
                            ////perfect99邮箱里,Content-Type:和charset在同一行

                        }
                        else if (s.IndexOf("Content-Transfer-Encoding: ") == 0)
                        {
                            curItem = PFEmailItemType.ContentTransferEncoding;
                            ContentTransferEncoding = GetTransferEncodeType(s.Substring(27));
                            //if (s.Substring(27) == "base64")
                            //{
                            //    ContentTransferEncoding = PFEmailTransferEncodeType.Base64;
                            //}
                            //else if (s.Substring(27) == "8Bit")
                            //{
                            //    ContentTransferEncoding = PFEmailTransferEncodeType.Bit8;
                            //}
                        }
                        //else if (s.IndexOf("boundary=") >-1)
                        //{
                        //    var idx = s.IndexOf("boundary=");
                        //    ChildBoundary = s.Substring(idx + 10).Replace("\"","");
                        //    if (ChildBoundary[ChildBoundary.Length - 1] == ';') { ChildBoundary = ChildBoundary.Substring(0, ChildBoundary.Length - 1); }
                        //    if (!Boundarys.Contains(ChildBoundary)) { Boundarys.Add(ChildBoundary); }
                        //}
                        else if (s.IndexOf("Mime-Version: ") == 0)
                        {
                            curItem = PFEmailItemType.MimeVersion;
                        }
                        else if (s.IndexOf("X-Priority: ") == 0)
                        {
                            curItem = PFEmailItemType.XPriority;
                        }
                        else if (s.IndexOf("X-Mailer: ") == 0)
                        {
                            curItem = PFEmailItemType.XMailer;
                        }
                        else if (s.IndexOf("X-CM-TRANSID:") == 0)
                        {
                            curItem = PFEmailItemType.XCMTRANSID;

                        }
                        else if (s == "")//perfect99邮箱的正文内容似乎在""下一个line(图见D:\wxj\工作记录\20200119_CSharp接收邮件研究)
                        {
                            curItem = PFEmailItemType.Body;
                        }
                        else
                        {
                            if (curItem == PFEmailItemType.Subject)
                            {
                                //Subject += DecodeSubject(s);
                                EncodeSubjects.Add(s);
                            }
                            else if (curItem == PFEmailItemType.Body)
                            {
                                if (ContentType == PFEmailContentType.MultipartAlternative
                                    //&&s== "This is a multi-part message in MIME format."//只有QQ邮箱才有这句
                                    )
                                {
                                    MultiBody = new List<PFEmailBody>();
                                    var bodys = GetMultiBody(property.Skip(_idx).ToList());
                                    PFEmailBody body = null;
                                    body = bodys.FirstOrDefault(a => a.ContentType == PFEmailContentType.TextHtml);
                                    if (body == null) { body = bodys.FirstOrDefault(a => a.ContentType == PFEmailContentType.TextPlain); }
                                    if (body != null)
                                    {
                                        Body = body.Body;
                                    }
                                    break;
                                }
                                else if (ContentType == PFEmailContentType.MultipartMixed
                                    )
                                {
                                    MultiBody = new List<PFEmailBody>();
                                    var bodys = GetMultiBody(property.Skip(_idx).ToList());
                                    PFEmailBody body = null;
                                    body = bodys.FirstOrDefault(a => a.ContentType == PFEmailContentType.TextHtml);
                                    if (body == null) { body = bodys.FirstOrDefault(a => a.ContentType == PFEmailContentType.TextPlain); }
                                    if (body != null)
                                    {
                                        Body = body.Body;
                                    }
                                    break;
                                }
                                else
                                {
                                    //Body += DecodeBody(s,ContentTransferEncoding,Charset);
                                    EncodeBody.Add(s);
                                }
                            }
                        }
                    }
                    Messages.Add(s);
                    _idx++;
                }
            }
            //由于读Subject时并不能保证已经读了ContentTransferEncoding,所以最后再解码
            foreach (var i in EncodeSubjects)
            {
                Subject += DecodeSubject(i);
            }
            //Subject = DecodeSubject(string.Join("", EncodeSubjects));

            string[] multiMimeFormatTest = new string[]
            {
                "This is a multipart message in MIME format.",
                "This is a multi-part message in MIME format."
            };
            if (sbody != null && sbody.Any())
            {
                Body = string.Join("", sbody.ToArray());
                if (!multiMimeFormatTest.Contains(Body))
                {
                    Body = PFDataHelper.Decode(Body, GetEncodeTypeByTransferEncodeType(ContentTransferEncoding), Charset);
                }
            }
            #endregion

            List<string> bodyLeftPart = null;

            MultiBody = new List<PFEmailBody>();
            while (leftPart != null && leftPart.Any())
            {
                //if(leftPart[1]== "--=====003_Dragon765187361670_=====--")
                //{
                //    var aa = "aa";
                //}
                //MessageSplit(leftPart, out property, out sbody, out bodyLeftPart,ChildBoundary,out childBoundary);
                //MessageSplit(leftPart, out property, out sbody, out bodyLeftPart,out boundary);
                MessageSplit(leftPart, out property, out sbody, out bodyLeftPart, out boundary, out childBoundary);

                var bodyItem = new PFEmailBody();
                bodyItem.Boundary = boundary;
                bodyItem.ChildBoundary = childBoundary;
                //if (property != null && property.Any()) {
                //    bodyItem =GetMultiBody(bodyLeftPart)[0];

                //}
                GetBodyProperty(ref bodyItem, property);

                //bodyItem.Body = sbody;
                //bodyItem.Body = DecodeBody(sbody, bodyItem.ContentTransferEncoding, bodyItem.Charset);
                if (bodyItem.ContentTransferEncoding == PFEmailTransferEncodeType.QuotedPrintable)//perfect99含附件时,行尾有=号,如果不去掉解码时会报错,暂这样处理--benjamin20200306
                {
                    sbody = sbody.Select(a => a.Length > 0 && a[a.Length - 1] == '=' ? a.Substring(0, a.Length - 1) : a).ToList();
                }

                //try
                //{
                if (sbody != null) { bodyItem.Body = PFDataHelper.Decode(string.Join("", sbody.ToArray()), GetEncodeTypeByTransferEncodeType(bodyItem.ContentTransferEncoding), bodyItem.Charset); }
                //}catch(Exception e)
                //{
                //    var a = "aa";
                //}

                MultiBody.Add(bodyItem);

                leftPart = bodyLeftPart;
            }

            if (PFDataHelper.StringIsNullOrWhiteSpace(Body) || multiMimeFormatTest.Contains(Body))
            {
                Body = MultiBody.Where(a => a.ContentType == PFEmailContentType.TextPlain).Select(a => a.Body).FirstOrDefault()
                    ?? MultiBody.Where(a => a.ContentType == PFEmailContentType.TextHtml).Select(a => a.Body).FirstOrDefault();
            }
            //var aa = "aa";
            ////body必需多行合一起解码,因为汉字的编码有可能是拆到两行了
            //if (ContentType == PFEmailContentType.MultipartAlternative)
            //{

            //}
            //else
            //{
            //    Body = DecodeBody(string.Join("", EncodeBody), ContentTransferEncoding, Charset);
            //}
        }
        private void GetBodyProperty(ref PFEmailBody body, IList<string> messages)
        {
            if (messages == null) { return; }
            PFEmailItemType curItem = PFEmailItemType.Default;

            for (int i = 0; i < messages.Count; i++)
            {
                string s = messages[i];
                if (s != null)
                {
                    if (s == "")//perfect99邮箱的正文内容似乎在""下一个line(图见D:\wxj\工作记录\20200119_CSharp接收邮件研究)
                    {
                        curItem = PFEmailItemType.Body;
                    }
                    else if (s.IndexOf("------=") == 0)//------=似乎是每个part的分隔
                    {
                        curItem = PFEmailItemType.Default;

                    }
                    else if (s.IndexOf("Content-Type: ") == 0)
                    {
                        curItem = PFEmailItemType.ContentType;
                        PFEmailBody item = new PFEmailBody();
                        GetContentType(s, a => item.ContentType = a, a => item.Charset = a);
                        body.ContentType = item.ContentType;
                        body.Charset = item.Charset;

                    }
                    else if (s.IndexOf("charset=\"") > -1)
                    {
                        body.Charset = PFDataHelper.GetEncoding(s.Substring(s.IndexOf("charset=\"") + 9).Replace("\"", ""));

                    }
                    else if (s.IndexOf("Content-Transfer-Encoding: ") == 0)
                    {
                        curItem = PFEmailItemType.ContentTransferEncoding;
                        body.ContentTransferEncoding = GetTransferEncodeType(s.Substring(27));
                    }
                    //else if (s.IndexOf("boundary=") > -1)
                    //{
                    //    var idx = s.IndexOf("boundary=");
                    //    body.ChildBoundary = s.Substring(idx + 10).Replace("\"", "");
                    //    if (body.ChildBoundary[body.ChildBoundary.Length - 1] == ';') { body.ChildBoundary = body.ChildBoundary.Substring(0, body.ChildBoundary.Length - 1); }
                    //}
                    else
                    {
                        //if (curItem == PFEmailItemType.ContentType)
                        //{
                        //    //Subject += DecodeSubject(s);
                        //    item.EncodeSubjects.Add(s);
                        //}
                        //else 
                        if (curItem == PFEmailItemType.Body)
                        {
                            //item.EncodeBody.Add(s);

                        }
                        //else if(curItem == PFEmailItemType.ContentType)
                        //{
                        //    curItem = PFEmailItemType.ContentType;
                        //    GetContentType(s, a => item.ContentType = a, a => item.Charset = a);
                        //}
                    }
                }
                //idx++;
            }
        }
        private List<PFEmailBody> GetMultiBody(List<string> messages)
        {
            //var result = new List<PFEmailBody>();
            PFEmailItemType curItem = PFEmailItemType.Default;
            //var idx = 0;
            PFEmailBody item = null;
            for (int i = 0; i < messages.Count; i++)
            {
                string s = messages[i];
                if (s != null)
                {
                    //if (s == "")//perfect99邮箱的正文内容似乎在""下一个line(图见D:\wxj\工作记录\20200119_CSharp接收邮件研究)
                    //{
                    //    if (messages.Count > i + 2&& messages[i+1].IndexOf("------=")==0)
                    //    {
                    //        var newItem = new PFEmailBody();//必须新指针,否则MultiBody每一项都是一样的
                    //        item = newItem;
                    //        MultiBody.Add(newItem);
                    //    }else
                    //    {
                    //        curItem = PFEmailItemType.Body;
                    //    }
                    //}
                    if (s == "")//perfect99邮箱的正文内容似乎在""下一个line(图见D:\wxj\工作记录\20200119_CSharp接收邮件研究)
                    {
                        curItem = PFEmailItemType.Body;
                    }
                    else if (s.IndexOf("------=") == 0)//------=似乎是每个part的分隔
                    {
                        curItem = PFEmailItemType.Default;

                        if (item != null)
                        {
                            item.Body = DecodeBody(string.Join("", item.EncodeBody.ToArray()), item.ContentTransferEncoding, item.Charset);
                            MultiBody.Add(item);
                        }

                        var newItem = new PFEmailBody();//必须新指针,否则MultiBody每一项都是一样的
                        item = newItem;
                        //MultiBody.Add(newItem);
                    }
                    else if (s.IndexOf("Content-Type: ") == 0)
                    {
                        curItem = PFEmailItemType.ContentType;
                        GetContentType(s, a => item.ContentType = a, a => item.Charset = a);
                        //var charset = item.Charset;
                        //item.ContentType = GetContentType(s,ref charset);
                        //item.Charset = charset;

                        //if (s.Substring(14) == "text/html")
                        //{
                        //    item.ContentType = PFEmailContentType.TextHtml;
                        //}
                        //else if (s.Substring(14) == "text/plain")
                        //{
                        //    item.ContentType = PFEmailContentType.TextPlain;
                        //}
                    }
                    else if (s.IndexOf("charset=\"") > -1)
                    {
                        item.Charset = PFDataHelper.GetEncoding(s.Substring(s.IndexOf("charset=\"") + 9).Replace("\"", ""));
                        //item.Charset = GetCharset(s.Substring(s.IndexOf("charset=\"") + 9).Replace("\"", ""));
                    }
                    else if (s.IndexOf("Content-Transfer-Encoding: ") == 0)
                    {
                        curItem = PFEmailItemType.ContentTransferEncoding;
                        item.ContentTransferEncoding = GetTransferEncodeType(s.Substring(27));
                    }
                    else
                    {
                        //if (curItem == PFEmailItemType.ContentType)
                        //{
                        //    //Subject += DecodeSubject(s);
                        //    item.EncodeSubjects.Add(s);
                        //}
                        //else 
                        if (curItem == PFEmailItemType.Body)
                        {
                            //item.Body += DecodeBody(s, item.ContentTransferEncoding, item.Charset);
                            item.EncodeBody.Add(s);

                            //if (s.IndexOf("------=") == 0)
                            //{
                            //    curItem = PFEmailItemType.Default;
                            //}
                            //else
                            //{
                            //    item.Body += DecodeBody(s,item.ContentTransferEncoding,item.Charset);
                            //}
                        }
                        //else if(curItem == PFEmailItemType.ContentType)
                        //{
                        //    curItem = PFEmailItemType.ContentType;
                        //    GetContentType(s, a => item.ContentType = a, a => item.Charset = a);
                        //}
                    }
                }
                //idx++;
            }
            //if (item != null)
            //{
            //    MultiBody.Add(item);
            //}
            return MultiBody;
        }
        private void GetContentType(string s, Action<PFEmailContentType> contentType, Action<PFEncodeType> charset)
        {
            //var result = PFEmailContentType.TextPlain;
            //charset = PFEmailCharsetType.UTF8;
            //perfect99邮箱里,Content-Type:和charset在同一行,如 Content-Type: text/html; charset=utf-8
            var items = s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var i in items)
            {
                if (i.IndexOf("Content-Type: ") == 0)
                {
                    if (i.Substring(14) == "text/html")
                    {
                        contentType(PFEmailContentType.TextHtml);
                        //contentType = PFEmailContentType.TextHtml;
                    }
                    else if (i.Substring(14) == "text/plain")
                    {
                        contentType(PFEmailContentType.TextPlain);
                        // contentType = PFEmailContentType.TextPlain;
                    }
                    else if (i.Substring(14) == "multipart/alternative")
                    {
                        contentType(PFEmailContentType.MultipartAlternative);
                        //contentType = PFEmailContentType.MultipartAlternative;
                    }
                    else if (i.Substring(14) == "multipart/mixed")
                    {
                        contentType(PFEmailContentType.MultipartMixed);
                        //contentType = PFEmailContentType.MultipartAlternative;
                    }
                }
                else if (i.IndexOf("charset=") > -1)
                {
                    //charset(PFDataHelper.GetEncoding(i.Substring(i.IndexOf("charset=")+8)));
                    charset(PFDataHelper.GetEncoding(i.Substring(i.IndexOf("charset=") + 8).Replace("\"", "")));
                    ////charset(GetCharset(i.Substring(8)));
                    ////charset = GetCharset(i.Substring(8));
                }
            }

            //if (s.Substring(14) == "text/html")
            //{
            //    item.ContentType = PFEmailContentType.TextHtml;
            //}
            //else if (s.Substring(14) == "text/plain")
            //{
            //    item.ContentType = PFEmailContentType.TextPlain;
            //}
            //return result;
        }
        //private String DecodeSubjectByCharset(string s, List<PFEmailCharsetType> charsets)
        //{
        //    string result = s;
        //    foreach (var i in charsets)
        //    {
        //        if (i == PFEmailCharsetType.UTF8)
        //        {
        //            result =
        //        }
        //    }
        //}

        //private static PFEmailCharsetType GetCharset(string s)
        //{
        //    PFEmailCharsetType result = PFEmailCharsetType.UTF8;
        //    if (s == "gb18030")
        //    {
        //        result = PFEmailCharsetType.GB18030;
        //    }
        //    else if (s == "utf-8")
        //    {
        //        result = PFEmailCharsetType.UTF8;
        //    }
        //    return result;
        //}
        private static PFEmailTransferEncodeType GetTransferEncodeType(string s)
        {
            PFEmailTransferEncodeType result = PFEmailTransferEncodeType.Base64;
            if (s == "base64")
            {
                result = PFEmailTransferEncodeType.Base64;
            }
            else if (s == "8Bit")
            {
                result = PFEmailTransferEncodeType.Bit8;
            }
            else if (s == "quoted-printable")
            {
                result = PFEmailTransferEncodeType.QuotedPrintable;
            }
            return result;
        }
        private static PFEncodeType GetEncodeTypeByTransferEncodeType(PFEmailTransferEncodeType encodeType)
        {
            return PFDataHelper.ObjectToEnum<PFEncodeType>(encodeType) ?? PFEncodeType.Base64;
        }
        //private static PFEncodeType GetEncodeTypeByCharset(PFEmailCharsetType encodeType)
        //{
        //    return PFDataHelper.ObjectToEnum<PFEncodeType>(encodeType) ?? PFEncodeType.UTF8;
        //}
        private String DecodeSubject(string line)
        {
            ////string line = "=?utf-8?B?2KfbjNmGINuM2qkg2YXYqtmGINiz2KfYr9mHINin2LPYqg==?= =?utf-8?B?2KfbjNmGINuM2qkg2YXYqtmGINiz2KfYr9mHINin2LPYqg==?= =?utf-8?B?2YbYr9is?=";
            //Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape("=?utf-8?B?"), Regex.Escape("?=")));
            Regex regex = new Regex(string.Format("^\\s*{0}(.*?){1}$", Regex.Escape("=?"), Regex.Escape("?=")));
            var matches = regex.Matches(line);
            var result = "";
            if (matches.Count > 0)//增加邮件中文标题解码--benjamin20200119
            {
                foreach (Match match in matches)
                {
                    var v = match.Groups[1].Value;
                    Regex encodeTypeRegex = new Regex("([^\\?]+)\\?");
                    var encodeTypeMatches = encodeTypeRegex.Matches(v);
                    if (encodeTypeMatches.Count > 1)
                    {
                        result += PFDataHelper.Decode(
                            //Regex.Replace(v, "(gb18030\\?)|(utf-8\\?)|(B\\?)", ""),
                            Regex.Replace(v, "([^\\?]+\\?){2}", ""),
                            PFDataHelper.GetEncoding(encodeTypeMatches[1].Groups[1].Value),
                            PFDataHelper.GetEncoding(encodeTypeMatches[0].Groups[1].Value)
                            );
                    }

                    //v = Regex.Replace(v, "(gb18030\\?)|(utf-8\\?)|(B\\?)", "");
                    ////result += Encoding.UTF8.GetString(Convert.FromBase64String(match.Groups[1].Value));

                    //byte[] v1 = null;
                    //if (ContentTransferEncoding == PFEmailTransferEncodeType.Base64)
                    //{
                    //    v1 = Convert.FromBase64String(v);
                    //}else if (ContentTransferEncoding == PFEmailTransferEncodeType.Bit8)
                    //{
                    //    v1 = Convert.FromBase64String(v);
                    //    //v1 = System.Text.Encoding.Default.GetBytes(v);
                    //}
                    //string v2 = "";
                    //if(Charset== PFEmailCharsetType.UTF8)
                    //{
                    //    v2 = Encoding.UTF8.GetString(v1);
                    //}else if (Charset == PFEmailCharsetType.GB18030)
                    //{
                    //    v2 = Encoding.Default.GetString(v1);
                    //}
                    //result += v2;
                }
            }
            else
            {
                result += line;
            }
            return result;
        }
        private String DecodeBody(string line, PFEmailTransferEncodeType teType, PFEncodeType cType)
        {
            var result = "";
            result += PFDataHelper.Decode(line, GetEncodeTypeByTransferEncodeType(teType), cType);
            //result += PFDataHelper.Decode(line,GetEncodeTypeByTransferEncodeType(teType),GetEncodeTypeByCharset(cType));
            //result += Encoding.UTF8.GetString(Convert.FromBase64String(line));
            return result;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            var t = obj as PFEmail;
            if (t == null) { return false; }
            return t.Date == Date && t.Subject == Subject;
        }
        public void Dispose()
        {
            if (Messages != null)
            {
                Messages.Clear();
                PFDataHelper.GCCollect();
                //GC.Collect();
            }
        }
        public override string ToString()
        {
            return string.Format(@"
Subject:{0}
Date:{1}
", Subject, Date);
        }
        //public static PFEmail Empty { get { return new PFEmail { }; } }

        //public void Read(string s) {
        //    if(s.IndexOf("Subject: ") ==0)
        //    {
        //        Subject=s.
        //    }
        //}
    }
    public class PFEmailManager
    {

        public TcpClient Server;
        public NetworkStream NetStrm;
        public StreamReader RdStrm;
        public string Data;
        public byte[] szData;
        public string CRLF = "\r\n";
        #region 操作状态
        public List<string> _statusList = null;
        public List<string> statusList { get { return _statusList ?? (_statusList = new List<string>()); } set { _statusList = value; } }

        public string ReceiveStatus { get; set; }
        public List<string> _receiveStatus = null;
        public List<string> receiveStatus { get { return _receiveStatus ?? (_receiveStatus = new List<string>()); } set { _receiveStatus = value; } }
        public string DisConnectStatus { get; set; }
        public string DeleteStatus { get; set; }
        public string StatStatus { get; set; }
        #endregion 操作状态
        public List<string> _message = null;
        public List<string> Message { get { return _message ?? (_message = new List<string>()); } set { _message = value; } }
        public int MessageCount = 0;
        public int LastMessageIdx = 0;
        public string _hostName { get; set; }
        public string _userName { get; set; }
        public string _pwd { get; set; }
        private bool _isConnect { get; set; }
        public PFEmailManager(string hostName, string userName, string pwd)
        {
            _hostName = hostName;
            _userName = userName;
            _pwd = pwd;
        }
        private void ClearList()
        {
            statusList.Clear();
            receiveStatus.Clear();
            Message.Clear();
            PFDataHelper.GCCollect();
            //GC.Collect();
        }

        private void CloseStream()
        {
            try
            {
                //断开连接 
                NetStrm.Close();
                RdStrm.Close();
            }
            catch (Exception) { }
        }
        /// <summary>
        /// 连接
        /// </summary>
        public void Connect_Click()//string hostName,string userName,string pwd)
        {
            if (_isConnect)
            {
                _isConnect = false;
                Disconnect_Click();
            }

            try
            {
                //用110端口新建POP3服务器连接 
                Server = new TcpClient(_hostName, 110);
            }
            catch (Exception e)
            {
                _isConnect = false;
                PFDataHelper.WriteError(e);
                return;
            }
            Server.ReceiveTimeout = PFDataHelper.EmailTimeout;//这里加了超时之后，Server.GetStream()的NetworkStream也会有相同的超时值--benjamin20191205
            Server.SendTimeout = PFDataHelper.EmailTimeout;//benjamin20191205

            ////Status.Items.Clear();
            //statusList.Clear();
            //receiveStatus.Clear();
            //statusList.Clear();
            //GC.Collect();
            ClearList();
            try
            {
                //初始化 
                NetStrm = Server.GetStream();
                RdStrm = new StreamReader(Server.GetStream());
                //RdStrm = new StreamReader(Server.GetStream(),PFDataHelper._encoding);
                statusList.Add(RdStrm.ReadLine());

                //登录服务器过程 
                Data = "USER " + _userName + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                statusList.Add(RdStrm.ReadLine());

                Data = "PASS " + _pwd + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                statusList.Add(RdStrm.ReadLine());

                //////向服务器发送STAT命令，从而取得邮箱的相关信息：邮件数量和大小 
                GetStat();

                _isConnect = true;

            }
            catch (InvalidOperationException err)
            {
                CloseStream();
                //Console.Write("Error: " + err.ToString());
                //Status.Items.Add("Error: " + err.ToString());
                PFDataHelper.WriteError(err);
            }
        }
        /// <summary>
        /// 测试用
        /// </summary>
        /// <param name="statusAction"></param>
        [Obsolete]
        public void Connect_Click(Action<string> statusAction)//string hostName,string userName,string pwd)
        {
            ////将光标置为等待状态 
            ////Cursor cr = Cursor.Current;
            ////Cursor.Current = Cursors.WaitCursor;

            ////用110端口新建POP3服务器连接 
            ////Server = new TcpClient(PopServer.Text, 110);

            //string hostName = "pop.qq.com";
            //string userName = "251561897@qq.com";
            //string pwd = "llcffwhxezsicadi";

            Server = new TcpClient(_hostName, 110);
            ////Status.Items.Clear();
            //statusList.Clear();
            //receiveStatus.Clear();
            //statusList.Clear();
            //GC.Collect();
            ClearList();
            try
            {
                string tmpStr = "";
                //初始化 
                NetStrm = Server.GetStream();
                RdStrm = new StreamReader(Server.GetStream());
                tmpStr = RdStrm.ReadLine();
                statusList.Add(tmpStr);
                statusAction(string.Format("初始化:{0}", tmpStr));

                //登录服务器过程 
                Data = "USER " + _userName + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                tmpStr = RdStrm.ReadLine();
                statusList.Add(tmpStr);
                statusAction(string.Format("登录服务器:{0}", tmpStr));

                Data = "PASS " + _pwd + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                tmpStr = RdStrm.ReadLine();
                statusList.Add(tmpStr);
                statusAction(string.Format("登录服务器:{0}", tmpStr));

                //////向服务器发送STAT命令，从而取得邮箱的相关信息：邮件数量和大小 
                tmpStr = GetStat().ToString();
                statusAction(string.Format("邮件数:{0}", tmpStr));

                _isConnect = true;

            }
            catch (InvalidOperationException err)
            {
                statusAction(string.Format("连接错误:{0}", err));
                CloseStream();
                //Console.Write("Error: " + err.ToString());
                //Status.Items.Add("Error: " + err.ToString());
                PFDataHelper.WriteError(err);
            }
        }
        public int GetStat()
        {
            //向服务器发送STAT命令，从而取得邮箱的相关信息：邮件数量和大小 
            Data = "STAT" + CRLF;
            szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
            //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
            NetStrm.Write(szData, 0, szData.Length);
            string stat = RdStrm.ReadLine();
            //statusList.Add(stat);
            StatStatus = stat;

            MessageCount = -1;
            if (!string.IsNullOrEmpty(stat))
            {
                var status = stat.Split(' ');
                if (status.Length > 1)
                {
                    //MessageCount = int.Parse(status[2]);
                    MessageCount = int.Parse(status[1]);
                }
            }

            return MessageCount;
        }
        #region 新邮件列表
        //LIST命令
        //新邮件?http://blog.sina.com.cn/s/blog_5538da0a01009jav.html
        //其实和STAT获得的邮件数是一样的,好像不只是未读邮件
        public string GetList()
        {
            return ExecuteCommand("LIST") + "\r\n";
        }
        /// <summary>
        /// 执行Pop3命令，并检查执行的结果
        /// </summary>
        /// <param name="command">Pop3命令行</param>
        /// <returns>
        /// 类型：字符串
        /// 内容：Pop3命令的执行结果
        /// </returns>
        private string ExecuteCommand(string command)
        {
            string strMessage = null;  //执行Pop3命令后返回的消息

            try
            {
                //发送命令
                WriteToNetStream(ref NetStrm, command);

                //读取多行
                if (command.Substring(0, 4).Equals("LIST") || command.Substring(0, 4).Equals("RETR") || command.Substring(0, 4).Equals("UIDL")) //记录STAT后的消息（其中包含邮件数）
                {
                    strMessage = ReadMultiLine();

                    if (command.Equals("LIST")) //记录LIST后的消息（其中包含邮件数）
                        mstrStatMessage = strMessage;
                }
                //读取单行
                else
                    strMessage = RdStrm.ReadLine();

                //判断执行结果是否正确
                if (CheckCorrect(strMessage, "+OK"))
                    return strMessage;
                else
                    return "Error";
            }
            catch (IOException exc)
            {
                throw new Pop3Exception(exc.ToString());
            }
        }
        private string mstrStatMessage = null;  //执行STAT命令后得到的消息（从中得到邮件数）
        /// <summary>
        /// 检查命令行结果是否正确
        /// </summary>
        /// <param name="message">命令行的执行结果</param>
        /// <param name="check">正确标志</param>
        /// <returns>
        /// 类型：布尔
        /// 内容：true表示没有错误，false为有错误
        /// </returns>
        /// <remarks>检查命令行结果是否有错误</remarks>
        private bool CheckCorrect(string message, string check)
        {
            if (message.IndexOf(check) == -1)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 在Pop3命令中，LIST、RETR和UIDL命令的结果要返回多行，以点号（.）结尾，
        /// 所以如果想得到正确的结果，必须读取多行
        /// </summary>
        /// <returns>
        /// 类型：字符串
        /// 内容：执行Pop3命令后的结果
        /// </returns>
        private string ReadMultiLine()
        {
            string strMessage = RdStrm.ReadLine();
            string strTemp = null;
            while (strMessage != ".")
            {
                strTemp = strTemp + strMessage;
                strMessage = RdStrm.ReadLine();
            }
            return strTemp;
        }
        /// <summary>
        /// 向网络访问的基础数据流中写数据（发送命令码）
        /// </summary>
        /// <param name="netStream">可以用于网络访问的基础数据流</param>
        /// <param name="command">命令行</param>
        /// <remarks>向网络访问的基础数据流中写数据（发送命令码）</remarks>
        private void WriteToNetStream(ref NetworkStream netStream, String command)
        {
            string strToSend = command + "\r\n";
            byte[] arrayToSend = System.Text.Encoding.ASCII.GetBytes(strToSend.ToCharArray());
            netStream.Write(arrayToSend, 0, arrayToSend.Length);
        }

        #endregion
        /// <summary>
        /// 断开
        /// </summary>
        public void Disconnect_Click()
        {
            if (_isConnect)
            {
                try
                {
                    ////将光标置为等待状态 
                    //Cursor cr = Cursor.Current;
                    //Cursor.Current = Cursors.WaitCursor;

                    //向服务器发送QUIT命令从而结束和POP3服务器的会话 
                    Data = "QUIT" + CRLF;
                    szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                    //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
                    NetStrm.Write(szData, 0, szData.Length);
                    //statusList.Add(RdStrm.ReadLine());
                    DisConnectStatus = RdStrm.ReadLine();

                    //断开连接 
                    NetStrm.Close();
                    RdStrm.Close();

                    ////改变按钮的状态 
                    //Connect.Enabled = true;
                    //Disconnect.Enabled = false;
                    //Retrieve.Enabled = false;

                    ////将光标置回原来的状态 
                    //Cursor.Current = cr;
                    _isConnect = false;
                    Server.Close();
                }
                catch (InvalidOperationException err)
                {
                    CloseStream();
                    //Console.Write("Error: " + err.ToString());
                    //Status.Items.Add("Error: " + err.ToString());
                    PFDataHelper.WriteError(err);
                }
            }
        }

        public PFEmail Retrieve_Click(int mailNum)
        {
            if (mailNum < 0) { return null; }//如果内部用了递归(以前的方案),可以防止死循环--benjamin20190929

            ////将光标置为等待状态 
            //Cursor cr = Cursor.Current;
            //Cursor.Current = Cursors.WaitCursor;
            string szTemp;
            Message.Clear();
            receiveStatus.Clear();

            PFDataHelper.GCCollect();
            //GC.Collect();
            try
            {
                //根据邮件编号从服务器获得相应邮件 
                //Data = "RETR " + mailNum + CRLF;
                //返回多少行https://bbs.csdn.net/topics/390695103?page=1
                Data = "TOP " + mailNum + " " + 10000 + CRLF;
                //Data = "RETR 1" + CRLF;
                szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
                //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
                NetStrm.Write(szData, 0, szData.Length);
                szTemp = RdStrm.ReadLine();

                #region 方法1(因为没用EndOfStream判断,可能有漏数据的风险,但似乎效率效高)
                if (szTemp == null)
                {
                    return null;
                }
                if (szTemp[0] != '-')
                {
                    //不断地读取邮件内容，只到结束标志：英文句号 
                    int whileCnt = 0;
                    while (szTemp != ".")
                    {
                        //Message.Text += szTemp;
                        Message.Add(szTemp);//System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown. --benjamin 
                        try
                        {
                            szTemp = RdStrm.ReadLine();
                        }
                        catch (Exception e)
                        {
                            PFDataHelper.WriteError(e);
                            return null;
                        }

                        if (szTemp == null)
                        {
                            break;//有时会返回Message:["+OK",null,...]  估计是网络错误--benjamin20191009
                        }

                        whileCnt++;

                        #region 使用TOP命令之后,不需要这样了(这部分之前也有bug,下次执行RETR命令时,还会接着读之前一个邮件未读完的部分)
                        //////测试暂取消
                        //if (whileCnt > 10000)//测试时Message.Add(szTemp)报内存错误了,暂这样处理(steam邮件超1000行)
                        //{
                        //    //RdStrm.Close();
                        //    //Disconnect_Click();
                        //    //Connect_Click();
                        //    //RdStrm.DiscardBufferedData();//.Dispose();
                        //    RdStrm.Close();
                        //    RdStrm = new StreamReader(Server.GetStream());
                        //    //PFDataHelper.WriteError(new Exception(string.Format("读Message行数太多,可能是超时引起的\r\n\r\nMessage:\r\n{0}\r\n\r\nszTemp:\r\n{1}", JsonConvert.SerializeObject(Message), szTemp)));//这日志太大了,会是20G以上的--benjamin 20200302
                        //    break;//读邮件时,如果超过10000不关闭流的话,下次再读就有问题
                        //} 
                        #endregion
                    }

                }
                else
                {
                    receiveStatus.Add(szTemp);
                    var errCodes = new string[] {
                        "-ERR Message not exists",
                        "-ERR Unknown message"
                    };
                    //if (szTemp.Equals("-ERR Message not exists"))
                    if (errCodes.Contains(szTemp))
                    {
                        return null;

                        //这样似乎会造成死循环--benjamin20190929
                        //LastMessageIdx = mailNum - 1;
                        //return Retrieve_Click(LastMessageIdx);
                    }
                }
                #endregion

                #region 方法2
                //if (szTemp == null)
                //{
                //    return null;
                //}
                //if (szTemp[0] != '-')
                //{
                //    int whileCnt = 0;
                //    try
                //    {
                //        while (!RdStrm.EndOfStream)
                //        {
                //            try
                //            {
                //                Message.Add(szTemp);//System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown. --benjamin 
                //                szTemp = RdStrm.ReadLine();
                //            }
                //            catch (Exception) { }

                //            whileCnt++;
                //            if (whileCnt > 10000)//测试时Message.Add(szTemp)报内存错误了,暂这样处理(steam邮件超1000行)
                //            {
                //                PFDataHelper.WriteError(new Exception(string.Format("读Message行数太多,可能是超时引起的\r\n\r\nMessage:\r\n{0}\r\n\r\nszTemp:\r\n{1}", JsonConvert.SerializeObject(Message), szTemp)));
                //                break;
                //            }
                //        }
                //    }catch(Exception )
                //    {

                //    }
                //}
                //else
                //{
                //    receiveStatus.Add(szTemp);
                //    if (szTemp.Equals("-ERR Message not exists"))
                //    {
                //        return null;
                //    }
                //} 
                #endregion

                //if (PFDataHelper.IsDebug)
                //{
                //    return new PFEmail(mailNum, Message,true);
                //}
                return new PFEmail(mailNum, Message);
                //return new PFEmail( Message);
                ////将光标置回原来的状态 
                //Cursor.Current = cr;
            }
            catch (InvalidOperationException err)
            {
                receiveStatus.Add("Error: " + err.ToString());

            }
            return null;
        }
        public void DeleteEmail(PFEmail email)
        {
            Data = "DELE" + email.MailNum + CRLF;
            szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
            //szData = PFDataHelper._encoding.GetBytes(Data.ToCharArray());
            NetStrm.Write(szData, 0, szData.Length);
            //statusList.Add(RdStrm.ReadLine());
            DeleteStatus = RdStrm.ReadLine();
        }
        public bool IsConnect()
        {
            return _isConnect;
        }
    }

    /// <summary>   
    /// 用于取得一个文本文件的编码方式(Encoding)。   
    /// </summary>   
    public class TxtFileEncoder
    {
        public enum PFEncoding
        {
            ANSI, BigEndianUnicode, Unicode, UTF8_HasBom, UTF8_NoBom
        }
        public TxtFileEncoder()
        {
            //   
            // TODO: 在此处添加构造函数逻辑   
            //   
        }

        /// <summary>   
        /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(FileStream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }
        /// <summary>   
        /// 取得一个文本文件的编码方式。   
        /// </summary>   
        /// <param name="fileName">文件名。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
            fs.Close();
            return targetEncoding;
        }
        /// <summary>   
        /// 取得一个文本文件流的编码方式。   
        /// </summary>   
        /// <param name="stream">文本文件流。</param>   
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>   
        /// <returns></returns>   
        public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (stream != null && stream.Length >= 2)
            {
                //保存文件流的前4个字节   
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;
                //保存当前Seek位置   
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);

                int nByte = stream.ReadByte();
                byte1 = Convert.ToByte(nByte);
                byte2 = Convert.ToByte(stream.ReadByte());
                if (stream.Length >= 3)
                {
                    byte3 = Convert.ToByte(stream.ReadByte());
                }
                if (stream.Length >= 4)
                {
                    byte4 = Convert.ToByte(stream.ReadByte());
                }
                //根据文件流的前4个字节判断Encoding   
                //Unicode {0xFF, 0xFE};   
                //BE-Unicode {0xFE, 0xFF};   
                //UTF8 = {0xEF, 0xBB, 0xBF};   
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe   
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode   
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8   
                {
                    targetEncoding = Encoding.UTF8;
                }
                //恢复Seek位置         
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return targetEncoding;
        }



        // 新增加一个方法，解决了不带BOM的 UTF8 编码问题   

        /// <summary>   
        /// 通过给定的文件流，判断文件的编码类型   
        /// </summary>   
        /// <param name="fs">文件流</param>   
        /// <returns>文件的编码类型</returns>   
        public static System.Text.Encoding GetEncoding(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM   
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(4);
            if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            else
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    reVal = Encoding.UTF8;
                }
                else
                {
                    int i;
                    int.TryParse(fs.Length.ToString(), out i);
                    ss = r.ReadBytes(i);

                    if (IsUTF8Bytes(ss))
                        reVal = Encoding.UTF8;
                }
            }
            r.Close();
            return reVal;

        }

        public static PFEncoding GetPFEncoding(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM   
            PFEncoding reVal = PFEncoding.ANSI;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(4);
            if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = PFEncoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = PFEncoding.Unicode;
            }
            else
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    reVal = PFEncoding.UTF8_HasBom;
                }
                else
                {
                    int i;
                    int.TryParse(fs.Length.ToString(), out i);
                    ss = r.ReadBytes(i);

                    if (IsUTF8Bytes(ss))
                        reVal = PFEncoding.UTF8_NoBom;
                }
            }
            r.Close();
            return reVal;

        }

        /// <summary>   
        /// 判断是否是不带 BOM 的 UTF8 格式   
        /// </summary>   
        /// <param name="data"></param>   
        /// <returns></returns>   
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数   
            byte curByte; //当前分析的字节.   
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前   
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　   
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1   
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式!");
            }
            return true;
        }
    }

    /// <summary>
    /// 网格线的方向
    /// </summary>
    [Flags]
    public enum TreeMatrixNet
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }
    /// <summary>
    /// 树型矩阵
    /// </summary>
    public class TreeMatrix
    {
        /// <summary>
        /// 节点坐标矩阵
        /// </summary>
        bool[,] _node;
        /// <summary>
        /// 每一层的最后一个节点坐标矩阵
        /// </summary>
        bool[,] _lastChild;
        /// <summary>
        /// 线条网矩阵
        /// </summary>
        TreeMatrixNet[,] _net;

        /// <summary>
        /// 指net的x最大值，node的x要比net的多1
        /// </summary>
        int _xMax;
        /// <summary>
        /// net和node的y相等
        /// </summary>
        int _yMax;

        public TreeMatrix(IList rows)
        {
            var x = GetDepth(rows);
            var y = GetAllChildrenCount(rows);
            _node = new bool[x, y];
            _lastChild = new bool[x, y];
            _net = new TreeMatrixNet[x - 1, y];
            _xMax = x - 2;//注意:这里不是_net的x-1,是因为_xMax是最大索引号,而x-1指的是数量
            _yMax = y - 1;

            var i = 0;
            foreach (TreeListItem row in rows)
            {
                SetNode(0, i);
                if (rows.Count - 1 == i) { SetLastChild(0, i); }
                i++;
                row.EachChild((child, depth, parent) =>
                {
                    SetNode(depth - 1, i);
                    if (parent.Children.Last() == child) { SetLastChild(depth - 1, i); }
                    i++;
                });
            }
            SetNetByNode();
        }
        /// <summary>
        ///     获得深度
        /// </summary>
        /// <returns></returns>
        private int GetDepth(IList rows)
        {
            int max = 0;
            foreach (var i in rows)
            {
                var row = (TreeListItem)i;
                var d = row.GetDepth();
                if (d > max) max = d;
            }
            return max;
        }

        /// <summary>
        ///     获得所有children的数量,递归查找
        /// </summary>
        /// <returns></returns>
        private int GetAllChildrenCount(IList rows)
        {
            int total = 0;
            foreach (var i in rows)
            {
                total += 1;
                var row = (TreeListItem)i;
                total += row.GetAllChildrenCount();
            }
            return total;
        }
        public void SetMatrix(bool[,] matrix, int x, int y)
        {
            matrix[x, y] = true;
        }
        /// <summary>
        /// x即是treecolumn的缩进等级lv;y即是row的行号
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetNode(int x, int y)
        {
            SetMatrix(_node, x, y);//把node阵的xy行赋值，因为setNetByNode时要用到
        }
        /// <summary>
        /// x即是treecolumn的缩进等级lv;y即是row的行号
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetLastChild(int x, int y)
        {

            SetMatrix(_lastChild, x, y);//把lastChild阵的xy行赋值，因为setNetByNode时要用到
        }
        /// <summary>
        /// 根据所有节点生成连线网
        /// </summary>
        public void SetNetByNode()
        {
            for (var x = 0; x <= _xMax; x++)
            {
                for (var y = 1; y <= _yMax; y++)
                {//注意网线是从序号1开始的
                    if (_node[x + 1, y]) { _net[x, y] |= TreeMatrixNet.Right; }
                    if (
                         ((PFDataHelper.EnumHasFlag(_net[x, y - 1], TreeMatrixNet.Down)))
                         || (_node[x, y - 1])//上格有下方向线或者是节点时，本格加上线
                       )
                    {
                        _net[x, y] |= TreeMatrixNet.Up;
                    }
                    if (
                        !((!PFDataHelper.EnumHasFlag(_net[x, y], TreeMatrixNet.Up)) || _lastChild[x + 1, y])//上格有下方向线或者是节点时，本格加上线
                       )
                    {
                        _net[x, y] |= TreeMatrixNet.Down;
                    }

                }
            }
        }
        /// <summary>
        /// 查询连线网某个点的线条形状
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TreeMatrixNet GetNetLine(int x, int y)
        {
            return _net[x, y];
        }
        public string GetNetLineString(int x, int y)
        {
            var net = _net[x, y];
            string urd = "┝", //(上右下)
                ud = "│", //(上下)
                ur = "┕";//(上右)

            if (PFDataHelper.EnumHasFlag(net, TreeMatrixNet.Up) && PFDataHelper.EnumHasFlag(net, TreeMatrixNet.Right) && PFDataHelper.EnumHasFlag(net, TreeMatrixNet.Down))
            {
                return urd;// "linearea-urd";
            }
            if (PFDataHelper.EnumHasFlag(net, TreeMatrixNet.Up) && PFDataHelper.EnumHasFlag(net, TreeMatrixNet.Down))
            {
                return ud;// "linearea -ud";
            }
            if (PFDataHelper.EnumHasFlag(net, TreeMatrixNet.Up) && PFDataHelper.EnumHasFlag(net, TreeMatrixNet.Right))
            {
                //return "tree-tr-linearea-ur";
                return ur;// "linearea-ur";
            }
            return "  ";
        }
    }

    public class Pop3Exception : Exception
    {
        public Pop3Exception(string s) : base(s) { }
    }
    /// <summary>
    /// 便于生成 200PV以下	200-299PV... 6000或以上 这种列
    /// </summary>
    public class PFVolRange
    {
        public bool IsBegin { get; set; }
        public bool IsEnd { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public string Title { get; set; }
        public string Code { get { return IsEnd ? ("over" + Min) : Max.ToString(); } }
        public static List<PFVolRange> Init(int[] vols, string unit)
        {
            var result = new List<PFVolRange>();

            var idx = 0;
            foreach (var i in vols)
            {
                var item = new PFVolRange();
                if (idx == 0)
                {
                    item.IsBegin = true;
                    item.Max = i;
                    item.Title = string.Format("{0}{1}以下", i - 1, unit);
                }
                else
                {
                    item.Max = i;
                    item.Min = vols[idx - 1];
                    item.Title = string.Format("{0}-{1}{2}", item.Min, item.Max - 1, unit);
                }
                result.Add(item);
                idx++;
            }
            var end = new PFVolRange
            {
                IsEnd = true,
                Min = vols[vols.Length - 1]
            };
            end.Title = string.Format("{0}{1}以上", end.Min, unit);
            result.Add(end);
            return result;
        }
        public bool IsInRange(int num)
        {
            return (IsBegin || num >= Min)
                    && (IsEnd || num < Max);
        }
        public bool IsInRange(decimal num)
        {
            return (IsBegin || num >= Min)
                    && (IsEnd || num < Max);
        }
    }
    public class PFCmonth
    {
        private string _cmonth;
        private string _ym;
        /// <summary>
        /// 年月:如2018.01
        /// </summary>
        public string Cmonth { get { return _cmonth; } set { _cmonth = value; _ym = (_cmonth ?? "").Replace(".", ""); } }
        /// <summary>
        /// 年月:如201801
        /// </summary>
        public string Ym { get { return _ym; } set { _ym = value; if (_ym != null && _ym.Length > 5) { _cmonth = _ym.Insert(4, "."); } } }
    }
    /// <summary>
    /// 迁移项
    /// </summary>
    public class PFSqlTransferItem
    {
        //public bool _hasWhereMonth=false;
        private bool _useDataReader = false;
        private bool _removeOldData = true;
        private Func<DataTable, DataTable> _dataRender = null;
        private string _srcConnName = null;
        //private string _dstConnName = "dayConnection";
        private string _dstConnName =null;
        public string SrcConn { get; set; }
        //public string SrcConnName { get; set; }
        public string SrcConnName {
            //get { return _srcConnName; }
            set { _srcConnName = value;SrcConn = ConfigurationManager.ConnectionStrings[value].ConnectionString; }
        }
        public PFSqlType? SrcSqlType { get; set; }
        public string DstConn { get; set; }
        /// <summary>
        /// 原来都是33.balance(DaySqlExec),现在多了华姐的
        /// </summary>
        //public string DstConnName { get { return _dstConnName; } set { _dstConnName = value; } }
        public string DstConnName
        {
            //get { return _dstConnName; }
            set { _dstConnName = value; DstConn = ConfigurationManager.ConnectionStrings[value].ConnectionString; }
        }
        public PFSqlType? DstSqlType { get; set; }
        //public string SrcSql { get { return PFDataHelper.ReadLocalTxt("mySqlTest_" + DstTableName + ".txt"); } }
        public string SrcSql { get; set; }
        //public DayTableEnum? DstTable { get; set; }
        //public string DstTableName { get { return DstTable.HasValue ? DstTable.ToString() : null; } }
        public string DstTableName { get; set; }
        public string ProcedureName { get; set; }
        //public Action<SqlInsertCollection> BeforeInsertAction { get; set; }
        public Action<BaseSqlUpdateCollection> BeforeInsertAction { get; set; }
        public Action<DataRow> BeforeBulkAction { get; set; }
        /// <summary>
        /// 转移数据之后(执行存储过程之前)的操作(如转Json,根据增量表更新主表等)
        /// </summary>
        //public Action<DbReportService> AfterTransferAction { get; set; }
        public Action AfterTransferAction { get; set; }
        ///// <summary>
        ///// 有sql的where条件(pay_finish_time >= '2019-04-01' and pay_finish_time 小于等于 '2019-05-01')
        ///// </summary>
        //public bool HasWhereMonth { get { return _hasWhereMonth; } set { _hasWhereMonth = value; } }
        /// <summary>
        /// 使用dataReader,当不需要转换数据时,直接用dataReader效率更高
        /// </summary>
        public bool UseDataReader { get { return _useDataReader && _dataRender == null; } set { _useDataReader = value; } }
        //public bool UseDataReader { get { return DataRender!=null; } }//考虑启用这句

        /// <summary>
        /// 数量大的时候要设置不超时
        /// </summary>
        public bool IsHugeData { get; set; }
        /// <summary>
        /// 便于做一些数据转换
        /// </summary>
        public Func<DataTable, DataTable> DataRender { get { return _dataRender; } set { _dataRender = value; } }
        public Func<string, DateTime, string> SqlRender { get; set; }

        public SqlInsertCollection InsertCollection { get; set; }

        /// <summary>
        /// 源数据分表
        /// </summary>
        public string[] SrcTableSeparate { get; set; }

        /// <summary>
        /// 默认删除原数据,王总的t_orders_sum例外
        /// </summary>
        public bool RemoveOldData { get { return _removeOldData; } set { _removeOldData = value; } }
        public int? BulkBatch { get; set; }
        /// <summary>
        /// 是增量表
        /// </summary>
        public bool IsTableChange { get; set; }
        ///// <summary>
        ///// 上次增量更新的时时间
        ///// </summary>
        //public DateTime? LastChangeUpdateDate { get; set; }
        /// <summary>
        /// 有对应的增量表
        /// </summary>
        public bool HasTableChange { get; set; }
    }

    public enum PFSqlType
    {
        SqlServer, MySql
    }

    public class PFChineseCity
    {
        public string Province { get; set; }
        public string City { get; set; }
        /// <summary>
        /// 纬
        /// </summary>
        public decimal Latitude { get; set; }
        /// <summary>
        /// 经
        /// </summary>
        public decimal Longitude { get; set; }
    }
}
