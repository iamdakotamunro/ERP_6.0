using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 往来帐说明类型
    /// </summary>
    public enum MemoType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        NoSet = 0,

        /// <summary>
        /// 折扣说明
        /// </summary>
        [Enum("折扣说明")]
        Discount = 1,

        /// <summary>
        /// 差额说明
        /// </summary>
        [Enum("差额说明")]
        Subject = 2
    }
}
