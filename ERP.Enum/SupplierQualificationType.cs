using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 资质类型枚举
    /// </summary>
    public enum SupplierQualificationType
    {
        #region 供应商资质
        /// <summary>
        /// 营业执照
        /// </summary>
        [Enum("营业执照")]
        BusinessLicense = 1,

        /// <summary>
        /// 组织机构代码证
        /// </summary>
        [Enum("组织机构代码证")]
        OrganizationCodeCertificate = 2,

        /// <summary>
        /// 税务登记证
        /// </summary>
        [Enum("税务登记证")]
        TaxRegistrationCertificate = 3,

        /// <summary>
        /// 商品销售授权书
        /// </summary>
        [Enum("商品销售授权书")]
        GoodsSalesAuthorization = 4,

        /// <summary>
        /// 医疗器械企业经营许可证
        /// </summary>
        [Enum("医疗器械企业经营许可证")]
        MedicalDeviceBusinessLicense = 5,

        /// <summary>
        /// 医疗器械生产许可证
        /// </summary>
        [Enum("医疗器械生产许可证")]
        MedicalDeviceProductionLicense = 6,
        #endregion
    }
}
