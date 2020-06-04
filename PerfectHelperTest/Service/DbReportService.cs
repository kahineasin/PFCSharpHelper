using Newtonsoft.Json.Linq;
using Perfect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YJQuery.Models;

namespace SaveDbReport.Service
{
    /// <summary>
    /// 有种子列的表
    /// </summary>
    public class HasSeedTable
    {
        public string Database { get; set; }
        public string TableName { get; set; }
        public string Fields { get; set; }
    }
    /// <summary>
    /// 不要多线程调用同一个实例
    /// </summary>
    public class DbReportService
    {
        #region 这样的话多线程似乎不安全--benjamin20190730
        //protected ProcManager _srcSqlExec;
        //protected ProcManager _dstSqlExec;
        //protected MySqlExecute _unicomHyzlSqlExec;
        //protected ProcManager SrcSqlExec { get { return _srcSqlExec??(_srcSqlExec=new ProcManager(ConfigurationManager.ConnectionStrings["srcConnection"].ConnectionString)); } }
        //protected ProcManager DstSqlExec { get { return _dstSqlExec??(_dstSqlExec=new ProcManager(ConfigurationManager.ConnectionStrings["dstConnection"].ConnectionString)); } }
        //protected MySqlExecute UnicomHyzlSqlExec { get { return _unicomHyzlSqlExec??(_unicomHyzlSqlExec=new MySqlExecute(ConfigurationManager.ConnectionStrings["unicomHyzlConnection"].ConnectionString)); } } 
        public static string DaySqlConn { get { return ConfigurationManager.ConnectionStrings["dayConnection"].ConnectionString; } }
        #endregion
        /// <summary>
        /// 192.168.0.30
        /// </summary>
        protected ProcManager SrcSqlExec { get { return new ProcManager(ConfigurationManager.ConnectionStrings["srcConnection"].ConnectionString); } }
        /// <summary>
        /// 192.168.0.33
        /// </summary>
        protected ProcManager DstSqlExec { get { return new ProcManager(ConfigurationManager.ConnectionStrings["dstConnection"].ConnectionString); } }
        /// <summary>
        /// 192.168.0.33 balance
        /// </summary>
        protected ProcManager DaySqlExec { get { return new ProcManager(ConfigurationManager.ConnectionStrings["dayConnection"].ConnectionString); } }
        protected ProcManager CkglSqlExec { get { return new ProcManager(ConfigurationManager.ConnectionStrings[SqlConn.Ckgl].ConnectionString); } }
        protected MySqlExecute UnicomHyzlSqlExec { get { return new MySqlExecute(ConfigurationManager.ConnectionStrings["unicomHyzlConnection"].ConnectionString); } }
        protected MySqlExecute UnicomTradeSqlExec { get { return new MySqlExecute(ConfigurationManager.ConnectionStrings["unicomTradeConnection"].ConnectionString); } }

        private string _cmonth;
        /// <summary>
        /// 格式如:2019.01
        /// </summary>
        protected string Cmonth { get { return _cmonth; } }
        /// <summary>
        /// 格式如:201901
        /// </summary>
        protected string Ym { get { return Cmonth.Replace(".", ""); } }
        public IDataReader ReadingDR { get; set; }
        private ISqlExecute UsingProc { get; set; }
        //protected int HugeCommandTimeOut = 3600;
        #region 需要根据表结构更新的字段
        //private string _bonusTcFields = @"[agentno]
        //   ,[hybh]
        //   ,[hyxm]
        //   ,[type]
        //   ,[moneyflag]
        //   ,[old_pv]
        //   ,[pv]
        //   ,[old_bjhybh]
        //   ,[bjhybh]
        //   ,[old_hpos]
        //   ,[hpos]
        //   ,[tcmonth]
        //   ,[lrmonth]
        //   ,[lrman]
        //   ,[lrdate]
        //   ,[flag]
        //   ,[tcType]
        //   ,[editdate]
        //   ,[grjf]
        //   ,[zzjf]";
        //public List<HasSeedTable> SeedTable = new List<HasSeedTable>
        //{
        //    new HasSeedTable
        //    {
        //        Database="bonus",
        //        TableName="t_tc",
        //        Fields=@"[id],[agentno]
        //   ,[hybh]
        //   ,[hyxm]
        //   ,[type]
        //   ,[moneyflag]
        //   ,[old_pv]
        //   ,[pv]
        //   ,[old_bjhybh]
        //   ,[bjhybh]
        //   ,[old_hpos]
        //   ,[hpos]
        //   ,[tcmonth]
        //   ,[lrmonth]
        //   ,[lrman]
        //   ,[lrdate]
        //   ,[flag]
        //   ,[tcType]
        //   ,[editdate]
        //   ,[grjf]
        //   ,[zzjf]"
        //    }
        //};
        #endregion
        public DbReportService(string cmonth)
        {
            _cmonth = cmonth;
        }

        public void ChangeCMonth(string cmonth) {
            _cmonth = cmonth;
        }

        /// <summary>
        /// if(@dstTbExist=1)是为了,如果30中加了新表,那之前的月份的dst库中可能不存在该表
        /// </summary>
        /// <param name="backupDatabase">不包含月份</param>
        /// <param name="needRowCount">不需要行数时不查（提高效率）</param>
        /// <param name="notInTableNames">排除某些表（提高效率）,为空时默认查全部表</param>
        /// <returns></returns>
        public List<CompareCnt> GetCompareCntList(string backupDatabase, bool needRowCount, string[] notInTableNames = null)
        {
            var proc = SrcSqlExec;

            var sql = string.Format(@"

declare @sql nvarchar(max),@cmonth varchar(7),@ym varchar(6),@now datetime,@b bit,@c int,@dbname varchar(max),@dbpath varchar(max),@src varchar(max),@dst varchar(max),@commited bit,@dstTbExist bit;
set @src='192.168.0.30'
set @dst='192.168.0.33'
set @cmonth='{0}'
set @ym=left(@cmonth,4)+RIGHT(@cmonth,2)

create table #m001(SrcTbName varchar(max),idd int  identity(1,1),Flag bit,Total int,TotalMonth varchar(7),TotalDate DateTime,SrcCnt int,DstCnt int)
set @sql=N'insert into #m001(SrcTbName)
select name from {1}.dbo.sysobjects where xtype=''u'' and name<>''t_zxbmonth'' {3} ';
exec (@sql)
Declare @idx int=1,@max int,@tname varchar(max);
select @max=MAX(idd) from #m001
While(@idx<=@max)
Begin
	select top 1 @tname=SrcTbName from #m001
	where idd=@idx	
	
	SELECT @sql= N'
		if exists (select * from ['+@dst+'].{1}'+@ym+'.dbo.sysobjects where xtype=''u'' and name='''+@tname+''') 
		  begin	
			set @a=1
		  end	
		else
		  begin
			set @a=0
		  end
	'	 
	EXEC sp_executesql @sql,N'@a bit output',@dstTbExist OUTPUT

	if(@dstTbExist=1 and 1={2})
	begin
	    set @sql=N'
	    update #m001 set 
	    SrcCnt=(
		    select count(*) from {1}.dbo.'+@tname+'
	    ),
	    DstCnt=(
		    select count(*) from ['+@dst+'].{1}'+@ym+'.dbo.'+@tname+'
	    )
	    where SrcTbName='''+@tname+'''
	    '
	    exec (@sql)
    end
		
	update #m001 set Flag=aa.Flag,Total=aa.qty,TotalMonth=aa.cmonth,TotalDate=aa.cdate
	from (select Flag,qty,cmonth,cdate from {1}.dbo.t_zxbmonth where tbname=@tname) aa
	where SrcTbName=@tname 
	
	set @idx=@idx+1
End
--统计表单独
set @sql=N'insert into #m001(SrcTbName,SrcCnt,DstCnt)
select 
''t_zxbmonth'',
(select count(*) from {1}.dbo.t_zxbmonth),
(select count(*) from ['+@dst+'].{1}'+@ym+'.dbo.t_zxbmonth)
';
exec (@sql)
select * from #m001 
drop table #m001
",
Cmonth,
backupDatabase,
needRowCount ? 1 : 0,
(notInTableNames != null && notInTableNames.Any()) ? string.Format(" and name not in (''{0}'') ", string.Join("'',''", notInTableNames)) : "");
            var dt = proc.GetQueryTable(sql);
            var list = new List<CompareCnt>();
            if (dt != null && dt.Rows.Count > 0)
            {
                list = PFDataHelper.DataTableToList<CompareCnt>(dt).OrderBy(a => a.SrcTbName).ToList();
            }
            return list;
        }
        public List<CompareCnt> GetAllMonthDataSrcList(string backupDatabase)
        {
            string sql = string.Format(@"
declare @sql nvarchar(max)
create table #m001(SrcTbName varchar(max),idd int  identity(1,1),Flag bit,Total int,TotalMonth varchar(7),TotalDate DateTime,SrcCnt int,DstCnt int)
set @sql=N'insert into #m001(SrcTbName)
select name from {0}.dbo.sysobjects where xtype=''u'' and name<>''t_zxbmonth''  ';
exec (@sql)
select * from #m001 
", backupDatabase);
            var proc = SrcSqlExec;
            var dt = proc.GetQueryTable(sql);
            var list = new List<CompareCnt>();
            if (dt != null && dt.Rows.Count > 0)
            {
                list = PFDataHelper.DataTableToList<CompareCnt>(dt).OrderBy(a => a.SrcTbName).ToList();
            }
            return list;
        }
    }
}
