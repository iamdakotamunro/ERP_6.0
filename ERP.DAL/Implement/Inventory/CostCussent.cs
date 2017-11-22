using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper;
using ERP.Environment;
using Keede.DAL.RWSplitting;

namespace ERP.DAL.Implement.Inventory
{
    public class CostCussent : ICostCussent
    {
        public CostCussent(Environment.GlobalConfig.DB.FromType fromType) { }

        private const string SQL_INSERT_COMPANYCUSSENT = "INSERT INTO CostCompany VALUES(@CompanyId,@CompanyClassId,@CompanyName,@Linkman,@Address,@PostalCode,@Phone,@Mobile,@Fax,@WebSite,@Email,@BankAccounts,@AccountNumber,@DateCreated,@CompanyType,@State,@Description,@SubjectInfo,@InvoiceAccountsId,@VoucherAccountsId,@CashAccountsId,@NoVoucherAccountsId);";
        private const string SQL_UPDATE_COMPANYCUSSENT = "UPDATE CostCompany SET CompanyClassId=@CompanyClassId,CompanyName=@CompanyName,Linkman=@Linkman,Address=@Address,PostalCode=@PostalCode,Phone=@Phone,Mobile=@Mobile,Fax=@Fax,WebSite=@WebSite,Email=@Email,BankAccounts=@BankAccounts,AccountNumber=@AccountNumber,DateCreated=@DateCreated,CompanyType=@CompanyType,State=@State,Description=@Description,SubjectInfo=@SubjectInfo,InvoiceAccountsId=@InvoiceAccountsId,VoucherAccountsId=@VoucherAccountsId,CashAccountsId=@CashAccountsId,NoVoucherAccountsId=@NoVoucherAccountsId WHERE CompanyId=@CompanyId;";
        private const string SQL_DELETE_COMPANYCUSSENT = "DELETE FROM lmShop_CostReckoning WHERE CompanyId=@CompanyId;DELETE FROM CostCompany WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENT = "SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,InvoiceAccountsId,VoucherAccountsId,CashAccountsId,NoVoucherAccountsId FROM CostCompany WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENT_LIST = "SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,InvoiceAccountsId,VoucherAccountsId,CashAccountsId,NoVoucherAccountsId FROM CostCompany ORDER BY DateCreated ASC;";
        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPE_LIST = "SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,InvoiceAccountsId,VoucherAccountsId,CashAccountsId,NoVoucherAccountsId FROM CostCompany WHERE CompanyType=@CompanyType;";
        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPEANDSTATE_LIST = "SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,InvoiceAccountsId,VoucherAccountsId,CashAccountsId,NoVoucherAccountsId FROM CostCompany WHERE CompanyType=@CompanyType AND State=@State;";

        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYCLASS_LIST_ALL = "SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,InvoiceAccountsId,VoucherAccountsId,CashAccountsId,NoVoucherAccountsId FROM CostCompany WHERE CompanyClassId=@CompanyClassId;";


        private const string SQL_SELECT_COMPANYCUSSENT_BY_STATE_LIST = "SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,InvoiceAccountsId,VoucherAccountsId,CashAccountsId,NoVoucherAccountsId FROM CostCompany WHERE State=@State;";
        private const string SQL_SELECT_COMPANYCUSSENT_TOTALLED = "SELECT NonceTotalled FROM lmShop_CostReckoning WHERE CompanyId=@CompanyId ORDER BY DateCreated DESC;";
        private const string SQL_SELECT_COMPANYCUSSENT_ISBEING = "SELECT CompanyId, CompanyClassId, CompanyName, Linkman, Address, PostalCode, Phone, Mobile, Fax, WebSite, Email, BankAccounts, AccountNumber, DateCreated, CompanyType, State, Description, SubjectInfo, InvoiceAccountsId, VoucherAccountsId, CashAccountsId, NoVoucherAccountsId FROM CostCompany WHERE CompanyName=@CompanyName;";
        private const string SQL_SELECT_COMPANYCUSSENT_ISEXPRESS = "SELECT ExpressId, DeliverId, CompanyId, ExpressName, OrderIndex, Homepage, Linkman, Phone, ExpressBill, Memo, FilialeID, WarehouseID, Code, CodeId, BackDisplayName, ExpressSurfaceType, ExpressProductType, ExpressServiceCodeType FROM lmShop_Express WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENT_ISMEMBER = "SELECT CompanyType FROM CostCompany WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENTEXTENDINFO_BY_COMPANYID = "select subjectInfo from CostCompany where CompanyId=@CompanyId;";
        private const string SQL_UPDAE_COMPANYCUSSENTEXTENDINFO_BY_COMPANYID = "UPDATE CostCompany SET subjectInfo=@subjectInfo+'\r\n'+Convert(varchar(1024),isnull(subjectInfo,''))   where CompanyId=@CompanyId";

        private const string PARM_COMPANYID = "@CompanyId";
        private const string PARM_COMPANYCLASSID = "@CompanyClassId";
        private const string PARM_COMPANYNAME = "@CompanyName";
        private const string PARM_COMPANYLINKMAN = "@Linkman";
        private const string PARM_COMPANYADDRESS = "@Address";
        private const string PARM_POSTALCODE = "@PostalCode";
        private const string PARM_PHONE = "@Phone";
        private const string PARM_MOBILE = "@Mobile";
        private const string PARM_FAX = "@Fax";
        private const string PARM_WEBSITE = "@WebSite";
        private const string PARM_EMAIL = "@Email";
        private const string PARM_BANKACCOUNTS = "@BankAccounts";
        private const string PARM_ACCOUNTNUMBER = "@AccountNumber";
        private const string PARM_DATECREATED = "@DateCreated";
        private const string PARM_COMPANYTYPE = "@CompanyType";
        private const string PARM_STATE = "@State";
        private const string PARM_DESCRIPTION = "@Description";
        private const string PARM_SUBJECTINFO = "@SubjectInfo";
        private const string PARM_PARENTCOMPANYCLASSID = "@parentCompanyClassId";
        private const string PARM_FILIALE_ID = "@FilialeId";
        private const string PARM_BRANCH_ID = "@BranchId";

        private const string PARM_INVOICE_ACCOUNTS_ID = "@InvoiceAccountsId";
        private const string PARM_VOUCHER_ACCOUNTS_ID = "@VoucherAccountsId";
        private const string PARM_CASH_ACCOUNTS_ID = "@CashAccountsId";
        private const string PARM_NO_VOUCHER_ACCOUNTS_ID = "@NoVoucherAccountsId";

        /// <summary>
        /// 或者往来单位回扣流水信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public String GetCussentExtendInfo(Guid companyId)
        {
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };
            string retValue = string.Empty;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENTEXTENDINFO_BY_COMPANYID, parm);
            if (obj != DBNull.Value)
            {
                retValue = obj.ToString();
            }
            return retValue;
        }

        /// <summary>
        /// 更新往来单位回扣流水信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="extend"></param>
        /// <returns></returns>
        public void UpDatetCussentExtendInfo(Guid companyId, String extend)
        {
            var parms = new[]{
                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_SUBJECTINFO,SqlDbType.VarChar)
            };
            parms[0].Value = companyId;
            parms[1].Value = extend;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDAE_COMPANYCUSSENTEXTENDINFO_BY_COMPANYID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        public decimal GetNonceCostByClassId(Guid companyClassId)
        {
            string sql = @"
SELECT ISNULL((SELECT TOP 1 NonceTotalled FROM lmShop_CostReckoning CR
	WHERE  CR.CompanyId=CC.CompanyId
	ORDER BY DateCreated DESC),0)AS NonceTotalled
FROM CostCompany CC WHERE CompanyClassId=@CompanyClassId
";
            if (companyClassId == Guid.Empty)
            {
                sql = @"
SELECT ISNULL((SELECT TOP 1 NonceTotalled FROM lmShop_CostReckoning CR
	WHERE  CR.CompanyId=CC.CompanyId
	ORDER BY DateCreated DESC),0)AS NonceTotalled
FROM CostCompany CC
";
            }
            using (var db = DatabaseFactory.Create())
            {
                var list = db.GetValues<decimal>(true, sql, new Parameter("CompanyClassId", companyClassId));
                return list.Sum(ent => ent);
            }
        }

        public decimal GetNonceCost(Guid companyId)
        {
            const string SQL = @"
SELECT TOP 1 NonceTotalled FROM lmShop_CostReckoning
WHERE  CompanyId=@CompanyId
ORDER BY DateCreated DESC
";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<decimal>(true, SQL, new Parameter("CompanyId", companyId));
            }
        }

        /// <summary>
        /// 往来单位总帐计算
        /// </summary>
        /// <param name="companyClassId">分类ID</param>
        /// <param name="companyId">公司ID</param>
        /// <param name="parentCompanyClassId">父类ID</param>
        /// <param name="assumeFilialeId">结算公司Id</param>
        /// <returns></returns>
        public Double GetCussentCount(Guid companyClassId, Guid companyId, Guid parentCompanyClassId, Guid assumeFilialeId)
        {  
            var parms = new[]{
            new SqlParameter(PARM_COMPANYCLASSID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_COMPANYID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_PARENTCOMPANYCLASSID,SqlDbType.UniqueIdentifier),
            new SqlParameter("@AssumeFilialeId",assumeFilialeId)
            };
            parms[0].Value = companyClassId;
            parms[1].Value = companyId;
            parms[2].Value = parentCompanyClassId;
            double counts = 0;
            object obj = SqlHelper.ExecuteScalarSP(GlobalConfig.ERP_DB_NAME, true, CommandType.StoredProcedure, "P_GetCostReckoningCount", parms);
            if (obj != DBNull.Value)
            {
                counts = Convert.ToDouble(obj);
            }
            return counts;
        }
        /// <summary>
        /// 添加往来单位信息
        /// </summary>
        /// <param name="companyCussent">往来单位信息类</param>
        public void Insert(CostCussentInfo companyCussent)
        {
            SqlParameter[] parms = GetCompanyCussentParameters();
            parms[0].Value = companyCussent.CompanyId;
            parms[1].Value = companyCussent.CompanyClassId;
            parms[2].Value = companyCussent.CompanyName;
            parms[3].Value = companyCussent.Linkman;
            parms[4].Value = companyCussent.Address;
            parms[5].Value = companyCussent.PostalCode;
            parms[6].Value = companyCussent.Phone;
            parms[7].Value = companyCussent.Mobile;
            parms[8].Value = companyCussent.Fax;
            parms[9].Value = companyCussent.WebSite;
            parms[10].Value = companyCussent.Email;
            parms[11].Value = companyCussent.BankAccounts;
            parms[12].Value = companyCussent.AccountNumber;
            parms[13].Value = companyCussent.DateCreated;
            parms[14].Value = companyCussent.CompanyType;
            parms[15].Value = companyCussent.State;
            parms[16].Value = companyCussent.Description;
            parms[17].Value = companyCussent.SubjectInfo;
            parms[18].Value = companyCussent.InvoiceAccountsId;
            parms[19].Value = companyCussent.VoucherAccountsId;
            parms[20].Value = companyCussent.CashAccountsId;
            parms[21].Value = companyCussent.NoVoucherAccountsId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_COMPANYCUSSENT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 更新往来单位
        /// </summary>
        /// <param name="companyCussent">往来单位信息类</param>
        public void Update(CostCussentInfo companyCussent)
        {
            SqlParameter[] parms = GetCompanyCussentParameters();
            parms[0].Value = companyCussent.CompanyId;
            parms[1].Value = companyCussent.CompanyClassId;
            parms[2].Value = companyCussent.CompanyName;
            parms[3].Value = companyCussent.Linkman;
            parms[4].Value = companyCussent.Address;
            parms[5].Value = companyCussent.PostalCode;
            parms[6].Value = companyCussent.Phone;
            parms[7].Value = companyCussent.Mobile;
            parms[8].Value = companyCussent.Fax;
            parms[9].Value = companyCussent.WebSite;
            parms[10].Value = companyCussent.Email;
            parms[11].Value = companyCussent.BankAccounts;
            parms[12].Value = companyCussent.AccountNumber;
            parms[13].Value = companyCussent.DateCreated;
            parms[14].Value = companyCussent.CompanyType;
            parms[15].Value = companyCussent.State;
            parms[16].Value = companyCussent.Description;
            parms[17].Value = companyCussent.SubjectInfo;
            parms[18].Value = companyCussent.InvoiceAccountsId;
            parms[19].Value = companyCussent.VoucherAccountsId;
            parms[20].Value = companyCussent.CashAccountsId;
            parms[21].Value = companyCussent.NoVoucherAccountsId;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_COMPANYCUSSENT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除往来单位信息
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        public void Delete(Guid companyId)
        {
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };

            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_COMPANYCUSSENT, parm);
        }

        /// <summary>
        /// 获取往来单位信息类
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <returns></returns>
        public CostCussentInfo GetCompanyCussent(Guid companyId)
        {
            CostCussentInfo companyCussent;
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT, parm))
            {
                if (rdr.Read())
                {
                    companyCussent = new CostCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                        rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9), rdr.GetString(10), rdr.GetString(11), rdr.GetString(12),
                        rdr.GetDateTime(13), rdr.GetInt32(14), rdr.GetInt32(15), rdr.GetString(16), rdr[17] == DBNull.Value ? "" : rdr.GetString(17),
                        rdr[18] == DBNull.Value ? Guid.Empty : rdr.GetGuid(18), rdr[19] == DBNull.Value ? Guid.Empty : rdr.GetGuid(19), rdr[20] == DBNull.Value ? Guid.Empty : rdr.GetGuid(20), rdr[21] == DBNull.Value ? Guid.Empty : rdr.GetGuid(21));
                }
                else
                {
                    companyCussent = new CostCussentInfo();
                }
            }
            return companyCussent;
        }

        /// <summary>
        /// 获取往来单位信息列表
        /// </summary>
        /// <returns></returns>
        public IList<CostCussentInfo> GetCompanyCussentList()
        {
            IList<CostCussentInfo> companyCussentList = new List<CostCussentInfo>();

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_LIST, null))
            {
                while (rdr.Read())
                {
                    var companyCussent = new CostCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                        rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9), rdr.GetString(10), rdr.GetString(11), rdr.GetString(12),
                        rdr.GetDateTime(13), rdr.GetInt32(14), rdr.GetInt32(15), rdr.GetString(16), rdr[17] == DBNull.Value ? "" : rdr.GetString(17),
                        rdr[18] == DBNull.Value ? Guid.Empty : rdr.GetGuid(18), rdr[19] == DBNull.Value ? Guid.Empty : rdr.GetGuid(19), rdr[20] == DBNull.Value ? Guid.Empty : rdr.GetGuid(20), rdr[21] == DBNull.Value ? Guid.Empty : rdr.GetGuid(21));
                    companyCussentList.Add(companyCussent);
                }
            }
            return companyCussentList;
        }

        /// <summary>
        /// 获取指定类型的往来单位信息列表
        /// </summary>
        /// <returns></returns>
        public IList<CostCussentInfo> GetCompanyCussentList(CompanyType companyType)
        {
            IList<CostCussentInfo> companyCussentList = new List<CostCussentInfo>();
            var parm = new SqlParameter(PARM_COMPANYTYPE, SqlDbType.Int) { Value = companyType };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPE_LIST, parm))
            {
                while (rdr.Read())
                {
                    var companyCussent = new CostCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                        rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9), rdr.GetString(10), rdr.GetString(11), rdr.GetString(12),
                        rdr.GetDateTime(13), rdr.GetInt32(14), rdr.GetInt32(15), rdr.GetString(16), rdr[17] == DBNull.Value ? "" : rdr.GetString(17),
                        rdr[18] == DBNull.Value ? Guid.Empty : rdr.GetGuid(18), rdr[19] == DBNull.Value ? Guid.Empty : rdr.GetGuid(19), rdr[20] == DBNull.Value ? Guid.Empty : rdr.GetGuid(20), rdr[21] == DBNull.Value ? Guid.Empty : rdr.GetGuid(21));
                    companyCussentList.Add(companyCussent);
                }
            }
            return companyCussentList;
        }

        /// <summary>
        /// 获取指定类型，指定状态的往来单位信息列表
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<CostCussentInfo> GetCompanyCussentList(CompanyType companyType, State state)
        {
            IList<CostCussentInfo> companyCussentList = new List<CostCussentInfo>();
            var parms = new[] {
                new SqlParameter(PARM_COMPANYTYPE,  SqlDbType.Int),
                new SqlParameter(PARM_STATE, SqlDbType.Int)
             };
            parms[0].Value = companyType;
            parms[1].Value = state;

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPEANDSTATE_LIST, parms))
            {
                while (rdr.Read())
                {
                    var companyCussent = new CostCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                        rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9), rdr.GetString(10), rdr.GetString(11), rdr.GetString(12),
                        rdr.GetDateTime(13), rdr.GetInt32(14), rdr.GetInt32(15), rdr.GetString(16), rdr[17] == DBNull.Value ? "" : rdr.GetString(17),
                        rdr[18] == DBNull.Value ? Guid.Empty : rdr.GetGuid(18), rdr[19] == DBNull.Value ? Guid.Empty : rdr.GetGuid(19), rdr[20] == DBNull.Value ? Guid.Empty : rdr.GetGuid(20), rdr[21] == DBNull.Value ? Guid.Empty : rdr.GetGuid(21));
                    companyCussentList.Add(companyCussent);
                }
            }
            return companyCussentList;
        }

        /// <summary>
        /// 获取指定状态的往来单位记录
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<CostCussentInfo> GetCompanyCussentList(State state)
        {
            IList<CostCussentInfo> companyCussentList = new List<CostCussentInfo>();
            var parm = new SqlParameter(PARM_STATE, SqlDbType.Int) { Value = state };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_BY_STATE_LIST, parm))
            {
                while (rdr.Read())
                {
                    var companyCussent = new CostCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                        rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9), rdr.GetString(10), rdr.GetString(11), rdr.GetString(12),
                        rdr.GetDateTime(13), rdr.GetInt32(14), rdr.GetInt32(15), rdr.GetString(16), rdr[17] == DBNull.Value ? "" : rdr.GetString(17),
                        rdr[18] == DBNull.Value ? Guid.Empty : rdr.GetGuid(18), rdr[19] == DBNull.Value ? Guid.Empty : rdr.GetGuid(19), rdr[20] == DBNull.Value ? Guid.Empty : rdr.GetGuid(20), rdr[21] == DBNull.Value ? Guid.Empty : rdr.GetGuid(21));
                    companyCussentList.Add(companyCussent);
                }

            }
            return companyCussentList;
        }

        /// <summary>
        /// 获取指定分类号的往来单位列表
        /// </summary>
        /// <param name="companyClassId">往来单位分类编号</param>
        /// <returns></returns>
        public IList<CostCussentInfo> GetCompanyCussentList(Guid companyClassId)
        {
            IList<CostCussentInfo> companyCussentList = new List<CostCussentInfo>();
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = companyClassId };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_BY_COMPANYCLASS_LIST_ALL, parm))
            {
                while (rdr.Read())
                {
                    var companyCussent = new CostCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                        rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9), rdr.GetString(10), rdr.GetString(11), rdr.GetString(12),
                        rdr.GetDateTime(13), rdr.GetInt32(14), rdr.GetInt32(15), rdr.GetString(16), rdr[17] == DBNull.Value ? "" : rdr.GetString(17),
                        rdr[18] == DBNull.Value ? Guid.Empty : rdr.GetGuid(18), rdr[19] == DBNull.Value ? Guid.Empty : rdr.GetGuid(19), rdr[20] == DBNull.Value ? Guid.Empty : rdr.GetGuid(20), rdr[21] == DBNull.Value ? Guid.Empty : rdr.GetGuid(21));
                    companyCussentList.Add(companyCussent);
                }

            }
            return companyCussentList;
        }

        /// <summary>
        /// 获取会员总帐
        /// </summary>
        /// <returns></returns>
        public CostCussentInfo GetMemberGeneralLedger()
        {
            IList<CostCussentInfo> companyCussentList = GetCompanyCussentList(CompanyType.MemberGeneralLedger);
            if (companyCussentList.Count > 0)
                return companyCussentList[0];
            return new CostCussentInfo();
        }

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        public double GetNonceReckoningTotalled(Guid companyId)
        {
            double totalled = 0;
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };

            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_TOTALLED, parm);
            if (obj != DBNull.Value)
                totalled = Convert.ToDouble(obj);
            return totalled;
        }

        /// <summary>
        /// 判断是否包含该公司
        /// </summary>
        /// <param name="companyName">公司名称</param>
        /// <returns></returns>
        public bool IsBeing(string companyName)
        {
            bool being;
            var parm = new SqlParameter(PARM_COMPANYNAME, SqlDbType.VarChar, 128) { Value = companyName };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_ISBEING, parm))
            {
                being = rdr.Read();
            }
            return being;
        }

        /// <summary>
        /// 判断是否为快递公司往来帐
        /// </summary>
        /// <param name="companyId">公司编号</param>
        /// <returns></returns>
        public bool IsExpress(Guid companyId)
        {
            bool isExpress;
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_ISEXPRESS, parm))
            {
                isExpress = rdr.Read();
            }
            return isExpress;
        }

        /// <summary>
        /// 判断是否为会员总帐号
        /// </summary>
        /// <param name="companyId">公司编号</param>
        /// <returns></returns>
        public bool IsMemberGeneralLedger(Guid companyId)
        {
            bool isMember = false;
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };

            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_ISMEMBER, parm);
            if (obj != DBNull.Value)
                isMember = (CompanyType)obj == CompanyType.MemberGeneralLedger;

            return isMember;
        }

        /// <summary>
        /// 会员总帐是否被使用
        /// </summary>
        /// <returns></returns>
        public bool IsUseMemberGeneralLedger()
        {
            IList<CostCussentInfo> companyCussentList = GetCompanyCussentList(CompanyType.MemberGeneralLedger);
            if (companyCussentList.Count > 0)
                return true;
            return false;
        }

        private static SqlParameter[] GetCompanyCussentParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier),
				new SqlParameter(PARM_COMPANYNAME, SqlDbType.VarChar, 128),
                new SqlParameter(PARM_COMPANYLINKMAN, SqlDbType.VarChar, 64),
                new SqlParameter(PARM_COMPANYADDRESS, SqlDbType.VarChar, 128),
                new SqlParameter(PARM_POSTALCODE, SqlDbType.VarChar, 32),
				new SqlParameter(PARM_PHONE, SqlDbType.VarChar, 32),
                new SqlParameter(PARM_MOBILE, SqlDbType.VarChar, 32),
                new SqlParameter(PARM_FAX, SqlDbType.VarChar, 32),
				new SqlParameter(PARM_WEBSITE, SqlDbType.VarChar, 128),
                new SqlParameter(PARM_EMAIL, SqlDbType.VarChar, 128),
                new SqlParameter(PARM_BANKACCOUNTS, SqlDbType.VarChar, 128),
				new SqlParameter(PARM_ACCOUNTNUMBER, SqlDbType.VarChar, 32),
                new SqlParameter(PARM_DATECREATED, SqlDbType.DateTime),
                new SqlParameter(PARM_COMPANYTYPE, SqlDbType.Int),
                new SqlParameter(PARM_STATE, SqlDbType.Int),
                new SqlParameter(PARM_DESCRIPTION, SqlDbType.VarChar, 512),
                new SqlParameter(PARM_SUBJECTINFO,SqlDbType.Text),
                new SqlParameter(PARM_INVOICE_ACCOUNTS_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_VOUCHER_ACCOUNTS_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_CASH_ACCOUNTS_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_NO_VOUCHER_ACCOUNTS_ID,SqlDbType.UniqueIdentifier)
            };
            return parms;
        }

        #region 费用单位开放权限

        /// <summary>增加费用单位开放权限
        /// </summary>
        /// <param name="companyCussentId">费用单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        public void AddCussionPersion(Guid companyCussentId, Guid filialeId, Guid branchId)
        {
            const string SQLCOST_PERSION_ADD = @"insert CostPermission(CompanyId,FilialeId,BranchId) values(@CompanyId,@FilialeId,@BranchId)";
            var parms = new[]
                                       {
                                           new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier),
                                           new SqlParameter(PARM_FILIALE_ID, SqlDbType.UniqueIdentifier),
                                           new SqlParameter(PARM_BRANCH_ID, SqlDbType.UniqueIdentifier)
                                       };
            parms[0].Value = companyCussentId;
            parms[1].Value = filialeId;
            parms[2].Value = branchId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQLCOST_PERSION_ADD, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>删除费用单位开放权限
        /// </summary>
        /// <param name="companyCussentId">费用单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        public void DeleteCussionPersion(Guid companyCussentId, Guid filialeId, Guid branchId)
        {
            const string SQLCOST_PERSION_DELETE = @"  delete CostPermission where  CompanyId=@CompanyId and FilialeId=@FilialeId and BranchId=@BranchId";
            var parms = new[]
                                       {
                                           new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier),
                                           new SqlParameter(PARM_FILIALE_ID, SqlDbType.UniqueIdentifier),
                                           new SqlParameter(PARM_BRANCH_ID, SqlDbType.UniqueIdentifier)
                                       };
            parms[0].Value = companyCussentId;
            parms[1].Value = filialeId;
            parms[2].Value = branchId;
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQLCOST_PERSION_DELETE, parms);
        }

        /// <summary>获取费用单位开放权限列表
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public IEnumerable<CostPermissionInfo> GetCostPermissionList(Guid filialeId, Guid branchId, Guid companyId)
        {
            string sql = @"
SELECT CP.FilialeId,CP.BranchId,CC.CompanyId,CC.CompanyName
FROM CostCompany CC 
INNER JOIN CostPermission CP ON CC.CompanyId=CP.CompanyId
WHERE 1=1
";
            var parms = new[]
                                       {
                                           new Parameter("@CompanyId", companyId),
                                           new Parameter(PARM_FILIALE_ID, filialeId),
                                           new Parameter(PARM_BRANCH_ID, branchId)
                                       };

            if (filialeId != Guid.Empty)
            {
                sql += " AND CP.FilialeId=@FilialeId";
            }
            if (branchId != Guid.Empty)
            {
                sql += "  AND CP.BranchId=@BranchId";
            }
            if (companyId != Guid.Empty)
            {
                sql += " and CC.CompanyId=@CompanyId";
            }
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CostPermissionInfo>(true, sql, parms);
            }

        }

        /// <summary>
        /// 根据单位ID删除该单位相关的所有权限
        /// Add by Liucaijun at 2010-january-07th
        /// 删除单位时候使用，删除该单位相关的所有权限
        /// </summary>
        /// <param name="companyCussentId">单位ID</param>
        public void DeleteCussionPersion(Guid companyCussentId)
        {
            const string SQLCOST_PERSION_DELETE = @"DELETE CostPermission WHERE CompanyId=@CompanyId";
            var parm = new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier) { Value = companyCussentId };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQLCOST_PERSION_DELETE, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #endregion

        /// <summary>根据费用分类ID获取具有相应权限的费用单位  ADD 2014-12-09  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <param name="companyClassId">费用分类ID</param>
        /// <returns></returns>
        public IList<CostCussentInfo> GetPermissionCompanyCussentList(Guid filialeId, Guid branchId, Guid companyClassId)
        {
            const string SQL = @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,
[Address],PostalCode,Phone,Mobile,Fax,WebSite,Email,
BankAccounts,AccountNumber,DateCreated,CompanyType,
[State],[Description],SubjectInfo,InvoiceAccountsId,
VoucherAccountsId,CashAccountsId,NoVoucherAccountsId 
FROM CostCompany 
WHERE CompanyClassId=@CompanyClassId
AND CompanyId IN
(
	SELECT CompanyId FROM CostPermission WHERE FilialeId=@FilialeId
	AND BranchId=@BranchId
)";
            var parms = new[]
                            {
                                new SqlParameter("@CompanyClassId", SqlDbType.UniqueIdentifier) {Value = companyClassId}
                                ,
                                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier) {Value = filialeId},
                                new SqlParameter("@BranchId", SqlDbType.UniqueIdentifier) {Value = branchId}
                            };
            IList<CostCussentInfo> companyCussentList = new List<CostCussentInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    var companyCussent = new CostCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                             rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                                                             rdr.GetString(6), rdr.GetString(7), rdr.GetString(8),
                                                             rdr.GetString(9), rdr.GetString(10), rdr.GetString(11),
                                                             rdr.GetString(12),
                                                             rdr.GetDateTime(13), rdr.GetInt32(14), rdr.GetInt32(15),
                                                             rdr.GetString(16),
                                                             rdr[17] == DBNull.Value ? "" : rdr.GetString(17),
                                                             rdr[18] == DBNull.Value ? Guid.Empty : rdr.GetGuid(18),
                                                             rdr[19] == DBNull.Value ? Guid.Empty : rdr.GetGuid(19),
                                                             rdr[20] == DBNull.Value ? Guid.Empty : rdr.GetGuid(20),
                                                             rdr[21] == DBNull.Value ? Guid.Empty : rdr.GetGuid(21));
                    companyCussentList.Add(companyCussent);
                }
                return companyCussentList;
            }
        }

        /// <summary>获取费用分类绑定的公司资金帐号   ADD  2015-01-20  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="costCompanyId">费用分类ID</param>
        /// <returns></returns>
        public CostCompanyBindingBankAccountsInfo GetCostCompanyBindingBankAccountsInfo(Guid filialeId, Guid costCompanyId)
        {
            const string SQL = @"SELECT [CostCompanyId]
      ,[FilialeId]
      ,[InvoiceAccountsId]
      ,[VoucherAccountsId]
      ,[CashAccountsId]
      ,[NoVoucherAccountsId]
  FROM [CostCompanyBindingBankAccounts]
  
  WHERE FilialeId=@FilialeId AND CostCompanyId=@CostCompanyId";
            using (var db = DatabaseFactory.Create())
            {
                return db.Single<CostCompanyBindingBankAccountsInfo>(true, SQL, new Parameter("FilialeId", filialeId), new Parameter("CostCompanyId", costCompanyId));
            }
        }

        /// <summary>更新费用分类绑定的公司资金帐号（含新增）   ADD  2015-01-20  陈重文
        /// </summary>
        /// <param name="info"> 费用分类绑定公司资金账户模型</param>
        /// <returns></returns>
        public Boolean InsertOrUpdateCostCompanyBindingBankAccountsInfo(CostCompanyBindingBankAccountsInfo info)
        {
            const string SQL = @"
IF EXISTS(SELECT CostCompanyId FROM [CostCompanyBindingBankAccounts]  WHERE CostCompanyId=@CostCompanyId AND FilialeId=@FilialeId)
BEGIN 
      UPDATE [CostCompanyBindingBankAccounts]
			SET 
			   [InvoiceAccountsId] =@InvoiceAccountsId
			  ,[VoucherAccountsId] = @VoucherAccountsId
			  ,[CashAccountsId] = @CashAccountsId
			  ,[NoVoucherAccountsId] = @NoVoucherAccountsId
			WHERE FilialeId=@FilialeId AND CostCompanyId=@CostCompanyId
 END 
 ELSE BEGIN
			INSERT INTO [CostCompanyBindingBankAccounts]
			   ([CostCompanyId]
			   ,[FilialeId]
			   ,[InvoiceAccountsId]
			   ,[VoucherAccountsId]
			   ,[CashAccountsId]
			   ,[NoVoucherAccountsId])
			 VALUES
			   (@CostCompanyId
			   ,@FilialeId
			   ,@InvoiceAccountsId
			   ,@VoucherAccountsId
			   ,@CashAccountsId
			   ,@NoVoucherAccountsId)
END    ";
            var parms = new[]{
                new SqlParameter("@CostCompanyId", SqlDbType.UniqueIdentifier){Value = info.CostCompanyId},
                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier){Value = info.FilialeId},
                new SqlParameter("@InvoiceAccountsId", SqlDbType.UniqueIdentifier){Value = info.InvoiceAccountsId},
                new SqlParameter("@VoucherAccountsId", SqlDbType.UniqueIdentifier){Value = info.VoucherAccountsId},
                new SqlParameter("@CashAccountsId", SqlDbType.UniqueIdentifier){Value = info.CashAccountsId},
                new SqlParameter("@NoVoucherAccountsId", SqlDbType.UniqueIdentifier){Value = info.NoVoucherAccountsId}
            };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
