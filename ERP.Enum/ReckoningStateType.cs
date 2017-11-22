using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ������ʵ�״̬ö��
    /// </summary>
    [Serializable]
    public enum ReckoningStateType
    {
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Currently = 1,
        /// <summary>
        /// ���
        /// </summary>
        [Enum("���")]
        Cancellation = 2
    }
}
