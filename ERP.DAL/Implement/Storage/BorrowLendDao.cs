using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ERP.DAL.Interface.IStorage;
using ERP.Model;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Storage
{
    public class BorrowLendDao : IBorrowLendDao
    {
        public BorrowLendDao(Environment.GlobalConfig.DB.FromType fromType) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="list"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddBorrowLendAndDetailList(BorrowLendInfo info, IList<BorrowLendDetailInfo> list, out string errorMessage)
        {
            errorMessage = string.Empty;
            string sql = string.Format("INSERT INTO [BorrowLend]([BorrowLendId],[StockId],[AccountReceivable],[SubtotalQuantity],[DateCreated]) VALUES('{0}','{1}',{2},{3},GETDATE())", info.BorrowLendId, info.StockId, info.AccountReceivable, info.SubtotalQuantity);
            const string SQL_DETAIL = "INSERT INTO [BorrowLendDetail]([BorrowLendId],[GoodsId],[RealGoodsId],[GoodsName],[GoodsCode],[Specification],[UnitPrice],[Quantity],[BatchNo],[Description]) VALUES(@BorrowLendId,@GoodsId,@RealGoodsId,@GoodsName,@GoodsCode,@Specification,@UnitPrice,@Quantity,@BatchNo,@Description)";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(sql, null, trans);
                        foreach (var detailInfo in list)
                        {
                            conn.Execute(SQL_DETAIL, new
                            {
                                BorrowLendId = detailInfo.BorrowLendId,
                                GoodsId = detailInfo.GoodsId,
                                RealGoodsId = detailInfo.RealGoodsId,
                                GoodsName = detailInfo.GoodsName,
                                GoodsCode = detailInfo.GoodsCode,
                                Specification = detailInfo.Specification,
                                UnitPrice = detailInfo.UnitPrice,
                                Quantity = detailInfo.Quantity,
                                BatchNo = detailInfo.BatchNo,
                                Description = detailInfo.Description,
                            }, trans);
                        }
                        trans.Commit();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        errorMessage = ex.Message;
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public BorrowLendInfo GetBorrowLendInfo(Guid stockId)
        {
            string sql = string.Format("SELECT [BorrowLendId],[StockId],[AccountReceivable],[SubtotalQuantity],[DateCreated] FROM [BorrowLend] WHERE [StockId]='{0}'", stockId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<BorrowLendInfo>(sql);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borrowLendId"></param>
        /// <returns></returns>
        public IList<BorrowLendDetailInfo> GetBorrowLendDetailList(Guid borrowLendId)
        {
            string sql = string.Format("SELECT [BorrowLendId],[GoodsId],[RealGoodsId],[GoodsName],[GoodsCode],[Specification],[UnitPrice],[Quantity],[BatchNo],[Description] FROM [BorrowLendDetail] WHERE [BorrowLendId]='{0}'", borrowLendId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<BorrowLendDetailInfo>(sql).AsList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borrowLendId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public int DeleteBorrowLendAndDetailList(Guid borrowLendId, out string errorMessage)
        {
            errorMessage = string.Empty;
            string sql = string.Format("DELETE FROM [BorrowLendDetail] WHERE [BorrowLendId]='{0}';DELETE FROM [BorrowLend] WHERE [BorrowLendId]='{0}'", borrowLendId);
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                try
                {
                    return conn.Execute(sql);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    throw;
                }
            }
        }
    }
}
