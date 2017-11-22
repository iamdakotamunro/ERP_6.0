
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum CheckDataState
    {
        /// <summary>
        /// 所有
        /// </summary>
        [Enum("所有")]
        All = -1,

        /// <summary>
        /// 上传完毕
        /// </summary>
        [Enum("已上传")]
        Loaded = 0,
       
        /// <summary>
        /// 已录入
        /// </summary>
        [Enum("已录入")]
        Entered = 1,

        /// <summary>
        /// 已对比
        /// </summary>
        [Enum("已对比")]
        Contrasted = 2,

        /// <summary>
        /// 已确认
        /// </summary>
        [Enum("已确认")]
        Confirmed = 3,

        /// <summary>
        /// 处理中
        /// </summary>
        [Enum("处理中")]
        Inhanding = 4,

        /// <summary>
        /// 更新插入
        /// </summary>
        [Enum("更新插入")]
        UpdateInsert = 5,

        /// <summary>
        /// 批量转移
        /// </summary>
        [Enum("批量转移")]
        Transfer = 6,

        /// <summary>
        /// 完成
        /// </summary>
        [Enum("完成")]
        Finished = 7,

        /// <summary>
        /// 已对账
        /// </summary>
        [Enum("已对账")]
        Checked = 8,

        /// <summary>
        /// 删除
        /// </summary>
        [Enum("删除")]
        Deleted = 99
    }
}
