using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary> ������ʵ�����ö��
    /// </summary>
    [Serializable]
    public enum ReckoningType
    {
        /// <summary>Ӧ��
        /// </summary>
        [Enum("Ӧ�ա�Ӧ���Ѹ�")]
        Income = 0,

        /// <summary>Ӧ��
        /// </summary>
        [Enum("Ӧ����Ӧ������")]
        Defray = 1
    }
}
