using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// �Ƿ�ѡ��ö��
    /// </summary>
    [Serializable]
    public enum YesOrNo
    {
        /// <summary>
        /// ��
        /// </summary>
        [Enum("��")]
        No = 0,
        /// <summary>
        /// ��
        /// </summary>
        [Enum("��")]
        Yes = 1,
        /// <summary>
        /// ȫ��
        /// </summary>
        [Enum("ȫ��")]
        All = -1
    }
}
