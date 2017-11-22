using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class PurchaseOrderDetailForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Rgd_GoodsPrint_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var list = new List<PurchasingDetailInfo>();
            Rgd_GoodsPrint.DataSource = list;
        }
    }
}