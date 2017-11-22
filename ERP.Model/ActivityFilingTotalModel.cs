using System;

namespace ERP.Model
{
    /// <summary>
    /// 活动报备统计类
    /// </summary>
    [Serializable]
    public class ActivityFilingTotalModel
    {
        /// <summary>
        /// 预估销量
        /// </summary>
        public decimal TotalProspectSaleNumber { get; set; }

        /// <summary>
        /// 正常销量
        /// </summary>
        public decimal TotalNormalSaleNumber { get; set; }

        /// <summary>
        /// 实际销量
        /// </summary>
        public decimal TotalActualSaleNumber { get; set; }
    }
}
