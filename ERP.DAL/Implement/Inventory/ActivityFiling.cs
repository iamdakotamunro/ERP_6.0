using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using System.Data.SqlClient;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using Dapper.Extension;
using System.Transactions;

namespace ERP.DAL.Implement.Inventory
{
    public class ActivityFiling : IActivityFiling
    {
        public ActivityFiling(Environment.GlobalConfig.DB.FromType fromType) { }


        #region SQL

        private const string SQL_SELECTACTIVITYFILING =
            @"SELECT ID, ActivityFilingTitle, FilingCompanyName, ActivityFilingState, 
                                                            GoodsName, ActivityStateDate, ActivityEndDate, ProspectSaleNumber, 
                                                            ProspectReadyNumber, ActualSaleNumber, NormalSaleNumber,
                                                            PurchasePersonnelName, FilingTerraceName, WarehouseID,FilingCompanyID,
                                                            OperatePersonnelName,ErrorProbability,CreateDate,GoodsID,FilingTerraceID
                                                            FROM activityfiling WHERE 1=1";

        #endregion

        #region 参数

        private const string PARM_ID = "@ID";
        private const string PARM_ACTIVITY_FILING_TITLE = "@ActivityFilingTitle";
        private const string PARM_FILING_COMPANY_ID = "@FilingCompanyID";
        private const string PARM_FILING_COMPANY_NAME = "@FilingCompanyName";
        private const string PARM_ACTIVITY_FILING_STATE = "@ActivityFilingState";
        private const string PARM_GOODS_ID = "@GoodsID";
        private const string PARM_GOODS_CODE = "@GoodsCode";
        private const string PARM_GOODS_NAME = "@GoodsName";
        private const string PARM_ACTIVITY_STATE_DATE = "@ActivityStateDate";
        private const string PARM_ACTIVITY_END_DATE = "@ActivityEndDate";
        private const string PARM_PROSPECT_SALE_NUMBER = "@ProspectSaleNumber";
        private const string PARM_PROSPECT_READY_NUMBER = "@ProspectReadyNumber";
        private const string PARM_ACTUAL_SALE_NUMBER = "@ActualSaleNumber";
        private const string PARM_NORMAL_SALE_NUMBER = "@NormalSaleNumber";
        private const string PARM_PURCHASE_PERSONNEL_ID = "@PurchasePersonnelID";
        private const string PARM_PURCHASE_PERSONNEL_NAME = "@PurchasePersonnelName";
        private const string PARM_CREATE_DATE = "@CreateDate";
        private const string PARM_FILING_TERRACE_NAME = "@FilingTerraceName";
        private const string PARM_FILING_TERRACE_ID = "@FilingTerraceID";
        private const string PARM_OPERATE_PERSONNEL_ID = "@OperatePersonnelID";
        private const string PARM_OPERATE_PERSONNEL_NAME = "@OperatePersonnelName";
        private const string PARM_ERROR_PROBABILITY = "@ErrorProbability";
        private const string PARM_WAREHOUSE_ID = "@WarehouseID";

        #endregion

        #region SELECT

        /// <summary>
        /// 查询活动报备
        /// </summary>
        /// <param name="title"></param>
        /// <param name="goodsName"></param>
        /// <param name="activityStateDate"></param>
        /// <param name="activityEndDate"></param>
        /// <param name="saleTerrace"></param>
        /// <param name="purchaseState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>

        public IList<ActivityFilingInfo> SelectActivityFilings(string title, string goodsName, DateTime activityStateDate, DateTime activityEndDate, Guid saleTerrace, int purchaseState, int pageIndex, int pageSize, out int total)
        {
            var stringBuilderSql = new StringBuilder(SQL_SELECTACTIVITYFILING);
            if (!string.IsNullOrEmpty(title))
            {
                stringBuilderSql.AppendFormat(@" AND ActivityFilingTitle LIKE '{0}%'", title);
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                stringBuilderSql.AppendFormat(@" AND GoodsName LIKE '{0}%'", goodsName);
            }
            if (activityStateDate != DateTime.MinValue)
            {
                stringBuilderSql.AppendFormat(@" AND ActivityEndDate>='{0}'", activityStateDate);
            }
            if (activityEndDate != DateTime.MinValue)
            {
                stringBuilderSql.AppendFormat(@" AND ActivityEndDate<='{0}'", activityEndDate);
            }
            if (saleTerrace != Guid.Empty)
            {
                stringBuilderSql.AppendFormat(@" AND FilingCompanyID='{0}'", saleTerrace);
            }
            if (purchaseState != 0 && purchaseState != -1)
            {
                stringBuilderSql.AppendFormat(@" AND ActivityFilingState={0} ", purchaseState);
            }
            else if (purchaseState != -1 && (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(goodsName)))
            {
                stringBuilderSql.AppendFormat(@" AND ActivityFilingState!=4");
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                total = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", stringBuilderSql.ToString()));
                return conn.QueryPaged<ActivityFilingInfo>(stringBuilderSql.ToString(), pageIndex, pageSize, "CreateDate DESC").AsList();
            }
        }

        /// <summary>
        /// 获取子商品的销售统计记录
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="minDays"></param>
        /// <param name="maxDays"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public int GetGoodsSale(Guid goodsID, Guid warehouseId, Guid saleFilialeId, Guid salePlatformId, int minDays,
            int maxDays)
        {
            string sql = string.Format(@" SELECT 
	                                ABS(SUM(GoodsSales)) AS SaleQuantity
                                FROM [lmshop_GoodsDaySalesStatistics]
                                WHERE 
	                                DayTime >= DATEADD(DAY,{0},GETDATE()) AND DayTime < DATEADD(DAY,{1},GETDATE())
	                                AND goodsid='{2}'
	                                AND DeliverWarehouseId='{3}' AND SaleFilialeId='{4}' AND SalePlatformId='{5}'
                                GROUP BY goodsid,DeliverWarehouseId ", maxDays, minDays, goodsID, warehouseId, saleFilialeId, salePlatformId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(sql);
            }
        }

        /// <summary>
        /// 获得实际销量
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public int GetGoodsRealSale(Guid goodsID, Guid warehouseId, Guid saleFilialeId, Guid salePlatformId, DateTime startDateTime, DateTime endDateTime)
        {
            string sql = string.Format(@"SELECT 
                                          ABS(SUM(GoodsSales)) AS SaleQuantity
                                          FROM [lmshop_GoodsDaySalesStatistics]
                                         WHERE 
                                        DayTime >= '{0}' AND DayTime < '{1}'
                                        AND goodsid='{2}'
                                        AND DeliverWarehouseId='{3}' AND SaleFilialeId='{4}' AND SalePlatformId='{5}'
                                        GROUP BY goodsid,DeliverWarehouseId", startDateTime, endDateTime, goodsID, warehouseId, saleFilialeId, salePlatformId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(sql);
            }
        }

        #endregion

        #region insert

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="activityFilingInfo"></param>
        /// <returns></returns>
        public bool InsertActivityFiling(ActivityFilingInfo activityFilingInfo)
        {
            const string SQL = @"
                                INSERT into ActivityFiling(ID, ActivityFilingTitle, FilingCompanyID, FilingCompanyName, ActivityFilingState, GoodsID, 
                                GoodsCode, GoodsName, ActivityStateDate, ActivityEndDate, ProspectSaleNumber, ProspectReadyNumber, ActualSaleNumber, NormalSaleNumber, 
                                PurchasePersonnelID, PurchasePersonnelName, CreateDate, FilingTerraceID, FilingTerraceName, OperatePersonnelID, OperatePersonnelName, ErrorProbability,WarehouseID)
                                VALUES(@ID, @ActivityFilingTitle, @FilingCompanyID, @FilingCompanyName, @ActivityFilingState, @GoodsID, @GoodsCode, @GoodsName, @ActivityStateDate, 
                                @ActivityEndDate, @ProspectSaleNumber, @ProspectReadyNumber, @ActualSaleNumber, @NormalSaleNumber, @PurchasePersonnelID, @PurchasePersonnelName, 
                                @CreateDate, @FilingTerraceID, @FilingTerraceName, @OperatePersonnelID, @OperatePersonnelName, @ErrorProbability,@WarehouseID)";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new {
                    ID = activityFilingInfo.ID,
                    ActivityFilingTitle = activityFilingInfo.ActivityFilingTitle,
                    FilingCompanyID = activityFilingInfo.FilingCompanyID,
                    FilingCompanyName = activityFilingInfo.FilingCompanyName,
                    ActivityFilingState = activityFilingInfo.ActivityFilingState,
                    GoodsID = activityFilingInfo.GoodsID,
                    GoodsCode = activityFilingInfo.GoodsCode,
                    GoodsName = activityFilingInfo.GoodsName,
                    ActivityStateDate = activityFilingInfo.ActivityStateDate,
                    ActivityEndDate = activityFilingInfo.ActivityEndDate,
                    ProspectSaleNumber = activityFilingInfo.ProspectSaleNumber,
                    ProspectReadyNumber = activityFilingInfo.ProspectReadyNumber,
                    ActualSaleNumber = activityFilingInfo.ActualSaleNumber,
                    NormalSaleNumber = activityFilingInfo.NormalSaleNumber,
                    PurchasePersonnelID = activityFilingInfo.PurchasePersonnelID,
                    PurchasePersonnelName = activityFilingInfo.PurchasePersonnelName,
                    CreateDate = activityFilingInfo.CreateDate,
                    FilingTerraceID = activityFilingInfo.FilingTerraceID,
                    FilingTerraceName = activityFilingInfo.FilingTerraceName,
                    OperatePersonnelID = activityFilingInfo.OperatePersonnelID,
                    OperatePersonnelName = activityFilingInfo.OperatePersonnelName,
                    ErrorProbability = activityFilingInfo.ErrorProbability,
                    WarehouseID = activityFilingInfo.WarehouseID,
                }) > 0;
            }
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActivityFilingInfo SelectFilingInfoById(Guid id)
        {
            const string SQL = @"
                    SELECT  ActivityFilingTitle, FilingCompanyID,FilingCompanyName, GoodsID, GoodsCode, GoodsName, 
                    ActivityStateDate, ActivityEndDate, ProspectSaleNumber, ProspectReadyNumber, ActualSaleNumber, NormalSaleNumber, PurchasePersonnelID, 
                    PurchasePersonnelName, CreateDate, FilingTerraceID,FilingTerraceName, OperatePersonnelID,ActivityFilingState,
                    ErrorProbability, WarehouseID FROM ActivityFiling WHERE ID=@ID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ActivityFilingInfo>(SQL, new
                {
                    ID = id,
                });
            }
        }

        /// <summary>
        /// 修改1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="purchasePersonnelID"></param>
        /// <param name="purchasePersonnelName"></param>
        /// <param name="prospectReadyNumber"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateFilingInfo(Guid id, Guid purchasePersonnelID, string purchasePersonnelName,
            int prospectReadyNumber, int state)
        {
            const string SQL =
                @"UPDATE ActivityFiling SET PurchasePersonnelID=@PurchasePersonnelID,PurchasePersonnelName=@PurchasePersonnelName,ProspectReadyNumber=@ProspectReadyNumber,
                                    ActivityFilingState=@ActivityFilingState
                                    WHERE ID=@ID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    PurchasePersonnelID = purchasePersonnelID,
                    PurchasePersonnelName = purchasePersonnelName,
                    ActivityFilingState = state,
                    ProspectReadyNumber = prospectReadyNumber,
                    ID = id,
                }) > 0;
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateFilingState(Guid id, int state)
        {
            const string SQL = @"UPDATE ActivityFiling SET ActivityFilingState=@ActivityFilingState WHERE ID=@ID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ActivityFilingState = state,
                    ID = id,
                }) > 0;
            }
        }

        public bool UpdateGoodsSaleNumber(Guid id, int actualSaleNumber, decimal errorProbability)
        {
            const string SQL =
                @"UPDATE ActivityFiling SET ActualSaleNumber=@ActualSaleNumber,ErrorProbability=@ErrorProbability where ID=@ID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ID = id,
                    ActualSaleNumber = actualSaleNumber,
                    ErrorProbability = errorProbability,
                }) > 0;
            }
        }

        public ActivityFilingTotalModel TotalNumber(string title, string goodsName, DateTime activityStateDate,
            DateTime activityEndDate, Guid saleTerrace, int purchaseState)
        {
            var activityFilingTotalModel = new ActivityFilingTotalModel();
            const string SQL = @"SELECT  ProspectSaleNumber,  ActualSaleNumber, NormalSaleNumber
                                FROM activityfiling WHERE 1=1";
            var stringBuilderSql = new StringBuilder(SQL);
            if (!string.IsNullOrEmpty(title))
            {
                stringBuilderSql.AppendFormat(@" AND ActivityFilingTitle LIKE '{0}%'", title);
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                stringBuilderSql.AppendFormat(@" AND GoodsName LIKE '{0}%'", goodsName);
            }
            if (activityStateDate != DateTime.MinValue)
            {
                stringBuilderSql.AppendFormat(@" AND ActivityEndDate>='{0}'", activityStateDate);
            }
            if (activityEndDate != DateTime.MinValue)
            {
                stringBuilderSql.AppendFormat(@" AND ActivityEndDate<='{0}'", activityEndDate);
            }
            if (saleTerrace != Guid.Empty)
            {
                stringBuilderSql.AppendFormat(@" AND FilingCompanyID='{0}'", saleTerrace);
            }
            if (purchaseState != 0 && purchaseState != -1)
            {
                stringBuilderSql.AppendFormat(@" AND ActivityFilingState={0}", purchaseState);
            }
            else if (purchaseState != -1 && (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(goodsName)))
            {
                stringBuilderSql.AppendFormat(@" AND ActivityFilingState!=4");
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                var list = conn.Query<ActivityFilingInfo>(stringBuilderSql.ToString()).AsList();
                if (list != null)
                {
                    var listResult = list.ToList();
                    if (listResult.Any())
                    {
                        activityFilingTotalModel.TotalActualSaleNumber = listResult.Sum(x => x.ActualSaleNumber);
                        activityFilingTotalModel.TotalNormalSaleNumber = listResult.Sum(x => x.NormalSaleNumber);
                        activityFilingTotalModel.TotalProspectSaleNumber = listResult.Sum(x => x.ProspectSaleNumber);
                    }
                }
            }
            return activityFilingTotalModel;
        }

        public bool SelectGoods(Guid filingCompanyID, Guid goodsId, Guid filingTerraceID, Guid warehouseID)
        {
            string sql = string.Format(@"select count(0) as result from ActivityFiling
                        where FilingCompanyID='{0}' and GoodsId='{1}'
                        and FilingTerraceID='{2}' and warehouseid='{3}' and activityFilingState not in(5) and ActivityEndDate>=CONVERT(varchar(100), GETDATE(), 23)",
                filingCompanyID, goodsId, filingTerraceID, warehouseID);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(sql) > 0;
            }
        }

        /// <summary>
        /// 修改活动报备单基本信息
        /// </summary>
        /// <param name="activityFilingInfo"></param>
        /// <returns></returns>
        public bool UpdateFilingBaseInfo(ActivityFilingInfo activityFilingInfo)
        {
            const string SQL =
                @"UPDATE ActivityFiling SET ActivityFilingTitle=@ActivityFilingTitle, FilingCompanyID=@FilingCompanyID, FilingCompanyName=@FilingCompanyName, 
                           GoodsID=@GoodsID, GoodsCode=@GoodsCode, GoodsName=@GoodsName, ActivityStateDate=@ActivityStateDate, ActivityEndDate=@ActivityEndDate, ActivityFilingState=@ActivityFilingState,
                          ProspectSaleNumber=@ProspectSaleNumber,  NormalSaleNumber=@NormalSaleNumber, FilingTerraceID=@FilingTerraceID, FilingTerraceName=@FilingTerraceName,  WarehouseID=@WarehouseID
                          WHERE id=@ID  ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ActivityFilingTitle = activityFilingInfo.ActivityFilingTitle,
                    FilingCompanyID = activityFilingInfo.FilingCompanyID,
                    FilingCompanyName = activityFilingInfo.FilingCompanyName,
                    GoodsID = activityFilingInfo.GoodsID,
                    GoodsCode = activityFilingInfo.GoodsCode,
                    GoodsName = activityFilingInfo.GoodsName,
                    ActivityStateDate = activityFilingInfo.ActivityStateDate,
                    ActivityEndDate = activityFilingInfo.ActivityEndDate,
                    ProspectSaleNumber = activityFilingInfo.ProspectSaleNumber,
                    NormalSaleNumber = activityFilingInfo.NormalSaleNumber,
                    FilingTerraceID = activityFilingInfo.FilingTerraceID,
                    FilingTerraceName = activityFilingInfo.FilingTerraceName,
                    WarehouseID = activityFilingInfo.WarehouseID,
                    ID = activityFilingInfo.ID,
                    ActivityFilingState = activityFilingInfo.ActivityFilingState,
                }) > 0;
            }
        }

        /// <summary>
        /// 修改采购人的信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="purchasePersonnelID"></param>
        /// <param name="purchasePersonnelName"></param>
        /// <param name="prospectReadyNumber"></param>
        /// <returns></returns>
        public bool UpdateFilingInfo(Guid id, Guid purchasePersonnelID, string purchasePersonnelName,
            int prospectReadyNumber)
        {
            const string SQL =
                @"UPDATE ActivityFiling SET PurchasePersonnelID=@PurchasePersonnelID,PurchasePersonnelName=@PurchasePersonnelName,ProspectReadyNumber=@ProspectReadyNumber                                    
                                    WHERE ID=@ID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    OperatePersonnelID = purchasePersonnelID,
                    OperatePersonnelName = purchasePersonnelName,
                    ProspectReadyNumber = prospectReadyNumber,
                    ID = id,
                }) > 0;
            }
        }
        #endregion
    }
}
