using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.Environment;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>商品需求查询
    /// </summary>
    public partial class SearchRequirementAw : BasePage
    {

        /// <summary>页面加载
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region [显示商品具体类型明细(js窗体页面)]

        /// <summary>显示商品具体类型明细
        /// </summary>
        /// <param name="goodsId">子商品ID</param>
        /// <param name="type">1占用量， 2出库量 </param>
        /// <returns></returns>
        protected string GetShowFrom(object goodsId, int type)
        {
            return "javascript:ShowRequirementOrderFrom('" + new Guid(goodsId.ToString()) + "'," + type + ");return false;";
        }

        #endregion

        #region [绑定数据源]

        /// <summary>绑定数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGGoodsOrder_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchKey) && (ViewState["WareHouseId"] == null || ViewState["FilialeId"] == null))
            {
                RGGoodsOrder.DataSource = new List<GoodsOrderInfo>();
            }
        }

        #endregion

        #region [搜索]

        /// <summary>搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RGGoodsOrder_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                FilialeId = new Guid(((RadComboBox)e.Item.FindControl("RCB_FilileF")).SelectedValue);
                WarehouseId = new Guid(((RadComboBox)e.Item.FindControl("RCB_InStockF")).SelectedValue);
                SearchKey = ((TextBox)e.Item.FindControl("TB_Search")).Text.Trim();
                RGGoodsOrder.CurrentPageIndex = 0;
                RGGoodsOrder.Rebind();
            }
        }

        #endregion

        #region 公司和仓库绑定

        /// <summary>绑定公司
        /// </summary>
        /// <returns></returns>
        protected IList<FilialeInfo> BindFilile()
        {
            IList<FilialeInfo> filialeList = new List<FilialeInfo>();
            var erpId = GlobalConfig.ERPFilialeID;
            var erpFilialeInfo = CacheCollection.Filiale.Get(erpId);
            filialeList.Add(erpFilialeInfo);
            return filialeList;
        }

        /// <summary> 绑定仓库
        /// </summary>
        /// <returns></returns>
        protected IList<WarehouseAuth> BindWareHouse()
        { 
            IList <WarehouseAuth> warehouseAuthList= CurrentSession.Personnel.WarehouseList;
            if (warehouseAuthList.Count==0)
            {
                warehouseAuthList.Add(new WarehouseAuth
                {
                    WarehouseId = Guid.Empty,
                    WarehouseName = "请选择",
                    Storages = new List<StorageAuth>()
                });
            }
            return warehouseAuthList;
        }

        #endregion

        #region [ViewState]

        /// <summary>公司ID
        /// </summary>
        public Guid FilialeId
        {
            get
            {
                if (ViewState["FilialeId"] == null)
                {
                    ViewState["FilialeId"] = GlobalConfig.ERPFilialeID;
                }
                return new Guid(ViewState["FilialeId"].ToString());
            }
            set
            {
                ViewState["FilialeId"] = value.ToString();
            }
        }

        /// <summary>仓库ID
        /// </summary>
        protected Guid WarehouseId
        {
            get
            {
                if (ViewState["WareHouseId"] == null)
                {
                    if (BindWareHouse().Count >= 1)
                    {
                        return BindWareHouse()[0].WarehouseId;
                    }
                    return Guid.Empty;
                }
                return new Guid(ViewState["WareHouseId"].ToString());
            }
            set
            {
                ViewState["WareHouseId"] = value.ToString();
            }
        }

        /// <summary>商品名称
        /// </summary>
        protected string SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null) return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set
            {
                ViewState["SearchKey"] = value;
            }
        }

        #endregion
    }
}
