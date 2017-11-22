using System;

namespace ERP.Model
{
    /// <summary>
    /// 员工操作日志
    /// </summary>
    [Serializable]
    public class OperationLogInfo
    {
        /// <summary>
        /// 操作日志ID
        /// </summary>
        public Guid LogId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid OperatorId { get; set; }

        /// <summary>
        /// 操作者名称
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作点类型ID
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// 操作类型描述
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 操作点ID
        /// </summary>
        public Guid PointId { get; set; }

        /// <summary>
        /// 操作点描述
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 操作对象的标识(最好是【GUID】字段，如订单ID，配镜单ID等。)
        /// </summary>
        public String IdentifyKey { get; set; }

        /// <summary>
        /// 关键词(主要用于商品相关操作)
        /// </summary>
        public string IdentifyCode { get; set; }

        /// <summary>
        /// 是否是手动记录
        /// </summary>
        public bool IsHand { get; set; }

        /// <summary>
        /// 工作记录
        /// </summary>
        public int Workload { get; set; }
    }
}
