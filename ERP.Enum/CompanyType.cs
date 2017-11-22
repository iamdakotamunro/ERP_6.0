using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ��˾����ö��
    /// </summary>
    [Serializable]
    public enum CompanyType
    {
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Other = 0,
        /// <summary>
        /// ��Ӧ��
        /// </summary>
        [Enum("��Ӧ��")]
        Suppliers = 1,
        /// <summary>
        /// ������
        /// </summary>
        [Enum("������")]
        Vendors = 2,
        /// <summary>
        /// ������˾
        /// </summary>
        [Enum("������˾")]
        Express = 3,
        /// <summary>
        /// ��Ա����
        /// </summary>
        [Enum("��Ա����")]
        MemberGeneralLedger = 4
    }
}
