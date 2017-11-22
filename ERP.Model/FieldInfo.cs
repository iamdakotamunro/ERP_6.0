//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年3月18日
// 文件创建人:马力
// 最后修改时间:2006年3月18日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;

namespace ERP.Model
{
    /// <summary>
    /// 产品属性信息类
    /// </summary>
    [Serializable]
    public class FieldInfo
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FieldInfo()
        {
            ChildFields = new List<FieldInfo>();
        }

        /// <summary>
        /// 属性编号
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// 父属性编号
        /// </summary>
        public Guid ParentFieldId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// 子属性
        /// </summary>
        public IList<FieldInfo> ChildFields { get; set; }
    }
}
