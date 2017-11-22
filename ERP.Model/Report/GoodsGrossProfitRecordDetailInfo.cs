using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Report
{
    /// <summary>
    /// 商品毛利明细
    /// zal 2016-07-04
    /// </summary>
    [Serializable]
    public class GoodsGrossProfitRecordDetailInfo
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 商品总数
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        ///销售总额 
        /// </summary>
        public decimal SalesPriceTotal { get; set; }

        /// <summary>
        /// 进货成本总额
        /// </summary>
        public decimal PurchaseCostTotal { get; set; }

        /// <summary>
        /// 订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)
        /// </summary>
        /// zal 2016-05-18
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
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 满减金额
        /// </summary>
        public decimal PromotionValue { get; set; }
        /// <summary>
        /// 发货公司
        /// </summary>
        public Guid DeliverFilialeId { get; set; }

        /// <summary>
        /// 发货仓库
        /// </summary>
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }
        #endregion
    }
}
