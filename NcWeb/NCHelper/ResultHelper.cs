using System;
using System.Xml;
/*CreateDate:2013-06-01 11:06*/
/*DesignBy:momo QQ:261754265*/
namespace NCHelper
{
    /// <summary>
    ///OrderHelper 的摘要说明
    /// </summary>
    public static class ResultHelper
    {

        /// <summary>
        /// 返回银企互联付款节点状态
        /// </summary>
        /// <param name="status"> </param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string CheckYqReceiveXml(string status, string xml)
        {
            string returnSta = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNodeList xnl = xmlDoc.SelectNodes("CMS/eb/out/rd");

                if (xnl != null)
                    foreach (XmlNode linkNode in xnl)
                    {
                        var xe = (XmlElement) linkNode; //将子节点类型转换为XmlElement类型
                        XmlNode selectSingleNode = xe.SelectSingleNode(status);
                        if (selectSingleNode != null) returnSta = selectSingleNode.InnerText.Trim();
                    }
            }
            catch (Exception ex)
            {
                

            }

            return returnSta;
        }

        /// <summary>
        /// 返回处理结果消息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string CheckYqRetMsgXml(string status, string xml)
        {
            string returnSta = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNodeList xnl = xmlDoc.SelectNodes("CMS/eb/pub");
                XmlNodeList xnl1 = xmlDoc.SelectNodes("CMS/eb/out/rd");
                if (xnl != null)
                    foreach (XmlNode linkNode in xnl)
                    {
                        var xe = (XmlElement) linkNode; //将子节点类型转换为XmlElement类型
                        XmlNode selectSingleNode = xe.SelectSingleNode(status);
                        if (selectSingleNode != null) returnSta = selectSingleNode.InnerText.Trim();
                    }
                if (xnl1 != null)
                    foreach (XmlNode linkNode in xnl1)
                    {
                        var xe = (XmlElement)linkNode; //将子节点类型转换为XmlElement类型
                        XmlNode selectSingleNode = xe.SelectSingleNode("iRetMsg");
                        if (selectSingleNode != null) returnSta = selectSingleNode.InnerText.Trim();
                    }
            }
            catch (Exception)
            {

                returnSta = xml;
            }

            return returnSta;
        }
    }
}
