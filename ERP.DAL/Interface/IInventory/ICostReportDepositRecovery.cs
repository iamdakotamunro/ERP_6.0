using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/7/5 10:09:42 
     * 描述    : 押金回收接口类
     * =====================================================================
     * 修改时间：2016/7/5 10:09:42 
     * 修改人  ：  
     * 描述    ：
     */
    public interface ICostReportDepositRecovery
    {
        /// <summary>
        /// 添加押金回收
        /// </summary>
        /// <param name="info">押金回收详细模型</param>
        bool InsertDepositRecovery(CostReportDepositRecoveryInfo info);

        /// <summary>
        /// 根据费用申报ID获得押金回收记录
        /// </summary>
        /// <returns></returns>
        List<CostReportDepositRecoveryInfo> GetDepositRecoveryList(Guid reportId);
    }
}
