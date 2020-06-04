//using MySql.Data.MySqlClient;
//using Newtonsoft.Json.Linq;
using Perfect;
//using SaveDbReport.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//using YJQuery.Models;

namespace Perfect
{
    /// <summary>
    /// 如果类里有读Config的字段,在Control的继承类(如CMonthComboBox)的构造方法里使用时会报错,xxComboBox未声明或未赋值
    /// </summary>
    public static class ProjDataHelperFunc
    {
        public static List<string> GetRecentCMonthList()
        {

            var now = DateTime.Now;
            int idx = 0;
            var result = new List<string>();
            //cmonthDGView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            for (int i = 0; i < 12; i++)
            {
                var cmonth = PFDataHelper.ObjectToDateString(now, "yyyy.MM");
                result.Add(cmonth);
                now = now.AddMonths(-1);
                idx++;
            }
            return result;
        }

    }
    public static class ProjDataHelper
    {
        //public static Color ErrorColor = Color.Red;
        //public static Color FinishColor = Color.Blue;
        //public static Color UnfinishColor = Color.Gray;
        public static string SrcSqlExec = ConfigurationManager.ConnectionStrings["srcConnection"].ConnectionString;
        public static string DstSqlExec = ConfigurationManager.ConnectionStrings["dstConnection"].ConnectionString;

        public static int CheckMessageInterval { get { return int.Parse(ConfigurationManager.AppSettings["CheckMessageInterval"]); } }
        public static string[] DisabledTask
        { get {
                var sDisableTask = ConfigurationManager.AppSettings["DisabledTask"];
                return sDisableTask == null ? null : sDisableTask.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            } }
        //public static string[] DisAutorunTask
        //{
        //    get
        //    {
        //        var s = ConfigurationManager.AppSettings["DisAutorunTask"];
        //        return s == null ? null : s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    }
        //}
        public static BackupDatabase CurrentBackupDatabase;
        public static PFTabControl MainTab;
        private static string _currentCMonth = "";
        ///// <summary>
        ///// 日结表(30.balance)
        ///// </summary>
        //public static string[] DayTables =new string[] { "t_agent", "t_hyzl", "t_inv", "t_invtype", "t_orders", "t_ordersdetail" };
        
        public static string CurrentCMonth {
            get {
                if (PFDataHelper.StringIsNullOrWhiteSpace(_currentCMonth))
                {
                    var now = DateTime.Now.AddMonths(-1);
                    var year = now.Year;
                    var month = now.Month;
                    _currentCMonth = year + "." + month.ToString("00");
                }
                return _currentCMonth;
            }
            set
            {
                _currentCMonth = value;
            }
        }

        public static string EmailReceiveHostName = "pop.qq.com";
        public static string EmailUserName = "251561897@qq.com";
        public static string EmailPwd = "llcffwhxezsicadi";

        /// <summary>
        /// 更新模式,默认增量更新
        /// </summary>
        public static UpdateMode UpdateMode = UpdateMode.ChangeData;
        public static DateTime? InventoryBeginTime = null;
        public static DateTime? InventoryEndTime = null;

        public static List<BackupDatabase> GetBackupDatabases() {

            string xmlfile = Path.Combine(PFDataHelper.BaseDirectory, "XmlConfig\\BackupDatabaseConfig.xml");
            var _xml = new XmlDocument();
            _xml.Load(xmlfile);
            var _syses = _xml.SelectSingleNode("Database").ChildNodes;
            var result = new List<BackupDatabase>();
            foreach (XmlNode sys in _syses)
            {
                result.Add(new BackupDatabase(sys));
            }
            return result;
        }

        //public static List<string> GetRecentCMonthList() {

        //    var now = DateTime.Now;
        //    int idx = 0;
        //    var result = new List<string>();
        //    //cmonthDGView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        //    for (int i = 0; i < 12; i++)
        //    {
        //        var cmonth = PFDataHelper.ObjectToDateString(now, "yyyy.MM");
        //        result.Add(cmonth);
        //        now = now.AddMonths(-1);
        //        idx++;
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 批量导入(考虑版本号)(陈超)
        ///// </summary>
        ///// <param name="connStr">MySql连接字符串</param>
        ///// <param name="tableNameT">目标表名</param>
        ///// <param name="csvFileName">csc文件绝对路径</param>
        ///// <param name="columnNames">列名列表，可以使用@DataTable.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList()</param>
        ///// <returns></returns>
        //public static int BulkLoad(string connStr, string tableNameT, string csvFileName, List<string> columnNames)
        //{
        //    using (MySqlConnection conn = new MySqlConnection(connStr))
        //    {
        //        conn.Open();
        //        MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
        //        {
        //            FieldTerminator = ",",
        //            FieldQuotationCharacter = '"',
        //            EscapeCharacter = '"',
        //            LineTerminator = "\r\n",
        //            FileName = csvFileName,
        //            NumberOfLinesToSkip = 0,
        //            TableName = tableNameT,
        //        };
        //        bulk.Columns.AddRange(columnNames);
        //        int count = bulk.Load();
        //        return count;
        //    }
        //}

        /// <summary>
        /// 转换成CSV格式,边处理边清理DataTable（防止内存溢出）。
        /// 执行完之后DataTable数据已经被清空了（不是Null，只是Rows.Count=0）。
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool DataToCSV(ref DataTable dataTable, string fileName)
        {
            ////以半角逗号（即,）作分隔符，列为空也要表达其存在。
            ////列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            ////列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            //if (!HadData(dataTable))
            //    return false;
            if (dataTable == null || dataTable.Rows.Count < 1) { return false; }
            if (File.Exists(fileName))
                File.Delete(fileName);
            //using (StreamWriter streamWriter = new StreamWriter(fileName, true, Encoding.UTF8))//中文乱码
            using (StreamWriter streamWriter = new StreamWriter(fileName, true, UnicodeEncoding.GetEncoding(PFEncodeType.GB2312.ToString())))
            {
                StringBuilder sb = new StringBuilder();
                DataColumn colum;
                DataRow row;
                int index = 0;
                //不使用foreach，为了边处理边清理DataTable
                while (dataTable.Rows.Count > 0)
                {
                    row = dataTable.Rows[0];
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        colum = dataTable.Columns[i];
                        if (i != 0)
                            sb.Append(",");
                        if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                            sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                        else
                            sb.Append(row[colum].ToString());
                    }
                    dataTable.Rows.RemoveAt(0); //关键代码，清理
                    index++;
                    sb.AppendLine();
                    //释放一次内存
                    if (index % (100 * 1000) == 0)
                    {
                        streamWriter.Write(sb.ToString());
                        streamWriter.Flush();
                        sb = new StringBuilder();
                    }
                    if (index % (1000 * 1000) == 0)
                        GC.Collect();
                }
                streamWriter.Write(sb.ToString());
            }
            return true;
        }

        /// <summary>
        /// 获得迁移的列表
        /// </summary>
        /// <returns></returns>
        //public static List<DayTable> GetDayTransferList(DayTableEnum[] dayTableEnums=null)
        //{
        //    #region old
            //OrderDataRender =(src)=> {
            //    var list = PFDataHelper.DataTableToList<DayOrders>(src, (agent, row) => {
            //        var s = row["amount_detail"] as string;
            //        var jObj = JObject.Parse(s);//这样是最方便的
            //        JToken value = null;
            //        if (jObj.TryGetValue("returnRates", out value))
            //        {
            //            agent.backrate = value.Value<decimal>();
            //        }
            //        if (jObj.TryGetValue("totalRebackAmount", out value))
            //        {
            //            agent.Backmoney = value.Value<decimal>();
            //        }
            //        if (jObj.TryGetValue("eCouponAmount", out value))
            //        {
            //            agent.Giftmoney = value.Value<decimal>();
            //        }
            //        if (jObj.TryGetValue("rebackAmount", out value))
            //        {
            //            agent.Discount = value.Value<decimal>();
            //        }
            //        if (jObj.TryGetValue("freightAmount", out value))
            //        {
            //            agent.dispatchmoney = value.Value<decimal>();
            //        }
            //        if (jObj.TryGetValue("couponAmount", out value))
            //        {
            //            agent.coupon = value.Value<decimal>();
            //        }
            //        agent.Orderno = PFDataHelper.SetWordsCharLength(agent.Orderno, 20);
            //    });
            //    return list.ListToDataTable();
            //} , 
            //OrderDetailDataRender =(src)=> {
            //    //cnt = dt.Rows.Count;
            //    foreach (DataRow row in src.Rows)
            //    {
            //        if (row["Inv_name"] != DBNull.Value)
            //        {
            //            row["Inv_name"] = PFDataHelper.SetWordsCharLength(row["Inv_name"].ToString(), 50);
            //        }
            //        if (row["Orderno"] != DBNull.Value)
            //        {
            //            row["Orderno"] = PFDataHelper.SetWordsCharLength(row["Orderno"].ToString(), 20);
            //        }
            //    }
            //    return src;
            //} ,
            //#endregion

            //#region 订单json解析
            //Action<DataRow> orderBeforeBulkAction = (row) =>
            //  {
            //    //var item = new DayOrdersUpdate();
            //    var s = row["amount_detail"].ToString();
            //    //item.Orderno = rdr["Orderno"].ToString();
            //    //item.amount_detail = s;
            //    var jObj = JObject.Parse(s);//这样是最方便的
            //    JToken value = null;
            //      if (jObj.TryGetValue("returnRates", out value))
            //      {
            //        //item.backrate = value.Value<decimal>();
            //        row["backrate"] = value.Value<decimal>();
            //      }
            //      if (jObj.TryGetValue("totalRebackAmount", out value))
            //      {
            //        //item.Backmoney = value.Value<decimal>();
            //        row["Backmoney"] = value.Value<decimal>();
            //      }
            //      if (jObj.TryGetValue("eCouponAmount", out value))
            //      {
            //        //item.Giftmoney = value.Value<decimal>();
            //        row["Giftmoney"] = value.Value<decimal>();
            //      }
            //      if (jObj.TryGetValue("rebackAmount", out value))
            //      {
            //        //item.Discount = value.Value<decimal>();
            //        row["Discount"] = value.Value<decimal>();
            //      }
            //      if (jObj.TryGetValue("freightAmount", out value))
            //      {
            //        //item.dispatchmoney = value.Value<decimal>();
            //        row["dispatchmoney"] = value.Value<decimal>();
            //      }
            //      if (jObj.TryGetValue("couponAmount", out value))
            //      {
            //        //item.coupon = value.Value<decimal>();
            //        row["coupon"] = value.Value<decimal>();
            //      }

            //  };
            //Action<SqlInsertCollection> orderBeforeInsertAction = (insert) =>
            //{
            //    //var item = new DayOrdersUpdate();
            //    var s = insert["amount_detail"].Value.ToString();
            //    //item.Orderno = rdr["Orderno"].ToString();
            //    //item.amount_detail = s;
            //    var jObj = JObject.Parse(s);//这样是最方便的
            //    JToken value = null;
            //    if (jObj.TryGetValue("returnRates", out value))
            //    {
            //        //item.backrate = value.Value<decimal>();
            //        insert["backrate"].Value = value.Value<decimal>();
            //    }
            //    if (jObj.TryGetValue("totalRebackAmount", out value))
            //    {
            //        //item.Backmoney = value.Value<decimal>();
            //        insert["Backmoney"].Value = value.Value<decimal>();
            //    }
            //    if (jObj.TryGetValue("eCouponAmount", out value))
            //    {
            //        //item.Giftmoney = value.Value<decimal>();
            //        insert["Giftmoney"].Value = value.Value<decimal>();
            //    }
            //    if (jObj.TryGetValue("rebackAmount", out value))
            //    {
            //        //item.Discount = value.Value<decimal>();
            //        insert["Discount"].Value = value.Value<decimal>();
            //    }
            //    if (jObj.TryGetValue("freightAmount", out value))
            //    {
            //        //item.dispatchmoney = value.Value<decimal>();
            //        insert["dispatchmoney"].Value = value.Value<decimal>();
            //    }
            //    if (jObj.TryGetValue("couponAmount", out value))
            //    {
            //        //item.coupon = value.Value<decimal>();
            //        insert["coupon"].Value = value.Value<decimal>();
            //    }

            //};
            //#endregion

            //#region SqlRender
//            Func<string, DateTime, string> inventorySqlRender = (srcSql, cDay1) =>
//            {
//                //if(InventoryBeginTime==null|| InventoryEndTime == null)
//                //{
//                    DateTime? bDate = null;
//                    DateTime? eDate = null;
//                    if (new DbReportService(null).GetYaHuoBeginEndTime(out bDate, out eDate))
//                    {
//                        InventoryBeginTime = bDate;
//                        InventoryEndTime = eDate;
//                    }
//                    else
//                    {

//                    var mailBody = string.Format(@"
//导入发货押货表失败:
//备份时间:
//{0}
//原因:
//ltdzlog where flag=0 没有查询到数据", cDay1.ToString(PFDataHelper.DateFormat));
//                    throw new Exception(mailBody);
//                }
//                //}
//                if (InventoryBeginTime!=null&&InventoryEndTime!=null)
//                {
//                    return srcSql
//                    .Replace("@time1", "'"+InventoryBeginTime.Value.ToString(PFDataHelper.DateFormat)+ "'" )
//                    .Replace("@time2", "'" + InventoryEndTime.Value.ToString(PFDataHelper.DateFormat) + "'")
//                    ;
//                }
//                return "";
//            };
            //#endregion SqlRender

        //    var result = new List<DayTable> {
        //        new DayTable {SrcConnName=UnicomConn.Hyzl,DstTable=DayTableEnum.l_us_organization_info,ProcedureName=null,UseDataReader=true },//被 pUpdateAgent 引用
        //        new DayTable {SrcConnName=UnicomConn.BaseData,DstTable=DayTableEnum.l_bd_area,ProcedureName=null ,UseDataReader=true},//被 pUpdateAgent 引用
        //        new DayTable {SrcConnName=UnicomConn.Hyzl,DstTable=DayTableEnum.l_us_user,ProcedureName=null ,UseDataReader=true,IsHugeData=true,InsertCollection=new SqlInsertCollection(new LUsUser()),HasTableChange=true},//被 pUpdateHyzl,pUpdateAgent 引用
        //        new DayTable {SrcConnName=UnicomConn.Hyzl,DstTable=DayTableEnum.l_us_user_change,ProcedureName=null ,UseDataReader=true,IsHugeData=true,InsertCollection=new SqlInsertCollection(new LUsUser()),IsTableChange=true},//被 pUpdateHyzl,pUpdateAgent 引用
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_card_map_user,ProcedureName=null ,UseDataReader=true,IsHugeData=true,BulkBatch=100000,HasTableChange=true,InsertCollection=new SqlInsertCollection(new LMmCardMapUser())},
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_card_map_user_change,ProcedureName=null ,UseDataReader=true,IsHugeData=true,BulkBatch=100000,IsTableChange=true,InsertCollection=new SqlInsertCollection(new LMmCardMapUser())
        //            //,SqlRender =(srcSql,cDay)=> {
        //            //    return srcSql
        //            //        .Replace("{YesterdayStart}",cDay.AddDays(-1).GetDayStart().ToString(PFDataHelper.DateFormat) );
        //            //}
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_member_history_data,ProcedureName=null,UseDataReader=true,IsHugeData=true,HasTableChange=true },//被 p_update_t_hyzl 引用
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_member_history_data_change,ProcedureName=null,UseDataReader=true,IsHugeData=true,IsTableChange=true },//被 p_update_t_hyzl 引用
        //        new DayTable {SrcConnName=UnicomConn.Agent,DstTable=DayTableEnum.l_sc_shop,ProcedureName=null,UseDataReader=true },
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_member_card,ProcedureName=null,UseDataReader=true ,IsHugeData=true,BulkBatch=100000,HasTableChange=true},
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_member_card_change,ProcedureName=null,UseDataReader=true ,IsHugeData=true,BulkBatch=100000,IsTableChange=true
        //            //,SqlRender =(srcSql,cDay)=> {
        //            //    return srcSql
        //            //        .Replace("{YesterdayStart}",cDay.AddDays(-1).GetDayStart().ToString(PFDataHelper.DateFormat) );
        //            //}
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_member,ProcedureName=null,UseDataReader=true,IsHugeData=true ,BulkBatch=100000,HasTableChange=true},
        //        new DayTable {SrcConnName=UnicomConn.Member,DstTable=DayTableEnum.l_mm_member_change,ProcedureName=null,UseDataReader=true,IsHugeData=true ,BulkBatch=100000,IsTableChange=true
        //            //,SqlRender =(srcSql,cDay)=> {
        //            //    return srcSql
        //            //        .Replace("{YesterdayStart}",cDay.AddDays(-1).GetDayStart().ToString(PFDataHelper.DateFormat) );
        //            //}
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Iscm,DstTable=DayTableEnum.t_hyzl_bank,ProcedureName="p_update_t_hyzl_bank",UseDataReader=true ,IsHugeData=true},
        //        new DayTable {SrcConnName=UnicomConn.Hyzl,DstTable=DayTableEnum.t_hyzl_pe,ProcedureName=null,UseDataReader=true ,IsHugeData=true},
        //        new DayTable {SrcConnName=UnicomConn.Hyzl,DstTable=DayTableEnum.t_hyzl,ProcedureName="p_update_t_hyzl",
        //            UseDataReader =true,IsHugeData=true,
        //        //,DataRender=(src)=> {
        //        //    var list = PFDataHelper.DataTableToList<DayHyzl>(src);
        //        //    list = list.Distinct(new HyzlComparer()).ToList();
        //        //    var random = new Random();
        //        //    foreach (var i in list)
        //        //    {
        //        //        i.RepairField(ref random);
        //        //    }
        //        //    return list.ListToDataTable();
        //        //}
        //            AfterTransferAction=(service)=> {
        //                string ms=null;
        //                List<string> errors=null;
        //                var b = service.FixHyzlData( null, out ms,out errors);
        //                if(errors!=null) {errors.Clear(); }
        //            }
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Hyzl,DstTable=DayTableEnum.t_tmp_hyzl,ProcedureName="p_update_t_tmp_hyzl",
        //            UseDataReader =true,IsHugeData=true,
        //        //,DataRender=(src)=> {
        //        //    var list = PFDataHelper.DataTableToList<DayHyzl>(src);
        //        //    list = list.Distinct(new HyzlComparer()).ToList();
        //        //    var random = new Random();
        //        //    foreach (var i in list)
        //        //    {
        //        //        i.RepairField(ref random);
        //        //    }
        //        //    return list.ListToDataTable();
        //        //}
        //            AfterTransferAction=(service)=> {
        //                string ms=null;
        //                List<string> errors=null;
        //                var b = service.FixHyzlData( null, out ms,out errors);
        //                if(errors!=null) {errors.Clear(); }
        //            }
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Agent,DstTable=DayTableEnum.t_agent,
        //            ProcedureName ="p_update_t_agent",
        //            IsHugeData =true,DataRender=(src)=> {
        //                var random=new Random();

        //                var list = PFDataHelper.DataTableToList<DayAgent>(src, (agent, row) => {
        //                var s = row["l_detail_json"] as string;
        //                if (!PFDataHelper.StringIsNullOrWhiteSpace(s))
        //                {
        //                    try
        //                    {
        //                        var jObj = JObject.Parse(s);//这样是最方便的
        //                        JToken value = null;
        //                        if (jObj.TryGetValue("address", out value))
        //                        {
        //                            agent.Sendaddress = value.Value<string>();
        //                            //row["Sendaddress"] = value.Value<string>();
        //                        }
        //                    }
        //                    catch (Exception) { }
        //                }
        //                agent.RepairField(ref random);
        //            });

        //            return list.ListToDataTable();
        //        } },
        //        new DayTable {SrcConnName=UnicomConn.Inv,DstTable=DayTableEnum.t_inv,ProcedureName=null ,DataRender=(src)=> {
                    
        //            //cnt = dt.Rows.Count;
        //            foreach (DataRow row in src.Rows)
        //            {
        //                if (row["Inv_name"] != DBNull.Value)
        //                {
        //                    row["Inv_name"] = PFDataHelper.SetWordsCharLength(row["Inv_name"].ToString(), 50);
        //                }
        //            }
        //            return src;
        //        } },
        //        new DayTable {SrcConnName=UnicomConn.Inv,DstTable=DayTableEnum.l_it_item_version,ProcedureName="p_update_t_invtype" ,UseDataReader=true},

        //        new DayTable {SrcConnName=UnicomConn.Trade,DstTable=DayTableEnum.t_orders,ProcedureName=null,
        //            UseDataReader =true,IsHugeData=true,
        //            InsertCollection=new SqlInsertCollection(new DayOrders()),
        //            SqlRender =(srcSql,cDay)=> {
        //                var orderCMonth=ProjDataHelper.GetOrderCMonthTime(cDay);
        //                return srcSql
        //                    .Replace("{OrderCMonthStart}",orderCMonth.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                    .Replace("{OrderCMonthEnd}",orderCMonth.GetMonthEnd().ToString(PFDataHelper.DateFormat) );
        //                //var orderCMonth=ProjDataHelper.GetOrderCMonth(cDay);
        //                //return srcSql
        //                //    .Replace("{OrderCMonth}",orderCMonth );
        //            },
        //            BeforeInsertAction=orderBeforeInsertAction,
        //            BeforeBulkAction=orderBeforeBulkAction,
        //            AfterTransferAction=(service)=> {
        //                string ms=null;
        //                var b = service.UpdateOrdersFromJsonField("balance.dbo.t_orders", null, out ms);
        //            }
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Trade,DstTable=DayTableEnum.t_ordersdetail,ProcedureName=null,
        //            UseDataReader=true,IsHugeData=true,
        //            SqlRender =(srcSql,cDay)=> {
        //                var orderCMonth=ProjDataHelper.GetOrderCMonthTime(cDay);
        //                return srcSql
        //                    .Replace("{OrderCMonthStart}",orderCMonth.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                    .Replace("{OrderCMonthEnd}",orderCMonth.GetMonthEnd().ToString(PFDataHelper.DateFormat) );
        //            //return srcSql
        //            //    .Replace("{OrderCMonth}", ProjDataHelper.GetOrderCMonth(cDay));
        //            },
        //            AfterTransferAction=(service)=> {
        //                string ms=null;
        //                var b = service.UpdateOrdersWithoutDetail("t_orders", null, out ms);
        //            }
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Trade,DstTable=DayTableEnum.t_cur_orders,ProcedureName=null,
        //            UseDataReader =true,IsHugeData=true,
        //            InsertCollection=new SqlInsertCollection(new DayOrders()),
        //            SqlRender =(srcSql,cDay)=> {
        //                return srcSql
        //                    .Replace("{OrderCMonthStart}",cDay.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                    .Replace("{OrderCMonthEnd}",cDay.GetMonthEnd().ToString(PFDataHelper.DateFormat) );
        //            },
        //            BeforeInsertAction=orderBeforeInsertAction,
        //            BeforeBulkAction=orderBeforeBulkAction,
        //            AfterTransferAction=(service)=> {
        //                string ms=null;
        //                var b = service.UpdateOrdersFromJsonField("balance.dbo.t_cur_orders", null, out ms);
        //            }
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Trade,DstTable=DayTableEnum.t_cur_ordersdetail,ProcedureName="p_update_t_orders",
        //            UseDataReader=true,IsHugeData=true,
        //            SqlRender =(srcSql,cDay)=> {
        //                return srcSql
        //                    .Replace("{OrderCMonthStart}",cDay.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                    .Replace("{OrderCMonthEnd}",cDay.GetMonthEnd().ToString(PFDataHelper.DateFormat) );
        //            },
        //            AfterTransferAction=(service)=> {
        //                string ms=null;
        //                var b = service.UpdateOrdersWithoutDetail("t_cur_orders", null, out ms);
        //            }
        //        },


        //        new DayTable {SrcConnName=UnicomConn.Trade,DstTable=DayTableEnum.t_tmp_orders,ProcedureName=null,
        //            UseDataReader =true,IsHugeData=true,
        //            InsertCollection=new SqlInsertCollection(new DayOrders()),
        //            SqlRender =(srcSql,cDay)=> {
        //                return srcSql
        //                    .Replace("{OrderCMonthStart}",cDay.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                    .Replace("{OrderCMonthEnd}",cDay.GetMonthEnd().ToString(PFDataHelper.DateFormat) );
        //            },
        //            BeforeInsertAction=orderBeforeInsertAction,
        //            BeforeBulkAction=orderBeforeBulkAction,
        //            AfterTransferAction=(service)=> {
        //                string ms=null;
        //                var b = service.UpdateOrdersFromJsonField("balance.dbo.t_tmp_orders", null, out ms);
        //            }
        //        },
        //        new DayTable {SrcConnName=UnicomConn.Trade,DstTable=DayTableEnum.t_tmp_ordersdetail,ProcedureName=null,
        //            UseDataReader=true,IsHugeData=true,
        //            SqlRender =(srcSql,cDay)=> {
        //                return srcSql
        //                    .Replace("{OrderCMonthStart}",cDay.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                    .Replace("{OrderCMonthEnd}",cDay.GetMonthEnd().ToString(PFDataHelper.DateFormat) );
        //        }},

        //         new DayTable {
        //                        SrcConnName =UnicomConn.Trade,DstTable=DayTableEnum.t_orders_sum,ProcedureName=null,
        //                        UseDataReader =true,IsHugeData=true,
        //                        RemoveOldData=false,
        //                        SqlRender =(srcSql,cDay1)=> {
        //                                var orderSumNextId=new DbReportService(null).GetOrdersSumMaxId()+1;
        //                                var cDay2=cDay1.AddDays(-1);//如当天是x月1号,那应该取上月的数(王天庆)--benjamin20191008
        //                                return srcSql
        //                                    .Replace("{OrderSumNextId}",orderSumNextId.ToString())
        //                                    //.Replace("{NaturalMonthStart}",cDay1.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                                    //.Replace("{NaturalMonthEnd}",new DateTime(cDay1.Year,cDay1.Month,cDay1.Day,7,0,0).ToString(PFDataHelper.DateFormat) )
        //                                    .Replace("{NaturalMonthStart}",cDay2.GetMonthStart().ToString(PFDataHelper.DateFormat) )
        //                                    .Replace("{NaturalMonthEnd}",cDay2.GetDayEnd().ToString(PFDataHelper.DateFormat) )
        //                                    ;
        //                        }
        //                    },
        //        new DayTable {SrcConnName=UnicomConn.Trade,DstTable=DayTableEnum.l_tr_order_coupon,ProcedureName=null,UseDataReader=true,IsHugeData=true,SqlRender=(srcSql,cDay)=> {
        //            return srcSql
        //                .Replace("{OrderCMonth}", ProjDataHelper.GetOrderCMonth(cDay));
        //        } },
        //        new DayTable {SrcConnName=UnicomConn.Promotion,DstTable=DayTableEnum.t_gift,ProcedureName=null,UseDataReader=true,IsHugeData=true 
        //        //,SqlRender=(srcSql,cDay)=> {//阿伟说要全量
        //        //    //var orderDate=cDay.AddMonths(-1).GetMonthStart().ToString(PFDataHelper.DateFormat);
        //        //    var orderCMonth=ProjDataHelper.GetOrderCMonth(cDay);
        //        //    DateTime? s;
        //        //    DateTime? e;
        //        //    PFDataHelper.CMonthToDateRange(orderCMonth,out s,out e);
        //        //    return srcSql
        //        //        .Replace("{OrderDate}", s.Value.ToString(PFDataHelper.DateFormat));
        //        //}
        //        ,SrcTableSeparate=new string[] { "0","15"}//待改为16个分表--benjamin todo
        //        },

        //        //江华压货表
        //        new DayTable {SrcConnName=UnicomConn.Inventory,DstConnName=SqlConn.Ckgl,DstTable=DayTableEnum.ltdzyahuo,ProcedureName=null,UseDataReader=true,IsHugeData=true ,
        //                        SqlRender =inventorySqlRender},
        //        new DayTable {SrcConnName=UnicomConn.Inventory,DstConnName=SqlConn.Ckgl,DstTable=DayTableEnum.ltdzfahuo,ProcedureName=null ,UseDataReader=true,IsHugeData=true,
        //                        SqlRender =inventorySqlRender}
        //    };

        //    if (dayTableEnums == null)
        //    {
        //        return result;
        //    }
        //    return result.Where(a => a.DstTable != null&& dayTableEnums.Contains(a.DstTable.Value)).ToList()
        //        ;
        //}

        ///// <summary>
        ///// 江华压货表
        ///// </summary>
        ///// <param name="dayTableEnums"></param>
        ///// <returns></returns>
        //public static List<DayTable> GetYaHuoTransferList()//DayTableEnum[] dayTableEnums = null)
        //{
        //    return ProjDataHelper.GetDayTransferList(new DayTableEnum[] {
        //        DayTableEnum.ltdzyahuo,
        //        DayTableEnum.ltdzfahuo
        //    });

        //    //var result = new List<DayTable> {
        //    //    new DayTable {SrcConnName=UnicomConn.Inventory,DstConnName=SqlConn.Ckgl,DstTable=DayTableEnum.ltdzyahuo,ProcedureName=null,UseDataReader=true,IsHugeData=true },//被 pUpdateAgent 引用
        //    //    new DayTable {SrcConnName=UnicomConn.Inventory,DstConnName=SqlConn.Ckgl,DstTable=DayTableEnum.ltdzfahuo,ProcedureName=null ,UseDataReader=true,IsHugeData=true},//被 pUpdateAgent 引用
        //    //};

        //    //if (dayTableEnums == null)
        //    //{
        //    //    return result;
        //    //}
        //    //return result.Where(a => a.DstTable != null && dayTableEnums.Contains(a.DstTable.Value)).ToList()
        //    //    ;
        //}

        ///// <summary>
        ///// 会员资料的全量表
        ///// </summary>
        ///// <returns></returns>
        //public static List<DayTable> GetDayTransferHyzlFullList()
        //{
        //    return ProjDataHelper.GetDayTransferList(new DayTableEnum[] {
        //        DayTableEnum.l_us_organization_info,
        //        DayTableEnum.l_bd_area,
        //        DayTableEnum.l_us_user,
        //        DayTableEnum.l_mm_card_map_user,
        //        DayTableEnum.l_mm_member_history_data,
        //        DayTableEnum.l_sc_shop,
        //        DayTableEnum.l_mm_member_card,
        //        DayTableEnum.l_mm_member,
        //        DayTableEnum.t_hyzl_bank,
        //        DayTableEnum.t_hyzl_pe,
        //        DayTableEnum.t_hyzl
        //    });
        //}

        ///// <summary>
        ///// 获得迁移的增量列表
        ///// </summary>
        ///// <returns></returns>
        //public static List<DayTable> GetDayTransferChangeList()
        //{
        //    return ProjDataHelper.GetDayTransferList(new DayTableEnum[] {
        //        DayTableEnum.l_us_organization_info,
        //        DayTableEnum.l_bd_area,
        //        DayTableEnum.l_us_user_change,
        //        DayTableEnum.l_mm_card_map_user_change,
        //        DayTableEnum.l_mm_member_history_data_change,
        //        DayTableEnum.l_sc_shop,
        //        DayTableEnum.l_mm_member_card_change,
        //        DayTableEnum.l_mm_member_change,
        //        DayTableEnum.t_hyzl_bank,
        //        DayTableEnum.t_hyzl_pe,
        //        DayTableEnum.t_hyzl,
        //        DayTableEnum.t_agent,
        //        DayTableEnum.t_inv,
        //        DayTableEnum.l_it_item_version,
        //        DayTableEnum.t_orders,
        //        DayTableEnum.t_ordersdetail,
        //        DayTableEnum.t_cur_orders,
        //        DayTableEnum.t_cur_ordersdetail
        //    });
        //}

        ////public static List<DayTable> DayTransferList { get; set; }
        /////// <summary>
        /////// 上次增量更新的时间
        /////// </summary>
        ////public static DateTime? LastChangeUpdateTime { get; set; }
        ///// <summary>
        ///// 获得迁移的全量列表
        ///// </summary>
        ///// <returns></returns>
        //public static List<DayTable> GetDayTransferFullList()
        //{
        //    return ProjDataHelper.GetDayTransferList(new DayTableEnum[] {
        //        DayTableEnum.l_us_organization_info,
        //        DayTableEnum.l_bd_area,
        //        DayTableEnum.l_us_user,
        //        DayTableEnum.l_mm_card_map_user,
        //        DayTableEnum.l_mm_member_history_data,
        //        DayTableEnum.l_sc_shop,
        //        DayTableEnum.l_mm_member_card,
        //        DayTableEnum.l_mm_member,
        //        DayTableEnum.t_hyzl_bank,
        //        DayTableEnum.t_hyzl_pe,
        //        DayTableEnum.t_hyzl,
        //        DayTableEnum.t_agent,
        //        DayTableEnum.t_inv,
        //        DayTableEnum.l_it_item_version,
        //        DayTableEnum.t_orders,
        //        DayTableEnum.t_ordersdetail,
        //        DayTableEnum.t_cur_orders,
        //        DayTableEnum.t_cur_ordersdetail
        //    });
        //}

        #region old
        ///// <summary>
        ///// 获得迁移的增量列表
        ///// </summary>
        ///// <returns></returns>
        //public static List<DayTable> GetDayTransferChangeList() {
        //    return ProjDataHelper.GetDayTransferList()
        //                .Where(a =>
        //                    (
        //                        a.DstTable == null
        //                        || (!new DayTableEnum[] {
        //                            DayTableEnum.t_orders_sum, DayTableEnum.l_tr_order_coupon,DayTableEnum.t_gift
        //                            //,
        //                            //DayTableEnum.l_mm_card_map_user,DayTableEnum.l_mm_member_card,DayTableEnum.l_mm_member//使用增量表
        //                        }.Contains(a.DstTable.Value))
        //                    )
        //                    && (!a.HasTableChange)//使用增量表
        //                )
        //                //.Skip(9).ToList()
        //                //.Where(a => a.DstTable == DayTableEnum.t_orders || a.DstTable == DayTableEnum.t_ordersdetail).ToList()
        //                //.Skip(10)
        //                .ToList();
        //} 
        #endregion
        /// <summary>
        /// 被禁用的任务
        /// </summary>
        /// <param name="taskKey"></param>
        /// <returns></returns>
        public static bool IsDisabledTask(string taskKey) {
            return DisabledTask != null && DisabledTask.Contains(taskKey);
        }
        ///// <summary>
        ///// 不自动执行的任务
        ///// </summary>
        ///// <param name="taskKey"></param>
        ///// <returns></returns>
        //public static bool IsDisAutorunTask(string taskKey)
        //{
        //    return DisAutorunTask != null && DisAutorunTask.Contains(taskKey);
        //}
        public static string GetLastCMonth(string cmonth)
        {
            var year = int.Parse(cmonth.Substring(0, 4));
            var month = int.Parse(cmonth.Substring(5, 2));
            var lastMonth = new DateTime(year, month, 1).AddMonths(-1);
            return lastMonth.ToString("yyyy.MM");
        }
        /// <summary>
        /// 订单月份,15号之前是上个月,16号之后是本月
        /// </summary>
        /// <param name="cDay"></param>
        /// <returns></returns>
        public static string GetOrderCMonth(DateTime cDay)
        {
            return GetOrderCMonthTime(cDay).ToString(PFDataHelper.MonthFormat);
            //return cDay < new DateTime(cDay.Year, cDay.Month, 16)
            //    ? cDay.AddMonths(-1).ToString(PFDataHelper.MonthFormat)
            //    : cDay.ToString(PFDataHelper.MonthFormat);
        }
        /// <summary>
        /// 订单月份,15号之前是上个月,16号之后是本月
        /// </summary>
        /// <param name="cDay"></param>
        /// <returns></returns>
        public static DateTime GetOrderCMonthTime(DateTime cDay)
        {
            return cDay < new DateTime(cDay.Year, cDay.Month, 16)
                ? cDay.AddMonths(-1)
                : cDay;
        }



        ///**
        //* "pm_coupon_grant为分表，会员卡号作为分表键，查询需要根据卡号求出表索引，再找到对应分表查询
        //* 得到hsah子表名
        //* </p>
        //* @param splitKeyVal
        //*        表拆分键的值(根据splitKeyVal值来取模)<br>
        //* @param subTableCount
        //*        要拆分子表总数<br>
        //* @return
        //*/
        //public static String GetSplitTableName(String tableName, Object splitKeyVal, int subTableCount)
        //{
        //    if (splitKeyVal == null)
        //    {
        //        throw new Exception("splitKeyVal is null.tableName:" + tableName);
        //    }
        //    long hashVal = splitKeyVal.ToString().GetHashCode();
        //    // 斐波那契（Fibonacci）散列
        //    hashVal = (hashVal * 2654435769L) >> 28;
        //    // 避免hashVal超出 MAX_VALUE = 0x7fffffff时变为负数,取绝对值
        //    hashVal = Math.Abs(hashVal) % subTableCount;
        //    return tableName + "_" + hashVal;
        //}


        public static void SendEmail(string[] toEmails, string mailTitle, string mailBody)
        {
            var sendHostName = "smtp.qq.com";
            var userName = "251561897@qq.com";
            var pwd = "llcffwhxezsicadi";
            PFDataHelper.SendEmail(userName, pwd, sendHostName, toEmails, mailTitle, mailBody,a=> { a.IsBodyHtml = false; });
        }
    }

    public class BackupDatabase :IEquatable<BackupDatabase>
    {

        public BackupDatabase(XmlNode sys)
        {

            Database = sys.Name;
            CreateDatabaseProcedue = sys.SelectSingleNode("CreateDatabaseProcedue").InnerText;//本地项目路径
        }
        public string Database { get; set; }
        public string CreateDatabaseProcedue { get; set; }

        public bool Equals(BackupDatabase other)
        {
            if (other == null) { return false; }
            return Database==other.Database;
        }
    }
    /// <summary>
    /// 联通的MySql连接
    /// </summary>
    public static class UnicomConn
    {
        public static string Hyzl = "unicomHyzlConnection";
        public static string Agent = "unicomAgentConnection";
        public static string BaseData = "unicomBaseDataConnection";
        public static string Inv = "unicomInvConnection";
        public static string Trade = "unicomTradeConnection";
        public static string Iscm = "unicomIscmConnection";
        public static string Member = "unicomMemberConnection";
        public static string Promotion = "unicomPromotionConnection";

        public static string Inventory = "unicomInventoryConnection";
    }
    /// <summary>
    /// 本地的SqlServer连接
    /// </summary>
    public static class SqlConn
    {
        public static string Ckgl = "ckglConnection";
        public static string Day = "dayConnection";
    }
    //public class UnicomConn : PFCustomStringType
    //{
    //    public UnicomConn(string v) : base(v)
    //    {
    //    }
    //    public static UnicomConn Hyzl { get { return new UnicomConn("unicomHyzlConnection"); } }
    //    public static UnicomConn Agent { get { return new UnicomConn("unicomAgentConnection"); } }
    //    public static UnicomConn BaseData { get { return new UnicomConn("unicomBaseDataConnection"); } }
    //    public static UnicomConn Inv { get { return new UnicomConn("unicomInvConnection"); } }
    //    public static UnicomConn Trade { get { return new UnicomConn("unicomTradeConnection"); } }
    //    public static UnicomConn Iscm { get { return new UnicomConn("unicomIscmConnection"); } }
    //    public static UnicomConn Member { get { return new UnicomConn("unicomMemberConnection"); } }
    //    public static UnicomConn Promotion { get { return new UnicomConn("unicomPromotionConnection"); } }
    //}
    /// <summary>
    /// 日结表(30.balance)
    /// </summary>
    public enum DayTableEnum
    {
        t_agent=1, t_hyzl=2, t_inv=3,
        //t_invtype =4,//不用更新,通过l_it_item_version更新
        t_orders =5, t_ordersdetail=6,
        l_bd_area=7, t_hyzl_bank = 8, l_it_item_version=9,
        l_sc_shop = 10, l_us_organization_info= 11, l_us_user=12,
        //l_bank_base_info=13, l_bank_account_info=14,
        l_mm_card_map_user =15,
        l_mm_member_history_data=16, l_tr_order_coupon=17,t_gift = 18, t_hyzl_pe=19, t_orders_sum=20,
        l_mm_member_card =21, l_mm_member=22,
        t_cur_orders=23, t_cur_ordersdetail=24,
        l_mm_card_map_user_change =25, l_mm_member_card_change=26, l_mm_member_change = 27,
        l_mm_member_history_data_change =28, l_us_user_change=29,
        t_tmp_orders =30, t_tmp_ordersdetail=31, //便于临时读其它月份的订单
        t_tmp_hyzl=32,

        //江华压货表
        ltdzyahuo=33,
        ltdzfahuo=34
    }
    public class DayTable
    {
        //public bool _hasWhereMonth=false;
        private bool _useDataReader = false;
        private bool _removeOldData = true;
        private Func<DataTable, DataTable> _dataRender = null;
        private string _dstConnName = "dayConnection";
        //public string SrcConn { get; set; }
        public string SrcConnName { get; set; }
        //public string DstConn { get { return _dstConnName; } set { _dstConnName = value; } }
        /// <summary>
        /// 原来都是33.balance(DaySqlExec),现在多了华姐的
        /// </summary>
        public string DstConnName { get { return _dstConnName; } set { _dstConnName = value; } }
        public string SrcSql { get { return PFDataHelper.ReadLocalTxt("mySqlTest_" + DstTableName + ".txt"); } }
        public DayTableEnum? DstTable { get; set; }
        //public string TableName { get; set; }
        public string DstTableName { get { return DstTable.HasValue ? DstTable.ToString() : null; } }
        public string ProcedureName { get; set; }
        public Action<SqlInsertCollection> BeforeInsertAction { get; set; }
        public Action<DataRow> BeforeBulkAction { get; set; }
        /// <summary>
        /// 转移数据之后(执行存储过程之前)的操作(如转Json,根据增量表更新主表等)
        /// </summary>
        //public Action<DbReportService> AfterTransferAction { get; set; }
        ///// <summary>
        ///// 有sql的where条件(pay_finish_time >= '2019-04-01' and pay_finish_time 小于等于 '2019-05-01')
        ///// </summary>
        //public bool HasWhereMonth { get { return _hasWhereMonth; } set { _hasWhereMonth = value; } }
        /// <summary>
        /// 使用dataReader,当不需要转换数据时,直接用dataReader效率更高
        /// </summary>
        public bool UseDataReader { get { return _useDataReader&& _dataRender == null; } set { _useDataReader = value; } }
        //public bool UseDataReader { get { return DataRender!=null; } }//考虑启用这句

        /// <summary>
        /// 数量大的时候要设置不超时
        /// </summary>
        public bool IsHugeData { get; set; }
        /// <summary>
        /// 便于做一些数据转换
        /// </summary>
        public Func<DataTable, DataTable> DataRender { get { return _dataRender; }set { _dataRender = value; } }
        public Func<string,DateTime,string> SqlRender { get; set; }

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
    /// <summary>
    /// 更新模式
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// 全量更新
        /// </summary>
        FullData=1,
        /// <summary>
        /// 增量更新
        /// </summary>
        ChangeData=2
    }
}
