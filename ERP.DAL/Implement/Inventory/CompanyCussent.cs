using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;
using ERP.SAL.WMS;
using Keede.DAL.Helper;
using ERP.Environment;
using Keede.DAL.RWSplitting;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>往来单位数据层
    /// </summary>
    public partial class CompanyCussent : ICompanyCussent
    {
        public CompanyCussent(Environment.GlobalConfig.DB.FromType fromType) { }

        private const string SQL_INSERT_COMPANYCUSSENT =
            @"INSERT INTO lmShop_CompanyCussent (CompanyId,CompanyClassId,CompanyName,Linkman,[Address],PostalCode,
            Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,IsNeedInvoices,
            DateCreated,CompanyType,[State],[Description],SubjectInfo,Information,PaymentDays,RelevanceFilialeId,SalesScope,DeliverType) VALUES(@CompanyId,@CompanyClassId,@CompanyName,
@Linkman,@Address,@PostalCode,@Phone,@Mobile,@Fax,@WebSite,@Email,@BankAccounts,@AccountNumber,@IsNeedInvoices,@DateCreated,@CompanyType,@State,@Description,@SubjectInfo,@Information,@PaymentDays,@RelevanceFilialeId,@SalesScope,@DeliverType);";
        private const string SQL_UPDATE_COMPANYCUSSENT =
            @"UPDATE lmShop_CompanyCussent SET CompanyClassId=@CompanyClassId,CompanyName=@CompanyName,Linkman=@Linkman,
            Address=@Address,PostalCode=@PostalCode,Phone=@Phone,Mobile=@Mobile,Fax=@Fax,WebSite=@WebSite,Email=@Email,
            BankAccounts=@BankAccounts,AccountNumber=@AccountNumber,DateCreated=@DateCreated,CompanyType=@CompanyType,            State=@State,Description=@Description,SubjectInfo=@SubjectInfo,IsNeedInvoices=@IsneedInvoices,Information=@Information,PaymentDays=@PaymentDays,RelevanceFilialeId=@RelevanceFilialeId,SalesScope=@SalesScope,DeliverType=@DeliverType WHERE CompanyId=@CompanyId;";
        private const string SQL_DELETE_COMPANYCUSSENT = @"DELETE FROM lmShop_Reckoning WHERE ThirdCompanyID=@CompanyId;DELETE FROM lmShop_CompanyCussent WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENT =
            @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,
            BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,
            IsNeedInvoices,Information,OwnBankAccountName,OwnBankAccountID,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENT_FILIALEID =
            @"SELECT Top 1 CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,
            BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,
            IsNeedInvoices,Information,OwnBankAccountName,OwnBankAccountID,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent WHERE RelevanceFilialeId=@RelevanceFilialeId;";

        private const string SQL_SELECT_COMPANYCUSSENT_LIST =
            @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,
            DateCreated,CompanyType,State,Description,SubjectInfo,IsNeedInvoices,Information,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType 
            FROM lmShop_CompanyCussent ORDER BY DateCreated ASC;";
        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPE_LIST =
            @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,
            DateCreated,CompanyType,State,Description,SubjectInfo,Information,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent WHERE State=1 AND CompanyType=@CompanyType;";
        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPEANDSTATE_LIST =
            @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,
            DateCreated,CompanyType,State,Description,SubjectInfo,Information,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent WHERE CompanyType=@CompanyType AND State=@State;";
        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPESANDSTATE_LIST =
            @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,
            DateCreated,CompanyType,State,Description,SubjectInfo,Information,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent WHERE {0} AND State=@State;";
        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYCLASS_LIST =
            @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,
            WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,IsNeedInvoices,Information,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent WHERE State=1 AND CompanyClassId=@CompanyClassId;";

        private const string SQL_SELECT_COMPANYCUSSENT_BY_COMPANYCLASS_LISTBYPERSION = @" SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,
 Fax,WebSite,Email,BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description
 ,SubjectInfo,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent a right join lmshop_PersonnelCostCussent b on a.CompanyId=b.CostCussentId
 WHERE State=1 ANDCompanyClassId=@CompanyClassId and b.PersonnelId=@PersonnelId ;";
        private const string SQL_SELECT_COMPANYCUSSENT_BY_STATE_LIST = @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,
            DateCreated,CompanyType,State,Description,SubjectInfo,Information,IsNeedInvoices,PaymentDays,Complete,Expire,RelevanceFilialeId,SalesScope,DeliverType FROM lmShop_CompanyCussent WHERE State=@State;";
        private const string SQL_SELECT_COMPANYCUSSENT_TOTALLED = "SELECT NonceBalance FROM lmShop_CompanyBalance WHERE CompanyId=@CompanyId ";
        private const string SQL_SELECT_COMPANYCUSSENTDETAIL_TOTALLED = "SELECT NonceBalance FROM lmShop_CompanyBalanceDetail WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId ";
        private const string SQL_SELECT_COMPANYCUSSENT_ISBEING = @"SELECT COUNT(0) FROM lmShop_CompanyCussent WHERE CompanyName=@CompanyName;";
        private const string SQL_SELECT_COMPANYCUSSENT_ISEXPRESS = @"SELECT ExpressId, DeliverId, CompanyId, ExpressName, OrderIndex, Homepage, Linkman, Phone, ExpressBill, Memo, FilialeID, WarehouseID, Code, CodeId, BackDisplayName, ExpressSurfaceType, ExpressProductType, ExpressServiceCodeType FROM lmShop_Express WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENT_ISMEMBER = "SELECT CompanyType FROM lmShop_CompanyCussent WHERE CompanyId=@CompanyId;";
        private const string SQL_SELECT_COMPANYCUSSENTEXTENDINFO_BY_COMPANYID = "select subjectInfo from lmShop_CompanyCussent where CompanyId=@CompanyId;";

        private const string SQL_SELECT_EXISTS_FILIALEID = @"SELECT RelevanceFilialeId FROM lmShop_CompanyCussent WHERE RelevanceFilialeId<>'00000000-0000-0000-0000-000000000000' GROUP BY RelevanceFilialeId ";

        /// <summary>
        /// 2013.7.8 > 阮剑锋 > 转变字段脚本text替换varchar(max)
        /// </summary>
        private const string SQL_UPDAE_COMPANYCUSSENTEXTENDINFO_BY_COMPANYID = "UPDATE lmShop_CompanyCussent SET subjectInfo=@subjectInfo+'\r\n'+Convert(varchar(max),isnull(subjectInfo,''))   where CompanyId=@CompanyId";

        /// <summary>
        /// 2013.7.8 > 阮剑锋 > 转变字段脚本text替换varchar(max)
        /// </summary>
        private const string SQL_UPDAE_COMPANYCUSSENTDISCOUNTMEMO_BY_COMPANYID = "UPDATE lmShop_CompanyCussent SET DiscountMemo=@DiscountMemo+'\r\n'+Convert(varchar(max),isnull(DiscountMemo,''))   where CompanyId=@CompanyId";

        /// <summary>
        /// 获取往来单位折扣说明
        /// Add By liucaijun at 2011-August-16th
        /// </summary>
        private const string SQL_SELECT_COMPANYCUSSENTDISCOUNTMEMO_BY_COMPANYID = "SELECT DiscountMemo FROM lmShop_CompanyCussent WHERE CompanyId=@CompanyId;";

        private const string PARM_COMPANYID = "@CompanyId";
        private const string PARM_FILIALEID = "@FilialeId";
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
        private const string PARM_ISNEEDINVOICES = "@IsneedInvoices";
        private const string PARM_RELEVANCEFILIALEID = "@RelevanceFilialeId";
        private const string PARM_SALESSCOPE = "@SalesScope";
        private const string PARM_DELIVERTYPE = "@DeliverType";

        /// <summary>
        /// 折扣说明
        /// </summary>
        private const string PARM_DISCOUNTMEMO = "@DiscountMemo";
        /// <summary>
        /// 单位资料
        /// </summary>
        private const string PARM_INFORMATION = "@Information";

        /// <summary>
        /// 或者往来单位回扣流水信息
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public string GetCussentExtendInfo(Guid companyId)
        {
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };
            string retValue = string.Empty;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENTEXTENDINFO_BY_COMPANYID, parm);
            if (obj != DBNull.Value && obj != null)
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
        public void UpDatetCussentExtendInfo(Guid companyId, string extend)
        {
            var parms = new[]{
                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_SUBJECTINFO,SqlDbType.VarChar,512)
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

        /// <summary>
        /// 添加往来单位信息
        /// </summary>
        /// <param name="companyCussent">往来单位信息类</param>
        public void Insert(CompanyCussentInfo companyCussent)
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
            parms[18].Value = companyCussent.IsNeedInvoices;
            parms[19].Value = companyCussent.Information;
            parms[20].Value = companyCussent.PaymentDays;
            parms[21].Value = companyCussent.RelevanceFilialeId;
            parms[22].Value = companyCussent.SalesScope;
            parms[23].Value = companyCussent.DeliverType;
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
        public void Update(CompanyCussentInfo companyCussent)
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
            //parms[18].Value = companyCussent.OwnBankAccountName;
            //parms[19].Value = companyCussent.OwnBankAccountID;
            parms[18].Value = companyCussent.IsNeedInvoices;
            parms[19].Value = companyCussent.Information;
            parms[20].Value = companyCussent.PaymentDays;
            parms[21].Value = companyCussent.RelevanceFilialeId;
            parms[22].Value = companyCussent.SalesScope;
            parms[23].Value = companyCussent.DeliverType;
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
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL_DELETE_COMPANYCUSSENT, parm);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// 获取往来单位信息类
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <returns></returns>
        public CompanyCussentInfo GetCompanyCussent(Guid companyId)
        {
            CompanyCussentInfo companyCussent;
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT, parm))
            {
                if (rdr.Read())
                {
                    companyCussent = new CompanyCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                            rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                                                            rdr.GetString(6), rdr.GetString(7), rdr.GetString(8),
                                                            rdr.GetString(9), rdr.GetString(10), rdr.GetString(11),
                                                            rdr.GetString(12), rdr.GetDateTime(13), rdr.GetInt32(14),
                                                            rdr.GetInt32(15), rdr.GetString(16),
                                                            rdr[17] == DBNull.Value ? "" : rdr.GetString(17), new Guid(rdr["RelevanceFilialeId"].ToString()), rdr["SalesScope"] == DBNull.Value ? 0 : int.Parse(rdr["SalesScope"].ToString()), rdr["DeliverType"] == DBNull.Value ? 0 : int.Parse(rdr["DeliverType"].ToString()))
                    {
                        IsNeedInvoices = !rdr.IsDBNull(18) && rdr.GetBoolean(18),
                        Information = rdr.IsDBNull(19) ? "" : rdr.GetString(19),
                        PaymentDays = Convert.ToInt32(rdr["PaymentDays"]),
                        Complete = Convert.ToInt32(rdr["Complete"]),
                        Expire = rdr["Expire"].ToString()
                    };
                }
                else
                {
                    companyCussent = null;
                }
            }
            return companyCussent;
        }

        /// <summary>
        /// 获取往来单位信息类
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="filialeId">绑定公司Id</param>
        /// <returns></returns>
        /// zal 2016-03-16
        public CompanyCussentInfo GetCompanyCussentInfoByCompanyIdAndFilialeId(Guid companyId, Guid filialeId)
        {
            string sql = @"
            SELECT A.CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,B.WebSite,Email,
            A.BankAccounts,AccountNumber,DateCreated,CompanyType,State,Description,SubjectInfo,
            IsNeedInvoices,Information,OwnBankAccountName,OwnBankAccountID,PaymentDays,Complete,Expire,RelevanceFilialeId 
            FROM lmShop_CompanyCussent A
            inner join CompanyBankAccountBind B on A.CompanyId=B.CompanyId
            WHERE A.CompanyId=@CompanyId and B.FilialeId=@FilialeId
            ";
            SqlParameter[] paras = new[]
            {
                new SqlParameter("@CompanyId",companyId),
                new SqlParameter("@FilialeId",filialeId)
            };
            CompanyCussentInfo companyCussent;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras))
            {
                if (rdr.Read())
                {
                    companyCussent = new CompanyCussentInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2),
                                                            rdr.GetString(3), rdr.GetString(4), rdr.GetString(5),
                                                            rdr.GetString(6), rdr.GetString(7), rdr.GetString(8),
                                                            rdr.GetString(9), rdr.GetString(10), rdr.GetString(11),
                                                            rdr.GetString(12), rdr.GetDateTime(13), rdr.GetInt32(14),
                                                            rdr.GetInt32(15), rdr.GetString(16),
                                                            rdr[17] == DBNull.Value ? "" : rdr.GetString(17), new Guid(rdr["RelevanceFilialeId"].ToString()), rdr["SalesScope"] == DBNull.Value ? 0 : int.Parse(rdr["SalesScope"].ToString()), rdr["DeliverType"] == DBNull.Value ? 0 : int.Parse(rdr["DeliverType"].ToString()))
                    {
                        IsNeedInvoices = !rdr.IsDBNull(18) && rdr.GetBoolean(18),
                        Information = rdr.IsDBNull(19) ? "" : rdr.GetString(19),
                        PaymentDays = Convert.ToInt32(rdr["PaymentDays"]),
                        Complete = Convert.ToInt32(rdr["Complete"]),
                        Expire = rdr["Expire"].ToString()
                    };
                }
                else
                {
                    companyCussent = null;
                }
            }
            return companyCussent;
        }

        /// <summary>
        /// 获取往来单位信息列表
        /// </summary>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentList()
        {
            return GetCompanyCussentInfos(SQL_SELECT_COMPANYCUSSENT_LIST);
        }

        /// <summary>
        /// 获取指定类型的往来单位信息列表
        /// </summary>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType companyType)
        {
            var parm = new Parameter(PARM_COMPANYTYPE, companyType);
            return GetCompanyCussentInfos(SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPE_LIST, parm);
        }

        /// <summary>
        /// 通过往来单位名称获取往来单位列表  模糊搜索
        /// </summary>
        /// <param name="companyName">往来单位分类编号</param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentListByCompanyName(string companyName)
        {
            string sql = @"SELECT CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,Fax,WebSite,Email,BankAccounts,AccountNumber,
            DateCreated,CompanyType,State,Description,SubjectInfo,Information,PaymentDays,Complete,Expire,RelevanceFilialeId FROM lmShop_CompanyCussent ";
            if (!string.IsNullOrEmpty(companyName))
            {
                sql += string.Format(" WHERE CompanyName Like '%{0}%'", companyName);
            }
            return GetCompanyCussentInfos(sql);
        }

        /// <summary>
        /// 获取指定类型，指定状态的往来单位信息列表
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType[] companyType, State state)
        {

            var strCompanyType = new StringBuilder();
            strCompanyType.Append(" CompanyType IN ");
            strCompanyType.Append("(");
            int i = 0;
            foreach (CompanyType type in companyType)
            {
                if (i != 0)
                {
                    strCompanyType.Append(",");
                }
                strCompanyType.Append("'").Append(((int)type).ToString(CultureInfo.InvariantCulture)).Append("'");
                i = 1;
            }
            strCompanyType.Append(")");
            var parms = new[] { new Parameter(PARM_STATE, state) };
            return GetCompanyCussentInfos(String.Format(SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPESANDSTATE_LIST, strCompanyType), parms);
        }

        /// <summary>
        /// 获取指定类型，指定状态的往来单位信息列表
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType companyType, State state)
        {
            var parms = new[] {
                new Parameter(PARM_COMPANYTYPE,  companyType),
                new Parameter(PARM_STATE, state)
             };
            return GetCompanyCussentInfos(SQL_SELECT_COMPANYCUSSENT_BY_COMPANYTYPEANDSTATE_LIST, parms);
        }

        /// <summary>
        /// 获取指定状态的往来单位记录
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentList(State state)
        {
            var parm = new Parameter(PARM_STATE, state);
            return GetCompanyCussentInfos(SQL_SELECT_COMPANYCUSSENT_BY_STATE_LIST, parm);
        }

        /// <summary>
        /// 获取指定分类号的往来单位列表
        /// </summary>
        /// <param name="companyClassId">往来单位分类编号</param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentList(Guid companyClassId)
        {
            var parm = new Parameter(PARM_COMPANYCLASSID, companyClassId);
            return GetCompanyCussentInfos(SQL_SELECT_COMPANYCUSSENT_BY_COMPANYCLASS_LIST, parm);
        }

        public IList<Guid> GetCompanyIdList(Guid companyClassId)
        {
            var strb = new StringBuilder();
            strb.Append(@"
SELECT CompanyId FROM lmShop_CompanyCussent 
WHERE CompanyClassId IN (
	SELECT CompanyClassId FROM lmShop_CompanyClass 
	WHERE CompanyClassId=@CompanyClassId OR ParentCompanyClassId=@CompanyClassId
)");
            var ids = new List<Guid>();
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = companyClassId };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strb.ToString(), parm))
            {
                if (rdr != null)
                {
                    while (rdr.Read())
                    {
                        ids.Add(rdr["CompanyId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["CompanyId"].ToString()));
                    }
                }
            }
            return ids;
        }
        /// <summary>
        /// 获取指定员工分类号的往来单位列表
        /// </summary>
        /// <param name="companyClassId">往来单位分类编号</param>
        /// <param name="persionID">员工id</param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentListByPersion(Guid companyClassId, Guid persionID)
        {
            var parms = new[] {
                new Parameter(PARM_COMPANYCLASSID,  companyClassId),
                new Parameter("@PersonnelId", persionID)
             };
            return GetCompanyCussentInfos(SQL_SELECT_COMPANYCUSSENT_BY_COMPANYCLASS_LISTBYPERSION, parms);
        }
        /// <summary>
        /// 获取会员总帐
        /// </summary>
        /// <returns></returns>
        public CompanyCussentInfo GetMemberGeneralLedger()
        {
            IList<CompanyCussentInfo> companyCussentList = GetCompanyCussentList(CompanyType.MemberGeneralLedger);
            if (companyCussentList.Count > 0)
                return companyCussentList[0];
            return new CompanyCussentInfo();
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
            if (obj != DBNull.Value && obj != null)
                totalled = Convert.ToDouble(obj);
            return totalled;
        }

        /// <summary>
        /// 获取指定公司现在的应付款数，即相对于本公司的应收款数
        /// </summary>
        /// <param name="companyId">往来公司编号</param>
        /// <param name="filialeId"> </param>
        /// <returns>返回对本公司而言的指定公司应收款</returns>
        public double GetNonceReckoningTotalled(Guid companyId, Guid filialeId)
        {
            double totalled = 0;
            var parms = new[] {
                new SqlParameter(PARM_COMPANYID,  SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_FILIALEID, SqlDbType.UniqueIdentifier)
             };
            parms[0].Value = companyId;
            parms[1].Value = filialeId;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENTDETAIL_TOTALLED, parms);
            if (obj != DBNull.Value && obj != null)
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
            var parm = new SqlParameter(PARM_COMPANYNAME, SqlDbType.VarChar, 128) { Value = companyName };
            var result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENT_ISBEING, parm);
            return result != null && (int)result > 0;
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
            if (obj != DBNull.Value && obj != null)
                isMember = (CompanyType)obj == CompanyType.MemberGeneralLedger;

            return isMember;
        }

        /// <summary>
        /// 会员总帐是否被使用
        /// </summary>
        /// <returns></returns>
        public bool IsUseMemberGeneralLedger()
        {
            IList<CompanyCussentInfo> companyCussentList = GetCompanyCussentList(CompanyType.MemberGeneralLedger);
            return companyCussentList.Count > 0;
        }

        #region[更新往来单位折扣信息]
        /// <summary>
        /// 更新往来单位折扣信息
        /// Add by liucaijun at 2011-August-16th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="discountMemo">折扣信息</param>
        /// <returns></returns>
        public void UpDatetCussentDiscountMemoInfo(Guid companyId, string discountMemo)
        {
            var parms = new[]{
                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_DISCOUNTMEMO,SqlDbType.VarChar,512)
            };
            parms[0].Value = companyId;
            parms[1].Value = discountMemo;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDAE_COMPANYCUSSENTDISCOUNTMEMO_BY_COMPANYID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region[根据往来单位获取折扣信息]
        /// <summary>
        /// 根据往来单位获取折扣信息
        /// Add by liucaijun at 2011-August-16th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public string GetCussentDiscountMemo(Guid companyId)
        {
            var parm = new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier) { Value = companyId };
            string retValue = string.Empty;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCUSSENTDISCOUNTMEMO_BY_COMPANYID, parm);
            if (obj != DBNull.Value && obj != null)
            {
                retValue = obj.ToString();
            }
            return retValue;
        }
        #endregion

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
                new SqlParameter(PARM_ISNEEDINVOICES,SqlDbType.Bit),
                new SqlParameter(PARM_INFORMATION,SqlDbType.VarChar,256),
                new SqlParameter("@PaymentDays",SqlDbType.Int),
                new SqlParameter(PARM_RELEVANCEFILIALEID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_SALESSCOPE,SqlDbType.Int),
                new SqlParameter(PARM_DELIVERTYPE,SqlDbType.Int)
    };
            return parms;
        }

        /// <summary>
        /// 添加往来单位对应我方银行账号信息
        /// </summary>
        public void InsertCompanyBankAccounts(CompanyBankAccountsInfo info)
        {
            const string SQL = @"INSERT INTO [CompanyBankAccounts]([CompanyId],[FilialeId],[BankAccountsId])VALUES(@CompanyId,@FilialeId,@BankAccountsId)";
            var parms = new[]
                            {
                                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@BankAccountsId", SqlDbType.UniqueIdentifier)
                            };
            parms[0].Value = info.CompanyId;
            parms[1].Value = info.FilialeId;
            parms[2].Value = info.BankAccountsId;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 更新往来单位对应我方银行账号信息
        /// </summary>
        public void UpdateCompanyBankAccounts(CompanyBankAccountsInfo info)
        {
            const string SQL = @"UPDATE [CompanyBankAccounts] SET BankAccountsId=@BankAccountsId WHERE CompanyId=@CompanyId and FilialeId=@FilialeId";
            var parms = new[]
                            {
                                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@BankAccountsId", SqlDbType.UniqueIdentifier)
                            };
            parms[0].Value = info.CompanyId;
            parms[1].Value = info.FilialeId;
            parms[2].Value = info.BankAccountsId;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 获取所有往来单位对应我方银行账号信息
        /// </summary>
        /// <returns></returns>
        public IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList()
        {
            const string SQL = @"SELECT CompanyId,FilialeId,CBA.BankAccountsId,BA.BankName FROM [CompanyBankAccounts] CBA 
 INNER JOIN lmshop_BankAccounts BA ON CBA.BankAccountsId=BA.BankAccountsId";
            IList<CompanyBankAccountsInfo> companyBankAccountList = new List<CompanyBankAccountsInfo>();

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, null))
            {
                while (rdr.Read())
                {
                    var companyBankAccounts = new CompanyBankAccountsInfo(rdr.GetGuid(0), rdr.GetGuid(1),
                                                                              rdr.GetGuid(2))
                    { BankName = rdr.GetString(3) };
                    companyBankAccountList.Add(companyBankAccounts);
                }
            }
            return companyBankAccountList;
        }

        public IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList(Guid companyId)
        {
            IList<CompanyBankAccountsInfo> list = new List<CompanyBankAccountsInfo>();

            string sql = @"SELECT CompanyId,FilialeId,CBA.BankAccountsId,BA.BankName FROM [CompanyBankAccounts] CBA 
 INNER JOIN lmshop_BankAccounts BA ON CBA.BankAccountsId=BA.BankAccountsId WHERE CompanyId='" + companyId + "'";
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                if (rdr != null)
                {
                    while (rdr.Read())
                    {
                        var info = new CompanyBankAccountsInfo
                        {
                            CompanyId = rdr["CompanyId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["CompanyId"].ToString()),
                            FilialeId = rdr["FilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["FilialeId"].ToString()),
                            BankAccountsId = rdr["BankAccountsId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["BankAccountsId"].ToString()),
                            BankName = rdr["BankName"] == DBNull.Value ? string.Empty : rdr["BankName"].ToString(),
                        };
                        list.Add(info);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 删除指定往来单位对应我方银行账号信息
        /// </summary>
        /// <param name="info">往来单位对应我方银行账号信息 </param>
        public void DelCompanyBankAccounts(CompanyBankAccountsInfo info)
        {
            const string SQL = @"Delete From [CompanyBankAccounts] WHERE CompanyId=@CompanyId and FilialeId=@FilialeId";
            var parms = new[]
                            {
                                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier)
                            };
            parms[0].Value = info.CompanyId;
            parms[1].Value = info.FilialeId;
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>根据公司ID找出对应的绑定的往来单位
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyCussentByFilialeId(Guid filialeId)
        {
            const string SQL = @"SELECT b.CompanyId,CompanyClassId,CompanyName,Linkman,Address,PostalCode,Phone,Mobile,
 Fax,WebSite,Email,BankAccounts,AccountNumber,IsNeedInvoices,DateCreated,CompanyType,State,Description
 ,SubjectInfo,PaymentDays,Complete,Expire,RelevanceFilialeId FROM [CompanyBankAccounts] as a inner join dbo.lmShop_CompanyCussent as b on a.CompanyId=b.CompanyId WHERE State=1 AND FilialeId=@FilialeId AND IsNeedInvoices=1 ";
            using (var db = new Database(GlobalConfig.ERP_DB_NAME))
            {
                return db.Select<CompanyCussentInfo>(true, SQL, new Parameter("@FilialeId", filialeId)).ToList();
            }
        }

        /// <summary>CompanyBalanceDetail
        /// </summary>
        /// <returns></returns>
        public IList<CompanyBalanceDetailInfo> GetCompanyBalanceDetailList()
        {
            const string SQL = "SELECT CompanyId,FilialeId,NonceBalance FROM lmShop_CompanyBalanceDetail ";
            using (var db = new Database(GlobalConfig.ERP_DB_NAME))
            {
                return db.Select<CompanyBalanceDetailInfo>(true, SQL).ToList();
            }
        }

        /// <summary>
        ///  查找供应商往来余额非0的往来公司 CompanyBalanceDetail
        /// </summary>
        /// <returns></returns>
        public IList<Guid> GetCompanyBalanceDetailFilialeIdList(Guid companyId)
        {
            const string SQL = "SELECT FilialeId FROM lmShop_CompanyBalanceDetail WHERE CompanyId=@CompanyId AND NonceBalance!=0 ";
            IList<Guid> filialeIds = new List<Guid>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@CompanyId", companyId)))
            {
                while (rdr.Read())
                {
                    filialeIds.Add(rdr.GetGuid(0));
                }
            }
            return filialeIds;
        }

        /// <summary>CompanyBalance
        /// </summary>
        /// <returns></returns>
        public IList<CompanyBalanceInfo> GetCompanyBalanceList()
        {
            const string SQL = "SELECT CompanyId,NonceBalance FROM lmShop_CompanyBalance ";
            using (var db = new Database(GlobalConfig.ERP_DB_NAME))
            {
                return db.Select<CompanyBalanceInfo>(true, SQL).ToList();
            }
        }

        /// <summary>保存供应商绑定到公司
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        public bool SaveCompanyBindingFiliale(Guid companyId, Guid filialeId)
        {
            const string SQL = @"
IF NOT EXISTS(SELECT CompanyId FROM [CompanyBindingFiliale]  WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId )
    INSERT INTO [CompanyBindingFiliale]([CompanyId],[FilialeId])VALUES(@CompanyId,@FilialeId)";
            var parms = new[]
                            {
                                new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier){Value = companyId},
                                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier){Value = filialeId}
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

        /// <summary>删除供应商绑定到公司
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        public bool DeleteCompanyBindingFiliale(Guid companyId, Guid filialeId)
        {
            const string SQL = @"DELETE FROM [CompanyBindingFiliale] WHERE CompanyId=@CompanyId and FilialeId=@FilialeId";
            var parms = new[]
                            {
                                new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier){Value = companyId},
                                new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier){Value = filialeId}
                            };
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL, parms);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>供应是否绑定该公司
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId">公司ID</param>
        /// <returns></returns>
        public bool GetCompanyIsBindingFiliale(Guid companyId, Guid filialeId)
        {
            const string SQL = "SELECT COUNT(0) FROM [CompanyBindingFiliale] WHERE CompanyId=@CompanyId AND FilialeId=@FilialeId ";
            var parms = new[] { new Parameter("@CompanyId", companyId), new Parameter("@FilialeId", filialeId) };
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, SQL, parms) == 1;
            }
        }

        /// <summary>获取供应商绑定的公司Ids
        /// modify by liangcanren at 2015-05-26
        /// </summary>
        /// <param name="companyId">供应商Id</param>
        /// <returns></returns>
        public IList<Guid> GetCompanyBindingFiliale(Guid companyId)
        {
            var builder = new StringBuilder("SELECT FilialeId FROM [CompanyBindingFiliale] ");
            if (companyId != Guid.Empty)
            {
                builder.AppendFormat(" WHERE CompanyId='{0}' ", companyId);
            }
            builder.Append(" GROUP BY FilialeId ");
            IList<Guid> filialeIds = new List<Guid>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), null))
            {
                while (rdr.Read())
                {
                    filialeIds.Add(rdr.GetGuid(0));
                }
            }
            return filialeIds;
        }

        /// <summary>判断此往来单位是否存在 ADD 2015-03-14 陈重文
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <returns></returns>
        public Boolean IsExistCompanyInfo(Guid companyId)
        {
            const string SQL = @"SELECT COUNT(0) FROM lmShop_CompanyCussent WHERE CompanyId=@CompanyId";
            var parms = new[] { new Parameter("@CompanyId", companyId) };
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, SQL, parms) == 1;
            }
        }

        /// <summary>
        /// 更新供应商资质状态
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="completeState">资质完整状态</param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public Boolean UpdateQualificationCompleteState(Guid companyId, int completeState, string expire)
        {
            const string SQL = "UPDATE lmShop_CompanyCussent SET Complete=@Complete,Expire=@Expire WHERE CompanyId=@CompanyId;";
            var parms = new[]{
                new Parameter("@CompanyId", companyId),
                new Parameter("@Complete",completeState),
                new Parameter("@Expire",expire)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL, parms);
            }
        }

        /// <summary>
        /// 供应商资质数据查询
        /// </summary>
        /// <param name="companyType">往来单位类型</param>
        /// <param name="state">往来单位状态</param>
        /// <param name="searchKey">搜索关键字(名称)</param>
        /// <param name="complete">是否完整</param>
        /// <param name="expire">是否过期</param>
        /// <returns></returns>
        public IList<SupplierGoodsInfo> GetSupplierGoodsInfos(CompanyType companyType, State state, string searchKey,
            int complete, int expire)
        {
            var builder = new StringBuilder(@"SELECT CompanyId AS ID,CompanyName AS Name,Complete,Expire FROM lmShop_CompanyCussent with(nolock) 
 WHERE CompanyType=@CompanyType AND [State]=@State ");
            if (!string.IsNullOrEmpty(searchKey))
            {
                builder.AppendFormat(" AND CompanyName LIKE '%{0}%' ", searchKey);
            }
            if (complete != 0)
            {
                if (complete == (int)SupplierCompleteType.NoComplete)
                {
                    builder.AppendFormat(" AND Complete IN(0,{0}) ", complete);
                }
                else
                {
                    builder.AppendFormat(" AND Complete={0} ", complete);
                }
            }
            if (expire != 0)
            {
                builder.AppendFormat(" AND Expire LIKE '%{0}%' ", expire);
            }
            var parms = new[] { new Parameter("@CompanyType", (int)companyType), new Parameter("@State", (int)state) };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<SupplierGoodsInfo>(true, builder.ToString(), parms).ToList();
            }
        }

        protected IList<CompanyCussentInfo> GetCompanyCussentInfos(string sql, params Parameter[] parms)
        {
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CompanyCussentInfo>(true, sql, parms).ToList();
            }
        }


        /// <summary>
        /// 获取快递类型的往来单位字典
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public IDictionary<Guid, String> GetCompanyDic()
        {
            var builder = new StringBuilder(@"select CompanyId, CompanyName from lmShop_CompanyCussent where CompanyType=3 AND [State]=1");
            IDictionary<Guid, String> dics = null;
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), null))
                {
                    if (rdr != null)
                    {
                        dics = new Dictionary<Guid, String>();
                        while (rdr.Read())
                        {
                            dics.Add(rdr["CompanyId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["CompanyId"].ToString()),
                                rdr["CompanyName"] == DBNull.Value ? "" : rdr["CompanyName"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("获取往来单位字典失败!", ex);
            }
            return dics;
        }

        /// <summary>判断此往来单位是否被搁置
        /// </summary>
        /// <param name="thirdCompanyID">第三方公司ID</param>
        /// <returns></returns>
        public Boolean IsAbeyanced(Guid thirdCompanyID)
        {
            const string SQL = @"SELECT [State] FROM lmshop_CompanyCussent AS C Where C.CompanyId=@ThirdCompanyID";
            var parms = new[] { new Parameter("@ThirdCompanyID", thirdCompanyID) };
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, SQL, parms) == (int)State.Disable;
            }
        }

        /// <summary>获取已被关联的公司列表 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> GetRelevanceFilialeIdList()
        {
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValues<Guid>(true, SQL_SELECT_EXISTS_FILIALEID);
            }
        }

        /// <summary>
        /// 通过关联公司获取对应的往来单位
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public CompanyCussentInfo GetCompanyByRelevanceFilialeId(Guid filialeId)
        {
            using (var db = DatabaseFactory.Create())
            {
                return db.Single<CompanyCussentInfo>(true, SQL_SELECT_COMPANYCUSSENT_FILIALEID, new[] { new Parameter(PARM_RELEVANCEFILIALEID, filialeId) });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public Guid GetCompanyIdByRelevanceFilialeId(Guid filialeId)
        {
            const string SQL = @"SELECT CompanyId FROM lmShop_CompanyCussent WHERE RelevanceFilialeId=@RelevanceFilialeId";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<Guid>(true, SQL, new[] { new Parameter(PARM_RELEVANCEFILIALEID, filialeId) });
            }
        }

        /// <summary>
        /// 通过往来单位id获取关联的公司id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Guid GetRelevanceFilialeIdByCompanyId(Guid companyId)
        {
            const string SQL = @"SELECT RelevanceFilialeId FROM lmShop_CompanyCussent WHERE CompanyId=@CompanyId";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<Guid>(true, SQL, new[] { new Parameter(PARM_COMPANYID, companyId) });
            }
        }

        /// <summary>
        /// 根据关联公司获取往来单位ID列表
        /// </summary>
        /// <param name="filialeIds"></param>
        /// <returns></returns>
        public List<PurchaseFilialeAuth> GetCompanyIdNameListByRelevanceFilialeIds(IEnumerable<Guid> filialeIds)
        {
            List<PurchaseFilialeAuth> result = new List<PurchaseFilialeAuth>();
            if (filialeIds == null || !filialeIds.Any())
            {
                return result;
            }
            const string SQL = @"SELECT CompanyId,CompanyName FROM lmShop_CompanyCussent WHERE RelevanceFilialeId in ('{0}')";
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<PurchaseFilialeAuth>(true, string.Format(SQL, string.Join("','", filialeIds.Select(m => m.ToString())))).ToList();
            }
        }


        public Dictionary<Guid, Guid> GetGoodsAndCompanyDic(IEnumerable<Guid> goodsId)
        {
            const String SQL = @"SELECT PS.GoodsId,PS.CompanyId FROM lmshop_PurchaseSet AS PS
  INNER JOIN lmShop_CompanyCussent AS CC ON PS.CompanyId=CC.CompanyId WHERE PS.GoodsId IN('{0}') AND CC.SalesScope=@SalesScope GROUP BY PS.GoodsId,PS.CompanyId";
            var sqlParams = new[] {new SqlParameter("@SalesScope", (int)Enum.Overseas.SalesScopeType.Overseas)};
            var dics = new Dictionary<Guid, Guid>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format(SQL, string.Join("','", goodsId.Select(m => m.ToString()))), sqlParams))
            {
                while (rdr.Read())
                {
                    dics.Add(rdr.GetGuid(0), rdr.GetGuid(1));
                }
            }
            return dics;
        }

        public Dictionary<Guid, String> GetAbroadCompanyList()
        {
            const String SQL = @"SELECT CompanyId,CompanyName FROM lmShop_CompanyCussent WHERE SalesScope=@SalesScope AND [State]=@State  ";
            var sqlParams = new[] {new SqlParameter("@SalesScope",(int)Enum.Overseas.SalesScopeType.Overseas),new SqlParameter("@State",(int)YesOrNo.Yes) }; 
            var dics = new Dictionary<Guid, String>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, sqlParams))
            {
                while (rdr.Read())
                {
                    dics.Add(rdr.GetGuid(0), rdr.GetString(1));
                }
            }
            return dics;
        }
    }
}
