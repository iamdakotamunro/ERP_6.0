using ERP.Service.Contract;
using System;
using System.Collections.Generic;
using ERP.Model;
using ERP.BLL.Implement.Inventory;

namespace ERP.Service.Implement
{
    public class TestForJMeter : ITestForJMeter
    {
        private StorageManager _storageManager = StorageManager.WriteInstance;

        public ResultInfo InsertStorageRecord(StorageRecordInfo storageRecord, IList<StorageRecordDetailInfo> storageRecordDetails)
        {
            try
            {
                _storageManager.NewInsertStockAndGoods(storageRecord, storageRecordDetails);
                return ResultInfo.Success();
            }
            catch (Exception ex)
            {
                return ResultInfo.Failure(ex.Message);
            }
        }
    }
}
