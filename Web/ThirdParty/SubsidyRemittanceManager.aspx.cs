using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.UI.Web.Base;
using Telerik.Web.UI;
using ERP.BLL.Implement.Organization;

namespace ERP.UI.Web.ThirdParty
{
    public partial class SubsidyRemittanceManager : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RcbSaleFilialeOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            
        }

        #region 按钮点击
        protected void BtnSearchClick(object sender, EventArgs e)
        {
            
        }

        protected void BtnBatchClick(object sender, EventArgs e)
        {
            
        }

        protected void BtnExportExcelClick(object sender, EventArgs e)
        {
            
        }
        #endregion

        protected void RgRemittanceNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            
        }

        public string GetSaleFilialeName(object saleFilialeId)
        {
            return FilialeManager.GetName(new Guid(saleFilialeId.ToString()));
        }

        public string GetSalePlatformName(object salePlatformId)
        {
            return FilialeManager.GetName(new Guid(salePlatformId.ToString()));
        }
    }
}