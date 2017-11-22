using ERP.DAL.Finance.Interface;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.Finance.Gathering;
using Keede.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Finance
{
    /// <summary>
    /// 财务API：收款单的DAL
    /// </summary>
    public class GatheringDAL : IGatheringDAL
    {
        /// <summary>
        /// 财务API：收款单的DAL
        /// </summary>
        /// <param name="fromType"></param>
        public GatheringDAL(GlobalConfig.DB.FromType fromType) { }

        #region 收款单

        /// <summary>
        /// 获取收款单：往来单位收付款（收付款类型=收款单、劳务类型）
        /// 条件：类型=收款、劳务 && 单据状态=完成打款
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<CompanyFundReceiptDTO> GetGathering_CompanyFundReceipt(DateTime StartTime, DateTime EndTime)
        {
            IList<CompanyFundReceiptDTO> list = new List<CompanyFundReceiptDTO>();

            string sql = string.Format(@" SELECT ReceiptID,ReceiptNo,FilialeId,BillingDate,ReceiveBankAccountId,RealityBalance,ApplicantID
                            FROM [lmshop_CompanyFundReceipt]
                            WHERE ApplyDateTime >= @start AND ApplyDateTime < @end 
                                AND [ReceiptType]={0} 
                                AND ReceiptStatus={1}",
                            (int)CompanyFundReceiptType.Receive, CompanyFundReceiptState.Finish);
            var listparam = InitSqlParameter(StartTime, EndTime);

            sql += " ORDER BY ApplyDateTime DESC";

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, listparam.ToArray()))
            {
                while (dr.Read())
                {
                    var entity = new CompanyFundReceiptDTO
                    {
                        ReceiptID = dr["ReceiptID"].ObjToGuid(),
                        ReceiptNo = dr["ReceiptNo"].ToString(),
                        FilialeId = dr["FilialeId"].ObjToGuid(),
                        BillingDate = dr["BillingDate"].ObjToDatetime(),
                        ReceiveBankAccountId = dr["ReceiveBankAccountId"].ObjToGuid(),
                        RealityBalance = dr["RealityBalance"].ObjToDecimal(),
                        ApplicantID = dr["licantID"].ObjToGuid(),
                        Type = true,
                    };
                    list.Add(entity);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取收款单：费用申报（申报类型=费用收入）
        /// 条件：申报类型=费用收入 && 审批状态=完成
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public IList<CostReportDTO> GetGathering_CostReport(DateTime StartTime, DateTime EndTime)
        {
            IList<CostReportDTO> list = new List<CostReportDTO>();
            string sql = string.Format(@" SELECT ReportId,ReportNo,CompanyId,ReportDate,CostType,BankAccountName,RealityCost,ReportPersonnelId
                            FROM [lmShop_CostReport]
                            WHERE FinishDate >= @start AND FinishDate < @end
                                AND [ReportKind]={0}
                                AND [State] = {1}",
                                (int)CostReportKind.FeeIncome, (int)CostReportState.Complete);
            List<SqlParameter> listparam = InitSqlParameter(StartTime, EndTime);

            sql += " ORDER BY FinishDate DESC";

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, listparam.ToArray()))
            {
                while (dr.Read())
                {
                    var entity = new CostReportDTO
                    {
                        ReportId = dr["ReportId"].ObjToGuid(),
                        ReportNo = dr["ReportNo"].ToString(),
                        CompanyId = dr["CompanyId"].ObjToGuid(),
                        ReportDate = dr["ReportDate"].ObjToDatetime(),
                        CostType = dr["ReportDate"].ObjToInt(),
                        BankAccountName = dr["BankAccountName"].ToString(),
                        RealityCost = dr["RealityCost"].ObjToDecimal(),
                        ReportPersonnelId = dr["ReportPersonnelId"].ObjToGuid(),
                    };
                    list.Add(entity);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取收款单：资金流（转账）。百秀兰溪》上海百秀旗舰店转账。
        /// 条件：记账本类型=收入 && 资金流单据状态=正常 操作状态 = 已处理
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="LogisticsCompany">物流公司</param>
        /// <param name="SalesCompany">销售公司</param>
        /// <returns></returns>
        public IList<WasteBookDTO> GetGathering_WasteBook(DateTime StartTime, DateTime EndTime, string LogisticsCompany, string SalesCompany)
        {
            //转出公司（物流公司）：百秀兰溪 75621b55-2fa3-4fcf-b68a-039c28f560b6
            //转入公司（销售公司）：上海百秀 444e0c93-1146-4386-bae2-cb352da70499
            IList<WasteBookDTO> list = new List<WasteBookDTO>();
            string sql = string.Format(@" SELECT TradeCode,SaleFilialeId,DateCreated,BankAccountsId,Income
                            FROM [lmShop_WasteBook]
                            WHERE DateCreated >= @start AND DateCreated < @end
                                AND [WasteBookType]={0}
                                AND [State] ={1}
                                AND [OperateState] ={3}
                                AND [AuditingState] ={4}
                                AND [BankAccountsId] in(SELECT A.[BankAccountsId] AS BankAccountsId 
							FROM BankAccountBinding AS A WITH(NOLOCK)
							INNER JOIN lmShop_BankAccounts AS B WITH(NOLOCK) ON A.BankAccountsId=B.BankAccountsId AND B.IsUse=1
							WHERE TargetId=@TargetId)
                                AND SaleFilialeId =@SaleFilialeId)",
                                (int)WasteBookType.Increase, (int)WasteBookState.Currently, 1, AuditingState.Yes);

            List<SqlParameter> listparam = InitSqlParameter(StartTime, EndTime);
            listparam.Add(new SqlParameter("@TargetId", LogisticsCompany));
            listparam.Add(new SqlParameter("@SaleFilialeId", SalesCompany));

            sql += " ORDER BY DateCreated DESC";

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, listparam.ToArray()))
            {
                while (dr.Read())
                {
                    var entity = new WasteBookDTO
                    {
                        TradeCode = dr["TradeCode"].ToString(),
                        SaleFilialeId = dr["SaleFilialeId"].ObjToGuid(),
                        DateCreated = dr["DateCreated"].ObjToDatetime(),
                        BankAccountsId = dr["BankAccountsId"].ObjToGuid(),
                        Income = dr["Income"].ObjToDecimal(),
                        Type = true,
                    };
                    list.Add(entity);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取收款单：客户付款-款到发货（排除 补偿、赠送 ）、余额支付（排除 补偿、赠送 ）
        /// 条件：订单状态=完成发货(完成订单后) && 支付状态=已支付 && 支付方式=款到发货（余额支付待定）
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public IList<GoodsOrderDTO> GetGathering_GoodsOrder(DateTime StartTime, DateTime EndTime)
        {
            IList<GoodsOrderDTO> list = new List<GoodsOrderDTO>();
            string sql = string.Format(@" SELECT OrderId,OrderNo,SaleFilialeId,OrderTime,BankAccountsId,TotalPrice,MemberId,SalePlatformId
                            FROM [lmshop_GoodsOrder]
                            WHERE OrderTime >= @start AND OrderTime < @end
                                AND [OrderState]={0}
                                AND [PayState] = {1}
                                AND [PayMode] = {2}",
                                (int)OrderState.Consignmented, (int)PayState.Paid, PayMode.COG);
            List<SqlParameter> listparam = InitSqlParameter(StartTime, EndTime);

            sql += " ORDER BY OrderTime DESC";

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, listparam.ToArray()))
            {
                while (dr.Read())
                {
                    var entity = new GoodsOrderDTO
                    {
                        OrderId = dr["OrderId"].ObjToGuid(),
                        OrderNo = dr["OrderNo"].ToString(),
                        SaleFilialeId = dr["SaleFilialeId"].ObjToGuid(),
                        OrderTime = dr["OrderTime"].ObjToDatetime(),
                        BankAccountsId = dr["BankAccountsId"].ObjToGuid(),
                        TotalPrice = dr["TotalPrice"].ObjToDecimal(),
                        MemberId = dr["MemberId"].ObjToGuid(),
                        SalePlatformId = dr["SalePlatformId"].ObjToGuid(),
                        Type = true,
                    };
                    list.Add(entity);
                }
            }
            return list;
        }

        #endregion 收款单

        #region 收款单（负数）

        /// <summary>
        /// 获取收款单（负数)：商品检查
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public IList<GatheringDTO> GetGathering_ReturnMoney(DateTime StartTime, DateTime EndTime)
        {
            //调用加盟店的wcf服务。 GetReturnMoneyByOrderId(Guid orderId);
            return null;
        }

        #endregion 收款单（负数）

        #region Helper Method

        /// <summary>
        /// 初始化sql参数
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        private static List<SqlParameter> InitSqlParameter(DateTime StartTime, DateTime EndTime)
        {
            return new List<SqlParameter>
                                               {
                                                   StartTime == DateTime.MinValue?new SqlParameter("@start", new DateTime(2000, 1, 1)):new SqlParameter("@start", StartTime),
                                                   new SqlParameter("@end", EndTime)
                                               };
        }


        #endregion Helper Method
    }
}