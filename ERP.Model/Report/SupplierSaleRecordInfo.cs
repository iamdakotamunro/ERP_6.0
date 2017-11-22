using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 公司月销售记录(数据)模型
    /// </summary>
    [Serializable]
    public class SupplierSaleRecordInfo
    {
        public Guid CompanyID { get; set; }

        public Guid CompanyName { get; set; }

        public decimal TotalSettlePrice { get; set; }

        public int Quantity { get; set; }

        public DateTime DayTime { get; set; }

    }
}
