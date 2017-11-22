using System.Collections.Generic;
using System.Data.SqlClient;
using ERP.DAL.Interface.IShop;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model.ShopFront;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Shop
{  /// <summary>
    /// 加盟店活动图片
    /// </summary>
    public class ShopActivityImageDal : IShopActivityImageDal
    {
        public ShopActivityImageDal(Environment.GlobalConfig.DB.FromType fromType) { }

        /// <summary>
        /// 插入或修改活动图片
        /// </summary>
        /// <returns></returns>
        public bool InsertOrUpdate(ShopActivityImageInfo imageInfo)
        {
            const string SQL = @" IF NOT EXISTS(SELECT ShopActivityImage FROM [ShopActivityImage] WHERE ShopActivityImageType=@ShopActivityImageType)
                                                          begin 
                                                              INSERT INTO ShopActivityImage([ShopActivityImage],[ShopActivityImageType])VALUES(@ShopActivityImage,@ShopActivityImageType)
                                                       end
                                                    else
                                                            begin	                                                                                                
                                                                UPDATE [ShopActivityImage]SET [ShopActivityImage] = @ShopActivityImage WHERE ShopActivityImageType=@ShopActivityImageType
                                                            end";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ShopActivityImage = imageInfo.ShopActivityImage,
                    ShopActivityImageType = imageInfo.ShopActivityImageType,
                }) > 0;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public List<ShopActivityImageInfo> SelectShopActivityImageInfo()
        {
            const string SQL = @"SELECT ShopActivityImage,ShopActivityImageType FROM [ShopActivityImage] ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopActivityImageInfo>(SQL).AsList();
            }
        }
    }
}
