using Newtonsoft.Json;
using Perfect;
using SaveDbReport.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfect
{
    public class TestMonthDataCompareCntConsumer : PFMqHelper.PFConsumerResponseTask
    {
        public object handle(String consumerTag, PFMqMessage message)
        {
            var lastCMonth = ProjDataHelper.GetLastCMonthByDate(DateTime.Now);
            
            var p = JsonConvert.DeserializeObject<PagingParameters>(message.ToString());
            var backupDatabase = PFDataHelper.ObjectToString(p["backupDatabase"]) ?? "";
            var service = new DbReportService(lastCMonth);
            var list = service.GetCompareCntList(backupDatabase, false);
            return list;
            
        }

        public PFMqConfig GetMqConfig(PFMqConfig mqConfig)
        {
            mqConfig.setMqType(PFMqHelper.PFMqType.PFEmailMq);
            mqConfig.setTopic(MqTopic.Test_TcTask_MonthDataCompareCnt.ToString());

            return mqConfig;
        }

    }
}
