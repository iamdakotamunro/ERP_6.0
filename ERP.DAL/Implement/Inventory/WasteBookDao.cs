using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Report;
using Keede.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Keede.DAL.RWSplitting;

namespace ERP.DAL.Implement.Inventory
{
    public class WasteBookDao : IWasteBookReport
    {
        /// <summary>
        /// 批量插入交易佣金表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddWasteBook(IList<WasteBookInfo> list)
        {
            var dics = new Dictionary<string, string>
                {
                    {"Id","Id"},{"OrderNo","OrderNo"},{"Income","Income"},{"DateCreated","DateCreated"},{"State","State"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, list, "WasteBook", dics) > 0;
        }

        /// <summary>
        /// 根据创建时间删除未处理的数据
        /// </summary>
        /// <param name="dateCreated">创建时间</param>
        /// <returns></returns>
        public bool DelWasteBook(DateTime dateCreated)
        {
            string sql = @"
            DELETE WasteBook
            WHERE [State]=0 AND DateCreated ='{0}'
            ";
            sql = string.Format(sql, dateCreated);
            
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME,false))
            {
                try
                {
                    return connection.Execute(sql, dateCreated) > 0;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
