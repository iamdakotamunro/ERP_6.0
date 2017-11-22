using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    ///lmshop_CheckStockDetail �̵�ƻ���ϸ��
    /// </summary>
    [Serializable]
    public class CheckStockDetailInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CheckStockDetailInfo() { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="checkID">�̵�ƻ�ID</param>
        /// <param name="goodsId">����ƷID</param>
        /// <param name="realGoodsId">����ƷID</param>
        /// <param name="goodsName">��Ʒ����</param>
        /// <param name="specification">���</param>
        /// <param name="shelfNo">��λ��</param>
        /// <param name="goodsStock">���</param>
        /// <param name="firstCountAmount">��������</param>
        /// <param name="secondCountAmount">��������</param>
        /// <param name="secondCheckAmount">�������</param>
        public CheckStockDetailInfo(Guid checkID, Guid goodsId, Guid realGoodsId, String goodsName, String specification, String shelfNo,
            Int32 goodsStock, Int32 firstCountAmount, Int32 secondCountAmount, Int32 secondCheckAmount)
        {
            CheckID = checkID;
            GoodsId = goodsId;
            RealGoodsId = realGoodsId;
            GoodsName = goodsName;
            Specification = specification;
            ShelfNo = shelfNo;
            GoodsStock = goodsStock;
            FirstCountAmount = firstCountAmount;
            SecondCountAmount = secondCountAmount;
            SecondCheckAmount = secondCheckAmount;
        }

        /// <summary>
        /// ���캯��������ã�
        /// </summary>
        /// <param name="checkID">�̵�ƻ�ID</param>
        /// <param name="goodsId">����ƷID</param>
        /// <param name="realGoodsId">����ƷID</param>
        /// <param name="goodsName">��Ʒ����</param>
        /// <param name="specification">���</param>
        /// <param name="shelfNo">��λ��</param>
        public CheckStockDetailInfo(Guid checkID, Guid goodsId, Guid realGoodsId, String goodsName, String specification, String shelfNo)
        {
            CheckID = checkID;
            GoodsId = goodsId;
            RealGoodsId = realGoodsId;
            GoodsName = goodsName;
            Specification = specification;
            ShelfNo = shelfNo;
        }

        /// <summary>
        ///�̵�ƻ�ID
        /// </summary>
        public Guid CheckID { get; set; }

        /// <summary>
        ///����ƷID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        ///����ƷID
        /// </summary>
        public Guid RealGoodsId { get; set; }

        /// <summary>
        ///��Ʒ����
        /// </summary>
        public String GoodsName { get; set; }

        /// <summary>
        ///���
        /// </summary>
        public String Specification { get; set; }

        /// <summary>
        ///��λ��
        /// </summary>
        public String ShelfNo { get; set; }
        /// <summary>
        ///���
        /// </summary>
        public Int32 GoodsStock { get; set; }

        /// <summary>
        ///��������
        /// </summary>
        public Int32 FirstCountAmount { get; set; }

        /// <summary>
        ///��������
        /// </summary>
        public Int32 SecondCountAmount { get; set; }

        /// <summary>
        ///�������
        /// </summary>
        public Int32 SecondCheckAmount { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string GoodsCode { get; set; }

    }
}
