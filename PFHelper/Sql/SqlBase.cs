//using System;
//using System.Configuration;
//using System.Data.SqlClient;
//using System.Data;
//using System.Data.Common;

//namespace Perfect
//{
//    #region 泛型,不安全
//    //public class SqlBase<TSqlConnection>
//    //    where TSqlConnection: DbConnection,new()
//    //{
//    //    //protected string _connectionstring = "data source=10.0.0.11;initial catalog=yjquery;persist security info=False;user id=sa;password=perfect;Connect Timeout=2000";// ConfigurationManager.ConnectionStrings["yjquery"].ConnectionString;
//    //    //protected string _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
//    //    protected string _connectionstring = null;
//    //    protected TSqlConnection _sqlconnection = new TSqlConnection();
//    //    public SqlBase()
//    //    {
//    //        //_sqlconnection.ConnectionString = _connectionstring;
//    //        _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
//    //        _sqlconnection.ConnectionString = _connectionstring;
//    //        if (_sqlconnection.State != ConnectionState.Open)
//    //            _sqlconnection.Open();
//    //    }
//    //    /// <summary>
//    //    /// 
//    //    /// </summary>
//    //    /// <param name="_connString"></param>
//    //    /// <param name="tryTimes">重试次数(因为联通的数据库不稳定)</param>
//    //    public SqlBase(string _connString, int tryTimes = 3)
//    //    {
//    //        while (tryTimes > 0)
//    //        {
//    //            try
//    //            {
//    //                _connectionstring = _connString;
//    //                _sqlconnection.ConnectionString = _connectionstring;
//    //                if (_sqlconnection.State != ConnectionState.Open)
//    //                {
//    //                    _sqlconnection.Open();
//    //                }
//    //                break;
//    //            }
//    //            catch (Exception e)
//    //            {

//    //            }
//    //            tryTimes--;
//    //        }
//    //    }

//    //    public void Connection(string _connString)
//    //    {
//    //        _connectionstring = _connString;

//    //        if (_sqlconnection.State == ConnectionState.Open)
//    //        {
//    //            _sqlconnection.Close();
//    //        }
//    //        _sqlconnection.ConnectionString = _connectionstring;
//    //        _sqlconnection.Open();
//    //    }

//    //    protected void OpenConn()
//    //    {
//    //        if (this._sqlconnection.State == ConnectionState.Closed)
//    //        { this._sqlconnection.Open(); }
//    //    }
//    //    protected void CloseConn()
//    //    {
//    //        this._sqlconnection.Close();
//    //    }
//    //} 
//    #endregion
//    #region old
//    /// <summary>
//    /// DataBase 的摘要说明。
//    /// </summary>
//    public class DataBase
//    {
//        //protected string _connectionstring = "data source=10.0.0.11;initial catalog=yjquery;persist security info=False;user id=sa;password=perfect;Connect Timeout=2000";// ConfigurationManager.ConnectionStrings["yjquery"].ConnectionString;
//        //protected string _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
//        protected string _connectionstring = null;
//        protected SqlConnection _sqlconnection = new SqlConnection();
//        public DataBase()
//        {
//            //_sqlconnection.ConnectionString = _connectionstring;
//            _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
//            _sqlconnection.ConnectionString = _connectionstring;
//            if (_sqlconnection.State != ConnectionState.Open)
//                _sqlconnection.Open();
//        }
//        public DataBase(string _connString)
//        {
//            _connectionstring = _connString;
//            _sqlconnection.ConnectionString = _connectionstring;
//            if (_sqlconnection.State != ConnectionState.Open)
//                _sqlconnection.Open();
//        }

//        public void Connection(string _connString)
//        {
//            _connectionstring = _connString;

//            if (_sqlconnection.State == ConnectionState.Open)
//            {
//                _sqlconnection.Close();
//            }
//            _sqlconnection.ConnectionString = _connectionstring;
//            _sqlconnection.Open();
//        }

//    }
//    #endregion
//}
