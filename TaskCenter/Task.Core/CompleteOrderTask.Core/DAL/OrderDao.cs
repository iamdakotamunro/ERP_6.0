using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper;

namespace CompleteOrderTask.Core.DAL
{
    public class OrderDao
    {
        public static void LogMessage(Guid taskId, string message)
        {
            const string SQL = @"
INSERT INTO [FinishOrderTaskMessage]
           ([TaskId]
           ,[FinishMessage])
     VALUES
           (@TaskId
           ,@FinishMessage)
";
            using (var db = DbFactory.Create())
            {
                db.Execute(false, SQL, new[]
                    {
                        new Parameter("TaskId",taskId),
                        new Parameter("FinishMessage",message)
                    });
            }
        }

        public static bool AddTask(Guid taskId, string taskDate, Guid warehouseId, Guid expressId, string operationer)
        {
            const string ADD_SQL = @"
	    INSERT INTO [FinishOrderTask]
			       ([TaskId]
			       ,[TaskDate]
			       ,[Operationer]
			       ,[ExpressId]
			       ,[WarehouseId])
		     VALUES
			       (@TaskId
			       ,@TaskDate
			       ,@Operationer
			       ,@ExpressId
			       ,@WarehouseId)	
";

            var parms = new[]
                {
                    new Parameter("TaskId", taskId),
                    new Parameter("Operationer", operationer),
                    new Parameter("TaskDate", taskDate),
                    new Parameter("ExpressId", expressId),
                    new Parameter("WarehouseId", warehouseId)
                };
            using (var db = DbFactory.Create())
            {
                return db.Execute(false, ADD_SQL, parms);
            }
        }

        public static FinishOrderInfo GetNoIsFinish(string taskDate, Guid warehouseId, Guid expressId)
        {
            const string GET_SQL = @"
SELECT * FROM [FinishOrderTask] WHERE [TaskDate]=@TaskDate AND ExpressId=@ExpressId AND WarehouseId=@WarehouseId AND IsFinish=0
";

            var parms = new[]
                {
                    new Parameter("TaskDate", taskDate),
                    new Parameter("ExpressId", expressId),
                    new Parameter("WarehouseId", warehouseId)
                };
            using (var db = DbFactory.Create())
            {
                return db.Single<FinishOrderInfo>(true, GET_SQL, parms);
            }
        }

        public static Guid GetTaskId(string taskDate, Guid warehouseId, Guid expressId)
        {
            const string GET_SQL = @"
SELECT [TaskId] FROM [FinishOrderTask] WHERE [TaskDate]=@TaskDate AND ExpressId=@ExpressId AND WarehouseId=@WarehouseId
";

            var parms = new[]
                {
                    new Parameter("TaskDate", taskDate),
                    new Parameter("ExpressId", expressId),
                    new Parameter("WarehouseId", warehouseId)
                };
            using (var db = DbFactory.Create())
            {
                return db.GetValue<Guid>(true, GET_SQL, parms);
            }
        }

        public static bool FinishOver(Guid taskId, Guid warehouseId, Guid expressId)
        {
            const string GET_SQL = @"
UPDATE [FinishOrderTask] SET IsFinish=1 WHERE [TaskId]=@TaskId AND ExpressId=@ExpressId AND WarehouseId=@WarehouseId
";

            var parms = new[]
                {
                    new Parameter("TaskId", taskId),
                    new Parameter("ExpressId", expressId),
                    new Parameter("WarehouseId", warehouseId)
                };
            using (var db = DbFactory.Create())
            {
                return db.Execute(false, GET_SQL, parms);
            }
        }

        public static bool ExistsFinish(Guid warehouseId)
        {
            const string SQL = @"
IF EXISTS (SELECT 1 FROM [FinishOrderTask] WHERE WarehouseId=@WarehouseId AND IsFinish = 0)
    BEGIN
        SELECT 0;
    END
ELSE
    BEGIN
        SELECT 1;
    END    
";
            using (var db = DbFactory.Create())
            {
                return db.GetValue<int>(true, SQL, new Parameter("WarehouseId", warehouseId)) == 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderState"></param>
        /// <param name="expressId"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public static int CountByState(int orderState, Guid expressId, Guid saleFilialeId)
        {
            const string SQL1 =
                @"SELECT COUNT(1) FROM [lmshop_GoodsOrder] WHERE OrderState=@OrderState AND SaleFilialeId=@SaleFilialeId";
            const string SQL2 =
                @"SELECT COUNT(1) FROM [lmshop_GoodsOrder] WHERE OrderState=@OrderState AND ExpressId=@ExpressId AND SaleFilialeId=@SaleFilialeId";
            using (var db = DbFactory.Create())
            {
                if (expressId == Guid.Empty)
                {
                    return db.Run(SQL1)
                        .AddParameter("OrderState", orderState)
                        .AddParameter("SaleFilialeId", saleFilialeId)
                        .GetValue<int>();
                }
                return db.Run(SQL2)
                    .AddParameter("OrderState", orderState)
                    .AddParameter("SaleFilialeId", saleFilialeId)
                    .AddParameter("ExpressId", expressId)
                    .GetValue<int>();
            }
        }

        public static IList<GoodsOrderInfo> GetOrderListByState(int orderState, Guid expressId, Guid saleFilialeId, Guid deliverWarehouseId)
        {
            const string SQL1 = @"SELECT * FROM [lmshop_GoodsOrder] WHERE OrderState=@OrderState AND SaleFilialeId=@SaleFilialeId AND DeliverWarehouseId=@DeliverWarehouseId";
            const string SQL2 = @"SELECT * FROM [lmshop_GoodsOrder] WHERE OrderState=@OrderState AND ExpressId=@ExpressId AND SaleFilialeId=@SaleFilialeId AND DeliverWarehouseId=@DeliverWarehouseId";
            using (var db = DbFactory.Create())
            {
                if (expressId == Guid.Empty)
                {
                    return db.Select<GoodsOrderInfo>(true, SQL1, new Parameter("@OrderState", orderState), new Parameter("@SaleFilialeId", saleFilialeId), new Parameter("@DeliverWarehouseId", deliverWarehouseId)).ToList();
                }
                return db.Select<GoodsOrderInfo>(true, SQL2, new Parameter("@OrderState", orderState), new Parameter("@ExpressId", expressId), new Parameter("@SaleFilialeId", saleFilialeId), new Parameter("@DeliverWarehouseId", deliverWarehouseId)).ToList();
            }
        }
    }
}
