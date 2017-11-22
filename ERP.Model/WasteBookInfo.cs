using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Keede.Ecsoft.Model
{
    /// <summary>▄︻┻┳═一 资金流模型  最后修改提交  陈重文  2014-12-25 
    /// </summary>
    [Serializable]
    [DataContract]
    public class WasteBookInfo
    {
        /// <summary>记账本ID
        /// </summary>
        [DataMember]
        public Guid WasteBookId { get; set; }

        /// <summary> 银行账户ID
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary> 账单编号
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string TradeCodeforT { get; set; }

        /// <summary> 创建日期
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary> 收入
        /// </summary>
        [DataMember]
        public decimal Income { get; set; }

        /// <summary>权衡实际收入
        /// </summary>
        [DataMember]
        public decimal NonceBalance { get; set; }

        /// <summary>审计状态
        /// </summary>
        [DataMember]
        public int AuditingState { get; set; }

        /// <summary>记账本类型
        /// </summary>
        [DataMember]
        public int WasteBookType { get; set; }

        /// <summary>销售公司ID
        /// </summary>
        [DataMember]
        public Guid SaleFilialeId
        {
            get;
            set;
        }

        /// <summary>产生资金流的单据号
        /// </summary>
        [DataMember]
        public string LinkTradeCode { get; set; }

        /// <summary>产生资金流的对应单据类型
        /// </summary>
        [DataMember]
        public int LinkTradeType { get; set; }

        /// <summary>资金流单据状态（枚举WasteBookState   1.正常，2红冲）
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>资金交易流水号(记录银行、第三方单据号)
        /// </summary>
        [DataMember]
        public string BankTradeCode { get; set; }

        /// <summary>记录字段（对内自己用的一个字段 ，用于查询）
        /// </summary>
        [DataMember]
        public bool IsOut { get; set; }

        /// <summary>
        /// 资金流来源(1:天猫、京东、第三方交易佣金;2:积分代扣;3:订单交易金额;)
        /// </summary>
        /// zal 2016-05-18
        [DataMember]
        public int WasteSource { get; set; }

        /// <summary>
        /// 操作状态(0:未处理；1:已处理)
        /// </summary>
        /// zal 2016-09-21
        [DataMember]
        public int OperateState { get; set; }

        /// <summary>是否转账
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public Boolean IsTranfer
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TradeCode))
                {
                    return false;
                }
                if (TradeCode.Length < 2)
                {
                    return false;
                }
                return TradeCode.Substring(0, 2) == "VI";
            }
        }

        /// <summary>默认构造函数
        /// </summary>
        public WasteBookInfo()
        {
        }

        /// <summary> 用于查询方法
        /// </summary>
        /// <param name="wasteBookId">记账本ID</param>
        /// <param name="bankAccountsId">银行账户ID</param>
        /// <param name="tradeCode">账单编号</param>
        /// <param name="dateCreated">创建日期</param>
        /// <param name="description">描述</param>
        /// <param name="income">收入</param>
        /// <param name="nonceBalance">权衡实际收入</param>
        /// <param name="auditingState">审计状态</param>
        /// <param name="wasteBookType">记账本类型</param>
        /// <param name="saleFilialeId">公司ID </param>
        public WasteBookInfo(Guid wasteBookId, Guid bankAccountsId, string tradeCode, DateTime dateCreated, string description, decimal income, decimal nonceBalance, int auditingState, int wasteBookType, Guid saleFilialeId)
        {
            WasteBookId = wasteBookId;
            BankAccountsId = bankAccountsId;
            TradeCode = tradeCode;
            DateCreated = dateCreated;
            Description = description;
            Income = income;
            NonceBalance = nonceBalance;
            AuditingState = auditingState;
            WasteBookType = wasteBookType;
            SaleFilialeId = saleFilialeId;
        }

        /// <summary> 用于插入方法
        /// </summary>
        /// <param name="wasteBookId">记账本ID</param>
        /// <param name="bankAccountsId">银行账户ID</param>
        /// <param name="tradeCode">账单编号</param>
        /// <param name="description">描述</param>
        /// <param name="income">收入</param>
        /// <param name="auditingState">审计状态</param>
        /// <param name="wasteBookType">记账本类型</param>
        /// <param name="saleFilialeId"></param>
        public WasteBookInfo(Guid wasteBookId, Guid bankAccountsId, string tradeCode, string description, decimal income, int auditingState, int wasteBookType, Guid saleFilialeId)
        {
            WasteBookId = wasteBookId;
            BankAccountsId = bankAccountsId;
            TradeCode = tradeCode;
            Description = description;
            Income = income;
            AuditingState = auditingState;
            WasteBookType = wasteBookType;
            SaleFilialeId = saleFilialeId;
        }

        /// <summary>用于更新方法
        /// </summary>
        /// <param name="wasteBookId">记账本ID</param>
        /// <param name="dateCreated">创建日期</param>
        /// <param name="description">描述</param>
        /// <param name="income">收入</param>
        /// <param name="wasteBookType">记账本类型</param>
        public WasteBookInfo(Guid wasteBookId, DateTime dateCreated, string description, decimal income, int wasteBookType)
        {
            WasteBookId = wasteBookId;
            DateCreated = dateCreated;
            Description = description;
            Income = income;
            WasteBookType = wasteBookType;
        }

        /// <summary>
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is WasteBookInfo)
                return (compareObj as WasteBookInfo).WasteBookId == WasteBookId;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (WasteBookId == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + WasteBookId;
            return stringRepresentation.GetHashCode();
        }
    }
}
