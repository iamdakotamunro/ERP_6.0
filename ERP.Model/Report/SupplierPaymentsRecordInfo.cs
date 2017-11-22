using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 供应商应付款记录数据模型
    /// </summary>
    [Serializable]
    public class SupplierPaymentsRecordInfo
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 账期
        /// </summary>
        public int PaymentDays { get; set; }

        /// <summary>
        /// 记录月份
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 应付款总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 应付款(未付)
        /// </summary>
        public decimal TotalNoPayed { get; set; }

    }
}
