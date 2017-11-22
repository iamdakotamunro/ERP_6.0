using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 退款日志类
    /// </summary>
    [Serializable]
    public class RefundLogInfo
    {
        #region Model

        /// <summary>
        /// 退款申请表ID
        /// </summary>
        public Guid RefundId { get; set; }

        /// <summary>
        /// 记录类型[0=用户提交,1=客服提交]
        /// </summary>
        public int LogType { get; set; }

        /// <summary>
        /// 记录内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitDate { get; set; }

        /// <summary>
        /// 类型：0默认，1短信，2邮件
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        public string Submitter { get; set; }

        #endregion Model
    }
}

