using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ���ϻ�Ա����
    /// </summary>
    [Serializable]
    public enum OldCustomerType
    {
        /// <summary>
        /// ���û�,���ɵ�ַ��¼
        /// </summary>
        [Enum("���û�,���ɵ�ַ��¼")]
        NewCustomer = 0,
        /// <summary>
        /// ���û�,û���κ��޸�
        /// </summary>
        [Enum("���û�,û���κ��޸�")]
        OldCustomer = 1,

        /// <summary> ��ע�Ṻ���û�
        /// </summary>
        [Enum("��ע�Ṻ���û�")]
        NonRegCustomer = 2,
    }
}
