using System;
using System.Runtime.Serialization;

namespace ERP.Model.RefundsMoney
{
    /// <summary>
    /// �˿���
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundsMoneyInfo
    {
        #region Model

        /// <summary>
        /// �������˿���ID
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
        /// �˿���
        /// </summary>
        [DataMember]
        public decimal RefundsAmount { get; set; }

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
        /// �ܾ�����
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }

        /// <summary>
        /// �˿����״̬��1������ˡ�2�����ˡ�3������4�������ϡ�5���Ѵ�
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
        /// �ۺ󵥺ţ��˻����ţ�
        /// </summary>
        [DataMember]
        public string AfterSalesNumber { get; set; }

        /// <summary>
        /// ��ע��Ϣ
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        #endregion Model
    }
}
