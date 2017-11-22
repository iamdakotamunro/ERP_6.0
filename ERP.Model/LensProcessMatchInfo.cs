using System;

namespace ERP.Model
{
    /// <summary>
    /// for 加工位匹配
    /// add by liangcanren at 2015-04-16
    /// </summary>
    [Serializable]
    public class LensProcessMatchInfo
    {
        /// <summary>
        /// 加工单ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 加工单号
        /// </summary>
        public string ProcessNo { get; set; }

        /// <summary>
        /// 匹配时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 加工位
        /// </summary>
        public string LensPlace { get; set; }

        /// <summary>
        /// 是否配货完成
        /// </summary>
        public bool IsComplete { get; set; }
    }
}
