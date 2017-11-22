using System;
using ERP.Enum;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 发票信息类
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceInfo
    {
        /// <summary>
        /// 发票编号
        /// </summary>
        [DataMember]
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        [DataMember]
        public string InvoiceName { get; set; }

        /// <summary>
        /// 品名规格
        /// </summary>
        [DataMember]
        public string InvoiceContent { get; set; }

        /// <summary>
        /// 开票人姓名
        /// </summary>
        [DataMember]
        public string Receiver { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// 收发票地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        [DataMember]
        public DateTime RequestTime { get; set; }

        /// <summary>
        /// 发票金额
        /// </summary>
        [DataMember]
        public double InvoiceSum { get; set; }

        /// <summary>
        /// 发票状态：0 未开1 申请2 已开3 取消
        /// </summary>
        [DataMember]
        public int InvoiceState { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public string OrderNos { get; set; }

        /// <summary>
        /// 接受时间
        /// </summary>
        [DataMember]
        public DateTime AcceptedTime { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        [DataMember]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        [DataMember]
        public long InvoiceNo { get; set; }

        /// <summary>
        /// 原发票代码，如果这个发票记录是开退票的，要记录原发票代码
        /// </summary>
        [DataMember]
        public string InvoiceChCode { get; set; }

        /// <summary>
        /// 原发票号码，如果这个发票记录是开退票的，要记录原发票号码
        /// </summary>
        [DataMember]
        public long InvoiceChNo { get; set; }

        /// <summary>
        /// 发票是否报税提交
        /// </summary>
        [DataMember]
        public bool IsCommit { get; set; }

        /// <summary>
        /// 发票购货方企业类型
        /// </summary>
        [DataMember]
        public InvoicePurchaserType PurchaserType { get; set; }

        /// <summary>
        /// 发票票据类型
        /// </summary>
        [DataMember]
        public InvoiceNoteType NoteType { get; set; }

        /// <summary>
        /// 发票对方纳税人识别号
        /// </summary>
        [DataMember]
        public string TaxpayerID { get; set; }

        /// <summary>
        /// 作废申请人
        /// </summary>
        [DataMember]
        public string CancelPersonel { get; set; }

        /// <summary>
        /// 是否需要手动打印发票
        /// </summary>
        [DataMember]
        public bool IsNeedManual { get; set; }

        /// <summary>销售公司
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>销售平台
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>是否门店订单发票
        /// </summary>
        [DataMember]
        public Boolean IsShopInvoice { get; set; }

        /// <summary>
        /// 是否事后申请发票(True:是;False:否;)
        /// </summary>
        [DataMember]
        public bool IsAfterwardsApply { get; set; }

        /// <summary>
        /// 发货仓库ID
        /// </summary>
        [DataMember]
        public Guid DeliverWarehouseId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public InvoiceInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceName">发票抬头</param>
        /// <param name="invoiceContent">品名规格</param>
        /// <param name="invoiceSum">发票金额</param>
        /// <param name="invoiceState">发票状态：0 未开1 申请2 已开3 取消</param>
        /// <param name="acceptedTime">接受时间</param>
        /// <param name="orderNos">序号</param>
        public InvoiceInfo(Guid invoiceId, string invoiceName, string invoiceContent, DateTime acceptedTime, double invoiceSum, int invoiceState, string orderNos)
        {
            InvoiceId = invoiceId;
            InvoiceName = invoiceName;
            InvoiceContent = invoiceContent;
            AcceptedTime = acceptedTime;
            InvoiceSum = invoiceSum;
            InvoiceState = invoiceState;
            OrderNos = orderNos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="memberId">会员ID</param>
        /// <param name="invoiceName">发票抬头</param>
        /// <param name="invoiceContent">品名规格</param>
        /// <param name="receiver">开票人姓名</param>
        /// <param name="postalCode">邮政编码</param>
        /// <param name="address">收发票地址</param>
        /// <param name="requestTime">申请时间</param>
        /// <param name="invoiceSum">发票金额</param>
        /// <param name="invoiceState">发票状态：0 未开1 申请2 已开3 取消</param>
        /// <param name="acceptedTime">接受时间</param>
        /// <param name="purchaserType">发票购货方企业类型</param>
        public InvoiceInfo(Guid invoiceId, Guid memberId, string invoiceName, string invoiceContent, string receiver, string postalCode, string address, DateTime requestTime, double invoiceSum, int invoiceState, DateTime acceptedTime, InvoicePurchaserType purchaserType)
        {
            InvoiceId = invoiceId;
            MemberId = memberId;
            InvoiceName = invoiceName;
            InvoiceContent = invoiceContent;
            Receiver = receiver;
            PostalCode = postalCode;
            Address = address;
            RequestTime = requestTime;
            InvoiceSum = invoiceSum;
            InvoiceState = invoiceState;
            AcceptedTime = acceptedTime;
            PurchaserType = purchaserType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="memberId">会员ID</param>
        /// <param name="invoiceName">发票抬头</param>
        /// <param name="invoiceContent">品名规格</param>
        /// <param name="receiver">开票人姓名</param>
        /// <param name="postalCode">邮政编码</param>
        /// <param name="address">收发票地址</param>
        /// <param name="requestTime">申请时间</param>
        /// <param name="invoiceSum">发票金额</param>
        /// <param name="invoiceState">发票状态：0 未开1 申请2 已开3 取消</param>
        public InvoiceInfo(Guid invoiceId, Guid memberId, string invoiceName, string invoiceContent, string receiver, string postalCode, string address, DateTime requestTime, double invoiceSum, int invoiceState)
        {
            InvoiceId = invoiceId;
            MemberId = memberId;
            InvoiceName = invoiceName;
            InvoiceContent = invoiceContent;
            Receiver = receiver;
            PostalCode = postalCode;
            Address = address;
            RequestTime = requestTime;
            InvoiceSum = invoiceSum;
            InvoiceState = invoiceState;
        }
    }
}
