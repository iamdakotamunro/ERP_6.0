using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Interface
{
    /// <summary>
    /// 出入库记录处理
    /// </summary>
    public interface IStorageManager
    {
        /// <summary>
        /// 添加销售出库单据
        /// ADD 阮剑锋 2014.4.8
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="isValidateStock">是否验证库存</param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="errorMessage">返回的错误信息</param>
        bool AddBySellStockOut(string orderNo, Guid orderId, bool isValidateStock, out string errorMessage);




        /// <summary>
        /// 获取出入库异步表
        /// </summary>
        /// <returns></returns>
        IList<Model.ASYN.ASYNStorageRecordInfo> GetAsynList(int top);

        /// <summary>
        /// 删除异步数据
        /// </summary>
        /// <param name="asynId"></param>
        /// <returns></returns>
        bool DeleteAsyn(Guid asynId);

        /// <summary>
        /// 插入异步数据信息
        /// </summary>
        /// <param name="asynStorageRecordInfo"></param>
        /// <returns></returns>
        bool InsertAsyn(Model.ASYN.ASYNStorageRecordInfo asynStorageRecordInfo);
    }
}
