using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IManuallyCheckBill
    {
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回ManuallyCheckBill表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<ManuallyCheckBillInfo> GetAllManuallyCheckBill();

        /// <summary>
        /// 根据条件获取对账记录
        /// </summary>
        /// <param name="checkBillPersonnelId"></param>
        /// <param name="tradeCode"></param>
        /// <param name="checkState"></param>
        /// <param name="checkBillDateStart"></param>
        /// <param name="checkBillDateEnd"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="receiptState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<ManuallyCheckBillInfo> GetAllManuallyCheckBill(Guid checkBillPersonnelId, string tradeCode,CheckType checkState, DateTime checkBillDateStart, DateTime checkBillDateEnd, Guid salePlatformId,int receiptState, int pageIndex, int pageSize, out int total);
        /// <summary>
        /// 根据ManuallyCheckBill表的id字段返回数据  
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        List<ManuallyCheckBillInfo> GetManuallyCheckBillById(Guid id);
        #endregion
        #region delete data
        /// <summary>
        /// 根据ManuallyCheckBill表的id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        bool DeleteManuallyCheckBillById(Guid id);
        #endregion
        #region update data
        /// <summary>
        /// 根据ManuallyCheckBill表的Id字段更新数据 
        /// </summary> 
        /// <param name="manuallyCheckBillInfo">manuallyCheckBillInfo</param>
        /// <returns></returns>       
        bool UpdateManuallyCheckBillByBillId(ManuallyCheckBillInfo manuallyCheckBillInfo);
        #endregion
        #region insert data
        /// <summary>
        /// 向ManuallyCheckBill表插入一条数据
        /// </summary>
        /// <param name="manuallyCheckBillInfo">manuallyCheckBillInfo</param>       
        /// <returns></returns>        
        bool AddManuallyCheckBill(ManuallyCheckBillInfo manuallyCheckBillInfo);
        #endregion
        #endregion
    }
}
