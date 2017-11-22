using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ��Ʊ״̬ö��
    /// </summary>
    [Serializable]
    public enum InvoiceState
    {
        /// <summary>
        /// ����״̬
        /// </summary>
        [Enum("���з�Ʊ")]
        All = -1,
        /// <summary>
        /// δ��
        /// </summary>
        [Enum("δ����Ʊ")]
        NoRequest = 0,
        /// <summary>
        /// ���뷢Ʊ
        /// </summary>
        [Enum("���뷢Ʊ")]
        Request = 1,
        /// <summary>
        /// �ѿ�
        /// </summary>
        [Enum("�ѿ���Ʊ")]
        Success = 2,
        /// <summary>
        /// ȡ��
        /// </summary>
        [Enum("ȡ����Ʊ")]
        Cancel = 3,
        /// <summary>
        /// ��������
        /// </summary>
        [Enum("��������")]
        WasteRequest = 4,
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Waste = 5,
    }
}
