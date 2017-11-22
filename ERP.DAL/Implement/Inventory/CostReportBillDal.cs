using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ERP.DAL.Implement.Inventory
{
    public class CostReportBillDal : ICostReportBill
    {
        public CostReportBillDal(GlobalConfig.DB.FromType fromType)
        {

        }
        const string SQL_SELECT = "SELECT [BillId],[ReportId],[BillUnit],[BillDate],[BillNo],[BillCode],[NoTaxAmount],[Tax],[TaxAmount],[BillState],[OperatingTime],[Memo],[Remark],[IsPay],[IsPass] FROM [lmShop_CostReportBill] ";
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回lmshop_CostReportBill表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<CostReportBillInfo> GetAlllmshop_CostReportBill()
        {
            List<CostReportBillInfo> lmshopCostReportBillList = new List<CostReportBillInfo>();
            IDataReader reader = null;

            string sql = SQL_SELECT;
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                CostReportBillInfo lmshopCostReportBill = new CostReportBillInfo(reader);
                lmshopCostReportBillList.Add(lmshopCostReportBill);
            }
            reader.Close();
            return lmshopCostReportBillList;
        }
        /// <summary>
        /// 根据lmshop_CostReportBill表的ReportId字段返回数据  
        /// </summary>
        /// <param name="reportId">ReportId</param>
        /// <returns></returns>        
        public List<CostReportBillInfo> Getlmshop_CostReportBillByReportId(Guid reportId)
        {
            List<CostReportBillInfo> lmshopCostReportBillList = new List<CostReportBillInfo>();
            
            string sql = SQL_SELECT + "where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            while (reader.Read())
            {
                CostReportBillInfo lmshopCostReportBill = new CostReportBillInfo(reader);
                lmshopCostReportBillList.Add(lmshopCostReportBill);
            }
            reader.Close();
            return lmshopCostReportBillList;
        }
        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段返回数据 
        /// </summary>
        /// <param name="billId">BillId</param>
        /// <returns></returns>       
        public CostReportBillInfo Getlmshop_CostReportBillByBillId(Guid billId)
        {
            CostReportBillInfo lmshopCostReportBill = null;
            IDataReader reader = null;

            string sql = SQL_SELECT + "where [BillId] = @BillId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@BillId",billId)
            };
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            if (reader.Read())
            {
                lmshopCostReportBill = new CostReportBillInfo(reader);
            }
            reader.Close();
            return lmshopCostReportBill;
        }

        /// <summary>
        /// 根据条件返回lmshop_CostReportBill表的数据 
        /// </summary>
        /// <param name="reportPersonnelId">申报人</param>
        /// <param name="invoiceTitleFilialeId">发票抬头</param>
        /// <param name="reportDateStart">申报时间</param>
        /// <param name="reportDateEnd">申报时间</param>
        /// <param name="reportStatus">费用申报状态</param>
        /// <param name="reportNo">申报编号</param>
        /// <param name="billNo">票据号码</param>
        /// <param name="billState">票据状态</param>
        /// <param name="operatingTimeStart">操作时间</param>
        /// <param name="operatingTimeEnd">操作时间</param>
        /// <param name="invoiceType">票据类型(1:普通发票;2:增值税专用发票;3:收据;)</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sumTable"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable Getlmshop_CostReportBillByPage(Guid reportPersonnelId, Guid invoiceTitleFilialeId, DateTime? reportDateStart, DateTime? reportDateEnd, string reportStatus, string reportNo, string billNo, int? billState, DateTime? operatingTimeStart, DateTime? operatingTimeEnd, int invoiceType, int pageIndex, int pageSize, out DataTable sumTable, out int total)
        {
            var sql = new StringBuilder();
            sql.Append(@"
            select A.ReportId,A.ReportNo,A.InvoiceTitleFilialeId,A.RealityCost,A.ActualAmount,A.ApplyForCost,A.ReportPersonnelId,A.[State],A.ReportDate,A.InvoiceType,A.ReportCost,A.ReportKind,
            B.BillId,B.BillUnit,B.BillDate,B.BillNo,B.BillCode,B.NoTaxAmount,B.Tax,B.TaxAmount,B.BillState,B.OperatingTime,B.Memo,B.Remark,
            ROW_NUMBER() over(order by A.ReportDate desc) as RowNum 
            from lmShop_CostReport AS A with(nolock)
            inner join lmShop_CostReportBill AS B with(nolock) on A.ReportId=B.ReportId
            where A.InvoiceTitleFilialeId!='00000000-0000-0000-0000-000000000000' and B.IsPass=1
            ");

            if (reportPersonnelId != Guid.Empty)
            {
                sql.Append(" and A.ReportPersonnelId='").Append(reportPersonnelId).Append("'");
            }
            if (invoiceTitleFilialeId != Guid.Empty)
            {
                sql.Append(" and A.InvoiceTitleFilialeId='").Append(invoiceTitleFilialeId).Append("'");
            }
            if (reportDateStart != null)
            {
                sql.Append(" and A.ReportDate>='").Append(reportDateStart).Append("'");
            }
            if (reportDateEnd != null)
            {
                sql.Append(" and A.ReportDate<'").Append(reportDateEnd).Append("'");
            }
            if (!string.IsNullOrEmpty(reportStatus))
            {
                sql.Append(" and A.[State] in(" + reportStatus + ")");
            }
            if (!string.IsNullOrEmpty(reportNo))
            {
                sql.Append(" and A.ReportNo='").Append(reportNo).Append("'");
            }
            if (!string.IsNullOrEmpty(billNo))
            {
                sql.Append(" and B.BillNo='").Append(billNo).Append("'");
            }
            if (billState != null)
            {
                sql.Append(" and B.BillState='").Append(billState).Append("'");
            }
            if (operatingTimeStart != null)
            {
                sql.Append(" and B.OperatingTime>='").Append(operatingTimeStart).Append("'");
            }
            if (operatingTimeEnd != null)
            {
                sql.Append(" and B.OperatingTime<'").Append(operatingTimeEnd).Append("'");
            }
            if (invoiceType != 0)
            {
                sql.Append(" and A.InvoiceType='").Append(invoiceType).Append("'");
            }

            var strSql = new StringBuilder();
            strSql.Append("with tabs as(" + sql + ")");

            sumTable = Getlmshop_CostReportBillFromSum(strSql.ToString());

            var sqlCount = strSql + " select count(0) from tabs";
            var count = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlCount, null);
            total = int.Parse(count.ToString());

            strSql.Append(@"
select ReportId,ReportNo,InvoiceTitleFilialeId,RealityCost,ActualAmount,ApplyForCost,ReportPersonnelId,[State],ReportDate,InvoiceType,ReportCost,ReportKind,
BillId,BillUnit,BillDate,BillNo,BillCode,NoTaxAmount,Tax,TaxAmount,BillState,OperatingTime,Memo,Remark
from tabs where RowNum between @pageSize*(@pageIndex-1) + 1 and @pageSize*@pageIndex");

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@pageIndex", pageIndex)
            };

            var dr= SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strSql.ToString(), paras);
            DataTable dt = new DataTable();
            dt.Load(dr);

            return dt;
        }

        /// <summary>
        /// 返回lmShop_CostReportBill表的合计数据 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable Getlmshop_CostReportBillFromSum(string sql)
        {
            var strSql = sql + @" 
                                select isnull(SUM(#temp.TaxAmount),0) AS TaxAmount, 
                                isnull(SUM(#temp.NoTaxAmount),0) AS NoTaxAmount,
                                isnull(SUM(#temp.Tax),0) AS Tax,
                                isnull(SUM(#temp.BillTotal),0) AS BillTotal,
                                isnull(SUM(#temp.ActualAmount),0) AS ActualAmount,
                                case InvoiceType 
                                when 1 then '普通发票'
                                when 5 then '增值税专用发票'
                                when 2 then '收据'
                                end AS InvoiceTypeName 
                                from (
                                    select isnull(SUM(TaxAmount),0) AS TaxAmount, 
                                    isnull(SUM(NoTaxAmount),0) AS NoTaxAmount,
                                    isnull(SUM(Tax),0) AS Tax,
                                    count(0) AS BillTotal,
                                    isnull(ActualAmount,0) AS ActualAmount,
                                    InvoiceType 
                                    from tabs where TaxAmount > 0
                                    group by ReportId,InvoiceType,ActualAmount
                                ) #temp
                                group by InvoiceType";

            var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strSql, null);
            DataTable dt = new DataTable();
            dt.Load(dr);

            return dt;
        }

        /// <summary>
        /// 根据billIdList统计票据单总数、含税金额
        /// </summary>
        /// <param name="billIdList"></param>
        /// <returns></returns>
        /// zal 2016-08-26
        public ArrayList GetSumBill(string[] billIdList)
        {
            ArrayList arrayList = new ArrayList();
            if (!billIdList.Any())
            {
                return arrayList;
            }
            var billIdStr = "'" + string.Join("','", billIdList.ToArray()) + "'";
            string sql = @"
            select COUNT(distinct BillId) as 'SumBill',
            IsNull(SUM(TaxAmount),0) as 'SumTaxAmount'
            from lmShop_CostReportBill with(nolock)
            where BillId in(" + billIdStr + ")";

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    arrayList.Add(dr["SumBill"] == DBNull.Value ? string.Empty : dr["SumBill"].ToString());
                    arrayList.Add(dr["SumTaxAmount"] == DBNull.Value ? string.Empty : dr["SumTaxAmount"].ToString());
                }
            }
            return arrayList;
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmshop_CostReportBill表的InvoiceId字段删除数据 
        /// </summary>
        /// <param name="billId">BillId</param>
        /// <returns></returns>        
        public bool Deletelmshop_CostReportBillByBillId(Guid billId)
        {
            string sql = "delete from [lmshop_CostReportBill] where [BillId] = @BillId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@BillId",billId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmshop_CostReportBill表的receiptId字段删除数据 
        /// </summary>
        /// <param name="reportId">ReportId</param>
        /// <returns></returns>        
        public bool Deletelmshop_CostReportBillByReportId(Guid reportId)
        {
            string sql = "delete from [lmshop_CostReportBill] where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region update data
        /// <summary>
        /// prepare parameters 
        /// </summary>
        public static SqlParameter[] PrepareCommandParameters(CostReportBillInfo lmshopCostReportBillInfo)
        {
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@BillId",lmshopCostReportBillInfo.BillId),
            new SqlParameter("@ReportId",lmshopCostReportBillInfo.ReportId),
            new SqlParameter("@BillUnit",lmshopCostReportBillInfo.BillUnit),
            new SqlParameter("@BillDate",lmshopCostReportBillInfo.BillDate),
            new SqlParameter("@BillNo",lmshopCostReportBillInfo.BillNo),
            new SqlParameter("@BillCode",lmshopCostReportBillInfo.BillCode),
            new SqlParameter("@NoTaxAmount",lmshopCostReportBillInfo.NoTaxAmount),
            new SqlParameter("@Tax",lmshopCostReportBillInfo.Tax),
            new SqlParameter("@TaxAmount",lmshopCostReportBillInfo.TaxAmount),
            new SqlParameter("@BillState",lmshopCostReportBillInfo.BillState),
            new SqlParameter("@OperatingTime",lmshopCostReportBillInfo.OperatingTime),
            new SqlParameter("@Memo",lmshopCostReportBillInfo.Memo),
            new SqlParameter("@Remark",lmshopCostReportBillInfo.Remark),
            new SqlParameter("@IsPay",lmshopCostReportBillInfo.IsPay),
            new SqlParameter("@IsPass",lmshopCostReportBillInfo.IsPass)
            };
            return paras;
        }
        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段更新数据 
        /// </summary> 
        /// <param name="lmshopCostReportBillInfo">lmshopCostReportBillInfo</param>
        /// <returns></returns>       
        public bool Updatelmshop_CostReportBillByBillId(CostReportBillInfo lmshopCostReportBillInfo)
        {
            string sql = "update [lmshop_CostReportBill] set [ReportId] = @ReportId,[BillUnit]=@BillUnit,[BillDate] = @BillDate,[BillNo] = @BillNo,[BillCode] = @BillCode,[NoTaxAmount] = @NoTaxAmount,[Tax] = @Tax,[TaxAmount] = @TaxAmount,[BillState] = @BillState,[OperatingTime] = @OperatingTime,[Memo] = @Memo,[Remark]=@Remark,[IsPay]=@IsPay,[IsPass]=@IsPass where [BillId] = @BillId";
            SqlParameter[] paras = PrepareCommandParameters(lmshopCostReportBillInfo);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段修改数据
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="billId">主键</param>
        /// <param name="billState">票据状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        public bool Updatelmshop_CostReportBillByBillId(string remark, Guid billId, int billState)
        {
            string sql = "update [lmshop_CostReportBill] set [Remark]= isnull(Remark,'')+@Remark,BillState=@BillState,OperatingTime=@OperatingTime where [BillId] = @BillId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@BillId",billId),
            new SqlParameter("@Remark",remark),
            new SqlParameter("@BillState",billState),
            new SqlParameter("@OperatingTime",DateTime.Now)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段修改备注信息
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="billId">主键</param>
        /// <returns></returns>
        public bool Updatelmshop_CostReportBillRemarkByBillId(string remark, Guid billId)
        {
            string sql = "update [lmshop_CostReportBill] set [Remark]= isnull(Remark,'')+@Remark where [BillId] = @BillId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@BillId",billId),
            new SqlParameter("@Remark",remark)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmshop_CostReportBill表的ReportId字段修改数据
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="reportId">外键</param>
        /// <param name="billState">票据状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        public bool Updatelmshop_CostReportBillByReportId(string remark, Guid reportId, int billState)
        {
            string sql = "update [lmshop_CostReportBill] set [Remark]= isnull(Remark,'')+@Remark,BillState=@BillState,OperatingTime=@OperatingTime where [ReportId] = @ReportId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId),
            new SqlParameter("@Remark",remark),
            new SqlParameter("@BillState",billState),
            new SqlParameter("@OperatingTime",DateTime.Now)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据ReportId修改未付款的票据的“受理通过”状态
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        /// zal 2016-11-25
        public bool Updatelmshop_CostReportBillForPassByReportId(Guid reportId, bool isPass)
        {
            string sql = "update [lmshop_CostReportBill] set [IsPass]=@IsPass" + (isPass ? "" : " ,BillState=0") + " where [ReportId] = @ReportId and IsPay=0";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReportId",reportId),
            new SqlParameter("@IsPass",isPass)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向lmshop_CostReportBill表插入一条数据
        /// </summary>
        /// <param name="lmshopCostReportBill">lmshopCostReportBill</param>       
        /// <returns></returns>        
        public bool Addlmshop_CostReportBill(CostReportBillInfo lmshopCostReportBill)
        {
            string sql = "insert into [lmshop_CostReportBill]([BillId],[ReportId],[BillUnit],[BillDate],[BillNo],[BillCode],[NoTaxAmount],[Tax],[TaxAmount],[BillState],[OperatingTime],[Memo],[Remark],[IsPay],[IsPass])values(@BillId,@ReportId,@BillUnit,@BillDate,@BillNo,@BillCode,@NoTaxAmount,@Tax,@TaxAmount,@BillState,@OperatingTime,@Memo,@Remark,@IsPay,@IsPass)";
            SqlParameter[] paras = PrepareCommandParameters(lmshopCostReportBill);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 向lmshop_CostReportBill表批量插入数据
        /// </summary>
        /// <param name="lmshopCostReportBillList"></param>
        /// <returns></returns>
        public bool AddBatchlmshop_CostReportBill(List<CostReportBillInfo> lmshopCostReportBillList)
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            dics.Add("BillId", "BillId");
            dics.Add("ReportId", "ReportId");
            dics.Add("BillUnit", "BillUnit");
            dics.Add("BillDate", "BillDate");
            dics.Add("BillNo", "BillNo");
            dics.Add("BillCode", "BillCode");
            dics.Add("NoTaxAmount", "NoTaxAmount");
            dics.Add("Tax", "Tax");
            dics.Add("TaxAmount", "TaxAmount");
            dics.Add("BillState", "BillState");
            dics.Add("OperatingTime", "OperatingTime");
            dics.Add("Memo", "Memo");
            dics.Add("Remark", "Remark");
            dics.Add("IsPay", "IsPay");
            dics.Add("IsPass", "IsPass");
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, lmshopCostReportBillList, "lmshop_CostReportBill", dics) > 0;
        }
        #endregion
        #endregion
    }
}
