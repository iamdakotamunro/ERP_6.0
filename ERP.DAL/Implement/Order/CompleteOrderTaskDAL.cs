using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using Keede.Ecsoft.Model;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Order
{
    public class CompleteOrderTaskDAL : ICompleteOrderTask
    {
        public IEnumerable<CompleteOrderTaskRecordInfo> GetWaitConsignmentedList(Guid warehouseId, Guid expressId)
        {
            var strbSql = new StringBuilder();
            strbSql.Append(@"
SELECT 
	t1.[OrderID],t1.[OrderNo],t1.[SaleFilialeId],SalePlatformId,t1.DeliverWarehouseId AS WarehouseID,t1.[ExpressId],t1.HostingFilialeId
FROM lmshop_GoodsOrder t1 WITH(NOLOCK)
LEFT JOIN CompleteOrderTaskRecord t2 WITH(NOLOCK) ON t2.OrderID=t1.OrderId
WHERE OrderState=8 AND t2.OrderID IS NULL");
            strbSql.AppendFormat(" AND DeliverWarehouseId='{0}'", warehouseId);
            if (expressId != Guid.Empty)
                strbSql.AppendFormat(" AND t1.ExpressId='{0}'", expressId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CompleteOrderTaskRecordInfo>(strbSql.ToString());
            }
        }

        public bool AddCompleteOrderTaskAndDetails(CompleteOrderTaskInfo info, List<CompleteOrderTaskDetailsInfo> detailsList)
        {
            var taskId = Guid.NewGuid();
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        //添加完成订单任务
                        AddCompleteOrderTask(trans, taskId, info);

                        //添加完成订单任务明细
                        AddCompleteOrderTaskDetails(trans, taskId, detailsList);

                        //订单记录设置任务编号ID
                        AddCompleteOrderTaskRecord(trans, taskId, detailsList.Select(w => w.OrderId).ToList());

                        trans.Commit();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        throw new ApplicationException(ex.Message);
                    }
                }
            }
            return true;
        }

        public IEnumerable<CompleteOrderTaskDetailsInfo> GetCompleteOrderTaskDetailsList(Guid taskId)
        {
            var strbSql = new StringBuilder();
            strbSql.AppendFormat(@"
SELECT 
	[ID],[TaskId],[OrderId],[OrderNo],[SaleFilialeId]
	,[IsSuccessERP],[IsSuccessB2C],[IsSuccessStock],[IsSuccessMember],[IsSuccessPromotion],[IsAllComplete]
	,[Description]
FROM [CompleteOrderTaskDetails]
WHERE TaskId='{0}'
ORDER BY [SaleFilialeId]
", taskId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CompleteOrderTaskDetailsInfo>(strbSql.ToString());
            }
        }

        public bool RebootTask(Guid taskId)
        {
            string sql = string.Format("UPDATE CompleteOrderTask SET TaskState={0},FailureQuantity=0 WHERE ID='{1}'", (int)CompleteOrderTaskState.Wait, taskId);
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        public bool SetCompleteOrderTaskDetail(Guid orderId)
        {
            string sql = string.Format(@"
DECLARE @taskId UNIQUEIDENTIFIER;
DECLARE @failureQuantity INT;
IF EXISTS(SELECT TOP 1 1 FROM CompleteOrderTaskDetails WHERE OrderId='{0}' AND IsAllComplete=0)
BEGIN
	SELECT @taskId=TaskId FROM CompleteOrderTaskDetails WITH(NOLOCK) WHERE OrderId='{0}';
	UPDATE CompleteOrderTaskDetails SET IsAllComplete=1 WHERE OrderId='{0}';
	SELECT @failureQuantity=FailureQuantity FROM CompleteOrderTask WITH(NOLOCK) WHERE ID=@taskId;
	IF(@failureQuantity>0)
	    UPDATE CompleteOrderTask SET FailureQuantity=FailureQuantity-1 WHERE ID=@taskId;
END
", orderId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        public IEnumerable<CompleteOrderTaskInfo> GetCompleteOrderTaskExpressByTaskId(Guid taskId)
        {
            const string SQL = @"
SELECT ExpressId,COUNT(1) AS TotalQuantity FROM lmshop_GoodsOrder t1 WITH(NOLOCK)
INNER JOIN CompleteOrderTaskRecord t2 WITH(NOLOCK) ON t2.OrderID=t1.OrderId
WHERE t2.TaskId=@TaskId
GROUP BY ExpressId
";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CompleteOrderTaskInfo>(SQL, new { TaskId = taskId });
            }
        }

        public IEnumerable<GoodsOrderInfo> GetExportGoodsOrderList(Guid taskId, Guid expressId)
        {
            string sql = @"
SELECT 
	gor.OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,
	Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,
	TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,
	DeliverWarehouseId,LatencyDay,gor.Memo,PromotionValue,PromotionDescription,EffectiveTime,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,
	p.PickNo,gor.DistrictID,gor.HostingFilialeId  
FROM lmShop_GoodsOrder gor WITH(NOLOCK)
INNER JOIN CompleteOrderTaskRecord t1 WITH(NOLOCK) ON t1.OrderID=gor.OrderId
LEFT JOIN lmshop_Pick p WITH(NOLOCK) ON p.OrderId = gor.OrderId";

            sql += " WHERE t1.TaskId='" + taskId + "' AND ExpressId='" + expressId + "' ORDER BY OrderTime ASC";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderInfo>(sql);
            }
        }

        #region --> Private
        private void AddCompleteOrderTaskRecord(SqlTransaction trans, Guid taskId, List<Guid> orderIds)
        {
            const string SQL = "INSERT INTO CompleteOrderTaskRecord(OrderID,TaskId) VALUES(@OrderID,@TaskId)";
            foreach (var orderId in orderIds)
            {
                trans.Connection.Execute(SQL, new
                {
                    OrderID = orderId,
                    TaskId = taskId
                }, trans);
            }
        }

        private void AddCompleteOrderTask(SqlTransaction trans, Guid taskId, CompleteOrderTaskInfo info)
        {
            var strbSql = new StringBuilder();
            strbSql.AppendLine(@"
INSERT INTO [CompleteOrderTask]([ID],[CreateTime],[ExpressId],[WarehouseId],[TaskState],[TotalQuantity],[Operationer],[OperationId])
 VALUES(@ID,GETDATE(),@ExpressId,@WarehouseId,@TaskState,@TotalQuantity,@Operationer,@OperationId)
");
            
            trans.Connection.Execute(strbSql.ToString(), new {
                ID = taskId,
                ExpressId = info.ExpressId,
                WarehouseId = info.WarehouseId,
                TaskState = (int)CompleteOrderTaskState.Wait,
                TotalQuantity = info.TotalQuantity,
                Operationer = info.Operationer,
                OperationId = info.OperationId,
            }, trans);
        }

        private void AddCompleteOrderTaskDetails(SqlTransaction trans, Guid taskId, List<CompleteOrderTaskDetailsInfo> detailsList)
        {
            var strbSql = new StringBuilder();
            strbSql.AppendLine(@"
INSERT INTO [CompleteOrderTaskDetails]([ID],[TaskId],[OrderId],[OrderNo],[SaleFilialeId],[SalePlatformId])
 VALUES(NEWID(),@TaskId,@OrderId,@OrderNo,@SaleFilialeId,@SalePlatformId)
");
            foreach (var detailInfo in detailsList)
            {
                trans.Connection.Execute(strbSql.ToString(), new {
                    TaskId= taskId,
                    OrderId= detailInfo.OrderId,
                    OrderNo = detailInfo.OrderNo,
                    SaleFilialeId = detailInfo.SaleFilialeId,
                    SalePlatformId = detailInfo.SalePlatformId,
                }, trans);
            }
        }
        #endregion
    }
}
