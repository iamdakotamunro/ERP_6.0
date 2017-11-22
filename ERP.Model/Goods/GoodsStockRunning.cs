//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2008年12月20日
// 文件创建人:杨海飞
// 最后修改时间:
// 最后一次修改人:
//================================================

using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 出入库明细信息
    /// </summary>
    [Serializable]
    public class GoodsStockRunning
    {
        /// <summary>
        /// 
        /// </summary>
        public GoodsStockRunning() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stockid">出入库主键</param>
        /// <param name="filialeid">分公司ID</param>
        /// <param name="tradecode">订单号</param>
        /// <param name="companytype">公司类型</param>
        /// <param name="goodsid">商品ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="unitprice">单价</param>
        /// <param name="nonceWarehouseGoodsStock">仓库库存</param>
        /// <param name="noncefilialegoodsstock">子公司库存</param>
        /// <param name="specification">规格型号</param>
        /// <param name="datecreated">创建时间</param>
        /// <param name="companyname">公司名</param>
        public GoodsStockRunning(Guid stockid, Guid filialeid, string tradecode, int companytype, Guid goodsid, double quantity, decimal unitprice,double nonceWarehouseGoodsStock, double noncefilialegoodsstock, string specification, DateTime datecreated, string companyname)
        {
            StockId = stockid;
            FilialeId = filialeid;
            TradeCode = tradecode;
            CompanyType = companytype;
            GoodsId = goodsid;
            Quantity = quantity;
            UnitPrice = unitprice;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
            NonceFilialeGoodsStock = noncefilialegoodsstock;
            Specification = specification;
            DateCreated = datecreated;
            Companyname = companyname;
        }
        #region Model

        /// <summary>
        /// 出入库主键
        /// </summary>
        public Guid StockId { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary>
        /// 公司类型
        /// </summary>
        public int CompanyType { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 商品仓库库存
        /// </summary>
        public double NonceWarehouseGoodsStock { get; set; }

        /// <summary>
        /// 子公司库存
        /// </summary>
        public double NonceFilialeGoodsStock { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string Companyname { get; set; }

        /// <summary>
        /// 出入库单据类型
        /// </summary>
        public int StorageType { get; set; }

        #endregion Model
    }
}
