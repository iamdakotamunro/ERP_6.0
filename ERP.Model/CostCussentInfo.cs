//================================================
//Func : 费用 往来单位类
//Code : dyy Modify by liucaijun at 2011-February-25th
//Date: 2009 sept 16th
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>费用单位信息类
    /// </summary>
    [Serializable]
    public class CostCussentInfo
    {
        //基本字段属性

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public Guid CompanyId { get;  set; }

        /// <summary>
        /// 往来单位类型ID
        /// </summary>
        public Guid CompanyClassId { get; set; }

        /// <summary>
        /// 往来单位名
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Linkman { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 网站
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccounts { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 往来单位类型
        /// </summary>
        public int CompanyType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///主题信息
        /// </summary>
        public string SubjectInfo { get; set; }

        /// <summary>
        /// 发票打款账号
        /// </summary>
        public Guid InvoiceAccountsId { get; set; }

        /// <summary>
        /// 凭证打款账号
        /// </summary>
        public Guid VoucherAccountsId { get; set; }

        /// <summary>
        /// 现金打款账号
        /// </summary>
        public Guid CashAccountsId { get; set; }

        /// <summary>
        /// 无凭证打款账号
        /// </summary>
        public Guid NoVoucherAccountsId { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CostCussentInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="companyClassId">往来单位类型ID</param>
        /// <param name="companyName">往来单位名</param>
        /// <param name="linkman">联系人</param>
        /// <param name="address">地址</param>
        /// <param name="postalCode">邮政编码</param>
        /// <param name="phone">电话</param>
        /// <param name="mobile">手机</param>
        /// <param name="fax">传真</param>
        /// <param name="webSite">网站</param>
        /// <param name="email">邮箱</param>
        /// <param name="bankAccounts">银行账户</param>
        /// <param name="accountNumber">账户</param>
        /// <param name="dateCreated">创建日期</param>
        /// <param name="companyType">往来单位类型</param>
        /// <param name="state">状态</param>
        /// <param name="description">描述</param>
        /// <param name="subjectinfo">主题信息</param>
        /// <param name="invoiceAccountsId">发票打款账号</param>
        /// <param name="voucherAccountsId">凭证打款账号</param>
        /// <param name="cashAccountsId">现金打款账号</param>
        /// <param name="noVoucherAccountsId">无凭证打款账号</param>
        public CostCussentInfo(Guid companyId, Guid companyClassId, string companyName, string linkman, string address,
            string postalCode, string phone, string mobile, string fax, string webSite, string email, string bankAccounts,
            string accountNumber, DateTime dateCreated, int companyType, int state, string description, string subjectinfo,
            Guid invoiceAccountsId, Guid voucherAccountsId, Guid cashAccountsId, Guid noVoucherAccountsId)
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
            InvoiceAccountsId = invoiceAccountsId;
            VoucherAccountsId = voucherAccountsId;
            CashAccountsId = cashAccountsId;
            NoVoucherAccountsId = noVoucherAccountsId;
        }
    }
}
