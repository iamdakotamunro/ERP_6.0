using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Model.ThirdParty;

namespace ERP.UI.Web.ThirdParty
{
    public partial class SubsidyAuditForm : System.Web.UI.Page
    {
        public bool IsAudit
        {
            get { return Request.QueryString["Type"] != null && Convert.ToBoolean(Request.QueryString["Type"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        private void BindData(SubsidyApplyDTO applyDto)
        {
            TbOrderNo.Text = applyDto.OrderNo;
            TbThirdPartyOrderNo.Text = applyDto.ThirdPartyOrderNo;
        }

        protected void BtBackClick(object sender, EventArgs e)
        {
            
        }

        protected void BtSaveClick(object sender, EventArgs e)
        {
            
        }
    }
}