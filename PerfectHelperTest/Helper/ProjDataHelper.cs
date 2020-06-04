using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Perfect;
using SaveDbReport.Service;
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
using YJQuery.Models;

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
        //public static string DstSqlExec = ConfigurationManager.ConnectionStrings["dstConnection"].ConnectionString;

        public static int CheckMessageInterval { get { return int.Parse(ConfigurationManager.AppSettings["CheckMessageInterval"]); } }

        public static string GetLastCMonthByDate(DateTime date)
        {
            var lastMonth = date.AddMonths(-1);
            return lastMonth.ToString("yyyy.MM");
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
        ltdzfahuo=34,

        //联通表的卡号有重复,先读到此表再转到t_hyzl
        l_hyzl = 35,
        //联通表的副卡号也有重复,先读到此表再转到t_hyzl_pe
        l_hyzl_pe = 36,
        l_hyzl_change=37,
        l_hyzl_pe_change=38
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
        public Action<DbReportService> AfterTransferAction { get; set; }
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
