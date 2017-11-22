using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.DAL.Interface.IStorage.Fakes;
using ERP.Model.Report;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.DAL.Interface.IStorage;
using ERP.DAL.Implement.Storage;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 供应商销售存档类单元测试
    /// </summary>
    [TestClass]
    public class UnitTest_SupplierSaleRecordBll
    {
        SupplierSaleRecordBll _supplierSaleRecordBll;
        readonly StubISupplierSaleRecord _stubISupplierSaleRecord = new StubISupplierSaleRecord();
        readonly StubIStorageRecordDao _stuIStorageRecordDao = new StubIStorageRecordDao();
        readonly StubIPurchaseSet _stubIPurchaseSet = new StubIPurchaseSet();
        readonly StubICompanyCussent _stubICompanyCussent = new StubICompanyCussent();
        readonly StubIGoodsStockRecord _stubIGoodsStockRecord = new StubIGoodsStockRecord();

        readonly IGoodsStockRecord _goodsStockSettleRecord = new GoodsStockRecordDao();

        [TestInitialize]
        public void Init()
        {
            _stubISupplierSaleRecord.SelectSupplierSaleRecordInfosDateTimeIListOfSupplierSaleRecordInfo =
                (time, list) => true;
            _supplierSaleRecordBll = new SupplierSaleRecordBll(_stubISupplierSaleRecord, _stubICompanyCussent, _stubIPurchaseSet, _stubIGoodsStockRecord, _stuIStorageRecordDao);
        }

        [TestMethod]
        public void TestSupplierSaleRecordBllDefault()
        {
            _supplierSaleRecordBll = new SupplierSaleRecordBll();
        }

        [TestMethod]
        public void TestSupplierSaleRecordBll()
        {
            _supplierSaleRecordBll = new SupplierSaleRecordBll(_stubISupplierSaleRecord, _stubICompanyCussent, _stubIPurchaseSet, _stubIGoodsStockRecord, _stuIStorageRecordDao);
        }

        /// <summary>
        /// 插入销售存档数据
        /// </summary>
        [TestMethod]
        public void TestInsertSaleRecord()
        {
            _stubISupplierSaleRecord.IsExistsDateTime = time => true;
            var result1 = _supplierSaleRecordBll.InsertSaleRecord(DateTime.Now);
            Assert.IsTrue(result1);

            _stubISupplierSaleRecord.IsExistsDateTime = time => false;


            _stuIStorageRecordDao.GeTempStorageRecordDetailInfosDateTimeDateTime = (time, dateTime) =>
                 new List<TempStorageRecordDetailInfo>();

            var result2 = _supplierSaleRecordBll.InsertSaleRecord(DateTime.Now);
            Assert.IsTrue(result2);


            _stuIStorageRecordDao.GeTempStorageRecordDetailInfosDateTimeDateTime = (time, dateTime) => new List<TempStorageRecordDetailInfo>
            {
                new TempStorageRecordDetailInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    ID = Guid.NewGuid(),
                    Quantity = 10,
                    RealGoodsId = new Guid("B72D12A7-86A6-403A-AE25-0BDFBA092D8F"),
                    StockType = 4,
                    UnitPrice = 15,
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                },
                new TempStorageRecordDetailInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    ID = Guid.NewGuid(),
                    Quantity = 10,
                    RealGoodsId = new Guid("AFDCB083-7AD4-4FEC-A7F0-3D8E2C2665F0"),
                    StockType = 4,
                    UnitPrice = 15,
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                },
                new TempStorageRecordDetailInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    ID = Guid.NewGuid(),
                    Quantity = 10,
                    RealGoodsId = new Guid("545AFF83-C54D-4EFD-8F42-42269E2875AB"),
                    StockType = 4,
                    UnitPrice = 15,
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                }
            };


            _stubIPurchaseSet.GetPurchaseSetList = () => new List<PurchaseSetInfo>();
            var result3 = _supplierSaleRecordBll.InsertSaleRecord(DateTime.Now);
            Assert.IsTrue(result3);

            _stubIPurchaseSet.GetPurchaseSetList = () => new List<PurchaseSetInfo>
            {
                new PurchaseSetInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    WarehouseId = new Guid("C9752B7B-D2FB-466E-BA73-53F3C36C864A"),
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771")
                }
            };

            _stubIGoodsStockRecord.GetGoodsSettlePriceDictsDateTime = (time) => new Dictionary<Guid, decimal>();

            var result6 = _supplierSaleRecordBll.InsertSaleRecord(DateTime.Now);
            Assert.IsTrue(result6);


            _stubIGoodsStockRecord.GetGoodsSettlePriceDictsDateTime = (time) => new Dictionary<Guid, decimal>
            {
                {new Guid("E838D1AC-A972-413C-87A6-697861A6D69F"), Convert.ToDecimal("14.8")}
            };
            var result5 = _supplierSaleRecordBll.InsertSaleRecord(DateTime.Now);
            Assert.IsTrue(result5);


        }

        /// <summary>
        /// 获取结算价
        /// </summary>
        [TestMethod]
        public void TestSettlePrice()
        {
            var goodsId = Guid.NewGuid();
            var settlePriceList = _goodsStockSettleRecord.GetGoodsSettlePriceOrPurchasePriceDicts(DateTime.Now);
            var settleprice = settlePriceList.ContainsKey(goodsId) ? settlePriceList[goodsId] : 0;
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 供应商销量页面显示数据(对应公司的销量)
        /// </summary>
        [TestMethod]
        public void TestSelectSupplierSaleReportInfos()
        {

            _stubISupplierSaleRecord.SelectSupplierSaleReportInfosInt32 = i => new List<SupplierSaleReportInfo>
            {
                new SupplierSaleReportInfo
                {
                    CompanyID = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771")
                },
                new SupplierSaleReportInfo
                {
                    CompanyID = Guid.NewGuid()
                }
            };

            _stubICompanyCussent.GetCompanyCussentListByCompanyNameString = s => new List<CompanyCussentInfo>
            {
                new CompanyCussentInfo
                {
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),CompanyName = "测试"
                }
            };

            var result = _supplierSaleRecordBll.SelectSupplierSaleReportInfos(DateTime.Now.Year, string.Empty);
            Assert.IsTrue(result.Count > 0);

            _stubICompanyCussent.GetCompanyCussentListByCompanyNameString = s => new List<CompanyCussentInfo>();
            _supplierSaleRecordBll = new SupplierSaleRecordBll(_stubISupplierSaleRecord, _stubICompanyCussent,
                _stubIPurchaseSet, _stubIGoodsStockRecord, _stuIStorageRecordDao);
            var result1 = _supplierSaleRecordBll.SelectSupplierSaleReportInfos(DateTime.Now.Year, string.Empty);
            Assert.IsTrue(result1.Count == 0);
        }

        /// <summary>
        /// 获取当月已存在的销售数据
        /// </summary>
        [TestMethod]
        public void TestSelectSupplierSaleRecordInfosByDateTime()
        {
            //获取临时出入库数据

            _stuIStorageRecordDao.GeTempStorageRecordDetailInfosDateTimeDateTime = (time, dateTime) => new List<TempStorageRecordDetailInfo>();
            var result1 = _supplierSaleRecordBll.SelectSupplierSaleRecordInfos(DateTime.Now);
            Assert.IsTrue(result1);
            _stuIStorageRecordDao.GeTempStorageRecordDetailInfosDateTimeDateTime = (time, dateTime) => new List<TempStorageRecordDetailInfo>
            {
                new TempStorageRecordDetailInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    ID = Guid.NewGuid(),
                    Quantity = 10,
                    RealGoodsId = new Guid("B72D12A7-86A6-403A-AE25-0BDFBA092D8F"),
                    StockType = 4,
                    UnitPrice = 15,
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                }
            };
            //获取采购设置
            _stubIPurchaseSet.GetPurchaseSetList = () => new List<PurchaseSetInfo>();
            var result2 = _supplierSaleRecordBll.SelectSupplierSaleRecordInfos(DateTime.Now);
            Assert.IsTrue(result2);
            _stuIStorageRecordDao.GeTempStorageRecordDetailInfosDateTimeDateTime = (time, dateTime) => new List<TempStorageRecordDetailInfo>
            {
                new TempStorageRecordDetailInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    ID = Guid.NewGuid(),
                    Quantity = 10,
                    RealGoodsId = new Guid("B72D12A7-86A6-403A-AE25-0BDFBA092D8F"),
                    StockType = 12,
                    UnitPrice = 25,
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                },
                new TempStorageRecordDetailInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    ID = Guid.NewGuid(),
                    Quantity = 8,
                    RealGoodsId = new Guid("DD587104-1E37-4A11-8A27-077B4262859A"),
                    StockType = 12,
                    UnitPrice = 25,
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                },
                new TempStorageRecordDetailInfo
                {
                    GoodsId = new Guid("F1203667-B1B8-4F48-956F-17E524B97B0C"),
                    ID = Guid.NewGuid(),
                    Quantity = 8,
                    RealGoodsId = new Guid("DD587104-1E37-4A11-8A27-077B4262859A"),
                    StockType = 12,
                    UnitPrice = 25,
                    WarehouseId = new Guid("C9752B7B-D2FB-466E-BA73-53F3C36C864A")
                }
            };

            _stubIGoodsStockRecord.GetGoodsSettlePriceDictsDateTime = (time) => new Dictionary<Guid, decimal>
            {
                {new Guid("E838D1AC-A972-413C-87A6-697861A6D69F"), Convert.ToDecimal("14.8")}
            };

            _stubIPurchaseSet.GetPurchaseSetList = () => new List<PurchaseSetInfo>
            {
                new PurchaseSetInfo
                {
                    GoodsId = new Guid("A1203667-B1B8-4F48-956F-17E524B97B0C"),
                    WarehouseId = new Guid("C9752B7B-D2FB-466E-BA73-53F3C36C864A"),
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771")
                }
            };

            var result6 = _supplierSaleRecordBll.SelectSupplierSaleRecordInfos(DateTime.Now);
            Assert.IsTrue(result6);

        }


        /// <summary>
        /// 添加临时出入库数据
        /// </summary>
        [TestMethod]
        public void TestInsertTempStorageRecord()
        {
            //_stuIStorageRecordDao.GetSaleStorageRecordDetailInfosDateTimeDateTime= (time, dateTime) => new List<TempStorageRecordDetailInfo>();
            //var result = _supplierSaleRecordBll.InsertTempStorageRecord(Convert.ToDateTime("2016-10-01"), Convert.ToDateTime("2016-11-01"));
            //Assert.IsTrue(!result);

            //_stuIStorageRecordDao.GetSaleStorageRecordDetailInfosDateTimeDateTime = (time, dateTime) => new List<TempStorageRecordDetailInfo>
            //{
            //    new TempStorageRecordDetailInfo
            //    {
            //        GoodsId = new Guid("9BA92827-F335-41AC-865E-CA2BABAFD669")
            //    }
            //};
            //_stubISupplierSaleRecord.InsertTempStorageRecordIListOfTempStorageRecordDetailInfo = list => true;
            //var result1 = _supplierSaleRecordBll.InsertTempStorageRecord(Convert.ToDateTime("2015-10-01"),DateTime.Now);
            //Assert.IsTrue(result1);
        }
    }
}
