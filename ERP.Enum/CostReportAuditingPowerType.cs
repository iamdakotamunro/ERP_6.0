using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// Ȩ������ö��
    /// </summary>
    [Serializable]
    public enum CostReportAuditingType
    {
        /// <summary>
        /// ���Ȩ��
        /// </summary>
        [Enum("���Ȩ��")]
        Auditing = 0,
        /// <summary>
        /// Ʊ������Ȩ��
        /// </summary>
        [Enum("Ʊ������Ȩ��")]
        Invoice = 1
    }
}
