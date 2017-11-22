using System;
namespace ERP.Model
{
    /// <summary>物流流转信息
    /// </summary>
    [Serializable]
    public class LogisticsMessageInfo
    {
        /// <summary>物流信息
        /// </summary>
        public String Description { get; set; }

        /// <summary>时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
