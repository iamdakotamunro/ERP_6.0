using System;

namespace ERP.SAL.LogCenter
{
    public class LogService
    {
        private log4net.Util.Serializer.ISerializer _;// 误删
        private readonly static log4net.ILog _localLogger = log4net.LogManager.GetLogger("LogService");

        private LogService()
        {

        }

        /// <summary>记录调试日志
        /// </summary>
        public static void LogDebug(string message, string logTag, Exception exception = null)
        {
            _localLogger.Debug(string.Format("{0}:{1}", logTag, message), exception);
        }

        /// <summary>记录普通日志
        /// </summary>
        public static void LogInfo(string message, string logTag, Exception exception = null)
        {
            _localLogger.Info(string.Format("{0}:{1}", logTag, message), exception);
        }

        /// <summary>记录警告日志
        /// </summary>
        public static void LogWarn(string message, string logTag, Exception exception = null)
        {
            _localLogger.Warn(string.Format("{0}:{1}", logTag, message), exception);
        }

        /// <summary>记录错误日志
        /// </summary>
        public static void LogError(string message, string logTag, Exception exception = null)
        {
            _localLogger.Error(string.Format("{0}:{1}", logTag, message), exception);
        }

        /// <summary>记录致命日志
        /// </summary>
        public static void LogFatal(string message, string logTag, Exception exception = null)
        {
            _localLogger.Fatal(string.Format("{0}:{1}", logTag, message), exception);
        }
    }
}
