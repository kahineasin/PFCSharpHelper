using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Perfect;
using System.Collections.Generic;
using Aspose.Cells;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Net;

namespace PerfectHelperTest
{
    [TestClass]
    public class PFDataHelperTest
    {
        public static string UserEmailHostName = "smtp.perfect99.com";
        public static string UserEmailUserName = "wxj@perfect99.com";
        public static string UserEmailPwd = "shi3KjkE48QZ3SPA";

        /// <summary>
        /// 修复月份数据库的方法(临时用)
        /// </summary>
        [TestMethod]
        public void FixYjqueryMonthDatabaseSql()
        {
            return;
            //200801~201804
            //开始月份
            var time = new DateTime(2018, 2, 1);
            var cmonth = time.ToString("yyyyMM");
            //结束月份
            int endMonth = 200801;
            string tmpStr = "";
            //while (int.Parse(cmonth) >= 200801)
            while (int.Parse(cmonth) >= endMonth)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["YJQuery"].ConnectionString;
                connectionString = PFDataHelper.GetConnectionString(connectionString, "YJQuery" + cmonth);
                var sqlExec = new ProcManager(connectionString);

                string sql = @"
alter table cus add province varchar(20)  
alter table cus add city varchar(50)  
";
                if (!sqlExec.ExecuteSql(sql))
                {
                    break;
                };
                sql = @"
update a set a.province=b.province,a.city=b.city  -- a.[cmonth]=b.cmonth...
from cus a
inner join yjquery201908.dbo.cus b on b.agentno=a.agentno
";
                if (!sqlExec.ExecuteSql(sql))
                {
                    break;
                };
                //sqlExec.ExecuteSql("select province from cus group by province");

                tmpStr += cmonth + "\r\n";
                time = time.AddMonths(-1);
                cmonth = time.ToString("yyyyMM");
            }
            var aa = tmpStr;
        }
        /// <summary>
        /// 从月份数据库里查订单(临时用)
        /// </summary>
        [TestMethod]
        public void QueryOrdersFromMonthDatabase()
        {
            return;
            //200801~201804
            //开始月份
            var time = new DateTime(2017, 7, 2);
            int endMonth = 200801;
            var cmonth = time.ToString("yyyyMM");
            //结束月份
            string hasDataMonth = "";
            string errorMonth = "";
            //while (int.Parse(cmonth) >= 200801)
            while (int.Parse(cmonth) >= endMonth)
            {
                try
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["YJQuery"].ConnectionString;
                    connectionString = PFDataHelper.GetConnectionString(connectionString, "YJQuery" + cmonth);
                    var sqlExec = new ProcManager(connectionString);

                    string sql = @"
select count(*) from orders where hybh='40027931'
";

                    int cnt = PFDataHelper.ObjectToInt(sqlExec.QuerySingleValue(sql)) ?? 0;
                    if (cnt > 0)
                    {
                        hasDataMonth += cmonth + "\r\n";
                    };
                    if (!PFDataHelper.StringIsNullOrWhiteSpace(sqlExec.ErrorMessage))
                    {
                        errorMonth += cmonth + "\r\n";
                    }
                }catch(Exception e)
                {
                    errorMonth += cmonth + "\r\n";
                }

                time = time.AddMonths(-1);
                cmonth = time.ToString("yyyyMM");
            }
            var aa = hasDataMonth;
        }

        [TestMethod]
        public void TestObjectToDateString()
        {
            DateTime? o1;
            o1 = null;
            var r1 = PFDataHelper.ObjectToDateString((object)o1, "yyyy-MM-dd");
            string o2;
            o2 = null;
            var r2 = PFDataHelper.ObjectToDateString((object)o2, "yyyy-MM-dd");
            var r3 = PFDataHelper.ObjectToDateString((object)DateTime.Now, "yyyy-MM-dd");
            var r4 = PFDataHelper.ObjectToDateString((object)"20180321", "yyyy-MM-dd");
        }

        [TestMethod]
        public void TestObjectToString()
        {
            var os = GetEachTypeObject();
            foreach (var i in os)
            {
                PFDataHelper.ObjectToString(i);
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestStringDesensitization()
        {
            string sfzh = "442000198807022634";
            string rSfzh = PFDataHelper.StringDesensitization(sfzh, 3, 4);
            Assert.IsTrue(rSfzh[2] == '*' && rSfzh[3] != '*'
                && rSfzh[13] != '*' && rSfzh[14] == '*');

            string rSfzh2 = PFDataHelper.StringDesensitization(sfzh,
                new KeyValuePair<int, int>(0, 2),
                new KeyValuePair<int, int>(14, 17)
                );
            Assert.IsTrue(rSfzh.Equals(rSfzh2));
        }


        [TestMethod]
        public void TestZip()
        {
            return;
            string f1 = "D:\\wxj\\releaseProject\\ReleaseProject\\ReleaseProject\\UpdateLog\\SysTest\\UpdatePackage";
            string f2 = "D:\\wxj\\releaseProject\\ReleaseProject\\ReleaseProject\\UpdateLog\\SysTest\\UpdatePackage.zip";
            //ZipHelper.ZipDirectory("E:\\test", "E:\\test1.zip");
            //ZipHelper.ZipDirectory(f1, f2);
            //ZipHelper.ZipFile(f1, f2);

            ZipUtility zip = new ZipUtility();
            zip.ZipFileFromDirectory(f1, f2, 4);
        }

        [TestMethod]
        public void TestEnum()
        {
            var aa =PFDataHelper.ObjectToEnum<FuncAuthority>(1);
            return;
            Func<FuncAuthority, FuncAuthority, bool> checkAuthor = (myAuth, needAuth) =>
            {
                return PFDataHelper.EnumAnyFlag(myAuth, needAuth);
            };
            var a = FuncAuthority.Add;
            var b = FuncAuthority.Edit;
            //var c = FuncAuthority.Delete;
            var e = FuncAuthority.Default;
            var Default = FuncAuthority.Default;
            var Add = FuncAuthority.Add;
            var Edit = FuncAuthority.Edit;
            var Export = FuncAuthority.Export;
            var Delete = FuncAuthority.Delete;

            Assert.IsTrue(checkAuthor(a, a | b));//应为true

            Assert.IsTrue(checkAuthor(b, a | b));//应为true

            Assert.IsTrue(checkAuthor(a | b, a));//应为true

            Assert.IsTrue(checkAuthor(a | b, a | b));//应为true

            Assert.IsFalse(checkAuthor(a, b));//应为false

            Assert.IsFalse(checkAuthor(e, a | b));//应为false
            Assert.IsFalse(checkAuthor(e, a));//应为false
            Assert.IsTrue(checkAuthor(a, e));//应为true

            Assert.IsTrue(checkAuthor(e, e));//应为true
            Assert.IsTrue(checkAuthor(Add | Export, Add | Edit));//应为true
            Assert.IsFalse(checkAuthor(Delete | Export, Add | Edit));//应为false
            Assert.IsFalse(checkAuthor(Default, Export));//应为false
            Assert.IsTrue(checkAuthor(Default | Export, Export));//应为true

        }

        [TestMethod]
        public void TestListRandomTake()
        {
            var list = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                list.Add(i);
            }
            var result = new List<int>();

            var random = new Random();
            for (int i = 0; i < 100; i++)
            {

                result.Add(PFDataHelper.ListRandomTake(ref random, list, 1)[0]);
            }
            Assert.IsTrue(list.Count == 4);

            Assert.IsTrue(list.TrueForAll(a => result.Contains(a)));


            for (int i = 0; i < 4; i++)
            {
                PFDataHelper.ListRandomTake(ref random, list, 1, true);
            }
            Assert.IsTrue(list.Count == 0);
        }

        [TestMethod]
        public void TestTransExpV2()
        {
            Student s = new Student() { Age = 20, Id = 1, Name = "Emrys" };
            StudentSecond ss = TransExpV2<Student, StudentSecond>.Trans(s);
            Student sss = TransExpV2<Student, Student>.Trans(s);
            Assert.IsTrue(s.Age == ss.Age && s.Id == ss.Id && s.Name == ss.Name);
            Assert.IsTrue(s.Age == sss.Age && s.Id == sss.Id && s.Name == sss.Name);
        }

        [TestMethod]
        public void TestExcel()
        {
            return;
            try
            {
                Perfect.PFDataHelper.SetConfigMapper(new Perfect.PFConfigMapper());

                #region 2003,只能65535行
                //var _excelFormat = FileFormatType.Excel97To2003;
                //var workbook = new Workbook(_excelFormat);
                //var sheet = workbook.Worksheets[0];
                ////var otherSheet = other.GetSheet();
                //var otherSheet = workbook.Worksheets.Add("otherSheet");
                ////var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
                ////s.Copy(otherSheet);
                ////var export = new XlsExport();
                ////export.Init(null, new PrintPageScheme());
                //for (var i = 0; i < 75536; i++)//65536
                //{
                //    var x = 0;//列
                //    var y = i;//行
                //    sheet.Cells[y, x].PutValue(i.ToString());
                //    otherSheet.Cells[y, x].PutValue(i.ToString());
                //    //export.FillData(0, i, null, i.ToString());
                //}
                //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excel.xls");
                //var directoryName = Path.GetDirectoryName(path);
                //PFDataHelper.CreateDirectory(directoryName);
                ////export.workbook.Save(path);
                //workbook.Save(path);

                ////Console.Write("保存到" + path);
                //System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹 
                #endregion

                #region 2007,可以全部行
                //var _excelFormat = FileFormatType.Xlsx;
                //var workbook = new Workbook(_excelFormat);
                //var sheet = workbook.Worksheets[0];
                ////var otherSheet = other.GetSheet();
                //var otherSheet = workbook.Worksheets.Add("otherSheet");
                ////var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
                ////s.Copy(otherSheet);
                ////var export = new XlsExport();
                ////export.Init(null, new PrintPageScheme());
                //for (var i = 0; i < 75536; i++)//65536
                //{
                //    var x = 0;//列
                //    var y = i;//行
                //    sheet.Cells[y, x].PutValue(i.ToString());
                //    otherSheet.Cells[y, x].PutValue(i.ToString());
                //    //export.FillData(0, i, null, i.ToString());
                //}
                //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excel.xlsx");
                //var directoryName = Path.GetDirectoryName(path);
                //PFDataHelper.CreateDirectory(directoryName);
                ////export.workbook.Save(path);
                //workbook.Save(path);

                ////Console.Write("保存到" + path);
                //System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹 
                #endregion


                #region 超过100万行
                //var _excelFormat = FileFormatType.Xlsx;
                //var workbook = new Workbook(_excelFormat);
                //var sheet = workbook.Worksheets[0];
                //////var otherSheet = other.GetSheet();
                ////var otherSheet = workbook.Worksheets.Add("otherSheet");
                //////var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
                //////s.Copy(otherSheet);
                //////var export = new XlsExport();
                //////export.Init(null, new PrintPageScheme());
                //for (var i = 0; i < 1048577; i++)//65536,新版本最大行数是1048576
                //{
                //    var x = 0;//列
                //    var y = i;//行
                //    sheet.Cells[y, x].PutValue(i.ToString());
                //    //otherSheet.Cells[y, x].PutValue(i.ToString());
                //    ////export.FillData(0, i, null, i.ToString());
                //}
                //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excel.xlsx");
                //var directoryName = Path.GetDirectoryName(path);
                //PFDataHelper.CreateDirectory(directoryName);
                ////export.workbook.Save(path);
                //workbook.Save(path);

                ////Console.Write("保存到" + path);
                //System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹  
                #endregion

                #region 2007破解,可以全部行
                //Perfect.XlsExport.Crack();//破解excel
                //var _excelFormat = FileFormatType.Xlsx;
                //var workbook = new Workbook(_excelFormat);
                //var sheet = workbook.Worksheets[0];
                ////var otherSheet = other.GetSheet();
                //var otherSheet = workbook.Worksheets.Add("otherSheet");
                ////var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
                ////s.Copy(otherSheet);
                ////var export = new XlsExport();
                ////export.Init(null, new PrintPageScheme());
                //for (var i = 0; i < 75536; i++)//65536
                //{
                //    var x = 0;//列
                //    var y = i;//行
                //    sheet.Cells[y, x].PutValue(i.ToString());
                //    otherSheet.Cells[y, x].PutValue(i.ToString());
                //    //export.FillData(0, i, null, i.ToString());
                //}
                //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excelPo.xlsx");
                //var directoryName = Path.GetDirectoryName(path);
                //PFDataHelper.CreateDirectory(directoryName);
                ////export.workbook.Save(path);
                //workbook.Save(path);

                ////Console.Write("保存到" + path);
                //System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹 
                #endregion


                #region 2007破解(export),可以全部行
                //Perfect.XlsExport.Crack();//破解excel
                ////var _excelFormat = FileFormatType.Xlsx;
                ////var workbook = new Workbook(_excelFormat);
                ////var sheet = workbook.Worksheets[0];
                //////var otherSheet = other.GetSheet();
                ////var otherSheet = workbook.Worksheets.Add("otherSheet");
                //////var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
                //////s.Copy(otherSheet);
                //var export = new XlsExport();
                //export.Init(null, new PrintPageScheme());
                //for (var i = 0; i < 75536; i++)//65536
                //{
                //    var x = 0;//列
                //    var y = i;//行
                //    //sheet.Cells[y, x].PutValue(i.ToString());
                //    //otherSheet.Cells[y, x].PutValue(i.ToString());
                //    export.FillData(x, y, null, i.ToString());
                //}
                //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excelPo.xlsx");
                //var directoryName = Path.GetDirectoryName(path);
                //PFDataHelper.DeleteFile(path);
                //PFDataHelper.CreateDirectory(directoryName);
                //export.workbook.Save(path);
                ////workbook.Save(path);

                ////Console.Write("保存到" + path);
                //System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹 
                #endregion


                #region 2007破解(export),超过100万行
                //Perfect.XlsExport.Crack();//破解excel
                ////var _excelFormat = FileFormatType.Xlsx;
                ////var workbook = new Workbook(_excelFormat);
                ////var sheet = workbook.Worksheets[0];
                //////var otherSheet = other.GetSheet();
                ////var otherSheet = workbook.Worksheets.Add("otherSheet");
                //////var s = workbook.Worksheets.Add(title ?? otherSheet.Name);
                //////s.Copy(otherSheet);
                //var export = new XlsxExport1048576();
                //export.Init(null, new PrintPageScheme());
                //for (var i = 0; i < 1048577*3; i++)//65536,新版本最大行数是1048576
                //{
                //    var x = 0;//列
                //    var y = i;//行
                //    //sheet.Cells[y, x].PutValue(i.ToString());
                //    //otherSheet.Cells[y, x].PutValue(i.ToString());
                //    export.FillData(x, y, null, i.ToString());
                //}
                //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excelPo.xlsx");
                //var directoryName = Path.GetDirectoryName(path);
                //PFDataHelper.DeleteFile(path);
                //PFDataHelper.CreateDirectory(directoryName);
                //export.workbook.Save(path);
                ////workbook.Save(path);

                ////Console.Write("保存到" + path);
                //System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹  
                #endregion

                #region 2007破解(export)(实际数据),可以全部行
                //                Perfect.XlsExport.Crack();//破解excel
                //                var cmonth = "2019.11";
                //                var month = cmonth.Replace(".", "");
                //                var query = new SqlWhereCollection {
                //                { "a.hybh", "" },
                //                { "a.dqbm", "" },
                //                { "a.accmon", cmonth }
                //            };
                //                var SqlString = string.Format(@" 
                //select  c.province,a.dqbm,a.hybh,a.hyxm,a.hysfzh,c.tel,b.pv
                //from [hyzl] a
                //left join [s1] b on b.hybh=a.hybh
                //left join [cus] c on c.cus_no=a.dqbm
                //{0}
                //", query.ToSql());
                //                var connectionString = ConfigurationManager.ConnectionStrings["YJQuery"].ConnectionString;
                //                connectionString = PFDataHelper.GetConnectionString(connectionString, "YJQuery" + month);
                //                //Connection(connectionString);
                //                var SqlExec = new ProcManager(connectionString);
                //                var dt = SqlExec.GetQueryTable(SqlString);
                //                var all = dt;
                //                StoreColumnCollection columns = null;
                //                if (all != null && all.Rows.Count > 0)
                //                {
                //                    var c = all.Columns;
                //                    columns = new StoreColumnCollection("yjquery") {
                //  {c["province"]}
                //,  {c["dqbm"],a=>a.title="网点编号"}
                //,  {c["hybh"]}
                //,  {c["hyxm"]}
                //,  {c["hysfzh"]}
                //,  {c["tel"]}
                //,  {c["pv"]}

                //                };
                //                    new StoreColumn { Children = columns }.EachLeaf(a => a.SetWidthByTitleWords());
                //                }
                //                var pagingResult = PFDataHelper.PagingStore(dt, new MvcPagingParameters { },
                //                    columns,
                //                    false, null);
                //                var exporter = Exporter.Instance(pagingResult ?? new PagingResult(), new ExporterOption
                //                {
                //                    FileType = "xlsx",//benjamin todo
                //                    Scheme = Exporter.FinancialScheme
                //                    ,
                //                    SheetTitle = cmonth
                //                    //,
                //                    //SheetTitle = GetWordCMonth(cmonthff) + hr + fgsname
                //                }).FileName("总表");//这里的下载名没用到
                //                var export = (exporter.GetExport() as XlsExport);

                //                var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excelPo.xlsx");
                //                var directoryName = Path.GetDirectoryName(path);
                //                PFDataHelper.DeleteFile(path);
                //                PFDataHelper.CreateDirectory(directoryName);
                //                export.workbook.Save(path);
                //                //workbook.Save(path);

                //                //Console.Write("保存到" + path);
                //                System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹 
                #endregion

                #region 2007破解(export)(实际数据),超过100万行
                Perfect.XlsExport.Crack();//破解excel
                var hybh = "";
                var cmonth = "202001";
                var agentno = "";

                var accmon = "";
                var qx = false;
                var qxmonth = "";
                var s = 0;
                var e = 0;
                var sf = "";
                var city = "";
                //int top = 500000; //50万是可以的
                int top = 1000000;
                var query = new SqlWhereCollection {
                        {"a.hybh",hybh },
                        {"a.dqbm",agentno },
                        {"a.accmon",accmon },
                        {"a.qx",qx },
                        {"a.qxmonth",qxmonth },
                        {"b.province",sf }

                };


                string SqlString = string.Format(@" 
select top {1} a.hybh,a.hyxm,a.hyxb,a.qxmonth,a.accmon ,
  b.agentno,b.name as agentname,b.province,b.city,c.hpos,c.qpos,1 as counthy,d.sales,d.pv
from hyzl a 
left join cus b on b.agentno=a.dqbm
left join p1 c on c.hybh=a.hybh
left join s1 d on d.hybh=a.hybh
{0} 
",
            query.ToSql(),
            top
                );

                var connectionString = ConfigurationManager.ConnectionStrings["YJQuery"].ConnectionString;
                connectionString = PFDataHelper.GetConnectionString(connectionString, "YJQuery" + cmonth);
                var SqlExec = new ProcManager(connectionString);

                var sqlExec = SqlExec;
                sqlExec.CommandTimeOut = 20000;

                var dt = SqlExec.GetQueryTable(SqlString);
                var all = dt;

                StoreColumnCollection columns = null;
                if (all != null && all.Rows.Count > 0)
                {
                    var c = all.Columns;
                    columns = new StoreColumnCollection("hyzl,yjquery,Hyzl.Product,bonus") {
  {c["hybh"]}
,  {c["hyxm"]}
//,  {c["hysfzh"]}
,  {c["hyxb"]}
,  {c["hpos"]}
,  {c["qpos"]}
,  {c["sales"]}
,  {c["pv"]}

//,  {c["pexm"]}
//,  {c["pesfzh"]}
//,  {c["pexb"]}
//,  {c["yzbm"]}
//,  {c["dhqh"]}
//,  {c["zzdh"]}
//,  {c["bgdh"]}
//,  {c["bjhybh"]}
//,  {c["rhrq"]}
//,  {c["dqbm"]}
//,  {c["ch"]}
//,  {c["sys"]}
//,  {c["f"]}
//,  {c["g"]}
//,  {c["pc"]}
//,  {c["sysman"]}
//,  {c["code"]}
//,  {c["flag"]}
//,  {c["yhybh"]}
//,  {c["dqsf"]}
//,  {c["dqsx"]}
//,  {c["qx"]}
,  {c["qxmonth"]}
//,  {c["yhzh"]}
//,  {c["yhmc"]}
//,  {c["yhbh"]}
//,  {c["yhyb"]}
//,  {c["yhsf"]}
//,  {c["yhsx"]}
//,  {c["hycsrq"]}
//,  {c["txdz"]}
//,  {c["yhzh1"]}
//,  {c["pecsrq"]}
,  {c["accmon"]}
//,  {c["cus_no"]}
//,  {c["name"]}
//,  {c["name1"]}
//,  {c["adrs"]}
//,  {c["adrs1"]}
//,  {c["adrs2"]}
//,  {c["seqno"]}
//,  {c["chair"]}
//,  {c["area_no"]}
//,  {c["hybh1"]}
//,  {c["tel"]}
//,  {c["fax"]}
//,  {c["pcode"]}
//,  {c["limit"]}
//,  {c["unrecv"]}
//,  {c["untax"]}
//,  {c["flag2"]}
//,  {c["flag1"]}
//,  {c["pfb"]}
//,  {c["month"]}
//,  {c["ztno"]}
//,  {c["tel1"]}
//,  {c["sfno"]}
//,  {c["highqty"]}
//,  {c["yj"]}
//,  {c["flb"]}
//,  {c["px"]}
//,  {c["email"]}
//,  {c["hyxm1"]}
//,  {c["cus_nobak"]}
,  {c["agentno"]}
,  {c["agentname"]}
,  {c["province"]}
,  {c["city"]}
                    , {c["counthy"],a=> {
                        a.visible=false;
                    }

                    }

                };
                }
                var pagingResult = PFDataHelper.PagingStore(dt, new MvcPagingParameters { PageIndex = 0, PageSize = top },
                columns,
                false, null);
                var exporter = Exporter.Instance(pagingResult ?? new PagingResult(), new ExporterOption
                {
                    FileType = "xlsx",//benjamin todo
                    Scheme = Exporter.FinancialScheme
                    ,
                    SheetTitle = cmonth
                    //,
                    //SheetTitle = GetWordCMonth(cmonthff) + hr + fgsname
                }).FileName("总表");//这里的下载名没用到
                var export = (exporter.GetExport() as XlsxExport1048576);

                var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "excelPo.xlsx");
                var directoryName = Path.GetDirectoryName(path);
                PFDataHelper.DeleteFile(path);
                PFDataHelper.CreateDirectory(directoryName);
                export.workbook.Save(path);
                //workbook.Save(path);

                //Console.Write("保存到" + path);
                System.Diagnostics.Process.Start(directoryName); //如果是本地访问就直接打开文件夹 
                #endregion

            }
            catch (Exception e)
            {
                throw e;
                // PFDataHelper.WriteError(e);
            }
        }

        /// <summary>
        /// 行转列
        /// </summary>
        [TestMethod]
        public void PFPivotTable()
        {
            return;
            Perfect.PFDataHelper.SetConfigMapper(new Perfect.PFConfigMapper());
            Perfect.XlsExport.Crack();//破解excel
            var query = new SqlWhereCollection
            {
                //{ "CDay", DateTime.Now.AddMonths(-6) ,SqlExpressionOperator.Greater}
            };
            var SqlString = string.Format(@" 
SELECT  [SalesDayId]
      ,[FgsNo]
      ,[InvTypeName]
      ,[CDay]
      ,[TotalMoney]
      ,[TotalPv]
  FROM [balance].[dbo].[SalesDay]
   where CDay>'2019-01-01 00:00:00.000'
  {0} 
", query.ToSql());
            var connectionString = ConfigurationManager.ConnectionStrings["dayConnection"].ConnectionString;
            var SqlExec = new ProcManager(connectionString);
            var dt = SqlExec.GetQueryTable(SqlString);

            #region All situations
            //var pivotTable = (new PFPivotTable(dt))
            //    //.SetLeft("FgsNo")
            //    //.SetTop("InvTypeName")

            //    //.SetTop("FgsNo", "InvTypeName")

            //    .SetLeft("FgsNo", "InvTypeName")

            //    //.SetLeft("InvTypeName")
            //    //.SetTop("FgsNo")

            //    .SetValue("TotalMoney","TotalPv" )
            //    .GroupDataTable()
            //    ;

            //var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "pivotTable.xlsx");
            //pivotTable.SaveToFile(path); 
            #endregion

            #region lt
            var pivotTable = (new PFPivotTable(dt))
                .SetLeft("FgsNo")
                .SetTop("InvTypeName")
                .SetValue("TotalPv")
                .GroupDataTable()
                ;
            var path = Path.Combine(PFDataHelper.BaseDirectory, "output", "pivotTable_lt.xlsx");
            pivotTable.SaveToFile(path);
            #endregion

            #region ll
            pivotTable = (new PFPivotTable(dt))
                .SetLeft("FgsNo", "InvTypeName")
                .SetValue("TotalMoney", "TotalPv")
                .GroupDataTable()
                ;
            path = Path.Combine(PFDataHelper.BaseDirectory, "output", "pivotTable_ll.xlsx");
            pivotTable.SaveToFile(path);
            #endregion

            #region tt
            pivotTable = (new PFPivotTable(dt))
                .SetTop("FgsNo", "InvTypeName")
                .SetValue("TotalMoney", "TotalPv")
                .GroupDataTable()
                ;
            path = Path.Combine(PFDataHelper.BaseDirectory, "output", "pivotTable_tt.xlsx");
            pivotTable.SaveToFile(path);
            #endregion
        }

        [TestMethod]
        public void TestWriteLog() {
            return;
            PFDataHelper.WriteError(new Exception("aaaa"));
            PFDataHelper.WriteLog("aaaa");
        }
        [TestMethod]
        public void TestCopyModelConfig()
        {
            PFDataHelper.SetConfigMapper(new PFConfigMapper());
            var m1 =PFDataHelper.GetModelConfig("yjquery", "yjquery")["hybh"];
            var m2 = m1.Clone();
            var t = typeof(PFModelConfig);
            var ps = t.GetProperties().Where(p => p.PropertyType.IsPublic && p.CanRead
            //&& p.CanWrite //CanRead已经够了--benjamin20190802
            ).ToArray();
            Array.ForEach<PropertyInfo>(ps, p => {
                var v1 = p.GetValue(m1);
                var v2 = p.GetValue(m2);
                if (
                //p.GetValue(m1) != p.GetValue(m2)
                //!p.GetValue(m1).Equals(p.GetValue(m2))
                !PFDataHelper.IsObjectEquals(v1,v2)
                )
                {
                    throw new Exception(string.Format("m1 m2 的属性 {0} 的值不相等",p.Name));
                }

            });
        }
        [TestMethod]
        public void TestReadFileToCodes()
        {
            var fs = new FileStream("D:\\wxj\\workRecord\\20200420_批量取消crm的一批直销员\\取消.txt", FileMode.Open);
            var codes = PFDataHelper.ReadFileToCodes(fs);
            fs.Close();
        }


        protected string GetSaveTempDSellerNosSqlOld(List<string> dsellernos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
create table #dsellerno1(dsellerno varchar(30)) 
");//dsellerno 30长度
            int idx = 0;
            foreach (var i in dsellernos)
            {
                sb.AppendFormat(@" insert into #dsellerno1(dsellerno) values ('{0}')  ", i.Replace("'", "''"));//直销员证有单引号
                idx++;
            }
            return sb.ToString();
        }
        protected string GetSaveTempDSellerNosSql(List<string> dsellernos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
create table #dsellerno1(dsellerno varchar(30)) 
");//dsellerno 30长度
            int idx = 0;
            foreach (var i in dsellernos)
            {
                if (idx > 999)
                {
                    idx = 0;
                    sb.AppendFormat(@" insert into #dsellerno1(dsellerno) values ('{0}')  ", i.Replace("'", "''"));//直销员证有单引号
                }
                else if (idx == 0)
                {
                    sb.AppendFormat(@"insert into #dsellerno1(dsellerno) values('{0}')", i.Replace("'", "''"));//INSERT 语句中行值表达式的数目超出了 1000 行值的最大允许值。 
                }
                else
                {
                    sb.AppendFormat(@",('{0}')", i.Replace("'", "''"));
                }
                idx++;
            }
            return sb.ToString();
        }
        [TestMethod]
        public void TestInsertSpeed()
        {
            int total = 100000;// 1000; 10000条成功 100000条报错
            var list = new List<string>();
            for (int i = 0; i < total; i++)
            {
                list.Add(i.ToString());
            }

            var t1=PFDataHelper.CountTime(() => {
                //StringBuilder sb = new StringBuilder();
                //sb.Append(@" create table #hybh1(hybh varchar(20)) ");
                //for(int i = 0; i < total; i++)
                //{
                //    sb.AppendFormat(@" insert into #hybh1(hybh) values ('{0}')  ", i);
                //}
                var sqlExec = new ProcManager(ConfigurationManager.ConnectionStrings["messageServerTest"].ConnectionString);
                sqlExec.SetHugeCommandTimeOut();
                var a=sqlExec.QuerySingleValue(string.Format(@"
{0}
select count(*) from #dsellerno1
", GetSaveTempDSellerNosSqlOld(list)));
                var bb = "bb";
            });
            var t2 = PFDataHelper.CountTime(() => {
                //StringBuilder sb = new StringBuilder();

                //sb.Append(@"
                //create table #hybh1(hybh varchar(20)) 
                //insert into #hybh1(hybh) values
                //");//dsellerno 30长度
                //int idx = 0;
                //for (int i = 0; i < total; i++)
                //{
                //    if (idx == 0)
                //    {
                //        sb.AppendFormat(@"('{0}')", i);//INSERT 语句中行值表达式的数目超出了 1000 行值的最大允许值。 
                //    }
                //    else
                //    {
                //        sb.AppendFormat(@",('{0}')", i);
                //    }
                //    //sb.AppendFormat(@" insert into #" + fieldName + "1(" + fieldName + ") values ('{0}')  ", i);
                //    idx++;
                //}
                //var sqlExec = new ProcManager(ConfigurationManager.ConnectionStrings["messageServerTest"].ConnectionString);
                //sqlExec.ExecuteSql(sb.ToString());

                var sqlExec = new ProcManager(ConfigurationManager.ConnectionStrings["messageServerTest"].ConnectionString);
                sqlExec.SetHugeCommandTimeOut();
                var a = sqlExec.QuerySingleValue(string.Format(@"
{0}
select count(*) from #dsellerno1
", GetSaveTempDSellerNosSql(list)));
                var bb = "bb";
            });
            long time = t2 / t1; // time=1148/857  1126/871
            var aa = "aa";
        }
        [TestMethod]
        public void TestDictIndexSpeed()
        {
            var dict = new Dictionary<string, bool>();
            for(var i = 0; i < 10000000; i++)
            {
                dict.Add(i.ToString(), true);
            }
            var array = dict.Select(a=>a.Key).ToArray();

            var t1=PFDataHelper.CountTime(() => {
                //var a = dict.ContainsKey("9999999");//t1:0
                //foreach (var i in dict) { }//t1:142

                foreach (var i in dict.Keys) { }//t1:49
            });
            var t2 = PFDataHelper.CountTime(() => {
                //var a = array.Contains("9999999");//t2:49
                //foreach (var i in array) { }//t2:19

                var tmpArr = dict.Keys.ToArray();
                foreach (var i in tmpArr) { }//t2:67
            });
    

            var aa = "aa";
        }
        [TestMethod]
        public void TestCache()
        {
            var list= new List<Dictionary<string, object>>();
            list.Add(new Dictionary<string, object> { { "cmonth", "2020.04" }, { "hybh","00001118"} });
            //PFDataHelper.WriteLocalJson(list, "TestCache.json");
            var fileName = "TestCache.txt";
            PFDataHelper.WriteLocalTxtLine(fileName, list.ToArray());

            var cache = PFDataHelper.ReadLocalTxtLines<Dictionary<string, object>>(fileName);
            var aa = "aa";
        }
        //private string HttpGetCookie(string Url)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        //    request.Method = "GET";
        //    request.ContentType = "text/html;charset=UTF-8";
        //    request.CookieContainer = cookie;

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    Stream myResponseStream = response.GetResponseStream();
        //    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        //    string retString = myStreamReader.ReadToEnd();
        //    myStreamReader.Close();
        //    myResponseStream.Close();

        //    return retString;
        //}
        [TestMethod]
        public void TestHttpGet()
        {
            string url = "";

            ////url = "http://192.168.0.69:38201/crm/getusersmodify";
            //url = "https://perfect.zhixueyun.com/api/v1/course-study/subject/chapter-progress?courseId=a785316d-1f4d-4a8c-b187-cd745d5e62d3&_=1589851030046";
            //var result = PFDataHelper.HttpGet(url);
            //return;


            var cookie = new CookieContainer();
            //Cookie cook = new Cookie();
            //cook.Value = "781bad2915898671495572963e5494d49834d8d8a728eac2b6a269bbe3b732";
            //cook.Name = "acw_tc";
            //cookie.Add(cook);
            cookie.Add(new Cookie("acw_tc", "781bad2915898671495572963e5494d49834d8d8a728eac2b6a269bbe3b732","/", ".zhixueyun.com"));
            cookie.Add(new Cookie("captcha","adb64d17-f87a-44cb-af7c-ee2c97648909", "/", ".zhixueyun.com"));

            Action<HttpPostOption> postOptionAction = null;
            //var xx = PFDataHelper.HttpGet("https://zxy9.zhixueyun.com/oauth/api/v1/auth");
             url = "https://zxy9.zhixueyun.com/oauth/api/v1/auth";
           // url = "https://zxy9.zhixueyun.com/oauth/api/v1/auth?organization_id=MhWc4Po8qmaJpjBZTtZV/vzWDNrS6+m+38Rn3E8jigd6vnra+L4edfeKJL6eB2BL&login_method=JZNm+1f9txtGtiE0oRMJ1g==&username=6NDtwKSc8/mSvJ577UxTsA==&pword=W1tl9ctj1QpoN/0oArzwSQ==&remember=U5odry+hxJVf/JHFehA/oA==";

            Dictionary<string, string> bodyDict = new Dictionary<string, string>
            {
                { "organization_id","MhWc4Po8qmaJpjBZTtZV/vzWDNrS6+m+38Rn3E8jigd6vnra+L4edfeKJL6eB2BL" },
                { "login_method","JZNm+1f9txtGtiE0oRMJ1g==" },
                { "username","6NDtwKSc8/mSvJ577UxTsA==" },
                { "pword","W1tl9ctj1QpoN/0oArzwSQ==" },
                { "remember","U5odry+hxJVf/JHFehA/oA==" }
            };

            //string body= JsonConvert.SerializeObject(bodyDict);
            string body = null;
            var postOption = new HttpPostOption();
            if (postOptionAction != null) { postOptionAction(postOption); }
            postOption.Cookie = cookie;
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            //request.Accept = "text/html, application/xhtml+xml, */*";
            request.Accept = "*/*";
            //request.ContentType = "application/json";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            //request.ContentType = "application/json;charset=UTF-8";
            request.Referer = "https://zxy9.zhixueyun.com/oauth/";
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
            HttpWebResponse response = null;

            try
            {
                response= (HttpWebResponse)request.GetResponse(); 
            }catch(Exception e)
            {
                var aa = e;
                throw e;
            }
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
                    System.GC.Collect();
                }
                var aa = "aa";
                //return r;
            }
        }
        //[TestMethod]
        //public void TestLoadArea()
        //{
        //    var area=new FeatureCollection()
        //}
        [TestMethod]
        public void TestIDCardToBirthDay()
        {
            object[][] idcards = new object[][] {
                new object[] { "43022319351230151X",1935,84 },
                new object[] { "330327199207200960", 1992, 27 },
                new object[] { "4415227604082731", null, null }
            };
            foreach(var i in idcards)
            {
                var birthday = PFDataHelper.IDCardToBirthDay(i[0].ToString());
                Assert.IsTrue((i[1]==null&& birthday==null)||birthday.Value.Year == PFDataHelper.ObjectToInt(i[1]));
                var age = PFDataHelper.GetAge(birthday);
                Assert.IsTrue((i[2] == null && age == null) || age == PFDataHelper.ObjectToInt(i[2]));
            }
            //var birthday = PFDataHelper.IDCardToBirthDay("43022319351230151X");
            //Assert.IsTrue(birthday.Value.Year ==1935);
            //var age = PFDataHelper.GetAge(birthday);
            //Assert.IsTrue(age == 84);

            //birthday = PFDataHelper.IDCardToBirthDay("330327199207200960");
            //Assert.IsTrue(birthday.Value.Year == 1992);
            //age = PFDataHelper.GetAge(birthday);
            //Assert.IsTrue(age == 27);
        }

        [TestMethod]
        public void TestCreateChineseCityTable()
        { 

            SqlCreateTableCollection models = new SqlCreateTableCollection
            {
                TableName = "chinese_city",
                PrimaryKey = new string[] { "province", "city" },
                TableIndex = null
            };
            models.Add(new SqlCreateTableItem
            {
                FieldName = "province",
                FieldType = typeof(string)
            });
            models.Add(new SqlCreateTableItem
            {
                FieldName = "city",
                FieldType = typeof(string)
            });
            models.Add(new SqlCreateTableItem
            {
                FieldName = "latitude",
                FieldType = typeof(decimal),
                FieldSqlLength = 12,
                Precision = 8
            });
            models.Add(new SqlCreateTableItem
            {
                FieldName = "longitude",
                FieldType = typeof(decimal),
                FieldSqlLength=12,
                Precision=8
            });

            string ms = null;
            var sqlExec = new MySqlExecute("Database=sale;Data Source=172.16.1.246;User Id=root;Password=perfectTIDB;pooling=false;CharSet=utf8;port=10010;ConnectionTimeout=90000;AllowZeroDateTime=true;ConvertZeroDateTime=true;Protocol=tcp");
            var b = sqlExec.CreateTable(models, out ms);
        }
        /// <summary>
        /// 导入中国省市坐标
        /// </summary>
        [TestMethod]
        public void TestImportChineseCity()
        {
            var listStr = PFDataHelper.ReadLocalTxtLines("E:\\svn\\SaveDbReport\\PFHelper\\PFHelper\\Content\\pfCoordinatesOfChineseCity.txt");
            var list = new List<PFChineseCity>();
            foreach(var i in listStr)
            {
                var strSplit = i.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (strSplit.Length < 3) { continue; }
                var citySplit = strSplit[2].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (citySplit.Length < 2) { continue; }
                var coordinateSplit = strSplit[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (coordinateSplit.Length < 2) { continue; }

                var item = new PFChineseCity();
                item.Province = citySplit[0];
                item.City = citySplit[1];
                item.Latitude =PFDataHelper.ObjectToDecimal( coordinateSplit[1])??0;
                item.Longitude = PFDataHelper.ObjectToDecimal(coordinateSplit[0]) ?? 0;
                list.Add(item);
            }
            var sqlExec = new MySqlExecute("Database=sale;Data Source=172.16.1.246;User Id=root;Password=perfectTIDB;pooling=false;CharSet=utf8;port=10010;ConnectionTimeout=90000;AllowZeroDateTime=true;ConvertZeroDateTime=true;Protocol=tcp");

            MySqlInsertCollection insert = new MySqlInsertCollection()
            {
                { "province" ,""},
                { "city" ,""},
                { "latitude" ,""},
                { "longitude" ,""},
            };
            //SqlInsertCollection insert = new SqlInsertCollection(list[0], "province", "city", "latitude", "longitude");
            var b = sqlExec.HugeInsertList(
               insert,
                list,
                "chinese_city",
                a => a.ProcessBatch = 50000,
               (a,i)=> {
                   a["province"].Value = i.Province;
                   a["city"].Value = i.City;
                   a["latitude"].Value = i.Latitude;
                   a["longitude"].Value = i.Longitude;
               },
                (already) => {
                    //total = already;
                    //loadingfrm.SetJD("当前：正在导入reader", "当前进度：(" + PFDataHelper.ScientificNotation(already) + "/未知)");
                }
                );
            Assert.IsTrue(b);
        }
        [TestMethod]
        public void TestJWT()
        {
            var payload = new Dictionary<string, object>
            {
                { "resource",new { dashboard=7} },
                { "params",new { } },
                { "email","wxj@perfect99.com" },
                { "first_name","肖均" },
                { "last_name","吴" },
                { "exp", DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds() }//到期时间
            };
            string secret = "9f0b293e2a66003d8ab39a6f26431be3798cdd1912975be8a42b9be6c552248c";
            //string secret = "9bcd6721912dc3419f4eea3acb1db7f432a106c34dc8a3c44af9b5b12925aa42";//登陆密钥
            //生成JWT
            string JWTString = JwtHelper.CreateJWT(payload, secret);

            //校验JWT
            string ResultMessage;//需要解析的消息
            string Payload;//获取负载
            Dictionary<string, object> resultPayLoad = null;

            var b = JwtHelper.ValidateJWT(JWTString, out Payload, out ResultMessage, secret);
            if (b)
            {
                resultPayLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(Payload);
            }
            Assert.IsTrue(b
                &&payload["email"].ToString()== resultPayLoad["email"].ToString()
                );
        }
        #region Private
        /// <summary>
        /// 获得各种类型封装的object,便于测试 ObjectTo...()方法
        /// </summary>
        /// <returns></returns>
        private List<object> GetEachTypeObject()
        {
            var r = new List<object>();
            DateTime? o1;
            o1 = null;
            r.Add((object)o1);//空DateTime

            string o2;
            o2 = null;
            r.Add((object)o2);//空String

            object o3 = (object)DateTime.Now;
            r.Add(o3);//DateTime

            object o4 = (object)"20180321";
            r.Add(o4);//String

            Guid? o5 = null;
            r.Add(o5);//空Guid

            Guid? o6 = Guid.NewGuid();
            r.Add(o6);//Guid

            return r;
        }
        #endregion Private
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class StudentSecond : ICloneable
    {
        //public StudentSecond(string name) {
        //    Name = name;
        //}
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
