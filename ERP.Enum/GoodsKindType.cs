using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 商品打包类型枚举,数据查询返回信息过滤用
    /// </summary>
    [Serializable]
    public enum GoodsKindType
    {
        /// <summary>
        /// 不设置,全部返回
        /// </summary>
        [EnumAttribute("未设置")]
        NoSet = 0,
        /// <summary>
        /// 隐形眼镜
        /// </summary>
        [EnumAttribute("隐形眼镜")]
        ContactLenses = 1,
        /// <summary>
        /// 彩色隐形眼镜
        /// </summary>
        [EnumAttribute("彩色隐形眼镜")]
        ColorContactLenses = 2,
        /// <summary>
        /// 护理液
        /// </summary>
        [EnumAttribute("护理液")]
        LensSolution = 3,
        /// <summary>
        /// 框架眼镜
        /// </summary>
        [EnumAttribute("框架眼镜")]
        Frames = 4,
        /// <summary>
        /// 镜片
        /// </summary>
        [EnumAttribute("镜片")]
        Glasse = 5,
        /// <summary>
        /// 太阳眼镜
        /// </summary>
        [EnumAttribute("太阳眼镜")]
        SunFrame = 6,
        /// <summary>
        /// 运动眼镜
        /// </summary>
        [EnumAttribute("运动眼镜")]
        SportFrame = 7,
        /// <summary>
        /// 功能眼镜
        /// </summary>
        [EnumAttribute("功能眼镜")]
        FunctionFrame = 8,
        /// <summary>
        /// 护理用品
        /// </summary>
        [EnumAttribute("护理用品")]
        CareProducts = 9,
        /// <summary>
        /// 其他
        /// </summary>
        [EnumAttribute("其他")]
        Other = 10,

        ///// <summary>
        ///// 打包商品
        ///// </summary>
        //[EnumArrtibute("打包商品")]
        //PackGoods = 11,

        /// <summary>
        /// 非处方药
        /// </summary>
        [EnumAttribute("非处方药")]
        Medicinal = 12,

        /// <summary>
        /// 保健食品/食品
        /// </summary>
        [EnumAttribute("保健食品/食品")]
        HealthProducts = 13,

        /// <summary>
        /// 化妆护肤品
        /// </summary>
        [EnumAttribute("化妆护肤品")]
        CosmeticSkinCare = 14,

        /// <summary>
        /// 计生用品
        /// </summary>
        [EnumAttribute("计生用品")]
        FamilyPlanningProuducts = 15,

        /// <summary>
        /// 医疗器械
        /// </summary>
        [EnumAttribute("医疗器械")]
        MedicalEquipment = 16,

        /// <summary>
        /// 处方药
        /// </summary>
        [EnumAttribute("处方药")]
        PrescriptionMedicine = 17,

        /// <summary>
        /// 中药饮片/参茸
        /// </summary>
        [EnumAttribute("中药饮片/参茸")]
        ChineseMedicineYinPian = 18,

        /// <summary>
        /// 消杀类商品
        /// </summary>
        [EnumAttribute("消杀类商品")]
        DisinfectionTypeGoods = 19,
    }
}
