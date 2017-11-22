using System;
using System.Linq;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;

namespace ERP.UI.Web.Windows
{
    public partial class CompanyFundReceiptCancel : WindowsPage
    {
        private readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice = new CompanyFundReceiptInvoice(GlobalConfig.DB.FromType.Write);

        #region -- Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var receiptID = Request.QueryString["ID"];
                var receiptNo = Request.QueryString["No"];
                lbReceiptNo.Text = receiptNo;
                if (Request.QueryString["ID"] == null || receiptID.Trim() == "")
                {
                    RAM.ResponseScripts.Add("alert('获取单据ID错误！');CancelWindow();");
                }
                else
                {
                    HF_ReceiptID.Value = receiptID;
                    //查询该单据发票除”未提交“以外的所有状态的发票信息
                    var invoiceList = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(new Guid(receiptID)).Where(p => (new int?[] { (int)CompanyFundReceiptInvoiceState.Submit, (int)CompanyFundReceiptInvoiceState.Receive, (int)CompanyFundReceiptInvoiceState.Authenticate, (int)CompanyFundReceiptInvoiceState.Verification }).Contains(p.InvoiceState));
                    if (!invoiceList.Any())//该单据所有的发票都是”未提交“状态
                    {
                        CompanyFundReceiptInfo model = _companyFundReceipt.GetCompanyFundReceiptInfo(new Guid(receiptID));
                        var erpFilialeId = ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID");
                        if (!erpFilialeId.ToLower().Equals(model.FilialeId.ToString()))
                        {
                            Lit_Msg.Text = "该单据对应的发票将一同删除！";
                        }
                    }
                }
            }
        }
        #endregion

        #region -- 点击单据作废
        protected void LB_Accept_OncLick(object sender, EventArgs e)
        {
            var receiptId = new Guid(HF_ReceiptID.Value);

            CompanyFundReceiptInfo model = _companyFundReceipt.GetCompanyFundReceiptInfo(receiptId);
            if (model.ReceiptStatus != (int)CompanyFundReceiptState.WaitAuditing &&
                model.ReceiptStatus != (int)CompanyFundReceiptState.NoAuditing &&
                model.ReceiptStatus != (int)CompanyFundReceiptState.NoAuditingPass &&
                model.ReceiptStatus != (int)CompanyFundReceiptState.WaitInvoice)
            {
                RAM.Alert("状态已更新，不允许此操作！");
                return;
            }

            #region 如果该单据有”已提交“的发票，则该单据不能作废
            var invoiceList = _companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(receiptId).Where(p => (new int?[] { (int)CompanyFundReceiptInvoiceState.Submit, (int)CompanyFundReceiptInvoiceState.Receive, (int)CompanyFundReceiptInvoiceState.Authenticate, (int)CompanyFundReceiptInvoiceState.Verification }).Contains(p.InvoiceState));
            if (invoiceList.Any())
            {
                RAM.Alert("该单据有已操作的发票，不能作废！");
                return;
            }
            #endregion

            bool isSuccess = false;
            if (receiptId != Guid.Empty)
            {
                isSuccess = _companyFundReceipt.UpdateToReceiptStatus(receiptId, (int)CompanyFundReceiptState.Cancel);
            }
            if (isSuccess)
            {
                #region 如果该单据所有的发票都是”未提交“状态，作废单据的同时，将删除相对应的发票信息
                if (!string.IsNullOrEmpty(Lit_Msg.Text))
                {
                    _companyFundReceiptInvoice.Deletelmshop_CompanyFundReceiptInvoiceByReceiptID(receiptId);
                }
                #endregion

                string remarkContent = WebControl.RetrunUserAndTime("设置单据作废！");
                _companyFundReceipt.InsertToRemark(receiptId, remarkContent);
                //往来收付款作废增加操作记录添加
                var receiptNo = lbReceiptNo.Text;

                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receiptId, receiptNo,
                    OperationPoint.CurrentReceivedPayment.Cancel.GetBusinessInfo(), string.Empty);

                //IsFromShop(receiptID, CompanyFundReceiptState.Cancel, remarkContent);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                RAM.Alert("设置单据作废失败！");
            }
        }
        #endregion

        #region  判断是否是门店的收付款往来帐
        //public void IsFromShop(Guid receiptId,CompanyFundReceiptState status,string remark)
        //{
        //    var reciptInfo = new ERP.BLL.Implement.Inventory.CompanyFundReceipt().GetCompanyFundReceiptInfo(receiptId);
        //    if(reciptInfo!=null)
        //    {
        //        var list = MISService.GetAllFiliales();
        //        var info = list.FirstOrDefault(act => act.ID == reciptInfo.CompanyID && act.Type == (int)FilialeType.EntityShop);
        //        if (info != null)
        //        {
        //            //bool result = ReckoningSao.UpdateReceiptStatus(reciptInfo.FilialeId, receiptId, remark, (int)status);
        //            //if (!result)
        //            //{
        //            //    RAM.Alert("往来帐收付款状态同步到实体门店失败");
        //            //}
        //        }
        //    }
        //}

        #endregion
    }
}
