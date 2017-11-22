using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    ///lmshop_WaitCheckGoods 待盘点商品表
    /// </summary>
    [Serializable]
    public class WaitCheckGoodsInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public WaitCheckGoodsInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="goodsName">商品名称</param>
        /// <param name="state">状态</param>
        /// <param name="warehouseId">仓库ID</param>
        public WaitCheckGoodsInfo(Guid goodsId, string goodsName, int state, Guid warehouseId)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            State = state;
            WarehouseId = warehouseId;
        }

        /// <summary>
        ///商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        ///商品名称
        /// </summary>
        public String GoodsName { get; set; }

        /// <summary>
        ///状态
        /// </summary>
        public Int32 State { get; set; }

        /// <summary>
        ///仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

    }
}
