using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfect
{
    public class PFSqlUpdateValidateHelper
    {
        /// <summary>
        /// 适用场境,在测试库时,更新完数据后,验证数据是否有异常
        /// 
        /// 常用方法:
        ///var update = new SqlUpdateCollection(new
        ///{
        ///    id = id,
        ///    agentno = agentno
        ///})
        ///.PrimaryKeyFields("id");
        ///PFSqlUpdateValidateHelper.TestUpdate("t_hyzl_orders", update, sqlExec);
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="update"></param>
        /// <param name="sql"></param>
        public static void TestUpdate(string tableName, SqlUpdateCollection update, ProcManager sql)
        {
            string updateSqlString = string.Format(@" select * from {0} {1}
                ", tableName, update.ToWhereSql());
            string totalSqlString = string.Format(@" select count(*) from {0}
                ", tableName);

            //用set条件的字段做where来查总数,如果行数等于全表行数,那说明把整个表的值都更新了(where没有生效)
            var updateSet = new SqlWhereCollection();
            foreach (var i in update)
            {
                updateSet.Add(i.Key, i.Value.Value);
            }
            string updateSetTotalSqlString = string.Format(@" select count(*) from {0} {1}
                ", tableName, updateSet.ToSql());

            var updated = sql.GetQueryTable(updateSqlString);
            var total = PFDataHelper.ObjectToInt(sql.QuerySingleValue(totalSqlString));
            var setTotal = PFDataHelper.ObjectToInt(sql.QuerySingleValue(updateSetTotalSqlString));
            if (updated == null) { throw new Exception("更新后的数据全部丢失.异常"); }
            if (total < 2) { throw new Exception("测试数据少于2条,这样不保险"); }
            if (total == updated.Rows.Count) { throw new Exception("更新了整个表的数据,请确认是否缺少where条件.异常"); }
            if (total == setTotal) { throw new Exception("更新了整个表的数据,请确认是否缺少where条件.异常"); }
            AssertIsTrue(updated != null && updated.Rows.Count == 1);
            AssertIsTrue(total > 1);
            AssertIsTrue(IsDataRowMatchUpdate(updated.Rows[0], update));
            AssertIsTrue(setTotal >= updated.Rows.Count && setTotal < total);
        }

        #region Private
        private static bool AssertIsTrue(bool b)
        {
            if (!b) { throw new Exception("不为true"); }
            return b;
        }
        private static bool IsDataRowMatchUpdate(DataRow row, SqlUpdateCollection update)
        {
            foreach (var i in update)
            {
                if (PFDataHelper.ObjectToString(i.Value.Value) == PFDataHelper.ObjectToString(row[i.Key]))
                {

                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
