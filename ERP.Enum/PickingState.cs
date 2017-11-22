using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 订单分拣状态枚举
    /// </summary>
    [Serializable]
    public enum PickingState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [EnumMember]
        [Enum("全部")]
        All=-1,

        /// <summary>
        /// 待确认
        /// </summary>
        [EnumMember]
        [Enum("待确认")]
        Waitting=0,

        /// <summary>
        /// 确认
        /// </summary>
        [EnumMember]
        [Enum("确认")]
        Passing=1,

        /// <summary>
        /// 删除
        /// </summary>
        [EnumMember]
        [Enum("删除")]
        Deleted=2
    }
}
