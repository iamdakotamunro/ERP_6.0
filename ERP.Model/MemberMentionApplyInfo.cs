using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 会员提现申请模型
    /// Add by liucaijun at 2011-October-21th
    /// </summary>
    [Serializable]
    [DataContract]
    public class MemberMentionApplyInfo
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MemberMentionApplyInfo()
        {
        }

        ///<summary>
        /// 构造函数--总后台用
        ///</summary>
        ///<param name="id">ID</param>
        ///<param name="applyTime">申请时间</param>
        ///<param name="processTime">受理时间</param>
        ///<param name="userName">会员用户名</param>
        ///<param name="amount">提现金额</param>
        ///<param name="balance">账户余额</param>
        ///<param name="bankName">银行名称</param>
        ///<param name="bankAccountName">银行账户名称</param>
        ///<param name="bankAccounts">银行账号</param>
        ///<param name="state">状态</param>
        ///<param name="memo">备注</param>
        /// <param name="memberId">会员id</param>
        /// <param name="applyNo">提现单号</param>
        ///<param name="description">操作流水</param>
        ///<param name="saleFilialeId">如果是门店，则FromSourceId字段，保存的是门店公司id如果是网站，则FromSourceId字段，保存的是 网站的标识id</param>
        ///<param name="salePlatformId">来源类型分为：0 = 门店，1 = 网站。</param>
        public MemberMentionApplyInfo(Guid id,DateTime applyTime,DateTime processTime,string userName,Decimal amount,
            Decimal balance, string bankName, string bankAccountName, string bankAccounts, int state, string memo,
            Guid memberId, string applyNo, string description, Guid saleFilialeId, Guid salePlatformId)
            : this(id, applyTime, processTime, userName, amount, balance, bankName, bankAccountName, bankAccounts, state, memo, memberId, applyNo, description)
        {
            SaleFilialeId = saleFilialeId;
            SalePlatformId = salePlatformId;
        }

        ///<summary>
        /// 构造函数--分后台用(EYESEE,KEEDE)
        ///</summary>
        ///<param name="id">ID</param>
        ///<param name="applyTime">申请时间</param>
        ///<param name="processTime">受理时间</param>
        ///<param name="userName">会员用户名</param>
        ///<param name="amount">提现金额</param>
        ///<param name="balance">账户余额</param>
        ///<param name="bankName">银行名称</param>
        ///<param name="bankAccountName">银行账户名称</param>
        ///<param name="bankAccounts">银行账号</param>
        ///<param name="state">状态</param>
        ///<param name="memo">备注</param>
        /// <param name="memberId">会员id</param>
        /// <param name="applyNo">提现单号</param>
        ///<param name="description">操作流水</param>
        MemberMentionApplyInfo(Guid id, DateTime applyTime, DateTime processTime, string userName, Decimal amount,
            Decimal balance, string bankName, string bankAccountName, string bankAccounts, int state, string memo,
            Guid memberId, string applyNo, string description)
        {
            Id = id;
            ApplyTime = applyTime;
            ProcessTime = processTime;
            UserName = userName;
            Amount = amount;
            Balance = balance;
            BankName = bankName;
            BankAccountName = bankAccountName;
            BankAccounts = bankAccounts;
            State = state;
            Memo = memo;
            MemberId = memberId;
            ApplyNo = applyNo;
            Description = description;
        }

        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public Guid Id{ get; set;}
        /// <summary>
        /// 申请时间
        /// </summary>
        [DataMember]
        public DateTime ApplyTime{ get; set;}
        /// <summary>
        /// 受理时间
        /// </summary>
        [DataMember]
        public DateTime ProcessTime{ get; set;}
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName{ get; set;}
        /// <summary>
        /// 提现金额
        /// </summary>
        [DataMember]
        public Decimal Amount{ get; set;}
        /// <summary>
        /// 账户余额
        /// </summary>
        [DataMember]
        public Decimal Balance{ get; set;}
        /// <summary>
        /// 银行名称
        /// </summary>
        [DataMember]
        public string BankName{ get; set;}
        /// <summary>
        /// 银行账户名称
        /// </summary>
        [DataMember]
        public string BankAccountName{ get; set;}
        /// <summary>
        /// 银行帐号
        /// </summary>
        [DataMember]
        public string BankAccounts{ get; set;}
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int State{ get; set;}
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo { get; set;}
        /// <summary>
        /// 会员ID
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }
        /// <summary>
        /// 提现单号
        /// </summary>
        [DataMember]
        public string ApplyNo { get; set; }
        /// <summary>
        /// 操作流水
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 销售公司
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 销售平台
        /// </summary>
        [DataMember]
        public Guid SalePlatformId { get; set; }

        /// <summary>订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>第三方平台订单号
        /// </summary>
        public string ThirdPartyOrderNo { get; set; }
    }
}
