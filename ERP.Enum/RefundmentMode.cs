using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// �˿�ģʽö��
    /// </summary>
    [Serializable]
    public enum RefundmentMode
    {
        /// <summary>
        /// �����ʺ�(��̨)
        /// </summary>
        [Enum("�����ʺ�")]
        Accounts = 0,
        /// <summary>
        /// ԭ�ʺ��˻�(����)
        /// </summary>
        [Enum("ԭ�ʺ��˻�")]
        Untread = 1
    }
}
