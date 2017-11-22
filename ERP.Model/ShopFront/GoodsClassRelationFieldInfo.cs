using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 商品分类对应属性关系信息
    /// </summary>
    [Serializable]
    public class GoodsClassRelationFieldInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid FieldId { get; set; }
    }
}
