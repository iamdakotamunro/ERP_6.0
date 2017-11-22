using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 订单配货实体类
    /// </summary>
    [Serializable]
    public class PickInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public PickInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pickNo">配货号</param>
        /// <param name="orderId">订单ID</param>
        /// <param name="creationTime">创建时间</param>
        /// <param name="pickPersonnelId">配货人员ID</param>
        public PickInfo(string pickNo, Guid orderId, DateTime creationTime, Guid pickPersonnelId)
        {
            PickNo = pickNo;
            OrderId = orderId;
            CreationTime = creationTime;
            PickPersonnelId = pickPersonnelId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pickNo">配货号</param>
        /// <param name="orderId">订单ID</param>
        /// <param name="creationTime">创建时间</param>
        /// <param name="orderNo">序号</param>
        public PickInfo(string pickNo, Guid orderId, DateTime creationTime, string orderNo)
        {
            PickNo = pickNo;
            OrderId = orderId;
            CreationTime = creationTime;
            OrderNo = orderNo;
        }

        /// <summary>
        /// 配货号
        /// </summary>
        public string PickNo { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 是否配货
        /// </summary>
        public bool IsPick { get; set; }

        /// <summary>
        /// 配货时间
        /// </summary>
        public DateTime PickTime { get; set; }

        /// <summary>
        /// 是否配货异常
        /// </summary>
        public bool IsPickException { get; set; }

        /// <summary>
        /// 配货人员ID
        /// </summary>
        public Guid PickPersonnelId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public String OrderNo { get; set; }
    }
}
