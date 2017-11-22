using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Enum.Attribute;


namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 发票日统计实体
    /// </summary>
    [Serializable]
    public class InvoiceStatisticsInfo
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="noteStatistics"></param>
        /// <param name="invoiceList"></param>
        public InvoiceStatisticsInfo(IList<InvoiceNoteStatisticsInfo> noteStatistics, IList<InvoiceBriefInfo> invoiceList)
        {
            NoteStatistics = noteStatistics;
            InvoiceList = invoiceList;
        }

        /// <summary>
        /// 票据金额统计
        /// </summary>
        public IList<InvoiceNoteStatisticsInfo> NoteStatistics { get; private set; }

        /// <summary>
        /// 票据金额统计
        /// </summary>
        public IList<InvoiceBriefInfo> InvoiceList { get; private set; }
    }

    /// <summary>
    /// 发票票据金额汇总
    /// </summary>
    [Serializable]
    public class InvoiceNoteStatisticsInfo
    {
        /// <summary>
        /// 票据类型
        /// </summary>
        public InvoiceNoteType NoteType { get; set; }

        /// <summary>
        /// 统计发票数量
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 统计金额
        /// </summary>
        public float TotalMoney { get; set; }
    }

    /// <summary>
    /// 发票简短信息
    /// </summary>
    [Serializable]
    public class InvoiceBriefInfo
    {
        /// <summary>
        /// 发票数据ID
        /// </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// 发票订单数据ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 发票对应订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoicePayerName { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public long InvoiceNo { get; set; }

        /// <summary>
        /// 退票代码
        /// </summary>
        public string RetreatInvoiceCode { get; set; }

        /// <summary>
        /// 退票号码
        /// </summary>
        public long RetreatInvoiceNo { get; set; }

        /// <summary>
        /// 发票创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 发票打印时间
        /// </summary>
        public DateTime PrintDate { get; set; }

        /// <summary>
        /// 统计金额
        /// </summary>
        public float TotalMoney { get; set; }

        /// <summary>
        /// 发票是否报税
        /// </summary>
        public bool IsCommit { get; set; }

        /// <summary>
        /// 发票对方纳税人识别号
        /// </summary>
        public string TaxpayerID { get; set; }

        /// <summary>
        /// 票据类型
        /// </summary>
        public InvoiceNoteType NoteType { get; set; }

        /// <summary>
        /// 发票购货方企业类型
        /// </summary>
        public InvoicePurchaserType PurchaserType { get; set; }

        /// <summary>
        /// 发票寄送地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 发票内容
        /// </summary>
        public string InvoiceContent { get; set; }

        /// <summary>
        /// 票据类型名称
        /// </summary>
        public string NoteTypeName
        {
            get
            {
                if ((int)ERP.Enum.InvoiceKindType.Electron == InvoiceKindType)
                {
                    return NoteType == InvoiceNoteType.Effective ? "蓝票" : NoteType == InvoiceNoteType.Abolish ? "红票" : "-";
                }
                return EnumAttribute.GetKeyName(NoteType);
            }
        }

        /// <summary>
        /// 票据状态
        /// </summary>
        public InvoiceState State { get; set; }

        /// <summary>
        /// 票据状态名称
        /// </summary>
        public string StateName
        {
            get { return (int)ERP.Enum.InvoiceKindType.Electron == InvoiceKindType && State==InvoiceState.All ?"-" : EnumAttribute.GetKeyName(State); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid SaleFilialeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int InvoiceKindType { get; set; }

        /// <summary>
        /// 税额 
        /// </summary>
        public decimal RateMoney { get; set; }

        /// <summary>
        /// 价税合计：商品销售总额
        /// </summary>
        public decimal ActualMoney { get; set; }
    }
}
