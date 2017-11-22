using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class InvoiceRollDetail : WindowsPage
    {
        private readonly IInvoice _invoiceDao=new Invoice(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request.QueryString["rollid"]))
                {
                    InvoiceRollId = Guid.Empty;
                }
                InvoiceRollId = new Guid(Request.QueryString["rollid"]);
            }
        }

        protected Guid InvoiceRollId
        {
            get
            {
                if (ViewState["InvoiceRollId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["InvoiceRollId"].ToString());
            }
            set { ViewState["InvoiceRollId"] = value; }
        }

        protected void RadGrid_InvoiceRollDetailList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RadGrid_InvoiceRollDetailList.DataSource = _invoiceDao.GetRollDetailList(InvoiceRollId);
        }

        protected string GetState(object state)
        {
            var stateType = (InvoiceRollState)state;
            var stateInfo = EnumAttribute.GetKeyName(stateType, string.Empty);
            return stateInfo;
        }

        protected bool IsDistribute(object state)
        {
            var stateType = (InvoiceRollState)state;
            if (stateType == InvoiceRollState.NoInvocation)
            {
                return true;
            }
            return false;
        }

        protected bool IsVisble(object state, object isSubmit)
        {
            var stateType = (InvoiceRollState)state;
            var submit = bool.Parse(isSubmit.ToString());
            if (stateType == InvoiceRollState.Lost && !submit)
            {
                return true;
            }
            return false;
        }

        protected void RadGrid_InvoiceRollDetailList_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Lost")
            {
                try
                {
                    var editedItem = e.Item as GridEditableItem;
                    if(editedItem==null)return;
                    var rollId = new Guid(editedItem.GetDataKeyValue("RollId").ToString());
                    long startNo = long.Parse(editedItem.GetDataKeyValue("StartNo").ToString());
                    long endNo = long.Parse(editedItem.GetDataKeyValue("EndNo").ToString());
                    var state = (InvoiceRollState)editedItem.GetDataKeyValue("State");
                    _invoiceDao.LostSubmit(rollId, startNo, endNo, state);
                    _invoiceDao.DeleteRollDetail(rollId, startNo, endNo, state);
                    RAM.ResponseScripts.Add("alert('上报成功');window.close(); ");
                }
                catch (Exception exception)
                {
                    RAM.Alert("上报失败，系统提示：" + exception.Message);
                }
            }
        }

        #region[Ajax页面返回]
        protected void RadGrid_InvoiceRollDetailListAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RadGrid_InvoiceRollDetailList, e);
        }
        #endregion

        /// <summary>
        /// 批量分发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// zal 2015-07-29
        protected void btn_Distribution_Click(object sender, EventArgs e)
        {
            var strStartNo = string.Empty;
            var strEndNo = string.Empty;
            bool flag = false;
            foreach (GridDataItem item in RadGrid_InvoiceRollDetailList.Items)
            {
                if (item.Selected)
                {
                    var state = GetState(item.GetDataKeyValue("State"));
                    if (state.Equals("未启用"))
                    {
                        var startNo = item.GetDataKeyValue("StartNo");
                        var endNo = item.GetDataKeyValue("EndNo");

                        strStartNo += "," + startNo;
                        strEndNo += "," + endNo;
                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }
            }

            if (flag)
            {
                RAM.Alert("您选择的发票卷不符合规则!");
                return;
            }
            if (!string.IsNullOrEmpty(strStartNo) && !string.IsNullOrEmpty(strEndNo))
            {
                strStartNo = strStartNo.Substring(1);
                strEndNo = strEndNo.Substring(1);
                RAM.ResponseScripts.Add("Distribute(\"" + strStartNo + "\",\"" + strEndNo + "\");");
            }
            else
            {
                RAM.Alert("请选择发票卷!");
            }
        }
    }
}
