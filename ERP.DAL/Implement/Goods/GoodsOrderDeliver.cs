using System;
using System.Data.SqlClient;
using ERP.DAL.Interface.IGoods;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Goods
{
    public class GoodsOrderDeliver : IGoodsOrderDeliver
    {
        public GoodsOrderDeliver(GlobalConfig.DB.FromType fromType) { }

        #region  SQL
        private const string SQL_INSERT = @"INSERT INTO GoodsOrderDeliver(OrderId,TotalWeight,CarriageFee,ExpressId,ExpressNo,MaxWrongValue,Province,City) 
VALUES(@OrderId,@TotalWeight,@CarriageFee,@ExpressId,@ExpressNo,@MaxWrongValue,@Province,@City)";
        private const string SQL_UPDATE = @"UPDATE GoodsOrderDeliver SET TotalWeight=@TotalWeight,CarriageFee=@CarriageFee,ExpressId=@ExpressId,ExpressNo=@ExpressNo,MaxWrongValue=@MaxWrongValue WHERE OrderId=@OrderId";
        private const string SQL_DELETE = @"DELETE GoodsOrderDeliver WHERE OrderId=@OrderId";
        private const string SQL_SELECT_SINGLE = @"SELECT TOP 1 OrderId,TotalWeight,CarriageFee,ExpressId,ExpressNo,MaxWrongValue FROM GoodsOrderDeliver WHERE OrderId=@OrderId";
        #endregion

        #region  参数
        private const string PARM_ORDERID = "@OrderId";
        private const string PARM_TOTALWEIGHT = "@TotalWeight";
        private const string PARM_CARRIAGEFEE = "@CarriageFee";
        private const string PARM_EXPRESSID = "@ExpressId";
        private const string PARM_EXPRESSNO = "@ExpressNo";
        private const string PARM_MAXWRONGVALUE = "@MaxWrongValue";
        #endregion

        /// <summary>添加订单快递运费
        /// </summary>
        /// <param name="orderDeliverInfo"></param>
        /// <returns></returns>
        public bool InsertOrderDeliver(GoodsOrderDeliverInfo orderDeliverInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_INSERT, new
                {
                    OrderId = orderDeliverInfo.OrderId,
                    TotalWeight = orderDeliverInfo.TotalWeight,
                    CarriageFee = orderDeliverInfo.CarriageFee,
                    ExpressId = orderDeliverInfo.ExpressId,
                    ExpressNo = orderDeliverInfo.ExpressNo,
                    MaxWrongValue = orderDeliverInfo.MaxWrongValue,
                    Province = orderDeliverInfo.ProvinceName,
                    City = orderDeliverInfo.CityName,
                }) > 0;
            }
        }

        /// <summary> 更新订单快递运费
        /// </summary>
        /// <param name="orderDeliverInfo"></param>
        /// <returns></returns>
        public bool UpdateOrderDeliver(GoodsOrderDeliverInfo orderDeliverInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_UPDATE, new
                {
                    OrderId = orderDeliverInfo.OrderId,
                    TotalWeight = orderDeliverInfo.TotalWeight,
                    CarriageFee = orderDeliverInfo.CarriageFee,
                    ExpressId = orderDeliverInfo.ExpressId,
                    ExpressNo = orderDeliverInfo.ExpressNo,
                    MaxWrongValue = orderDeliverInfo.MaxWrongValue,
                }) > 0;
            }
        }

        public bool DeleteOrderDeliver(Guid orderId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_DELETE, new
                {
                    OrderId = orderId,
                }) > 0;
            }
        }

        /// <summary> 获取订单的快递运费信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public GoodsOrderDeliverInfo GetOrderDeliverInfoByOrderId(Guid orderId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<GoodsOrderDeliverInfo>(SQL_SELECT_SINGLE, new
                {
                    OrderId = orderId,
                });
            }
        }
    }
}
