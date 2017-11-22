using System;
using System.Collections.Generic;
using ERP.Model.Goods;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IGoods
{
    /// <summary>
    /// ��Ʒ������Ϣ�ӿ���
    /// </summary>
    public interface IGoodsDemand
    {
        /// <summary>
        /// ��ȡ��Ʒ�Ļ�λ�ţ��ֲֺ�ʹ�ã�
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        string GetGoodsShelfNo(Guid goodsId, Guid filialeId, Guid warehouseId);

        /// <summary>
        /// ɾ������Ʒ����֪ͨ
        /// </summary>
        /// <param name="goodsId"></param>
        void DeleteGoodsStockStatement(Guid goodsId);

        /// <summary>
        /// ��ȡ��Ʒ�����б�
        /// Key����ƷID��Value�����۸���
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<KeyValuePair<Guid, int>> GetGoodsSales(DateTime startTime, DateTime endTime);
    }
}
