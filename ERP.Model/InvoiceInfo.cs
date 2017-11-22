using System;
using ERP.Enum;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ��Ʊ��Ϣ��
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvoiceInfo
    {
        /// <summary>
        /// ��Ʊ���
        /// </summary>
        [DataMember]
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// �û�ID
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// ��Ʊ̧ͷ
        /// </summary>
        [DataMember]
        public string InvoiceName { get; set; }

        /// <summary>
        /// Ʒ�����
        /// </summary>
        [DataMember]
        public string InvoiceContent { get; set; }

        /// <summary>
        /// ��Ʊ������
        /// </summary>
        [DataMember]
        public string Receiver { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// �շ�Ʊ��ַ
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember]
        public DateTime RequestTime { get; set; }

        /// <summary>
        /// ��Ʊ���
        /// </summary>
        [DataMember]
        public double InvoiceSum { get; set; }

        /// <summary>
        /// ��Ʊ״̬��0 δ��1 ����2 �ѿ�3 ȡ��
        /// </summary>
        [DataMember]
        public int InvoiceState { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        [DataMember]
        public string OrderNos { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember]
        public DateTime AcceptedTime { get; set; }

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        [DataMember]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        [DataMember]
        public long InvoiceNo { get; set; }

        /// <summary>
        /// ԭ��Ʊ���룬��������Ʊ��¼�ǿ���Ʊ�ģ�Ҫ��¼ԭ��Ʊ����
        /// </summary>
        [DataMember]
        public string InvoiceChCode { get; set; }

        /// <summary>
        /// ԭ��Ʊ���룬��������Ʊ��¼�ǿ���Ʊ�ģ�Ҫ��¼ԭ��Ʊ����
        /// </summary>
        [DataMember]
        public long InvoiceChNo { get; set; }

        /// <summary>
        /// ��Ʊ�Ƿ�˰�ύ
        /// </summary>
        [DataMember]
        public bool IsCommit { get; set; }

        /// <summary>
        /// ��Ʊ��������ҵ����
        /// </summary>
        [DataMember]
        public InvoicePurchaserType PurchaserType { get; set; }

        /// <summary>
        /// ��ƱƱ������
        /// </summary>
        [DataMember]
        public InvoiceNoteType NoteType { get; set; }

        /// <summary>
        /// ��Ʊ�Է���˰��ʶ���
        /// </summary>
        [DataMember]
        public string TaxpayerID { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        [DataMember]
        public string CancelPersonel { get; set; }

        /// <summary>
        /// �Ƿ���Ҫ�ֶ���ӡ��Ʊ
        /// </summary>
        [DataMember]
        public bool IsNeedManual { get; set; }

        /// <summary>���۹�˾
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>����ƽ̨
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>�Ƿ��ŵ궩����Ʊ
        /// </summary>
        [DataMember]
        public Boolean IsShopInvoice { get; set; }

        /// <summary>
        /// �Ƿ��º����뷢Ʊ(True:��;False:��;)
        /// </summary>
        [DataMember]
        public bool IsAfterwardsApply { get; set; }

        /// <summary>
        /// �����ֿ�ID
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
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="invoiceName">��Ʊ̧ͷ</param>
        /// <param name="invoiceContent">Ʒ�����</param>
        /// <param name="invoiceSum">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬��0 δ��1 ����2 �ѿ�3 ȡ��</param>
        /// <param name="acceptedTime">����ʱ��</param>
        /// <param name="orderNos">���</param>
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
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="memberId">��ԱID</param>
        /// <param name="invoiceName">��Ʊ̧ͷ</param>
        /// <param name="invoiceContent">Ʒ�����</param>
        /// <param name="receiver">��Ʊ������</param>
        /// <param name="postalCode">��������</param>
        /// <param name="address">�շ�Ʊ��ַ</param>
        /// <param name="requestTime">����ʱ��</param>
        /// <param name="invoiceSum">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬��0 δ��1 ����2 �ѿ�3 ȡ��</param>
        /// <param name="acceptedTime">����ʱ��</param>
        /// <param name="purchaserType">��Ʊ��������ҵ����</param>
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
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="memberId">��ԱID</param>
        /// <param name="invoiceName">��Ʊ̧ͷ</param>
        /// <param name="invoiceContent">Ʒ�����</param>
        /// <param name="receiver">��Ʊ������</param>
        /// <param name="postalCode">��������</param>
        /// <param name="address">�շ�Ʊ��ַ</param>
        /// <param name="requestTime">����ʱ��</param>
        /// <param name="invoiceSum">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬��0 δ��1 ����2 �ѿ�3 ȡ��</param>
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
