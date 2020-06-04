using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfect
{
    public static class PFTaskHelper
    {
        public static int CheckMessageInterval { get { return PFDataHelper.ObjectToInt(ConfigurationManager.AppSettings["CheckMessageInterval"])?? 5000; } }

    }
    public interface IPFTask
    {
        void Start();
        void Stop();
    }
    public interface IPFTimeTask : IPFTask
    {
        //void Start();
        //void Stop();
        /// <summary>
        /// 重设备份的时间点
        /// </summary>
        /// <param name="time"></param>
        void ResetBackupTime(DateTime time);

        /// <summary>
        /// 获得当前任务状态的说明,如:任务xx运行中,定时设置在8时9分
        /// </summary>
        /// <returns></returns>
        string GetStatusDescription();

        string GetHashId();
    }
    /// <summary>
    /// 每月执行一次的任务
    /// </summary>
    public class PFMonthTask : IPFTimeTask
    {
        public string HashId { get; set; }
        /// <summary>
        /// 参数执行的月份
        /// </summary>
        public Action<string> DoAction { get; set; }
        public Thread TaskThread { get; set; }
        public bool _running = false;

        protected string Cmonth
        {
            get
            {
                var now = DateTime.Now;
                var year = now.Year;
                var month = now.Month;
                var cmonth = year + "." + month.ToString("00");
                return cmonth;
            }
        }
        /// <summary>
        /// 上次执行的年月
        /// </summary>
        public string _lastBackupCmonth { get; set; }
        /// <summary>
        /// 备份的日期
        /// </summary>
        public int BackupDay { get; set; }
        /// <summary>
        /// 备份的小时
        /// </summary>
        public int BackupHour { get; set; }
        /// <summary>
        /// 备份的分钟(便于测试)
        /// </summary>
        public int BackupMinute { get; set; }

        /// <summary>
        /// 第一次执行要在此时间之后(默认为类初始化的时间)
        /// 注:这是为了实现18点设置1点执行的话,是立即执行还是第二天开始执行
        /// </summary>
        public DateTime FirstRunTime { get; set; }
        /// <summary>
        /// 下次执行的时间(准备用这个属性代替FirstRunTime)
        /// </summary>
        public DateTime _nextRunTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="doAction">参数1：执行的月份</param>
        /// <param name="backupDay"></param>
        /// <param name="backupHour"></param>
        public PFMonthTask(string hashId, Action<string> doAction, int backupDay, int backupHour, int backupMinute)
        {
            HashId = hashId;
            DoAction = doAction;
            BackupDay = backupDay;
            BackupHour = backupHour;
            BackupMinute = backupMinute;

            //这里按一般的习惯,如果当前10日,设置为2日,会等到下个月的2日才执行
            var now = DateTime.Now;
            var firstRunTime = new DateTime(now.Year, now.Month, BackupDay, BackupHour, BackupMinute, 0);
            FirstRunTime = now > firstRunTime ? firstRunTime.AddMonths(1) : firstRunTime;
            _nextRunTime = FirstRunTime;
        }
        public void Start()
        {
            if (!_running)
            {
                _running = true;

                TaskThread = new Thread(new ParameterizedThreadStart(StartThread));
                TaskThread.Start();
            }
        }
        public void Stop()
        {
            if (_running)
            {
                _running = false;
                TaskThread.Abort();//之前先释放MessageService的话,进程里仍在使用MessageService会报错,现在改为先释放Thread试试

            }
        }
        public void StartThread(object ps)
        {
            _lastBackupCmonth = null;
            while (_running == true)
            {
                try
                {

                    var cmonth = Cmonth;
                    if (cmonth == _lastBackupCmonth)//该月已执行
                    {
                        Thread.Sleep(PFTaskHelper.CheckMessageInterval);
                        continue;
                    }
                    var now = DateTime.Now;

                    var backupDay = new DateTime(now.Year, now.Month, BackupDay, BackupHour, BackupMinute, 0);
                    if (backupDay > now || FirstRunTime > now)//未到执行的日期
                    {
                        Thread.Sleep(PFTaskHelper.CheckMessageInterval);
                        continue;
                    }

                    _lastBackupCmonth = cmonth;
                    _nextRunTime = backupDay.AddMonths(1);

                    PFDataHelper.WriteLog(string.Format("任务{0}开始执行,月份为:{1}", HashId, cmonth));
                    try
                    {
                        DoAction(cmonth);
                        PFDataHelper.WriteLog(string.Format("任务{0}执行完毕,月份为:{1}", HashId, cmonth));
                    }
                    catch (Exception e)
                    {
                        PFDataHelper.WriteError(e);
                    }
                    GC.Collect();//一定要有句，否则SendMobileMessage里面的所有List会使内存越来越高
                }
                catch (Exception e)
                {
                    PFDataHelper.WriteError(e);
                }
            }
        }

        public void ResetBackupTime(DateTime time)
        {
            BackupDay = time.Day;
            BackupHour = time.Hour;
            BackupMinute = time.Minute;

            FirstRunTime = time;
            _nextRunTime = FirstRunTime;
            _lastBackupCmonth = null;
        }

        public string GetStatusDescription()
        {
            if (_running)
            {
                return string.Format("任务{0}运行中,定时设置在每月的{1}日{2}时{3}分,下次执行的时间为{4}", HashId, BackupDay, BackupHour, BackupMinute, _nextRunTime.ToString());
            }
            else
            {
                return string.Format("任务{0}已停止", HashId);
            }
        }

        public string GetHashId()
        {
            return HashId;
        }
    }
    /// <summary>
    /// 每日执行一次的任务
    /// </summary>
    public class PFDayTask : IPFTimeTask
    {
        public string HashId { get; set; }
        /// <summary>
        /// 参数执行的月份
        /// </summary>
        public Action<string, DateTime?, PFDayTask> DoAction { get; set; }
        public Thread TaskThread { get; set; }
        public bool _running = false;

        protected string CDay
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
        /// <summary>
        /// 上次执行的年月日(有成员是为了重设时间是清空)
        /// </summary>
        public string _lastBackupCDay { get; set; }
        /// <summary>
        /// 上次备份的时间,便于增量更新
        /// </summary>
        public DateTime? _lastBackupTime { get; set; }
        /// <summary>
        /// 备份的小时
        /// </summary>
        public int BackupHour { get; set; }
        /// <summary>
        /// 备份的分钟(便于测试)
        /// </summary>
        public int BackupMinute { get; set; }

        /// <summary>
        /// 第一次执行要在此时间之后(默认为类初始化的时间)
        /// 注:这是为了实现18点设置1点执行的话,是立即执行还是第二天开始执行
        /// </summary>
        public DateTime FirstRunTime { get; set; }

        /// <summary>
        /// 下次执行的时间(准备用这个属性代替FirstRunTime)
        /// </summary>
        public DateTime _nextRunTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="doAction">参数1：执行的日期,参数2:上次执行的时间</param>
        /// <param name="backupDay"></param>
        /// <param name="backupHour"></param>
        public PFDayTask(string hashId, Action<string, DateTime?, PFDayTask> doAction, int backupHour, int backupMinute)
        {
            HashId = hashId;
            DoAction = doAction;
            BackupHour = backupHour;
            BackupMinute = backupMinute;

            //这里按一般的习惯,如果当前8时,设置为2时,会等到明天2时才执行
            var now = DateTime.Now;
            var firstRunTime = new DateTime(now.Year, now.Month, now.Day, BackupHour, BackupMinute, 0);
            FirstRunTime = now > firstRunTime ? firstRunTime.AddDays(1) : firstRunTime;
            _nextRunTime = FirstRunTime;
        }
        public void Start()
        {
            if (!_running)
            {
                _running = true;

                TaskThread = new Thread(new ParameterizedThreadStart(StartThread));
                TaskThread.Start();
            }
        }
        public void Stop()
        {
            if (_running)
            {
                _running = false;
                TaskThread.Abort();//之前先释放MessageService的话,进程里仍在使用MessageService会报错,现在改为先释放Thread试试

            }
        }
        public void StartThread(object ps)
        {
            _lastBackupCDay = null;
            _lastBackupTime = null;
            while (_running == true)
            {
                try
                {
                    var cDay = CDay;
                    if (cDay == _lastBackupCDay)//该日已执行
                    {
                        Thread.Sleep(PFTaskHelper.CheckMessageInterval);
                        continue;
                    }
                    var now = DateTime.Now;

                    var backupDay = new DateTime(now.Year, now.Month, now.Day, BackupHour, BackupMinute, 0);
                    if (backupDay > now || FirstRunTime > now)//未到执行的时间
                    {
                        Thread.Sleep(PFTaskHelper.CheckMessageInterval);
                        continue;
                    }

                    _lastBackupCDay = cDay;
                    _nextRunTime = backupDay.AddDays(1);

                    PFDataHelper.WriteLog(string.Format("任务{0}开始执行,日期为:{1}", HashId, cDay));
                    try
                    {
                        DoAction(cDay, _lastBackupTime, this);
                        _lastBackupTime = now;
                        PFDataHelper.WriteLog(string.Format("任务{0}执行完毕,日期为:{1}", HashId, cDay));
                    }
                    catch (Exception e)
                    {
                        PFDataHelper.WriteError(e);
                    }
                    GC.Collect();//一定要有句，否则SendMobileMessage里面的所有List会使内存越来越高
                }
                catch (Exception e)
                {
                    PFDataHelper.WriteError(e);
                }
            }
        }

        public void ResetBackupTime(DateTime time)//如果传入08:30 会自动转为当天
        {
            BackupHour = time.Hour;
            BackupMinute = time.Minute;

            FirstRunTime = time;
            _nextRunTime = FirstRunTime;
            _lastBackupCDay = null;
            _lastBackupTime = null;
        }

        public string GetStatusDescription()
        {
            if (_running)
            {
                return string.Format("任务{0}运行中,定时设置在每天的{1}时{2}分,下次执行的时间为{3}", HashId, BackupHour, BackupMinute, _nextRunTime.ToString());
            }
            else
            {
                return string.Format("任务{0}已停止", HashId);
            }
        }

        public string GetHashId()
        {
            return HashId;
        }
    }

    /// <summary>
    /// 每月执行一次的任务,与PFMonthTask不同的是,满足条件时执行
    /// </summary>
    public class PFMonthCheckTask<TUserData> : IPFTimeTask
    {
        public string HashId { get; set; }
        //public Action DoAction { get; set; }
        /// <summary>
        /// 参数执行的月份
        /// </summary>
        public Action<string, PFMonthCheckTask<TUserData>> DoAction { get; set; }
        /// <summary>
        /// 可以执行的条件
        /// </summary>
        public Func<string, PFMonthCheckTask<TUserData>, bool> CanDoAction { get; set; }
        public Thread TaskThread { get; set; }
        public bool _running = false;

        /// <summary>
        /// 检测的过程可能非常耗时,所以允许自定义
        /// </summary>
        public int CheckMessageInterval { get { return PFDataHelper.ObjectToInt(ConfigurationManager.AppSettings["CheckMessageInterval_" + HashId]) ?? PFTaskHelper.CheckMessageInterval; } }
        protected string Cmonth
        {
            get
            {
                var now = DateTime.Now;
                var year = now.Year;
                var month = now.Month;
                var cmonth = year + "." + month.ToString("00");
                return cmonth;
            }
        }
        /// <summary>
        /// 上次执行的年月
        /// </summary>
        public string _lastBackupCmonth { get; set; }
        /// <summary>
        /// 备份的日期
        /// </summary>
        public int BackupDay { get; set; }
        /// <summary>
        /// 备份的小时
        /// </summary>
        public int BackupHour { get; set; }
        /// <summary>
        /// 备份的分钟(便于测试)
        /// </summary>
        public int BackupMinute { get; set; }

        /// <summary>
        /// 第一次执行要在此时间之后(默认为类初始化的时间)
        /// 注:这是为了实现18点设置1点执行的话,是立即执行还是第二天开始执行
        /// </summary>
        public DateTime FirstRunTime { get; set; }

        /// <summary>
        /// 下次执行的时间(准备用这个属性代替FirstRunTime)
        /// </summary>
        public DateTime _nextRunTime { get; set; }

        /// <summary>
        /// 便于在candoAction里传参数到doAction--benjamin20200115
        /// </summary>
        public TUserData UserData { get; set; }

        [Obsolete]
        public PFMonthCheckTask(string hashId, Action<string, PFMonthCheckTask<TUserData>> doAction, Func<string, PFMonthCheckTask<TUserData>, bool> canDoAction)
        {
            HashId = hashId;
            DoAction = doAction;
            CanDoAction = canDoAction;
        }
        public PFMonthCheckTask(string hashId, Action<string, PFMonthCheckTask<TUserData>> doAction, Func<string, PFMonthCheckTask<TUserData>, bool> canDoAction, int backupDay, int backupHour, int backupMinute)
        {
            HashId = hashId;
            DoAction = doAction;
            CanDoAction = canDoAction;
            BackupDay = backupDay;
            BackupHour = backupHour;
            BackupMinute = backupMinute;

            //这里按一般的习惯,如果当前10日,设置为2日,会等到下个月的2日才执行
            var now = DateTime.Now;
            var firstRunTime = new DateTime(now.Year, now.Month, BackupDay, BackupHour, BackupMinute, 0);
            FirstRunTime = now > firstRunTime ? firstRunTime.AddMonths(1) : firstRunTime;
            _nextRunTime = FirstRunTime;
        }
        public void Start()
        {
            if (!_running)
            {
                _running = true;

                TaskThread = new Thread(new ParameterizedThreadStart(StartThread));
                TaskThread.Start();
            }
        }
        public void Stop()
        {
            if (_running)
            {
                _running = false;
                TaskThread.Abort();//之前先释放MessageService的话,进程里仍在使用MessageService会报错,现在改为先释放Thread试试
                //SaveThread.Abort();
                //MessageService.Dispose();
                //MessageService = null;
            }
        }
        public void StartThread(object ps)
        {
            _lastBackupCmonth = null;
            while (_running == true)
            {
                try
                {
                    var cmonth = Cmonth;
                    if (cmonth == _lastBackupCmonth)//该月已执行
                    {
                        //Thread.Sleep(ProjDataHelper.CheckMessageInterval);
                        Thread.Sleep(CheckMessageInterval);
                        continue;
                    }

                    var now = DateTime.Now;

                    var backupDay = new DateTime(now.Year, now.Month, BackupDay, BackupHour, BackupMinute, 0);
                    if (backupDay > now || FirstRunTime > now)//未到执行的日期
                    {
                        Thread.Sleep(PFTaskHelper.CheckMessageInterval);
                        continue;
                    }

                    if (!CanDoAction(cmonth, this))
                    {
                        Thread.Sleep(CheckMessageInterval);
                        continue;
                    }

                    _lastBackupCmonth = cmonth;
                    _nextRunTime = backupDay.AddMonths(1);

                    PFDataHelper.WriteLog(string.Format("任务{0}开始执行,月份为:{1}", HashId, cmonth));//调用这个方法来写这个值并不好,因为每天都单独一个txt的
                    try
                    {
                        DoAction(cmonth, this);
                        PFDataHelper.WriteLog(string.Format("任务{0}执行完毕,月份为:{1}", HashId, cmonth));
                    }
                    catch (Exception e)
                    {
                        PFDataHelper.WriteError(e);
                    }
                    GC.Collect();//一定要有句，否则SendMobileMessage里面的所有List会使内存越来越高

                }
                catch (Exception e)
                {
                    PFDataHelper.WriteError(e);
                }

            }
        }

        public void ResetBackupTime(DateTime time)
        {
            BackupDay = time.Day;
            BackupHour = time.Hour;
            BackupMinute = time.Minute;

            FirstRunTime = time;
            _nextRunTime = FirstRunTime;
            _lastBackupCmonth = null;
        }

        public string GetStatusDescription()
        {
            if (_running)
            {
                return string.Format("任务{0}运行中,定时设置在每月的{1}日{2}时{3}分,每月条件满足时运行一次,下次执行的时间为{4}", HashId, BackupDay, BackupHour, BackupMinute, _nextRunTime.ToString());
            }
            else
            {
                return string.Format("任务{0}已停止", HashId);
            }
        }

        public string GetHashId()
        {
            return HashId;
        }
    }
    public class PFMonthCheckTask : PFMonthCheckTask<dynamic>
    {
        //public TUserData UserData { get; set; }
        public PFMonthCheckTask(string hashId, Action<string, PFMonthCheckTask<dynamic>> doAction, Func<string, PFMonthCheckTask<dynamic>, bool> canDoAction, int backupDay, int backupHour, int backupMinute) : base(hashId, doAction, canDoAction, backupDay, backupHour, backupMinute)
        {
        }
    }

    /// <summary>
    /// 监听邮件的任务
    /// </summary>
    public class PFListenEmailTask : IPFTask, IDisposable
    {
        public string HashId { get; set; }
        /// <summary>
        /// 条件满足时执行的代码,如果有返回结果,会通过邮件回复
        /// </summary>
        public Action<PFEmail> DoAction { get; set; }
        //public Func<PFEmail, object> DoAction { get; set; }
        /// <summary>
        /// 可以执行的条件
        /// </summary>
        public Func<PFEmail, bool> SubjectMatch { get; set; }
        //public Func<PFEmail, PFListenEmailTask, bool> SubjectMatch { get; set; }
        public Thread TaskThread { get; set; }
        public bool _running = false;

        /// <summary>
        /// 检测的过程可能非常耗时,所以允许自定义
        /// </summary>
        public int CheckMessageInterval { get { return PFDataHelper.ObjectToInt(ConfigurationManager.AppSettings["CheckMessageInterval_" + HashId]) ?? PFTaskHelper.CheckMessageInterval; } }
        public int EmailConnectFailWaitInterval { get { return PFDataHelper.ObjectToInt(ConfigurationManager.AppSettings["EmailConnectFailWaitInterval"]) ?? (1000 * 60 * 5); } }

        private DateTime _lastListenTime;
        private PFEmailManager _emailManager;

        /// <summary>
        /// 由于邮件id大的,邮件的时间不一定是大(测试email时发现的),所以保存一天的已读邮件,和_lastListenTime结合来判断邮件是否已经读过
        /// </summary>
        private List<PFEmail> _emailAtToday=new List<PFEmail>();
        private DateTime _initTime;

        /// <summary>
        /// 只监听一次,常用于emailMq的生产者监听回复等情况
        /// </summary>
        private bool _onlyListenOnce = false;
        public PFListenEmailTask(string hashId,
            PFEmailManager emailManager,
             Action<PFEmail> doAction,
             //Func<PFEmail, object> doAction,
             Func<PFEmail, bool> subjectMatch,
            // Func<PFEmail, PFListenEmailTask, bool> subjectMatch
            bool onlyListenOnce=false
            )
        {
            HashId = hashId;
            DoAction = doAction;
            SubjectMatch = subjectMatch;

            _initTime=_lastListenTime = DateTime.Now;
            _emailManager = emailManager;

            _onlyListenOnce = onlyListenOnce;
            //一开始就读邮件的话,浪费很多性能的,还是不要了
            //ReadEmailAtYesterday();
        }

        public void Start()
        {
            if (!_running)
            {
                _running = true;

                TaskThread = new Thread(new ParameterizedThreadStart(StartThread));
                TaskThread.Start();
            }
        }
        public void Stop()
        {
            if (_running)
            {
                _running = false;
                TaskThread.Abort();//之前先释放MessageService的话,进程里仍在使用MessageService会报错,现在改为先释放Thread试试
            }
        }

        public void Dispose()
        {
            if (_running)
            {
                try
                {
                    _running = false;
                    TaskThread.Abort();
                }
                catch (Exception) { }
            }
        }
        /// <summary>
        /// 自然停止.让线程执行完一个循环再停止(防止Abort报错)
        /// </summary>
        public void NaturalStop()
        {
            _running = false;
        }
        #region 新方式,先读新邮件
        //public void StartThread(object ps)
        //{
        //    //PFEmail email = null;
        //    while (_running == true)
        //    {
        //        try
        //        {
        //            //try
        //            //{
        //            //    //为了检查26服务器上监听邮件失效的问题--benjamin 
        //            //    PFDataHelper.WriteLocalTxt(string.Format("监听邮件的while进行中，时间:{0}", DateTime.Now.ToString()), "PFListenEmailTask_while.txt");
        //            //}
        //            //catch (Exception) { }

        //            //if (PFDataHelper.IsDebug)
        //            //{
        //            //    PFDataHelper.WriteLog("1");
        //            //}
        //            _emailManager.Connect_Click();
        //            if (!_emailManager.IsConnect())//Connect_Click的new TcpClient(_hostName, 110) 是有可能连接失败的--benjamin20200318
        //            {
        //                Thread.Sleep(1000 * 60 * 5);//如果连接失败,等5分钟
        //                continue;
        //            }

        //            try
        //            {
        //                //为了检查26服务器上监听邮件失效的问题--benjamin todo
        //                PFDataHelper.WriteLocalTxt(string.Format("监听邮件的while的_emailManager.Connect_Click()执行成功，时间:{0}，邮件数：{1}", DateTime.Now.ToString(), _emailManager.MessageCount), "PFListenEmailTask_while_Connect_Click.txt");
        //            }
        //            catch (Exception) { }

        //            DateTime? newestMailTime = null;//记录最新邮件的时间

        //            List<PFEmail> newEmailList = new List<PFEmail>();
        //            for (int i = _emailManager.MessageCount; i > 0; i--)
        //            {
        //                var email = _emailManager.Retrieve_Click(i);
        //                if (email.Date <= _lastListenTime)//旧邮件
        //                {
        //                    break;
        //                }
        //                if (email==null) {
        //                    newEmailList.ForEach(a => a.Dispose());
        //                    newEmailList.Clear();
        //                    throw new Exception("读取新邮件时出错");//只要一个新邮件读不到,就重新读,这样比较保险,也不会重复消费
        //                }
        //                newEmailList.Add(email);
        //            }

        //            if (newEmailList.Any())
        //            {
        //                continue;//没有新邮件
        //            }

        //            foreach (var email in newEmailList)
        //            {
        //                try
        //                {
        //                    //为了检查26服务器上监听邮件失效的问题--benjamin todo
        //                    PFDataHelper.WriteLocalTxt(string.Format("监听邮件的while的_emailManager.Retrieve_Click()执行成功，时间:{0}，邮件数：{1}", DateTime.Now.ToString(), _emailManager.MessageCount), "PFListenEmailTask_while_Retrieve_Click.txt");
        //                }
        //                catch (Exception) { }


        //                var b = false;
        //                try
        //                {
        //                    b = SubjectMatch(email);
        //                }
        //                catch (Exception e)
        //                {
        //                    newestMailTime = null;
        //                    PFDataHelper.WriteError(new Exception(string.Format("SubjectMatch报错{0}", e)));
        //                }
        //                if (b)
        //                //if (SubjectMatch(email,this))
        //                {
        //                    //if (PFDataHelper.IsDebug)
        //                    //{
        //                    //    PFDataHelper.WriteLog("8");
        //                    //}
        //                    try
        //                    {
        //                        _emailManager.Disconnect_Click();

        //                        DoAction(email);
        //                        //var result=DoAction(email);
        //                        //if (result != null)//回复邮件
        //                        //{
        //                        //    //PFDataHelper.SendEmail(PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd, PFDataHelper.SysEmailHostName,
        //                        //    //    new string[] { email.From }, "PFListenEmailTask_AutoReply_" + email.Subject, JsonConvert.SerializeObject(result));
        //                        //    PFDataHelper.SendEmail(PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd, PFDataHelper.SysEmailHostName,
        //                        //        new string[] { email.From }, HashId+"_Reply_" + email.Subject, JsonConvert.SerializeObject(result));
        //                        //}

        //                        //if (PFDataHelper.IsDebug)
        //                        //{
        //                        //    PFDataHelper.WriteLog("9");
        //                        //}
        //                        //PFDataHelper.WriteLog(string.Format("任务{0}执行完毕,月份为:{1}", HashId, cmonth));
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        PFDataHelper.WriteError(e);
        //                    }
        //                    email.Dispose();
        //                    ////_emailManager.DeleteEmail(email);
        //                    //_lastListenTime = email.Date ?? DateTime.Now;
        //                    break;
        //                }
        //                //if (PFDataHelper.IsDebug)
        //                //{
        //                //    PFDataHelper.WriteLog("10");
        //                //}
        //                email.Dispose();
        //            }
        //            //if (PFDataHelper.IsDebug)
        //            //{
        //            //    PFDataHelper.WriteLog("11");
        //            //}
        //            //不管有没有匹配到邮件,下次都应该找时间更新的邮件了
        //            if (newestMailTime != null && newestMailTime > _lastListenTime) { _lastListenTime = newestMailTime.Value; }

        //            //if (PFDataHelper.IsDebug)
        //            //{
        //            //    PFDataHelper.WriteLog("12");
        //            //}
        //            _emailManager.Disconnect_Click();
        //            //if (PFDataHelper.IsDebug)
        //            //{
        //            //    PFDataHelper.WriteLog("13");
        //            //}
        //            Thread.Sleep(CheckMessageInterval);
        //            //if (PFDataHelper.IsDebug)
        //            //{
        //            //    PFDataHelper.WriteLog("14");
        //            //}
        //            GC.Collect();//一定要有句，否则SendMobileMessage里面的所有List会使内存越来越高  

        //            //if (PFDataHelper.IsDebug)
        //            //{
        //            //    PFDataHelper.WriteLog("15");
        //            //}
        //        }
        //        catch (Exception e)
        //        {
        //            PFDataHelper.WriteError(e);
        //            Thread.Sleep(CheckMessageInterval);
        //        }
        //    }
        //} 
        #endregion


        private void RemoveEmailAtYesterday()
        {
            _emailAtToday.RemoveAll(a => a.Date == null || a.Date < _lastListenTime.AddDays(-2));//保存2天的邮件比较保险
        }
        ///// <summary>
        ///// 运行时要读昨天的邮件(之后不匹配)
        ///// </summary>
        //private void ReadEmailAtYesterday()
        //{
        //    _emailManager.Connect_Click();
        //    for (int i = _emailManager.MessageCount; i > 0; i--)
        //    {
        //        var email = _emailManager.Retrieve_Click(i);
        //        if (email != null && email.Date != null )
        //        {
        //            if(email.Date >= _lastListenTime.AddDays(-1))
        //            {
        //                _emailAtToday.Add(email);
        //            }else
        //            {
        //                break;
        //            }
        //        }else
        //        {
        //            throw new Exception("监听邮件失败,ReadEmailAtYesterday报错");
        //        }
        //    }
        //    _emailManager.Disconnect_Click();
        //}
        #region 存在漏洞,有时MessageCount比较大时,邮件时间反而小--benjamin20200413        
        public void StartThread(object ps)
        {
            PFEmail email = null;
            while (_running == true)
            {
                try
                {
                    //try
                    //{
                    //    //为了检查26服务器上监听邮件失效的问题--benjamin 
                    //    PFDataHelper.WriteLocalTxt(string.Format("监听邮件的while进行中，时间:{0}", DateTime.Now.ToString()), "PFListenEmailTask_while.txt");
                    //}
                    //catch (Exception) { }

                    //if (PFDataHelper.IsDebug)
                    //{
                    //    PFDataHelper.WriteLog("1");
                    //}
                    _emailManager.Connect_Click();
                    if (!_emailManager.IsConnect())//Connect_Click的new TcpClient(_hostName, 110) 是有可能连接失败的--benjamin20200318
                    {
                        Thread.Sleep(1000 * 60 * 5);//如果连接失败,等5分钟
                        continue;
                    }

                    try
                    {
                        //为了检查26服务器上监听邮件失效的问题--benjamin todo
                        PFDataHelper.WriteLocalTxt(string.Format("监听邮件的while的_emailManager.Connect_Click()执行成功，时间:{0}，邮件数：{1}", DateTime.Now.ToString(), _emailManager.MessageCount), "PFListenEmailTask_while_Connect_Click.txt");
                    }
                    catch (Exception) { }

                    DateTime? newestMailTime = null;//记录最新邮件的时间
                                                    //bool hasMatch = false;
                                                    //PFDataHelper.WriteLog("MessageCount:" + _emailManager.MessageCount);
                                                    //if (PFDataHelper.IsDebug)
                                                    //{
                                                    //    PFDataHelper.WriteLog("2");
                                                    //    //PFDataHelper.WriteLocalTxt(string.Format("时间{0}\r\nMessageCount:{1}",DateTime.Now, _emailManager.MessageCount), "listenEmail_Connect_Click.txt");
                                                    //}
                    bool isAnyReceiveFail = false;
                    for (int i = _emailManager.MessageCount; i > 0; i--)
                    {

                        //if (PFDataHelper.IsDebug)
                        //{
                        //    PFDataHelper.WriteLog("3");
                        //}
                        email = _emailManager.Retrieve_Click(i);//System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown. --benjamin 

                        try
                        {
                            //为了检查26服务器上监听邮件失效的问题--benjamin todo
                            PFDataHelper.WriteLocalTxt(string.Format("监听邮件的while的_emailManager.Retrieve_Click()执行成功，时间:{0}，邮件数：{1}", DateTime.Now.ToString(), _emailManager.MessageCount), "PFListenEmailTask_while_Retrieve_Click.txt");
                        }
                        catch (Exception) { }

                        //if (PFDataHelper.IsDebug)
                        //{
                        //    PFDataHelper.WriteLog("4");
                        //}
                        if (email == null)
                        {
                            //为了解决这个漏洞:
                            //当第一个邮件能收到,那newestMailTime变成它的时间,但后面的都接收失败了,那么下次就会永远读不到那个失败的邮件
                            newestMailTime = null;
                            isAnyReceiveFail = true;
                            continue;
                        }//这里不用break是防止id有中间非连续的情况--benjamin20190929

                        if (i == _emailManager.MessageCount)
                        {
                            newestMailTime = email.Date;
                        }

                        //if (email.Date <= _lastListenTime)
                        //if (email.Date != null && _lastListenTime != null &&
                        //    email.Date.Value.AddMinutes(2) < _lastListenTime) //这里用等号似乎会有问题,有时生产者发的邮件时间反而大于消费者回复的邮件的时间(可能邮件上的时间是根据发送端的电脑时间来的,有误差),可能有更好的方法可以统一时间?--benjamin todo
                        //为解决邮件id大但邮件时间反而小的问题
                        if(email.Date != null)
                        {
                            if (email.Date <= _lastListenTime.AddDays(-1)) //昨天的邮件直接不读
                            {
                                email.Dispose();
                                break;
                            }
                            else if (email.Date<= _lastListenTime)//今天的邮件
                            {
                                if(_emailAtToday.Any(a => a.Equals(email)))//今天的邮件要比对
                                {
                                    email.Dispose();
                                    //continue;
                                    break;
                                }
                                else
                                {
                                    _emailAtToday.Add(email);
                                    if (email.Date < _initTime.AddMinutes(-1))
                                    {
                                        email.Dispose();
                                        break;
                                    }
                                }
                            }else//新邮件(没有这句的话,新邮件会读两次--benjamin20200414
                            {
                                if(!_emailAtToday.Any(a => a.Equals(email)))
                                {
                                    _emailAtToday.Add(email);
                                }
                            }
                        }else
                        {
                            isAnyReceiveFail = true;
                        }
                        //if (PFDataHelper.IsDebug)
                        //{
                        //    PFDataHelper.WriteLog("7");
                        //}

                        var b = false;
                        try
                        {
                            b = SubjectMatch(email);
                        }
                        catch (Exception e)
                        {
                            newestMailTime = null;
                            PFDataHelper.WriteError(new Exception(string.Format("SubjectMatch报错{0}", e)));
                        }
                        if (b)
                        {
                            try
                            {
                                _emailManager.Disconnect_Click();

                                DoAction(email);
                              
                            }
                            catch (Exception e)
                            {
                                PFDataHelper.WriteError(e);
                            }
                            email.Dispose();
                            ////_emailManager.DeleteEmail(email);
                            //_lastListenTime = email.Date ?? DateTime.Now;
                            if (_onlyListenOnce)
                            {
                                _running = false;
                                return;
                            }
                            break;
                        }
 
                        email.Dispose();
                    }
                    //if (PFDataHelper.IsDebug)
                    //{
                    //    PFDataHelper.WriteLog("11");
                    //}
                    //不管有没有匹配到邮件,下次都应该找时间更新的邮件了
                    if (newestMailTime != null && newestMailTime > _lastListenTime&&(!isAnyReceiveFail)) {
                        _lastListenTime = newestMailTime.Value;
                    }

                    //if (PFDataHelper.IsDebug)
                    //{
                    //    PFDataHelper.WriteLog("12");
                    //}
                    _emailManager.Disconnect_Click();
                    //if (PFDataHelper.IsDebug)
                    //{
                    //    PFDataHelper.WriteLog("13");
                    //}
                    Thread.Sleep(CheckMessageInterval);
                    //if (PFDataHelper.IsDebug)
                    //{
                    //    PFDataHelper.WriteLog("14");
                    //}
                    GC.Collect();//一定要有句，否则SendMobileMessage里面的所有List会使内存越来越高  

                    //if (PFDataHelper.IsDebug)
                    //{
                    //    PFDataHelper.WriteLog("15");
                    //}

                    RemoveEmailAtYesterday();
                }
                catch (Exception e)
                {
                    PFDataHelper.WriteError(e);
                    Thread.Sleep(CheckMessageInterval);
                }
            }
        }
        #endregion
    }
}
