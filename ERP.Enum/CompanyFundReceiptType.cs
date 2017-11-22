using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ������λ�ո��������ö��
    /// </summary>
    [Serializable]
    public enum CompanyFundReceiptType
    {
        /// <summary>
        /// ȫ��
        /// </summary>
        [Enum("ȫ��")]
        All = -1,

        /// <summary>
        /// �տ�
        /// </summary>
        [Enum("�տ�")]
        Receive = 0,
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Payment = 1
    }
}
