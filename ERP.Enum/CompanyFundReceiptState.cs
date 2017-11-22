using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ������λ�ո���״̬ö��
    /// </summary>
    [Serializable]
    public enum CompanyFundReceiptState
    {
        /// <summary>
        /// ȫ��
        /// </summary>
        [Enum("ȫ��")]
        All = -1,

        /// <summary>
        /// 
        /// </summary>
        [Enum("δ����")]
        NoHandle = -2,

        /// <summary>
        /// 
        /// </summary>
        [Enum("�Ѳ���")]
        Handle = -3,

        /// <summary>
        /// δͨ��
        /// </summary>
        [Enum("δͨ��")]
        NoAuditing = 0,

        /// <summary>
        /// ���δͨ��
        /// </summary>
        [Enum("���δͨ��")]
        NoAuditingPass = 13,

        /// <summary>
        /// �����
        /// </summary>
        [Enum("�����")]
        WaitAuditing = 1,

        /// <summary>
        /// ������Ʊ
        /// </summary>
        [Enum("������Ʊ")]
        WaitInvoice = 2,

        /// <summary>
        /// �����
        /// </summary>
        [Enum("�����")]
        Audited = 3,

        /// <summary>
        /// ��ִ��
        /// </summary>
        [Enum("��ִ��")]
        Executed = 4,

        /// <summary>
        /// ��Ʊ����ȡ
        /// </summary>
        [Enum("��Ʊ����ȡ")]
        GettingInvoice = 5,

        /// <summary>
        /// ��Ʊ����ȡ
        /// </summary>
        [Enum("��Ʊ����ȡ")]
        HasInvoice = 6,

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        [Enum("��Ʊ����")]
        InvoiceVerification = 7,

        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Cancel = 8,

        /// <summary>
        /// ��ɴ��
        /// </summary>
        [Enum("��ɴ��")]
        Finish = 9,

        /// <summary>
        /// ����˻�
        /// </summary>
        [Enum("����˻�")]
        PayBack = 10,

        /// <summary>��Ʊ����֤
        /// </summary>
        [Enum("��Ʊ����֤")]
        WaitAttestation = 11,

        /// <summary>��Ʊ����֤
        /// </summary>
        [Enum("��Ʊ����֤")]
        FinishAttestation = 12
    }
}
