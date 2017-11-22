using System;
using ERP.Enum;


namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 订单分拣记录模型
    /// </summary>
    [Serializable]
    public class PickingOrderInfo
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public Guid GoodsOrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 分拣人id
        /// </summary>
        public Guid CreateMemberId { get; set; }

        /// <summary>
        /// 分拣人
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 分拣时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 分拣状态
        /// </summary>
        public PickingState OrderState { get; set; }
    }
}
