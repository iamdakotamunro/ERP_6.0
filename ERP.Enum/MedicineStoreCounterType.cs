using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 门店柜台
    /// </summary>
    public enum MedicineStoreCounterType
    {
        /// <summary>
        /// 001-抗微生物类
        /// </summary>
        [Enum("001-抗微生物类")]
        AntimicrobialClass = 1,

        /// <summary>
        /// 002-心脑血管类
        /// </summary>
        [Enum("002-心脑血管类")]
        CardiovascularCategory = 2,

        /// <summary>
        /// 003-消化系统类
        /// </summary>
        [Enum("003-消化系统类")]
        DigestiveSystem = 3,
        /// <summary>
        /// 004-呼吸系统类
        /// </summary>
        [Enum("004-呼吸系统类")]
        RespiratorySystemClass = 4,

        /// <summary>
        /// 005-神经系统类
        /// </summary>
        [Enum("005-神经系统类")]
        NervousSystem = 5,

        /// <summary>
        /// 006-泌尿系统类
        /// </summary>
        [Enum("006-泌尿系统类")]
        UrinarySystemClass = 6,

        /// <summary>
        /// 007-激素及内分泌类
        /// </summary>
        [Enum("007-激素及内分泌类")]
        HormonesAndEndocrine = 7,
        /// <summary>
        /// 008-免疫系统类
        /// </summary>
        [Enum("008-免疫系统类")]
        ImmuneSystemClass = 8,

        /// <summary>
        /// 009-妇科类
        /// </summary>
        [Enum("009-妇科类")]
        Gynecological = 9,

        /// <summary>
        /// 010-五官科类
        /// </summary>
        [Enum("010-五官科类")]
        EntClass = 10,
        /// <summary>
        /// 011-解热镇痛类
        /// </summary>
        [Enum("011-解热镇痛类")]
        AntipyreticAnalgesics = 11,

        /// <summary>
        /// 012-清热解毒类
        /// </summary>
        [Enum("012-清热解毒类")]
        Qingrejiedu = 12,

        /// <summary>
        /// 013-维生素、微量元素与矿物质
        /// </summary>
        [Enum("013-维生素、微量元素与矿物质")]
        VitaminsTraceElementsMinerals = 13,
        /// <summary>
        /// 014-补益类
        /// </summary>
        [Enum("014-补益类")]
        Tonic = 14,

        /// <summary>
        /// 015-伤骨科类
        /// </summary>
        [Enum("015-伤骨科类")]
        OrthopedicsCategory = 15,

        /// <summary>
        /// 016-外用药
        /// </summary>
        [Enum("016-外用药")]
        ExternalUse = 16,

        /// <summary>
        /// 017-综合类-1-抗过敏药物
        /// </summary>
        [Enum("017-综合类")]
        Allergy = 17,
        ///// <summary>
        ///// 017-综合类-2-皮肤科内服药
        ///// </summary>
        //[Enum("017-综合类-2-皮肤科内服药")]
        //InternalMedicineDermatology = 18,

        ///// <summary>
        ///// 017-综合类-4-麻醉用药物
        ///// </summary>
        //[Enum("017-综合类-4-麻醉用药物")]
        //UseNarcoticDrugs = 19,

        ///// <summary>
        ///// 017-综合类-5-营养治疗药物
        ///// </summary>
        //[Enum("017-综合类-5-营养治疗药物")]
        //NutritionalTherapy = 20,

        ///// <summary>
        ///// 017-综合类-6-抗肿瘤药物
        ///// </summary>
        //[Enum("017-综合类-6-抗肿瘤药物")]
        //AnticancerDrugs = 21,
        ///// <summary>
        ///// 017-综合类-7-血液系统药物
        ///// </summary>
        //[Enum("017-综合类-7-血液系统药物")]
        //BloodSystemAgents = 22,

        ///// <summary>
        ///// 017-综合类-8-解毒药物
        ///// </summary>
        //[Enum("017-综合类-8-解毒药物")]
        //Antidotes = 23,

        /// <summary>
        /// 018-含麻黄碱类复方制剂类
        /// </summary>
        [Enum("018-含麻黄碱类复方制剂类")]
        CompoundPreparationsContainingEphedrine = 24
    }
}
