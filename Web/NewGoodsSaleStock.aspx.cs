using System;
using System.Collections.Generic;
using System.Web.UI;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    ///<summary>
    /// 卖库存商品管理页面
    ///</summary>
    public partial class NewGoodsSaleStock : BasePage
    {
        readonly IGoodsCenterSao _goodSaleStock = new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null) return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set { ViewState["SearchKey"] = value; }
        }

        /// <summary> 是否有审核权限
        /// </summary>
        protected bool HasAudit
        {
            get
            {
                if (ViewState["HasAudit"] == null)
                {
                    ViewState["HasAudit"] = GetPowerOperationPoint("Auditing");
                }
                return (bool)ViewState["HasAudit"];
            }
        }

        /// <summary> 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            string pageName = WebControl.FileName;
            return WebControl.GetPowerOperationPoint(pageName, powerName);
        }

        protected void GoodsGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            int totalCount;
            var startPage = GoodsGrid.CurrentPageIndex + 1;
            int pageSize = GoodsGrid.PageSize;
            IList<GoodsSaleStockGridModel> goodsSaleStockGridModelList = _goodSaleStock.GetGoodsSaleStockListToPage(SearchKey, startPage, pageSize, out totalCount);
            GoodsGrid.DataSource = goodsSaleStockGridModelList;
            GoodsGrid.VirtualItemCount = totalCount;
        }

        protected void GoodsGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                e.Item.FindControl("GTBtn_Audit").Visible = HasAudit;
            }
        }

        protected void RamGoodsAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(GoodsGrid, e);
        }

        // 搜索
        protected void LbSearchClick(object sender, ImageClickEventArgs e)
        {
            SearchKey = TB_Search.Text;
            GoodsGrid.Rebind();
        }
    }
}
