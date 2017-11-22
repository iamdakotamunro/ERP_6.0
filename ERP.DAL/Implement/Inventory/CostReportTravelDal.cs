//Author: zal
//createtime:2016-8-5 14:50:46
//Description:

using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.Model;
using Keede.DAL.Helper;
using ERP.Environment;
using ERP.DAL.Interface.IInventory;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 该类提供了一系列操作 "lmShop_CostReportTravel" 表的方法;
    /// </summary>
    public class CostReportTravelDal : ICostReportTravel
    {
        public CostReportTravelDal(GlobalConfig.DB.FromType fromType)
        {

        }
        const string SQL_SELECT = "select [TravelId],[ReportId],[Entourage],[TravelAddressAndCourse],[Matter],[DayOrNum],[Amount],[OperatingTime] from [lmShop_CostReportTravel] ";
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回lmShop_CostReportTravel表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<CostReportTravelInfo> GetAlllmShop_CostReportTravel()
        {
            List<CostReportTravelInfo> lmShopCostReportTravelList = new List<CostReportTravelInfo>();
            IDataReader reader = null;

            string sql = SQL_SELECT;
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                CostReportTravelInfo lmShopCostReportTravel = new CostReportTravelInfo(reader);
                lmShopCostReportTravelList.Add(lmShopCostReportTravel);
            }
            reader.Close();
            return lmShopCostReportTravelList;
        }

        /// <summary>
        /// 根据ReportId返回lmShop_CostReportTravel表的数据 
        /// </summary>
        /// <returns></returns>        
        public List<CostReportTravelInfo> GetlmShop_CostReportTravelByReportId(Guid reportId)
        {
            List<CostReportTravelInfo> lmShopCostReportTravelList = new List<CostReportTravelInfo>();
            
            string sql = SQL_SELECT + "where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            while (reader.Read())
            {
                CostReportTravelInfo lmShopCostReportTravel = new CostReportTravelInfo(reader);
                lmShopCostReportTravelList.Add(lmShopCostReportTravel);
            }
            reader.Close();
            return lmShopCostReportTravelList;
        }
        /// <summary>
        /// 根据lmShop_CostReportTravel表的TravelId字段返回数据 
        /// </summary>
        /// <param name="travelId">TravelId</param>
        /// <returns></returns>       
        public CostReportTravelInfo GetlmShop_CostReportTravelByTravelId(Guid travelId)
        {
            CostReportTravelInfo lmShopCostReportTravel = null;
            IDataReader reader = null;

            string sql = SQL_SELECT + "where [TravelId] = @TravelId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@TravelId",travelId)
            };
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            if (reader.Read())
            {
                lmShopCostReportTravel = new CostReportTravelInfo(reader);
            }
            reader.Close();
            return lmShopCostReportTravel;
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmShop_CostReportTravel表的TravelId字段删除数据 
        /// </summary>
        /// <param name="travelId">travelId</param>
        /// <returns></returns>        
        public bool DeletelmShop_CostReportTravelByTravelId(Guid travelId)
        {
            string sql = "delete from [lmShop_CostReportTravel] where [TravelId] = @TravelId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@TravelId",travelId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmShop_CostReportTravel表的ReportId字段删除数据 
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>        
        public bool DeletelmShop_CostReportTravelByReportId(Guid reportId)
        {
            string sql = "delete from [lmShop_CostReportTravel] where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras)>0;
        }
        #endregion
        #region update data
        /// <summary>
        /// prepare parameters 
        /// </summary>
        public SqlParameter[] PrepareCommandParameters(CostReportTravelInfo lmShopCostReportTravel)
        {
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@TravelId",lmShopCostReportTravel.TravelId),
            new SqlParameter("@ReportId",lmShopCostReportTravel.ReportId),
            new SqlParameter("@Entourage",lmShopCostReportTravel.Entourage),
            new SqlParameter("@TravelAddressAndCourse",lmShopCostReportTravel.TravelAddressAndCourse),
            new SqlParameter("@Matter",lmShopCostReportTravel.Matter),
            new SqlParameter("@DayOrNum",lmShopCostReportTravel.DayOrNum),
            new SqlParameter("@Amount",lmShopCostReportTravel.Amount),
            new SqlParameter("@OperatingTime",lmShopCostReportTravel.OperatingTime)
            };
            return paras;
        }
        /// <summary>
        /// 根据lmShop_CostReportTravel表的TravelId字段更新数据 
        /// </summary> 
        /// <param name="lmShopCostReportTravel">lmShopCostReportTravel</param>
        /// <returns></returns>       
        public bool UpdatelmShop_CostReportTravelByTravelId(CostReportTravelInfo lmShopCostReportTravel)
        {
            string sql = "update [lmShop_CostReportTravel] set [ReportId] = @ReportId,[Entourage] = @Entourage,[TravelAddressAndCourse] = @TravelAddressAndCourse,[Matter] = @Matter,[DayOrNum] = @DayOrNum,[Amount] = @Amount,[OperatingTime]=@OperatingTime where [TravelId] = @TravelId";
            SqlParameter[] paras = PrepareCommandParameters(lmShopCostReportTravel);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向lmShop_CostReportTravel表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="lmShopCostReportTravel">CostReportTravelInfo</param>       
        /// <returns></returns>        
        public bool AddlmShop_CostReportTravel(CostReportTravelInfo lmShopCostReportTravel)
        {
            string sql = "insert into [lmShop_CostReportTravel]([TravelId],[ReportId],[Entourage],[TravelAddressAndCourse],[Matter],[DayOrNum],[Amount],[OperatingTime])values(@TravelId,@ReportId,@Entourage,@TravelAddressAndCourse,@Matter,@DayOrNum,@Amount,@OperatingTime)    select @@identity";
            SqlParameter[] paras = PrepareCommandParameters(lmShopCostReportTravel);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 批量插入差旅费
        /// </summary>
        /// <param name="costReportTravelInfoList"></param>
        /// <returns></returns>
        public bool AddBatchlmShop_CostReportTravel(IList<CostReportTravelInfo> costReportTravelInfoList)
        {
            var dics = new Dictionary<string, string>
                {
                    {"TravelId","TravelId"},{"ReportId","ReportId"},{"Entourage","Entourage"},{"TravelAddressAndCourse","TravelAddressAndCourse"},
                    {"Matter","Matter"},{"DayOrNum","DayOrNum"},{"Amount","Amount"},{"OperatingTime","OperatingTime"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, costReportTravelInfoList, "lmShop_CostReportTravel", dics) > 0;
        }
        #endregion
        #endregion
    }
}
