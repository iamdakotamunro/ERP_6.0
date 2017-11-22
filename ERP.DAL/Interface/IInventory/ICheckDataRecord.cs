using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICheckDataRecord
    {
        /// <summary>
        /// 插入对账信息
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <returns></returns>
        void InsertData(CheckDataRecordInfo dataInfo);

        /// <summary>
        /// 添加对账明细
        /// </summary>
        /// <param name="dataDetailInfo"></param>
        /// <returns></returns>
        void InsertDataDetail(CheckDataDetailInfo dataDetailInfo);

        /// <summary>
        /// 修改插入记录明细
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="expressNo"></param>
        /// <param name="postMoney"></param>
        /// <param name="postWeight"></param>
        void UpdateDataDetail(Guid checkId, string expressNo, decimal postMoney, decimal postWeight);

        /// <summary>
        /// 删除对账明细
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        void DeleteDataDetail(Guid checkId);

        /// <summary>
        /// 更新对账结果
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <returns></returns>
        void UpdateResult(CheckDataRecordInfo dataInfo);

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="checkDataState"></param>
        void UpdateState(Guid checkId,int checkDataState);

        /// <summary>
        /// 删除对账信息
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        void DeleteData(Guid checkId);

        /// <summary>
        /// 判断是否存在非删除的对账记录
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="checkType"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool IsExistCheckData(Guid companyId, int checkType, string filePath);

        /// <summary>
        /// 获取对帐信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CheckDataRecordInfo GetCheckDataInfoById(Guid id);

        /// <summary>
        /// 获取对账明细记录
        /// </summary>
        /// <param name="checkIds"></param>
        /// <param name="dataStates"> </param>
        /// <param name="count"> </param>
        /// <returns></returns>
        IList<CheckDataDetailInfo> GetCheckDataDetailList(IList<Guid> checkIds, int[] dataStates,int count);

        /// <summary>
        /// 获取选定的对账记录（用于对账完成,多条对账记录一起处理）
        /// </summary>
        /// <param name="checkIds"></param>
        /// <returns></returns>
        IList<CheckDataRecordInfo> GetCheckDataRecordList(IList<Guid> checkIds);
 
        /// <summary>
        /// 获取对账信息列表
        /// </summary>
        /// <param name="companyId">快递公司</param>
        /// <param name="checkType">对账类型()</param>
        /// <param name="searchKey">关键字查询，文件名</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="states">对账状态</param>
        /// <returns></returns>
        IList<CheckDataRecordInfo> GetCheckDataList(Guid companyId, int checkType, string searchKey, DateTime startTime,DateTime endTime,int[] states);

        /// <summary>
        /// 更改详细状态
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="dataState"></param>
        /// <param name="oldDataState"></param>
        /// <param name="expressNo"></param>
        /// <param name="confirmMoney"></param>
        /// <param name="financeConfirmMoney"> </param>
        void UpdateDataState(Guid checkId, int dataState,int oldDataState, string expressNo, decimal confirmMoney, decimal financeConfirmMoney);

        /// <summary>
        /// 获取对账的实际总金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        IList<ExceptionReckoningInfo> GetTotalMoney(Guid checkId);

        /// <summary>
        /// 批量将数据表中的数据插入的备份表中
        /// </summary>
        ///<param name="checkId"></param>
        void DataToTemp(Guid checkId);

        /// <summary>
        /// 批量插入快递明细
        /// </summary>
        /// <param name="addList"></param>
        /// <param name="tableName"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        int BitchInsert(IList<CheckDataDetailInfo> addList, string tableName, Dictionary<string, string> dics);

        /// <summary>获取对比数据  2015-04-03  陈重文
        /// </summary>  
        /// <param name="recordInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="checkType">对账状态</param>
        /// <returns></returns>
        IList<CheckDataDetailInfo> GetContrastDataList(CheckDataRecordInfo recordInfo, DateTime startTime, DateTime endTime, CheckType checkType);

        /// <summary>
        /// 删除确认数据
        /// </summary>
        /// <param name="checkId"></param>
        void DeleteConfirmData(Guid checkId);

        /// <summary>
        /// 确认数据批量转移
        /// </summary>
        /// <param name="checkId"></param>
        void TransferConfirmData(Guid checkId);
    }
}
