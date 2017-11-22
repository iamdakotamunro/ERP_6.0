using System;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class ShippAddressDal : IShippAddress
    {
        public ShippAddressDal()
        {
        }

        /// <summary>
        /// 根据“发货仓库ID”和“销售公司ID或销售平台ID”获取发货地址信息
        /// </summary>
        /// <param name="deliverWarehouseId">发货仓库ID</param>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <param name="salePlatformId">销售平台ID</param>
        /// <returns></returns>
        public ShippAddress GetShippAddress(Guid deliverWarehouseId, Guid saleFilialeId, Guid salePlatformId)
        {
            string sql = @"SELECT [AddressDetail]
                          ,[Province]
                          ,[City]
                          ,[Area]
                          ,[Town]
                          ,[CompanyName]
                          ,[ContactWay]
                          ,[Postcode]
                          ,[DeliverWarehouseId]
                          ,[SaleCompanyId]
                           FROM [ShippAddress]
                           WHERE [DeliverWarehouseId]=@DeliverWarehouseId AND ([SaleCompanyId]=@SaleFilialeId OR [SaleCompanyId]=@SalePlatformId)";

     
            SqlParameter[] paras = {
                new SqlParameter("@DeliverWarehouseId", deliverWarehouseId),
                new SqlParameter("@SaleFilialeId", saleFilialeId),
                new SqlParameter("@SalePlatformId", salePlatformId)
            };

            using (var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras))
            {
                if (reader.Read())
                {
                    var shippAddress = new ShippAddress();
                    shippAddress.Province = reader["Province"] == DBNull.Value ? string.Empty : reader["Province"].ToString();
                    shippAddress.City = reader["City"] == DBNull.Value ? string.Empty : reader["City"].ToString();
                    shippAddress.Area = reader["Area"] == DBNull.Value ? string.Empty : reader["Area"].ToString();
                    shippAddress.Town = reader["Town"] == DBNull.Value ? string.Empty : reader["Town"].ToString();
                    shippAddress.AddressDetail = reader["AddressDetail"] == DBNull.Value ? string.Empty : reader["AddressDetail"].ToString();
                    shippAddress.CompanyName = reader["CompanyName"] == DBNull.Value ? string.Empty : reader["CompanyName"].ToString();
                    shippAddress.ContactWay = reader["ContactWay"] == DBNull.Value ? string.Empty : reader["ContactWay"].ToString();
                    shippAddress.Postcode = reader["Postcode"] == DBNull.Value ? string.Empty : reader["Postcode"].ToString();
                    shippAddress.DeliverWarehouseId = reader["DeliverWarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(reader["DeliverWarehouseId"].ToString());
                    shippAddress.SaleCompanyId = reader["SaleCompanyId"] == DBNull.Value ? Guid.Empty : new Guid(reader["SaleCompanyId"].ToString());
                    return shippAddress;
                }
                return null;
            }
        }
    }
}
