using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>采购单详情
    /// </summary>
    public partial class PurchasingStockingFrom : WindowsPage
    {
        private static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Read);
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Read);
        private readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Read);
        readonly StorageManager _storageManager = new StorageManager();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary> 存储商品列表
        /// </summary>
        private Guid PurchasingID
        {
            get
            {
                if (ViewState["PurchasingID"] == null)
                {
                    return new Guid(Request["PurchasingID"]);
                }
                return new Guid(ViewState["PurchasingID"].ToString());
            }
        }

        /// <summary> 加载页面数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Rgd_PurchasingDetail_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var list = _purchasingDetail.Select(PurchasingID).Where(p => p.PlanQuantity - p.RealityQuantity >= 1);
            Rgd_PurchasingDetail.DataSource = list.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
            PurchasingInfo pInfo = _purchasing.GetPurchasingById(PurchasingID);
            lab_Purchasing.Text = pInfo.CompanyName + " " + pInfo.PurchasingNo;
            if (pInfo.PurchasingState >= (int)PurchasingState.AllComplete)
            {
                Rgd_PurchasingDetail.Enabled = true;
            }

        }

        protected void Rgd_StockGrid_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var list = _storageRecordDao.GetStorageRecordsByLinkTradeId(PurchasingID, StorageRecordState.Finished).ToList();
            if (list.Any(ent => ent.TradeBothPartiesType == (int) TradeBothPartiesType.HostingToHosting))
            {
                Rgd_StockGrid.DataSource = list.Where(ent=>ent.StockType == (int)StorageRecordType.BuyStockIn);
            }
            else
            {
                Rgd_StockGrid.DataSource = list;
            }
        }

        /// <summary>
        /// 功    能:DetailTable绑定 显示其供应商下的子商品集合
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void Rgd_StockGrid_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var dataItem = e.DetailTableView.ParentItem;
            var stockId = new Guid(dataItem.GetDataKeyValue("StockId").ToString());
            e.DetailTableView.DataSource = _storageManager.GetStorageRecordDetailListByStockId(stockId);
        }

        /// <summary>
        /// 获取仓库名
        /// </summary>
        /// <param name="warehouseId"> </param>
        /// <returns></returns>
        protected string GetWareHouseName(Guid warehouseId)
        {
            var dic = WarehouseManager.GetWarehouseDic();
            return dic.ContainsKey(warehouseId) ? dic[warehouseId] : string.Empty;
        }

        double _planTotal;
        decimal _total;
        protected void Rgd_PurchasingDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var dataItem = e.Item as GridDataItem;
            if (dataItem != null)
            {
                double planQ = Convert.ToDouble(dataItem.GetDataKeyValue("PlanQuantity"));
                _planTotal += planQ;
                decimal price = Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString()) == -1 ? 0 : Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString());
                _total += Convert.ToDecimal(planQ) * price;
            }
            if (!(e.Item is GridFooterItem)) return;
            var footerItem = e.Item as GridFooterItem;
            footerItem["PlanQuantity"].Text = string.Format("采购总数量: {0}", _planTotal);
            footerItem["RealityQuantity"].Text = string.Format("采购总金额: {0}", WebControl.NumberSeparator(_total));
        }
    }
}
