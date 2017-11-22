using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 订单状态组枚举
    /// </summary>
    [Serializable]
    public enum OrderOtherState
    {
        ///// <summary>
        ///// 所有有效订单,订单状态小于等于Consignmented的订单
        ///// </summary>
        //[EnumArrtibute("所有有效订单,订单状态小于等于Consignmented的订单")]
        //All = -1,
        ///// <summary>
        ///// 未经受理,订单状态为UnVerify
        ///// </summary>
        //[EnumArrtibute("未经受理,订单状态为UnVerify")]
        //UnApproved = 0,
        ///// <summary>
        ///// 已受理,订单状态大于UnVerify并小于Consignmented
        ///// </summary>
        //[EnumArrtibute("已受理,订单状态大于UnVerify并小于Consignmented")]
        //Approved = 1,
        ///// <summary>
        ///// 未完成,订单状态小于Consignmented
        ///// </summary>
        //[EnumArrtibute("未完成,订单状态小于Consignmented")]
        //UnConsignmented = 2,
        ///// <summary>
        ///// 已完成,订单状态等于Consignmented
        ///// </summary>
        //[EnumArrtibute("已完成,订单状态等于Consignmented")]
        //Consignmented = 3,
        ///// <summary>
        ///// 有效订单 待打印的订单
        ///// </summary>
        //[EnumArrtibute("有效订单 待打印的订单")]
        //Effective = 4
        
        /// <summary>
        /// 
        /// </summary>
        [Enum("订单分拣")]
        Sorting=100,

        /// <summary>
        /// 
        /// </summary>
        [Enum("订单打包")]
        Pack=200,

        /// <summary>
        /// 
        /// </summary>
        [Enum("订单匹配")]
        Matching=300
    }
}
