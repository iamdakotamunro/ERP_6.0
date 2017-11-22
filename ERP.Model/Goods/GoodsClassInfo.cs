using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 产品分类模型
    /// </summary>
    [Serializable]
    public class GoodsClassInfo
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// 分类父节点编号
        /// </summary>
        public Guid ParentClassId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>分类属性
        /// </summary>
        public List<Guid> GoodsClassFieldList { get; set; }
    }
}
