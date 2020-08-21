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
    public static class PFWinFormHelper
    {
        public static Color ErrorColor = Color.Red;
        public static Color FinishColor = Color.Blue;
        public static Color UnfinishColor = Color.Gray;
        /// <summary>
        /// flag 为false时候，退出执行耗时操作
        /// </summary>
        public static bool MultiFuncLoadingFlag = false;
        public static void GridClear(this DataGridView grid)
        {
            for (int i = grid.Rows.Count - 1; i >= 0; i--)
            {
                var row = grid.Rows[i];
                if (!row.IsNewRow) { grid.Rows.Remove(row); }
            }
        }
        public static void SetFinish(this TextBox textBox, string msg)
        {
            textBox.Text = msg;
            SetTextBoxForeColor(textBox, FinishColor);
        }
        public static void SetUnfinish(this TextBox textBox, string msg)
        {
            textBox.Text = msg;
            SetTextBoxForeColor(textBox, UnfinishColor);
        }
        private static void SetTextBoxForeColor(TextBox textBox, Color color)
        {
            if (textBox.ReadOnly) { textBox.BackColor = textBox.BackColor; }//没有此句时,readOnly的textBox不会刷新文字颜色
            textBox.ForeColor = color;
        }
        /// <summary>
        /// 设置下拉数据源（winfrom的comboBox好像没有keyValue的用法
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="data"></param>
        public static void SetComboBoxListData(this ComboBox comboBox, Enum data)
        {
            var list = PFDataHelper.EnumToKVList(data);
            comboBox.DataSource= list.Select(a=>a.Value).ToList();
        }
        /// <summary>
        /// 写信息到TextArea第一行
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="info"></param>
        public static void WriteInfoToTextArea(this TextBox tb, string info)
        {
            int thrInfoMaxLen = 10000;
            if (tb.Text.Length > thrInfoMaxLen) { tb.Text = tb.Text.Substring(0, thrInfoMaxLen / 2); }
            info += "--" + DateTime.Now + "\r\n";
            tb.Text = info + tb.Text;
        }

        public static void GetGridSelectedRow(DataGridView grid, Action<DataGridViewRow> rowAction)
        {
            var selects = grid.SelectedRows;
            if (selects.Count != 1)
            {
                MessageBox.Show("请选择一条数据");
                return;
            }
            else
            {
                rowAction(selects[0]);
            }
        }
        public static void GetGridSelectedRows(DataGridView grid,
            //Action<DataGridViewRowCollection> rowAction
            Action<List<DataGridViewRow>> rowAction
            )
        {
            var selects = grid.SelectedRows;
            if (selects.Count < 1)
            {
                MessageBox.Show("请先选择数据");
                return;
            }
            else
            {
                //var rows = new DataGridViewRowCollection(grid);//当grid是DataSource绑定DataTable时不能这样用
                var rows = new List<DataGridViewRow>();
                foreach (DataGridViewRow i in selects)
                {
                    //var x=new 
                    //var row = new DataGridViewRow(i);
                    rows.Add(i);
                }
                rowAction(rows);
            }
        }
        public static void GetGridSelectedRowsByIndex(DataGridView grid,
            //Action<DataGridViewRowCollection> rowAction
            Action<List<DataGridViewRow>> rowAction
            )
        {
            //var rows = new DataGridViewRowCollection(grid);//当grid是DataSource绑定DataTable时不能这样用
            var rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow i in grid.Rows)
            {
                if (i.Selected)
                {
                    rows.Add(i);
                }
            }
            
            if (rows.Count < 1)
            {
                MessageBox.Show("请先选择数据");
                return;
            }
            else
            {
                rowAction(rows);
            }
        }
        public static void SelectGridRows(DataGridView grid,
            //Action<DataGridViewRowCollection> rowAction
            Func<DataGridViewRow,bool> rowAction
            )
        {
            foreach (DataGridViewRow i in grid.Rows)
            {
                if (rowAction(i))
                {
                    i.Selected = true;
                    //grid.CurrentCell = i.Cells[0];
                }
            }
        }
    }

}
