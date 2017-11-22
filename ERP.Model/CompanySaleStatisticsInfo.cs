using System;

namespace ERP.Model
{
    /// <summary>
    /// 供应商销量统计模型
    /// </summary>
    [Serializable]
    public class CompanySaleStatisticsInfo
    {
        public Guid GoodsId { get; set; }

        public Guid SaleFilialeId { get; set; }

        public int Month { get; set; }

        public int Quantity { get; set; }

        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; }
    }
}
