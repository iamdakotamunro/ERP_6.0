using System;
using System.Collections.Generic;

namespace ERP.Model
{
    /// <summary>
    /// 会计科目信息
    /// </summary>
    [Serializable]
    public class SubjectInfo
    {
        /// <summary>
        /// 票据编号
        /// </summary>
        public Guid BillId { get; set; }

        /// <summary>
        /// 会计科目编号
        /// </summary>
        public string SubjectID { get; set; }

        /// <summary>
        /// 会计科目
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}