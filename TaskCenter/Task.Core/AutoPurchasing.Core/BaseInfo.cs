using System;

namespace AutoPurchasing.Core
{
    public class BaseInfo
    {
        public static string AppDirPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 单据编号类型
        /// </summary>
        public enum CodeType
        {
            PH = 21
        }        
    }
}
