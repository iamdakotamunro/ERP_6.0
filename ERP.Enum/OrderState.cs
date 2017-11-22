using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ����״̬ö��
    /// </summary>
    [Serializable]
    public enum OrderState
    {
        /// <summary>
        /// ����״̬
        /// </summary>
        [Enum("����״̬")]
        All = -1,

        /// <summary>
        /// δ�����
        /// </summary>
        [Enum("δ�����")]
        UnVerify = 0,

        /// <summary>
        /// ��ȷ��
        /// </summary>
        [Enum("��ȷ��")]
        WaitNotarize = 1,

        /// <summary>
        /// �ȴ�����,������
        /// </summary>
        [Enum("������")]
        Approved = 2,

        /// <summary>
        /// ������ȷ��
        /// </summary>
        [Enum("������ȷ��")]
        WaitForPay = 3,

        /// <summary>
        /// �����(�����������ȷ�Ϻ�)
        /// </summary>
        [Enum("�����")]
        StockUp = 4,

        /// <summary> ������
        /// </summary>
        [Enum("������")]
        WaitOutbound = 5,

        /// <summary>
        /// �����
        /// </summary>
        [Enum("�����")]
        Redeploy = 6,

        /// <summary>
        /// ��ɹ�
        /// </summary>
        [Enum("��ɹ�")]
        RequirePurchase = 7,

        /// <summary>
        /// ��ɷ���(��ɶ�����)
        /// </summary>
        [Enum("��ɷ���")]
        Consignmented = 9,

        /// <summary>
        /// ���϶���
        /// </summary>
        [Enum("���϶���")]
        Cancellation = 12
    }
}
