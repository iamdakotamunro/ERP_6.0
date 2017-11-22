using System;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 订单管理意见
    /// </summary>
    public partial class ShowClewForm : WindowsPage
    {
        private readonly IOperationLogManager _operationLogManager = new OperationLogManager();

        static readonly DAL.Implement.Company.TemplateManage _templateManage = new DAL.Implement.Company.TemplateManage(GlobalConfig.DB.FromType.Read);
        private readonly IGoodsOrder _goodsOrder=new GoodsOrder(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GoodsOrderInfo info = _goodsOrder.GetGoodsOrder(OrderId);
                if (info == null || info.OrderId == Guid.Empty)
                {
                    info = OrderSao.GetGoodsOrderInfo(SaleFilialeId, OrderId);
                }
                if (info.OrderId != Guid.Empty)
                {
                    var extensions = _operationLogManager.GetOperationLogList(info.OrderId).OrderBy(w => w.OperateTime).ToList();
                    RP_Clew.DataSource = extensions;
                    RP_Clew.DataBind();
                }
                foreach (TemplateInfo t in _templateManage.GetTemplateList())
                {
                    if (t.TemplateType == 1 && t.TemplateState == 1)
                    {
                        RCB_Clew.Items.Add(new RadComboBoxItem(t.TemplateCaption, t.TemplateContent));
                    }
                }
                OldGoodsOrderInfo = info;
            }
        }

        protected Guid OrderId
        {
            get
            {
                return WebControl.GetGuidFromQueryString("OrderId");
            }
        }
        protected Guid SaleFilialeId
        {
            get
            {
                return WebControl.GetGuidFromQueryString("SaleFilialeId");
            }
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

        /// <summary>
        /// 原订单信息
        /// </summary>
        protected GoodsOrderInfo OldGoodsOrderInfo
        {
            set { ViewState["OldGoodsOrderInfo"] = value; }
            get
            {
                if (ViewState["OldGoodsOrderInfo"] == null)
                    return null;
                return (GoodsOrderInfo)ViewState["OldGoodsOrderInfo"];
            }
        }

        protected void RCB_Clew_SelectedIndexChanged(object sender, EventArgs e)
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
            string clew = RCB_Clew.Text.Trim();
            if (!string.IsNullOrEmpty(clew))
            {
                try
                {
                    //订单受理添加到管理系统——自定义
                    var personnelInfo = CurrentSession.Personnel.Get();
                    var result = WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, OldGoodsOrderInfo.OrderId, OldGoodsOrderInfo.OrderNo,
                                    OperationPoint.OrderProcess.HandAddMemo.GetBusinessInfo(), clew);
                    RAM.ResponseScripts.Add(result ? "CancelWindow()" : "添加管理意见失败!");
                }
                catch
                {
                    RAM.Alert("添加管理意见失败！");
                }
            }
        }

        protected void RP_Clew_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var extension = (OperationLogInfo)e.Item.DataItem;
                var clewContent = extension.IsHand ? "@手动编辑-" : "@系统记录-";
                clewContent += "[" + extension.OperateTime + "][" + extension.OperatorName + "]" + " * " + extension.Description;

                ((Literal)e.Item.FindControl("Lit_Clew")).Text = clewContent.Length > 50 ? clewContent.Substring(0, 50) + "......" : clewContent;
                ((Label)e.Item.FindControl("Lab_ClewDetail")).Text = clewContent;
            }
        }
    }
}