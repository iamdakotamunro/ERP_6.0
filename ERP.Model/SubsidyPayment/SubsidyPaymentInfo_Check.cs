using System;
using System.Runtime.Serialization;

namespace ERP.Model.SubsidyPayment
{
    /// <summary>
    /// ������ˡ����������������
    /// </summary>
    [Serializable]
    [DataContract]
    public class SubsidyPaymentInfo_Check
    {
        #region Model

        /// <summary>
        /// �������������ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// ����Ƿ�ͨ��
        /// </summary>
        [DataMember]
        public bool IsApproved { get; set; }
        /// <summary>
        /// �ܾ�����
        /// </summary>
        [DataMember]
        public string RejectReason { get; set; }
        
        /// <summary>
        /// ����޸���
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }

        #endregion Model
    }
}
