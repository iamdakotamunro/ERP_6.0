using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using B2C.Model.PromotionModel;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 采购明细类
    /// </summary>
    public partial class PurchasingDetailForm : WindowsPage
    {
        private static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private static readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        private static readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        private static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly PurchasingManager _purchasingManager = new PurchasingManager(_purchasing, _goodsCenterSao, _purchasingDetail, _companyCussent, null);
        private readonly PurchasingDetailManager _purchasingDetailManager = new PurchasingDetailManager(_purchasingDetail, _purchasing);
        private readonly IDebitNote _debitNoteDao = new DebitNote(GlobalConfig.DB.FromType.Write);
        readonly IPurchaseSet _purchaseSetDao = new PurchaseSet(GlobalConfig.DB.FromType.Write);
        private readonly IPurchaseSetLog _purchaseSetLogDao = new PurchaseSetLog(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasePromotion _purchasePromotionDal = new PurchasePromotion(GlobalConfig.DB.FromType.Read);


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["readly"]))
                {
                    Rgd_PurchasingDetail.Enabled = true;
                }

                PurchasingInfo pInfo = PurchasingInfo1;
                if (pInfo == null)
                {
                    RAM.Alert("采购单信息未找到！");
                    return;
                }
                WareHouseId = pInfo.WarehouseID;
                if (pInfo.PurchasingState != (int)PurchasingState.PartComplete)
                {
                    Rgd_PurchasingDetail.Columns[12].Visible = false;
                }
                var personnelInfo = CurrentSession.Personnel.Get();
                //获取授权仓库列表
                var warehouseDto = WMSSao.GetWarehouseAndFilialeAuth(personnelInfo.PersonnelId).ToDictionary(k=>k.WarehouseId,v=>v);
                if (warehouseDto.ContainsKey(pInfo.WarehouseID))
                {
                    var warehouseAuth = warehouseDto[pInfo.WarehouseID];
                    lab_Purchasing.Text = pInfo.CompanyName + " " + pInfo.PurchasingNo;

                    var item = new ListItem
                    {
                        Text = warehouseAuth.WarehouseName,
                        Value = string.Format("{0}", pInfo.WarehouseID),
                        Selected = true
                    };
                    RCB_Warehouse.Items.Add(item);

                    //采购公司
                    var hostingFilialeAuth =
                        warehouseAuth.FilialeAuths.FirstOrDefault(
                            ent => ent.HostingFilialeId == pInfo.PurchasingFilialeId);
                    if (hostingFilialeAuth!=null)
                    {
                        lab_Purchasing.Text += "<br/><span style='line-height: 22px;'>采购公司：" + hostingFilialeAuth.HostingFilialeName + "</span>";
                    }
                    if (pInfo.PurchasingState >= (int)PurchasingState.AllComplete)
                    {
                        Rgd_PurchasingDetail.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// 存储商品列表
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

        /// <summary>
        /// 仓库
        /// </summary>
        private Guid WareHouseId
        {
            get
            {
                if (ViewState["WareHouseId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["WareHouseId"].ToString());
            }
            set { ViewState["WareHouseId"] = value; }
        }

        protected PurchasingInfo PurchasingInfo1
        {
            get
            {
                if (ViewState["PurchasingInfo1"] == null)
                {
                    ViewState["PurchasingInfo1"] = _purchasing.GetPurchasingById(PurchasingID);
                }
                return (PurchasingInfo)ViewState["PurchasingInfo1"];
            }
        }

        protected Dictionary<Guid, GoodsInfo> GoodsInfos
        {
            get
            {
                if (ViewState["GoodsInfos"] == null)
                {
                    return null;
                }
                return (Dictionary<Guid, GoodsInfo>)(ViewState["GoodsInfos"]);
            }
            set { ViewState["GoodsInfos"] = value; }
        }

        protected Dictionary<Guid, int> GoodsQuantityDic
        {
            get
            {
                if (ViewState["GoodsQuantityDic"] == null)
                {
                    return new Dictionary<Guid, int>();
                }
                return (Dictionary<Guid, int>)(ViewState["GoodsQuantityDic"]);
            }
            set { ViewState["GoodsQuantityDic"] = value; }
        }

        /// <summary>
        /// 记录批量修改商品
        /// </summary>
        protected Dictionary<Guid, decimal> SelectedUpdatePrice
        {
            get
            {
                if (ViewState["SelectedUpdatePrice"] == null)
                {
                    return new Dictionary<Guid, decimal>();
                }
                return (Dictionary<Guid, decimal>)(ViewState["SelectedUpdatePrice"]);
            }
            set { ViewState["SelectedUpdatePrice"] = value; }
        }

        public string GetUsableStock(object goodsId)
        {
            if (goodsId != null)
            {
                if (GoodsQuantityDic.ContainsKey(new Guid(goodsId.ToString())))
                {
                    return string.Format("{0}", GoodsQuantityDic[new Guid(goodsId.ToString())]);
                }
            }
            return "0";
        }

        protected static bool ShowFlag;

        public List<GridGoodsInfo> GridGoodsList
        {
            get
            {
                if (ViewState["GridGoodsList"] == null)
                    return new List<GridGoodsInfo>();
                return (List<GridGoodsInfo>)ViewState["GridGoodsList"];
            }
            set { ViewState["GridGoodsList"] = value; }
        }

        /// <summary>
        /// 加载页面数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Rgd_PurchasingDetail_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RgdDataBind();
        }

        public void RgdDataBind()
        {
            PurchasingInfo pInfo = PurchasingInfo1;
            var list = _purchasingDetail.SelectByGoodsDayAvgSales(PurchasingID);
            if(list==null || list.Count == 0)
            {
                Rgd_PurchasingDetail.DataSource = list??new List<PurchasingDetailInfo>();
                return;
            }
            List<Guid> goodsIdOrRealGoodsIdList = list.Select(w => w.GoodsID).Distinct().ToList();
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdOrRealGoodsIdList);
            if (GoodsInfos == null)
            {
                GoodsInfos = dicGoods;
            }
            var purchasePromotionInfos = new Dictionary<Guid, List<PurchasePromotionInfo>>();
            foreach (var pdInfo in list)
            {
                var goodsBaseInfo = new GoodsInfo();
                if (dicGoods != null && dicGoods.Count > 0)
                {
                    bool hasKey = dicGoods.ContainsKey(pdInfo.GoodsID);
                    if (hasKey)
                    {
                        goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == pdInfo.GoodsID).Value;
                    }
                }
                List<PurchasePromotionInfo> currentPromotions;
                if (purchasePromotionInfos.ContainsKey(pdInfo.GoodsID))
                {
                    currentPromotions = purchasePromotionInfos[pdInfo.GoodsID];
                }
                else
                {
                    currentPromotions = _purchasePromotionDal.GetPurchasePromotionListByGoodsId(goodsBaseInfo.GoodsId,pInfo.PurchasingFilialeId ,pInfo.WarehouseID).Where(
                            w => (w.PromotionKind == (int)PromotionKind.Both || w.PromotionKind == (int)PromotionKind.PromotionInfo)).ToList().OrderByDescending(w => w.StartDate).ToList();
                    if (currentPromotions.Count > 0)
                        purchasePromotionInfos.Add(pdInfo.GoodsID, currentPromotions);
                }
                foreach (var purchasePromotionInfo in currentPromotions)
                {
                    if (!string.IsNullOrEmpty(purchasePromotionInfo.PromotionInfo))
                    {
                        pdInfo.GoodsName += " [" + purchasePromotionInfo.PromotionInfo + "]";
                    }
                }
            }
            if (GridGoodsList.Count > 0)
            {
                var myGridGoodsList = GridGoodsList;
                for (int i = 0; i < myGridGoodsList.Count; i++)
                {
                    var gridGoodsInfo = myGridGoodsList[i];
                    var detailInfo = list.FirstOrDefault(w => w.PurchasingGoodsID == gridGoodsInfo.Id);
                    if (detailInfo == null)
                    {
                        myGridGoodsList.RemoveAt(i);
                    }
                    else
                    {
                        gridGoodsInfo.CompanyId = detailInfo.CompanyID;

                        detailInfo.PlanQuantity = gridGoodsInfo.PlanQuantity;
                        detailInfo.Price = gridGoodsInfo.NewPrice;
                    }
                }
                GridGoodsList = myGridGoodsList;
            }
            GoodsQuantityDic = WMSSao.StockSearch(goodsIdOrRealGoodsIdList,null ,pInfo.WarehouseID, pInfo.PurchasingFilialeId);
            Rgd_PurchasingDetail.DataSource = list.OrderBy(act=>act.GoodsName).ThenBy(act=>act.Specification);
        }

        #region 获取供应商集合
        /// <summary>
        ///  获取供应商集合
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> Rcb_CommanyDataSource()
        {
            return _companyCussent.GetCompanyCussentList(CompanyType.Suppliers).Where(ent => ent.State == 1).ToList();
        }

        /// <summary>
        /// 选择设置统一供应商
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Rcb_AllCommanyList_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            IList<CompanyCussentInfo> cbList = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers).Where(c => c.CompanyName.IndexOf(e.Text, StringComparison.Ordinal) > -1).ToList();
            if (cbList.Count == 0)
                cbList = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers);
            foreach (CompanyCussentInfo userBaseInfo in cbList)
            {
                var item = new RadComboBoxItem
                {
                    Text = userBaseInfo.CompanyName,
                    Value = userBaseInfo.CompanyId.ToString()
                };
                combo.Items.Add(item);
            }
        }
        #endregion

        /// <summary> 保存修改的采购单信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ib_SaveGoods_Click(object sender, EventArgs e)
        {
            var btsave = (ImageButton)sender;
            var item = (GridCommandItem)btsave.TemplateControl.Parent.Parent;
            var purchasingId = new Guid(Request["PurchasingID"]);
            if (GridGoodsList.Any(ent => ent.PurchasingGoodsType != (int) PurchasingGoodsType.Gift && ent.NewPrice <= 0))
            {
                RAM.Alert("操作提示,非赠品类型的商品采购价必须大于0!");
                return;
            }
            _purchasing.PurchasingWarehouseId(purchasingId, new Guid(RCB_Warehouse.SelectedItem.Value));
            var dics = new Dictionary<Guid, decimal>(); //记录修改的商品及价格
            var goodsIds = new List<Guid>();
            foreach (var goodsInfo in GoodsInfos.Values)
            {
                if (goodsIds.Contains(goodsInfo.GoodsId)) continue;
                goodsIds.Add(goodsInfo.GoodsId);
            }
            if (goodsIds.Count == 0)
            {
                RAM.Alert("操作提示,GMS商品列表获取失败!");
                return;
            }
            if (_purchaseSetLogDao.IsExistNoAuditSetLog(goodsIds, (int)PurchaseSetLogStatue.NotAudit, -1, (int)PurchaseSetLogType.PurchasePrice))
            {
                RAM.Alert("操作提示,待修改价格商品已存在未审核调价记录!");
                return;
            }
            string errorGoodsNames;
            if (!IsMatch(dics, out errorGoodsNames))
            {
                RAM.Alert("操作提示,同一主商品下出现2个及以上的价格! /n" + errorGoodsNames.Trim(','));
                return;
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            PurchasingInfo pInfo = _purchasing.GetPurchasingById(PurchasingID);
            var arrivalTime = (RadDateTimePicker)item.FindControl("RDP_ArrivalTime");
            if ((pInfo.ArrivalTime == DateTime.MinValue || pInfo.ArrivalTime == DateTime.Parse("1753/01/01 00:00:00")) || !string.IsNullOrEmpty(arrivalTime.DateInput.Text))
            {
                if (string.IsNullOrEmpty(arrivalTime.DateInput.Text) || arrivalTime.SelectedDate == null)
                {
                    RAM.Alert("请选择到货日期！");
                    return;
                }
                _purchasing.PurchasingArrivalTime(PurchasingID, arrivalTime.SelectedDate.Value, personnelInfo.RealName);
            }
            bool isExist = false;   //判断是否存在调高价
            bool hasChange = false;  //判断是否存在改价
            var purchaseSetList = _purchaseSetDao.GetPurchaseSetInfoList(goodsIds, pInfo.WarehouseID,pInfo.PurchasingFilialeId) ?? new List<PurchaseSetInfo>();
            var updatePriceList = new List<UpdatePriceInfo>();
            //采购调价记录日志
            IList<PurchaseSetLogInfo> logInfos = new List<PurchaseSetLogInfo>();
            foreach (var gridGoodsInfo in GridGoodsList)
            {
                var purchasetInfo = purchaseSetList.FirstOrDefault(act => act.GoodsId == GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId);
                bool isDefaultCompanyId = purchasetInfo != null && purchasetInfo.CompanyId == pInfo.CompanyID;
                if (gridGoodsInfo.PurchasingGoodsType == (int)PurchasingGoodsType.Gift)
                {
                    continue;
                }
                var dInfo = new PurchasingDetailInfo
                {
                    PurchasingID = purchasingId,
                    GoodsID = gridGoodsInfo.RealGoodsId,
                    Price = gridGoodsInfo.NewPrice,
                    PlanQuantity = Convert.ToDouble(gridGoodsInfo.PlanQuantity),
                    CPrice = gridGoodsInfo.OldPrice
                };
                if (pInfo.PurchasingState == (int)PurchasingState.Refusing)
                {
                    gridGoodsInfo.OldPrice = purchasetInfo.PurchasePrice;
                }
                bool isUpdate = SelectedUpdatePrice.ContainsKey(GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId);
                if (isDefaultCompanyId && (gridGoodsInfo.OldPrice != gridGoodsInfo.NewPrice || isUpdate))
                {
                    hasChange = true;
                    if (logInfos.All(act => act.GoodsId != GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId))
                    {
                        decimal changeValue = isUpdate
                                                  ? SelectedUpdatePrice[GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId] - gridGoodsInfo.CPrice
                                                  : gridGoodsInfo.NewPrice - gridGoodsInfo.OldPrice;
                        if (changeValue > 0 && !isExist) isExist = true;
                        var purchaseSetInfo = new PurchaseSetLogInfo
                        {
                            LogId = Guid.NewGuid(),
                            GoodsId = GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId,
                            GoodsName = GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsName,
                            WarehouseId = PurchasingInfo1.WarehouseID,
                            Applicant = CurrentSession.Personnel.Get().PersonnelId,
                            ChangeDate = DateTime.Now,
                            ChangeReason = "采购时调价",
                            ChangeValue = changeValue,
                            LogType = (int)PurchaseSetLogType.PurchasePrice,
                            Statue = changeValue < 0
                                         ? (int)PurchaseSetLogStatue.Pass
                                         : (int)PurchaseSetLogStatue.NotAudit,
                            NewValue = isUpdate ? SelectedUpdatePrice[GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId] : gridGoodsInfo.NewPrice,
                            OldValue = isUpdate ? gridGoodsInfo.CPrice : gridGoodsInfo.OldPrice,
                            HostingFilialeId = pInfo.PurchasingFilialeId
                        };
                        logInfos.Add(purchaseSetInfo);
                    }
                }
                if (isDefaultCompanyId)
                {
                    if (dics.ContainsKey(GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId) && (gridGoodsInfo.OldPrice != 0 || isUpdate))
                    {
                        dInfo.Price = isUpdate ? SelectedUpdatePrice[GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId] : dics[GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId];
                    }

                    if (pInfo.PurchasingState == (int)PurchasingState.Refusing)
                    {
                        if (purchasetInfo != null && dInfo.Price > purchasetInfo.PurchasePrice)
                            isExist = true;
                    }
                }
                var newPrice = gridGoodsInfo.NewPrice;
                if (gridGoodsInfo.OldPrice != 0 && newPrice != gridGoodsInfo.CPrice)
                {
                    if (updatePriceList.Count(w => w.GoodsId == GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId) == 0)
                        updatePriceList.Add(new UpdatePriceInfo
                                            {
                                                GoodsId = GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId,
                                                NewPrice = newPrice,
                                                CPrice = gridGoodsInfo.OldPrice
                                            });
                }
            }
            if (updatePriceList.Count > 0)
            {
                foreach (var updatePriceInfo in updatePriceList)
                {
                    var realGoodsIds = _goodsCenterSao.GetRealGoodsIdsByGoodsId(updatePriceInfo.GoodsId);
                    _purchasingDetail.UpdateDetailsPrice(pInfo.PurchasingID, realGoodsIds.ToList(), updatePriceInfo.NewPrice, updatePriceInfo.CPrice);
                }
            }
            if (isExist)  //存在调高的价位
            {
                _purchasingManager.PurchasingUpdate(purchasingId, PurchasingState.WaitingAudit);
            }
            else
            {
                if (pInfo.PurchasingState == (int)PurchasingState.Refusing)
                {
                    _purchasingManager.PurchasingUpdate(purchasingId, PurchasingState.NoSubmit);
                }
            }
            foreach (var purchaseSetLogInfo in logInfos)   //添加采购设置调价记录
            {
                _purchaseSetLogDao.AddPurchaseSetLog(purchaseSetLogInfo);
                if (purchaseSetLogInfo.ChangeValue < 0) //调低，修改绑定价格
                {
                    var setInfo = _purchaseSetDao.GetPurchaseSetInfo(purchaseSetLogInfo.GoodsId, pInfo.PurchasingFilialeId, pInfo.WarehouseID);
                    if (setInfo != null)
                    {
                        setInfo.PurchasePrice = purchaseSetLogInfo.NewValue;
                        string errorMessage;
                        _purchaseSetDao.UpdatePurchaseSet(setInfo, out errorMessage);
                    }
                }
                //修改采购单商品单价
                var realGoodsIds = _goodsCenterSao.GetRealGoodsIdsByGoodsId(purchaseSetLogInfo.GoodsId);
                _purchasingDetail.UpdatePurchasingDetailPrice(realGoodsIds, pInfo.CompanyID, purchaseSetLogInfo.NewValue, pInfo.WarehouseID,pInfo.PurchasingFilialeId);
            }

            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pInfo.PurchasingID, pInfo.PurchasingNo, OperationPoint.PurchasingManager.Edit.GetBusinessInfo(), string.Empty);
            if (hasChange)
            {
                RAM.Alert("商品修改价格时,有SKU的商品会全部更新！");//将更改主商品所有其他SKU
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        decimal _total;
        int _planTotal;
        protected void Rgd_PurchasingDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var dataItem = e.Item as GridDataItem;
                int planQuantity = Convert.ToInt32(dataItem.GetDataKeyValue("PlanQuantity").ToString());
                _planTotal += planQuantity;
                decimal price = Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString()) == -1 ? 0 : Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString());
                _total += planQuantity * price;
                var isException = (bool)DataBinder.Eval(e.Item.DataItem, "IsException");
                var purchasingToDate = DataBinder.Eval(e.Item.DataItem, "PurchasingToDate");
                if (isException)
                {
                    if (string.IsNullOrEmpty(purchasingToDate.ToString()) || DateTime.Parse(purchasingToDate.ToString()) < DateTime.Now)
                        e.Item.Style.Add("background-color", "#FF6666"); //红色
                }
            }
            if (e.Item is GridFooterItem)
            {
                var footerItem = e.Item as GridFooterItem;
                footerItem["PlanQuantity"].Text = string.Format("合计数量：{0}", _planTotal);
                footerItem["PriceResult"].Text = string.Format("合计金额： {0}", _total.ToString("0.##"));
            }
        }

        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void Rgd_PurchasingDetail_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.Item != null)
            {
                #region  更改供应商
                if ("UpdateCompany" == e.CommandName)
                {
                    GridItem dataItem = e.Item;
                    if (Rgd_PurchasingDetail.SelectedItems.Count == 0)
                    {
                        RAM.Alert("没有选择要更改的商品!");
                        return;
                    }
                    var purchasingId = new Guid(Request["PurchasingID"]);
                    PurchasingInfo pinfo = _purchasing.GetPurchasingById(purchasingId);
                    var arrivalTime = dataItem.FindControl("RDP_ArrivalTime") as RadDateTimePicker;
                    if (arrivalTime != null && ((pinfo.ArrivalTime == DateTime.MinValue || pinfo.ArrivalTime == DateTime.Parse("1753/01/01 00:00:00")) || !string.IsNullOrEmpty(arrivalTime.DateInput.Text)))
                    {
                        if (string.IsNullOrEmpty(arrivalTime.DateInput.Text))
                        {
                            RAM.Alert("请选择到货日期！");
                            return;
                        }
                        if (arrivalTime.SelectedDate != null)
                            _purchasing.PurchasingArrivalTime(PurchasingID, arrivalTime.SelectedDate.Value, CurrentSession.Personnel.Get().RealName);
                    }

                    if ((int)PurchasingState.AllComplete == pinfo.PurchasingState || (int)PurchasingState.PartComplete == pinfo.PurchasingState || (int)PurchasingState.StockIn == pinfo.PurchasingState)
                    {
                        RAM.Alert("采购完成,部分完成,入库中的采购单商品不允许更改供应商!");
                        return;
                    }
                    var rbox = (dataItem.FindControl("RCB_AllCommanyList") as RadComboBox);
                    if (rbox == null || string.IsNullOrEmpty(rbox.SelectedValue))
                    {
                        RAM.Alert("请选择要更改的供应商!");
                        return;
                    }
                    var companyId = new Guid(rbox.SelectedValue);
                    string companyName = rbox.SelectedItem.Text;
                    foreach (GridDataItem item in Rgd_PurchasingDetail.SelectedItems)
                    {
                        var purchasingGoodsId = new Guid(item.GetDataKeyValue("PurchasingGoodsID").ToString());
                        _purchasingDetailManager.UpdatePurchsingCompany(purchasingId, purchasingGoodsId, companyId, companyName, CurrentSession.Personnel.Get());
                    }
                    Rgd_PurchasingDetail.Rebind();
                    if (Rgd_PurchasingDetail.SelectedItems.Count == Rgd_PurchasingDetail.Items.Count)
                    {
                        _purchasingManager.Delete(purchasingId);
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");

                    }
                }
                #endregion

                #region  入库中
                else if ("Stocking".Equals(e.CommandName))
                {
                    GridItem dataItem = e.Item;
                    if (Rgd_PurchasingDetail.SelectedItems.Count == 0)
                    {
                        RAM.Alert("没有选择要修改的商品!");
                        return;
                    }
                    var tbxPrice = (TextBox)dataItem.FindControl("tbx_Price");
                    var tbxStockingDay = (TextBox)dataItem.FindControl("tbx_stockingDay");
                    if (tbxStockingDay != null && (tbxPrice != null && (string.IsNullOrEmpty(tbxPrice.Text) && string.IsNullOrEmpty(tbxStockingDay.Text))))
                    {
                        RAM.Alert("商品和备货天数至少一个不能为空!");
                        return;
                    }

                    var purchasingId = new Guid(Request["PurchasingID"]);
                    PurchasingInfo pinfo = _purchasing.GetPurchasingById(purchasingId);
                    bool changeCompany = pinfo.Description.Contains("更改供应商");
                    if ((int)PurchasingState.AllComplete == pinfo.PurchasingState || (int)PurchasingState.PartComplete == pinfo.PurchasingState || (int)PurchasingState.StockIn == pinfo.PurchasingState)
                    {
                        RAM.Alert("采购完成,部分完成,入库中的采购单商品不允许修改!");
                        return;
                    }
                    var selectedList = new Dictionary<Guid, decimal>();
                    foreach (GridDataItem item in Rgd_PurchasingDetail.SelectedItems)
                    {
                        var goodsId = new Guid(item.GetDataKeyValue("GoodsID").ToString());
                        decimal price = Convert.ToDecimal(item.GetDataKeyValue("Price").ToString());
                        var purchasingGoodsId = new Guid(item.GetDataKeyValue("PurchasingGoodsID").ToString());
                        double dayAvgStocking = Convert.ToDouble(item.GetDataKeyValue("DayAvgStocking").ToString());
                        int purchasingGoodsType = Convert.ToInt32(item.GetDataKeyValue("PurchasingGoodsType").ToString());
                        if (purchasingGoodsType == (int)PurchasingGoodsType.Gift)
                        {
                            continue;
                        }
                        var dinfo = new PurchasingDetailInfo
                        {
                            PurchasingGoodsID = purchasingGoodsId,
                            GoodsID = goodsId,
                            PurchasingID = pinfo.PurchasingID,
                            CPrice = price
                        };
                        if (tbxPrice == null || string.IsNullOrEmpty(tbxPrice.Text))
                        {
                            dinfo.Price = price;
                        }
                        else
                        {
                            if (Convert.ToDecimal(tbxPrice.Text) != dinfo.Price && !changeCompany)
                            {
                                if (!selectedList.ContainsKey(GoodsInfos[goodsId].GoodsId))
                                {
                                    selectedList.Add(GoodsInfos[goodsId].GoodsId, Convert.ToDecimal(tbxPrice.Text));
                                }
                            }
                            dinfo.Price = Convert.ToDecimal(tbxPrice.Text);
                        }
                        if (tbxStockingDay != null && string.IsNullOrEmpty(tbxStockingDay.Text))
                        {
                            dinfo.PlanQuantity = Convert.ToDouble(item.GetDataKeyValue("PlanQuantity").ToString());
                        }
                        else
                        {
                            double planStocking = Convert.ToDouble(item.GetDataKeyValue("PlanQuantity").ToString());
                            dinfo.PlanQuantity = Math.Round(planStocking + dayAvgStocking * (tbxStockingDay == null ? 0 : string.IsNullOrEmpty(tbxStockingDay.Text) ? 0 : Convert.ToDouble(tbxStockingDay.Text)), 0);
                        }
                        if (dinfo.Price < 0)
                        {
                            RAM.Alert("您更改的商品中,存在未定价的商品!请确定后再保存!");
                        }
                    }
                    SelectedUpdatePrice = selectedList;
                    RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                #endregion

                #region 分配
                else if ("Distribution" == e.CommandName)
                {
                    //验证是否可分配
                    if (Rgd_PurchasingDetail.SelectedItems.Count == 0)
                    {
                        RAM.Alert("请选择你要分配的主商品数量!");
                        return;
                    }

                    Dictionary<Guid, double> realGoodsDics = new Dictionary<Guid, double>(); //商品分配比例
                    List<PurchasingDetailInfo> originalDetails = _purchasingDetail.Select(PurchasingID);
                    List<Guid> unSetPrices = new List<Guid>();
                    foreach (GridDataItem dataItem in Rgd_PurchasingDetail.SelectedItems)
                    {
                        var realGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                        var planQuantity= Convert.ToDouble(dataItem.GetDataKeyValue("PlanQuantity").ToString());
                        if (realGoodsDics.ContainsKey(realGoodsId))
                            realGoodsDics[realGoodsId] = realGoodsDics[realGoodsId] + planQuantity;
                        else
                        {
                            realGoodsDics.Add(realGoodsId, planQuantity);
                        }
                            
                        decimal oldprice = Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString());
                        var purchasingGoodsId = new Guid(dataItem.GetDataKeyValue("PurchasingGoodsID").ToString());
                        string strprice = ((TextBox)dataItem.FindControl("tbx_Price")).Text;
                        //double planQuantity = Convert.ToDouble(((TextBox)dataItem.FindControl("tbx_PlanQuantity")).Text);
                        var price = Convert.ToDecimal(strprice) <= 0 ? oldprice : Convert.ToDecimal(strprice);
                        if (!unSetPrices.Contains(purchasingGoodsId) && price <= 0)
                            unSetPrices.Add(purchasingGoodsId);
                        
                        var currentDetail = originalDetails.FirstOrDefault(ent=>ent.PurchasingGoodsID== purchasingGoodsId);
                        currentDetail.SetPrice(price, oldprice);
                    }

                    Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsDics.Keys.ToList());
                    var goodsIds = dicGoods.Values.Select(ent => ent.GoodsId).Distinct();
                    if (goodsIds.Count() > 1)
                    {
                        RAM.Alert("你要分配的商品的子属性商品不在同一个主商品下面,请检查你选中的子商品!");
                        return;
                    }
                    
                    Guid goodsId = goodsIds.First();
                    double planquantity = Convert.ToDouble(((TextBox)e.Item.FindControl("tbx_GoodsNum")).Text);
                    if (planquantity < realGoodsDics.Count)
                    {
                        RAM.Alert("商品有效分配数不能小于商品(子)商品种数!");
                        return;
                    }
                    
                    var purchasePromotions = _purchasePromotionDal.GetPurchasePromotionListByGoodsId(goodsId, PurchasingInfo1.PurchasingFilialeId, PurchasingInfo1.WarehouseID);
                    if (purchasePromotions.Count > 0)
                    {
                        purchasePromotions = purchasePromotions.Where(w => w.GivingCount > 0 && w.StartDate <= DateTime.Now && w.EndDate >= DateTime.Now).ToList();
                    }

                    var deleteDetails = originalDetails.Where(ent => realGoodsDics.ContainsKey(ent.GoodsID)).ToList();
                    DebitNoteInfo debitNote;
                    List<DebitNoteDetailInfo> debitNoteDetailInfos;
                    List<PurchasingDetailInfo> newPurchasingDetailInfos;
                    Distribution(PurchasingInfo1, deleteDetails, realGoodsDics, goodsId, planquantity, purchasePromotions, out debitNote, out debitNoteDetailInfos, out newPurchasingDetailInfos);
                    using (var ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        foreach (var updateDetail in deleteDetails)
                        {
                            _purchasingDetail.DeleteByGoodsId(PurchasingID, updateDetail.GoodsID, updateDetail.PurchasingGoodsID);
                        }
                        foreach (var newDetail in newPurchasingDetailInfos)
                        {
                            _purchasingDetail.Insert(newDetail);
                        }

                        if (debitNoteDetailInfos.Count > 0)
                        {
                            _debitNoteDao.DeleteDebitNote(PurchasingID);
                            _debitNoteDao.AddPurchaseSetAndDetail(debitNote, debitNoteDetailInfos);
                        }
                        ts.Complete();
                    }

                    if (originalDetails.Any(ent => unSetPrices.Contains(ent.PurchasingGoodsID)))
                    {
                        RAM.Alert("您更改的商品中,存在未定价的商品!请确定后再保存!");
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                #endregion

                #region 更改状态
                else if ("UpdateState" == e.CommandName)
                {
                    var dataItem = e.Item as GridDataItem;
                    if (dataItem != null)
                    {
                        DebitNoteInfo debitNoteInfo = _debitNoteDao.GetDebitNoteInfoByNewPurchasingId(PurchasingID);

                        var purchasingGoodsId = new Guid(dataItem.GetDataKeyValue("PurchasingGoodsID").ToString());
                        _purchasingDetail.UpdateGoodState(YesOrNo.Yes, purchasingGoodsId);
                        //借记单
                        var purchasingGoodsType = int.Parse(dataItem.GetDataKeyValue("PurchasingGoodsType").ToString());
                        if (debitNoteInfo != null && purchasingGoodsType == (int)PurchasingGoodsType.Gift)
                        {
                            var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                            var tbPlanQuantity = (TextBox)dataItem.FindControl("tbx_PlanQuantity");
                            int arrivalCount = Convert.ToInt32(tbPlanQuantity.Text);
                            _debitNoteDao.UpdateDebitNoteDetail(debitNoteInfo.PurchasingId, goodsId, (int)YesOrNo.Yes, arrivalCount, true);
                            if (debitNoteInfo.State != (int)DebitNoteState.PartComplete)
                            {
                                //借记单部分完成
                                _debitNoteDao.UpdateDebitNoteStateByNewPurchasingId(debitNoteInfo.PurchasingId, (int)DebitNoteState.PartComplete);
                            }
                        }
                        IList<PurchasingDetailInfo> pdList = _purchasingDetail.Select(PurchasingID);
                        if (pdList.Count(p => p.State == (int)YesOrNo.No) == 0)
                        {
                            _purchasingManager.PurchasingUpdate(PurchasingID, PurchasingState.AllComplete);
                            if (debitNoteInfo != null)
                            {
                                //借记单全部完成
                                _debitNoteDao.UpdateDebitNoteStateByNewPurchasingId(debitNoteInfo.PurchasingId, (int)DebitNoteState.AllComplete);
                            }
                        }
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                #endregion

                else if ("ExpandCollapse".Equals(e.CommandName))
                {
                    var dataItem = e.Item as GridGroupHeaderItem;
                    if (dataItem != null)
                    {
                        foreach (GridItem item in dataItem.GetChildItems())
                        {
                            item.Selected = !item.Selected;
                        }
                        dataItem.Expanded = false;
                    }
                }
            }
        }

        protected void Rgd_PurchasingDetail_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            ReloadGridGoodsList();
            RgdDataBind();
        }

        /// <summary> 更新数据
        /// </summary>
        private void ReloadGridGoodsList()
        {
            var myGridGoodsList = (List<GridGoodsInfo>)GridGoodsList.DeepCopy();
            foreach (GridDataItem dataItem in Rgd_PurchasingDetail.Items)
            {
                var id = new Guid(dataItem.GetDataKeyValue("PurchasingGoodsID").ToString());
                var realGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                var companyId = new Guid(dataItem.GetDataKeyValue("CompanyID").ToString());
                var purchasingGoodsType = int.Parse(dataItem.GetDataKeyValue("PurchasingGoodsType").ToString());
                var oldPrice = decimal.Parse(dataItem.GetDataKeyValue("Price").ToString());
                var cPrice = decimal.Parse(dataItem.GetDataKeyValue("CPrice").ToString());
                string strNewPrice = ((TextBox)dataItem.FindControl("tbx_Price")).Text;
                string strPlanQuantity = ((TextBox)dataItem.FindControl("tbx_PlanQuantity")).Text;
                decimal newPirce = string.IsNullOrEmpty(strNewPrice) ? 0 : decimal.Parse(strNewPrice);
                if (myGridGoodsList.Count(w => w.Id == id) == 0)
                {
                    myGridGoodsList.Add(new GridGoodsInfo
                    {
                        Id = id,
                        CompanyId = companyId,
                        RealGoodsId = realGoodsId,
                        OldPrice = oldPrice,
                        NewPrice = newPirce,
                        CPrice = cPrice,
                        PlanQuantity = int.Parse(strPlanQuantity),
                        PurchasingGoodsType = purchasingGoodsType,
                        IsSelected = dataItem.Selected
                    });
                }
                else
                {
                    var info = myGridGoodsList.First(w => w.Id == id);
                    info.CompanyId = companyId;
                    info.NewPrice = newPirce;
                    info.IsSelected = dataItem.Selected;
                }
            }
            GridGoodsList = myGridGoodsList;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ib_DeleteAll_Click(object sender, EventArgs e)
        {
            if (Rgd_PurchasingDetail.SelectedItems.Count == 0)
            {
                RAM.Alert("没有选择要删除的商品!");
                return;
            }
            var purchasingId = new Guid(Request["PurchasingID"]);
            PurchasingInfo pinfo = _purchasing.GetPurchasingById(purchasingId);
            if ((int)PurchasingState.AllComplete == pinfo.PurchasingState || (int)PurchasingState.PartComplete == pinfo.PurchasingState || (int)PurchasingState.StockIn == pinfo.PurchasingState)
            {
                RAM.Alert("采购完成,部分完成,入库中的采购单商品不允许删除!");
                return;
            }
            foreach (GridDataItem dataItem in Rgd_PurchasingDetail.SelectedItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                var purchasingGoodsId = new Guid(dataItem.GetDataKeyValue("PurchasingGoodsID").ToString());
                _purchasingDetail.DeleteByGoodsId(purchasingId, goodsId, purchasingGoodsId);
            }
            if (Rgd_PurchasingDetail.SelectedItems.Count == Rgd_PurchasingDetail.Items.Count)
            {
                _purchasingManager.Delete(purchasingId);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        /// <summary>
        /// 未定价商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ib_GoodsPrice_Click(object sender, EventArgs e)
        {
            if (Rgd_PurchasingDetail.SelectedItems.Count == 0)
            {
                RAM.Alert("没有选择要未定价的商品!");
                return;
            }
            var purchasingId = new Guid(Request["PurchasingID"]);
            PurchasingInfo sinfo = _purchasing.GetPurchasingById(purchasingId);
            //过滤采购完成,部分完成,入库中这种状态
            if (sinfo.PurchasingState == (int)PurchasingState.PartComplete || sinfo.PurchasingState == (int)PurchasingState.AllComplete || sinfo.PurchasingState == (int)PurchasingState.StockIn)
            {
                RAM.Alert("采购完成,部分完成,入库中的采购单商品不允许未定价");
                return;
            }
            bool flag = false;
            string strMessage = "";
            foreach (GridDataItem dataItem in Rgd_PurchasingDetail.SelectedItems)
            {
                var purchasingGoodsId = new Guid(dataItem.GetDataKeyValue("PurchasingGoodsID").ToString());
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                double planQuantity = Convert.ToDouble(dataItem.GetDataKeyValue("PlanQuantity"));
                decimal price = Convert.ToDecimal(dataItem.GetDataKeyValue("Price"));

                var pinfo = new PurchasingDetailInfo
                {
                    PurchasingID = purchasingId,
                    GoodsID = goodsId,
                    Price = price,
                    PlanQuantity = planQuantity
                };


                if (price > 0)
                {
                    _purchasingDetail.Update(pinfo, purchasingGoodsId);
                }
                else
                {
                    strMessage += pinfo.GoodsName + " " + pinfo.Specification + " ";
                    flag = true;
                }
                if (flag)
                {
                    RAM.Alert("商品为 " + strMessage + " 为赠品或者未定价的商品,更改不成功!其余已更改.");
                }
            }
            Rgd_PurchasingDetail.Rebind();
        }

        protected bool Enable()
        {
            if (!string.IsNullOrEmpty(Request["readly"]))
            {
                return false;
            }
            if (PurchasingInfo1 == null)
                return false;
            if ((int)PurchasingState.NoSubmit == PurchasingInfo1.PurchasingState || (int)PurchasingState.Purchasing == PurchasingInfo1.PurchasingState || (int)PurchasingState.Refusing == PurchasingInfo1.PurchasingState)
                return true;
            return false;
        }

        protected bool AllowEidtPrice()
        {
            if (!string.IsNullOrEmpty(Request["readly"]))
            {
                return false;
            }
            if (PurchasingInfo1 == null)
                return false;
            if ((int)PurchasingState.NoSubmit == PurchasingInfo1.PurchasingState || (int)PurchasingState.Refusing == PurchasingInfo1.PurchasingState)
                return true;
            return false;
        }

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(Rgd_PurchasingDetail, e);

        }

        protected string GetDescription(object description)
        {
            string html = string.Empty;
            string info = description == null ? string.Empty : description.ToString();
            if (!string.IsNullOrEmpty(info))
            {
                string divId = DivDescriptionId;
                html = "<img id='imageRemark' src='App_Themes/Default/images/Memo.gif' alt='' onmousemove='ShowImg(\"" + divId + "\");' onmouseout='HiddleImg(\"" + divId + "\");' />";
                html += "<div style='position: absolute;'>";
                html += "<div id='" + divId + "' style='z-index: 1000; left: -200px; top: 20px; position: relative;display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px;font-weight: bold; height: auto; overflow: visible; word-break: break-all;' runat='server'>";
                html += info + "</div></div>";
            }
            return html;
        }

        private string DivDescriptionId
        {
            get
            {
                if (ViewState["DescriptionId"] == null)
                    ViewState["DescriptionId"] = "0";
                int id = Convert.ToInt32(ViewState["DescriptionId"].ToString()) + 1;
                ViewState["DescriptionId"] = id;
                return "DivDescriptionId" + id;
            }
        }

        /// <summary>
        /// 是否显示修改前的价格
        /// </summary>
        /// <param name="eval"></param>
        /// <returns></returns>
        protected bool IsVisibal(object eval)
        {
            return false;
        }

        /// <summary>
        /// 判断待保存的采购单是否符合要求
        /// 主商品一样，子商品价格有2个或以上的价格为不合格
        /// </summary>
        /// <returns></returns>
        protected bool IsMatch(Dictionary<Guid, decimal> dictionary, out string errorGoodsNames)
        {
            errorGoodsNames = string.Empty;
            ReloadGridGoodsList();
            var dics = new Dictionary<Guid, List<decimal>>();
            foreach (var gridGoodsInfo in GridGoodsList)
            {
                var id = GoodsInfos[gridGoodsInfo.RealGoodsId].GoodsId;
                if (GoodsInfos.ContainsKey(gridGoodsInfo.RealGoodsId) && gridGoodsInfo.OldPrice > 0 && gridGoodsInfo.NewPrice > 0)
                {
                    if (dics.ContainsKey(id))
                    {
                        if (!dics[id].Contains(gridGoodsInfo.OldPrice)) dics[id].Add(gridGoodsInfo.NewPrice);
                        if (!dics[id].Contains(gridGoodsInfo.NewPrice)) dics[id].Add(gridGoodsInfo.NewPrice);
                    }
                    else
                    {
                        dics.Add(id, new List<decimal> { gridGoodsInfo.OldPrice });
                        if (gridGoodsInfo.OldPrice != gridGoodsInfo.NewPrice) dics[id].Add(gridGoodsInfo.NewPrice);
                    }
                    if (!dictionary.ContainsKey(id) && gridGoodsInfo.OldPrice != gridGoodsInfo.NewPrice)
                    {
                        dictionary.Add(id, gridGoodsInfo.NewPrice);
                    }
                }
                if (SelectedUpdatePrice != null && SelectedUpdatePrice.ContainsKey(id))
                {
                    if (dics.ContainsKey(id))
                    {
                        if (!dics[id].Contains(SelectedUpdatePrice[id]))
                            dics[id].Add(Convert.ToDecimal(SelectedUpdatePrice[id]));
                    }
                    else
                    {
                        dics.Add(id, new List<decimal> { SelectedUpdatePrice[id] });
                    }
                }
            }
            var goodsIds = dics.Where(w => w.Value.Count > 2).Select(w => w.Key).ToList();
            if (goodsIds.Count > 0)
            {
                var goodsInfoList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds);
                errorGoodsNames = goodsInfoList.Aggregate(errorGoodsNames, (current, goodsInfo) => current + (goodsInfo.GoodsName + ","));
            }
            return dics.Values.All(act => act.Count <= 2);
        }

        public void Distribution(PurchasingInfo pInfo, IList<PurchasingDetailInfo> deleteDetails, Dictionary<Guid,double> dics,Guid goodsId,double distributionQuantity,
            IList<PurchasePromotionInfo> promotionInfos,out DebitNoteInfo debitNote, out List<DebitNoteDetailInfo> debitNoteDetailList,out List<PurchasingDetailInfo> newDetailInfos)
        {
            newDetailInfos = new List<PurchasingDetailInfo>();
            debitNoteDetailList = new List<DebitNoteDetailInfo>();
            debitNote = new DebitNoteInfo();
            Dictionary<Guid,double> finalDics=new Dictionary<Guid, double>();
            bool hasPromotion = false;
            if(promotionInfos!=null && promotionInfos.Any()){
                #region [现返]
                PurchasePromotionInfo usingPromotion = promotionInfos.FirstOrDefault(w => w.PromotionType == (int)PurchasePromotionType.Back);

                if (usingPromotion != null)
                {
                    hasPromotion = true;
                    var joinQuantity = usingPromotion.BuyCount + usingPromotion.GivingCount;

                    #region  按商品总数量进行赠送
                    if (!usingPromotion.IsSingle)
                    {
                        var extra = (int)distributionQuantity % joinQuantity;
                        var sentCount = (int)(distributionQuantity / joinQuantity);
                        if (extra > 0 && extra >= (usingPromotion.BuyCount/float.Parse("2.0"))) //补足
                        {
                            sentCount++;
                            finalDics = DistributionByGoodsId(distributionQuantity + joinQuantity - extra, dics);
                        }
                        else
                        {
                            finalDics = DistributionByGoodsId(distributionQuantity, dics);
                        }

                        foreach (var goodsGroup in deleteDetails.GroupBy(ent=>ent.GoodsID).OrderByDescending(ent=>ent.Sum(act=>act.PlanQuantity)))
                        {
                            if(!finalDics.ContainsKey(goodsGroup.Key))continue;
                            var detail = goodsGroup.First();
                            if (sentCount>0)
                            {
                                var sendDetail = goodsGroup.FirstOrDefault(ent => ent.PurchasingGoodsType == (Byte) PurchasingGoodsType.Gift);
                                if (finalDics[goodsGroup.Key] > sentCount)
                                {
                                    newDetailInfos.Add(new PurchasingDetailInfo(pInfo.PurchasingID, detail.GoodsID, detail.GoodsName, detail.Units, detail.GoodsCode,
                                        detail.Specification, detail.CompanyID, goodsGroup.Max(act => act.Price), finalDics[goodsGroup.Key] - sentCount, goodsGroup.Sum(act => act.RealityQuantity),
                                        detail.State, detail.Description, detail.PurchasingGoodsID, (int)PurchasingGoodsType.NoGift)
                                    { CPrice = goodsGroup.Max(act => act.CPrice) });

                                    newDetailInfos.Add(new PurchasingDetailInfo(pInfo.PurchasingID, detail.GoodsID, detail.GoodsName, detail.Units, detail.GoodsCode,
                                        detail.Specification, detail.CompanyID, 0, sentCount, 0,
                                        detail.State, detail.Description, sendDetail!=null? sendDetail.PurchasingGoodsID : Guid.NewGuid(), (int)PurchasingGoodsType.Gift));
                                    sentCount = 0;
                                }
                                else
                                {
                                    newDetailInfos.Add(new PurchasingDetailInfo(pInfo.PurchasingID, detail.GoodsID, detail.GoodsName, detail.Units, detail.GoodsCode,
                                        detail.Specification, detail.CompanyID, 0, finalDics[goodsGroup.Key], 0,
                                        detail.State, detail.Description, sendDetail != null ? sendDetail.PurchasingGoodsID : Guid.NewGuid(), (int)PurchasingGoodsType.Gift));
                                    sentCount = sentCount - (int)finalDics[goodsGroup.Key];
                                }
                            }
                            else
                            {
                                newDetailInfos.Add(new PurchasingDetailInfo(pInfo.PurchasingID, detail.GoodsID, detail.GoodsName, detail.Units, detail.GoodsCode,
                                        detail.Specification, detail.CompanyID, goodsGroup.Max(act => act.Price), finalDics[detail.GoodsID], goodsGroup.Sum(act => act.RealityQuantity),
                                        detail.State, detail.Description, detail.PurchasingGoodsID, (int)PurchasingGoodsType.NoGift)
                                { CPrice = goodsGroup.Max(act => act.CPrice) });
                            }
                        }
                    }
                    #endregion

                    #region 按单光度
                    else
                    {
                        finalDics = DistributionByGoodsId(distributionQuantity, dics);
                        foreach (var detailGroup in deleteDetails.GroupBy(ent => ent.GoodsID).OrderByDescending(ent => ent.Sum(act => act.PlanQuantity)))
                        {
                            if (!finalDics.ContainsKey(detailGroup.Key)) continue;
                            var detail = detailGroup.First();
                            var actquantity = (int)finalDics[detailGroup.Key] % joinQuantity;
                            var sentCount= (int)finalDics[detailGroup.Key] / joinQuantity;
                            if (actquantity > 0)
                            {
                                if (actquantity >= (usingPromotion.BuyCount / float.Parse("2.0")))
                                {
                                    finalDics[detailGroup.Key] = finalDics[detailGroup.Key] + joinQuantity - actquantity;
                                    sentCount++;
                                }
                            }

                            newDetailInfos.Add(new PurchasingDetailInfo(pInfo.PurchasingID, detail.GoodsID, detail.GoodsName, detail.Units, detail.GoodsCode,
                                        detail.Specification, detail.CompanyID, detailGroup.Max(act => act.Price), (int)finalDics[detailGroup.Key] - sentCount, detailGroup.Sum(act => act.RealityQuantity),
                                        detail.State, detail.Description, detail.PurchasingGoodsID, (int)PurchasingGoodsType.NoGift)
                            { CPrice = detailGroup.Max(act => act.CPrice) });

                            if (sentCount>0)
                            {
                                newDetailInfos.Add(new PurchasingDetailInfo(pInfo.PurchasingID, detail.GoodsID, detail.GoodsName, detail.Units, detail.GoodsCode,
                                detail.Specification, detail.CompanyID, 0, sentCount, 0,
                                detail.State, detail.Description, Guid.NewGuid(), (int)PurchasingGoodsType.Gift));
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region [非现返生成借记单]
                Dictionary<Guid, int> noteDics = new Dictionary<Guid, int>();
                PurchasePromotionInfo debitPromotion = promotionInfos.FirstOrDefault(w => w.PromotionType == (int)PurchasePromotionType.NoBack);
                if (debitPromotion != null)
                {
                    hasPromotion = true;
                    finalDics = DistributionByGoodsId(distributionQuantity, dics);
                    int quantity = ((int)distributionQuantity / debitPromotion.BuyCount) * debitPromotion.GivingCount;
                    var info = deleteDetails.First();
                    //按商品总数量进行赠送

                    foreach (var goodsGroup in deleteDetails.GroupBy(ent => ent.GoodsID))
                    {
                        if(!finalDics.ContainsKey(goodsGroup.Key))continue;
                        var count = ((int)(finalDics[goodsGroup.Key] / debitPromotion.BuyCount)) * debitPromotion.GivingCount;
                        if (count > 0)
                        {
                            noteDics.Add(goodsGroup.Key, count);
                            quantity = quantity - count;
                        }    
                    }

                    if (!debitPromotion.IsSingle)
                    {
                        if (quantity > 0)
                        {
                            if (noteDics.Count > 0 && noteDics.ContainsKey(info.GoodsID))
                            {
                                noteDics[info.GoodsID] = noteDics[info.GoodsID] + quantity;
                            } 
                            else
                                noteDics.Add(info.GoodsID, quantity);
                        }
                    }
                    if (noteDics.Count > 0)
                    {
                        debitNote = new DebitNoteInfo
                        {
                            PurchasingId = pInfo.PurchasingID,
                            PurchasingNo = pInfo.PurchasingNo,
                            CompanyId = pInfo.CompanyID,
                            PresentAmount = debitNoteDetailList.Sum(w => w.Amount),
                            CreateDate = DateTime.Now,
                            FinishDate = DateTime.MinValue,
                            State = (int)DebitNoteState.ToPurchase,
                            WarehouseId = pInfo.WarehouseID,
                            Memo = "",
                            PersonResponsible = pInfo.PersonResponsible,
                            NewPurchasingId = Guid.Empty
                        };

                        var details= from item in noteDics
                                     let first = deleteDetails.First(ent => ent.GoodsID == item.Key)
                                     select new DebitNoteDetailInfo
                                     {
                                         PurchasingId = pInfo.PurchasingID,
                                         GoodsId = first.GoodsID,
                                         GoodsName = first.GoodsName,
                                         Specification = first.Specification,
                                         GivingCount = item.Value,
                                         ArrivalCount = 0,
                                         Price = first.Price,
                                         State = 0,
                                         Amount = item.Value * first.Price,
                                         Memo = "",
                                         Id = Guid.NewGuid()
                                     };
                        debitNoteDetailList.AddRange(details);
                    }
                }
                #endregion
            }

            if (newDetailInfos.Count==0)
            {
                if(!hasPromotion)
                {
                    finalDics = DistributionByGoodsId(distributionQuantity, deleteDetails.ToDictionary(k=>k.PurchasingGoodsID,v=>v.PlanQuantity));
                    foreach (var detail in deleteDetails)
                    {
                        detail.PlanQuantity = finalDics[detail.PurchasingGoodsID];
                        newDetailInfos.Add(detail);
                    }
                }
                else
                {
                    finalDics = DistributionByGoodsId(distributionQuantity, dics);
                    newDetailInfos.AddRange(deleteDetails.Select(detail => new PurchasingDetailInfo
                    {
                        PurchasingID = pInfo.PurchasingID,
                        GoodsID = detail.GoodsID,
                        GoodsName = detail.GoodsName,
                        Units = detail.Units,
                        GoodsCode = detail.GoodsCode,
                        Specification = detail.Specification,
                        CompanyID = detail.CompanyID,
                        Price = detail.Price,
                        PlanQuantity = finalDics[detail.GoodsID],
                        RealityQuantity = detail.RealityQuantity,
                        State = detail.State,
                        Description = detail.Description,
                        PurchasingGoodsID = detail.PurchasingGoodsID,
                        PurchasingGoodsType = detail.PurchasingGoodsType,
                        CPrice = detail.CPrice
                    }));
                }
                //newDetailInfos.AddRange(from detailGroup in deleteDetails.GroupBy(ent => ent.GoodsID).OrderByDescending(ent => ent.Sum(act => act.PlanQuantity))
                    //                        let detail = detailGroup.First()
                    //                        where finalDics.ContainsKey(detailGroup.Key)
                    //                        select new PurchasingDetailInfo(pInfo.PurchasingID, detail.GoodsID, detail.GoodsName, detail.Units, detail.GoodsCode,
                    //                        detail.Specification, detail.CompanyID, detail.Price, finalDics[detail.GoodsID], detailGroup.Sum(act => act.RealityQuantity),
                    //                        detail.State, detail.Description, detail.PurchasingGoodsID, (int)PurchasingGoodsType.NoGift)
                    //                        { CPrice = detailGroup.Max(act => act.CPrice) });
            }
        }

        public Dictionary<Guid, double> DistributionByGoodsId(double planquantity,Dictionary<Guid,double> originalDics)
        {
            var dics = new Dictionary<Guid, double>();
            var oldPlanQuantity = originalDics.Values.Sum();
            double yun;
            bool isAdd = true;
            if (planquantity >= oldPlanQuantity)
            {
                var turn = (int)(planquantity/oldPlanQuantity);
                dics = originalDics.ToDictionary(k => k.Key, v => v.Value * turn);
                yun = planquantity - turn * oldPlanQuantity;
            }
            else
            {
                yun = planquantity;
                isAdd = false;
            }
            int lastIndex = originalDics.Count;
            foreach (var item in originalDics.OrderByDescending(ent => ent.Value))
            {
                if (yun == 0) break;
                var percent = (yun * item.Value) / oldPlanQuantity;  //百分比
                var str = string.Format("{0}", percent);
                var count = (int)percent;  //小数的整数位
                var after = Convert.ToInt32(str.Substring(str.IndexOf(".") + 1, 1)); //小数位后一位
                if (isAdd)
                {
                    count = count == 0 ? 1 : after >= 5 ? count + 1 : count;
                    dics[item.Key] = dics[item.Key] + count;
                }
                else
                {
                    if (lastIndex == 1)
                        count = (int)yun;
                    else
                        count = after >= 5 ? count + 1 : count;
                    dics.Add(item.Key,count);
                }
                yun = (int)yun - count;
                lastIndex--;
            }
            return dics.Where(ent=>ent.Value>0).ToDictionary(k=>k.Key,v=>v.Value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UpdatePriceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal NewPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal CPrice { get; set; }
    }

    [Serializable]
    public class GridGoodsInfo
    {
        public Guid Id { get; set; }
        public Guid RealGoodsId { get; set; }
        public Guid CompanyId { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public decimal CPrice { get; set; }
        public int PlanQuantity { get; set; }
        public int PurchasingGoodsType { get; set; }
        public bool IsSelected { get; set; }
    }


}
