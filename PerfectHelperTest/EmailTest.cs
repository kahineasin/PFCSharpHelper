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


namespace PerfectHelperTest
{
    [TestClass]
    public class  EmailTest
    {
        public static string UserEmailHostName = "smtp.perfect99.com";
        public static string UserEmailUserName = "wxj@perfect99.com";
        public static string UserEmailPwd = "shi3KjkE48QZ3SPA";
        [TestMethod]
        public void TestSendEmail()
        {
            PFDataHelper.SendEmail(UserEmailUserName, UserEmailPwd, UserEmailHostName,
                  new string[] { PFDataHelper.SysEmailUserName },
                  "aaa", "aaaaa");
            return;

            string emailTitle = "test_PFEmailMq_producer_会员资料表";//中文有问题--benjamin todo
            //string emailBody = "会员资料表";
            //string emailBody = "hyzlTable_aabb_中国人";
            string emailBody = @"
<p>2019.01月结数据备份情况:<p>
<ol>
<li>aaa</li>
<li>bbb</li>
</ol>
";
            //string producerEmailTitle = "PFEmailMq_producer_" + "hyzl";
            //消费方(使用系统邮箱)
            PFEmail result = null;
            bool success = false;
            var consumerTask = new PFListenEmailTask("TestForceUpdateHyzl",
            new PFEmailManager(PFDataHelper.SysEmailHostName, PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd),
            email =>
            {
                result = email;
                success = true;
            },
            (email//, task
            ) =>
            {
                return email.Subject == emailTitle;
            });
            consumerTask.Start();

            //生产方(使用User邮箱,也可以用系统邮箱吧)
            var rt = new Task(() =>
            {
                PFDataHelper.SendEmail(UserEmailUserName, UserEmailPwd, UserEmailHostName,
               new string[] { PFDataHelper.SysEmailUserName },
               emailTitle, emailBody);
            });
            rt.Start();
            rt.Wait();
            while (!success)
            {
                Thread.Sleep(2000);
            }
            Assert.IsTrue(emailTitle == result.Subject);
        }

        //[TestMethod]
        //public void TestSendEmailAsync() {
        //    string producerEmailTitle = "PFEmailMq_producer_"+ "会员资料表";//中文有问题--benjamin todo
        //    //string producerEmailTitle = "PFEmailMq_producer_" + "hyzl";
        //    //消费方(使用系统邮箱)
        //    string result = "";
        //    bool success = false;
        //    var consumerTask = new PFListenEmailTask("TestForceUpdateHyzl",
        //    new PFEmailManager(PFDataHelper.SysEmailHostName, PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd),
        //    email =>
        //    {
        //        result = "{success:true}";

        //        //消费方回复邮件
        //        PFDataHelper.SendEmail(PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd, PFDataHelper.SysEmailHostName,
        //            new string[] { UserEmailUserName }, "PFEmailMq_consumer_" + email.Subject,
        //            result);
        //    },
        //    (email//, task
        //    ) =>
        //    {
        //       //消费方监听生产方邮件
        //       //return email.Subject != null && email.Subject.IndexOf("TestForceUpdateHyzl_") == 0;//这里不要用>-1,否则可能把自动回复的邮件也当作是了
        //       return email.Subject== producerEmailTitle;
        //    });
        //    consumerTask.Start();

        //    //生产方(使用User邮箱,也可以用系统邮箱吧)
        //    var rt=PFDataHelper.SendEmailAsync(UserEmailUserName, UserEmailPwd, UserEmailHostName, 
        //        new string[] { PFDataHelper.SysEmailUserName },
        //        producerEmailTitle, "会员资料表");
        //    rt.Wait();
        //    var resultTitle = rt.Result.Subject;
        //    Assert.IsTrue(resultTitle== "PFEmailMq_consumer_" + producerEmailTitle);
        //}
        private String GetDateString()
        {
            return PFDataHelper.ObjectToDateString(DateTime.Now, PFDataHelper.DateFormat);
        }
        #region TestPFEmailMq
        /// <summary>
        /// 这个方法有时报错,原因未明
        /// </summary>
        [TestMethod]
        public void TestPFEmailMq()
        {

            var resMsg = "";

            var p = new PagingParameters();
            p["backupDatabase"] = "bonus";

            TestPFEmailMqConsumer tmpObj = new TestPFEmailMqConsumer();
            PFMqHelper.BuildConsumer(tmpObj);
            Thread.Sleep(2000);//不延迟的话,后面太快了,前面还没开始监听

            var st = DateTime.Now;
            var message = JsonConvert.SerializeObject(p);
            var t = PFMqHelper.BuildProducer(message,
                    new TestPFEmailMqProducer());
            t.Wait();
            var et = DateTime.Now;
            resMsg = t.Result.Body;

            var usedTime = PFDataHelper.GetTimeSpan(et - st);//平均6秒

            Assert.IsTrue(resMsg == JsonConvert.SerializeObject("接收到信息:" + message));
            //Assert.IsTrue(true);
        }
        /// <summary>
        /// ok
        /// </summary>
        [TestMethod]
        public void TestPFEmailMq2()
        {
            #region 消费者
            TestMonthDataCompareCntConsumer consumer = new TestMonthDataCompareCntConsumer();
            PFMqHelper.BuildConsumer(consumer);
            Thread.Sleep(2000);//不延迟的话,后面太快了,前面还没开始监听
            #endregion

            var cacheList = TestMonthDataCompareCntProducer.Product("bonus");

            Assert.IsTrue(cacheList != null && cacheList.Any());
            //Assert.IsTrue(true);
        }

        /// <summary>
        /// 有问题
        /// 改PFListenEmailTask这句后没问题了  email.Date.Value.AddMinutes(2) 
        /// </summary>
        [TestMethod]
        public void TestPFEmailMq3()
        {
            var cacheList = MonthDataCompareCntProducer.Product("bonus");

            Assert.IsTrue(cacheList != null && cacheList.Any());
            //Assert.IsTrue(true);
        }
        #endregion

        [TestMethod]
        public void TestEncode()
        {
            //=?utf-8?B?UEZFbWFpbE1xX3Byb2R1Y2VyX+S8muWRmOi1hOaWmeihqA==?=
            string s1 = "UEZFbWFpbE1xX3Byb2R1Y2VyX+S8muWRmOi1hOaWmeihqA==";
            var v1 = PFDataHelper.Decode(s1, PFEncodeType.Base64, PFEncodeType.UTF8);
            Assert.IsTrue(v1 == "PFEmailMq_producer_会员资料表");
            var r1 = PFDataHelper.Encode(v1, PFEncodeType.Base64, PFEncodeType.UTF8);
            Assert.IsTrue(r1 == s1);

            //=?gb18030?B?UEZFbWFpbE1xX3Byb2R1Y2VyX2Zyb21RUU1haWxf?=
            string s2 = "UEZFbWFpbE1xX3Byb2R1Y2VyX2Zyb21RUU1haWxf";
            var v2 = PFDataHelper.Decode(s2, PFEncodeType.Bit8, PFEncodeType.GB18030);
            Assert.IsTrue(v2 == "PFEmailMq_producer_fromQQMail_");
            var r2 = PFDataHelper.Encode(v2, PFEncodeType.Bit8, PFEncodeType.GB18030);
            Assert.IsTrue(r2 == s2);

            //qqmail body
            string s3 = "MjAxOS4wMdTCveHK/b7dsbi33cfpv/Y6DQoNCiANCiANCmFhYQ0KIA0KYmJi";
            var v3 = PFDataHelper.Decode(s3, PFEncodeType.Bit8, PFEncodeType.GB18030);
            Assert.IsTrue(v2 == "PFEmailMq_producer_fromQQMail_");
            //var r2 = PFDataHelper.Encode(v2, PFEncodeType.Bit8, PFEncodeType.GB18030);
            //Assert.IsTrue(r2 == s2);

            //126mail Subject: =?GBK?Q?PFEmailMq=5Fproducer?=
            //=?GBK?Q?=5Ffrom126Mail=5F=BB=E1=D4=B1=D7=CA=C1=CF=B1=ED?=
            var s4 = "PFEmailMq=5Fproducer";
            var v4 = PFDataHelper.Decode(s4, PFEncodeType.QuotedPrintable, PFEncodeType.GBK);
            Assert.IsTrue(v4 == "PFEmailMq_producer");
            var s5 = "=5Ffrom126Mail=5F=BB=E1=D4=B1=D7=CA=C1=CF=B1=ED";
            var v5 = PFDataHelper.Decode(s5, PFEncodeType.QuotedPrintable, PFEncodeType.GBK);
            Assert.IsTrue(v5 == "_from126Mail_会员资料表");

            //aliyunEmail
            //PGRpdiBjbGFzcz0iX19hbGl5dW5fZW1haWxfYm9keV9ibG9jayI+PGRpdiAgc3R5bGU9ImxpbmUt
            var s6 = "PGRpdiBjbGFzcz0iX19hbGl5dW5fZW1haWxfYm9keV9ibG9jayI+PGRpdiAgc3R5bGU9ImxpbmUt";
            var v6 = PFDataHelper.Decode(s6, PFEncodeType.Base64, PFEncodeType.UTF8);
            Assert.IsTrue(v6 == "<div class=\"__aliyun_email_body_block\"><div  style=\"line-");

            var s7 = "<html xmlns:v=3D\"urn:schemas-microsoft-com:vml\" ";//只要没最后的等号就不报错
            var v7 = PFDataHelper.Decode(s7, PFEncodeType.QuotedPrintable, PFEncodeType.GB2312);
            //正确是  <html xmlns:v="urn:schemas-microsoft-com:vml" %
            //参考http://web.chacuo.net/charsetquotedprintable/


            //=?GB2312?B?16q3ojogRnc6IMfryr4=?=
            string s8 = "16q3ojogRnc6IMfryr4=";
            var v8 = PFDataHelper.Decode(s8, PFEncodeType.Bit8, PFEncodeType.GB2312);
            Assert.IsTrue(v8 == "转发: Fw: 请示");
            var r8 = PFDataHelper.Encode(v8, PFEncodeType.Bit8, PFEncodeType.GB2312);
            Assert.IsTrue(r8 == s8);
        }
        [TestMethod]
        public void TestEmail()
        {
            PFEmail email = null;
            string subject = null;
            string body = null;



            //var message = JsonConvert.DeserializeObject<List<string>>(PFDataHelper.ReadLocalTxt("mailFujianJson.txt"));
            //email = new PFEmail(message);
            //body = email.Body;
            ////return;
            //Assert.IsTrue(body == "各们领导、同事：\r\n\r\n       附件为油葱架构文档V1.5版，请查收，谢谢\r\n\r\n \r\n\r\n \r\n\r\n陈辉\r\n\r\n");

            List<KeyValuePair<string, KeyValuePair<string, string>>> fileNames = new List<KeyValuePair<string, KeyValuePair<string, string>>> {
                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "126Email.txt" , new KeyValuePair<string, string>(
                        "PFEmailMq_producer_from126Mail_会员资料表" ,
                        "2019.01月结数据备份情况:\n\naaa\nbbb\n\n" ) ),
                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "aliyunEmail.txt" , new KeyValuePair<string, string>(
                        "[SPAM]PFEmailMq_producer_fromSellGirl_会员资料表" ,
                        "\n<p>2019.01月结数据备份情况:<p>\n<ol>\n<li>aaa</li>\n<li>bbb</li>\n</ol>" )),
                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "perfect99Email.txt" , new KeyValuePair<string, string>(
                        "PFEmailMq_producer_会员资料表" ,
                        "\r\n<p>2019.01月结数据备份情况:<p>\r\n<ol>\r\n<li>aaa</li>\r\n<li>bbb</li>\r\n</ol>\r\n")),
                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "perfect99Email_含附件.txt" , new KeyValuePair<string, string>(
                        "油葱架构文档V1.5版",
                        "各们领导、同事：\r\n\r\n       附件为油葱架构文档V1.5版，请查收，谢谢\r\n\r\n \r\n\r\n \r\n\r\n陈辉\r\n\r\n" )),
#region qqEmail
		                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "qqEmail.txt" ,new KeyValuePair<string, string>(
                        "PFEmailMq_producer_fromQQMail_会员资料表" ,
                        "2019.01月结数据备份情况:\r\n\r\n \r\n \r\naaa\r\n \r\nbbb" )), 
	#endregion qqEmail
#region perfect99Email2
		                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "perfect99Email2.txt" ,new KeyValuePair<string, string>(
                        "转发: Fw: 请示" ,
                        @"



wxw@perfect99.com
 
发件人： 完美（中国）有限公司 信息中心
发送时间： 2020-04-13 16:15
收件人： 王现伟
抄送： 余凤瑜
主题： Fw: 请示
王现伟，您好！ 
 
　　
 
 
= = = = = = 下面是转发邮件 = = = = = = =

原邮件发件人名字：胡文祥
原邮件发件人地址：huwenxiang@perfect99.com
原邮件收件人名字：王天庆总监; 李观棉
原邮件收件人地址：kcgee@perfect99.com; Info@perfect99.com
原邮件抄送人名字：
原邮件抄送人地址：

信息中心，您好！
我部2020年2月19日已取消购买宜悦空气净化器优惠购（AP60025)获得800元奖励的活动。后因服务中心库存较多，经过请示领导特将服务中心后台报单时间调整至3月31日。请继续提取空机（AP60025）2月及3月的实际销售名单给到财务以发放相应补贴：

请同步发一份销售名单给到此邮箱号（huwenxiang@perfect99.com）谢谢！    附：补报返现补贴的请示


胡文祥（推广专员）
市场中心 -策划部 
TEL:0760-88701828-88225/13924995602
E-Mail ：huwenxiang@perfect99.com

完美（中国）有限公司  Perfect (China) Co., Ltd.
地址：广东省中山市石岐区东明北路(民营科技园),邮编：528402
Add ：Dongming North Road, Shiqi District, Zhongshan
City, Guangdong Province, PRC （528400


= = = = = = = = = = = = = = = = = = = = 
　　　　　　　　致
礼！ 
                          完美（中国）有限公司 信息中心
                          Info@perfect99.com
                          2020-04-13
　　　　　　　　　　　　　　
" )), 
	#endregion perfect99Email2

#region perfect99Email3
		                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "perfect99Email3.txt" ,new KeyValuePair<string, string>(
                        "Fw: 关联关系申报与处理规定签订事宜Message-ID: <202004131617413024898@perfect99.com>Organization: =?gb2312?B?zerDwKOo1tC5+qOp09DP3rmry74=?=X-mailer: Foxmail 6, 15, 201, 20 [cn]" ,
                        @"陈超，您好！ 

　　


= = = = = = 下面是转发邮件 = = = = = = =

原邮件发件人名字：人力资源中心
原邮件发件人地址：rlzy@perfect99.com

原邮件收件人名字：财务中心; 采购中心; 法务外事中心; 供应商改造项目组; 基建办; 集团战略法务部; 监察中心; 健康公司工作小组; 企业门户项目组; 人力资源中心; 市场中心; 完美大学; 信息中心; 研发中心; 业务中心; 制造中心; 总裁办; 董事长助理徐奕新高级经理; 董事长秘书陈嘉丽; 许生秘书许丽萍; 董事会办公室江晓君; 董事长助理秘书-柯冬燕; 董事长秘书张子卉; 副董事长办-赖丽霞; 副董事长秘书-黄学军; 常务副总裁秘书黄绮柔; 总裁办黄思苑; 分子机构管理室; 扬州人力资源部
原邮件收件人地址：finance.gr@perfect99.com; cgzx@perfect99.com; fwwszx@perfect99.com; SZH-04@perfect99.com; JJB@perfect99.com; flzx@perfect99.com; spv@perfect99.com; djkxz@perfect99.com; SZH-11@perfect99.com; rlzy@perfect99.com; sczx@perfect99.com; pxzx@perfect99.com; Info@perfect99.com; yfzx@perfect99.com; ywzx@perfect99.com; cmc@perfect99.com; zcb@perfect99.com; eason@perfect99.com; kellychan@perfect99.com; sheenahooy@perfect99.com; jiangxiaojun328@perfect99.com; kdy@perfect99.com; zzh@perfect99.com; llx@perfect99.com; 1113@perfect99.com; huangqirou@perfect99.com; Huangsy@perfect99.com; fzjggl@perfect99.com; yzhr@perfect99.com
原邮件抄送人名字：
原邮件抄送人地址：

各中心/部门：
     您好！
     为了进一步完善个人信息申报体系，有效管控公司与个人的利益冲突，公司制定了《关联关系申报与处理规定》及相应申报表，现请各位员工按以下要求完成相关申报：
   
      一、申报对象：文员级及以上级别员工（名单单独发各中心秘书）。
   
     二、提交要求：
     A、总部、完美广东员工需在4月16日下午16:00前，将《关联关系申报与处理规定》、《关联关系申报表》签字版原件以中心为单位提交至人力资源中心王成 
瑶处（IP:88390）。
     B、分公司员工需在4月16日下午16:00前将签字版原件寄至人力资源中心黄燕婵收（请勿与其他资料混在一起）。
     C、扬州完美人员由扬州完美人力资源部负责收集汇总与存档，其中经理级及以上员工的《关联关系申报与处理规定》、《关联关系申报表》扫描件，由扬州人力资源部扫描后发至：rsglz@perfect99.com。
   
     三、填写要求：
     A、申报内容可以打印，必须按“申报要求”将相关内容填写完整。
     B、规定签名处及申报表的签名处必须亲笔签名，且正楷字清晰签署。
     C、规定与申报表可双面打印，两面均需按要求签名。


    以上请知悉配合，如有疑问，请与谭富豪（88389）/王成瑶（88390）联系。谢谢！

2020-04-13



祝工作顺利！

       致

  礼！
 
  人力资源中心  HR CENTER
 Tel:88701828-88392 方玉英
 Fax:88701828-88370





= = = = = = = = = = = = = = = = = = = = 
　　　　　　　　致
礼！ 
                          完美（中国）有限公司 信息中心
                          Info@perfect99.com
                          2020-04-13
　　　　　　　　　　　　　　
" )), 
	#endregion perfect99Email3

#region perfect99Email4
		                new KeyValuePair<string,KeyValuePair<string, string>>(
                    "perfect99Email4.txt" ,new KeyValuePair<string, string>(
                        "新电子商务系统权限申请_0603015" ,
                        @"<p>新电子商务系统权限申请</p>
<p>申请人工号：0603015</p>
" ))//, 
	#endregion perfect99Email4
            };
            //fileNames = fileNames.Where(a => a.Key == "perfect99Email4.txt").ToList();
            foreach (var i in fileNames)
            {
                //email = new PFEmail(PFDataHelper.ReadLocalTxt(i).Split(new char[] { '\r', '\n' }));
                try
                {
                    email = new PFEmail(PFDataHelper.ReadLocalTxt(i.Key).Split(new string[] { "\r\n" }, StringSplitOptions.None));
                    subject = email.Subject;
                    //PFDataHelper.WriteLog(subject);
                    body = email.Body;
                }
                catch (Exception e)
                {
                    throw new Exception(i.Key + e.ToString());
                }
                if (subject != i.Value.Key || body != i.Value.Value) {
                    throw new Exception(i.Key);
                }
                //Assert.IsTrue(body == i.Value);
            }

            var aa = "aa";
        }
    }
}
