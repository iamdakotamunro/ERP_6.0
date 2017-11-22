using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// zal 2015-10-27
    /// </summary>
    [TestClass]
    public class UnitTestLotInquiryManager
    {
        private static readonly StubILotInquiry _stubILotInquiry = new StubILotInquiry();
        private static readonly LotInquiryManager _lotInquiryManager = new LotInquiryManager(_stubILotInquiry);

        [TestMethod]
        public void GetAllLotInquiryTest()
        {
            _stubILotInquiry.GetAllLotInquiry = () => new List<LotInquiry>
            {
                new LotInquiry
                {
                    Id =Guid.NewGuid(),
                    BatchNo = "test1",
                    DateTime = DateTime.Now,
                    GoodsCode ="007",
                    TradeCode = "110"
                }
            };

            var result1 = _lotInquiryManager.GetAllLotInquiry();
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.Any());

            _stubILotInquiry.GetAllLotInquiry = () => new List<LotInquiry>();

            var result2 = _lotInquiryManager.GetAllLotInquiry();
            Assert.IsNotNull(result2);
            Assert.IsFalse(result2.Any());
        }

        [TestMethod]
        public void GetAllLotInquiryByTradeCodeTest()
        {
            _stubILotInquiry.GetAllLotInquiryByTradeCodeString = para => new List<LotInquiry>
            {
                new LotInquiry
                {
                    Id =Guid.NewGuid(),
                    BatchNo = "test1",
                    DateTime = DateTime.Now,
                    GoodsCode ="007",
                    TradeCode = "110"
                }
            };

            var result1 = _lotInquiryManager.GetAllLotInquiryByTradeCode("110");
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.Any());

            _stubILotInquiry.GetAllLotInquiryByTradeCodeString = para => new List<LotInquiry>();

            var result2 = _lotInquiryManager.GetAllLotInquiryByTradeCode(string.Empty);
            Assert.IsNotNull(result2);
            Assert.IsFalse(result2.Any());
        }

        [TestMethod]
        public void GetAllLotInquiryByPageTest()
        {
            _stubILotInquiry.GetAllLotInquiryByPageNullableOfDateTimeNullableOfDateTimeInt32GuidStringInt32Int32Int32Out
                = (DateTime? starTime, DateTime? endTime, int type,
                    Guid goodsId, string batchNo, int pageIndex, int pageSize, out int total1) =>
                {
                    total1 = 1;
                    return new List<LotInquiry>
                    {
                        new LotInquiry
                        {
                            Id =Guid.NewGuid(),
                            BatchNo = "test1",
                            DateTime = DateTime.Now,
                            GoodsCode ="007",
                            TradeCode = "110"
                        }
                    };
                };

            int total;
            var result1 = _lotInquiryManager.GetAllLotInquiryByPage(DateTime.Now.AddMonths(-1), DateTime.Now, 1, new Guid("3886BF6D-3DD1-4DDE-9647-94E5B6574E7C"), "c", 1, 10, out total);
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.Any());

            _stubILotInquiry.GetAllLotInquiryByPageNullableOfDateTimeNullableOfDateTimeInt32GuidStringInt32Int32Int32Out
                = (DateTime? starTime, DateTime? endTime, int type,
                    Guid goodsId, string batchNo, int pageIndex, int pageSize, out int total1) =>
                {
                    total1 = 1;
                    return new List<LotInquiry>();
                };

            var result2 = _lotInquiryManager.GetAllLotInquiryByPage(DateTime.Now.AddMonths(-1), DateTime.Now, 1, new Guid("3886BF6D-3DD1-4DDE-9647-94E5B6574E7C"), string.Empty, 1, 10, out total);
            Assert.IsNotNull(result2);
            Assert.IsFalse(result2.Any());

            _stubILotInquiry.GetAllLotInquiryByPageNullableOfDateTimeNullableOfDateTimeInt32GuidStringInt32Int32Int32Out
                = (DateTime? starTime, DateTime? endTime, int type,
                    Guid goodsId, string batchNo, int pageIndex, int pageSize, out int total1) =>
                {
                    total1 = 1;
                    return null;
                };

            var result3 = _lotInquiryManager.GetAllLotInquiryByPage(DateTime.Now.AddMonths(-1), DateTime.Now, 1, new Guid("3886BF6D-3DD1-4DDE-9647-94E5B6574E7C"), string.Empty, 1, 10, out total);
            Assert.IsNull(result3);
        }

        [TestMethod]
        public void GetLotInquiryByIdTest()
        {
            _stubILotInquiry.GetLotInquiryByIdGuid = id => new LotInquiry
            {
                Id = Guid.NewGuid(),
                BatchNo = "test1",
                DateTime = DateTime.Now,
                GoodsCode = "007",
                TradeCode = "110"
            };

            var result1 = _lotInquiryManager.GetLotInquiryById(Guid.NewGuid());
            Assert.IsNotNull(result1);

            _stubILotInquiry.GetLotInquiryByIdGuid = id => new LotInquiry();

            var result2 = _lotInquiryManager.GetLotInquiryById(Guid.Empty);
            Assert.IsNotNull(result2);

            _stubILotInquiry.GetLotInquiryByIdGuid = id => null;

            var result3 = _lotInquiryManager.GetLotInquiryById(Guid.Empty);
            Assert.IsNull(result3);
        }

        [TestMethod]
        public void GetLotInquiryByTradeCodeAndRealGoodsIdTest()
        {
            _stubILotInquiry.GetLotInquiryByTradeCodeAndRealGoodsIdStringGuid = (para1, para2) => new LotInquiry
            {
                Id = Guid.NewGuid(),
                BatchNo = "test1",
                DateTime = DateTime.Now,
                GoodsCode = "007",
                TradeCode = "110"
            };

            var result1 = _lotInquiryManager.GetLotInquiryByTradeCodeAndRealGoodsId(string.Empty, Guid.Empty);
            Assert.IsNotNull(result1);

            _stubILotInquiry.GetLotInquiryByTradeCodeAndRealGoodsIdStringGuid = (para1, para2) => new LotInquiry();

            var result2 = _lotInquiryManager.GetLotInquiryByTradeCodeAndRealGoodsId(string.Empty, Guid.Empty);
            Assert.IsNotNull(result2);

            _stubILotInquiry.GetLotInquiryByTradeCodeAndRealGoodsIdStringGuid = (para1, para2) => null;

            var result3 = _lotInquiryManager.GetLotInquiryByTradeCodeAndRealGoodsId(string.Empty, Guid.Empty);
            Assert.IsNull(result3);
        }

        [TestMethod]
        public void DeleteLotInquiryByIdTest()
        {
            _stubILotInquiry.DeleteLotInquiryByIdGuid = para => true;

            var result1 = _lotInquiryManager.DeleteLotInquiryById(Guid.Empty);
            Assert.IsTrue(result1);

            _stubILotInquiry.DeleteLotInquiryByIdGuid = para => false;

            var result2 = _lotInquiryManager.DeleteLotInquiryById(Guid.Empty);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void DeleteLotInquiryByTradeCodeTest()
        {
            _stubILotInquiry.DeleteLotInquiryByTradeCodeString = para => true;

            var result1 = _lotInquiryManager.DeleteLotInquiryByTradeCode(string.Empty);
            Assert.IsTrue(result1);

            _stubILotInquiry.DeleteLotInquiryByTradeCodeString = para => false;

            var result2 = _lotInquiryManager.DeleteLotInquiryByTradeCode(string.Empty);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void UpdateLotInquiryByIdTest()
        {
            _stubILotInquiry.UpdateLotInquiryByIdLotInquiry = para => true;

            var result1 = _lotInquiryManager.UpdateLotInquiryById(new LotInquiry());
            Assert.IsTrue(result1);

            _stubILotInquiry.UpdateLotInquiryByIdLotInquiry = para => false;

            var result2 = _lotInquiryManager.UpdateLotInquiryById(new LotInquiry());
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void AddLotInquiryTest()
        {
            _stubILotInquiry.AddLotInquiryLotInquiry = para => true;

            var result1 = _lotInquiryManager.AddLotInquiry(new LotInquiry());
            Assert.IsTrue(result1);

            _stubILotInquiry.AddLotInquiryLotInquiry = para => false;

            var result2 = _lotInquiryManager.AddLotInquiry(new LotInquiry());
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void AddOrUpdateLotInquiryTest()
        {
            _stubILotInquiry.AddOrUpdateLotInquiryLotInquiry = para => true;

            var result1 = _lotInquiryManager.AddOrUpdateLotInquiry(new LotInquiry());
            Assert.IsTrue(result1);

            _stubILotInquiry.AddOrUpdateLotInquiryLotInquiry = para => false;

            var result2 = _lotInquiryManager.AddOrUpdateLotInquiry(new LotInquiry());
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void BulkAddOrUpdateLotInquiryTest()
        {
            _stubILotInquiry.BulkAddOrUpdateLotInquiryListOfLotInquiryStringOut = (List<LotInquiry> list, out string errorMsg) =>
            {
                errorMsg = string.Empty;
                return true;
            };

            string errorMessage;
            var result1 = _lotInquiryManager.BulkAddOrUpdateLotInquiry(new List<LotInquiry>(), out errorMessage);
            Assert.IsTrue(result1);

            _stubILotInquiry.BulkAddOrUpdateLotInquiryListOfLotInquiryStringOut = (List<LotInquiry> list, out string errorMsg) =>
            {
                errorMsg = string.Empty;
                return false;
            };

            var result2 = _lotInquiryManager.BulkAddOrUpdateLotInquiry(new List<LotInquiry>(), out errorMessage);
            Assert.IsFalse(result2);
        }
    }
}
