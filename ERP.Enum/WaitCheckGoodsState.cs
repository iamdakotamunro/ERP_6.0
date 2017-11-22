using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 创建人：刘彩军
    /// 创建时间：2011-June-29th
    /// 文件描述：待盘点商品状态枚举
    /// </summary>
    [Serializable]
    public enum WaitCheckGoodsState
    {
        /// <summary>
        ///  0 待盘点
        /// </summary>
        [Enum("待盘点")]
        WaitCheck = 0,
        /// <summary>
        /// 1 已盘点
        /// </summary>
        [Enum("已盘点")]
        Checked = 1
    }
}
