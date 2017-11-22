using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class CompanyFundReceipt : ICompanyFundReceipt
    {
        public CompanyFundReceipt(GlobalConfig.DB.FromType fromType) { }

        #region -- SQL脚本信息
        private const string SQL_SELECT_ALL_FIELDS = @" SELECT 
                                                        ReceiptID,
                                                        ReceiptNo,
                                                        ReceiptType,
                                                        ApplicantID,
                                                        ApplyDateTime,
                                                        PurchaseOrderNo,
                                                        CompanyID,
                                                        HasInvoice,
                                                        SettleStartDate,
                                                        SettleEndDate,
                                                        ExpectBalance,
                                                        RealityBalance,
                                                        DiscountMoney,
                                                        DiscountCaption,
                                                        OtherDiscountCaption,
                                                        ReceiptStatus,
                                                        Remark,
                                                        AuditorID,
                                                        AuditFailureReason,
                                                        InvoicesDemander,
                                                        StockOrderNos,
                                                        ExecuteDateTime,Poundage,
                                                        AuditingDate,FinishDate,FilialeId,DealFlowNo,IsOut,PayBankAccountsId,IncludeStockNos,DebarStockNos,LastRebate,PaymentDate,ReturnOrder,PayOrder,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState ";
        private const string SQL_FROMTABLE = @" FROM [lmshop_CompanyFundReceipt] ";
        private const string SQL_WHERE_DEFAULT_FILTER = @" WHERE ReceiptStatus IN (@ReceiptStatus) AND ApplicantID=@ApplicantID";
        private const string SQL_ORDERBY_DEFAULT = @" ORDER BY ApplyDateTime DESC";
        private const string SQL_INSERT = @"INSERT INTO [lmshop_CompanyFundReceipt] (ReceiptNo,ReceiptType,ApplicantID,ApplyDateTime,PurchaseOrderNo,CompanyID,HasInvoice,SettleStartDate,SettleEndDate,ExpectBalance,RealityBalance,DiscountMoney,DiscountCaption,OtherDiscountCaption,ReceiptStatus,StockOrderNos,FilialeId,IsOut,PayBankAccountsId,IncludeStockNos,DebarStockNos,LastRebate,PaymentDate,ReturnOrder,PayOrder,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState)
                                        VALUES (@ReceiptNo,@ReceiptType,@ApplicantID,@ApplyDateTime,@PurchaseOrderNo,@CompanyID,@HasInvoice,@SettleStartDate,@SettleEndDate,@ExpectBalance,@RealityBalance,@DiscountMoney,@DiscountCaption,@OtherDiscountCaption,@ReceiptStatus,@StockOrderNos,@FilialeId,@IsOut,@PayBankAccountsId,@IncludeStockNos,@DebarStockNos,@LastRebate,@PaymentDate,@ReturnOrder,@PayOrder,@ReceiveBankAccountId,@InvoiceType,@IsUrgent,@UrgentRemark,
@InvoiceUnit,@InvoiceState)";

        private const string SQL_INSERT_NEW = @"INSERT INTO [lmshop_CompanyFundReceipt] (ReceiptNo,ReceiptType,ApplicantID,ApplyDateTime,PurchaseOrderNo,CompanyID,HasInvoice,SettleStartDate,SettleEndDate,ExpectBalance,RealityBalance,DiscountMoney,DiscountCaption,OtherDiscountCaption,ReceiptStatus,StockOrderNos,FilialeId,Remark,IsOut,PayBankAccountsId,IncludeStockNos,DebarStockNos,LastRebate,PaymentDate,ReturnOrder,PayOrder,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState)
                                        VALUES (@ReceiptNo,@ReceiptType,@ApplicantID,@ApplyDateTime,@PurchaseOrderNo,@CompanyID,@HasInvoice,@SettleStartDate,@SettleEndDate,@ExpectBalance,@RealityBalance,@DiscountMoney,@DiscountCaption,@OtherDiscountCaption,@ReceiptStatus,@StockOrderNos,@FilialeId,@Remark,@IsOut,@PayBankAccountsId,@IncludeStockNos,@DebarStockNos,@LastRebate,@PaymentDate,@ReturnOrder,@PayOrder,@ReceiveBankAccountId,@InvoiceType,@IsUrgent,@UrgentRemark,@InvoiceUnit,@InvoiceState)";

        private const string SQL_UPDATE = @"UPDATE [lmshop_CompanyFundReceipt] SET PurchaseOrderNo=@PurchaseOrderNo,CompanyID=@CompanyID,HasInvoice=@HasInvoice,SettleStartDate=@SettleStartDate,
                                          SettleEndDate=@SettleEndDate,ExpectBalance=@ExpectBalance,RealityBalance=@RealityBalance,DiscountMoney=@DiscountMoney,
                                          DiscountCaption=@DiscountCaption,OtherDiscountCaption=@OtherDiscountCaption,ReceiptStatus=@ReceiptStatus,StockOrderNos=@StockOrderNos,FilialeId=@FilialeId,Remark=Remark+@Remark,PayBankAccountsId=@PayBankAccountsId,IncludeStockNos=@IncludeStockNos,DebarStockNos=@DebarStockNos,LastRebate=@LastRebate,PaymentDate=@PaymentDate,ReturnOrder=@ReturnOrder,PayOrder=@PayOrder,ReceiveBankAccountId=@ReceiveBankAccountId,InvoiceType=@InvoiceType,IsUrgent=@IsUrgent,UrgentRemark=@UrgentRemark,InvoiceUnit=@InvoiceUnit,InvoiceState=@InvoiceState WHERE ReceiptID=@ReceiptID";

        private const string SQL_UPDATE_POUNDAGE = @"UPDATE [lmshop_CompanyFundReceipt] SET Poundage=@Poundage WHERE ReceiptID=@ReceiptID ";

        #endregion

        #region ICompanyFundReceipt 成员

        public IList<CompanyFundReceiptInfo> GetAllFundReceiptInfoList(Guid filialeId, ReceiptPage page, CompanyFundReceiptState state, DateTime appStartTime, DateTime appEndTime, string companyFundReceiptNo, CompanyFundReceiptType type, params Guid[] invoicesDemander)
        {
            string sql = SQL_SELECT_ALL_FIELDS + SQL_FROMTABLE + " WHERE ApplyDateTime >= @start AND ApplyDateTime < @end  AND CHARINDEX(@receiptNo,ReceiptNo)>0 AND [ReceiptType]=@type ";
            var listparam = new List<SqlParameter>
                                               {
                                                   appStartTime == DateTime.MinValue?
                                                   new SqlParameter("@start", new DateTime(2000, 1, 1)):
                                                   new SqlParameter("@start", appStartTime),
                                                   new SqlParameter("@end", appEndTime)
                                               };
            if (filialeId != Guid.Empty)
            {
                sql += string.Format(" AND FilialeId='{0}' ", filialeId);
            }
            if (-1 == (int)type)
            {
                sql = sql.Replace("AND [ReceiptType]=@type", "");
            }
            else listparam.Add(new SqlParameter("@type", (int)type));

            if (string.IsNullOrEmpty(companyFundReceiptNo))
            {
                sql = sql.Replace("AND CHARINDEX(@receiptNo,ReceiptNo)>0", "");
            }
            else listparam.Add(new SqlParameter("@receiptNo", companyFundReceiptNo));

            if (invoicesDemander.Length != 0)
            {
                sql += " AND [InvoicesDemander]=@demander";
                listparam.Add(new SqlParameter("@demander", invoicesDemander[0]));
            }
            //如果是按收付款编号搜索则过滤掉状态限制
            if (string.IsNullOrWhiteSpace(companyFundReceiptNo))
            {
                //根据不同页面获取不同状态sql
                var stateSql = GetReceiptStateByPage(page, state);
                sql += stateSql;
            }
            sql += " ORDER BY ApplyDateTime DESC";

            IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, listparam.ToArray()))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    list.Add(entity);
                }
            }
            return list;
        }

        /// <summary>
        /// 开具发票(New)
        /// </summary>
        /// <param name="status"></param>
        /// <param name="state"></param>
        /// <param name="appStartTime"></param>
        /// <param name="appEndTime"></param>
        /// <param name="companyFundReceiptNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<CompanyFundReceiptInfo> GetFundReceiptInfoList(List<int> status, List<int> otherStatus, int state, DateTime appStartTime, DateTime appEndTime, string companyFundReceiptNo, int type)
        {
            StringBuilder builder = new StringBuilder(SQL_SELECT_ALL_FIELDS);
            builder.Append(SQL_FROMTABLE);
            builder.Append(" WHERE ApplyDateTime >= @start AND ApplyDateTime < @end AND [ReceiptType]=@type ");
            var listparam = new List<SqlParameter>
                                               {
                                                   new SqlParameter("@type", type),
                                                   new SqlParameter("@start", appStartTime == DateTime.MinValue ? new DateTime(2000, 1, 1) : appStartTime),
                                                   new SqlParameter("@end", appEndTime == DateTime.MinValue?DateTime.Now:appEndTime)
                                               };
            if (status.Count > 0)
            {
                if (state != -1)
                    builder.AppendFormat(" AND (ReceiptStatus IN({0}) OR (ReceiptStatus IN({1}) AND InvoiceState={2}))", string.Join(",", status), string.Join(",", otherStatus), state);
                else
                    builder.AppendFormat(" AND ReceiptStatus IN({0}) ", string.Join(",", status));
            }
            else
            {
                builder.AppendFormat(" AND ReceiptStatus IN({0}) AND InvoiceState={1}", string.Join(",", otherStatus), state);
            }
            if (!string.IsNullOrEmpty(companyFundReceiptNo))
                builder.AppendFormat(" AND ReceiptNo LIKE '%{0}%'", companyFundReceiptNo);

            List<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), listparam.ToArray()))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    list.Add(entity);
                }
            }
            return list;
        }

        public int UpdateFundReceiptState(Guid receiptId, CompanyFundReceiptState state)
        {
            const string SQL = "UPDATE lmshop_CompanyFundReceipt SET [ReceiptStatus]=@state WHERE [ReceiptID]=@receiptID";
            var sqlparams = new[]{
                new SqlParameter("@state",(int)state),
                new SqlParameter("@receiptID",receiptId)};
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlparams);
        }

        public int UpdateFundReceiptAuditorID(Guid receiptId, Guid personnelId)
        {
            const string SQL = "UPDATE lmshop_CompanyFundReceipt SET [AuditorID]=@personnelId WHERE [ReceiptID]=@receiptID";
            var sqlparams = new[]{
                new SqlParameter("@personnelId",personnelId),
                new SqlParameter("@receiptID",receiptId)};
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlparams);
        }

        public int UpdataFundReceiptInvoicesDemander(Guid receiptId, Guid personnelId)
        {
            const string SQL = "UPDATE lmshop_CompanyFundReceipt SET [InvoicesDemander]=@personnelId WHERE [ReceiptID]=@receiptID";
            var sqlparams = new[]{
                new SqlParameter("@personnelId",personnelId),
                new SqlParameter("@receiptID",receiptId)};
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlparams);
        }

        public int UpdateFundReceiptRemark(Guid receiptId, string remark)
        {
            const string SQL = "UPDATE lmshop_CompanyFundReceipt SET [Remark]=ISNULL([Remark],'')+@Remark WHERE [ReceiptID]=@receiptID";
            var sqlparams = new[]{
                new SqlParameter("@remark",remark),
                new SqlParameter("@receiptID",receiptId)};
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlparams);
        }

        public IList<CompanyFundReceiptInfo> GetAllPrompinfo(Guid personnelId, Guid filialeId, Guid branchId, Guid positionId)
        {
            string sql = "SELECT DISTINCT ReceiptStatus,ReceiptType,[ReceiptID] FROM [lmshop_CompanyFundReceipt] INNER JOIN [lmshop_CompanyAuditingPower] [power] on lmshop_CompanyFundReceipt.CompanyID=[power].CompanyID AND [power].[FilialeId]=@filialeId AND [power].BranchID=@branchId AND [power].[PositionID]=@positionId AND RealityBalance<=[power].[UpperMoney] AND RealityBalance>=[power].[LowerMoney] AND (ReceiptStatus=" + (int)CompanyFundReceiptState.WaitAuditing + " OR ReceiptStatus=" + (int)CompanyFundReceiptState.Audited + ") union SELECT DISTINCT ReceiptStatus,ReceiptType,[ReceiptID] FROM [lmshop_CompanyFundReceipt] inner join lmshop_CompanyInvoicePower ipower on lmshop_CompanyFundReceipt.CompanyID=ipower.CompanyID AND ((ReceiptStatus=" + (int)CompanyFundReceiptState.WaitInvoice + " AND ipower.InvoicesType=" + (int)CompanyFundReceiptInvoiceType.OpenInvoice + ") OR (ReceiptStatus=" + (int)CompanyFundReceiptState.GettingInvoice + " AND ipower.InvoicesType=" + (int)CompanyFundReceiptInvoiceType.CollectInvoice + ")) AND ((ipower.[FilialeId]=@filialeId AND ipower.BranchID=@branchId AND ipower.[PositionID]=@positionId) OR ipower.[AuditorID]=@personnelId)";

            var sqlparams = new[]{
                new SqlParameter("@positionId",positionId),
                new SqlParameter("@personnelId",personnelId),
                new SqlParameter("@filialeId",filialeId),
                new SqlParameter("@branchId",branchId)
            };


            IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, sqlparams))
            {
                while (dr.Read())
                {
                    var entity = new CompanyFundReceiptInfo { ReceiptStatus = dr.GetInt32(0), ReceiptType = dr.GetInt32(1) };
                    list.Add(entity);
                }
            }
            return list;
        }

        public CompanyFundReceiptInfo GetCompanyFundReceiptInfo(Guid id)
        {
            const string SQL = SQL_SELECT_ALL_FIELDS + SQL_FROMTABLE + " WHERE [ReceiptID]=@ID";
            var sqlparams = new[] { new SqlParameter("@ID", id) };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, sqlparams))
            {
                if (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    return entity;
                }
            }
            return null;
        }

        #region -- 设定单据状态
        /// <summary>
        /// 设定单据状态
        /// code：阮剑锋
        /// </summary>
        /// <param name="receiptId">单据ID</param>
        /// <param name="receiptStatus">单据状态</param>
        /// <returns></returns>
        public bool UpdateToReceiptStatus(Guid receiptId, int receiptStatus)
        {
            const string CMD_TEXT = "UPDATE [lmshop_CompanyFundReceipt] SET [ReceiptStatus]=@ReceiptStatus WHERE [ReceiptID]=@receiptID ";
            var sqlparams = new[]
            {
                new SqlParameter("@ReceiptStatus",receiptStatus),
                new SqlParameter("@receiptID",receiptId)
            };
            return ExecuteNonQuery(CMD_TEXT, sqlparams);

        }
        #endregion

        #region -- 增加备注信息
        /// <summary>
        /// 增加备注信息，
        /// Code：阮剑锋
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="remarkContent"></param>
        /// <returns></returns>
        public bool InsertToRemark(Guid receiptId, string remarkContent)
        {
            const string CMD_TEXT = "UPDATE [lmshop_CompanyFundReceipt] SET [Remark]=(ISNULL([Remark],'')+@Remark) WHERE [ReceiptID]=@receiptID ";
            var sqlparams = new[]
            {
                new SqlParameter("@Remark",remarkContent),
                new SqlParameter("@receiptID",receiptId)
            };
            return ExecuteNonQuery(CMD_TEXT, sqlparams);
        }
        #endregion

        #region -- 获取备注内容
        /// <summary>
        /// 获取备注内容
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        public string GetRemarkContent(Guid receiptId)
        {
            const string SQL = "SELECT Remark " + SQL_FROMTABLE + " WHERE [ReceiptID]=@receiptID ";
            object obj = ExecuteScalar(SQL, new SqlParameter("@receiptID", receiptId));
            return obj == null ? string.Empty : obj.ToString();
        }
        #endregion

        #region -- ExecuteNonQuery(私有，通用)
        private bool ExecuteNonQuery(string cmdText, params SqlParameter[] parms)
        {
            int a = SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, cmdText, parms);
            return a > 0;
        }
        #endregion

        #region -- ExecuteScalar(私有，通用)
        private object ExecuteScalar(string cmdText, params SqlParameter[] parms)
        {
            return SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, cmdText, parms);
        }
        #endregion

        #region -- 默认初次载入的单据列表
        /// <summary>
        /// 默认初次载入的单据列表
        /// code：阮剑锋
        /// </summary>
        /// <returns></returns>
        public IList<CompanyFundReceiptInfo> GetDefaultFundReceiptInfoList(Guid applicantId)
        {
            IList<CompanyFundReceiptInfo> receiptList = new List<CompanyFundReceiptInfo>();
            var statusList = new[] { ((int)CompanyFundReceiptState.NoAuditing).ToString(), ((int)CompanyFundReceiptState.WaitAuditing).ToString(), ((int)CompanyFundReceiptState.WaitInvoice).ToString(), ((int)CompanyFundReceiptState.Audited).ToString() };
            var sqlParamS = new[] { new SqlParameter("@ApplicantID", applicantId) };
            string cmdText = SQL_SELECT_ALL_FIELDS + SQL_FROMTABLE + SQL_WHERE_DEFAULT_FILTER.Replace("@ReceiptStatus", string.Join(",", statusList)) + SQL_ORDERBY_DEFAULT;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, cmdText, sqlParamS))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    receiptList.Add(entity);
                }
            }
            return receiptList;
        }
        /// <summary>
        /// 往来单位收付款列表
        /// </summary>
        /// <param name="applicantId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        /// zal 2016-03-22
        public IList<CompanyFundReceiptInfo> GetDefaultFundReceiptInfoListByPage(Guid applicantId, int pageIndex, int pageSize, out int total)
        {
            var sql = @"
with tabs as  
(  
" + SQL_SELECT_ALL_FIELDS + @",ROW_NUMBER() over(order by ApplyDateTime desc) as RowNum
from lmshop_CompanyFundReceipt
WHERE ReceiptStatus IN (0,1,2,3) 
AND ApplicantID=@ApplicantID
and ReceiptID not in(
select ReceiptID from lmshop_CompanyFundReceipt
WHERE ReceiptStatus=3 
AND ApplicantID=@ApplicantID
and ReceiptType=1
)
) 
";
            var paras = new[] { new SqlParameter("@ApplicantID", applicantId) };
            var sqlCount = sql + " select count(0) from tabs";
            var count = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlCount, paras);
            total = int.Parse(count.ToString());
            var sqlData = sql + SQL_SELECT_ALL_FIELDS + " from tabs where RowNum between @pageSize*(@pageIndex-1) + 1 and @pageSize*@pageIndex";
            var sqlDataParas = new[]
            {
                new SqlParameter("@ApplicantID", applicantId),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@pageIndex", pageIndex)
            };
            IList<CompanyFundReceiptInfo> receiptList = new List<CompanyFundReceiptInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlData, sqlDataParas))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    receiptList.Add(entity);
                }
            }
            return receiptList;
        }

        /// <summary>
        /// 往来单位收付款 查询按钮调用的方法
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="page"></param>
        /// <param name="state"></param>
        /// <param name="appStartTime"></param>
        /// <param name="appEndTime"></param>
        /// <param name="companyFundReceiptNo"></param>
        /// <param name="type"></param>
        /// <param name="companyId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="invoicesDemander"></param>
        /// <returns></returns>
        /// zal 2016-03-22
        public IList<CompanyFundReceiptInfo> GetAllFundReceiptInfoListByPage(Guid filialeId, ReceiptPage page, CompanyFundReceiptState state, DateTime appStartTime, DateTime appEndTime, string companyFundReceiptNo, CompanyFundReceiptType type, Guid companyId, int pageIndex, int pageSize, out int total, params Guid[] invoicesDemander)
        {

            string sql = "with tabs as (" + SQL_SELECT_ALL_FIELDS + ",ROW_NUMBER() over(order by ApplyDateTime desc) as RowNum " + SQL_FROMTABLE + " WHERE ApplyDateTime >= @start AND ApplyDateTime < @end  AND CHARINDEX(@receiptNo,ReceiptNo)>0 AND [ReceiptType]=@type ";
            var listparam = new List<SqlParameter>
                                               {
                                                   appStartTime == DateTime.MinValue?
                                                   new SqlParameter("@start", new DateTime(2000, 1, 1)):
                                                   new SqlParameter("@start", appStartTime),
                                                   new SqlParameter("@end", appEndTime)
                                               };
            if (filialeId != Guid.Empty)
            {
                sql += string.Format(" AND FilialeId='{0}' ", filialeId);
            }
            if (companyId != Guid.Empty)
            {
                sql += string.Format(" AND CompanyId='{0}' ", companyId);
            }
            if (-1 == (int)type)
            {
                sql = sql.Replace("AND [ReceiptType]=@type", "");
            }
            else listparam.Add(new SqlParameter("@type", (int)type));

            if (string.IsNullOrEmpty(companyFundReceiptNo))
            {
                sql = sql.Replace("AND CHARINDEX(@receiptNo,ReceiptNo)>0", "");
            }
            else listparam.Add(new SqlParameter("@receiptNo", companyFundReceiptNo));

            if (invoicesDemander.Length != 0)
            {
                sql += " AND [InvoicesDemander]=@demander";
                listparam.Add(new SqlParameter("@demander", invoicesDemander[0]));
            }
            //如果是按收付款编号搜索则过滤掉状态限制
            if (string.IsNullOrWhiteSpace(companyFundReceiptNo))
            {
                //根据不同页面获取不同状态sql
                var stateSql = GetReceiptStateByPage(page, state);
                sql += stateSql;
            }
            sql += ")";

            var sqlCount = sql + " select count(0) from tabs";
            var count = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlCount, listparam.ToArray());
            total = int.Parse(count.ToString());
            var sqlData = sql + SQL_SELECT_ALL_FIELDS + " from tabs where RowNum between @pageSize*(@pageIndex-1) + 1 and @pageSize*@pageIndex";
            listparam.Add(new SqlParameter("@pageSize", pageSize));
            listparam.Add(new SqlParameter("@pageIndex", pageIndex));

            IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlData, listparam.ToArray()))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    list.Add(entity);
                }
            }
            return list;
        }
        #endregion

        #region -- 增加单据

        /// <summary>增加单据 code:阮剑锋
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Insert(CompanyFundReceiptInfo entity)
        {
            var list = new List<SqlParameter>
            {
                new SqlParameter("@ReceiptNo",entity.ReceiptNo),
                new SqlParameter("@ReceiptType",entity.ReceiptType),
                new SqlParameter("@ApplicantID",entity.ApplicantID),
                new SqlParameter("@ApplyDateTime",entity.ApplyDateTime),
                new SqlParameter("@PurchaseOrderNo",entity.PurchaseOrderNo),
                new SqlParameter("@CompanyID",entity.CompanyID),
                new SqlParameter("@HasInvoice",entity.HasInvoice),
                new SqlParameter("@SettleStartDate",entity.SettleStartDate),
                new SqlParameter("@SettleEndDate",entity.SettleEndDate),
                new SqlParameter("@ExpectBalance",entity.ExpectBalance),
                new SqlParameter("@RealityBalance",entity.RealityBalance),
                new SqlParameter("@DiscountMoney",entity.DiscountMoney),
                new SqlParameter("@DiscountCaption",entity.DiscountCaption),
                new SqlParameter("@OtherDiscountCaption",entity.OtherDiscountCaption),
                new SqlParameter("@ReceiptStatus",entity.ReceiptStatus),
                new SqlParameter("@StockOrderNos",entity.StockOrderNos),
                new SqlParameter("@FilialeId",entity.FilialeId),
                new SqlParameter("@IsOut",entity.IsOut),
                new SqlParameter("@PayBankAccountsId",entity.PayBankAccountsId),
                new SqlParameter("@IncludeStockNos",entity.IncludeStockNos),
                new SqlParameter("@DebarStockNos",entity.DebarStockNos),
                new SqlParameter("@LastRebate",entity.LastRebate),
                new SqlParameter("@PaymentDate",entity.PaymentDate),
                new SqlParameter("@ReturnOrder",entity.ReturnOrder),
                new SqlParameter("@PayOrder",entity.PayOrder),
                new SqlParameter("@ReceiveBankAccountId",entity.ReceiveBankAccountId),
                new SqlParameter("@InvoiceType",entity.InvoiceType),
                new SqlParameter("@IsUrgent",entity.IsUrgent),
                new SqlParameter("@UrgentRemark",entity.UrgentRemark),
                new SqlParameter("@InvoiceUnit",entity.InvoiceUnit??string.Empty),
                new SqlParameter("@InvoiceState",entity.InvoiceState)
            };
            string sql = string.IsNullOrEmpty(entity.Remark) ? SQL_INSERT : SQL_INSERT_NEW;
            if (!string.IsNullOrEmpty(entity.Remark))
            {
                list.Add(new SqlParameter("@Remark", entity.Remark));
            }
            return ExecuteNonQuery(sql, list.ToArray());
        }

        #endregion

        #region -- 修改保存单据

        /// <summary> 修改保存单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(CompanyFundReceiptInfo entity)
        {
            var sqlparams = new[]
            {
                new SqlParameter("@ReceiptID",entity.ReceiptID),
                new SqlParameter("@PurchaseOrderNo",entity.PurchaseOrderNo),
                new SqlParameter("@CompanyID",entity.CompanyID),
                new SqlParameter("@HasInvoice",entity.HasInvoice),
                new SqlParameter("@SettleStartDate",entity.SettleStartDate),
                new SqlParameter("@SettleEndDate",entity.SettleEndDate),
                new SqlParameter("@ExpectBalance",entity.ExpectBalance),
                new SqlParameter("@RealityBalance",entity.RealityBalance),
                new SqlParameter("@DiscountMoney",entity.DiscountMoney),
                new SqlParameter("@DiscountCaption",entity.DiscountCaption),
                new SqlParameter("@OtherDiscountCaption",entity.OtherDiscountCaption),
                new SqlParameter("@ReceiptStatus",entity.ReceiptStatus),
                new SqlParameter("@StockOrderNos",entity.StockOrderNos),
                new SqlParameter("@FilialeId",entity.FilialeId),
                new SqlParameter("@Remark",string.IsNullOrEmpty(entity.Remark)?"":entity.Remark),
                new SqlParameter("@PayBankAccountsId",entity.PayBankAccountsId),
                new SqlParameter("@IncludeStockNos",entity.IncludeStockNos),
                new SqlParameter("@DebarStockNos",entity.DebarStockNos),
                new SqlParameter("@LastRebate",entity.LastRebate),
                new SqlParameter("@PaymentDate",entity.PaymentDate),
                new SqlParameter("@ReturnOrder",entity.ReturnOrder),
                new SqlParameter("@PayOrder",entity.PayOrder),
                new SqlParameter("@ReceiveBankAccountId",entity.ReceiveBankAccountId),
                new SqlParameter("@InvoiceType",entity.InvoiceType),
                new SqlParameter("IsUrgent",entity.IsUrgent),
                new SqlParameter("@UrgentRemark",entity.UrgentRemark),
                new SqlParameter("@InvoiceUnit",entity.InvoiceUnit??string.Empty),
                new SqlParameter("@InvoiceState",entity.InvoiceState)
            };
            return ExecuteNonQuery(SQL_UPDATE, sqlparams);
        }

        public bool UpdateInvoice(Guid receiptId, int receiptStatus, string invoiceUnit, int invoiceState, string remark, Guid auditorId)
        {
            StringBuilder builder = new StringBuilder(@"UPDATE lmshop_CompanyFundReceipt SET ReceiptStatus=@ReceiptStatus,InvoiceUnit=@InvoiceUnit,InvoiceState=@InvoiceState,Remark=Remark+@Remark ");
            if (auditorId != Guid.Empty)
            {
                builder.AppendFormat(",AuditorID='{0}',AuditingDate='{1}'", auditorId, DateTime.Now);
            }
            builder.Append(" WHERE ReceiptID=@ReceiptID ");
            return ExecuteNonQuery(builder.ToString(), new[]
            {
                new SqlParameter("@ReceiptStatus",receiptStatus),
                new SqlParameter("@InvoiceUnit",invoiceUnit??string.Empty),
                new SqlParameter("@InvoiceState",invoiceState),
                new SqlParameter("@Remark",remark),
                new SqlParameter("@ReceiptID",receiptId)
            });
        }
        #endregion

        #region[统计]

        /// <summary>
        /// 统计
        /// Add by liucaijun at 2011-June-13th
        /// Modify by liangcanren at 2015-03-19  添加往来单位类型条件限制
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="auditingId">审批人ID</param>
        /// <param name="payBankAccountsId">付款银行</param>
        /// <param name="hasInvoice">是否有发票</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截至日期</param>
        /// <param name="filialeId"></param> //,Guid filialeId)
        /// <returns></returns>
        public IList<CompanyFundReceiptInfo> GetCompanyFundStatistics(Guid companyId, Guid auditingId, Guid payBankAccountsId, bool hasInvoice,
            DateTime startDate, DateTime endDate, Guid filialeId)
        {
            var sqlWhere = new StringBuilder(" AND ((ReceiptStatus>=4 AND ReceiptStatus<8) OR ReceiptStatus=9) ");
            if (companyId != Guid.Empty)
            {
                sqlWhere.Append(" AND CompanyID=@CompanyID ");
            }
            if (hasInvoice)
                sqlWhere.Append(" AND IsOut=@IsOut ");
            if (startDate != DateTime.MinValue)
            {
                sqlWhere.Append(" AND ApplyDateTime>='" + startDate + "' ");
            }
            if (endDate != DateTime.MinValue)
            {
                sqlWhere.Append(" AND ApplyDateTime<='" + endDate + "' ");
            }
            if (payBankAccountsId != Guid.Empty)
            {
                sqlWhere.Append(" AND PayBankAccountsId='" + payBankAccountsId + "' ");
            }
            if (filialeId != Guid.Empty)
            {
                sqlWhere.AppendFormat(" AND FilialeId ='{0}' ", filialeId);
            }
            var sql = new StringBuilder();
            if (auditingId == Guid.Empty)
            {
                sql.Append(@"SELECT CompanyID,SUM(CASE ReceiptType WHEN 0 then RealityBalance else 0 end ) AS RealityBalance,null as AuditorID,
            SUM(CASE ReceiptType WHEN 1 then DiscountMoney else 0 end) AS DiscountMoney,SUM(LastRebate) AS LastRebate,SUM(CASE ReceiptType WHEN 1 then RealityBalance else 0 end ) 
AS PayRealityBalance FROM lmshop_CompanyFundReceipt WHERE 1=1 ");
                sql.Append(sqlWhere);
                sql.Append(" GROUP BY CompanyID ");
            }
            else
            {
                sql.Append(@"SELECT CompanyID,SUM(CASE ReceiptType WHEN 0 then RealityBalance else 0 end ) AS RealityBalance, AuditorID,
                           SUM(CASE ReceiptType WHEN 1 then DiscountMoney else 0 end) AS DiscountMoney,SUM(LastRebate) AS LastRebate,SUM(CASE ReceiptType WHEN 1 then RealityBalance else 0 end ) 
AS PayRealityBalance FROM lmshop_CompanyFundReceipt WHERE AuditorID=@AuditorID ");
                sql.Append(sqlWhere);
                sql.Append(" GROUP BY CompanyID,AuditorID  ");
            }

            var sqlparams = new[]
            {
                new SqlParameter("@CompanyID",companyId),
                new SqlParameter("@IsOut",hasInvoice),
                new SqlParameter("@AuditorID",auditingId)
            };
            IList<CompanyFundReceiptInfo> receiptList = new List<CompanyFundReceiptInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), sqlparams))
            {
                while (dr.Read())
                {
                    var entity = new CompanyFundReceiptInfo
                    {
                        CompanyID = dr["CompanyID"] == DBNull.Value ? Guid.Empty : new Guid(dr["CompanyID"].ToString()),
                        RealityBalance = dr["RealityBalance"] == DBNull.Value ? 0 : decimal.Parse(dr["RealityBalance"].ToString()),
                        AuditorID = dr["AuditorID"] == DBNull.Value ? Guid.Empty : new Guid(dr["AuditorID"].ToString()),
                        DiscountMoney = dr["DiscountMoney"] == DBNull.Value ? 0 : decimal.Parse(dr["DiscountMoney"].ToString()),
                        LastRebate = dr["LastRebate"] == DBNull.Value ? 0 : decimal.Parse(dr["LastRebate"].ToString()),
                        PayRealityBalance = dr["PayRealityBalance"] == DBNull.Value ? 0 : decimal.Parse(dr["PayRealityBalance"].ToString()),
                    };
                    receiptList.Add(entity);
                }
            }
            return receiptList;
        }
        #endregion

        #region[根据往来单位获取付款单]

        /// <summary>
        /// 根据往来单位获取付款单
        /// Add by liucaijun at 2011-June-14th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="receiptType"></param>
        /// <returns></returns>
        public IList<CompanyFundReceiptInfo> GetFundReceiptListByCompanyID(Guid companyId, bool? receiptType)
        {
            var builder = new StringBuilder(SQL_SELECT_ALL_FIELDS);
            builder.AppendFormat(SQL_FROMTABLE);
            builder.AppendFormat(
                " WHERE CompanyID=@CompanyID AND ((ReceiptStatus>=4 AND ReceiptStatus<8)OR ReceiptStatus=9) ");
            if (receiptType != null)
            {
                builder.AppendFormat(" AND ReceiptType={0}", (bool)receiptType ? 1 : 0);
            }
            IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
            var parm = new SqlParameter("@CompanyID", SqlDbType.UniqueIdentifier) { Value = companyId };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), parm))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    list.Add(entity);
                }
            }
            return list;
        }
        #endregion

        #region[根据往来单位获取收款单]
        /// <summary>
        /// 根据往来单位获取收款单
        /// Add by liucaijun at 2011-August-24th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public IList<CompanyFundReceiptInfo> GetFundListByCompanyId(Guid companyId)
        {
            const string SQL = SQL_SELECT_ALL_FIELDS + SQL_FROMTABLE + " WHERE CompanyID=@CompanyID AND ReceiptStatus>=4 AND ReceiptStatus<8 AND ReceiptType=0";
            IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
            var parm = new SqlParameter("@CompanyID", SqlDbType.UniqueIdentifier) { Value = companyId };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    list.Add(entity);
                }
            }
            return list;
        }
        #endregion

        #region[更新执行、审核、完成时间]

        /// <summary>
        /// 更新执行、审核、完成时间
        /// Modify by liucaijun at 2011-03-15th
        /// </summary>
        /// <param name="receiptId">单据ID</param>
        /// <param name="type">更新哪个时间，1审核时间2执行时间3完成时间</param>
        public void SetDateTime(Guid receiptId, int type)
        {
            const string UPDATE_DATE = "UPDATE lmshop_CompanyFundReceipt SET ";
            var sql = new StringBuilder(UPDATE_DATE);
            if (type == 1)
            {
                sql.Append(" AuditingDate=getdate()");
            }
            if (type == 2)
            {
                sql.Append(" ExecuteDateTime=getdate()");
            }
            if (type == 3)
            {
                sql.Append(" FinishDate=getdate()");
            }
            sql.Append("  WHERE [ReceiptID]=@receiptID");
            var param = new SqlParameter("@receiptID", receiptId);
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql.ToString(), param);
        }
        #endregion

        #region[根据往来单位获取付款单]
        /// <summary>
        /// 根据往来单位获取付款单
        /// Add by liucaijun at 2011-June-14th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public IList<CompanyFundReceiptInfo> GetAllFundReceiptListByCompanyId(Guid companyId)
        {
            const string SQL = SQL_SELECT_ALL_FIELDS + SQL_FROMTABLE + " WHERE CompanyID=@CompanyID AND ReceiptType=1 AND ReceiptStatus!=8 ";
            IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
            var parm = new SqlParameter("@CompanyID", SqlDbType.UniqueIdentifier) { Value = companyId };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    list.Add(entity);
                }
            }
            return list;
        }
        #endregion

        #region[更新往来单位收付款手续费]
        /// <summary>
        /// 更新往来单位收付款手续费
        /// Add by liucaijun at 2012-02-08
        /// </summary>
        /// <param name="id"></param>
        /// <param name="poundage"></param>
        public void UpdatePoundage(Guid id, decimal poundage)
        {
            var sqlparams = new[]
            {
                new SqlParameter("@ReceiptID",id),
                new SqlParameter("@Poundage",poundage)
            };
            ExecuteNonQuery(SQL_UPDATE_POUNDAGE, sqlparams);
        }
        #endregion

        #region [根据公司ID获取当前月付款单据金额]
        /// <summary>根据公司ID获取当前月付款单据金额
        /// </summary>
        /// <returns></returns>
        public IList<CompanyFundReceiptInfo> GetFundReceiptListByFilialeIdAndCurrentMonth(Guid filialeId)
        {
            const string SQL = "SELECT ReceiptID, ReceiptNo, ReceiptType, ApplicantID, ApplyDateTime, PurchaseOrderNo, CompanyID, HasInvoice, SettleStartDate, SettleEndDate, ExpectBalance, RealityBalance, DiscountMoney, DiscountCaption, OtherDiscountCaption, ReceiptStatus, Remark, AuditorID, AuditFailureReason, InvoicesDemander, StockOrderNos, ExecuteDateTime, Poundage, AuditingDate, FinishDate, FilialeId, DealFlowNo, PayBankAccountsId, IsOut, IncludeStockNos, DebarStockNos, LastRebate,InvoiceType  FROM dbo.lmshop_CompanyFundReceipt WHERE MONTH(ApplyDateTime)=MONTH(GETDATE()) AND YEAR(ApplyDateTime)=YEAR(GETDATE()) AND receiptType=1 AND ReceiptStatus IN(1,2,3,4,5,6,7,9) AND FilialeId=@FilialeId";
            IList<CompanyFundReceiptInfo> list = new List<CompanyFundReceiptInfo>();
            var parm = new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier) { Value = filialeId };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    CompanyFundReceiptInfo entity = GetEntity(dr);
                    list.Add(entity);
                }
            }
            return list;
        }
        #endregion

        #endregion

        #region -- 获取实体类
        private static CompanyFundReceiptInfo GetEntity(IDataReader dr)
        {
            var entity = new CompanyFundReceiptInfo
            {
                ReceiptID = dr[0] != null ? dr.GetGuid(0) : Guid.Empty,
                ReceiptNo = dr[1] != null ? dr.GetString(1) : string.Empty,
                ReceiptType = dr[2] != null ? dr.GetInt32(2) : 0,
                ApplicantID = dr[3] != null ? dr.GetGuid(3) : Guid.Empty,
                ApplyDateTime = dr[4] != null ? dr.GetDateTime(4) : DateTime.MinValue,
                PurchaseOrderNo = dr[5] != null ? dr.GetString(5) : string.Empty,
                CompanyID = dr[6] != null ? dr.GetGuid(6) : Guid.Empty,
                HasInvoice = dr[7] != null && dr.GetBoolean(7),
                SettleStartDate = dr[8] != null ? dr.GetDateTime(8) : DateTime.MinValue,
                SettleEndDate = dr[9] != null ? dr.GetDateTime(9) : DateTime.MinValue,
                ExpectBalance = dr[10] != null ? dr.GetDecimal(10) : 0,
                RealityBalance = dr[11] != null ? dr.GetDecimal(11) : 0,
                DiscountMoney = dr[12] != null ? dr.GetDecimal(12) : 0,
                DiscountCaption =
                                     dr[13] != null && dr[13] != DBNull.Value ? dr.GetString(13) : string.Empty,
                OtherDiscountCaption =
                                     dr[14] != null && dr[14] != DBNull.Value ? dr.GetString(14) : string.Empty,
                ReceiptStatus = dr[15] != null && dr[15] != DBNull.Value ? dr.GetInt32(15) : 0,
                Remark = dr[16] != null && dr[16] != DBNull.Value ? dr.GetString(16) : string.Empty,
                AuditorID = dr[17] != null && dr[17] != DBNull.Value ? dr.GetGuid(17) : Guid.Empty,
                AuditFailureReason =
                                     dr[18] != null && dr[18] != DBNull.Value ? dr.GetString(18) : string.Empty,
                InvoicesDemander =
                                     dr[19] != null && dr[19] != DBNull.Value ? dr.GetGuid(19) : Guid.Empty,
                StockOrderNos =
                                     dr[20] != null && dr[20] != DBNull.Value ? dr.GetString(20) : string.Empty,
                ExecuteDateTime = dr[21] != DBNull.Value ? dr.GetDateTime(21) : DateTime.MinValue,
                Poundage = dr[22] != DBNull.Value ? dr.GetDecimal(22) : 0,
                AuditingDate = dr[23] != DBNull.Value ? dr.GetDateTime(23) : DateTime.MinValue,
                FinishDate = dr[24] != DBNull.Value ? dr.GetDateTime(24) : DateTime.MinValue,
                FilialeId = dr[25] != DBNull.Value ? dr.GetGuid(25) : Guid.Empty,
                DealFlowNo = dr[26] != DBNull.Value ? dr.GetString(26) : string.Empty,
                IsOut = Convert.ToBoolean(dr["IsOut"]),
                PayBankAccountsId = dr["PayBankAccountsId"] != DBNull.Value ? new Guid(dr["PayBankAccountsId"].ToString()) : Guid.Empty,
                IncludeStockNos = dr["IncludeStockNos"] != DBNull.Value ? dr["IncludeStockNos"].ToString() : string.Empty,
                DebarStockNos = dr["DebarStockNos"] != DBNull.Value ? dr["DebarStockNos"].ToString() : string.Empty,
                LastRebate = dr["LastRebate"] != DBNull.Value ? Convert.ToDecimal(dr["LastRebate"]) : 0,
                PaymentDate = dr["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dr["PaymentDate"]) : DateTime.MinValue,
                ReturnOrder = dr["ReturnOrder"] != DBNull.Value ? dr["ReturnOrder"].ToString() : string.Empty,
                PayOrder = dr["PayOrder"] != DBNull.Value ? dr["PayOrder"].ToString() : string.Empty,
                ReceiveBankAccountId = dr["ReceiveBankAccountId"] != DBNull.Value ? new Guid(dr["ReceiveBankAccountId"].ToString()) : Guid.Empty,
                InvoiceType = dr["InvoiceType"] != DBNull.Value ? Convert.ToInt32(dr["InvoiceType"]) : 0,
                IsUrgent = dr["IsUrgent"] != DBNull.Value && Convert.ToBoolean(dr["IsUrgent"]),
                UrgentRemark = dr["UrgentRemark"] != DBNull.Value ? dr["UrgentRemark"].ToString() : string.Empty,
                InvoiceState = dr["InvoiceState"] != DBNull.Value ? Convert.ToInt32(dr["InvoiceState"]) : 0,
                InvoiceUnit = dr["InvoiceUnit"] != DBNull.Value ? dr["InvoiceUnit"].ToString() : string.Empty
            };
            return entity;
        }
        #endregion

        private static string GetReceiptStateByPage(ReceiptPage page, CompanyFundReceiptState state)
        {
            string sql = string.Empty;
            switch (page)
            {
                //收付款单据
                case ReceiptPage.CompanyFundReceipt:
                    switch ((int)state)
                    {
                        case -1:
                            sql = (" AND [ReceiptStatus]!=-1");
                            break;
                        case -2:
                            //未操作：待审核1、发票待索取5、打款退回10
                            sql = (" AND [ReceiptStatus] IN (1,5,10)");
                            break;
                        case -3:
                            //已操作：未通过0、已审核3、已执行4、发票已索取6、发票核销7、作废8、完成打款9、审核未通过13
                            sql = " AND [ReceiptStatus] IN (0,3,4,6,7,8,9,13)";
                            break;
                        default:
                            sql = " AND [ReceiptStatus]=" + (int)state;
                            break;
                    }
                    break;
                //开具发票
                case ReceiptPage.DoFoudReceive:
                    switch ((int)state)
                    {
                        case -1:
                            sql = (" AND [ReceiptStatus]!=-1");
                            break;
                        case -2:
                            sql = (" AND [ReceiptStatus]=2");
                            break;
                        case -3:
                            sql = " AND [ReceiptStatus]=4";
                            break;
                        default:
                            sql = " AND [ReceiptStatus]=" + (int)state;
                            break;
                    }
                    break;
                //付款审核
                case ReceiptPage.PayCheckList:
                    switch ((int)state)
                    {
                        case -1:
                            sql = (" AND [ReceiptStatus]!=-1");
                            break;
                        case -2:
                            sql = (" AND [ReceiptStatus] IN (0,1,10)");
                            break;
                        case -3:
                            sql = " AND [ReceiptStatus] IN (3,4,6,7,9)";
                            break;
                        default:
                            sql = " AND [ReceiptStatus]=" + (int)state;
                            break;
                    }
                    break;
                //申请打款
                case ReceiptPage.DoReceivePay:
                    switch ((int)state)
                    {
                        case -1:
                            sql = (" AND [ReceiptStatus]!=-1");
                            break;
                        case -2:
                            sql = (" AND [ReceiptStatus] =3");
                            break;
                        case -3:
                            sql = " AND [ReceiptStatus] IN (4,6,7,9)";
                            break;
                        default:
                            sql = " AND [ReceiptStatus]=" + (int)state;
                            break;
                    }
                    break;
                //完成打款
                case ReceiptPage.CompanyFundPayReceiptFinish:
                    switch ((int)state)
                    {
                        case -1:
                            sql = (" AND [ReceiptStatus]!=-1");
                            break;
                        case -2:
                            sql = (" AND [ReceiptStatus]=4");
                            break;
                        case -3:
                            sql = " AND [ReceiptStatus] IN (6,7,9)";
                            break;
                        default:
                            sql = " AND [ReceiptStatus]=" + (int)state;
                            break;
                    }
                    break;
                //索取发票
                case ReceiptPage.DemandReceipt:
                    switch ((int)state)
                    {
                        case -1:
                            sql = (" AND [ReceiptStatus]!=-1");
                            break;
                        case -2:
                            sql = (" AND [ReceiptStatus] IN(5,11)");
                            break;
                        case -3:
                            sql = " AND [ReceiptStatus] IN (6,7,12)";
                            break;
                        default:
                            sql = " AND [ReceiptStatus]=" + (int)state;
                            break;
                    }
                    break;
                case ReceiptPage.Else:
                    switch ((int)state)
                    {
                        case -2:
                            sql = (" AND [ReceiptStatus]=12");
                            break;
                        case -3:
                            sql = (" AND [ReceiptStatus]=7");
                            break;
                    }
                    break;
            }
            return sql;
        }

        /// <summary>
        /// 通过出入库单据号获取往来单位收付款单据状态
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="linkTradeCode"></param>
        /// <returns></returns>
        public int GetFundReceiptStatusByLinkTradeCode(Guid companyId, string linkTradeCode)
        {
            const string SQL = @" SELECT TOP 1 ReceiptStatus FROM [lmshop_CompanyFundReceipt] WHERE CompanyID=@CompanyId AND  (StockOrderNos LIKE '%{0}%' OR IncludeStockNos LIKE '%{0}%')";
            var parm = new[]
            {
                new SqlParameter("@CompanyId",companyId)
            };
            var result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, String.Format(SQL, linkTradeCode), parm);
            return result == null || result == DBNull.Value ? -1 : (int)result;
        }

        /// <summary>
        /// 根据往来帐收付款单据查找单据信息
        /// </summary>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        public CompanyFundReceiptInfo GetFundReceiptInfoByReceiptNo(string receiptNo)
        {
            const string SQL = @" SELECT TOP 1 ReceiptID, ReceiptNo,ReceiptType,ApplicantID,ApplyDateTime, PurchaseOrderNo,CompanyID,HasInvoice,
                                   SettleStartDate,SettleEndDate,ExpectBalance,RealityBalance,DiscountMoney,DiscountCaption,
                                   OtherDiscountCaption, ReceiptStatus,Remark,AuditorID,AuditFailureReason,InvoicesDemander,
                                   StockOrderNos,ExecuteDateTime,Poundage,AuditingDate,FinishDate,FilialeId,DealFlowNo,IsOut,PayBankAccountsId
                                    ,IncludeStockNos,DebarStockNos,LastRebate,PaymentDate,ReturnOrder,PayOrder,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState FROM [lmshop_CompanyFundReceipt]
                                     WHERE  ReceiptNo=@ReceiptNo";
            var parm = new SqlParameter("@ReceiptNo", receiptNo);
            var companyFundReceiptInfo = new CompanyFundReceiptInfo();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (dr.Read())
                {
                    companyFundReceiptInfo = GetEntity(dr);
                }
            }
            return companyFundReceiptInfo;
        }

        /// <summary>
        /// 更新往来帐收付款的交易流水号
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="dealFlowNo"></param>
        public void UpdateDealFlowNo(Guid receiptId, string dealFlowNo)
        {
            const string SQL = "update lmshop_CompanyFundReceipt set DealFlowNo=@DealFlowNo where ReceiptID=@ReceiptID";
            var param = new[]
                            {
                                new SqlParameter("@DealFlowNo", dealFlowNo),
                                new SqlParameter("@receiptID", receiptId)
                            };
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, param);
        }

        /// <summary>
        /// 根据往来帐收付款单据查找单据信息
        /// </summary>
        /// <returns></returns>
        public IList<CompanyFundReceiptInfo> GetFundReceiptList(DateTime? startTime, DateTime? endTime, Guid? companyId, Guid filialeId, int? receiptType, string searchKey)
        {
            var builder = new StringBuilder(@" SELECT ReceiptID, ReceiptNo,ReceiptType,ApplicantID,ApplyDateTime, PurchaseOrderNo,CompanyID,HasInvoice,
                                   SettleStartDate,SettleEndDate,ExpectBalance,RealityBalance,DiscountMoney,DiscountCaption,
                                   OtherDiscountCaption, ReceiptStatus,Remark,AuditorID,AuditFailureReason,InvoicesDemander,
                                   StockOrderNos,ExecuteDateTime,Poundage,AuditingDate,FinishDate,FilialeId,DealFlowNo,IsOut,
                                    PayBankAccountsId,IncludeStockNos,DebarStockNos,LastRebate,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState FROM [lmshop_CompanyFundReceipt]
                                     WHERE FilialeId=@FilialeId ");
            var parm = new List<SqlParameter>
                           {
                               new SqlParameter("@FilialeId", filialeId)
                           };
            if (startTime != null && endTime != null)
            {
                parm.Add(new SqlParameter("@StartTime", startTime));
                parm.Add(new SqlParameter("@EndTime", endTime));
                builder.Append(" AND ApplyDateTime BETWEEN @StartTime AND @EndTime ");
            }
            if (companyId != null)
            {
                parm.Add(new SqlParameter("@CompanyID", companyId));
                builder.Append(" AND CompanyID = @CompanyID ");
            }
            if (receiptType != null)
            {
                parm.Add(new SqlParameter("@ReceiptType", receiptType));
                builder.Append(" AND ReceiptType = @ReceiptType ");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                builder.AppendFormat(" AND ReceiptNo like '%{0}%' ", searchKey);
            }
            var companyFundReceiptList = new List<CompanyFundReceiptInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), parm.ToArray()))
            {
                while (dr.Read())
                {
                    companyFundReceiptList.Add(GetEntity(dr));
                }
            }
            return companyFundReceiptList;
        }


        /// <summary>
        /// 更新往来帐收付款状态和添加备注
        /// </summary>
        /// <param name="receiptId"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateCompanyFundReceiptStatus(Guid receiptId, int status, string remark)
        {
            const string SQL = @"update lmshop_CompanyFundReceipt set ReceiptStatus=@ReceiptStatus,Remark=Remark+@Remark where ReceiptID=@ReceiptID";
            var parameters = new[]
                                        {
                                            new Parameter("@ReceiptStatus",status),
                                            new Parameter("@Remark",remark),
                                            new Parameter("@ReceiptID",receiptId)
                                        };
            using (var db = new Database(GlobalConfig.ERP_DB_NAME))
            {
                return db.Execute(false, SQL, parameters);
            }
        }

        /// <summary>获取往来单位付款单据最近一条付款单据（填写付款单按日期付款使用）
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID </param>
        /// <returns></returns>
        public CompanyFundReceiptInfo GetFundReceiptInfoByLately(Guid companyId, Guid filialeId)
        {
            const string SQL = @"
 SELECT TOP 1
                                                        ReceiptID,
                                                        ReceiptNo,
                                                        ReceiptType,
                                                        ApplicantID,
                                                        ApplyDateTime,
                                                        PurchaseOrderNo,
                                                        CompanyID,
                                                        HasInvoice,
                                                        SettleStartDate,
                                                        SettleEndDate,
                                                        ExpectBalance,
                                                        RealityBalance,
                                                        DiscountMoney,
                                                        DiscountCaption,
                                                        OtherDiscountCaption,
                                                        ReceiptStatus,
                                                        Remark,
                                                        AuditorID,
                                                        AuditFailureReason,
                                                        InvoicesDemander,
                                                        StockOrderNos,
                                                        ExecuteDateTime,Poundage,
                                                        AuditingDate,FinishDate,FilialeId,DealFlowNo,IsOut,PayBankAccountsId,IncludeStockNos,DebarStockNos,
                                                        LastRebate,PaymentDate,ReturnOrder,PayOrder,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState  
                                                        FROM [lmshop_CompanyFundReceipt] 
																WHERE CompanyID=@CompanyID
																AND ReceiptType=1
																AND ReceiptStatus!=8
																AND FilialeId=@FilialeId
                                                                AND SettleEndDate!='1999-09-09 00:00:00.000'
                                                        ORDER BY SettleEndDate DESC";


            var parms = new[]{
                new SqlParameter("@CompanyID",SqlDbType.UniqueIdentifier){Value = companyId},
                new SqlParameter("@FilialeId",SqlDbType.UniqueIdentifier){Value =filialeId }
            };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                if (dr.Read())
                {
                    return GetEntity(dr);
                }
            }
            return null;
        }

        /// <summary>设置往来单位单位收付款银行帐号  ADD  陈重文  2015-03-02
        /// </summary>
        /// <param name="receiptId">往来单位收付款ID</param>
        /// <param name="bankAccountsId">银行账号ID</param>
        public Boolean SetPayBankAccountsId(Guid receiptId, Guid bankAccountsId)
        {
            const string SQL = "UPDATE lmshop_CompanyFundReceipt SET PayBankAccountsId=@PayBankAccountsId WHERE ReceiptID=@receiptID";
            var param = new[]
                            {
                                new SqlParameter("@PayBankAccountsId", bankAccountsId),
                                new SqlParameter("@ReceiptID", receiptId)
                            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, param) > 0;
        }

        /// <summary>设置往来单位单位收付款公司  ADD  陈重文  2015-03-30
        /// </summary>
        /// <param name="receiptId">往来单位收付款ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="isOut"></param>
        public Boolean UpdateFilialeId(Guid receiptId, Guid filialeId, Boolean isOut)
        {
            const string SQL = "UPDATE lmshop_CompanyFundReceipt SET FilialeId=@FilialeId,IsOut=@IsOut,HasInvoice=@IsOut WHERE ReceiptID=@ReceiptID";
            var param = new[]
                            {
                                new SqlParameter("@FilialeId", filialeId),
                                new SqlParameter("@ReceiptID", receiptId),
                                new SqlParameter("@IsOut", isOut)
                            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, param) > 0;
        }

        /// <summary>
        /// 判断入库单是否已经生成收付款单据 排除已作废的
        /// </summary>
        /// <param name="stockNo"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        public bool IsExistsByStockOrderNos(string stockNo, string receiptNo)
        {
            const string SQL = @"SELECT TOP 1 ReceiptNo FROM [lmshop_CompanyFundReceipt] where ReceiptStatus!=8 AND {0}
 (IncludeStockNos like '%{1}%' OR StockOrderNos like '%{1}%')";
            var result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true,
                string.Format(SQL, string.IsNullOrEmpty(receiptNo) ? string.Empty : string.Format(" ReceiptNo!='{0}' AND ", receiptNo), stockNo));
            return result != DBNull.Value && result != null && result.ToString().Length > 0;
        }

        #region 根据“单据编号”获取数据
        /// <summary>
        /// 根据“单据编号”获取数据
        /// </summary>
        /// <param name="receiptNo">单据编号(多个单据编号，用英文状态下的逗号隔开)</param>
        /// <returns></returns>
        /// zal 2016-01-21
        public IList<CompanyFundReceiptInfo> GetCompanyFundReceiptList(string receiptNo)
        {
            IList<CompanyFundReceiptInfo> companyFundReceiptInfoList = new List<CompanyFundReceiptInfo>();
            if (string.IsNullOrEmpty(receiptNo))
            {
                return companyFundReceiptInfoList;
            }
            else
            {
                receiptNo = "'" + receiptNo.Replace(",", "','") + "'";

                var sql = string.Format(@"
                        select ReceiptID, ReceiptNo,ReceiptType,ApplicantID,ApplyDateTime, PurchaseOrderNo,CompanyID,HasInvoice,
                        SettleStartDate,SettleEndDate,ExpectBalance,RealityBalance,DiscountMoney,DiscountCaption,
                        OtherDiscountCaption, ReceiptStatus,Remark,AuditorID,AuditFailureReason,InvoicesDemander,
                        StockOrderNos,ExecuteDateTime,Poundage,AuditingDate,FinishDate,FilialeId,DealFlowNo,IsOut,PayBankAccountsId
                        ,IncludeStockNos,DebarStockNos,LastRebate,PaymentDate,ReturnOrder,PayOrder,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState FROM lmshop_CompanyFundReceipt 
                        where ReceiptNo in({0}) ", receiptNo);

                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
                {
                    while (rdr.Read())
                    {
                        companyFundReceiptInfoList.Add(GetEntity(rdr));
                    }
                }
                return companyFundReceiptInfoList;
            }
        }
        #endregion

        #region 获取所有单据数据
        /// <summary>
        /// 获取所有单据数据
        /// </summary>
        /// <returns></returns>
        /// zal 2016-02-17
        public IList<CompanyFundReceiptInfo> GetAllCompanyFundReceiptList()
        {
            IList<CompanyFundReceiptInfo> companyFundReceiptInfoList = new List<CompanyFundReceiptInfo>();
            var sql = @"
                        select ReceiptID, ReceiptNo,ReceiptType,ApplicantID,ApplyDateTime, PurchaseOrderNo,CompanyID,HasInvoice,
                        SettleStartDate,SettleEndDate,ExpectBalance,RealityBalance,DiscountMoney,DiscountCaption,
                        OtherDiscountCaption, ReceiptStatus,Remark,AuditorID,AuditFailureReason,InvoicesDemander,
                        StockOrderNos,ExecuteDateTime,Poundage,AuditingDate,FinishDate,FilialeId,DealFlowNo,IsOut,PayBankAccountsId
                        ,IncludeStockNos,DebarStockNos,LastRebate,PaymentDate,ReturnOrder,PayOrder,ReceiveBankAccountId,InvoiceType,IsUrgent,UrgentRemark,InvoiceUnit,InvoiceState FROM lmshop_CompanyFundReceipt";

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    companyFundReceiptInfoList.Add(GetEntity(rdr));
                }
            }
            return companyFundReceiptInfoList;
        }
        #endregion

        #region 验证“退货单”或者“付款单”是否被占用
        /// <summary>
        /// 验证“退货单”或者“付款单”是否被占用
        /// </summary>
        /// <param name="receiptId">往来单位收付款主键id</param>
        /// <param name="returnOrder">退货单</param>
        /// <param name="payOrder">付款单</param>
        /// <returns></returns>
        public bool CheckReturnOrderOrPayOrder(Guid receiptId, string returnOrder, string payOrder)
        {
            var sql = @"select count(*) from lmshop_CompanyFundReceipt where 1=1 and ReceiptStatus!=8 ";

            if (!receiptId.Equals(Guid.Empty))
            {
                sql += " and ReceiptID !='" + receiptId + "'";
            }
            if (!string.IsNullOrEmpty(returnOrder))
            {
                sql += " and ReturnOrder ='" + returnOrder + "'";
            }
            if (!string.IsNullOrEmpty(payOrder))
            {
                sql += " and PayOrder ='" + payOrder + "'";
            }
            var result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sql, null);
            return int.Parse(result.ToString()) > 0;
        }


        #endregion

        /// <summary>
        /// 按日期付款时，验证是否存在重复付款单据
        /// </summary>
        /// <param name="companyId">往来单位</param>
        /// <param name="filialeId">付款公司</param>
        /// <param name="settleStartDate">结账开始日期</param>
        /// <param name="settleEndDate">结账截止日期</param>
        /// <returns>按日期付款时(1:所填写的结账日期范围内的入库单的审核时间不能包含在已经按日期付款的结账日期范围内;2:所填写的结账日期范围内的入库单不能是按日期付款中已经包含的单据;3:所填写的结账日期范围内的入库单不能是已经按入库单付过款的单据;4:所填写的结账日期范围内的入库单可以是按日期付款中已经排除的单据;5:所填写的入库单不能是按入库单付款中已经退货的单据;)</returns>
        /// zal 2016-10-09
        public List<string> CheckExistForDate(Guid companyId, Guid filialeId, DateTime settleStartDate, DateTime settleEndDate)
        {
            string sql = @"
            SELECT TradeCode FROM (
	            SELECT AuditTime,TradeCode FROM StorageRecord WITH(NOLOCK)
	            WHERE 
	            ThirdCompanyID=@CompanyID 
	             AND FilialeId='" + filialeId + "'" + @"
	            AND StockState!=" + (int)StorageRecordState.Canceled + " AND StockState=" + (int)StorageRecordState.Finished + @"
	            AND @SettleStartDate<=AuditTime AND AuditTime<=@SettleEndDate
            ) A 
            INNER JOIN 
            (
	            SELECT ReceiptNo,SettleStartDate,SettleEndDate,IncludeStockNos,DebarStockNos,StockOrderNos,ReturnOrder FROM lmshop_CompanyFundReceipt WITH(NOLOCK)
	            WHERE 
	            CompanyID=@CompanyID AND FilialeId='" + filialeId + "'" + @"
	            AND ReceiptType=1
	            AND ReceiptStatus!=8 AND ReceiptStatus!=9
	            AND ((SettleStartDate!='1999-09-09 00:00:00.000' AND SettleEndDate!='1999-09-09 00:00:00.000') OR LEN(LTRIM(RTRIM(StockOrderNos)))>0)
            ) B
            ON ((B.SettleStartDate<=A.AuditTime AND A.AuditTime<=B.SettleEndDate) OR CHARINDEX(A.TradeCode,B.IncludeStockNos)>0 OR CHARINDEX(A.TradeCode,B.StockOrderNos)>0 OR CHARINDEX(A.TradeCode,B.ReturnOrder)>0)
            AND CHARINDEX(A.TradeCode,B.DebarStockNos)=0";

            var paras = new[]
            {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SettleStartDate", settleStartDate),
                new SqlParameter("@SettleEndDate", settleEndDate)
            };

            List<string> tradeCodeList = new List<string>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras))
            {
                while (rdr.Read())
                {
                    tradeCodeList.Add(rdr["TradeCode"].ToString());
                }
            }
            return tradeCodeList;
        }

        /// <summary>
        /// 按入库单付款时，验证是否存在重复付款单据
        /// </summary>
        /// <param name="companyId">往来单位</param>
        /// <param name="filialeId">付款公司</param>
        /// <param name="tradeCode">入库单号</param>
        /// <param name="isOut">是否ERP(0:否;1:是;)</param>
        /// <returns>按入库单付款时(1:所填写的入库单的审核时间不能包含在已经按日期付款的结账日期范围内;2:所填写的入库单不能是按日期付款中已经包含的单据;3:所填写的入库单不能是已经按入库单付过款的单据;4:所填写的入库单可以是按日期付款中已经排除的单据;5:所填写的入库单不能是按入库单付款中已经退货的单据;)</returns>
        /// zal 2016-10-09
        public List<string> CheckExistForStorageNo(Guid companyId, Guid filialeId, string tradeCode)
        {
            var tradeCodes = tradeCode.Split(',');
            var conditionStr = String.Empty;
            foreach (var condition in tradeCodes.Where(condition => !String.IsNullOrWhiteSpace(condition)))
            {
                if (String.IsNullOrEmpty(conditionStr))
                    conditionStr += "'" + condition + "'";
                else
                    conditionStr += ",'" + condition + "'";
            }
            var sql = String.Format(@"
            SELECT TradeCode,ReceiptNo FROM (
	            SELECT AuditTime,TradeCode FROM StorageRecord WITH(NOLOCK)
	            WHERE 
	            ThirdCompanyID=@CompanyID {0}
	            AND StockState!=" + (int)StorageRecordState.Canceled + " AND StockState=" + (int)StorageRecordState.Finished + @"
			    AND TradeCode IN({1})
            ) A 
            INNER JOIN 
            (
	            SELECT ReceiptNo,SettleStartDate,SettleEndDate,IncludeStockNos,DebarStockNos,StockOrderNos,ReturnOrder FROM lmshop_CompanyFundReceipt WITH(NOLOCK)
	            WHERE 
	            CompanyID=@CompanyID  {0}
	            AND ReceiptType=1
	            AND ReceiptStatus!=8 AND ReceiptStatus!=9
	            AND ((SettleStartDate!='1999-09-09 00:00:00.000' AND SettleEndDate!='1999-09-09 00:00:00.000') OR LEN(LTRIM(RTRIM(StockOrderNos)))>0)
            ) B
            ON ((B.SettleStartDate<=A.AuditTime AND A.AuditTime<=B.SettleEndDate) OR CHARINDEX(A.TradeCode,B.IncludeStockNos)>0 OR CHARINDEX(A.TradeCode,B.StockOrderNos)>0 OR CHARINDEX(A.TradeCode,B.ReturnOrder)>0)
            AND CHARINDEX(A.TradeCode,B.DebarStockNos)=0", " AND FilialeId='" + filialeId + "'", conditionStr);

            var paras = new[]
            {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@TradeCode", tradeCode)
            };

            List<string> tradeCodeList = new List<string>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras))
            {
                while (rdr.Read())
                {
                    tradeCodeList.Add(rdr["TradeCode"] + "【" + rdr["ReceiptNo"] + "】");
                }
            }
            return tradeCodeList;
        }

        public bool SetReceiveBankAccountIdByReceiptId(Guid receiptId, Guid bankAccountId)
        {
            const string SQL = "UPDATE lmshop_CompanyFundReceipt SET ReceiveBankAccountId=@ReceiveBankAccountId WHERE ReceiptID=@ReceiptID";

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, new[]
            {
                new SqlParameter("@ReceiptID",receiptId),
                 new SqlParameter("@ReceiveBankAccountId",bankAccountId)
            }) > 0;
        }
    }
}
