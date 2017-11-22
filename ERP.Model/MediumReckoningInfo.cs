using System;

namespace ERP.Model
{
    /// <summary>
    /// create by liangcanren ,2014-2-1
    /// 用于对账，处理完成往来账的修改和添加中间模型
    /// </summary>
    [Serializable]
    public class MediumReckoningInfo
    {
        /// <summary>
        /// 账单编号
        /// </summary>
        public Guid ReckoningId { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        public String FilialeName { get; set; }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 账单编号
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary>
        /// 新建日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 金额.为正数，表示应收帐。.为负数，表示应付帐。
        /// </summary>
        public decimal AccountReceivable { get; set; }

        /// <summary>
        /// 该往来单位当前余额
        /// </summary>
        public decimal NonceTotalled { get; set; }

        /// <summary>
        /// 账单类型：1.应收款2.应付款3.已收4.已付5.增加资金6.减少资金
        /// </summary>
        public int ReckoningType { get; set; }

        /// <summary>
        /// 账单状态
        /// </summary>
        public int ReckoningState { get; set; }

        /// <summary>
        /// 对账状态
        /// </summary>
        public int IsChecked { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int AuditingState { get; set; }

        /// <summary>
        /// 原始单据号[快递单号]
        /// </summary>
        public string OriginalTradeCode { get; set; }
	    
                /// <summary>
        /// 该账目是哪个仓库产生的
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 往来账对账类型
        /// </summary>
        public int ReckoningCheckType{ get; set; }

        /// <summary>
        /// 对应的对账记录Id
        /// </summary>
        public Guid CheckId { get; set; }

        /// <summary>
        /// 往来账处理类型，0修改，1，添加
        /// </summary>
        public int HandleType { get; set; }

        /// <summary>
        /// 调账金额
        /// </summary>
        public decimal DiffMoney { get; set; }

        /// <summary>
        /// </summary>
        public Boolean IsOut { get; set; }
    }
}
