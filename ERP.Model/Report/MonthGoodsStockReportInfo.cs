using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 库存金额显示模型
    /// </summary>
    [Serializable]
    public class MonthGoodsStockReportInfo
    {
        /// <summary>
        /// 商品类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 1份月库存总金额
        /// </summary>
        public decimal January { get; set; }

        /// <summary>
        /// 2份月库存总金额
        /// </summary>
        public decimal February { get; set; }

        /// <summary>
        /// 3份月库存总金额
        /// </summary>
        public decimal March { get; set; }

        /// <summary>
        /// 4份月库存总金额
        /// </summary>
        public decimal April { get; set; }

        /// <summary>
        /// 5份月库存总金额
        /// </summary>
        public decimal May { get; set; }

        /// <summary>
        /// 6份月库存总金额
        /// </summary>
        public decimal June { get; set; }

        /// <summary>
        /// 7份月库存总金额
        /// </summary>
        public decimal July { get; set; }

        /// <summary>
        /// 8份月库存总金额
        /// </summary>
        public decimal August { get; set; }

        /// <summary>
        /// 9份月库存总金额
        /// </summary>
        public decimal September { get; set; }

        /// <summary>
        /// 10月份库存总金额
        /// </summary>
        public decimal October { get; set; }

        /// <summary>
        /// 11月份库存总金额
        /// </summary>
        public decimal November { get; set; }

        /// <summary>
        /// 12月份库存总金额
        /// </summary>
        public decimal December { get; set; }
    }
}
