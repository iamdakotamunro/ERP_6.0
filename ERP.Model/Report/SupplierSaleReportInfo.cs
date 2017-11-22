using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 供应商销售额报表模型(展示模型)
    /// </summary>
    [Serializable]
    public class SupplierSaleReportInfo
    {
        /// <summary>
        /// 公司ID/往来单位ID
        /// </summary>
        public Guid CompanyID { get; set; }

        /// <summary>
        /// 公司名称/往来单位名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 1份月销量总金额
        /// </summary>
        public decimal January { get; set; }

        /// <summary>
        /// 2份月销量总金额
        /// </summary>
        public decimal February { get; set; }

        /// <summary>
        /// 3份月销量总金额
        /// </summary>
        public decimal March { get; set; }

        /// <summary>
        /// 4份月销量总金额
        /// </summary>
        public decimal April { get; set; }

        /// <summary>
        /// 5份月销量总金额
        /// </summary>
        public decimal May { get; set; }

        /// <summary>
        /// 6份月销量总金额
        /// </summary>
        public decimal June { get; set; }

        /// <summary>
        /// 7份月销量总金额
        /// </summary>
        public decimal July { get; set; }

        /// <summary>
        /// 8份月销量总金额
        /// </summary>
        public decimal August { get; set; }

        /// <summary>
        /// 9份月销量总金额
        /// </summary>
        public decimal September { get; set; }

        /// <summary>
        /// 10月份销量总金额
        /// </summary>
        public decimal October { get; set; }

        /// <summary>
        /// 11月份销量总金额
        /// </summary>
        public decimal November { get; set; }

        /// <summary>
        /// 12月份销量总金额
        /// </summary>
        public decimal December { get; set; }
    }
}
