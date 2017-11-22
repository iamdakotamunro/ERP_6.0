//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年6月8日
// 文件创建人:马力
// 最后修改时间:2007年6月8日
// 最后一次修改人:马力
//================================================

using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品库存记录信息
    /// </summary>
    [Serializable]
    public class GoodsStockPileInfo
    {
        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品代码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 数量单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 当前仓库商品库存
        /// </summary>
        public double NonceWarehouseGoodsStock { get; set; }

        /// <summary>
        /// 当前分公司商品库存
        /// </summary>
        public double NonceFilialeGoodsStock { get; set; }

        /// <summary>
        /// 当前商品库存
        /// </summary>
        public double NonceGoodsStock { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 最后一次进价
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 最后进货时间
        /// </summary>
        public DateTime? RecentInDate { get; set; }

        /// <summary>
        /// 代发货量
        /// </summary>
        public int WaitConsignmentedGoodsStock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsStockPileInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="goodsCode">商品代码</param>
        /// <param name="units">数量单位</param>
        /// <param name="specification">规格</param>
        /// <param name="nonceFilialeGoodsStock">当前分公司商品库存</param>
        /// <param name="nonceGoodsStock">当前商品库存</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="sellPrice">销售价</param>
        /// <param name="nonceWarehouseGoodsStock">当前仓库商品库存</param>
        public GoodsStockPileInfo(Guid goodsId, string goodsName, string goodsCode, string units, string specification, double nonceFilialeGoodsStock, double nonceGoodsStock, decimal unitPrice, decimal sellPrice, double nonceWarehouseGoodsStock)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Units = units;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            UnitPrice = unitPrice;
            SellPrice = sellPrice;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
        }
    }
}
