using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 出入库单据明细模型
    /// 作者：刘彩军
    /// 时间：2012-06-27
    /// </summary>
    [Serializable]
    public class StockDetailInfo
    {
        /// <summary>
        /// 出入库单据ID
        /// </summary>
        public Guid StockId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public String GoodsName { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        public String Specification { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 代理价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否出现异常
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExceptionMsg { get; set; }

        /// <summary>
        /// 加工单号
        /// </summary>
        public string ProcessNo { get; set; }

        /// <summary>
        /// 门店公司ID
        /// </summary>
        public Guid ShopFilialeId { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid CompGoodsId { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string BatchNo { get; set; }
    }
}
