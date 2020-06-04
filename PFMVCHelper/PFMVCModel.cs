using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
//using Perfect.MVC;
using System.Linq.Expressions;
using System.Collections;
using System.Data;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Perfect
{

    #region 生成前端Table组件的JSON数据模型(不要放在PFXxHelper里,因为Helper一般放在Web项目,而本类在Controller项目也会调用到(考虑多控制器且没有公共项目的情况,如生活网程序)

    public class PagingParametersModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;
            var requestWrapper = new MvcPagingParameters(new NameValueCollection(request.Params));
            return requestWrapper;
        }
    }
    /// <summary>
    /// 分页参数(使用单模型绑定是为了便于/Home/Download方法里导出时共用接口)
    /// </summary>
    [System.Web.Mvc.ModelBinder(typeof(PagingParametersModelBinder))]
    public partial class MvcPagingParameters : PagingParameters
    {
        public MvcPagingParameters(NameValueCollection query) : base(query)
        {
        }

        public MvcPagingParameters() : base()
        {
        }
    }
    ///// <summary>
    ///// 分页参数(使用单模型绑定是为了便于/Home/Download方法里导出时共用接口)
    ///// </summary>
    //[System.Web.Mvc.ModelBinder(typeof(PagingParametersModelBinder))]
    //public partial class MvcPagingParameters
    //{
    //    private int _pageIndex = 0;
    //    private int? _pageSize = null;
    //    private NameValueCollection request { get; set; }

    //    public int? PageIndex { get { return _pageIndex; } set { if (value.HasValue) { _pageIndex = value.Value; } } }//从0开始
    //    //public int? PageSize { get { return _pageSize; } set { if (value.HasValue) { _pageSize = value.Value; } } }
    //    public int? PageSize { get { return _pageSize; } set { _pageSize = value; } }
    //    public int? PageStart { get { return PageIndex == null || PageSize == null ? null : PageIndex * PageSize; } }//从0开始
    //    public int? PageEnd { get { return PageStart == null ? null : PageStart + PageSize - 1; } }//这个值有可能超出Table的Rows索引

    //    public string Sort { get; set; }
    //    /// <summary>
    //    /// 模糊查找
    //    /// </summary>
    //    public string FilterValue { get; set; }

    //    public string this[string name]
    //    {
    //        get
    //        {
    //            var field = string.Empty;
    //            if (name.IndexOf(".") >= 0) field = name.Split('.')[1];
    //            return request[field] ?? request[name]// ?? request[getAliasName(name)]
    //                ;
    //        }
    //        set
    //        {
    //            if (name.IndexOf(".") >= 0) name = name.Split('.')[1];
    //            request[name] = value;
    //        }
    //    }
    //    public MvcPagingParameters(NameValueCollection query)
    //    {
    //        this.SetRequestData(query);
    //    }

    //    public MvcPagingParameters()
    //    {
    //        this.SetRequestData(new NameValueCollection());
    //    }
    //    public MvcPagingParameters SetRequestData(NameValueCollection values)
    //    {
    //        this.request = values;
    //        foreach (var i in values.AllKeys)
    //        {
    //            int ti = 0;
    //            if (i.ToLower() == "pageindex" && int.TryParse(values[i], out ti))
    //            {
    //                PageIndex = ti;
    //            }
    //            else
    //            if (i.ToLower() == "pagesize" && int.TryParse(values[i], out ti))
    //            {
    //                PageSize = ti;
    //            }
    //            else
    //            if (i.ToLower() == "sort")
    //            {
    //                Sort = values[i];
    //            }
    //            else
    //            if (i.ToLower() == "filtervalue")
    //            {
    //                FilterValue = values[i];
    //            }
    //        }
    //        return this;
    //    }
    //    public MvcPagingParameters SetRequestData(JToken values)
    //    {
    //        if (values != null)
    //        {
    //            foreach (JProperty item in values.Children())
    //                if (item != null) this[item.Name] = item.Value.ToString();
    //        }
    //        return this;
    //    }
    //}
    ///// <summary>
    ///// 分页查询结果
    ///// </summary>
    //public class PagingResult
    //{
    //    public object data { get; set; }
    //    public object exData { get; set; }//为了减少前端多次请求,便于放其它数据
    //    public object columns { get; set; }
    //    public int total { get; set; }
    //}
    ///// <summary>
    ///// 汇总类型
    ///// </summary>
    //public enum SummaryType
    //{
    //    None=0,
    //    Sum=1,
    //    Average=2,
    //    Count=4
    //}
    ///// <summary>
    ///// 用于前端pfTable和pfTreeTable的列的json格式
    ///// </summary>
    //[JsonObject(MemberSerialization.OptOut)]
    //public class StoreColumn : TreeListItem<StoreColumn>
    //{
    //    private int _rowspan = 1;
    //    private int _colspan = 1;
    //    private bool _visible = true;
    //    private bool _hasSummary = false;
    //    private SummaryType _summaryType = SummaryType.None;
    //    public string data { get; set; }
    //    public string title { get; set; }
    //    [DefaultValue(null)]
    //    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    //    public string width { get; set; }//需然FieldSets里面不写px单位,但设到这里时补上
    //    [DefaultValue(1)]
    //    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    //    public int rowspan { get { return _rowspan; } set { _rowspan = value; } }
    //    [DefaultValue(1)]
    //    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    //    public int colspan { get { return _colspan; } set { _colspan = value; } }
    //    [DefaultValue(null)]
    //    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    //    public string dataType { get; set; }
    //    [DefaultValue(true)]
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    //    public bool visible { get { return _visible; } set { _visible = value; } }

    //    [JsonIgnore]
    //    public bool hasSummary { get { return _hasSummary; } set { if (value) { _summaryType = SummaryType.Sum; }_hasSummary = value; } }
    //    [JsonIgnore]
    //    public SummaryType summaryType { get { return _summaryType; } set { if (value != SummaryType.None) { _hasSummary = true; }_summaryType = value; } }
    //    [DefaultValue("")]
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    //    public object summary { get; set; }

    //    public StoreColumn() { }
    //    public StoreColumn(string fieldName)
    //    {
    //        title = data = fieldName;
    //    }
    //    public StoreColumn(DataColumn c):this(c.ColumnName)
    //    {
    //        dataType = PFDataHelper.GetStringByType(c.DataType);
    //    }
    //    public StoreColumn(DataColumn c, PFModelConfig config) : this(c.ColumnName)
    //    {
    //        dataType = PFDataHelper.GetStringByType(c.DataType);
    //        SetPropertyByModelConfig(config);
    //    }
    //    public StoreColumn(string fieldName, PFModelConfig config) : this(fieldName)
    //    {
    //        SetPropertyByModelConfig(config);
    //        //if (config != null)
    //        //{
    //        //    ////data = config.FieldName;
    //        //    title = config.FieldText;
    //        //    width = config.FieldWidth;
    //        //    dataType = PFDataHelper.GetStringByType(config.FieldType);
    //        //    visible = config.Visible;
    //        //}
    //    }
    //    public void SetPropertyByModelConfig(PFModelConfig config)
    //    {
    //        if (config != null)
    //        {
    //            //data = config.FieldName;
    //            title = config.FieldText;
    //            if (!string.IsNullOrWhiteSpace(config.FieldWidth)) { width = config.FieldWidth; }
    //            if (config.FieldType != null) { dataType = PFDataHelper.GetStringByType(config.FieldType); }
    //            visible = config.Visible;
    //        }
    //    }
    //    //public void SetPropertyByModelConfig(PFModelConfig config)
    //    //{
    //    //    if (config != null)
    //    //    {
    //    //        if (string.IsNullOrWhiteSpace(title))
    //    //        {
    //    //            title = config.FieldText;
    //    //        }
    //    //        if (width == null)
    //    //        {
    //    //            width = config.FieldWidth;
    //    //            //if (!string.IsNullOrWhiteSpace(width) && width.IndexOf("px") < 0)
    //    //            //{
    //    //            //    width += "px";
    //    //            //}
    //    //        }
    //    //        if (dataType == null && config.FieldType != null)
    //    //        {
    //    //            dataType = PFDataHelper.GetStringByType(config.FieldType);
    //    //        }
    //    //    }
    //    //    if (string.IsNullOrWhiteSpace(title)) { title = data; }
    //    //}
    //    public void SetWidthByTitleWords()
    //    {
    //        if (string.IsNullOrWhiteSpace(title)) { return; }
    //        width = PFDataHelper.GetWordsWidth(title);
    //    }
    //}
    //public class StoreColumnCollection : List<StoreColumn>
    //{
    //    protected PFModelConfigCollection _modelConfig;
    //    public StoreColumnCollection() { }
    //    public StoreColumnCollection(string modelConfig)
    //    {
    //        _modelConfig = PFDataHelper.GetMultiModelConfig(modelConfig);
    //    }
    //    public void Add(string data, Action<StoreColumn> action = null)//, bool setWidthByHeaderWord = true)
    //    {
    //        var config = _modelConfig == null ? null : _modelConfig[data];
    //        var c = new StoreColumn(data, config);

    //        if (action != null) { action(c); }//action里有可能改title,所以此句一定在GetWordsWidth之前--wxj20180906

    //        //if (//setWidthByHeaderWord&&
    //        //    string.IsNullOrWhiteSpace(c.width))//如果xml配置中有宽度,就不用计算字长了
    //        //{
    //        //    var w = PFDataHelper.GetWordsWidth(c.title ?? c.data);
    //        //    if (!string.IsNullOrWhiteSpace(w)) { c.width = w; }
    //        //}

    //        Add(c);
    //    }
    //    public void Add(DataColumn column, Action<StoreColumn> action = null)//, bool setWidthByHeaderWord = true)
    //    {
    //        var config = _modelConfig == null ? null : _modelConfig[column.ColumnName];
    //        var c = new StoreColumn(column, config);

    //        if (action != null) { action(c); }//action里有可能改title,所以此句一定在GetWordsWidth之前--wxj20180906

    //        //if (//setWidthByHeaderWord&&
    //        //    string.IsNullOrWhiteSpace(c.width))//如果xml配置中有宽度,就不用计算字长了
    //        //{
    //        //    var w = PFDataHelper.GetWordsWidth(c.title ?? c.data);
    //        //    if (!string.IsNullOrWhiteSpace(w)) { c.width = w; }
    //        //}

    //        Add(c);
    //    }
    //    /// <summary>
    //    /// 树型转二维数组
    //    /// </summary>
    //    /// <returns></returns>
    //    public static void StoreColumnTo2DArray(ref List<List<StoreColumn>> target, StoreColumnCollection columns, ref int maxDepth)
    //    {
    //        //var result = new List<List<StoreColumn>>();
    //        var floor = new List<StoreColumn>();
    //        var next = new StoreColumnCollection();
    //        var currentDepth = target.Count + 1;
    //        var rowSpan = maxDepth - currentDepth + 1;
    //        columns.ForEach(a => {
    //            if (a.Children.Any())
    //            {
    //                next.AddRange(a.Children);
    //                //a.colspan = a.Children.Count;
    //                a.colspan = a.GetAllLeafCount();
    //            }
    //            else
    //            {
    //                a.rowspan = rowSpan;
    //            }
    //            //a.Children = null;
    //            floor.Add(a);
    //        });
    //        target.Add(floor);
    //        if (next.Any())
    //        {
    //            StoreColumnTo2DArray(ref target, next, ref maxDepth);
    //        }
    //    }
    //}

    #region Json

    /// <summary>
    /// 控制器如果返回原生JsonResult,是调用new JavaScriptSerializer().Serialize(header);来序列化的,这种序列化如果想在类上加特性控制的话只有[ScriptIgnore]一种,功能太小不灵活
    /// 所以改为用JsonConvert序列化的话,就可以用[JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]等多种方式控制,非常灵活
    /// </summary>
    public class PFJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            var jsonSerizlizerSetting = new JsonSerializerSettings();
            ////设置取消循环引用
            //jsonSerizlizerSetting.MissingMemberHandling = MissingMemberHandling.Ignore;
            ////设置首字母小写
            //jsonSerizlizerSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            ////设置日期的格式为：yyyy-MM-dd
            //jsonSerizlizerSetting.DateFormatString = "yyy-MM-dd";//Newtonsoft.Json, Version=6.0.0.0 才支持此属性
            var json = JsonConvert.SerializeObject(Data, Formatting.None, jsonSerizlizerSetting);
            response.Write(json);
        }
    }
    #endregion
    #endregion
}
