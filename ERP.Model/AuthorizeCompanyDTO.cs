using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 已授权的往来单位
    /// </summary>
    [Serializable]
    [DataContract]
    public class AuthorizeCompanyDTO
    {
        /// <summary>
        /// 供应商Id
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        public String CompanyName { get; set; }

        /// <summary>
        /// 供应商类型
        /// </summary>
        [DataMember]
        public int CompanyType { get; set; }

        /// <summary>
        /// 发货类型
        /// </summary>
        [DataMember]
        public int DeliverType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int State { get; set; }

        [IgnoreDataMember]
        public bool IsUsing => State == (int)Enum.YesOrNo.Yes;

        /// <summary>
        /// 是否为供应商(True 供应商)
        /// </summary>
        [IgnoreDataMember]
        public bool IsSupplier => CompanyType == (int) Enum.CompanyType.Suppliers;

        /// <summary>
        /// true 直邮  false 转运
        /// </summary>
        [IgnoreDataMember]
        public bool IsDirect => CompanyType == (int)Enum.Overseas.DeliverType.Direct;

        public AuthorizeCompanyDTO() { }

        public AuthorizeCompanyDTO(Guid companyId,String companyName,int companyType,int deliverType,int state)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            CompanyType = companyType;
            DeliverType = deliverType;
            State = state;
        }
    }
}
