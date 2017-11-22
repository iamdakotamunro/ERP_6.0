using ERP.BLL.Implement.Order;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace AllianceUnit
{
    /// <summary>
    ///这是 CheckRefundTest 的测试类，旨在
    ///包含所有 CheckRefundTest 单元测试
    ///</summary>
    [TestClass]
    public class CheckRefundTest
    {
        readonly CheckRefund _target = new CheckRefund(); 

        /// <summary>
        ///GetShopCheckRefundDetailDic 的测试
        ///</summary>
        [TestMethod]
        public void GetShopCheckRefundDetailDicTest()
        {
            
            var refundId = Guid.Empty; 
            var applyId = new Guid("E362D97B-B422-47F4-9CD2-57F6A04BEDBB"); 
            const int CHECK_STATE = 0; 
            var expected = new Dictionary<Guid, int> { { new Guid("06A762B8-01E3-4559-80AE-D973628126D5"), 9 } }; 
            Dictionary<Guid, int> actual = _target.GetShopCheckRefundDetailDic(refundId, applyId, CHECK_STATE);
            Assert.AreEqual(expected.Count, actual.Count);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetShopCheckRefundInfo 的测试
        ///</summary>
        [TestMethod]
        public void GetShopCheckRefundInfoTest()
        {
            var refundId = new Guid("0CE38518-754C-4408-8BC4-55965C4BF2F8"); 
            CheckRefundInfo actual = _target.GetShopCheckRefundInfo(refundId);
            Assert.AreEqual(refundId, actual.RefundId);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
