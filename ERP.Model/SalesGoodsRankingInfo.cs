using System;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ��Ʒ��������
    /// </summary>
    [Serializable]
    public class SalesGoodsRankingInfo
    {
        /// <summary>
        /// ��ƷId
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// ��Ʒ���
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// ��Ʒ��λ
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// ָ��ʱ����ڵ�������
        /// </summary>
        public double SalesNumber { get; set; }

        /// <summary>
        /// ָ��ʱ����ڵ�0Ԫ������
        /// </summary>
        public int ZeroNumber { get; set; }

        /// <summary>
        /// ��Ʒ�۸�
        /// </summary>
        public decimal GoodsPrice { get; set; }

        /// <summary>
        /// ���ۼ۸�
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public DateTime DayTime { get; set; }

        /// <summary>
        /// ����ƽ̨
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// ��Ʒϵ��
        /// </summary>
        public Guid SeriesId { get; set; }

        /// <summary>
        /// ��ƷƷ��
        /// </summary>
        public Guid BrandId { get; set; }

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public SalesGoodsRankingInfo() { }

        /// <summary>
        /// ��ʼ�����캯��
        /// </summary>
        /// <param name="goodsId">��ƷId</param>
        /// <param name="goodsName">��Ʒ����</param>
        /// <param name="goodsCode">��Ʒ���</param>
        /// <param name="units">��Ʒ��λ</param>
        /// <param name="salesNumber">ָ��ʱ����ڵ�������</param>
        public SalesGoodsRankingInfo(Guid goodsId, string goodsName, string goodsCode, string units, double salesNumber)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Units = units;
            SalesNumber = salesNumber;
        }
    }
}
