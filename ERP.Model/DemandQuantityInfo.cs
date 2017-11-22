using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 库存需求表 MODEL
    /// DYY
    /// 2010-6-28
    /// </summary>
    [Serializable]
    public class DemandQuantityInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsID { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseID { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 商品实际库存
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// 需求量，订单状态（小于 4）
        /// </summary>
        public int RequireQuantity { get; set; }

        /// <summary>
        /// 占用量
        /// </summary>
        public int DemandQuantity { get; set; }

        /// <summary>
        /// 出库量
        /// </summary>
        public int OutboundQuantity { get; set; }

        /// <summary>
        /// 商品缺货说明
        /// </summary>
        public string GoodsStatement { get; set; }

        /// <summary>
        /// 订单详情页是否备货
        /// </summary>
        public string Beihuo { get; set; }

        /// <summary>
        /// 商品库存和需求信息
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="stockQuantity">商品实际库存</param>
        /// <param name="beforePrintQuantity">打印前需求[1-4]</param>
        /// <param name="filialeId">分公司ID</param>
        /// <param name="warehouseId">仓库ID</param>
        public DemandQuantityInfo(Guid goodsId, Int32 stockQuantity, Int32 beforePrintQuantity, Guid filialeId, Guid warehouseId)
        {
            GoodsID = goodsId;
            StockQuantity = stockQuantity;
            RequireQuantity = beforePrintQuantity;
            FilialeID = filialeId;
            WarehouseID = warehouseId;
        }

        /// <summary>
        /// 商品需求信息
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="beforePrintQuantity">打印前需求[1-4]</param>
        /// <param name="filialeId">分公司ID</param>
        /// <param name="warehouseId">仓库ID</param>
        public DemandQuantityInfo(Guid goodsId, Int32 beforePrintQuantity, Guid filialeId, Guid warehouseId)
        {
            GoodsID = goodsId;
            RequireQuantity = beforePrintQuantity;
            FilialeID = filialeId;
            WarehouseID = warehouseId;
        }
        /// <summary>
        /// 
        /// </summary>
        public DemandQuantityInfo()
        {
        }
    }
}
