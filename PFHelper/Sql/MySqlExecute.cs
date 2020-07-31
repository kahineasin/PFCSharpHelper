using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Collections;

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
            Action<int> sqlRowsCopiedAction = null)
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
                    //var b = ExecuteSql(sb.ToString(), false);
                    var b = ExecuteSql(sb.ToString(), false);
                    if (!b)
                    {
                        CloseReader(rdr);
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
                    rdr.Close();
                    CloseConn();
                    return false;
                }
            }
            rdr.Close();
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

            //int batch = 1000;
            //int cur = 0;
            int already = 0;
            //string sql = "";
            var sb = new StringBuilder();
            int idx = 0;
            int batchSize = insertOption.ProcessBatch;// 50000;// tidb设置大些试试,测试100万行/25秒
            int batchCnt = 0;
            bool hasUnDo = false;
            while (rdr.Read())
            {
                //////var item = rdrAction(rdr);
                //////if (item == null) { continue; }
                //////update.UpdateModelValue(item);

                ////if (rowAction != null) { rowAction(update,rdr); }
                //foreach (var i in update)
                //{
                //    i.Value.Value = rdr[i.Key];
                //}
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
                //sql += string.Format(@" update {0} set {1} {2};
                //", tableName, update.ToSetSql(), update.ToWhereSql());

                //cur++;
                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    //continue;//benjamin 
                    //if (!ExecuteTransactSql(sql))
                    if (!ExecuteSql(sb.ToString()))
                    {
                        return false;
                    }
                    //already += batch;
                    if (sqlRowsUpdatedAction != null) { sqlRowsUpdatedAction(idx); }
                    batchCnt = 0;
                    //batchList.Clear();
                    //sql = "";
                    sb.Clear();
                    hasUnDo = false;
                    //batchList = new List<DayOrdersUpdate>();
                }else
                {
                    batchCnt++;
                }
                idx++;
            }
            rdr.Close();
            CloseConn();
            if (hasUnDo)
            {
                //return false;//benjamin 
                //if (!ExecuteTransactSql(sql))
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
        }
    }
}
