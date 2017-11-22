using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    public class Cost : ICost
    {
        public Cost(Environment.GlobalConfig.DB.FromType fromType) { }

        private const string SQL_INSERT_COMPANYCLASS = "INSERT INTO lmShop_CostCompanyClass VALUES(@CompanyClassId,@ParentCompanyClassId,@CompanyClassCode,@CompanyClassName);";
        private const string SQL_UPDATE_COMPANYCLASS = "UPDATE lmShop_CostCompanyClass SET CompanyClassCode=@CompanyClassCode,CompanyClassName=@CompanyClassName WHERE CompanyClassId=@CompanyClassId;";
        private const string SQL_DELETE_COMPANYCLASS = "DELETE FROM lmShop_CostCompanyClass WHERE CompanyClassId=@CompanyClassId;";
        private const string SQL_SELECT_COMPANYCLASS = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CostCompanyClass WHERE CompanyClassId=@CompanyClassId;";
        private const string SQL_SELECT_PARENTCOMPANYCLASS = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CostCompanyClass WHERE CompanyClassId=(SELECT ParentCompanyClassId FROM lmShop_CostCompanyClass WHERE CompanyClassId=@CompanyClassId);";
        private const string SQL_SELECT_COMPANYCLASS_LIST = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CostCompanyClass ORDER BY CompanyClassCode ASC;";
        private const string SQL_SELECT_CHILDCOMPANYCLASS_LIST = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CostCompanyClass WHERE ParentCompanyClassId=@ParentCompanyClassId ORDER BY CompanyClassCode ASC;";

        private const string SQL_SELECT_COMPANYCLASS_CHILDCOUNT = "SELECT COUNT(*) FROM lmShop_CostCompanyClass WHERE ParentCompanyClassId=@CompanyClassId;";
        private const string SQL_SELECT_COMPANYCLASS_FIRECOUNT = "SELECT COUNT(*) FROM lmShop_CompanyCussent WHERE CompanyClassId=@CompanyClassId;";
        private const string PARM_COMPANYCLASSID = "@CompanyClassId";
        private const string PARM_PARENTCOMPANYCLASSID = "@ParentCompanyClassId";
        private const string PARM_COMPANYCLASSCODE = "@CompanyClassCode";
        private const string PARM_COMPANYCLASSNAME = "@CompanyClassName";
        private const string SQL_SELECT_BY_CLASS_ID = "SELECT [CompanyClassId],[ParentCompanyClassId],[CompanyClassCode],[CompanyClassName] FROM [lmShop_CostCompanyClass] WHERE [ParentCompanyClassId] = @ParentCostClassID ORDER BY CompanyClassCode";
        private const string PARM_COST_CLASS_PARENT_ID = "@ParentCostClassID";
        /// <summary>
        /// 得到下级分类列表
        /// </summary>
        /// <param name="costClassId">分类ID</param>
        /// <returns></returns>
        public IList<CostCompanyClassInfo> GetChildrenList(Guid costClassId)
        {
            var prms = new SqlParameter(PARM_COST_CLASS_PARENT_ID, SqlDbType.UniqueIdentifier) { Value = costClassId };
            IList<CostCompanyClassInfo> ccInfoList = new List<CostCompanyClassInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_BY_CLASS_ID, prms))
            {
                while (rdr.Read())
                {
                    var ccInfo = new CostCompanyClassInfo
                                     {
                                         CompanyClassId = rdr.GetGuid(0),
                                         ParentCompanyClassId = rdr.GetGuid(1),
                                         CompanyClassCode = rdr.GetString(2),
                                         CompanyClassName = rdr.GetString(3)
                                     };
                    ccInfoList.Add(ccInfo);
                }

            }
            return ccInfoList;
        }

        /// <summary>
        /// 添加往来单位信息
        /// </summary>
        /// <param name="companyClass">往来单位分类类实例</param>
        public void Insert(CostCompanyClassInfo companyClass)
        {
            SqlParameter[] parms = GetCompanyClassParameters();
            parms[0].Value = companyClass.CompanyClassId;
            parms[1].Value = companyClass.ParentCompanyClassId;
            parms[2].Value = companyClass.CompanyClassCode;
            parms[3].Value = companyClass.CompanyClassName;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_COMPANYCLASS, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 更新往来单位信息
        /// </summary>
        /// <param name="companyClass">往来单位分类类实例</param>
        public void Update(CostCompanyClassInfo companyClass)
        {
            SqlParameter[] parms = GetCompanyClassParameters();
            parms[0].Value = companyClass.CompanyClassId;
            parms[1].Value = companyClass.ParentCompanyClassId;
            parms[2].Value = companyClass.CompanyClassCode;
            parms[3].Value = companyClass.CompanyClassName;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_COMPANYCLASS, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除往来单位分类
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
        public void Delete(Guid companyClassId)
        {
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = companyClassId };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_COMPANYCLASS, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 获取指定编号往来单位类实例
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
        /// <returns></returns>
        public CostCompanyClassInfo GetCompanyClass(Guid companyClassId)
        {
            CostCompanyClassInfo companyClass;
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = companyClassId };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCLASS, parm))
            {
                companyClass = rdr.Read() ? new CostCompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3)) : new CostCompanyClassInfo();
            }
            return companyClass;
        }

        /// <summary>
        /// 获取指定编号的往来单位父类
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
        /// <returns></returns>
        public CostCompanyClassInfo GetParentCompanyClass(Guid companyClassId)
        {
            CostCompanyClassInfo companyClass;
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = companyClassId };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PARENTCOMPANYCLASS, parm))
            {
                companyClass = rdr.Read() ? new CostCompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3)) : new CostCompanyClassInfo();
            }
            return companyClass;
        }

        /// <summary>
        /// 获取往来单位列表
        /// </summary>
        /// <returns></returns>
        public IList<CostCompanyClassInfo> GetCompanyClassList()
        {
            IList<CostCompanyClassInfo> companyClassList = new List<CostCompanyClassInfo>();

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCLASS_LIST, null))
            {
                while (rdr.Read())
                {
                    var companyClass = new CostCompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3));
                    companyClassList.Add(companyClass);
                }
            }
            return companyClassList;
        }

        /// <summary>
        /// 获取指定分类的子分类列表
        /// </summary>
        /// <param name="parentCompanyClassId">父分类编号</param>
        /// <returns></returns>
        public IList<CostCompanyClassInfo> GetChildCompanyClassList(Guid parentCompanyClassId)
        {
            IList<CostCompanyClassInfo> companyClassList = new List<CostCompanyClassInfo>();
            var parm = new SqlParameter(PARM_PARENTCOMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = parentCompanyClassId };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_CHILDCOMPANYCLASS_LIST, parm))
            {
                while (rdr.Read())
                {
                    var companyClass = new CostCompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3));
                    companyClassList.Add(companyClass);
                }

            }
            return companyClassList;
        }


        /// <summary>
        /// 获取子分类数量
        /// </summary>
        /// <param name="companyClassId">分类编号</param>
        /// <returns>返回int型,子分类数量</returns>
        public int GetChildCompanyClassCount(Guid companyClassId)
        {
            int count = 0;
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = companyClassId };
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCLASS_CHILDCOUNT, parm);
            if (obj != DBNull.Value)
                count = Convert.ToInt32(obj);
            return count;
        }

        /// <summary>
        /// 返回直接绑定到该分类的公司数量,不计子分类中公司的数量
        /// </summary>
        /// <param name="companyClassId">公司编号</param>
        /// <returns>返回int型,绑定公司分类数量</returns>
        public int GetFireCompanyCount(Guid companyClassId)
        {
            int count = 0;
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) { Value = companyClassId };
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCLASS_FIRECOUNT, parm);
            if (obj != DBNull.Value)
                count = Convert.ToInt32(obj);
            return count;
        }

        private static SqlParameter[] GetCompanyClassParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PARENTCOMPANYCLASSID, SqlDbType.UniqueIdentifier),
				new SqlParameter(PARM_COMPANYCLASSCODE, SqlDbType.VarChar, 16),
                new SqlParameter(PARM_COMPANYCLASSNAME, SqlDbType.VarChar, 32)
            };
            return parms;
        }

        public bool CanDelete(Guid companyClassId)
        {
            var strbSql = new StringBuilder();
            strbSql.Append("select COUNT(CompanyClassId) as IdCount from CostCompany where CompanyClassId='").Append(companyClassId).Append("'");
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString()))
            {
                if (rdr != null && rdr.Read())
                {
                    int idCount = rdr["IdCount"] == DBNull.Value ? 0 : int.Parse(rdr["IdCount"].ToString());
                    if (idCount > 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>获取具有权限的费用分类  ADD  2014-12-09 陈重文 
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="branchId">部门ID</param>
        /// <returns></returns>
        public IList<CostCompanyClassInfo> GetPermissionCompanyClassList(Guid filialeId, Guid branchId)
        {
            const string SQL = @"SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CostCompanyClass WHERE CompanyClassId IN
(
	SELECT DISTINCT CompanyClassId FROM CostCompany WHERE CompanyId IN
	(
		SELECT CompanyId FROM CostPermission WHERE FilialeId=@FilialeId
		AND BranchId=@BranchId
	)
)
ORDER BY CompanyClassCode ASC;";
            var parms = new[]
                {
                    new Parameter("@FilialeId",filialeId),
                    new Parameter("@BranchId",branchId)
                };

            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CostCompanyClassInfo>(true, SQL, parms).ToList();
            }
        }
    }
}
