using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>商品库存详情
    /// </summary>
    public partial class ShowGoodsStock : WindowsPage
    {
        private readonly GoodsStockPile _goodsStockPile = GoodsStockPile.ReadInstance;

        #region [Page_Load 加载]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                const string PAGE_NAME = "GoodsStock.aspx";
                if (!WebControl.GetPowerOperationPoint(PAGE_NAME, "UnitPrice"))
                {
                    RGGoodsStock.MasterTableView.Columns.FindByUniqueName("UnitPrice").Visible = false;
                }
            }
            else
            {
                RAM.ResponseScripts.Add("removeClass();");
            }
        }
        #endregion

        #region [Current.Request.QueryString (GoodsId,WarehouseId)]
        private static Guid GoodsId
        {
            get { return WebControl.GetGuidFromQueryString("GoodsId"); }
        }

        private static Guid WarehouseId
        {
            get { return WebControl.GetGuidFromQueryString("WarehouseId"); }
        }

        private static Guid HostingFilialeId
        {
            get { return WebControl.GetGuidFromQueryString("FilialeId"); }
        }
        public Dictionary<Guid, String> FilialeDic
        {
            get
            {
                if (ViewState["FilialeDic"] == null) return new Dictionary<Guid, String>();
                return (Dictionary<Guid, String>)ViewState["FilialeDic"];
            }
            set { ViewState["FilialeDic"] = value; }
        }

        public Dictionary<Guid, Dictionary<Guid, int>> StockSearchDic
        {
            get
            {
                if (ViewState["StockSearchDic"] == null) return new Dictionary<Guid, Dictionary<Guid, int>>();
                return (Dictionary<Guid, Dictionary<Guid, int>>)ViewState["StockSearchDic"];
            }
            set { ViewState["StockSearchDic"] = value; }
        }

        #endregion

        /// <summary>绑定数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGGoodsStock_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var list = _goodsStockPile.GetChildGoodsStockPileList(GoodsId, WarehouseId, HostingFilialeId).OrderBy(ent => ent.Specification).ToList();
            var stockSearchList = WMSSao.StockSearchList(list.Select(ent=>ent.GoodsId), WarehouseId, HostingFilialeId);
            FilialeDic =
                (from item in stockSearchList.GroupBy(ent => ent.FilialeId) let filiale = item.First() select filiale)
                    .ToDictionary(k => k.FilialeId, v => v.FilialeName);
            Dictionary<Guid, Dictionary<Guid, int>> goodsStocks = new Dictionary<Guid, Dictionary<Guid, int>>();
            if (list.Count > 0)
            {
                foreach (var info in list)
                {
                    Dictionary<Guid,int> dics=new Dictionary<Guid, int>();
                    
                    foreach (var item in stockSearchList)
                    {
                        var filiaeQuantity = item.StockQuantity.Where(ent => info.GoodsId == ent.Key).Sum(ent => ent.Value);
                        dics.Add(item.FilialeId, filiaeQuantity);
                    }
                    if(dics.Values.Sum(ent=>ent)>0)
                        goodsStocks.Add(info.GoodsId, dics);
                }
                StockSearchDic = goodsStocks;
            }
            RGGoodsStock.DataSource = list.Where(ent=>goodsStocks.Keys.Contains(ent.GoodsId));
        }

        /// <summary>导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Lbxls_Click(object sender, EventArgs e)
        {
            var goodsInfo = new GoodsCenterSao().GetGoodsBaseInfoById(GoodsId);
            if (goodsInfo != null)
            {
                string fileName = goodsInfo.GoodsName;
                fileName += WebControl.GetNowTime().ToShortDateString() + ".xls";
                fileName = Server.UrlEncode(fileName);
                RGGoodsStock.ExportSettings.ExportOnlyData = true;
                RGGoodsStock.ExportSettings.IgnorePaging = true;
                RGGoodsStock.ExportSettings.FileName = fileName;
                RGGoodsStock.MasterTableView.ExportToExcel();
            }
        }


        protected void GridGoodsStockItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                if (HostingFilialeId == Guid.Empty)
                {
                    string row1 = "<tr>";
                    string row2 = "<tr>";
                    int i = 0;

                    foreach (var filiale in FilialeDic)
                    {
                        row1 += "<td class='Group' style='width:160px;padding-top:5px; padding-bottom:5px;" + (i == 0 ? " border-left:0px;" : "") + "'>" + filiale.Value + "</td>";      
                        i++;
                    }
                    row1 += "</tr>";
                    var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%; text-align:center;\">" + row1+ "</table>";
                    e.Item.Cells[5].Text = headerText;
                }
            }
            else if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (HostingFilialeId == Guid.Empty)
                {
                    var goodsId = new Guid(((GridDataItem)e.Item).GetDataKeyValue("GoodsId").ToString());
                    var goodsStocks =  StockSearchDic[goodsId];
                    string row = "<tr>";
                    foreach (var filiale in FilialeDic)
                    {
                        int stockQuantity = goodsStocks.ContainsKey(filiale.Key) ? goodsStocks[filiale.Key] : 0;
                        row += "<td style='text-align:center;'>" + stockQuantity + "</td>";
                    }
                    row += "</tr>";
                    var headerText = "<table cellspacing='0' cellpadding='0' style=\"width:100%; text-align:center;\">" + row + "</table>";
                    e.Item.Cells[5].Text = headerText;
                }
            }
        }
    }
}