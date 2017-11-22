using ERP.DAL.Finance.Interface;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.Finance.Gathering;
using ERP.Model.Finance.Payment;
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
    /// 财务API：付款单的DAL
    /// </summary>
    public class PaymentDAL : IPaymentDAL
    {
        /// <summary>
        /// 财务API：付款单的DAL
        /// </summary>
        /// <param name="fromType"></param>
        public PaymentDAL(GlobalConfig.DB.FromType fromType) { }

        #region 付款单

        /// <summary>
        /// 获取付款单：往来单位收付款（收付款类型=付款单）
        /// 条件：类型=付款
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<CompanyFundReceiptDTO> GetPayment_CompanyFundReceipt(DateTime StartTime, DateTime EndTime)
        {
            IList<CompanyFundReceiptDTO> list = new List<CompanyFundReceiptDTO>();

            string sql = @" SELECT ReceiptID,ReceiptNo,FilialeId,BillingDate,RealityBalance,ApplicantID
                            FROM [lmshop_CompanyFundReceipt]
                            WHERE lyDateTime >= @start AND lyDateTime < @end AND [ReceiptType]=" + (int)CompanyFundReceiptType.Payment;
            var listparam = InitSqlParameter(StartTime, EndTime);

            sql += " ORDER BY lyDateTime DESC";

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
                        RealityBalance = dr["BillingDate"].ObjToDecimal(),
                        ApplicantID = dr["licantID"].ObjToGuid(),
                        Type = false,
                    };
                    list.Add(entity);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取付款单：资金流（转账）。上海百秀旗舰店》百秀兰溪转账。
        /// 条件：记账本类型=收入 && 资金流单据状态=红冲 && 操作状态 = 已处理 && 单据类型=转账 && 销售公司=百秀兰溪 && 银行=上海百秀所有的银行
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="LogisticsCompany">物流公司</param>
        /// <param name="SalesCompany">销售公司</param>
        /// <returns></returns>
        public IList<WasteBookDTO> GetPayment_WasteBook(DateTime StartTime, DateTime EndTime, string LogisticsCompany, string SalesCompany)
        {
            //转出公司（物流公司）：上海百秀 444e0c93-1146-4386-bae2-cb352da70499
            //转入公司（销售公司）：百秀兰溪 75621b55-2fa3-4fcf-b68a-039c28f560b6

            IList<WasteBookDTO> list = new List<WasteBookDTO>();
            string sql = string.Format(@" SELECT TradeCode,SaleFilialeId,DateCreated,Income
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
                                (int)WasteBookType.Increase, (int)WasteBookState.Cancellation, 1, AuditingState.Yes);

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
                        Income = dr["Income"].ObjToDecimal(),
                        Type = false,
                    };
                    list.Add(entity);
                }
            }
            return list;
        }

        #endregion 付款单

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