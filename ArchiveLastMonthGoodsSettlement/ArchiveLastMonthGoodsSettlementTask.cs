using ERP.BLL.Implement;
using ERP.BLL.Interface;

namespace ArchiveLastMonthGoodsSettlement
{
    /// <summary>
    /// 商品即时结算价按月归档
    /// </summary>
    /// <remarks>By Jerry Bai 2017/5/16</remarks>
    public class ArchiveLastMonthGoodsSettlementTask
    {
        private static readonly IRealTimeGrossSettlementManager _realTimeGrossSettlementManager = RealTimeGrossSettlementManager.WriteInstance;

        /// <summary>
        /// 生成
        /// </summary>
        public static void Generate()
        {
            if(ArchiveLastMonthGoodsSettlementTaskConfig.CanTrigger())
            {
                _realTimeGrossSettlementManager.ArchiveLastMonth();
            }
        }
    }
}
