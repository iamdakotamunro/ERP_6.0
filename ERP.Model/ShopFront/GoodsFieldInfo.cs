using System;
namespace Keede.Ecsoft.Model.ShopFront
{
    /// <summary>
    /// 产品属性信息类
    /// </summary>
    [Serializable]
    public class GoodsFieldInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ParentFieldId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IsCompField { get; set; }
    }
}
