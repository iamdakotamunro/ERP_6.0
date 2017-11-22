using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 加盟店活动公告
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopActivityNoticeInfo
    {
        /// <summary>
        /// 公告Id
        /// </summary>
        [DataMember]
        public Guid NoticeID { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        [DataMember]
        public string NoticeTitle { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        [DataMember]
        public string NoticeContent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否显示在首页
        /// </summary>
        [DataMember]
        public bool IsNotice { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [DataMember]
        public bool IsShow { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int OrderIndex { get; set; }

    }
}
