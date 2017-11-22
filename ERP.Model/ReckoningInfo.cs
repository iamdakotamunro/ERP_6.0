//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年12月31日
// 文件创建人:马力
// 最后修改时间:2010年8月12日
// 最后一次修改人:刘修明
//================================================
using System;
using ERP.Enum;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>▄︻┻┳═一 往来账模型  最后修改提交  陈重文  2014-12-24 
    /// </summary>
    [Serializable]
    [DataContract]
    public class ReckoningInfo
    {
        /// <summary>往来账记录ID
        /// </summary>
        [DataMember]
        public Guid ReckoningId { get; set; }

        /// <summary> 公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeId { get; set; }

        /// <summary>公司名称
        /// </summary>
        [DataMember]
        public string FilialeName { get; set; }

        /// <summary>往来单位ID
        /// </summary>
        [DataMember]
        public Guid ThirdCompanyID { get; set; }

        /// <summary>往来单位名称 
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>单据编号
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>创建日期
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>账目描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>单据金额.为正数，表示应收帐。.为负数，表示应付帐。
        /// </summary>
        [DataMember]
        public decimal AccountReceivable { get; set; }

        /// <summary>往来单位当前余额
        /// </summary>
        [DataMember]
        public decimal NonceTotalled { get; set; }

        /// <summary> 财务记帐单类型：0收入，1支出
        /// </summary>
        [DataMember]
        public int ReckoningType { get; set; }

        /// <summary> 财务记帐单状态  对应枚举 ReckoningStateType  1正常  ，2红冲
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary> 对账状态 
        /// </summary>
        [DataMember]
        public int IsChecked { get; set; }

        /// <summary> 单据审核状态 对应枚举 AuditingState
        /// </summary>
        [DataMember]
        public int AuditingState { get; set; }

        /// <summary>原始单据号
        /// </summary>
        [DataMember]
        public string LinkTradeCode { get; set; }

        /// <summary> 此往来账是否是货到付款拒收而产生 这个属性，暂时只有对账时候用
        /// </summary>
        [DataMember]
        public bool IsRefuse { get; set; }

        /// <summary> 该账目是哪个仓库产生的
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        /// <summary> 判断构造函数的类型，让方法调用准确的构造函数
        /// </summary>
        [DataMember]
        public ContructType ContructType { get; set; }

        /// <summary> 往来账对账类型  对应枚举  ReckoningCheckType
        /// </summary>
        [DataMember]
        public int ReckoningCheckType { get; set; }

        /// <summary>用于外联ReckoningCheck表，“EMPTY”未获得到数据
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary> 快递差额
        /// </summary>
        [DataMember]
        public double Carriage { get; set; }

        /// <summary> 订单重量
        /// </summary>
        [DataMember]
        public double Weight { get; set; }

        /// <summary>加盟总价
        /// </summary>
        [DataMember]
        public decimal JoinTotalPrice { get; set; }

        /// <summary>对应单据类型 
        /// </summary>
        [DataMember]
        public int LinkTradeType { get; set; }

        /// <summary>记录字段（对内自己用的一个字段 ，用于查询）
        /// </summary>
        [DataMember]
        public bool IsOut { get; set; }

        /// <summary>往来单位所有公司总余额（往来账点击具体往来单位所有显示的余额）
        /// </summary>
        [DataMember]
        public decimal ComCurrBalance { get; set; }

        /// <summary>往来单位公司总余额（对账完成时更新）
        /// </summary>
        [DataMember]
        public decimal CurrentTotalled { get; set; }

        /// <summary>
        /// 是否允许同一公司、同一往来单位、同一财务记帐单类型、同一财务记帐单状态、同一单据审核状态、同一原始单据号、同一单据金额插入相同的账务记录
        /// 允许：True；不允许：False；
        /// </summary>
        /// zal 206-12-24
        [DataMember]
        public bool IsAllow { get; set; }

        /// <summary> 默认构造函数
        /// </summary>
        public ReckoningInfo()
        {
            ContructType = ContructType.Default;
        }

        /// <summary> 
        /// </summary>
        /// <param name="guReckoningID">账单编号</param>
        public ReckoningInfo(Guid guReckoningID)
        {
            ContructType = ContructType.Default;
            ReckoningId = guReckoningID;
        }

        /// <summary>用于查询方法
        /// </summary>
        /// <param name="reckoningId">账单编号</param>
        /// <param name="filialeId">分公司ID</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="tradeCode">账单编号</param>
        /// <param name="dateCreated">新建日期</param>
        /// <param name="description">描述</param>
        /// <param name="accountReceivable">金额.为正数，表示应收帐。.为负数，表示应付帐。</param>
        /// <param name="nonceTotalled">该往来单位当前余额</param>
        /// <param name="reckoningType">账单类型：1.应收款2.应付款3.已收4.已付5.增加资金6.减少资金</param>
        /// <param name="reckoningState">账单状态</param>
        /// <param name="isChecked">默认是0</param>
        /// <param name="auditingState">默认0(int)AuditingState.Unverify</param>
        /// <param name="originalTradeCode">默认是null</param>
        public ReckoningInfo(Guid reckoningId, Guid filialeId, Guid companyId, string tradeCode, DateTime dateCreated, string description, decimal accountReceivable, decimal nonceTotalled, int reckoningType, int reckoningState, int isChecked, int auditingState, string originalTradeCode)
        {
            ReckoningId = reckoningId;
            FilialeId = filialeId;
            ThirdCompanyID = companyId;
            TradeCode = tradeCode;
            DateCreated = dateCreated;
            Description = description;
            AccountReceivable = accountReceivable;
            NonceTotalled = nonceTotalled;
            ReckoningType = reckoningType;
            State = reckoningState;
            IsChecked = isChecked;
            AuditingState = auditingState;
            LinkTradeCode = originalTradeCode;

            //无实际数据库操作字段，判断构造函数类型
            ContructType = ContructType.Select;
        }


        /// <summary>用于插入方法
        /// </summary>
        /// <param name="reckoningId">账单编号</param>
        /// <param name="filialeId">分公司ID</param>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="tradeCode">账单编号</param>
        /// <param name="description">描述</param>
        /// <param name="accountReceivable">金额.为正数，表示应收帐。.为负数，表示应付帐。</param>
        /// <param name="reckoningType">账单类型：1.应收款2.应付款3.已收4.已付5.增加资金6.减少资金</param>
        /// <param name="reckoningState">账单状态</param>
        /// <param name="isChecked">默认是0</param>
        /// <param name="auditingState">默认0(int)AuditingState.Unverify</param>
        /// <param name="originalTradeCode">默认是null</param>
        /// <param name="warehouseId">仓库ID</param>
        public ReckoningInfo(Guid reckoningId, Guid filialeId, Guid companyId, string tradeCode, string description, decimal accountReceivable, int reckoningType, int reckoningState, int isChecked, int auditingState, string originalTradeCode, Guid warehouseId)
        {
            ReckoningId = reckoningId;
            FilialeId = filialeId;
            ThirdCompanyID = companyId;
            TradeCode = tradeCode;
            Description = description;
            AccountReceivable = accountReceivable;
            ReckoningType = reckoningType;
            State = reckoningState;
            IsChecked = isChecked;
            AuditingState = auditingState;
            LinkTradeCode = originalTradeCode;
            WarehouseId = warehouseId;

            //无实际数据库操作字段，判断构造函数类型
            ContructType = ContructType.Insert;
        }

        /// <summary>用于更新方法  
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="accountReceivable">金额.为正数，表示应收帐。.为负数，表示应付帐。</param>
        /// <param name="reckoningId">账单编号</param>
        /// <param name="dateCreated">创建日期</param>
        public ReckoningInfo(decimal accountReceivable, string description, Guid reckoningId, DateTime dateCreated)
        {
            AccountReceivable = accountReceivable;
            Description = description;
            ReckoningId = reckoningId;
            DateCreated = dateCreated;

            //无实际数据库操作字段，判断构造函数类型
            ContructType = ContructType.Update;
        }
    }
}
