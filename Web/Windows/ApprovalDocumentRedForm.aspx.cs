using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;


namespace ERP.UI.Web.Windows
{
    public partial class ApprovalDocumentRedForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var goodsStockList = new List<StorageRecordDetailInfo>();
            RGGoods.DataSource = goodsStockList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }
    }
}