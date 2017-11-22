using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 会员提现状态枚举
    /// Add by liucaijun at 2011-October-21th
    /// </summary>
    [Serializable]
    public enum MemberMentionState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All=-1,
        /// <summary>
        /// 待打款
        /// </summary>
        [Enum("待打款")]
        Process = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        Auditing = 1,
        /// <summary>
        /// 完成
        /// </summary>
        [Enum("完成")]
        Finish = 2,
        /// <summary>
        /// 提现失败,取消该状态，暂时不用了
        /// </summary>
        [Enum("提现失败")]
        NoPass = 3,

        /// <summary>
        /// 退回
        /// </summary>
        [Enum("退回")]
        SendBack=4,

        /// <summary>
        /// 作废
        /// </summary>
        [Enum("作废")]
        Invalid=5
    }
}
