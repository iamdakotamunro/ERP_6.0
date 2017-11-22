using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Model;
using ERP.Model.ShopFront;
using Keede.Ecsoft.Model;
using ERP.Environment;
using Keede.DAL.RWSplitting;
using Dapper;
using Dapper.Extension;
using System.Transactions;

namespace ERP.DAL.Implement.Order
{
    public class GoodsOrder : IGoodsOrder
    {
        public GoodsOrder(GlobalConfig.DB.FromType fromType) { }
        public GoodsOrder() { }

        private const string SQL_SELECT_GOODSORDER_FULL = @"
SELECT 
    OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,
    Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,
    TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,
    DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime,
    SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,IsOut,StorageType,HostingFilialeId,ShopId,ConsigneeIdCard
FROM lmShop_GoodsOrder 
WHERE OrderId=@OrderId;";



        private const string SQL_SELECT_GOODSORDER_LIST_BY_INVOICE = @"
SELECT 
OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,Mobile,OldCustomer,PayMode,
PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,
PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,
EffectiveTime,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,HostingFilialeId 
FROM lmShop_GoodsOrder 
WHERE  OrderId IN (SELECT OrderId FROM lmShop_OrderInvoice WHERE InvoiceId=@InvoiceId) 
ORDER BY OrderTime DESC;";




        private const string SQL_GET_ORDER_HALE_HOUR1 = @"DECLARE @SelectTime DATETIME; 
DECLARE @i INT; 
DECLARE @SaleFilialeId uniqueidentifier;
DECLARE @SUMS INT; 
DECLARE @Temp TABLE (dates int, counts INT);
SET @i=0 
SET @SelectTime='{0}' 
WHILE @i<48 
BEGIN 
	select @SUMS=Count(orderId) FROM lmshop_GoodsOrder WITH(NOLOCK) 
	WHERE (ordertime>=@SelectTime and ordertime<DATEADD(mi,30,@SelectTime)) 
	and orderstate {1} 
	insert into @Temp(dates,counts) select @i,@SUMS 
	SET @SelectTime=DATEADD(mi,30,@SelectTime)
	SET @i=@i+1
END 
select counts,dates from @Temp";
        private const string SQL_GET_ORDER_HALE_HOUR2 = @"DECLARE @SelectTime DATETIME; 
DECLARE @i INT; 
DECLARE @SaleFilialeId uniqueidentifier;
DECLARE @SUMS INT; 
DECLARE @Temp TABLE (dates int, counts INT);
SET @i=0 
SET @SelectTime='{0}' 
WHILE @i<48 
BEGIN 
	select @SUMS= Count(orderId) FROM lmshop_GoodsOrder WITH(NOLOCK)
	WHERE (OrderTime >= @SelectTime and OrderTime <DATEADD(mi,30,@SelectTime)) 
	and OrderState {1} {2} 
	insert into @Temp(dates,counts) select @i,@SUMS 
	SET @SelectTime=DATEADD(mi,30,@SelectTime) 
	SET @i=@i+1 
END 
select counts,dates from @Temp";
        private const string SQL_GET_ORDER_HALE_HOUR3 = @"DECLARE @SelectTime DATETIME; 
DECLARE @i INT; 
DECLARE @SaleFilialeId uniqueidentifier;
DECLARE @SUMS INT ; 
DECLARE @Temp TABLE (dates int, counts INT);
SET @i=0 
SET @SelectTime='{0}' 
WHILE @i<48 
BEGIN 
	select @SUMS= Count(orderId) FROM lmshop_GoodsOrder WITH(NOLOCK) 
	WHERE (OrderTime >= @SelectTime and OrderTime <DATEADD(mi,30,@SelectTime)) 
	and OrderState {1} {2} {3}  
	insert into @Temp(dates,counts) select @i,@SUMS 
	SET @SelectTime=DATEADD(mi,30,@SelectTime) 
	SET @i=@i+1  
END 
select counts,dates from  @Temp";

        #region query parameter , query statement
        private const string SQL_INSERT_GOODSORDER = @"
          if(not exists(SELECT 0 FROM lmShop_GoodsOrder  WITH(NOLOCK) WHERE OrderNo=@OrderNo AND OrderState=@OrderState) )
            begin 
            INSERT  INTO lmShop_GoodsOrder([OrderId],[OrderNo],[MemberId],[OrderTime],[Consignee],[Direction],[CountryId],[ProvinceId],[CityId],[PostalCode],[Phone],[Mobile],[OldCustomer],[PayMode],[PayState],[PayType],[BankAccountsId],[BankTradeNo],[RefundmentMode],[ExpressId],[ExpressNo],[TotalPrice],[Carriage],[RealTotalPrice],[PaymentByBalance],[PaidUp],[OrderState],[InvoiceState],[CancleReason],[ConsignTime],[DeliverWarehouseId],[LatencyDay],[Memo],[PromotionValue],[PromotionDescription],EffectiveTime,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,IsOut,[StorageType],[HostingFilialeId])
            VALUES(@OrderId,@OrderNo,@MemberId,@OrderTime,@Consignee,@Direction,@CountryId,@ProvinceId,@CityId,@PostalCode,@Phone,@Mobile,@OldCustomer,@PayMode,@PayState,@PayType,@BankAccountsId,@BankTradeNo,@RefundmentMode,@ExpressId,@ExpressNo,@TotalPrice,@Carriage,@RealTotalPrice,@PaymentByBalance,@PaidUp,@OrderState,@InvoiceState,@CancleReason,@ConsignTime,@DeliverWarehouseId,@LatencyDay,@Memo,@PromotionValue,@PromotionDescription,@EffectiveTime,@SaleFilialeId,@SalePlatformId,@ScoreDeduction,@ScoreDeductionProportion,@DistrictID,@IsOut,@StorageType,@HostingFilialeId);
            end";

        #endregion

        #region --参数
        private const string PARM_ORDERID = "@OrderId";
        private const string PARM_ORDERNO = "@OrderNo";
        private const string PARM_MEMBERID = "@MemberId";
        private const string PARM_ORDERTIME = "@OrderTime";
        private const string PARM_CONSIGNEE = "@Consignee";
        private const string PARM_DIRECTION = "@Direction";
        private const string PARM_COUNTRYID = "@CountryId";
        private const string PARM_PROVINCEID = "@ProvinceId";
        private const string PARM_CITYID = "@CityId";
        private const string PARM_POSTALCODE = "@PostalCode";
        private const string PARM_PHONE = "@Phone";
        private const string PARM_MOBILE = "@Mobile";
        private const string PARM_OLDCUSTOMER = "@OldCustomer";
        private const string PARM_PAYMODE = "@PayMode";
        private const string PARM_PAYSTATE = "@PayState";
        private const string PARM_PAYMENTTYPE = "@PayType";
        private const string PARM_BANKACCOUNTSID = "@BankAccountsId";
        private const string PARM_BANKTRADENO = "@BankTradeNo";
        private const string PARM_REFUNDMENTMODE = "@RefundmentMode";
        private const string PARM_EXPRESSID = "@ExpressId";
        private const string PARM_EXPRESSNO = "@ExpressNo";
        private const string PARM_TOTALPRICE = "@TotalPrice";
        private const string PARM_CARRIAGE = "@Carriage";
        private const string PARM_REALTOTALPRICE = "@RealTotalPrice";
        private const string PARM_PAYMENTBYBALANCE = "@PaymentByBalance";
        private const string PARM_PAIDUP = "@PaidUp";
        private const string PARM_ORDERSTATE = "@OrderState";
        private const string PARM_INVOICESTATE = "@InvoiceState";
        private const string PARM_CANCLEREASON = "@CancleReason";
        private const string PARM_CONSIGNTIME = "@ConsignTime";
        private const string PARM_LATENCYDAY = "@LatencyDay";
        private const string PARM_MEMO = "@Memo";
        private const string PARM_VOUCHERVALUE = "@PromotionValue";
        private const string PARM_VOUCHERXPLAIN = "@PromotionDescription";
        private const string PARM_INVOICEID = "@InvoiceId";
        private const string PARM_STARTTIME = "@StartTime";
        private const string PARM_ENDTIME = "@EndTime";

        private const string PARM_EFFECTIVETIME = "@EffectiveTime";
        private const string PARM_SALEFILIALEID = "@SaleFilialeId";
        private const string PARM_SALEPLATFORMID = "@SalePlatformId";
        private const string PARM_DELIVERWAREHOUSEID = "@DeliverWarehouseId";
        private const string PARM_SCOREDEDUCTION = "@ScoreDeduction";
        private const string PARM_SCOREDEDUCTIONPROPORTION = "@ScoreDeductionProportion";
        private const string PARM_DISTRICTID = "@DistrictID";
        private const string PARM_HOSTINGFILIALEID = "@HostingFilialeId";

        #endregion


        /// <summary>删除订单（含明细，发票处理）（订单同步时先删后加）
        /// </summary>
        /// <param name="orderId"></param>
        public bool Delete(Guid orderId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                var invoiceIdList = conn.Query<Guid>("SELECT InvoiceId FROM lmshop_OrderInvoice WITH(NOLOCK) where OrderId=@OrderId", new { OrderId = orderId });
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (var trans = conn.BeginTransaction())
                {
                    foreach (var invoiceId in invoiceIdList)
                    {
                        var strSql = new StringBuilder();
                        strSql.Append("IF EXISTS(SELECT 1 FROM  lmShop_Invoice WHERE NoteType<0 AND InvoiceId='").Append(invoiceId).AppendLine("')");
                        strSql.AppendLine(" BEGIN");
                        strSql.Append(" DELETE FROM lmshop_OrderInvoice where OrderId='").Append(orderId).AppendLine("'");
                        strSql.Append(" DELETE FROM lmShop_Invoice where InvoiceId='").Append(invoiceId).AppendLine("'");
                        strSql.AppendLine(" END");
                        conn.Execute(strSql.ToString(), null, trans);
                    }
                    conn.Execute("DELETE FROM lmShop_GoodsOrderDetail where OrderId=@OrderId", new { OrderId = orderId }, trans);
                    conn.Execute("DELETE FROM lmShop_GoodsOrder WHERE OrderId=@OrderId", new { OrderId = orderId }, trans);
                    trans.Commit();
                    return true;
                }
            }
        }


        /// <summary>添加订单
        /// </summary>
        public bool Insert(GoodsOrderInfo goodsOrder, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_INSERT_GOODSORDER, new
                    {
                        OrderId = goodsOrder.OrderId,
                        OrderNo = goodsOrder.OrderNo,
                        MemberId = goodsOrder.MemberId,
                        OrderTime = goodsOrder.OrderTime,
                        Consignee = goodsOrder.Consignee,
                        Direction = goodsOrder.Direction,
                        CountryId = goodsOrder.CountryId,
                        ProvinceId = goodsOrder.ProvinceId,
                        CityId = goodsOrder.CityId,
                        PostalCode = goodsOrder.PostalCode,
                        Phone = goodsOrder.Phone,
                        Mobile = goodsOrder.Mobile,
                        OldCustomer = goodsOrder.OldCustomer,
                        PayMode = goodsOrder.PayMode,
                        PayState = goodsOrder.PayState,
                        PayType = goodsOrder.PayType,
                        BankAccountsId = goodsOrder.BankAccountsId,
                        BankTradeNo = goodsOrder.BankTradeNo,
                        RefundmentMode = goodsOrder.RefundmentMode,
                        ExpressId = goodsOrder.ExpressId,
                        ExpressNo = goodsOrder.ExpressNo,
                        TotalPrice = goodsOrder.TotalPrice,
                        Carriage = goodsOrder.Carriage,
                        RealTotalPrice = goodsOrder.RealTotalPrice,
                        PaymentByBalance = goodsOrder.PaymentByBalance,
                        PaidUp = goodsOrder.PaidUp,
                        OrderState = goodsOrder.OrderState,
                        InvoiceState = goodsOrder.InvoiceState,
                        CancleReason = goodsOrder.CancleReason,
                        ConsignTime = goodsOrder.ConsignTime == DateTime.MinValue ? null : new Nullable<DateTime>(goodsOrder.ConsignTime),
                        DeliverWarehouseId = goodsOrder.DeliverWarehouseId,
                        LatencyDay = goodsOrder.LatencyDay,
                        Memo = goodsOrder.Memo,
                        PromotionValue = goodsOrder.PromotionValue,
                        PromotionDescription = goodsOrder.PromotionDescription,
                        EffectiveTime = goodsOrder.EffectiveTime == DateTime.MinValue ? null : new Nullable<DateTime>(goodsOrder.EffectiveTime),
                        SaleFilialeId = goodsOrder.SaleFilialeId,
                        SalePlatformId = goodsOrder.SalePlatformId,
                        ScoreDeduction = goodsOrder.ScoreDeduction,
                        ScoreDeductionProportion = goodsOrder.ScoreDeductionProportion,
                        DistrictID = goodsOrder.DistrictID,
                        IsOut = goodsOrder.IsOut,
                        StorageType = goodsOrder.StorageType,
                        HostingFilialeId = goodsOrder.HostingFilialeId,
                        ShopId = goodsOrder.ShopId,
                        ConsigneeIdCard = goodsOrder.ConsigneeIdCard
                    }) > 0;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>订单按状态与时间柱状图
        /// </summary>
        /// <param name="orderState">状态</param>
        /// <param name="datetime">时间</param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="saleFilialeId"> </param>
        /// <param name="salePlatformId"> </param>
        /// <returns></returns>
        public IList<KeyValuePair<int, int>> GetGoodOrderChart(OrderState[] orderState, DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatformId)
        {
            var sql = new StringBuilder(@"select MONTH(Ordertime) as months,count(0) as counts from lmshop_GoodsOrder where year(Ordertime)=year(@OrderTime) ");
            if (orderState.Length == 1)
            {
                sql.Append(" AND OrderState=@OrderState");
            }
            else
            {
                string states = "(";
                foreach (var orderState1 in orderState)
                {
                    var s = (int)orderState1;
                    states += s + ",";
                }
                states = states.Substring(0, states.LastIndexOf(",", StringComparison.Ordinal)) + ")";
                sql.Remove(0, sql.Length);
                sql.Append("select MONTH(Ordertime) as months,count(0) as counts from lmshop_GoodsOrder WITH(NOLOCK) where year(Ordertime)=year(@OrderTime)  and orderstate in" + states + " group by MONTH(Ordertime) order by Months");
            }
            if (saleFilialeId != Guid.Empty)
            {
                sql.Append(" AND SaleFilialeId=@SaleFilialeId");
            }
            if (salePlatformId != Guid.Empty)
            {
                sql.Append(" AND salePlatformId=@salePlatformId");
            }
            sql.Append("  group by MONTH(Ordertime) order by Months ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(datetime.Year), Transaction.Current == null))
            {
                return conn.Query(sql.ToString(), new
                {
                    OrderState = (int)orderState[0],
                    SaleFilialeId = saleFilialeId,
                    salePlatformId = salePlatformId,
                    OrderTime = datetime
                }).Select(m => new KeyValuePair<int, int>((int)m.months, (int)m.counts)).AsList();
            }
        }

        //TODO:待处理（退单数目）
        /// <summary>按状态与时间柱状图（退单数目）
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="keepyear"></param>
        /// <param name="saleFilialeId"> </param>
        /// <param name="salePlatformId"> </param>
        /// <returns></returns>
        public IList<KeyValuePair<int, int>> GetGoodOrderChart(DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatformId)
        {;
            string sqlstr = string.Empty;
            if (saleFilialeId != Guid.Empty)
            {
                sqlstr = " AND SaleFilialeId=@SaleFilialeId";
            }
            if (salePlatformId != Guid.Empty)
            {
                sqlstr += (" AND SalePlatformId=@salePlatformId");
            }
            string sql = string.Format(@"select month(CreateDate) as months,count(*) as counts from 
(
	select distinct OrderId,CreateDate from lmShop_CheckRefund as cr with(nolock) 
	inner join lmShop_CheckRefundDetails as cfd with(nolock)
	on cr.RefundId=cfd.RefundId 
	where  CheckState=1
	and ReturnType in (2,3,4)
	and year(CreateDate)=year(@OrderTime) 
    {0}
) i1
group by MONTH(i1.CreateDate) order by months", sqlstr);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(datetime.Year), Transaction.Current == null))
            {
                return conn.Query(sql, new
                {
                    SaleFilialeId = saleFilialeId,
                    salePlatformId = salePlatformId,
                    OrderTime = datetime
                }).Select(m => new KeyValuePair<int, int>((int)m.months, (int)m.counts)).AsList();
            }
        }

        /// <summary> 每半个小时订单的统计
        /// </summary>
        /// <returns></returns>
        public IList<KeyValuePair<int, int>> GetOrderHalfHour(DateTime datetime, int[] orderStates, Guid saleFilialeId, Guid salePlatformId)
        {
            string where;
            if (orderStates.Length == 1)
            {
                if (orderStates[0] == -1)
                    where = "<10";
                else if (orderStates[0] == 14)//退货
                    where = @">0 AND OrderId IN (
				                select OrderId from [lmShop_Refund] where RefundId IN (
					                select RefundId from lmshop_Treatment where TreatmentType=2 OR TreatmentId in (select TreatmentId from lmshop_TreatmentDetail where ReasonType=4 and IsReissue=0)
				                ) AND Status=30
			                )";
                else
                    where = "=" + orderStates[0];
            }
            else
            {
                string states = "(";
                foreach (int s in orderStates)
                {
                    states += s + ",";
                }
                states = states.Substring(0, states.LastIndexOf(",", StringComparison.Ordinal)) + ")";
                where = " in" + states;
            }
            string sql = "";

            if (saleFilialeId != Guid.Empty)
            {
                sql = string.Format(SQL_GET_ORDER_HALE_HOUR2, datetime.ToShortDateString(), where, " AND SaleFilialeId='" + saleFilialeId + "' ");
            }

            if (salePlatformId != Guid.Empty)
            {
                sql = string.Format(SQL_GET_ORDER_HALE_HOUR2, datetime.ToShortDateString(), where, "AND SalePlatformId='" + salePlatformId + "' ");
            }

            if (saleFilialeId != Guid.Empty && salePlatformId != Guid.Empty)
            {
                sql = string.Format(SQL_GET_ORDER_HALE_HOUR3, datetime.ToShortDateString(), where, "AND SaleFilialeId='" + saleFilialeId + "' ", "AND SalePlatformId='" + salePlatformId + "' ");
            }

            if (saleFilialeId == Guid.Empty && salePlatformId == Guid.Empty)
            {
                sql = string.Format(SQL_GET_ORDER_HALE_HOUR1, datetime.ToShortDateString(), where);
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(sql).Select(m => new KeyValuePair<int, int>((int)m.counts, (int)m.dates)).AsList();
            }
        }

        /// <summary>按月订单单按状态与时间柱状图
        /// </summary>
        /// <returns></returns>
        public IList<KeyValuePair<int, int>> GetGoodOrderChartDay(OrderState[] orderState, DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatformId)
        {
            var sql = new StringBuilder("select day(Ordertime) as days,count(0) as counts from lmshop_GoodsOrder with(nolock) where OrderTime>=@FromTime and OrderTime<@ToTime ");

            var fromTime = datetime.ToString("yyyy-MM-01");
            var toTime = datetime.AddMonths(1).ToString("yyyy-MM-01");
            if (orderState.Length == 1)
            {
                sql.Append(" AND OrderState=@OrderState");
            }
            else
            {
                string states = "(";
                foreach (var orderState1 in orderState)
                {
                    var s = (int)orderState1;
                    states += s + ",";
                }
                states = states.Substring(0, states.LastIndexOf(",", StringComparison.Ordinal)) + ")";
                sql.Remove(0, sql.Length);
                sql.Append("select day(Ordertime) as days,count(*) as counts from lmshop_GoodsOrder WITH(NOLOCK) where OrderTime>=@FromTime and OrderTime<@ToTime  and orderstate in" + states + " group by day(Ordertime) order by days");
            }
            if (saleFilialeId != Guid.Empty)
            {
                sql.Append(" AND SaleFilialeId=@SaleFilialeId");
            }
            if (salePlatformId != Guid.Empty)
            {
                sql.Append(" AND SalePlatformId=@salePlatformId");
            }
            sql.Append(" group by day(Ordertime) order by days");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(datetime.Year), Transaction.Current == null))
            {
                return conn.Query(sql.ToString(), new
                {
                    FromTime = fromTime,
                    ToTime = toTime,
                    OrderState = (int)orderState[0],
                    SaleFilialeId = saleFilialeId,
                    salePlatformId = salePlatformId,
                }).Select(m => new KeyValuePair<int, int>((int)m.days, (int)m.counts)).AsList();
            }
        }

        //TODO:待处理（退单数目）
        /// <summary>按月（每天）定单按状态与时间柱状图（退单数目）
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="keepyear">保留几年数据</param>
        /// <param name="saleFilialeId"> </param>
        /// <param name="salePlatformId"> </param>
        /// <returns></returns>
        public IList<KeyValuePair<int, int>> GetGoodOrderChartDay(DateTime datetime, int keepyear, Guid saleFilialeId, Guid salePlatformId)
        {
            var sqlstr = string.Empty;
            if (saleFilialeId != Guid.Empty)
            {
                sqlstr = " AND SaleFilialeId=@SaleFilialeId";
            }
            if (salePlatformId != Guid.Empty)
            {
                sqlstr += (" AND SalePlatformId=@SalePlatformId");
            }
            string sql = string.Format(@"
select day(CreateDate) as days,count(*) as counts from 
(
	select distinct OrderId,CreateDate from lmShop_CheckRefund as cr with(nolock) 
	inner join lmShop_CheckRefundDetails as cfd with(nolock)
	on cr.RefundId=cfd.RefundId 
	where  CheckState=1
	and ReturnType in (2,3,4)
	and month(CreateDate)=month(@OrderTime)
	and year(CreateDate)=year(@OrderTime) 
    {0}
) i1
group by day(i1.CreateDate) order by days ", sqlstr);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(datetime.Year), Transaction.Current == null))
            {
                return conn.Query(sql, new
                {
                    SaleFilialeId = saleFilialeId,
                    salePlatformId = salePlatformId,
                    OrderTime = datetime
                }).Select(m => new KeyValuePair<int, int>((int)m.days, (int)m.counts)).AsList();
            }
        }


        /// <summary> 获得全部或地区(省级）| 货到付款、款到发货订单金额分析表
        /// </summary>
        /// <returns></returns>
        public IList<KeyValuePair<int, double>> GetOrderAmountRecord(Guid provinceId, Guid cityId, DateTime startTime, DateTime endTime, int payMode, int[] orderState, int result, int showMode, int keepyear, Guid saleFilialeId, Guid salePlatformId)
        {
            string ordersate = "";
            if (orderState.Length == 1)
            {
                if (orderState[0] == -1)
                {
                    ordersate = "-1";
                }
                else if (orderState[0] == 999)
                {
                    ordersate = "999";
                }
                else
                {
                    ordersate = "(" + orderState[0] + ")";
                }
            }
            else
            {
                var states = orderState.Aggregate("(", (current, s) => current + (s + ","));
                states = states.Substring(0, states.LastIndexOf(",", StringComparison.Ordinal)) + ")";
                ordersate = states;
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(startTime.Year), Transaction.Current == null))
            {
                return conn.Query("P_Raifei_OrderUnitprice_Analyze", new
                {
                    ProvinceId = provinceId,
                    CityId = cityId,
                    StartTime = startTime,
                    EndTime = endTime,
                    PayMode = payMode,
                    ordersate = ordersate,
                    Result = result,
                    ShowMode = showMode,
                    SaleFilialeId = saleFilialeId,
                    salePlatformId = salePlatformId,
                }, commandType: CommandType.StoredProcedure).Select(m => new KeyValuePair<int, double>((int)m.totalcount, (double)m.per)).AsList();
            }
        }

        /// <summary>根据年份和指定条件查询订单
        /// </summary>
        /// <returns></returns>
        public IList<GoodsOrderInfo> GetGoodsOrder(Guid salePlatformId, DateTime startTime, DateTime endTime, Guid orderId, string orderNo, string consignee,
                                            string expressNo, string mobile, Guid memberId, int year, int startPage, int pageSize, out long recordCount)
        {
            var sql = new StringBuilder(@"
SELECT 
    a.OrderId,OrderNo,a.MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode
    ,Phone,Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId
    ,a.ExpressNo,TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime
    ,DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime
    ,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,a.DistrictID 
FROM lmShop_GoodsOrder as a  
WHERE OrderTime BETWEEN @StartTime AND @EndTime ");

            if (salePlatformId != Guid.Empty)
            {
                sql.Append(" AND SalePlatformId='").Append(salePlatformId).Append("' ");
            }
            if (orderId != Guid.Empty)
            {
                sql.Append(" AND OrderId='").Append(orderId).Append("' ");
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                sql.Append(" AND OrderNo=@OrderNo ");
            }
            if (!string.IsNullOrEmpty(consignee))
            {
                sql.Append(" AND Consignee=@Consignee ");
            }
            if (!string.IsNullOrEmpty(expressNo))
            {
                sql.Append(" AND ExpressNo=@ExpressNo ");
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                sql.Append(" AND Mobile=@Mobile ");
            }
            if (memberId != default(Guid))
            {
                sql.Append(" AND MemberId=@MemberId ");
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(startTime.Year), Transaction.Current == null))
            {
                var pageQuery = conn.QueryPaged<GoodsOrderInfo>(sql.ToString(), startPage, pageSize, "OrderTime DESC", new
                {
                    StartTime = startTime == DateTime.MinValue ? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue : startTime,
                    EndTime = endTime == DateTime.MinValue ? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue : endTime,
                    OrderNo = orderNo,
                    Consignee = consignee,
                    ExpressNo = expressNo,
                    Mobile = mobile,
                    MemberId = memberId
                });
                recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", sql.ToString()), new
                {
                    StartTime = startTime == DateTime.MinValue ? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue : startTime,
                    EndTime = endTime == DateTime.MinValue ? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue : endTime,
                    OrderNo = orderNo,
                    Consignee = consignee,
                    ExpressNo = expressNo,
                    Mobile = mobile,
                    MemberId = memberId
                });
                return pageQuery;
            }
        }


        public IList<GoodsOrderInfo> GetOrderList(List<Guid> authWarehouseIds, DateTime startTime, DateTime endTime, Guid goodsId, string identifyCode, List<OrderState> orderStates, int pageIndex, int pageSize, out int recordCount)
        {
            var sql = new StringBuilder(@"
SELECT  a.OrderId,OrderNo,a.MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode
    ,Phone,Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId
    ,a.ExpressNo,TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime
    ,DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime
    ,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,a.DistrictID 
FROM lmShop_GoodsOrder as a  WHERE  a.DeliverWarehouseId IN({0}) AND OrderState IN({1}) ");
            var stateStr = orderStates.Aggregate("", (current, orderState) => current + string.Format("{0}{1}", current.Length == 0 ? "" : ",", (int)orderState));
            var warehouseStr = authWarehouseIds.Aggregate("", (current, warehouseId) => current + string.Format("{0}'{1}'", current.Length == 0 ? "" : ",", warehouseId));
            if (startTime != default(DateTime))
            {
                sql.AppendFormat(" AND OrderTime>='{0}' ", startTime);
            }
            if (endTime != default(DateTime))
            {
                sql.AppendFormat(" AND OrderTime<='{0}' ", endTime);
            }
            if (!string.IsNullOrEmpty(identifyCode))
            {
                sql.AppendFormat(" AND (Consignee='{0}' OR OrderNo='{0}') ", identifyCode);
            }
            if (goodsId != Guid.Empty)
            {
                sql.AppendFormat(" AND OrderId in(select OrderId from lmshop_GoodsOrderDetail where GoodsId='{0}' and OrderId=a.OrderId GROUP BY OrderId)", goodsId);
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(startTime != default(DateTime) ? startTime.Year : DateTime.Now.Year), Transaction.Current == null))
            {
                var pageQuery = conn.QueryPaged<GoodsOrderInfo>(string.Format(sql.ToString(), warehouseStr, stateStr), pageIndex, pageSize, "OrderTime DESC");
                recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", string.Format(sql.ToString(), warehouseStr, stateStr)));
                return pageQuery;
            }
        }

        /// <summary>插入待完成订单
        /// </summary>
        /// <returns></returns>
        public bool InsertWaitConsignmentOrder(Guid orderId, int orderState, string operatorName)
        {
            const string SQL = @"
INSERT INTO WaitConsignmentOrder
(OrderId,OrderState,Operator)
VALUES
(@OrderId,@OrderState,@Operator)
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new {
                    OrderId = orderId,
                    OrderState = orderState,
                    Operator = operatorName
                }) > 0;
            }
        }

        /// <summary>获取待完成订单
        /// </summary>
        /// <returns></returns>
        public IList<WaitConsignmentOrderInfo> GetWaitConsignmentOrder(int top)
        {
            string sql = string.Format(@"
SELECT TOP {0} OrderId,Operator,CreateTime FROM WaitConsignmentOrder WHERE OrderState = 9
ORDER BY CreateTime", top);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<WaitConsignmentOrderInfo>(sql).AsList();
            }
        }

        /// <summary>删除待完成出货单数据
        /// </summary>
        /// <returns></returns>
        public bool DeleteWaitConsignmentOrder(Guid orderId)
        {
            const string SQL = @"
DELETE WaitConsignmentOrder WHERE OrderId = @OrderId
";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    OrderId = orderId,
                }) > 0;
            }
        }

        /// <summary> 根据商品ID获取会员ID集合
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        public IList<Guid> GetMemberIdListByRealGoodsIds(int keepyear, DateTime startTime, DateTime endTime, List<Guid> realGoodsIds)
        {
            IList<Guid> list = new List<Guid>();
            if (realGoodsIds.Count > 0)
            {
                var strbSql = new StringBuilder();
                foreach (Guid realGoodsId in realGoodsIds)
                {
                    if (string.IsNullOrEmpty(strbSql.ToString()))
                        strbSql.Append("'").Append(realGoodsId).Append("'");
                    else
                        strbSql.Append(",'").Append(realGoodsId).Append("'");
                }
                string sql = string.Format("SELECT distinct MemberId FROM lmShop_GoodsOrder WHERE OrderId in (SELECT OrderId FROM lmShop_GoodsOrderDetail WHERE RealGoodsID IN({0})) AND (OrderTime >= '{1}' OR OrderTime <= '{2}')", strbSql, startTime, endTime);

                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(startTime.Year), Transaction.Current == null))
                {
                    return conn.Query(sql).Select(m => (Guid)m.MemberId).AsList();
                }
            }
            return list;
        }

        /// <summary>根据订单号获取订单简单信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public SimpleGoodsOrderInfo GetOrderBasic(string orderNo)
        {
            string sql = string.Format(@"select OrderId,Consignee,Direction,PostalCode,RealTotalPrice,SaleFilialeId from lmShop_GoodsOrder where OrderNo=@OrderNo");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<SimpleGoodsOrderInfo>(sql);
            }
        }

        /// <summary>根据订单号 获取订单信息 如果是货到付款的则返回金额，如果不是则返回默认值
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="realtotalPrice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public bool GetOrderCodRealTotalPrice(string orderNo, out decimal realtotalPrice)
        {
            realtotalPrice = default(decimal);
            if (string.IsNullOrEmpty(orderNo))
            {
                return false;
            }
            string sql = string.Format(@"select RealTotalPrice,PayMode from lmShop_GoodsOrder where OrderNo=@OrderNo");

            GoodsOrderInfo goodsOrderInfo = null;
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                goodsOrderInfo = conn.QueryFirstOrDefault<GoodsOrderInfo>(sql, new { OrderNo = orderNo });
            }
            if (goodsOrderInfo == null)
            {
                return false;
            }
            if (goodsOrderInfo.PayMode == (int)PayMode.COD)
            {
                realtotalPrice = goodsOrderInfo.RealTotalPrice;
            }
            return true;
        }

        /// <summary>根据订单号集合，是否打印发票来获取订单信息 
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public List<GoodsOrderInvoiceInfo> GetGoodsOrderInfoByorderNos(List<string> orderNos)
        {
            var orderNosStr = "'" + string.Join("','", orderNos.ToArray()) + "'";
            string sql = string.Format(@"SELECT OrderId,OrderNo,Consignee,TotalPrice,Carriage,RealTotalPrice,PayMode,Mobile,Phone,Direction,ExpressNo,PromotionValue,PaidUp FROM lmshop_GoodsOrder WITH(NOLOCK) WHERE OrderNo IN({0})", orderNosStr);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderInvoiceInfo>(sql).AsList();
            }
        }

        /// <summary>根据发票号获得订单id和订单号
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public GoodsOrderInfo GetGoodsOrderByInvoiceId(Guid invoiceId)
        {
            string sql = string.Format(@"select a.OrderId,a.OrderNo from lmshop_GoodsOrder a
                                            inner join lmshop_OrderInvoice b
                                            on a.OrderId = b.OrderId
                                            where b.InvoiceId=@InvoiceId");
            var parm = new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier) { Value = invoiceId };
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<GoodsOrderInfo>(sql,new { InvoiceId  = invoiceId });
            }
        }

        /// <summary>更新订单状态为出货中
        /// </summary>
        /// <returns></returns>
        public bool SetGoodsOrderStateToWaitOutbound(List<string> orderNos)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set OrderState='{0}' where OrderNo in ({1})", (Int32)OrderState.WaitOutbound, GetOrderStr(orderNos));
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        /// <summary>
        /// 订单转移发货仓同步到出库中
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="warehouseId"></param>
        /// <param name="storageType"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="expressId"></param>
        /// <returns></returns>
        public bool SetGoodsOrderStateToWaitOutboundNew(List<string> orderNos, Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid expressId)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set OrderState={0},DeliverWarehouseId='{1}',HostingFilialeId='{2}',StorageType={3},ExpressId='{4}',ExpressNo='' where OrderNo in ({5})", (Int32)OrderState.WaitOutbound,warehouseId,hostingFilialeId,storageType,expressId, GetOrderStr(orderNos));
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, sql);
            }
        }

        /// <summary>设置订单快递号
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="expressNo"></param>
        /// <returns></returns>
        public bool SetGoodsOrderExpressNo(List<string> orderNos, string expressNo)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set ExpressNo='{0}' where OrderNo in ({1})", expressNo, GetOrderStr(orderNos));
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        /// <summary>更新订单状态为需调拨
        /// </summary>
        /// <param name="orderNos"></param>
        /// <returns></returns>
        public bool SetGoodsOrderToRedeploy(List<string> orderNos)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set OrderState='{0}' where OrderNo in ({1})", (Int32)OrderState.Redeploy, GetOrderStr(orderNos));
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        /// <summary>
        /// 订单转移发货仓更新到需调拨
        /// </summary>
        /// <param name="orderNos"></param>
        /// <param name="warehouseId"></param>
        /// <param name="storageType"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="expressId"></param>
        /// <returns></returns>
        public bool SetGoodsOrderToRedeployNew(List<string> orderNos, Guid warehouseId, byte storageType, Guid hostingFilialeId, Guid expressId)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set OrderState={0},DeliverWarehouseId='{1}',HostingFilialeId='{2}',StorageType={3},ExpressId='{4}',ExpressNo='' where OrderNo in ({5})", 
                (Int32)OrderState.Redeploy, warehouseId, hostingFilialeId, storageType, expressId, GetOrderStr(orderNos));
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, sql);
            }
        }

        /// <summary>更新订单状态为需采购
        /// </summary>
        /// <param name="orderNos"></param>
        /// <returns></returns>
        public bool SetGoodsOrderToPurchase(List<string> orderNos)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set OrderState='{0}' where OrderNo in ({1})", (Int32)OrderState.RequirePurchase, GetOrderStr(orderNos));
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        /// <summary>更新订单状态为完成
        /// </summary>
        /// <returns></returns>
        public bool SetGoodsOrderToConsignmented(string orderNo)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set OrderState='{0}',ConsignTime=getdate() where OrderNo = @OrderNo", (Int32)OrderState.Consignmented);
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql, new { OrderNo = orderNo }) > 0;
            }
        }

        /// <summary>更新订单状态为作废
        /// </summary>
        /// <returns></returns>
        public bool SetGoodsOrderToCancellation(string orderNo)
        {
            var sql = String.Format(@"update lmshop_GoodsOrder set OrderState='{0}' where OrderNo='{1}'", (Int32)OrderState.Cancellation, orderNo);
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        /// <summary>获取订单ID和订单号字典
        /// </summary>>
        /// <returns></returns>
        public Dictionary<Guid, String> GetOrderNoDicAndWarehouseIdsByOrderIds(List<Guid> orderIds, out Guid warehouseId)
        {
            warehouseId = Guid.Empty;
            var orderNoDic = new Dictionary<Guid, String>();
            if (orderIds.Count <= 0) return orderNoDic;
            var strbSql = new StringBuilder();
            foreach (var realGoodsId in orderIds)
            {
                if (string.IsNullOrEmpty(strbSql.ToString()))
                    strbSql.Append("'").Append(realGoodsId).Append("'");
                else
                    strbSql.Append(",'").Append(realGoodsId).Append("'");
            }
            var sql = string.Format("select OrderId,OrderNo,DeliverWarehouseId FROM lmShop_GoodsOrder where OrderId IN({0}) ", strbSql);
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                var val = conn.Query(sql);
                if (val.Any())
                {
                    orderNoDic = val.ToDictionary(kv => (Guid)kv.OrderId, kv => (string)kv.OrderNo);
                    warehouseId = (Guid)val.First().DeliverWarehouseId;
                }
            }
            return orderNoDic;
        }

        /// <summary>获取会员历史订单记录
        /// </summary>
        /// <returns></returns>
        public IList<GoodsOrderInfo> GetHistoryGoodsOrderByMemberIdAndOrderTime(Guid memberId, DateTime orderTime)
        {
            var SQL = String.Format("SELECT OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime+,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,StorageType,ShopId,ConsigneeIdCard FROM lmshop_GoodsOrder with(nolock) WHERE MemberId='{0}' AND OrderTime<'{1}'", memberId, orderTime);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(orderTime.Year), Transaction.Current == null))
            {
                return conn.Query<GoodsOrderInfo>(SQL).AsList();
            }
        }

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetHistoryOrderInfoByOrderId(Guid orderId, DateTime orderTime)
        {
            var sql = String.Format("SELECT OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,StorageType,ShopId,ConsigneeIdCard FROM lmshop_GoodsOrder with(nolock) WHERE OrderId='{0}' AND OrderTime<'{1}'", orderId, orderTime);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(orderTime.Year), Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<GoodsOrderInfo>(sql);
            }
        }

        /// <summary>获取历史订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetHistoryOrderInfoByOrderNo(String orderNo, DateTime orderTime)
        {
            var sql = String.Format("SELECT OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,StorageType,ShopId,ConsigneeIdCard FROM lmshop_GoodsOrder with(nolock) WHERE OrderNo='{0}' AND OrderTime<'{1}'", orderNo, orderTime);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(orderTime.Year), Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<GoodsOrderInfo>(sql);
            }
        }

        /// <summary>获取会员订单金额，积分列表
        /// </summary>
        /// <returns></returns>
        public IList<OrderScoreInfo> GetOrderScoreListToPage(Guid memeberId, int year, int pageSize, int pageIndex, out long recordCount)
        {
            var sql = new StringBuilder(String.Format(@"SELECT OrderNo,TotalPrice,GD.OrderScore,OrderTime FROM lmShop_GoodsOrder G
INNER JOIN (SELECT OrderId,SUM(GiveScore) AS OrderScore FROM lmShop_GoodsOrderDetail GROUP BY OrderId) GD 
ON G.OrderId=GD.OrderId WHERE MemberId='{0}' AND OrderState='{1}' ", memeberId, (Int32)OrderState.Consignmented));
            var diff = DateTime.Now.Year - year;
            if (diff < 2)
            {
                sql.AppendFormat(" AND OrderTime>'{0}'", DateTime.Now.AddYears(-2));
            }
            else
            {
                sql.AppendFormat(" AND OrderTime>'{0}' AND OrderTime< '{1}'", new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1));
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(year), Transaction.Current == null))
            {
                recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", sql));
                return conn.QueryPaged<OrderScoreInfo>(sql.ToString(), pageIndex, pageSize, "OrderTime DESC");
            }
        }


        //TODO:出入库记录，需要转移
        /// <summary>根据商品ID获取特定条件下的单据是否存在()
        /// </summary>
        /// <param name="goodsId">主商品Id</param>
        /// <param name="realGoodsIds">子商品Id列表</param>
        /// <param name="stockTypes">单据类型列表</param>
        /// <param name="days">查询时限,多少天内</param>
        /// <param name="stockStates">单据状态</param>
        /// <returns></returns>
        public bool SelectSemiStockAtOneYearByGoodsId(Guid goodsId, List<Guid> realGoodsIds, List<int> stockTypes, int days, List<int> stockStates)
        {
            var strbSql = new StringBuilder();
            strbSql.AppendLine("DECLARE @cdate datetime");
            strbSql.AppendLine(" SELECT @cdate=[DateCreated] FROM StorageRecord sr with(nolock)");
            strbSql.AppendLine(" INNER JOIN StorageRecordDetail srd with(nolock) ON srd.StockId=sr.StockId");
            strbSql.Append(" WHERE (GoodsId='").Append(goodsId).Append("'");
            if (realGoodsIds != null && realGoodsIds.Count > 0)
            {
                var strb = new StringBuilder();
                foreach (var id in realGoodsIds)
                {
                    if (strb.Length == 0)
                        strb.Append(id);
                    else
                        strb.Append(",").Append(id);
                }
                strbSql.Append(" OR RealGoodsId IN (SELECT id as RealGoodsId FROM splitToTable('" + strb + "',','))");
            }
            strbSql.AppendLine(")");
            if (stockTypes != null && stockTypes.Count != 0)
            {
                string types = "";
                for (int i = 0; i < stockTypes.Count; i++)
                {
                    if (i == stockTypes.Count - 1)
                    {
                        types += stockTypes[i];
                    }
                    else
                    {
                        types += stockTypes[i] + ",";
                    }
                }
                strbSql.AppendFormat(" AND StockType IN({0}) ", types);
            }
            if (stockStates != null && stockStates.Count != 0)
            {
                string states = "";
                for (int i = 0; i < stockStates.Count; i++)
                {
                    if (i == stockStates.Count - 1)
                    {
                        states += stockStates[i];
                    }
                    else
                    {
                        states += stockStates[i] + ",";
                    }
                }
                strbSql.AppendFormat(" AND StockState IN({0}) ", states);
            }
            strbSql.AppendLine(" ORDER BY DateCreated ASC; SELECT DATEDIFF(DD,@cdate,GETDATE()) as DiffDay");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                var objDiffDay = conn.ExecuteScalar(strbSql.ToString());
                var diffDay = objDiffDay == null ? int.MaxValue : (int)objDiffDay;
                if (diffDay > days)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary> 会员订单搜索
        /// </summary>
        public IList<GoodsOrderInfo> GetOrderList(DateTime start, DateTime end, OrderState state, string orderNo, Guid memberId, string mobile, string consignee, Guid warehouseId, string expressNo, Guid saleFilialeId, Guid salePlatformId, int keepyear, int startPage, int pageSize, out long recordCount)
        {
            const string sql = @"
SELECT 
    a.OrderId,OrderNo,a.MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode
    ,Phone,Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode
    ,ExpressId,a.ExpressNo,TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState
    ,InvoiceState,CancleReason,ConsignTime,DeliverWarehouseId,LatencyDay,Memo,PromotionValue
    ,PromotionDescription,EffectiveTime,SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID 
FROM lmShop_GoodsOrder a with(nolock)
WHERE 1=1 ";
            var sbSql = new StringBuilder(sql);
            if (!string.IsNullOrEmpty(orderNo))
            {
                sbSql.Append(" AND OrderNo='").Append(orderNo).Append("'");
            }
            if (state != OrderState.All)
            {
                sbSql.Append(" AND OrderState=").Append((int)state).Append("");
            }
            if (memberId != Guid.Empty)
            {
                sbSql.Append(" AND MemberId='").Append(memberId).Append("'");
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                sbSql.Append(" AND Mobile='").Append(mobile).Append("'");
            }
            if (!string.IsNullOrEmpty(consignee))
            {
                sbSql.Append(" AND Consignee='").Append(consignee).Append("'");
            }
            if (warehouseId != Guid.Empty)
            {
                sbSql.Append(" AND DeliverWarehouseId='").Append(warehouseId).Append("'");
            }
            if (!string.IsNullOrEmpty(expressNo))
            {
                sbSql.Append(" AND ExpressNo='").Append(expressNo).Append("'");
            }
            if (saleFilialeId != Guid.Empty)
            {
                sbSql.Append(" AND SaleFilialeId='").Append(saleFilialeId).Append("'");
            }
            if (salePlatformId != Guid.Empty)
            {
                sbSql.Append(" AND SalePlatformId='").Append(salePlatformId).Append("'");
            }
            //如果是使用订单号查询，则时间搜索条件不加，DBA龚确定（2016-01-25）
            if (string.IsNullOrEmpty(orderNo))
            {
                sbSql.Append(" AND OrderTime>='").Append(start).Append("' AND OrderTime<'").Append(end).Append("'");
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(start.Year), Transaction.Current == null))
            {
                recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", sbSql));
                return conn.QueryPaged<GoodsOrderInfo>(sbSql.ToString(), startPage, pageSize, "OrderTime DESC");
            }
        }

        /// <summary>获取订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetGoodsOrder(Guid orderId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<GoodsOrderInfo>(SQL_SELECT_GOODSORDER_FULL, new { OrderId = orderId });
            }
        }

        /// <summary>获取订单信息
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo GetGoodsOrder(String orderNo)
        {
            const string sql = @"
SELECT 
    OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,
    Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,
    TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,
    DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime,
    SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,IsOut,StorageType,HostingFilialeId,ShopId,ConsigneeIdCard
FROM lmShop_GoodsOrder 
WHERE OrderNo=@OrderNo ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<GoodsOrderInfo>(sql, new { OrderNo = orderNo });
            }
        }


        /// <summary> 获取指定发票中包含的订单
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public IList<GoodsOrderInfo> GetInvoiceGoodsOrderList(Guid invoiceId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderInfo>(SQL_SELECT_GOODSORDER_LIST_BY_INVOICE, new { InvoiceId = invoiceId }).AsList();
            }
        }

        /// <summary>获取指定的订单号是否存在
        /// </summary>
        /// <param name="orderNo">订单编号</param>
        /// <returns></returns>
        public bool IsOrderNo(string orderNo)
        {
            const string sql = "SELECT 0 FROM lmShop_GoodsOrder WITH(NOLOCK) WHERE OrderNo=@OrderNo;";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(sql, new { OrderNo = orderNo }).Any();
            }
        }

        /// <summary>订单额月排行
        /// </summary>
        /// <returns></returns>
        public IList<KeyValuePair<int, decimal>> GetQueryOrderByFinancial(int nonceYear, OrderState orderstate,
            int statisticsType, Guid countryId, Guid provinceid, Guid cityId, int keepyear, Guid saleFilialeId,
            Guid salePlatformId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(nonceYear), Transaction.Current == null))
            {
                return conn.Query("P_Riafei_QueryOrderByFinancial", new
                {
                    OrderTime = nonceYear,
                    OrderState = (int)orderstate,
                    StatisticsType = statisticsType,
                    CountryId = countryId,
                    provinceid = provinceid,
                    CityId = cityId,
                    SaleFilialeId = saleFilialeId,
                    SalePlatformId = salePlatformId,
                }, commandType: CommandType.StoredProcedure).Select(m => new KeyValuePair<int, decimal>((int)m.Months, (decimal)m.PaidUp)).AsList();
            }
        }

        private String GetOrderStr(IEnumerable<string> orderNos)
        {
            var orderNoStr = String.Empty;
            foreach (var condition in orderNos)
            {
                if (String.IsNullOrEmpty(orderNoStr))
                    orderNoStr += "'" + condition + "'";
                else
                    orderNoStr += ",'" + condition + "'";
            }
            return orderNoStr;
        }

        /// <summary>
        /// 可以从数据库中取出订单，不需要有仓库权限
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="keepyear"></param>
        /// <param name="orderTime"></param>
        /// <returns></returns>
        public GoodsOrderInfo GetGoodsOrder(string orderNo, int keepyear, DateTime orderTime)
        {
            const string SQL = @"
SELECT 
    OrderId,OrderNo,MemberId,OrderTime,Consignee,Direction,CountryId,ProvinceId,CityId,PostalCode,Phone,
    Mobile,OldCustomer,PayMode,PayState,PayType,BankAccountsId,BankTradeNo,RefundmentMode,ExpressId,ExpressNo,
    TotalPrice,Carriage,RealTotalPrice,PaymentByBalance,PaidUp,OrderState,InvoiceState,CancleReason,ConsignTime,
    DeliverWarehouseId,LatencyDay,Memo,PromotionValue,PromotionDescription,EffectiveTime,
    SaleFilialeId,SalePlatformId,ScoreDeduction,ScoreDeductionProportion,DistrictID,StorageType,SaleFilialeId AS HostingFilialeId   
FROM lmShop_GoodsOrder WHERE OrderNo=@OrderNo;";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.GetErpDbName(orderTime.Year), Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<GoodsOrderInfo>(SQL, new
                {
                    OrderNo = orderNo,
                });
            }
        }

        public List<StoTempInfo> GetStoTempList(int top)
        {
            var sql = String.Format(@"SELECT  TOP {0}
	   [OrderNo]
      ,[ExpressNo] as billNo
      ,[Consignee] as name
      ,[Direction] as address
      ,[ShortAddress] as districtCode
      ,[PackageCenterName] as Container
      ,[Province] as prov
      ,[City] as city
      ,[District] as district  FROM StoTemp 
WHERE IsHandled=0 
ORDER BY OrderTime ASC", top == 0 ? 1000 : top);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<StoTempInfo>(sql).AsList();
            }
        }

        public void SetStoTempHandled(string orderNo)
        {
            var sql = String.Format("UPDATE StoTemp SET IsHandled=1 WHERE OrderNo='{0}'", orderNo);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(sql);
            }
        }

        public List<TempOrderInfo> TempOrderList(Int32 top, DateTime fromOrderTime, DateTime toOrderTime)
        {
            var sql = String.Format(@"SELECT TOP {0} OrderId,DeliverFilialeId,DeliverWarehouseId,OrderTime,SaleFilialeId,SalePlatformId FROM lmshop_GoodsOrder  WITH(NOLOCK) 
WHERE   OrderState <=9 AND OrderTime BETWEEN '{1}' AND '{2}'
AND SaleFilialeId='7AE62AF0-EB1F-49C6-8FD1-128D77C84698'
AND OrderId not in (select OrderID from lmshop_GoodsTempOrder with(nolock))", top, fromOrderTime, toOrderTime);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<TempOrderInfo>(sql).AsList();
            }
        }

        public bool InsertGoodsTempOrder(Guid orderId)
        {
            var sql = String.Format("INSERT INTO [dbo].[lmshop_GoodsTempOrder] ([OrderID]) VALUES ('{0}')", orderId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }

        public bool IsCalculated(Guid orderId)
        {
            var sql = String.Format(@"SELECT 1  FROM [dbo].[lmshop_GoodsTempOrder] WITH(NOLOCK) WHERE OrderID='{0}'", orderId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(sql) == 1;
            }
        }

        public void RenewGoodsDaySalesStatisticsOfSellPrice(decimal subtractSellPrice, Guid deliverFilialeId, Guid deliverWarehouseId, DateTime dayTime, Guid realGoodsId, Guid salePlatformId)
        {
            var sql = String.Format(@"UPDATE lmshop_GoodsDaySalesStatistics SET SellPrice=SellPrice-({0}) WHERE 
            DeliverFilialeId='{1}' 
            AND DeliverWarehouseId='{2}'
            AND RealGoodsID='{3}' 
            AND DayTime='{4}'
            AND SalePlatformId='{5}'", subtractSellPrice, deliverFilialeId, deliverWarehouseId, realGoodsId, dayTime.ToShortDateString(), salePlatformId);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(sql);
            }
        }

        /// <summary>第三方订单直接完成     ADD   陈重文
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="expressId">订单快递Id</param>
        /// <param name="expressNo">快递号</param>
        /// <returns></returns>
        public bool ThirdPartyOrderDirectlyToComplete(string orderNo, Guid expressId, string expressNo)
        {
            var sql = String.Format(@"UPDATE lmshop_GoodsOrder SET OrderState={0},ExpressId='{1}',ExpressNo='{2}',ConsignTime=GETDATE() WHERE OrderNo='{3}'", (Int32)OrderState.Consignmented, expressId, expressNo, orderNo);

            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(sql) > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 为处理作废订单销量使用
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// zal 2017-03-13
        public IList<GoodsOrderInfo> GetGoodsOrderInfoListForHistory(int pageIndex, int pageSize)
        {
            var sql = @"
            select Num,OrderId,OrderNo,DeliverWarehouseId,SaleFilialeId,SalePlatformId,OrderTime,HostingFilialeId from (
            select CONVERT(int,row_number() over(order by OrderTime asc)) as Num,a.OrderId,a.OrderNo,DeliverWarehouseId,SaleFilialeId,SalePlatformId,OrderTime,HostingFilialeId 
            from lmshop_GoodsOrder a with(nolock)
            inner join OrderIdTable b with(nolock) on a.OrderId = b.OrderId
            and b.Flag = 0
            )t
            where t.Num>=" + ((pageIndex - 1) * pageSize + 1) + @" and t.Num<=" + pageIndex * pageSize + @"
            ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<GoodsOrderInfo>(sql).AsList();
            }
        }

        /// <summary>
        /// 更新OrderIdTable表中的状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool UpdateOrderIdTable(Guid orderId)
        {
            var sql = @"
            update OrderIdTable
            set Flag=1
            where OrderId='" + orderId + "'";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(sql) > 0;
            }
        }
    }
}

