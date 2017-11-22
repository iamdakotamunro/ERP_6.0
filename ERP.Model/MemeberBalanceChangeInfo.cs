using System;

namespace ERP.Model
{
    /// <summary>会员余额管理模型   ADD  2014-09-02   陈重文
    /// </summary>
    public class MemeberBalanceChangeInfo
    {
        /// <summary>申请人ID
        /// </summary>
        public Guid ApplicantID { get; set; }

        /// <summary>申请人
        /// </summary>
        public string ApplicantName { get; set; }

        /// <summary>单据ID
        /// </summary>
        public Guid ApplyID { get; set; }

        /// <summary> 余额充值此编号为交易号，余额赠送和余额扣除则为订单号
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary>余额充值银行ID
        /// </summary>
        public Guid BankAccountId { get; set; }

        /// <summary>用户ID
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>当前余额
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>存入
        /// </summary>
        public decimal Increase { get; set; }

        /// <summary>支出
        /// </summary>
        public decimal Subtract { get; set; }

        /// <summary>余额变动状态
        /// </summary>
        public int State { get; set; }

        /// <summary> 操作类型
        /// </summary>
        public int UserBalanceChangeType { get; set; }

        /// <summary>备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>销售公司ID
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>销售平台ID
        /// </summary>
        public Guid SalePlatformId { get; set; }

        /// <summary>
        /// 支付宝
        /// </summary>
        public string PayTreasureAccount { get; set; }

        /// <summary>
        /// 问题类型
        /// </summary>
        public string TypeOfProblemName { get; set; }

        /// <summary>
        /// 真实名称
        /// </summary>
        public string PayeeRealName { get; set; }

        /// <summary>
        /// 支付宝/银行名称
        /// </summary>
        public string PayBankName { get; set; }

    }
}
