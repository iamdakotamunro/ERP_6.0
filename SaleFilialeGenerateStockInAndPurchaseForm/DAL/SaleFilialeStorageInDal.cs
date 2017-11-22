using SaleFilialeGenerateStockInAndPurchaseForm.Model;
using System;
using System.Collections.Generic;
using ERP.Enum;

namespace SaleFilialeGenerateStockInAndPurchaseForm.DAL
{
    internal class SaleFilialeStorageInDal
    {
        /// <summary>
        /// 按指定统计日期，获取销售公司、仓库、储位的列表
        /// </summary>
        /// <param name="calculateDate"></param>
        /// <returns></returns>
        public static IEnumerable<SaleFilialeWarehouseStorageTypeInfo> GetSaleFilialeThirdCompanyWarehouseStorageTypeList(DateTime calculateDate)
        {
            DateTime startDate = calculateDate.Date;
            DateTime endDate = calculateDate.Date.AddDays(1).AddSeconds(-1);
            const string SQL = @"
SELECT	FilialeId HostingFilialeId
		,ThirdCompanyID SaleFilialeThirdCompanyID
		,WarehouseId
		,StorageType
FROM StorageRecord with(nolock)
where StockType=@StockType and LinkTradeType=@LinkTradeType and StockState=@StockState and TradeBothPartiesType=@TradeBothPartiesType and DateCreated between @CalculateStartDate and @CalculateEndDate
and FilialeId<>'00000000-0000-0000-0000-000000000000'
group by FilialeId,ThirdCompanyID,WarehouseId,StorageType
";

            using (var db = DbFactory.Create())
            {
                return db.Run(SQL)
                    .AddParameter("StockType", (int)StorageRecordType.SellStockOut)
                    .AddParameter("LinkTradeType", (int)StorageRecordLinkTradeType.GoodsOrder)
                    .AddParameter("StockState", (int)StorageRecordState.Finished)
                    .AddParameter("TradeBothPartiesType", (int)TradeBothPartiesType.HostingToSale)
                    .AddParameter("CalculateStartDate", startDate)
                    .AddParameter("CalculateEndDate", endDate)
                    .Select<SaleFilialeWarehouseStorageTypeInfo>();
            }
        }

        /// <summary>
        /// 按指定统计日期、销售公司、仓库、储位，获取待生成的采购入库的明细列表
        /// </summary>
        /// <param name="calculateDate"></param>
        /// <param name="saleFilialeThirdCompanyID">销售公司对应的往来单位</param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="storageType"></param>
        /// <returns></returns>
        public static IEnumerable<SaleFilialeStockInDetail> GetSaleFilialeStockInDetailList(DateTime calculateDate, Guid saleFilialeThirdCompanyID, Guid hostingFilialeId, Guid warehouseId, int storageType)
        {
            DateTime startDate = calculateDate.Date;
            DateTime endDate = calculateDate.Date.AddDays(1).AddSeconds(-1);
            const string SQL = @"
select t1.GoodsId,t1.RealGoodsId,t1.GoodsCode,max(t1.GoodsName) GoodsName,max(t1.Specification) Specification,sum(-t1.Quantity) Quantity,sum(-t1.Quantity*t1.UnitPrice) Amount
from StorageRecordDetail t1 with(nolock)
inner join StorageRecord t2 with(nolock) on t2.StockId=t1.StockId
where t2.StockType=@StockType and t2.LinkTradeType=@LinkTradeType and t2.StockState=@StockState and t2.TradeBothPartiesType=@TradeBothPartiesType and DateCreated between @CalculateStartDate and @CalculateEndDate
    and t2.ThirdCompanyID=@SaleFilialeThirdCompanyID and t2.FilialeId=@HostingFilialeId and t2.WarehouseId=@WarehouseId and t2.StorageType=@StorageType
group by t1.GoodsId,t1.RealGoodsId,t1.GoodsCode
";

            using (var db = DbFactory.Create())
            {
                return db.Run(SQL)
                    .AddParameter("StockType", (int)StorageRecordType.SellStockOut)
                    .AddParameter("LinkTradeType", (int)StorageRecordLinkTradeType.GoodsOrder)
                    .AddParameter("StockState", (int)StorageRecordState.Finished)
                    .AddParameter("TradeBothPartiesType", (int)TradeBothPartiesType.HostingToSale)
                    .AddParameter("CalculateStartDate", startDate)
                    .AddParameter("CalculateEndDate", endDate)
                    .AddParameter("SaleFilialeThirdCompanyID", saleFilialeThirdCompanyID)
                    .AddParameter("HostingFilialeId", hostingFilialeId)
                    .AddParameter("WarehouseId", warehouseId)
                    .AddParameter("StorageType", storageType)
                    .Select<SaleFilialeStockInDetail>();
            }
        }

        /// <summary>
        /// 检查是否已经生成了凌晨的那笔单据
        /// </summary>
        /// <param name="calculateDate"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="hostingFilialeThirdCompanyID">物流配送公司对应的往来单位ID</param>
        /// <param name="warehouseId"></param>
        /// <param name="storageType"></param>
        /// <returns></returns>
        public static bool IsGenerated(DateTime calculateDate, Guid saleFilialeId, Guid hostingFilialeThirdCompanyID, Guid warehouseId, int storageType)
        {
            DateTime startDate = calculateDate.Date;
            DateTime endDate = calculateDate.Date.AddDays(1).AddSeconds(-1);
            const string SQL = @"
SELECT	top 1 1
FROM StorageRecord with(nolock)
where StockType=@StockType and LinkTradeType=@LinkTradeType and StockState=@StockState and TradeBothPartiesType=@TradeBothPartiesType and DateCreated between @CalculateStartDate and @CalculateEndDate
    and ThirdCompanyID=@HostingFilialeThirdCompanyID and FilialeId=@SaleFilialeId and WarehouseId=@WarehouseId and StorageType=@StorageType
";

            using (var db = DbFactory.Create())
            {
                return db.Run(SQL)
                    .AddParameter("StockType", (int)StorageRecordType.BuyStockIn)
                    .AddParameter("LinkTradeType", (int)StorageRecordLinkTradeType.Purchasing)
                    .AddParameter("StockState", (int)StorageRecordState.Finished)
                    .AddParameter("TradeBothPartiesType", (int)TradeBothPartiesType.HostingToSale)
                    .AddParameter("CalculateStartDate", startDate)
                    .AddParameter("CalculateEndDate", endDate)
                    .AddParameter("SaleFilialeId", saleFilialeId)
                    .AddParameter("HostingFilialeThirdCompanyID", hostingFilialeThirdCompanyID)
                    .AddParameter("WarehouseId", warehouseId)
                    .AddParameter("StorageType", storageType)
                    .GetValue<int>() == 1;
            }
        }
    }
}
