using System;
using System.Collections.Generic;
using System.Text;
using ERP.DAL.Interface.IGoods;
using ERP.Model.Goods;
using Keede.Ecsoft.Model;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using System.Data.SqlClient;
using Dapper;
using Dapper.Extension;
using System.Transactions;

namespace ERP.DAL.Implement.Goods
{
    public class GoodsPriceDao : IGoodsPrice
    {
        public GoodsPriceDao(Environment.GlobalConfig.DB.FromType fromType) { }

        public IList<GoodsFetchPriceInfo> GetFetchPriceList(string goodsIdString)
        {
            const string SQL = @"
SELECT [SiteId],[GoodsGuid] AS GoodsId,GoodsPrice
FROM [lmShop_FetchData]
WHERE [IsChecked] = 1 AND GoodsGuId IN ({0})
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsFetchPriceInfo>(string.Format(SQL, goodsIdString)).AsList();
            }
        }

        /// <summary>
        /// 商品采集价格,根据商品名称模糊搜索
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="goodsName"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public IList<FetchDataInfo> GetFetchDataList(int pageIndex, int pageSize, string goodsName, out long recordCount)
        {
            string sql = @"SELECT 
	                                    Id,SiteName,GoodsName,GoodsPrice,GoodsUrl,GoodsGuId,FD.SiteId,LastUpdateTime,IsChecked 
                                    FROM lmShop_FetchData FD
                                    LEFT JOIN lmShop_FetchSite FS ON FS.SiteId=FD.SiteId 
                                    WHERE GoodsGuId='00000000-0000-0000-0000-000000000000'";

            if (goodsName.Contains(",") || goodsName.Contains("，"))
            {
                var goodsNames = goodsName.Split(',', '，');
                var numGoodsNames = goodsNames.Length;
                for (int i = 0; i < numGoodsNames; i++)
                {
                    if (i == 0)
                    {
                        sql += " And (GoodsName like '%" + goodsNames[i].Replace("'", "''") + "%' ";
                    }
                    else if (i == numGoodsNames - 1)
                    {
                        sql += " Or GoodsName like '%" + goodsNames[i].Replace("'", "''") + "%') ";
                    }
                    else
                    {
                        sql += " Or GoodsName like '%" + goodsNames[i].Replace("'", "''") + "%' ";
                    }
                }
            }
            else
            {
                sql += " And GoodsName like '%" + goodsName.Replace("'", "''") + "%' ";
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", sql));
                return conn.QueryPaged<FetchDataInfo>(sql, pageIndex, pageSize, "GoodsName ASC");
            }
        }

        /// <summary>获取商品的最近进货价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public decimal GetGoodsRecentInPrice(Guid goodsId, Guid warehouseId, Guid filialeId)
        {
            var strbSql = new StringBuilder();
            strbSql.Append("SELECT TOP 1 RecentInPrice FROM lmShop_GoodsStockCurrent WITH(NOLOCK)");
            strbSql.Append(" WHERE (GoodsId='").Append(goodsId).Append("' OR RealGoodsId='").Append(goodsId).Append("')");
            strbSql.Append(" AND WarehouseId='").Append(warehouseId).Append("'");
            strbSql.Append(" AND FilialeId='").Append(filialeId).Append("'");
            strbSql.Append(" ORDER BY RecentCDate DESC");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<decimal>(strbSql.ToString());
            }
        }
    }
}