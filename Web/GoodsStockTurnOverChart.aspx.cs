using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using Telerik.Charting;
using Telerik.Web.UI;
using ERP.DAL.Interface.IStorage;
using ERP.DAL.Implement.Storage;

namespace ERP.UI.Web
{
    /// <summary>新版存货周转率  2015-04-23  陈重文
    /// </summary>
    public partial class GoodsStockTurnOver : BasePage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        /// <summary>30天内销量数据
        /// </summary>
        private static Dictionary<Guid, int> _salesDic = new Dictionary<Guid, int>();

        /// <summary>加权月平均销量
        /// </summary>
        private static Dictionary<Guid, int> _weightedAverageSaleDic = new Dictionary<Guid, int>();
        readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Read);

        #region [页面加载Page_Load]

        /// <summary>页面加载Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                GetTreeGoodsClass();
            }
        }

        #endregion

        /// <summary>
        /// zal 2017-03-28
        /// </summary>
        protected void LoadSales()
        {
            _salesDic.Clear();
            _weightedAverageSaleDic.Clear();
            var list = GoodsStockPile.ReadInstance.GetGoodsSalesVolume(WarehouseId);
            //获取30天商品销量字典
            _salesDic = list.ToDictionary(ent => ent.GoodsId, ent => ent.ThirtyDaySales);
            _weightedAverageSaleDic = list.ToDictionary(ent => ent.GoodsId, ent => ent.WeightedAverageSaleQuantity);
        }

        #region [加载公司，仓库，采购人]

        /// <summary>加载公司，仓库，采购人
        /// </summary>
        private void LoadData()
        {
            //仓库列表
            var warehouseDics = WMSSao.GetWarehouseAuth(CurrentSession.Personnel.Get().PersonnelId);
            if (warehouseDics != null)
            {
                WarehouseAuths = warehouseDics;
                foreach (var warehouseDic in warehouseDics)
                {
                    RCB_Warehouse.Items.Add(new RadComboBoxItem(warehouseDic.WarehouseName, warehouseDic.WarehouseId.ToString()));
                }
                RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("--请选择仓库--", Guid.Empty.ToString()));
            }
            RCB_Warehouse.SelectedIndex = 0;
            //WarehouseId = GlobalConfig.MainWarehouseID;

            RCB_Personnel.DataTextField = "RealName";
            RCB_Personnel.DataValueField = "PersonnelId";
            RCB_Personnel.DataSource = PersonnelList;
            RCB_Personnel.DataBind();
            RCB_Personnel.Items.Insert(0, new RadComboBoxItem("全部采购人", Guid.Empty.ToString()));
            RCB_Personnel.SelectedIndex = 0;
            PersonnelId = Guid.Empty;

            var list = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers, Enum.State.Enable).ToList();
            RCB_Company.DataSource = list;
            RCB_Company.DataTextField = "CompanyName";
            RCB_Company.DataValueField = "CompanyId";
            RCB_Company.DataBind();
            RCB_Company.Items.Insert(0, new RadComboBoxItem("全部供应商", Guid.Empty.ToString()));
            RCB_Company.SelectedIndex = 0;
            CompanyId = Guid.Empty;
        }

        #endregion

        #region [商品分类树]

        /// <summary>加载商品分类树
        /// </summary>
        private void GetTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            TVGoodsClass.Nodes.Add(rootNode);
            IList<GoodsClassInfo> goodsClassList = _goodsCenterSao.GetAllClassList().ToList();
            RecursivelyGoodsClass(Guid.Empty, rootNode, goodsClassList);
        }

        /// <summary>遍历产品分类
        /// </summary>
        private void RecursivelyGoodsClass(Guid goodsClassId, RadTreeNode node, IList<GoodsClassInfo> goodsClassList)
        {
            IList<GoodsClassInfo> childGoodsClassList = goodsClassList.Where(w => w.ParentClassId == goodsClassId).ToList();
            foreach (GoodsClassInfo goodsClassInfo in childGoodsClassList)
            {
                string sAdd = string.Empty;
                RadTreeNode goodsClassNode = CreateNode(goodsClassInfo.ClassName + (sAdd == String.Empty ? string.Empty : sAdd), false, goodsClassInfo.ClassId.ToString());

                if (node == null)
                    TVGoodsClass.Nodes.Add(goodsClassNode);
                else
                    node.Nodes.Add(goodsClassNode);
                RecursivelyGoodsClass(goodsClassInfo.ClassId, goodsClassNode, goodsClassList);
            }
        }

        /// <summary>创建节点
        /// </summary>
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { Expanded = expanded };
            return node;
        }

        #endregion

        #region [商品分类树选择事件]

        /// <summary>商品分类树选择事件
        /// </summary>
        protected void TvGoodsClass_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                GoodsClassId = new Guid(e.Node.Value);
                if (new Guid(RCB_Warehouse.SelectedValue).Equals(Guid.Empty))
                {
                    RAM.Alert("请选择仓库！");
                    return;
                }
                GridGoodsStock.Rebind();
            }
        }

        #endregion

        #region [搜索]

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IbCreationDataClick(object sender, ImageClickEventArgs e)
        {
            if (new Guid(RCB_Warehouse.SelectedValue).Equals(Guid.Empty))
            {
                RAM.Alert("请选择仓库！");
                return;
            }
            if (!string.IsNullOrWhiteSpace(RCB_State.SelectedValue))
                State = Convert.ToInt32(RCB_State.SelectedValue);
            if (!string.IsNullOrWhiteSpace(RCB_Personnel.SelectedValue))
                PersonnelId = new Guid(RCB_Personnel.SelectedValue);
            if (!string.IsNullOrWhiteSpace(RCB_Company.SelectedValue))
                CompanyId = new Guid(RCB_Company.SelectedValue);
            if (!string.IsNullOrWhiteSpace(RCB_SaleFiliale.SelectedValue))
                FilialeId = new Guid(RCB_SaleFiliale.SelectedValue);
            GoodsNameOrCode = RTB_GoodsNameOrCode.Text.Trim();
            GridGoodsStock.Rebind();
        }

        #endregion

        #region [报表点击事件]

        /// <summary>报表点击事件
        /// </summary>
        protected void GoodsStockTurnOverRadChart_OnClick(object sender, ChartClickEventArgs args)
        {
            RAM.ResponseScripts.Add(string.Format("return GoodsStockTurnOverRadChartClick('{0}','{1}','{2}',{3})", GoodsClassId, PersonnelId, GoodsNameOrCode, State));
        }

        #endregion

        #region [绑定数据源]

        /// <summary>绑定数据源
        /// </summary>
        protected void GridGoodsStock_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<StockTurnOverInfo> stockTurnOverList = new List<StockTurnOverInfo>();
            if (!WarehouseId.Equals(Guid.Empty))
            {
                //选择了责任人，则获取当前责任人负责的商品ID集合
                var goodsIds = new List<Guid>();
                if (PersonnelId != Guid.Empty || CompanyId != Guid.Empty)
                    goodsIds = _purchaseSet.GetGoodsIdByPersonnelId(PersonnelId, CompanyId).ToList();
                //根据条件获取商品信息
                List<GoodsPerformance> goodsList = _goodsCenterSao.GetGoodsPerformanceList(GoodsClassId, goodsIds,
                    GoodsNameOrCode, CB_IsPerformance.Checked);
                var tempGoodsIds = new List<Guid>();
                //var isNeedStock = true;
                switch (State)
                {
                    case 0: //全部
                        tempGoodsIds = goodsList.Select(ent => ent.GoodsId).ToList();
                        break;
                    case 1: //下架缺货有库存
                            //备注:  PurchaseState 0下架, 1上架
                        tempGoodsIds.AddRange(from item in goodsList
                                              where item.IsScarcity || item.PurchaseState == 0
                                              select item.GoodsId);
                        //isNeedStock = true;
                        break;
                    case 2: //无销售商品
                        tempGoodsIds.AddRange(from item in goodsList
                                              where !_salesDic.ContainsKey(item.GoodsId)
                                              select item.GoodsId);
                        var newGoodsList =
                            tempGoodsIds.Select(goodsId => goodsList.FirstOrDefault(ent => ent.GoodsId == goodsId)).ToList();
                        goodsList.Clear();
                        goodsList.AddRange(newGoodsList);
                        break;
                }

                //获取商品库存信息
                var goodsStockList = WMSSao.StockSearchByGoodsIds(tempGoodsIds, WarehouseId, FilialeId) ?? new Dictionary<Guid, int>();
                //获取所有的商品采购设置
                var allGoodsPurchaseSet = _purchaseSet.GetAllPurchaseSet(WarehouseId);

                var companyIdList = allGoodsPurchaseSet.Select(p => p.CompanyId).Distinct().ToList();
                //获取商品的最后一次进货价信息
                var goodsPurchaseLastPriceInfoList = _storageRecordDao.GetGoodsPurchaseLastPriceInfoByWarehouseId(WarehouseId);
                goodsPurchaseLastPriceInfoList = goodsPurchaseLastPriceInfoList.Where(p => tempGoodsIds.Contains(p.GoodsId) && companyIdList.Contains(p.ThirdCompanyId)).ToList();

                foreach (var goodsInfo in goodsList)
                {
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var info = new StockTurnOverInfo();
                    info.GoodsID = goodsInfo.GoodsId;
                    info.GoodsName = goodsInfo.GoodsName;
                    info.GoodsCode = goodsInfo.GoodsCode;
                    info.IsStatisticalPerformance = goodsInfo.IsStatisticalPerformance;
                    info.IsStatisticalPerformanceStr = goodsInfo.IsStatisticalPerformance ? "√" : string.Empty;
                    info.IsScarcity = goodsInfo.IsScarcity;
                    info.IsScarcityStr = goodsInfo.IsScarcity ? "√" : string.Empty;
                    info.State = goodsInfo.PurchaseState == 0;
                    info.IsStateStr = goodsInfo.PurchaseState == 0 ? "√" : string.Empty;
                    //库存信息获取
                    var stockQuantity = goodsStockList.ContainsKey(goodsInfo.GoodsId)
                        ? goodsStockList[goodsInfo.GoodsId]
                        : 0;
                    if (stockQuantity <= 0)
                        continue;
                    info.StockNums = stockQuantity;
                    if (!_salesDic.ContainsKey(goodsInfo.GoodsId))
                        info.StockNumSort = stockQuantity;

                    //责任人供应商信息获取
                    var tempGoodsPurchaseSetInfo = allGoodsPurchaseSet.FirstOrDefault(ent => ent.GoodsId == goodsInfo.GoodsId);

                    //根据商品id获取供应商
                    var companyId = tempGoodsPurchaseSetInfo!=null ? tempGoodsPurchaseSetInfo.CompanyId : Guid.Empty;

                    decimal unitPrice = 0;
                    GoodsPurchaseLastPriceInfo goodsPurchaseLastPriceInfo = null;
                    if (goodsPurchaseLastPriceInfoList.Count > 0)
                    {
                        goodsPurchaseLastPriceInfo = goodsPurchaseLastPriceInfoList.FirstOrDefault(p => p.GoodsId.Equals(goodsInfo.GoodsId) && p.ThirdCompanyId.Equals(companyId));
                        unitPrice = goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.UnitPrice : 0;
                    }
                    info.RecentInPrice = unitPrice;
                    info.RecentCDate = (goodsPurchaseLastPriceInfo != null ? goodsPurchaseLastPriceInfo.LastPriceDate : DateTime.MinValue).ToString("yyyy-MM-dd");

                    if (tempGoodsPurchaseSetInfo != null)
                    {
                        info.PersonResponsible = tempGoodsPurchaseSetInfo.PersonResponsible;
                        var firstOrDefault = PersonnelList.FirstOrDefault(ent => ent.PersonnelId == tempGoodsPurchaseSetInfo.PersonResponsible);
                        if (firstOrDefault != null)
                            info.PersonResponsibleName = firstOrDefault.RealName;
                        info.CompanyId = tempGoodsPurchaseSetInfo.CompanyId;
                        info.CompanyName = tempGoodsPurchaseSetInfo.CompanyName;
                    }
                    //销售数量
                    if (_salesDic.ContainsKey(goodsInfo.GoodsId))
                    {
                        info.SaleNums = _salesDic[goodsInfo.GoodsId];
                        info.SaleNumSort = 1;
                        info.StockNumSort = 0;
                    }

                    #region [计算商品库存周转情况(天数)]
                    if (info.State && info.StockNums == 0)
                    {
                        info.TurnOverDays = 0;
                        info.TurnOverStr = "下架";
                    }
                    else if (info.State && info.StockNums != 0)
                    {
                        info.TurnOverDays = 0;
                        info.TurnOverStr = "下架有库存";
                    }
                    else if (info.IsScarcity && info.StockNums == 0)
                    {
                        info.TurnOverDays = 0;
                        info.TurnOverStr = "缺货";
                    }
                    else if (info.IsScarcity && info.StockNums != 0)
                    {
                        info.TurnOverDays = 0;
                        info.TurnOverStr = "缺货有库存";
                    }
                    else if (info.StockNums == 0)
                    {
                        info.TurnOverDays = 0;
                        info.TurnOverStr = "0天";
                    }
                    else if (info.SaleNums > 0)
                    {
                        var tempTurnOverDays = info.StockNums * 30 / info.SaleNums;
                        info.TurnOverDays = tempTurnOverDays;
                        info.TurnOverStr = tempTurnOverDays + "天";
                    }
                    else
                    {
                        info.TurnOverDays = 0;
                        info.TurnOverStr = "无销售";
                    }
                    #endregion

                    #region [计算商品报备周转天数]

                    if (_weightedAverageSaleDic.ContainsKey(info.GoodsID))
                    {
                        var weightedAverageSale = _weightedAverageSaleDic[info.GoodsID];
                        if (_weightedAverageSaleDic[info.GoodsID] != 0)
                            info.TurnOverByFiling = info.StockNums * 30 / weightedAverageSale + "天";
                    }

                    #endregion

                    stockTurnOverList.Add(info);
                }
            }
            var pageIndex = GridGoodsStock.CurrentPageIndex;
            var pageSize = GridGoodsStock.PageSize;
            stockTurnOverList = stockTurnOverList.OrderBy(ent => ent.SaleNumSort).ThenByDescending(ent => ent.StockNumSort).ThenByDescending(ent => ent.TurnOverDays).ToList();
            GridGoodsStock.DataSource = stockTurnOverList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            GridGoodsStock.VirtualItemCount = stockTurnOverList.Count;
        }

        #endregion

        #region [生成折线图]

        /// <summary>生成折线图
        /// </summary>
        protected void IbCreationChartClick(object sender, ImageClickEventArgs e)
        {
            RAM.ResponseScripts.Add(string.Format("return GoodsStockTurnOverRadChartClick('{0}','{1}','{2}','{3}','{4}','{5}')", GoodsClassId, PersonnelId, CompanyId, GoodsNameOrCode, State, CB_IsPerformance.Checked));
        }

        #endregion

        #region [导出Excel]

        /// <summary>导出Excel
        /// </summary>
        protected void Ib_ExportData_Click(object sender, ImageClickEventArgs e)
        {
            string fileName = "库存周转率";
            fileName = Server.UrlEncode(fileName);
            GridGoodsStock.ExportSettings.ExportOnlyData = true;
            GridGoodsStock.HorizontalAlign = HorizontalAlign.Right;
            GridGoodsStock.ExportSettings.IgnorePaging = true;
            GridGoodsStock.ExportSettings.FileName = fileName;
            GridGoodsStock.MasterTableView.ExportToExcel();
        }

        #endregion

        #region [ViewState]

        /// <summary>商品分类ID
        /// </summary>
        protected Guid GoodsClassId
        {
            get
            {
                if (ViewState["GoodsClassId"] == null)
                    ViewState["GoodsClassId"] = Guid.Empty;
                return new Guid(ViewState["GoodsClassId"].ToString());
            }
            set
            {
                ViewState["GoodsClassId"] = value;
            }
        }

        /// <summary>公司ID
        /// </summary>
        protected Guid FilialeId
        {
            get
            {
                if (ViewState["FilialeId"] == null)
                    ViewState["FilialeId"] = Guid.Empty;
                return new Guid(ViewState["FilialeId"].ToString());
            }
            set
            {
                ViewState["FilialeId"] = value;
            }
        }

        /// <summary>仓库ID
        /// </summary>
        protected Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null)
                    ViewState["WarehouseId"] = Guid.Empty;
                return new Guid(ViewState["WarehouseId"].ToString());
            }
            set
            {
                ViewState["WarehouseId"] = value;
            }
        }

        /// <summary>状态 0全部，1下架缺货有库存，2无销售商品
        /// </summary>
        protected int State
        {
            get
            {
                if (ViewState["State"] == null)
                    ViewState["State"] = 0;
                return Convert.ToInt32(ViewState["State"].ToString());
            }
            set
            {
                ViewState["State"] = value;
            }
        }

        /// <summary>采购人ID
        /// </summary>
        protected Guid PersonnelId
        {
            get
            {
                if (ViewState["PersonnelId"] == null)
                    ViewState["PersonnelId"] = Guid.Empty;
                return new Guid(ViewState["PersonnelId"].ToString());
            }
            set
            {
                ViewState["PersonnelId"] = value;
            }
        }

        /// <summary>授权仓储和物流配送公司
        /// </summary>
        protected List<WarehouseAuth> WarehouseAuths
        {
            get
            {
                if (ViewState["WarehouseAuths"] == null)
                    ViewState["WarehouseAuths"] = new List<WarehouseAuth>();
                return (List<WarehouseAuth>)ViewState["WarehouseAuths"];
            }
            set
            {
                ViewState["WarehouseAuths"] = value;
            }
        }

        /// <summary>供应商ID
        /// </summary>
        protected Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null)
                    ViewState["CompanyId"] = Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value;
            }
        }

        /// <summary>商品名称/编号
        /// </summary>
        protected string GoodsNameOrCode
        {
            get
            {
                if (ViewState["GoodsNameOrCode"] == null)
                    ViewState["GoodsNameOrCode"] = string.Empty;
                return ViewState["GoodsNameOrCode"].ToString();
            }
            set
            {
                ViewState["GoodsNameOrCode"] = value;
            }
        }

        /// <summary>
        /// 获取(采购部)所有员工的信息
        /// </summary>
        /// <returns></returns>
        protected IList<PersonnelInfo> PersonnelList
        {
            get
            {
                if (ViewState["PersonnelList"] == null)
                {
                    var systemBranchId = new Guid("D9D6002C-196C-4375-B41A-E7040FE12B09"); //系统部门ID
                    var systemPostionList = MISService.GetAllSystemPositionList().ToList();
                    var positonIds = systemPostionList.Where(
                        act => act.ParentSystemBranchID == systemBranchId || act.SystemBranchID == systemBranchId)
                        .Select(act => act.SystemBranchPositionID);
                    IList<PersonnelInfo> list = new PersonnelSao().GetList().Where(ent => positonIds.Contains(ent.SystemBrandPositionId) && ent.IsActive).ToList();
                    ViewState["PersonnelList"] = list;
                }
                return (IList<PersonnelInfo>)ViewState["PersonnelList"];
            }
        }

        #endregion

        protected void RcbWarehouseSelectChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RCB_SaleFiliale.Items.Clear();
            if (!string.IsNullOrWhiteSpace(e.Value) && e.Value != Guid.Empty.ToString())
            {
                if (WarehouseAuths.Count > 0)
                {
                    var warehouse = WarehouseAuths.FirstOrDefault(act => act.WarehouseId == new Guid(e.Value));
                    BindHostingFiliale(warehouse);
                }
                WarehouseId = new Guid(RCB_Warehouse.SelectedValue);
                LoadSales();
            }
        }

        public void BindHostingFiliale(WarehouseAuth warehouse)
        {
            var hostingFilialeAuths = new List<HostingFilialeAuth>();
            if (warehouse != null)
            {
                foreach (var storage in warehouse.Storages)
                {
                    foreach (var hostingFilialeAuth in storage.Filiales.Where(act => act.HostingFilialeId != Guid.Empty))
                    {
                        if (hostingFilialeAuths.Any(act => act.HostingFilialeId == hostingFilialeAuth.HostingFilialeId))
                            continue;
                        hostingFilialeAuths.Add(hostingFilialeAuth);
                    }
                }
            }
            var datasource = new List<HostingFilialeAuth>();
            if (hostingFilialeAuths.Count > 0)
            {
                datasource.Add(new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "--请选择--" });
            }
            datasource.AddRange(hostingFilialeAuths);
            RCB_SaleFiliale.DataSource = datasource;
            RCB_SaleFiliale.DataTextField = "HostingFilialeName";
            RCB_SaleFiliale.DataValueField = "HostingFilialeId";
            RCB_SaleFiliale.DataBind();
        }
    }
}