using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ��Ϣ״̬ö��
    /// </summary>
    [Serializable]
    public enum State
    {
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Disable = 0,
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Enable = 1
    }
}
