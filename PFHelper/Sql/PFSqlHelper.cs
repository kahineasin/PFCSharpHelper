using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace Perfect
{
    public class PFSqlHelper
    {
        private string monthConnectionKey= "YJQuery";//10.0.0.11
        private SqlConnection sqlConnect;

        public NameValueCollection ConnectStrings = new NameValueCollection();

        private string systemPath;

        public void SetSystemPath(string systempath)
        {
            this.systemPath = systempath;
        }

        public string ErrorMessage;

        public String DataSource
        {
            get
            {
                return ConnectStrings["Data Source"];
            }
        }

        public String Database
        {
            get
            {
                return ConnectStrings["Initial Catalog"];
            }
        }

        public String Userid
        {
            get
            {
                return ConnectStrings["User ID"];
            }
        }

        public string Password
        {
            get
            {
                return ConnectStrings["Password"];
            }
        }

        public int CommandTimeOut = 100;

        public PFSqlHelper()
        {
            
        }


        public PFSqlHelper(string connectionString)
        {
            sqlConnect = new SqlConnection(connectionString);
            SetConnectArray(connectionString);
            StartConnect();
        }

        public PFSqlHelper(string connectionString,string databaseName)
        {
            
            SetConnectArray(connectionString);
            connectionString = SetDatabase(databaseName);
            sqlConnect = new SqlConnection(connectionString);
            StartConnect();
        }

        public bool IsConnect
        {
            get
            {
                if (sqlConnect.State == ConnectionState.Open)
                    return true;
                else
                    return false;
            }
        }

        private string SetDatabase(string databaseName)
        {
            ConnectStrings["Initial Catalog"] = databaseName;
            return GetConnectionString();
        }

        public SqlDataReader GetDataReader(string sqlStr)
        {
            SqlCommand sqlCmd= sqlConnect.CreateCommand();
            sqlCmd.CommandText = sqlStr;
            return sqlCmd.ExecuteReader();
        }

        public bool ExecuteSql(string sqlStr)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlConnect);
                sqlCmd.CommandTimeout = 0;
                int result = sqlCmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        public bool ExecuteProduce(string sqlStr)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlConnect);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandTimeout = 9600;
                int result = sqlCmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        public bool ExecuteProduce(string procName,SqlParameter[] parameters)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand(procName,sqlConnect);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = 9600;
                foreach (SqlParameter parameter in parameters)
                {
                    sqlCmd.Parameters.Add(parameters);
                }
                int result = sqlCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        public object GetSqlValue(string sqlStr)
        {
            SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlConnect);
            sqlCmd.CommandTimeout = CommandTimeOut;
            try
            {
                return sqlCmd.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
        }

        public static string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        public void SetConnectArray(string connectionString)
        {
            string[] conn1=connectionString.Split(new char[] { ';' });
            foreach(string conn in conn1)
            {
                string[] conn2 = conn.Split(new char[]{'='});
                ConnectStrings.Add(conn2[0],conn2[1]);
            }
        }

        public static NameValueCollection  GetConnectArray(string connectionString)
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

        public string GetConnectionString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ConnectStrings.Count; i++)
            {
                
                if(i<ConnectStrings.Count-1)
                {
                    sb.Append(ConnectStrings.GetKey(i)+"="+ConnectStrings[i]+";");
                }
                else
                {
                    sb.Append(ConnectStrings.GetKey(i)+"="+ConnectStrings[i]);
                }
            }
            return sb.ToString();
        }

        public static string GetConnectionString(string connectionString,string databaseName)
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


        public void StartConnect()
        {
            if (sqlConnect.State != ConnectionState.Open)
            {
                sqlConnect.Open();
            }
        }

        public void StartConnect(string connectionString)
        {
            if (sqlConnect == null)
                sqlConnect = new SqlConnection();

            CloseConnect();
            sqlConnect.ConnectionString = connectionString;
            StartConnect();
        }

        public void CloseConnect()
        {
            if (sqlConnect != null)
            {
                if (sqlConnect.State == ConnectionState.Open)
                {
                    sqlConnect.Close();
                }
            }
        }


        public static string GetConfigString(string filepath)
        {
            StreamReader txtReader = new StreamReader(filepath);
            string content = txtReader.ReadToEnd();
            txtReader.Close();
            return content;
        }

        public string GetCMonth()
        {
            //string connectionString = GetConnectionString("10.0.0.11");
            string connectionString = GetConnectionString(monthConnectionKey);            
            StartConnect(connectionString);
            object cmonth = GetSqlValue("declare @cmonth varchar(7);select top 1 @cmonth=left(cmonth,4)+'.'+right(cmonth,2) from databasetable order by cmonth desc;select @cmonth;");
            CloseConnect();
            return cmonth.ToString();
        }

        public string GetCDatabase(out string month)
        {
            string connectionString = GetConnectionString("10.0.0.11");
            StartConnect(connectionString);
            object database = GetSqlValue("select top 1 databasename from databasetable order by cmonth desc ");
            object month2 = GetSqlValue("select top 1 left(cmonth,4)+'.'+right(cmonth,2) from databasetable order by cmonth desc ");
            CloseConnect();
            month = month2.ToString();
            return database.ToString();
        }

        public string GetMonth()
        {
            string connectionString = GetConnectionString("10.0.0.11");
            StartConnect(connectionString);
            object cmonth = GetSqlValue("declare @cmonth varchar(6);select top 1 @cmonth=left(cmonth,4)+right(cmonth,2) from databasetable order by cmonth desc;select @cmonth;");
            CloseConnect();
            return cmonth.ToString();
        }

        public static string GetConfigValue(string xmlFile, string key, string value)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(xmlFile);
       
            string s=ds.Tables[0].Select("ID='" + key + "'")[0]["ExecFlag"].ToString();
            return s;
        }

        public void SetConnectionConfigValue(string key,string attriname,string value)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(systemPath + ".config");
            XmlNode node = doc.SelectSingleNode(@"//add[@name='" + key + "']");
            XmlElement ele = (XmlElement)node;

            string[] conn1 = ele.Attributes["connectionString"].Value.Split(new char[] { ';' });
            string newvalue = "";
            foreach (string conn in conn1)
            {
                string[] conn2 = conn.Split(new char[] { '=' });
                if (conn2[0] == attriname)
                {
                    conn2[1] = value;
                }
                newvalue += conn2[0] + "=" + conn2[1]+";";
            }
            newvalue = newvalue.Substring(0, newvalue.Length - 1);

            ele.SetAttribute("connectionString", newvalue);
            doc.Save(systemPath + ".config"); 
        }

        public bool ColumnIsInTable(string columnName, string tableName)
        {
            string sqlstr = string.Format("select [name] from [syscolumns] where id in(select id from [sysobjects] where [name]='{0}')  and [name]='{1}'", tableName, columnName);
            object obj=GetSqlValue(sqlstr);
            if (obj == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 可查看cmd的查询语句--wxj20171212
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static String CommandAsSql(SqlCommand sc)
        {
            StringBuilder sql = new StringBuilder();
            Boolean FirstParam = true;

            sql.AppendLine("use " + sc.Connection.Database + ";");
            switch (sc.CommandType)
            {
                case CommandType.StoredProcedure:
                    sql.AppendLine("declare @return_value int;");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {
                            sql.Append("declare " + sp.ParameterName + "\t" + sp.SqlDbType.ToString() + "\t= ");

                            sql.AppendLine(((sp.Direction == ParameterDirection.Output) ? "null" : ParameterValueForSQL(sp)) + ";");

                        }
                    }

                    sql.AppendLine("exec [" + sc.CommandText + "]");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if (sp.Direction != ParameterDirection.ReturnValue)
                        {
                            sql.Append((FirstParam) ? "\t" : "\t, ");

                            if (FirstParam) FirstParam = false;

                            if (sp.Direction == ParameterDirection.Input)
                                sql.AppendLine(sp.ParameterName + " = " + ParameterValueForSQL(sp));
                            else

                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterName + " output");
                        }
                    }
                    sql.AppendLine(";");

                    sql.AppendLine("select 'Return Value' = convert(varchar, @return_value);");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {
                            sql.AppendLine("select '" + sp.ParameterName + "' = convert(varchar, " + sp.ParameterName + ");");
                        }
                    }
                    break;
                case CommandType.Text:
                    sql.AppendLine(sc.CommandText);
                    break;
            }

            return sql.ToString();
        }

        #region Private
        private static String ParameterValueForSQL(SqlParameter sp)
        {
            String retval = "";

            switch (sp.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                    break;

                case SqlDbType.Bit:
                    //retval = (sp.Value.ToBooleanOrDefault(false)) ? "1" : "0";
                    retval = (sp.Value == (object)true) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        } 
        #endregion
    }
}
