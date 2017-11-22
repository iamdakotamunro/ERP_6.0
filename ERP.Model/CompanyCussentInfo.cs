//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年4月27日
// 文件创建人:马力
// 最后修改时间:2006年4月27日
// 最后一次修改人:马力
//================================================
using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来单位类
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyCussentInfo
    {
        /// <summary>
        /// 往来单位编号
        /// </summary>
        [DataMember]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 往来单位分类编号
        /// </summary>
        [DataMember]
        public Guid CompanyClassId { get; set; }

        /// <summary>
        /// 往来单位名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [DataMember]
        public string Linkman { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [DataMember]
        public string Fax { get; set; }

        /// <summary>
        /// 往来单位的公司网站
        /// </summary>
        [DataMember]
        public string WebSite { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// 银行帐户
        /// </summary>
        [DataMember]
        public string BankAccounts { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        [DataMember]
        public string AccountNumber { get; set; }

        /// <summary>
        /// 本公司的银行帐号名称
        /// </summary>
        [DataMember]
        public string OwnBankAccountName { get; set; }

        /// <summary>
        /// 本公司的对应银行帐号表ID
        /// </summary>
        [DataMember]
        public Guid OwnBankAccountID { get; set; }

        /// <summary>
        /// 是否需要发票
        /// </summary>
        [DataMember]
        public bool IsNeedInvoices { get; set; }

        /// <summary>
        /// 新建日期
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 往来单位类型：0  其他 ,1  供应商 2  销售商3  物流公司 4  会员总帐
        /// </summary>
        [DataMember]
        public int CompanyType { get; set; }

        /// <summary>
        /// 状态 ：1  使用； 0 搁置
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 详细说明
        /// </summary>
        [DataMember]
        public string SubjectInfo { get; set; }

        /// <summary>
        /// 单位资料
        /// </summary>
        [DataMember]
        public string Information { get; set; }

        /// <summary>往来单位账期
        /// </summary>
        [DataMember]
        public int PaymentDays { get; set; }

        /// <summary>
        /// 资质是否完整
        /// </summary>
        [DataMember]
        public int Complete { get; set; }

        /// <summary>
        /// 是否过期
        /// </summary>
        [DataMember]
        public string Expire { get; set; }

        [DataMember]
        public Guid RelevanceFilialeId { get; set; }

        public int SalesScope { get; set; }

        public int DeliverType { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CompanyCussentInfo() { }

        /// <summary>
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="companyClassId">往来单位分类编号</param>
        /// <param name="companyName">往来单位名称</param>
        /// <param name="linkman">联系人</param>
        /// <param name="address">地址</param>
        /// <param name="postalCode">邮政编码</param>
        /// <param name="phone">电话</param>
        /// <param name="mobile">手机</param>
        /// <param name="fax">传真</param>
        /// <param name="webSite">网站</param>
        /// <param name="email">邮箱</param>
        /// <param name="bankAccounts">银行账户</param>
        /// <param name="accountNumber">银行帐号</param>
        /// <param name="dateCreated">新建日期</param>
        /// <param name="companyType">往来单位类型：0  其他 ,1  供应商 2  销售商3  物流公司 4  会员总帐</param>
        /// <param name="state">状态 ：1  使用； 0 搁置</param>
        /// <param name="description">备注说明</param>
        /// <param name="subjectinfo">详细说明</param>
        /// <param name="filialeId"></param>
        /// <param name="salesScope">销售范围</param>
        /// <param name="deliverType">发货类型</param>
        public CompanyCussentInfo(Guid companyId, Guid companyClassId, string companyName, string linkman, string address, string postalCode, string phone, string mobile, string fax, string webSite, string email, string bankAccounts, string accountNumber, DateTime dateCreated, int companyType, int state, string description, string subjectinfo, Guid filialeId, int salesScope, int deliverType)
        {
            CompanyId = companyId;
            CompanyClassId = companyClassId;
            CompanyName = companyName;
            Linkman = linkman;
            Address = address;
            PostalCode = postalCode;
            Phone = phone;
            Mobile = mobile;
            Fax = fax;
            WebSite = webSite;
            Email = email;
            BankAccounts = bankAccounts;
            AccountNumber = accountNumber;
            DateCreated = dateCreated;
            CompanyType = companyType;
            State = state;
            Description = description;
            SubjectInfo = subjectinfo;
            RelevanceFilialeId = filialeId;
            SalesScope = salesScope;
            DeliverType = deliverType;
        }
    }
}
