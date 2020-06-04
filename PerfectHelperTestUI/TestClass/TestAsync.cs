using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfect
{
   public class TestAsync
    {
        public static int num = 4;
        public static List<string> sList = new List<string>();
        public static async void Test()
        {
            sList = new List<string>();
            //num=6
            //DoAsync().Wait();

            //num=4
            //DoAsync();


            //DoAsync().ConfigureAwait(continueOnCapturedContext: false);//num=6(如果DoAsync方法内部没有await,那就是5)
            await DoAsync();
            Thread.Sleep(2000);

            Console.Write(num);
        }
        public static async Task<int> DoAsync() {
            //var result = 4;
            await DoAsync2();
            await DoAsync3();
            num = 6;
            //await Task.Run(() =>
            //{
            //    Thread.Sleep(1000);
            //    num = 5;
            //});
            return  num;
        }
        public static async Task<int> DoAsync2()
        {
            //var result = 4;
            await Task.Run(() =>
            {
                sList.Add("begin 2 \r\n");
                //Console.Write("begin 2 \r\n");
                Thread.Sleep(1000);
                sList.Add("end 2 \r\n");
                //Console.Write("end 2 \r\n");
                num = 5;
            });
            return num;
        }
        public static async Task<int> DoAsync3()
        {
            //var result = 4;
            await Task.Run(() =>
            {
                sList.Add("begin 3 \r\n");
                //Console.Write("begin 3 \r\n");
                Thread.Sleep(1000);
                sList.Add("end 3 \r\n");
                //Console.Write("end 3 \r\n");
                num = 5;
            });
            return num;
        }
    }
}
