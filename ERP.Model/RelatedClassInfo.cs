using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 相关分类
    /// </summary>
    [Serializable]
    public class RelatedClassInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public RelatedClassInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId">类ID</param>
        /// <param name="relatedClassId">关联类ID</param>
        /// <param name="relatedType">关联类型</param>
        public RelatedClassInfo(Guid classId, Guid relatedClassId, Int32 relatedType)
        {
            ClassId = classId;
            RelatedClassId = relatedClassId;
            RelatedType = relatedType;
        }

        /// <summary>
        /// 类ID
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// 关联类ID
        /// </summary>
        public Guid RelatedClassId { get; set; }

        /// <summary>
        /// 关联类型
        /// </summary>
        public int RelatedType { get; set; }
    }
}
