using System;
using System.Collections.Generic;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IGoods
{
    /// <summary>
    /// ��Ʒ��ǰ�����Ϣ
    /// </summary>
    public interface IGoodsStockPile
    {

        /// <summary>�õ�ƽ�������ת 2015-04-29  ������
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <param name="goodsIds">��ƷID����</param>
        /// <param name="state">0ȫ����1�¼ܻ�ȱ�����п�棬2��������Ʒ</param>
        /// <returns></returns>
        IList<StockTurnOverInfo> GetAvgStockTurnOver(DateTime startTime, DateTime endTime, Guid warehouseId, List<Guid> goodsIds, int state);

        /// <summary>��ȡ��Ʒ�Ŀ����ת��  2015-04-30 ������
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="goodsId">��ƷID</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <returns></returns>
        IList<StockTurnOverInfo> GetGoodsStockTurnOverByGoodsId(DateTime startTime, DateTime endTime, Guid goodsId, Guid warehouseId);

        /// <summary>��ȡ��Ʒ��3�������������  2015-06-16  ������
        /// </summary>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <returns></returns>
        IList<SalesVolumeInfo> GetGoodsSalesVolume(Guid warehouseId);
    }
}
