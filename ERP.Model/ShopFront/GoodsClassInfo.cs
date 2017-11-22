using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 商品分类信息
    /// </summary>
    [Serializable]
    public class GoodsClassInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ParentClassId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderIndex { get; set; }
    }
}
