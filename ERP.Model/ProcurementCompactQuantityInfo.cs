using System;

namespace ERP.Model
{
    /// <summary>▄︻┻┳═一 采购合同量数据模型   ADD 2014-12-18  陈重文
    /// </summary>
    [Serializable]
    public class ProcurementCompactQuantityInfo
    {
        /// <summary>供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>供应商名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>年份
        /// </summary>
        public int DateYear { get; set; }

        /// <summary>合同签约金额
        /// </summary>
        public decimal CompactMoney { get; set; }

        /// <summary>已完成签约金额
        /// </summary>
        public decimal FinishedCompactMoney { get; set; }

        /// <summary>当月采购金额
        /// </summary>
        public decimal TheMonthProcurementMoney { get; set; }

        /// <summary>剩余签约金额
        /// </summary>
        public decimal SurplusCompactMoney { get; set; }

        /// <summary>完成百分比
        /// </summary>
        public string Percent
        {
            get
            {
                if (CompactMoney == 0)
                {
                    return "-";
                }
                return (FinishedCompactMoney / CompactMoney).ToString("p2");
            }
        }
    }
}
