using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 退回支付金额流水
    /// </summary>
    [Serializable]
    public class DrawBackInfo
    {
        /// <summary>
        /// 退回No
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 支付号
        /// </summary>
        public string PaiNo { get; set; }

        /// <summary>
        /// 退回金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 退回原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 管理意见
        /// </summary>
        public string Clew { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 退回次数
        /// </summary>
        public int ApplyTimes { get; set; }

        /// <summary>
        /// 最后退回时间
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// 会员账号
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }

    }
}
