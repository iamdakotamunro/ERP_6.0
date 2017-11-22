using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 商品对应属性关系信息
    /// </summary>
    [Serializable]
    public class GoodsRelationFieldInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid FieldId { get; set; }
    }
}
