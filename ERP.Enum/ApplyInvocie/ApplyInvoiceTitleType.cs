using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum.ApplyInvocie
{
    [Serializable]
    [DataContract]
    public enum ApplyInvoiceTitleType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        [EnumMember]
        All = 0,

        /// <summary>
        /// 个人
        /// </summary>
        [Enum("个人")]
        [EnumMember]
        Individual = 1,

        /// <summary>
        /// 公司
        /// </summary>
        [Enum("公司")]
        [EnumMember]
        Company = 2,
    }
}
