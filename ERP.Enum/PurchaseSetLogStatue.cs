using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品采购设置变更值状态
    /// </summary>
    [Serializable]
    public enum PurchaseSetLogStatue
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        NotAudit = 0,

        /// <summary>
        /// 通过
        /// </summary>
        [Enum("通过")]
        Pass = 1,

        /// <summary>
        /// 不通过
        /// </summary>
        [Enum("不通过")]
        NotPass = 2

    }
}
