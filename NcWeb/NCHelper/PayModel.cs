using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCHelper
{
    public class PayModel
    {
        /// <summary>
        /// 银行帐号
        /// </summary>
        public string RecAccNo { get; set; }
        /// <summary>
        /// 帐号中文名
        /// </summary>
        public string RecAccNameCN { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public int PayAmt { get; set; }
    }
}
