using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 订单模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class GoodsOrderInfo
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 会员ID
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [DataMember]
        public string Direction { get; set; }

        /// <summary>
        /// 国家ID
        /// </summary>
        [DataMember]
        public Guid CountryId { get; set; }

        /// <summary>
        /// 省ID
        /// </summary>
        [DataMember]
        public Guid ProvinceId { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        [DataMember]
        public Guid CityId { get; set; }

        /// <summary>
        /// 地区ID
        /// </summary>
        [DataMember]
        public Guid DistrictID { get; set; }

        /// <summary>
        /// 邮政编号
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 新老会员类型
        /// </summary>
        [DataMember]
        public int OldCustomer { get; set; }

        /// <summary>
        /// 支付模式
        /// </summary>
        [DataMember]
        public int PayMode { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        [DataMember]
        public int PayState { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        [DataMember]
        public int PayType { get; set; }

        /// <summary>
        /// 银行账号ID
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// 银行交易号
        /// </summary>
        [DataMember]
        public string BankTradeNo { get; set; }

        /// <summary>
        /// 退款模式
        /// </summary>
        [DataMember]
        public int RefundmentMode { get; set; }

        /// <summary>
        /// 对应快递公司ID
        /// </summary>
        [DataMember]
        public Guid ExpressId { get; set; }

        /// <summary>
        /// 快递编号
        /// </summary>
        [DataMember]
        public string ExpressNo { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 运输费
        /// </summary>
        [DataMember]
        public decimal Carriage { get; set; }

        /// <summary>
        /// 实际应收
        /// </summary>
        [DataMember]
        public decimal RealTotalPrice { get; set; }

        /// <summary>
        /// 余额支付
        /// </summary>
        [DataMember]
        public decimal PaymentByBalance { get; set; }

        /// <summary>
        /// 实际支付
        /// </summary>
        [DataMember]
        public decimal PaidUp { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public int OrderState { get; set; }

        /// <summary>
        /// 发票状态
        /// </summary>
        [DataMember]
        public int InvoiceState { get; set; }

        /// <summary>
        /// 取消原因
        /// </summary>
        [DataMember]
        public string CancleReason { get; set; }

        /// <summary>
        /// 托运时间
        /// </summary>
        [DataMember]
        public DateTime ConsignTime { get; set; }

        /// <summary>
        /// 等待天数
        /// </summary>
        [DataMember]
        public int LatencyDay { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// 有效时间
        /// </summary>
        [DataMember]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// 促销价值
        /// </summary>
        [DataMember]
        public decimal PromotionValue { get; set; }

        /// <summary>
        /// 促销说明
        /// </summary>
        [DataMember]
        public string PromotionDescription { get; set; }

        /// <summary>
        /// 配货单号
        /// </summary>
        [DataMember]
        public string PickNo { get; set; }

        /// <summary>
        /// 代收未成功
        /// </summary>
        [DataMember]
        public bool CommissionFailure { get; set; }

        /// <summary>
        /// 快递
        /// </summary>
        [DataMember]
        public string Express { get; set; }

        /// <summary>
        /// 售后处理时间
        /// </summary>
        [DataMember]
        public string ReturnTime { get; set; }

        /// <summary>
        /// 财务运费补贴
        /// </summary>
        [DataMember]
        public decimal CarriageSubsidy { get; set; }

        /// <summary>
        /// 退货单号
        /// </summary>
        [DataMember]
        public string ReturnOrder { get; set; }

        /// <summary>
        /// 退换货ID(用于查看订单是否进过售后处理)
        /// add by fanguan at 2012-07-03
        /// </summary>
        [DataMember]
        public Guid RefundId { get; set; }

        /// <summary>
        /// 发货形式（门店字段）
        /// </summary>
        [DataMember]
        public int SendType { get; set; }

        /// <summary>
        /// 销售公司ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 发货仓库ID
        /// </summary>
        [DataMember]
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        [IgnoreDataMember]
        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// 使用了多少积分来抵扣钱【全卖积分不算入内】
        /// </summary>
        [DataMember]
        public int ScoreDeduction { get; set; }

        /// <summary>
        /// 使用积分抵扣时的比例（多少积分抵1块钱）
        /// </summary>
        [DataMember]
        public int ScoreDeductionProportion { get; set; }

        /// <summary>
        /// 是否报税
        /// </summary>
        /// <remarks>目前所有都要报税 By Jerry Bai 2017/6/2</remarks>
        [IgnoreDataMember]
        public bool IsOut
        {
            get { return true; }
            set { }
        }

        /// <summary>储位（对应WMS枚举 StorageType）
        /// </summary>
        [DataMember]
        public Byte StorageType { get; set; }

        /// <summary>店铺ID
        /// </summary>
        public Guid ShopId { get; set; }

        /// <summary>收货人身份证号
        /// </summary>
        public String ConsigneeIdCard { get; set; }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public GoodsOrderInfo()
        {
            ConsignTime = DateTime.MinValue;
            ExpressId = Guid.Empty;
            BankAccountsId = Guid.Empty;
            PayState = 1;
        }


        /// <summary>
        /// This model just used in only one place : goodsdemand.GetGoodsOrderInfoList(goodsId, string.IsNullOrEmpty(StartTime)?DateTime.MinValue.ToString():StartTime, string.IsNullOrEmpty(EndTime)?DateTime.MaxValue.ToString():EndTime, CompanyId, OrderState, FilialeId)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderNo"></param>
        /// <param name="payMode"></param>
        /// <param name="payState"></param>
        /// <param name="orderState"></param>
        /// <param name="memo"></param>
        public GoodsOrderInfo(Guid orderId, string orderNo, int payMode, int payState, int orderState, string memo)
        {
            ConsignTime = DateTime.MinValue;
            ExpressId = Guid.Empty;
            BankAccountsId = Guid.Empty;
            OrderId = orderId;
            OrderNo = orderNo;
            PayMode = payMode;
            PayState = payState;
            OrderState = orderState;
            Memo = memo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderNo">订单号</param>
        /// <param name="memberId">会员ID</param>
        /// <param name="orderTime">下单时间</param>
        /// <param name="consignee">收货人</param>
        /// <param name="direction">说明</param>
        /// <param name="countryId">国家ID</param>
        /// <param name="provinceId">省ID</param>
        /// <param name="cityId">城市ID</param>
        /// <param name="postalCode">邮政编码</param>
        /// <param name="phone">电话</param>
        /// <param name="mobile">手机</param>
        /// <param name="oldCustomer">新老会员类型</param>
        /// <param name="payMode">支付方式</param>
        /// <param name="payState">支付状态</param>
        /// <param name="payType">支付类型</param>
        /// <param name="bankAccountsId">银行账户ID</param>
        /// <param name="bankTradeNo">银行交易号</param>
        /// <param name="refundmentMode">退款方式</param>
        /// <param name="expressId">对应快递公司ID</param>
        /// <param name="expressNo">快递编号</param>
        /// <param name="totalPrice">最高价</param>
        /// <param name="carriage">运输费</param>
        /// <param name="realTotalPrice">实际应收</param>
        /// <param name="paymentByBalance">平均付款</param>
        /// <param name="paidUp">实际支付</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancleReason">取消原因</param>
        /// <param name="consignTime">托运时间</param>
        /// <param name="deliverWarehouseId">发货仓库ID</param>
        /// <param name="latencyDay">等待天数</param>
        /// <param name="memo">备注</param>
        /// <param name="promotionValue">促销价值</param>
        /// <param name="promotionDescription">促销说明</param>
        /// <param name="effectivTime">有效时间</param>
        /// <param name="saleFilialeId">销售公司ID </param>
        /// <param name="salePlatformId">门店公司ID，网站ID </param>
        public GoodsOrderInfo(Guid orderId, string orderNo, Guid memberId, DateTime orderTime, string consignee,
                              string direction, Guid countryId, Guid provinceId, Guid cityId, string postalCode,
                              string phone, string mobile, int oldCustomer, int payMode, int payState, int payType,
                              Guid bankAccountsId, string bankTradeNo, int refundmentMode, Guid expressId,
                              string expressNo, decimal totalPrice, decimal carriage, decimal realTotalPrice,
                              decimal paymentByBalance, decimal paidUp, int orderState, int invoiceState,
                              string cancleReason, DateTime consignTime, Guid deliverWarehouseId,
                              int latencyDay, string memo, decimal promotionValue,
                              string promotionDescription, DateTime effectivTime, Guid saleFilialeId, Guid salePlatformId)
        {
            OrderId = orderId;
            OrderNo = orderNo;
            MemberId = memberId;
            OrderTime = orderTime;
            Consignee = consignee;
            Direction = direction;
            CountryId = countryId;
            ProvinceId = provinceId;
            CityId = cityId;
            PostalCode = postalCode;
            Phone = phone;
            Mobile = mobile;
            OldCustomer = oldCustomer;
            PayMode = payMode;
            PayState = payState;
            PayType = payType;
            BankAccountsId = bankAccountsId;
            BankTradeNo = bankTradeNo;
            RefundmentMode = refundmentMode;
            ExpressId = expressId;
            ExpressNo = expressNo;
            TotalPrice = totalPrice;
            Carriage = carriage;
            RealTotalPrice = realTotalPrice;
            PaymentByBalance = paymentByBalance;
            PaidUp = paidUp;
            OrderState = orderState;
            InvoiceState = invoiceState;
            CancleReason = cancleReason;
            ConsignTime = consignTime;
            LatencyDay = latencyDay;
            Memo = memo;
            PromotionValue = promotionValue;
            PromotionDescription = promotionDescription;
            EffectiveTime = effectivTime;
            DeliverWarehouseId = deliverWarehouseId;
            SaleFilialeId = saleFilialeId;
            SalePlatformId = salePlatformId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderNo">订单号</param>
        /// <param name="memberId">会员ID</param>
        /// <param name="orderTime">下单时间</param>
        /// <param name="consignee">收货人</param>
        /// <param name="direction">说明</param>
        /// <param name="countryId">国家ID</param>
        /// <param name="provinceId">省ID</param>
        /// <param name="cityId">城市ID</param>
        /// <param name="postalCode">邮政编码</param>
        /// <param name="phone">电话</param>
        /// <param name="mobile">手机</param>
        /// <param name="oldCustomer">新老会员类型</param>
        /// <param name="payMode">支付方式</param>
        /// <param name="payState">支付状态</param>
        /// <param name="payType">支付类型</param>
        /// <param name="bankAccountsId">银行账户ID</param>
        /// <param name="bankTradeNo">银行交易号</param>
        /// <param name="refundmentMode">退款方式</param>
        /// <param name="expressId">对应快递公司ID</param>
        /// <param name="expressNo">快递编号</param>
        /// <param name="totalPrice">最高价</param>
        /// <param name="carriage">运输费</param>
        /// <param name="realTotalPrice">实际应收</param>
        /// <param name="paymentByBalance">平均付款</param>
        /// <param name="paidUp">实际支付</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancleReason">取消原因</param>
        /// <param name="consignTime">托运时间</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="latencyDay">等待天数</param>
        /// <param name="memo">备注</param>
        /// <param name="promotionValue">促销价值</param>
        /// <param name="promotionDescription">促销说明</param>
        /// <param name="effectivTime">有效时间</param>
        /// <param name="pickNo">配货单号</param>
        /// <param name="saleFilialeId">销售公司ID </param>
        /// <param name="salePlatformId">门店公司ID，网站ID </param>
        public GoodsOrderInfo(Guid orderId, string orderNo, Guid memberId, DateTime orderTime, string consignee,
                              string direction, Guid countryId, Guid provinceId, Guid cityId,
                              string postalCode, string phone, string mobile, int oldCustomer, int payMode, int payState,
                              int payType, Guid bankAccountsId, string bankTradeNo, int refundmentMode, Guid expressId,
                              string expressNo, decimal totalPrice, decimal carriage, decimal realTotalPrice,
                              decimal paymentByBalance, decimal paidUp, int orderState, int invoiceState,
                              string cancleReason, DateTime consignTime, Guid warehouseId,
                              int latencyDay, string memo, decimal promotionValue,
                              string promotionDescription, DateTime effectivTime, string pickNo, Guid saleFilialeId, Guid salePlatformId)
            : this(orderId, orderNo, memberId, orderTime, consignee, direction, countryId, provinceId, cityId,
                   postalCode, phone, mobile, oldCustomer, payMode, payState, payType, bankAccountsId, bankTradeNo,
                   refundmentMode, expressId,
                   expressNo, totalPrice, carriage, realTotalPrice, paymentByBalance, paidUp, orderState, invoiceState,
                   cancleReason, consignTime, warehouseId,
                   latencyDay, memo, promotionValue, promotionDescription, effectivTime, saleFilialeId, salePlatformId)
        {
            PickNo = pickNo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is GoodsOrderInfo)
                return (compareObj as GoodsOrderInfo).OrderId == OrderId;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (OrderId == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" +
                                          OrderId.ToString();
            return stringRepresentation.GetHashCode();
        }


        /// <summary>
        /// Func : 拷贝
        /// Code : dyy
        /// Date : 2009 Nov 12th
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo Clone()
        {
            return MemberwiseClone() as GoodsOrderInfo;
        }

        /// <summary>
        /// Func : 深度拷贝
        /// Code : dyy
        /// Date : 2009 Nov 12th
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo DeepClone()
        {
            var srcInfo = new GoodsOrderInfo
                {
                    OrderId = OrderId,
                    OrderNo = OrderNo,
                    MemberId = MemberId,
                    OrderTime = OrderTime,
                    Consignee = Consignee,
                    Direction = Direction,
                    CountryId = CountryId,
                    ProvinceId = ProvinceId,
                    CityId = CityId,
                    PostalCode = PostalCode,
                    Phone = Phone,
                    Mobile = Mobile,
                    OldCustomer = OldCustomer,
                    PayMode = PayMode,
                    PayState = PayState,
                    PayType = PayType,
                    BankAccountsId = BankAccountsId,
                    BankTradeNo = BankTradeNo,
                    RefundmentMode = RefundmentMode,
                    ExpressId = ExpressId,
                    ExpressNo = ExpressNo,
                    TotalPrice = TotalPrice,
                    Carriage = Carriage,
                    RealTotalPrice = RealTotalPrice,
                    PaymentByBalance = PaymentByBalance,
                    PaidUp = PaidUp,
                    OrderState = OrderState,
                    InvoiceState = InvoiceState,
                    CancleReason = CancleReason,
                    ConsignTime = ConsignTime,
                    DeliverWarehouseId = DeliverWarehouseId,
                    LatencyDay = LatencyDay,
                    Memo = Memo,
                    PromotionValue = PromotionValue,
                    PromotionDescription = PromotionDescription,
                    EffectiveTime = EffectiveTime,
                    SaleFilialeId = SaleFilialeId,
                    SalePlatformId = SalePlatformId
                };
            return srcInfo;
        }
    }
}
