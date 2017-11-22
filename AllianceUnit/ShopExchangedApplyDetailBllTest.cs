using System.Linq;
using ERP.BLL.Implement.Shop;
using ERP.Enum.ShopFront;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ERP.Model.ShopFront;
using System.Collections.Generic;

namespace AllianceUnit
{
    /// <summary>
    ///这是 ShopExchangedApplyDetailBllTest 的测试类，旨在
    ///包含所有 ShopExchangedApplyDetailBllTest 单元测试
    ///</summary>
    [TestClass()]
    public class ShopExchangedApplyDetailBllTest
    {
        readonly ShopExchangedApplyDetailBll _target = new ShopExchangedApplyDetailBll();
        private readonly Guid _shopId = new Guid("FE410FFD-8BF0-4DEC-AC65-26F0FD8F0D58");
        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext { get; set; }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///InsertShopExchangedApplyDetail 的测试
        ///</summary>
        [TestMethod]
        public void InsertShopExchangedApplyDetailTest()
        {
            var flag = false;
            var applyDetailInfo = new ShopExchangedApplyDetailInfo(Guid.NewGuid()
                ,new Guid("0EED8C63-64EB-4AEB-A63F-1C1BD74ADF78"),
                new Guid("DB195F6E-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("DB195F6E-9ED5-4AEE-9B19-6EE688255C05"), "ceshi", "goodscode",
                "光度：1.0", 20, 5, "盒", new Guid("52EE8255-9525-4AEE-9B19-6EE688255C05"),
                new Guid("52EE8255-9525-4AEE-9B19-6EE688255C05"), "ceshi2", "goodscode2", "光度：2.0");
            try
            {
                _target.InsertShopExchangedApplyDetail(applyDetailInfo);
                flag = true;
            }
            catch (Exception ex)
            {
                
            }
            Assert.IsTrue(flag);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///DeleteApplyDetailInfo 的测试
        ///</summary>
        [TestMethod()]
        public void DeleteApplyDetailInfoTest()
        {
            var id = new Guid("6AF02838-4C58-4E09-A422-06CDA1FE62D2");
            int actual = _target.DeleteApplyDetailInfo(id);
            Assert.AreEqual(1, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///DeleteApplyDetails 的测试
        ///</summary>
        [TestMethod()]
        public void DeleteApplyDetailsTest()
        {
            Guid applyId = new Guid("0EED8C63-64EB-4AEB-A63F-1C1BD74ADF78"); 
            int actual = _target.DeleteApplyDetails(applyId);
            Assert.AreEqual(1, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetShopExchangedApplyDetailList 的测试
        ///</summary>
        [TestMethod()]
        public void GetShopExchangedApplyDetailListTest()
        {
            Guid applyId = new Guid("0EED8C63-64EB-4AEB-A63F-1C1BD74ADF78");
            IEnumerable<ShopExchangedApplyDetailInfo> actual = _target.GetShopExchangedApplyDetailList(applyId);
            Assert.IsTrue(actual!=null && actual.Any());
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetShopRefundApplyDetailList 的测试
        ///</summary>
        [TestMethod()]
        public void GetShopRefundApplyDetailListTest()
        {
            Guid applyId = new Guid("828FE2DF-C6D9-42EB-9C9C-C19510ADDDBF");
            IEnumerable<ShopApplyDetailInfo> actual = _target.GetShopRefundApplyDetailList(applyId);
            Assert.IsTrue(actual!=null && actual.Any());
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///InsertShopdApplyDetail 的测试
        ///</summary>
        [TestMethod()]
        public void InsertShopdApplyDetailTest()
        {
            var flag = false;
            try
            {
                var applyDetailInfo = new ShopApplyDetailInfo(Guid.NewGuid()
                , new Guid("828FE2DF-C6D9-42EB-9C9C-C19510ADDDBF"),
                new Guid("DB195F6E-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("DB195F6E-9ED5-4AEE-9B19-6EE688255C05"), "ceshi", "goodscode",
                "", 20, 5, "盒");
                _target.InsertShopdApplyDetail(applyDetailInfo);
                flag = true;
            }
            catch (Exception ex)
            {

            }
            Assert.IsTrue(flag);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///IsAllowPurchase 的测试
        ///</summary>
        [TestMethod()]
        public void IsAllowPurchaseTest()
        {
            IList<Guid> goodsIds = new List<Guid>() { new Guid("9B195F6E-9525-4AEE-9B19-6EE688255C05") }; 
            var startTime = DateTime.Now.AddMonths(-6); 
            var endTime = DateTime.Now; 
            Dictionary<Guid, bool> actual = _target.IsAllowPurchase(_shopId, true, goodsIds, startTime, endTime);
            Assert.IsTrue(actual!=null && actual.Any());
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///IsExistExchangedData 的测试
        ///</summary>
        [TestMethod()]
        public void IsExistExchangedDataTest()
        {
            Guid goodsId = new Guid("9B195F6E-9525-4AEE-9B19-6EE688255C05");
            IList<int> states = new List<int>() { (byte)ExchangedState.Checking, (byte)ExchangedState.CheckPending, (byte)ExchangedState.Checked, }; 
            bool actual = _target.IsExistExchangedData(_shopId, goodsId, states);
            Assert.AreEqual(true, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetExchangedApplyGoodsQuantity 的测试
        ///</summary>
        [TestMethod()]
        public void GetExchangedApplyGoodsQuantityTest()
        {
            var startTime = DateTime.Now.AddMonths(-12); 
            var endTime =DateTime.Now; 
            //IList<byte> states = null; 
            //IList<Guid> goodsIds = null; 
            Dictionary<Guid, Dictionary<Guid, int>> actual = _target.GetExchangedApplyGoodsQuantity(new Guid("fe410ffd-8bf0-4dec-ac65-26f0fd8f0d58"), -1, new DateTime(2014, 9, 1),
                DateTime.Now, new List<int> { (int)ExchangedState.CheckPending }, new List<Guid> { new Guid("4ddec2c2-16d7-42e0-aac4-8bc95002468b") });
            Assert.AreEqual(4, actual.Count);
            //Assert.Inconclusive("验证此测试方法的正确性。");

        }

        /// <summary>
        ///GetLastUnitPrice 的测试
        ///</summary>
        [TestMethod()]
        public void GetLastUnitPriceTest()
        {
            Decimal actual = _target.GetLastUnitPrice(new Guid("8BF7B6E2-5F4C-4252-ADF0-9ACF07529B39"), new Guid("4DDEC2C2-16D7-42E0-AAC4-8BC95002468B"));
            Assert.IsTrue(actual==0);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
