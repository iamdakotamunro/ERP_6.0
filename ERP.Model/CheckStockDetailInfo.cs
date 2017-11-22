using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    ///lmshop_CheckStockDetail 盘点计划明细表
    /// </summary>
    [Serializable]
    public class CheckStockDetailInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CheckStockDetailInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="checkID">盘点计划ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="goodsName">商品名称</param>
        /// <param name="specification">规格</param>
        /// <param name="shelfNo">货位号</param>
        /// <param name="goodsStock">库存</param>
        /// <param name="firstCountAmount">初盘数量</param>
        /// <param name="secondCountAmount">复盘数量</param>
        /// <param name="secondCheckAmount">审核数量</param>
        public CheckStockDetailInfo(Guid checkID, Guid goodsId, Guid realGoodsId, String goodsName, String specification, String shelfNo,
            Int32 goodsStock, Int32 firstCountAmount, Int32 secondCountAmount, Int32 secondCheckAmount)
        {
            CheckID = checkID;
            GoodsId = goodsId;
            RealGoodsId = realGoodsId;
            GoodsName = goodsName;
            Specification = specification;
            ShelfNo = shelfNo;
            GoodsStock = goodsStock;
            FirstCountAmount = firstCountAmount;
            SecondCountAmount = secondCountAmount;
            SecondCheckAmount = secondCheckAmount;
        }

        /// <summary>
        /// 构造函数（添加用）
        /// </summary>
        /// <param name="checkID">盘点计划ID</param>
        /// <param name="goodsId">主商品ID</param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="goodsName">商品名称</param>
        /// <param name="specification">规格</param>
        /// <param name="shelfNo">货位号</param>
        public CheckStockDetailInfo(Guid checkID, Guid goodsId, Guid realGoodsId, String goodsName, String specification, String shelfNo)
        {
            CheckID = checkID;
            GoodsId = goodsId;
            RealGoodsId = realGoodsId;
            GoodsName = goodsName;
            Specification = specification;
            ShelfNo = shelfNo;
        }

        /// <summary>
        ///盘点计划ID
        /// </summary>
        public Guid CheckID { get; set; }

        /// <summary>
        ///主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        ///子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        ///商品名称
        /// </summary>
        public String GoodsName { get; set; }

        /// <summary>
        ///规格
        /// </summary>
        public String Specification { get; set; }

        /// <summary>
        ///货位号
        /// </summary>
        public String ShelfNo { get; set; }
        /// <summary>
        ///库存
        /// </summary>
        public Int32 GoodsStock { get; set; }

        /// <summary>
        ///初盘数量
        /// </summary>
        public Int32 FirstCountAmount { get; set; }

        /// <summary>
        ///复盘数量
        /// </summary>
        public Int32 SecondCountAmount { get; set; }

        /// <summary>
        ///审核数量
        /// </summary>
        public Int32 SecondCheckAmount { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string GoodsCode { get; set; }

    }
}
