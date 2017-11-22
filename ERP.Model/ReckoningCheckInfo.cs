using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来账核对表
    /// </summary>
    public class ReckoningCheckInfo
    {
        /// <summary>
        /// 账单编号
        /// </summary>
        public Guid ReckoningId { get; set; }

        /// <summary>
        /// 核对备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
