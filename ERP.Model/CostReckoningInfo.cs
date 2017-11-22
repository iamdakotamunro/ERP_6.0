//*****************************//
//** Func : 费用流水账
//** Date : 2009-6-22
//** Code : dyy
//*****************************//

using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 费用流水账
    /// </summary>
    [Serializable]
    public class CostReckoningInfo
    {
        /// <summary>
        /// 账目编号
        /// </summary>
        public Guid ReckoningId { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 结算公司ID
        /// </summary>
        public Guid AssumeFilialeId { get; set; }

        /// <summary>
        /// 往来单位编号
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 单据编号,整个流程下单单据
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 单据说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 应收款账
        /// </summary>
        public decimal AccountReceivable { get; set; }

        /// <summary>
        /// 应收总额
        /// </summary>
        public decimal NonceTotalled { get; set; }

        /// <summary>
        /// 账目类型
        /// </summary>
        public int ReckoningType { get; set; }

        /// <summary>
        /// 账目状态
        /// </summary>
        public int ReckoningState { get; set; }

        /// <summary>
        /// 是否对账
        /// </summary>
        public int IsChecked { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>
        public int AuditingState { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CostReckoningInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guReckoningID">账目编号</param>
        public CostReckoningInfo(Guid guReckoningID)
        {
            ReckoningId = guReckoningID;
        }

        /// <summary>
        /// 初始化构造函数   --  add by dyy 2009-07-02 edit by lxm 20100521
        /// </summary>
        /// <param name="reckoningId">账目编号</param>
        /// <param name="filialeId">公司编号</param>
        /// <param name="assumeFilialeId">结算公司Id</param>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="tradeCode">单据编号,整个流程下单单据</param>
        /// <param name="dateCreated">创建时间</param>
        /// <param name="description">单据说明</param>
        /// <param name="accountReceivable">应收款账</param>
        /// <param name="nonceTotalled">应收总额</param>
        /// <param name="reckoningState">账目状态</param>
        /// <param name="isChecked">是否对账</param>
        /// <param name="auditingState">是否审核</param>
        /// <param name="reckoningType">账目类型</param>
        public CostReckoningInfo(Guid reckoningId, Guid filialeId, Guid assumeFilialeId, Guid companyId, string tradeCode, DateTime dateCreated, string description, decimal accountReceivable, decimal nonceTotalled, int reckoningType, int reckoningState, int isChecked, int auditingState)
        {
            ReckoningId = reckoningId;
            FilialeId = filialeId;
            AssumeFilialeId = assumeFilialeId;
            CompanyId = companyId;
            TradeCode = tradeCode;
            DateCreated = dateCreated;
            Description = description;
            AccountReceivable = accountReceivable;
            NonceTotalled = nonceTotalled;
            ReckoningType = reckoningType;
            ReckoningState = reckoningState;
            IsChecked = isChecked;
            AuditingState = auditingState;
        }
    }
}
