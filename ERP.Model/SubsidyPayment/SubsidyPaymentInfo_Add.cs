using System;
using System.Runtime.Serialization;

namespace ERP.Model.SubsidyPayment
{
    /// <summary>
    /// ������ˡ������������
    /// </summary>
    [Serializable]
    [DataContract]
    public class SubsidyPaymentInfo_Add
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
        /// ������
        /// </summary>
        [DataMember]
        public string CreateUser { get; set; }


        /// <summary>
        /// ��ע��Ϣ
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        #endregion Model
    }
}
