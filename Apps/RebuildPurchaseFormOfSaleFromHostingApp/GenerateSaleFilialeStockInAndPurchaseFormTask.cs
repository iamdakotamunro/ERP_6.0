using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.BLL.Interface;
using ERP.BLL.Implement;
using System;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using ERP.Enum;
using Keede.Ecsoft.Model;
using ERP.Model;
using RebuildPurchaseFormOfSaleFromHostingApp.DAL;
using System.Configuration;
using System.Threading;

namespace RebuildPurchaseFormOfSaleFromHostingApp
{
    /// <summary>
    /// 销售公司每天凌晨生成采购单及入库单（来自B2C的订单）
    /// </summary>
    public class GenerateSaleFilialeStockInAndPurchaseFormTask
    {
        private static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        private static readonly StorageManager _storageManager = StorageManager.WriteInstance;
        private static readonly PurchasingManager _purchasingManager = PurchasingManager.WriteInstance;
        private static readonly CodeManager _codeManager = new CodeManager();
        private static readonly IRealTimeGrossSettlementManager _realTimeGrossSettlementManager = RealTimeGrossSettlementManager.WriteInstance;


        public static void Generate()
        {
            var dateList = ConfigurationManager.AppSettings["RebuildDateList"].Split(',').Distinct().ToList();
            foreach(var date in dateList)
            {
                var calculateDate = DateTime.Parse(date).Date;
                var saleFilialeInfoList = SaleFilialeStorageInDal.GetSaleFilialeThirdCompanyWarehouseStorageTypeList(calculateDate).ToList();
                if (saleFilialeInfoList.Count > 0)
                {
                    foreach (var saleFilialeInfo in saleFilialeInfoList)
                    {
                        try
                        {
                            GenerateSaleFilialeStockInAndPurchasing(calculateDate, saleFilialeInfo.SaleFilialeThirdCompanyID, saleFilialeInfo.HostingFilialeId, saleFilialeInfo.WarehouseId, saleFilialeInfo.StorageType);
                        }
                        catch { }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 按指定计算日期、销售公司、物流配送公司、仓库、储位，生成销售公司的采购单和采购入库单
        /// </summary>
        /// <param name="calculateDate"></param>
        /// <param name="saleFilialeThirdCompanyId">销售公司往来单位Id</param>
        /// <param name="hostingFilialeId">物流公司ID</param>
        /// <param name="warehouseId"></param>
        /// <param name="storageType"></param>
        internal static void GenerateSaleFilialeStockInAndPurchasing(DateTime calculateDate, Guid saleFilialeThirdCompanyId, Guid hostingFilialeId, Guid warehouseId, int storageType)
        {
            var hostingFilialeThirdCompany=_companyCussent.GetCompanyByRelevanceFilialeId(hostingFilialeId); //根据物流公司获取往来单位
            if (hostingFilialeThirdCompany == null)
                return;
            var saleFilialeId = _companyCussent.GetRelevanceFilialeIdByCompanyId(saleFilialeThirdCompanyId);  //根据往来单位获取公司
            if (saleFilialeId == Guid.Empty)
                return;
            if (saleFilialeId == hostingFilialeId)
                return;

            bool isGenerated = SaleFilialeStorageInDal.IsGenerated(calculateDate, saleFilialeId, hostingFilialeThirdCompany.CompanyId, warehouseId, storageType);
            if (!isGenerated)
            {
                var simpleStockRecordDetails = SaleFilialeStorageInDal.GetSaleFilialeStockInDetailList(calculateDate, saleFilialeThirdCompanyId, hostingFilialeId, warehouseId, storageType);
                if (!simpleStockRecordDetails.Any())
                {
                    return;
                }

                // 不重新获取采购价，直接取对应的物流配送公司给销售公司的的销售出库单里计算即可得到
                var purchasePriceDict = (from item in simpleStockRecordDetails.GroupBy(g => g.GoodsId)
                                         select new KeyValuePair<Guid, decimal>(item.Key, item.Sum(s => s.Amount) / item.Sum(s => s.Quantity)))
                                        .ToDictionary(kv => kv.Key, kv => kv.Value);
                //销售公司采购单
                var purchasingRecord = new PurchasingInfo
                {
                    PurchasingID = Guid.NewGuid(),
                    PurchasingNo = _codeManager.GetCode(CodeType.PH),
                    CompanyID = hostingFilialeThirdCompany.CompanyId,
                    CompanyName = hostingFilialeThirdCompany.CompanyName,
                    FilialeID = saleFilialeId,
                    WarehouseID = warehouseId,
                    PurchasingState = (int)PurchasingState.AllComplete,
                    PurchasingType = (int)PurchasingType.AutoInternal,
                    StartTime = calculateDate,
                    EndTime = DateTime.Now,
                    Description = "[内部采购]",
                    PmId = Guid.Empty,
                    PmName = string.Empty,
                    Director = "系统",
                    PersonResponsible = Guid.Empty,
                    ArrivalTime = calculateDate.Date,
                    PurchasingFilialeId = saleFilialeId
                };
                var purchasingRecordDetailList = simpleStockRecordDetails.Select(m => new PurchasingDetailInfo
                {
                    PurchasingGoodsID = Guid.NewGuid(),
                    PurchasingID = purchasingRecord.PurchasingID,
                    GoodsID = m.RealGoodsId,
                    GoodsName = m.GoodsName,
                    GoodsCode = m.GoodsCode,
                    Specification = m.Specification,
                    CompanyID = hostingFilialeThirdCompany.CompanyId,
                    Price = purchasePriceDict.ContainsKey(m.GoodsId) ? purchasePriceDict[m.GoodsId] : 0,
                    PlanQuantity = m.Quantity,
                    PurchasingGoodsType = (int)PurchasingGoodsType.NoGift,
                    RealityQuantity = m.Quantity,
                    State = 1,
                    Description = "[内部采购]",
                    Units = string.Empty
                }).ToList();

                //销售公司采购进货入库单
                var stockRecord = new StorageRecordInfo
                {
                    StockId = Guid.NewGuid(),
                    TradeCode = _codeManager.GetCode(CodeType.RK),
                    FilialeId = saleFilialeId,
                    WarehouseId = warehouseId,
                    ThirdCompanyID = hostingFilialeThirdCompany.CompanyId,
                    DateCreated = calculateDate.Date,
                    Transactor = "系统操作",
                    Description = "[内部采购]",
                    LogisticsCode = string.Empty,
                    StockType = (int)StorageRecordType.BuyStockIn,
                    StockState = (int)StorageRecordState.Finished,
                    LinkTradeType = (int)StorageRecordLinkTradeType.Purchasing,
                    LinkTradeCode = purchasingRecord.PurchasingNo,
                    LinkTradeID = purchasingRecord.PurchasingID,
                    RelevanceFilialeId = Guid.Empty,
                    RelevanceWarehouseId = Guid.Empty,
                    StockValidation = false,
                    StorageType = storageType,
                    SubtotalQuantity = 0,
                    AccountReceivable = 0,
                    TradeBothPartiesType = (int)TradeBothPartiesType.HostingToSale
                };
                var stockRecordDetailList = simpleStockRecordDetails.Select(m => new StorageRecordDetailInfo
                {
                    StockId = stockRecord.StockId,
                    GoodsId = m.GoodsId,
                    Specification = m.Specification,
                    Quantity = m.Quantity,
                    UnitPrice = purchasePriceDict.ContainsKey(m.GoodsId) ? purchasePriceDict[m.GoodsId] : 0,
                    GoodsName = m.GoodsName,
                    GoodsCode = m.GoodsCode,
                    RealGoodsId = m.RealGoodsId,
                    Description = "[内部采购]",
                    NonceWarehouseGoodsStock = 0,
                    Units = string.Empty,
                    ShelfType = 1
                }).ToList();

                using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
                {
                    try
                    {
                        var result = _purchasingManager.Insert(purchasingRecord, purchasingRecordDetailList);
                        if (!result)
                        {
                            LogError(string.Format("生成采购单失败！计算日期：{0}，销售公司：{1}，物流配送公司：{2}，仓库：{3}，储位：{4}", calculateDate, saleFilialeId, hostingFilialeId, warehouseId, storageType));
                            return;
                        }
                        result = _storageManager.AddStorageRecord(stockRecord, stockRecordDetailList);
                        if (!result)
                        {
                            LogError(string.Format("生成采购入库单失败！计算日期：{0}，销售公司：{1}，物流配送公司：{2}，仓库：{3}，储位：{4}", calculateDate, saleFilialeId, hostingFilialeId, warehouseId, storageType));
                            return;
                        }

                        // 计算即时结算价
                        var items =_realTimeGrossSettlementManager.CreateByPurchaseStockIn(stockRecord.StockId, calculateDate).ToList();
                        items.ForEach(m => _realTimeGrossSettlementManager.Calculate(m));

                        tran.Complete();
                        LogInfo(string.Format("生成成功！计算日期：{0}，销售公司：{1}，物流配送公司：{2}，仓库：{3}，储位：{4}", calculateDate, saleFilialeId, hostingFilialeId, warehouseId, storageType));
                    }
                    catch (Exception ex)
                    {
                        LogError(string.Format("生成采购单和采购入库单失败！计算日期：{0}，销售公司：{1}，物流配送公司：{2}，仓库：{3}，储位：{4}", calculateDate, saleFilialeId, hostingFilialeId, warehouseId, storageType), ex);
                        return;
                    }
                }
            }
        }

        static void LogInfo(string message)
        {
            ERP.SAL.LogCenter.LogService.LogInfo(message, "每日凌晨生成销售公司的采购单及入库单", null);
        }

        static void LogError(string message)
        {
            ERP.SAL.LogCenter.LogService.LogError(message, "每日凌晨生成销售公司的采购单及入库单", null);
        }

        static void LogError(string message, Exception ex)
        {
            ERP.SAL.LogCenter.LogService.LogError(message, "每日凌晨生成销售公司的采购单及入库单", ex);
        }
    }
}
