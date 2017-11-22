using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品品牌信息类
    /// </summary>
    [Serializable]
    public class GoodsBrandInfo
    {
        /// <summary>
        /// 品牌编号
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 品牌Logo的链接
        /// </summary>
        public string BrandLogo { get; set; }

        /// <summary>
        /// 品牌描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 商品资料文档列表
        /// </summary>
        public IEnumerable<GoodsInformationInfo> GoodsInformationList { get; set; }
    }
}
