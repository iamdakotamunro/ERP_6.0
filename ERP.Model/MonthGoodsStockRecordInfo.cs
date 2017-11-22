using System;

namespace ERP.Model
{
    /// <summary>
    /// 月商品库存记录
    /// </summary>
    [Serializable]
    public class MonthGoodsStockRecordInfo
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 月末库存数
        /// </summary>
        public int NonceGoodsStock { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlePrice { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalAmount
        {
            get
            {
                return NonceGoodsStock*SettlePrice;
            }
        }
    }
}
