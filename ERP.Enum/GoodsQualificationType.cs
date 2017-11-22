using ERP.Enum.Attribute;

namespace ERP.Enum
{
    public enum GoodsQualificationType
    {
        #region 商品资质
        /// <summary>
        /// 生产厂家(必须)
        /// </summary>
        [Enum("生产厂家")]
        ProductionUnit = 1,

        /// <summary>
        /// 生产许可证号(必须)
        /// </summary>
        [Enum("生产许可证号")]
        ProductionPermitNo = 2,

        /// <summary>
        /// 注册证号/批准文号(必须)
        /// </summary>
        [Enum("注册证号/批准文号")]
        MedicalDeviceRegistrationNumber = 3,

        /// <summary>
        /// 医疗器械注册登记表(必须)
        /// </summary>
        [Enum("医疗器械注册登记表")]
        MedicalDeviceRegistrationForm = 4,

        /// <summary>
        /// 检验报告(必须)
        /// </summary>
        [Enum("检验报告")]
        InspectionReport = 5,

        /// <summary>
        /// 产品标准
        /// </summary>
        [Enum("产品标准")]
        ProductStandard = 6,

        /// <summary>
        /// 进口报关单（进口医疗器械）
        /// </summary>
        [Enum("进口报关单（进口医疗器械）")]
        ImportCustomsDeclaration = 7
        #endregion
    }
}
