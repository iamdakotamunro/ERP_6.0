using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.RWSplitting;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class CompanyPurchaseGoupDao : ICompanyPurchaseGoupDao
    {
        public CompanyPurchaseGoupDao(GlobalConfig.DB.FromType fromType) { }

        public int Insert(CompanyPurchaseGoupInfo info, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (info == null || info.CompanyId == Guid.Empty)
            {
                errorMessage = "参数不能为空";
                return 0;
            }
            string sql = string.Format("INSERT INTO [CompanyPurchaseGoup]([CompanyId],[PurchaseGroupId],[PurchaseGroupName],[OrderIndex]) VALUES('{0}','{1}',@PurchaseGroupName,{2})", info.CompanyId, info.PurchaseGroupId, info.OrderIndex);
            var parms = new[]
                        {
                            new SqlParameter("@PurchaseGroupName", SqlDbType.VarChar)
                        };
            parms[0].Value = info.PurchaseGroupName;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(ex.Message);
            }
        }

        public bool InsertList(List<CompanyPurchaseGoupInfo> list, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (list.Count == 0)
            {
                return false;
            }
            string sqlDelete = string.Format("DELETE FROM [CompanyPurchaseGoup] WHERE [OrderIndex]!=0 AND [CompanyId]='{0}'", list[0].CompanyId);
            string sqlInsert = "INSERT INTO [CompanyPurchaseGoup]([CompanyId],[PurchaseGroupId],[PurchaseGroupName],[OrderIndex]) VALUES(@CompanyId,@PurchaseGroupId,@PurchaseGroupName,@OrderIndex)";
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, sqlDelete);

                    var parms = new[]
                                {
                                    new SqlParameter("@CompanyId", SqlDbType.UniqueIdentifier),
                                    new SqlParameter("@PurchaseGroupId", SqlDbType.UniqueIdentifier),
                                    new SqlParameter("@PurchaseGroupName", SqlDbType.VarChar),
                                    new SqlParameter("@OrderIndex", SqlDbType.Int)
                                };
                    int orderIndex = 1;
                    foreach (var info in list)
                    {
                        if (info.OrderIndex == 0) continue;
                        parms[0].Value = info.CompanyId;
                        parms[1].Value = info.PurchaseGroupId;
                        parms[2].Value = info.PurchaseGroupName;
                        parms[3].Value = orderIndex++;
                        SqlHelper.ExecuteNonQuery(trans, sqlInsert, parms);
                    }
                    trans.Commit();
                }
                catch (SqlException ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            return true;
        }

        public int Update(CompanyPurchaseGoupInfo info, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (info == null || info.CompanyId == Guid.Empty)
            {
                errorMessage = "参数不能为空";
                return 0;
            }
            string sql = string.Format("UPDATE [CompanyPurchaseGoup] SET [PurchaseGroupName]=@PurchaseGroupName WHERE [CompanyId]='{0}' AND [PurchaseGroupId]='{1}'", info.CompanyId, info.PurchaseGroupId);
            var parms = new[]
                        {
                            new SqlParameter("@PurchaseGroupName", SqlDbType.VarChar)
                        };
            parms[0].Value = info.PurchaseGroupName;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(ex.Message);
            }
        }

        public IList<CompanyPurchaseGoupInfo> GetCompanyPurchaseGoupList(Guid companyId)
        {
            IList<CompanyPurchaseGoupInfo> list = new List<CompanyPurchaseGoupInfo>();
            if (companyId != Guid.Empty)
            {
                string sql = string.Format("SELECT [CompanyId],[PurchaseGroupId],[PurchaseGroupName],[OrderIndex] FROM [CompanyPurchaseGoup] WHERE [CompanyId]='{0}'", companyId);
                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            list.Add(new CompanyPurchaseGoupInfo
                                     {
                                         CompanyId = SqlRead.GetGuid(dr, "CompanyId"),
                                         PurchaseGroupId = SqlRead.GetGuid(dr, "PurchaseGroupId"),
                                         PurchaseGroupName = SqlRead.GetString(dr, "PurchaseGroupName"),
                                         OrderIndex = SqlRead.GetInt(dr, "OrderIndex")
                                     });
                        }
                    }
                }
            }
            return list;
        }

        public bool IsExist(Guid companyId, Guid purchaseGroupId)
        {
            string sql = string.Format("SELECT COUNT(0) FROM [CompanyPurchaseGoup] WHERE  [CompanyId]='{0}' AND [PurchaseGroupId]='{1}'", companyId, purchaseGroupId);
            var obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sql);
            return Convert.ToInt32(obj) > 0;
        }

        /// <summary>根据采购分组ID获取分组信息
        /// </summary>
        /// <param name="purchaseGroupId">采购分组ID</param>
        /// <returns></returns>
        public CompanyPurchaseGoupInfo GetCompanyPurchaseGoupInfo(Guid purchaseGroupId)
        {
            const string SQL = @"SELECT [CompanyId],[PurchaseGroupId],[PurchaseGroupName],[OrderIndex] FROM [CompanyPurchaseGoup] WHERE [PurchaseGroupId]=@PurchaseGroupId";
            try
            {
                var parm = new Parameter("@PurchaseGroupId", purchaseGroupId);
                using (var db = new Database(GlobalConfig.ERP_DB_NAME))
                {
                    return db.Single<CompanyPurchaseGoupInfo>(true, SQL, parm);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public bool Delete(Guid companyId, Guid purchaseGroupId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (companyId == Guid.Empty || purchaseGroupId == Guid.Empty)
            {
                errorMessage = "参数不能为空";
                return false;
            }
            string sql = string.Format("DELETE FROM [CompanyPurchaseGoup] WHERE [CompanyId]='{0}' AND [PurchaseGroupId]='{1}'", companyId, purchaseGroupId);

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql) > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
