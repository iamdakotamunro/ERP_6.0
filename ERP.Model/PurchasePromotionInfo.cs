using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 采购促销明细
    /// </summary>
    [Serializable]
    public class PurchasePromotionInfo
    {
        /// <summary>
        /// 采购促销明细
        /// </summary>
        public PurchasePromotionInfo()
        {
        }

        /// <summary>
        /// 促销ID
        /// </summary>
        public Guid PromotionId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 现返/非现返
        /// </summary>
        public int PromotionType { get; set; }

        /// <summary>
        /// 买几赠几/促销信息
        /// </summary>
        public int PromotionKind { get; set; }

        /// <summary>
        /// 买几个
        /// </summary>
        public int BuyCount { get; set; }

        /// <summary>
        /// 送几个
        /// </summary>
        public int GivingCount { get; set; }

        /// <summary>
        /// 促销信息
        /// </summary>
        public string PromotionInfo { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// true 为单光度,false 总数量赠送
        /// </summary>
        public bool IsSingle { get; set; }

        public Guid WarehouseId { get; set; }

        public Guid HostingFilialeId { get; set; }
    }
}
