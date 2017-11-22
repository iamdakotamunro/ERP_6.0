using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 剂型
    /// </summary>
    public enum MedicineDosageFormType
    {
        /// <summary>
        /// 保健食品
        /// </summary>
        [Enum("保健食品")]
        HealthyFood = 0,

        /// <summary>
        /// 鼻喷剂
        /// </summary>
        [Enum("鼻喷剂")]
        NasalSpray = 1,

        /// <summary>
        /// 擦剂
        /// </summary>
        [Enum("擦剂")]
        Liniment = 2,

        /// <summary>
        /// 参茸
        /// </summary>
        [Enum("参茸")]
        Ginseng = 3,
        /// <summary>
        /// 茶剂
        /// </summary>
        [Enum("茶剂")]
        Tea = 4,

        /// <summary>
        /// 搽剂
        /// </summary>
        [Enum("搽剂")]
        MedicineLiniment = 5,

        /// <summary>
        /// 滴鼻剂
        /// </summary>
        [Enum("滴鼻剂")]
        NasalDrops = 6,

        /// <summary>
        /// 滴耳剂
        /// </summary>
        [Enum("滴耳剂")]
        EarDrops = 7,
        /// <summary>
        /// 滴剂
        /// </summary>
        [Enum("滴剂")]
        Drops = 8,

        /// <summary>
        /// 滴眼剂
        /// </summary>
        [Enum("滴眼剂")]
        EyeDrops = 9,

        /// <summary>
        /// 酊剂
        /// </summary>
        [Enum("酊剂")]
        Tincture = 10,
        /// <summary>
        /// 锭剂
        /// </summary>
        [Enum("锭剂")]
        Pastille = 11,

        /// <summary>
        /// 冻干粉针剂
        /// </summary>
        [Enum("冻干粉针剂")]
        FreezeDriedPowder = 12,

        /// <summary>
        /// 粉剂
        /// </summary>
        [Enum("粉剂")]
        Powder = 13,
        /// <summary>
        /// 粉针剂
        /// </summary>
        [Enum("粉针剂")]
        NeedlePowder = 14,

        /// <summary>
        /// 膏剂
        /// </summary>
        [Enum("膏剂")]
        Ointment = 15,

        /// <summary>
        /// 合剂
        /// </summary>
        [Enum("合剂")]
        Mixture = 16,

        /// <summary>
        /// 糊剂
        /// </summary>
        [Enum("糊剂")]
        Cataplasm = 17,
        /// <summary>
        /// 混悬剂
        /// </summary>
        [Enum("混悬剂")]
        Suspensions = 18,

        /// <summary>
        /// 胶剂
        /// </summary>
        [Enum("胶剂")]
        Glue = 19,

        /// <summary>
        /// 胶囊
        /// </summary>
        [Enum("胶囊")]
        Capsule = 20,

        /// <summary>
        /// 胶囊剂
        /// </summary>
        [Enum("胶囊剂")]
        MedicineCapsule = 21,
        /// <summary>
        /// 酒剂
        /// </summary>
        [Enum("酒剂")]
        LiquorAgents = 22,

        /// <summary>
        /// 咀嚼片
        /// </summary>
        [Enum("咀嚼片")]
        ChewableTablets = 23,

        /// <summary>
        /// 颗粒剂
        /// </summary>
        [Enum("颗粒剂")]
        Granules = 24,
        /// <summary>
        /// 口服粉剂
        /// </summary>
        [Enum("口服粉剂")]
        OralPowder = 25,

        /// <summary>
        /// 口服液
        /// </summary>
        [Enum("口服液")]
        Oral = 26,

        /// <summary>
        /// 膜剂
        /// </summary>
        [Enum("膜剂")]
        Agent = 27,
        /// <summary>
        /// 抹剂
        /// </summary>
        [Enum("抹剂")]
        DrugLiniment = 28,

        /// <summary>
        /// 内服水剂
        /// </summary>
        [Enum("内服水剂")]
        OralAgent = 29,

        /// <summary>
        /// 凝胶剂
        /// </summary>
        [Enum("凝胶剂")]
        Gels = 30,

        /// <summary>
        /// 喷物剂
        /// </summary>
        [Enum("喷物剂")]
        Sprays = 31,
        /// <summary>
        /// 喷雾剂
        /// </summary>
        [Enum("喷雾剂")]
        MedicineSprays = 32,

        /// <summary>
        /// 片剂
        /// </summary>
        [Enum("片剂")]
        Tablet = 33,

        /// <summary>
        /// 其他
        /// </summary>
        [Enum("其他")]
        Other = 34,

        /// <summary>
        /// 气雾剂
        /// </summary>
        [Enum("气雾剂")]
        Aerosol = 35,
        /// <summary>
        /// 溶液剂
        /// </summary>
        [Enum("溶液剂")]
        Solutions = 36,

        /// <summary>
        /// 乳膏剂
        /// </summary>
        [Enum("乳膏剂")]
        Creams = 37,

        /// <summary>
        /// 乳剂
        /// </summary>
        [Enum("乳剂")]
        MedicineCream = 38,
        /// <summary>
        /// 软膏剂
        /// </summary>
        [Enum("软膏剂")]
        Ointments = 39,

        /// <summary>
        /// 软胶囊
        /// </summary>
        [Enum("软胶囊")]
        SoftCapsule = 40,

        /// <summary>
        /// 散剂
        /// </summary>
        [Enum("散剂")]
        MedicinePowder = 41,
        /// <summary>
        /// 商品组合
        /// </summary>
        [Enum("商品组合")]
        Assortment = 42,

        /// <summary>
        /// 湿敷剂
        /// </summary>
        [Enum("湿敷剂")]
        WetAgent = 43,

        /// <summary>
        /// 漱口水
        /// </summary>
        [Enum("漱口水")]
        Mouthwash = 44,

        /// <summary>
        /// 栓剂
        /// </summary>
        [Enum("栓剂")]
        Suppository = 45,
        /// <summary>
        /// 水剂
        /// </summary>
        [Enum("水剂")]
        MedicineAgent = 46,

        /// <summary>
        /// 水丸
        /// </summary>
        [Enum("水丸")]
        Shuiwan = 47,

        /// <summary>
        /// 糖浆剂
        /// </summary>
        [Enum("糖浆剂")]
        Syrups = 48,

        /// <summary>
        /// 贴剂
        /// </summary>
        [Enum("贴剂")]
        Patch = 49,
        /// <summary>
        /// 透皮膏剂
        /// </summary>
        [Enum("透皮膏剂")]
        TransdermalPlasters = 50,

        /// <summary>
        /// 透皮膏贴
        /// </summary>
        [Enum("透皮膏贴")]
        MedicineTransdermalPlasters = 51,

        /// <summary>
        /// 透皮贴膏
        /// </summary>
        [Enum("透皮贴膏")]
        TransdermalPlastersOintment = 52,
        /// <summary>
        /// 透皮贴膏剂
        /// </summary>
        [Enum("透皮贴膏剂")]
        MedicineTransdermalPatchesOintment = 53,

        /// <summary>
        /// 涂膜剂
        /// </summary>
        [Enum("涂膜剂")]
        Balm = 54,

        /// <summary>
        /// 外用水剂
        /// </summary>
        [Enum("外用水剂")]
        TopicalSolution = 55,
        /// <summary>
        /// 丸剂
        /// </summary>
        [Enum("丸剂")]
        Pill = 56,

        /// <summary>
        /// 吸入剂
        /// </summary>
        [Enum("吸入剂")]
        Inhalants = 57,

        /// <summary>
        /// 洗剂
        /// </summary>
        [Enum("洗剂")]
        Lotion = 58,
        /// <summary>
        /// 橡胶膏剂
        /// </summary>
        [Enum("橡胶膏剂")]
        AdhesivePlasters = 59,

        /// <summary>
        /// 眼膏剂
        /// </summary>
        [Enum("眼膏剂")]
        EyeOintment = 60,

        /// <summary>
        /// 眼科用药
        /// </summary>
        [Enum("眼科用药")]
        OphthalmicDrugs = 61,
        /// <summary>
        /// 饮片
        /// </summary>
        [Enum("饮片")]
        Pieces = 62,

        /// <summary>
        /// 原货
        /// </summary>
        [Enum("原货")]
        OriginalGoods = 63,

        /// <summary>
        /// 原药
        /// </summary>
        [Enum("原药")]
        OriginalDrug = 64,
        /// <summary>
        /// 植入剂
        /// </summary>
        [Enum("植入剂")]
        Implant = 65,

        /// <summary>
        /// 注射剂
        /// </summary>
        [Enum("注射剂")]
        Injection = 66,
    }
}
