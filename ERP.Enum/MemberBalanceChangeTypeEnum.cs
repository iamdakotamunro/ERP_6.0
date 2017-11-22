using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>会员余额管理操作类型枚举
    /// </summary>
    [Serializable]
    public enum MemberBalanceChangeTypeEnum
    {
        /// <summary>余额扣除
        /// </summary>
        [Enum("余额扣除")]
        BalanceSubtract = -1,

        /// <summary>余额赠送
        /// </summary>
        [Enum("余额赠送")]
        BalancePresent = 1,

        /// <summary>余额充值
        /// </summary>
        [Enum("余额充值")]
        BalanceRecharge = 3,
    }
}
