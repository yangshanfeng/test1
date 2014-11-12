using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace NCHelper
{
    public class NCWebHelper
    {
        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="payModelList"></param>
        /// <returns></returns>
        public static string Pay(List<PayModel> payModelList)
        {
            PayOrder model = new PayOrder();
            PayOrder.eb eb = new PayOrder.eb();
            int i = 1, toalAmt = 0;
            foreach (PayModel p in payModelList)
            {
                PayOrder.Rd order = new PayOrder.Rd()
                {
                    iSeqno = i.ToString(),
                    PayType = "1",
                    RecAccNo = p.RecAccNo,
                    //RecAccNameCN = "约困咒磁比屠啡听屁复哒鼎听屁",
                    RecAccNameCN = ZHUrlEncode(p.RecAccNameCN),
                    SysIOFlg = "1",
                    IsSameCity = "1",
                    //RecBankName = "中国工商银行",
                    RecBankName = ZHUrlEncode("中国工商银行"),
                    CurrType = "001",
                    PayAmt = p.PayAmt.ToString(),
                };
                i++;
                toalAmt += p.PayAmt;
                eb.@in.rds.Add(order);
            }
            eb.pub.fSeqno = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            eb.pub.TranDate = DateTime.Now.ToString("yyyyMMdd");
            eb.pub.TranTime = DateTime.Now.ToString("HHmmssfff");
            eb.@in.OnlBatF = "1";
            eb.@in.SettleMode = "0";
            eb.@in.TotalNum = payModelList.Count.ToString();
            eb.@in.TotalAmt = toalAmt.ToString();
            eb.@in.SignTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            model.Detial = eb;
            return SendToNC(model);
            
        }

        /// <summary>
        /// 中文UrlEncode编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ZHUrlEncode(string s)
        {
            return System.Web.HttpUtility.UrlEncode(s, Encoding.GetEncoding("gbk"));
        }
        /// <summary>
        /// 获得当前网站绝对路径
        /// </summary>
        /// <returns></returns>
        private static string GetStartPath()
        {
            string path = HttpContext.Current.Server.MapPath("~/");
            return path;
        }

        /// <summary>
        /// 发送到nc，支付指令提交
        /// </summary>
        /// <param name="model"></param>
        /// <param name="logName"></param>
        /// <returns></returns>
        public static string SendToNC(PayOrder model)
        {
            string FileName = GetStartPath() + "LOG\\Script.txt";
            //写日志
            var sw = new StreamWriter(FileName, false, Encoding.GetEncoding("gb2312"));
            sw.WriteLine("Template");
            sw.WriteLine("TransCode|{0}", model.Detial.pub.TransCode);
            sw.WriteLine("CIS|{0}", model.Detial.pub.CIS);
            sw.WriteLine("BankCode|{0}", model.Detial.pub.BankCode);
            sw.WriteLine("ID|{0}", model.Detial.pub.ID);
            sw.WriteLine("fSeqno|{0}", model.Detial.pub.fSeqno);

            sw.WriteLine("Ordertype|{0}", "1");
            sw.Close();
            //发送数据并获得返回数据
            string reqData = SerializeToXml(model, "gb2312");

            string url = "http://192.168.1.139:448/servlet/ICBCCMPAPIReqServlet?userID=" +
                         model.Detial.pub.ID + "&PackageID=" + model.Detial.pub.fSeqno +
                         "&SendTime=" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string cont = "Version=0.0.0.1&TransCode=" + model.Detial.pub.TransCode + "&BankCode=102&GroupCIS=" + model.Detial.pub.CIS +
                          "&ID=" + model.Detial.pub.ID + "&PackageID=" + model.Detial.pub.fSeqno + "&Cert=&reqData=" + reqData;
            int sec = Convert.ToInt16(15);
            //新建日志
            string currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string logDir = GetStartPath() + "LOG\\" + currentTime.Substring(0, 8);
            if (Directory.Exists(logDir) == false)
            {
                Directory.CreateDirectory(logDir);
            }

            var swLog = new StreamWriter(logDir + "\\" + currentTime.Substring(8) + "-PayOrder.txt", false, Encoding.GetEncoding("gb2312"));
            //发送日志
            swLog.Write(currentTime);
            swLog.WriteLine("发送地址：");
            swLog.WriteLine(url);
            swLog.WriteLine();
            swLog.WriteLine("被发送内容：");
            swLog.WriteLine(cont);
            swLog.WriteLine();
            swLog.Flush();
            //获取NC返回的xml
            string ncReturn = NcPost(url, cont, sec, false);
            //日志
            swLog.Write(currentTime);
            swLog.WriteLine("银行返回内容：");
            swLog.WriteLine(ncReturn);
            swLog.WriteLine();
            string proclaimed = ProcessCiphertext(ncReturn);
            swLog.WriteLine("银行返回内容明文：");
            swLog.WriteLine(proclaimed);
            swLog.WriteLine();
            swLog.Close();
            return ProcessReturnXml(proclaimed);
        }


        /// <summary>
        /// 处理nc返回的密文
        /// </summary>
        /// <param name="ncReturn"></param>
        /// <returns></returns>
        private static string ProcessCiphertext(string ncReturn)
        {
            string result;
            if (char.ToLower(ncReturn[0]) == 'e' || char.ToLower(ncReturn[0]) == 's') result = ncReturn;
            else
            {
                int x = ncReturn.IndexOf('=');
                byte[] bytes = Convert.FromBase64String(ncReturn.Substring(x + 1));
                result = Encoding.GetEncoding("gbk").GetString(bytes);
            }
            return result;
        }

        /// <summary>
        /// 发送数据并返回，交易接口
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static string RunPost(Trade model)
        {
            return null;
        }

        /// <summary>
        /// 处理返回xml
        /// </summary>
        /// <param name="xmlResult"></param>
        /// <returns></returns>
        private static string ProcessReturnXml(string xmlResult)
        {
            string resultStr = "";
            if (!string.IsNullOrEmpty(xmlResult))
            {
                string res = ResultHelper.CheckYqReceiveXml("Result", xmlResult);//返回数字结果
                string retMsg = ResultHelper.CheckYqRetMsgXml("RetMsg", xmlResult);//返回文字消息结果
                if (!string.IsNullOrEmpty(res))
                {
                    int rs = int.Parse(res);
                    string mood = GetRejectMood(rs);
                    if (!string.IsNullOrEmpty(mood))
                    {
                        resultStr = mood;
                    }
                }
                resultStr = resultStr + "\\n" + retMsg;
            }
            if (resultStr == "\\n")
            {
                resultStr = "成功";
            }
            return resultStr;
        }
        /// <summary>
        /// 获取查询返回数据里的状态
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private static string GetRejectMood(int res)
        {
            string temp = string.Empty;
            switch (res)
            {
                case 0:
                    temp = "提交成功,等待银行处理";
                    break;
                case 1:
                    temp = "授权成功, 等待银行处理";
                    break;
                case 2:
                    temp = "等待授权";
                    break;
                case 3:
                    temp = "等待二次授权";
                    break;
                case 4:
                    temp = "等待银行答复";
                    break;
                case 5:
                    temp = "主机返回待处理";
                    break;
                case 6:
                    temp = "被银行拒绝";
                    break;
                case 7:
                    temp = "处理成功";
                    break;
                case 8:
                    temp = "指令被拒绝授权";
                    break;
                case 9:
                    temp = "银行正在处理";
                    break;
                case 98:
                    temp = "区域中心通讯可疑";
                    break;
                case 10:
                    temp = "预约指令";
                    break;
                case 11:
                    temp = "预约取消";
                    break;
                case 95:
                    temp = "待核查";
                    break;
            }
            return temp;
        }
        
        /// <summary>
        /// 序列化为XML
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static string SerializeToXml(object obj, string encoding)
        {
            if (obj != null)
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");


                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                MemoryStream ms = new MemoryStream();
                XmlTextWriter xtw = new XmlTextWriter(ms, System.Text.Encoding.GetEncoding(encoding));
                xtw.Formatting = Formatting.Indented;
                serializer.Serialize(xtw, obj, ns);

                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms, System.Text.Encoding.GetEncoding(encoding));
                string str = sr.ReadToEnd();
                xtw.Close();
                ms.Close();
                return str;
            }
            return string.Empty;
        }
        
        /// <summary>
        /// 发送xml信息到NC，并返回
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postCont"></param>
        /// <param name="timeOut"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        private static string NcPost(string url, string postCont, int timeOut, bool sign)
        {
            Encoding encoding = Encoding.GetEncoding("gb2312");
            byte[] bytesToPost = encoding.GetBytes(postCont);
            string cookieheader = string.Empty;

            var cookieCon = new CookieContainer();

            #region 创建HttpWebRequest对象

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            #endregion

            #region 初始化HtppWebRequest对象

            httpRequest.CookieContainer = cookieCon;
            httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Method = "POST";
            httpRequest.Timeout = timeOut * 1000;
            if (sign)
            {
                httpRequest.ContentType = "INFOSEC_SIGN/1.0";
                httpRequest.ContentLength = bytesToPost.Length;
            }


            if (cookieheader.Equals(string.Empty))
            {
                httpRequest.CookieContainer.GetCookieHeader(new Uri(url));
            }
            else
            {
                httpRequest.CookieContainer.SetCookies(new Uri(url), cookieheader);
            }

            #endregion

            string stringResponse = "";
            try
            {

                #region 附加Post给服务器的数据到HttpWebRequest对象

                httpRequest.ContentLength = bytesToPost.Length;
                Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                requestStream.Close();

                #endregion


                #region 读取服务器返回信息


                Stream responseStream = httpRequest.GetResponse().GetResponseStream();

                if (responseStream != null)
                {
                    using (
                        var responseReader = new StreamReader(responseStream, Encoding.GetEncoding("gbk")))
                    {
                        stringResponse = responseReader.ReadToEnd();
                    }
                    responseStream.Close();
                }

                #endregion
            }
            catch (Exception ex)
            {
                stringResponse = ex.ToString();
            }
            return stringResponse;
        }
    }
}