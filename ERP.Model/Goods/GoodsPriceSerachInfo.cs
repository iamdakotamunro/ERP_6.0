using System;

namespace ERP.Model.Goods
{
    /// <summary>商品价格查询模型 ADD 2014-08-08 陈重文
    /// </summary>
    public class GoodsPriceSerachInfo
    {
        /// <summary>商品ID
        /// </summary>
        public Guid GoodsID { get; set; }

        /// <summary>商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>采购价
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>加盟价
        /// </summary>
        public decimal JoinPrice { get; set; }

        /// <summary>参考价
        /// </summary>
        public decimal ReferencePrice { get; set; }

        /// <summary>可得价
        /// </summary>
        public decimal KeedePrice { get; set; }

        /// <summary>百秀价
        /// </summary>
        public decimal BaishopPrice { get; set; }
        /// <summary>
        /// 批发价
        /// zal 2015-09-10
        /// </summary>
        public decimal WholesalePrice { set; get; }

    }
}
