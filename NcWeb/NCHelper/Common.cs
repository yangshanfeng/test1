using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCHelper
{
    public class Common
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public static string TransCode = "PAYENT";
        /// <summary>
        /// 集团CIS号
        /// </summary>
        public static string CIS = "140890000472038";
        /// <summary>
        /// 归属银行编号
        /// </summary>
        public static string BankCode = "102";
        /// <summary>
        /// 证书ID
        /// </summary>
        public static string ID = "jmwtest.y.1408";
        /// <summary>
        /// 币种
        /// </summary>
        public static string CurrType = "001";
        /// <summary>
        /// 本方帐号
        /// </summary>
        public static string PayAccNo = "1408010919007004567";
        /// <summary>
        /// 本方账户名称
        /// </summary>
        //public static string PayAccNameCN = "约困咒磁比屠啡听屁";
        public static string PayAccNameCN = NCWebHelper.ZHUrlEncode("约困咒磁比屠啡听屁");
    }
}
