using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AllianceUnit
{
    
    
    /// <summary>
    ///这是 StorageManagerTest 的测试类，旨在
    ///包含所有 StorageManagerTest 单元测试
    ///</summary>
    [TestClass()]
    public class StorageManagerTest
    {
        readonly StorageManager _target = new StorageManager();

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
        ///GetStorageGoodsQuantity 的测试
        ///</summary>
        [TestMethod()]
        public void GetStorageGoodsQuantityTest()
        {
            var shopId = new Guid("00000000-0000-0000-8888-000000000000"); 
            var startTime = DateTime.Now.AddMonths(-6); 
            var endTime = DateTime.Now;
            var stockTypes = new List<int> { (int)StorageType.TransferStockOut };
            var states = new List<int> { (int)StorageState.Normal, (int)StorageState.Pass };
            Dictionary<Guid, Dictionary<Guid, int>> actual = _target.GetStorageGoodsQuantity(shopId, startTime, endTime, stockTypes, states,null);
            Assert.IsTrue(actual.Count>0);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetSendQuantityByApplyNo 的测试
        ///</summary>
        [TestMethod()]
        public void GetSendQuantityByApplyNoTest()
        {
            string applyNo = "004PH14072211003";
            int stockType = (int)StorageType.TransferStockOut;
            var states = new List<int> { (int)StorageState.Normal, (int)StorageState.Pass };
            Dictionary<Guid, int> actual = _target.GetSendQuantityByApplyNo(applyNo, stockType, states);
            Assert.IsTrue(actual.Count>0 && actual.Any(act=>act.Value!=0));
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetStorageGoodsQuantity 的测试
        ///</summary>
        [TestMethod()]
        public void GetStorageGoodsQuantityTest1()
        {
            StorageManager target = new StorageManager(); // TODO: 初始化为适当的值
            Guid shopId = new Guid(); // TODO: 初始化为适当的值
            DateTime startTime = new DateTime(); // TODO: 初始化为适当的值
            DateTime endTime = new DateTime(); // TODO: 初始化为适当的值
            List<int> stockTypes = null; // TODO: 初始化为适当的值
            List<int> stockStates = null; // TODO: 初始化为适当的值
            IList<Guid> goodsIds = null; // TODO: 初始化为适当的值
            Dictionary<Guid, Dictionary<Guid, int>> expected = null; // TODO: 初始化为适当的值
            Dictionary<Guid, Dictionary<Guid, int>> actual;
            actual = target.GetStorageGoodsQuantity(shopId, startTime, endTime, stockTypes, stockStates, goodsIds);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
