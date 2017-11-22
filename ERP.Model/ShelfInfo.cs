using System;
//================================================
// 功能：货位管理实体类
// 作者：刘彩军
// 时间：2011-01-05
//================================================
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 货位管理实体类
    /// </summary>
    public class ShelfInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ShelfInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsID">商品ID</param>
        /// <param name="realGoodsId"> </param>
        /// <param name="filialeID">分公司ID</param>
        /// <param name="warehouseID">仓库ID</param>
        /// <param name="shelfNo">货架编号</param>
        public ShelfInfo(Guid goodsID,Guid realGoodsId, Guid filialeID, Guid warehouseID, string shelfNo)
        {
            GoodsID = goodsID;
            RealGoodsID = realGoodsId;
            FilialeID = filialeID;
            WarehouseID = warehouseID;
            ShelfNo = shelfNo;
        }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsID { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsID { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseID { get; set; }

        /// <summary>
        /// 货架编号
        /// </summary>
        public String ShelfNo { get; set; }
    }
}
