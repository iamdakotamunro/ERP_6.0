using System.Runtime.Serialization;

namespace ERP.Enum
{
    /// <summary>
    /// 数据操作方式枚举
    /// </summary>
    [DataContract]
    public enum ContructType
    {
        /// <summary>
        /// 插入
        /// </summary>
        [EnumMember]
        Insert,

        /// <summary>
        /// 更新
        /// </summary>
        [EnumMember]
        Update,

        /// <summary>
        /// 删除
        /// </summary>
        [EnumMember]
        Delete,

        /// <summary>
        /// 选择
        /// </summary>
        [EnumMember]
        Select,

        /// <summary>
        /// 默认
        /// </summary>
        [EnumMember]
        Default

    }
}
