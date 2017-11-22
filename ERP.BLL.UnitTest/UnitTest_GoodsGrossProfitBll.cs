using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.DAL.Interface.IOrder.Fakes;
using ERP.DAL.Interface.IStorage.Fakes;
using ERP.Model.Goods;
using ERP.Model.Report;
using ERP.SAL.Interface.Fakes;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationLog.Core;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class UnitTest_GoodsGrossProfitBll
    {
        readonly StubIGoodsGrossProfit _stubIGoodsGrossProfit = new StubIGoodsGrossProfit();
        readonly StubIGoodsCenterSao _stubIGoodsInfoSao = new StubIGoodsCenterSao();
        readonly StubIGoodsOrderDetail _stubIGoodsOrderDetail = new StubIGoodsOrderDetail();
        readonly StubIGoodsStockRecord _stubIGoodsStockRecord = new StubIGoodsStockRecord();
        readonly StubIPurchaseSet _stubIPurchaseSet = new StubIPurchaseSet();
        readonly StubIPromotionSao _stubIpromotionSao = new StubIPromotionSao();
        readonly StubIGoodsGrossProfitRecordDetail _stubIGoodsGrossProfitRecordDetail = new StubIGoodsGrossProfitRecordDetail();
        private GoodsGrossProfitBll _goodsGrossProfitBll;

        [TestMethod]
        public void TestInsertData()
        {
            #region 非1号
            _stubIGoodsGrossProfit.ExistsDateTime = time => true;  //存在历史记录
            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsGrossProfit, _stubIGoodsOrderDetail, _stubIGoodsStockRecord, _stubIpromotionSao, _stubIGoodsGrossProfitRecordDetail);
            string errorMsg;
            var result = _goodsGrossProfitBll.InsertData(new DateTime(2015, 12, 2), out errorMsg);
            Assert.IsTrue(result);

            _stubIGoodsGrossProfit.ExistsDateTime = time => false;  //不存在历史记录

            //无明细记录
            _stubIGoodsOrderDetail.GetGoodsOrderDetailsForProfitsDateTimeDateTime = (time, dateTime) => new List<GoodsGrossProfitRecordDetailInfo>();
            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsGrossProfit, _stubIGoodsOrderDetail, _stubIGoodsStockRecord, _stubIpromotionSao, _stubIGoodsGrossProfitRecordDetail);
            try
            {
                var result9 = _goodsGrossProfitBll.InsertData(new DateTime(2015, 12, 2), out errorMsg);
                Assert.IsTrue(!result9);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }

            _stubIGoodsGrossProfit.AddDataIListOfGoodsGrossProfitInfo = list => true;
            _stubIGoodsStockRecord.GetGoodsSettlePriceDictsDateTime = (time) => new Dictionary<Guid, decimal>()
            {
                {new Guid("8F67F6D6-B9C8-498C-BBAA-CDAE6D26FAEB"),8},{new Guid("53270A40-798D-443B-A439-B7F034E3F925"),8}
            };
            _stubIPurchaseSet.GetPurchaseSetListListOfGuidGuid = (list, guid) => new List<PurchaseSetInfo>
            {
                new PurchaseSetInfo
                {
                    GoodsId = new Guid("8F67F6D6-B9C8-498C-BBAA-CDAE6D26FAEB"),PurchasePrice = 9
                },
                new PurchaseSetInfo
                {
                    GoodsId = new Guid("53270A40-798D-443B-A439-B7F034E3F925"),PurchasePrice = 9
                }
            };
            _stubIpromotionSao.GetGoodsPriceDictGuid = guid => new Dictionary<Guid, decimal>
            {
                {new Guid("C874C168-DFFC-44CE-8F38-ABF31A673577"),8}
            };

            _stubIGoodsOrderDetail.GetGoodsOrderDetailsForProfitsDateTimeDateTime = (time, dateTime) => new List<ERP.Model.Report.GoodsGrossProfitRecordDetailInfo>()
            {
                new GoodsGrossProfitRecordDetailInfo
                {
                    GoodsId = new Guid("C874C168-DFFC-44CE-8F38-ABF31A673577"),
                    GoodsType = 1,
                    OrderId = new Guid("5B5C6675-96E5-401B-84C6-F35104BB0186"),
                    Quantity = 2,
                    PromotionValue = 0,
                    SaleFilialeId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                    SalePlatformId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                },
                new GoodsGrossProfitRecordDetailInfo
                {
                    GoodsId = new Guid("8F67F6D6-B9C8-498C-BBAA-CDAE6D26FAEB"),
                    GoodsType = 1,
                    OrderId = new Guid("B1EE681E-7637-47E8-9992-B18070FA257F"),
                    Quantity = 3,
                    PromotionValue = 0,
                    SaleFilialeId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                    SalePlatformId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                },
                new GoodsGrossProfitRecordDetailInfo
                {
                    GoodsId = new Guid("C874C168-DFFC-44CE-8F38-ABF31A673577"),
                    GoodsType = 1,
                    OrderId = new Guid("B1EE681E-7637-47E8-9992-B18070FA257F"),
                    Quantity = 3,
                    PromotionValue = 0,
                    SaleFilialeId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                    SalePlatformId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                },
                new GoodsGrossProfitRecordDetailInfo
                {
                    GoodsId = new Guid("C874C168-DFFC-44CE-8F38-ABF31A673577"),
                    GoodsType = 1,
                    OrderId = new Guid("5B5C6675-96E5-401B-84C6-F35104BB0186"),
                    Quantity = 2,
                    PromotionValue = 10,
                    SaleFilialeId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                    SalePlatformId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                },
                new GoodsGrossProfitRecordDetailInfo
                {
                    GoodsId = new Guid("C874C168-DFFC-44CE-8F38-ABF31A673577"),
                    GoodsType = 1,
                    OrderId = new Guid("B1EE681E-7637-47E8-9992-B18070FA257F"),
                    Quantity = 2,
                    PromotionValue = 10,
                    SaleFilialeId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                    SalePlatformId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                },
                new GoodsGrossProfitRecordDetailInfo
                {
                    GoodsId = new Guid("53270A40-798D-443B-A439-B7F034E3F925"),
                    GoodsType = 1,
                    OrderId = new Guid("B1EE681E-7637-47E8-9992-B18070FA257F"),
                    Quantity = 3,
                    PromotionValue = 10,
                    SaleFilialeId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                    SalePlatformId = new Guid("EE327642-B1C7-4B5A-85D1-94404D7E52BF"),
                },
            };
            //有明细记录
            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsGrossProfit, _stubIGoodsOrderDetail, _stubIGoodsStockRecord, _stubIpromotionSao, _stubIGoodsGrossProfitRecordDetail);
            try
            {
                var result8 = _goodsGrossProfitBll.InsertData(new DateTime(2015, 12, 2), out errorMsg);

            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }

            #endregion

            #region 1号
            _stubIGoodsGrossProfit.CurrentIsExistDateTimeDateTime = (time, dateTime) => true;
            _stubIGoodsGrossProfit.DeleteDataInt32Int32 = (i, i1) => false;
            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsGrossProfit, _stubIGoodsOrderDetail, _stubIGoodsStockRecord, _stubIpromotionSao, _stubIGoodsGrossProfitRecordDetail);
            var result7 = _goodsGrossProfitBll.InsertData(new DateTime(2015, 12, 1), out errorMsg);
            Assert.IsTrue(result7);

            _stubIGoodsGrossProfit.DeleteDataInt32Int32 = (i, i1) => true;
            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsGrossProfit, _stubIGoodsOrderDetail, _stubIGoodsStockRecord, _stubIpromotionSao, _stubIGoodsGrossProfitRecordDetail);
            var result5 = _goodsGrossProfitBll.InsertData(new DateTime(2015, 12, 1), out errorMsg);
            Assert.IsTrue(result5);


            _stubIGoodsGrossProfit.CurrentIsExistDateTimeDateTime = (time, dateTime) => false;
            _stubIGoodsGrossProfit.ExistsDateTime = time => true;
            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsGrossProfit, _stubIGoodsOrderDetail, _stubIGoodsStockRecord, _stubIpromotionSao, _stubIGoodsGrossProfitRecordDetail);
            var result6 = _goodsGrossProfitBll.InsertData(new DateTime(2015, 12, 1), out errorMsg);
            Assert.IsTrue(result6);
            #endregion
        }

        [TestMethod]
        public void TestSelectGoodsGrossProfitInfos()
        {
            //查询历史数据无数据
            _stubIGoodsGrossProfit.SelectGoodsGrossProfitInfosDateTimeNullableOfDateTimeStringGuidStringString = (time, dateTime, arg3, arg4, arg5, arg6) => new List<ERP.Model.Report.GoodsGrossProfitInfo>();

            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsInfoSao, _stubIGoodsGrossProfit, null, null, null, null);
            var result = _goodsGrossProfitBll.GetShowGoodsGrossProfitInfos(DateTime.Now, DateTime.Now, "0", string.Empty, Guid.Empty,
                string.Empty, string.Empty, 0);
            Assert.IsTrue(result.Count == 0);

            //有数据
            _stubIGoodsInfoSao.GetGoodsListByGoodsIdsListOfGuid = list => new List<GoodsInfo>
            {
                new GoodsInfo
                {
                    GoodsId = new Guid("07E08130-CD48-4253-B776-9F200A74373F"),
                    GoodsName = "TESTJLK",
                    GoodsCode = "klsjf",OldGoodsCode = ""
                },
                new GoodsInfo
                {
                    GoodsId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),
                    GoodsName = "6531d3d535",
                    GoodsCode = "dd62552",OldGoodsCode = ""
                }
            };

            _stubIGoodsGrossProfit.SelectGoodsGrossProfitInfosDateTimeNullableOfDateTimeStringGuidStringString = (time, dateTime, arg3, arg4, arg5, arg6) => new List<ERP.Model.Report.GoodsGrossProfitInfo>
            {
                new GoodsGrossProfitInfo
                {
                    GoodsId = new Guid("07E08130-CD48-4253-B776-9F200A74373F")
                },
                new GoodsGrossProfitInfo
                {
                    GoodsId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698")
                }
            };
            _goodsGrossProfitBll = new GoodsGrossProfitBll(_stubIGoodsInfoSao, _stubIGoodsGrossProfit, null, null, null, null);
            var result2 = _goodsGrossProfitBll.GetShowGoodsGrossProfitInfos(DateTime.Now, DateTime.Now, "0", "TEST", Guid.Empty,
                string.Empty, string.Empty, 0);
            Assert.IsTrue(result2.Count == 1);

        }


    }
}
