using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using B2C.Model.PromotionModel;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using ERP.UI.Web.UserControl;
using Telerik.Web.UI;
using ERP.DAL.Interface.IStorage;
using ERP.DAL.Implement.Storage;
using ERP.Model.WMS;
using ERP.SAL;

namespace ERP.UI.Web
{
    /// <summary>库存查询 
    /// </summary>
    public partial class GoodsStockAw : BasePage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);

        #region [ Page_Load 加载 ]
        /// <summary>页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                const string PAGE_NAME = "GoodsStock.aspx";
                if (!WebControl.GetPowerOperationPoint(PAGE_NAME, "RecentInPrice"))
                {
                    GridGoodsStock.MasterTableView.Columns.FindByUniqueName("RecentInPrice").Visible = false;
                }
                BindWarehouse();
                GetTreeGoodsClass();
            }
            else
            {
                RAM.ResponseScripts.Add("removeClass();");
            }
        }
        #endregion

        #region [商品分类树]
        private void GetTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            TVGoodsClass.Nodes.Add(rootNode);
            IList<GoodsClassInfo> goodsClassList = _goodsCenterSao.GetAllClassList().ToList();
            RecursivelyGoodsClass(Guid.Empty, rootNode, goodsClassList);
        }

        //遍历产品分类
        private void RecursivelyGoodsClass(Guid goodsClassId, RadTreeNode node, IEnumerable<GoodsClassInfo> goodsClassList)
        {
            var goodsClassInfos = goodsClassList as IList<GoodsClassInfo> ?? goodsClassList.ToList();
            IList<GoodsClassInfo> childGoodsClassList = goodsClassInfos.Where(w => w.ParentClassId == goodsClassId).ToList();
            foreach (GoodsClassInfo info in childGoodsClassList)
            {
                RadTreeNode goodsClassNode = CreateNode(info.ClassName, false, info.ClassId.ToString());
                if (node == null)
                {
                    TVGoodsClass.Nodes.Add(goodsClassNode);
                }
                else
                {
                    node.Nodes.Add(goodsClassNode);
                }
                RecursivelyGoodsClass(info.ClassId, goodsClassNode, goodsClassInfos);
            }
        }

        //创建节点
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { Expanded = expanded };
            return node;
        }
        #endregion

        #region 仓库绑定
        private void BindWarehouse()
        {
            var warehouseList = CurrentSession.Personnel.WarehouseList;
            if (warehouseList.Count == 0)
            {
                RAM.Alert("您没有授权的仓库，需要开放权限请咨询相关人员！");
                return;
            }
            RCB_Warehouse.DataSource = warehouseList;
            RCB_Warehouse.DataTextField = "WarehouseName";
            RCB_Warehouse.DataValueField = "WarehouseId";
            RCB_Warehouse.DataBind();
        }

        #endregion

        /// <summary>商品分类树点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TvGoodsClass_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            SearchText = null;
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                GoodsClassId = new Guid(e.Node.Value);
                WarehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
                GridGoodsStock.Rebind();
            }
            else
            {
                GoodsClassId = Guid.Empty;
            }
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

        public Dictionary<Guid, String> FilialeDic
        {
            get
            {
                if (ViewState["FilialeDic"] == null) return new Dictionary<Guid, String>();
                return (Dictionary<Guid, String>)ViewState["FilialeDic"];
            }
            set { ViewState["FilialeDic"] = value; }
        }

        /// <summary>绑定数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridGoodsStock_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridGoodsStock.MasterTableView.Columns[2].Display = IsPostBack;
            IList<GoodsStockSearchInfo> goodsStockSearchList = new List<GoodsStockSearchInfo>();
            var pageIndex = GridGoodsStock.CurrentPageIndex + 1;
            int pageSize = GridGoodsStock.PageSize;
            int totalCount = 0;
            if (IsPostBack)
            {
                if (WarehouseId == Guid.Empty)
                {
                    RAM.Alert("您没有授权的仓库！");
                }
                else
                {
                    var goodsIdList = new List<Guid>();
                    if (CompanyId != Guid.Empty)
                    {
                        var purchaseings = _purchaseSet.GetPurchaseSetListByWarehouseIdAndCompanyId(WarehouseId, CompanyId);
                        if (purchaseings.Count == 0)
                        {
                            RAM.Alert("系统提示：当前选择供应商没有设置相关商品，无法为您查询库存信息！");
                            return;
                        }
                        goodsIdList = purchaseings.Select(act => act.GoodsId).ToList();
                    }
                    var goodsInfoList = _goodsCenterSao.GetGoodsStockGridList(GoodsClassId, SearchText, goodsIdList, pageIndex, pageSize, out totalCount).ToList();
                    if (goodsInfoList.Count > 0)
                    {
                        var goodsIds = goodsInfoList.Select(ent => ent.GoodsId).ToList();
                        var childGoodsList = _goodsCenterSao.GetRealGoodsListByGoodsId(goodsIds).ToList();
                        var list = WMSSao.StockSearchList(childGoodsList.Select(ent => ent.RealGoodsId), WarehouseId, HostingFilialeId);
                        FilialeDic =
                            (from item in list.GroupBy(ent => ent.FilialeId) let filiale = item.First() select filiale)
                                .ToDictionary(k => k.FilialeId, v => v.FilialeName);
                        Dictionary<Guid, List<Guid>> dics = new Dictionary<Guid, List<Guid>>();
                        Dictionary<Guid, Dictionary<Guid, int>> goodsStocks = new Dictionary<Guid, Dictionary<Guid, int>>();
                        //根据仓库Id获取供应商Id
                        var dicGoodsIdAndCompanyId = _purchaseSet.GetAllPurchaseSet(WarehouseId);
                        //获取商品的最后一次进货价信息
                        var goodsPurchaseLastPriceInfoList = _storageRecordDao.GetGoodsPurchaseLastPriceInfoByWarehouseId(WarehouseId);
                        goodsPurchaseLastPriceInfoList = goodsPurchaseLastPriceInfoList.Where(p => goodsIds.Contains(p.GoodsId)).ToList();

                        foreach (var info in goodsInfoList)
                        {
                            var realGoodsIds = childGoodsList.Where(ent => ent.GoodsId == info.GoodsId).Select(ent => ent.RealGoodsId);
                            dics.Add(info.GoodsId, realGoodsIds.ToList());
                            goodsStocks.Add(info.GoodsId, new Dictionary<Guid, int>());
                            if (childGoodsList.Count > 0)
                            {
                                var goodsStockSearchInfo = new GoodsStockSearchInfo();
                                //根据商品id获取供应商
                                var companyIds = dicGoodsIdAndCompanyId.Where(ent => ent.GoodsId == info.GoodsId).Select(ent => ent.CompanyId);

                                decimal unitPrice = 0;
                                GoodsPurchaseLastPriceInfo goodsPurchaseLastPriceInfo = null;
                                if (goodsPurchaseLastPriceInfoList.Count > 0)
                                {
                                    goodsPurchaseLastPriceInfo = goodsPurchaseLastPriceInfoList.FirstOrDefault(p => p.GoodsId.Equals(info.GoodsId) && companyIds.Contains(p.ThirdCompanyId));
                                    unitPrice = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.UnitPrice : 0;
                                }
                                goodsStockSearchInfo.RecentCDate = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.LastPriceDate : DateTime.MinValue;
                                goodsStockSearchInfo.RecentInPrice = unitPrice;

                                if (list != null && list.Count > 0)
                                {
                                    int quantity = 0;
                                    foreach (var item in list)
                                    {
                                        var filiaeQuantity = item.StockQuantity.Where(ent => realGoodsIds.Contains(ent.Key)).Sum(ent => ent.Value);
                                        quantity += filiaeQuantity;
                                        goodsStocks[info.GoodsId].Add(item.FilialeId, filiaeQuantity);
                                    }
                                    goodsStockSearchInfo.GoodsId = info.GoodsId;
                                    goodsStockSearchInfo.GoodsName = info.GoodsName;
                                    goodsStockSearchInfo.GoodsCode = info.GoodsCode;
                                    goodsStockSearchInfo.IsScarcity = info.IsStockScarcity;
                                    goodsStockSearchInfo.IsOnShelf = info.IsOnShelf;
                                    goodsStockSearchInfo.CurrentQuantity = quantity;
                                    goodsStockSearchInfo.CurrentSumPrice = quantity * unitPrice;
                                }
                                else
                                {
                                    goodsStockSearchInfo.GoodsId = info.GoodsId;
                                    goodsStockSearchInfo.GoodsName = info.GoodsName;
                                    goodsStockSearchInfo.GoodsCode = info.GoodsCode;
                                    goodsStockSearchInfo.IsScarcity = info.IsStockScarcity;
                                    goodsStockSearchInfo.IsOnShelf = info.IsOnShelf;
                                }
                                goodsStockSearchList.Add(goodsStockSearchInfo);
                            }
                        }
                        StockSearchDic = goodsStocks;
                    }
                }
            }
            #region FooterText 统计显示
            var recentCDate = GridGoodsStock.MasterTableView.Columns.FindByUniqueName("RecentCDate");
            var isOnShelf = GridGoodsStock.MasterTableView.Columns.FindByUniqueName("IsOnShelf");
            if (goodsStockSearchList.Count > 0)
            {
                var currentSumPrice = goodsStockSearchList.Sum(ent => ent.CurrentSumPrice);
                recentCDate.FooterText = "库存金额总计:";
                isOnShelf.FooterText = WebControl.NumberSeparator(currentSumPrice.ToString("N"));
            }
            else
            {
                recentCDate.FooterText = "库存金额总计：-";
            }
            #endregion

            GridGoodsStock.DataSource = goodsStockSearchList.OrderByDescending(w => w.CurrentQuantity).ToList();
            GridGoodsStock.VirtualItemCount = totalCount;
        }

        /// <summary>搜索 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void GridGoodsStock_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (!(e.Item is GridDataItem) && !(e.Item is GridCommandItem)) return;
            if (e.CommandName == "Search")
            {
                var cs = (CommonEnterSearchControl)e.Item.FindControl("CommonSearch1");
                SearchText = cs.SearchText;
                WarehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
                var rcbCompanyList = e.Item.FindControl("RCB_Company") as RadComboBox;
                if (rcbCompanyList != null) CompanyId = new Guid(rcbCompanyList.SelectedValue);
                GridGoodsStock.Rebind();
            }
        }



        /// <summary>是否显示标记 √
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected string GetIsTrue(object i)
        {
            return (bool)i ? "√" : "-";
        }

        /// <summary>导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Lbxls_Click(object sender, EventArgs e)
        {
            string fileName = TVGoodsClass.SelectedNode == null ? "商品分类" : new Guid(TVGoodsClass.SelectedNode.Value) == Guid.Empty ? "商品分类" : Regex.Replace(TVGoodsClass.SelectedNode.Text, @"[\s\|\-\/\<>\*\?\\]", "");
            fileName += WebControl.GetNowTime().ToShortDateString();
            fileName = Server.UrlEncode(fileName);
            GridGoodsStock.ExportSettings.ExportOnlyData = true;
            GridGoodsStock.ExportSettings.IgnorePaging = true;
            GridGoodsStock.ExportSettings.FileName = fileName;
            GridGoodsStock.MasterTableView.ExportToExcel();
        }

        /// <summary>绑定往来单位数据源
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> BindCompanyDataBound()
        {
            var dic = new Dictionary<string, string> { { Guid.Empty.ToString(), "供应商列表" } };
            var companyList = _companyCussent.GetCompanyCussentList().Where(ent => ent.State == (int)State.Enable);
            foreach (var info in companyList)
            {
                dic.Add(info.CompanyId.ToString(), info.CompanyName);
            }
            return dic;
        }

        #region [ViewState参数]
        /// <summary>商品名字或CODE
        /// </summary>
        protected string SearchText
        {
            get
            {
                if (ViewState["SearchKey"] == null) return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set { ViewState["SearchKey"] = value; }
        }

        /// <summary>仓库ID
        /// </summary>
        public Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null)
                {
                    ViewState["WarehouseId"] = Guid.Empty;
                }
                return new Guid(ViewState["WarehouseId"].ToString());
            }
            set { ViewState["WarehouseId"] = value.ToString(); }
        }

        /// <summary>物流配送公司
        /// </summary>
        public Guid HostingFilialeId
        {
            get
            {
                if (ViewState["HostingFilialeId"] == null)
                {
                    ViewState["HostingFilialeId"] = Guid.Empty;
                }
                return new Guid(ViewState["HostingFilialeId"].ToString());
            }
            set { ViewState["HostingFilialeId"] = value.ToString(); }
        }

        /// <summary>商品分类ID
        /// </summary>
        protected Guid GoodsClassId
        {
            get
            {
                if (ViewState["GoodsClassId"] == null) return Guid.Empty;
                return new Guid(ViewState["GoodsClassId"].ToString());
            }
            set { ViewState["GoodsClassId"] = value.ToString(); }
        }

        /// <summary>供应商ID
        /// </summary>
        protected Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set { ViewState["CompanyId"] = value.ToString(); }
        }

        #endregion

        protected void GridGoodsStockItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                if (HostingFilialeId == Guid.Empty)
                {
                    string row1 = "<tr>";
                    foreach (var filiale in FilialeDic)
                    {
                        row1 += "<td class='Group' style='width:160px;padding-top:5px; padding-bottom:5px; border-bottom:1px solid #3d556c;" + (" border-left:1px solid #3d556c;") + "'>" + filiale.Value + "(库存)</td>";
                    }
                    row1 += "</tr>";
                    var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\">" + row1 + "</table>";
                    e.Item.Cells[4].Text = headerText;
                }
            }
            else if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (HostingFilialeId == Guid.Empty)
                {
                    var goodsId = new Guid(((GridDataItem)e.Item).GetDataKeyValue("GoodsId").ToString());
                    var goodsStocks = StockSearchDic[goodsId];
                    string row = "<tr>";
                    int i = 0;
                    foreach (var filiale in FilialeDic)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            int stockQuantity = goodsStocks.ContainsKey(filiale.Key) ? goodsStocks[filiale.Key] : 0;
                            if (i == 0 && j == 0)
                            {
                                row += "<td style='text-align:center;'>" + stockQuantity + "</td>";
                                continue;
                            }
                            if (j == 0)
                            {
                                row += "<td style='text-align:center;'>" + stockQuantity + "</td>";
                            }
                        }
                        i++;
                    }
                    row += "</tr>";
                    var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\" ondblclick=\"DbClick('" + WarehouseId + "','" + goodsId + "')\">" + row + "</table>";
                    e.Item.Cells[4].Text = headerText;
                }
            }
        }
    }
}