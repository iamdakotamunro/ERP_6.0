using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ֧����ʽö��
    /// </summary>
    [Serializable]
    public enum PayMode
    {
        /// <summary>
        /// 
        /// </summary>
        [Enum("����ȷ��")]
        NoSet=-1,
        /// <summary>
        /// ��������
        /// </summary>
        [Enum("��������")]
        COD=0,
        /// <summary>
        /// �����
        /// </summary>
        [Enum("�����")]
        COG=1,

        /// <summary>
        ///���������ֻ�֧�� 
        /// </summary>
        [Enum("���������ֻ�֧��")]
        COM=2

    }
}
