//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��9��11��
// �ļ�������:����
// ����޸�ʱ��:2007��9��11��
// ���һ���޸���:����
//================================================
using System;
using System.Runtime.Serialization;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ��Ա��ϵ��ַ
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConsigneeInfo
    {
        /// <summary>
        /// �ջ��˱��
        /// </summary>
        [DataMember]
        public Guid ConsigneeId { get; set; }

        /// <summary>
        /// �ͻ����
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// �ջ���
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }

        /// <summary>
        /// ���ұ��
        /// </summary>
        [DataMember]
        public Guid CountryId { get; set; }

        /// <summary>
        /// ʡ/�� ���
        /// </summary>
        [DataMember]
        public Guid ProvinceId { get; set; }

        /// <summary>
        /// ������ ���
        /// </summary>
        [DataMember]
        public Guid CityId { get; set; }

        /// <summary>
        /// ���ر��
        /// </summary>
        [DataMember]
        public Guid DistrictID { get; set; }

        /// <summary>
        /// �ջ���ַ
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// �ֻ�����
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// �绰����
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// ֧����ʽ
        /// </summary>
        [DataMember]
        public int PayMode { get; set; }

        /// <summary>
        /// ֧������
        /// </summary>
        [DataMember]
        public int PaymentType { get; set; }

        /// <summary>
        /// �����ʺű��
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// �˿�����0.ֱ���˵�keede�˻�1.�˿���У�ԭ�����ʺţ�
        /// </summary>
        [DataMember]
        public int RefundmentMode { get; set; }

        /// <summary>
        /// ע��ʱ��
        /// </summary>
        [DataMember]
        public DateTime RegTime { get; set; }

        /// <summary>
        /// �Ƿ�Ĭ�ϡ�ÿ�����һ���ջ���ϢΪĬ�ϡ�
        /// </summary>
        [DataMember]
        public int IsDefault { get; set; }

        /// <summary>
        /// ���ID
        /// </summary>
        [DataMember]
        public Guid ExpressID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConsigneeInfo()
        {
            IsDefault = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consigneeId">�ջ��˱��</param>
        /// <param name="memberId">�ͻ����</param>
        /// <param name="consignee">�ջ���</param>
        /// <param name="countryId">���ұ��</param>
        /// <param name="provinceId">ʡ/�� ���</param>
        /// <param name="cityId">������ ���</param>
        /// <param name="address">�ջ���ַ</param>
        /// <param name="postalCode">��������</param>
        /// <param name="mobile">�ֻ�����</param>
        /// <param name="phone">�绰����</param>
        /// <param name="payMode">֧����ʽ</param>
        /// <param name="paymentType">֧������</param>
        /// <param name="bankAccountsId">�����ʺű��</param>
        /// <param name="refundmentMode">�˿�����0.ֱ���˵�keede�˻�1.�˿���У�ԭ�����ʺţ�</param>
        /// <param name="regTime">ע��ʱ��</param>
        /// <param name="isDefault">�Ƿ�Ĭ�ϡ�ÿ�����һ���ջ���ϢΪĬ��</param>
        /// <param name="expressId">���ID</param>
        public ConsigneeInfo(Guid consigneeId, Guid memberId, string consignee, Guid countryId, Guid provinceId, Guid cityId, string address, string postalCode, string mobile, string phone, int payMode, int paymentType, Guid bankAccountsId, int refundmentMode, DateTime regTime, int isDefault, Guid expressId)
        {
            ConsigneeId = consigneeId;
            MemberId = memberId;
            Consignee = consignee;
            CountryId = countryId;
            ProvinceId = provinceId;
            CityId = cityId;
            Address = address;
            PostalCode = postalCode;
            Mobile = mobile;
            Phone = phone;
            PayMode = payMode;
            PaymentType = paymentType;
            BankAccountsId = bankAccountsId;
            RefundmentMode = refundmentMode;
            RegTime = regTime;
            IsDefault = isDefault;
            ExpressID = expressId;
        }
    }
}
