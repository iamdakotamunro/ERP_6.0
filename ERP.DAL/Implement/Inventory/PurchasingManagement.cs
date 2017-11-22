using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class PurchasingManagement : IPurchasingManagement
    {
        public PurchasingManagement(Environment.GlobalConfig.DB.FromType fromType)
        {
        }

        #region[根据日期获取需调拨统计的业务数据]
        /// <summary>
        /// 根据日期获取进入需调拨统计的业务数据
        /// zhangfan added at 2013-Sep-26th
        /// </summary>
        /// <returns></returns>
        public IList<NeedToAllocateInfo> GetNeedToAllocateDataInfos(DateTime startTime, DateTime endTime)
        {
            const string SQL = @"
SELECT 
	[GO].OrderId,GOD.RealGoodsID,[GO].DeliverWarehouseId,SUM(GOD.Quantity) as Quantity
FROM lmshop_GoodsOrder [GO] 
INNER JOIN lmShop_GoodsOrderDetail GOD WITH(NOLOCK) ON [GO].OrderId=GOD.OrderId 
AND [GO].OrderState=6 AND [GO].EffectiveTime >= @startdate AND [GO].EffectiveTime < @enddate
GROUP BY [GO].OrderId,GOD.RealGoodsID,[GO].DeliverWarehouseId";

            var result = new List<NeedToAllocateInfo>();

            var parms = new[]
             {
               new SqlParameter("@startdate",startTime){SqlDbType = SqlDbType.DateTime},
               new SqlParameter("@enddate",endTime){SqlDbType = SqlDbType.DateTime}
             };

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (dr.Read())
                {
                    result.Add(new NeedToAllocateInfo
                                   {
                                       BusinessIdentify = dr["OrderId"] == DBNull.Value ? Guid.Empty : new Guid(dr["OrderId"].ToString()),
                                       RealGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString()),
                                       DeliverWarehouseId = dr["DeliverWarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["DeliverWarehouseId"].ToString()),
                                       Quantity = dr["Quantity"] == DBNull.Value ? 0 : int.Parse(dr["Quantity"].ToString())
                                   });
                }
            }
            return result;
        }
        #endregion
    }
}
