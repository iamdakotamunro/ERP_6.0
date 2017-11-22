using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.DAL.Interface.IStorage.Fakes;
using ERP.Model;
using ERP.Model.Goods;
using ERP.Model.Report;
using ERP.SAL.Interface.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 商品结算价存档相关
    /// </summary>
    [TestClass]
    public class UnitTest_GoodsStockSettleRecord
    {
        readonly StubIGoodsStockRecord  _stubIGoodsStockRecord=new StubIGoodsStockRecord ();
        GoodsStockSettleRecordBll _goodsStockSettleRecordBll;
        readonly StubIStorageRecordDao _stubIStorageRecordDao=new StubIStorageRecordDao();
        readonly StubIGoodsCenterSao _stubIGoodsInfoSao=new StubIGoodsCenterSao();
        [TestInitialize]
        public void Init()
        {
            _stubIGoodsStockRecord.IsExistsGoodsStockRecordDateTime = time => true;
            _stubIGoodsStockRecord.SelectMonthGoodsStockRecordInfosInt32Int32Int32=(i, i1, arg3) => new List<MonthGoodsStockRecordInfo>
            {
                new MonthGoodsStockRecordInfo
                {
                    DateCreated=DateTime.Now,
                    DayTime = Convert.ToDateTime("2015-09-01"),
                    FilialeId = new Guid("06B30857-82F5-45F5-8768-79BD4211806C"),
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"),
                    GoodsCode = "LKJSFLK",GoodsId = new Guid("1137DD20-7C7E-4107-BE19-4EE40866F274"),
                    GoodsName = "测试",GoodsType = 2,ID = Guid.NewGuid(),NonceGoodsStock = 10,RealGoodsId = new Guid("C531E492-CA5C-44E1-9862-2CC031B5DCCD"),
                    SettlePrice = 10
                    
                }
            };

            _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_stubIGoodsStockRecord, _stubIStorageRecordDao, _stubIGoodsInfoSao);
        }


        #region  已测试通过

        [TestMethod]
        public void TestAddSettlePriceAndStockRecord()
        {
            _stubIGoodsStockRecord.IsExistsSettlePriceRecordDateTime = time => true;
            _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_stubIGoodsStockRecord, _stubIStorageRecordDao, _stubIGoodsInfoSao);
            var result = _goodsStockSettleRecordBll.AddSettlePriceAndStockRecord(DateTime.Now.Year, DateTime.Now.Month);
            Assert.IsTrue(result);


            _stubIGoodsStockRecord.IsExistsSettlePriceRecordDateTime = time => false;


            _stubIStorageRecordDao.SelectMonthGoodsStockRecordInfosDateTime = (i) => new List<MonthGoodsStockRecordInfo>();
            _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_stubIGoodsStockRecord, _stubIStorageRecordDao, _stubIGoodsInfoSao);
            var result1 = _goodsStockSettleRecordBll.AddSettlePriceAndStockRecord(DateTime.Now.Year, DateTime.Now.Month);
            Assert.IsTrue(result1);



            _stubIStorageRecordDao.SelectMonthGoodsStockRecordInfosDateTime = time => new List<MonthGoodsStockRecordInfo>
            {
                new MonthGoodsStockRecordInfo
                {
                    GoodsId = new Guid("1137DD20-7C7E-4107-BE19-4EE40866F274")
                }
            };

            _stubIStorageRecordDao.SelectGoodsStockInfosListOfInt32DateTimeDateTime= (ints, time, arg3) => new List<MonthGoodsStockInfo>
                {
                    new MonthGoodsStockInfo
                    {
                        GoodsId = new Guid("1137DD20-7C7E-4107-BE19-4EE40866F274"),
                        Quantity = 10,
                        RealGoodsId = new Guid("C531E492-CA5C-44E1-9862-2CC031B5DCCD"),
                        TotalPrice = 1000
                    }
                };

            _stubIGoodsStockRecord.GetGoodsSettlePriceDictsDateTime = (time) => new Dictionary<Guid, decimal>
            {
                {new Guid("057C679C-7ED1-4FB0-98BA-22F4D466755C"), decimal.Parse("9.00")},
                {new Guid("D6445A88-AB5C-404A-A7AD-23151238A51D"), decimal.Parse("20.00")},
                {new Guid("6A4B292C-AAD9-42F2-889D-2333AEBD2F63"), decimal.Parse("6.00")},
                {new Guid("D9D599E3-D61C-4FD9-AE9F-237367B77528"), decimal.Parse("18.00")}
            };

            _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_stubIGoodsStockRecord, _stubIStorageRecordDao, _stubIGoodsInfoSao);
            _stubIGoodsStockRecord.BatchInsertIListOfGoodsStockPriceRecordInfo = list => true;
            var result4 = _goodsStockSettleRecordBll.AddSettlePriceAndStockRecord(DateTime.Now.Year + 1, DateTime.Now.Month);
            Assert.IsTrue(result4);
        }
        #endregion

        /// <summary>
        /// 根据商品类型查询商品销售额
        /// </summary>
        [TestMethod]
        public void TestSelectMonthGoodsStockRecordInfosType()
        {
            _stubIGoodsInfoSao.GetGoodsListByGoodsIdsListOfGuid = list => new List<GoodsInfo>
            {
                new GoodsInfo
                {
                    GoodsId = new Guid("1137DD20-7C7E-4107-BE19-4EE40866F274")
                }
            };
            _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_stubIGoodsStockRecord, _stubIGoodsInfoSao);
            var result = _goodsStockSettleRecordBll.SelectMonthGoodsStockRecordInfos(1, DateTime.Now.Year, DateTime.Now.Month);
            Assert.IsTrue(result.Count>0);
        }

        /// <summary>
        /// 备份月末库存
        /// </summary>
        [TestMethod]
        public void TestCopyMonthGoodsStockInfos()
        {
            _stubIGoodsStockRecord.CopyMonthGoodsStockInfosIListOfMonthGoodsStockRecordInfo = list => true;

            var result = _goodsStockSettleRecordBll.CopyMonthGoodsStockInfos(new DateTime(2015, 9, 1));
            Assert.IsTrue(result);

            _stubIGoodsStockRecord.IsExistsGoodsStockRecordDateTime = date => false;
            var result3 = _goodsStockSettleRecordBll.CopyMonthGoodsStockInfos(new DateTime(2015, 7, 1));
            Assert.IsTrue(result3);


            _stubIStorageRecordDao.SelectMonthGoodsStockRecordInfosDateTime= time => new List<MonthGoodsStockRecordInfo>
            {
                new MonthGoodsStockRecordInfo
                {
                    GoodsId = new Guid("1137DD20-7C7E-4107-BE19-4EE40866F274")
                }
            };

            _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_stubIGoodsStockRecord, _stubIStorageRecordDao, _stubIGoodsInfoSao);

            var result4 = _goodsStockSettleRecordBll.CopyMonthGoodsStockInfos(new DateTime(2015, 11, 1));
            Assert.IsTrue(result4);

            _stubIGoodsInfoSao.GetGoodsListByGoodsIdsListOfGuid= list => new List<GoodsInfo>
            {
                new GoodsInfo
                {
                    GoodsId = new Guid("1137DD20-7C7E-4107-BE19-4EE40866F274")
                }
            };
            _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_stubIGoodsStockRecord, _stubIStorageRecordDao, _stubIGoodsInfoSao);

            var result2 = _goodsStockSettleRecordBll.CopyMonthGoodsStockInfos(new DateTime(2015, 7, 1));
            Assert.IsTrue(result2);
        }
    }
}
