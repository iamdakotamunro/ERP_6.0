using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// ���Ԥ����ӿ�
    /// </summary>
    public interface IStockWarning
    {
        /// <summary>
        /// ��ȡָ��ʱ�䷶Χ��ָ����˾ĳ��Ʒ�¾����Ʒ�Ŀ��Ԥ����Ϣ
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="realGoodsList">����Ʒ����</param>
        /// <param name="days"> </param>
        /// <returns></returns>
        IList<StockWarningInfo> GetStockWarningList(Guid warehouseId, Guid hostingFilialeId,List<Guid> realGoodsList, int days);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realGoodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        IList<GoodsDaySalesInfo> GetGoodsDaySalesInfos(List<Guid> realGoodsIdList, Guid warehouseId,DateTime startTime,DateTime endTime,Guid saleFilialeId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realGoodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        IList<GoodsDaySalesInfo> GetGoodsDaySalesInfos(List<Guid> realGoodsIdList, Guid warehouseId, DateTime startTime, DateTime endTime, IEnumerable<Guid> saleFilialeId);

        /// <summary>
        /// ��ȡ��Ʒ��������
        /// </summary>
        /// <param name="realGoodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="dateTime"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        Dictionary<Guid,int> GetSaleDays(List<Guid> realGoodsIdList, Guid warehouseId, DateTime dateTime,
            Guid saleFilialeId);

        /// <summary>
        /// ��ȡδ��ɵĳ�����
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        Dictionary<Guid, int> GetSubtotalQuantity(Guid warehouseId, List<Guid> realGoodsIdList, List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates);

        List<StorageLackQuantity> GetSubtotalStorageRecord(Guid warehouseId, Guid realGoodsId,
            List<StorageRecordType> storageRecordTypes, List<StorageRecordState> storageRecordStates);
    }
}
