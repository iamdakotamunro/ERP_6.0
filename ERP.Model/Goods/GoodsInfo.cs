using System;
using System.Collections.Generic;
using EntityPurchaseType = ERP.Enum.EntityPurchaseType;

namespace ERP.Model.Goods
{
    /// <summary>
    /// ��Ʒ��
    /// </summary>
    [Serializable]
    public class GoodsInfo
    {
        #region -- ��������ģ��
        /// <summary>
        /// 
        /// </summary>
        public GoodsClassInfo ClassInfo { get; set; }

        /// <summary>
        /// Ʒ����Ϣ
        /// </summary>
        public GoodsBrandInfo BrandInfo { get; set; }

        /// <summary>
        /// ��Ʒ��չ��Ϣ
        /// </summary>
        public GoodsExtendInfo ExpandInfo { get; set; }

        /// <summary>
        /// ����Ʒ�б�
        /// </summary>
        public IList<ChildGoodsInfo> ChildGoodsList { get; set; }

        ///// <summary>
        ///// �ŵ�ɹ���
        ///// </summary>
        //public IList<FilialeInfo> ShopPurchasingGroupList { get; set; }

        /// <summary>
        /// �ŵ�ɹ���
        /// 1ֱӪ��2���ˡ�3����
        /// </summary>
        public Dictionary<Guid, List<EntityPurchaseType>> DictGoodsPurchase { get; set; }

        /// <summary>
        /// ����ƽ̨��
        /// </summary>
        public IList<GoodsGroupInfo> SaleGroupList { get; set; }

        /// <summary>
        /// �����Ϣ
        /// </summary>
        public FrameGoodsInfo FrameGoodsInfo { get; set; }

        /// <summary>
        /// ��ƷҩƷ��Ϣ
        /// </summary>
        public GoodsMedicineInfo GoodsMedicineInfo { get; set; }

        /// <summary>
        /// ��Ʒ������Ϣ
        /// </summary>
        public List<GoodsQualificationDetailInfo> GoodsQualificationDetailInfos { get; set; }

        #endregion

        /// <summary>
        /// ��ƷID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// ֱ������ID
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Ʒ��ID
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// ���������
        /// </summary>
        public int SaleStockType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SaleStockState { get; set; }

        /// <summary>
        /// ������λ
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// �г���
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// ���״���������Ʒȱ������ʾ��ǰ̨��������Ϣ��
        /// </summary>
        public string StockStatus { get; set; }

        /// <summary>
        /// �Ƿ��治��
        /// </summary>
        public bool IsStockScarcity { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// ״̬��1�ϼܣ�0�¼�
        /// </summary>
        public bool IsOnShelf { get; set; }

        /// <summary>
        /// ��׼�ĺ�
        /// </summary>
        public string ApprovalNO { get; set; }

        /// <summary>
        /// �Ƿ�������Ʒ
        /// </summary>
        public bool HasRealGoods { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>��ƷID���� ������ѯʱʹ�ã������ط����ã� 2015-04-17  ������
        /// </summary>
        public IList<Guid> GoodsIds { get; set; }

        public Guid SeriesId { get; set; }

        /// <summary>
        /// ����ƷCode
        /// </summary>
        public string OldGoodsCode { get; set; }

        /// <summary>
        /// ��Ʒ��������ĸ
        /// </summary>
        public string PurchaseNameFirstLetter { get; set; }

        /// <summary>
        /// ��Ʒ���(��Ӧ��)
        /// </summary>
        public string SupplierGoodsCode { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string ShelfLife { get; set; }

        /// <summary>
        /// �Ƿ������Ʒ
        /// </summary>
        public bool IsImportedGoods { get; set; }

        /// <summary>
        /// �Ƿ��ݳ�Ʒ
        /// </summary>
        public bool IsLuxury { get; set; }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        public bool IsBannedPurchase { get; set; }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        public bool IsBannedSale { get; set; }

        /// <summary>
        /// ͼƬ·��
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// ��Ʒ���״̬(0:���δͨ��;1:���ɹ��������;2:���ʹܲ����;3:������������;4:���ͨ��;)
        /// </summary>
        public int GoodsAuditState { get; set; }

        /// <summary>
        /// ��Ʒ��˱�ע
        /// </summary>
        public string GoodsAuditStateMemo { get; set; }

        public int? MedicineDosageFormType { get; set; }

        public int MedicineLibraryManageType { get; set; }

        public int MedicineStorageModeType { get; set; }
    }
}
