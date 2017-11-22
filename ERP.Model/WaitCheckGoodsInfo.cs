using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    ///lmshop_WaitCheckGoods ���̵���Ʒ��
    /// </summary>
    [Serializable]
    public class WaitCheckGoodsInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public WaitCheckGoodsInfo() { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="goodsId">��ƷID</param>
        /// <param name="goodsName">��Ʒ����</param>
        /// <param name="state">״̬</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        public WaitCheckGoodsInfo(Guid goodsId, string goodsName, int state, Guid warehouseId)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            State = state;
            WarehouseId = warehouseId;
        }

        /// <summary>
        ///��ƷID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        ///��Ʒ����
        /// </summary>
        public String GoodsName { get; set; }

        /// <summary>
        ///״̬
        /// </summary>
        public Int32 State { get; set; }

        /// <summary>
        ///�ֿ�ID
        /// </summary>
        public Guid WarehouseId { get; set; }

    }
}
