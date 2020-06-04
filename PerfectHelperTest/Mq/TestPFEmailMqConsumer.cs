using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfect;

namespace Perfect
{
    public class TestPFEmailMqConsumer : PFMqHelper.PFConsumerResponseTask
    {
        //public Action<string> TestAction { get; set; }
        public object handle(String consumerTag, PFMqMessage message)
        {
            String strMessage = message.getMessage();
            //if (TestAction != null) { TestAction(strMessage); }
            return "接收到信息:"+ strMessage;
	
        }
        public PFMqConfig GetMqConfig(PFMqConfig mqConfig)
        {
            mqConfig = new TestPFEmailMqProducer().GetMqConfig(mqConfig);
            mqConfig.setTag("*");
            return mqConfig;
        }

    }
    public class TestPFEmailMqProducer : PFMqHelper.PFProdutResponseTask
    {

        public PFMqConfig GetMqConfig(PFMqConfig mqConfig)
        {
            mqConfig.setMqType(Perfect.PFMqHelper.PFMqType.PFEmailMq);
            mqConfig.setTopic(MqTopic.Test_TcTask_MonthDataCompareCnt.ToString());
            return mqConfig;
        }
    }
}
