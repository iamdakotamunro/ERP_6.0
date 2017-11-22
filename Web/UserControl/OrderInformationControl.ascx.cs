using System;
using System.Globalization;
using ERP.BLL.Implement.Inventory;
using ERP.Cache;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.UserControl
{
    /// <summary>
    /// 创建人：刘彩军
    /// 创建时间：2011-May-11th
    /// 作用：根据传递进来的订单ID显示订单信息
    /// </summary>
    public partial class OrderInformationControl : System.Web.UI.UserControl
    {
        private readonly IGoodsOrder _goodsOrder=new GoodsOrder(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["OrderId"]))
            {
                OrderId = new Guid(Request.QueryString["OrderId"]);
            }
            GoodsOrderInfo goodsOrderInfo = _goodsOrder.GetGoodsOrder(OrderId);
            goodsOrderInfo.ExpressNo = goodsOrderInfo.ExpressNo;  //TODO WMS 老业务有展示物流信息。
            NonceGoodsOrderInfo = goodsOrderInfo;
        }

        public Guid OrderId
        {
            get
            {
                return new Guid(ViewState["OrderId"].ToString());
            }
            set
            {
                ViewState["OrderId"] = value.ToString();
            }
        }

        protected GoodsOrderInfo NonceGoodsOrderInfo
        {
            set
            {
                if (value == null) value = new GoodsOrderInfo();
                decimal sumPrice = value.TotalPrice + value.Carriage;
                LB_Score.Text = ((int)WebControl.GetTotalOrderCurrency(value.OrderId, 2)).ToString();
                Lit_TotalPrice.Text = value.TotalPrice.ToString("0.##");
                Lit_Carriage.Text = value.Carriage.ToString("0.##");
                Lit_SumPrice.Text = sumPrice.ToString("0.##");
                Lit_PaymentByBalance.Text = value.PaymentByBalance.ToString("0.##");
                Lit_VoucherValue.Text = value.PromotionValue.ToString("0.##");
                Lit_RealTotalPrice.Text = value.RealTotalPrice.ToString("0.##");
                Lit_PaidUp.Text = value.PaidUp.ToString("0.##");
                Lit_OrderNo.Text = value.OrderNo;
                Lit_ExpressNo.Text = value.ExpressNo;
                Lit_OldCustomer.Text = value.OldCustomer == 1 ? "*" : "";
                Lit_Consignee.Text = value.Consignee;
                Lit_OrderTime.Text = value.OrderTime.ToString(CultureInfo.InvariantCulture);
                Lit_Direction.Text = value.Direction;
                Lit_PostalCode.Text = value.PostalCode;
                Lit_Phone.Text = value.Phone;
                Lit_Mobile.Text = value.Mobile;
                Lit_PayMode.Text = GetPayMode(value.PayMode);
                Lit_PayState.Text = GetPayState(value.PayState);
                Lit_BankAccounts.Text = BankAccountManager.ReadInstance.GetBankAccountName(value.BankAccountsId);
                Lit_RefundmentMode.Text = GetRefundmentMode(value.RefundmentMode);
                Lit_Memo.Text = value.Memo;
                Lit_Express.Text = value.ExpressId != Guid.Empty ? Express.Instance.Get(value.ExpressId).ExpressFullName : string.Empty;
                Lit_ConsignTime.Text = value.ConsignTime.ToString(CultureInfo.InvariantCulture);
                LB_Invoice.Text = EnumAttribute.GetKeyName((InvoiceState)value.InvoiceState);
            }
        }

        private string GetPayState(int payState)
        {
            return EnumAttribute.GetKeyName((PayState)payState);
        }

        private string GetRefundmentMode(int refundmentMode)
        {
            return EnumAttribute.GetKeyName((RefundmentMode)refundmentMode);
        }

        private string GetPayMode(int payMode)
        {
            return EnumAttribute.GetKeyName((PayMode)payMode);
        }
    }
}