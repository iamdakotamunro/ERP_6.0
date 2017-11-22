//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2008年5月16日
// 文件创建人:马力
// 最后修改时间:2008年5月16日
// 最后一次修改人:马力
//================================================

using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品需求类
    /// </summary>
    [Serializable]
    public class GoodsDemandInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 需求
        /// </summary>
        public int Demand { get; set; }

        /// <summary>
        /// 当前仓库的商品库存
        /// </summary>
        public double NonceWarehouseGoodsStock { get; set; }

        /// <summary>
        /// 当前分公司的商品库存
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
        /// 公司ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 购买量
        /// </summary>
        public int PurchaseQuantity { get; set; }

        /// <summary>
        /// 缺货仓
        /// </summary>
        public Guid NeedWarehouseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsDemandInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="goodsCode">商品编号</param>
        /// <param name="specification">规格</param>
        /// <param name="demand">需求</param>
        /// <param name="nonceFilialeGoodsStock">当前分公司的商品库存</param>
        /// <param name="nonceGoodsStock">当前商品库存</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="companyName">公司名</param>
        public GoodsDemandInfo(Guid goodsId, string goodsName, string goodsCode, string specification, int demand, double nonceFilialeGoodsStock, double nonceGoodsStock, decimal unitPrice, string companyName)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            Demand = demand;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            UnitPrice = unitPrice;
            CompanyName = companyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="goodsCode">商品编号</param>
        /// <param name="specification">规格</param>
        /// <param name="demand">需求</param>
        /// <param name="nonceWarehouseGoodsStock">当前仓库的商品库存</param>
        /// <param name="nonceFilialeGoodsStock">当前分公司的商品库存</param>
        /// <param name="nonceGoodsStock">当前商品库存</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="companyName">公司名</param>
        public GoodsDemandInfo(Guid goodsId, string goodsName, string goodsCode, string specification, int demand, double nonceWarehouseGoodsStock, double nonceFilialeGoodsStock, double nonceGoodsStock, decimal unitPrice, string companyName)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            Demand = demand;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            UnitPrice = unitPrice;
            CompanyName = companyName;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is GoodsDemandInfo)
                return (obj as GoodsDemandInfo).GoodsId == GoodsId && (obj as GoodsDemandInfo).Specification == Specification;
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
