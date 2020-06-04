using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfect;
using Newtonsoft.Json;
using YJQuery.Models;

namespace Perfect
{
    public class TestMonthDataCompareCntProducer : Perfect.PFMqHelper.PFProdutResponseTask
    {
        public static List<CompareCnt> Product(string backupDatabase) {
                        
            var resMsg = "";

            var p = new PagingParameters();
            p["backupDatabase"] = "bonus";

            var st = DateTime.Now;
            var t = PFMqHelper.BuildProducer(JsonConvert.SerializeObject(p),
                    new TestMonthDataCompareCntProducer());
            t.Wait();

            resMsg = t.Result.Body;
            var result = JsonConvert.DeserializeObject<List<CompareCnt>>(resMsg);
           return result;
        }

        public PFMqConfig GetMqConfig(PFMqConfig mqConfig)
        {
            mqConfig.setMqType(PFMqHelper.PFMqType.PFEmailMq);
            mqConfig.setTopic(MqTopic.Test_TcTask_MonthDataCompareCnt.ToString());
            return mqConfig;
        }
    }
    public class MonthDataCompareCntProducer : Perfect.PFMqHelper.PFProdutResponseTask
    {
        public static List<CompareCnt> Product(string backupDatabase)
        {

            var resMsg = "";

            var p = new PagingParameters();
            p["backupDatabase"] = "bonus";

            var st = DateTime.Now;
            var t = PFMqHelper.BuildProducer(JsonConvert.SerializeObject(p),
                    new MonthDataCompareCntProducer());
            t.Wait();

            resMsg = t.Result.Body;
            var result = JsonConvert.DeserializeObject<List<CompareCnt>>(resMsg);
            return result;
        }

        public PFMqConfig GetMqConfig(PFMqConfig mqConfig)
        {
            mqConfig.setMqType(PFMqHelper.PFMqType.PFEmailMq);
            mqConfig.setTopic(MqTopic.TcTask_MonthDataCompareCnt.ToString());
            return mqConfig;
        }
    }
}
