namespace ERP.Enum
{
    /// <summary>
    /// 邮件和短信状态类型枚举
    /// </summary>
    public enum EmailSmsStatusType
    {
        /// <summary>
        /// 单发邮件短信-3
        /// </summary>
        SingleSenderEmailSms = -3,

        /// <summary>
        /// 单发短信-2
        /// </summary>
        SingleSenderSms = -2,

        /// <summary>
        /// 单发邮件-1
        /// </summary>
        SingleSenderEmail = -1,

        /// <summary>
        /// 邮件群发1
        /// </summary>
        GroupSenderEmail = 1,

        /// <summary>
        /// 短信群发2
        /// </summary>
        GroupSenderSms = 2,

        /// <summary>
        /// 邮件短信群发3
        /// </summary>
        GroupSenderEmailSms = 3

    }
}
