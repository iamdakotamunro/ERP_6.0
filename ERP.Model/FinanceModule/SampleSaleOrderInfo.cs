using ERP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.FinanceModule
{
    /// <summary>
    /// 简单销售订单信息，作为记录销售订单关联的结算价的原始数据
    /// </summary>
    [Serializable]
    public class SampleSaleOrderInfo
    {
        /// <summary>
        /// 单据类型
        /// </summary>
        public SaleOrderGoodsSettlementRelatedTradeType TradeType { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 主商品列表
        /// </summary>
        public IList<Guid> GoodsIds { get; set; }

        /// <summary>
        /// 产生的时间
        /// </summary>
        public DateTime OccurTime { get; set; }
    }
}
