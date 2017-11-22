using System;

namespace ERP.Model
{
    /// <summary>▄︻┻┳═一 采购索票额度模型   ADD 2014-12-19  陈重文
    /// </summary>
    [Serializable]
    public class ProcurementTicketLimitInfo
    {
        /// <summary>公司ID 
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        public string FilialeName { get; set; }

        /// <summary>供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>供应商名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>年份
        /// </summary>
        public int DateYear { get; set; }

        /// <summary>月份
        /// </summary>
        public int DateMonth { get; set; }

        /// <summary>具体供应商收票额度
        /// </summary>
        public decimal TakerTicketLimit { get; set; }

        /// <summary>其他公司
        /// </summary>
        public Guid ElseFilialeId { get; set; }

        /// <summary>其他公司该供应商额度
        /// </summary>
        public decimal ElseCompanyIdTakerTicketLimit { get; set; }

        /// <summary>具体公司所有供应商额度总计
        /// </summary>
        public decimal TotalTakerTicketLimit { get; set; }

        /// <summary>已完成额度
        /// </summary>
        public decimal AlreadyCompleteLimit { get; set; }

        /// <summary>剩余额度（不超过10%额外额度）
        /// </summary>
        public decimal SurplusLimit
        {
            get { return TakerTicketLimit - AlreadyCompleteLimit + TakerTicketLimit * Convert.ToDecimal(0.1); }
        }

        /// <summary>剩余比例
        /// </summary>
        public decimal SurplusProportion
        {
            get
            {
                if (TakerTicketLimit == 0)
                    return 0;
                return SurplusLimit / (TakerTicketLimit + TakerTicketLimit * Convert.ToDecimal(0.1));
            }
        }

        /// <summary>占用比例
        /// </summary>
        public decimal Proportion { get; set; }
    }
}
