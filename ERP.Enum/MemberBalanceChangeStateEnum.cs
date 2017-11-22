using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>会员余额管理操作状态枚举
    /// </summary>
    [Serializable]
    public enum MemberBalanceChangeStateEnum
    {
        /// <summary>待审核
        /// </summary>
        [Enum("待审核")]
        WaitAuditing = 1,

        /// <summary>待确认
        /// </summary>
        [Enum("待确认")]
        WaitAffirm = 2,

        /// <summary>不通过
        /// </summary>
        [Enum("不通过")]
        NoPass = 3,

        /// <summary>完成
        /// </summary>
        [Enum("完成")]
        Complete = 4,

        /// <summary>取消
        /// </summary>
        [Enum("取消")]
        Cancel = 5,
    }
}
