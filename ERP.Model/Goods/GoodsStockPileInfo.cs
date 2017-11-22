//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��6��8��
// �ļ�������:����
// ����޸�ʱ��:2007��6��8��
// ���һ���޸���:����
//================================================

using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// ��Ʒ����¼��Ϣ
    /// </summary>
    [Serializable]
    public class GoodsStockPileInfo
    {
        /// <summary>
        /// ����ƷID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// ��Ʒ��
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// ������λ
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// ��ǰ�ֿ���Ʒ���
        /// </summary>
        public double NonceWarehouseGoodsStock { get; set; }

        /// <summary>
        /// ��ǰ�ֹ�˾��Ʒ���
        /// </summary>
        public double NonceFilialeGoodsStock { get; set; }

        /// <summary>
        /// ��ǰ��Ʒ���
        /// </summary>
        public double NonceGoodsStock { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// ���һ�ν���
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        public DateTime? RecentInDate { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public int WaitConsignmentedGoodsStock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsStockPileInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">��ƷID</param>
        /// <param name="goodsName">��Ʒ��</param>
        /// <param name="goodsCode">��Ʒ����</param>
        /// <param name="units">������λ</param>
        /// <param name="specification">���</param>
        /// <param name="nonceFilialeGoodsStock">��ǰ�ֹ�˾��Ʒ���</param>
        /// <param name="nonceGoodsStock">��ǰ��Ʒ���</param>
        /// <param name="unitPrice">����</param>
        /// <param name="sellPrice">���ۼ�</param>
        /// <param name="nonceWarehouseGoodsStock">��ǰ�ֿ���Ʒ���</param>
        public GoodsStockPileInfo(Guid goodsId, string goodsName, string goodsCode, string units, string specification, double nonceFilialeGoodsStock, double nonceGoodsStock, decimal unitPrice, decimal sellPrice, double nonceWarehouseGoodsStock)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Units = units;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            UnitPrice = unitPrice;
            SellPrice = sellPrice;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
        }
    }
}
