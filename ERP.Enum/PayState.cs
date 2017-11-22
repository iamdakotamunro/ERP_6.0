using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ֧��״̬ö��
    /// </summary>
    [Serializable]
    public enum PayState
    {
        /// <summary>
        /// ����״̬
        /// </summary>
        [Enum("����״̬")]
        NoSet = 0,
        /// <summary>
        /// δ֧��
        /// </summary>
        [Enum("δ֧��")]
        NoPay = 1,
        /// <summary>
        /// ��֧��
        /// </summary>
        [Enum("��֧��")]
        Paid = 2,
        /// <summary>
        /// ��ȷ��
        /// </summary>
        [Enum("��ȷ��")]
        Wait = 3,

        /// <summary>
        /// Ԥ����
        /// </summary>
        [Enum("Ԥ����")]
        PrePaid = 4,
    }
}
