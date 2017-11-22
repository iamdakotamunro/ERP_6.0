using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IInventory;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 往来单位收付款发票权限数据层
    /// </summary>
    public class CompanyInvoicePower : ICompanyInvoicePower
    {
        public CompanyInvoicePower(Environment.GlobalConfig.DB.FromType fromType) { }

        #region[SQL]
        /// <summary>
        /// 获取往来单位收付款所有权限
        /// </summary>
        private const string SQL_SELECT_COMPANY_INVOICE_POWER =
            @"SELECT PowerID,InvoicesType,CompanyID,FilialeID,BranchID,PositionID,AuditorID,BindingType,ParentPowerID FROM lmshop_CompanyInvoicePower WHERE 1=1 ";
        /// <summary>
        /// 添加往来单位收付款发票权限
        /// </summary>
        private const string SQL_INSERT_COMPANY_INVOICE_POWER =
            @"INSERT INTO lmshop_CompanyInvoicePower VALUES(@PowerID,@InvoicesType,@CompanyID,@FilialeId,@BranchID,@PositionID,@AuditorID,@BindingType,@ParentPowerID)";
        /// <summary>
        /// 修改往来单位收付款发票权限
        /// </summary>
        private const string SQL_UPDATE_COMPANY_INVOICE_POWER =
            @"UPDATE lmshop_CompanyInvoicePower SET InvoicesType=@InvoicesType,
            FilialeId=@FilialeId,BranchID=@BranchID,PositionID=@PositionID,AuditorID=@AuditorID WHERE 1=1 ";
        /// <summary>
        /// 删除往来单位收付款发票权限
        /// </summary>
        private const string SQL_DELETE_COMPANY_INVOICE_POWER =
            @"DELETE lmshop_CompanyInvoicePower WHERE PowerID=@PowerID OR ParentPowerID=@PowerID ";

        #endregion

        #region[参数]
        /// <summary>
        /// 权限ID
        /// </summary>
        private const string PARM_POWER_ID = "@PowerID";
        /// <summary>
        /// 发票类型
        /// </summary>
        private const string PARM_INVOICES_TYPE = "@InvoicesType";
        /// <summary>
        /// 往来单位ID
        /// </summary>
        private const string PARM_COMPANY_ID = "@CompanyID";
        /// <summary>
        /// 公司ID
        /// </summary>
        private const string PARM_FILIALE_ID = "@FilialeId";
        /// <summary>
        /// 部门ID
        /// </summary>
        private const string PARM_BRANCH_ID = "@BranchID";
        /// <summary>
        /// 职务ID
        /// </summary>
        private const string PARM_POSITION_ID = "@PositionID";
        /// <summary>
        /// 审核人ID
        /// </summary>
        private const string PARM_AUDITOR_ID = "@AuditorID";
        /// <summary>
        /// 绑定方式
        /// </summary>
        private const string PARM_BINDING_TYPE = "@BindingType";
        /// <summary>
        /// 所属直接绑定权限ID
        /// </summary>
        private const string PARM_PARENT_POWER_ID = "@ParentPowerID";

        #endregion

        #region[声明参数]
        private static SqlParameter[] GetCompanyParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_POWER_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_INVOICES_TYPE, SqlDbType.Int),
                new SqlParameter(PARM_COMPANY_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_FILIALE_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_BRANCH_ID, SqlDbType.UniqueIdentifier),
				new SqlParameter(PARM_POSITION_ID, SqlDbType.UniqueIdentifier),
				new SqlParameter(PARM_AUDITOR_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_BINDING_TYPE, SqlDbType.Int),
                new SqlParameter(PARM_PARENT_POWER_ID,SqlDbType.UniqueIdentifier)
            };
            return parms;
        }
        #endregion

        #region[获取所有往来单位绑定的权限]
        /// <summary>
        /// 获取所有往来单位绑定的权限
        /// </summary>
        /// <returns></returns>
        public IList<CompanyInvoicePowerInfo> GetALLCompanyInvoicePower()
        {
            IList<CompanyInvoicePowerInfo> list = new List<CompanyInvoicePowerInfo>();
            try
            {
                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANY_INVOICE_POWER, null))
                {
                    while (dr.Read())
                    {
                        var info = new CompanyInvoicePowerInfo(dr.GetGuid(0), dr.GetInt32(1), dr.GetGuid(2), dr.GetGuid(3),
                            dr.GetGuid(4), dr.GetGuid(5), dr.GetGuid(6), dr.GetInt32(7), dr.GetGuid(8));
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[添加往来单位收付款发票权限]
        /// <summary>
        /// 添加往来单位收付款发票权限
        /// </summary>
        /// <param name="info">权限模型</param>
        public void InsertCompanyInvoicePower(CompanyInvoicePowerInfo info)
        {
            SqlParameter[] parms = GetCompanyParameters();
            parms[0].Value = info.PowerID;
            parms[1].Value = info.InvoicesType;
            parms[2].Value = info.CompanyID;
            parms[3].Value = info.FilialeID;
            parms[4].Value = info.BranchID;
            parms[5].Value = info.PositionID;
            parms[6].Value = info.AuditorID;
            parms[7].Value = info.BindingType;
            parms[8].Value = info.ParentPowerID;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_COMPANY_INVOICE_POWER, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[根据往来单位获取所绑定的权限]
        /// <summary>
        /// 根据往来单位获取所绑定的权限
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public IList<CompanyInvoicePowerInfo> GetCompanyInvoicePowerByCompanyID(Guid companyId)
        {
            var parm = new SqlParameter(PARM_COMPANY_ID, SqlDbType.UniqueIdentifier) { Value = companyId };
            var sql = new StringBuilder(SQL_SELECT_COMPANY_INVOICE_POWER);
            sql.Append(" AND CompanyID=@CompanyID ");
            IList<CompanyInvoicePowerInfo> list = new List<CompanyInvoicePowerInfo>();
            try
            {
                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), parm))
                {
                    while (dr.Read())
                    {
                        var info = new CompanyInvoicePowerInfo(dr.GetGuid(0), dr.GetInt32(1), dr.GetGuid(2), dr.GetGuid(3),
                            dr.GetGuid(4), dr.GetGuid(5), dr.GetGuid(6), dr.GetInt32(7), dr.GetGuid(8));
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[修改往来单位收付款发票权限]
        /// <summary>
        /// 修改往来单位收付款发票权限
        /// </summary>
        /// <param name="info">权限模型</param>
        /// <param name="updateType">修改模式：0按权限ID修改，1按所属权限ID修改</param>
        public void UpdateCompanyInvoicePower(CompanyInvoicePowerInfo info, int updateType)
        {
            SqlParameter[] parms = GetCompanyParameters();
            parms[0].Value = info.PowerID;
            parms[1].Value = info.InvoicesType;
            parms[2].Value = info.CompanyID;
            parms[3].Value = info.FilialeID;
            parms[4].Value = info.BranchID;
            parms[5].Value = info.PositionID;
            parms[6].Value = info.AuditorID;
            parms[7].Value = info.BindingType;
            parms[8].Value = info.ParentPowerID;

            var sql = new StringBuilder(SQL_UPDATE_COMPANY_INVOICE_POWER);
            sql.Append(updateType == 0 ? " AND PowerID=@PowerID " : " AND ParentPowerID=@ParentPowerID ");
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql.ToString(), parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[根据所属权限ID获取所绑定的权限]
        /// <summary>
        /// 根据所属权限ID获取所绑定的权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        /// <returns></returns>
        public IList<CompanyInvoicePowerInfo> GetCompanyInvoicePowerByPowerID(Guid powerId)
        {
            var parm = new SqlParameter(PARM_POWER_ID, SqlDbType.UniqueIdentifier) { Value = powerId };
            var sql = new StringBuilder(SQL_SELECT_COMPANY_INVOICE_POWER);
            sql.Append(" AND ParentPowerID=@PowerID ");
            IList<CompanyInvoicePowerInfo> list = new List<CompanyInvoicePowerInfo>();
            try
            {
                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), parm))
                {
                    while (dr.Read())
                    {
                        var info = new CompanyInvoicePowerInfo(dr.GetGuid(0), dr.GetInt32(1), dr.GetGuid(2), dr.GetGuid(3),
                            dr.GetGuid(4), dr.GetGuid(5), dr.GetGuid(6), dr.GetInt32(7), dr.GetGuid(8));
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[删除往来单位收付款发票权限]
        /// <summary>
        /// 删除往来单位收付款发票权限
        /// </summary>
        /// <param name="powerId">权限ID</param>
        public void DeleteCompanyInvoicePower(Guid powerId)
        {
            var parm = new SqlParameter(PARM_POWER_ID, SqlDbType.UniqueIdentifier) { Value = powerId };

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_COMPANY_INVOICE_POWER, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
    }
}
