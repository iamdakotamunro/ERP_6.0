using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ERP.BLL.Implement.Purchasing;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class StockDeclareAw : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindInStock();
                BindCompany();
            }
        }

        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Read);
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        #region 声明的变量

        public Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null)
                {
                    var value = RCB_Warehouse.SelectedValue;
                    return string.IsNullOrEmpty(value) ? Guid.Empty : new Guid(value);
                }
                return new Guid(ViewState["WarehouseId"].ToString());
            }
            set { ViewState["WarehouseId"] = value.ToString(); }
        }

        public Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set { ViewState["CompanyId"] = value.ToString(); }
        }

        public string KeyWords
        {
            get
            {
                if (ViewState["KeyWords"] == null) return string.Empty;
                return ViewState["KeyWords"].ToString();
            }
            set { ViewState["KeyWords"] = value; }
        }
        #endregion

        public WarehouseAuthAndFilialeDTO WarehouseAuth
        {
            get
            {
                if (ViewState["WarehouseAuth"] == null)
                    return new WarehouseAuthAndFilialeDTO();
                return (WarehouseAuthAndFilialeDTO)ViewState["WarehouseAuth"];
            }
            set { ViewState["WarehouseAuth"] = value; }
        }

        public List<PurchaseDeclarationDTO> StockDeclareDtos
        {
            get
            {
                if (ViewState["StockDeclareDtos"] == null)
                    return new List<PurchaseDeclarationDTO>();
                return (List<PurchaseDeclarationDTO>)ViewState["StockDeclareDtos"];
            }
            set { ViewState["StockDeclareDtos"] = value; }
        }

        protected void GridRGGoodsDemand_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var datasource = new List<StockDeclareDTO>();
            if (Page.IsPostBack)
            {
                if (!e.IsFromDetailTable)
                {
                    var purchasingSets = CompanyId != Guid.Empty
                        ? _purchaseSet.GetPurchaseSetListByWarehouseIdAndCompanyId(WarehouseId, CompanyId) :
                        _purchaseSet.GetPurchaseSetListByWarehouseId(WarehouseId);
                    if (purchasingSets == null || purchasingSets.Count == 0)
                    {
                        RAM.Alert("该仓库未找到对应的采购设置商品！");
                        RGGoodsDemand.DataSource = new List<StockDeclareDTO>();
                        return;
                    }

                    var dics = WMSSao.GetStockDeclareDtos(WarehouseId, new List<Guid>());
                    if (!string.IsNullOrWhiteSpace(TextBoxKeys.Text))
                    {
                        var realGoodsIds = _goodsCenterSao.GetRealGoodsIdListByGoodsNameOrCode(TextBoxKeys.Text);
                        if (realGoodsIds.Count > 0)
                        {
                            dics = dics.Where(act => realGoodsIds.Contains(act.Key)).ToDictionary(k => k.Key, v => v.Value);
                        }
                    }
                    var realGoodsInfos = _goodsCenterSao.GetStockDeclareGridList(dics.Keys.ToList()).ToDictionary(k => k.RealGoodsId, v => v);
                    var purchasingGoodsDics = _purchasingDetail.GetStockDeclarePursingGoodsDicsWithFiliale(WarehouseId, dics.Values.SelectMany(ent => ent).Select(ent => ent.FilialeId),
                        new[] { PurchasingState.NoSubmit, PurchasingState.Purchasing, PurchasingState.PartComplete, PurchasingState.StockIn, PurchasingState.WaitingAudit, PurchasingState.Refusing },
                        dics.Keys.ToList());
                    List<StockDeclareDTO> data;
                    StockDeclareDtos = GetDataList(dics, purchasingGoodsDics, realGoodsInfos, purchasingSets, out data);
                    datasource.AddRange(data);
                }
                else //加载对应物流公司申报数
                {
                    datasource = GetDataList(StockDeclareDtos);
                }
            }
            RGGoodsDemand.DataSource = datasource.OrderBy(w => w.GoodsName).ThenBy(w => w.Sku).ToList();
        }

        public List<PurchaseDeclarationDTO> GetDataList(Dictionary<Guid, List<PurchaseDeclarationDTO>> dics, Dictionary<Guid, Dictionary<Guid, int>> purchasingGoodsDics, Dictionary<Guid, ChildGoodsInfo> goodsInfos
            , IList<PurchaseSetInfo> purchaseSetInfos, out List<StockDeclareDTO> list)
        {
            var purchasingSetDics = purchaseSetInfos.GroupBy(ent => ent.HostingFilialeId).ToDictionary(k => k.Key, v => v.ToDictionary(kk => kk.GoodsId, vv => vv));
            var allDeclaretionInfos = dics.Where(ent => goodsInfos.ContainsKey(ent.Key)).SelectMany(ent => ent.Value);
            List<PurchaseDeclarationDTO> newDatasource = new List<PurchaseDeclarationDTO>();
            list = new List<StockDeclareDTO>();
            if (CompanyId != Guid.Empty)
            {
                var companyInfo = _companyCussent.GetCompanyCussent(CompanyId);
                if (companyInfo != null)
                {
                    foreach (var filiaeAndGoodsGroup in allDeclaretionInfos.Where(ent => purchasingSetDics.ContainsKey(ent.FilialeId) && purchasingSetDics[ent.FilialeId].ContainsKey(ent.GoodsId)).GroupBy(ent => new { ent.FilialeId, ent.GoodsId }))
                    {
                        var purchaseSetInfo = purchasingSetDics[filiaeAndGoodsGroup.Key.FilialeId][filiaeAndGoodsGroup.Key.GoodsId];
                        foreach (var item in filiaeAndGoodsGroup)
                        {
                            var goodsInfo = goodsInfos[item.RealGoodsId];
                            item.GoodsCode = goodsInfo.GoodsCode;
                            item.Units = goodsInfo.Units;
                            item.GoodsName = goodsInfo.GoodsName;
                            item.Sku = goodsInfo.Specification;
                            item.CompanyId = companyInfo.CompanyId;
                            item.CompanyName = companyInfo.CompanyName;
                            item.PersonResponsible = purchaseSetInfo.PersonResponsible;
                            item.PersonResponsibleName = purchaseSetInfo.PersonResponsibleName;
                            item.PurchasePrice = purchaseSetInfo.PurchasePrice;
                            if (purchasingGoodsDics.ContainsKey(item.FilialeId) &&
                                purchasingGoodsDics[item.FilialeId].ContainsKey(item.RealGoodsId))
                            {
                                item.Quantity -= purchasingGoodsDics[item.FilialeId][item.RealGoodsId];
                            }
                            if (item.Quantity > 0)
                                newDatasource.Add(item);
                        }
                    }
                }
            }
            else
            {
                var companyCussents = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers, State.Enable).ToDictionary(k => k.CompanyId, v => v.CompanyName);
                foreach (var filiaeAndGoodsGroup in allDeclaretionInfos.Where(ent => purchasingSetDics.ContainsKey(ent.FilialeId)).GroupBy(ent => new { ent.FilialeId, ent.GoodsId }))
                {
                    if (!purchasingSetDics[filiaeAndGoodsGroup.Key.FilialeId].ContainsKey(filiaeAndGoodsGroup.Key.GoodsId)) continue;
                    var purchaseSetInfo = purchasingSetDics[filiaeAndGoodsGroup.Key.FilialeId][filiaeAndGoodsGroup.Key.GoodsId];
                    foreach (var item in filiaeAndGoodsGroup)
                    {
                        var goodsInfo = goodsInfos[item.RealGoodsId];
                        item.GoodsCode = goodsInfo.GoodsCode;
                        item.Units = goodsInfo.Units;
                        item.GoodsName = goodsInfo.GoodsName;
                        item.Sku = goodsInfo.Specification;
                        item.CompanyName = companyCussents.ContainsKey(purchaseSetInfo.CompanyId) ? companyCussents[purchaseSetInfo.CompanyId] : "";
                        item.CompanyId = purchaseSetInfo.CompanyId;
                        item.PersonResponsible = purchaseSetInfo.PersonResponsible;
                        item.PersonResponsibleName = purchaseSetInfo.PersonResponsibleName;
                        item.PurchasePrice = purchaseSetInfo.PurchasePrice;
                        if (purchasingGoodsDics.ContainsKey(item.FilialeId) &&
                            purchasingGoodsDics[item.FilialeId].ContainsKey(item.RealGoodsId))
                        {
                            item.Quantity -= purchasingGoodsDics[item.FilialeId][item.RealGoodsId];
                        }
                        if (item.Quantity > 0)
                            newDatasource.Add(item);
                    }
                }

            }
            if (newDatasource.Count > 0)
            {
                list = (from item in newDatasource.GroupBy(ent => ent.RealGoodsId)
                        let childInfo = goodsInfos[item.Key]
                        let first = item.First()
                        select new StockDeclareDTO
                        {
                            RealGoodsId = item.Key,
                            CompanyId = first.CompanyId,
                            CompanyName = first.CompanyName,
                            Demand = item.Sum(act => act.Quantity),
                            PurchaseQuantity = item.Sum(act => act.Quantity),
                            NonceGoodsStock = item.Sum(act => act.CurrentQuantity),
                            Sku = childInfo.Specification,
                            GoodsId = childInfo.GoodsId,
                            GoodsName = childInfo.GoodsName
                        }).ToList();
            }
            return newDatasource;
        }

        private List<StockDeclareDTO> GetDataList(List<PurchaseDeclarationDTO> dics)
        {
            var datasource = new List<StockDeclareDTO>();
            if (CompanyId != Guid.Empty)
            {
                var companyInfo = _companyCussent.GetCompanyCussent(CompanyId);
                if (companyInfo != null)
                {
                    datasource.AddRange(from item in dics.GroupBy(ent => ent.RealGoodsId)
                                        let first = item.First()
                                        select new StockDeclareDTO
                                        {
                                            RealGoodsId = item.Key,
                                            CompanyId = first.CompanyId,
                                            CompanyName = first.CompanyName,
                                            Demand = item.Sum(act => act.Quantity),
                                            PurchaseQuantity = item.Sum(act => act.Quantity),
                                            NonceGoodsStock = item.Sum(act => act.CurrentQuantity),
                                            Sku = first.Sku,
                                            GoodsId = first.GoodsId,
                                            GoodsName = first.GoodsName
                                        });
                }
            }
            else
            {
                datasource.AddRange(from item in dics.GroupBy(ent => ent.RealGoodsId)
                                    let first = item.First()
                                    select new StockDeclareDTO
                                    {
                                        RealGoodsId = item.Key,
                                        CompanyId = first.CompanyId,
                                        CompanyName = first.CompanyName,
                                        Demand = item.Sum(act => act.Quantity),
                                        PurchaseQuantity = item.Sum(act => act.Quantity),
                                        NonceGoodsStock = item.Sum(act => act.CurrentQuantity),
                                        Sku = first.Sku,
                                        GoodsId = first.GoodsId,
                                        GoodsName = first.GoodsName
                                    });
            }
            return datasource;
        }

        protected string GetWarehouseName(object objWarehouseId)
        {
            if (objWarehouseId == null)
                return string.Empty;
            return WarehouseAuth.WarehouseDics != null &&
                   WarehouseAuth.WarehouseDics.ContainsKey(new Guid(objWarehouseId.ToString()))
                ? WarehouseAuth.WarehouseDics[new Guid(objWarehouseId.ToString())]
                : string.Empty;
        }

        protected string GetFilialeName(object filialeId)
        {
            if (filialeId == null)
                return string.Empty;
            return WarehouseAuth.HostingFilialeDic != null &&
                   WarehouseAuth.HostingFilialeDic.ContainsKey(new Guid(filialeId.ToString()))
                ? WarehouseAuth.HostingFilialeDic[new Guid(filialeId.ToString())]
                : string.Empty;
        }

        #region 分公司和仓库绑定

        private void BindInStock()
        {
            var personnel = CurrentSession.Personnel.Get();
            var result = WMSSao.GetWarehouseAuthDic(personnel.PersonnelId);
            WarehouseAuth = result;
            RCB_Warehouse.DataSource = result != null && result.WarehouseDics != null ? result.WarehouseDics : new Dictionary<Guid, string>();
            RCB_Warehouse.DataTextField = "Value";
            RCB_Warehouse.DataValueField = "Key";
            RCB_Warehouse.DataBind();
            if (result != null && result.WarehouseDics != null && result.WarehouseDics.Count > 0)
            {
                RCB_Warehouse.SelectedValue = result.WarehouseDics.First().Key.ToString();
            }
        }

        private void BindCompany()
        {
            RCB_Company.Items.Clear();
            RCB_Company.DataSource = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers).OrderBy(w => w.CompanyName).ToList();
            RCB_Company.DataTextField = "CompanyName";
            RCB_Company.DataValueField = "CompanyId";
            RCB_Company.DataBind();
            RCB_Company.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
        }
        #endregion

        // 生成报表
        protected void Ib_CreationData_Click(object sender, ImageClickEventArgs e)
        {
            WarehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            CompanyId = string.IsNullOrEmpty(RCB_Company.SelectedValue) ? Guid.Empty : new Guid(RCB_Company.SelectedValue);
            KeyWords = TextBoxKeys.Text;
            RGGoodsDemand.Rebind();
        }

        // 生成采购单
        protected void Ib_CreatePurchase_Click(object sender, EventArgs e)
        {
            if (RDP_ArrivalTime.SelectedDate == null)
            {
                RAM.Alert("请选择到货日期");
                return;
            }
            if (!StockDeclareDtos.Any())
            {
                RAM.Alert("暂时没有要进货的数据！");
                return;
            }
            var failMsg = new StringBuilder();
            StockDeclareManager.WriteInstance.BuilderPurchasing(StockDeclareDtos, null, WarehouseId, RDP_ArrivalTime.SelectedDate, CurrentSession.Personnel.Get().RealName, ref failMsg);
            RAM.Alert(!string.IsNullOrEmpty(failMsg.ToString().Trim()) ? failMsg.ToString() : "采购单生成成功!");
            RGGoodsDemand.Rebind();
        }

        public string SortField
        {
            get
            {
                if (ViewState["SortField"] == null)
                {
                    return "GoodsCode";
                }
                return ViewState["SortField"].ToString();
            }
            set { ViewState["SortField"] = value; }
        }

        /// <summary> 用户-排序方式
        /// </summary>
        public string SortStyle
        {
            get
            {
                if (ViewState["SortStyle"] == null)
                {
                    return "desc";
                }
                return ViewState["SortStyle"].ToString();
            }
            set { ViewState["SortStyle"] = value; }
        }

        /// <summary> 用户-排序
        /// </summary>
        private string SortUser
        {
            get
            {
                if (ViewState["WareHouseId"] == null)
                {
                    return "GoodsCode asc";
                }
                return ViewState["WareHouseId"].ToString();
            }
            set { ViewState["WareHouseId"] = value; }
        }

        protected void RGGoodsDemand_SortCommand(object source, GridSortCommandEventArgs e)
        {
            SortField = e.SortExpression;
            if (e.NewSortOrder == GridSortOrder.Ascending)
            {
                SortStyle = "desc";
                SortUser = string.Format("{0}  {1}  {2}", SortField, "", SortStyle);
                RGGoodsDemand.MasterTableView.SortExpressions.AddSortExpression(SortUser);
            }
            if (e.OldSortOrder == GridSortOrder.Descending)
            {
                SortStyle = "asc";
                SortUser = string.Format("{0}  {1}  {2}", SortField, "", SortStyle);
                RGGoodsDemand.MasterTableView.SortExpressions.AddSortExpression(SortUser);
            }
        }

        protected void RGGoodsDemandDetail_NeedDataSource(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = e.DetailTableView.ParentItem;
            var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
            var dataList = StockDeclareDtos.Where(ent => ent.RealGoodsId == realGoodsId);
            foreach (var purchaseDeclarationDTO in dataList)
            {
                purchaseDeclarationDTO.PurchaseQuantity = purchaseDeclarationDTO.PurchaseQuantity == 0
                    ? purchaseDeclarationDTO.Quantity
                    : purchaseDeclarationDTO.PurchaseQuantity;
            }
            e.DetailTableView.DataSource = dataList;
        }

        protected void TbTextChanged(object sender, EventArgs e)
        {
            var txt = (System.Web.UI.WebControls.TextBox)sender;
            if (string.IsNullOrEmpty(txt.Text))
            {
                return;
            }
            var filialeId = ((GridDataItem)txt.Parent.Parent).GetDataKeyValue("FilialeId").ToString();
            var realGoodsId = new Guid(((GridDataItem)txt.Parent.Parent).GetDataKeyValue("RealGoodsId").ToString());
            var temp = StockDeclareDtos;
            var dataInfo = temp.FirstOrDefault(ent => ent.RealGoodsId == realGoodsId && ent.FilialeId == new Guid(filialeId));
            if (dataInfo != null && Convert.ToInt32(txt.Text) != dataInfo.PurchaseQuantity)
            {
                dataInfo.Quantity = dataInfo.PurchaseQuantity = Convert.ToInt32(txt.Text);
            }
            StockDeclareDtos = temp;
        }
    }
}