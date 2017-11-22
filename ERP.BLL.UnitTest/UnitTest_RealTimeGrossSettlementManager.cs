using ERP.BLL.Implement;
using ERP.BLL.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class UnitTest_RealTimeGrossSettlementManager
    {
        private IRealTimeGrossSettlementManager _realTimeGrossSettlementManager = RealTimeGrossSettlementManager.WriteInstance;

        [TestMethod]
        public void TestGetLatestUnitPrice()
        {
            var result = _realTimeGrossSettlementManager.GetLatestUnitPrice(new Guid("1f1380d5-a81e-469a-9b24-5099838bb825"), new Guid("73886780-548e-406a-a267-2fcb6cb96e6e"));
        }

        [TestMethod]
        public void TestGetLatestUnitPriceListByMultiGoods()
        {
            var result = _realTimeGrossSettlementManager.GetLatestUnitPriceListByMultiGoods(new Guid("1f1380d5-a81e-469a-9b24-5099838bb825"), new Guid[] { new Guid("73886780-548e-406a-a267-2fcb6cb96e6e") });
        }

        [TestMethod]
        public void TestGetLatest()
        {
            var result = _realTimeGrossSettlementManager.GetLatest(new Guid("1f1380d5-a81e-469a-9b24-5099838bb825"), new Guid("73886780-548e-406a-a267-2fcb6cb96e6e"));
        }

        [TestMethod]
        public void TestCalculateByPurchaseStockIn()
        {
            // 获取可测试的采购入库单
            const string SQL = @"
select distinct top 20 t1.StockId,t1.TradeCode from StorageRecord t1
inner join StorageRecordDetail t2 on t2.StockId=t1.StockId
where StockType=1 and StockState=4
	and exists (
		select HostingFilialeId,GoodsId
		from [Group.WMS].dbo.View_RealGoodsWithBatch
		where HostingFilialeId=t1.FilialeId and GoodsId=t2.GoodsId
		group by HostingFilialeId,GoodsId
		having SUM(Quantity+WaitUpQuantity-WaitDownQuantity)>0
)
";

            Guid purchaseStockInId = new Guid("A1E8B225-4E80-4846-9B30-16157B57458B");
            var items = _realTimeGrossSettlementManager.CreateByPurchaseStockIn(purchaseStockInId, DateTime.MinValue).ToList();
            items.ForEach(m => _realTimeGrossSettlementManager.Calculate(m));
        }

        [TestMethod]
        public void TestCalculateByPurchaseReturnStockOut()
        {
            // 获取可测试的采购退货出库单
            const string SQL = @"
select distinct top 20 t1.StockId,t1.TradeCode from StorageRecord t1
inner join StorageRecordDetail t2 on t2.StockId=t1.StockId
where StockType=5 and StockState=4
	and exists (
		select HostingFilialeId,GoodsId
		from [Group.WMS].dbo.View_RealGoodsWithBatch
		where HostingFilialeId=t1.FilialeId and GoodsId=t2.GoodsId
		group by HostingFilialeId,GoodsId
		having SUM(Quantity+WaitUpQuantity-WaitDownQuantity)>0
)
";

            Guid purchaseReturnStockOutId = new Guid("A1E8B225-4E80-4846-9B30-16157B57458B");
            var items = _realTimeGrossSettlementManager.CreateByPurchaseReturnStockOut(purchaseReturnStockOutId, DateTime.MinValue).ToList();
            items.ForEach(m => _realTimeGrossSettlementManager.Calculate(m));
        }

        [TestMethod]
        public void TestCalculateByNewInDocumentAtRed()
        {
            Guid purchaseReturnStockOutId = new Guid("A1E8B225-4E80-4846-9B30-16157B57458B");
            var items = _realTimeGrossSettlementManager.CreateByNewInDocumentAtRed(purchaseReturnStockOutId, DateTime.MinValue).ToList();
            items.ForEach(m => _realTimeGrossSettlementManager.Calculate(m));
        }

        [TestMethod]
        public void TestCalculateByCombineSplit()
        {
            var bill = new SAL.WMS.CombineSplitBillDTO
            {
                BillNo = string.Empty,
                HostingFilialeId = new Guid("1f1380d5-a81e-469a-9b24-5099838bb825"),
                GoodsId = new Guid("7AFC32E7-7677-4C80-9FF8-4925DDDF8D51"),
                Quantity = 2,
                IsSplit = false,
                Details = new List<SAL.WMS.CombineSplitBillDetailDTO>()
            };
            bill.Details.Add(new SAL.WMS.CombineSplitBillDetailDTO
            {
                GoodsId = new Guid("B036C978-89F1-4139-806B-23AA3FB9FC70"),
                Quantity = 2
            });
            bill.Details.Add(new SAL.WMS.CombineSplitBillDetailDTO
            {
                GoodsId = new Guid("2AFCB9B2-FFA8-434F-AE62-8D74AC03C909"),
                Quantity = 4
            });

            //bill.StockQuantitys.Add(new Guid("5B2CD00D-C58B-4CB8-B724-BB7BB767443E"), 2);
            //bill.StockQuantitys.Add(new Guid("2AFCB9B2-FFA8-434F-AE62-8D74AC03C909"), 96);
            //bill.StockQuantitys.Add(new Guid("B036C978-89F1-4139-806B-23AA3FB9FC70"), 98);

            var items = _realTimeGrossSettlementManager.CreateByCombineSplit(bill, DateTime.MinValue).ToList();
            items.ForEach(m => _realTimeGrossSettlementManager.Calculate(m));
        }

        [TestMethod]
        public void TestArchiveLastMonth()
        {
            _realTimeGrossSettlementManager.ArchiveLastMonth();
        }

        [TestMethod]
        public void TestGetArchivedUnitPriceHistoryList()
        {
            var result = _realTimeGrossSettlementManager.GetArchivedUnitPriceHistoryList(new Guid("1F1380D5-A81E-469A-9B24-5099838BB825"), new Guid("E5188AEA-00CD-4DA3-AF0B-610BF7BE55FC"));
        }
    }
}
