using System;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.IStorage;

namespace StorageTask.Core
{
    public class AddStorageTaskManager
    {
        private static readonly IASYNStorageRecordDao _iasynStorageRecordDao = new ASYNStorageRecordDao(ERP.Environment.GlobalConfig.DB.FromType.Write);
        private static readonly StorageManager _asynStorageRecordDao = StorageManager.WriteInstance;

        /// <summary>
        /// 添加销售出库单据
        /// </summary>
        public static void AddSellOutTask()
        {
            var list = _iasynStorageRecordDao.GetList(ConfigInfo.NeedAddStorageReadQuantity);
            if (list == null || list.Count == 0) return;
            foreach (var item in list)
            {
                try
                {
                    string errorMessage;
                    var isSuccess = _asynStorageRecordDao.AddBySellStockOut(item.IdentifyKey, item.IdentifyId,out errorMessage);
                    if (isSuccess)
                    {
                        //删除异步表数据
                        _iasynStorageRecordDao.Delete(item.ID);
                    }
                    if (!isSuccess && !string.IsNullOrEmpty(errorMessage))
                    {
                        ERP.SAL.LogCenter.LogService.LogError(string.Format("添加销售出库单据异常:{0}, {1}", errorMessage, Framework.Common.Serialization.JsonSerialize(item)), "出入库任务", null);
                    }
                }
                catch (Exception exp)
                {
                    ERP.SAL.LogCenter.LogService.LogError(string.Format("添加销售出库单据异常:{0}, {1}", exp.Message, Framework.Common.Serialization.JsonSerialize(item)), "出入库任务", exp);
                    continue;
                }
            }
        }
    }
}
