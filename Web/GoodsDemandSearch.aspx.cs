using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>
    /// 商品需求查询
    /// </summary>
    public partial class GoodsDemandSearch : BasePage
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindWarehouse();
            }
        }

        public Guid HostingFilialeId
        {
            get
            {
                if (ViewState["HostingFilialeId"] == null) return Guid.Empty;
                return (Guid)ViewState["HostingFilialeId"];
            }
            set { ViewState["HostingFilialeId"] = value; }
        }

        public Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null) return Guid.Empty;
                return (Guid)ViewState["WarehouseId"];
            }
            set { ViewState["WarehouseId"] = value; }
        }

        public Dictionary<Guid, String> FilialeDics
        {
            get
            {
                if (ViewState["FilialeDics"] == null) return new Dictionary<Guid, String>();
                return (Dictionary<Guid, String>)ViewState["FilialeDics"];
            }
            set { ViewState["FilialeDics"] = value; }
        }

        public List<RealGoodsTotal> RealGoodsTotals
        {
            get
            {
                if (ViewState["RealGoodsTotals"] == null) return new List<RealGoodsTotal>();
                return (List<RealGoodsTotal>)ViewState["RealGoodsTotals"];
            }
            set { ViewState["RealGoodsTotals"] = value; }
        }

        #region 下拉框选择事件
        /// <summary>
        /// 入库仓Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDLWaerhouse_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
               ? Guid.Empty
               : new Guid(RCB_Warehouse.SelectedValue);

            //获得入库储
            var personinfo = CurrentSession.Personnel.Get();
            var warehouseAuth = WMSSao.GetSingleWarehouseAndFilialeAuth(personinfo.PersonnelId,warehouseId);
            //获得物流配送公司
            var hlist = new List<HostingFilialeAuth>();
            var saleFiliales = new List<HostingFilialeAuth>();
            if (warehouseAuth != null)
            {
                foreach (var filialeAuth in warehouseAuth.FilialeAuths)
                {
                    hlist.Add(filialeAuth);
                    foreach (var filiale in filialeAuth.ProxyFiliales)
                    {
                        if (hlist.All(p => p.HostingFilialeId != filiale.ProxyFilialeId) && saleFiliales.All(ent => ent.HostingFilialeId != filiale.ProxyFilialeId))
                        {
                            saleFiliales.Add(new HostingFilialeAuth { HostingFilialeId = filiale.ProxyFilialeId, HostingFilialeName = filiale.ProxyFilialeName });
                        }
                    }
                }
            }
            RCB_Filile.DataSource = hlist.Union(saleFiliales);
            RCB_Filile.DataTextField = "HostingFilialeName";
            RCB_Filile.DataValueField = "HostingFilialeId";
            RCB_Filile.DataBind();
            RCB_Filile.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
        }
        #endregion

        /// <summary>搜索商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            //此处商品搜索有待更新，需要 过滤是否下架商品。
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                var list = _goodsCenterSao.GetGoodsSelectList(e.Text);
                var totalCount = list.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in list)
                    {
                        var rcb = new RadComboBoxItem
                        {
                            Text = item.Value,
                            Value = item.Key,
                        };
                        combo.Items.Add(rcb);
                    }
                }
            }
        }

        /// <summary>搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LB_Search_Click(object sender, EventArgs e)
        {
            RGGoodsOrder.CurrentPageIndex = 0;
            if (string.IsNullOrEmpty(RCB_Goods.SelectedValue) || RCB_Goods.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请输入待查询的商品！");
                return;
            }
            if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择待查询的仓库！");
                return;
            }
            WarehouseId = new Guid(RCB_Warehouse.SelectedValue);
            if (!string.IsNullOrEmpty(RCB_Filile.SelectedValue) && RCB_Filile.SelectedValue != Guid.Empty.ToString())
            {
                HostingFilialeId = new Guid(RCB_Filile.SelectedValue);
            }
            else
            {
                HostingFilialeId = Guid.Empty;
            }
            RGGoodsOrder.Rebind();
        }

        /// <summary>绑定数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RGGoodsOrder_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_Goods.SelectedValue) || string.IsNullOrEmpty(RCB_Warehouse.SelectedValue))
            {
                RGGoodsOrder.DataSource = new List<GoodsDemandSearchInfo>();
            }
            else
            {
                var goodsId = new Guid(RCB_Goods.SelectedValue);
                var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
                var goods = _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
                var goodsList = _goodsCenterSao.GetRealGoodsListByGoodsId(new List<Guid> { goodsId }).ToList();
                List<Guid> realGoodsIdList = goodsList.Select(p => p.RealGoodsId).Distinct().ToList();
                var result = WMSSao.GetRequireSearchDtos(warehouseId, realGoodsIdList, HostingFilialeId);
                List< GoodsDemandSearchInfo> datasource=new List<GoodsDemandSearchInfo>();
                if (result!=null)
                {
                    if (result.Count>1)
                    {
                        List<RealGoodsTotal> total = result.Select(filiale => new RealGoodsTotal
                        {
                            HostingFilialeId = filiale.HostingFilialeId, HostingFilialeName = filiale.HostingFilialeName, StockQuantity = filiale.StockQuantity, RequiresDics = filiale.RequiresDics,
                        }).ToList();
                        RealGoodsTotals = total;
                        FilialeDics = total.ToDictionary(k => k.HostingFilialeId, v => v.HostingFilialeName);
                        datasource =(from item in goodsList where total.Any(ent=>ent.RequiresDics.ContainsKey(item.RealGoodsId))
                         select new GoodsDemandSearchInfo
                         {
                             GoodsId = goodsId,
                             GoodsName = goods.GoodsName,
                             RealGoodsId = item.RealGoodsId,
                             Specification = item.Specification,
                             EffictiveRequire = 0,
                             CanUseGoodsStock = 0,
                         }).ToList();
                    }
                    else
                    {
                        var filialeStock = result.FirstOrDefault();
                        var dics = filialeStock!=null && filialeStock.RequiresDics!=null? filialeStock.RequiresDics : new Dictionary<Guid, int>();
                        var canUseDics = filialeStock != null ? filialeStock.StockQuantity : new Dictionary<Guid, int>();
                        datasource=(from item in goodsList
                        let goodsStock= canUseDics == null ? 0 : canUseDics.ContainsKey(item.RealGoodsId) ? canUseDics[item.RealGoodsId] : 0
                        let require = dics == null ? 0 : dics.ContainsKey(item.RealGoodsId) ? dics[item.RealGoodsId] : 0
                        where require>0
                        select new GoodsDemandSearchInfo
                        {
                            GoodsId = goodsId,
                            GoodsName = goods.GoodsName,
                            RealGoodsId = item.RealGoodsId,
                            Specification = item.Specification,
                            EffictiveRequire = require,
                            CanUseGoodsStock = goodsStock
                        }).ToList();
                        RealGoodsTotals = new List<RealGoodsTotal>();
                    }
                }
                RGGoodsOrder.DataSource = datasource.Count>0? datasource.OrderBy(p => p.Specification).ToList() : datasource;
            }
            RGGoodsOrder.MasterTableView.Columns[2].Display = !IsPostBack || RealGoodsTotals.Count<1;
            RGGoodsOrder.MasterTableView.Columns[3].Display = !IsPostBack || RealGoodsTotals.Count < 1;
            RGGoodsOrder.MasterTableView.Columns[4].Display = IsPostBack && RealGoodsTotals.Count > 1;
        }

        #region [显示商品具体类型明细(js窗体页面)]

        /// <summary>显示商品具体类型明细
        /// </summary>
        /// <param name="goodsId">子商品ID</param>
        /// <returns></returns>
        protected string GetShowFrom(object goodsId)
        {
            var warehouseId = RCB_Warehouse.SelectedValue;
            var filileId = RCB_Filile.SelectedValue;
            return "javascript:ShowGoodsDemandNotOutQuantityDetailForm('" + new Guid(goodsId.ToString()) + "','" + new Guid(warehouseId) + "','" + new Guid(filileId) + "');return false;";
        }

        #endregion

        #region 下拉框绑定
        /// <summary>绑定入库仓储
        /// </summary>
        private void BindWarehouse()
        {
            var wList = CurrentSession.Personnel.WarehouseList;
            RCB_Warehouse.DataSource = wList;
            RCB_Warehouse.DataTextField = "WarehouseName";
            RCB_Warehouse.DataValueField = "WarehouseId";
            RCB_Warehouse.DataBind();
            RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("请选择", Guid.Empty.ToString()));
        }

        #endregion

        protected void RGGoodsOrderDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                if (RealGoodsTotals.Count>1)
                {
                    string row1 = "<tr>";
                    string row2 = "<tr>";
                    int i = 0;

                    foreach (var filiale in RealGoodsTotals)
                    {
                        row1 += "<td class='Group' style='width:160px;padding-top:5px; padding-bottom:5px; border-bottom:1px solid #3d556c;" + (i == 0 ? " border-left:0px;" : " border-left:1px solid #3d556c;") + "'  colspan=\"" + 2 + "\">" + filiale.HostingFilialeName + "</td>";

                        for (int j = 0; j < 2; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                row2 += "<td class='title' style='padding-top:5px; padding-bottom:5px;width:80px;text-align:center;border-left:0px;'>" + "有效需求数" + "</td>";
                                continue;
                            }
                            row2 += "<td class='title' style='padding-top:5px; padding-bottom:5px;width:80px;text-align:center;border-left:1px solid #3d556c;'>" + (j == 0 ? "有效需求数" : "可用库存数") + "</td>";
                        }
                        i++;
                    }
                    row1 += "</tr>";
                    row2 += "</tr>";
                    var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\">" + row1 + row2 + "</table>";
                    e.Item.Cells[6].Text = headerText;
                }
            }
            else if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (RealGoodsTotals.Count > 1)
                {
                    var realGoodsId = new Guid(((GridDataItem)e.Item).GetDataKeyValue("RealGoodsId").ToString());
                    string row = "<tr>";
                    int i = 0;
                    foreach (var filiale in RealGoodsTotals)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                row += "<td class='title' style='padding-top:5px; padding-bottom:5px;width:80px;text-align:center;border-left:0px;'><span style='cursor: pointer;text-decoration:underline;' onclick=\"ShowGoodsDemandNotOutQuantityDetailForm('" + realGoodsId + "','" + WarehouseId + "','" + filiale.HostingFilialeId + "')\">" +
                                    (filiale.RequiresDics.ContainsKey(realGoodsId) ? filiale.RequiresDics[realGoodsId] : 0) + "</span></td>";
                                continue;
                            }
                            if (j == 0)
                            {
                                row += "<td class='title' style='padding-top:5px; padding-bottom:5px;text-align:center; width:80px;border-left:1px solid #3d556c;'><span style='cursor: pointer;text-decoration:underline;' onclick=\"ShowGoodsDemandNotOutQuantityDetailForm('" + realGoodsId + "','" + WarehouseId + "','" + filiale.HostingFilialeId + "')\">" +
                              (filiale.RequiresDics.ContainsKey(realGoodsId) ? filiale.RequiresDics[realGoodsId] : 0) + "</span></td>";
                            }
                            else
                            {
                                row += "<td class='title' style='padding-top:5px; padding-bottom:5px;text-align:center; width:80px;border-left:0px;border-left:1px solid #3d556c;'>" +
                                    (filiale.StockQuantity.ContainsKey(realGoodsId) ? filiale.StockQuantity[realGoodsId] : 0) + "</td>";
                            }
                        }
                        i++;
                    }
                    row += "</tr>";
                    var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\">" + row + "</table>";
                    e.Item.Cells[6].Text = headerText;
                }
            }
        }
    }

    [Serializable]
    public class RealGoodsTotal
    {
        public Guid HostingFilialeId { get; set; }

        public String HostingFilialeName { get; set; }

        public Dictionary<Guid, int> RequiresDics { get; set; }

        public Dictionary<Guid, int> StockQuantity { get; set; }
    }
}