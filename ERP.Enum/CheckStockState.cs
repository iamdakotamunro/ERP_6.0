using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 创建人：刘彩军
    /// 创建时间：2011-June-28th
    /// 文件描述：库存盘点计划状态枚举
    /// </summary>
    [Serializable]
    public enum CheckStockState
    {
        /// <summary>
        ///  0 待批准
        /// </summary>
        [Enum("待批准")]
        WaitAuditing = 0,
        /// <summary>
        /// 1 待初盘
        /// </summary>
        [Enum("待初盘")]
        WaitFirstCount = 1,
        /// <summary>
        /// 2 待复盘
        /// </summary>
        [Enum("待复盘")]
        WaitSecondCount = 2,
        /// <summary>
        /// 3 待审核
        /// </summary>
        [Enum("待审核")]
        WaitSecondCheck = 3,
        /// <summary>
        /// 4 待纠正
        /// </summary>
        [Enum("待纠正")]
        WaitToCorrect = 4,
        /// <summary>
        /// 5 完成
        /// </summary>
        [Enum("完成")]
        Finish = 5
    }
}
