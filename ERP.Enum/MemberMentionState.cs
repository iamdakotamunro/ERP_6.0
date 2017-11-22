using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ��Ա����״̬ö��
    /// Add by liucaijun at 2011-October-21th
    /// </summary>
    [Serializable]
    public enum MemberMentionState
    {
        /// <summary>
        /// ȫ��
        /// </summary>
        [Enum("ȫ��")]
        All=-1,
        /// <summary>
        /// �����
        /// </summary>
        [Enum("�����")]
        Process = 0,
        /// <summary>
        /// �����
        /// </summary>
        [Enum("�����")]
        Auditing = 1,
        /// <summary>
        /// ���
        /// </summary>
        [Enum("���")]
        Finish = 2,
        /// <summary>
        /// ����ʧ��,ȡ����״̬����ʱ������
        /// </summary>
        [Enum("����ʧ��")]
        NoPass = 3,

        /// <summary>
        /// �˻�
        /// </summary>
        [Enum("�˻�")]
        SendBack=4,

        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Invalid=5
    }
}
