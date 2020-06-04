using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfect
{
    public interface ISqlExecute//<TSqlDataReader>
                                //where TSqlDataReader:DbDataReader
    {
        void OpenConn();
        void CloseConn();
        //void SetHugeCommandTimeOut();
        //DataTable GetQueryTable(string sqlval);

        ////TSqlDataReader GetDataReader2(string sqlstr);
    }
}
