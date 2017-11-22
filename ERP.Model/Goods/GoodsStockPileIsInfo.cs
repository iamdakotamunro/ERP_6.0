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
    public class GoodsStockPileIsInfo
    {
        /// <summary>
        /// 主商品ID
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

        //g.IsScarcity,g.State

        /// <summary>
        /// 是否缺货
        /// </summary>
        public int IsScarcity { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

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
        /// 销售价
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 最近时间
        /// </summary>
        public DateTime RecentTime { get; set; }

        /// <summary>
        /// 代发货数量
        /// </summary>
        public int WaitQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsStockPileIsInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="goodsCode">商品代码</param>
        /// <param name="units">数量单位</param>
        /// <param name="isScarcity">是否缺货</param>
        /// <param name="state">状态</param>
        /// <param name="specification">规格</param>
        /// <param name="nonceWarehouseGoodsStock">当前仓库商品库存</param>
        /// <param name="nonceFilialeGoodsStock">当前分公司商品库存</param>
        /// <param name="nonceGoodsStock">当前商品库存</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="sellPrice">销售价</param>
        /// <param name="recentTime">最近时间</param>
        public GoodsStockPileIsInfo(Guid goodsId, string goodsName, string goodsCode, string units, int isScarcity, int state, string specification, double nonceWarehouseGoodsStock, double nonceFilialeGoodsStock, double nonceGoodsStock, decimal unitPrice, decimal sellPrice, DateTime recentTime)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Units = units;
            IsScarcity = isScarcity;
            State = state;
            Specification = specification;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            UnitPrice = unitPrice;
            SellPrice = sellPrice;
            RecentTime = recentTime;
        }
    }
}
