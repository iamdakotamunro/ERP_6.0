using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 资金流类型
    /// </summary>
    [Serializable]
    public enum WasteSource
    {
        /// <summary>
        /// 天猫、京东、第三方交易佣金
        /// </summary>
        [Enum("天猫、京东、第三方交易佣金")]
        TradingCommissions =1,

        /// <summary>
        /// 积分代扣
        /// </summary>
        [Enum("积分代扣")]
        PointsWithholding = 2,

        /// <summary>
        /// 订单交易金额
        /// </summary>
        [Enum("订单交易金额")]
        OrderBusinessMoney = 3
    }
}
