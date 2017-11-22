using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IBasis;
using Keede.Ecsoft.Model;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Basis
{
    /// <summary>基本单位 2015-05-08  陈重文  （优化代码，增加注释，去除无使用方法）
    /// </summary>
    public class Units : IUnits
    {
        public Units(Environment.GlobalConfig.DB.FromType fromType) { }

        #region [SQL]
        private const string SQL_INSERT_UNITS = "INSERT INTO lmShop_Units VALUES(@UnitsId,@Units);";
        private const string SQL_UPDATE_UNITS = "UPDATE lmShop_Units SET Units=@Units WHERE UnitsId=@UnitsId;";
        private const string SQL_DELETE_UNITS = "DELETE FROM lmShop_Units WHERE UnitsId=@UnitsId;";
        private const string SQL_SELECT_UNITS_ID = "SELECT UnitsId,Units FROM lmShop_Units WHERE UnitsId=@UnitsId;";
        private const string SQL_SELECT_UNITS_LIST = "SELECT UnitsId,Units FROM lmShop_Units;";
        #endregion

        /// <summary>添加基本单位
        /// </summary>
        /// <param name="units">单位名称</param>
        public int Insert(UnitsInfo units)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_INSERT_UNITS, new { UnitsId = units.UnitsId, Units = units.Units });
            }
        }

        /// <summary> 更新基本单位
        /// </summary>
        /// <param name="units">单位名称</param>
        public int Update(UnitsInfo units)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_UPDATE_UNITS, new { UnitsId = units.UnitsId, Units = units.Units });
            }
        }

        /// <summary> 删除基本单位
        /// </summary>
        /// <param name="unitsId">基本单位编号</param>
        public int Delete(Guid unitsId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_DELETE_UNITS, new { UnitsId = unitsId });
            }
        }

        /// <summary> 获取基本单位
        /// </summary>
        /// <param name="unitsId">基本单位Id</param>
        /// <returns>返回基本单位类实例</returns>
        public UnitsInfo GetUnits(Guid unitsId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<UnitsInfo>(SQL_SELECT_UNITS_ID, new { UnitsId = unitsId });
            }
        }

        /// <summary> 获取基本单位列表
        /// </summary>
        /// <returns>返回基本单位列表</returns>
        public IList<UnitsInfo> GetUnitsList()
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<UnitsInfo>(SQL_SELECT_UNITS_LIST).AsList();
            }
        }
    }
}
