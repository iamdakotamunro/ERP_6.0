using System;
using System.Runtime.Serialization;

namespace ERP.Model.RefundsMoney
{
    /// <summary>
    /// �˿�������
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundsMoneyInfo_Payment
    {
        #region Model

        /// <summary>
        /// �������˿���ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// �Ƿ������
        /// </summary>
        [DataMember]
        public bool IsPayment { get; set; }

        /// <summary>
        /// �ܾ�����
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }

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
        /// ����޸���
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }

        #endregion Model
    }
}
