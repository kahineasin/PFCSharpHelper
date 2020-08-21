using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Collections;
using System.Linq;

namespace Perfect
{
    #region old
    /// <summary>
    /// DataBase 的摘要说明。(NuGet安装MySql.Data)
    /// </summary>
    public class MySqlBase : IDisposable
    {
        //protected string _connectionstring = "data source=10.0.0.11;initial catalog=yjquery;persist security info=False;user id=sa;password=perfect;Connect Timeout=2000";// ConfigurationManager.ConnectionStrings["yjquery"].ConnectionString;
        //protected string _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        protected string _connectionstring = null;
        protected MySqlConnection _sqlconnection = new MySqlConnection();
        public MySqlBase()
        {
            //_sqlconnection.ConnectionString = _connectionstring;
            _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            _sqlconnection.ConnectionString = _connectionstring;
            if (_sqlconnection.State != ConnectionState.Open)
                _sqlconnection.Open();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_connString"></param>
        /// <param name="tryTimes">重试次数(因为联通的数据库不稳定)</param>
        public MySqlBase(string _connString, int tryTimes = 3)
        {
            var maxTryTimes = tryTimes;
            while (tryTimes > 0)
            {
                try
                {
                    _connectionstring = _connString;
                    _sqlconnection.ConnectionString = _connectionstring;
                    OpenConn();
                    //if (_sqlconnection.State != ConnectionState.Open)
                    //{
                    //    _sqlconnection.Open();
                    //}
                    break;
                }
                catch (Exception e)
                {
                    if (tryTimes == 1)
                    {
                        throw new Exception(string.Format("尝试打开[{0}]{1}次失败", _sqlconnection.Database, maxTryTimes));
                    }
                }
                tryTimes--;
            }
        }

        public void Connection(string _connString)
        {
            _connectionstring = _connString;

            if (_sqlconnection.State == ConnectionState.Open)
            {
                CloseConn();
                //_sqlconnection.Close();
            }
            _sqlconnection.ConnectionString = _connectionstring;
            _sqlconnection.Open();
        }

        public void OpenConn()
        {
            if (this._sqlconnection.State == ConnectionState.Closed)
            {
                this._sqlconnection.Open();
                SqlConnCounter.Add(_sqlconnection.ConnectionString);
            }
        }
        public void CloseConn()
        {
            SqlConnCounter.Subtract(_sqlconnection.ConnectionString);
            this._sqlconnection.Close();
        }
        public void Dispose()
        {
            if (_sqlconnection.State == ConnectionState.Open)
            {
                CloseConn();
                //_sqlconnection.Close();
                _sqlconnection.Dispose();
                _sqlconnection = null;
            }
        }
    }
    #endregion

    public class MySqlExecute : MySqlBase// SqlBase<MySqlConnection>
        , ISqlExecute
    {

        protected MySqlCommand sqlCmd = new MySqlCommand();
        protected MySqlDataAdapter sqlda = new MySqlDataAdapter();
        protected DataSet ds = new DataSet("Customer");

        public MySqlParameterCollection ParameterArray = null;

        public string ErrorMessage { get { return Error == null ? null : Error.Message; } set { Error = new Exception(value); } }
        public string ErrorFullMessage { get { return Error == null ? null : Error.ToString(); } }
        public Exception Error = null;
        private string _procname = "";
        public string ProcName
        {
            get
            {
                return _procname;
            }
            set
            {
                _procname = value;
            }
        }
        #region 超时设置
        /// <summary>
        /// 大数据时的超时时间,25小时,单位秒
        /// </summary>
        public const int HugeCommandTimeOut = 90000;//drds:900最大; 999999; //3600大量数据时的超时时间--benjamin 20190814
        public int CommandTimeOut
        {
            get
            {
                return sqlCmd.CommandTimeout;
            }
            set
            {
                sqlCmd.CommandTimeout = value;
            }
        }
        /// <summary>
        /// 设为更长的超时时间--benjamin 20190814
        /// </summary>
        public void SetHugeCommandTimeOut()
        {
            if (CommandTimeOut != 0)
            {
                if (HugeCommandTimeOut == 0 || CommandTimeOut < HugeCommandTimeOut)
                {
                    CommandTimeOut = HugeCommandTimeOut;
                }
            }
            //////此处设置读取的超时，不然在海量数据时很容易超时
            OpenConn();
            MySqlCommand cmd = new MySqlCommand("set net_write_timeout=" + CommandTimeOut + "; set net_read_timeout=" + CommandTimeOut, this._sqlconnection); // Setting tiimeout on mysqlServer
            cmd.ExecuteNonQuery();
        }
        #endregion
        /// <summary>
        /// 类构造函数
        /// </summary>
        /// <returns></returns>
        public MySqlExecute()
        {
        }
        /// <summary>
        /// 类构造函数，输入参数是数据库的连接字符串
        /// </summary>
        /// <returns></returns>
        public MySqlExecute(string connectionString) : base(connectionString) { }

        public bool CreateTable(SqlCreateTableCollection models,out string ms)
        {
            ms = null;
            var b = this.ExecuteSql(models.ToSql());
            if (!b)
            {
                ms = ErrorFullMessage;
            }
            return b;
        }

        /// <summary>
        /// 执行数据库查询语句，返回结果为表结构
        /// </summary>
        /// <returns>结果表</returns>
        public DataTable GetQueryTable(string sqlval, bool autoClose = true)
        {
            sqlCmd.Connection = this._sqlconnection;
            OpenConn();

            sqlCmd.CommandTimeout = 0;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlval;
            sqlda.SelectCommand = sqlCmd;
            try
            {
                ds.Tables.Clear();
                sqlda.Fill(ds);
                //将参数导出
                ParameterArray = sqlCmd.Parameters;
                if (autoClose)
                {
                    CloseConn();
                    //this._sqlconnection.Close();
                }
                return ds.Tables[0];
            }
            catch (System.Exception ex)
            {
                Error = ex;
                CloseConn();
                //this._sqlconnection.Close();
                return null;
            }
        }
        /// <summary>
        ///　查询返回单个结果
        /// </summary>
        /// <param name="sqlval">sql字符串</param>
        /// <returns>返回结果</returns>
        public object QuerySingleValue(string sqlval)
        {
            sqlCmd.Connection = this._sqlconnection;
            OpenConn();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlval;
            object robj = null;
            try
            {
                robj = sqlCmd.ExecuteScalar();
                if (robj == DBNull.Value)
                {
                    robj = null;
                }
                CloseConn();
                //this._sqlconnection.Close();
                return robj;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                CloseConn();
                //this._sqlconnection.Close();
                return robj;
            }
        }
        /// <summary>
        /// 执行查询操作，输入参数为SqlDataReader实例值，返回true表示成功,false表示失败
        /// </summary>
        /// <returns></returns>
        public MySqlDataReader GetDataReader2(string sqlstr)
        {
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlstr;
            OpenConn();
            try
            {
                return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Exception ex)
            {
                Error = ex;
                CloseConn();
                //this._sqlconnection.Close();
                return null;
            }
        }
        public DbDataReader GetDataReader3(string sqlstr)
        {
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlstr;
            OpenConn();
            try
            {
                return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Exception ex)
            {
                Error = ex;
                CloseConn();
                //this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// 执行sql语句,返回true表示执行成功,false表示执行失败
        /// </summary>
        public bool ExecuteSql(string sqlstr, bool autoClose = true)
        {
            sqlCmd.Connection = this._sqlconnection;
            OpenConn();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlstr;
            try
            {
                sqlCmd.ExecuteNonQuery();
                //将参数导出
                if (autoClose)
                {
                    CloseConn();
                    //this._sqlconnection.Close();
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                CloseConn();
                //this._sqlconnection.Close();
                return false;
            }
        }
        public int ExecuteSqlInt(string sqlstr, bool autoClose = true)
        {
            sqlCmd.Connection = this._sqlconnection;
            OpenConn();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlstr;
            try
            {
                int affected= sqlCmd.ExecuteNonQuery();
                //将参数导出
                if (autoClose)
                {
                    CloseConn();
                    //this._sqlconnection.Close();
                }
                return affected;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                CloseConn();
                //this._sqlconnection.Close();
                return -1;
            }
        }

        #region old
        ///// <summary>
        ///// tidb里删除大量里会报错Transaction is too large, size: 104857600
        ///// 用此方法解决
        ///// </summary>
        ///// <returns></returns>
        //        public bool TidbHugeDelete(string tableName,string whereSql)
        //        {
        //            int cnt = 1;
        //            while (cnt > 0)
        //            {
        //                if(!ExecuteSql(string.Format(@"
        //delete from {0} {1} limit 1000000
        //", tableName, whereSql)))
        //                {
        //                    return false;
        //                }
        //                //报错,关键字不在字典
        ////                var cntObj = QuerySingleValue(string.Format(@"
        ////select exists(select * from {0} {1} limit 0,1) as cnt
        ////", tableName, whereSql));
        //                var cntObj = QuerySingleValue(string.Format(@"
        // select 1 from {0} {1} limit 0,1
        //", tableName, whereSql));
        //                cnt =PFDataHelper.ObjectToInt(cntObj) ??0;
        //            }
        //            return true;
        //        } 
        #endregion
        /// <summary>
        /// tidb里删除大量里会报错Transaction is too large, size: 104857600
        /// 用此方法解决
        /// </summary>
        /// <returns></returns>
        public bool TidbHugeDelete(string updateSql)
        {
            SetHugeCommandTimeOut();
            int batch = 1000000;
            updateSql = string.Format(@"
{0}
limit {1}
", updateSql, batch);
            int affected = 1;
            while (affected > 0)
            {
                affected = ExecuteSqlInt(updateSql);
                if (affected == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// tidb的事务有限制,要分批更新,所以要如下使用(要求必需有has_updated字段辅助更新)
        /// 
        /// 用法1:(重点是where is_new_hy判断更新完毕)
        /// update monthly_hyzl set is_new_hy=(case when accmon=date_format(create_date, '%Y.%m') then CONVERT(bit,1) else CONVERT(bit,0) end)
        /// where is_new_hy is null
        /// 
        /// 用法2:(重点是has_updated的设置,更新前先设置为0)
        ///sqlExec.TidbHugeUpdate(string.Format(@"
        ///update monthly_hyzl a
        ///inner join monthly_hyzl as lm on xxx
        ///set xxx,
        ///    a.has_updated=1
        ///where xxx
        ///and a.has_updated=0
        ///"),
        ///string.Format(@"
        ///update monthly_hyzl set has_updated=0
        ///where xxx
        ///and has_updated =1
        ///")
        ///);
        /// </summary>
        /// <param name="updateSql"></param>
        /// <returns></returns>
        public bool TidbHugeUpdate( string updateSql,params string[] resetHasUpdatedFieldSqls)
        {
            //if (updateSql.IndexOf("has_updated") < 0) { return false; }//自行用其它字段控制也行的

            SetHugeCommandTimeOut();
            int batch = 500000;
            updateSql = string.Format(@"
{0}
limit {1}
", updateSql, batch);

            int affected = 1;


            if (resetHasUpdatedFieldSqls != null)
            {
                foreach(var i in resetHasUpdatedFieldSqls)
                {
                    var resetHasUpdatedFieldSql = string.Format(@"
{0}
limit {1}
", i, batch);

                    affected = 1;
                    while (affected > 0)
                    {
                        affected = ExecuteSqlInt(resetHasUpdatedFieldSql);
                        if (affected == -1) {
                            PFDataHelper.WriteError(Error);
                            return false;
                        }
                    }
                }
            }

            affected = 1;
            while (affected > 0)
            {
                affected = ExecuteSqlInt(updateSql);
                if (affected == -1)
                {
                    PFDataHelper.WriteError(Error);
                    return false;
                }              
            }
            return true;
        }

        #region old
        //public bool HugeInsertReader(SqlInsertCollection insert,
        //    DbDataReader rdr, string tableName,
        //    Action<BatchInsertOption> insertOptionAction = null,
        //    Action<SqlInsertCollection> rowAction = null,
        //    Action<int> sqlRowsCopiedAction = null)
        //{
        //    if (insert == null)
        //    {
        //        insert = new SqlInsertCollection
        //        {
        //            FieldQuotationCharacterL = "`",
        //            FieldQuotationCharacterR = "`"
        //        };
        //        for (int i = 0; i < rdr.FieldCount; i++)
        //        {
        //            var updateItem = new SqlUpdateItem { Key = rdr.GetName(i), VType = rdr.GetFieldType(i) };
        //            insert.Add(updateItem);
        //        }
        //    }

        //    var insertOption = new BatchInsertOption();
        //    if (insertOptionAction != null) { insertOptionAction(insertOption); }

        //    OpenConn();


        //    var sb = new StringBuilder();

        //    int idx = 0;

        //    //int batchSize = 500;// 500时,用ExecuteSql,时间10000条/48秒;
        //    int batchSize = insertOption.ProcessBatch;// 50000;// tidb设置大些试试,测试100万行/25秒
        //    int batchCnt = 0;
        //    sb.AppendFormat(@" insert into {0}({1}) values", tableName, insert.ToKeysSql());
        //    bool hasUnDo = false;
        //    SetHugeCommandTimeOut();

        //    while (rdr.Read())
        //    {
        //        //var model=new TModel();
        //        foreach (var i in insert)
        //        {
        //            i.Value.Value = rdr[i.Key];
        //            //i.Value.PInfo.SetValue(model, rdr[i.Key]);
        //        }
        //        if (rowAction != null) { rowAction(insert); }
        //        //insert.UpdateModelValue(model);
        //        sb.AppendFormat(@"{0}({1})", batchCnt == 0 ? "" : ",", insert.ToValuesSql());
        //        hasUnDo = true;
        //        if (batchCnt > batchSize)
        //        {
        //            //var b = ExecuteSql(sb.ToString(), false);
        //            var b = ExecuteSql(sb.ToString(), false);
        //            if (!b)
        //            {
        //                rdr.Close();
        //                CloseConn();
        //                return false;
        //            }
        //            if (sqlRowsCopiedAction != null)
        //            {
        //                sqlRowsCopiedAction(idx);
        //            }
        //            sb.Clear();
        //            sb.AppendFormat(@" insert into {0}({1}) values", tableName, insert.ToKeysSql());
        //            hasUnDo = false;
        //            batchCnt = 0;
        //        }
        //        else
        //        {
        //            batchCnt++;
        //        }
        //        idx++;
        //    }

        //    if (hasUnDo)
        //    {
        //        var b = ExecuteSql(sb.ToString(), false);
        //        if (!b)
        //        {
        //            rdr.Close();
        //            CloseConn();
        //            return false;
        //        }
        //    }
        //    rdr.Close();
        //    CloseConn();

        //    return true;
        //} 
        #endregion

        public bool HugeInsertReader(SqlInsertCollection insert,
            DbDataReader rdr, string tableName,
            Action<BatchInsertOption> insertOptionAction = null,
            Action<SqlInsertCollection> rowAction = null,
            Action<int> sqlRowsCopiedAction = null,
            Func< bool> stopAction = null)
        {
            if (insert == null)
            {
                //insert = new SqlInsertCollection
                //{
                //    FieldQuotationCharacterL = "`",
                //    FieldQuotationCharacterR = "`"
                //};
                insert = new MySqlInsertCollection();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    var updateItem = new SqlUpdateItem { Key = rdr.GetName(i), VType = rdr.GetFieldType(i) };
                    insert.Add(updateItem);
                }
            }

            var insertOption = new BatchInsertOption();
            if (insertOptionAction != null) { insertOptionAction(insertOption); }

            OpenConn();


            var sb = new StringBuilder();

            int idx = 0;

            int batchSize = insertOption.ProcessBatch;// 50000;// tidb设置大些试试,测试100万行/25秒
            int batchCnt = 0;

            bool hasUnDo = false;
            SetHugeCommandTimeOut();

            int oneThousandCount = 0;
            while (rdr.Read())
            {
                foreach (var i in insert)
                {
                    i.Value.Value = rdr[i.Key];
                }
                if (rowAction != null) { rowAction(insert); }

                //if (oneThousandCount > 999)//sqlserver里values最多1000行,但tidb没有这个限制(但这句留着备用)(注释这句的话,可以从19秒减少到14秒完成)
                //{
                //    oneThousandCount = 0;
                //    sb.AppendFormat(@"; insert into {0}({1}) values({2})", tableName, insert.ToKeysSql(), insert.ToValuesSql());
                //}
                //else
                if (oneThousandCount == 0)
                {
                    sb.AppendFormat(@" insert into {0}({1}) values({2})", tableName, insert.ToKeysSql(), insert.ToValuesSql());
                }
                else
                {
                    sb.AppendFormat(@",({0})", insert.ToValuesSql());
                }

                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    var b = ExecuteSql(sb.ToString(), false);
                    if (!b)
                    {
                        CloseReader(rdr);
                        CloseConn();
                        return false;
                    }
                    if (sqlRowsCopiedAction != null)
                    {
                        sqlRowsCopiedAction(idx);
                    }
                    if (stopAction != null)
                    {
                        if (stopAction())
                        {//允许中途终止--benjamin20200812
                            CloseReader(rdr);
                            CloseConn();
                        }
                    }
                    sb.Clear();

                    hasUnDo = false;
                    batchCnt = 0;
                    oneThousandCount = 0;
                }
                else
                {
                    batchCnt++;
                    oneThousandCount++;
                }
                idx++;
            }

            if (hasUnDo)
            {
                var b = ExecuteSql(sb.ToString(), false);
                if (!b)
                {
                    CloseReader(rdr);
                    CloseConn();
                    return false;
                }
            }
            CloseReader(rdr);
            CloseConn();

            return true;
        }

        public bool HugeInsertReaderIf(SqlInsertCollection insert,
            DbDataReader rdr, string tableName,
            Action<BatchInsertOption> insertOptionAction = null,
            Func<SqlInsertCollection,bool> rowAction = null,
            Action<int> sqlRowsCopiedAction = null,
            bool stopIfError=true)
        {
            if (insert == null)
            {
                //insert = new SqlInsertCollection
                //{
                //    FieldQuotationCharacterL = "`",
                //    FieldQuotationCharacterR = "`"
                //};
                insert = new MySqlInsertCollection();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    var updateItem = new SqlUpdateItem { Key = rdr.GetName(i), VType = rdr.GetFieldType(i) };
                    insert.Add(updateItem);
                }
            }

            var insertOption = new BatchInsertOption();
            if (insertOptionAction != null) { insertOptionAction(insertOption); }

            OpenConn();


            var sb = new StringBuilder();

            int idx = 0;

            int batchSize = insertOption.ProcessBatch;// 50000;// tidb设置大些试试,测试100万行/25秒
            int batchCnt = 0;

            bool hasUnDo = false;
            SetHugeCommandTimeOut();

            int oneThousandCount = 0;
            while (rdr.Read())
            {
                foreach (var i in insert)
                {
                    i.Value.Value = rdr[i.Key];
                }
                //if (rowAction != null) { rowAction(insert); }
                if (rowAction != null) {
                    if (!rowAction(insert))
                    {
                        continue;
                    }
                }

                //if (oneThousandCount > 999)//sqlserver里values最多1000行,但tidb没有这个限制(但这句留着备用)(注释这句的话,可以从19秒减少到14秒完成)
                //{
                //    oneThousandCount = 0;
                //    sb.AppendFormat(@"; insert into {0}({1}) values({2})", tableName, insert.ToKeysSql(), insert.ToValuesSql());
                //}
                //else
                if (oneThousandCount == 0)
                {
                    sb.AppendFormat(@" insert into {0}({1}) values({2})", tableName, insert.ToKeysSql(), insert.ToValuesSql());
                }
                else
                {
                    sb.AppendFormat(@",({0})", insert.ToValuesSql());
                }

                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    //var b = ExecuteSql(sb.ToString(), false);
                    var b = ExecuteSql(sb.ToString(), false);
                    if ((!b)&& stopIfError)
                    {
                        rdr.Close();
                        CloseConn();
                        return false;
                    }
                    if (sqlRowsCopiedAction != null)
                    {
                        sqlRowsCopiedAction(idx);
                    }
                    sb.Clear();

                    hasUnDo = false;
                    batchCnt = 0;
                    oneThousandCount = 0;
                }
                else
                {
                    batchCnt++;
                    oneThousandCount++;
                }
                idx++;
            }

            if (hasUnDo)
            {
                var b = ExecuteSql(sb.ToString(), false);
                if ((!b)&& stopIfError)
                {
                    rdr.Close();
                    CloseConn();
                    return false;
                }
            }
            rdr.Close();
            CloseConn();

            return true;
        }
        /// <summary>
        /// 使用方法:b = sqlExec.HugeInsertList(null, updateList, "monthly_province_statistics", null, null);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insert"></param>
        /// <param name="list"></param>
        /// <param name="tableName"></param>
        /// <param name="insertOptionAction"></param>
        /// <param name="rowAction"></param>
        /// <param name="sqlRowsCopiedAction"></param>
        /// <returns></returns>
        public bool HugeInsertList<T>(SqlInsertCollection insert,
            IList<T> list, string tableName,
            Action<BatchInsertOption> insertOptionAction = null,
            Action<SqlInsertCollection,T> rowAction = null,
            Action<int> sqlRowsCopiedAction = null)
        {
            if (list == null || list.Count < 1) { return false; }
            bool autoUpdateByModel = false;//如果用户没提供insert,那说明字段是根据list生成的,可以自动更新
            if (insert == null)
            {
                insert = new MySqlInsertCollection(list[0]);
                autoUpdateByModel = true;
            }

            var insertOption = new BatchInsertOption();
            if (insertOptionAction != null) { insertOptionAction(insertOption); }

            OpenConn();


            var sb = new StringBuilder();

            int idx = 0;

            int batchSize = insertOption.ProcessBatch;// 50000;// tidb设置大些试试,测试100万行/25秒
            int batchCnt = 0;

            bool hasUnDo = false;
            SetHugeCommandTimeOut();

            int oneThousandCount = 0;
            foreach (var i in list)
            {
                if (autoUpdateByModel)
                {
                    insert.UpdateModelValue(i);
                }
                //foreach (var i in insert)
                //{
                //    i.Value.Value = rdr[i.Key];
                //}
                if (rowAction != null) { rowAction(insert,i); }

                //if (oneThousandCount > 999)//sqlserver里values最多1000行,但tidb没有这个限制(但这句留着备用)(注释这句的话,可以从19秒减少到14秒完成)
                //{
                //    oneThousandCount = 0;
                //    sb.AppendFormat(@"; insert into {0}({1}) values({2})", tableName, insert.ToKeysSql(), insert.ToValuesSql());
                //}
                //else
                if (oneThousandCount == 0)
                {
                    sb.AppendFormat(@" insert into {0}({1}) values({2})", tableName, insert.ToKeysSql(), insert.ToValuesSql());
                }
                else
                {
                    sb.AppendFormat(@",({0})", insert.ToValuesSql());
                }

                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    //var b = ExecuteSql(sb.ToString(), false);
                    var b = ExecuteSql(sb.ToString(), false);
                    if (!b)
                    {
                        //rdr.Close();
                        CloseConn();
                        return false;
                    }
                    if (sqlRowsCopiedAction != null)
                    {
                        sqlRowsCopiedAction(idx);
                    }
                    sb.Clear();

                    hasUnDo = false;
                    batchCnt = 0;
                    oneThousandCount = 0;
                }
                else
                {
                    batchCnt++;
                    oneThousandCount++;
                }
                idx++;
            }

            if (hasUnDo)
            {
                var b = ExecuteSql(sb.ToString(), false);
                if (!b)
                {
                    //rdr.Close();
                    CloseConn();
                    return false;
                }
            }
            //rdr.Close();
            CloseConn();

            return true;
        }


        public bool BatchUpdate( MySqlUpdateCollection update, DbDataReader rdr, string tableName,
            Action<BatchInsertOption> insertOptionAction ,
            //Func<MySqlUpdateCollection, DbDataReader,bool> rowAction,
            Func<BaseSqlUpdateCollection, DbDataReader, bool> rowAction,
            Action<int> sqlRowsUpdatedAction = null)
        {

            var insertOption = new BatchInsertOption();
            if (insertOptionAction != null) { insertOptionAction(insertOption); }

            var sb = new StringBuilder();
            int idx = 0;
            int batchSize = insertOption.ProcessBatch;// 50000;// tidb设置大些试试,测试100万行/25秒
            int batchCnt = 0;
            bool hasUnDo = false;
            while (rdr.Read())
            {
                if (insertOption.AutoUpdateModel)
                {
                    update.UpdateByDataReader(rdr);
                }
                if (rowAction!=null)
                {
                    if (!rowAction(update, rdr))
                    {
                        continue;
                    }
                }

                sb.AppendFormat(@" update {0} set {1} {2};
                ", tableName, update.ToSetSql(), update.ToWhereSql());

                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    if (!ExecuteSql(sb.ToString()))
                    {
                        CloseReader(rdr);
                        return false;
                    }
                    if (sqlRowsUpdatedAction != null) { sqlRowsUpdatedAction(idx); }
                    batchCnt = 0;

                    sb.Clear();
                    hasUnDo = false;
                }else
                {
                    batchCnt++;
                }
                idx++;
            }
            //rdr.Close();
            CloseReader(rdr);
            if (hasUnDo)
            {
                if (!ExecuteSql(sb.ToString()))
                {
                    return false;
                }
            }
            CloseConn();
            return true;//benjamin 
        }
        public bool HugeUpdateList<T>(MySqlUpdateCollection update, IList<T> list, string tableName,
            Action<BatchInsertOption> insertOptionAction,
            Func<BaseSqlUpdateCollection, T, bool> rowAction,//考虑这个是否必要
            Action<int> sqlRowsUpdatedAction = null)
        {

            var insertOption = new BatchInsertOption();
            if (insertOptionAction != null) { insertOptionAction(insertOption); }
            
            //string sql = "";
            var sb = new StringBuilder();
            int idx = 0;
            int batchSize = insertOption.ProcessBatch;// 50000;// tidb设置大些试试,测试100万行/25秒
            int batchCnt = 0;
            bool hasUnDo = false;
            foreach (var i in list)
            {

                if (insertOption.AutoUpdateModel)
                {
                    update.UpdateModelValue(i);
                }
                if (rowAction != null)
                {
                    if (!rowAction(update, i))
                    {
                        continue;
                    }
                }

                sb.AppendFormat(@" update {0} set {1} {2};
                ", tableName, update.ToSetSql(), update.ToWhereSql());
                
                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    if (!ExecuteSql(sb.ToString()))
                    {
                        return false;
                    }

                    if (sqlRowsUpdatedAction != null) { sqlRowsUpdatedAction(idx); }
                    batchCnt = 0;

                    sb.Clear();
                    hasUnDo = false;
                }
                else
                {
                    batchCnt++;
                }
                idx++;
            }
            //rdr.Close();
            CloseConn();
            if (hasUnDo)
            {

                if (!ExecuteSql(sb.ToString()))
                {
                    return false;
                }
            }

            return true;//benjamin 
        }

        /// <summary>
        /// 批量导入(考虑版本号)(陈超)(tidb使用报错:不支持此命令)
        /// </summary>
        /// <param name="connStr">MySql连接字符串</param>
        /// <param name="tableNameT">目标表名</param>
        /// <param name="csvFileName">csc文件绝对路径</param>
        /// <param name="columnNames">列名列表，可以使用@DataTable.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList()</param>
        /// <returns></returns>
        public int BulkLoad(string tableNameT, string csvFileName, List<string> columnNames)
        {
            //using (MySqlConnection conn = new MySqlConnection(connStr))
            //{
            //    conn.Open();
                OpenConn();
                var conn=this._sqlconnection;
                MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                {
                    FieldTerminator = ",",
                    FieldQuotationCharacter = '"',
                    EscapeCharacter = '"',
                    LineTerminator = "\r\n",
                    FileName = csvFileName,
                    NumberOfLinesToSkip = 0,
                    TableName = tableNameT,
                    Local=true
                };
                bulk.Columns.AddRange(columnNames);
                int count = bulk.Load();
                CloseConn();
                return count;
            //}
        }
        /// <summary>
        /// 更新同比环比字段(暂规定统一以create_date为时间字段,同比前缀ly_,环比前缀lm_)
        /// </summary>
        /// <param name="transfer"></param>
        /// <param name="valuefields"></param>
        public void UpdateYearOnYearField(string tableName,string cmonth,string[] primaryKeys, string[] valuefields)
        {
            //var cmonth = transfer.ViewData["cmonth"].ToString();
            var last_cmonth = PFDataHelper.CMonthAddMonths(cmonth, -1);
            var last_year = PFDataHelper.CMonthAddYears(cmonth, -1);

            //var sqlExec = new MySqlExecute(transfer.DstConn);
            var thisMonthDt = GetQueryTable(string.Format(@"
select * from {0} where create_date= STR_TO_DATE('{1}.01','%Y.%m.%d')
", tableName, cmonth));
            var thisMonthList = thisMonthDt == null ? new List<Dictionary<string, object>>() : thisMonthDt.DataTableToDictList(false);

            var lastMonthDt = GetQueryTable(string.Format(@"
select * from {0} where create_date= STR_TO_DATE('{1}.01','%Y.%m.%d')
", tableName, last_cmonth));
            var lastMonthList = lastMonthDt == null ? new List<Dictionary<string, object>>() : lastMonthDt.DataTableToDictList(false);

            var lastYearDt = GetQueryTable(string.Format(@"
select * from {0} where create_date= STR_TO_DATE('{1}.01','%Y.%m.%d')
", tableName, last_year));
            var lastYearList = lastYearDt == null ? new List<Dictionary<string, object>>() : lastYearDt.DataTableToDictList(false);

            var updateList = new List<Dictionary<string, object>>();
            var insertList = new List<Dictionary<string, object>>();
            var valueFieldZero= new Dictionary<string, object>();//值列最好是不要有null值,否则BI里相减会得null,很不方便,其实所有decimal列都要

            Action<List<Dictionary<string, object>>> findZero = list =>
            {
                if (list.Any())
                {
                    foreach (var i in list[0])
                    {
                        if ((!primaryKeys.Contains(i.Key))&&(!valueFieldZero.ContainsKey(i.Key)) && i.Value != null)
                        {
                            var typeString = PFDataHelper.GetStringByType(i.Value.GetType());
                            if (typeString == "decimal")
                            {
                                valueFieldZero.Add(i.Key, new decimal(0));
                            }else if (new string[] { "int","long" }.Contains(typeString))
                            {
                                valueFieldZero.Add(i.Key, 0);
                            }
                        }
                    }
                }
            };
            findZero(thisMonthList);
            findZero(lastMonthList);
            findZero(lastYearList);

            Func< Dictionary < string, object> ,string, Dictionary<string, object>> newRow = (srcRow,cmonth1) =>
            {
                var r =new Dictionary<string, object>
                    {
                        {"create_date",PFDataHelper.CMonthToMySqlDate(cmonth) }
                    };
                foreach (var k in primaryKeys)
                {
                    if (k != "create_date")
                    {
                        r.Add(k, srcRow[k]);
                    }
                }
                foreach (var k in valueFieldZero)
                {
                    r.Add(k.Key, k.Value);
                    //if (valuefields.Contains(k.Key))
                    //{
                    //    r.Add("lm_" + k.Key, k.Value);
                    //    r.Add("ly_" + k.Key, k.Value);
                    //}
                }
                return r;
            };

            foreach (var j in lastMonthList)
            {
                Dictionary<string, object> item = null;
                if (thisMonthList.Any())
                {
                    foreach (var i in thisMonthList)
                    {
                        var isMatch = true;
                        foreach (var k in primaryKeys)
                        {
                            if (k != "create_date")
                            {

                                if (!PFDataHelper.IsObjectEquals(i[k], j[k]))
                                {
                                    isMatch = false;
                                    break;
                                }
                                //if (!PFDataHelper.IsObjectEquals(i[k],j[k]))
                                //{
                                //    isMatch = false;
                                //    break;
                                //}
                            }
                        }
                        if (isMatch)
                        {
                            item = i;
                            break;
                        }
                    }
                }
                //var item = thisMonthList.Any()? thisMonthList.FirstOrDefault(a => a["province_name"] == j["province_name"] && a["hpos"] == j["hpos"] && a["qpos"] == j["qpos"] && a["is_new_hy"] == j["is_new_hy"])
                //    :null;
                //bool isAdd = false;
                if (item == null)
                {
                    //isAdd = true;
                    item = newRow(j,cmonth);
                    #region old
                    //item = new Dictionary<string, object>
                    //{
                    //    {"create_date",PFDataHelper.CMonthToMySqlDate(cmonth) }
                    //};
                    //foreach (var k in primaryKeys)
                    //{
                    //    if (k != "create_date")
                    //    {
                    //        item.Add(k, j[k]);
                    //    }
                    //}
                    //foreach (var k in valueFieldZero)
                    //{
                    //    item.Add(k.Key, k.Value);
                    //    if (valuefields.Contains(k.Key))
                    //    {
                    //        item.Add("lm_" + k.Key, k.Value);
                    //        item.Add("ly_" + k.Key, k.Value);
                    //    }
                    //} 
                    #endregion
                    insertList.Add(item);
                }
                else
                {
                    updateList.Add(item);
                }
                foreach (var k in valuefields)
                {
                    item["lm_" + k] = j[k];
                }

            }
            foreach (var j in lastYearList)
            {
                Dictionary<string, object> item = null;
                if (thisMonthList.Any())
                {
                    foreach (var i in thisMonthList)
                    {
                        var isMatch = true;
                        foreach (var k in primaryKeys)
                        {
                            if (k != "create_date")
                            {
                                if (!PFDataHelper.IsObjectEquals(i[k], j[k]))
                                {
                                    isMatch = false;
                                    break;
                                }
                            }
                        }
                        if (isMatch)
                        {
                            item = i;
                            break;
                        }
                    }
                }
                //var item = thisMonthList.FirstOrDefault(a => a.province_name == j.province_name && a.hpos == j.hpos && a.qpos == j.qpos && a.is_new_hy == j.is_new_hy);
                //bool isAdd = false;
                if (item == null)
                {
                    //isAdd = true;
                    if (insertList.Any())
                    {
                        foreach (var i in insertList)
                        {
                            var isMatch = true;
                            foreach (var k in primaryKeys)
                            {
                                if (k != "create_date")
                                {
                                    if (!PFDataHelper.IsObjectEquals(i[k], j[k]))
                                    {
                                        isMatch = false;
                                        break;
                                    }
                                }
                            }
                            if (isMatch)
                            {
                                item = i;
                                break;
                            }
                        }
                    }
                    //item = insertList.FirstOrDefault(a => a.province_name == j.province_name && a.hpos == j.hpos && a.qpos == j.qpos && a.is_new_hy == j.is_new_hy);
                    if (item == null)
                    {
                        item = newRow(j,cmonth);
                        #region old
                        //item = new Dictionary<string, object>
                        //{
                        //    {"create_date",PFDataHelper.CMonthToMySqlDate(cmonth) }//,
                        //};
                        //foreach (var k in primaryKeys)
                        //{
                        //    if (k != "create_date")
                        //    {
                        //        item.Add(k, j[k]);
                        //    }
                        //}
                        //foreach (var k in valueFieldZero)
                        //{
                        //    item.Add(k.Key, k.Value);
                        //    if (valuefields.Contains(k.Key))
                        //    {
                        //        item.Add("lm_" + k.Key, k.Value);
                        //        item.Add("ly_" + k.Key, k.Value);
                        //    }
                        //} 
                        #endregion
                        insertList.Add(item);
                    }
                }
                else
                {
                    Dictionary<string, object> updateItem = null;
                    if (updateList.Any())
                    {
                        foreach (var i in updateList)
                        {
                            var isMatch = true;
                            foreach (var k in primaryKeys)
                            {
                                if (k != "create_date")
                                {
                                    if (!PFDataHelper.IsObjectEquals(i[k], j[k]))
                                    {
                                        isMatch = false;
                                        break;
                                    }
                                }
                            }
                            if (isMatch)
                            {
                                updateItem = i;
                                break;
                            }
                        }
                    }

                    //var updateItem = updateList.FirstOrDefault(a => a.province_name == j.province_name && a.hpos == j.hpos && a.qpos == j.qpos && a.is_new_hy == j.is_new_hy);
                    if (updateItem == null)
                    {
                        updateList.Add(item);
                    }
                    else
                    {
                        item = updateItem;
                    }
                }
                foreach (var k in valuefields)
                {
                    item["ly_" + k] = j[k];
                }
            }

            var b = true;
            //当单独执行AfterTransferAction时,还是先把同比环比清空比较稳,否则会有上次残留结果
            //var valuefields = new string[] { "new_hy_qty", "valid_hy_qty", "valid_hy_hpos_above_5_qty", "valid_hy_hpos_0_qty" };
            b = ExecuteSql(string.Format(@"
update {0} set {1},{2} where create_date= STR_TO_DATE('{3}.01','%Y.%m.%d')
",
tableName,
string.Join(",", valuefields.Select(a => string.Format(" ly_{0}=0 ", a))),
string.Join(",", valuefields.Select(a => string.Format(" lm_{0}=0 ", a))),
cmonth));
            if (updateList.Any())
            {
                var update = new MySqlUpdateCollection(updateList.First());
                //update.UpdateFields("lm_new_hy_qty", "lm_valid_hy_qty", "lm_valid_hy_hpos_above_5_qty", "lm_valid_hy_hpos_0_qty");
                var updateFields = new List<string>();
                update.UpdateFields(PFDataHelper.MergeList(
                    valuefields.Select(a => "lm_" + a).ToList(),
                    valuefields.Select(a => "ly_" + a).ToList()
                    ).ToArray());
                update.PrimaryKeyFields(false, primaryKeys);
                b = HugeUpdateList(update, updateList, tableName, null, null);
            }
            if (insertList.Any())
            {
                //var insert = new MySqlInsertCollection(insertList.First());
                b = HugeInsertList(null, insertList,tableName, null, null);
            }
        }
        public bool CloseReader(DbDataReader reader)
        {
            sqlCmd.Cancel();//如果没有这句,数据很多时 dr.Close 会很慢 https://www.cnblogs.com/xyz0835/p/3379676.html
            reader.Close();
            return true;
        }

        public class BatchInsertOption
        {
            private int _processBatch = 500;
            public int ProcessBatch { get { return _processBatch; } set { _processBatch = value; } }
            private bool _autoUpdateModel = true;
            public bool AutoUpdateModel { get { return _autoUpdateModel; } set { _autoUpdateModel = value; } }

            /// <summary>
            /// 用于下面这种更新的行数
            /// update xx from xx where xx;
            /// update xx from xx where xx;//...n行
            /// 可以认为是tidb里的 max_allowed_packet=67108864 变量
            /// </summary>
            public void SetTidbBatchUpdateProcessBatch()
            {
                //// Packets larger than max_allowed_packet are not allowed
                //_processBatch =  500000;
                _processBatch = 100000;
            }
        }
    }
}
