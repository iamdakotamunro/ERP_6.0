using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ����ģ��
    /// </summary>
    [Serializable]
    [DataContract]
    public class GoodsOrderInfo
    {
        /// <summary>
        /// ����ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// ��ԱID
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// �µ�ʱ��
        /// </summary>
        [DataMember]
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// �ջ���
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }

        /// <summary>
        /// ˵��
        /// </summary>
        [DataMember]
        public string Direction { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        [DataMember]
        public Guid CountryId { get; set; }

        /// <summary>
        /// ʡID
        /// </summary>
        [DataMember]
        public Guid ProvinceId { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        [DataMember]
        public Guid CityId { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        [DataMember]
        public Guid DistrictID { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// �绰
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// �ֻ�
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// ���ϻ�Ա����
        /// </summary>
        [DataMember]
        public int OldCustomer { get; set; }

        /// <summary>
        /// ֧��ģʽ
        /// </summary>
        [DataMember]
        public int PayMode { get; set; }

        /// <summary>
        /// ֧��״̬
        /// </summary>
        [DataMember]
        public int PayState { get; set; }

        /// <summary>
        /// ֧������
        /// </summary>
        [DataMember]
        public int PayType { get; set; }

        /// <summary>
        /// �����˺�ID
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// ���н��׺�
        /// </summary>
        [DataMember]
        public string BankTradeNo { get; set; }

        /// <summary>
        /// �˿�ģʽ
        /// </summary>
        [DataMember]
        public int RefundmentMode { get; set; }

        /// <summary>
        /// ��Ӧ��ݹ�˾ID
        /// </summary>
        [DataMember]
        public Guid ExpressId { get; set; }

        /// <summary>
        /// ��ݱ��
        /// </summary>
        [DataMember]
        public string ExpressNo { get; set; }

        /// <summary>
        /// �ܼ�
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// �����
        /// </summary>
        [DataMember]
        public decimal Carriage { get; set; }

        /// <summary>
        /// ʵ��Ӧ��
        /// </summary>
        [DataMember]
        public decimal RealTotalPrice { get; set; }

        /// <summary>
        /// ���֧��
        /// </summary>
        [DataMember]
        public decimal PaymentByBalance { get; set; }

        /// <summary>
        /// ʵ��֧��
        /// </summary>
        [DataMember]
        public decimal PaidUp { get; set; }

        /// <summary>
        /// ����״̬
        /// </summary>
        [DataMember]
        public int OrderState { get; set; }

        /// <summary>
        /// ��Ʊ״̬
        /// </summary>
        [DataMember]
        public int InvoiceState { get; set; }

        /// <summary>
        /// ȡ��ԭ��
        /// </summary>
        [DataMember]
        public string CancleReason { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember]
        public DateTime ConsignTime { get; set; }

        /// <summary>
        /// �ȴ�����
        /// </summary>
        [DataMember]
        public int LatencyDay { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// ��Чʱ��
        /// </summary>
        [DataMember]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// ������ֵ
        /// </summary>
        [DataMember]
        public decimal PromotionValue { get; set; }

        /// <summary>
        /// ����˵��
        /// </summary>
        [DataMember]
        public string PromotionDescription { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [DataMember]
        public string PickNo { get; set; }

        /// <summary>
        /// ����δ�ɹ�
        /// </summary>
        [DataMember]
        public bool CommissionFailure { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        [DataMember]
        public string Express { get; set; }

        /// <summary>
        /// �ۺ���ʱ��
        /// </summary>
        [DataMember]
        public string ReturnTime { get; set; }

        /// <summary>
        /// �����˷Ѳ���
        /// </summary>
        [DataMember]
        public decimal CarriageSubsidy { get; set; }

        /// <summary>
        /// �˻�����
        /// </summary>
        [DataMember]
        public string ReturnOrder { get; set; }

        /// <summary>
        /// �˻���ID(���ڲ鿴�����Ƿ�����ۺ���)
        /// add by fanguan at 2012-07-03
        /// </summary>
        [DataMember]
        public Guid RefundId { get; set; }

        /// <summary>
        /// ������ʽ���ŵ��ֶΣ�
        /// </summary>
        [DataMember]
        public int SendType { get; set; }

        /// <summary>
        /// ���۹�˾ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// ����ƽ̨
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// �����ֿ�ID
        /// </summary>
        [DataMember]
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// ������˾
        /// </summary>
        [IgnoreDataMember]
        public Guid HostingFilialeId { get; set; }

        /// <summary>
        /// ʹ���˶��ٻ������ֿ�Ǯ��ȫ�����ֲ������ڡ�
        /// </summary>
        [DataMember]
        public int ScoreDeduction { get; set; }

        /// <summary>
        /// ʹ�û��ֵֿ�ʱ�ı��������ٻ��ֵ�1��Ǯ��
        /// </summary>
        [DataMember]
        public int ScoreDeductionProportion { get; set; }

        /// <summary>
        /// �Ƿ�˰
        /// </summary>
        /// <remarks>Ŀǰ���ж�Ҫ��˰ By Jerry Bai 2017/6/2</remarks>
        [IgnoreDataMember]
        public bool IsOut
        {
            get { return true; }
            set { }
        }

        /// <summary>��λ����ӦWMSö�� StorageType��
        /// </summary>
        [DataMember]
        public Byte StorageType { get; set; }

        /// <summary>����ID
        /// </summary>
        public Guid ShopId { get; set; }

        /// <summary>�ջ������֤��
        /// </summary>
        public String ConsigneeIdCard { get; set; }

        /// <summary>
        /// ��ʼ�����캯��
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
        /// <param name="orderId">����ID</param>
        /// <param name="orderNo">������</param>
        /// <param name="memberId">��ԱID</param>
        /// <param name="orderTime">�µ�ʱ��</param>
        /// <param name="consignee">�ջ���</param>
        /// <param name="direction">˵��</param>
        /// <param name="countryId">����ID</param>
        /// <param name="provinceId">ʡID</param>
        /// <param name="cityId">����ID</param>
        /// <param name="postalCode">��������</param>
        /// <param name="phone">�绰</param>
        /// <param name="mobile">�ֻ�</param>
        /// <param name="oldCustomer">���ϻ�Ա����</param>
        /// <param name="payMode">֧����ʽ</param>
        /// <param name="payState">֧��״̬</param>
        /// <param name="payType">֧������</param>
        /// <param name="bankAccountsId">�����˻�ID</param>
        /// <param name="bankTradeNo">���н��׺�</param>
        /// <param name="refundmentMode">�˿ʽ</param>
        /// <param name="expressId">��Ӧ��ݹ�˾ID</param>
        /// <param name="expressNo">��ݱ��</param>
        /// <param name="totalPrice">��߼�</param>
        /// <param name="carriage">�����</param>
        /// <param name="realTotalPrice">ʵ��Ӧ��</param>
        /// <param name="paymentByBalance">ƽ������</param>
        /// <param name="paidUp">ʵ��֧��</param>
        /// <param name="orderState">����״̬</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancleReason">ȡ��ԭ��</param>
        /// <param name="consignTime">����ʱ��</param>
        /// <param name="deliverWarehouseId">�����ֿ�ID</param>
        /// <param name="latencyDay">�ȴ�����</param>
        /// <param name="memo">��ע</param>
        /// <param name="promotionValue">������ֵ</param>
        /// <param name="promotionDescription">����˵��</param>
        /// <param name="effectivTime">��Чʱ��</param>
        /// <param name="saleFilialeId">���۹�˾ID </param>
        /// <param name="salePlatformId">�ŵ깫˾ID����վID </param>
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
        /// <param name="orderId">����ID</param>
        /// <param name="orderNo">������</param>
        /// <param name="memberId">��ԱID</param>
        /// <param name="orderTime">�µ�ʱ��</param>
        /// <param name="consignee">�ջ���</param>
        /// <param name="direction">˵��</param>
        /// <param name="countryId">����ID</param>
        /// <param name="provinceId">ʡID</param>
        /// <param name="cityId">����ID</param>
        /// <param name="postalCode">��������</param>
        /// <param name="phone">�绰</param>
        /// <param name="mobile">�ֻ�</param>
        /// <param name="oldCustomer">���ϻ�Ա����</param>
        /// <param name="payMode">֧����ʽ</param>
        /// <param name="payState">֧��״̬</param>
        /// <param name="payType">֧������</param>
        /// <param name="bankAccountsId">�����˻�ID</param>
        /// <param name="bankTradeNo">���н��׺�</param>
        /// <param name="refundmentMode">�˿ʽ</param>
        /// <param name="expressId">��Ӧ��ݹ�˾ID</param>
        /// <param name="expressNo">��ݱ��</param>
        /// <param name="totalPrice">��߼�</param>
        /// <param name="carriage">�����</param>
        /// <param name="realTotalPrice">ʵ��Ӧ��</param>
        /// <param name="paymentByBalance">ƽ������</param>
        /// <param name="paidUp">ʵ��֧��</param>
        /// <param name="orderState">����״̬</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancleReason">ȡ��ԭ��</param>
        /// <param name="consignTime">����ʱ��</param>
        /// <param name="warehouseId">�ֿ�ID</param>
        /// <param name="latencyDay">�ȴ�����</param>
        /// <param name="memo">��ע</param>
        /// <param name="promotionValue">������ֵ</param>
        /// <param name="promotionDescription">����˵��</param>
        /// <param name="effectivTime">��Чʱ��</param>
        /// <param name="pickNo">�������</param>
        /// <param name="saleFilialeId">���۹�˾ID </param>
        /// <param name="salePlatformId">�ŵ깫˾ID����վID </param>
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
        /// Func : ����
        /// Code : dyy
        /// Date : 2009 Nov 12th
        /// </summary>
        /// <returns></returns>
        public GoodsOrderInfo Clone()
        {
            return MemberwiseClone() as GoodsOrderInfo;
        }

        /// <summary>
        /// Func : ��ȿ���
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
