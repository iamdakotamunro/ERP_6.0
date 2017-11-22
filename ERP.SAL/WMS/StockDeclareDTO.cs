using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace ERP.SAL.WMS
{
    [Serializable]
    public class StockDeclareDTO
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
        public string Sku { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 需求
        /// </summary>
        public int Demand { get; set; }

        /// <summary>
        /// 当前商品库存
        /// </summary>
        public int NonceGoodsStock { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 购买量
        /// </summary>
        public int PurchaseQuantity { get; set; }
    }

    [Serializable]
    public class PurchaseDeclarationDTO
    {
        public Guid RealGoodsId { get; set; }

        public Guid GoodsId { get; set; }

        public string GoodsName { get; set; }

        public string Sku { get; set; }

        public int Quantity { get; set; }

        public int CurrentQuantity { get; set; }

        /// <summary>
        /// 购买量
        /// </summary>
        public int PurchaseQuantity { get; set; }

        public Guid FilialeId { get; set; }

        public string CompanyName { get; set; }

        public Guid CompanyId { get; set; }

        public decimal PurchasePrice { get; set; }

        public Guid PersonResponsible { get; set; }

        public String PersonResponsibleName { get; set; }

        public string Units { get; set; }

        public string GoodsCode { get; set; }
    }
}
