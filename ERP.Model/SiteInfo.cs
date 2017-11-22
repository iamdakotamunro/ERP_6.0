using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SiteInfo
    {
        /// <summary>
        /// 网站ID
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// 网站链接地址
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// 网站名
        /// </summary>
        public string SiteName { set; get; }

        /// <summary>
        /// 网站采用的编码方式
        /// </summary>
        public string Encode { set; get; }

        /// <summary>
        /// 最大索引
        /// </summary>
        public int MaxIndex { set; get; }

        /// <summary>
        /// 是否是js分页(是为true)
        /// </summary>
        public string IsJspage { set; get; }
    }
}
