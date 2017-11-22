using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 销售品种
    /// </summary>
    public enum MedicineSaleKindType
    {
        /// <summary>
        /// 单规制
        /// </summary>
        [Enum("单规制")]
        SingleRegulation = 0,

        /// <summary>
        /// 双规制
        /// </summary>
        [Enum("双规制")]
        TrackSystem = 1,

        /// <summary>
        /// 甲类
        /// </summary>
        [Enum("甲类")]
        GroupA = 2,

        /// <summary>
        /// 乙类
        /// </summary>
        [Enum("乙类")]
        GroupB = 3,
        /// <summary>
        /// 一类
        /// </summary>
        [Enum("一类")]
        OneType = 4,

        /// <summary>
        /// 二类
        /// </summary>
        [Enum("二类")]
        TwoType = 5,

        /// <summary>
        /// 三类
        /// </summary>
        [Enum("三类")]
        ThreeType = 6,

        /// <summary>
        /// 食品
        /// </summary>
        [Enum("食品")]
        Food = 7,
        /// <summary>
        /// 保健食品
        /// </summary>
        [Enum("保健食品")]
        HealthyFood = 8,

        /// <summary>
        /// 配方饮片
        /// </summary>
        [Enum("配方饮片")]
        RecipePieces = 9,

        /// <summary>
        /// 精制饮片
        /// </summary>
        [Enum("精制饮片")]
        RefinedPieces = 10,
        /// <summary>
        /// 参茸
        /// </summary>
        [Enum("参茸")]
        Ginseng = 11,

        /// <summary>
        /// 国产
        /// </summary>
        [Enum("国产")]
        MadeCountry = 12,

        /// <summary>
        /// 进口
        /// </summary>
        [Enum("进口")]
        Import = 13,
    }
}

