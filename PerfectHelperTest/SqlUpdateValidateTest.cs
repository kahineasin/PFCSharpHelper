//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Perfect;
//using System.Collections.Generic;
//using System.Data;

//namespace PerfectHelperTest
//{
//    /// <summary>
//    /// 适用场境,在测试库时,更新完数据后,验证数据是否有异常
//    /// </summary>
//    [TestClass]
//    public class SqlUpdateValidateTest
//    {
//        [TestMethod]
//        public void TestUpdate(string tableName,SqlUpdateCollection update, ProcManager sql)
//        {
//            var updated = sql.GetQueryTable(string.Format(@" select * from {0} 
//                ", update.ToWhereSql()));
//            var total =PFDataHelper.ObjectToInt(sql.QuerySingleValue(string.Format(@" select count(*) from {0} 
//                ", update.ToWhereSql())));
//            Assert.IsTrue(updated!=null&& updated.Rows.Count==1);
//            Assert.IsTrue(total>1);
//            Assert.IsTrue(IsDataRowMatchUpdate(updated.Rows[0], update));
//        }

//        private bool IsDataRowMatchUpdate(DataRow row,SqlUpdateCollection update) {
//            foreach(var i in update)
//            {
//                if(PFDataHelper.ObjectToString(i.Value)== PFDataHelper.ObjectToString(row[i.Key]))
//                {

//                }
//                else
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//    }

//    public class SqlUpdateValidateResultModel
//    {
//        /// <summary>
//        /// 更新完的行数,修改时一般是1
//        /// </summary>
//        public int UpdateRowCount { get; set; }
//    }
//}
