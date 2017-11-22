using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class VocabularyDal : IVocabulary
    {
        public VocabularyDal(GlobalConfig.DB.FromType fromType)
        {

        }
        
        /// <summary>
        /// 批量插入词汇
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddBatchVocabulary(IList<VocabularyInfo> list)
        {
            var dics = new Dictionary<string, string>
            {
                {"Id","Id"},{"VocabularyName","VocabularyName"},{"State","State"},{"Operator","Operator"},{"OperatingTime","OperatingTime"}
            };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, list, "Vocabulary", dics) > 0;
        }
        
        /// <summary>
        /// 根据id更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state">状态(0：禁用,1：启用)</param>
        /// <returns></returns>
        public bool UpdateStateById(Guid id, int state)
        {
            string sql = @"UPDATE [Vocabulary] SET [State]=" + state + " WHERE Id='" + id + "'";
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql) > 0;
        }

        /// <summary>
        /// 删除词汇
        /// </summary>
        /// <param name="arryId">主键Id</param>
        /// <returns></returns>
        public bool DelById(string[] arryId)
        {
            if (!arryId.Any())
            {
                return false;
            }
            var idStr = "'" + string.Join("','", arryId.ToArray()) + "'";

            string sql = @"DELETE FROM [Vocabulary] WHERE [Id] IN (" + idStr + ")";
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql) > 0;
        }

        /// <summary>
        /// 根据词汇名称查询
        /// </summary>
        /// <param name="vocabularyName">词汇名称</param>
        /// <returns></returns>
        public List<VocabularyInfo> GetVocabularyListByVocabularyName(string vocabularyName)
        {
            string sql = "SELECT [Id],[VocabularyName],[State],[Operator],[OperatingTime] FROM [dbo].[Vocabulary] WHERE VocabularyName LIKE '%" + vocabularyName + "%'";

            var list = new List<VocabularyInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    var vocabularyInfo = new VocabularyInfo
                    {
                        Id = rdr["Id"] == DBNull.Value ? Guid.Empty : new Guid(rdr["Id"].ToString()),
                        VocabularyName = rdr["VocabularyName"] == DBNull.Value ? string.Empty : rdr["VocabularyName"].ToString(),
                        State = rdr["State"] == DBNull.Value ? -1 : int.Parse(rdr["State"].ToString()),
                        Operator = rdr["Operator"] == DBNull.Value ? string.Empty : rdr["Operator"].ToString(),
                        OperatingTime = rdr["OperatingTime"] == DBNull.Value ? Convert.ToDateTime("1900-01-01") : DateTime.Parse(rdr["OperatingTime"].ToString())
                    };
                    list.Add(vocabularyInfo);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据词汇状态查询
        /// </summary>
        /// <param name="state">状态(0：禁用,1：启用)</param>
        /// <returns></returns>
        public List<VocabularyInfo> GetVocabularyListByState(int state)
        {
            string sql = "SELECT [Id],[VocabularyName],[State],[Operator],[OperatingTime] FROM [dbo].[Vocabulary] WHERE [State]=" + state;

            var list = new List<VocabularyInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    var vocabularyInfo = new VocabularyInfo
                    {
                        Id = rdr["Id"] == DBNull.Value ? Guid.Empty : new Guid(rdr["Id"].ToString()),
                        VocabularyName = rdr["VocabularyName"] == DBNull.Value ? string.Empty : rdr["VocabularyName"].ToString(),
                        State = rdr["State"] == DBNull.Value ? -1 : int.Parse(rdr["State"].ToString()),
                        Operator = rdr["Operator"] == DBNull.Value ? string.Empty : rdr["Operator"].ToString(),
                        OperatingTime = rdr["OperatingTime"] == DBNull.Value ? Convert.ToDateTime("1900-01-01") : DateTime.Parse(rdr["OperatingTime"].ToString())
                    };
                    list.Add(vocabularyInfo);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据词汇状态查询
        /// </summary>
        /// <param name="state">状态(0：禁用,1：启用)</param>
        /// <returns></returns>
        public List<string> GetVocabularyNameListByState(int state)
        {
            string sql = "SELECT [VocabularyName] FROM [dbo].[Vocabulary] WHERE [State]=" + state;

            var list = new List<string>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    list.Add(rdr["VocabularyName"] == DBNull.Value ? string.Empty : rdr["VocabularyName"].ToString());
                }
            }
            return list;
        }
    }
}
