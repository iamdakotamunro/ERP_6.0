using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 搜索时间枚举
    /// </summary>
    public enum SearchTime
    {
        /// <summary>
        /// 一个月之内
        /// </summary>
        [Enum("一个月之内")]
        OneMonth = 0,

        /// <summary>
        /// 三个月之内
        /// </summary>
        [Enum("三个月之内")]
        ThreeMonth = 1,

        /// <summary>
        /// 半年内
        /// </summary>
        [Enum("半年内")]
        HalfYear = 2,

        /// <summary>
        /// 一年内
        /// </summary>
        [Enum("一年内")]
        OneYear = 3,

        /// <summary>
        /// 当前所有数据
        /// </summary>
        [Enum("当前所有数据")]
        LocalAll = 4,

        /// <summary>
        /// 历史所有数据
        /// </summary>
        [Enum("历史所有数据")]
        HistoryAll = 5
    }
}
