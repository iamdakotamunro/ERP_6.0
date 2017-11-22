using System;
using ERP.Enum;


namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 发票卷详细
    /// </summary>
    public class InvoiceRollDetail
    {
        /// <summary>
        /// 对应发票领取记录ID
        /// </summary>
        public Guid RollId { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票卷起始号码
        /// </summary>
        public long StartNo { get; set; }

        /// <summary>
        /// 发票卷结束号码
        /// </summary>
        public long EndNo { get; set; }

        /// <summary>
        /// 发票卷状态
        /// </summary>
        public InvoiceRollState State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否上报
        /// </summary>
        public Boolean IsSubmit { get; set; }
    }
}
