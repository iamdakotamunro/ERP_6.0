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
    ///这是 ShopRefundMessageBllTest 的测试类，旨在
    ///包含所有 ShopRefundMessageBllTest 单元测试
    ///</summary>
    [TestClass()]
    public class ShopRefundMessageBllTest
    {
        readonly ShopRefundMessageBll _target = new ShopRefundMessageBll();
        private readonly Guid _shopId = new Guid("3C77FC44-D7A9-4DAB-87C8-50031C579214");

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
        ///InsertShopRefundMessage 的测试 
        ///</summary>
        [TestMethod]
        public void InsertShopRefundMessageTest()
        {
            var messageInfo = new ShopRefundMessageInfo(Guid.NewGuid(),
                _shopId, "联盟店O2O", 
                DateTime.Now, "i want...", (byte)ReturnMsgState.CheckPending, "测试");
            var flag = false;
            try
            {
                _target.InsertShopRefundMessage(messageInfo);
                flag = true;
            }
            catch (Exception ex)
            {
                
            }
            Assert.IsTrue(flag);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///GetLastedRefundMsgId 的测试
        ///</summary>
        [TestMethod()]
        public void GetLastedRefundMsgIdTest()
        {
            byte state = (byte)ReturnMsgState.CheckPending; 
            Guid expected = Guid.Empty; 
            Guid actual = _target.GetLastedRefundMsgId(_shopId, state);
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///DeleteShopRefundMessage 的测试
        ///</summary>
        [TestMethod()]
        public void DeleteShopRefundMessageTest()
        {
            Guid msgId = new Guid("45A89FC7-A02A-47A4-A803-12D77E056B2F"); 
            int actual = _target.DeleteShopRefundMessage(msgId);
            Assert.AreEqual(1, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetMessageCount 的测试
        ///</summary>
        [TestMethod()]
        public void GetMessageCountTest()
        {
            byte state = (byte)ReturnMsgState.CheckPending; 
            int actual = _target.GetMessageCount(_shopId, state);
            Assert.AreEqual(0, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetNoAuditMessageCount 的测试
        ///</summary>
        [TestMethod()]
        public void GetNoAuditMessageCountTest()
        {
            int actual = _target.GetNoAuditMessageCount(_shopId);
            Assert.AreEqual(1, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetShopRefundMesageList 的测试
        ///</summary>
        [TestMethod()]
        public void GetShopRefundMesageListTest()
        {
            var startTime = new DateTime(); 
            var endTime = new DateTime(); 
            byte state = (byte)ReturnMsgState.Pass; 
            bool ascOrDesc = false; 
            IEnumerable<ShopRefundMessageInfo> actual = _target.GetShopRefundMesageList(startTime, endTime, state, _shopId, ascOrDesc);
            Assert.AreEqual(2, actual.Count());
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetShopRefundMessageInfo 的测试
        ///</summary>
        [TestMethod()]
        public void GetShopRefundMessageInfoTest()
        {
            Guid id = new Guid("F1496AD5-FD3F-4B69-9655-5E61401DF8F7"); 
            ShopRefundMessageInfo actual = _target.GetShopRefundMessageInfo(id);
            Assert.AreEqual(id, actual.MsgID);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///UpdateShopRefundMessage 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateShopRefundMessageTest()
        {
            var id = new Guid("F1496AD5-FD3F-4B69-9655-5E61401DF8F7");
            ShopRefundMessageInfo result = _target.GetShopRefundMessageInfo(id);
            result.ApplyContent = "I want to return those goods";
            result.CreateTime = DateTime.Now;
            result.ApplyState = 0;
            result.Description = "修改测试";
            int actual = _target.UpdateShopRefundMessage(result);
            Assert.AreEqual(1, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///SetMessageState 的测试
        ///</summary>
        [TestMethod()]
        public void SetMessageStateTest()
        {
            Guid msgId = new Guid("F1496AD5-FD3F-4B69-9655-5E61401DF8F7"); 
            byte state = 0; 
            string description = ""; 
            int actual = _target.SetMessageState(msgId, state, description);
            Assert.AreEqual(1, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
