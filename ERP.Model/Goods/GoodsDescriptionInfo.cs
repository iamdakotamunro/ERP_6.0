using System;

namespace ERP.Model.Goods
{
    //商品描述模板实体类
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsDescriptionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public GoodsDescriptionInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsDescriptionId">商品描述模版编号</param>
        /// <param name="goodsDescriptionName">商品描述模版名称</param>
        /// <param name="goodsDescriptionContent">商品描述模版内容</param>
        public GoodsDescriptionInfo(Guid goodsDescriptionId, string goodsDescriptionName, string goodsDescriptionContent)
        {
            GoodsDescriptionID = goodsDescriptionId;
            GoodsDescriptionName = goodsDescriptionName;
            GoodsDescriptionContent = goodsDescriptionContent;
        }
        /// <summary>
        /// 商品描述模版编号
        /// </summary>
        public Guid GoodsDescriptionID { get; set; }
        /// <summary>
        /// 商品描述模版名称
        /// </summary>
        public String GoodsDescriptionName { get; set; }
        /// <summary>
        /// 商品描述模版内容
        /// </summary>
        public String GoodsDescriptionContent { get; set; }
    }
}
