using ERP.Model;
using System;
using System.Collections.Generic;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICostReportAmount
    {
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回lmShop_CostReportAmount表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<CostReportAmountInfo> GetAlllmShop_CostReportAmount();
        /// <summary>
        /// 根据ReportId返回lmShop_CostReportAmount表的数据 
        /// </summary>
        /// <returns></returns>        
        List<CostReportAmountInfo> GetmShop_CostReportAmountByReportId(Guid reportId);
        /// <summary>
        /// 根据lmShop_CostReportAmount表的AmountId字段返回数据 
        /// </summary>
        /// <param name="amountId">AmountId</param>
        /// <returns></returns>       
        CostReportAmountInfo GetlmShop_CostReportAmountByAmountId(Guid amountId);

        /// <summary>
        /// 根据ReportId查询该条费用申报数据对应的最大的申请次数
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>       
        int GetMaxNumFromlmShop_CostReportAmountByReportId(Guid reportId);
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmShop_CostReportAmount表的AmountId字段删除数据 
        /// </summary>
        /// <param name="amountId">terminiId</param>
        /// <returns></returns>        
        bool DeletelmShop_CostReportAmountByAmountId(Guid amountId);
        /// <summary>
        /// 根据lmShop_CostReportAmount表的ReportId字段删除数据 
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>        
        bool DeletelmShop_CostReportAmountByReportId(Guid reportId);
        #endregion
        #region update data
        /// <summary>
        /// 根据lmShop_CostReportAmount表的AmountId字段更新数据 
        /// </summary> 
        /// <param name="lmShopCostReportAmount">lmShopCostReportAmount</param>
        /// <returns></returns>       
        int UpdatelmShop_CostReportAmountByAmountId(CostReportAmountInfo lmShopCostReportAmount);
        #endregion
        #region insert data
        /// <summary>
        /// 向lmShop_CostReportAmount表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="lmShopCostReportAmount">CostReportAmountInfo</param>       
        /// <returns></returns>        
        bool AddlmShop_CostReportAmount(CostReportAmountInfo lmShopCostReportAmount);

        /// <summary>
        /// 向lmshop_CostReportAmount表批量插入数据
        /// </summary>
        /// <param name="lmshopCostReportAmountList"></param>
        /// <returns></returns>
        bool AddBatchlmshop_CostReportAmount(List<CostReportAmountInfo> lmshopCostReportAmountList);

        #endregion

        #endregion
    }
}
