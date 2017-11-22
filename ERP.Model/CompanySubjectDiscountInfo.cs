using System;

namespace ERP.Model
{
    /// <summary>
    /// 差额折扣说明
    /// </summary>
    [Serializable]
    public class CompanySubjectDiscountInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 往来单位
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 往来公司
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Datecreated { get; set; }

        /// <summary>
        /// 记录人
        /// </summary>
        public string PersonnelName { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal Income { get; set; }

        /// <summary>
        /// 折扣/差额说明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 说明类型
        /// </summary>
        public int MemoType  { get; set; }
    }
}
