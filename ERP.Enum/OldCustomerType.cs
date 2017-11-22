using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 新老会员类型
    /// </summary>
    [Serializable]
    public enum OldCustomerType
    {
        /// <summary>
        /// 新用户,生成地址记录
        /// </summary>
        [Enum("新用户,生成地址记录")]
        NewCustomer = 0,
        /// <summary>
        /// 老用户,没有任何修改
        /// </summary>
        [Enum("老用户,没有任何修改")]
        OldCustomer = 1,

        /// <summary> 非注册购买用户
        /// </summary>
        [Enum("非注册购买用户")]
        NonRegCustomer = 2,
    }
}
