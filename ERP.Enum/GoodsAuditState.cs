using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品状态
    /// </summary>
    [Serializable]
    public enum GoodsAuditState
    {
        /// <summary>
        /// 审核未通过
        /// </summary>
        [Enum("审核未通过")]
        NoPass = 0,
        /// <summary>
        /// 采购经理审核
        /// </summary>
        [Enum("采购经理审核")]
        PurchasingWaitAudit = 1,
        /// <summary>
        /// 质管部审核
        /// </summary>
        [Enum("质管部审核")]
        QualityWaitAudit = 2,

        /// <summary>
        /// 负责人终审
        /// </summary>
        [Enum("负责人终审")]
        CaptainWaitAudit = 3,

        /// <summary>
        /// 审核通过
        /// </summary>
        [Enum("审核通过")]
        Pass = 4
    }
}
