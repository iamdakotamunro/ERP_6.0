using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Report
{
    /// <summary>
    /// 公司毛利明细
    /// </summary>
    /// zal 2016-06-17
    [Serializable]
    public class CompanyGrossProfitRecordDetailInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 库存或者订单ID
        /// </summary>
        public Guid StockAndOrderId { get; set; }

        /// <summary>
        /// 库存或者订单编号
        /// </summary>
        public string StockAndOrderNo { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal SalesAmount { get; set; }

        /// <summary>
        /// 商品金额
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        ///运费收入 
        /// </summary>
        public decimal ShipmentIncome { get; set; }

        /// <summary>
        /// 促销抵扣
        /// </summary>
        public decimal PromotionsDeductible { get; set; }

        /// <summary>
        /// 积分抵扣
        /// </summary>
        public decimal PointsDeduction { get; set; }

        /// <summary>
        /// 运费成本
        /// </summary>
        public decimal ShipmentCost { get; set; }

        /// <summary>
        /// 进货成本
        /// </summary>
        public decimal PurchaseCosts { get; set; }
        
        /// <summary>
        /// 交易佣金(第三方佣金)
        /// </summary>
        public decimal CatCommission { get; set; }

        /// <summary>
        /// 订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 记录年月份
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 状态(0:未处理；1:已处理)
        /// </summary>
        public int State { get; set; }

        #region 只用于显示，不对应数据库字段
        /// <summary>
        /// 发货公司
        /// </summary>
        public Guid DeliverFilialeId { get; set; }

        /// <summary>
        /// 发货仓库
        /// </summary>
        public Guid DeliverWarehouseId { get; set; }
        #endregion
    }
}
