using System;
using System.Collections.Generic;
using EntityPurchaseType = ERP.Enum.EntityPurchaseType;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 产品类
    /// </summary>
    [Serializable]
    public class GoodsInfo
    {
        #region -- 其它附加模型
        /// <summary>
        /// 
        /// </summary>
        public GoodsClassInfo ClassInfo { get; set; }

        /// <summary>
        /// 品牌信息
        /// </summary>
        public GoodsBrandInfo BrandInfo { get; set; }

        /// <summary>
        /// 商品扩展信息
        /// </summary>
        public GoodsExtendInfo ExpandInfo { get; set; }

        /// <summary>
        /// 子商品列表
        /// </summary>
        public IList<ChildGoodsInfo> ChildGoodsList { get; set; }

        ///// <summary>
        ///// 门店采购组
        ///// </summary>
        //public IList<FilialeInfo> ShopPurchasingGroupList { get; set; }

        /// <summary>
        /// 门店采购组
        /// 1直营、2加盟、3联盟
        /// </summary>
        public Dictionary<Guid, List<EntityPurchaseType>> DictGoodsPurchase { get; set; }

        /// <summary>
        /// 销售平台组
        /// </summary>
        public IList<GoodsGroupInfo> SaleGroupList { get; set; }

        /// <summary>
        /// 框架信息
        /// </summary>
        public FrameGoodsInfo FrameGoodsInfo { get; set; }

        /// <summary>
        /// 商品药品信息
        /// </summary>
        public GoodsMedicineInfo GoodsMedicineInfo { get; set; }

        /// <summary>
        /// 商品资质信息
        /// </summary>
        public List<GoodsQualificationDetailInfo> GoodsQualificationDetailInfos { get; set; }

        #endregion

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 直属分类ID
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 卖库存类型
        /// </summary>
        public int SaleStockType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SaleStockState { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// 库存状况：如果商品缺货，显示在前台的文字信息。
        /// </summary>
        public string StockStatus { get; set; }

        /// <summary>
        /// 是否库存不足
        /// </summary>
        public bool IsStockScarcity { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 状态：1上架，0下架
        /// </summary>
        public bool IsOnShelf { get; set; }

        /// <summary>
        /// 批准文号
        /// </summary>
        public string ApprovalNO { get; set; }

        /// <summary>
        /// 是否有子商品
        /// </summary>
        public bool HasRealGoods { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>商品ID集合 （库存查询时使用，其他地方无用） 2015-04-17  陈重文
        /// </summary>
        public IList<Guid> GoodsIds { get; set; }

        public Guid SeriesId { get; set; }

        /// <summary>
        /// 老商品Code
        /// </summary>
        public string OldGoodsCode { get; set; }

        /// <summary>
        /// 商品名称首字母
        /// </summary>
        public string PurchaseNameFirstLetter { get; set; }

        /// <summary>
        /// 商品编号(供应商)
        /// </summary>
        public string SupplierGoodsCode { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 保质期
        /// </summary>
        public string ShelfLife { get; set; }

        /// <summary>
        /// 是否进口商品
        /// </summary>
        public bool IsImportedGoods { get; set; }

        /// <summary>
        /// 是否奢侈品
        /// </summary>
        public bool IsLuxury { get; set; }

        /// <summary>
        /// 是否禁采
        /// </summary>
        public bool IsBannedPurchase { get; set; }

        /// <summary>
        /// 是否禁销
        /// </summary>
        public bool IsBannedSale { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// 商品审核状态(0:审核未通过;1:待采购经理审核;2:待质管部审核;3:待负责人终审;4:审核通过;)
        /// </summary>
        public int GoodsAuditState { get; set; }

        /// <summary>
        /// 商品审核备注
        /// </summary>
        public string GoodsAuditStateMemo { get; set; }

        public int? MedicineDosageFormType { get; set; }

        public int MedicineLibraryManageType { get; set; }

        public int MedicineStorageModeType { get; set; }
    }
}
