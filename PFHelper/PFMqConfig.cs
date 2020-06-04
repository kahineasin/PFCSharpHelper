using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  Perfect;

namespace Perfect
{
    public class PFMqConfig : ICloneable
    {

        public void beforeInit()
        {
        }

        private String mqType;

        // rabbitMq
        private String queueName;
        private String host;

        // aliMq

        private String groupId;

        private String nameSrvAddr;

        private String onsAddr;

        private String accessKey;

        private String secretKey;

        private String messageModel;

        private String topic;

        private String tag;

        private String instanceName;


        public String getTag()
        {
            return tag;
        }

        public void setTag(String tag)
        {
            this.tag = tag;
        }

        // public String getMqType() {
        //		return mqType;
        //	}
        //	public void setMqType(String mqType) {
        //		this.mqType = mqType;
        //	}
        public Perfect.PFMqHelper.PFMqType getMqType()
        {
            Perfect.PFMqHelper.PFMqType r = Perfect.PFMqHelper.PFMqType.PFEmailMq;
            Enum.TryParse<Perfect.PFMqHelper.PFMqType>(mqType, out r);
            return r;
        }

        public void setMqType(Perfect.PFMqHelper.PFMqType mqType)
        {
            this.mqType = mqType.ToString();
        }

        public String getQueueName()
        {
            return queueName;
        }

        public void setQueueName(String queueName)
        {
            this.queueName = queueName;
        }

        public String getHost()
        {
            return host;
        }

        public void setHost(String host)
        {
            this.host = host;
        }

        public String getGroupId()
        {
            return groupId;
        }

        public void setGroupId(String groupId)
        {
            this.groupId = groupId;
        }

        public String getNameSrvAddr()
        {
            return nameSrvAddr;
        }

        public void setNameSrvAddr(String nameSrvAddr)
        {
            this.nameSrvAddr = nameSrvAddr;
        }

        public String getOnsAddr()
        {
            return onsAddr;
        }

        public void setOnsAddr(String onsAddr)
        {
            this.onsAddr = onsAddr;
        }

        public String getAccessKey()
        {
            return accessKey;
        }

        public void setAccessKey(String accessKey)
        {
            this.accessKey = accessKey;
        }

        public String getSecretKey()
        {
            return secretKey;
        }

        public void setSecretKey(String secretKey)
        {
            this.secretKey = secretKey;
        }

        public String getMessageModel()
        {
            return messageModel;
        }

        public void setMessageModel(String messageModel)
        {
            this.messageModel = messageModel;
        }

        public String getTopic()
        {
            return topic;
        }

        public void setTopic(String topic)
        {
            this.topic = topic;
        }

        public String getInstanceName()
        {
            return instanceName;
        }

        public void setInstanceName(String instanceName)
        {
            this.instanceName = instanceName;
        }
        public PFMqConfig TClone()
        {
            return TransExpV2<PFMqConfig, PFMqConfig>.Trans(this);
        }

        public object Clone()
        {
            return TClone();
        }

    }
}