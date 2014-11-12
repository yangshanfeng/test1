using System;
using System.Collections.Generic;

namespace NCHelper
{
    public class Payment
    {
        //私有字段
        private string _TransCode = "PAYENT";
        private string _CIS = "140890000472038";
        private string _BankCode = "102";
        private string _ID = "jmwtest.y.1408";
        private string _fSeqno = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        private string _SignTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");

        /// <summary>
        /// 交易代码
        /// </summary>
        public string TransCode
        {
            get
            {
                return _TransCode;
            }
            set
            {
                _TransCode = value;
            }
        }

        /// <summary>
        /// 集团CIS号
        /// </summary>
        public string CIS
        {
            get
            {
                return _CIS;
            }
            set
            {
                _CIS = value;
            }
        }

        /// <summary>
        /// 归属银行编号
        /// </summary>
        public string BankCode
        {
            get
            {
                return _BankCode;
            }
            set
            {
                _BankCode = value;
            }
        }

        /// <summary>
        /// 证书ID
        /// </summary>
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        /// <summary>
        /// 交易日期 格式是yyyyMMdd
        /// </summary>
        public string TranDate { get; set; }

        /// <summary>
        /// 交易时间 格式如HHmmssSSS，精确到毫秒；
        /// </summary>
        public string TranTime { get; set; }

        /// <summary>
        /// 指令包序列号,必须唯一
        /// </summary>
        public string fSeqno
        {
            get
            {
                return _fSeqno;
            }
            set
            {
                _fSeqno = value;
            }
        }

        /// <summary>
        /// 联机批量标志 1：联机
        /// </summary>
        public string OnlBatF;

        /// <summary>
        /// 入账方式 2：并笔入账 0：逐笔记账
        /// </summary>
        public string SettleMode;

        /// <summary>
        /// 总笔数 指令包内的指令笔数
        /// </summary>
        public string TotalNum { get; set; }

        /// <summary>
        /// 总金额 无正负号，不带小数点，以分作单位
        /// </summary>      
        public string TotalAmt { get; set; }

        /// <summary>
        /// 签名时间 格式是yyyyMMddHHmmssSSS
        /// </summary>
        public string SignTime
        {
            get
            {
                return _SignTime;
            }
            set
            {
                _SignTime = value;
            }
        }

        /// <summary>
        ///  备用，目前无意义
        /// </summary>
        public string ReqReserved1 { get; set; }

        /// <summary>
        ///  备用，目前无意义
        /// </summary>
        public string ReqReserved2 { get; set; }

        public List<Order> OrderList { get; set; }
    }
    public class Order
    {
        //私有字段
        private string _iSeqno = "1";
        private string _CurrType = "001";
        private string _PayAccNo = "1408010919007004567";
        private string _PayAccNameCN = NCWebHelper.ZHUrlEncode("约困咒磁比屠啡听屁");

        /// <summary>
        /// 指令顺序号 每笔指令的序号，本包内不重复。（工行只检查包内不重复，不同的包，工行不做指令顺序号重复性的检查。）
        /// </summary>
        public string iSeqno
        {
            get
            {
                return _iSeqno;
            }
            set
            {
                _iSeqno = value;
            }
        }
        /// <summary>
        /// 自定义序号
        /// </summary>
        public string ReimburseNo { get; set; }
        /// <summary>
        /// 单据张数
        /// </summary>
        public string ReimburseNum { get; set; }
        /// <summary>
        /// 定时启动日期 格式是yyyyMMdd
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 定时启动时间 格式是HHmmss
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 记账处理方式 1：加急 2：普通
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 本方账号  必须为数字
        /// </summary>
        public string PayAccNo
        {
            get
            {
                return _PayAccNo;
            }
            set
            {
                _PayAccNo = value;
            }
        }

        /// <summary>
        /// 本方账户名称 根据人行标准，人民币账户的户名不应超过60字节，否则该字段可能被截取
        /// </summary>
        public string PayAccNameCN
        {
            get
            {
                return _PayAccNameCN;
            }
            set
            {
                _PayAccNameCN = value;
            }
        }

        /// <summary>
        /// 本方账户英文名称 中文名称、英文名称二者必输其一。
        /// </summary>
        public string PayAccNameEN { get; set; }

        /// <summary>
        /// 对方账号
        /// </summary>
        public string RecAccNo { get; set; }

        /// <summary>
        /// 对方账户名称 根据人行标准，人民币账户的户名不应超过60字节，否则该字段可能被截取
        /// </summary>
        public string RecAccNameCN { get; set; }

        /// <summary>
        /// 对方账户英文名称 中文名称、英文名称二者必输其一。
        /// </summary>
        public string RecAccNameEN { get; set; }

        /// <summary>
        /// 系统内外标志 1：系统内 2：系统外（使用枚举: 系统内外标志）
        /// </summary>
        public string SysIOFlg { get; set; }

        /// <summary>
        /// 同城异地标志 1：同城 2：异地 (枚举:同城异地标志)
        /// </summary>
        public string IsSameCity { get; set; }

        /// <summary>
        /// 对公对私标志 跨行必输 0：对公账户 1：个人账户(枚举：对公对私标志)
        /// </summary>
        public string Prop { get; set; }

        /// <summary>
        /// 交易对方工行地区号 4位工行地区号：数字
        /// </summary>
        public string RecICBCCode { get; set; }

        /// <summary>
        /// 收款方所在城市名称 跨行指令此项必输
        /// </summary>
        public string RecCityName { get; set; }

        /// <summary>
        /// 对方行行号 非跨行支付时，此项上送空值
        /// </summary>
        public string RecBankNo { get; set; }

        /// <summary>
        /// 交易对方银行名称 跨行指令此项必输,中文，60位字符。
        /// </summary>
        public string RecBankName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string CurrType
        {
            get
            {
                return _CurrType;
            }
            set
            {
                _CurrType = value;
            }
        }

        /// <summary>
        /// 金额 无正负号，不带小数点，以分作单位
        /// </summary>
        public string PayAmt { get; set; }

        /// <summary>
        /// 用途代码
        /// </summary>
        public string UseCode { get; set; }

        /// <summary>
        /// 用途中文描述 如需跨行实时到帐此项最多10个字符.超长则落地处理.对私情况,根据人行要求,用途和英文备注必输其一.
        /// </summary>
        public string UseCN { get; set; }

        /// <summary>
        /// 英文备注 必须ASCII字符
        /// </summary>
        public string EnSummary { get; set; }

        /// <summary>
        /// 附言 如果是跨行交易，目前最多只支持60个字符。
        /// </summary>
        public string PostScript { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 业务编号（业务参考号） 必须ASCII字符
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 相关业务编号 必须ASCII字符
        /// </summary>
        public string Oref { get; set; }

        /// <summary>
        /// ERP流水号 必须ASCII字符
        /// </summary>
        public string ERPSqn { get; set; }

        /// <summary>
        /// 业务代码 必须ASCII字符
        /// </summary>
        public string BusCode { get; set; }
        /// <summary>
        /// ERP支票号 必须ASCII字符
        /// </summary>
        public string ERPcheckno { get; set; }
        /// <summary>
        /// 原始凭证种类 必须ASCII字符
        /// </summary>
        public string CrvouhType { get; set; }
        /// <summary>
        /// 原始凭证名称 必须ASCII字符
        /// </summary>
        public string CrvouhName { get; set; }
        /// <summary>
        /// 原始凭证号 必须ASCII字符
        /// </summary>
        public string CrvouhNo { get; set; }

        /// <summary>
        /// 请求备用字段3 付款账号行别，不输或输入102代表工行
        /// </summary>
        public string ReqReserved3 { get; set; }

        /// <summary>
        /// 请求备用字段4
        /// </summary>
        public string ReqReserved4 { get; set; }
    }
}