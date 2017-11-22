using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsExtendInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 参考价
        /// </summary>
        public decimal ReferencePrice { get; set; }

        /// <summary>
        /// 加盟价
        /// </summary>
        public decimal JoinPrice { get; set; }

        /// <summary>
        /// 隐性成本
        /// </summary>
        public decimal ImplicitCost { get; set; }

        /// <summary>
        /// 年终扣率
        /// </summary>
        public decimal YearDiscount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PackCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ApprovalTime { get; set; }

        /// <summary>
        /// 是否统计绩效 存货周转率统计使用 2015-05-06  陈重文
        /// </summary>
        public bool IsStatisticalPerformance { get; set; }
    }
}
