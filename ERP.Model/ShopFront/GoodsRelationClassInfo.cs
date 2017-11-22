using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsRelationClassInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 是直属分类还是扩展分类
        /// </summary>
        public int AffiliationType { get; set; }
    }
}
