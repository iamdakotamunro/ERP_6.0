using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品属性
    /// </summary>
    public class GoodsFieldInfo:FieldInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public Guid RealGoodsId { get; set; }
        
    }
}
