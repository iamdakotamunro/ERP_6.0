//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2008��5��16��
// �ļ�������:����
// ����޸�ʱ��:2008��5��16��
// ���һ���޸���:����
//================================================

using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// ��Ʒ������
    /// </summary>
    [Serializable]
    public class GoodsDemandInfo
    {
        /// <summary>
        /// ��ƷID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// ����ƷID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        /// ��Ʒ��
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// ��Ʒ���
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public int Demand { get; set; }

        /// <summary>
        /// ��ǰ�ֿ����Ʒ���
        /// </summary>
        public double NonceWarehouseGoodsStock { get; set; }

        /// <summary>
        /// ��ǰ�ֹ�˾����Ʒ���
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
        /// ��˾ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ��˾��
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public int PurchaseQuantity { get; set; }

        /// <summary>
        /// ȱ����
        /// </summary>
        public Guid NeedWarehouseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsDemandInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">��ƷID</param>
        /// <param name="goodsName">��Ʒ��</param>
        /// <param name="goodsCode">��Ʒ���</param>
        /// <param name="specification">���</param>
        /// <param name="demand">����</param>
        /// <param name="nonceFilialeGoodsStock">��ǰ�ֹ�˾����Ʒ���</param>
        /// <param name="nonceGoodsStock">��ǰ��Ʒ���</param>
        /// <param name="unitPrice">����</param>
        /// <param name="companyName">��˾��</param>
        public GoodsDemandInfo(Guid goodsId, string goodsName, string goodsCode, string specification, int demand, double nonceFilialeGoodsStock, double nonceGoodsStock, decimal unitPrice, string companyName)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            Demand = demand;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            UnitPrice = unitPrice;
            CompanyName = companyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">��ƷID</param>
        /// <param name="goodsName">��Ʒ��</param>
        /// <param name="goodsCode">��Ʒ���</param>
        /// <param name="specification">���</param>
        /// <param name="demand">����</param>
        /// <param name="nonceWarehouseGoodsStock">��ǰ�ֿ����Ʒ���</param>
        /// <param name="nonceFilialeGoodsStock">��ǰ�ֹ�˾����Ʒ���</param>
        /// <param name="nonceGoodsStock">��ǰ��Ʒ���</param>
        /// <param name="unitPrice">����</param>
        /// <param name="companyName">��˾��</param>
        public GoodsDemandInfo(Guid goodsId, string goodsName, string goodsCode, string specification, int demand, double nonceWarehouseGoodsStock, double nonceFilialeGoodsStock, double nonceGoodsStock, decimal unitPrice, string companyName)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            Demand = demand;
            NonceWarehouseGoodsStock = nonceWarehouseGoodsStock;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceGoodsStock = nonceGoodsStock;
            UnitPrice = unitPrice;
            CompanyName = companyName;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is GoodsDemandInfo)
                return (obj as GoodsDemandInfo).GoodsId == GoodsId && (obj as GoodsDemandInfo).Specification == Specification;
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
