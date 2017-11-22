using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 费用申报票据操作类
    /// </summary>
    public class CostReportInvoice : ICostReportInvoice
    {
        public CostReportInvoice(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region[SQL语句]
        /// <summary>
        /// 添加票据
        /// </summary>
        private const string SQL_INSERT_INVOICE =
            @"INSERT INTO lmshop_CostReportInvoice
            (InvoiceId,ImagePath,State,RealityCost,Memo) 
            VALUES(@InvoiceId,@ImagePath,@State,@RealityCost,@Memo)";
        /// <summary>
        /// 获取票据
        /// </summary>
        private const string SQL_GET_INVOICE =
            @"SELECT InvoiceId,ImagePath,State,RealityCost,Memo FROM lmShop_CostReportInvoice";
        /// <summary>
        /// 修改票据
        /// </summary>
        private const string SQL_UPDATE_INVOICE =
            @"UPDATE lmShop_CostReportInvoice SET ImagePath=@ImagePath,State=@State,RealityCost=@RealityCost,Memo=@Memo WHERE InvoiceId=@InvoiceId";
        /// <summary>
        /// 修改票据状态
        /// </summary>
        private const string SQL_UPDATE_INVOICE_STATE =
            @"UPDATE lmShop_CostReportInvoice SET State=@State WHERE InvoiceId=@InvoiceId";

        #endregion

        #region[参数]
        /// <summary>
        /// 票据ID
        /// </summary>
        private const string PARM_INVOICE_ID = "@InvoiceId";
        /// <summary>
        /// 图片地址
        /// </summary>
        private const string PARM_IMAGE_PATH = "@ImagePath";
        /// <summary>
        /// 票据状态
        /// </summary>
        private const string PARM_STATE = "@State";
        /// <summary>
        /// 实际金额
        /// </summary>
        private const string PARM_REALITY_COST = "@RealityCost";
        /// <summary>
        /// 说明
        /// </summary>
        private const string PARM_MEMO = "@Memo";
        #endregion

        #region[声明参数]
        /// <summary>
        /// 声明参数
        /// </summary>
        /// <returns></returns>
        private SqlParameter[] GetParameter()
        {
            var parms = new[]{
                new SqlParameter(PARM_INVOICE_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_IMAGE_PATH,SqlDbType.VarChar,128),
                new SqlParameter(PARM_STATE,SqlDbType.Int),
                new SqlParameter(PARM_REALITY_COST,SqlDbType.Decimal),
                new SqlParameter(PARM_MEMO,SqlDbType.VarChar,4024)
            };
            return parms;
        }
        #endregion

        #region[添加票据]
        /// <summary>
        /// 添加票据
        /// </summary>
        /// <param name="info">票据模型</param>
        public void InsertInvoice(CostReportInvoiceInfo info)
        {
            SqlParameter[] parms = GetParameter();
            parms[0].Value = info.InvoiceId;
            parms[1].Value = info.ImagePath;
            parms[2].Value = info.State;
            parms[3].Value = info.RealityCost;
            parms[4].Value = info.Memo;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_INVOICE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[获取待审核票据]
        /// <summary>
        /// 获取待审核票据
        /// </summary>
        /// <returns></returns>
        public IList<CostReportInvoiceInfo> GetInvoiceList()
        {
            IList<CostReportInvoiceInfo> list = new List<CostReportInvoiceInfo>();
            var sql = new StringBuilder(SQL_GET_INVOICE);
            sql.Append(" WHERE State=@State");
            var parm = new SqlParameter(PARM_STATE, SqlDbType.Int) { Value = (int)CostReportInvoiceState.NoAuditing };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), parm))
            {
                while (dr.Read())
                {
                    var info = new CostReportInvoiceInfo(dr.GetGuid(0), dr.GetString(1), dr.GetInt32(2), dr[3] == DBNull.Value ? 0 : dr.GetDecimal(3), dr[4] == DBNull.Value ? "" : dr.GetString(4));
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion

        #region[获取所有票据]
        /// <summary>
        /// 获取所有票据
        /// </summary>
        /// <returns></returns>
        public IList<CostReportInvoiceInfo> GetAllInvoiceList()
        {
            IList<CostReportInvoiceInfo> list = new List<CostReportInvoiceInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_GET_INVOICE, null))
            {
                while (dr.Read())
                {
                    var info = new CostReportInvoiceInfo(dr.GetGuid(0), dr.GetString(1), dr.GetInt32(2), dr[3] == DBNull.Value ? 0 : dr.GetDecimal(3), dr[4] == DBNull.Value ? "" : dr.GetString(4));
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion

        #region[根据票据ID获取待审核票据]
        /// <summary>
        /// 根据票据ID获取待审核票据
        /// </summary>
        /// <param name="invoiceId">票据ID</param>
        /// <returns></returns>
        public CostReportInvoiceInfo GetInvoice(Guid invoiceId)
        {
            var info = new CostReportInvoiceInfo();
            var sql = new StringBuilder(SQL_GET_INVOICE);
            sql.Append(" WHERE InvoiceId=@InvoiceId");
            var parm = new SqlParameter(PARM_INVOICE_ID, SqlDbType.UniqueIdentifier) { Value = invoiceId };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), parm))
            {
                if (dr.Read())
                {
                    info = new CostReportInvoiceInfo(dr.GetGuid(0), dr.GetString(1), dr.GetInt32(2), dr[3] == DBNull.Value ? 0 : dr.GetDecimal(3), dr[4] == DBNull.Value ? "" : dr.GetString(4));
                }
            }
            return info;
        }
        #endregion

        #region[修改票据]
        /// <summary>
        /// 修改票据
        /// </summary>
        /// <param name="info">票据模型</param>
        public void UpdateInvoice(CostReportInvoiceInfo info)
        {
            var parms = new[]{
                new SqlParameter("@InvoiceId",info.InvoiceId),
                new SqlParameter("@ImagePath",info.ImagePath),
                new SqlParameter("@State",info.State),
                new SqlParameter("@RealityCost",info.RealityCost),
                new SqlParameter("@Memo",info.Memo)
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_INVOICE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改票据状态]
        /// <summary>
        /// 修改票据状态
        /// </summary>
        /// <param name="info">票据模型</param>
        public void UpdateInvoiceState(CostReportInvoiceInfo info)
        {
            var parms = new[]{
                new SqlParameter(PARM_INVOICE_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_STATE,SqlDbType.Int)};
            parms[0].Value = info.InvoiceId;
            parms[1].Value = info.State;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_INVOICE_STATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[删除票据]
        /// <summary>
        /// 根据网页凭证id，删除网页凭证数据
        /// </summary>
        /// <param name="invoiceId">网页凭证id</param>
        /// zal 2015-12-07
        public bool DelInvoice(Guid invoiceId)
        {
            string sql = "delete lmShop_CostReportInvoice where InvoiceId=@InvoiceId";
            var parms = new SqlParameter("@InvoiceId", invoiceId);

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
    }
}
