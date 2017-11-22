using System;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using ERP.DAL.Interface.IUtilities;
using ERP.Environment;
using Keede.DAL.Helper;
using System.Data;

namespace ERP.DAL.Utilities
{
    /// <summary>
    /// 非全字段地操作某个表
    /// </summary>
    public class UtilityDal : IUtility
    {
        public UtilityDal(GlobalConfig.DB.FromType fromType) { }

        #region 非全字段地操作某个表
        /// <summary>
        /// 检查某个字段的是否存在某个值
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="checkField">列名</param>  
        /// <param name="checkValue">要检查的值</param>  
        /// <returns></returns> 
        public bool CheckExists(string tableName, string checkField, string checkValue)
        {
            var sql = new StringBuilder();
            sql.Append("select count(*) from [").Append(tableName).Append("] where [").Append(checkField).Append("]= @checkValue ");
            var paras = new []{
            new SqlParameter("@checkValue",checkValue)
            };
            string count = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), paras).ToString();
            return int.Parse(count) > 0;
        }
        /// <summary>
        /// 检查某个字段的是否存在某个值（返回值> 0）
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="conditionFieldNames">作为条件列的列名(可多列，用英文状态下逗号隔开)</param>
        /// <param name="conditionValues">查询条件值</param>  
        /// <returns>存在的个数</returns> 
        public int CheckExists(string tableName, string conditionFieldNames, object[] conditionValues)
        {
            string[] fields = conditionFieldNames.Split(',');
            if (fields.Length != conditionValues.Length)
            {
                throw new Exception("提供的列数与提供的值的个数不匹配！");
            }
            List<object> result = new List<object>();
            List<SqlParameter> paraList = new List<SqlParameter>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select count(1)  from [").Append(tableName).Append("] where 1=1 ");
            for (int i = 0; i < fields.Length; i++)
            {
                sql.Append(" And [").Append(fields[i]).Append("]=@").Append(fields[i]);
                paraList.Add(new SqlParameter("@" + fields[i], conditionValues[i]));
            }
            string count = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), paraList.ToArray()).ToString();
            return int.Parse(count);
        }
        /// <summary>
        /// 根据主键查询一列或多列的值(Datareader中的原始数据，顺序与给定的字段的顺序相同)
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>  
        /// <param name="pkFieldName">唯一键字段名称</param> 
        /// <param name="pk">唯一键的查询值</param>  
        /// <returns></returns> 
        public object[] GetFieldValueByPk(string tableName, string fields, string pkFieldName, string pk)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ").Append(fields).Append(" from [").Append(tableName).Append("] where [").Append(pkFieldName).Append("]= @PK ");
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@PK",pk)
            };
            int fieldCount = fields.Split(',').Length;
            object[] results = null;
            IDataReader reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), paras);
            if (reader.Read())
            {
                results = new object[fieldCount];
                for (int i = 0; i < fieldCount; i++)
                {
                    results[i] = reader[i];
                }
            }
            reader.Close();
            return results;
        }
        /// <summary>
        /// 根据主键查询一列或多列的值(Datareader中的原始数据，顺序与给定的字段的顺序相同)
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>  
        /// <param name="pkFieldNames">唯一键字段名称</param> 
        /// <param name="pkValues">唯一键的查询值</param>  
        /// <returns></returns>
        public object[] GetFieldValueByPk(string tableName, string fields, string pkFieldNames, object[] pkValues)
        {
            string[] fieldNames = pkFieldNames.Split(',');
            if (fieldNames.Length != pkValues.Length)
            {
                throw new Exception("提供的列数与提供的值的个数不匹配！");
            }
            List<object> result = new List<object>();
            List<SqlParameter> paraList = new List<SqlParameter>();
            StringBuilder sql = new StringBuilder();
            sql.Append("select Top 1 ").Append(fields).Append(" from [").Append(tableName).Append("] where 1=1 ");
            for (int i = 0; i < fieldNames.Length; i++)
            {
                sql.Append(" And [").Append(fieldNames[i]).Append("]=@").Append(fieldNames[i]);
                paraList.Add(new SqlParameter("@" + fieldNames[i], pkValues[i]));
            }
            int fieldCount = fields.Split(',').Length;
            object[] results = null;
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), paraList.ToArray());
            if (reader.Read())
            {
                results = new object[fieldCount];
                for (int i = 0; i < fieldCount; i++)
                {
                    results[i] = reader[i];
                }
            }
            reader.Close();
            return results;
        }
        /// <summary>
        /// 获取单列的值的列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="targetFieldName">要获取值的列名(单列)</param>
        /// <param name="conditionFieldNames">作为条件列的列名(可多列，用英文状态下逗号隔开)</param>
        /// <param name="conditionValues">查询条件值</param>
        /// <param name="distinct">是否去重</param>
        /// <returns></returns>
        public List<object> GetSingleFieldValueList(string tableName, string targetFieldName, string conditionFieldNames, object[] conditionValues, bool distinct)
        {
            string[] fields = conditionFieldNames.Split(',');
            if (fields.Length != conditionValues.Length)
            {
                throw new Exception("提供的列数与提供的值的个数不匹配！");
            }
            List<object> result = new List<object>();
            List<SqlParameter> paraList = new List<SqlParameter>();
            StringBuilder sql = new StringBuilder(" select ");
            sql.Append(distinct ? " distinct " : string.Empty).Append(" [").Append(targetFieldName).Append("] from [").Append(tableName).Append("] where 1=1 ");
            for (int i = 0; i < fields.Length; i++)
            {
                sql.Append(" And [").Append(fields[i]).Append("]=@").Append(fields[i]);
                paraList.Add(new SqlParameter("@" + fields[i], conditionValues[i]));
            }
            IDataReader reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), paraList.ToArray());
            while (reader.Read())
            {
                result.Add(reader[0]);
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// 根据主键更新一列或多列的值
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>  
        /// <param name="values">值</param>  
        /// <param name="pkFieldName">唯一键字段名称</param> 
        /// <param name="pk">唯一键的查询值</param>  
        /// <returns>受影响的行数</returns> 
        public int UpdateFieldByPk(string tableName, string fields, object[] values, string pkFieldName, string pk)
        {
            string[] strFields = fields.Split(',');
            if (strFields.Length != values.Length)
            {
                throw new Exception("提供的列数与提供的值的个数不匹配！");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("update [").Append(tableName).Append("] set ");
            for (int i = 0; i < strFields.Length; i++)
            {
                sql.Append("[").Append(strFields[i]).Append("] = @").Append(strFields[i]).Append(",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" where [").Append(pkFieldName).Append("] = @PK");
            List<SqlParameter> parasList = new List<SqlParameter>();
            for (int i = 0; i < strFields.Length; i++)
            {
                parasList.Add(new SqlParameter("@" + strFields[i], values[i]));
            }
            parasList.Add(new SqlParameter("@PK", pk));
            SqlParameter[] paras = parasList.ToArray();

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql.ToString(), paras);
        }

        /// <summary>
        /// 根据一个或多个主键跟新一列或多列的值
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>
        /// <param name="values">列对应的要更新的值</param>
        /// <param name="pkFieldNames">主键(如果为多列，请用英文状态下的逗号隔开)</param>
        /// <param name="pkValues">主键对应的值</param>
        /// <returns></returns>
        public int UpdateFieldByPk(string tableName, string fields, object[] values, string pkFieldNames, object[] pkValues)
        {
            string[] strFields = fields.Split(',');
            string[] fieldNames = pkFieldNames.Split(new Char[] { ',' });
            if (strFields.Length != values.Length || fieldNames.Length != pkValues.Length)
            {
                throw new Exception("提供的列数与提供的值的个数不匹配！");
            }
            StringBuilder sql = new StringBuilder();
            List<SqlParameter> parasList = new List<SqlParameter>();
            sql.Append("update [").Append(tableName).Append("] set ");
            for (int i = 0; i < strFields.Length; i++)
            {
                sql.Append("[").Append(strFields[i]).Append("] = @").Append(strFields[i]).Append(",");
                parasList.Add(new SqlParameter("@" + strFields[i], values[i]));
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" where ");
            for (int i = 0; i < fieldNames.Length; i++)
            {
                sql.Append(" [").Append(fieldNames[i]).Append("] = @").Append(fieldNames[i]).Append(" and");
                parasList.Add(new SqlParameter("@" + fieldNames[i], pkValues[i]));
            }
            sql.Remove(sql.Length - 3, 3);
            SqlParameter[] paras = parasList.ToArray();

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql.ToString(), paras);
        }
        #endregion
    }
}
