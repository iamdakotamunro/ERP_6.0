using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ERP.Environment;
using ERP.DAL.Interface.FinanceModule;
using ERP.Model.RefundsMoney;
using Keede.DAL.RWSplitting;
using Dapper;
using ERP.Enum.RefundsMoney;
using Dapper.Extension;

namespace ERP.DAL.Implement.FinanceModule
{
    /// <summary>
    /// 退款打款DAL
    /// </summary>
    public class RefundsMoneyDal : IRefundsMoneyDal
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddRefundsMoney(RefundsMoneyInfo_Add model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"INSERT INTO RefundsMoney(ID,OrderNumber,ThirdPartyOrderNumber,ThirdPartyAccountName,SalePlatformId,SaleFilialeId,RefundsAmount,
                                                          Status,CreateTime,CreateUser,IsDelete,ModifyTime,ModifyUser,AfterSalesNumber)
                                 VALUES(@ID,@OrderNumber,@ThirdPartyOrderNumber,@ThirdPartyAccountName,@SalePlatformId,@SaleFilialeId,@RefundsAmount,
                                        @Status,@CreateTime,@CreateUser,@IsDelete,@ModifyTime,@ModifyUser,@AfterSalesNumber)";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = model.ID,
                    OrderNumber = model.OrderNumber,
                    ThirdPartyOrderNumber = model.ThirdPartyOrderNumber,
                    ThirdPartyAccountName = model.ThirdPartyAccountName,
                    SalePlatformId = model.SalePlatformId,
                    SaleFilialeId = model.SaleFilialeId,
                    RefundsAmount = model.RefundsAmount,

                    Status = (int)RefundsMoneyStatusEnum.PendingCheck,
                    CreateTime = DateTime.Now,
                    CreateUser = model.CreateUser,
                    IsDelete = false,
                    ModifyTime = DateTime.Now,
                    ModifyUser = model.CreateUser,
                    AfterSalesNumber = model.AfterSalesNumber,
                });
                return result > 0;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateRefundsMoney(RefundsMoneyInfo_Edit model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"Update RefundsMoney
                                 Set BankName=@BankName,BankAccountNo=@BankAccountNo,UserName=@UserName,Remark=@Remark,
                                     Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser
                                 where ID=@ID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = model.ID,
                    BankName = model.BankName,
                    BankAccountNo = model.BankAccountNo,
                    UserName = model.UserName,
                    Remark = model.Remark,

                    Status = (int)RefundsMoneyStatusEnum.PendingCheck,
                    ModifyTime = DateTime.Now,
                    ModifyUser = model.ModifyUser,
                });
                return result > 0;
            }
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ApprovalRefundsMoney(RefundsMoneyInfo_Approval model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"Update RefundsMoney
                                 Set BankName=@BankName,BankAccountNo=@BankAccountNo,UserName=@UserName,RejectReason=@RejectReason,
                                     Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser
                                 where ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = model.ID,
                    BankName = model.BankName,
                    BankAccountNo = model.BankAccountNo,
                    UserName = model.UserName,
                    RejectReason = model.RejectReason,

                    Status = model.IsApproved ? (int)RefundsMoneyStatusEnum.PendingPayment : (int)RefundsMoneyStatusEnum.Rejected,
                    ModifyTime = DateTime.Now,
                    ModifyUser = model.ModifyUser,
                });
                return result > 0;
            }
        }

        /// <summary>
        /// 删除（设置为已作废）
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ModifyUser"></param>
        /// <returns></returns>
        public bool DeleteRefundsMoney(Guid ID, string ModifyUser)
        {
            const string SQL = @"Update RefundsMoney
                                 Set Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser
                                 where ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = ID,
                    Status = (int)RefundsMoneyStatusEnum.Cancel,
                    ModifyTime = DateTime.Now,
                    ModifyUser = ModifyUser,
                });
                return result > 0;
            }
        }

        /// <summary>
        /// 审核打款
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ApprovalPaymentRefundsMoney(RefundsMoneyInfo_Payment model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"Update RefundsMoney
                                 Set Fees=@Fees,AccountID=@AccountID,TransactionNumber=@TransactionNumber,RejectReason=@RejectReason,
                                     Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser
                                 where ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = model.ID,
                    Fees = model.Fees,
                    AccountID = model.AccountID,
                    TransactionNumber = model.TransactionNumber,
                    RejectReason = model.RejectReason,

                    Status = model.IsPayment ? (int)RefundsMoneyStatusEnum.HadPayment : (int)RefundsMoneyStatusEnum.Rejected,
                    ModifyTime = DateTime.Now,
                    ModifyUser = model.ModifyUser,
                });
                return result > 0;
            }
        }

        /// <summary>
        /// 获取分页的列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public IList<RefundsMoneyInfo> GetRefundsMoneyList(RefundsMoneyInfo_SeachModel model, out int recordCount)
        {
            var sqlStr = new StringBuilder();

            sqlStr.Append(@"SELECT ID,OrderNumber,ThirdPartyOrderNumber,ThirdPartyAccountName,SalePlatformId,SaleFilialeId,RefundsAmount,BankName,BankAccountNo,UserName,
                                   Fees,TransactionNumber,AccountID,RejectReason,Status,CreateTime,CreateUser,IsDelete,ModifyTime,ModifyUser,AfterSalesNumber,Remark
                            FROM RefundsMoney
                            WHERE 1=1 ");

            #region 筛选条件

            if (!string.IsNullOrEmpty(model.OrderNumber))
            {
                sqlStr.Append(" AND (OrderNumber ='").Append(model.OrderNumber).Append("'");
                sqlStr.Append(" or ThirdPartyOrderNumber ='").Append(model.OrderNumber).Append("'");
                sqlStr.Append(" or AfterSalesNumber ='").Append(model.OrderNumber).Append("')");
            }

            if (model.StartTime.HasValue)
            {
                sqlStr.Append(" AND CreateTime >= '").Append(model.StartTime).Append("'");
            }

            if (model.EndTime.HasValue)
            {
                sqlStr.Append(" AND CreateTime <= '").Append(model.EndTime).Append("'");
            }

            if (model.Status != 0)
            {
                sqlStr.Append(" AND Status =").Append(model.Status);
            }
            else if (model.listStatus != null && model.listStatus.Count > 0)
            {
                string strStatus = string.Join(",", model.listStatus.ToArray());
                sqlStr.Append(" AND Status in(" + strStatus + ")");
            }

            if (model.SalePlatformId != Guid.Empty)
            {
                sqlStr.Append(" AND SalePlatformId = '").Append(model.SalePlatformId).Append("'");
            }

            if (model.SaleFilialeId != Guid.Empty)
            {
                sqlStr.Append(" AND SaleFilialeId = '").Append(model.SaleFilialeId).Append("'");
            }

            #endregion 筛选条件

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", sqlStr.ToString()));

                var result = conn.QueryPaged<RefundsMoneyInfo>(sqlStr.ToString(), model.PageIndex, model.PageIndex, " ID DESC");
                return result.AsList();
            }
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public RefundsMoneyInfo GetRefundsMoneyByID(Guid ID)
        {
            var sqlStr = new StringBuilder();

            sqlStr.Append(@"SELECT ID,OrderNumber,ThirdPartyOrderNumber,ThirdPartyAccountName,SalePlatformId,SaleFilialeId,RefundsAmount,BankName,BankAccountNo,UserName,
                                   Fees,TransactionNumber,AccountID,RejectReason,Status,CreateTime,CreateUser,IsDelete,ModifyTime,ModifyUser,AfterSalesNumber,Remark
                            FROM RefundsMoney
                            WHERE ID= @ID");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                var result = conn.QueryFirstOrDefault<RefundsMoneyInfo>(sqlStr.ToString(), new { ID = ID, });
                return result;
            }
        }
    }
}