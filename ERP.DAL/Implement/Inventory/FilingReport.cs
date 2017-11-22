using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 功   能:采购总报表操作类
    /// 作   者:jiangsaibiao
    /// </summary>
    public class FilingReport : IFilingReport
    {
        public FilingReport(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region SQl
        private const string SQL_INSERT_FILING_REPORT = @" INSERT INTO [lmshop_FilingReport]
           ([FilingId]
           ,[GoodsId]
           ,[GoodsName]
           ,[GoodsCode]
           ,[Specification]
           ,[NonceFilialeGoodsStock]
           ,[NonceRequest]
           ,[SalesNumber]
           ,[MeanNumber]
           ,[proposalNumber]
           ,[Demand]
           ,[TotalNumber]
           ,[GoodsState]
           ,[FilialeId]
           ,[WareHouseId]
           ,[SaleType]
           ,[BackupDays]
           ,[FilingType]
           ,[SuppliersId]
           ,[Suppliers]
           ,[StartTime]
           ,[endtime]
           ,[PurchasePrice]
          )
     VALUES
            (NEWID(), @GoodsId,@GoodsName,@GoodsCode,@Specification,@NonceFilialeGoodsStock,  @NonceRequest, 
            @SalesNumber,@MeanNumber,@proposalNumber,@Demand,@TotalNumber,@GoodsState,@FilialeId,@WareHouseId,
            @SaleType,@BackupDays,@FilingType,@SuppliersId,@Suppliers,@StartTime,@endtime,@PurchasePrice)";
        private const string DELETE_BY_FILING_TYPE = @"Delete From lmshop_FilingReport Where FilingType=@FilingType ";
        private const string SQL_UPDATE_FILING_REPORT = @" Update [lmshop_FilingReport] set Demand=@Demand,Suppliers=@Suppliers,PurchasePrice=@PurchasePrice
 where FilingId=@FilingId ";
        private const string SQL_UPDATE_FILING_REPORT_FILING_TYPE = @" Update [lmshop_FilingReport] set Demand=@Demand,Suppliers=@Suppliers,PurchasePrice=@PurchasePrice,FilingType=@FilingType
 where FilingId=@FilingId ";

        private const string SQL_DELETE_FILING_ID = " Delete From lmshop_FilingReport Where FilingId=@FilingId ";
        private const string PARM_FILING_ID = "@FilingId";
        private const string PARM_GOODS_ID = "@GoodsId";
        private const string PARM_GOODS_CODE = "@GoodsCode";
        private const string PARM_GOODS_NAME = "@GoodsName";
        private const string PARM_SPECIFICATION = "@Specification";
        private const string PARM_NONCE_FILIALE_GOODS_STOCK = "@NonceFilialeGoodsStock";
        private const string PARM_NONCE_REQUEST = "@NonceRequest";
        private const string PARM_DEMAND = "@Demand";
        private const string PARM_SALES_NUMBER = "@SalesNumber";
        private const string PARM_MEAN_NUMBER = "@MeanNumber";
        private const string PARM_PROPOSAL_NUMBER = "@proposalNumber";
        private const string PARM_TOTAL_NUMBER = "@TotalNumber";
        private const string PARM_GOODS_STATE = "@GoodsState";
        private const string PARM_FILIALE_ID = "@FilialeId";
        private const string PARM_WARE_HOUSE_ID = "@WareHouseId";
        private const string PARM_SALE_TYPE = "@SaleType";
        private const string PARM_BACKUP_DAYS = "@BackupDays";
        private const string PARM_FILING_TYPE = "@FilingType";
        private const string PARM_SUPPLIERS_ID = "@SuppliersId";
        private const string PARM_SUPPLIERS = "@Suppliers";
        private const string PARM_START_TIME = "@StartTime";
        private const string PARM_ENDTIME = "@endtime";

        private const string PARM_PURCHASE_PRICE = "@PurchasePrice";
        #endregion
        /// <summary>
        /// 添加报备记录
        /// </summary>
        /// <param name="fInfo"></param>
        public void Insert(FilingReportInfo fInfo)
        {
            SqlParameter[] parms = GetFilingReportParameters();
            parms[0].Value = fInfo.FilingId;
            parms[1].Value = fInfo.GoodsId;
            parms[2].Value = fInfo.GoodsName;
            parms[3].Value = fInfo.GoodsCode;
            parms[4].Value = fInfo.Specification;
            parms[5].Value = fInfo.NonceFilialeGoodsStock;
            parms[6].Value = fInfo.NonceRequest;
            parms[7].Value = fInfo.SalesNumber;
            parms[8].Value = fInfo.MeanNumber;
            parms[9].Value = fInfo.ProposalNumber;
            parms[10].Value = fInfo.Demand;
            parms[11].Value = fInfo.TotalNumber;
            parms[12].Value = fInfo.GoodsState;
            parms[13].Value = fInfo.FilialeId;
            parms[14].Value = fInfo.WareHouseId;
            parms[15].Value = fInfo.SaleType ? 1 : 0;
            parms[16].Value = fInfo.BackupDays;
            parms[17].Value = (int)fInfo.FilingType;
            parms[18].Value = fInfo.SuppliersId;
            parms[19].Value = fInfo.Suppliers;
            parms[20].Value = fInfo.StartTime;
            parms[21].Value = fInfo.Endtime;
            parms[22].Value = fInfo.PurchasePrice;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_FILING_REPORT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// 添加报备记录
        /// </summary>
        /// <param name="fInfo"></param>
        public void Update(FilingReportInfo fInfo)
        {
            var parms = new[] 
            {
                 new SqlParameter(PARM_FILING_ID, SqlDbType.UniqueIdentifier),
                 new SqlParameter(PARM_DEMAND, SqlDbType.Int),
                 new SqlParameter(PARM_SUPPLIERS, SqlDbType.VarChar),
                 new SqlParameter(PARM_PURCHASE_PRICE, SqlDbType.VarChar )
          
            };
            parms[0].Value = fInfo.FilingId;
            parms[1].Value = fInfo.Demand;
            parms[2].Value = fInfo.Suppliers;
            parms[3].Value = fInfo.PurchasePrice;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_FILING_REPORT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// 提交给仓库管理员
        /// </summary>
        /// <param name="fInfo"></param>
        public void UpdateFilingType(FilingReportInfo fInfo)
        {
            var parms = new[] 
            {
                 new SqlParameter(PARM_FILING_ID, SqlDbType.UniqueIdentifier),
                 new SqlParameter(PARM_DEMAND, SqlDbType.Int),
                 new SqlParameter(PARM_SUPPLIERS, SqlDbType.VarChar),
                 new SqlParameter(PARM_PURCHASE_PRICE, SqlDbType.VarChar ),
                 new SqlParameter(PARM_FILING_TYPE, SqlDbType.Int )
          
            };
            parms[0].Value = fInfo.FilingId;
            parms[1].Value = fInfo.Demand;
            parms[2].Value = fInfo.Suppliers;
            parms[3].Value = fInfo.PurchasePrice;
            parms[4].Value = fInfo.FilingType;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_FILING_REPORT_FILING_TYPE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ftype">进货类别</param>
        /// <param name="wareHouseId">仓库ID</param>
        public void Delete(FilingType ftype, Guid wareHouseId)
        {
            var parms = new[] {
               new SqlParameter(PARM_FILING_TYPE, SqlDbType.Int),
                 new SqlParameter(PARM_WARE_HOUSE_ID, SqlDbType.UniqueIdentifier)
              };
            parms[0].Value = (int)ftype;
            parms[1].Value = wareHouseId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, DELETE_BY_FILING_TYPE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// 删除Id
        /// </summary>
        /// <param name="filingId">采购记录id</param>
        public void Delete(Guid filingId)
        {
            var parms = new[] {
               new SqlParameter(PARM_FILING_ID, SqlDbType.UniqueIdentifier)
               
              };
            parms[0].Value = filingId;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_FILING_ID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// 获取库存 edit by lxm 20110113
        /// </summary>
        /// <param name="ftype"></param>
        /// <param name="wareHouseId"></param>
        /// <returns></returns>
        public IList<FilingReportInfo> GetFilingReportByFilingType(FilingType ftype, Guid wareHouseId)
        {
            IList<FilingReportInfo> frlist = new List<FilingReportInfo>();
            string SQL_Select_ByftypeAndWhouseId = @"Select    
                                                    f.FilingId
                                                   ,f.GoodsId
                                                   ,f.GoodsName
                                                   ,f.GoodsCode
                                                   ,f.Specification
                                                   ,f.NonceFilialeGoodsStock
                                                   ,f.NonceRequest
                                                   ,f.Demand
                                                   ,f.FilialeId
                                                   ,f.WareHouseId       
                                                   ,f.Suppliers
                                                   ,f.FilingType
                                                   ,f.StartTime
                                                   ,f.endtime                                                   
                                                   ,isnull(g.RecentInPrice,0)
                                                   ,f.SuppliersId
                                                   From lmshop_FilingReport f left join lmShop_GoodsStockCurrent g on f.GoodsId = g.RealGoodsId and g.WareHouseId=@WareHouseId Where 1=1 ";
            if ((int)ftype != -1)
            {
                SQL_Select_ByftypeAndWhouseId += " and f.FilingType=@FilingType ";
            }
            else
            {
                SQL_Select_ByftypeAndWhouseId += " and f.FilingType!=2 ";
            }
            SQL_Select_ByftypeAndWhouseId += " Order by Suppliers ";
            var parms = new[] 
            {
                 new SqlParameter(PARM_FILING_TYPE, SqlDbType.Int),
                 new SqlParameter(PARM_WARE_HOUSE_ID, SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = (int)ftype;
            parms[1].Value = wareHouseId;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_Select_ByftypeAndWhouseId, parms))
            {
                while (rdr.Read())
                {
                    var frInfo = new FilingReportInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3)
                                , rdr.GetString(4), rdr.GetInt32(5), rdr.GetInt32(6), rdr.GetInt32(7), rdr.GetGuid(8), rdr.GetGuid(9),
                                rdr.GetString(10), (FilingType)rdr.GetInt32(11), rdr.GetDateTime(12), rdr.GetDateTime(13), rdr[14] == DBNull.Value ? 0 : (decimal)rdr.GetDouble(14), rdr.GetGuid(15));
                    frlist.Add(frInfo);
                }
            }
            return frlist;
        }
        /// <summary>
        /// 添加报备记录
        /// </summary>
        private static SqlParameter[] GetFilingReportParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_FILING_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_GOODS_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_GOODS_NAME, SqlDbType.VarChar),
                new SqlParameter(PARM_GOODS_CODE, SqlDbType.VarChar),
                new SqlParameter(PARM_SPECIFICATION, SqlDbType.VarChar),
                new SqlParameter(PARM_NONCE_FILIALE_GOODS_STOCK, SqlDbType.Int),
                new SqlParameter(PARM_NONCE_REQUEST, SqlDbType.Int),
                new SqlParameter(PARM_SALES_NUMBER, SqlDbType.Int),
                new SqlParameter(PARM_MEAN_NUMBER, SqlDbType.Int),
                new SqlParameter(PARM_PROPOSAL_NUMBER, SqlDbType.Int),
                new SqlParameter(PARM_DEMAND, SqlDbType.Int),
                new SqlParameter(PARM_TOTAL_NUMBER, SqlDbType.Int),
                new SqlParameter(PARM_GOODS_STATE, SqlDbType.Bit),
                new SqlParameter(PARM_FILIALE_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_WARE_HOUSE_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_SALE_TYPE, SqlDbType.Int ),
                new SqlParameter(PARM_BACKUP_DAYS, SqlDbType.Int ),
                new SqlParameter(PARM_FILING_TYPE, SqlDbType.Int ),
                new SqlParameter(PARM_SUPPLIERS_ID, SqlDbType.UniqueIdentifier ),
                new SqlParameter(PARM_SUPPLIERS, SqlDbType.VarChar ),
                new SqlParameter(PARM_START_TIME, SqlDbType.DateTime),
                new SqlParameter(PARM_ENDTIME, SqlDbType.DateTime),
                new SqlParameter(PARM_PURCHASE_PRICE, SqlDbType.Decimal)
            
            };
            return parms;
        }
    }
}
