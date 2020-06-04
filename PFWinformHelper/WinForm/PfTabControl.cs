using System;
using System.Drawing;
using System.Windows.Forms;

namespace Perfect
{
    /// <summary>
    /// 线程不安全
    /// </summary>
    public class PFTabControl : TabControl
    {
        public PFTabControl() : base()
        {
            //清空控件  
            this.TabPages.Clear();
            //绘制的方式OwnerDrawFixed表示由窗体绘制大小也一样  
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new System.Drawing.Point(CLOSE_SIZE, CLOSE_SIZE / 2);
            this.DrawItem += new DrawItemEventHandler(this.MainTabControl_DrawItem);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainTabControl_MouseDown);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainTabControl_MouseDoubleClick);

        }

        /// <summary>
        /// 不管Tab是否存在,form都是新的
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="form"></param>
        public void AddTab<TForm>(TForm form)
            where TForm : Form
        {
            string title = form.Text;
            bool ifExits = false;
            int idx = 0;
            foreach (TabPage tb in this.TabPages)
            {
                if (tb.Text == title)
                {
                    ifExits = true;
                    this.SelectedIndex = idx;
                    tb.Controls.RemoveAt(0);
                    form.FormBorderStyle = FormBorderStyle.None; //隐藏子窗体边框（去除最小花，最大化，关闭等按钮）
                    form.TopLevel = false; //指示子窗体非顶级窗体
                    tb.Controls.Add(form);
                    form.Show();
                }
                idx++;
            }
            if (!ifExits)
            {
                TabPage tabtage = new TabPage();
                tabtage.Text = title;
                tabtage.ToolTipText = title;
                form.FormBorderStyle = FormBorderStyle.None; //隐藏子窗体边框（去除最小花，最大化，关闭等按钮）
                form.TopLevel = false; //指示子窗体非顶级窗体
                tabtage.Controls.Add(form);//将子窗体载入panel
                form.Show();

                TabPages.Add(tabtage);

                form.AutoSize = true;
                //form.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                //form.MaximizeBox = true;
                form.Width = tabtage.Width;
                form.Height = tabtage.Height;
                var size = new Size(tabtage.Width, tabtage.Height);
                form.MinimumSize = size;
                form.MaximumSize = size;

                tabtage.SizeChanged += new EventHandler(this.totalDGView_CellContentClick);

                SelectedTab = tabtage;
                //image = new Bitmap(TestTabPageTabControl.Properties.Resources.box_okay);  
            }
        }

        private void totalDGView_CellContentClick(object sender, EventArgs e)
        {
            var tab = sender as TabPage;
            tab.Controls[0].Width = tab.Width;
            tab.Controls[0].Height = tab.Height;
            var size = new Size(tab.Width, tab.Height);
            tab.Controls[0].MinimumSize = size;
            tab.Controls[0].MaximumSize = size;
            //foreach (TabPage i in TabPages)
            //{
            //    i.Controls[0].Height=i.Height
            //}
        }
        private int CLOSE_SIZE = 12;
        private void MainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {

            try
            {

                Rectangle myTabRect = this.GetTabRect(e.Index);

                //先添加TabPage属性     
                e.Graphics.DrawString(this.TabPages[e.Index].Text
                , this.Font, SystemBrushes.ControlText, myTabRect.X + 5, myTabRect.Y + 5);

                //再画一个矩形框  
                using (Pen p = new Pen(Color.White))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 3);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);

                }

                //填充矩形框  
                Color recColor = e.State == DrawItemState.Selected ? Color.White : Color.White;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }
                //画关闭符号  
                using (Pen objpen = new Pen(Color.Black))
                {
                    //"\"线  
                    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    e.Graphics.DrawLine(objpen, p1, p2);

                    //"/"线  
                    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    e.Graphics.DrawLine(objpen, p3, p4);
                }

                e.Graphics.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void MainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;

                //计算关闭区域     
                Rectangle myTabRect = this.GetTabRect(this.SelectedIndex);

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;

                //如果鼠标在区域内就关闭选项卡     
                bool isClose = x > myTabRect.X && x < myTabRect.Right
                 && y > myTabRect.Y && y < myTabRect.Bottom;

                if (isClose == true)
                {
                    this.TabPages.Remove(this.SelectedTab);
                }
            }


        }
        private void MainTabControl_MouseDoubleClick(object sender, EventArgs e)
        {
            TabPages.Remove(SelectedTab);
        }
    }
}
