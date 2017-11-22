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
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// add by dinghq 2011-06-14
    /// </summary>
    public partial class ShowRemarkForm : WindowsPage
    {
        private static readonly IOperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly DAL.Implement.Company.TemplateManage _templateManage = new DAL.Implement.Company.TemplateManage(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //ICompanyFundReceipt companyFundReceipt=new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Read);
                //CompanyFundReceiptInfo info = companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                if (RceiptId != Guid.Empty)
                {
                    var list = _operationLogManager.GetOperationLogList(RceiptId);
                    RP_Clew.DataSource = list;
                    RP_Clew.DataBind();
                }

                foreach (TemplateInfo t in _templateManage.GetTemplateList())
                {
                    if (t.TemplateType == 1 && t.TemplateState == 1)
                    {
                        RCB_Remark.Items.Add(new RadComboBoxItem(t.TemplateCaption, t.TemplateContent));
                    }
                }
            }
        }

        protected bool IsApply
        {
            get
            {
                return Request.QueryString["Type"] != null && Request.QueryString["Type"] == "2";
            }
        }

        protected Guid RceiptId
        {
            get
            {
                return WebControl.GetGuidFromQueryString(IsApply ? "ApplyId" : "RceiptId");
            }
        }

        protected void RCB_Remark_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RCB_Remark.SelectedIndex != -1)
            {
                string temp = RCB_Remark.SelectedValue;
                RCB_Remark.SelectedIndex = -1;
                RCB_Remark.Text = temp;
            }
        }

        protected void Button_UpdateRemark(object sender, EventArgs e)
        {
            string remark = RCB_Remark.Text;
            if (!string.IsNullOrEmpty(remark))
            {
                var personnelInfo = CurrentSession.Personnel.Get();
                remark = WebControl.RetrunUserAndTime(remark);
                if (IsApply)
                {
                    var invoiceApply = new DAL.Implement.Inventory.InvoiceApply();
                    var info = invoiceApply.GetInvoiceApplyInfo(RceiptId);
                    try
                    {
                        if (info != null)
                        {
                            invoiceApply.UpdateRemark(RceiptId,remark);
                            //往来收付款备注添加到管理系统——自定义
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, RceiptId, info.TradeCode,
                                OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), remark);
                            RAM.ResponseScripts.Add("CloseAndRebind()");
                        }
                    }
                    catch
                    {
                        RAM.Alert("添加管理意见失败！");
                    }
                }
                else
                {
                    var companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
                    CompanyFundReceiptInfo info = companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                    try
                    {
                        companyFundReceipt.UpdateFundReceiptRemark(RceiptId, remark);
                        //往来收付款备注添加到管理系统——自定义
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, RceiptId, info.ReceiptNo,
                            OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), remark);
                        RAM.ResponseScripts.Add("CloseAndRebind()");
                    }
                    catch
                    {
                        RAM.Alert("添加管理意见失败！");
                    }
                }
            }
        }

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
                ((Literal)e.Item.FindControl("Lit_Clew")).Text = clewContent.Length > 40 ? "[" + extension.OperateTime + "]-" + clewContent.Substring(0, 40) + "......" : "[" + extension.OperateTime + "]-" + clewContent;
                ((Label)e.Item.FindControl("Lab_ClewDetail")).Text = "[" + extension.OperateTime + "]-" + clewContent;
            }
        }
    }
}
