using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IGoodsDaySalesStatistics
    {
        /// <summary>
        /// 获取商品在指定时间段内的销量
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        List<KeyAndValue> GetRealGoodsDaySales(Guid warehouseId, Guid hostingFilialeId, DateTime startTime, DateTime endTime, IEnumerable<Guid> realGoodsIds);
    }
}
