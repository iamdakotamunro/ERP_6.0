using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class GoodsDaySalesStatistics : IGoodsDaySalesStatistics
    {
        public GoodsDaySalesStatistics(Environment.GlobalConfig.DB.FromType fromType)
        {

        }


        public List<KeyAndValue> GetRealGoodsDaySales(Guid warehouseId, Guid hostingFilialeId, DateTime startTime, DateTime endTime,IEnumerable<Guid> realGoodsIds)
        {
            string realGoodsStr =realGoodsIds.Aggregate("", (current, goodsId) => current + string.Format("{0}'{1}'", current.Length == 0 ? "" : ",", goodsId));
            const string SQL = @"SELECT RealGoodsID AS [Key],Sum(GoodsSales) AS Value 
  FROM [lmshop_GoodsDaySalesStatistics] 
  where DeliverWarehouseId=@WarehouseId AND SaleFilialeId=@HostingFilialeId 
  and DayTime between @StartTime and @EndTime
  and RealGoodsId in({0})
  group by RealGoodsID";
            var sqlParams = new[]
            {
                new Parameter("@WarehouseId", warehouseId),
                new Parameter("@StartTime", startTime),
                new Parameter("@EndTime", endTime),
                new Parameter("@HostingFilialeId", hostingFilialeId)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<KeyAndValue>(true, string.Format(SQL, realGoodsStr),sqlParams).ToList();
            }
        }
    }
}
