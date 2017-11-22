using System;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Interface;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class CompanyFundReceiptRemark : WindowsPage
    {
        protected string ReceiptID = string.Empty;
        readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private static readonly IOperationLogManager _operationLogManager =
            new OperationLogManager();

        #region -- Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            ReceiptID = Request.QueryString["ID"];
            HF_ReceiptID.Value = ReceiptID;
            CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(new Guid(ReceiptID));
            if (info!=null && info.ReceiptID != Guid.Empty)
            {
                var list = _operationLogManager.GetOperationLogList(new Guid(ReceiptID));
                RP_Clew.DataSource = list;
                RP_Clew.DataBind();
            }
        }
        #endregion

        #region -- 点击保存管理意见
        protected void LB_Save_OncLick(object sender, EventArgs e)
        {
            string remarkText = RTB_RemarkInput.Text.Trim();
            if (remarkText != string.Empty)
            {
                var receiptID = new Guid(HF_ReceiptID.Value);
                CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(new Guid(ReceiptID));
                if(info==null)return;
                bool success = _companyFundReceipt.InsertToRemark(receiptID, WebControl.RetrunUserAndTime(remarkText));
                if (success)
                {
                    //往来收付款备注添加到管理系统——自定义
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, receiptID, info.ReceiptNo,
                        OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), remarkText);

                    ClientScript.RegisterStartupScript(GetType(), "js", "<script>setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");</script>");
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "js", "<script>alert('添加管理意见失败！');</script>");
                }
            }
        }
        #endregion

        protected void RP_Clew_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var extension = (OperationLogInfo)e.Item.DataItem;
                var clewContent = (extension.IsHand
                                                                      ? "@手动编辑-"
                                                                      : "@系统记录-") + "[" +
                                                                       extension.OperatorName + "]" + " *" +
                                                                        extension.Description;
                ((Literal)e.Item.FindControl("Lit_Clew")).Text = clewContent.Length > 40 ?    "[" + extension.OperateTime + "]-" + clewContent.Substring(0, 40)+ "......" : "[" + extension.OperateTime + "]-" + clewContent;
                ((Label)e.Item.FindControl("Lab_ClewDetail")).Text = "[" + extension.OperateTime + "]-"+clewContent ;
            }
        }
    }
}
