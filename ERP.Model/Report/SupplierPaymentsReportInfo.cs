using System;
namespace ERP.Model.Report
{
    /// <summary>
    /// 应付款查询和采购入库报表显示模型
    /// </summary>
    [Serializable]
    public class SupplierPaymentsReportInfo
    {
        public Guid FilialeId { get; set; }

        public string FilialeName { get; set; }

        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; }

        public int PaymentDays { get; set; }

        /// <summary>
        /// 1份月总金额
        /// </summary>
        public decimal January { get; set; }

        /// <summary>
        /// 2份月总金额
        /// </summary>
        public decimal February { get; set; }

        /// <summary>
        /// 3份月总金额
        /// </summary>
        public decimal March { get; set; }

        /// <summary>
        /// 4份月总金额
        /// </summary>
        public decimal April { get; set; }

        /// <summary>
        /// 5份月总金额
        /// </summary>
        public decimal May { get; set; }

        /// <summary>
        /// 6份月总金额
        /// </summary>
        public decimal June { get; set; }

        /// <summary>
        /// 7份月总金额
        /// </summary>
        public decimal July { get; set; }

        /// <summary>
        /// 8份月总金额
        /// </summary>
        public decimal August { get; set; }

        /// <summary>
        /// 9份月总金额
        /// </summary>
        public decimal September { get; set; }

        /// <summary>
        /// 10月份总金额
        /// </summary>
        public decimal October { get; set; }

        /// <summary>
        /// 11月份总金额
        /// </summary>
        public decimal November { get; set; }

        /// <summary>
        /// 12月份总金额
        /// </summary>
        public decimal December { get; set; }

        /// <summary>
        /// 年总金额汇总
        /// </summary>
        public decimal YearTotalAmount
        {
            get
            {
                return January + February + March + April + May + June + July + August + September + October + November +
                       December;
            }
        }

        #region  应付款查询显示模型独有
        /// <summary>
        /// 1份月未付总金额
        /// </summary>
        public decimal January1 { get; set; }

        /// <summary>
        /// 2份月未付总金额
        /// </summary>
        public decimal February2 { get; set; }

        /// <summary>
        /// 3份月未付总金额
        /// </summary>
        public decimal March3 { get; set; }

        /// <summary>
        /// 4份月未付总金额
        /// </summary>
        public decimal April4 { get; set; }

        /// <summary>
        /// 5份月未付总金额
        /// </summary>
        public decimal May5 { get; set; }

        /// <summary>
        /// 6份月未付总金额
        /// </summary>
        public decimal June6 { get; set; }

        /// <summary>
        /// 7份月未付总金额
        /// </summary>
        public decimal July7 { get; set; }

        /// <summary>
        /// 8份月未付总金额
        /// </summary>
        public decimal August8 { get; set; }

        /// <summary>
        /// 9份月未付总金额
        /// </summary>
        public decimal September9 { get; set; }

        /// <summary>
        /// 10月份未付总金额
        /// </summary>
        public decimal October10 { get; set; }

        /// <summary>
        /// 11月份未付总金额
        /// </summary>
        public decimal November11 { get; set; }

        /// <summary>
        /// 12月份未付总金额
        /// </summary>
        public decimal December12 { get; set; }

        /// <summary>
        /// 年未付总金额汇总
        /// </summary>
        public decimal YearTotalNoPayed
        {
            get
            {
                return January1 + February2 + March3 + April4 + May5 + June6 + July7 + August8 + September9 + October10 + November11 +
                       December12;
            }
        }
        #endregion
    }
}
