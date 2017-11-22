using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// �ʽ��˻���Ϣ��
    /// </summary>
    [Serializable]
    [DataContract]
    public class BankAccountInfo
    {
        /// <summary>
        /// �������˻�����ع�˾������ƽ̨ID
        /// </summary>
        [DataMember]
        public Guid TargetId
        {
            get;
            set;
        }

        /// <summary>
        /// �����˻����
        /// </summary>
        [DataMember]
        public Guid BankAccountsId
        {
            get;
            set;
        }

        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string BankName
        {
            get;
            set;
        }

        /// <summary>
        /// ֧���ӿڱ��
        /// </summary>
        [DataMember]
        public Guid PaymentInterfaceId
        {
            get;
            set;
        }

        /// <summary>
        /// �˻���
        /// </summary>
        [DataMember]
        public string AccountsName
        {
            get;
            set;
        }

        /// <summary>
        /// �ʺ�
        /// </summary>
        [DataMember]
        public string Accounts
        {
            get;
            set;
        }

        /// <summary>
        /// �ʺ��ܳ�
        /// </summary>
        [DataMember]
        public string AccountsKey
        {
            get;
            set;
        }

        /// <summary>
        /// ֧������0���ʾֻ�1���ֽ�2������֧����3�����л�4����㡣5������֧�����͡�6�����ÿ���
        /// </summary>
        [DataMember]
        public int PaymentType
        {
            get;
            set;
        }

        /// <summary>
        /// ����ͼ�ꡣ������վǰ̨���������ѡ��֧����ʽ����ʾ��
        /// </summary>
        [DataMember]
        public string BankIcon
        {
            get;
            set;
        }

        /// <summary>
        /// ���
        /// </summary>
        [DataMember]
        public int OrderIndex
        {
            get;
            set;
        }

        /// <summary>
        /// ������������վǰ̨���������ѡ��֧����ʽ����ʾ��
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�����
        /// Add By liucaijun at 2011-October-08th
        /// </summary>
        [DataMember]
        public bool IsUse
        {
            get;
            set;
        }
        /// <summary>
        /// �Ƿ���Ҫ���
        /// </summary>
        [DataMember]
        public bool IsFinish
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�ԭ·����
        /// </summary>
        [DataMember]
        public bool IsBacktrack { get; set; }

        /// <summary>�Ƿ����˺�
        /// </summary>
        [DataMember]
        public bool IsMain { get; set; }

        /// <summary>
        /// �Ƿ�ǰ̨��ʾ
        /// add by liangcanren at 2015-05-04
        /// </summary>
        [DataMember]
        public bool IsDisplay { get; set; }

        /// <summary>�˻����
        /// </summary>
        [DataMember]
        public decimal NonceBalance { get; set; }
        /// <summary>
        /// �����˻�����
        /// zal 2015-08-21
        /// </summary>
        [DataMember]
        public int AccountType { get; set; }
    }
}
