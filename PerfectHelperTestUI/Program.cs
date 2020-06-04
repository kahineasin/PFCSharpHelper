using Perfect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectHelperTestUI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            PFDataHelper.SetConfigMapper(new PFConfigMapper());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var win = new MainTabForm();
            win.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //Application.Run(new Form1());
            //Application.Run(new FuncForm());
            Application.Run(win);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ////Application.Run(new Form1());
            //Application.Run(new MainTabForm());
            
        }
    }
}
