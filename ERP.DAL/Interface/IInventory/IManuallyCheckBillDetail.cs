using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IManuallyCheckBillDetail
    {
        #region .对本表的维护.
        #region select data

        /// <summary>
        /// 返回ManuallyCheckBillDetail表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<ManuallyCheckBillDetailInfo> GetAllManuallyCheckBillDetail();

        /// <summary>
        /// 根据ManuallyCheckBillDetail表的id字段返回数据  
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        List<ManuallyCheckBillDetailInfo> GetManuallyCheckBillDetailById(Guid id);

        /// <summary>
        /// 根据ManuallyCheckBillDetail表的ManuallyCheckBillId字段返回数据  
        /// </summary>
        /// <param name="manuallyCheckBillId"></param>
        /// <param name="orderNo">系统订单号或者第三方订单号</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>        
        List<ManuallyCheckBillDetailInfo> GetManuallyCheckBillDetailByManuallyCheckBillId(Guid manuallyCheckBillId,string orderNo, int pageIndex, int pageSize, out int total);
        #endregion
        #region delete data
        /// <summary>
        /// 根据ManuallyCheckBillDetail表的id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        bool DeleteManuallyCheckBillDetailById(Guid id);
        #endregion
        #region update data
        /// <summary>
        /// 根据ManuallyCheckBillDetail表的Id字段更新数据 
        /// </summary> 
        /// <param name="manuallyCheckBillDetailInfo">ManuallyCheckBillDetailInfo</param>
        /// <returns></returns>       
        bool UpdateManuallyCheckBillDetailById(ManuallyCheckBillDetailInfo manuallyCheckBillDetailInfo);
        #endregion
        #region insert data
        /// <summary>
        /// 向ManuallyCheckBillDetail表插入一条数据
        /// </summary>
        /// <param name="manuallyCheckBillDetailInfo">ManuallyCheckBillDetailInfo</param>       
        /// <returns></returns>        
        bool AddManuallyCheckBillDetail(ManuallyCheckBillDetailInfo manuallyCheckBillDetailInfo);
        /// <summary>
        /// 批量插入人工对账明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddBatchManuallyCheckBillDetail(IList<ManuallyCheckBillDetailInfo> list);
        #endregion

        #endregion
    }
}
