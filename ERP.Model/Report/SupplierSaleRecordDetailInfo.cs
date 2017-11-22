using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 供应商月销售记录明细(数据模型)
    /// </summary>
    [Serializable]
    public class SupplierSaleRecordDetailInfo
    {
        public Guid ID { get; set; }

        public Guid RecordID { get; set; }

        public Guid CompanyID { get; set; }

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
