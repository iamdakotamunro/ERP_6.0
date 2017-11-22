using System;

namespace CMS.WCF.ExceptionService.Model
{
    [Serializable]
    public class FromSourceInfo
    {
        public Guid Id { get; set; }

        /// <summary>
        /// WCF终结点 
        /// </summary>
        public string EndPointName { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public Boolean IsDefault { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 0门店,1网站,2网站总后台,3.管理系统
        /// </summary>
        public byte FromType { get; set; }
    }
}
