//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��4��27��
// �ļ�������:����
// ����޸�ʱ��:2006��4��27��
// ���һ���޸���:����
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// �ʽ��˻�����ӿ���
    /// </summary>
    [Serializable]
    public class PaymentInterfaceInfo
    {
        /// <summary>
        /// ֧���ӿڱ��
        /// </summary>
        public Guid PaymentInterfaceId { get; private set; }

        /// <summary>
        /// ֧���ӿ�����
        /// </summary>
        public string PaymentInterfaceName { get; set; }

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public PaymentInterfaceInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInterfaceId">֧���ӿڱ��</param>
        /// <param name="paymentInterfaceName">֧���ӿ�����</param>
        public PaymentInterfaceInfo(Guid paymentInterfaceId, string paymentInterfaceName)
        {
            PaymentInterfaceId = paymentInterfaceId;
            PaymentInterfaceName = paymentInterfaceName;
        }
    }
}