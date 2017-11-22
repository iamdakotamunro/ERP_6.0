using ERP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICostReportTermini
    {
        #region .对本表的维护.
        #region select data

        /// <summary>
        /// 返回lmShop_CostReportTermini表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<CostReportTerminiInfo> GetAlllmShop_CostReportTermini();

        /// <summary>
        /// 根据ReportId返回lmShop_CostReportTermini表的数据 
        /// </summary>
        /// <returns></returns>        
        List<CostReportTerminiInfo> GetmShop_CostReportTerminiByReportId(Guid reportId);
        /// <summary>
        /// 根据lmShop_CostReportTermini表的TerminiId字段返回数据 
        /// </summary>
        /// <param name="terminiId">TerminiId</param>
        /// <returns></returns>       
        CostReportTerminiInfo GetlmShop_CostReportTerminiByTerminiId(Guid terminiId);
        #endregion
        #region delete data

        /// <summary>
        /// 根据lmShop_CostReportTermini表的TerminiId字段删除数据 
        /// </summary>
        /// <param name="terminiId">terminiId</param>
        /// <returns></returns>        
        bool DeletelmShop_CostReportTerminiByTerminiId(Guid terminiId);

        /// <summary>
        /// 根据lmShop_CostReportTermini表的ReportId字段删除数据 
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>        
        bool DeletelmShop_CostReportTerminiByReportId(Guid reportId);
        #endregion
        #region update data
        /// <summary>
        /// 根据lmShop_CostReportTermini表的TerminiId字段更新数据 
        /// </summary> 
        /// <param name="lmShopCostReportTermini">lmShopCostReportTermini</param>
        /// <returns></returns>       
        int UpdatelmShop_CostReportTerminiByTerminiId(CostReportTerminiInfo lmShopCostReportTermini);
        #endregion
        #region insert data

        /// <summary>
        /// 向lmShop_CostReportTermini表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="lmShopCostReportTermini">CostReportTerminiInfo</param>       
        /// <returns></returns>        
        bool AddlmShop_CostReportTermini(CostReportTerminiInfo lmShopCostReportTermini);

        /// <summary>
        /// 批量插入起讫
        /// </summary>
        /// <param name="costReportTerminiInfoList"></param>
        /// <returns></returns>
        bool AddBatchlmShop_CostReportTermini(IList<CostReportTerminiInfo> costReportTerminiInfoList);
        #endregion

        #endregion
    }
}
