using System;
namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品药品
    /// </summary>
    [Serializable]
    public class GoodsMedicineInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品化学名称
        /// </summary>
        public string ChemistryName { get; set; }

        /// <summary>
        /// 商品化学名称首字母
        /// </summary>
        public string ChemistryNameFirstLetter { get; set; }

        /// <summary>
        /// 药品质检分类类型
        /// </summary>
        public int MedicineQualityType { get; set; }

        /// <summary>
        /// 药品销售品种类型
        /// </summary>
        public int MedicineSaleKindType { get; set; }

        /// <summary>
        /// 药品批发价
        /// </summary>
        public decimal MedicineWholesalePrice { get; set; }

        /// <summary>
        /// 药品税率类型
        /// </summary>
        public int MedicineTaxRateType { get; set; }

        /// <summary>
        /// 质量标准描述
        /// </summary>
        public string QualityStandardDescription { get; set; }

        /// <summary>
        /// 剂型类型
        /// </summary>
        public int MedicineDosageFormType { get; set; }

        /// <summary>
        /// 储存条件类型
        /// </summary>
        public int MedicineStorageConditionType { get; set; }

        /// <summary>
        /// 储存方式
        /// </summary>
        public int MedicineStorageModeType { get; set; }

        /// <summary>
        /// 养护类别
        /// </summary>
        public int MedicineCuringKindType { get; set; }

        /// <summary>
        /// 养护周期类型
        /// </summary>
        public int MedicineCuringCycleType { get; set; }

        /// <summary>
        /// 门店柜台类型
        /// </summary>
        public int MedicineStoreCounterType { get; set; }

        /// <summary>
        /// 库位管理类型
        /// 
        /// </summary>
        public int MedicineLibraryManageType { get; set; }
    }
}
