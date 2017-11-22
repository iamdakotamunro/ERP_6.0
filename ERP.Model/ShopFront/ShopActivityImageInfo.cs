using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model.ShopFront 
{
    /// <summary>
    /// 加盟店活动图片
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopActivityImageInfo
    {
        /// <summary>
        /// 活动图片
        /// </summary>
        [DataMember]
        public string ShopActivityImage { get; set; }

        /// <summary>显示图片类型（对应枚举ShopActivityImageType，1网站，2手机）
        /// </summary>
        [DataMember]
        public int ShopActivityImageType { get; set; }
    }
}
