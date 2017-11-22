using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 交易佣金表
    /// </summary>
    /// zal 2016-07-20
    [Serializable]
    public class WasteBookInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Income { get; set; }

        /// <summary>
        ///创建时间 
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///状态(0:未处理；1:已处理) 
        /// </summary>
        public int State { get; set; }
    }
}
