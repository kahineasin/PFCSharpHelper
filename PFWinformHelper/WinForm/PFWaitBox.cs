using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perfect
{
    /// <summary>
    /// 等待窗口
    /// 使用方法:
    /*
    var f = new PFWaitBox(() => {

        MessageBox.Show("现在开始导入数据,请等待...");
        var transForm = new TransferDataForm(Cmonth);
        ProjDataHelper.MainTab.AddTab(transForm);
        transForm.TransferAll();

    });
    f.Show();
    */
    /// </summary>
    public class PFWaitBox : Form
    {
        public delegate void AddListItem(String myString);
        public AddListItem myDelegate;
        private Thread myThread;
        Action action = null;
        private Label myButton;
        public PFWaitBox(Action doAction, string waitMsg = "请等待", Action<PFWaitBox> formAction = null) : base()
        {
            myButton = new Label();
            //myButton.Location = new Point(72, 160);
            myButton.Location = new Point(72, 72);
            //myButton.Size = new Size(152, 32);
            myButton.TabIndex = 1;
            myButton.Text = waitMsg;
            myButton.Width = int.Parse(PFDataHelper.GetWordsWidth(waitMsg).Replace("px", ""));

            var lab = myButton;
            var waitForm = this;
            waitForm.Width = lab.Width + 200;
            waitForm.Height = lab.Height + 100;
            //waitForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            int windowBorder = (waitForm.Width - waitForm.ClientRectangle.Width) / 2;
            int topHeight = (waitForm.Height - waitForm.ClientRectangle.Height - windowBorder);
            lab.Location = new Point((waitForm.Width - lab.Width) / 2, (waitForm.Height - topHeight - lab.Height) / 2);

            //ClientSize = new Size(292, 273);
            //ClientSize = new Size(292, 185);
            Controls.AddRange(new Control[] { myButton });
            //Text = " 'Control_Invoke' example";
            Text = "稍后";
            #region by wxj
            action = doAction;
            if (formAction != null) { formAction(this); }
            #endregion

            myDelegate = new AddListItem(AddListItemMethod);

            //myThread = new Thread(new ThreadStart(ThreadFunction));
            //myThread.Start();
        }
        public void AddListItemMethod(String myString)
        {
            if (action != null) { action(); }
            this.Close();
            //String myItem;
            //for (int i = 1; i < 6; i++)
            //{
            //    myItem = "MyListItem" + i.ToString();
            //    myListBox.Items.Add(myItem);
            //    myListBox.Update();
            //    Thread.Sleep(300);
            //}
        }
        public new void Show()
        {
            base.Show();
            Button_Click(null, null);
        }
        private void Button_Click(object sender, EventArgs e)
        {
            myThread = new Thread(new ThreadStart(ThreadFunction));
            myThread.Start();
        }
        private void ThreadFunction()
        {
            PFWaitBoxThreadClass myThreadClassObject = new PFWaitBoxThreadClass(this);
            myThreadClassObject.Run();
        }
    }
    public class PFWaitBoxThreadClass
    {
        PFWaitBox myFormControl1;
        String myString;
        public PFWaitBoxThreadClass(PFWaitBox myForm)
        {
            myFormControl1 = myForm;
        }

        public void Run()
        {
            myString = "Step number executed";
            //Thread.Sleep(2000);
            myFormControl1.Invoke(myFormControl1.myDelegate,
                                   new Object[] { myString });
            //// Execute the specified delegate on the thread that owns
            //// 'myFormControl1' control's underlying window handle.
            //myFormControl1.Invoke(myFormControl1.myDelegate);
        }
    }
}
