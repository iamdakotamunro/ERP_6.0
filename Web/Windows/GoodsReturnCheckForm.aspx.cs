using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class GoodsReturnCheckForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var goodsStockList = new List<string>();
            RGGoods.DataSource = goodsStockList;
        }
    }
}