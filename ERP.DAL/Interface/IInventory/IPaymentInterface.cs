//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��5��6��
// �ļ�������:����
// ����޸�ʱ��:2007��5��6��
// ���һ���޸���:����
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IPaymentInterface
    {
        /// <summary>
        /// ��ȡָ��������֧���ӿ�
        /// </summary>
        /// <param name="paymentInterfaceId">����֧���ӿڱ��</param>
        /// <returns></returns>
        PaymentInterfaceInfo GetPaymentInterface(Guid paymentInterfaceId);

        /// <summary>
        /// ��ȡ����֧���ӿ���Ϣ��
        /// </summary>
        /// <returns></returns>
        IList<PaymentInterfaceInfo> GetPaymentInterfaceList();
        /// <summary>
        /// ȡ֧�����ʻ���ID
        /// </summary>
        /// <returns></returns>
        Guid GetBandAccountsId();
       
    }
}
