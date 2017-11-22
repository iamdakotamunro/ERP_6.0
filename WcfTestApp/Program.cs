using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfTestApp.ErpService;

namespace WcfTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ErpService.ServiceClient("Group.ERP");
            var service2 = new TestForJMeter.TestForJMeterClient("Group.ERP.ForTest");
            //var res = service.AddOrderAndInvoice(new ErpService.AddOrderAndInvoiceRequest
            //{
            //    pushDataId = Guid.NewGuid(),
            //    orderInfo = new GoodsOrderInfo { OrderId = Guid.NewGuid(), Consignee = "Jerry Bai", StorageType = 3, Direction = "仅双休日收" },
            //    orderDetailList = new GoodsOrderDetailInfo[] { new GoodsOrderDetailInfo { GoodsID = Guid.NewGuid(), GoodsType = 3 } },
            //    invoiceInfo = new InvoiceInfo { Address = "sfefef" },
            //});
            //var res2 = service.SetGoodsOrderToRedeploy(new SetGoodsOrderToRedeployRequest
            //{
            //    orderNos = new string[] { "RTTest001", "RTTest002" }
            //});
            //var res3 = service.FinishStorageRecordIn(new FinishStorageRecordInRequest
            //{
            //    inTradeCode = "RK07060112001",
            //    description = " ",
            //    stockQuantitys = new Dictionary<Guid, int> { { Guid.NewGuid(), 72 }, { Guid.NewGuid(), 36 } }
            //});
            //var res4 = service.InsertReckoningInfo(new InsertReckoningInfoRequest {
            //    pushDataId = Guid.NewGuid(),
            //    info = new ReckoningInfo {
            //        ContructType = ContructType.Select,
            //        ReckoningId = Guid.NewGuid()
            //    }
            //});
            //var res5 = service.InsertWastebook(new InsertWastebookRequest
            //{
            //    pushDataId = Guid.NewGuid(),
            //    wasteBookInfo = new WasteBookInfo
            //    {
            //        WasteBookId = Guid.NewGuid(),
            //        LinkTradeType = 3
            //    }
            //});
            var stockId = Guid.NewGuid();
            service2.InsertStorageRecord(new TestForJMeter.InsertStorageRecordRequest
            {
                storageRecord = new TestForJMeter.StorageRecordInfo
                {
                    StockId= stockId,
                    TradeCode="XXXX01",
                    StockType = 1,
                    StorageType = 2,
                },
                storageRecordDetail = new TestForJMeter.StorageRecordDetailInfo[] {
                    new TestForJMeter.StorageRecordDetailInfo {
                        StockId =stockId,
                        GoodsId = Guid.NewGuid()
                    }
                }
            });
        }
    }
}
