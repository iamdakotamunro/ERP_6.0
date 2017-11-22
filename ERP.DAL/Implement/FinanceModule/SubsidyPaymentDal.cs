using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ERP.Environment;
using ERP.DAL.Interface.FinanceModule;
using ERP.Model.SubsidyPayment;
using Keede.DAL.RWSplitting;
using Dapper;
using ERP.Enum.SubsidyPayment;
using Dapper.Extension;
using System.Collections;
using System.Linq;
using System.Data;
using ERP.Model;

namespace ERP.DAL.Implement.FinanceModule
{
    /// <summary>
    /// 补贴审核、补贴打款DAL
    /// </summary>
    public class SubsidyPaymentDal : ISubsidyPaymentDal
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddSubsidyPayment(SubsidyPaymentInfo_Add model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"INSERT INTO SubsidyPayment
                   (ID,OrderNumber,ThirdPartyOrderNumber,ThirdPartyAccountName,SalePlatformId,SaleFilialeId,OrderAmount,SubsidyAmount,SubsidyType,QuestionType,QuestionName,
                    BankName,BankAccountNo,UserName,Remark,
                    Status,CreateTime,CreateUser,IsDelete,ModifyTime,ModifyUser) 
              VALUES(@ID,@OrderNumber,@ThirdPartyOrderNumber,@ThirdPartyAccountName,@SalePlatformId,@SaleFilialeId,@OrderAmount,@SubsidyAmount,@SubsidyType,@QuestionType,@QuestionName,
                     @BankName,@BankAccountNo,@UserName,@Remark,
                     @Status,@CreateTime,@CreateUser,@IsDelete,@ModifyTime,@ModifyUser)";
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
                    OrderAmount = model.OrderAmount,
                    SubsidyAmount = model.SubsidyAmount,
                    SubsidyType = model.SubsidyType,
                    QuestionType = model.QuestionType,
                    QuestionName = model.QuestionName,

                    BankName = model.BankName,
                    BankAccountNo = model.BankAccountNo,
                    UserName = model.UserName,
                    Remark = model.Remark,

                    Status = (int)SubsidyPaymentStatusEnum.PendingCheck,
                    CreateTime = DateTime.Now,
                    CreateUser = model.CreateUser,
                    IsDelete = false,
                    ModifyTime = DateTime.Now,
                    ModifyUser = model.CreateUser,
                });
                return result > 0;
            }

        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateSubsidyPayment(SubsidyPaymentInfo_Edit model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"Update SubsidyPayment 
                Set OrderNumber=@OrderNumber,ThirdPartyOrderNumber=@ThirdPartyOrderNumber,SubsidyAmount=@SubsidyAmount,SubsidyType=@SubsidyType,QuestionType=@QuestionType,QuestionName=@QuestionName,
                    BankName=@BankName,BankAccountNo=@BankAccountNo,UserName=@UserName,Remark=@Remark,
                    Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser 
                where ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = model.ID,
                    OrderNumber = model.OrderNumber,
                    ThirdPartyOrderNumber = model.ThirdPartyOrderNumber,
                    SubsidyAmount = model.SubsidyAmount,
                    SubsidyType = model.SubsidyType,
                    QuestionType = model.QuestionType,
                    QuestionName = model.QuestionName,

                    BankName = model.BankName,
                    BankAccountNo = model.BankAccountNo,
                    UserName = model.UserName,
                    Remark = model.Remark,

                    Status = (int)SubsidyPaymentStatusEnum.PendingCheck,
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
        public bool ApprovalSubsidyPayment(SubsidyPaymentInfo_Approval model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"Update SubsidyPayment 
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

                    Status = model.IsApproved ? (int)SubsidyPaymentStatusEnum.PendingFinanceCheck : (int)SubsidyPaymentStatusEnum.Rejected,
                    ModifyTime = DateTime.Now,
                    ModifyUser = model.ModifyUser,
                });
                return result > 0;
            }
        }

        /// <summary>
        /// 财务审核
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckSubsidyPayment(SubsidyPaymentInfo_Check model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"Update SubsidyPayment 
                                 Set RejectReason=@RejectReason,
                                     Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser
                                 where ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = model.ID,
                    RejectReason = model.RejectReason,

                    Status = model.IsApproved ? (int)SubsidyPaymentStatusEnum.PendingPayment : (int)SubsidyPaymentStatusEnum.Rejected,
                    ModifyTime = DateTime.Now,
                    ModifyUser = model.ModifyUser,
                });
                return result > 0;
            }
        }

        /// <summary>
        /// 打款
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ApprovalPaymentSubsidyPayment(SubsidyPaymentInfo_Payment model)
        {
            if (model == null)
            {
                return false;
            }
            const string SQL = @"Update SubsidyPayment 
                                 Set AccountID=@AccountID,Fees=@Fees,TransactionNumber=@TransactionNumber,RejectReason=@RejectReason,
                                     Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser
                                 where ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = model.ID,
                    AccountID = model.AccountID,
                    Fees = model.Fees,
                    TransactionNumber = model.TransactionNumber,
                    RejectReason = model.RejectReason,

                    Status = model.IsPayment ? (int)SubsidyPaymentStatusEnum.HadPayment : (int)SubsidyPaymentStatusEnum.Rejected,
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
        public bool DeleteSubsidyPayment(Guid ID, string ModifyUser)
        {
            const string SQL = @"Update SubsidyPayment 
                                 Set Status=@Status,ModifyTime=@ModifyTime,ModifyUser=@ModifyUser 
                                 where ID=@ID";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                int result = conn.Execute(SQL, new
                {
                    ID = ID,
                    Status = (int)SubsidyPaymentStatusEnum.Cancel,
                    ModifyTime = DateTime.Now,
                    ModifyUser = ModifyUser,
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
        public IList<SubsidyPaymentInfo> GetSubsidyPaymentList(SubsidyPaymentInfo_SeachModel model, out int recordCount)
        {

            var sqlStr = new StringBuilder();

            sqlStr.Append(@"SELECT ID,OrderNumber,ThirdPartyOrderNumber,ThirdPartyAccountName,SalePlatformId,SaleFilialeId,OrderAmount,SubsidyAmount,SubsidyType,QuestionType,QuestionName,BankName,BankAccountNo,UserName,RejectReason,Status,CreateTime,CreateUser,IsDelete,ModifyTime,ModifyUser,Fees,TransactionNumber,AccountID,Remark
                            FROM SubsidyPayment
                            WHERE 1=1 ");
            if (!string.IsNullOrEmpty(model.OrderNumber))
            {
                sqlStr.Append(" AND (OrderNumber ='").Append(model.OrderNumber).Append("'");
                sqlStr.Append(" or ThirdPartyOrderNumber ='").Append(model.OrderNumber).Append("')");

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

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", sqlStr.ToString()));

                var result = conn.QueryPaged<SubsidyPaymentInfo>(sqlStr.ToString(), model.PageIndex, model.PageSize, "ModifyTime DESC");
                return result.AsList();

            }
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public SubsidyPaymentInfo GetSubsidyPaymentByID(Guid ID)
        {
            var sqlStr = new StringBuilder();

            sqlStr.Append(@"SELECT ID,OrderNumber,ThirdPartyOrderNumber,ThirdPartyAccountName,SalePlatformId,SaleFilialeId,OrderAmount,SubsidyAmount,SubsidyType,QuestionType,QuestionName,BankName,BankAccountNo,UserName,RejectReason,Status,CreateTime,CreateUser,IsDelete,ModifyTime,ModifyUser,Fees,TransactionNumber,AccountID,Remark
                            FROM SubsidyPayment
                            WHERE ID= @ID");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                var result = conn.QueryFirstOrDefault<SubsidyPaymentInfo>(sqlStr.ToString(), new { ID = ID, });
                return result;

            }

        }


        /// <summary>
        /// 根据第三方订单号 判断（1：有没有进行中（待审核、待财务审核、不通过）的费用补贴，2：补贴次数不超过2次）
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ResultModel IsExistSubsidyPayment(string ThirdPartyOrderNumber)
        {
            var listStatus = new List<int>() {
              (int)SubsidyPaymentStatusEnum.PendingCheck,
              (int)SubsidyPaymentStatusEnum.PendingFinanceCheck,
              (int)SubsidyPaymentStatusEnum.Rejected,
            };
            string strStatus = string.Join(",", listStatus.ToArray());

            var sqlStr = new StringBuilder();
            var sqlStr2 = new StringBuilder();
            sqlStr.Append(@"SELECT count(1) count
                            FROM SubsidyPayment
                            WHERE ThirdPartyOrderNumber= @ThirdPartyOrderNumber
                                AND Status in(" + strStatus + ")");

            sqlStr2.Append(@"SELECT count(1) count
                            FROM SubsidyPayment
                            WHERE ThirdPartyOrderNumber= @ThirdPartyOrderNumber
                                AND Status !=" + (int)SubsidyPaymentStatusEnum.Cancel);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                var result = conn.ExecuteScalar<int>(sqlStr.ToString(), new { ThirdPartyOrderNumber = ThirdPartyOrderNumber });
                if (result > 0)
                {
                    return new ResultModel(false, "您有进行中（待审核、待财务审核、不通过）的费用补贴！");
                }

                var result2 = conn.ExecuteScalar<int>(sqlStr2.ToString(), new { ThirdPartyOrderNumber = ThirdPartyOrderNumber });
                if (result2 > 1)
                {
                    return new ResultModel(false, "补贴次数不超过2次！");
                }
                return new ResultModel(true, null);

            }

        }
        private static string GetString(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? string.Empty : dr[column].ToString();
        }
        private static decimal GetDecimal(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : decimal.Parse(dr[column].ToString());
        }

        public ArrayList GetSumList(List<string> listID)
        {
            var sqlStr = new StringBuilder();
            ArrayList arrayList = new ArrayList();
            if (!listID.Any())
            {
                return arrayList;
            }
            string listIDStr = "'" + string.Join("','", listID) + "'";

            sqlStr.Append(@"SELECT count(*) as Count,sum(SubsidyAmount) as SumSubsidyAmount
                            FROM SubsidyPayment
                            WHERE ID in(" + listIDStr + ")");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                var dr = conn.ExecuteReader(sqlStr.ToString());

                while (dr.Read())
                {
                    arrayList.Add(GetString(dr, "Count"));
                    arrayList.Add(GetString(dr, "SumSubsidyAmount"));
                }

                return arrayList;

            }
        }
    }
}
