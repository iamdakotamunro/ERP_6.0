using System;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Inventory;
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
    public partial class ShowPurasingClewForm : WindowsPage
    {
        private static readonly IOperationLogManager _operationLogManager =
            new OperationLogManager();
        private readonly IPurchasing _purchasing=new Purchasing(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PurchasingInfo info = _purchasing.GetPurchasingById(PurchasingID);
                if (info.PurchasingID != Guid.Empty)
                {
                    var list = _operationLogManager.GetOperationLogList(PurchasingID);
                    RP_Clew.DataSource = list;
                    RP_Clew.DataBind();
                }
            }
        }

        protected Guid PurchasingID
        {
            get { return WebControl.GetGuidFromQueryString("PurchasingID"); }
        }

        protected int UpdateType
        {
            get
            {
                if (Request.QueryString["memo"] == "1")
                    return 1;
                return 0;
            }
        }


        protected void Rcb_Clew_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RCB_Clew.SelectedIndex != -1)
            {
                string temp = RCB_Clew.SelectedValue;
                RCB_Clew.SelectedIndex = -1;
                RCB_Clew.Text = temp;
            }
        }

        protected void Button_UpdateClew(object sender, EventArgs e)
        {
            string clew = RCB_Clew.Text;
            if (!string.IsNullOrEmpty(clew))
            {
                clew = WebControl.RetrunUserAndTime(clew);

                try
                {
                    PurchasingInfo info = _purchasing.GetPurchasingById(PurchasingID);
                    //采购单备注添加操作记录添加
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, PurchasingID, info.PurchasingNo,
                        OperationPoint.PurchasingManager.Remarks.GetBusinessInfo(), clew);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch
                {
                    RAM.Alert("添加备注失败！");
                }
            }
        }

        protected void Rp_Clew_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var extension = (OperationLogInfo)e.Item.DataItem;
                var clewContent = extension.IsHand ? "@手动编辑-" : "@系统记录-";
                clewContent += "[" + extension.OperatorName + "]" + " *" + extension.Description;
                ((Literal)e.Item.FindControl("Lit_Clew")).Text = clewContent.Length > 40 ? "[" + extension.OperateTime + "]-" + clewContent.Substring(0, 40) + "......" : "[" + extension.OperateTime + "]-" + clewContent;
                ((Label)e.Item.FindControl("Lab_ClewDetail")).Text = "[" + extension.OperateTime + "]-" + clewContent;
            }
        }
    }
}