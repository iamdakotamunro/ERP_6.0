using System;

namespace ERP.Model.Goods
{
    /// <summary>商品分类属性
    /// </summary>
    public class GoodsClassFieldInfo
    {
        /// <summary>分类ID 
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>属性ID
        /// </summary>
        public Guid FieldId { get; set; }
    }
}
