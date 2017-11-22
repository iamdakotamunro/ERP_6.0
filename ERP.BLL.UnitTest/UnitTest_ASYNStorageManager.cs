using System;
using System.Collections.Generic;
using ERP.BLL.Implement;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.DAL.Interface.IOrder.Fakes;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL.Interface.Fakes;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.StockCenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class UnitTest_ASYNStorageManager
    {
        private StorageManager _storageManager;
        private readonly StubIStockCenterSao _stockCenterSao=new StubIStockCenterSao();
        private readonly StubIGoodsCenterSao _goodsCenterSao=new StubIGoodsCenterSao();
        private readonly StubIStorageRecordDao _storageDao=new StubIStorageRecordDao();
        private readonly StubIGoodsOrder _goodsOrder=new StubIGoodsOrder();
        private readonly StubIGoodsOrderDetail _goodsOrderDetail=new StubIGoodsOrderDetail();
        private readonly StubILotInquiry _lotInquiry=new StubILotInquiry();
        private readonly StubICode _stubICode = new StubICode();

        [TestMethod]
        public void TestStorageManager()
        {
            _storageManager = new StorageManager(GlobalConfig.DB.FromType.Read);
        }

        [TestMethod]
        public void TestAddBySellStockOut()
        {
            string errorMsg;
            _goodsOrder.GetGoodsOrderGuid = guid => null;
            _storageManager = new StorageManager(_stockCenterSao, _goodsCenterSao, _storageDao, _goodsOrder, _goodsOrderDetail, _lotInquiry, _stubICode);
            var result = _storageManager.AddBySellStockOut(string.Empty, Guid.Empty, true, out errorMsg);
            Assert.IsFalse(result);

            _goodsOrder.GetGoodsOrderGuid = guid => new GoodsOrderInfo();
            _storageManager = new StorageManager(_stockCenterSao, _goodsCenterSao, _storageDao, _goodsOrder, _goodsOrderDetail, _lotInquiry, _stubICode);
            var result1 = _storageManager.AddBySellStockOut(string.Empty, Guid.Empty, true, out errorMsg);
            Assert.IsFalse(result1);

            _lotInquiry.GetAllLotInquiryByTradeCodeString = s => new List<LotInquiry>()
            {
                new LotInquiry
                {
                    RealGoodsId = new Guid("EDCF59C0-2320-43B9-8483-23C8C9E601EE"),BatchNo = "CESIJL"
                }
            };
            _goodsOrder.GetGoodsOrderGuid = guid => new GoodsOrderInfo
            {
                OrderId = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"),
                OrderNo = "BR15081318006",
                SaleFilialeId = new Guid("444E0C93-1146-4386-BAE2-CB352DA70499"),
                DeliverWarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"),
                ConsignTime = DateTime.Now,
                Consignee = "测试",
                IsOut = true
            };
            _goodsOrderDetail.GetGoodsOrderDetailByOrderIdGuid = guid => new List<GoodsOrderDetailInfo>
            {
                new GoodsOrderDetailInfo
                {
                    GoodsID = new Guid("FCCB8336-D3C8-4628-96D1-0681952C7A13"),
                    RealGoodsID = new Guid("FCCB8336-D3C8-4628-96D1-0681952C7A13"),
                    Quantity = 1,
                    PurchaseSpecification = "",
                    SellPrice=0,
                    GoodsName = "洁达隐形眼镜伴侣盒A-022（颜色随机）（百秀专用）",
                    GoodsCode = "JD050610"
                },
                new GoodsOrderDetailInfo
                {
                    GoodsID = new Guid("77D4931C-4579-4A6D-91AF-0CE0B4CFE1D0"),
                    RealGoodsID = new Guid("77D4931C-4579-4A6D-91AF-0CE0B4CFE1D0"),
                    Quantity = 1,
                    PurchaseSpecification = "",
                    SellPrice=0,
                    GoodsName = "蓝睛灵蓝润隐形眼镜润滑液15ML",
                    GoodsCode = "BULE150210"
                },new GoodsOrderDetailInfo
                {
                    GoodsID = new Guid("752D80BA-5E0C-46A9-951B-8A7343162D5D"),
                    RealGoodsID = new Guid("EDCF59C0-2320-43B9-8483-23C8C9E601EE"),
                    Quantity = 2,
                    PurchaseSpecification = "光度:-6.50 ",
                    SellPrice=44,
                    GoodsName = "海昌星眸银石灰半年抛隐形眼镜1片装",
                    GoodsCode = "256589001"
                },new GoodsOrderDetailInfo
                {
                    GoodsID = new Guid("0149B74A-1C94-4F19-9D8E-B3E1E7AFE27B"),
                    RealGoodsID = new Guid("0149B74A-1C94-4F19-9D8E-B3E1E7AFE27B"),
                    Quantity = 1,
                    PurchaseSpecification = "",
                    SellPrice=0,
                    GoodsName = "诺思纯澈隐形眼镜护理液150ml",
                    GoodsCode = "NS041501"
                }
            };
            _stubICode.GetCodeValueCodeTypeDateTime = (type,time) => 2;
            _storageManager = new StorageManager(_stockCenterSao, _goodsCenterSao, _storageDao, _goodsOrder, _goodsOrderDetail, _lotInquiry, _stubICode);
            var result2 = _storageManager.AddBySellStockOut(string.Empty, Guid.Empty, true, out errorMsg);
            Assert.IsFalse(result2);

            _stockCenterSao.GetGoodsQuantityByRealGoodsIdsIEnumerableOfGuidGuidGuid = (guids, guid, arg3) => new List<GoodsStockQuantityInfo>
            {
                new GoodsStockQuantityInfo
                {
                    RealGoodsId = new Guid("EDCF59C0-2320-43B9-8483-23C8C9E601EE"),
                    CurrentQuantity = 10
                }
            };
            _storageManager = new StorageManager(_stockCenterSao, _goodsCenterSao, _storageDao, _goodsOrder, _goodsOrderDetail, _lotInquiry, _stubICode);
            var result3 = _storageManager.AddBySellStockOut(string.Empty, Guid.Empty, true, out errorMsg);
            Assert.IsFalse(result3);

            _stockCenterSao.GetGoodsQuantityByRealGoodsIdsIEnumerableOfGuidGuidGuid = (guids, guid, arg3) => new List<GoodsStockQuantityInfo>
            {
                new GoodsStockQuantityInfo
                {
                    RealGoodsId = new Guid("FCCB8336-D3C8-4628-96D1-0681952C7A13"),
                    CurrentQuantity = 10
                },
                new GoodsStockQuantityInfo
                {
                    RealGoodsId = new Guid("77D4931C-4579-4A6D-91AF-0CE0B4CFE1D0"),
                    CurrentQuantity = 10
                },
                new GoodsStockQuantityInfo
                {
                    RealGoodsId = new Guid("EDCF59C0-2320-43B9-8483-23C8C9E601EE"),
                    CurrentQuantity = 10
                },
                new GoodsStockQuantityInfo
                {
                    RealGoodsId = new Guid("0149B74A-1C94-4F19-9D8E-B3E1E7AFE27B"),
                    CurrentQuantity = 10
                },
            };
            _storageDao.ExistsInt32String = (i, s) => true;
            _storageManager = new StorageManager(_stockCenterSao, _goodsCenterSao, _storageDao, _goodsOrder, _goodsOrderDetail, _lotInquiry, _stubICode);
            var result4 = _storageManager.AddBySellStockOut(string.Empty, Guid.Empty, true, out errorMsg);
            Assert.IsFalse(result4);

            _storageDao.ExistsInt32String = (i, s) => false;
            _goodsCenterSao.GetGoodsListByGoodsIdsListOfGuid = list => new List<GoodsInfo>
            {
                new GoodsInfo
                {
                    GoodsId = new Guid("752D80BA-5E0C-46A9-951B-8A7343162D5D"),ExpandInfo = new GoodsExtendInfo
                    {
                        JoinPrice = 42
                    }
                }
            };
            _storageDao.InsertStorageRecordInfoIListOfStorageRecordDetailInfoStringOut =
                (StorageRecordInfo info, IList<StorageRecordDetailInfo> list, out string @out) =>
                {
                    @out = "";
                    return true;
                };
            _storageManager = new StorageManager(_stockCenterSao, _goodsCenterSao, _storageDao, _goodsOrder, _goodsOrderDetail, _lotInquiry, _stubICode);
            var result5 = _storageManager.AddBySellStockOut(string.Empty, Guid.Empty, true, out errorMsg);
            Assert.IsFalse(result5);
        }
    }
}
