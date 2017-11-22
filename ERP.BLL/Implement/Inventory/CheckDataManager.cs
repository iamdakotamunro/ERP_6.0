using System;
using System.Collections.Generic;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class CheckDataManager : BllInstance<CheckDataManager>
    {
        readonly ICheckDataRecord _checkDataRecordDao ;

        public CheckDataManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _checkDataRecordDao = InventoryInstance.GetCheckDataRecordDao(fromType);
        }

        public void InsertData(CheckDataRecordInfo dataInfo)
        {
            _checkDataRecordDao.InsertData(dataInfo);
        }
        
        public void UpdateState(Guid checkId, int checkDataState)
        {
            _checkDataRecordDao.UpdateState(checkId, checkDataState);
        }

        public CheckDataRecordInfo GetCheckDataInfoById(Guid id)
        {
            return _checkDataRecordDao.GetCheckDataInfoById(id);
        }

        public IList<CheckDataRecordInfo> GetCheckDataList(Guid companyId, int checkType, string searchKey, DateTime startTime, DateTime endTime, int[] states)
        {
            return _checkDataRecordDao.GetCheckDataList(companyId, checkType, searchKey, startTime, endTime, states);
        }

        public IList<ExceptionReckoningInfo> GetTotalMoney(Guid checkId)
        {
            return _checkDataRecordDao.GetTotalMoney(checkId);
        }
        
        #region [对账服务引用，勿删 2015-03-26  陈重文]

        public void DeleteDataDetail(Guid checkId)
        {
            _checkDataRecordDao.DeleteDataDetail(checkId);
        }

        public void UpdateResult(CheckDataRecordInfo dataInfo)
        {
            _checkDataRecordDao.UpdateResult(dataInfo);
        }

        /// <summary>批量插入对账数据，对账服务引用  2015-03-26  陈重文
        /// </summary>
        /// <param name="addList"></param>
        /// <param name="tableName"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        public int BitchInsert(IList<CheckDataDetailInfo> addList, string tableName, Dictionary<string, string> dics)
        {
            return _checkDataRecordDao.BitchInsert(addList, tableName, dics);
        }

        /// <summary>更新运费确认数据  对账服务引用  
        /// </summary>
        /// <param name="checkId"></param>
        public void TransferConfirmData(Guid checkId)
        {
            _checkDataRecordDao.TransferConfirmData(checkId);
        }

        /// <summary>删除运费确认数据  对账服务引用 
        /// </summary>
        /// <param name="checkId"></param>
        public void DeleteConfirmData(Guid checkId)
        {
            _checkDataRecordDao.DeleteConfirmData(checkId);
        }

        /// <summary>获取对账数据 对账服务引用  
        /// </summary>
        /// <param name="checkIds"></param>
        /// <param name="dataStates"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IList<CheckDataDetailInfo> GetCheckDataDetailList(IList<Guid> checkIds, int[] dataStates, int count)
        {
            return _checkDataRecordDao.GetCheckDataDetailList(checkIds, dataStates, count);
        }

        /// <summary>
        /// 更改详细状态 对账服务引用  
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="dataState"></param>
        /// <param name="oldDataState"> </param>
        /// <param name="expressNo"></param>
        /// <param name="confirmMoney"> </param>
        /// <param name="financeConfirmMoney"> </param>
        public void UpdateDataState(Guid checkId, int dataState, int oldDataState, string expressNo, decimal confirmMoney, decimal financeConfirmMoney)
        {
            _checkDataRecordDao.UpdateDataState(checkId, dataState, oldDataState, expressNo, confirmMoney, financeConfirmMoney);
        }

        /// <summary>获取对比数据  2015-04-03  陈重文
        /// </summary>  
        /// <param name="recordInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="checkType">对账状态</param>
        /// <returns></returns>
        public IList<CheckDataDetailInfo> GetContrastDataList(CheckDataRecordInfo recordInfo, DateTime startTime, DateTime endTime, CheckType checkType)
        {
            return _checkDataRecordDao.GetContrastDataList(recordInfo, startTime, endTime, checkType);
        }

        /// <summary>
        /// 批量将数据表中的数据插入的备份表中
        /// </summary>
        ///<param name="checkId"></param>
        public void DataToTemp(Guid checkId)
        {
            _checkDataRecordDao.DataToTemp(checkId);
        }

        #endregion
    }
}
