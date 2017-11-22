using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 该类提供了一系列操作 "lmShop_CostReportAmount" 表的方法;
    /// </summary>
    public class CostReportAmountDal : ICostReportAmount
    {
        public CostReportAmountDal(GlobalConfig.DB.FromType fromType)
        {

        }
        const string SQL_SELECT = "SELECT [AmountId],[ReportId],[Num],[Amount],[IsPay],[IsSystem] FROM [lmShop_CostReportAmount] ";
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回lmShop_CostReportAmount表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<CostReportAmountInfo> GetAlllmShop_CostReportAmount()
        {
            List<CostReportAmountInfo> lmShopCostReportAmountList = new List<CostReportAmountInfo>();
            IDataReader reader = null;

            string sql = SQL_SELECT;
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                CostReportAmountInfo lmShopCostReportAmount = new CostReportAmountInfo(reader);
                lmShopCostReportAmountList.Add(lmShopCostReportAmount);
            }
            reader.Close();
            return lmShopCostReportAmountList;
        }
        /// <summary>
        /// 根据ReportId返回lmShop_CostReportAmount表的数据 
        /// </summary>
        /// <returns></returns>        
        public List<CostReportAmountInfo> GetmShop_CostReportAmountByReportId(Guid reportId)
        {
            List<CostReportAmountInfo> lmShopCostReportAmountList = new List<CostReportAmountInfo>();
            
            string sql = SQL_SELECT + "where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            while (reader.Read())
            {
                CostReportAmountInfo lmShopCostReportAmount = new CostReportAmountInfo(reader);
                lmShopCostReportAmountList.Add(lmShopCostReportAmount);
            }
            reader.Close();
            return lmShopCostReportAmountList;
        }
        /// <summary>
        /// 根据lmShop_CostReportAmount表的AmountId字段返回数据 
        /// </summary>
        /// <param name="amountId">AmountId</param>
        /// <returns></returns>       
        public CostReportAmountInfo GetlmShop_CostReportAmountByAmountId(Guid amountId)
        {
            CostReportAmountInfo lmShopCostReportAmount = null;
            IDataReader reader = null;

            string sql = SQL_SELECT + "where [AmountId] = @AmountId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@AmountId",amountId)
            };
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            if (reader.Read())
            {
                lmShopCostReportAmount = new CostReportAmountInfo(reader);
            }
            reader.Close();
            return lmShopCostReportAmount;
        }
        /// <summary>
        /// 根据ReportId查询该条费用申报数据对应的最大的申请次数
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>       
        public int GetMaxNumFromlmShop_CostReportAmountByReportId(Guid reportId)
        {
            string sql = "SELECT Max(Num) FROM [lmShop_CostReportAmount] where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };

            return int.Parse(SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME ,true, sql, paras).ToString());
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmShop_CostReportAmount表的AmountId字段删除数据 
        /// </summary>
        /// <param name="amountId">terminiId</param>
        /// <returns></returns>        
        public bool DeletelmShop_CostReportAmountByAmountId(Guid amountId)
        {
            string sql = "delete from [lmShop_CostReportAmount] where [TerminiId] = @TerminiId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@AmountId",amountId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmShop_CostReportAmount表的ReportId字段删除数据 
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// <returns></returns>        
        public bool DeletelmShop_CostReportAmountByReportId(Guid reportId)
        {
            string sql = "delete from [lmShop_CostReportAmount] where [ReportId] = @ReportId";
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
        public SqlParameter[] PrepareCommandParameters(CostReportAmountInfo lmShopCostReportAmount)
        {
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@AmountId",lmShopCostReportAmount.AmountId),
            new SqlParameter("@ReportId",lmShopCostReportAmount.ReportId),
            new SqlParameter("@Num",lmShopCostReportAmount.Num),
            new SqlParameter("@Amount",lmShopCostReportAmount.Amount),
            new SqlParameter("@IsPay",lmShopCostReportAmount.IsPay),
            new SqlParameter("@IsSystem",lmShopCostReportAmount.IsSystem)
            };
            return paras;
        }
        /// <summary>
        /// 根据lmShop_CostReportAmount表的AmountId字段更新数据 
        /// </summary> 
        /// <param name="lmShopCostReportAmount">lmShopCostReportAmount</param>
        /// <returns></returns>       
        public int UpdatelmShop_CostReportAmountByAmountId(CostReportAmountInfo lmShopCostReportAmount)
        {
            string sql = "update [lmShop_CostReportAmount] set [ReportId] = @ReportId,[Num] = @Num,[Amount] = @Amount,[IsPay] = @IsPay,[IsSystem]=@IsSystem where [AmountId] = @AmountId";
            SqlParameter[] paras = PrepareCommandParameters(lmShopCostReportAmount);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras);
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向lmShop_CostReportAmount表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="lmShopCostReportAmount">CostReportAmountInfo</param>       
        /// <returns></returns>        
        public bool AddlmShop_CostReportAmount(CostReportAmountInfo lmShopCostReportAmount)
        {
            string sql = "insert into [lmShop_CostReportAmount]([AmountId],[ReportId],[Num],[Amount],[IsPay],[IsSystem])values(@AmountId,@ReportId,@Num,@Amount,@IsPay,@IsSystem)    select @@identity";
            SqlParameter[] paras = PrepareCommandParameters(lmShopCostReportAmount);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 向lmshop_CostReportAmount表批量插入数据
        /// </summary>
        /// <param name="lmshopCostReportAmountList"></param>
        /// <returns></returns>
        public bool AddBatchlmshop_CostReportAmount(List<CostReportAmountInfo> lmshopCostReportAmountList)
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            dics.Add("AmountId", "AmountId");
            dics.Add("ReportId", "ReportId");
            dics.Add("Num", "Num");
            dics.Add("Amount", "Amount");
            dics.Add("IsPay", "IsPay");
            dics.Add("IsSystem", "IsSystem");
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, lmshopCostReportAmountList, "lmshop_CostReportAmount", dics) > 0;
        }
        #endregion
        #endregion
    }
}
