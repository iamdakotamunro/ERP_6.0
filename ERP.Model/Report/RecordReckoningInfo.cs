using System;

namespace ERP.Model.Report
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RecordReckoningInfo
    {
        public Guid ReckoningId { get; set; }

        public Guid FilialeId { get; set; }

        public Guid CompanyId { get; set; }

        public string TradeCode { get; set; }

        public DateTime DayTime { get; set; }

        public DateTime PaymentDayTime { get; set; }

        public decimal AccountReceivable { get; set; }

        public int State { get; set; }

        public int LinkTradeType { get; set; }

        public string LinkTradeCode { get; set; }

        public int IsChecked { get; set; }

    }
}
