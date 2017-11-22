using System;

namespace ERP.Model
{
    [Serializable]
    public class AfterSaleInfo
    {
        /// <summary> 
        ///	售后处理ID	
        /// </summary> 
        public Guid AfterSaleId { get; set; }
        /// <summary> 
        ///	售后处理NO	
        /// </summary> 
        public string AfterSaleNo { get; set; }
        /// <summary> 
        ///	订单ID	
        /// </summary> 
        public Guid OrderId { get; set; }
        /// <summary> 
        ///	订单编号	
        /// </summary> 
        public string OrderNo { get; set; }
        /// <summary> 
        ///	退款金额	
        /// </summary> 
        public decimal Amount { get; set; }
        /// <summary> 
        ///	售后类型	
        /// </summary> 
        public int TreatmentType { get; set; }
        /// <summary> 
        ///	是否补发	
        /// </summary> 
        public bool IsAllReissue { get; set; }
        /// <summary> 
        ///	退换货仓库	
        /// </summary> 
        public Guid WarehouseId { get; set; }
        /// <summary> 
        ///	扣除运费	
        /// </summary> 
        public decimal DeductCarriage { get; set; }
        /// <summary> 
        ///	扣除活动	
        /// </summary> 
        public decimal DeductPromotion { get; set; }
        /// <summary> 
        ///	扣除礼券	
        /// </summary> 
        public decimal DeductCoupon { get; set; }
        /// <summary> 
        ///	扣除赠品	
        /// </summary> 
        public decimal DeductGift { get; set; }
        /// <summary> 
        ///	退货理由	
        /// </summary> 
        public string ReturnReason { get; set; }
        /// <summary> 
        ///	备注	
        /// </summary> 
        public string Remark { get; set; }
        /// <summary> 
        ///	创建时间	
        /// </summary> 
        public DateTime CreateTime { get; set; }
        /// <summary> 
        ///	是否已付款	
        /// </summary> 
        public bool IsPayment { get; set; }
        /// <summary> 
        ///	丢件状态：0正常，1丢件赔偿，2丢件找回	
        /// </summary> 
        public int LostStatus { get; set; }
        /// <summary> 
        ///	快件赔偿状态：0正常，1已赔偿	
        /// </summary> 
        public int BadStatus { get; set; }
        /// <summary> 
        ///	丢件赔偿金额	
        /// </summary> 
        public decimal PayToKeedeMoney { get; set; }
        /// <summary> 
        ///	发票是否已寄回	
        /// </summary> 
        public bool IsReturnInvoice { get; set; }
        /// <summary> 
        ///	新订单ID	
        /// </summary> 
        public Guid NewOrderId { get; set; }
        /// <summary> 
        ///	损坏赔偿金额	
        /// </summary> 
        public decimal PayToKeedeMoneyBad { get; set; }
        /// <summary> 
        ///	申请公司ID	
        /// </summary> 
        public Guid ApplyCompanyId { get; set; }
        /// <summary> 
        ///	积分抵扣	
        /// </summary> 
        public int ScoreDeduction { get; set; }
        /// <summary> 
        ///	是否已经收到货【0=未收到货，1=已收到货】	
        /// </summary> 
        public bool IsReceiving { get; set; }
        /// <summary> 
        ///	用户ID	
        /// </summary> 
        public Guid MemberId { get; set; }
        /// <summary> 
        ///	满意度	
        /// </summary> 
        public int Survey { get; set; }
        /// <summary> 
        ///	售后处理状态：0待审核，10审核通过，20作废，30直接处理，90审核未通过	
        /// </summary> 
        public int Status { get; set; }
        /// <summary> 
        ///	物流名称	
        /// </summary> 
        public string ExpressName { get; set; }
        /// <summary> 
        ///	物流编号	
        /// </summary> 
        public string ExpressNo { get; set; }
        /// <summary> 
        ///	最终处理时间 [作废 或 完成申请退款]	
        /// </summary> 
        public DateTime EndDate { get; set; }
        /// <summary> 
        ///	附件	
        /// </summary> 
        public string Affix { get; set; }
        /// <summary> 
        ///	物流接口ID	
        /// </summary> 
        public Guid ExpressCodeId { get; set; }
        /// <summary> 
        ///	检查状态：-1所有，0待检查，1通过，2未通过	
        /// </summary> 
        public int CheckState { get; set; }
        /// <summary> 
        ///	记录最后时间	
        /// </summary> 
        public DateTime LogMaxTime { get; set; }

    }
}
