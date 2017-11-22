using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 资金账户信息类
    /// </summary>
    [Serializable]
    [DataContract]
    public class BankAccountInfo
    {
        /// <summary>
        /// 绑定银行账户的相关公司或销售平台ID
        /// </summary>
        [DataMember]
        public Guid TargetId
        {
            get;
            set;
        }

        /// <summary>
        /// 银行账户编号
        /// </summary>
        [DataMember]
        public Guid BankAccountsId
        {
            get;
            set;
        }

        /// <summary>
        /// 银行名称
        /// </summary>
        [DataMember]
        public string BankName
        {
            get;
            set;
        }

        /// <summary>
        /// 支付接口编号
        /// </summary>
        [DataMember]
        public Guid PaymentInterfaceId
        {
            get;
            set;
        }

        /// <summary>
        /// 账户名
        /// </summary>
        [DataMember]
        public string AccountsName
        {
            get;
            set;
        }

        /// <summary>
        /// 帐号
        /// </summary>
        [DataMember]
        public string Accounts
        {
            get;
            set;
        }

        /// <summary>
        /// 帐号密匙
        /// </summary>
        [DataMember]
        public string AccountsKey
        {
            get;
            set;
        }

        /// <summary>
        /// 支付类型0，邮局汇款。1，现金。2，网络支付。3，银行汇款。4，电汇。5，其它支付类型。6，信用卡。
        /// </summary>
        [DataMember]
        public int PaymentType
        {
            get;
            set;
        }

        /// <summary>
        /// 银行图标。会在网站前台款到发货订单选择支付方式后显示。
        /// </summary>
        [DataMember]
        public string BankIcon
        {
            get;
            set;
        }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int OrderIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 描述。会在网站前台款到发货订单选择支付方式后显示。
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 是否启用
        /// Add By liucaijun at 2011-October-08th
        /// </summary>
        [DataMember]
        public bool IsUse
        {
            get;
            set;
        }
        /// <summary>
        /// 是否需要完成
        /// </summary>
        [DataMember]
        public bool IsFinish
        {
            get;
            set;
        }

        /// <summary>
        /// 是否原路返回
        /// </summary>
        [DataMember]
        public bool IsBacktrack { get; set; }

        /// <summary>是否主账号
        /// </summary>
        [DataMember]
        public bool IsMain { get; set; }

        /// <summary>
        /// 是否前台显示
        /// add by liangcanren at 2015-05-04
        /// </summary>
        [DataMember]
        public bool IsDisplay { get; set; }

        /// <summary>账户余额
        /// </summary>
        [DataMember]
        public decimal NonceBalance { get; set; }
        /// <summary>
        /// 银行账户类型
        /// zal 2015-08-21
        /// </summary>
        [DataMember]
        public int AccountType { get; set; }
    }
}
