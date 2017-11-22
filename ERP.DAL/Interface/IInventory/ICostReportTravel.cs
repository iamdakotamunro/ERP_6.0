using ERP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICostReportTravel
    {
        #region .对本表的维护.
        #region select data

        /// <summary>
        /// 返回lmShop_CostReportTravel表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<CostReportTravelInfo> GetAlllmShop_CostReportTravel();

        /// <summary>
        /// 根据ReportId返回lmShop_CostReportTravel表的数据 
        /// </summary>
        /// <returns></returns>        
        List<CostReportTravelInfo> GetlmShop_CostReportTravelByReportId(Guid reportId);
        /// <summary>
        /// 根据lmShop_CostReportTravel表的TravelId字段返回数据 
        /// </summary>
        /// <param name="travelId">TravelId</param>
        /// <returns></returns>       
        CostReportTravelInfo GetlmShop_CostReportTravelByTravelId(Guid travelId);
        #endregion
        #region delete data

        /// <summary>
        /// 根据lmShop_CostReportTravel表的TravelId字段删除数据 
        /// </summary>
        /// <param name="travelId">travelId</param>
        /// <returns></returns>        
        bool DeletelmShop_CostReportTravelByTravelId(Guid travelId);

        /// <summary>
        /// 根据lmShop_CostReportTravel表的ReportId字段删除数据 
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>        
        bool DeletelmShop_CostReportTravelByReportId(Guid reportId);
        #endregion
        #region update data
        /// <summary>
        /// 根据lmShop_CostReportTravel表的TravelId字段更新数据 
        /// </summary> 
        /// <param name="lmShopCostReportTravel">lmShopCostReportTravel</param>
        /// <returns></returns>       
        bool UpdatelmShop_CostReportTravelByTravelId(CostReportTravelInfo lmShopCostReportTravel);
        #endregion
        #region insert data
        /// <summary>
        /// 向lmShop_CostReportTravel表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="lmShopCostReportTravel">CostReportTravelInfo</param>       
        /// <returns></returns>        
        bool AddlmShop_CostReportTravel(CostReportTravelInfo lmShopCostReportTravel);

        /// <summary>
        /// 批量插入差旅费
        /// </summary>
        /// <param name="costReportTravelInfoList"></param>
        /// <returns></returns>
        bool AddBatchlmShop_CostReportTravel(IList<CostReportTravelInfo> costReportTravelInfoList);
        #endregion

        #endregion
    }
}
