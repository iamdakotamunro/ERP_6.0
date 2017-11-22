using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    ///<summary>
    /// 子商品表
    ///</summary>
    [Serializable]
    public class ChildGoodsInfo
    {
        /// <summary>
        /// 主商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 子商品ID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// 是否缺货
        /// </summary>
        public bool IsScarcity { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 子商品是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long OrderIndex { get; set; }

        /// <summary>
        /// 前台是否显示
        /// </summary>
        public bool Disable { get; set; }

        /// <summary>
        /// 对应的属性字段ID集合
        /// </summary>
        public IList<FieldInfo> FieldList { get; set; }
    }
}
