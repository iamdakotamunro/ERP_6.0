using System;

namespace ERP.Model
{
    /// <summary>
    /// 需调拨统计基本业务模型
    /// </summary>
    public class NeedToAllocateInfo
    {
        /// <summary>
        /// 进行需调拨统计的单据唯一标识
        /// </summary>
        public Guid BusinessIdentify { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 发货仓库ID
        /// </summary>
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 发货仓库名称
        /// </summary>
        public String DeliverWarehouseName { get; set; }

        /// <summary>
        /// 采购负责人ID
        /// </summary>
        public Guid PersonResponsibleId { get; set; }

        /// <summary>
        /// 该子商品数量
        /// </summary>
        public Int32 Quantity { get; set; }
    }
}
