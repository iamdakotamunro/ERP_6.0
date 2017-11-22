using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 活动报备状态
    /// </summary>
    public enum ActivityFilingState
    {
        /// <summary>
        /// 运营申报
        /// </summary>
        [Enum("运营申报")]
        OperateFiling=1,
        /// <summary>
        /// 采购申报
        /// </summary>
        [Enum("采购申报")]
        PurchaseFiling = 2,
        /// <summary>
        /// 申报通过
        /// </summary>
        [Enum("申报通过")]
        FilingPass = 3,
        /// <summary>
        /// 申报失败
        /// </summary>
        [Enum("申报失败")]
        FilingFail = 4,
        /// <summary>
        /// 删除
        /// </summary>
        [Enum("已删除")]
        FilingDelete = 5,
    }
}
