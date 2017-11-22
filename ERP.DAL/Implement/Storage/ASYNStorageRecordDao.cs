using System;
using System.Collections.Generic;
using ERP.DAL.Interface.IStorage;
using ERP.Model.ASYN;
using System.Data.SqlClient;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Storage
{
    /// <summary>
    /// 
    /// </summary>
    public class ASYNStorageRecordDao : IASYNStorageRecordDao
    {
        public ASYNStorageRecordDao(Environment.GlobalConfig.DB.FromType fromType) { }

        public bool Insert(ASYNStorageRecordInfo asynStorageRecordInfo)
        {
            const string SQL = @"
IF NOT EXISTS(SELECT 1 FROM [ASYN_StorageRecord] WHERE [StorageType]=@StorageType AND [IdentifyId]=@IdentifyId)
    BEGIN
        INSERT INTO [ASYN_StorageRecord]
                   ([ID]
                   ,[StorageType]
                   ,[StockState]
                   ,[IdentifyKey]
                   ,[IdentifyId]
                   ,[WarehouseId]
                   ,[IsValidateStock]
                   ,[CreateTime])
        VALUES
                   (@ID
                   ,@StorageType
                   ,@StockState
                   ,@IdentifyKey
                   ,@IdentifyId
                   ,@WarehouseId
                   ,@IsValidateStock
                   ,@CreateTime)
    END
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new {
                    ID= Guid.NewGuid(),
                    StorageType= asynStorageRecordInfo.StorageType,
                    StockState = asynStorageRecordInfo.StorageState,
                    IdentifyKey = asynStorageRecordInfo.IdentifyKey,
                    IdentifyId = asynStorageRecordInfo.IdentifyId,
                    WarehouseId = asynStorageRecordInfo.WarehouseId,
                    IsValidateStock = asynStorageRecordInfo.IsValidateStock,
                    CreateTime = DateTime.Now,
                }) > 0;
            }
        }

        public IList<ASYNStorageRecordInfo> GetList(int top)
        {
            const string SQL = @"
SELECT TOP {0} [ID]
      ,[StorageType]
      ,[StockState]
      ,[IdentifyKey]
      ,[IdentifyId]
      ,[CreateTime]
      ,[IsFail]
      ,[IsValidateStock]
  FROM [ASYN_StorageRecord]
WHERE [IsFail] = 0
ORDER BY CreateTime
";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ASYNStorageRecordInfo>(string.Format(SQL, top)).AsList();
            }
        }

        public bool Delete(Guid id)
        {
            const string SQL = @"
DELETE [ASYN_StorageRecord]
WHERE [ID]=@ID
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ID = id,
                }) > 0;
            }
        }
    }
}
