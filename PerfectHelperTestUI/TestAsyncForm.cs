using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Perfect;
using System.Threading;

namespace PerfectHelperTestUI
{
    public partial class TestAsyncForm : Form
    {
        public TestAsyncForm()
        {
            InitializeComponent();
        }

        private async void testAsyncBtn_Click(object sender, EventArgs e)
        {
            SetResult("");
            #region 不阻塞
            ////不阻塞
            //Task.Run(() =>
            //{
            //    try
            //    {
            //        var t = PFDataHelper.CountTime(() =>
            //        {
            //            Thread.Sleep(5000);
            //        });
            //        AppendResult(string.Format("task finish {0}", PFDataHelper.GetTimeSpan(t)));
            //        //resultTBox.Text = "task finish";
            //    }
            //    catch (Exception e1)
            //    {
            //        var a = "a";
            //    }
            //});
            #endregion 不阻塞


            #region 不阻塞
            //await Task.Run(() =>
            // {
            //     try
            //     {
            //         var t = PFDataHelper.CountTime(() =>
            //         {
            //             Thread.Sleep(5000);
            //         });
            //         AppendResult(string.Format("task finish {0}", PFDataHelper.GetTimeSpan(t)));
            //     }
            //     catch (Exception e1)
            //     {
            //         var a = "a";
            //     }
            // });
            #endregion 不阻塞

            #region 不阻塞
            //await TestAsync();
            #endregion 不阻塞

            #region 输出12
            //await TestAsync(() =>
            //{
            //    Thread.Sleep(2000);
            //    AppendResult("1");
            //});
            //AppendResult("2");
            #endregion

            #region 输出21
            // TestAsync(() =>
            //{
            //    Thread.Sleep(2000);
            //    AppendResult("1");
            //});
            //AppendResult("2"); 
            #endregion 输出21

            #region 阻塞并死锁
            //AppendResult(IsTestAsync().Result.ToString());
            #endregion 阻塞并死锁

            #region 输出true
            var b = await IsTestAsync();
            AppendResult(b.ToString());
            #endregion 输出true
        }

        private  void testTaskBtn_Click(object sender, EventArgs e)
        {
            SetResult("");
            #region 不阻塞
            //TestTask();
            #endregion 不阻塞

            #region 阻塞并死锁
            //TestTask().Wait();
            #endregion 阻塞并死锁

            #region 阻塞并死锁
            //var b = IsTestTask().Result;
            #endregion 阻塞并死锁

            #region 不阻塞
            //TestTask().ConfigureAwait(
            //continueOnCapturedContext: false);
            #endregion 不阻塞

            #region 输出12
            //await TestTask(() =>
            //{
            //    Thread.Sleep(2000);
            //    AppendResult("1");
            //});
            //AppendResult("2");
            #endregion 输出12

            #region 输出21
            //TestTask(() =>
            //{
            //    Thread.Sleep(2000);
            //    AppendResult("1");
            //});
            //AppendResult("2"); 
            #endregion 输出21

            #region 输出12
            Task.Run(() =>
            {
                TestTask(() =>
                {
                    Thread.Sleep(2000);
                    AppendResult("1");
                }).Wait();
                AppendResult("2");
            }); 
            #endregion

            #region 阻塞并死锁
            //Task.WaitAll(TestTask());
            #endregion 阻塞并死锁

            #region 不阻塞
            //Task.WhenAll(TestTask()).ContinueWith((s) =>
            //{
            //    MessageBox.Show("ok");
            //    return s;
            //}); 
            #endregion 不阻塞

            #region 不阻塞
            //var b = await IsTestTask();
            //AppendResult(b.ToString()); 
            #endregion
        }

        #region 无参数
        private async Task TestAsync()
        {
            await Task.Run(() =>
            {
                var t = PFDataHelper.CountTime(() =>
                {
                    Thread.Sleep(5000);
                });
                AppendResult(string.Format("task finish {0}", PFDataHelper.GetTimeSpan(t)));
            });
        }
        private Task TestTask()
        {
            var rt = new Task(() =>
            {
                var t = PFDataHelper.CountTime(() =>
                {
                    Thread.Sleep(5000);
                });
                AppendResult(string.Format("task finish {0}", PFDataHelper.GetTimeSpan(t)));
            });
            rt.Start();
            return rt;
        }
        private async Task<bool> IsTestAsync()
        {
            var b = false;
            await Task.Run(() =>
            {
                var t = PFDataHelper.CountTime(() =>
                {
                    Thread.Sleep(5000);
                });
                AppendResult(string.Format("task finish {0}", PFDataHelper.GetTimeSpan(t)));
                b = true;
            });
            return b;
        }
        private Task<bool> IsTestTask()
        {
            var rt = new Task<bool>(() =>
            {
                var t = PFDataHelper.CountTime(() =>
                {
                    Thread.Sleep(5000);
                });
                AppendResult(string.Format("task finish {0}", PFDataHelper.GetTimeSpan(t)));
                return true;
            });
            rt.Start();
            return rt;
        }
        #endregion 无参数

        private async Task TestAsync(Action action)
        {
            await Task.Run(() => {
                action();
            });
        }
        private Task TestTask(Action action)
        {
            var rt = new Task(() =>
             {
                 action();
             });
            rt.Start();
            return rt;
        }


        public void SetResult(string text)
        {
            // 采用Invoke形式进行操作
            this.Invoke(new MethodInvoker(() =>
            {
                this.resultTBox.Text = text;
            }));
        }
        public void AppendResult(string text)
        {
            // 采用Invoke形式进行操作
            this.Invoke(new MethodInvoker(() =>
            {
                this.resultTBox.Text += text;
            }));
        }
    }
}
