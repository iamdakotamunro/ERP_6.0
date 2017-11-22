using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 供应商资质完整类型
    /// </summary>
    [Serializable]
    public enum SupplierCompleteType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = 0,
        
        /// <summary>
        /// 不完整
        /// </summary>
        [Enum("不完整")]
        NoComplete = 1,

        /// <summary>
        /// 完整
        /// </summary>
        [Enum("完整")]
        Complete = 2,
    }
}
