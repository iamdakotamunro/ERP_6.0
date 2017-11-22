using System;
using System.Collections.Generic;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using System.Data.SqlClient;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Inventory
{
    public class ActivityOperateLog : IActivityOperateLog
    {
        public ActivityOperateLog(Environment.GlobalConfig.DB.FromType fromType) { }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="activityOperateLogModel"></param>
        /// <returns></returns>
        public bool InsertLog(ActivityOperateLogModel activityOperateLogModel)
        {
            const string SQL = @"
                insert into ActivityOperateLog(ID, ActivityFilingID, OperatePersonnelID, OperatePersonnelName, Description, OperateDate)
                values(newid(), @ActivityFilingID, @OperatePersonnelID, @OperatePersonnelName, @Description, @OperateDate)
                ";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new {
                    ActivityFilingID = activityOperateLogModel.ActivityFilingID,
                    OperatePersonnelID = activityOperateLogModel.OperatePersonnelID,
                    OperatePersonnelName = activityOperateLogModel.OperatePersonnelName,
                    Description = activityOperateLogModel.Description,
                    OperateDate = activityOperateLogModel.OperateDate,
                }) > 0;
            }
        }

        /// <summary>
        /// 获取管理意见
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public List<ActivityOperateLogModel> SelectLogModels(Guid activityId)
        {
            const string SQL = @" SELECT  [Description]
                            FROM dbo.ActivityOperateLog
                            WHERE ActivityFilingID=@ActivityFilingID
                            ORDER BY  OperateDate";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ActivityOperateLogModel>(SQL, new
                {
                    ActivityFilingID = activityId,
                }).AsList();
            }
        }
    }
}
