using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 记账本类型枚举
    /// </summary>
    [Serializable]
    [DataContract]
    public enum WasteBookType
    {
        /// <summary>收入
        /// </summary>
        [EnumMember]
        [Enum("收入")]
        Increase = 0,

        /// <summary>支出
        /// </summary>
        [EnumMember]
        [Enum("支出")]
        Decrease = 1,

        ///// <summary>
        ///// 转入资金
        ///// </summary>
        //[EnumMember] 
        //[Enum("转入资金")]
        //TransferIn = 2,

        ///// <summary>
        ///// 转出资金
        ///// </summary>
        //[EnumMember] 
        //[Enum("转出资金")]
        //TransferOut = 3,

        ///// <summary>
        ///// 转出手续费
        ///// </summary>
        //[EnumMember] 
        //[Enum("转出手续费")]
        //TransferFee = 4,


        ////以下两条用在WasteTypeInfo.cs中的WasteBookType属性

        ///// <summary>
        ///// 转出资金有手续费
        ///// </summary>
        //[EnumMember] 
        //[Enum("转出资金有手续费")]
        //TransferOutFee = 5,

        ///// <summary>
        ///// 转出资金无手续费
        ///// </summary>
        //[EnumMember] 
        //[Enum("转出资金无手续费")]
        //TransferOutNoFee = 6
    }
}
