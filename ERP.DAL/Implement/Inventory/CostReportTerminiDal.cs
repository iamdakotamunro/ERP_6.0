//Author: zal
//createtime:2016-8-5 14:51:02
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
    /// 该类提供了一系列操作 "lmShop_CostReportTermini" 表的方法;
    /// </summary>
    public class CostReportTerminiDal : ICostReportTermini
    {
        public CostReportTerminiDal(GlobalConfig.DB.FromType fromType)
        {

        }
        const string SQL_SELECT = "select [TerminiId],[ReportId],[StartDay],[EndDay],[TerminiLocation],[OperatingTime] from [lmShop_CostReportTermini] ";
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回lmShop_CostReportTermini表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<CostReportTerminiInfo> GetAlllmShop_CostReportTermini()
        {
            List<CostReportTerminiInfo> lmShopCostReportTerminiList = new List<CostReportTerminiInfo>();
            IDataReader reader = null;

            string sql = SQL_SELECT;
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                CostReportTerminiInfo lmShopCostReportTermini = new CostReportTerminiInfo(reader);
                lmShopCostReportTerminiList.Add(lmShopCostReportTermini);
            }
            reader.Close();
            return lmShopCostReportTerminiList;
        }
        /// <summary>
        /// 根据ReportId返回lmShop_CostReportTravel表的数据 
        /// </summary>
        /// <returns></returns>        
        public List<CostReportTerminiInfo> GetmShop_CostReportTerminiByReportId(Guid reportId)
        {
            List<CostReportTerminiInfo> lmShopCostReportTerminiList = new List<CostReportTerminiInfo>();
            
            string sql = SQL_SELECT + "where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            while (reader.Read())
            {
                CostReportTerminiInfo lmShopCostReportTermini = new CostReportTerminiInfo(reader);
                lmShopCostReportTerminiList.Add(lmShopCostReportTermini);
            }
            reader.Close();
            return lmShopCostReportTerminiList;
        }
        /// <summary>
        /// 根据lmShop_CostReportTermini表的TerminiId字段返回数据 
        /// </summary>
        /// <param name="terminiId">TerminiId</param>
        /// <returns></returns>       
        public CostReportTerminiInfo GetlmShop_CostReportTerminiByTerminiId(Guid terminiId)
        {
            CostReportTerminiInfo lmShopCostReportTermini = null;
            IDataReader reader = null;

            string sql = SQL_SELECT + "where [TerminiId] = @TerminiId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@TerminiId",terminiId)
            };
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            if (reader.Read())
            {
                lmShopCostReportTermini = new CostReportTerminiInfo(reader);
            }
            reader.Close();
            return lmShopCostReportTermini;
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmShop_CostReportTermini表的TerminiId字段删除数据 
        /// </summary>
        /// <param name="terminiId">terminiId</param>
        /// <returns></returns>        
        public bool DeletelmShop_CostReportTerminiByTerminiId(Guid terminiId)
        {
            string sql = "delete from [lmShop_CostReportTermini] where [TerminiId] = @TerminiId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@TerminiId",terminiId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmShop_CostReportTermini表的ReportId字段删除数据 
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>        
        public bool DeletelmShop_CostReportTerminiByReportId(Guid reportId)
        {
            string sql = "delete from [lmShop_CostReportTermini] where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region update data
        /// <summary>
        /// prepare parameters 
        /// </summary>
        public SqlParameter[] PrepareCommandParameters(CostReportTerminiInfo lmShopCostReportTermini)
        {
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@TerminiId",lmShopCostReportTermini.TerminiId),
            new SqlParameter("@ReportId",lmShopCostReportTermini.ReportId),
            new SqlParameter("@StartDay",lmShopCostReportTermini.StartDay),
            new SqlParameter("@EndDay",lmShopCostReportTermini.EndDay),
            new SqlParameter("@TerminiLocation",lmShopCostReportTermini.TerminiLocation),
            new SqlParameter("@OperatingTime",lmShopCostReportTermini.OperatingTime) 
            };
            return paras;
        }
        /// <summary>
        /// 根据lmShop_CostReportTermini表的TerminiId字段更新数据 
        /// </summary> 
        /// <param name="lmShopCostReportTermini">lmShopCostReportTermini</param>
        /// <returns></returns>       
        public int UpdatelmShop_CostReportTerminiByTerminiId(CostReportTerminiInfo lmShopCostReportTermini)
        {
            string sql = "update [lmShop_CostReportTermini] set [ReportId] = @ReportId,[StartDay] = @StartDay,[EndDay] = @EndDay,[TerminiLocation] = @TerminiLocation,[OperatingTime]=@OperatingTime where [TerminiId] = @TerminiId";
            SqlParameter[] paras = PrepareCommandParameters(lmShopCostReportTermini);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras);
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向lmShop_CostReportTermini表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="lmShopCostReportTermini">CostReportTerminiInfo</param>       
        /// <returns></returns>        
        public bool AddlmShop_CostReportTermini(CostReportTerminiInfo lmShopCostReportTermini)
        {
            string sql = "insert into [lmShop_CostReportTermini]([TerminiId],[ReportId],[StartDay],[EndDay],[TerminiLocation],[OperatingTime])values(@TerminiId,@ReportId,@StartDay,@EndDay,@TerminiLocation,@OperatingTime)    select @@identity";
            SqlParameter[] paras = PrepareCommandParameters(lmShopCostReportTermini);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 批量插入起讫
        /// </summary>
        /// <param name="costReportTerminiInfoList"></param>
        /// <returns></returns>
        public bool AddBatchlmShop_CostReportTermini(IList<CostReportTerminiInfo> costReportTerminiInfoList)
        {
            var dics = new Dictionary<string, string>
                {
                    {"TerminiId","TerminiId"},{"ReportId","ReportId"},{"StartDay","StartDay"},
                    {"EndDay","EndDay"},{"TerminiLocation","TerminiLocation"},{"OperatingTime","OperatingTime"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, costReportTerminiInfoList, "lmShop_CostReportTermini", dics) > 0;
        }
        #endregion
        #endregion
    }
}
