using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 词汇管理状态
    /// </summary>
    public enum VocabularyState
    {
        /// <summary>
        /// 禁用
        /// </summary>
        [Enum("禁用")]
        Disable = 0,

        /// <summary>
        /// 启用
        /// </summary>
        [Enum("启用")]
        Enable = 1
    }
}
