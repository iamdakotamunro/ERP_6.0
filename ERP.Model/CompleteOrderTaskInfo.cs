using System;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CompleteOrderTaskInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 快递ID
        /// </summary>
        public Guid ExpressId { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid OperationId { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string Operationer { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 失败数量
        /// </summary>
        public int FailureQuantity { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 订单任务状态
        /// </summary>
        public int TaskState { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 等待处理数量
        /// </summary>
        public int WaitQuantity { get; set; }

        public int ProcessedQuantity
        {
            get
            {
                return TotalQuantity - WaitQuantity;
            }
        }
    }
}
