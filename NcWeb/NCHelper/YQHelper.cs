using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace NCHelper
{
    /// <summary>
    ///YQHelper 的摘要说明
    /// </summary>
    public class YqHelper
    {
       
        /// <summary>
        /// 中文UrlEncode编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ZHUrlEncode(string s)
        {
            return System.Web.HttpUtility.UrlEncode(s,Encoding.GetEncoding("gbk"));
        }
        /// <summary>
        /// 获得当前网站绝对路径
        /// </summary>
        /// <returns></returns>
        public static string GetStartPath()
        {
            string path = HttpContext.Current.Server.MapPath("~/");
            return path;
        }
        /// <summary>
        /// 发送数据到NC
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string SendToNC(Payment model)
        {
            string resultStr = "";
            //日志
            string FileName = GetStartPath() + "LOG\\Script.txt";
            var sw = new StreamWriter(FileName, false, Encoding.GetEncoding("gb2312"));
            sw.WriteLine("Template");
            sw.WriteLine("TransCode|{0}", model.TransCode);
            sw.WriteLine("CIS|{0}", model.CIS);
            sw.WriteLine("BankCode|{0}", model.BankCode);
            sw.WriteLine("ID|{0}", model.ID);
            sw.WriteLine("fSeqno|{0}", model.fSeqno);

            sw.WriteLine("Ordertype|{0}", "1");
            sw.Close();
            var result = new List<string>();
            //发送数据到NC，并返回数据
            string xmlResult = RunScript(FileName, ref result, model);
            //处理返回数据
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
                    resultStr = resultStr + "\\n"+ retMsg;
            }
            return resultStr;
        }

        
        /// <summary>
        /// 获取查询返回数据里的状态
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static string GetRejectMood(int res)
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
        /// 返回XML信息结构
        /// </summary>
        /// <param name="scriptFileName"></param>
        /// <param name="results"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RunScript(string scriptFileName, ref List<string> results, Payment model)
        {
            //获取xml数据模版
            string temp;
            var sr = new StreamReader(scriptFileName, Encoding.GetEncoding("gb2312"));
            string str;
            string transCode = sr.ReadLine();
            string xmlFileName = GetStartPath() + "XML\\" + transCode + ".txt";

            var sr2 = new StreamReader(xmlFileName, Encoding.GetEncoding("gb2312"));
            string xmlString = sr2.ReadToEnd();
            sr2.Close();
            //替换主要信息
            while ((str = sr.ReadLine()) != null)
            {
                int p = str.IndexOf('|');
                XmlReplace(ref xmlString, str.Substring(0, p), str.Substring(p + 1));
            }
            sr.Close();

            bool needSign = xmlString != null && xmlString.IndexOf("<SignTime>", StringComparison.Ordinal) > 0;
            int sec = Convert.ToInt16(15);
            //新建日志
            string currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string logDir = GetStartPath() + "LOG\\" + currentTime.Substring(0, 8);
            if (Directory.Exists(logDir) == false)
            {
                Directory.CreateDirectory(logDir);
            }

            var swLog = new StreamWriter(logDir + "\\" + currentTime.Substring(8) + "-" + transCode + ".txt",false, Encoding.GetEncoding("gb2312"));
            while (true)
            {
                string reqData = "";
                reqData = ReplacePayment(ref xmlString, model);

                string url = "http://192.168.1.139:448/servlet/ICBCCMPAPIReqServlet?userID=" +
                             model.ID + "&PackageID=" + model.fSeqno +
                             "&SendTime=" + model.SignTime;
                string cont = "Version=0.0.0.1&TransCode=" + model.TransCode + "&BankCode=102&GroupCIS=" + model.CIS +
                              "&ID=" + model.ID + "&PackageID=" + model.fSeqno + "&Cert=&reqData=" + reqData;

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
                if (ncReturn == "") ncReturn = "error未接受到银行返回信息！";

                string result;
                if (char.ToLower(ncReturn[0]) == 'e') result = ncReturn;
                else
                {
                    int x = ncReturn.IndexOf('=');
                    byte[] bytes = Convert.FromBase64String(ncReturn.Substring(x + 1));
                    result = Encoding.GetEncoding("gbk").GetString(bytes);
                    //日志
                    swLog.WriteLine("银行返回内容明文：");
                    swLog.WriteLine(result);
                    swLog.WriteLine();
                }
                results.Add(result);
                temp = result;
                int nt = result.IndexOf("<NextTag>", StringComparison.Ordinal);
                if (nt > 0)
                {
                    int nt2 = result.IndexOf("</", nt, StringComparison.Ordinal);
                    if (nt2 == nt + 9) break;
                    else
                    {
                        string nextTag = result.Substring(nt + 9, nt2 - nt - 9);
                        XmlReplace(ref xmlString, "NextTag", nextTag);
                        System.Threading.Thread.Sleep(50);
                    }
                }
                else break;

            }
            swLog.Close();

            return temp;
        }

        /// <summary>
        /// 付款支付指令接口替换
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        //private static string ReplacePayment(ref string xmlString, Payment model)
        //{
        //    XmlReplace(ref xmlString, "TranDate", model.TranDate);
        //    XmlReplace(ref xmlString, "TranTime", model.TranTime);
        //    XmlReplace(ref xmlString, "OnlBatF", model.OnlBatF.ToString());
        //    XmlReplace(ref xmlString, "SettleMode", model.SettleMode.ToString());
        //    XmlReplace(ref xmlString, "TotalNum", model.TotalNum.ToString());
        //    XmlReplace(ref xmlString, "TotalAmt", model.TotalAmt.ToString());
        //    XmlReplace(ref xmlString, "SignTime", model.SignTime);
        //    XmlReplace(ref xmlString, "ReqReserved1", model.ReqReserved1);
        //    XmlReplace(ref xmlString, "ReqReserved2", model.ReqReserved2);
        //    XmlReplace(ref xmlString, "iSeqno", model.iSeqno);
        //    XmlReplace(ref xmlString, "ReimburseNo", model.ReimburseNo);
        //    XmlReplace(ref xmlString, "ReimburseNum", model.ReimburseNum);
        //    XmlReplace(ref xmlString, "StartDate", model.StartDate);
        //    XmlReplace(ref xmlString, "StartTime", model.StartTime);
        //    XmlReplace(ref xmlString, "PayType", model.PayType.ToString());
        //    XmlReplace(ref xmlString, "PayAccNo", model.PayAccNo);
        //    XmlReplace(ref xmlString, "PayAccNameCN", model.PayAccNameCN);
        //    XmlReplace(ref xmlString, "PayAccNameEN", model.PayAccNameEN);
        //    XmlReplace(ref xmlString, "RecAccNo", model.RecAccNo);
        //    XmlReplace(ref xmlString, "RecAccNameCN", model.RecAccNameCN);
        //    XmlReplace(ref xmlString, "RecAccNameEN", model.RecAccNameEN);
        //    XmlReplace(ref xmlString, "SysIOFlg", model.SysIOFlg.ToString());
        //    XmlReplace(ref xmlString, "IsSameCity", model.IsSameCity.ToString());
        //    XmlReplace(ref xmlString, "Prop", model.Prop.ToString());
        //    XmlReplace(ref xmlString, "RecICBCCode", model.RecICBCCode);
        //    XmlReplace(ref xmlString, "RecCityName", model.RecCityName);
        //    XmlReplace(ref xmlString, "RecBankNo", model.RecBankNo);
        //    XmlReplace(ref xmlString, "RecBankName", model.RecBankName);
        //    XmlReplace(ref xmlString, "CurrType", model.CurrType);
        //    XmlReplace(ref xmlString, "PayAmt", model.PayAmt.ToString());
        //    XmlReplace(ref xmlString, "UseCode", model.UseCode);
        //    XmlReplace(ref xmlString, "UseCN", model.UseCN);
        //    XmlReplace(ref xmlString, "EnSummary", model.EnSummary);
        //    XmlReplace(ref xmlString, "PostScript", model.PostScript);
        //    XmlReplace(ref xmlString, "Summary", model.Summary);
        //    XmlReplace(ref xmlString, "Ref", model.Ref);
        //    XmlReplace(ref xmlString, "Oref", model.Oref);
        //    XmlReplace(ref xmlString, "ERPSqn", model.ERPSqn);
        //    XmlReplace(ref xmlString, "BusCode", model.BusCode);
        //    XmlReplace(ref xmlString, "ERPcheckno", model.ERPcheckno);
        //    XmlReplace(ref xmlString, "CrvouhType", model.CrvouhType);
        //    XmlReplace(ref xmlString, "CrvouhName", model.CrvouhName);
        //    XmlReplace(ref xmlString, "CrvouhNo", model.CrvouhNo);
        //    XmlReplace(ref xmlString, "ReqReserved3", model.ReqReserved3);
        //    XmlReplace(ref xmlString, "ReqReserved4", model.ReqReserved4);
        //    return xmlString;
        //}

        /// <summary>
        /// 发送xml信息到NC，并返回
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postCont"></param>
        /// <param name="timeOut"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string NcPost(string url, string postCont, int timeOut, bool sign)
        {
            Encoding encoding = Encoding.GetEncoding("gbk");
            byte[] bytesToPost = encoding.GetBytes(postCont);
            string cookieheader = string.Empty;

            var cookieCon = new CookieContainer();

            #region 创建HttpWebRequest对象

            var httpRequest = (HttpWebRequest) WebRequest.Create(url);

            #endregion

            #region 初始化HtppWebRequest对象

            httpRequest.CookieContainer = cookieCon;
            httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Method = "POST";
            httpRequest.Timeout = timeOut*1000;
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

        /// <summary>
        /// 替换模板的信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="searchStr"></param>
        /// <param name="targetStr"></param>
        public static void XmlReplace(ref string source, string searchStr, string targetStr)
        {
            searchStr = "<" + searchStr + ">";
            int p1 = source.IndexOf(searchStr, StringComparison.Ordinal);
            int p2 = source.IndexOf("</", p1 + searchStr.Length, StringComparison.Ordinal);
            if (p1 > 0 && p2 > p1)
            {
                string tmpStr = source.Substring(0, p1 + searchStr.Length) + targetStr + source.Substring(p2);
                source = tmpStr;
            }
        }

        //public static int XmlSearch(ref string source, string searchStr, int startp, out string cont)
        //{
        //    cont = "";
        //    searchStr = "<" + searchStr + ">";
        //    int p = source.IndexOf(searchStr, startp == -1 ? 0 : startp, System.StringComparison.Ordinal);
        //    if (p < 0) return p;
        //    int p2 = source.IndexOf("</", p + searchStr.Length, System.StringComparison.Ordinal);
        //    if (p > 0 && p2 > p)
        //    {
        //        cont = source.Substring(p + searchStr.Length, p2 - p - searchStr.Length);
        //    }

        //    return p2;

        //}
    }
}