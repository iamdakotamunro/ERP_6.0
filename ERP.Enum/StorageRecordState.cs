using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>出入库单据状态（新版对应WMS）
    /// </summary>
    public enum StorageRecordState
    {
        /// <summary>待审核
        /// </summary>
        [Enum("待审核")]
        WaitAudit = 1,

        /// <summary>核退
        /// </summary>
        [Enum("核退")]
        Refuse = 2,

        /// <summary>核准
        /// </summary>
        [Enum("核准")]
        Approved = 3,

        /// <summary>完成
        /// </summary>
        [Enum("完成")]
        Finished = 4,

        /// <summary>作废
        /// </summary>
        [Enum("作废")]
        Canceled = 5
    }
}
