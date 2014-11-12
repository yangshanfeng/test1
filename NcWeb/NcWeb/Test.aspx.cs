using NCHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NcWeb
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnCheck.Click += new EventHandler(BtnCheckClick);
        }

        void BtnCheckClick(object sender, EventArgs e)
        {
            List<PayModel> modelList = new List<PayModel>();
            PayModel pay1 = new PayModel();
            pay1.RecAccNameCN = "约困咒磁比屠啡听屁复哒鼎听屁";
            pay1.RecAccNo = "0200000519024549712";
            pay1.PayAmt = 5000;
            modelList.Add(pay1);
            //PayModel pay2 = new PayModel();
            //pay2.RecAccNameCN = "约困咒磁比屠啡听屁复哒鼎听屁";
            //pay2.RecAccNo = "0200000519024549712";
            //pay2.PayAmt = 5000;
            //modelList.Add(pay2);
            string retMsg = NCWebHelper.Pay(modelList);
            //PayOrder model = new PayOrder();
            //PayOrder.eb eb = new PayOrder.eb();
            //PayOrder.Rd order = new PayOrder.Rd()
            //{
            //    iSeqno = "1",
            //    PayType = "1",
            //    RecAccNo = "0200000519024549712",
            //    //RecAccNameCN = "约困咒磁比屠啡听屁复哒鼎听屁",
            //    RecAccNameCN = NCWebHelper.ZHUrlEncode("约困咒磁比屠啡听屁复哒鼎听屁"),
            //    SysIOFlg = "1",
            //    IsSameCity = "1",
            //    //RecBankName = "中国工商银行",
            //    RecBankName = NCWebHelper.ZHUrlEncode("中国工商银行"),
            //    CurrType = "001",
            //    PayAmt = "5000",
            //};
            //eb.@in.rds.Add(order);
            //eb.pub.fSeqno = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //eb.pub.TranDate = DateTime.Now.ToString("yyyyMMdd");
            //eb.pub.TranTime = DateTime.Now.ToString("HHmmssfff");
            //eb.@in.OnlBatF = "1";
            //eb.@in.SettleMode = "0";
            //eb.@in.TotalNum = "1";
            //eb.@in.TotalAmt = "5000";
            //eb.@in.SignTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //model.Detial = eb;
            //string retMsg = NCWebHelper.SendToNC(model);
            ScriptManager.RegisterStartupScript(Page, GetType(), "Message", "alert('" + retMsg + "');", true);
        }
    }
}