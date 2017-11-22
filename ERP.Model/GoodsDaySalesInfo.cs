using System;
using System.Security.AccessControl;

namespace ERP.Model
{
    /// <summary>
    /// 商品日销量模型  for 日均销量
    /// </summary>
    [Serializable]
    public class GoodsDaySalesInfo
    {
        public Guid RealGoodsID { get; set; }

        public Guid WarehouseId { get; set; }

        public Guid HostingFilialeId { get; set; }

        public DateTime DayTime { get; set; }

        public int GoodsSales { get; set; }

        public string Specification { get; set; }
    }
}
