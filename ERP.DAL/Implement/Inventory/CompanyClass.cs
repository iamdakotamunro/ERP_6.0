using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using Keede.Ecsoft.Model;
using ERP.Environment;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class CompanyClass : ICompanyClass
    {
        public CompanyClass(Environment.GlobalConfig.DB.FromType fromType = Environment.GlobalConfig.DB.FromType.Write) { }


        private const string SQL_INSERT_COMPANYCLASS = "INSERT INTO lmShop_CompanyClass VALUES(@CompanyClassId,@ParentCompanyClassId,@CompanyClassCode,@CompanyClassName);";
        private const string SQL_UPDATE_COMPANYCLASS = "UPDATE lmShop_CompanyClass SET CompanyClassCode=@CompanyClassCode,CompanyClassName=@CompanyClassName WHERE CompanyClassId=@CompanyClassId;";
        private const string SQL_DELETE_COMPANYCLASS = "DELETE FROM lmShop_CompanyClass WHERE CompanyClassId=@CompanyClassId;";
        private const string SQL_SELECT_COMPANYCLASS = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CompanyClass WHERE CompanyClassId=@CompanyClassId;";
        private const string SQL_SELECT_PARENTCOMPANYCLASS = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CompanyClass WHERE CompanyClassId=(SELECT ParentCompanyClassId FROM lmShop_CompanyClass WHERE CompanyClassId=@CompanyClassId);";
        private const string SQL_SELECT_COMPANYCLASS_LIST = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CompanyClass ORDER BY CompanyClassCode ASC;";
        private const string SQL_SELECT_CHILDCOMPANYCLASS_LIST = "SELECT CompanyClassId,ParentCompanyClassId,CompanyClassCode,CompanyClassName FROM lmShop_CompanyClass WHERE ParentCompanyClassId=@ParentCompanyClassId ORDER BY CompanyClassCode ASC;";
        private const string SQL_SELECT_COMPANYCLASS_CHILDCOUNT = "SELECT COUNT(*) FROM lmShop_CompanyClass WHERE ParentCompanyClassId=@CompanyClassId;";
        private const string SQL_SELECT_COMPANYCLASS_FIRECOUNT = "SELECT COUNT(*) FROM lmShop_CompanyCussent WHERE CompanyClassId=@CompanyClassId;";
        private const string PARM_COMPANYCLASSID = "@CompanyClassId";
        private const string PARM_PARENTCOMPANYCLASSID = "@ParentCompanyClassId";
        private const string PARM_COMPANYCLASSCODE = "@CompanyClassCode";
        private const string PARM_COMPANYCLASSNAME = "@CompanyClassName";

        /// <summary>
        /// 添加往来单位信息
        /// </summary>
        /// <param name="companyClass">往来单位分类类实例</param>
        public void Insert(CompanyClassInfo companyClass)
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
        public void Update(CompanyClassInfo companyClass)
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
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) {Value = companyClassId};
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
        public CompanyClassInfo GetCompanyClass(Guid companyClassId)
        {
            CompanyClassInfo companyClass;
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) {Value = companyClassId};
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCLASS, parm))
            {
                companyClass = rdr.Read() ? new CompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3)) : new CompanyClassInfo();
            }
            return companyClass;
        }

        /// <summary>
        /// 获取指定编号的往来单位父类
        /// </summary>
        /// <param name="companyClassId">往来单位编号</param>
        /// <returns></returns>
        public CompanyClassInfo GetParentCompanyClass(Guid companyClassId)
        {
            CompanyClassInfo companyClass;
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) {Value = companyClassId};
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PARENTCOMPANYCLASS, parm))
            {
                companyClass = rdr.Read() ? new CompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3)) : new CompanyClassInfo();
            }
            return companyClass;
        }

        /// <summary>
        /// 获取往来单位列表
        /// </summary>
        /// <returns></returns>
        public IList<CompanyClassInfo> GetCompanyClassList()
        {
            IList<CompanyClassInfo> companyClassList = new List<CompanyClassInfo>();

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_COMPANYCLASS_LIST, null))
            {
                while (rdr.Read())
                {
                    var companyClass = new CompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3));
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
        public IList<CompanyClassInfo> GetChildCompanyClassList(Guid parentCompanyClassId)
        {
            IList<CompanyClassInfo> companyClassList = new List<CompanyClassInfo>();
            var parm = new SqlParameter(PARM_PARENTCOMPANYCLASSID, SqlDbType.UniqueIdentifier)
                           {Value = parentCompanyClassId};
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_CHILDCOMPANYCLASS_LIST, parm))
            {
                while (rdr.Read())
                {
                    var companyClass = new CompanyClassInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3));
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
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) {Value = companyClassId};
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
            var parm = new SqlParameter(PARM_COMPANYCLASSID, SqlDbType.UniqueIdentifier) {Value = companyClassId};
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
    }
}
