using System;
using System.Runtime.Serialization;

namespace ERP.Model.Finance.Gathering
{
    /// <summary>
    /// �տ������
    /// </summary>
    [Serializable]
    [DataContract]
    public class GoodsOrderDTO
    {
        /// <summary>
        /// ����ID
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// ��ԱID
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// �µ�ʱ��
        /// </summary>
        [DataMember]
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// �ܼ�
        /// </summary>
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// ���۹�˾ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// ����ƽ̨
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// �տ���ͣ�true�������տ��false�������տ��
        /// </summary>
        [DataMember]
        public bool Type { get; set; }

        public Guid BankAccountsId { get; set; }
    }
}