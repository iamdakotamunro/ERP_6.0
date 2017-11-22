using ERP.BLL.Implement.FinanceModule;
using ERP.BLL.Interface;

namespace GenerateSaleOrderGoodsSettlement
{
    public class GenerateSaleOrderGoodsSettlementTask
    {
        private static readonly ISaleOrderGoodsSettlementManager _saleOrderGoodsSettlementManager = SaleOrderGoodsSettlementManager.WriteInstance;

        public static void Generate()
        {
            if (GenerateSaleOrderGoodsSettlementTaskConfig.CanTrigger())
            {
                _saleOrderGoodsSettlementManager.Generate(GenerateSaleOrderGoodsSettlementTaskConfig.StatisticDate, GenerateSaleOrderGoodsSettlementTaskConfig.MaxRowCount);
            }
        }
    }
}
