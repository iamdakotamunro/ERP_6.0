using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Enum.ApplyInvocie;
using ERP.Enum.Attribute;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Invoices
{
    public partial class ApprovaInvoiceManager : BasePage
    {
        private readonly InvoiceApplySerivce _invoiceApplySerivce=new InvoiceApplySerivce();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindApplyState();
                BindApplyType();
                BindApplyKindType();
                BindSaleFiliales();
            }
        }

        #region 下拉数据绑定

        private void BindApplyState()
        {
            DdlState.DataSource = EnumAttribute.GetDict<ApplyInvoiceState>();
            DdlState.DataBind();
            DdlState.SelectedValue = string.Format("{0}", (int)ApplyInvoiceState.WaitAudit);
        }
        private void BindApplyType()
        {
            var dataSource= EnumAttribute.GetDict<ApplyInvoiceType>();
            dataSource.Remove((int) ApplyInvoiceType.Receipt);
            DdlType.DataSource = dataSource;
            DdlType.DataBind();
            DdlType.SelectedValue = string.Format("{0}", (int)ApplyInvoiceType.All);
        }

        private void BindApplyKindType()
        {
            var dataSource = EnumAttribute.GetDict<ApplyInvoiceSourceType>();
            dataSource.Remove((int)ApplyInvoiceSourceType.All);
            DdlKindType.DataSource = dataSource;
            DdlKindType.DataBind();
            DdlKindType.SelectedValue = string.Format("{0}", (int)ApplyInvoiceSourceType.League);
        }

        private void BindSaleFiliales()
        {
            List<FilialeInfo> dataSource=new List<FilialeInfo> {new FilialeInfo
            {
                ID = Guid.Empty,Name = ""
            } };
            dataSource.AddRange(FilialeManager.GetB2CFilialeList());
            DdlSaleFiliale.DataSource = dataSource;
            DdlSaleFiliale.DataBind();
            DdlSaleFiliale.SelectedValue = string.Format("{0}", Guid.Empty);
        }

        private void BindSalePlatform(Guid saleFilialeId)
        {
            List<SalePlatformInfo> dataSource = new List<SalePlatformInfo> {new SalePlatformInfo
            {
                ID = Guid.Empty,Name = ""
            } };
            if(saleFilialeId!=Guid.Empty)
                dataSource.AddRange(CacheCollection.SalePlatform.GetListByFilialeId(saleFilialeId));
            DdlSalePlatform.DataSource = dataSource;
            DdlSalePlatform.DataBind();
            DdlSalePlatform.SelectedValue = string.Format("{0}", Guid.Empty);
        }
        #endregion

        protected void DdlKindTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlKindType = DdlKindType.SelectedValue;
            var isShow = Convert.ToInt32(ddlKindType) == (int) ApplyInvoiceSourceType.League;
            LbSalePlatform.Text = isShow?"加盟店名：":"销售平台：";
            RgInvoiceAudit.Rebind();
        }

        protected void RgInvoiceAuditNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var saleFilialeId = !string.IsNullOrEmpty(DdlSaleFiliale.SelectedValue)
                ? new Guid(DdlSaleFiliale.SelectedValue)
                : Guid.Empty;
            var salePlatformId = !string.IsNullOrEmpty(DdlSalePlatform.SelectedValue)
                ? new Guid(DdlSalePlatform.SelectedValue)
                : Guid.Empty;
            RgInvoiceAudit.DataSource= _invoiceApplySerivce.GetInvoiceApplyApprovalList(DateTime.MinValue,DateTime.MinValue,TbTradeCode.Text,Convert.ToInt32(DdlState.SelectedValue),Convert.ToInt32(DdlType.SelectedValue), Convert.ToInt32(DdlKindType.SelectedValue),
                saleFilialeId, salePlatformId);
            //RgInvoiceAudit.MasterTableView.Columns[""].HeaderText = true;
        }

        public string GetFilialeName(string targetId)
        {
            return FilialeManager.GetName(new Guid(targetId));
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            Common.WebControl.RamAjajxRequest(RgInvoiceAudit, e);
        }

        protected void LbSearchClick(object sender, EventArgs e)
        {
            RgInvoiceAudit.Rebind();
        }

        protected void DdlSaleFilialeSelectedIndexChanged(object sender, EventArgs e)
        {
            var saleFilialeId = ((DropDownList) sender).SelectedValue;
            BindSalePlatform(new Guid(saleFilialeId));
        }
    }
}