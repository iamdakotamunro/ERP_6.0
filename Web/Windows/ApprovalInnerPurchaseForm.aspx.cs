using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using ERP.Enum.Attribute;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class ApprovalInnerPurchaseForm : Page
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly CodeManager _code = new CodeManager();
        static readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        public DateTime dt = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            var stockId = new Guid(Request.QueryString["StockId"]);
            if (!string.IsNullOrEmpty(Request.QueryString["IsSel"]))
            {
                btn_Approval.Visible = false;
                btn_Return.Visible = false;
            }
            BindData(stockId);
        }

        private void BindData(Guid stockId)
        {
            var innerPurchase = _storageRecordDao.GetInnerPurchaseRelationInfo(stockId);
            OutStockId = innerPurchase.OutStockId;
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);//出库
            InDetailList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
            OutWarehouseId = storageRecordinfo.WarehouseId;
            OutHostingFilialeId = storageRecordinfo.FilialeId;
            OutStorageType = storageRecordinfo.StorageType;

            List<Guid> goodsIdOrRealGoodsIdList = InDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
            var goodsStockQuantityList = new Dictionary<Guid, int>();
            //出库仓储
            var personinfo = CurrentSession.Personnel.Get();
            var wList = WMSSao.GetWarehouseAuth(personinfo.PersonnelId)
                    .FirstOrDefault(p => p.WarehouseId == storageRecordinfo.WarehouseId);
            if (wList != null)
            {
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                if (slist != null)
                {
                    //根据储位判断是否根据物流配送公司获取库存
                    goodsStockQuantityList = WMSSao.GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(storageRecordinfo.WarehouseId, null,
                        goodsIdOrRealGoodsIdList,
                        slist.IsReal ? storageRecordinfo.FilialeId : Guid.Empty);
                }
            }
            var goodsIds = InDetailList.Select(ent => ent.GoodsId).Distinct().ToList();
            var goodsInfos = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds).ToDictionary(k => k.GoodsId, v => v);
            if (goodsInfos.Count == 0 || goodsIds.Count != goodsInfos.Count)
            {
                RAM.Alert("GMS获取商品信息失败!");
                return;
            }
            foreach (var info in InDetailList)
            {
                info.Units = goodsInfos[info.GoodsId].Units;
                if (goodsStockQuantityList != null)
                {
                    //可出库数
                    var goodsStockKeyValuePair = goodsStockQuantityList.FirstOrDefault(w => w.Key == info.RealGoodsId);
                    info.NonceWarehouseGoodsStock = goodsStockKeyValuePair.Value;
                }
                else
                {
                    info.NonceWarehouseGoodsStock = 0;
                }
            }
            var warehouseList = WarehouseManager.GetWarehouseDic();
            var list = new List<HostingFilialeAuth>();
            var listStorage = new List<StorageAuth>();
            var warehouseAuth = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == storageRecordinfo.WarehouseId);
            if (warehouseAuth != null)
            {
                if (warehouseAuth.Storages != null)
                {
                    listStorage.AddRange(warehouseAuth.Storages.Where(act => act.StorageType == (Byte)StorageAuthType.Z || act.StorageType == (Byte)StorageAuthType.L || act.StorageType == (Byte)StorageAuthType.S));

                    var storageAuth = warehouseAuth.Storages.FirstOrDefault(p => p.StorageType == storageRecordinfo.StorageType);
                    if (storageAuth != null && storageAuth.Filiales != null)
                    {
                        list.AddRange(storageAuth.Filiales);
                    }
                }
            }

            txt_DateCreated.Text = storageRecordinfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = storageRecordinfo.Transactor;
            RCB_OutWarehouse.Text = warehouseList[storageRecordinfo.WarehouseId];
            RCB_OutStorageAuth.Text = listStorage.First(ent => ent.StorageType == storageRecordinfo.StorageType).StorageTypeName;
            RCB_OutHostingFilialeAuth.Text = CacheCollection.Filiale.GetName(storageRecordinfo.FilialeId);
            RCB_InWarehouse.Text = warehouseList.ContainsKey(innerPurchase.InWarehouseId) ? warehouseList[innerPurchase.InWarehouseId] : String.Empty;
            RCB_InStorageAuth.Text = listStorage.First(ent => ent.StorageType == innerPurchase.InStorageType).StorageTypeName;
            RCB_InHostingFilialeAuth.Text = CacheCollection.Filiale.GetName(innerPurchase.InHostingFilialeId);
            txt_Description.Text = storageRecordinfo.Description;
        }

        protected void RGGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Lab_TotalNumber.Text = string.Format("{0}", InDetailList.Sum(p => Math.Abs(p.Quantity)));
            if (InDetailList.Count > 0)
            {
                List<Guid> goodsIdOrRealGoodsIdList = InDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
                var warehouseIdSelected = OutWarehouseId;
                var storageType = byte.Parse(OutStorageType.ToString());
                var stockQuantitys = new Dictionary<Guid, int>();
                if (storageType > 0)
                {
                    var warehouse = CurrentSession.Personnel.WarehouseList.FirstOrDefault(act => act.WarehouseId == warehouseIdSelected);
                    if (warehouse != null)
                    {
                        var storage = warehouse.Storages.FirstOrDefault(p => p.StorageType == storageType);
                        if (storage != null)
                        {
                            if (storage.IsReal)
                            {
                                if (!string.IsNullOrWhiteSpace(OutHostingFilialeId.ToString()) && OutHostingFilialeId != Guid.Empty)
                                {
                                    stockQuantitys = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, OutHostingFilialeId);
                                }
                            }
                            else
                            {
                                stockQuantitys = WMSSao.GetGoodsStockByStorageType(goodsIdOrRealGoodsIdList, warehouseIdSelected, storageType, null);
                            }
                        }
                    }
                }

                foreach (var info in InDetailList)
                {
                    if (string.IsNullOrWhiteSpace(info.Units))
                    {
                        var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(info.GoodsId);
                        info.Units = goodsInfo != null ? goodsInfo.Units : "";
                    }
                    info.NonceWarehouseGoodsStock = stockQuantitys.ContainsKey(info.RealGoodsId) ? stockQuantitys[info.RealGoodsId] : 0;
                }
            }
            RGGoods.DataSource = InDetailList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }

        /// <summary>核准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApproval_Click(object sender, EventArgs e)
        {
            var stockId = new Guid(Request.QueryString["StockId"]);
            var innerPurchase = _storageRecordDao.GetInnerPurchaseRelationInfo(stockId);
            var personnel = CurrentSession.Personnel.Get();
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string description = string.Format("[出库审核(审核人:{0};审核备注:{1});{2}]", personnel.RealName, "核准", dateTime);
            //获得出入库记录
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            if (storageRecordinfo.StockState != (int)StorageRecordState.WaitAudit)
            {
                RAM.Alert("当前单据状态已改变，审核失败！");
                return;
            }


            var goodsStockList = _storageManager.GetStorageRecordDetailListByStockId(stockId);

            //总金额
            decimal accountReceivable = 0;
            //总数量
            double subtotalQuantity = 0;
            if (goodsStockList != null)
            {
                string numString = String.Empty; ;
                foreach (StorageRecordDetailInfo goodsStockInfo in goodsStockList)
                {
                    if (goodsStockInfo.Quantity == 0)
                    {
                        numString += goodsStockInfo.GoodsName + ":" + goodsStockInfo.Specification + ",";
                    }
                    accountReceivable += Convert.ToDecimal(goodsStockInfo.Quantity) * goodsStockInfo.UnitPrice;
                    subtotalQuantity += Convert.ToDouble(goodsStockInfo.Quantity);
                }
                if (!String.IsNullOrWhiteSpace(numString))
                {
                    RAM.Alert(numString.Trim(',') + " 出库数不能为0！");
                    return;
                }
            }
            if ((decimal)subtotalQuantity == 0)
            {
                RAM.Alert("商品数量不能为0！");
                return;
            }
            string transactor = txt_Transactor.Text;
            StringBuilder errorMsg = new StringBuilder();
            var purchasingId = Guid.Empty;
            String linkTradeCode = String.Empty;
            var outFilialeThirdCompany = _companyCussent.GetCompanyByRelevanceFilialeId(innerPurchase.OutHostingFilialeId);
            if (outFilialeThirdCompany == null)
            {
                RAM.Alert("出库物流配送公司未关联往来单位");
                return;
            }
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    #region 采购单
                    var warehouseId = innerPurchase.InWarehouseId;
                    var filialeId = innerPurchase.InHostingFilialeId;
                    var pmid = Guid.Empty;
                    string pmName = string.Empty;

                    var goodsIdOrRealGoodsIdList = goodsStockList.ToDictionary(k => k.RealGoodsId, v => v.GoodsId);

                    IPurchaseSet purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Read);
                    var purchaseSetList = purchaseSet.GetPurchaseSetInfoList(goodsIdOrRealGoodsIdList.Values, warehouseId, filialeId);

                    var purchasingDic = new Dictionary<PurchasingInfo, IList<PurchasingDetailInfo>>();
                    var codeManager = new CodeManager();
                    foreach (GridDataItem dataItem in RGGoods.Items)
                    {
                        IList<PurchasingDetailInfo> plist = new List<PurchasingDetailInfo>();

                        var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                        var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsId").ToString());
                        var goodsCode = dataItem.GetDataKeyValue("GoodsCode").ToString();
                        var goodsName = dataItem.GetDataKeyValue("GoodsName").ToString();
                        var specification = dataItem.GetDataKeyValue("Specification").ToString();
                        var tbQuantity = dataItem.GetDataKeyValue("Quantity").ToString(); 
                         var units = dataItem.GetDataKeyValue("Units").ToString();
                        var tbNonceWarehouseGoodsStock = (Literal)dataItem.FindControl("Lab_NonceWarehouseGoodsStock");
                        if (tbNonceWarehouseGoodsStock.Text == "0")
                        {
                            RAM.Alert(goodsName + " 可用库存为0！");
                            return;
                        }

                        PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsId) ?? new PurchaseSetInfo();
                        if (purchaseSetInfo.GoodsId==Guid.Empty)
                        {
                            RAM.Alert(string.Format("{0}对应的入库仓库、物流公司没进行商品采购责任人绑定设置！", goodsName));
                            return;
                        }
                        var purchId = Guid.NewGuid();
                        //采购单明细
                        var dInfo = new PurchasingDetailInfo(purchId, realGoodsId, goodsName, units, goodsCode, specification, outFilialeThirdCompany.CompanyId,
                            0,Convert.ToDouble(tbQuantity), 0, 0, "", Guid.NewGuid(), (int)PurchasingGoodsType.NoGift)
                        {
                            
                            CPrice = purchaseSetInfo.PurchasePrice
                        };
                        var purchasingDetailInfo = _purchasingDetail.GetChildGoodsSale(realGoodsId, warehouseId, DateTime.Now, filialeId);
                        if (purchasingDetailInfo!=null)
                        {
                            dInfo.SixtyDaySales = purchasingDetailInfo.SixtyDaySales;
                            dInfo.ThirtyDaySales = purchasingDetailInfo.ThirtyDaySales;
                            dInfo.ElevenDaySales = purchasingDetailInfo.ElevenDaySales;
                        }

                        //采购单
                        var purchasingInfo = new PurchasingInfo(purchId, "", outFilialeThirdCompany.CompanyId, outFilialeThirdCompany.CompanyName,
                                                        filialeId, warehouseId, (int)PurchasingState.StockIn,
                                                       (int)PurchasingType.AutoInternal, DateTime.Now, DateTime.MaxValue,
                                                       string.Format("[采购类别:{0};采购人:{1}]", EnumAttribute.GetKeyName(PurchasingType.AutoInternal), personnel.RealName), pmid, pmName, personnel.RealName)
                        {
                            Director = purchaseSetInfo.PersonResponsibleName,
                            PersonResponsible = purchaseSetInfo.PersonResponsible,
                            ArrivalTime = dt,
                            PurchasingFilialeId = filialeId
                        };
                        bool isHave = false;
                        if (purchasingDic.Count > 0)
                        {
                            foreach (KeyValuePair<PurchasingInfo, IList<PurchasingDetailInfo>> keyValue in purchasingDic)
                            {
                                if (keyValue.Key.PersonResponsible == purchasingInfo.PersonResponsible && keyValue.Key.CompanyID == purchasingInfo.CompanyID)
                                {
                                    dInfo.PurchasingID = keyValue.Key.PurchasingID;
                                    keyValue.Value.Add(dInfo);
                                    isHave = true;
                                    break;
                                }
                            }
                        }
                        if (isHave == false)
                        {
                            purchasingInfo.PurchasingNo = codeManager.GetCode(CodeType.PH);
                            linkTradeCode = purchasingInfo.PurchasingNo;
                            plist.Add(dInfo);
                            purchasingDic.Add(purchasingInfo, plist);
                        }
                    }

                    foreach (KeyValuePair<PurchasingInfo, IList<PurchasingDetailInfo>> keyValue in purchasingDic)
                    {
                        var pInfo = keyValue.Key;
                        IList<PurchasingDetailInfo> plist = keyValue.Value;
                        //存在商品采购价大于绑定价格，采购单为调价待审核
                        if (plist.Any(act => act.Price != 0 && act.Price > act.CPrice))
                        {
                            pInfo.PurchasingState = (int)PurchasingState.WaitingAudit;
                        }
                        //判断是否选择采购公司
                        if (pInfo.PurchasingFilialeId != Guid.Empty)
                        {
                            pInfo.PurchasingFilialeId = innerPurchase.InHostingFilialeId;
                            //pInfo.IsOut = true;
                        }
                        purchasingId = pInfo.PurchasingID;
                        _purchasing.PurchasingInsert(pInfo);
                        //添加采购单添加操作记录添加
                        WebControl.AddOperationLog(personnel.PersonnelId, personnel.RealName, pInfo.PurchasingID, pInfo.PurchasingNo,
                            OperationPoint.PurchasingManager.CreatePurchaseList.GetBusinessInfo(), string.Empty);


                        var purchasingDetailManager = new PurchasingDetailManager(_purchasingDetail, _purchasing);
                        //保存采购单明细
                        purchasingDetailManager.Save(plist);
                    }
                    #endregion

                    #region 入库单
                    var inStockInfo = new StorageRecordInfo
                    {
                        StockId = Guid.NewGuid(),
                        FilialeId = innerPurchase.InHostingFilialeId,//入库
                        WarehouseId = innerPurchase.InWarehouseId,
                        ThirdCompanyID = outFilialeThirdCompany.CompanyId,
                        RelevanceFilialeId = Guid.Empty,//出库
                        RelevanceWarehouseId = Guid.Empty,
                        AccountReceivable = accountReceivable,
                        DateCreated = DateTime.Now,
                        Description = description,
                        LinkTradeCode = linkTradeCode,
                        LinkTradeID = purchasingId,
                        StockState = (int)StorageRecordState.WaitAudit,
                        StockType = (int)StorageRecordType.BuyStockIn,
                        StockValidation = false,
                        SubtotalQuantity = (decimal)subtotalQuantity,
                        TradeCode = _code.GetCode(CodeType.RK),
                        Transactor = transactor,
                        //IsOut = true,
                        StorageType = innerPurchase.InStorageType,
                        LinkTradeType = (int)StorageRecordLinkTradeType.Purchasing,
                        TradeBothPartiesType = (int)TradeBothPartiesType.HostingToHosting
                    };
                    var inResult = _storageManager.NewInsertStockAndGoods(inStockInfo, goodsStockList);
                    //PurchasingManager.WriteInstance.PurchasingUpdate(inStockInfo.LinkTradeID, PurchasingState.StockIn);
                    if (!inResult)
                    {
                        RAM.Alert("入库单添加失败！");
                        return;
                    }

                    #endregion

                    #region 出库单
                    var outStockInfo = new StorageRecordInfo
                    {
                        StockId = stockId,
                        Description = description,
                        LinkTradeCode = linkTradeCode,
                        LinkTradeID = purchasingId,
                        DateCreated = DateTime.Now

                    };
                    var updateStock = _storageManager.UpdateStockPurchse(outStockInfo);
                    if (!updateStock)
                    {
                        errorMsg.Append("更新出入库单据失败！");
                    }

                    #endregion

                    #region 关联表
                    _storageRecordDao.UpdateInnerPurchaseRelationInfo(OutStockId, inStockInfo.StockId, purchasingId, innerPurchase.OutWarehouseId, innerPurchase.OutHostingFilialeId, innerPurchase.OutStorageType, innerPurchase.InWarehouseId, innerPurchase.InHostingFilialeId, innerPurchase.InStorageType);
                    #endregion

                    #region 出货单
                    bool result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Approved, description);
                    if (result)
                    {
                        storageRecordinfo.Description = string.Format("{0}{1}", storageRecordinfo.Description, description);
                        //新增出货单据
                        string billNo, msg;
                        var resultAdd = WMSSao.InsertOutGoodsBill(_storageManager.ConvertToWMSOutGoodsBill(storageRecordinfo, goodsStockList, outFilialeThirdCompany.CompanyName, personnel.PersonnelId, personnel.RealName), out billNo, out msg);
                        if (resultAdd)
                        {
                            if (!string.IsNullOrEmpty(billNo))
                            {
                                if (!_storageRecordDao.SetBillNo(stockId, billNo))
                                {
                                    errorMsg.Append("出库单出货单号更新失败！");
                                }
                            }
                            else
                            {
                                errorMsg.Append("仓储反馈出货单号为空(" + msg + ")！");
                            }
                        }
                        else
                        {
                            errorMsg.AppendFormat("核准失败！{0}",msg);
                        }
                    }
                    else
                    {
                        errorMsg.Append("更新出入库单据失败！");
                    }
                    #endregion
                    
                    if (string.IsNullOrEmpty(errorMsg.ToString()))
                    {
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    errorMsg.Append(ex.Message);
                }
                finally
                {
                    ts.Dispose();
                }
            }
            if (string.IsNullOrEmpty(errorMsg.ToString()))
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            else
            {
                RAM.ResponseScripts.Add("alert('" + errorMsg + "');");
            }
        }

        /// <summary>核退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            var stockId = new Guid(Request.QueryString["StockId"]);
            //获得出入库记录
            var storageRecordinfo = _storageRecordDao.GetStorageRecord(stockId);
            if (storageRecordinfo.StockState != (int)StorageRecordState.WaitAudit)
            {
                RAM.Alert("当前单据状态已改变，审核失败！");
                return;
            }
            var realName = CurrentSession.Personnel.Get().RealName;
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string description = string.Format("[出库审核(审核人:{0};审核备注:{1});{2}]", realName, "核退", dateTime);
            //修改单据状态
            var result = _storageManager.NewSetStateStorageRecord(stockId, StorageRecordState.Refuse, description);
            RAM.ResponseScripts.Add(result ? "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");" : "alert('审批失败！')");
        }

        private IList<StorageRecordDetailInfo> InDetailList
        {
            get
            {
                if (ViewState["InDetailList"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["InDetailList"];
            }
            set { ViewState["InDetailList"] = value; }
        }

        private Guid OutWarehouseId
        {
            get
            {
                if (ViewState["OutWarehouseId"] == null)
                    return Guid.Empty;
                return (Guid)ViewState["OutWarehouseId"];
            }
            set { ViewState["OutWarehouseId"] = value; }
        }

        private int OutStorageType
        {
            get
            {
                if (ViewState["OutStorageType"] == null)
                    return 0;
                return (int)ViewState["OutStorageType"];
            }
            set { ViewState["OutStorageType"] = value; }
        }

        private Guid OutHostingFilialeId
        {
            get
            {
                if (ViewState["OutHostingFilialeId"] == null)
                    return Guid.Empty;
                return (Guid)ViewState["OutHostingFilialeId"];
            }
            set { ViewState["OutHostingFilialeId"] = value; }
        }

        private Guid OutStockId
        {
            get
            {
                if (ViewState["OutStockId"] == null)
                    return Guid.Empty;
                return (Guid)ViewState["OutStockId"];
            }
            set { ViewState["OutStockId"] = value; }
        }
    }
}