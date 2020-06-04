using Perfect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectHelperTestUI
{
    public partial class TestEmailForm : Form
    {
        private PFEmailManager _emailManager;
        public TestEmailForm()
        {
            InitializeComponent();
            _emailManager = new PFEmailManager(PFDataHelper.SysEmailHostName, PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd);
            emailDateBox.CustomFormat = PFDataHelper.DateFormat;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
            });
            _emailManager.Connect_Click(a =>
            {
                connectStatusTBox.Text = a;
            });
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _emailManager.Disconnect_Click();
        }

        private void emailCntBtn_Click(object sender, EventArgs e)
        {
            emailCntNBox.Value = _emailManager.GetStat();
        }

        private void receiveBtn_Click(object sender, EventArgs e)
        {
            var email = _emailManager.Retrieve_Click(int.Parse(emailCntNBox.Value.ToString()));
            if (email == null)
            {
                subjectTBox.Text = "找不到邮件";
            }else
            {
                try
                {
                    subjectTBox.Text = email.Subject;
                    emailDateBox.Value = email.Date ?? DateTime.MinValue;
                    emailDateTBox.Text = (email.Date ?? DateTime.MinValue).ToString(PFDataHelper.DateFormat);
                    bodyTBox.Text = email.Body;
                }catch(Exception exception)
                {
                    errorTBox.Text = exception.ToString();
                }
            }
        }

        private void disConnectBtn_Click(object sender, EventArgs e)
        {
            _emailManager.Disconnect_Click();
            connectStatusTBox.Text = _emailManager.DisConnectStatus;
        }

        private IPFTask _emailListenTask;
        private void testListerBtn_Click(object sender, EventArgs e)
        {
            if (_emailListenTask == null)
            {
                _emailListenTask = new PFListenEmailTask("PFTcBackupChecker",
                new PFEmailManager(PFDataHelper.SysEmailHostName, PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd),
                email =>
                {
                    // 采用Invoke形式进行操作
                    this.Invoke(new MethodInvoker(() =>
                    {
                        bodyTBox.Text += "监听到邮件 TestListenEmail\r\n";
                    }));
                },
                email =>
                {
                    return email.Subject != null && email.Subject.IndexOf("TestListenEmail") == 0;//这里不要用>-1,否则可能把自动回复的邮件也当作是了
                });
            }
            _emailListenTask.Start();
        }

        private void emailDateTBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void newEmailBtn_Click(object sender, EventArgs e)
        {
            var s = _emailManager.GetList();
            bodyTBox.Text = s;
        }

        private async void checkEachEmailBtn_Click(object sender, EventArgs e)
        {
            //var email = _emailManager.Retrieve_Click(int.Parse(emailCntNBox.Value.ToString()));
            //if (email == null)
            //{
            //    Task.Run(() => {
            //        this.Invoke(new MethodInvoker(() =>
            //        {
            //            subjectTBox.Text = "找不到邮件";
            //        }));
            //    });
            //}
            //else
            //{
            //    Task.Run(() => {
            //        this.Invoke(new MethodInvoker(() =>
            //        {
            //            subjectTBox.Text = email.Subject;
            //            emailDateBox.Value = email.Date ?? DateTime.MinValue;
            //            emailDateTBox.Text = (email.Date ?? DateTime.MinValue).ToString(PFDataHelper.DateFormat);
            //            bodyTBox.Text = email.Body;
            //        }));
            //    });
            //}
            
            await Task.Run(() => {
                bool hasError = false;
                while (emailCntNBox.Value > 0&& hasError==false)
                {
                    try
                    {
                        var email = _emailManager.Retrieve_Click(int.Parse(emailCntNBox.Value.ToString()));
                        if (email == null)
                        {
                            this.Invoke(new MethodInvoker(() =>
                            {
                                subjectTBox.Text = "找不到邮件";
                            }));
                        }
                        else
                        {
                            //if (email.Date == DateTime.MinValue)
                            //{
                            //    throw new Exception(string.Format("邮件[{0}]时间不正确", emailCntNBox.Value));
                            //}
                            this.Invoke(new MethodInvoker(() =>
                            {
                                //if (email.Date == DateTime.MinValue)
                                //{
                                //    hasError = true;
                                //    throw new Exception(string.Format("邮件[{0}]时间不正确", emailCntNBox.Value));
                                //}
                                subjectTBox.Text = email.Subject;
                                emailDateBox.Value = email.Date ?? DateTime.MinValue;
                                emailDateTBox.Text = (email.Date ?? DateTime.MinValue).ToString(PFDataHelper.DateFormat);
                                bodyTBox.Text = email.Body;
                            }));
                        }

                        this.Invoke(new MethodInvoker(() =>
                        {
                            emailCntNBox.Value = emailCntNBox.Value - 1;
                        }));
                    }catch(Exception exception)
                    {
                        this.Invoke(new MethodInvoker(() =>
                        {
                            errorTBox.Text = exception.ToString();
                        }));
                        hasError = true;
                    }
                    //Thread.Sleep(500);
                }
            });
        }
    }
}
