using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.Cache;
using ERP.DAL.Implement.Basis;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IBasis;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using Invoice = ERP.DAL.Implement.Inventory.Invoice;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    ///<summary> 订单信息
    ///</summary>
    public partial class ShowGoodsOrder : WindowsPage
    {
        private static readonly IWebRudder _webRudder = new WebRudder(GlobalConfig.DB.FromType.Read);
        private static readonly IInvoice _invoice = new Invoice(GlobalConfig.DB.FromType.Read);
        private static readonly IGoodsOrderDetail _goodsOrderDetail = new GoodsOrderDetail(GlobalConfig.DB.FromType.Read);
        private List<OrderGoodsBatchNoDTO> _orderGoodsBatchNoDtos = new List<OrderGoodsBatchNoDTO>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                OrderId = new Guid(Request.QueryString["OrderId"]);
                SetPrintMatter(OrderId);
            }
        }

        private GoodsOrderInfo MyGoodsOrderInfo
        {
            get
            {
                if (ViewState["MyGoodsOrderInfo"] == null)
                {
                    IGoodsOrder goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Read);
                    GoodsOrderInfo goodsOrderInfo = goodsOrder.GetGoodsOrder(OrderId);

                    string saleFilialeId = Request.QueryString["SaleFilialeId"];
                    if (!string.IsNullOrEmpty(saleFilialeId))
                    {
                        if (goodsOrderInfo.OrderId == Guid.Empty)
                        {
                            goodsOrderInfo = OrderSao.GetGoodsOrderInfo(new Guid(saleFilialeId), OrderId);
                        }
                    }
                    ViewState["MyGoodsOrderInfo"] = goodsOrderInfo;
                }
                return (GoodsOrderInfo)ViewState["MyGoodsOrderInfo"];
            }
        }


        private void SetPageTextValue()
        {
            LB_Score.Text = string.Format("{0}", (int)WebControl.GetTotalOrderCurrency(MyGoodsOrderInfo.OrderId, 2));
            Lit_TotalPrice.Text = WebControl.NumberSeparator(MyGoodsOrderInfo.TotalPrice);
            Lit_Carriage.Text = Math.Abs(MyGoodsOrderInfo.Carriage).ToString("0.##");
            Lit_PaymentByBalance.Text = WebControl.NumberSeparator(MyGoodsOrderInfo.PaymentByBalance);
            Lit_VoucherValue.Text = MyGoodsOrderInfo.PromotionValue.ToString("0.##");
            Lit_RealTotalPrice.Text = WebControl.NumberSeparator(Math.Abs(MyGoodsOrderInfo.RealTotalPrice));
            Lit_PaidUp.Text = WebControl.NumberSeparator(Math.Abs(MyGoodsOrderInfo.PaidUp));
            Lit_OrderNo.Text = MyGoodsOrderInfo.OrderNo;
            Lit_Consignee.Text = MyGoodsOrderInfo.Consignee;
            Lit_OrderTime.Text = string.Format("{0}", MyGoodsOrderInfo.OrderTime);
            Lit_Direction.Text = MyGoodsOrderInfo.Direction;
            Lit_PostalCode.Text = MyGoodsOrderInfo.PostalCode;
            Lit_Phone.Text = MyGoodsOrderInfo.Phone;
            Lit_Mobile.Text = MyGoodsOrderInfo.Mobile;
            Lit_PayMode.Text = GetPayMode(MyGoodsOrderInfo.PayMode);
            Lit_PayState.Text = GetPayState(MyGoodsOrderInfo.PayState);
            Lit_BankAccounts.Text = MyGoodsOrderInfo.PayMode == (int)PayMode.COD ? string.Empty : BankAccountManager.ReadInstance.GetBankAccountName(MyGoodsOrderInfo.BankAccountsId);
            Lab_Express.Text = MyGoodsOrderInfo.ExpressId != Guid.Empty ? Express.Instance.Get(MyGoodsOrderInfo.ExpressId).ExpressFullName : string.Empty;
            LB_InvoiceState.Text = EnumAttribute.GetKeyName((InvoiceState)MyGoodsOrderInfo.InvoiceState);
            LB_ConsignTime.Text = MyGoodsOrderInfo.ConsignTime == DateTime.MinValue ? "" : string.Format("{0}", MyGoodsOrderInfo.ConsignTime);
            LB_OrderState.Text = GetOrderState(MyGoodsOrderInfo.OrderState);
            LB_FromsourceId.Text = CacheCollection.Filiale.Get(MyGoodsOrderInfo.SaleFilialeId).Name;
            var wInfo = WarehouseManager.Get(MyGoodsOrderInfo.DeliverWarehouseId);
            lblConsigneeWarehouse.Text = wInfo == null ? "-" : wInfo.WarehouseName;
            Lit_RefundmentMode.Text = GetRefundmentMode(MyGoodsOrderInfo.RefundmentMode);
            Lit_Memo.Text = MyGoodsOrderInfo.Memo;
            if (!string.IsNullOrEmpty(MyGoodsOrderInfo.PromotionDescription))
            {
                VoucherExplainTab.Visible = true;
                LB_VoucherExplain.Text = MyGoodsOrderInfo.PromotionDescription;
            }
        }


        /// <summary> 订单支付流水信息
        /// </summary>
        private void GoodsOrderPayShow()
        {
            Guid orderId = MyGoodsOrderInfo.OrderId;

            InvoiceShow();
            var str = new StringBuilder();
            IList<GoodsOrderPayInfo> infos = OrderSao.GetGoodsOrderPayListByOrderId(MyGoodsOrderInfo.SaleFilialeId, orderId);
            if (infos != null)
            {
                if (infos.Count > 0)
                {
                    str.Append("<table class=\"PanelArea\" border=\"1\" bordercolor=\"#cccccc\"><thead><tr style=\"height\"><th width=\"19%\">流水号</th><th width=\"19%\">支付号</th><th width=\"8%\">支付金额</th><th width=\"10%\">支付银行</th><th width=\"8%\">支付状态</th><th width=\"6%\">状态</th><th width=\"10%\">支付时间</th><th width=\"10%\">创建时间</th></tr></thead><tbody>");
                    foreach (GoodsOrderPayInfo gop in infos)
                    {
                        str.Append("<tr>");
                        str.AppendFormat("<td>{0}</td>", gop.PaidNo);
                        str.AppendFormat("<td>{0}</td>", gop.BankTradeNo);
                        str.AppendFormat("<td>{0}</td>", gop.PaiSum);
                        var binfo = BankAccountManager.ReadInstance.Get(gop.BankAccountId);
                        if (binfo != null)
                        {
                            str.AppendFormat("<td>{0}</td>", binfo.BankName);
                        }
                        str.AppendFormat("<td>{0}</td>", EnumAttribute.GetKeyName((PayState)gop.PayState));
                        str.AppendFormat("<td>{0}</td>", EnumAttribute.GetKeyName((GoodsOrderPayState)gop.State));
                        str.AppendFormat("<td>{0}</td>", gop.PaidTime);
                        str.AppendFormat("<td>{0}</td>", gop.CreationDate);
                        str.Append("</tr>");
                    }
                    str.Append("</tbody></table>");
                    showGoodsOrderPay.InnerHtml = str.ToString();
                    showGoodsOrderPay.Visible = true;
                }
            }
        }

        protected Guid OrderId
        {
            get
            {
                return new Guid(ViewState["OrderId"].ToString());
            }
            set
            {
                ViewState["OrderId"] = value;
            }
        }

        private void SetPrintMatter(Guid orderId)
        {

            Lit_SumPrice.Text = (MyGoodsOrderInfo.TotalPrice + MyGoodsOrderInfo.Carriage).ToString("0.##");

            InvoiceInfo invoiceInfo = _invoice.GetInvoiceByGoodsOrder(orderId);
            if (invoiceInfo != null && invoiceInfo.InvoiceId != Guid.Empty)
            {
                Lit_InvoiceName.Text = invoiceInfo.InvoiceName;
                Lit_AcceptedTime.Text = invoiceInfo.AcceptedTime == DateTime.MinValue ? "未开具" : invoiceInfo.AcceptedTime.ToString("yyyy年MM月dd日 HH时mm分ss秒");
            }

            var webRudderInfo = _webRudder.GetWebRudder();
            Lit_FWebName.Text = webRudderInfo.WebName + " " + webRudderInfo.WebUrl;
            if (!FilialeHelper.IsEntityShop(MyGoodsOrderInfo.SalePlatformId))
                GoodsOrderPayShow();

            SetPageTextValue();
        }


        /// <summary> 发票显示
        /// </summary>
        private void InvoiceShow()
        {
            if (MyGoodsOrderInfo.InvoiceState > (int)InvoiceState.NoRequest)//申请发票
            {
                showinvoice.Visible = true;
                InvoiceInfo invinfo = _invoice.GetInvoiceByGoodsOrder(MyGoodsOrderInfo.OrderId);
                Lit_InvoiceName.Text = invinfo.InvoiceName;
                Lbl_Standard.Text = invinfo.InvoiceContent;
                Lbl_InvoiceState.Text = EnumAttribute.GetKeyName((InvoiceState)invinfo.InvoiceState);
                Lbl_InvoiceStartTime.Text = string.Format("{0}", invinfo.RequestTime);
                Lbl_Money.Text = string.Format("{0}", invinfo.InvoiceSum);
                Lit_AcceptedTime.Text = invinfo.AcceptedTime == DateTime.MinValue ? string.Empty : string.Format("{0}", invinfo.AcceptedTime);
            }
        }

        protected void RGGoodsOrderDetail_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var goodsOrderDetail = _goodsOrderDetail.GetGoodsOrderDetailList(OrderId, MyGoodsOrderInfo.OrderTime);

            _orderGoodsBatchNoDtos = WMSSao.GetOrderGoodsBatchNo(MyGoodsOrderInfo.OrderNo);
            foreach (var detailGroup in goodsOrderDetail.GroupBy(ent=>ent.RealGoodsID))
            {
                var order=_orderGoodsBatchNoDtos.FirstOrDefault(ent => ent.RealGoodsId == detailGroup.Key);
                foreach (var detail in detailGroup)
                {
                    detail.DemandQuantity = order!=null && order.StockQuantity < 0 ? order.StockQuantity : (int)detail.Quantity;
                }
            }
            RGGoodsOrderDetail.DataSource = goodsOrderDetail;
        }
        protected void RGGoodsOrderDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var realGoodsId = new Guid(((GridDataItem)e.Item).GetDataKeyValue("RealGoodsId").ToString());
                var batachNoInfo = _orderGoodsBatchNoDtos.FirstOrDefault(p => p.RealGoodsId.Equals(realGoodsId));
                if (batachNoInfo != null)
                {
                    var litEffectiveDate = (Literal)e.Item.FindControl("lit_EffectiveDate");
                    var litBatchNo = (Literal)e.Item.FindControl("lit_BatchNo");
                    litEffectiveDate.Text = batachNoInfo.ExpiryDate;
                    litBatchNo.Text = batachNoInfo.BatchNos;
                }

            }
        }

        protected string GetSubtotal(object subtotal, object sellType)
        {
            return string.Format("{0}{1}", WebControl.NumberSeparator(subtotal), WebControl.GetUnitsBySellType(int.Parse(sellType.ToString())));
        }

        private static string GetPayState(int payState)
        {
            return EnumAttribute.GetKeyName((PayState)payState);
        }

        private static string GetRefundmentMode(int refundmentMode)
        {
            return EnumAttribute.GetKeyName((RefundmentMode)refundmentMode);
        }

        private static string GetPayMode(int payMode)
        {
            return EnumAttribute.GetKeyName((PayMode)payMode);
        }

        protected string GetOrderState(int orderState)
        {
            return EnumAttribute.GetKeyName((OrderState)orderState);
        }

        public String ShowText(object realGoodsId, object quantity,object stockQuantity)
        {
            if (MyGoodsOrderInfo.OrderState == (int)OrderState.RequirePurchase)
            {
                if (Convert.ToInt32(stockQuantity) < 0)
                {
                    return string.Format("<font color='red'>{0}</font>", quantity);
                }
                return string.Format("{0}", quantity);
            }
            return string.Format("{0}",quantity);
        }
    }
}