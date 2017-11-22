//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��4��27��
// �ļ�������:����
// ����޸�ʱ��:2006��4��27��
// ���һ���޸���:����
//================================================
using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ������λ��
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyCussentInfo
    {
        /// <summary>
        /// ������λ���
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ������λ������
        /// </summary>
        [DataMember]
        public Guid CompanyClassId { get; set; }

        /// <summary>
        /// ������λ����
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// ��ϵ��
        /// </summary>
        [DataMember]
        public string Linkman { get; set; }

        /// <summary>
        /// ��ַ
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// �̶��绰
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// �ֻ�
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string Fax { get; set; }

        /// <summary>
        /// ������λ�Ĺ�˾��վ
        /// </summary>
        [DataMember]
        public string WebSite { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// �����ʻ�
        /// </summary>
        [DataMember]
        public string BankAccounts { get; set; }

        /// <summary>
        /// �����ʺ�
        /// </summary>
        [DataMember]
        public string AccountNumber { get; set; }

        /// <summary>
        /// ����˾�������ʺ�����
        /// </summary>
        [DataMember]
        public string OwnBankAccountName { get; set; }

        /// <summary>
        /// ����˾�Ķ�Ӧ�����ʺű�ID
        /// </summary>
        [DataMember]
        public Guid OwnBankAccountID { get; set; }

        /// <summary>
        /// �Ƿ���Ҫ��Ʊ
        /// </summary>
        [DataMember]
        public bool IsNeedInvoices { get; set; }

        /// <summary>
        /// �½�����
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// ������λ���ͣ�0  ���� ,1  ��Ӧ�� 2  ������3  ������˾ 4  ��Ա����
        /// </summary>
        [DataMember]
        public int CompanyType { get; set; }

        /// <summary>
        /// ״̬ ��1  ʹ�ã� 0 ����
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// ��ע˵��
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// ��ϸ˵��
        /// </summary>
        [DataMember]
        public string SubjectInfo { get; set; }

        /// <summary>
        /// ��λ����
        /// </summary>
        [DataMember]
        public string Information { get; set; }

        /// <summary>������λ����
        /// </summary>
        [DataMember]
        public int PaymentDays { get; set; }

        /// <summary>
        /// �����Ƿ�����
        /// </summary>
        [DataMember]
        public int Complete { get; set; }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        [DataMember]
        public string Expire { get; set; }

        [DataMember]
        public Guid RelevanceFilialeId { get; set; }

        public int SalesScope { get; set; }

        public int DeliverType { get; set; }

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public CompanyCussentInfo() { }

        /// <summary>
        /// </summary>
        /// <param name="companyId">������λ���</param>
        /// <param name="companyClassId">������λ������</param>
        /// <param name="companyName">������λ����</param>
        /// <param name="linkman">��ϵ��</param>
        /// <param name="address">��ַ</param>
        /// <param name="postalCode">��������</param>
        /// <param name="phone">�绰</param>
        /// <param name="mobile">�ֻ�</param>
        /// <param name="fax">����</param>
        /// <param name="webSite">��վ</param>
        /// <param name="email">����</param>
        /// <param name="bankAccounts">�����˻�</param>
        /// <param name="accountNumber">�����ʺ�</param>
        /// <param name="dateCreated">�½�����</param>
        /// <param name="companyType">������λ���ͣ�0  ���� ,1  ��Ӧ�� 2  ������3  ������˾ 4  ��Ա����</param>
        /// <param name="state">״̬ ��1  ʹ�ã� 0 ����</param>
        /// <param name="description">��ע˵��</param>
        /// <param name="subjectinfo">��ϸ˵��</param>
        /// <param name="filialeId"></param>
        /// <param name="salesScope">���۷�Χ</param>
        /// <param name="deliverType">��������</param>
        public CompanyCussentInfo(Guid companyId, Guid companyClassId, string companyName, string linkman, string address, string postalCode, string phone, string mobile, string fax, string webSite, string email, string bankAccounts, string accountNumber, DateTime dateCreated, int companyType, int state, string description, string subjectinfo, Guid filialeId, int salesScope, int deliverType)
        {
            CompanyId = companyId;
            CompanyClassId = companyClassId;
            CompanyName = companyName;
            Linkman = linkman;
            Address = address;
            PostalCode = postalCode;
            Phone = phone;
            Mobile = mobile;
            Fax = fax;
            WebSite = webSite;
            Email = email;
            BankAccounts = bankAccounts;
            AccountNumber = accountNumber;
            DateCreated = dateCreated;
            CompanyType = companyType;
            State = state;
            Description = description;
            SubjectInfo = subjectinfo;
            RelevanceFilialeId = filialeId;
            SalesScope = salesScope;
            DeliverType = deliverType;
        }
    }
}
