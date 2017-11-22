using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class GoodsCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RGPP_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<PurchasingInfo> list = new List<PurchasingInfo>();
            Rgd_Purchasing.DataSource = list;
        }
    }
}