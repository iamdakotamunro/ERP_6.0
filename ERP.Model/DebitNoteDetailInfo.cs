using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 借记单明细
    /// </summary>
    [Serializable]
    public class DebitNoteDetailInfo
    {
        /// <summary>
        /// 借记单明细
        /// </summary>
        public DebitNoteDetailInfo()
        {
        }

        /// <summary>
        /// 采购单ID
        /// </summary>
        public Guid PurchasingId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 赠送数量
        /// </summary>
        public int GivingCount { get; set; }

        /// <summary>
        /// 实到数量
        /// </summary>
        public int ArrivalCount { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 借记单明细ID
        /// </summary>
        public Guid Id { get; set; }
    }
}
