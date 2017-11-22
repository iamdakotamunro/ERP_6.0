using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.FinanceModule
{
    /// <summary>
    /// 按月归档的商品即时结算价
    /// </summary>
    public class GoodsGrossSettlementByMonthInfo
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 单价（结算价）
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 统计月份，格式：yyyy-MM
        /// </summary>
        public string StatisticMonth { get; set; }
    }
}
