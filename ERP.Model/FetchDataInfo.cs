#region usings create by dinghq 2011-04-20

using System;
using System.Reflection;

#endregion

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FetchDataInfo
    {
        /// <summary>
        /// 标识
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// 产品名
        /// </summary>
        public string GoodsName { set; get; }

        /// <summary>
        /// 产品链接地址
        /// </summary>
        public string GoodsUrl { get; set; }

        /// <summary>
        /// 产品所属网站ID
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// 产品在该网站报价
        /// </summary>
        public decimal GoodsPrice { get; set; }

        /// <summary>
        /// 最后获取该产品信息时间
        /// </summary>
        public DateTime LastUpdateTime { set; get; }

        /// <summary>
        /// 产品所属网站信息
        /// </summary>
        public SiteInfo Site { set; get; }

        /// <summary>
        /// 产品对应本身网站的产品标识
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 是否匹配
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
