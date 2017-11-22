using System;
using System.Collections.Generic;
using System.Data;
using ERP.Model;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class AgainStorageRecordApplyOutForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected DataTable GoodsTable
        {
            get
            {
                if (ViewState["GoodsTable"] == null)
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("GoodsId", typeof(Guid));
                    dataTable.Columns.Add("IsRealGoods", typeof(int));
                    dataTable.Columns.Add("商品编号", typeof(string));
                    dataTable.Columns.Add("商品名称", typeof(string));
                    dataTable.PrimaryKey = new[] { dataTable.Columns[0] };
                    return dataTable;
                }
                return (DataTable)ViewState["GoodsTable"];
            }
            set
            {
                ViewState["GoodsTable"] = value;
            }
        }
        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var list = new List<StorageRecordDetailInfo>();
            RGGoods.DataSource = list;
        }

        /// <summary>选择添加商品数据源绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgSelectGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGSelectGoods.DataSource = GoodsTable;
        }
    }
}