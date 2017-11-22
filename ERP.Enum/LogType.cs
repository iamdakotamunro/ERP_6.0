using System;

namespace ERP.Enum
{
    /// <summary>
    /// 记录日志类型枚举
    /// </summary>
    [Serializable]
    public enum LogType
    {
        /// <summary>
        /// 全部
        /// </summary>
        Both = 0,

        /// <summary>
        /// 记录到日志文件中
        /// </summary>
        NotePadLog = 1,

        /// <summary>
        /// 记录到窗体应用程序事件中
        /// </summary>
        WindowsEvent = 2
    }
}
