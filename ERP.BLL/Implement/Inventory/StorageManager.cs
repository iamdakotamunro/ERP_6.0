using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using Keede.Ecsoft.Model;
using KeedeGroup.WMS.Infrastructure.CrossCutting.Enum;
using CodeType = ERP.Enum.CodeType;
using ERP.Model.WMS;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using System.Transactions;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IShop;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>▄︻┻┳═一  商品出入库记录业务层  最后修改提交 陈重文  2014-12-26  （更新、删除、优化方法）
    /// </summary>
    public class StorageManager : BllInstance<StorageManager>
    {
        readonly IGoodsCenterSao _goodsInfoSao;
        readonly IGoodsCenterSao _goodsCenterSao;
        readonly IStorageRecordDao _storageDao;
        readonly IGoodsOrder _goodsOrder;
        readonly IGoodsOrderDetail _goodsOrderDetail;
        readonly CodeManager _codeManager;
        readonly WMSSao _wmsSao = new WMSSao();
        private readonly RealTimeGrossSettlementManager _realTimeGrossSettlementManager;
        private readonly IInternalPriceSetManager _internalPriceSetManager;
        private readonly ICompanyCussent _companyCussent;
        private readonly IApplyStockDAL _applyStockDal;

        public StorageManager(GlobalConfig.DB.FromType fromType = GlobalConfig.DB.FromType.Write)
        {
            _goodsInfoSao = new GoodsCenterSao();
            _storageDao = InventoryInstance.GetStorageRecordDao(fromType);
            _goodsCenterSao = new GoodsCenterSao();
            _goodsOrder = new GoodsOrder(fromType);
            _goodsOrderDetail = new GoodsOrderDetail(fromType);
            _codeManager = new CodeManager();
            _realTimeGrossSettlementManager = new RealTimeGrossSettlementManager(fromType);
            _internalPriceSetManager = new InternalPriceSetManager(fromType);
            _companyCussent = new CompanyCussent(fromType);
            _applyStockDal = new ApplyStockDAL(fromType);
        }

        #region 新出入库使用方法
        /// <summary> 根据出入库记录ID获取出入库明细
        /// </summary>
        /// <param name="stockId">出入库记录ID</param>
        /// <returns></returns>
        public IList<StorageRecordDetailInfo> GetStorageRecordDetailListByStockId(Guid stockId)
        {
            return _storageDao.GetStorageRecordDetailListByStockId(stockId);
        }

        /// <summary>更新出入库单据状态和描述
        /// </summary>
        /// ADD ww
        /// 2016-08-01
        /// <param name="stockId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="description"></param>
        public bool NewSetStateStorageRecord(Guid stockId, StorageRecordState state, string description)
        {
            return _storageDao.NewSetStateStorageRecord(stockId, state, description);
        }

        /// <summary>
        /// </summary>
        /// <param name="stockId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="accountReceivable">单据总额</param>
        /// <param name="description"></param>
        public bool SetStorageRecordToFinished(Guid stockId, StorageRecordState state, decimal accountReceivable, string description, Dictionary<Guid, int> stockQuantitys)
        {
            var flag = _storageDao.SetStateAndAccountReceivableForStorageRecord(stockId, state, accountReceivable, description);
            if (flag)
            {
                foreach (var i in stockQuantitys)
                {
                    flag = _storageDao.UpdateNonCurrentStockByRealGoodsId(stockId, i.Key, i.Value);
                    if (!flag)
                        break;
                }
            }
            return flag;
        }

        /// <summary>
        /// 新增出入库单据含明细
        /// </summary>
        /// ADD ww
        /// 2016-07-29
        /// <param name="storageRecordInfo">出入库单据模型</param>
        /// <param name="storageRecordDetailList">出入库单据详细模型</param>
        /// <param name="errorMessage">异常信息</param>
        public bool NewAddStorageRecordAndDetailList(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailList, out string errorMessage)
        {
            List<Guid> realGoodsId = storageRecordDetailList.Select(w => w.RealGoodsId).Distinct().ToList();
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsInfoSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsId);
            if (dicGoods != null && dicGoods.Count > 0)
            {
                foreach (var info in storageRecordDetailList)
                {
                    bool hasKey = dicGoods.ContainsKey(info.RealGoodsId);
                    if (hasKey)
                    {
                        GoodsInfo goodsBaseInfo = dicGoods[info.RealGoodsId];
                        if (goodsBaseInfo != null && goodsBaseInfo.ExpandInfo != null)
                        {
                            info.JoinPrice = goodsBaseInfo.ExpandInfo.JoinPrice;
                            info.GoodsType = goodsBaseInfo.GoodsType;
                        }
                    }
                }
            }

            return _storageDao.Insert(storageRecordInfo, storageRecordDetailList, out errorMessage);
        }


        /// <summary>
        /// 新增出入库记录
        /// </summary>
        /// ADD ww
        /// 2016-07-29
        /// <param name="stockInfo">出入库记录</param>
        /// <param name="goodsStockList">出入库详细记录</param>
        public bool NewInsertStockAndGoods(StorageRecordInfo stockInfo, IList<StorageRecordDetailInfo> goodsStockList)
        {
            List<Guid> realGoodsId = goodsStockList.Select(w => w.RealGoodsId).Distinct().ToList();
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsInfoSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsId);
            if (dicGoods != null && dicGoods.Count > 0)
            {
                foreach (var info in goodsStockList)
                {
                    bool hasKey = dicGoods.ContainsKey(info.RealGoodsId);
                    if (hasKey)
                    {
                        GoodsInfo goodsBaseInfo = dicGoods[info.RealGoodsId];
                        if (goodsBaseInfo != null && goodsBaseInfo.ExpandInfo != null)
                        {
                            info.JoinPrice = goodsBaseInfo.ExpandInfo.JoinPrice;
                            info.GoodsType = goodsBaseInfo.GoodsType;
                        }
                    }
                }
            }
            return _storageDao.NewSaveStoreRecord(stockInfo, goodsStockList);
        }

        public bool NewInsertStockAndGoodsNoTrans(StorageRecordInfo stockInfo, IList<StorageRecordDetailInfo> goodsStockList)
        {
            List<Guid> realGoodsId = goodsStockList.Select(w => w.RealGoodsId).Distinct().ToList();
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsInfoSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsId);
            if (dicGoods != null && dicGoods.Count > 0)
            {
                foreach (var info in goodsStockList)
                {
                    bool hasKey = dicGoods.ContainsKey(info.RealGoodsId);
                    if (hasKey)
                    {
                        GoodsInfo goodsBaseInfo = dicGoods[info.RealGoodsId];
                        if (goodsBaseInfo != null && goodsBaseInfo.ExpandInfo != null)
                        {
                            info.JoinPrice = goodsBaseInfo.ExpandInfo.JoinPrice;
                            info.GoodsType = goodsBaseInfo.GoodsType;
                        }
                    }
                }
            }
            return _storageDao.NewSaveStoreRecordNoTrans(stockInfo, goodsStockList);
        }
        public bool AddStorageRecord(StorageRecordInfo stockInfo, IList<StorageRecordDetailInfo> goodsStockList)
        {
            List<Guid> realGoodsId = goodsStockList.Select(w => w.RealGoodsId).Distinct().ToList();
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsInfoSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsId);
            if (dicGoods != null && dicGoods.Count > 0)
            {
                foreach (var info in goodsStockList)
                {
                    bool hasKey = dicGoods.ContainsKey(info.RealGoodsId);
                    if (hasKey)
                    {
                        GoodsInfo goodsBaseInfo = dicGoods[info.RealGoodsId];
                        if (goodsBaseInfo != null && goodsBaseInfo.ExpandInfo != null)
                        {
                            info.JoinPrice = goodsBaseInfo.ExpandInfo.JoinPrice;
                            info.GoodsType = goodsBaseInfo.GoodsType;
                        }
                    }
                }
            }
            return _storageDao.AddStorageRecord(stockInfo, goodsStockList);
        }

        /// <summary>
        /// 更新出入库记录和出入库库明细
        /// ADD ww
        /// 2016-08-01
        /// </summary>
        /// <param name="storageRecordInfo">出入库记录</param>
        /// <param name="storageRecordDetailInfoList">出入库明细</param>
        public void UpdateStorageRecordAndStorageRecordDetail(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailInfoList)
        {
            List<Guid> realGoodsId = storageRecordDetailInfoList.Select(w => w.RealGoodsId).Distinct().ToList();
            Dictionary<Guid, GoodsInfo> dicGoods = _goodsInfoSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsId);
            if (dicGoods != null && dicGoods.Count > 0)
            {
                foreach (var info in storageRecordDetailInfoList)
                {
                    bool hasKey = dicGoods.ContainsKey(info.RealGoodsId);
                    if (hasKey)
                    {
                        GoodsInfo goodsBaseInfo = dicGoods[info.RealGoodsId];
                        if (goodsBaseInfo != null && goodsBaseInfo.ExpandInfo != null)
                        {
                            info.JoinPrice = goodsBaseInfo.ExpandInfo.JoinPrice;
                            info.GoodsType = goodsBaseInfo.GoodsType;
                        }
                    }
                }
            }
            _storageDao.NewUpdateStockAndGoods(storageRecordInfo, storageRecordDetailInfoList);
        }

        public bool UpdateStockPurchse(StorageRecordInfo storageRecordInfo)
        {
            return _storageDao.UpdateStockPurchse(storageRecordInfo);
        }

        #endregion
        public StorageRecordApplyInDTO ConvertToWMSInGoodsBill(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailInfos, String companyName, Guid personnelId, String realName, Guid saleFilialeId=default(Guid), bool inferior = false)
        {
            var storageRecordApplyInDto = new StorageRecordApplyInDTO
            {
                HostingFilialeId = storageRecordInfo.FilialeId,
                SaleFilialeId = saleFilialeId==Guid.Empty?_companyCussent.GetRelevanceFilialeIdByCompanyId(storageRecordInfo.ThirdCompanyID):saleFilialeId,
                OperatorId = personnelId,
                OperatorName = realName,
                PurchaseResponsiblePersonName = storageRecordInfo.Transactor,
                SourceNo = storageRecordInfo.TradeCode,
                StorageType = Convert.ToByte(storageRecordInfo.StorageType),
                WarehouseId = storageRecordInfo.WarehouseId,
                SupplierName = companyName
            };

            var detaillist = new List<StorageRecordApplyInDetailDTO>();
            var goodsInfos = _goodsCenterSao.GetDictRealGoodsUnitModel(storageRecordDetailInfos.Select(act => act.RealGoodsId).Distinct().ToList());
            foreach (var info in storageRecordDetailInfos)
            {
                var detailInfo = detaillist.FirstOrDefault(p => p.RealGoodsId == info.RealGoodsId && p.BatchNo == info.BatchNo);
                if (detailInfo != null)
                {
                    detailInfo.InQuantity += info.Quantity;
                }
                else
                {
                    var storageRecordApplyInDetailDto = new StorageRecordApplyInDetailDTO
                    {
                        GoodsId = info.GoodsId,
                        GoodsName = info.GoodsName,
                        InQuantity = info.Quantity,
                        RealGoodsId = info.RealGoodsId,
                        Sku = info.Specification,
                        Unit = goodsInfos.ContainsKey(info.RealGoodsId) ? goodsInfos[info.RealGoodsId].Units : string.Empty,
                        BatchNo = info.BatchNo,
                        ShelfType = storageRecordInfo.StorageType==(int)StorageType.S?(info.ShelfType!=0?info.ShelfType : inferior?(Byte)ShelfType.Inferior:(Byte)ShelfType.Good):(Byte)ShelfType.Normal
                    };
                    detaillist.Add(storageRecordApplyInDetailDto);
                }
            }
            storageRecordApplyInDto.Details = detaillist;
            return storageRecordApplyInDto;
        }

        public StorageRecordApplyOutDTO ConvertToWMSOutGoodsBill(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailInfos, String companyName, Guid personnelId, String realName, bool isSale = false)
        {
            var storageRecordApplyOutDto = new StorageRecordApplyOutDTO
            {
                HostingFilialeId = storageRecordInfo.FilialeId,
                SaleFilialeId = isSale ? storageRecordInfo.FilialeId : _companyCussent.GetRelevanceFilialeIdByCompanyId(storageRecordInfo.ThirdCompanyID),
                OperatorId = personnelId,
                OperatorName = realName,
                OutBillNo = storageRecordInfo.TradeCode,
                OutDescription = storageRecordInfo.Description,
                StorageType = Convert.ToByte(storageRecordInfo.StorageType),
                SupplierName = companyName,
                WarehouseId = storageRecordInfo.WarehouseId,
                IsAfterSaleInferior = storageRecordInfo.StorageType == (Byte)StorageType.S && storageRecordInfo.StockType == (int)StorageRecordType.AfterSaleOut
            };

            var detaillist = new List<StorageRecordApplyOutDetailDTO>();
            var goodsInfos = _goodsCenterSao.GetDictRealGoodsUnitModel(storageRecordDetailInfos.Select(act => act.RealGoodsId).Distinct().ToList());
            foreach (var info in storageRecordDetailInfos)
            {
                var detailInfo = detaillist.FirstOrDefault(p => p.RealGoodsId == info.RealGoodsId);
                if (detailInfo != null)
                {
                    detailInfo.OutQuantity += info.Quantity;
                }
                else
                {
                    var storageRecordApplyOutDetailDto = new StorageRecordApplyOutDetailDTO
                    {
                        BatchNo = string.Empty,
                        GoodsCode = info.GoodsCode,
                        GoodsId = info.GoodsId,
                        GoodsName = info.GoodsName,
                        OutQuantity = info.Quantity,
                        RealGoodsId = info.RealGoodsId,
                        Sku = info.Specification,
                        Unit = goodsInfos.ContainsKey(info.RealGoodsId) ? goodsInfos[info.RealGoodsId].Units : string.Empty
                    };
                    detaillist.Add(storageRecordApplyOutDetailDto);
                }
            }
            storageRecordApplyOutDto.Details = detaillist;
            return storageRecordApplyOutDto;
        }

        #region [对订单添加销售出库单据]

        /// <summary>对订单添加销售出库单据
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="orderId">订单ID</param>
        /// <param name="errorMessage">异常信息</param>
        /// <returns></returns>
        public bool AddBySellStockOut(string orderNo, Guid orderId, out string errorMessage)
        {
            errorMessage = "";
            var orderInfo = _goodsOrder.GetGoodsOrder(orderId);
            if (orderInfo == null || orderInfo.OrderId == Guid.Empty)
            {
                errorMessage = "未找到对应订单信息";
                return false;
            }
            var orderDetailList = _goodsOrderDetail.GetGoodsOrderDetailByOrderId(orderId);
            if (orderInfo.HostingFilialeId == Guid.Empty)
            {
                orderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(orderInfo.DeliverWarehouseId, orderInfo.SaleFilialeId, orderDetailList.Select(ent => ent.GoodsType).Distinct());
            }
            Guid thirdCompanyId = Guid.Empty;
            if (orderInfo.SaleFilialeId != orderInfo.HostingFilialeId)
            {
                thirdCompanyId = _companyCussent.GetCompanyIdByRelevanceFilialeId(orderInfo.SaleFilialeId);
                if (thirdCompanyId == Guid.Empty)
                {
                    errorMessage = "对应的销售公司未关联往来单位";
                    return false;
                }
            }
            var storageRecordInfoList = CreateStorageRecordInfoBySellStockOutFromB2COrder(orderInfo, thirdCompanyId, orderDetailList);
            using (var tran = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    bool success = true;
                    foreach (var storageRecordInfo in storageRecordInfoList)
                    {
                        success = _storageDao.Insert(storageRecordInfo.Item1, storageRecordInfo.Item2, out errorMessage);
                        if (!success)
                        {
                            break;
                        }
                    }
                    if (success)
                    {
                        tran.Complete();
                        return true;
                    }
                    else
                    {
                        errorMessage = "关键字KEY：" + orderNo + "，没有获取相关的商品出入库记录\r\n" + errorMessage;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "关键字KEY：" + orderNo + "，没有获取相关的商品出入库记录\r\n" + ex.Message;
                    return false;
                }
            }
        }
        #endregion

        #region [创建出入库记录及明细]

        /// <summary>创建销售出库的出入库记录信息（来自B2C订单）
        /// </summary>
        /// <param name="orderInfo">订单</param>
        /// <param name="thirdCompanyId"></param>
        /// <param name="goodsOrderDetailList"></param>
        /// <returns></returns>
        private List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> CreateStorageRecordInfoBySellStockOutFromB2COrder(GoodsOrderInfo orderInfo, Guid thirdCompanyId, IList<GoodsOrderDetailInfo> goodsOrderDetailList)
        {
            List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> result = new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
            
            if (orderInfo.HostingFilialeId == Guid.Empty)
            {
                // 重新查找物流配送公司
                orderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(orderInfo.DeliverWarehouseId, orderInfo.SaleFilialeId, goodsOrderDetailList.Select(ent => ent.GoodsType).Distinct());
            }

            var stockQuantitys = _wmsSao.GetCurrentStockQuantity(orderInfo.DeliverWarehouseId, (Byte)orderInfo.StorageType, goodsOrderDetailList.Select(act => act.RealGoodsID).Distinct(), orderInfo.HostingFilialeId);
            var goodsInfoList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsOrderDetailList.Select(ent => ent.GoodsID).Distinct().ToList()).ToDictionary(k => k.GoodsId, v => v);
            var now = DateTime.Now;
            #region 销售公司的销售出库 By Jerry Bai 2017/4/24

            var billNo = WMSSao.GetOutGoodsBillNoByOrderNo(orderInfo.OrderNo);
            var storageRecordInfoForSellStockOutBySaleFiliale = new StorageRecordInfo
            {
                StockId = Guid.NewGuid(),
                FilialeId = orderInfo.SaleFilialeId,
                WarehouseId = orderInfo.DeliverWarehouseId,
                ThirdCompanyID = Guid.Empty,
                DateCreated = orderInfo.ConsignTime == DateTime.MinValue ? DateTime.Now : orderInfo.ConsignTime,
                AuditTime = now,
                Transactor = "系统操作",
                Description = EnumAttribute.GetKeyName((PaymentType)orderInfo.PayType) + "[网上销售][" + orderInfo.Consignee + "][订单完成出库]",
                StockType = (int)StorageRecordType.SellStockOut,
                StockState = (int)StorageRecordState.Finished,
                LinkTradeCode = orderInfo.OrderNo,
                LinkTradeID = orderInfo.OrderId,
                RelevanceFilialeId = orderInfo.SaleFilialeId,
                RelevanceWarehouseId = Guid.Empty,
                TradeCode = _codeManager.GetCode(CodeType.SL),
                StockValidation = false,
                LinkTradeType = (int)StorageRecordLinkTradeType.GoodsOrder,
                //IsOut = orderInfo.IsOut,
                StorageType = orderInfo.StorageType,
                BillNo = billNo,
                TradeBothPartiesType = (int)TradeBothPartiesType.Other
            };
            var storageRecordDetailListForSellStockOutBySaleFiliale = goodsOrderDetailList.Select(info => new StorageRecordDetailInfo
            {
                StockId = storageRecordInfoForSellStockOutBySaleFiliale.StockId,
                GoodsId = info.GoodsID,
                RealGoodsId = info.RealGoodsID,
                GoodsName = info.GoodsName,
                GoodsCode = info.GoodsCode,
                Specification = info.PurchaseSpecification,
                NonceWarehouseGoodsStock = 0,
                Quantity = (int)info.Quantity,// insert的时候会根据出入库类型确定保存正数还是负数
                UnitPrice = info.SellPrice,
                Description = "[网上销售]",
                JoinPrice = goodsInfoList.ContainsKey(info.GoodsID) && goodsInfoList[info.GoodsID].ExpandInfo != null ?
                    goodsInfoList[info.GoodsID].ExpandInfo.JoinPrice : 0,
                GoodsType = goodsInfoList.ContainsKey(info.GoodsID) ? goodsInfoList[info.GoodsID].GoodsType : 0
            }).ToList();
            result.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(storageRecordInfoForSellStockOutBySaleFiliale, storageRecordDetailListForSellStockOutBySaleFiliale));
            #endregion

            if (orderInfo.HostingFilialeId != orderInfo.SaleFilialeId)
            {
                // 销售公司，不是对应的物流配送公司，则额外新增物流配送公司销售给销售公司的出库单。销售公司向物流配送公司采购产生的采购单、入库单，在凌晨时生成前一天的单子，这个时候才会平掉销售公司的销售出库的单子
                #region 物流配送公司销售给销售公司的出库单 By Jerry Bai 2017/4/24
                var goodsIds = goodsOrderDetailList.Select(m => m.GoodsID).Distinct();
                var goodsIdTypeDict = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds.ToList()).ToDictionary(k => k.GoodsId, v => v.GoodsType);
                var purchasePriceDict = _internalPriceSetManager.GetInternalPurchasePriceByHostingFilialeIdGoodsIds(orderInfo.HostingFilialeId, goodsIdTypeDict);
                var storageRecordInfoForSellStockOutByHostingFiliale = new StorageRecordInfo
                {
                    StockId = Guid.NewGuid(),
                    FilialeId = orderInfo.HostingFilialeId,
                    WarehouseId = orderInfo.DeliverWarehouseId,
                    ThirdCompanyID = thirdCompanyId,
                    DateCreated = now,// 此时间用于在凌晨生成销售公司的采购单和采购入库单时，用于查询的时间点（必须确保后续产生的单据的时间不能早于之前单子的时间） By Jerry Bai 2017/04/27
                    AuditTime = now,
                    Transactor = "系统操作",
                    Description = "[内部采购][订单完成出库]",
                    StockType = (int)StorageRecordType.SellStockOut,
                    StockState = (int)StorageRecordState.Finished,
                    LinkTradeCode = orderInfo.OrderNo,
                    LinkTradeID = orderInfo.OrderId,
                    RelevanceFilialeId = Guid.Empty,
                    RelevanceWarehouseId = Guid.Empty,
                    TradeCode = _codeManager.GetCode(CodeType.SL),
                    StockValidation = false,
                    LinkTradeType = (int)StorageRecordLinkTradeType.GoodsOrder,
                    //IsOut = orderInfo.IsOut,
                    StorageType = orderInfo.StorageType,
                    BillNo = billNo,
                    TradeBothPartiesType = (int)TradeBothPartiesType.HostingToSale
                };
                var storageRecordDetailListForSellStockOutByHostingFiliale = goodsOrderDetailList.Select(info => new StorageRecordDetailInfo
                {
                    StockId = storageRecordInfoForSellStockOutByHostingFiliale.StockId,
                    GoodsId = info.GoodsID,
                    RealGoodsId = info.RealGoodsID,
                    GoodsName = info.GoodsName,
                    GoodsCode = info.GoodsCode,
                    Specification = info.PurchaseSpecification,
                    NonceWarehouseGoodsStock = stockQuantitys != null && stockQuantitys.ContainsKey(info.RealGoodsID) ? stockQuantitys[info.RealGoodsID] : 0,
                    Quantity = (int)info.Quantity,// insert的时候会根据出入库类型确定保存正数还是负数
                    UnitPrice = purchasePriceDict.ContainsKey(info.GoodsID) ? purchasePriceDict[info.GoodsID] : 0,
                    Description = "[内部采购]",
                    JoinPrice = goodsInfoList.ContainsKey(info.GoodsID) && goodsInfoList[info.GoodsID].ExpandInfo != null ?
                    goodsInfoList[info.GoodsID].ExpandInfo.JoinPrice : 0,
                    GoodsType = goodsInfoList.ContainsKey(info.GoodsID) ? goodsInfoList[info.GoodsID].GoodsType : 0
                }).ToList();
                result.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(storageRecordInfoForSellStockOutByHostingFiliale, storageRecordDetailListForSellStockOutByHostingFiliale));
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 审核销售出库单据（ERP销售出库） Add by Jerry Bai 2017/4/25 仅从页面剥离出来
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="companyName"></param>
        /// <param name="personnelId"></param>
        /// <param name="realName"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ApproveSellStockOut(Guid stockId, string companyName, Guid personnelId, string realName, out string errorMsg)
        {
            errorMsg = "";
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var storageRecordInfo = _storageDao.GetStorageRecord(stockId);
                    var storageRecordDetailList = _storageDao.GetStorageRecordDetailListByStockId(stockId);
                    if (storageRecordInfo == null
                        || storageRecordInfo.StockId == Guid.Empty
                        || storageRecordDetailList == null
                        || storageRecordDetailList.Count == 0)
                    {
                        return false;
                    }

                    String hostName = String.Empty;
                    var filialeInfo = FilialeManager.Get(storageRecordInfo.RelevanceFilialeId != Guid.Empty ? storageRecordInfo.RelevanceFilialeId : storageRecordInfo.ThirdCompanyID);
                    if (filialeInfo != null)
                    {
                        hostName = filialeInfo.Name;
                    }
                    var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string description = string.Format("[出库审核(审核人:{0};审核备注:{1});{2}]", realName, "核准", dateTime);
                    storageRecordInfo.Description = string.Format("{0}{1}", storageRecordInfo.Description, description);

                    // 设置销售出库状态为已审核
                    bool result = _storageDao.NewSetStateStorageRecord(stockId, StorageRecordState.Approved, description);
                    if (!result)
                    {
                        errorMsg = "更新出入库单据失败！";
                        return false;
                    }

                    //新增进货单据
                    string billNo, msg;
                    result = WMSSao.InsertOutGoodsBill(ConvertToWMSOutGoodsBill(storageRecordInfo, storageRecordDetailList, string.Format("{0}", hostName != String.Empty ? hostName : companyName), personnelId, realName), out billNo, out msg);
                    if (!result)
                    {
                        errorMsg = string.Format("核准失败！{0}",msg);
                        return false;
                    }
                    if (string.IsNullOrEmpty(billNo))
                    {
                        errorMsg = "仓储反馈出货单号为空(" + msg + ")！";
                        return false;
                    }
                    result = _storageDao.SetBillNo(stockId, billNo);
                    if (!result)
                    {
                        errorMsg = "出库单出货单号更新失败！";
                        return false;
                    }

                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    return false;
                }
                finally
                {
                    ts.Dispose();
                }
            }
            return true;
        }

        /// <summary>
        /// 审核销售退货入库单据（ERP销售退货入库） Add by Jerry Bai 2017/4/26 仅从页面剥离出来
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="thirdCompanyName"></param>
        /// <param name="personnelId"></param>
        /// <param name="realName"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ApproveSellReturnStockIn(Guid stockId, string thirdCompanyName, Guid personnelId, string realName, out string errorMsg)
        {
            errorMsg = "";
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string description = string.Format("[入库审核(审核人:{0};审核备注:{1});{2}]", realName, "核准", dateTime);
            //获得出入库记录
            var storageRecordInfo = _storageDao.GetStorageRecord(stockId);
            var storageRecordDetailList = _storageDao.GetStorageRecordDetailListByStockId(stockId);

            if (storageRecordInfo.StockState != (int)StorageRecordState.WaitAudit)
            {
                errorMsg = "当前单据状态已改变，审核失败！";
                return false;
            }

            Guid saleFilialeId=Guid.Empty;
            String hostName = String.Empty;
            if (storageRecordInfo.ThirdCompanyID==storageRecordInfo.RelevanceFilialeId)
            {
                var shopInfo = FilialeManager.Get(storageRecordInfo.ThirdCompanyID);
                saleFilialeId = shopInfo?.ParentId ?? Guid.Empty;
                hostName = shopInfo != null ? shopInfo.Name : string.Empty;
            }
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    // 设置销售退货入库状态为已审核
                    var result = _storageDao.NewSetStateStorageRecord(stockId, StorageRecordState.Approved,
                    description);
                    if (!result)
                    {
                        errorMsg = "审核失败";
                        return false;
                    }

                    //新增进货单据
                    string billNo;
                    var wmsResult = WMSSao.InsertInGoodsBill(ConvertToWMSInGoodsBill(storageRecordInfo, storageRecordDetailList,
                        string.Format("{0}", hostName != String.Empty ? hostName : thirdCompanyName), personnelId, realName, saleFilialeId), out billNo);
                    if (!wmsResult.IsSuccess)
                    {
                        errorMsg = string.Format("核准失败！{0}", wmsResult.Msg);
                        return false;
                    }
                    if (string.IsNullOrEmpty(billNo))
                    {
                        errorMsg = "仓储反馈进货单号为空！";
                        return false;
                    }
                    result = _storageDao.SetBillNo(stockId, billNo);
                    if (!result)
                    {
                        errorMsg = "入库单进货单号更新失败！";
                        return false;
                    }

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 核退出库单（来自WMS出货单的核退） Add by Jerry Bai 2017/4/25
        /// </summary>
        /// <param name="no"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public bool RejectStockOutFromWmsRejectOutGoods(string no, string description)
        {
            var storageRecord = _storageDao.GetStorageRecord(no);
            if (storageRecord == null || storageRecord.StockId == Guid.Empty)
            {
                return false;
            }
            if (storageRecord.StockType == (int)StorageRecordType.InnerPurchase)
                return _storageDao.RefuseInorOutGoodsBill(no, description);
            string errorMsg;
            int state = (int)StorageRecordState.Refuse;
            if (storageRecord.RelevanceFilialeId != Guid.Empty)
            {
                var filiale = FilialeManager.Get(storageRecord.RelevanceFilialeId);
                if (filiale != null && filiale.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop))
                {
                    state = (int)StorageRecordState.Canceled;
                }
            }
            return _storageDao.UpdateStorageState(no, state, description, out errorMsg);
        }

        #endregion

        #region  对订单退回商品检查添加已完成的销售退货入库单据

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="billNo">进货单据编号</param>
        /// <param name="outGoodsBillNo"></param>
        /// <param name="operatorName">操作人</param>
        /// <param name="returnDetails">退回明细</param>
        /// <param name="stockQuantitys">库存明细</param>
        /// <param name="reckoningInfos"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> AddBySellReturnIn(string orderNo, string billNo, string outGoodsBillNo, string operatorName,
            List<WMSReturnGoodsDetailRequest> returnDetails, Dictionary<Guid, int> stockQuantitys, out List<ReckoningInfo> reckoningInfos,
            out string errorMessage)
        {
            List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> resultDetail = new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
            var orderInfo = _goodsOrder.GetGoodsOrder(orderNo);
            reckoningInfos = new List<ReckoningInfo>();
            errorMessage = "";
            if (orderInfo == null || orderInfo.OrderId == Guid.Empty)
            {
                errorMessage = "未找到对应订单信息";
                return resultDetail;
            }

            var orderDetailList = _goodsOrderDetail.GetGoodsOrderDetailByOrderId(orderInfo.OrderId);
            if (orderDetailList == null || orderDetailList.Count == 0)
            {
                errorMessage = "未找到对应订单信息";
                return resultDetail;
            }

            if (orderInfo.HostingFilialeId == Guid.Empty)
            {
                // 之前订单表没有取到物流配送公司的，重新取一下
                orderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(orderInfo.DeliverWarehouseId, orderInfo.SaleFilialeId, orderDetailList.Select(ent => ent.GoodsType).Distinct());
            }

            var goodsIds = returnDetails.Select(ent => ent.GoodsId).Distinct();
            var orderDetails = orderDetailList.GroupBy(ent => ent.GoodsID).ToDictionary(k => k.Key, v => (v.Sum(act => act.TotalPrice) / v.Sum(act => (int)act.Quantity)));
            var realGoodsIds = returnDetails.Where(ent => ent.GoodsId != ent.RealGoodsId).Select(ent => ent.RealGoodsId).Distinct().ToList();
            var realGoodsInfos = realGoodsIds.Count > 0 ? _goodsInfoSao.GetDictRealGoodsUnitModel(realGoodsIds) ?? new Dictionary<Guid, RealGoodsUnitModel>() : new Dictionary<Guid, RealGoodsUnitModel>();
            var goodsInfoList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds.ToList()).ToDictionary(k => k.GoodsId, v => v);
            if (goodsInfoList.Count==0)
            {
                errorMessage = "GMS商品信息获取失败";
                return resultDetail;
            }

            if (orderInfo.HostingFilialeId != orderInfo.SaleFilialeId && orderInfo.HostingFilialeId!=Guid.Empty)
            {
                var saleFilialeCompany = _companyCussent.GetCompanyByRelevanceFilialeId(orderInfo.SaleFilialeId);
                if (saleFilialeCompany == null)
                {
                    errorMessage = "订单对应的销售公司未关联往来单位";
                    return resultDetail;
                }
                var hostFilialeCompany = _companyCussent.GetCompanyByRelevanceFilialeId(orderInfo.HostingFilialeId);
                if (hostFilialeCompany == null)
                {
                    errorMessage = "订单对应的物流公司未关联往来单位";
                    return resultDetail;
                }

                var description = EnumAttribute.GetKeyName((PaymentType)orderInfo.PayType);
                string message;
                //销售公司销售退货->客户 
                var saleFilialeToConsignee = CreateSellReturnInNew(orderInfo, Guid.Empty, 1, "", operatorName, description, "退回商品检查", (Byte)StorageType.S);
                var saleFilialeToConsigneeDetails = CreateStorageRecordDetailListBySellReturnIn(saleFilialeToConsignee, returnDetails, realGoodsInfos, goodsInfoList, new Dictionary<Guid, int>(), orderDetails, out message);
                if (saleFilialeToConsigneeDetails.Count == 0)
                    return resultDetail;

                var sellPriceDics = _storageDao.GetSellOutGoodsUnitPriceDic(orderInfo.HostingFilialeId, saleFilialeCompany.CompanyId, outGoodsBillNo);
                resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(saleFilialeToConsignee, saleFilialeToConsigneeDetails));
                if (sellPriceDics.Count != 0)
                {
                    //销售公司销售采购退货->物流公司
                    var saleFilialeToHostingFiliale = CreateSellReturnInNew(orderInfo, hostFilialeCompany.CompanyId, 2, "", operatorName, "", "退回商品检查", (Byte)StorageType.S, TradeBothPartiesType.HostingToSale);
                    var saleFilialeToHostingFilialeDetails = CreateStorageRecordDetailListBySellReturnIn(saleFilialeToHostingFiliale, returnDetails, realGoodsInfos, goodsInfoList, new Dictionary<Guid, int>(), sellPriceDics, out message);
                    
                    //物流公司销售退货-> 销售公司
                    var hostingFilialeToSaleFiliale = CreateSellReturnInNew(orderInfo, saleFilialeCompany.CompanyId, 3, billNo, operatorName, description, "退回商品检查", (Byte)StorageType.S, TradeBothPartiesType.HostingToSale);
                    var hostingFilialeToSaleFilialeDetails = CreateStorageRecordDetailListBySellReturnIn(hostingFilialeToSaleFiliale, returnDetails, realGoodsInfos, goodsInfoList, stockQuantitys, sellPriceDics, out message);


                    resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(saleFilialeToHostingFiliale, saleFilialeToHostingFilialeDetails));
                    resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(hostingFilialeToSaleFiliale, hostingFilialeToSaleFilialeDetails));
                    #region 销售公司对物流公司应收款  物流公司对销售公司应付款

                    var totalPrice = Math.Abs(Math.Round(saleFilialeToHostingFilialeDetails.Sum(ent => ent.UnitPrice * ent.Quantity), 2));
                    var hostingFilialeName = FilialeManager.GetName(orderInfo.HostingFilialeId);
                    var saleFilialeName = FilialeManager.GetName(orderInfo.SaleFilialeId);

                    var saleFilialeToHostingFilialeR = new ReckoningInfo
                    {
                        ContructType = ContructType.Insert,
                        ReckoningId = Guid.NewGuid(),
                        TradeCode = _codeManager.GetCode(CodeType.PY),
                        DateCreated = DateTime.Now,
                        ReckoningType = (int)ReckoningType.Income,
                        State = (int)ReckoningStateType.Currently,
                        IsChecked = (int)CheckType.NotCheck,
                        AuditingState = (int)AuditingState.Yes,
                        LinkTradeCode = saleFilialeToHostingFiliale.TradeCode,
                        WarehouseId = orderInfo.DeliverWarehouseId,
                        FilialeId = saleFilialeToHostingFiliale.FilialeId,
                        ThirdCompanyID = saleFilialeToHostingFiliale.ThirdCompanyID,
                        Description = string.Format("[退回商品检查,{0}对{1}采购退货应收款]", saleFilialeName, hostFilialeCompany.CompanyName),
                        AccountReceivable = totalPrice,
                        JoinTotalPrice = totalPrice,
                        ReckoningCheckType = (int)ReckoningCheckType.Other,
                        IsOut = orderInfo.IsOut,
                        LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                    };

                    var hostingFilialeToSaleFilialeR = new ReckoningInfo
                    {
                        ContructType = ContructType.Insert,
                        ReckoningId = Guid.NewGuid(),
                        TradeCode = _codeManager.GetCode(CodeType.PY),
                        DateCreated = DateTime.Now,
                        ReckoningType = (int)ReckoningType.Defray,
                        State = (int)ReckoningStateType.Currently,
                        IsChecked = (int)CheckType.NotCheck,
                        AuditingState = (int)AuditingState.Yes,
                        LinkTradeCode = hostingFilialeToSaleFiliale.TradeCode,
                        WarehouseId = orderInfo.DeliverWarehouseId,
                        FilialeId = hostingFilialeToSaleFiliale.FilialeId,
                        ThirdCompanyID = hostingFilialeToSaleFiliale.ThirdCompanyID,
                        Description = string.Format("[退回商品检查,{0}对{1}销售退货应付款]", hostingFilialeName, saleFilialeCompany.CompanyName),
                        AccountReceivable = -totalPrice,
                        JoinTotalPrice = -totalPrice,
                        ReckoningCheckType = (int)ReckoningCheckType.Other,
                        IsOut = orderInfo.IsOut,
                        LinkTradeType = (int)ReckoningLinkTradeType.StockIn,
                    };
                    reckoningInfos.Add(saleFilialeToHostingFilialeR);
                    reckoningInfos.Add(hostingFilialeToSaleFilialeR);
                    #endregion
                }
            }
            else
            {
                var storageRecordInfo = CreateStorageRecordInfoBySellReturnIn(orderInfo, billNo, operatorName);
                var storageRecordDetailList = CreateStorageRecordDetailListBySellReturnIn(storageRecordInfo, returnDetails, realGoodsInfos, goodsInfoList, stockQuantitys, orderDetails, out errorMessage);
                if (storageRecordDetailList.Count > 0)
                {
                    resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(storageRecordInfo, storageRecordDetailList));
                }
            }
            return resultDetail;
        }
        #endregion

        #region 创建销售退货入库记录及明细

        /// <summary>创建出入库记录信息
        /// </summary>
        /// <param name="orderInfo">订单</param>
        /// <param name="billNo"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        private StorageRecordInfo CreateStorageRecordInfoBySellReturnIn(GoodsOrderInfo orderInfo, string billNo, string operatorName)
        {
            var description = EnumAttribute.GetKeyName((PaymentType)orderInfo.PayType);
            return new StorageRecordInfo
            {
                StockId = Guid.NewGuid(),
                FilialeId = orderInfo.SaleFilialeId,
                WarehouseId = orderInfo.DeliverWarehouseId,
                ThirdCompanyID = Guid.Empty,
                DateCreated = orderInfo.ConsignTime == DateTime.MinValue ? DateTime.Now : orderInfo.ConsignTime,
                Transactor = operatorName,
                Description = description + "[退回商品检查][" + orderInfo.Consignee + "]",
                StockType = (int)StorageRecordType.SellReturnIn,
                StockState = (int)StorageRecordState.Finished,
                LinkTradeCode = orderInfo.OrderNo,
                LinkTradeID = orderInfo.OrderId,
                RelevanceFilialeId = Guid.Empty,
                RelevanceWarehouseId = Guid.Empty,
                TradeCode = _codeManager.GetCode(CodeType.SI),
                StockValidation = false,
                LinkTradeType = (int)StorageRecordLinkTradeType.GoodsOrder,
                //IsOut = orderInfo.IsOut,
                StorageType = (Int32)StorageType.S,
                BillNo = billNo
            };
        }

        /// <summary>创建出入库明细列表
        /// </summary>
        /// <param name="storageRecordInfo">出入库单据</param>
        /// <param name="realGoodsDics">商品信息</param>
        /// <param name="goodsDics"></param>
        /// <param name="stockQuantitys"></param>
        /// <param name="settlePrices">结算价</param>
        /// <param name="errorMessage">异常信息</param>
        /// <param name="returnDetails"></param>
        /// <returns></returns>
        private IList<StorageRecordDetailInfo> CreateStorageRecordDetailListBySellReturnIn(StorageRecordInfo storageRecordInfo,
            List<WMSReturnGoodsDetailRequest> returnDetails, Dictionary<Guid, RealGoodsUnitModel> realGoodsDics,
            Dictionary<Guid, GoodsInfo> goodsDics, Dictionary<Guid, int> stockQuantitys, IDictionary<Guid, decimal> settlePrices, out string errorMessage)
        {
            List<StorageRecordDetailInfo> dataSource = new List<StorageRecordDetailInfo>();
            foreach (var item in returnDetails)
            {
                var isChild = item.RealGoodsId != item.GoodsId;
                if (goodsDics == null || !goodsDics.ContainsKey(item.GoodsId))
                {
                    errorMessage = "未找到对应的主商品信息";
                    return new List<StorageRecordDetailInfo>();
                }
                if (isChild && !realGoodsDics.ContainsKey(item.RealGoodsId))
                {
                    errorMessage = "未找到对应的子商品信息";
                    return new List<StorageRecordDetailInfo>();
                }
                var realGoods = isChild ? realGoodsDics[item.RealGoodsId] : null;
                var goods = goodsDics[item.GoodsId];
                dataSource.Add(new StorageRecordDetailInfo
                {
                    StockId = storageRecordInfo.StockId,
                    GoodsId = item.GoodsId,
                    RealGoodsId = item.RealGoodsId,
                    GoodsName = goods.GoodsName,
                    GoodsCode = goods.GoodsCode,
                    Specification = realGoods != null ? realGoods.Specification : "",
                    NonceWarehouseGoodsStock = stockQuantitys != null && stockQuantitys.ContainsKey(item.RealGoodsId) ? stockQuantitys[item.RealGoodsId] : 0,
                    Quantity = Math.Abs(item.Quantity),
                    UnitPrice = settlePrices.ContainsKey(item.GoodsId) ? settlePrices[item.GoodsId] : 0,
                    Description = "[退回商品检查]",
                    BatchNo = item.BatchNo,
                    EffectiveDate = item.ExpiryDate,
                    ShelfType = item.ShelfType,
                    JoinPrice = goods.ExpandInfo != null ?
                    goods.ExpandInfo.JoinPrice : 0,
                    GoodsType = goods.GoodsType,
                    Units = goods.Units
                });
            }
            errorMessage = string.Empty;
            return dataSource;
        }

        private StorageRecordInfo CreateSellReturnInNew(GoodsOrderInfo orderInfo, Guid thirdCompanyId, int mode, string billNo, string operatorName, string description, string memo, byte storageType,
            TradeBothPartiesType tradeBothPartiesType = TradeBothPartiesType.Other)
        {
            Guid filialeId = orderInfo.SaleFilialeId;
            string remark;
            CodeType codeType = CodeType.SI; ;
            int stockType = (int)StorageRecordType.SellReturnIn;
            switch (mode)
            {
                case 1: //销售公司销售退货
                    remark = string.Format("{0}[{1}][{2}]", description, memo, orderInfo.Consignee);
                    break;
                case 2:
                    codeType = CodeType.SO;
                    stockType = (int)StorageRecordType.BuyStockOut;
                    remark = string.Format("[{0}][销售公司采购退货]", memo);
                    break;

                default:
                    filialeId = orderInfo.HostingFilialeId;
                    remark = string.Format("{0}[{1}][物流公司销售退货入库]", description, memo);
                    break;
            }
            return new StorageRecordInfo
            {
                StockId = Guid.NewGuid(),
                FilialeId = filialeId,
                WarehouseId = orderInfo.DeliverWarehouseId,
                ThirdCompanyID = thirdCompanyId,
                DateCreated = DateTime.Now,
                Transactor = operatorName,
                Description = remark,
                StockType = stockType,
                StockState = (int)StorageRecordState.Finished,
                LinkTradeCode = orderInfo.OrderNo,
                LinkTradeID = orderInfo.OrderId,
                RelevanceFilialeId = Guid.Empty,
                RelevanceWarehouseId = Guid.Empty,
                TradeCode = _codeManager.GetCode(codeType),
                StockValidation = false,
                LinkTradeType = (int)StorageRecordLinkTradeType.AfterSaleGoodsOrder,
                //IsOut = orderInfo.IsOut,
                StorageType = storageType,
                BillNo = billNo,
                TradeBothPartiesType = (int)tradeBothPartiesType
            };
        }

        /// <summary>
        /// 门店要货申请  销售公司对门店的销售出库单 
        /// </summary>
        /// <param name="storageRecordInfo"></param>
        /// <param name="joinPriceDics"></param>
        /// <param name="operatorName"></param>
        /// <param name="storageRecordDetailInfos"></param>
        /// <returns></returns>
        public List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> CreateShopSellOut(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailInfos, Guid filialeId, IDictionary<Guid, decimal> joinPriceDics, string operatorName)
        {
            List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> tuples = new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
            //销售公司销售给门店
            var storageRecord = new StorageRecordInfo
            {
                StockId = Guid.NewGuid(),
                FilialeId = filialeId,
                WarehouseId = storageRecordInfo.WarehouseId,
                ThirdCompanyID = storageRecordInfo.RelevanceFilialeId,
                DateCreated = DateTime.Now,
                Transactor = operatorName,
                Description = "门店要货申请",
                StockType = (int)StorageRecordType.SellStockOut,
                StockState = (int)StorageRecordState.Finished,
                LinkTradeCode = storageRecordInfo.LinkTradeCode,
                LinkTradeID = storageRecordInfo.LinkTradeID,
                RelevanceFilialeId = storageRecordInfo.RelevanceFilialeId,
                RelevanceWarehouseId = storageRecordInfo.RelevanceFilialeId,
                TradeCode = _codeManager.GetCode(CodeType.TSO),
                StockValidation = false,
                LinkTradeType = (int)StorageRecordLinkTradeType.Allot,
                //IsOut = storageRecordInfo.IsOut,
                StorageType = storageRecordInfo.StorageType,
                BillNo = storageRecordInfo.BillNo,
                TradeBothPartiesType = (int)TradeBothPartiesType.Other,

            };
            List<StorageRecordDetailInfo> sellDetails = storageRecordDetailInfos.Select(item => new StorageRecordDetailInfo
            {
                StockId = storageRecord.StockId,
                GoodsId = item.GoodsId,
                RealGoodsId = item.RealGoodsId,
                GoodsName = item.GoodsName,
                GoodsCode = item.GoodsCode,
                Specification = item.Specification,
                NonceWarehouseGoodsStock = 0,
                Quantity = Math.Abs(item.Quantity),
                UnitPrice = joinPriceDics.ContainsKey(item.GoodsId) ? joinPriceDics[item.GoodsId] : item.UnitPrice,
                Description = "[销售公司销售给门店]",
                BatchNo = item.BatchNo,
                EffectiveDate = item.EffectiveDate,
                ShelfType = item.ShelfType,
                JoinPrice = item.JoinPrice,
                GoodsType = item.GoodsType,
                Units = item.Units
            }).ToList();
            tuples.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(storageRecord, sellDetails));
            return tuples;
        }

        /// <summary>
        /// 门店退货、门店换货入库   
        /// </summary>
        /// <param name="storageRecordInfo">物流公司入库单据</param>
        /// <param name="thirdCompanyId"></param>
        /// <param name="joinPriceDics">加盟价</param>
        /// <param name="operatorName"></param>
        /// <param name="storageRecordDetailInfos"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> CreateShopReturnIn(StorageRecordInfo storageRecordInfo, IList<StorageRecordDetailInfo> storageRecordDetailInfos, Guid saleFilialeId, Guid thirdCompanyId, IDictionary<Guid, decimal> joinPriceDics, string operatorName)
        {
            List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> tuples = new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
            //销售公司销售给门店
            var storageRecord = new StorageRecordInfo
            {
                StockId = Guid.NewGuid(),
                FilialeId = saleFilialeId,
                WarehouseId = storageRecordInfo.WarehouseId,
                ThirdCompanyID = storageRecordInfo.RelevanceFilialeId,
                DateCreated = DateTime.Now,
                Transactor = operatorName,
                Description = "[门店退回商品检查]",
                StockType = (int)StorageRecordType.SellReturnIn,
                StockState = (int)StorageRecordState.Finished,
                LinkTradeCode = storageRecordInfo.LinkTradeCode,
                LinkTradeID = storageRecordInfo.LinkTradeID,
                RelevanceFilialeId = storageRecordInfo.RelevanceFilialeId,
                RelevanceWarehouseId = storageRecordInfo.RelevanceFilialeId,
                TradeCode = _codeManager.GetCode(CodeType.SI),
                StockValidation = false,
                LinkTradeType = (int)StorageRecordLinkTradeType.Allot,
                //IsOut = storageRecordInfo.IsOut,
                StorageType = storageRecordInfo.StorageType,
                BillNo = storageRecordInfo.BillNo,
                TradeBothPartiesType = (int)TradeBothPartiesType.Other
            };
            List<StorageRecordDetailInfo> sellDetails = storageRecordDetailInfos.Select(item => new StorageRecordDetailInfo
            {
                StockId = storageRecord.StockId,
                GoodsId = item.GoodsId,
                RealGoodsId = item.RealGoodsId,
                GoodsName = item.GoodsName,
                GoodsCode = item.GoodsCode,
                Specification = item.Specification,
                NonceWarehouseGoodsStock = 0,
                Quantity = Math.Abs(item.Quantity),
                UnitPrice = joinPriceDics.ContainsKey(item.GoodsId) ? joinPriceDics[item.GoodsId] : item.UnitPrice,
                Description = "[门店退回商品检查]",
                BatchNo = item.BatchNo,
                EffectiveDate = item.EffectiveDate,
                ShelfType = item.ShelfType,
                JoinPrice = item.JoinPrice,
                GoodsType = item.GoodsType,
                Units = item.Units
            }).ToList();
            tuples.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(storageRecord, sellDetails));

            var storageRecordIn = new StorageRecordInfo
            {
                StockId = Guid.NewGuid(),
                FilialeId = saleFilialeId,
                WarehouseId = storageRecordInfo.WarehouseId,
                ThirdCompanyID = thirdCompanyId,
                DateCreated = DateTime.Now,
                Transactor = operatorName,
                Description = "[门店退回商品检查]",
                StockType = (int)StorageRecordType.BuyStockOut,
                StockState = (int)StorageRecordState.Finished,
                LinkTradeCode = storageRecordInfo.LinkTradeCode,
                LinkTradeID = storageRecordInfo.LinkTradeID,
                RelevanceFilialeId = storageRecordInfo.RelevanceFilialeId,
                RelevanceWarehouseId = storageRecordInfo.RelevanceFilialeId,
                TradeCode = _codeManager.GetCode(CodeType.CC),
                StockValidation = false,
                LinkTradeType = (int)StorageRecordLinkTradeType.Allot,
                //IsOut = storageRecordInfo.IsOut,
                StorageType = storageRecordInfo.StorageType,
                BillNo = "",
                TradeBothPartiesType = (int)TradeBothPartiesType.HostingToSale
            };

            List<StorageRecordDetailInfo> buyDetails = storageRecordDetailInfos.Select(item => new StorageRecordDetailInfo
            {
                StockId = storageRecordIn.StockId,
                GoodsId = item.GoodsId,
                RealGoodsId = item.RealGoodsId,
                GoodsName = item.GoodsName,
                GoodsCode = item.GoodsCode,
                Specification = item.Specification,
                NonceWarehouseGoodsStock = 0,
                Quantity = Math.Abs(item.Quantity),
                UnitPrice = item.UnitPrice,
                Description = "[门店退回商品检查]",
                BatchNo = item.BatchNo,
                EffectiveDate = item.EffectiveDate,
                ShelfType = item.ShelfType,
                JoinPrice = item.JoinPrice,
                GoodsType = item.GoodsType,
                Units = item.Units
            }).ToList();

            tuples.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(storageRecordIn, buyDetails));

            return tuples;
        }
        #endregion

        #region WMS二期 丢件找回生成销售退货入库  快递退回销售公司  销售公司退物流公司
        //因为丢件找回坏件直接入的坏件储，所以就可能会生成一张进售后储、一张进坏件
        //而且次品、坏品是不需要记录往来账
        public List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> AddByLostBackReturnIn(WMSLostBackReturnDTO request, out List<ReckoningInfo> reckoningInfos,
            out string errorMessage)
        {
            List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>> resultDetail = new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
            var orderInfo = _goodsOrder.GetGoodsOrder(request.OrderNos.First());
            reckoningInfos = new List<ReckoningInfo>();
            if (orderInfo == null || orderInfo.OrderId == Guid.Empty)
            {
                errorMessage = "未找到对应订单信息";
                return resultDetail;
            }

            var orderDetailList = _goodsOrderDetail.GetGoodsOrderDetailByOrderId(orderInfo.OrderId);
            if (orderDetailList == null || orderDetailList.Count == 0)
            {
                errorMessage = "未找到对应订单信息";
                return resultDetail;
            }

            if (orderInfo.HostingFilialeId == Guid.Empty)
            {
                // 之前订单表没有取到物流配送公司的，重新取一下
                orderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(orderInfo.DeliverWarehouseId, orderInfo.SaleFilialeId, orderDetailList.Select(ent => ent.GoodsType).Distinct());
            }

            var details = request.Details.SelectMany(ent => ent.Details);
            var goodsIds = details.Select(ent => ent.GoodsId).Distinct();
            var realGoodsInfos = _goodsInfoSao.GetDictRealGoodsUnitModel(details.Select(ent => ent.RealGoodsId).Distinct().ToList()) ?? new Dictionary<Guid, RealGoodsUnitModel>();
            var goodsInfoList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIds.ToList()).ToDictionary(k => k.GoodsId, v => v);
            var description = EnumAttribute.GetKeyName((PaymentType)orderInfo.PayType);

            var sellAvgPrice = _storageDao.GetSellOutGoodsUnitPriceDic(orderInfo.SaleFilialeId, Guid.Empty, request.OutGoodsBillNo);
            if (sellAvgPrice.Count == 0)
            {
                errorMessage = "商品销售记录未找到";
                return resultDetail;
            }
            Dictionary<Guid, decimal> unitPriceDics = new Dictionary<Guid, decimal>();
            CompanyCussentInfo saleFilialeCompany = null;
            CompanyCussentInfo hostingCompany = null;
            bool isNewMode = false;
            if (orderInfo.HostingFilialeId != orderInfo.SaleFilialeId && orderInfo.HostingFilialeId!=Guid.Empty)
            {
                saleFilialeCompany = _companyCussent.GetCompanyByRelevanceFilialeId(orderInfo.SaleFilialeId);
                if (saleFilialeCompany == null)
                {
                    errorMessage = "订单对应的销售公司未关联往来单位";
                    return resultDetail;
                }
                hostingCompany = _companyCussent.GetCompanyByRelevanceFilialeId(orderInfo.HostingFilialeId);
                if (hostingCompany == null)
                {
                    errorMessage = "订单对应的物流配送公司未关联往来单位";
                    return resultDetail;
                }

                unitPriceDics = _storageDao.GetSellOutGoodsUnitPriceDic(orderInfo.HostingFilialeId, saleFilialeCompany.CompanyId, request.OutGoodsBillNo);
                isNewMode = unitPriceDics.Count != 0;
            }

            foreach (var lostBackDto in request.Details)
            {
                if (orderInfo.HostingFilialeId != orderInfo.SaleFilialeId && orderInfo.HostingFilialeId != Guid.Empty)
                {
                    //销售公司销售退货->客户
                    var saleFilialeToConsignee = CreateSellReturnInNew(orderInfo, Guid.Empty, 1, "", request.OperatorName, description, "丢件找回", lostBackDto.StorageType);
                    var saleFilialeToConsigneeDetails = CreateStorageRecordDetailListBySellReturnIn(saleFilialeToConsignee, lostBackDto.Details, realGoodsInfos, goodsInfoList, new Dictionary<Guid, int>(), sellAvgPrice, out errorMessage);
                    if (saleFilialeToConsigneeDetails.Count == 0)
                    {
                        return new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
                    }
                    resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(saleFilialeToConsignee, saleFilialeToConsigneeDetails));

                    if (isNewMode)
                    {
                        //销售公司销售采购退货->物流公司
                        var saleFilialeToHostingFiliale = CreateSellReturnInNew(orderInfo, hostingCompany.CompanyId, 2, "", request.OperatorName, "", "丢件找回", lostBackDto.StorageType, TradeBothPartiesType.HostingToSale);
                        var saleFilialeToHostingFilialeDetails = CreateStorageRecordDetailListBySellReturnIn(saleFilialeToHostingFiliale, lostBackDto.Details, realGoodsInfos, goodsInfoList, new Dictionary<Guid, int>(), unitPriceDics, out errorMessage);

                        //物流公司销售退货-> 销售公司
                        var hostingFilialeToSaleFiliale = CreateSellReturnInNew(orderInfo, saleFilialeCompany.CompanyId, 3, lostBackDto.BillNo, request.OperatorName, description, "丢件找回", lostBackDto.StorageType, TradeBothPartiesType.HostingToSale);
                        var hostingFilialeToSaleFilialeDetails = CreateStorageRecordDetailListBySellReturnIn(hostingFilialeToSaleFiliale, lostBackDto.Details, realGoodsInfos, goodsInfoList, lostBackDto.StockQuantitys, unitPriceDics, out errorMessage);


                        resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(saleFilialeToHostingFiliale, saleFilialeToHostingFilialeDetails));
                        resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(hostingFilialeToSaleFiliale, hostingFilialeToSaleFilialeDetails));

                        #region 销售公司对物流公司应收款  物流公司对销售公司应付款

                        var totalPrice = Math.Abs(Math.Round(saleFilialeToHostingFilialeDetails.Sum(ent => ent.UnitPrice * ent.Quantity), 2));
                        var hostingFilialeName = FilialeManager.GetName(orderInfo.HostingFilialeId);
                        var saleFilialeName = FilialeManager.GetName(orderInfo.SaleFilialeId);

                        var saleFilialeToHostingFilialeR = new ReckoningInfo
                        {
                            ContructType = ContructType.Insert,
                            ReckoningId = Guid.NewGuid(),
                            TradeCode = _codeManager.GetCode(CodeType.PY),
                            DateCreated = DateTime.Now,
                            ReckoningType = (int)ReckoningType.Income,
                            State = (int)ReckoningStateType.Currently,
                            IsChecked = (int)CheckType.NotCheck,
                            AuditingState = (int)AuditingState.Yes,
                            LinkTradeCode = saleFilialeToHostingFiliale.TradeCode,
                            WarehouseId = orderInfo.DeliverWarehouseId,
                            FilialeId = saleFilialeToHostingFiliale.FilialeId,
                            ThirdCompanyID = saleFilialeToHostingFiliale.ThirdCompanyID,
                            Description = string.Format("[丢件找回,{0}对{1}采购退货应收款]", saleFilialeName, hostingCompany.CompanyName),
                            AccountReceivable = totalPrice,
                            JoinTotalPrice = totalPrice,
                            ReckoningCheckType = (int)ReckoningCheckType.Other,
                            IsOut = orderInfo.IsOut,
                            LinkTradeType = (int)ReckoningLinkTradeType.StockOut,
                        };

                        var hostingFilialeToSaleFilialeR = new ReckoningInfo
                        {
                            ContructType = ContructType.Insert,
                            ReckoningId = Guid.NewGuid(),
                            TradeCode = _codeManager.GetCode(CodeType.PY),
                            DateCreated = DateTime.Now,
                            ReckoningType = (int)ReckoningType.Defray,
                            State = (int)ReckoningStateType.Currently,
                            IsChecked = (int)CheckType.NotCheck,
                            AuditingState = (int)AuditingState.Yes,
                            LinkTradeCode = hostingFilialeToSaleFiliale.TradeCode,
                            WarehouseId = orderInfo.DeliverWarehouseId,
                            FilialeId = hostingFilialeToSaleFiliale.FilialeId,
                            ThirdCompanyID = hostingFilialeToSaleFiliale.ThirdCompanyID,
                            Description = string.Format("[丢件找回,{0}对{1}销售退货应付款]", hostingFilialeName, saleFilialeCompany.CompanyName),
                            AccountReceivable = -totalPrice,
                            JoinTotalPrice = -totalPrice,
                            ReckoningCheckType = (int)ReckoningCheckType.Other,
                            IsOut = orderInfo.IsOut,
                            LinkTradeType = (int)ReckoningLinkTradeType.StockIn,
                        };
                        reckoningInfos.Add(saleFilialeToHostingFilialeR);
                        reckoningInfos.Add(hostingFilialeToSaleFilialeR);
                        #endregion
                    }
                }
                else
                {
                    //销售公司销售退货->客户
                    if (orderInfo.HostingFilialeId == Guid.Empty)
                        orderInfo.HostingFilialeId = orderInfo.SaleFilialeId;
                    var saleFilialeToConsignee = CreateSellReturnInNew(orderInfo, Guid.Empty, 1, lostBackDto.BillNo, request.OperatorName, description, "丢件找回", lostBackDto.StorageType);
                    var saleFilialeToConsigneeDetails = CreateStorageRecordDetailListBySellReturnIn(saleFilialeToConsignee, lostBackDto.Details, realGoodsInfos, goodsInfoList, lostBackDto.StockQuantitys, sellAvgPrice, out errorMessage);
                    if (saleFilialeToConsigneeDetails.Count == 0)
                        return new List<Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>>();
                    resultDetail.Add(new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(saleFilialeToConsignee, saleFilialeToConsigneeDetails));
                }
            }
            errorMessage = "";
            return resultDetail;
        }
        #endregion
    }
}
