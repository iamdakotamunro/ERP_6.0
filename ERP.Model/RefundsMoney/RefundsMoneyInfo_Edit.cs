using System;
using System.Runtime.Serialization;

namespace ERP.Model.RefundsMoney
{
    /// <summary>
    /// �˿�����༭
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundsMoneyInfo_Edit
    {
        #region Model

        /// <summary>
        /// �������˿���ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        
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
        /// ��ע��Ϣ
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// ����޸���
        /// </summary>
        [DataMember]
        public string ModifyUser { get; set; }
        

        #endregion Model
    }
}
