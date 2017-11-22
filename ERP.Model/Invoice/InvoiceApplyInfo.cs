using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ERP.Enum.ApplyInvocie;

namespace ERP.Model.Invoice
{
    [DataContract]
    [Serializable]
    public class InvoiceApplyInfo
    {
        [DataMember]
        public Guid ApplyId { get; set; }

        /// <summary>
        /// 订单类型的发票申请为订单号/加盟店类型为加盟店生成单号
        /// </summary>
        [DataMember]
        public string TradeCode { get; set; }

        /// <summary>
        /// 第三方订单号(加盟店类型的申请为空)
        /// </summary>
        [DataMember]
        public string ThirdPartyCode { get; set; }

        /// <summary>
        /// 订单类型为会员Id,加盟店类型为加盟店Id
        /// </summary>
        [DataMember]
        public Guid TargetId { get; set; }

        [DataMember]
        public DateTime ApplyDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ApplyState { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        [DataMember]
        public int ApplyType { get; set; }

        /// <summary>
        /// 票据类型(贷款发票、保证金)
        /// </summary>
        [DataMember]
        public int ApplyKind { get; set; }

        /// <summary>
        /// 票据来源类型
        /// </summary>
        [DataMember]
        public int ApplySourceType { get; set; }


        public Guid AuditorId { get; set; }

        public string Transactor { get; set; }

        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 加盟店类型总金额为明细总金额之和
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// 拒绝理由
        /// </summary>
        [DataMember]
        public string RetreatRemark { get; set; }

        /// <summary>
        /// 发票抬头类型
        /// </summary>
        [DataMember]
        public int InvoiceTitleType{ get; set; }

        #region 发票基础信息
        /// <summary>
        /// 发票抬头
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        [DataMember]
        public string TaxpayerNumber { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        [DataMember]
        public string BankAccountNo { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        [DataMember]
        public string ContactAddress { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DataMember]
        public string ContactTelephone { get; set; }
        #endregion

        #region 收货人信息
        /// <summary>
        /// 收货人
        /// </summary>
        [DataMember]
        public string Receiver { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Telephone { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        [DataMember]
        public string ApplyRemark { get; set; }
        #endregion

        [DataMember]
        public Guid SaleFilialeId { get; set; }

        [DataMember]
        public Guid SalePlatformId { get; set; }



        public List<InvoiceApplyDetailInfo> Details { get; set; }

        public List<InvoiceRelationInfo> RelationInfos { get; set; }

        public InvoiceApplyInfo() { }

        /// <summary>
        /// (加盟店)新增保证金
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="titleType"></param>
        /// <param name="targetId"></param>
        /// <param name="title"></param>
        /// <param name="receiver"></param>
        /// <param name="telephone"></param>
        /// <param name="address"></param>
        /// <param name="applyRemark"></param>
        /// <param name="saleFilialeId"></param>
        public InvoiceApplyInfo(string tradeCode,int titleType,int applyKind,Guid targetId,string title,string receiver,string telephone,string address,string applyRemark,Guid saleFilialeId)
        {
            ApplyId = Guid.NewGuid();
            TradeCode = tradeCode;
            TargetId = targetId;
            ApplyDateTime = DateTime.Now;
            InvoiceTitleType = titleType;
            Title = title;
            Receiver = receiver;
            Telephone = telephone;
            Address = address;
            ApplyRemark = applyRemark;
            ApplyType = (int) ApplyInvoiceType.Receipt;
            ApplyKind = (int)applyKind;
            ApplySourceType = (int)ApplyInvoiceSourceType.League;
            ApplyState = (int)ApplyInvoiceState.WaitInvoicing;
            SaleFilialeId = saleFilialeId;
            SalePlatformId = targetId;
            SalePlatformId = targetId;
            TaxpayerNumber = string.Empty;
            ThirdPartyCode = string.Empty;
        }

        /// <summary>
        /// (加盟店)贷款发票
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="titleType"></param>
        /// <param name="applyType"></param>
        /// <param name="targetId"></param>
        /// <param name="title"></param>
        /// <param name="taxNumber"></param>
        /// <param name="bankName"></param>
        /// <param name="bankAccountNo"></param>
        /// <param name="contactAddress"></param>
        /// <param name="contactPhone"></param>
        /// <param name="receiver"></param>
        /// <param name="telephone"></param>
        /// <param name="address"></param>
        /// <param name="applyRemark"></param>
        /// <param name="saleFilialeId"></param>
        public InvoiceApplyInfo(string tradeCode, int titleType,int applyType,int applyKind, Guid targetId, string title,string taxNumber,string bankName,string bankAccountNo,string contactAddress,string contactPhone, string receiver, string telephone, string address, string applyRemark, Guid saleFilialeId)
        {
            ApplyId = Guid.NewGuid();
            TradeCode = tradeCode;
            ThirdPartyCode = string.Empty;
            TargetId = targetId;
            ApplyDateTime = DateTime.Now;
            InvoiceTitleType = titleType;
            Title = title;
            TaxpayerNumber = taxNumber;
            BankName = bankName;
            BankAccountNo = bankAccountNo;
            ContactAddress = contactAddress;
            ContactTelephone = contactPhone;
            Receiver = receiver;
            Telephone = telephone;
            Address = address;
            ApplyRemark = applyRemark;
            ApplyType = applyType;
            ApplyKind = applyKind;
            ApplyState = (int)ApplyInvoiceState.WaitAudit;
            ApplySourceType = (int)ApplyInvoiceSourceType.League;
            SaleFilialeId = saleFilialeId;
            SalePlatformId = targetId;
        }

        /// <summary>
        /// 订单类型新增发票
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="thirdPartyCode"></param>
        /// <param name="amount"></param>
        /// <param name="titleType"></param>
        /// <param name="applyType"></param>
        /// <param name="targetId"></param>
        /// <param name="title"></param>
        /// <param name="taxNumber"></param>
        /// <param name="bankName"></param>
        /// <param name="bankAccountNo"></param>
        /// <param name="contactAddress"></param>
        /// <param name="contactPhone"></param>
        /// <param name="receiver"></param>
        /// <param name="telephone"></param>
        /// <param name="address"></param>
        /// <param name="applyRemark"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformId"></param>
        public InvoiceApplyInfo(string orderNo,string thirdPartyCode,decimal amount, int titleType, int applyType,int applyKind, Guid targetId, string title, string taxNumber, string bankName, string bankAccountNo, string contactAddress, string contactPhone, string receiver, string telephone, string address, string applyRemark, Guid saleFilialeId, Guid salePlatformId)
        {
            ApplyId = Guid.NewGuid();
            TradeCode = orderNo;
            ThirdPartyCode = thirdPartyCode;
            TargetId = targetId;
            ApplyDateTime = DateTime.Now;
            InvoiceTitleType = titleType;
            Title = title;
            TaxpayerNumber = taxNumber;
            BankName = bankName;
            BankAccountNo = bankAccountNo;
            ContactAddress = contactAddress;
            ContactTelephone = contactPhone;
            Receiver = receiver;
            Telephone = telephone;
            Address = address;
            ApplyRemark = applyRemark;
            ApplyType = applyType;
            ApplySourceType = (int)ApplyInvoiceSourceType.Order;
            ApplyKind = applyKind;
            ApplyState = (int)ApplyInvoiceState.WaitAudit;
            SaleFilialeId = saleFilialeId;
            SalePlatformId = salePlatformId;
            Amount = amount;
        }
    }
}
