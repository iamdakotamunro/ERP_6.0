using System;
using System.Runtime.Serialization;

namespace ERP.Model.SubsidyPayment
{
    /// <summary>
    /// ������ˡ��������
    /// </summary>
    [Serializable]
    [DataContract]
    public class SubsidyPaymentInfo
    {
        #region Model

        /// <summary>
        /// �������������ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// ������������
        /// </summary>
        [DataMember]
        public string ThirdPartyOrderNumber { get; set; }

        /// <summary>
        /// �������˻���
        /// </summary>
        [DataMember]
        public string ThirdPartyAccountName { get; set; }

        /// <summary>
        /// ����ƽ̨
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// ���۹�˾
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [DataMember]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [DataMember]
        public decimal SubsidyAmount { get; set; }

        /// <summary>
        /// �������ͣ�1��������2�����ͣ�
        /// </summary>
        [DataMember]
        public int SubsidyType { get; set; }

        /// <summary>
        /// ������˵���������
        /// </summary>
        [DataMember]
        public Guid QuestionType { get; set; }

        /// <summary>
        /// ������˵���������
        /// </summary>
        [DataMember]
        public string QuestionName { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// �ͻ�֧����\�����˻�
        /// </summary>
        [DataMember]
        public string BankAccountNo { get; set; }

        /// <summary>
        /// �ͻ�����
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// �ܾ�����
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }

        /// <summary>
        /// ������ˣ�1������ˡ�2����������ˡ�3������4�������ϡ�5���Ѵ�
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [DataMember]
        public string CreateUser { get; set; }

        /// <summary>
        /// �Ƿ�ɾ��
        /// </summary>
        [DataMember]
        public bool IsDelete { get; set; }

        /// <summary>
        /// ����޸�ʱ��
        /// </summary>
        [DataMember]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// ����޸���
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [DataMember]
        public decimal? Fees { get; set; }

        /// <summary>
        /// ���׺�
        /// </summary>
        [DataMember]
        public string TransactionNumber { get; set; }

        /// <summary>
        /// �ʽ��˻�
        /// </summary>
        [DataMember]
        public Guid? AccountID { get; set; }

        /// <summary>
        /// ��ע��Ϣ
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        #endregion Model
    }
}
