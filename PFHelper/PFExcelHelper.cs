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
using Aspose.Cells;
using System.Threading.Tasks;

namespace Perfect
{
    public partial class PFDataHelper
    {
        public static bool DownloadExcel(HttpContext context, Workbook workbook, string fileName, long speed)
        {
            bool ret = true;
            try
            {
                #region 定义局部变量
                long startBytes = 0;
                int packSize = 1024 * 10; //分块读取，每块10K bytes

                var tmpFileName = Guid.NewGuid().ToString("N") + DateTime.Now.ToString("yyyyMMddHHmmss") + fileName;
                var path = Path.Combine(PFDataHelper.BaseDirectory, "TempFile", tmpFileName);
                var directoryName = Path.GetDirectoryName(path);
                PFDataHelper.CreateDirectory(directoryName);
                workbook.Save(path);//注意，当xlsx文件超过65535行时，如果调用workbook.SaveToStream方法，会使行数变少，所以暂时只想到先保存到服务器的办法

                var filePath = path;
                FileStream myFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
                    PFDataHelper.DeleteFile(path);
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        public static List<Dictionary<string, object>> ExcelToDictList(Workbook workbook)
        {
            var list = new List<Dictionary<string, object>>();
            var cols = new List<string>();

            //if (Request.Files.Count < 1)
            //{
            //    return Json(JsonData.SetFault("文件为空"));
            //}
            //Workbook wb = new Workbook(Request.Files[0].InputStream);
            //if (wb == null)
            //{
            //    return Json(JsonData.SetFault("excel打开失败"));
            //}
            var sheet = workbook.Worksheets[0];
            int rowCnt = sheet.Cells.Rows.Count;
            int colCnt = sheet.Cells.Columns.Count;
            for (int j = 0; j < colCnt; j++)
            {
                cols.Add(PFDataHelper.ObjectToString(sheet.Cells[0, j].Value));
            }
            var telephoneIdx = cols.IndexOf("telephone");
            if (telephoneIdx < 0)
            {
                return null;
            }
            for (int i = 1; i < rowCnt; i++)
            {
                var item = new Dictionary<string, object>();
                var telephone = PFDataHelper.ObjectToString(sheet.Cells[i, telephoneIdx].Value);
                if (PFDataHelper.StringIsNullOrWhiteSpace(telephone))//只要有一行为空，就返回
                {
                    return list;
                }
                for (int j = 0; j < colCnt; j++)
                {
                    item[cols[j]] = sheet.Cells[i, j].Value;
                }
                list.Add(item);
            }
            return list;
        }

        public static List<T> ExcelToList<T>(Worksheet sheet, Dictionary<string, string> columnTextNameDict = null, string keyColumnName = null)
            where T : new()
        {
            //创建一个属性的列表
            //List<PropertyInfo> prlist = new List<PropertyInfo>();
            Dictionary<string, PropertyInfo> prDict = new Dictionary<string, PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            Type t = typeof(T);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表 
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => {
                if (columnTextNameDict == null || columnTextNameDict.Values.Contains(p.Name))
                {
                    prDict.Add(p.Name, p);
                }
            });
            //创建返回的集合
            List<T> oblist = new List<T>();

            //var list = new List<Dictionary<string, object>>();
            var cols = new List<string>();

            //if (Request.Files.Count < 1)
            //{
            //    return Json(JsonData.SetFault("文件为空"));
            //}
            //Workbook wb = new Workbook(Request.Files[0].InputStream);
            //if (wb == null)
            //{
            //    return Json(JsonData.SetFault("excel打开失败"));
            //}
            //var sheet = workbook.Worksheets[0];
            int rowCnt = sheet.Cells.Rows.Count;
            int colCnt = sheet.Cells.Columns.Count;
            for (int j = 0; j < colCnt; j++)
            {
                cols.Add(PFDataHelper.ObjectToString(sheet.Cells[0, j].Value));
            }
            int keyIdx = -1;
            //var keyColumnText = columnTextNameDict == null ? keyColumnName : columnTextNameDict.First(a => a.Value == keyColumnName).Key;
            if (keyColumnName != null)
            {
                keyIdx = cols.IndexOf(columnTextNameDict == null ? keyColumnName : columnTextNameDict.First(a => a.Value == keyColumnName).Key);
                if (keyIdx < 0)
                {
                    return null;
                }
            }
            for (int i = 1; i < rowCnt; i++)
            {
                //var item = new Dictionary<string, object>();
                if (keyColumnName != null)
                {
                    var keyValue = PFDataHelper.ObjectToString(sheet.Cells[i, keyIdx].Value);
                    if (PFDataHelper.StringIsNullOrWhiteSpace(keyValue))//只要有一行为空，就返回
                    {
                        return oblist;
                    }
                }
                var item = new T();
                for (int j = 0; j < colCnt; j++)
                {
                    //item[cols[j]] = sheet.Cells[i, j].Value;
                    //var sheetHeadRowText = ObjectToString(sheet.Cells[0, j].Value);
                    var columnText = cols[j];
                    var columnName = columnTextNameDict == null ? columnText : columnTextNameDict[columnText];
                    var p = prDict[columnName];
                    var pt = PFDataHelper.GetPropertyType(p);
                    var cellValue = sheet.Cells[i, j].Value;
                    if (pt.IsEnum)
                    {
                        p.SetValue(item, Enum.Parse(pt, cellValue.ToString()), null);
                    }
                    //else if (pt.IsSubclassOf(typeof(PFCustomStringType)))
                    //{
                    //    p.SetValue(ob, row[p.Name].ToString(), null);
                    //}
                    else if (cellValue != null)
                    {
                        //if (pt == typeof(decimal) && dt.Columns[p.Name].DataType == typeof(int))
                        //{
                        //    //由于某些数据库版本不统一的问题会导致,inv表的pv是decimal的，但tc_inv表的pv是int的，于是建model时tc_inv都采用decimal就行了
                        //}

                        p.SetValue(item, ConvertObjectByType(sheet.Cells[i, j].Value, cellValue.GetType(), p.PropertyType), null);

                    }
                }
                oblist.Add(item);
            }
            return oblist;
        }
    }
}
