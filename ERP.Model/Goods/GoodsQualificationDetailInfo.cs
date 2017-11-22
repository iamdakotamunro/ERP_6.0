using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Goods
{
    [Serializable]
    public class GoodsQualificationDetailInfo
    {
        /// <summary>
        /// 商品ID
        /// 
        /// </summary>
        public Guid GoodsID { get; set; }
        /// <summary>
        /// 商品资质类型
        /// 
        /// </summary>
        public int GoodsQualificationType { get; set; }
        /// <summary>
        /// 单位名称或证书号码
        /// 
        /// </summary>
        public string NameOrNo { get; set; }
        /// <summary>
        /// 路径
        /// 
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 扩展名
        /// 
        /// </summary>
        public string ExtensionName { get; set; }
        /// <summary>
        /// 过期时间
        /// 
        /// </summary>
        public DateTime OverdueDate { get; set; }
        /// <summary>
        /// 最后操作时间
        /// 
        /// </summary>
        public DateTime LastOperationTime { get; set; }
    }
}
