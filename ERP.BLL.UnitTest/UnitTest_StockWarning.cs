using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL.Interface.Fakes;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.StockCenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 报备统计类单元测试
    /// </summary>
    [TestClass]
    public class UnitTest_StockWarning
    {
        readonly StubIStockWarning _stubIStockWarning=new StubIStockWarning();
        private StockWarning _stockWarning;
        readonly StubIStockCenterSao _stubIStockCenterSao=new StubIStockCenterSao();
        readonly StubIGoodsCenterSao _stubIGoodsInfoSao = new StubIGoodsCenterSao();

        [TestInitialize]
        public void Init()
        {
            _stubIStockWarning.GetGoodsDaySalesInfosListOfGuidGuidInt32Int32= (list, guid, arg3, arg4) => new List<GoodsDaySalesInfo>
            {
                new GoodsDaySalesInfo
                {
                    RealGoodsID = new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193"),
                    GoodsSales = 60
                },
                new GoodsDaySalesInfo
                {
                    RealGoodsID = new Guid("22785481-1F31-4F3F-9ED6-995C87FD5A52"),
                    GoodsSales = 60
                }
            };
            //22785481-1F31-4F3F-9ED6-995C87FD5A52
            if (_stockWarning==null)
            {
                _stubIStockCenterSao.GetChildGoodsQuantityGuid = guid => new List<GoodsStockQuantityInfo>();
                _stockWarning = new StockWarning(_stubIStockWarning, _stubIGoodsInfoSao);
            }
        }


        [TestMethod]
        public void TestStockWarning()
        {
            _stubIStockCenterSao.GetChildGoodsQuantityGuid = guid => new List<GoodsStockQuantityInfo>();
            _stockWarning = new StockWarning(_stubIStockWarning, _stubIGoodsInfoSao);
        }

        [TestMethod]
        public void TestGetStockWarningList()
        {
            List<Guid> realGoodsIds1 = null;
            var result = _stockWarning.GetStockWarningList(new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"),Guid.Empty,
                Guid.Empty,
                10, new GoodsInfo { GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2") }, ref realGoodsIds1);
            Assert.IsTrue(result.Count == 0);

            _stubIStockCenterSao.GetChildGoodsQuantityGuid = guid => new List<GoodsStockQuantityInfo>
            {
                new GoodsStockQuantityInfo
                {
                    CurrentQuantity = 10,
                    RealGoodsId = new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193"),
                    GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                },
                new GoodsStockQuantityInfo
                {
                    CurrentQuantity = 10,
                    RealGoodsId = new Guid("9B19D606-235F-409A-A9C8-6F4FAFF5E193"),
                    GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                }
            };

            _stubIStockWarning.GetStockWarningListGuidListOfGuidInt32 = (guid, list, arg3) => new List<StockWarningInfo>
            {
                new StockWarningInfo
                {
                    GoodsId = new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193")
                },
                new StockWarningInfo
                {
                    GoodsId = new Guid("22785481-1F31-4F3F-9ED6-995C87FD5A52")
                }
            };
            List<Guid> realGoodsIds=null ;
            _stubIGoodsInfoSao.GetRealGoodsListByGoodsIdListOfGuid = list => new List<ChildGoodsInfo>
            {
                new ChildGoodsInfo{RealGoodsId = new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193")},
                new ChildGoodsInfo{RealGoodsId = new Guid("9625D606-245D-409A-A9C8-6F4FAFF5E193")}
            };
            _stockWarning = new StockWarning(_stubIStockWarning, _stubIGoodsInfoSao);
            var result1=_stockWarning.GetStockWarningList(new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"),Guid.Empty,
                new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
                10, new GoodsInfo {GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2")}, ref realGoodsIds);
            Assert.IsTrue(result1.Count > 0);
        }

        [TestMethod]
        public void TestGetStockWarningListNew()
        {
            var result2 = _stockWarning.GetStockWarningListNew(new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"), Guid.Empty,Guid.Empty, 10, null);
            Assert.IsTrue(result2.Count == 0);

            _stubIStockCenterSao.GetChildGoodsQuantityGuid = guid => new List<GoodsStockQuantityInfo>
            {
                new GoodsStockQuantityInfo
                {
                    CurrentQuantity = 10,
                    RealGoodsId = new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193"),
                    GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                },
                new GoodsStockQuantityInfo
                {
                    CurrentQuantity = 10,
                    RealGoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
                    GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
                    WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
                },
            };

            _stubIStockWarning.GetStockWarningListGuidListOfGuidInt32 = (guid, list, arg3) => new List<StockWarningInfo>
            {
                new StockWarningInfo
                {
                    GoodsId = new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193")
                },
                new StockWarningInfo
                {
                    GoodsId = new Guid("22785481-1F31-4F3F-9ED6-995C87FD5A52")
                }
            };

            _stubIGoodsInfoSao.GetRealGoodsListByGoodsIdListOfGuid= list => new List<ChildGoodsInfo>
            {
                new ChildGoodsInfo{RealGoodsId = new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193")},
                new ChildGoodsInfo{RealGoodsId = new Guid("9625D606-245D-409A-A9C8-6F4FAFF5E193")}
            };
            _stockWarning = new StockWarning(_stubIStockWarning, _stubIGoodsInfoSao);
            var result4 = _stockWarning.GetStockWarningListNew(new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"), Guid.Empty,new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"), 10,
                new GoodsInfo
                {
                    GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
                    GoodsCode = "112",
                    GoodsName = "自动报备01",
                    IsOnShelf = true
                });
            Assert.IsTrue(result4.Count > 0);

            //_stubIStockWarning.GetStockWarningListGuidListOfGuidInt32 = (guid, list, arg3) => new List<StockWarningInfo>
            //{
            //    new StockWarningInfo
            //    {
            //        GoodsId = new Guid("CA757573-B418-45AA-91DF-0005788D289C")
            //    }
            //};
            
            //var result1 = _stockWarning.GetStockWarningListNew(new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"), new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"), 10,
            //    new GoodsInfo
            //    {
            //        GoodsId = new Guid("6B9015C5-E2F2-4246-BA21-000229C38EF2"),
            //        GoodsCode = "112",
            //        GoodsName = "自动报备01",
            //        IsOnShelf = true
            //    });
            //Assert.IsTrue(result1.Count > 0);
        }

        [TestMethod]
        public void TestGetStockWarningListForReport()
        {
            var result = _stockWarning.GetStockWarningListForReport(new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"),Guid.Empty,new List<Guid>
            {
                new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193"),
                new Guid("22785481-1F31-4F3F-9ED6-995C87FD5A52")
            }, 10);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void TestGetMonthAvgGoodsSales()
        {
            var result = _stockWarning.GetMonthAvgGoodsSales(new List<Guid>
            {
                new Guid("9625D606-235F-409A-A9C8-6F4FAFF5E193"),
                new Guid("22785481-1F31-4F3F-9ED6-995C87FD5A52")
            }, new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"),new Guid());
            Assert.IsTrue(result.Count>0);
        }
    }
}
