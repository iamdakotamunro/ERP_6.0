using System;
using System.Runtime.Serialization;

namespace ERP.Model.RefundsMoney
{
    /// <summary>
    /// �˿�������
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundsMoneyInfo_Add
    {
        #region Model

        /// <summary>
        /// �������˿���ID
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }
        
        /// <summary>
        /// �ۺ󵥺ţ��˻����ţ�
        /// </summary>
        [DataMember]
        public string AfterSalesNumber { get; set; }

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
        /// ������
        /// </summary>
        [DataMember]
        public string CreateUser { get; set; }
                
        #endregion Model
    }
}
