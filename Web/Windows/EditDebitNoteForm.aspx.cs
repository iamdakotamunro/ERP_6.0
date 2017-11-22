using System;
using System.Linq;
using System.Web.UI;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class EditDebitNoteForm : WindowsPage
    {
        private readonly IDebitNote _debitNote = new DebitNote(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        #region [自定义]

        protected Guid PurchasingId
        {
            get
            {
                if (ViewState["PurchasingId"] == null)
                {
                    Guid id = Guid.Empty;
                    if (Request.QueryString["PurchasingId"] == null || Request.QueryString["PurchasingId"].Trim() == "")
                    {

                    }
                    else
                    {
                        id = new Guid(Request.QueryString["PurchasingId"]);
                    }
                    ViewState["PurchasingId"] = id;
                }
                return new Guid(ViewState["PurchasingId"].ToString());
            }
        }

        protected DebitNoteInfo OldDebitNoteInfo
        {
            get
            {
                if (ViewState["OldDebitNoteInfo"] == null)
                {
                    ViewState["OldDebitNoteInfo"] = _debitNote.GetDebitNoteInfo(PurchasingId);
                }
                return (DebitNoteInfo)ViewState["OldDebitNoteInfo"];
            }
        }

        #endregion

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_DebitNoteDetail, e);
        }

        protected void RgDebitNoteDetail_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RG_DebitNoteDetail.DataSource = _debitNote.GetDebitNoteDetailList(PurchasingId).OrderBy(act => act.Specification);
        }

        protected void BtnLogout_Click(object sender, ImageClickEventArgs e)
        {
            _debitNote.UpdateDebitNoteStateByPurchasingId(PurchasingId, (int)DebitNoteState.Logout);
        }
    }
}