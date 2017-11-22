//Author: zal
//createtime:2016/1/12 16:32:07
//Description:

using System.Collections.Generic;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using ERP.Environment;
using System;
using System.Data;
using System.Linq;
using System.Text;
using Keede.DAL.Helper;
using System.Transactions;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 该类提供了一系列操作 "lmshop_CompanyFundReceiptInvoice" 表的方法;
    /// </summary>
    public class CompanyFundReceiptInvoice : ICompanyFundReceiptInvoice
    {
        public CompanyFundReceiptInvoice(GlobalConfig.DB.FromType fromType) { }

        const string SQL_SELECT = "select [InvoiceId],[ReceiptID],[BillingUnit],[BillingDate],[InvoiceNo],[InvoiceCode],[NoTaxAmount],[Tax],[TaxAmount],[InvoiceState],[OperatingTime],[Memo],[Remark] from [lmshop_CompanyFundReceiptInvoice] ";
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回lmshop_CompanyFundReceiptInvoice表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<CompanyFundReceiptInvoiceInfo> GetAlllmshop_CompanyFundReceiptInvoice()
        {
            List<CompanyFundReceiptInvoiceInfo> lmshopCompanyFundReceiptInvoiceList = new List<CompanyFundReceiptInvoiceInfo>();
            
            string sql = SQL_SELECT;
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice = new CompanyFundReceiptInvoiceInfo(reader);
                lmshopCompanyFundReceiptInvoiceList.Add(lmshopCompanyFundReceiptInvoice);
            }
            reader.Close();
            return lmshopCompanyFundReceiptInvoiceList;
        }
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的receiptId字段返回数据  
        /// </summary>
        /// <param name="receiptId">receiptId</param>
        /// <returns></returns>        
        public List<CompanyFundReceiptInvoiceInfo> Getlmshop_CompanyFundReceiptInvoiceByReceiptID(Guid receiptId)
        {
            List<CompanyFundReceiptInvoiceInfo> lmshopCompanyFundReceiptInvoiceList = new List<CompanyFundReceiptInvoiceInfo>();
            
            string sql = SQL_SELECT + "where [ReceiptID] = @ReceiptID";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReceiptID",receiptId)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            while (reader.Read())
            {
                CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice = new CompanyFundReceiptInvoiceInfo(reader);
                lmshopCompanyFundReceiptInvoiceList.Add(lmshopCompanyFundReceiptInvoice);
            }
            reader.Close();
            return lmshopCompanyFundReceiptInvoiceList;
        }
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段返回数据 
        /// </summary>
        /// <param name="invoiceId">InvoiceId</param>
        /// <returns></returns>       
        public CompanyFundReceiptInvoiceInfo Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(Guid invoiceId)
        {
            CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice = null;
            
            string sql = SQL_SELECT + "where [InvoiceId] = @InvoiceId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@InvoiceId",invoiceId)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            if (reader.Read())
            {
                lmshopCompanyFundReceiptInvoice = new CompanyFundReceiptInvoiceInfo(reader);
            }
            reader.Close();
            return lmshopCompanyFundReceiptInvoice;
        }

        /// <summary>
        /// 返回lmshop_CompanyFundReceiptInvoice表的所有数据 
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="startApplyDateTime"></param>
        /// <param name="endApplyDateTime"></param>
        /// <param name="receiptStatus"></param>
        /// <param name="receiptNo"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceState"></param>
        /// <param name="invoiceType"></param>
        /// <param name="startOperatingTime"></param>
        /// <param name="endOperatingTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataTable Getlmshop_CompanyFundReceiptInvoiceByPage(Guid filialeId, Guid companyId, DateTime? startApplyDateTime, DateTime? endApplyDateTime, string receiptStatus, string receiptNo, string invoiceNo, int? invoiceState, DateTime? startOperatingTime, DateTime? endOperatingTime, string billingUnit, int? invoiceType, int pageIndex, int pageSize, out int total)
        {
            var sql = new StringBuilder();
            sql.Append(@"
select A.ReceiptNo,A.CompanyID,A.FilialeId,A.RealityBalance,A.ApplicantID,A.ReceiptStatus,A.ApplyDateTime,A.InvoiceType,
B.InvoiceId,B.BillingUnit,B.BillingDate,B.InvoiceNo,B.InvoiceCode,B.NoTaxAmount,B.Tax,B.TaxAmount,B.InvoiceState,B.OperatingTime,B.Memo,B.Remark,
ROW_NUMBER() over(order by B.OperatingTime desc) as RowNum 
from lmshop_CompanyFundReceipt AS A with(nolock)
inner join lmshop_CompanyFundReceiptInvoice AS B with(nolock) on A.ReceiptID=B.ReceiptID
where A.ReceiptType=1 
");


            if (filialeId != Guid.Empty)
            {
                sql.Append(" and FilialeId='").Append(filialeId).Append("'");
            }
            if (companyId != Guid.Empty)
            {
                sql.Append(" and CompanyID='").Append(companyId).Append("'");
            }
            if (startApplyDateTime != null)
            {
                sql.Append(" and ApplyDateTime>='").Append(startApplyDateTime).Append("'");
            }
            if (endApplyDateTime != null)
            {
                sql.Append(" and ApplyDateTime<'").Append(endApplyDateTime).Append("'");
            }
            if (!string.IsNullOrEmpty(receiptStatus))
            {
                sql.Append(" and CHARINDEX (cast(ReceiptStatus as varchar(8)),'").Append(receiptStatus).Append("')>0");
            }
            if (!string.IsNullOrEmpty(receiptNo))
            {
                sql.Append(" and ReceiptNo='").Append(receiptNo).Append("'");
            }
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                sql.Append(" and InvoiceNo='").Append(invoiceNo).Append("'");
            }
            if (invoiceState != null)
            {
                sql.Append(" and InvoiceState='").Append(invoiceState).Append("'");
            }
            if (invoiceType != null)
            {
                sql.Append(" and A.InvoiceType='").Append(invoiceType).Append("'");
            }
            if (startOperatingTime != null)
            {
                sql.Append(" and OperatingTime>='").Append(startOperatingTime).Append("'");
            }
            if (endOperatingTime != null)
            {
                sql.Append(" and OperatingTime<'").Append(endOperatingTime).Append("'");
            }
            if (!string.IsNullOrEmpty(billingUnit))
            {
                sql.Append(" and BillingUnit like'%").Append(billingUnit).Append("%'");
            }

            var strSql = new StringBuilder();
            strSql.Append("with tabs as(" + sql + ")");

            var sqlCount = strSql + " select count(0) from tabs";
            var count = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sqlCount, null);
            total = int.Parse(count.ToString());

            strSql.Append(@"
select ReceiptNo,CompanyID,FilialeId,RealityBalance,ApplicantID,ReceiptStatus,ApplyDateTime,InvoiceType,
InvoiceId,BillingUnit,BillingDate,InvoiceNo,InvoiceCode,NoTaxAmount,Tax,TaxAmount,InvoiceState,OperatingTime,Memo,Remark
from tabs where RowNum between @pageSize*(@pageIndex-1) + 1 and @pageSize*@pageIndex");

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@pageIndex", pageIndex)
            };
            using (var conn = Keede.DAL.RWSplitting.Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                using (var sqlCommand = new SqlCommand(strSql.ToString(), conn))
                {
                    sqlCommand.Parameters.AddRange(paras);
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet, "table");
                        sqlCommand.Parameters.Clear();
                        return dataSet.Tables[0];
                    }
                }
            }
        }

        /// <summary>
        /// 返回lmshop_CompanyFundReceiptInvoice表的合计数据 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable Getlmshop_CompanyFundReceiptInvoiceFromSum(string sql)
        {
            var strSql = sql + " select isnull(SUM(TaxAmount),0) AS TaxAmount, isnull(SUM(NoTaxAmount),0) AS NoTaxAmount,isnull(SUM(Tax),0) AS Tax,count(0) AS InvoiceTotal from tabs where TaxAmount > 0";

            using (var conn = Keede.DAL.RWSplitting.Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                using (var sqlCommand = new SqlCommand(strSql, conn))
                {
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet, "table");
                        sqlCommand.Parameters.Clear();
                        return dataSet.Tables[0];
                    }
                }
            }
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段删除数据 
        /// </summary>
        /// <param name="invoiceId">invoiceId</param>
        /// <returns></returns>        
        public bool Deletelmshop_CompanyFundReceiptInvoiceByInvoiceId(Guid invoiceId)
        {
            string sql = "delete from [lmshop_CompanyFundReceiptInvoice] where [InvoiceId] = @InvoiceId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@InvoiceId",invoiceId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的receiptId字段删除数据 
        /// </summary>
        /// <param name="receiptId">receiptId</param>
        /// <returns></returns>        
        public bool Deletelmshop_CompanyFundReceiptInvoiceByReceiptID(Guid receiptId)
        {
            string sql = "delete from [lmshop_CompanyFundReceiptInvoice] where [ReceiptID] = @ReceiptID";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@ReceiptID",receiptId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region update data
        /// <summary>
        /// prepare parameters 
        /// </summary>
        public static SqlParameter[] PrepareCommandParameters(CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice)
        {
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@InvoiceId",lmshopCompanyFundReceiptInvoice.InvoiceId),
            new SqlParameter("@ReceiptID",lmshopCompanyFundReceiptInvoice.ReceiptID),
            new SqlParameter("@BillingUnit",lmshopCompanyFundReceiptInvoice.BillingUnit),
            new SqlParameter("@BillingDate",lmshopCompanyFundReceiptInvoice.BillingDate),
            new SqlParameter("@InvoiceNo",lmshopCompanyFundReceiptInvoice.InvoiceNo),
            new SqlParameter("@InvoiceCode",lmshopCompanyFundReceiptInvoice.InvoiceCode),
            new SqlParameter("@NoTaxAmount",lmshopCompanyFundReceiptInvoice.NoTaxAmount),
            new SqlParameter("@Tax",lmshopCompanyFundReceiptInvoice.Tax),
            new SqlParameter("@TaxAmount",lmshopCompanyFundReceiptInvoice.TaxAmount),
            new SqlParameter("@InvoiceState",lmshopCompanyFundReceiptInvoice.InvoiceState),
            new SqlParameter("@OperatingTime",lmshopCompanyFundReceiptInvoice.OperatingTime),
            new SqlParameter("@Memo",lmshopCompanyFundReceiptInvoice.Memo),
            new SqlParameter("@Remark",lmshopCompanyFundReceiptInvoice.Remark)
            };
            return paras;
        }
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段更新数据 
        /// </summary> 
        /// <param name="lmshopCompanyFundReceiptInvoice">lmshopCompanyFundReceiptInvoice</param>
        /// <returns></returns>       
        public bool Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice)
        {
            string sql = "update [lmshop_CompanyFundReceiptInvoice] set [ReceiptID] = @ReceiptID,[BillingUnit]=@BillingUnit,[BillingDate] = @BillingDate,[InvoiceNo] = @InvoiceNo,[InvoiceCode] = @InvoiceCode,[NoTaxAmount] = @NoTaxAmount,[Tax] = @Tax,[TaxAmount] = @TaxAmount,[InvoiceState] = @InvoiceState,[OperatingTime] = @OperatingTime,[Memo] = @Memo,[Remark]=@Remark where [InvoiceId] = @InvoiceId";
            SqlParameter[] paras = PrepareCommandParameters(lmshopCompanyFundReceiptInvoice);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段修改数据
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="invoiceId">主键</param>
        /// <param name="invoiceState">发票状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        public bool Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(string remark, Guid invoiceId, int invoiceState)
        {
            string sql = "update [lmshop_CompanyFundReceiptInvoice] set [Remark]= isnull(Remark,'')+@Remark,InvoiceState=@InvoiceState,OperatingTime=@OperatingTime where [InvoiceId] = @InvoiceId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@InvoiceId",invoiceId),
            new SqlParameter("@Remark",remark),
            new SqlParameter("@InvoiceState",invoiceState),
            new SqlParameter("@OperatingTime",DateTime.Now)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段修改备注信息
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="invoiceId">主键</param>
        /// <returns></returns>
        public bool Updatelmshop_CompanyFundReceiptInvoiceRemarkByInvoiceId(string remark, Guid invoiceId)
        {
            string sql = "update [lmshop_CompanyFundReceiptInvoice] set [Remark]= isnull(Remark,'')+@Remark where [InvoiceId] = @InvoiceId";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@InvoiceId",invoiceId),
            new SqlParameter("@Remark",remark)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向lmshop_CompanyFundReceiptInvoice表插入一条数据
        /// </summary>
        /// <param name="lmshopCompanyFundReceiptInvoice">lmshop_CompanyFundReceiptInvoice</param>       
        /// <returns></returns>        
        public bool Addlmshop_CompanyFundReceiptInvoice(CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice)
        {
            string sql = "insert into [lmshop_CompanyFundReceiptInvoice]([InvoiceId],[ReceiptID],[BillingUnit],[BillingDate],[InvoiceNo],[InvoiceCode],[NoTaxAmount],[Tax],[TaxAmount],[InvoiceState],[OperatingTime],[Memo],[Remark])values(@InvoiceId,@ReceiptID,@BillingUnit,@BillingDate,@InvoiceNo,@InvoiceCode,@NoTaxAmount,@Tax,@TaxAmount,@InvoiceState,@OperatingTime,@Memo,@Remark)";
            SqlParameter[] paras = PrepareCommandParameters(lmshopCompanyFundReceiptInvoice);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 向lmshop_CompanyFundReceiptInvoice表批量插入数据
        /// </summary>
        /// <param name="lmshopCompanyFundReceiptInvoiceList"></param>
        /// <returns></returns>
        public bool AddBulklmshop_CompanyFundReceiptInvoice(List<CompanyFundReceiptInvoiceInfo> lmshopCompanyFundReceiptInvoiceList)
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            dics.Add("InvoiceId", "InvoiceId");
            dics.Add("ReceiptID", "ReceiptID");
            dics.Add("BillingUnit", "BillingUnit");
            dics.Add("BillingDate", "BillingDate");
            dics.Add("InvoiceNo", "InvoiceNo");
            dics.Add("InvoiceCode", "InvoiceCode");
            dics.Add("NoTaxAmount", "NoTaxAmount");
            dics.Add("Tax", "Tax");
            dics.Add("TaxAmount", "TaxAmount");
            dics.Add("InvoiceState", "InvoiceState");
            dics.Add("OperatingTime", "OperatingTime");
            dics.Add("Memo", "Memo");
            dics.Add("Remark", "Remark");
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, lmshopCompanyFundReceiptInvoiceList, "lmshop_CompanyFundReceiptInvoice", dics) > 0;
        }
        #endregion
        #endregion
    }
}
