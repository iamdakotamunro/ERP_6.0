using System;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CompleteOrderTaskDetailsInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 销售公司ID
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台ID
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// ERP是否执行成功
        /// </summary>
        public bool IsSuccessERP { get; set; }

        /// <summary>
        /// B2C是否执行成功
        /// </summary>
        public bool IsSuccessB2C { get; set; }

        /// <summary>
        /// 库存中心是否执行成功
        /// </summary>
        public bool IsSuccessStock { get; set; }

        /// <summary>
        /// 会员中心是否执行成功
        /// </summary>
        public bool IsSuccessMember { get; set; }

        /// <summary>
        /// 促销中心是否执行成功
        /// </summary>
        public bool IsSuccessPromotion { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsAllComplete { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
