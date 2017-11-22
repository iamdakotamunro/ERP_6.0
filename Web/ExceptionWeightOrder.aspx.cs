using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class ExceptionWeightOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RGPP_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<string> list = new List<string>();
            Rgd_Purchasing.DataSource = list;
        }
    }
}