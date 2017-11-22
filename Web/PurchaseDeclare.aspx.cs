using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.Goods;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class PurchaseDeclare : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GridRGGoodsDemand_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsDemandInfo> goodsDemandList = new List<GoodsDemandInfo>();
            RGGoodsDemand.DataSource = goodsDemandList.OrderBy(w => w.GoodsName).ThenBy(w => w.Specification).ToList();
        }
    }
}