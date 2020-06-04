using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfect
{
    public class PFConfigMapper : IPFConfigMapper
    {
        public List<PFModelConfigMapper> GetModelConfigMapper()
        {
            return new List<PFModelConfigMapper> {
    #region 对应XML节点
		        new PFModelConfigMapper
                {
                    ModelName="yjquery",
                    XmlDataSetName="yjquery"
                },
                new PFModelConfigMapper
                {
                    ModelName="newshop",
                    XmlDataSetName="newshop"
                },
                new PFModelConfigMapper
                {
                    ModelName="inv",
                    XmlDataSetName="inv"
                },
                new PFModelConfigMapper
                {
                    ModelName="order",
                    XmlDataSetName="order"
                },
                new PFModelConfigMapper
                {
                    ModelName="hyzl",
                    XmlDataSetName="hyzl"
                },
                new PFModelConfigMapper
                {
                    ModelName="tc",
                    XmlDataSetName="tc"
                },
	#endregion
    #region 按Area
		        new PFModelConfigMapper//奖金
                {
                    ModelName="bonus",
                    XmlDataSetName="bonus",
                    OtherXmlDataSetName= {  "newshop","yjquery"  }
                },
	#endregion
    #region 对应功能
                new PFModelConfigMapper
                {
                    ModelName="ConfirmOldCardActive",
                    XmlDataSetName="ConfirmOldCardActive",
                    OtherXmlDataSetName= {  "yjquery"  }
                },
	#endregion 对应功能
    #region 对应Model
		            new PFModelConfigMapper{
                    ModelName="InvoiceDBInfo",
                    XmlDataSetName="InvoiceDBInfo",
                    OtherXmlDataSetName=new List<string>{"yjquery"}
                    //ExProperty=new List<PFModelPropertyConfigMapper>{
                    //    new PFModelPropertyConfigMapper("AgentNo","yjquery","hyxm")
                    //}
                },
                new PFModelConfigMapper
                {
                    ModelName="PerformanceAnalysis",
                    XmlDataSetName="PerformanceAnalysis"
                },
                new PFModelConfigMapper
                {
                    ModelName="Hyzl",
                    XmlDataSetName="yjquery",
                    OtherXmlDataSetName= {  "hyzl" , "newshop" }
                },
                new PFModelConfigMapper
                {
                    ModelName="Agent",
                    XmlDataSetName="yjquery"
                },
                new PFModelConfigMapper
                {
                    ModelName="HyRecentShop",
                    XmlDataSetName="yjquery",
                    OtherXmlDataSetName= { { "hyzl" } }
                },
                new PFModelConfigMapper
                {
                    ModelName="HyLiBao",
                    XmlDataSetName="yjquery",
                    OtherXmlDataSetName= { { "hyzl" } }
                },
                new PFModelConfigMapper
                {
                    ModelName="HyNoActivity",
                    XmlDataSetName="HyNoActivity",
                    OtherXmlDataSetName= { { "yjquery" },{ "hyzl" } }
                },
                new PFModelConfigMapper
                {
                    ModelName="BFreeze",
                    XmlDataSetName="BFreeze"
                },
                new PFModelConfigMapper
                {
                    ModelName="Hyzl.Product",
                    XmlDataSetName="Hyzl.Product",
                    OtherXmlDataSetName= {  "hyzl","inv","yjquery","order"  }
                } ,
                new PFModelConfigMapper
                {
                    ModelName="Hyzl.Home.AgentPopups",
                    XmlDataSetName="Hyzl.Home.AgentPopups",
                    OtherXmlDataSetName= {  "yjquery" }
                } ,
                new PFModelConfigMapper
                {
                    ModelName="YJProvinceNet",
                    XmlDataSetName="bonus",
                    OtherXmlDataSetName= {  "yjquery" }
                } ,
                new PFModelConfigMapper
                {
                    ModelName="Meshwork",
                    XmlDataSetName="bonus",
                    OtherXmlDataSetName= {  "yjquery" }
                } ,
                new PFModelConfigMapper
                {
                    ModelName="UserVisitLog",
                    XmlDataSetName="UserVisitLog"
                },
                new PFModelConfigMapper
                {
                    ModelName="Customer",
                    XmlDataSetName="Customer",
                    OtherXmlDataSetName= { "yjquery" }
                } ,
                new PFModelConfigMapper
                {
                    ModelName="MobileMessage",
                    XmlDataSetName="MobileMessage"
                },
                new PFModelConfigMapper
                {
                    ModelName="DayOrders",
                    XmlDataSetName="DayOrders"
                }
	#endregion 对应Model
            };
        }

        public PFNetworkConfig GetNetworkConfig()
        {
            return new PFNetworkConfig();
        }

        public PFPathConfig GetPathConfig()
        {
            return new PFPathConfig { ConfigPath = "XmlConfig" };
        }
    }
}
