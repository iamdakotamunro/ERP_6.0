using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Model;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class AllocationOutForm : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //商品 度数明细
        protected void RGGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<StorageRecordDetailInfo> goodsMergeStockList = new List<StorageRecordDetailInfo>();
            RGGoods.DataSource = goodsMergeStockList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }


        //选择商品框的商品绑定
        protected void RGSelectGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGSelectGoods.DataSource = GoodsTable;
        }

        protected DataTable GoodsTable
        {
            get
            {
                if (Session["GoodsTable"] == null)
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("GoodsId", typeof (Guid));
                    dataTable.Columns.Add("IsRealGoods", typeof (int));
                    dataTable.Columns.Add("商品编号", typeof (string));
                    dataTable.Columns.Add("商品名称", typeof (string));
                    dataTable.PrimaryKey = new[] {dataTable.Columns[0]};
                    return dataTable;
                }
                return (DataTable) Session["GoodsTable"];
            }
            set { Session["GoodsTable"] = value; }
        }
    }
}