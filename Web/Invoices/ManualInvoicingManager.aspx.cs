using System;
using ERP.BLL.Implement.Inventory;
using ERP.Enum.ApplyInvocie;
using ERP.Enum.Attribute;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web.Invoices
{
    public partial class ManualInvoicingManager : BasePage
    {
        private readonly InvoiceApplySerivce _invoiceApplySerivce = new InvoiceApplySerivce();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindApplyState();
                BindApplyType();
                BindApplyKindType();
            }
        }

        private void BindApplyState()
        {
            var dataSource = EnumAttribute.GetDict<ApplyInvoiceState>();
            dataSource.Remove((int) ApplyInvoiceState.WaitAudit);
            DdlState.DataSource = dataSource;
            DdlState.DataBind();
            DdlState.SelectedValue = string.Format("{0}", (int)ApplyInvoiceState.All);
        }
        private void BindApplyType()
        {
            var dataSource = EnumAttribute.GetDict<ApplyInvoiceType>();
            DdlType.DataSource = dataSource;
            DdlType.DataBind();
            DdlType.SelectedValue = string.Format("{0}", (int)ApplyInvoiceType.All);
        }

        private void BindApplyKindType()
        {
            var dataSource = EnumAttribute.GetDict<ApplyInvoiceSourceType>();
            DdlKindType.DataSource = dataSource;
            DdlKindType.DataBind();
            DdlKindType.SelectedValue = string.Format("{0}", (int)ApplyInvoiceSourceType.All);
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            Common.WebControl.RamAjajxRequest(RgManualInvoice, e);
        }



        protected void RgManualInvoiceNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RgManualInvoice.DataSource = _invoiceApplySerivce.GetInvoiceApplyMakeList(DateTime.MinValue, DateTime.MinValue, TbTradeCode.Text, Convert.ToInt32(DdlState.SelectedValue), Convert.ToInt32(DdlType.SelectedValue), Convert.ToInt32(DdlKindType.SelectedValue));
        }

        protected void LbSearchClick(object sender, EventArgs e)
        {
            RgManualInvoice.CurrentPageIndex = 0;
            RgManualInvoice.Rebind();
        }
    }
}