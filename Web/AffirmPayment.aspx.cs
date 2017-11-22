using System;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>
    ///  到款通知
    /// </summary>
    public partial class AffirmPayment : BasePage
    {
        private readonly IPaymentNotice _pnbll = new PaymentNotice(GlobalConfig.DB.FromType.Read);

        protected int State
        {
            get
            {
                if (ViewState["PayState"] == null) return (int)PayState.Wait;
                return  Convert.ToInt32(ViewState["PayState"]);
            }
            set
            {
                ViewState["PayState"] = value;
            }
        }
        protected string SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null) return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set
            {
                ViewState["SearchKey"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

            }
        }

        protected void RG_PayNotice_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RG_PayNotice.DataSource = _pnbll.GetPayNoticListByPayState(State, SearchKey);
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_PayNotice, e);
        }

        protected void RgGoodsOrderItemCommand(object sender, GridCommandEventArgs e)
        {
            //搜索操作
            if (e.CommandName == "Search")
            {
                var btn = e.Item.FindControl("TB_Search") as TextBox;
                SearchKey = btn!=null?btn.Text.Trim():string.Empty;
                var rcb = e.Item.FindControl("RCB_PayState") as RadComboBox;
                if(rcb!=null)
                    State = Convert.ToInt32(rcb.SelectedValue);
                RG_PayNotice.Rebind();
            }
        }
    }
}
