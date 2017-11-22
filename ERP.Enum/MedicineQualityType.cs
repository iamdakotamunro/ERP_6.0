using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 质检分类
    /// </summary>
    public enum MedicineQualityType
    {
        /// <summary>
        /// 药品质量检测
        /// </summary>
        [Enum("药品质量检测")]
        Quality= 0,

        /// <summary>
        /// 药品成分检测
        /// </summary>
        [Enum("药品成分检测")]
        Ingredient = 1,

        /// <summary>
        /// 药品重金属检测
        /// </summary>
        [Enum("药品重金属检测")]
        HeavyMetal = 2,

        /// <summary>
        /// 药品不良反应检测
        /// </summary>
        [Enum("药品不良反应检测")]
        AdverseReactions = 3,
        /// <summary>
        /// 药品密封性检测
        /// </summary>
        [Enum("药品密封性检测")]
        Tightness = 4,

        /// <summary>
        /// 生物药品检测
        /// </summary>
        [Enum("生物药品检测")]
        Biopharmaceuticals = 5,

        /// <summary>
        /// 药品外观检测
        /// </summary>
        [Enum("药品外观检测")]
        Appearance = 6,

        /// <summary>
        /// 药品常规检测
        /// </summary>
        [Enum("药品常规检测")]
        Conventional = 7,
        /// <summary>
        /// 药品理化检测
        /// </summary>
        [Enum("药品理化检测")]
        PhysicsAndChemistry = 8,

        /// <summary>
        /// 药品安全检测
        /// </summary>
        [Enum("药品安全检测")]
        Safety = 9,

        /// <summary>
        /// 药品缺陷检测
        /// </summary>
        [Enum("药品缺陷检测")]
        Defect = 10
    }
}
