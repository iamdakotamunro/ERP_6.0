using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class AddPurchaseOrderForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 商品选择属性 RGSelectGoods
        /// </summary>
        protected DataTable GoodsTable
        {
            get
            {
                if (Session["GoodsTable"] == null)
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("GoodsId", typeof(Guid));
                    dataTable.Columns.Add("IsRealGoods", typeof(int));
                    dataTable.Columns.Add("商品编号", typeof(string));
                    dataTable.Columns.Add("商品名称", typeof(string));
                    dataTable.Columns.Add("SKU", typeof(string));
                    dataTable.PrimaryKey = new[] { dataTable.Columns[0] };
                    return dataTable;
                }
                return (DataTable)Session["GoodsTable"];
            }
            set
            {
                Session["GoodsTable"] = value;
            }
        }

        /// <summary>选择商品框的商品绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGSelectGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGSelectGoods.DataSource = GoodsTable;
        }

        #region -- 绑定要采购明细商品
        /// <summary>绑定要采购明细商品
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void Rgd_PurchasingDetail_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var purchasingDetailList = new List<PurchasingDetailInfo>();
            Rgd_PurchasingDetail.DataSource = purchasingDetailList.OrderBy(w => w.GoodsName).ThenBy(ent => ent.Specification).ToList();
        }
        #endregion
    }
}