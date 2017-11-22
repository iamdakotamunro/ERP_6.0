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
        /// ���������λ��Ϣ
        /// </summary>
        /// <param name="companyClass">������λ������ʵ��</param>
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
        /// ����������λ��Ϣ
        /// </summary>
        /// <param name="companyClass">������λ������ʵ��</param>
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
        /// ɾ��������λ����
        /// </summary>
        /// <param name="companyClassId">������λ���</param>
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
        /// ��ȡָ�����������λ��ʵ��
        /// </summary>
        /// <param name="companyClassId">������λ���</param>
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
        /// ��ȡָ����ŵ�������λ����
        /// </summary>
        /// <param name="companyClassId">������λ���</param>
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
        /// ��ȡ������λ�б�
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
        /// ��ȡָ��������ӷ����б�
        /// </summary>
        /// <param name="parentCompanyClassId">��������</param>
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
        /// ��ȡ�ӷ�������
        /// </summary>
        /// <param name="companyClassId">������</param>
        /// <returns>����int��,�ӷ�������</returns>
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
        /// ����ֱ�Ӱ󶨵��÷���Ĺ�˾����,�����ӷ����й�˾������
        /// </summary>
        /// <param name="companyClassId">��˾���</param>
        /// <returns>����int��,�󶨹�˾��������</returns>
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
