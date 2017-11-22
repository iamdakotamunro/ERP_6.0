using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 往来账对账类型
    /// </summary>
    [Serializable]
    public enum ReckoningCheckType
    {
        /// <summary> 运费对账
        /// </summary>
        [Enum("运费对账")]
        Carriage = 1,

        /// <summary>代收对账
        /// </summary>
        [Enum("代收对账")]
        Collection = 2,

        /// <summary>供应商对账
        /// </summary>
        [Enum("供应商对账")]
        Supplier = 3,

        /// <summary> 其他
        /// </summary>
        [Enum("其他")]
        Other = 4
    }
}
