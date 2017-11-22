using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品库存进货
    /// </summary>
    [Serializable]
    public class GoodsStockCurrentInfo
    {
        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 当前仓库库存
        /// </summary>
        public double NonceWarehouseGoodsStock { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 最近进货价格
        /// </summary>
        public decimal RecentInPrice { get; set; }

        /// <summary>
        /// 最近一次进货的公司
        /// </summary>
        public Guid RecentCompanyId { get; set; }

        /// <summary>
        /// 最后一次进货日期
        /// </summary>
        public DateTime RecentCDate { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 最低价进货价格
        /// </summary>
        public decimal LowestInPrice { get; set; }

        /// <summary>
        /// 最低价进货的公司
        /// </summary>
        public Guid LowestInCompanyId { get; set; }

        /// <summary>
        /// 最低价进货日期
        /// </summary>
        public DateTime LowestInDate { get; set; }

        /// <summary>
        /// 库存信息类
        /// </summary>
        public GoodsStockCurrentInfo() { }

        /// <summary>
        /// 主商品子商品对
        /// </summary>
        /// <param name="goodsId">主商品ID</param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="filialeId">分公司ID</param>
        public GoodsStockCurrentInfo(Guid goodsId, Guid realGoodsId, Guid warehouseId, Guid filialeId)
        {
            GoodsId = goodsId;
            RealGoodsId = realGoodsId;
            WarehouseId = warehouseId;
            FilialeId = filialeId;
        }

        /// <summary>
        /// 库存信息类
        /// </summary>
        /// <param name="goodsId">主商品ID</param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="nonceWarehouseGoodsStock">当前仓库库存</param>
        /// <param name="filialeId">分公司ID</param>
        /// <param name="recentInPrice">最近进货价格</param>
        /// <param name="recentCompanyId">最近一次进货的公司</param>
        /// <param name="recentCDate">最后一次进货日期</param>
        /// <param name="specification">商品规格</param>
        /// <param name="lowestInCompanyId">最低价进货的公司</param>
        /// <param name="lowestInDate">最低价进货日期</param>
        /// <param name="lowestInPrice">最低价进货价格</param>
        public GoodsStockCurrentInfo(Guid goodsId, Guid realGoodsId, Guid warehouseId, double nonceWarehouseGoodsStock, Guid filialeId, decimal recentInPrice, Guid recentCompanyId,
            DateTime recentCDate, string specification, Guid lowestInCompanyId, DateTime lowestInDate, Decimal lowestInPrice)
        {
            GoodsId = goodsId;
            RealGoodsId = realGoodsId;
            WarehouseId = warehouseId;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
            FilialeId = filialeId;
            RecentInPrice = recentInPrice;
            RecentCompanyId = recentCompanyId;
            RecentCDate = recentCDate;
            Specification = specification;
            LowestInDate = lowestInDate;
            LowestInPrice = lowestInPrice;
            LowestInCompanyId = lowestInCompanyId;

        }
        /// <summary>
        /// 库存信息类
        /// </summary>
        /// <param name="goodsId">主商品ID</param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="nonceWarehouseGoodsStock">当前仓库库存</param>
        /// <param name="filialeId">分公司ID</param>
        /// <param name="recentInPrice">最近进货价格</param>
        /// <param name="recentCompanyId">最近一次进货的公司</param>
        /// <param name="recentCDate">最后一次进货日期</param>
        /// <param name="specification">商品规格</param>
        public GoodsStockCurrentInfo(Guid goodsId, Guid realGoodsId, Guid warehouseId, double nonceWarehouseGoodsStock, Guid filialeId, decimal recentInPrice, Guid recentCompanyId,
            DateTime recentCDate, string specification)
        {
            GoodsId = goodsId;
            RealGoodsId = realGoodsId;
            WarehouseId = warehouseId;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
            FilialeId = filialeId;
            RecentInPrice = recentInPrice;
            RecentCompanyId = recentCompanyId;
            RecentCDate = recentCDate;
            Specification = specification;
        }
        ///// <summary>
        ///// 某子商品的三类库存信息
        ///// </summary>
        ///// <param name="nonceWarehouseGoodsStock">当前仓库库存</param>
        ///// <param name="lowestInCompanyId">最低价进货的公司</param>
        ///// <param name="lowestInPrice">最低价进货价格</param>
        //public GoodsStockCurrentInfo(double nonceWarehouseGoodsStock,decimal lowestInPrice, Guid lowestInCompanyId)
        //{
        //    NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
        //    LowestInPrice = lowestInPrice;
        //    LowestInCompanyId = lowestInCompanyId;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is GoodsStockCurrentInfo)
                return ((obj as GoodsStockCurrentInfo).GoodsId == GoodsId && (obj as GoodsStockCurrentInfo).RealGoodsId == RealGoodsId);
            return base.Equals(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (RealGoodsId == Guid.Empty || GoodsId == Guid.Empty)
                return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + GoodsId + "#" + RealGoodsId;
            return stringRepresentation.GetHashCode();
        }
    }
}
