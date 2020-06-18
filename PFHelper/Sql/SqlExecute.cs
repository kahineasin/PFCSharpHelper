using Perfect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfect
{ /// <summary>
  /// DataBase ��ժҪ˵����
  /// </summary>
    public class DataBase : IDisposable
    {
        //protected string _connectionstring = "data source=10.0.0.11;initial catalog=yjquery;persist security info=False;user id=sa;password=perfect;Connect Timeout=2000";// ConfigurationManager.ConnectionStrings["yjquery"].ConnectionString;
        //protected string _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        protected string _connectionstring = null;
        protected SqlConnection _sqlconnection = new SqlConnection();
        public DataBase()
        {
            //_sqlconnection.ConnectionString = _connectionstring;
            _connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            _sqlconnection.ConnectionString = _connectionstring;
            if (_sqlconnection.State != ConnectionState.Open)
                _sqlconnection.Open();
        }
        public DataBase(string _connString)
        {
            _connectionstring = _connString;
            _sqlconnection.ConnectionString = _connectionstring;
            if (_sqlconnection.State != ConnectionState.Open)
                _sqlconnection.Open();
        }

        public void Connection(string _connString)
        {
            _connectionstring = _connString;

            if (_sqlconnection.State == ConnectionState.Open)
            {
                _sqlconnection.Close();
            }
            _sqlconnection.ConnectionString = _connectionstring;
            _sqlconnection.Open();
        }
        public void OpenConn()
        {
            if (this._sqlconnection.State == ConnectionState.Closed)
            { this._sqlconnection.Open(); }
        }
        public void CloseConn()
        {
            this._sqlconnection.Close();
        }

        public void Dispose()
        {
            if (_sqlconnection.State == ConnectionState.Open)
            {
                _sqlconnection.Close();
                _sqlconnection.Dispose();
            }
        }
    }
    /// <summary>
    /// SqlExecute ��ժҪ˵����
    /// �洢�����÷�:
    /// 
    ///            SqlExec.ClearParameter();
    ///            SqlExec.ProcName = "p_tax_import";
    ///            SqlExec.AddInParameter("@batchno", SqlDbType.VarChar, 12, batchno);
    ///            SqlExec.AddInParameter("@operator", SqlDbType.VarChar, 30, operuser);
    ///            SqlExec.AddInParameter("@tb", SqlDbType.Structured, "tp_tax", dt);
    ///            SqlExec.AddOutParameter("@flag", SqlDbType.Bit);
    ///            SqlExec.AddOutParameter("@mess", SqlDbType.VarChar, 100);
    ///            if (SqlExec.ExecProcedure())
    ///            {
    ///                message = SqlExec.ParameterArray["@mess"].Value.ToString();
    ///                return (bool)SqlExec.ParameterArray["@flag"].Value;
    ///            }
    ///            else
    ///            {
    ///                return false;
    ///            }
    /// </summary>

    public class ProcManager : DataBase, ISqlExecute// SqlBase<SqlConnection>
    {
        //public ProcManager(string month, bool flag)
        //{
        //    string connectionString = SqlHelper.GetConnectionString(this._connectionstring, "YJQuery" + month);
        //    Connection(connectionString);
        //}

        protected SqlCommand sqlCmd = new SqlCommand();
        protected SqlDataAdapter sqlda = new SqlDataAdapter();
        protected DataSet ds = new DataSet("Customer");

        public SqlParameterCollection ParameterArray = null;
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

        #region ��ʱ����
        public const int HugeCommandTimeOut = 90000; //3600��������ʱ�ĳ�ʱʱ��--benjamin 20190814
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
        /// ��Ϊ�����ĳ�ʱʱ��--benjamin 20190814
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
        }
        #endregion
        /// <summary>
        /// �ú���������Ӳ���,��ֵ�����Ϊ��������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pSizeΪ��С,pValueΪ����ֵ
        /// </summary>
        public void AddParameter(string ParaName, SqlDbType pSqlType, object pValue, ParameterDirection pInOrOut)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType);
            if (pInOrOut == ParameterDirection.Input)
            {
                para.Direction = pInOrOut;
                para.Value = pValue;
            }
            else
            {
                para.Direction = pInOrOut;
            }
            sqlCmd.Parameters.Add(para);
        }
        /// <summary>
        /// �ú�����������ַ�������,��ֵ����Ϊ����:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pSizeΪ��С,pValueΪ����ֵ
        /// </summary>
        public void AddParameter(string ParaName, SqlDbType pSqlType, int pSize, string pValue)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType, pSize);
            para.Direction = ParameterDirection.Input;
            para.Value = pValue;
            sqlCmd.Parameters.Add(para);
        }
        /// <summary>
        /// �ú�����������������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pSizeΪ��С,pValueΪ����ֵ
        /// </summary>
        public void AddOutParameter(string ParaName, SqlDbType pSqlType, int pSize, byte pScale)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType, pSize);
            para.Direction = ParameterDirection.Output;
            para.Scale = pScale;
            sqlCmd.Parameters.Add(para);
        }

        /// <summary>
        /// �ú�����������������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pSizeΪ��С
        /// </summary>
        public void AddOutParameter(string ParaName, SqlDbType pSqlType, int pSize)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType, pSize);
            para.Direction = ParameterDirection.Output;
            sqlCmd.Parameters.Add(para);
        }

        /// <summary>
        /// �ú�����������������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������
        /// </summary>
        public void AddOutParameter(string ParaName, SqlDbType pSqlType)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType);
            para.Direction = ParameterDirection.Output;
            sqlCmd.Parameters.Add(para);
        }

        /// <summary>
        /// �ú�����������ַ�������,��ֵ����Ϊ����:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pSizeΪ��С,pValueΪ����ֵ,pScaleΪС��λ��
        /// </summary>
        public void AddParameter(string ParaName, SqlDbType pSqlType, int pSize, byte pScale, string pValue)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType, pSize);
            para.Direction = ParameterDirection.Input;
            para.Scale = pScale;
            para.Value = pValue;
            sqlCmd.Parameters.Add(para);
        }

        /// <summary>
        /// �ú�����������������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pSizeΪֵ��С,pValueΪ����ֵ,pScaleΪС��λ��
        /// </summary>
        public void AddInParameter(string ParaName, SqlDbType pSqlType, int pSize, byte pScale, object pValue)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType, pSize);
            para.Direction = ParameterDirection.Input;
            para.Scale = pScale;
            para.Value = pValue;
            sqlCmd.Parameters.Add(para);
        }
        public void AddInParameter(string ParaName, SqlDbType pSqlType, string typeName, object pValue)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType);
            para.Direction = ParameterDirection.Input;
            para.Value = pValue;
            para.TypeName = typeName;
            sqlCmd.Parameters.Add(para);
        }

        /// <summary>
        /// �ú�����������������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pSizeΪֵ��С,pValueΪ����ֵ
        /// </summary>
        public void AddInParameter(string ParaName, SqlDbType pSqlType, int pSize, object pValue)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType, pSize);
            para.Direction = ParameterDirection.Input;
            para.Value = pValue;
            sqlCmd.Parameters.Add(para);
        }

        /// <summary>
        /// �ú�����������������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������,pValueΪ����ֵ
        /// </summary>
        public void AddInParameter(string ParaName, SqlDbType pSqlType, object pValue)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType);
            para.Direction = ParameterDirection.Input;
            para.Value = pValue;
            sqlCmd.Parameters.Add(para);
        }
        /// <summary>
        /// �ú�����������ַ�������������Ϊ������������:ParaNameΪ�洢���̲�����,pSqlTypeΪ��������
        /// </summary>
        public void AddParameter(string ParaName, SqlDbType pSqlType, int pSize, string pValue, ParameterDirection pInOrOut)
        {
            SqlParameter para = new SqlParameter(ParaName, pSqlType, pSize);
            if (pInOrOut == ParameterDirection.Input)
            {
                para.Direction = pInOrOut;
                para.Value = pValue;
            }
            else
            {
                para.Direction = pInOrOut;
            }

            sqlCmd.Parameters.Add(para);
        }

        /// <summary>
        ///     ���ģ������StartWith����--wxj20171211
        /// </summary>
        /// <param name="pSize">�������ȣ����ݿ⣩</param>
        /// <param name="pValue">����ֵ</param>
        public void AddStartWithParameter(string pName, int pSize, string pValue)
        {
            pValue = pValue ?? "";
            this.AddParameter("sales", System.Data.SqlDbType.VarChar, pSize + 1, pValue.Substring(0, pValue.Length > pSize ? pSize : pValue.Length) + '%');//������ֶ���ȡ���ᵼ��%�Ŷ�ʧ--wxj20171211
        }

        /// <summary>
        /// ִ�д洢����,����true��ʾִ�гɹ�,false��ʾִ��ʧ��
        /// </summary>
        public bool ExecProcedure()
        {
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = _procname;
            try
            {
                sqlCmd.ExecuteNonQuery();
                //����������
                ParameterArray = sqlCmd.Parameters;
                this._sqlconnection.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return false;
            }
        }

        /// <summary>
        /// ִ�д洢����,����true��ʾִ�гɹ�,false��ʾִ��ʧ��
        /// </summary>
        public bool ExecProcedure(int timeout)
        {
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandTimeout = timeout;
            sqlCmd.CommandText = _procname;
            try
            {
                sqlCmd.ExecuteNonQuery();
                //����������
                ParameterArray = sqlCmd.Parameters;
                this._sqlconnection.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return false;
            }
        }

        /// <summary>
        /// ִ��sql���,����true��ʾִ�гɹ�,false��ʾִ��ʧ��
        /// </summary>
        public bool ExecuteSql(string sqlstr, bool autoClose = true)
        {
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlstr;
            try
            {
                sqlCmd.ExecuteNonQuery();
                //����������
                if (autoClose)
                {
                    this._sqlconnection.Close();
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return false;
            }
        }

        /// <summary>
        /// ִ������Sql ���
        /// </summary>
        /// <param name="sqlStr">sql���</param>
        /// <returns></returns>
        public bool ExecuteTransactSql(string sqlStr, bool autoClose = true)
        {
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            bool flag = false;
            SqlTransaction sqltran = this._sqlconnection.BeginTransaction();
            SqlCommand sqlcmd = this._sqlconnection.CreateCommand();
            SqlParameter[] paras = new SqlParameter[this.sqlCmd.Parameters.Count];
            this.sqlCmd.Parameters.CopyTo(paras, 0);
            this.sqlCmd.Parameters.Clear();
            sqlcmd.CommandTimeout = CommandTimeOut;//benjamin 20190814
            sqlcmd.Parameters.AddRange(paras);
            try
            {
                sqlcmd.Transaction = sqltran;
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sqlStr;
                sqlcmd.ExecuteNonQuery();
                sqltran.Commit();
                flag = true;
            }
            catch (System.Exception e)
            {
                try
                {
                    sqltran.Rollback();
                    Error = e;
                }
                catch (System.Exception ex)
                {
                    Error = ex;
                }
            }
            finally
            {
                if (autoClose)
                {
                    this._sqlconnection.Close();
                }
            }
            return flag;
        }

        /// <summary>
        /// ִ�в�ѯ���������ؽ��Ϊ��ṹ
        /// </summary>
        /// <returns></returns>
        public DataTable GetQueryTable()
        {
            EnsureProcNotNull();
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = _procname;
            sqlCmd.CommandTimeout = 9600;
            sqlda.SelectCommand = sqlCmd;
            try
            {
                ds.Tables.Clear();
                sqlda.Fill(ds);
                //����������
                ParameterArray = sqlCmd.Parameters;
                this._sqlconnection.Close();
                return ds.Tables[0];
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// ִ�в�ѯ���������ؽ��Ϊ��ṹ
        /// </summary>
        /// <returns></returns>
        public DataTable GetQueryTable1()
        {
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandTimeout = 300;
            sqlCmd.CommandText = _procname;
            sqlda.SelectCommand = sqlCmd;
            try
            {
                ds.Tables.Clear();
                sqlda.Fill(ds);
                //����������
                ParameterArray = sqlCmd.Parameters;
                this._sqlconnection.Close();
                return ds.Tables[0];
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// ִ�в�ѯ�������������ΪSqlDataReaderʵ��ֵ������true��ʾ�ɹ�,false��ʾʧ��
        /// </summary>
        /// <returns></returns>
        public bool GetDataReader(ref SqlDataReader dr)
        {
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = _procname;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                dr = sqlCmd.ExecuteReader(CommandBehavior.SingleResult);
                //����������
                ParameterArray = sqlCmd.Parameters;
                return true;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return false;
            }
        }

        /// <summary>
        /// ִ�в�ѯ�������������ΪSqlDataReaderʵ��ֵ������true��ʾ�ɹ�,false��ʾʧ��
        /// </summary>
        /// <returns></returns>
        public SqlDataReader GetDataReader2(string sqlstr)
        {
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlstr;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }
        public DbDataReader GetDataReader3(string sqlstr)
        {
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlstr;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// ִ�в�ѯ����
        /// </summary>
        /// <returns>��ѯ���</returns>
        public NameValueCollection GetDataReader()
        {
            SqlDataReader dr = null;
            NameValueCollection myvalues = new NameValueCollection();
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = _procname;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                dr = sqlCmd.ExecuteReader(CommandBehavior.SingleResult);
                //����������
                ParameterArray = sqlCmd.Parameters;
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        myvalues.Add(dr.GetName(i), dr[i].ToString());
                    }
                }
                dr.Close();
                this._sqlconnection.Close();
                return myvalues;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        public object[] QueryValueArray(string sqlval)
        {
            SqlDataReader dr = null;
            object[] myvalues = null;
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlval;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                dr = sqlCmd.ExecuteReader(CommandBehavior.SingleResult);
                //����������
                ParameterArray = sqlCmd.Parameters;
                if (dr.Read())
                {
                    dr.GetValues(myvalues);
                }
                dr.Close();
                this._sqlconnection.Close();
                return myvalues;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                myvalues = null;
                return myvalues;
            }
        }

        public object[] QueryValueArray(string sqlval, int flag)
        {
            SqlDataReader dr = null;
            object[] myvalues = null;
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlval;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                dr = sqlCmd.ExecuteReader(CommandBehavior.SingleResult);
                //����������
                ParameterArray = sqlCmd.Parameters;
                if (dr.Read())
                {
                    myvalues = new object[dr.FieldCount];
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (dr[i] == System.DBNull.Value)
                        {
                            if (dr.GetFieldType(i) == typeof(string))
                            {
                                myvalues[i] = "";
                            }
                            else if (dr.GetFieldType(i) == typeof(int))
                            {
                                myvalues[i] = 0;
                            }
                            else if (dr.GetFieldType(i) == typeof(decimal))
                            {
                                myvalues[i] = 0.00m;
                            }
                            else if (dr.GetFieldType(i) == typeof(double))
                            {
                                myvalues[i] = 0;
                            }
                            else if (dr.GetFieldType(i) == typeof(DateTime))
                            {
                                myvalues[i] = "";
                            }
                            else
                            {
                                myvalues[i] = "";
                            }
                        }
                        else
                        {
                            myvalues[i] = dr[i];
                        }
                    }
                }
                dr.Close();
                this._sqlconnection.Close();
                return myvalues;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                myvalues = null;
                return myvalues;
            }
        }


        /// <summary>
        /// ִ����ֵ��ѯ����
        /// </summary>
        /// <returns>��ѯ���</returns>
        public NameValueCollection GetNameValueArray()
        {
            SqlDataReader dr = null;
            NameValueCollection myvalues = new NameValueCollection();
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = _procname;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                dr = sqlCmd.ExecuteReader(CommandBehavior.SingleResult);
                //����������
                ParameterArray = sqlCmd.Parameters;
                while (dr.Read())
                {
                    myvalues.Add(dr[0].ToString(), dr[1].ToString());
                }
                dr.Close();
                this._sqlconnection.Close();
                return myvalues;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// ִ����ֵ��ѯ����
        /// </summary>
        /// <returns>��ѯ���</returns>
        public NameValueCollection GetNameValueArray(string sqlval)
        {
            SqlDataReader dr = null;
            NameValueCollection myvalues = new NameValueCollection();
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlval;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                dr = sqlCmd.ExecuteReader(CommandBehavior.SingleResult);
                //����������
                ParameterArray = sqlCmd.Parameters;
                while (dr.Read())
                {
                    myvalues.Add(dr[0].ToString(), dr[1].ToString());
                }
                dr.Close();
                this._sqlconnection.Close();
                return myvalues;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// ִ�в�ѯ����
        /// </summary>
        /// <returns>��ѯ���</returns>
        public NameValueCollection GetDataReader(string sqlval)
        {

            SqlDataReader dr = null;
            NameValueCollection myvalues = new NameValueCollection();
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sqlval;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            try
            {
                dr = sqlCmd.ExecuteReader(CommandBehavior.SingleResult);
                //����������
                ParameterArray = sqlCmd.Parameters;
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        myvalues.Add(dr.GetName(i), dr[i].ToString());
                    }
                }
                dr.Close();
                this._sqlconnection.Close();
                return myvalues;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// ִ�в�ѯ���������ؽ��Ϊ��ṹ
        /// </summary>
        /// <returns>���ز�ѯ���ݼ�</returns>
        public DataSet GetQueryDataSet()
        {
            sqlCmd.Connection = this._sqlconnection;
            sqlCmd.CommandTimeout = 9600;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = _procname;
            sqlda.SelectCommand = sqlCmd;
            try
            {
                ds.Tables.Clear();
                sqlda.Fill(ds);
                //����������
                ParameterArray = sqlCmd.Parameters;
                this._sqlconnection.Close();
                return ds;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        /// ִ�в�ѯ���������ؽ��Ϊ��ṹ
        /// </summary>
        /// <returns>���ز�ѯ���ݼ�</returns>
        public DataSet GetQueryDataSet(string sql)
        {
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sql;
            sqlda.SelectCommand = sqlCmd;
            try
            {
                ds.Tables.Clear();
                sqlda.Fill(ds);
                //����������
                ParameterArray = sqlCmd.Parameters;
                this._sqlconnection.Close();
                return ds;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }
        /// <summary>
        /// ִ�в�ѯ���������ؽ��Ϊ��ṹ(ͬʱִ������������)
        /// </summary>
        /// <returns>���ز�ѯ���ݼ�</returns>
        public DataSet GetQueryDataSetTransact(string sql)
        {
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();
            SqlTransaction sqltran = this._sqlconnection.BeginTransaction();
            sqlCmd.Transaction = sqltran;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sql;
            sqlda.SelectCommand = sqlCmd;
            try
            {
                ds.Tables.Clear();
                sqlda.Fill(ds);
                //����������
                ParameterArray = sqlCmd.Parameters;
                sqltran.Commit();
                this._sqlconnection.Close();
                return ds;
            }
            catch (System.Exception ex)
            {
                sqltran.Rollback();
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }


        /// <summary>
        /// ��ղ�������
        /// </summary>
        public void ClearParameter()
        {
            if (this.ParameterArray != null)
                this.ParameterArray.Clear();
            if (this.sqlCmd.Parameters != null)
                this.sqlCmd.Parameters.Clear();
        }

        /// <summary>
        /// ִ�����ݿ��ѯ��䣬���ؽ��Ϊ��ṹ
        /// </summary>
        /// <returns>�����</returns>
        public DataTable GetQueryTable(string sqlval)
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
                //����������
                ParameterArray = sqlCmd.Parameters;
                this._sqlconnection.Close();
                return ds.Tables[0];
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return null;
            }
        }

        /// <summary>
        ///����ѯ���ص������
        /// </summary>
        /// <param name="sqlval">sql�ַ���</param>
        /// <returns>���ؽ��</returns>
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
                this._sqlconnection.Close();
                return robj;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return robj;
            }
        }
        public bool BulkDelete(string sqlval)
        {
            var sql = string.Format(@"
SET ROWCOUNT 10000;
declare @rc int

WHILE 1 = 1
BEGIN
    begin tran t1
    {0} 
    set @rc=@@ROWCOUNT  --commic��,@@rowcountΪ0
    commit tran t1
    IF @rc = 0
        BREAK;
END

SET ROWCOUNT 0;
", sqlval);
            SetHugeCommandTimeOut();
            return ExecuteTransactSql(sql);
        }
        public bool Truncate(string tableName)
        {
            if (tableName.IndexOf("where") > -1)
            {
                ErrorMessage = "TRUNCATE�����Դ�where����";
                return false;
            }
            var sql = string.Format(@"
TRUNCATE TABLE {0};
", tableName);
            SetHugeCommandTimeOut();
            return ExecuteTransactSql(sql);
        }
        /// <summary>
        /// ����Ǩ��(�������ԭ����)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool BulkTable(DataTable dt, string tableName)
        {
            if (!(dt != null && dt.Rows.Count != 0))
            {
                ErrorMessage = "Bulk��DataTableΪ��";
                return false;
            }
            sqlCmd.Connection = this._sqlconnection;
            OpenConn();

            try
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlCmd.Connection, SqlBulkCopyOptions.KeepIdentity, null);//KeepIdentity���Զ�sql�е�������ǿ��ָ��ֵ
                bulkCopy.DestinationTableName = tableName;
                //bulkCopy.BatchSize = dt.Rows.Count;//������ʱ�ᳬʱ
                bulkCopy.BatchSize = 10000;
                bulkCopy.BulkCopyTimeout = CommandTimeOut;
                bulkCopy.WriteToServer(dt);
                this._sqlconnection.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return false;
            }
        }
        public bool BulkReader(DbDataReader dr, string tableName
            , Action<int> sqlRowsCopiedAction = null
            )
        {
            if (dr == null)
            {
                ErrorMessage = "Bulk��DataReaderΪ��";
                return false;
            }
            sqlCmd.Connection = this._sqlconnection;
            if (this._sqlconnection.State == ConnectionState.Closed)
                this._sqlconnection.Open();

            try
            {
                int batch = 10000;
                var opts = SqlBulkCopyOptions.KeepIdentity;
                if (sqlRowsCopiedAction != null)
                {
                    opts |= SqlBulkCopyOptions.FireTriggers;
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlCmd.Connection, opts, null);//KeepIdentity���Զ�sql�е�������ǿ��ָ��ֵ
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.BatchSize = batch;//������ʱ�ᳬʱ
                if (sqlRowsCopiedAction != null)
                {
                    int already = 0;
                    bulkCopy.NotifyAfter = batch;
                    bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler((obj, arg) =>
                    {
                        sqlRowsCopiedAction(already);
                        already += batch;
                    });
                }
                bulkCopy.BulkCopyTimeout = CommandTimeOut;
                bulkCopy.EnableStreaming = true;
                bulkCopy.WriteToServer(dr);
                dr.Close();
                this._sqlconnection.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Error = ex;
                this._sqlconnection.Close();
                return false;
            }
        }
        /// <summary>
        /// ʹ�÷�ʽ:
        /// 1.�ȴ�
        ///   var b = proc.BulkReaderAsync(dr, fullTableName, sqlRowsCopiedAction).Result;
        /// 2.���ȴ�  
        ///   proc.BulkReaderAsync(dr, fullTableName, sqlRowsCopiedAction);//������Result�����첽
        ///   
        /// �첽ԭ��:
        /// ��Ϊ�ⲿ���ô˷������첽,���Ա������ڵĴ���Ӧ����һ���߳���
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlRowsCopiedAction"></param>
        /// <returns></returns>
        public virtual Task<bool> BulkReaderAsync(DbDataReader dr, string tableName
            , Action<int> sqlRowsCopiedAction = null, bool? keepIdentityNullable = null, int? batchNullable = null)
        {
            Task<bool> rTask = new Task<bool>(() =>
            {
                bool keepIdentity = keepIdentityNullable ?? false;
                int batch = batchNullable ?? 10000;

                var b = false;
                if (dr == null)
                {
                    ErrorMessage = "Bulk��DataReaderΪ��";

                    return false;
                    //b = false;
                }
                else
                {
                    sqlCmd.Connection = this._sqlconnection;
                    if (this._sqlconnection.State == ConnectionState.Closed)
                        this._sqlconnection.Open();

                    try
                    {
                        //int batch = 100000;// 10000;
                        var opts = keepIdentity ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default;
                        if (sqlRowsCopiedAction != null)
                        {
                            opts |= SqlBulkCopyOptions.FireTriggers;
                        }

                        SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlCmd.Connection, opts, null);//KeepIdentity���Զ�sql�е�������ǿ��ָ��ֵ
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BatchSize = batch;//������ʱ�ᳬʱ
                        if (sqlRowsCopiedAction != null)
                        {
                            int already = 0;
                            bulkCopy.NotifyAfter = batch;
                            bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler((obj, arg) =>
                            {
                                already += batch;
                                sqlRowsCopiedAction(already);
                            });
                        }
                        bulkCopy.BulkCopyTimeout = CommandTimeOut;
                        bulkCopy.EnableStreaming = true;
                        var bulkTask = bulkCopy.WriteToServerAsync(dr);
                        bulkTask.Wait();
                        dr.Close();
                        this._sqlconnection.Close();
                        return true;

                    }
                    catch (System.Exception ex)
                    {
                        Error = ex;
                        this._sqlconnection.Close();
                        b = false;
                    }
                }
                return b;
            });
            rTask.Start();
            return rTask;
        }
        //public void HugeInsertReader<TModel>(//SqlInsertCollection insert,
        //    TModel ModelItem,
        //    DbDataReader rdr, string tableName,
        //    Action<TModel> rowAction=null)
        //    where TModel:new()
        public bool HugeInsertReader(SqlInsertCollection insert,
            DbDataReader rdr, string tableName,
            Action<SqlInsertCollection> rowAction = null,
            Action<int> sqlRowsCopiedAction = null)
        {
            //dst
            //var dstConnectionString = ConfigurationManager.ConnectionStrings["dayConnection"].ConnectionString;

            //using (MySqlConnection srcConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["unicomMemberConnection"].ConnectionString))
            //{
            OpenConn();
            //srcConn.Open();
            //�˴����ö�ȡ�ĳ�ʱ����Ȼ�ں�������ʱ�����׳�ʱ

            //var c = new MySqlCommand("set net_write_timeout=9000099; set net_read_timeout=9000099", srcConn);
            //c.ExecuteNonQuery();
            //MySqlCommand cmd = new MySqlCommand(sqlTBox.Text, srcConn);

            //////����д�Ļ�,��ѯ��䲻�ܼ�limit,������Ȼ����:���Զ�ȡ������ĩβ�����ݡ�
            ////MySqlCommand cmd = new MySqlCommand(@"set net_write_timeout=90000; set net_read_timeout=90000; " + sqlTBox.Text, srcConn);

            //cmd.CommandTimeout = 90000;//û�����ʱ����:�޷��Ӵ��������ж�ȡ����: �������ӷ���һ��ʱ���û����ȷ�𸴻����ӵ�����û�з�Ӧ�����ӳ���ʧ�ܡ��� ---> System.Net.Sockets.SocketException: �������ӷ���һ��ʱ���û����ȷ�𸴻����ӵ�����û�з�Ӧ�����ӳ���ʧ��

            //// Fetched source data successfully from MySQL DB at this point
            //MySqlDataReader rdr = cmd.ExecuteReader();

            var sb = new StringBuilder();

            //var insert = new SqlInsertCollection(new TModel());
            int idx = 0;
            //var cnt = rdr.FieldCount;
            //var lastIdx = rdr.FieldCount-1;
            int batchSize = 500;// 500ʱ,��ExecuteSql,ʱ��10000��/48��;
            int batchCnt = 0;
            sb.AppendFormat(@" insert into {0}({1}) values", tableName, insert.ToKeysSql());
            bool hasUnDo = false;
            SetHugeCommandTimeOut();
            ////var dstExec = new ProcManager(dstConnectionString);
            //int already=0;
            while (rdr.Read())
            {
                //var model=new TModel();
                foreach (var i in insert)
                {
                    i.Value.Value = rdr[i.Key];
                    //i.Value.PInfo.SetValue(model, rdr[i.Key]);
                }
                if (rowAction != null) { rowAction(insert); }
                //insert.UpdateModelValue(model);
                sb.AppendFormat(@"{0}({1})", batchCnt == 0 ? "" : ",", insert.ToValuesSql());
                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    //var b = ExecuteSql(sb.ToString(), false);
                    var b = ExecuteTransactSql(sb.ToString(), false);
                    if (!b)
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
                    sb.AppendFormat(@" insert into {0}({1}) values", tableName, insert.ToKeysSql());
                    hasUnDo = false;
                    batchCnt = 0;
                }
                else
                {
                    batchCnt++;
                }
                idx++;
            }
            //var s = sb.ToString();
            //s = s.Substring(0, s.Length - 1);
            //MessageBox.Show(s);

            if (hasUnDo)
            {
                //var b = ExecuteSql(sb.ToString(), false);
                var b = ExecuteTransactSql(sb.ToString(), false);
                if (!b)
                {
                    rdr.Close();
                    CloseConn();
                    return false;
                }
            }
            rdr.Close();
            CloseConn();
            //dstExec.CloseConn();
            //MessageBox.Show("����l_mm_card_map_user���");
            //srcConn.Close();
            //}
            //srcConnectionString.Close();
            //MessageBox.Show("����l_mm_card_map_user���");
            return true;
        }

        public bool HugeInsertList<T>(
            IList<T> list, string tableName,
            SqlInsertCollection insert = null,
            Action<BatchInsertOption> insertOptionAction = null,
            Action<SqlInsertCollection, T> rowAction = null,
            Action<int> sqlRowsCopiedAction = null)
        {
            if (list == null || list.Count < 1) { return false; }
            bool autoUpdateByModel = false;//����û�û�ṩinsert,��˵���ֶ��Ǹ���list���ɵ�,�����Զ�����
            if (insert == null)
            {
                insert = new SqlInsertCollection(list[0]);
                autoUpdateByModel = true;
            }

            var insertOption = new BatchInsertOption();
            if (insertOptionAction != null) { insertOptionAction(insertOption); }

            OpenConn();


            var sb = new StringBuilder();

            int idx = 0;

            int batchSize = insertOption.ProcessBatch;// 50000;// tidb���ô�Щ����,����100����/25��
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
                if (rowAction != null) { rowAction(insert, i); }

                //if (oneThousandCount > 999)//sqlserver��values���1000��,��tidbû���������(��������ű���)(ע�����Ļ�,���Դ�19����ٵ�14�����)
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

        public bool PFBulkTable(DataTable tableColumn,
            DbDataReader rdr, string tableName,
            Action<DataRow> rowAction = null,
            Action<int> sqlRowsCopiedAction = null)
        {
            //dst
            //var dstConnectionString = ConfigurationManager.ConnectionStrings["dayConnection"].ConnectionString;

            //using (MySqlConnection srcConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["unicomMemberConnection"].ConnectionString))
            //{
            OpenConn();
            //srcConn.Open();
            //�˴����ö�ȡ�ĳ�ʱ����Ȼ�ں�������ʱ�����׳�ʱ

            //var c = new MySqlCommand("set net_write_timeout=9000099; set net_read_timeout=9000099", srcConn);
            //c.ExecuteNonQuery();
            //MySqlCommand cmd = new MySqlCommand(sqlTBox.Text, srcConn);

            //////����д�Ļ�,��ѯ��䲻�ܼ�limit,������Ȼ����:���Զ�ȡ������ĩβ�����ݡ�
            ////MySqlCommand cmd = new MySqlCommand(@"set net_write_timeout=90000; set net_read_timeout=90000; " + sqlTBox.Text, srcConn);

            //cmd.CommandTimeout = 90000;//û�����ʱ����:�޷��Ӵ��������ж�ȡ����: �������ӷ���һ��ʱ���û����ȷ�𸴻����ӵ�����û�з�Ӧ�����ӳ���ʧ�ܡ��� ---> System.Net.Sockets.SocketException: �������ӷ���һ��ʱ���û����ȷ�𸴻����ӵ�����û�з�Ӧ�����ӳ���ʧ��

            //// Fetched source data successfully from MySQL DB at this point
            //MySqlDataReader rdr = cmd.ExecuteReader();

            //var sb = new StringBuilder();

            //var insert = new SqlInsertCollection(new TModel());
            int idx = 0;
            //var cnt = rdr.FieldCount;
            //var lastIdx = rdr.FieldCount-1;
            int batchSize = 500;// 500ʱ,��ExecuteSql,ʱ��10000��/48��;
            int batchCnt = 0;
            //sb.AppendFormat(@" insert into {0}({1}) values", tableName, insert.ToKeysSql());
            bool hasUnDo = false;
            ////var dstExec = new ProcManager(dstConnectionString);
            //int already=0;
            while (rdr.Read())
            {
                var row = tableColumn.NewRow();
                //var model=new TModel();
                foreach (DataColumn i in tableColumn.Columns)
                {
                    //i.Value.Value = rdr[i.Key];
                    ////i.Value.PInfo.SetValue(model, rdr[i.Key]);
                    row[i.ColumnName] = rdr[i.ColumnName];
                }
                if (rowAction != null) { rowAction(row); }
                tableColumn.Rows.Add(row);
                //insert.UpdateModelValue(model);
                //sb.AppendFormat(@"{0}({1})", batchCnt == 0 ? "" : ",", insert.ToValuesSql());
                hasUnDo = true;
                if (batchCnt > batchSize)
                {
                    var b = BulkTable(tableColumn, tableName);
                    //var b = ExecuteSql(sb.ToString(), false);
                    //var b = ExecuteTransactSql(sb.ToString(), false);
                    if (!b)
                    {
                        rdr.Close();
                        CloseConn();
                        //MessageBox.Show(dstExec.ErrorMessage);
                        return false;
                    }
                    if (sqlRowsCopiedAction != null)
                    {
                        sqlRowsCopiedAction(idx);
                    }
                    //sb.Clear();
                    //sb.AppendFormat(@" insert into {0}({1}) values", tableName, insert.ToKeysSql());
                    hasUnDo = false;
                    batchCnt = 0;
                }
                else
                {
                    batchCnt++;
                }
                idx++;
            }
            //var s = sb.ToString();
            //s = s.Substring(0, s.Length - 1);
            //MessageBox.Show(s);

            if (hasUnDo)
            {
                var b = BulkTable(tableColumn, tableName);
                //var b = ExecuteSql(sb.ToString(), false);
                //var b = ExecuteTransactSql(sb.ToString(), false);
                if (!b)
                {
                    rdr.Close();
                    CloseConn();
                    ////MessageBox.Show(dstExec.ErrorMessage);
                    return false;
                }
            }
            rdr.Close();
            CloseConn();
            //dstExec.CloseConn();
            //MessageBox.Show("����l_mm_card_map_user���");
            //srcConn.Close();
            //}
            //srcConnectionString.Close();
            //MessageBox.Show("����l_mm_card_map_user���");
            return true;
        }
        public bool BatchUpdate<T>(string tableName, SqlDataReader rdr, Func<SqlDataReader, T> rdrAction, ref SqlUpdateCollection update, Action<int> batchProcess = null)
            where T : new()
        {

            int batch = 1000;
            int cur = 0;
            int already = 0;
            string sql = "";
            while (rdr.Read())
            {
                var item = rdrAction(rdr);
                if (item == null) { continue; }
                update.UpdateModelValue(item);

                sql += string.Format(@" update {0} set {1} {2};
                ", tableName, update.ToSetSql(), update.ToWhereSql());

                cur++;
                if (cur > batch)
                {
                    //continue;//benjamin 
                    if (!ExecuteTransactSql(sql))
                    {
                        return false;
                    }
                    already += batch;
                    if (batchProcess != null) { batchProcess(already); }
                    cur = 0;
                    //batchList.Clear();
                    sql = "";
                    //batchList = new List<DayOrdersUpdate>();
                }
            }
            rdr.Close();
            CloseConn();
            if (sql != "")
            {
                //return false;//benjamin 
                if (!ExecuteTransactSql(sql))
                {
                    return false;
                }
            }

            return true;//benjamin 
        }
        //��ȫ������BatchUpdateȡ��
        //public bool BatchUpdateByList<T>(string tbName, ref SqlUpdateCollection update, ref List<T> list)
        //    where T : new()
        //{
        //    string sql = "";
        //    foreach (var i in list)
        //    {
        //        update.UpdateModelValue(i);

        //        sql += string.Format(@" update {0} set {1} {2};
        //        ", tbName, update.ToSetSql(), update.ToWhereSql());
        //    }
        //    return ExecuteSql(sql);
        //}
        /// <summary>
        /// ������������(�Զ��ȶ�,�ж��Ǹ��»�������)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        /// <param name="insertAction"></param>
        /// <param name="compareKeys"></param>
        /// <param name="batchProcess"></param>
        /// <returns></returns>
        //public bool BatchUpdateChange<T>(string tableName, IList<T> list, ref SqlUpdateCollection update, Action<int> batchProcess = null)
        //    where T : new()
        public bool BatchImportChange<T>(string tableName, IList<T> list, Func<T, SqlInsertCollection> insertAction,
            //Func<T, SqlUpdateCollection> updateAction, 
            //string [] upateKeys,
            string[] compareKeys,
            Action<int> batchProcess = null)
            where T : new()
        {

            int batch = 1000;
            int cur = 0;
            int already = 0;
            //string sql = "";
            var sb = new StringBuilder();
            var tableNameArr = tableName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var shortTbName = tableNameArr[tableNameArr.Length - 1];
            var b = ExecuteTransactSql(string.Format("select top 0 * into pf_tmp_{0} from {1}", shortTbName, tableName), false);
            //if (!b) { return b; }
            foreach (var item in list)
            {
                if (item == null) { continue; }
                ////update.UpdateModelValue(item);
                //var update = updateAction(item);
                var insert = insertAction(item);
                sb.AppendFormat("insert into pf_tmp_{0}({1}) values({2});", shortTbName, insert.ToKeysSql(), insert.ToValuesSql());

                //sql += string.Format(@" update {0} set {1} {2};
                //", tableName, update.ToSetSql(), update.ToWhereSql());

                cur++;
                if (cur > batch)
                {
                    //continue;//benjamin 
                    if (!ExecuteTransactSql(sb.ToString(), false))
                    {
                        return false;
                    }
                    already += batch;
                    if (batchProcess != null) { batchProcess(already); }
                    cur = 0;
                    //batchList.Clear();
                    //sql = "";
                    sb.Clear();
                    //batchList = new List<DayOrdersUpdate>();
                }
            }
            //rdr.Close();
            CloseConn();
            //if (sql != "")
            if (sb.Length > 0)
            {
                //return false;//benjamin 
                if (!ExecuteTransactSql(sb.ToString(), false))
                {
                    return false;
                }
            }

            var keys = insertAction(list[0]).ToKeysSql()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //var updateSetList= ;
            var updateSetSql = string.Join(",", keys
                .Select(a => "a." + a + "=b." + a).ToList());//��ʵ����ֻ��Ҫ���²����ֶΣ����Ǹ���ȫ��Ҳû����
            var insertKeySql = insertAction(list[0]).ToKeysSql();
            var aKeySql = string.Join(",", keys
                .Select(a => "a." + a).ToList());

            var onSql = string.Join(" and ", compareKeys
                .Select(a => "b." + a + "=a." + a).ToList());
            var sql = string.Format(@"
update a set --a.FgsNo=b.FgsNo,a.InvTypeNo=b.InvTypeNo,a.InvTypeName=b.InvTypeName,a.CDay=b.CDay,a.TotalMoney=b.TotalMoney,a.TotalPv=b.TotalPv
{2}
from {1} a
inner join pf_tmp_{0} b on -- b.FgsNo=a.FgsNo and b.InvTypeNo=a.InvTypeNo and b.CDay=a.CDay
{5}

insert into {1}({3}) 
select -- a.FgsNo,a.InvTypeNo,a.InvTypeName,a.CDay,a.TotalMoney,a.TotalPv 
{4}
from pf_tmp_{0} a
left join {1} b on -- b.FgsNo=a.FgsNo and b.InvTypeNo=a.InvTypeNo and b.CDay=a.CDay
{5}
where b.{6} is null

drop table pf_tmp_{0}

", shortTbName, tableName, updateSetSql, insertKeySql, aKeySql, onSql, compareKeys[0]);
            //return true;//benjamin 
            return ExecuteTransactSql(sql);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TransferToOtherTable(string srcTable, string srcDb, string dstTable, string dstDb)
        {
            string sql = string.Format(@"
declare @colNames varchar(max),@sql nvarchar(max),@hasBiaoShi bit,@biaoShiCol varchar(max);
set @colNames=''

--select @colNames+=name+','  from  [{1}].dbo.syscolumns where id in(
--  select object_id('{1}.dbo.{0}')  
--)
----and colstat<>1 ������
--and colstat<>4 --������

--��Ϊ��Դ��Ŀ�궼�е��ֶ�--benjamin20191129
select @colNames+=a.name+',' 
from (
	select * from  [{1}].dbo.syscolumns where id in(
	  select object_id('{1}.dbo.{0}')  
	)
	--and colstat<>1 ������
	and colstat<>4 --������
) a
inner join (
	select * from  {3}.dbo.syscolumns where id in(
	  select object_id('{3}.dbo.{2}')  
	)
	--and colstat<>1 ������
	and colstat<>4 --������
) b on b.name=a.name

set @sql=N'
 select top 1 @a=name from {3}.dbo.syscolumns
 where id in(
  object_id(''{3}.dbo.{2}'')
 )and colstat=1

'
EXEC sp_executesql @sql,N'@a varchar(max) output',@biaoShiCol OUTPUT
set @hasBiaoShi=0 
if(@colNames like '%'+@biaoShiCol+',%')
begin
  set @hasBiaoShi=1 
end

set @colNames=substring(@colNames,0,Len(@colNames))
-- select @colNames

if @hasBiaoShi =1 
begin
  SET IDENTITY_Insert {3}.[dbo].{2} ON    
end

set @sql=N'
insert into [{3}].dbo.{2}('+@colNames+') 
    select '+@colNames+' from {1}.dbo.{0}
'
exec(@sql)

if @hasBiaoShi=1 
begin
  SET IDENTITY_Insert {3}.[dbo].{2} OFF
end

", srcTable, srcDb, dstTable, dstDb);
            return ExecuteTransactSql(sql);
        }

        /// <summary>
        /// qlDataReader ����csv�ļ�(���ڵ��뵽mysql)
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool BulkToCSV(string sqlstr, string fileName, Action<int> batchProcess = null)
        {
            ////�԰�Ƕ��ţ���,�����ָ�������Ϊ��ҲҪ�������ڡ�
            ////����������ڰ�Ƕ��ţ���,�����ð�����ţ���""�������ֶ�ֵ����������
            ////����������ڰ�����ţ���"����Ӧ�滻�ɰ��˫���ţ�""��ת�壬���ð�����ţ���""�������ֶ�ֵ����������
            //if (!HadData(dataTable))
            //    return false;
            //if (dataTable == null || dataTable.Rows.Count < 1) { return false; }
            if (File.Exists(fileName))
                File.Delete(fileName);
            var dr = GetDataReader2(sqlstr);
            //using (StreamWriter streamWriter = new StreamWriter(fileName, true, Encoding.UTF8))//��������
            using (StreamWriter streamWriter = new StreamWriter(fileName, true, UnicodeEncoding.GetEncoding(PFEncodeType.GB2312.ToString())))
            {
                int batch = 1000;
                int cur = 0;
                int already = 0;
                string fieldName = string.Empty;

                StringBuilder sb = new StringBuilder();
                //DataColumn colum;
                //DataRow row;
                int index = 0;
                //��ʹ��foreach��Ϊ�˱ߴ��������DataTable
                while (dr.Read())
                {
                    cur++;
                    if (cur > batch)
                    {
                        already += batch;
                        if (batchProcess != null) { batchProcess(already); }
                        cur = 0;
                    }

                    //��ͷ
                    if (index == 0)
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            //colum = dataTable.Columns[i];
                            fieldName = dr.GetName(i);
                            if (i != 0)
                            { sb.Append(","); }

                            sb.Append(fieldName);
                        }
                        sb.AppendLine();
                    }

                    //row = dataTable.Rows[0];
                    //for (int i = 0; i < dataTable.Columns.Count; i++)
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        //colum = dataTable.Columns[i];
                        fieldName = dr.GetName(i);
                        if (i != 0)
                            sb.Append(",");
                        if (dr.GetFieldType(i) == typeof(string) && dr[i].ToString().Contains(","))
                            sb.Append("\"" + dr[i].ToString().Replace("\"", "\"\"") + "\"");
                        else
                            sb.Append(dr[i].ToString());
                    }
                    //dataTable.Rows.RemoveAt(0); //�ؼ����룬����
                    index++;
                    sb.AppendLine();
                    //�ͷ�һ���ڴ�
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

        public bool CloseReader(DbDataReader reader)
        {
            sqlCmd.Cancel();//���û�����,���ݺܶ�ʱ dr.Close ����� https://www.cnblogs.com/xyz0835/p/3379676.html
            reader.Close();
            return true;
        }

        #region ѹ��
        ///// <summary>
        ///// SqlDataReader ����csv�ļ�(���ڵ��뵽mysql)
        /////https://www.cnblogs.com/LeeHuan/p/3921239.html
        ///// </summary>
        ///// <param name="fileNameCsv">�ļ���(�����ļ�·��)</param>
        ///// <param name="dr">���ݱ�</param>
        ///// <param name="hideColumnNames">Ҫ���ص�����</param>
        ///// <param name="encoding">���롾Ĭ��:GB2312��</param>
        ///// <returns></returns>
        //protected static void DownloadCsv(string fileNameCsv, string fileNameZip, SqlDataReader dr, string[] hideColumnNames, string encoding = "gb2312")
        //{
        //    if (dr != null)
        //    {
        //        try
        //        {
        //            Dictionary<string, string> hideCol = new Dictionary<string, string>();
        //            foreach (string item in hideColumnNames)
        //            {
        //                hideCol.Add(item.Trim(), item);
        //            }

        //            if (!File.Exists(fileNameCsv))
        //            {
        //                using (StreamWriter sw = new StreamWriter(fileNameCsv, false, Encoding.GetEncoding(encoding)))
        //                {
        //                    string fieldName = string.Empty;
        //                    StringBuilder sb = new StringBuilder();
        //                    //д���ͷ
        //                    for (int i = 0; i < dr.FieldCount; i++)
        //                    {
        //                        fieldName = dr.GetName(i);
        //                        if (hideCol.ContainsKey(fieldName) == true || fieldName == "")
        //                            continue;
        //                        else
        //                        {
        //                            sb.Append(fieldName);
        //                            sb.Append(",");

        //                        }
        //                    }
        //                    sw.WriteLine(sb.ToString().TrimEnd(','));
        //                    sb.Clear();
        //                    //д�뵼������
        //                    while (dr.Read())
        //                    {
        //                        //����ÿһ��
        //                        for (int i = 0; i < dr.FieldCount; i++)
        //                        {
        //                            fieldName = dr.GetName(i);
        //                            if (hideCol.ContainsKey(fieldName) == true || fieldName == "")
        //                                continue;
        //                            else
        //                            {
        //                                if (!Convert.IsDBNull(dr[i]))
        //                                {
        //                                    string content = string.Format("\"{0}\"", dr[i].ToString().Replace("\"", "\"\""));
        //                                    sb.Append(content);
        //                                }
        //                                else
        //                                {
        //                                    sb.Append("\"\"");
        //                                }
        //                                sb.Append(",");

        //                            }
        //                        }
        //                        sw.WriteLine(sb.ToString().TrimEnd(','));
        //                        sb.Clear();//ÿ�����ݽ�������Ѿ�д���ı�������
        //                    }
        //                }
        //            }
        //            dr.Close();
        //            string filePath = fileNameCsv.Substring(0, fileNameCsv.LastIndexOf(@"\") + 1);//+1 �������һ��б��
        //            //CreateZipFile(filePath, fileNameZip);//����ѹ���ļ�
        //        }
        //        catch (Exception ex)
        //        {
        //            if (!dr.IsClosed)
        //            {
        //                dr.Close();
        //            }
        //            LogHelper.WriteErrorLog("ProcessDocument.DownloadCsv", ex.Message);
        //        }
        //    }
        //} 
        #endregion

        /// <summary>
        /// �๹�캯��
        /// </summary>
        /// <returns></returns>
        public ProcManager()
        {
        }
        /// <summary>
        /// �๹�캯����������������ݿ�������ַ���
        /// </summary>
        /// <returns></returns>
        public ProcManager(string connectionString) : base(connectionString) { }

        public ProcManager(string month, bool flag)
        {
            string connectionString = PFSqlHelper.GetConnectionString(this._connectionstring, "YJQuery" + month);
            Connection(connectionString);
        }
        #region Private
        /// <summary>
        /// ��Ϊ����DAL��SqlExec��Աÿ�ε���ʱ�����³�ʼ����,�Ѿ�ϵͳ�����ƹ���ʱ����������,����д������
        /// </summary>
        private void EnsureProcNotNull()
        {
            if (string.IsNullOrWhiteSpace(ProcName))
            {
                //PFDataHelper.WriteError(new Exception("ProcNameΪnull,�����Ƿ����³�ʼ����SqlExec����,���:" + sqlCmd.Connection.ConnectionString));
                //PFDataHelper.WriteError(new Exception("ProcNameΪnull,�����Ƿ����³�ʼ����SqlExec����,���:" + this._connectionstring));
            }
        }
        #endregion

        public class BatchInsertOption
        {
            private int _processBatch = 500;
            public int ProcessBatch { get { return _processBatch; } set { _processBatch = value; } }
            private bool _autoUpdateModel = true;
            public bool AutoUpdateModel { get { return _autoUpdateModel; } set { _autoUpdateModel = value; } }
        }
    }
}
