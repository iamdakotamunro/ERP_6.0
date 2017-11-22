using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ERP.DAL.Interface.ICompany;
using ERP.Environment;
using Keede.Ecsoft.Model;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Company
{
    public class ComparePrice : IComparePrice
    {
        public ComparePrice(GlobalConfig.DB.FromType fromType) { }

        public int AddProduct(FetchDataInfo product)
        {
            const string SQL = "INSERT lmShop_FetchData([GoodsName],[GoodsPrice],[GoodsUrl],[GoodsGuId],[SiteId],[LastUpdateTime],IsChecked) VALUES(@GoodsName,@GoodsPrice,@GoodsUrl,@GoodsGuId,@SiteId,@date,@IsChecked)";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    Id = product.Id,
                    GoodsName = product.GoodsName,
                    GoodsPrice = product.GoodsName,
                    GoodsUrl = product.GoodsUrl,
                    GoodsGuId = product.GoodsId,
                    SiteId = product.SiteId,
                    date = product.LastUpdateTime,
                    IsChecked = product.IsChecked,
                });
            }
        }

        public int ModifyProduct(FetchDataInfo product)
        {
            const string SQL = "update lmShop_FetchData set goodsname=@goodsname,GoodsPrice=@GoodsPrice,GoodsUrl=@GoodsUrl,LastUpdateTime=@date,IsChecked=@IsChecked where Id=@Id";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    Id = product.Id,
                    GoodsName = product.GoodsName,
                    GoodsPrice = product.GoodsName,
                    GoodsUrl = product.GoodsUrl,
                    GoodsGuId = product.GoodsId,
                    SiteId = product.SiteId,
                    date = product.LastUpdateTime,
                    IsChecked = product.IsChecked,
                });
            }
        }

        public int UpdateBinding(int id, Guid goodsId)
        {
            const string SQL = "UPDATE lmShop_FetchData SET [GoodsGuId]=@goodsId WHERE [Id]=@id";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    goodsId = goodsId,
                    id = id,
                });
            }
        }

        /// <summary>
        /// 根据商品ID 获取
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public IList<FetchDataInfo> GetFetchDataListByGoodsId(Guid goodsId)
        {
            IList<FetchDataInfo> list = new List<FetchDataInfo>();
            const string SQL = @"SELECT FS.[SiteId],[SiteName],[Id],[GoodsName],[GoodsPrice],[GoodsUrl],[GoodsGuId],[LastUpdateTime],[IsChecked] 
                                 FROM [lmShop_FetchSite] FS 
                                 LEFT JOIN(SELECT Id, GoodsName, GoodsPrice, GoodsUrl, GoodsGuId, SiteId, LastUpdateTime, IsChecked FROM [lmShop_FetchData] where [GoodsGuId]=@GoodsGuId) FD ON FD.[SiteId]=FS.[SiteId] 
                                 ORDER BY FS.[SiteId],[GoodsGuId]";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<FetchDataInfo>(SQL, new
                {
                    GoodsGuId = goodsId,
                }).AsList();
            }
        }
        
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public bool SetChecked(int id, bool isChecked)
        {
            const string SQL = @"UPDATE [lmShop_FetchData] SET [IsChecked]=@IsChecked WHERE Id=@Id";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    IsChecked = isChecked,
                    Id = id
                }) > 0;
            }
        }
    }
}
