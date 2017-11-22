using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using Keede.Ecsoft.Model;
using ERP.Environment;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public partial class CompanyCussentRelation : ICompanyCussentRelation
    {
        readonly Environment.GlobalConfig.DB.FromType _fromType;
        public CompanyCussentRelation(GlobalConfig.DB.FromType fromType)
        {
            _fromType = fromType;
        }

        private const String SQL_SELECT_COMPANYCUSSENTRELATION = @"SELECT Id,AccountNo,AccountName,CompanyId,CompanyName,SaleFilialeId,SaleFilialeName FROM lmShop_CompanyCussent_Relation WHERE CompanyId = @CompanyId";
        private const String SQL_SELECT_EDIT_COMPANYCUSSENTRELATION = @"SELECT Id,AccountNo,AccountName,CompanyId,CompanyName,SaleFilialeId,SaleFilialeName FROM lmShop_CompanyCussent_Relation WHERE CompanyId = @CompanyId AND AccountNo = @AccountNo";
        private const String SQL_INSERT_COMPANYCUSSENTRELATION = @"INSERT INTO lmShop_CompanyCussent_Relation (Id,AccountNo,AccountName,CompanyId,CompanyName,SaleFilialeId,SaleFilialeName) VALUES (@Id,@AccountNo,@AccountName,@CompanyId,@CompanyName,@SaleFilialeId,@SaleFilialeName)";
        private const String SQL_DELETEDETAIL_COMPANYCUSSENTRELATION = @"DELETE FROM lmShop_CompanyCussent_Relation  WHERE AccountNo = @AccountNo AND CompanyId = @CompanyId ";
        private const String SQL_SELECT_COMPANYCUSSENTRELATION_ACCOUNTNO = @"SELECT cc.[CompanyId] ,cc.[CompanyName],cc.[CompanyType],ISNULL(DeliverType,0) AS DeliverType,cc.[State] FROM lmShop_CompanyCussent_Relation as cr
  inner join [lmShop_CompanyCussent] as cc on cr.CompanyId=cc.CompanyId
  WHERE cr.AccountNo=@AccountNo and cr.SaleFilialeId=@SaleFilialeId and SalesScope=@SalesScope ";
        private const String SQL_DELETEDETAIL = @"DELETE FROM lmShop_CompanyCussent_Relation  WHERE Id = @Id ";

        private const string PARM_ID = "@Id";
        private const string PARM_ACCOUNTNO = "@AccountNo";
        private const string PARM_ACCOUNTNAME = "@AccountName";
        private const string PARM_COMPANYID = "@CompanyId";
        private const string PARM_COMPANYNAME = "@CompanyName";
        private const string PARM_SALEFILIALEID = "@SaleFilialeId";
        private const string PARM_SALEFILIALENAME = "@SaleFilialeName";
        private const string PARM_SALESSCOPE = "@SalesScope";

        /// <summary>
        /// 查询列表
        /// </summary>
        public IList<CompanyCussentRelationInfo> GetCompanyCussentRelationInfoList(Guid companyId)
        {
            var parm = new Parameter(PARM_COMPANYID, companyId);
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CompanyCussentRelationInfo>(true, SQL_SELECT_COMPANYCUSSENTRELATION, parm).ToList();
            }
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        public IList<CompanyCussentRelationInfo> GetEditCompanyCussentRelationInfoList(String accountNo, Guid companyId)
        {
            var parm = new[]{
                new Parameter(PARM_ACCOUNTNO,accountNo),
                new Parameter(PARM_COMPANYID,companyId)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CompanyCussentRelationInfo>(true, SQL_SELECT_EDIT_COMPANYCUSSENTRELATION, parm).ToList();
            }
        }

        /// <summary>
        /// 新增权限
        /// </summary>
        public bool Insert(List<CompanyCussentRelationInfo> companyCussentRelation)
        {
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();
                foreach (CompanyCussentRelationInfo companyCussentRelationInfo in companyCussentRelation)
                {
                    var parmsDetail = new[]
                    {
                        new Parameter(PARM_ID, companyCussentRelationInfo.Id),
                        new Parameter(PARM_ACCOUNTNO, companyCussentRelationInfo.AccountNo),
                        new Parameter(PARM_ACCOUNTNAME, companyCussentRelationInfo.AccountName),
                        new Parameter(PARM_COMPANYID, companyCussentRelationInfo.CompanyId),
                        new Parameter(PARM_COMPANYNAME, companyCussentRelationInfo.CompanyName),
                        new Parameter(PARM_SALEFILIALEID, companyCussentRelationInfo.SaleFilialeId),
                        new Parameter(PARM_SALEFILIALENAME, companyCussentRelationInfo.SaleFilialeName)

                    };
                    db.Execute(false, SQL_INSERT_COMPANYCUSSENTRELATION, parmsDetail);
                }
                return db.CompleteTransaction();
            }
        }

        public bool Save(List<Guid> deleteIds, List<CompanyCussentRelationInfo> insertList)
        {
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();
                foreach (var id in deleteIds)
                {
                    db.Execute(false, SQL_DELETEDETAIL, new Parameter(PARM_ID, id));
                }
                foreach (CompanyCussentRelationInfo companyCussentRelationInfo in insertList)
                {
                    var parmsDetail = new[]
                    {
                        new Parameter(PARM_ID, companyCussentRelationInfo.Id),
                        new Parameter(PARM_ACCOUNTNO, companyCussentRelationInfo.AccountNo),
                        new Parameter(PARM_ACCOUNTNAME, companyCussentRelationInfo.AccountName),
                        new Parameter(PARM_COMPANYID, companyCussentRelationInfo.CompanyId),
                        new Parameter(PARM_COMPANYNAME, companyCussentRelationInfo.CompanyName),
                        new Parameter(PARM_SALEFILIALEID, companyCussentRelationInfo.SaleFilialeId),
                        new Parameter(PARM_SALEFILIALENAME, companyCussentRelationInfo.SaleFilialeName)

                    };
                    db.Execute(false, SQL_INSERT_COMPANYCUSSENTRELATION, parmsDetail);
                }
                return db.CompleteTransaction();
            }
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        public bool Update(List<CompanyCussentRelationInfo> companyCussentRelation)
        {
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();
                foreach (CompanyCussentRelationInfo companyCussentRelationInfo in companyCussentRelation)
                {
                    var parmsDetail = new[]
                    {
                        new Parameter(PARM_ACCOUNTNO, companyCussentRelationInfo.AccountNo),
                        new Parameter(PARM_ACCOUNTNAME, companyCussentRelationInfo.AccountName),
                        new Parameter(PARM_COMPANYID, companyCussentRelationInfo.CompanyId),
                        new Parameter(PARM_COMPANYNAME, companyCussentRelationInfo.CompanyName),
                        new Parameter(PARM_SALEFILIALEID, companyCussentRelationInfo.SaleFilialeId),
                        new Parameter(PARM_SALEFILIALENAME, companyCussentRelationInfo.SaleFilialeName)

                    };
                    db.Execute(false, SQL_INSERT_COMPANYCUSSENTRELATION, parmsDetail);
                }
                return db.CompleteTransaction();
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="accountNo">账户</param>
        /// <param name="companyId">往来单位编号</param>
        public void Delete(String accountNo, Guid companyId)
        {
            var parms = new[]{
                new SqlParameter(PARM_ACCOUNTNO, SqlDbType.VarChar) {Value = accountNo},
                new SqlParameter(PARM_COMPANYID,SqlDbType.UniqueIdentifier){Value = companyId}
            };
            using (var connection = Keede.DAL.RWSplitting.Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL_DELETEDETAIL_COMPANYCUSSENTRELATION, parms);
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
        /// 
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public List<AuthorizeCompanyDTO> GetAuthorizeCompanyDtos(string accountNo, Guid saleFilialeId)
        {
            var parm = new[]{
                new Parameter(PARM_ACCOUNTNO,accountNo),
                new Parameter(PARM_SALEFILIALEID,saleFilialeId),
                new Parameter(PARM_SALESSCOPE,(int)Enum.Overseas.SalesScopeType.Overseas)
            };
            using (var db = DatabaseFactory.Create())
            {
                var result = db.Select<AuthorizeCompanyDTO>(true, SQL_SELECT_COMPANYCUSSENTRELATION_ACCOUNTNO, parm);
                return result?.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public bool IsExist(Guid companyId)
        {
            const string SQL = @"SELECT COUNT(*) FROM lmShop_CompanyCussent_Relation WHERE CompanyId=@CompanyId";
            var parm = new[]{
                new Parameter(PARM_COMPANYID,companyId)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, SQL, parm) > 0;
            }
        }
    }
}
