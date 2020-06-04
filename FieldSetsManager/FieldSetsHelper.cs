using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FieldSetsManager
{
    public static class FieldSetsHelper
    {
        public  static XmlNode FindField(XmlDocument xml,string dataSet,string fieldName) {
            var names = xml.SelectNodes(string.Format(@"DataSets/DataSet[@name='{0}']/Fields/Field/FieldName", dataSet));
            if (names != null)
            {
                foreach (XmlNode nameNode in names)
                {
                    if (string.IsNullOrWhiteSpace(fieldName)) { continue; }
                    if (nameNode.InnerText.ToLower() == fieldName.ToLower())
                    {
                        return nameNode.ParentNode;
                        //nameNode.ParentNode.ParentNode.RemoveChild(nameNode.ParentNode);
                        //_fieldSetsXml.Save(_xmlPath);
                        //MessageBox.Show("删除成功");
                        //RefreshGrid();
                        //break;
                    }
                }
                return null;
            }
            return null;
        }
    }
}
