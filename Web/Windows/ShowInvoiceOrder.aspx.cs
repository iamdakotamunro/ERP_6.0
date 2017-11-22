using System;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShowInvoiceOrder : WindowsPage
    {
        private readonly IGoodsOrder _order = new GoodsOrder(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private Guid InvoiceId
        {
            get
            {
                return WebControl.GetGuidFromQueryString("InvoiceId");
            }
        }
    
        protected void RGGoodsOrder_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGGoodsOrder.DataSource = _order.GetInvoiceGoodsOrderList(InvoiceId);
        }

        protected string GetOrderState(int orderState)
        {
            return EnumAttribute.GetKeyName((OrderState)orderState);
        }

        //protected string GetMemberInfo()
        //{
        //    Guid memberId = WebControl.GetGuidFromQueryString("MemberId");
        //    MemberInfo memberInfo = new Member().GetMember(memberId);
        //    string retValue = "会员帐号:" + memberInfo.UserName;
        //    return retValue;
        //}
    }
}