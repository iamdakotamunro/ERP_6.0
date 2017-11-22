using ERP.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace ERP.Service.Contract
{
    /// <summary>
    /// 仅用于测试
    /// </summary>
    [ServiceContract]
    public interface ITestForJMeter
    {
        /// <summary>
        /// 插入出入库单据
        /// </summary>
        /// <param name="storageRecord"></param>
        /// <param name="storageRecordDetail"></param>
        /// <returns></returns>
        [OperationContract]
        ResultInfo InsertStorageRecord(StorageRecordInfo storageRecord, IList<StorageRecordDetailInfo> storageRecordDetail);
    }
}
