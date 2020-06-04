using Perfect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace YJQuery.Models
{
    public class CompareCnt
    {
        public CompareCnt() { }
        //public CompareCnt(DataGridViewRow grow) {
        //    SrcTbName =PFDataHelper.ObjectToString( grow.Cells["srcTbName"].Value);
        //    Flag = PFDataHelper.ObjectToBool(grow.Cells["flag"].Value);
        //    Total = PFDataHelper.ObjectToInt(grow.Cells["total"].Value);
        //    TotalMonth = PFDataHelper.ObjectToString(grow.Cells["totalMonth"].Value);
        //    TotalDate = PFDataHelper.ObjectToDateTime(grow.Cells["totalDate"].Value);
        //    SrcCnt = PFDataHelper.ObjectToInt(grow.Cells["srcCnt"].Value)??0;
        //    DstCnt = PFDataHelper.ObjectToInt(grow.Cells["dstCnt"].Value) ?? 0;
        //}
        public string SrcTbName { get; set; }
        public bool? Flag { get; set; }
        public int? Total { get; set; }
        public string TotalMonth { get; set; }
        public DateTime? TotalDate { get; set; }
        public int SrcCnt { get; set; }
        //public string DstTbName { get; set; }
        //public int DstCnt { get; set; }
        public int? DstCnt { get; set; }//如果有新增表,旧的月份数据里没有备份该表,就显示为null--benjamin 20190812

        /// <summary>
        /// 源比目标大
        /// </summary>
        /// <returns></returns>
        public bool SrcMoreThenDst() {
           return SrcCnt>DstCnt;
        }
        /// <summary>
        /// 是统计表
        /// </summary>
        /// <returns></returns>
        public bool IsTotalTb()
        {
            return SrcTbName.IndexOf("t_zxbmonth")==0;
        }
        /// <summary>
        /// 已转移
        /// </summary>
        /// <returns></returns>
        public bool IsTransfered()
        {
            return SrcCnt==DstCnt&& DstCnt>0;
        }
        /// <summary>
        /// 允许转移
        /// </summary>
        /// <returns></returns>
        public bool AllowTransfer()
        {
            return (Flag==true&& SrcCnt>0 && DstCnt == 0 && Total==SrcCnt)//普通表
                ||(IsTotalTb()&& SrcCnt > 0 && DstCnt == 0);
        }
        /// <summary>
        /// Src数值不正确
        /// </summary>
        /// <returns></returns>
        public bool IsSrcCntWrong()
        {
            return SrcCnt !=Total&&!IsTotalTb();
        }
        /// <summary>
        /// 应注意Total的值
        /// </summary>
        /// <returns></returns>
        public bool ShouldNoticeTotal()
        {
            if (IsTotalTb()) { return false; }
            return (SrcCnt>0&&SrcCnt!=Total);
        }
        /// <summary>
        /// 允许删除已转移的数据
        /// </summary>
        /// <returns></returns>
        public bool AllowDeleteTransfered()
        {
            return Flag != false && (SrcCnt != DstCnt||IsTotalTb()) && DstCnt > 0
                //&&SrcCnt>0
                ;
        }
        public static string SuccessStatus = "导入完整";
        public string GetStatus()
        {
            if (SrcCnt > DstCnt && DstCnt > 0) { return "未导完整"; }
            if (SrcCnt > DstCnt&&DstCnt==0) { return "未导入"; }
            if (SrcCnt <= DstCnt && SrcCnt == 0&&DstCnt==Total) { return SuccessStatus; }
            if (SrcCnt == DstCnt) { return SuccessStatus; }
            if (SrcCnt > DstCnt) { return "未导完整"; }
            return "异常";
        }
        public bool IsSuccess() {
            return GetStatus() == SuccessStatus;
        }
    }
}
