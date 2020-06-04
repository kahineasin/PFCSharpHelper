using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfect
{
    public class PFMqMessage
    {
        private String message;
        public PFMqMessage(PFEmail email)
        {
            message = email.Body;
        }
        //public PFMqMessage(Delivery delivery)
        //{
        //    try
        //    {
        //        message = new String(delivery.getBody(), PFDataHelper.encoding);
        //    }
        //    catch (UnsupportedEncodingException e)
        //    {
        //        // TODO Auto-generated catch block
        //        e.printStackTrace();
        //    }
        //}
        //public PFMqMessage(com.aliyun.openservices.ons.api.Message aliMessage)
        //{
        //    try
        //    {
        //        message = new String(aliMessage.getBody(), PFDataHelper.encoding);
        //    }
        //    catch (UnsupportedEncodingException e)
        //    {
        //        // TODO Auto-generated catch block
        //        e.printStackTrace();
        //    }
        //}
        //public PFMqMessage(org.apache.rocketmq.common.message.Message aliMessage)
        //{
        //    try
        //    {
        //        message = new String(aliMessage.getBody(), PFDataHelper.encoding);
        //    }
        //    catch (UnsupportedEncodingException e)
        //    {
        //        // TODO Auto-generated catch block
        //        e.printStackTrace();
        //    }
        //}
        public String getMessage()
        {
            return message;
        }

        public void setMessage(String message)
        {
            this.message = message;
        }
        public override string ToString()
        {
            return message;
        }
    }

    public class PFMq
    {
        private PFMqConfig _mqConfig;
        public PFMq(PFMqConfig mqConfig)
        {
            _mqConfig = mqConfig;
        }


        //	public  void BuildRabbitMqConsumer(PFMqHelper.PFConsumerTask pfDeliverCallback) {

        //		try {
        //	    	//rabbitmq
        //		    PFMqConfig mqConfig=pfDeliverCallback.GetMqConfig(_mqConfig);

        //	    	com.rabbitmq.client.ConnectionFactory factory = new com.rabbitmq.client.ConnectionFactory();
        //		    factory.setHost(mqConfig.getHost());
        //		    Connection connection;
        //			connection = factory.newConnection();
        //		    Channel channel = connection.createChannel();
        //		    String QUEUE_NAME=mqConfig.getQueueName();
        //		    channel.queueDeclare(QUEUE_NAME, false, false, false, null);
        //		    System.out.println("\r\n  [*][rabbitMq] queueName:"+QUEUE_NAME+"\r\n    Waiting for messages.\r\n");

        //		    DeliverCallback deliverCallback = (consumerTag, delivery) -> {
        //		    	PFMqMessage pfMessage=new PFMqMessage(delivery);
        //		        String logMsg="\r\n  [x][rabbitMq] queueName:"+QUEUE_NAME+"\r\n    "+pfDeliverCallback.getClass().getSimpleName()+" Received '" + pfMessage.getMessage() + "' \r\n";
        //		        WriteLog(logMsg);
        //		    	pfDeliverCallback.handle(consumerTag,pfMessage);
        //		    };
        //		    channel.basicConsume(QUEUE_NAME, true, deliverCallback, consumerTag -> { });
        //		} catch (IOException | TimeoutException e) {
        //			// TODO Auto-generated catch block
        //			e.printStackTrace();
        //		}
        //	}
        //	public  void BuildRocketMqConsumer(PFConsumerTask pfDeliverCallback) {

        //        try {
        //		    PFMqConfig mqConfig=pfDeliverCallback.GetMqConfig(_mqConfig);   
        //			//org.apache.rocketmq.client.consumer.DefaultMQPushConsumer consumer = new org.apache.rocketmq.client.consumer.DefaultMQPushConsumer("test-group");
        //	//        consumer.setNamesrvAddr("localhost:9876");
        //	//        consumer.setInstanceName("rmq-instance");
        //	//        consumer.subscribe("log-topic", "user-tag");
        //			org.apache.rocketmq.client.consumer.DefaultMQPushConsumer consumer = new org.apache.rocketmq.client.consumer.DefaultMQPushConsumer(mqConfig.getGroupId());
        //	        consumer.setNamesrvAddr(mqConfig.getNameSrvAddr());

        //	        if(!PFDataHelper.StringIsNullOrWhiteSpace(_mqConfig.getInstanceName())) {
        //	        	consumer.setInstanceName(_mqConfig.getInstanceName());
        //	        }

        //	        consumer.subscribe(mqConfig.getTopic(),mqConfig.getTag());

        //	        consumer.registerMessageListener(new org.apache.rocketmq.client.consumer.listener.MessageListenerConcurrently() {
        //				@Override
        //				public org.apache.rocketmq.client.consumer.listener.ConsumeConcurrentlyStatus consumeMessage(List<MessageExt> msgs, ConsumeConcurrentlyContext context) {

        //		          for (MessageExt msg : msgs) {
        //				    	PFMqMessage pfMessage=new PFMqMessage(msg);
        //				        String logMsg="\r\n [x][rocketMq] topic:"+mqConfig.getTopic()+" tag:"+mqConfig.getTag()+"\r\n    "+pfDeliverCallback.getClass().getSimpleName()+" Received '" + pfMessage.getMessage() + "' \r\n ";
        //				        WriteLog(logMsg);				        
        //						pfDeliverCallback.handle( mqConfig.getTag(),pfMessage);
        //		          }
        //		          return org.apache.rocketmq.client.consumer.listener.ConsumeConcurrentlyStatus.CONSUME_SUCCESS;
        //				}
        //	        });
        ////	        consumer.registerMessageListener(new org.apache.rocketmq.client.consumer.listener.MessageListenerOrderly() {
        ////
        ////				@Override
        ////				public ConsumeOrderlyStatus consumeMessage(List<MessageExt> msgs, ConsumeOrderlyContext context) {
        ////			          for (MessageExt msg : msgs) {
        ////			              System.out.println("消费者消费数据:"+new String(msg.getBody()));
        ////			          }
        ////			          return org.apache.rocketmq.client.consumer.listener.ConsumeOrderlyStatus.SUCCESS;
        ////				}
        ////	        });
        //			consumer.start();
        //			System.out.println("\r\n [*][rocketMq] topic:"+mqConfig.getTopic()+" tag:"+mqConfig.getTag()+"\r\n    Waiting for messages.\r\n");
        //		} catch (MQClientException e) {
        //			// TODO Auto-generated catch block
        //			e.printStackTrace();
        //		}

        //	}
        //	public  void BuildAliMqConsumer(PFConsumerTask pfDeliverCallback) {
        //		//参考:D:\eclipse_workspace\IpaasTest\src\com\mq\simple\ConsumerTest.java
        //        Properties properties = GetAliMqProperties();        

        //	    PFMqConfig mqConfig=pfDeliverCallback.GetMqConfig(_mqConfig);        

        //        Consumer consumer = ONSFactory.createConsumer(properties);

        //		consumer.subscribe(mqConfig.getTopic(), mqConfig.getTag(), new MessageListener() { 
        //		    public Action consume(Message message, ConsumeContext context) {
        //		    	PFMqMessage pfMessage=new PFMqMessage(message);
        //		        String logMsg="\r\n [x][aliMq] topic:"+mqConfig.getTopic()+" tag:"+mqConfig.getTag()+"\r\n    "+pfDeliverCallback.getClass().getSimpleName()+" Received '" + pfMessage.getMessage() + "' \r\n ";
        //		        WriteLog(logMsg);
        //				pfDeliverCallback.handle( mqConfig.getTag(),pfMessage);
        //		    	return Action.CommitMessage;
        //		    }
        //		});
        //        consumer.start();
        //	    System.out.println("\r\n [*][aliMq] topic:"+mqConfig.getTopic()+" tag:"+mqConfig.getTag()+"\r\n    Waiting for messages.\r\n");
        //	}

        //public void BuildPFEmailMqConsumer(PFMqHelper.PFConsumerTask pfDeliverCallback)
        //{
        //    PFMqConfig mqConfig = pfDeliverCallback.GetMqConfig(_mqConfig);
        //    //string producerEmailTitle = "PFEmailMq_producer_" + "会员资料表";
        //    //string producerEmailTitle = "PFEmailMq_producer_" + "hyzl";
        //    //消费方(使用系统邮箱)
        //    string result = "";
        //    //bool success = false;
        //    var consumerTask = new PFListenEmailTask("PFEmailMqConsumerListener_" + mqConfig.getTopic(),
        //    new PFEmailManager(PFDataHelper.SysEmailHostName, PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd),
        //    email =>
        //    {
        //        //result = "{success:true}";
        //        PFMqMessage pfMessage = new PFMqMessage(email);
        //        var r = pfDeliverCallback.handle(mqConfig.getTag(), pfMessage);
        //        if (r != null)
        //        {
        //            var UserEmailUserName = PFDataHelper.SysEmailUserName;
        //            ////消费方回复邮件(暂不回复--benjamin)
        //            PFDataHelper.SendEmail(PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd, PFDataHelper.SysEmailHostName,
        //                new string[] { UserEmailUserName }, "PFEmailMq_consumer_Response_" + email.Subject + email.Body,
        //               JsonConvert.SerializeObject(r));
        //        }
        //    },
        //    (email//, task
        //    ) =>
        //    {
        //        //消费方监听生产方邮件
        //        //return email.Subject != null && email.Subject.IndexOf("TestForceUpdateHyzl_") == 0;//这里不要用>-1,否则可能把自动回复的邮件也当作是了
        //        return email.Subject == mqConfig.getTopic();
        //    });
        //    consumerTask.Start();

        //}
        public void BuildPFEmailMqConsumer(PFMqHelper.PFConsumerTask pfDeliverCallback)
        {
            PFMqConfig mqConfig = pfDeliverCallback.GetMqConfig(_mqConfig);
            //string producerEmailTitle = "PFEmailMq_producer_" + "会员资料表";
            //string producerEmailTitle = "PFEmailMq_producer_" + "hyzl";
            //消费方(使用系统邮箱)
            string result = "";
            //bool success = false;
            var consumerTask = new PFListenEmailTask("PFEmailMqConsumerListener_" + mqConfig.getTopic(),
            new PFEmailManager(PFDataHelper.SysEmailHostName, PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd),
            email =>
            {
                //result = "{success:true}";
                PFMqMessage pfMessage = new PFMqMessage(email);
                pfDeliverCallback.handle(mqConfig.getTag(), pfMessage);
            },
            (email//, task
            ) =>
            {
                //消费方监听生产方邮件
                //return email.Subject != null && email.Subject.IndexOf("TestForceUpdateHyzl_") == 0;//这里不要用>-1,否则可能把自动回复的邮件也当作是了
                return email.Subject == "PFEmailMq_product_" + mqConfig.getTopic();
            });
            consumerTask.Start();

        }
        public void BuildPFEmailMqConsumer(PFMqHelper.PFConsumerResponseTask pfDeliverCallback)
        {
            PFMqConfig mqConfig = pfDeliverCallback.GetMqConfig(_mqConfig);
            //string producerEmailTitle = "PFEmailMq_producer_" + "会员资料表";//中文有问题--benjamin todo
            //string producerEmailTitle = "PFEmailMq_producer_" + "hyzl";
            //消费方(使用系统邮箱)
            string result = "";
            //bool success = false;
            var consumerTask = new PFListenEmailTask("PFEmailMqConsumerListener_" + mqConfig.getTopic(),
            new PFEmailManager(PFDataHelper.SysEmailHostName, PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd),
            email =>
            {
                //result = "{success:true}";
                PFMqMessage pfMessage = new PFMqMessage(email);
                var r = pfDeliverCallback.handle(mqConfig.getTag(), pfMessage);
                if (r != null)
                {
                    var UserEmailUserName = PFDataHelper.SysEmailUserName;
                    ////消费方回复邮件(暂不回复--benjamin)
                    PFDataHelper.SendEmail(PFDataHelper.SysEmailUserName, PFDataHelper.SysEmailPwd, PFDataHelper.SysEmailHostName,
                        new string[] { UserEmailUserName }, "PFEmailMq_consumer_Response_" + mqConfig.getTopic() + email.Body,
                       JsonConvert.SerializeObject(r));
                }
            },
            (email//, task
            ) =>
            {
                //消费方监听生产方邮件
                //return email.Subject != null && email.Subject.IndexOf("TestForceUpdateHyzl_") == 0;//这里不要用>-1,否则可能把自动回复的邮件也当作是了
                return email.Subject == "PFEmailMq_product_" + mqConfig.getTopic();
            });
            consumerTask.Start();

        }


        //	public  void BuildMqProducer(String message) {
        //        ConnectionFactory factory = new ConnectionFactory();
        //        factory.setHost("localhost");
        //        try (Connection connection = factory.newConnection();
        //             Channel channel = connection.createChannel()
        //            		 ) {
        //        	try {
        //            channel.queueDeclare(_mqConfig.getQueueName(), false, false, false, null);
        //            channel.basicPublish("", _mqConfig.getQueueName(), null, message.getBytes("UTF-8"));
        //	        String logMsg="\r\n [x][rabbitMq] queueName:"+_mqConfig.getQueueName()+" \r\n    Sent '" + message + "' \r\n";
        //	        WriteLog(logMsg);
        //        	}catch(Exception e) {

        //        	}
        //        } catch (IOException e1) {
        //			// TODO Auto-generated catch block
        //			e1.printStackTrace();
        //		} catch (TimeoutException e1) {
        //			// TODO Auto-generated catch block
        //			e1.printStackTrace();
        //		}

        //	}
        //	public  void BuildRocketMqProducer(String message) {

        ////		org.apache.rocketmq.client.producer.DefaultMQProducer producer = new org.apache.rocketmq.client.producer.DefaultMQProducer("test-group");
        ////        producer.setNamesrvAddr("localhost:9876");
        ////        producer.setInstanceName("rmq-instance");
        //		org.apache.rocketmq.client.producer.DefaultMQProducer producer = new org.apache.rocketmq.client.producer.DefaultMQProducer(_mqConfig.getGroupId());
        //        producer.setNamesrvAddr(_mqConfig.getNameSrvAddr());

        //        if(!PFDataHelper.StringIsNullOrWhiteSpace(_mqConfig.getInstanceName())) {
        //        	producer.setInstanceName(_mqConfig.getInstanceName());
        //        }

        //        try {
        //        	producer.start();
        //            org.apache.rocketmq.common.message.Message mmessage = new org.apache.rocketmq.common.message.Message(_mqConfig.getTopic(), _mqConfig.getTag(),message.getBytes());
        //            //System.out.println("生产者发送消息:"+JSON.toJSONString(user));
        //            org.apache.rocketmq.client.producer.SendResult sendResult= producer.send(mmessage);
        //            if (sendResult != null) {
        //    	        String logMsg="\r\n [x][rocketMq] topic:"+_mqConfig.getTopic()+" tag:"+_mqConfig.getTag()+" \r\n    Sent '" + message + "' \r\n";
        //    	        WriteLog(logMsg);
        //            } 
        //        } catch (Exception e) {
        //            e.printStackTrace();
        //        }
        //        producer.shutdown();
        //	}
        //public  void BuildAliMqProducer(String message) {

        //	//参考:D:\eclipse_workspace\IpaasTest\src\com\mq\simple\ProducerTest.java
        //	Properties properties = GetAliMqProperties() ;

        //       Producer producer = ONSFactory.createProducer(properties);

        //       // 在发送消息前，必须调用 start 方法来启动 Producer，只需调用一次即可
        //       producer.start();

        //       Message msg = new Message( //
        //       		_mqConfig.getTopic(),
        //       		_mqConfig.getTag(),// "*"
        //        		 message.getBytes()
        //       		 );
        //       // 设置代表消息的业务关键属性，请尽可能全局唯一。
        //       // 以方便您在无法正常收到消息情况下，可通过阿里云服务器管理控制台查询消息并补发
        //       // 注意：不设置也不会影响消息正常收发
        //       String msgKey="ORDERID_" + _mqConfig.getTopic()+ PFDataHelper.ObjectToDateString(Calendar.getInstance(), "yyyyMMddHHmmss");
        //       msg.setKey(msgKey);
        //       try {
        //           SendResult sendResult = producer.send(msg);
        //           // 同步发送消息，只要不抛异常就是成功
        //           if (sendResult != null) {
        //   	        String logMsg="\r\n [x][aliMq] topic:"+_mqConfig.getTopic()+" tag:"+_mqConfig.getTag()+" \r\n    Sent '" + message + "' \r\n";
        //   	        WriteLog(logMsg);
        //           }
        //       }
        //       catch (Exception e) {
        //           // 消息发送失败，需要进行重试处理，可重新发送这条消息或持久化这条数据进行补偿处理
        //           System.out.println("\r\n"+new Date() + " TIANGONG TEST -Send mq message failed. Topic is:" + msg.getTopic()+" \r\n");
        //           e.printStackTrace();
        //       }        

        //       // 在应用退出前，销毁 Producer 对象
        //       // 注意：如果不销毁也没有问题
        //       producer.shutdown();
        //}

        //参考TestSendEmailAsync()
        public void BuildPFEmailMqProducer(String message)
        {
            var UserEmailUserName = PFDataHelper.SysEmailUserName;
            var UserEmailPwd = PFDataHelper.SysEmailPwd;
            var UserEmailHostName = PFDataHelper.SysEmailHostName;

            ////生产方(使用User邮箱,也可以用系统邮箱吧)
            //var rt = PFDataHelper.SendEmailAsync(UserEmailUserName, UserEmailPwd, UserEmailHostName,
            //    new string[] { PFDataHelper.SysEmailUserName },
            //    _mqConfig.getTopic(), message);
            var rt = PFDataHelper.SendEmail(UserEmailUserName, UserEmailPwd, UserEmailHostName,
                new string[] { PFDataHelper.SysEmailUserName },
                "PFEmailMq_product_" + _mqConfig.getTopic(), message);


            //rt.Wait();//先不测试回调
            //var resultTitle = rt.Result.Subject;
            ////Assert.IsTrue(resultTitle == "PFEmailMq_consumer_" + producerEmailTitle);
        }

        /// <summary>
        /// 异步是要等待消费者的回复
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<PFEmail> BuildPFEmailMqProducerAsync(String message)
        {
            var rt = new Task<PFEmail>(() =>
            {

                var UserEmailUserName = PFDataHelper.SysEmailUserName;
                var UserEmailPwd = PFDataHelper.SysEmailPwd;
                var UserEmailHostName = PFDataHelper.SysEmailHostName;


                //生产方监听回复
                PFEmail result = null;
                bool hasGotResult = false;
                var st = DateTime.Now;
                //var nowStr = DateTime.Now.ToString(PFDataHelper.DateFormat);
                var producerListenTask = new PFListenEmailTask("PFEmailMqProducerListener_" + _mqConfig.getTopic(),
                new PFEmailManager(UserEmailHostName, UserEmailUserName, UserEmailPwd),
                email =>
                {
                    result = email;
                    hasGotResult = true;
                },
                (email//, task
                ) =>
                {
                    return email.Subject == "PFEmailMq_consumer_Response_" + _mqConfig.getTopic() + message;
                }, true);
                producerListenTask.Start();

                //Thread.Sleep(2000);//不延迟的话,后面太快了,前面还没开始监听(其实没问题,因为_lastListenTime是在PFListenEmailTask初始化时就赋值了
                ////生产方发邮件
                BuildPFEmailMqProducer(message);
                //PFDataHelper.SendEmail(UserEmailUserName, UserEmailPwd, UserEmailHostName,
                //    new string[] { PFDataHelper.SysEmailUserName },
                //    _mqConfig.getTopic(), message);

                while (!hasGotResult)
                {
                    if ((DateTime.Now - st).TotalHours > 1)
                    {
                        producerListenTask.Dispose();
                        result = new PFEmail();
                        result.Subject = "消费者超过1小时没有响应";
                        break;
                    }
                    Thread.Sleep(2000);
                }
                producerListenTask.NaturalStop();
                return result;
            });
            rt.Start();
            return rt;

        }

        //private Properties GetAliMqProperties() {
        //       Properties properties = new Properties();

        //       if(!PFDataHelper.StringIsNullOrWhiteSpace(_mqConfig.getGroupId())) {
        //           properties.put(PropertyKeyConst.GROUP_ID, _mqConfig.getGroupId());
        //       }
        //       if(!PFDataHelper.StringIsNullOrWhiteSpace(_mqConfig.getNameSrvAddr())) {
        //           properties.put(PropertyKeyConst.NAMESRV_ADDR, _mqConfig.getNameSrvAddr());
        //       }
        //       // 设置 TCP 接入域名（此处以公共云生产环境为例）
        //       if(!PFDataHelper.StringIsNullOrWhiteSpace(_mqConfig.getOnsAddr())) {
        //           properties.put(PropertyKeyConst.ONSAddr, _mqConfig.getOnsAddr());
        //       }
        //       // AccessKey 阿里云身份验证，在阿里云服务器管理控制台创建
        //       properties.put(PropertyKeyConst.AccessKey,_mqConfig.getAccessKey());
        //       // SecretKey 阿里云身份验证，在阿里云服务器管理控制台创建
        //       properties.put(PropertyKeyConst.SecretKey, _mqConfig.getSecretKey());
        //       //设置发送超时时间，单位毫秒
        //       properties.setProperty(PropertyKeyConst.SendMsgTimeoutMillis, "3000");

        //       if("CLUSTERING".equals(_mqConfig.getMessageModel())) {
        //           properties.put(PropertyKeyConst.MessageModel, PropertyValueConst.CLUSTERING);
        //       }else if("BROADCASTING".equals(_mqConfig.getMessageModel())) {
        //       	properties.put(PropertyKeyConst.MessageModel, PropertyValueConst.BROADCASTING);
        //       }

        //       return properties;
        //} 
        private void WriteLog(String logMsg)
        {
            PFDataHelper.WriteLog(logMsg);
            //System.out.println(logMsg);
        }
    }

    public class PFMqHelper
    {
        public enum PFMqType
        {
            AliMq, RabbitMq, RocketMq, PFEmailMq
        }
        //@FunctionalInterface
        public interface PFConsumerTask
        {
            void handle(String consumerTag, PFMqMessage message);
            PFMqConfig GetMqConfig(PFMqConfig mqConfig);

        }
        /// <summary>
        /// 消费者会回复生产者
        /// </summary>
        public interface PFConsumerResponseTask
        {
            /// <summary>
            /// 返回null时,不会回应producer;返回数据时,把发送数据给producer(producer需要使用PFMqHelper.BuildProducerAsync()方法)
            /// </summary>
            /// <param name="consumerTag"></param>
            /// <param name="message"></param>
            /// <returns>响应producer的数据(注意:返回的对象会经过JsonConvert.SerializeObject()转换)</returns>
            object handle(String consumerTag, PFMqMessage message);
            PFMqConfig GetMqConfig(PFMqConfig mqConfig);

        }
        public interface PFProdutTask
        {
            PFMqConfig GetMqConfig(PFMqConfig mqConfig);

        }
        /// <summary>
        /// 生产者需要等待消费者回复的
        /// </summary>
        public interface PFProdutResponseTask : PFProdutTask
        {
        }

        private static PFMqType _mqType = PFMqType.RabbitMq;
        //private static PFMqConfig _mqConfig;
        private static PFMqConfig _mqConfig = new PFMqConfig();
        //private static ApplicationContext _applicationContext;
        //@Autowired
        public PFMqHelper()//PFMqConfig mqConfig, ApplicationContext applicationContext)
        {
            //PFMqHelper._mqConfig = mqConfig;
            //PFMqHelper._applicationContext = applicationContext;
            //_mqType=PFMqType.valueOf(mqConfig.getMqType());
            //_mqType = mqConfig.getMqType();
            //PFMqHelper._mqConfig = new PFMqConfig();
            _mqType = _mqConfig.getMqType();
        }
        public static void BuildConsumer(PFConsumerTask task)
        {
            PFMqConfig mqConfig = task.GetMqConfig(_mqConfig.TClone());
            PFMqType mqType = mqConfig.getMqType();
            switch (mqType)
            {
                //case PFMqType.RabbitMq:
                //	(new PFMq(mqConfig)).BuildRabbitMqConsumer(task);
                //	break;
                //case PFMqType.RocketMq:
                //	(new PFMq(mqConfig)).BuildRocketMqConsumer(task);
                //	break;
                //case PFMqType.AliMq:
                //	(new PFMq(mqConfig)).BuildAliMqConsumer(task);
                //	break;
                case PFMqType.PFEmailMq:
                    (new PFMq(mqConfig)).BuildPFEmailMqConsumer(task);
                    break;
                default:
                    break;
            }
        }
        public static void BuildConsumer(PFConsumerResponseTask task)
        {
            PFMqConfig mqConfig = task.GetMqConfig(_mqConfig.TClone());
            PFMqType mqType = mqConfig.getMqType();
            switch (mqType)
            {
                //case PFMqType.RabbitMq:
                //	(new PFMq(mqConfig)).BuildRabbitMqConsumer(task);
                //	break;
                //case PFMqType.RocketMq:
                //	(new PFMq(mqConfig)).BuildRocketMqConsumer(task);
                //	break;
                //case PFMqType.AliMq:
                //	(new PFMq(mqConfig)).BuildAliMqConsumer(task);
                //	break;
                case PFMqType.PFEmailMq:
                    (new PFMq(mqConfig)).BuildPFEmailMqConsumer(task);
                    break;
                default:
                    break;
            }
        }
        public static void BuildProducer(String message)
        {
            switch (_mqType)
            {
                //case RabbitMq:
                //    (new PFMq(_mqConfig)).BuildMqProducer(message);
                //    break;
                //case RocketMq:
                //    (new PFMq(_mqConfig)).BuildRocketMqProducer(message);
                //    break;
                //case AliMq:
                //    (new PFMq(_mqConfig)).BuildAliMqProducer(message);
                //    break;
                case PFMqType.PFEmailMq:
                    (new PFMq(_mqConfig)).BuildPFEmailMqProducer(message);
                    break;
                default:
                    break;
            }
        }
        //public static void BuildProducer(String message, PFProdutTask task)
        //{
        //    PFMqConfig mqConfig = task.GetMqConfig(_mqConfig.TClone());
        //    PFMqType mqType = mqConfig.getMqType();
        //    switch (mqType)
        //    {
        //        //case RabbitMq:
        //        //	(new PFMq(mqConfig)).BuildMqProducer(message);
        //        //	break;
        //        //case RocketMq:
        //        //	(new PFMq(mqConfig)).BuildRocketMqProducer(message);
        //        //	break;
        //        //case AliMq:
        //        //	(new PFMq(mqConfig)).BuildAliMqProducer(message);
        //        //	break;
        //        case PFMqType.PFEmailMq:
        //            if (task is PFProdutTask)
        //            (new PFMq(mqConfig)).BuildPFEmailMqProducer(message);
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //public static Task<PFEmail> BuildProducerAsync(String message, PFProdutTask task)
        //{
        //    PFMqConfig mqConfig = task.GetMqConfig(_mqConfig.TClone());
        //    PFMqType mqType = mqConfig.getMqType();
        //    switch (mqType)
        //    {
        //        //case RabbitMq:
        //        //	(new PFMq(mqConfig)).BuildMqProducer(message);
        //        //	break;
        //        //case RocketMq:
        //        //	(new PFMq(mqConfig)).BuildRocketMqProducer(message);
        //        //	break;
        //        //case AliMq:
        //        //	(new PFMq(mqConfig)).BuildAliMqProducer(message);
        //        //	break;
        //        case PFMqType.PFEmailMq:
        //            return (new PFMq(mqConfig)).BuildPFEmailMqProducerAsync(message);
        //            break;
        //        default:
        //            return (new PFMq(mqConfig)).BuildPFEmailMqProducerAsync(message);
        //            break;
        //    }
        //}

        public static Task<PFEmail> BuildProducer(String message, PFProdutTask task)
        {
            PFMqConfig mqConfig = task.GetMqConfig(_mqConfig.TClone());
            PFMqType mqType = mqConfig.getMqType();
            switch (mqType)
            {
                //case RabbitMq:
                //	(new PFMq(mqConfig)).BuildMqProducer(message);
                //	break;
                //case RocketMq:
                //	(new PFMq(mqConfig)).BuildRocketMqProducer(message);
                //	break;
                //case AliMq:
                //	(new PFMq(mqConfig)).BuildAliMqProducer(message);
                //	break;
                case PFMqType.PFEmailMq:
                    if (task is PFProdutResponseTask)
                    {
                        return (new PFMq(mqConfig)).BuildPFEmailMqProducerAsync(message);
                    }
                    (new PFMq(mqConfig)).BuildPFEmailMqProducer(message);
                    break;
                default:
                    break;
            }
            return null;
        }
        public static void ListenMq()
        {
            //var mappers = AppDomain.CurrentDomain.GetAssemblies()
            //                    .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(PFConsumerTask))));
            Type[] taskTypes = new Type[] { typeof(PFConsumerTask), typeof(PFConsumerResponseTask) };
            var mappers = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Any(b => taskTypes.Contains(b))));

            //if (!mappers.Any()) { throw new Exception("必需实现接口IPFConfigMapper"); }
            foreach (var i in mappers)
            {
                var taskObj = Activator.CreateInstance(i);
                if (taskObj is PFConsumerResponseTask)
                {
                    PFConsumerResponseTask tmpObj = taskObj as PFConsumerResponseTask;
                    PFMqHelper.BuildConsumer(tmpObj);
                }
                else if (taskObj is PFConsumerTask)
                {
                    PFConsumerTask tmpObj = taskObj as PFConsumerTask;
                    PFMqHelper.BuildConsumer(tmpObj);
                }
                //PFConsumerTask tmpObj = Activator.CreateInstance(i) as PFConsumerTask;
                //PFMqHelper.BuildConsumer(tmpObj);
            }
            //var mapper = Activator.CreateInstance(mappers.First());

        }

    }

}
