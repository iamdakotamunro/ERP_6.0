using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 支付类型枚举
    /// modify by liangcanren at 2015-05-04
    /// </summary>
    [Serializable]
    public enum PaymentType
    {
        /// <summary>
		/// 未设置
		/// </summary>
        [Enum("未设置")]
		NoSet = -1,

        /// <summary>
        /// 在线支付
        /// </summary>
        [Enum("在线支付")]
        OnLine = 0,

        /// <summary>
        /// 传统帐号
        /// </summary>
        [Enum("传统帐号")]
        Tradition = 1,

        /// <summary>
        /// 刷卡帐号
        /// </summary>
        [Enum("刷卡帐号")]
        SwipeCard = 2,

        ///// <summary>
        ///// 邮局汇款
        ///// </summary>
        //[Enum("邮局汇款")]
        //Post = 0,

        ///// <summary>
        ///// 现金
        ///// </summary>
        //[Enum("现金")]
        //Cash = 1,

        ///// <summary>
        ///// 网络支付
        ///// </summary>
        //[Enum("网络支付")]
        //Internet = 2,

        ///// <summary>
        ///// 银行汇款
        ///// </summary>
        //[Enum("银行汇款")]
        //Remittance = 3,

        ///// <summary>
        ///// 电汇
        ///// </summary>
        //[Enum("电汇")]
        //TMO = 4,

        ///// <summary>
        ///// 其他支付类型
        ///// </summary>
        //[Enum("其他支付类型")]
        //Other = 5,

        ///// <summary>
        ///// 信用卡
        ///// </summary>
        //[Enum("信用卡")]
        //CreditCard=6
    }
}
