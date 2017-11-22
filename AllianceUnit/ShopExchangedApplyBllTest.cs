using System.Linq;
using ERP.BLL.Implement.Goods;
using ERP.BLL.Implement.Shop;
using ERP.Enum.ShopFront;
using Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ERP.Model.ShopFront;
using System.Collections.Generic;

namespace AllianceUnit
{
    
    
    /// <summary>
    ///这是 ShopExchangedApplyBllTest 的测试类，旨在
    ///包含所有 ShopExchangedApplyBllTest 单元测试
    ///</summary>
    [TestClass()]
    public class ShopExchangedApplyBllTest
    {
        private readonly ShopExchangedApplyBll _target = new ShopExchangedApplyBll();
        private readonly Guid _shopId = new Guid("FE410FFD-8BF0-4DEC-AC65-26F0FD8F0D58");
        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///AddShopExchangedApply 的测试  添加换货申请
        ///</summary>
        [TestMethod]
        public void AddShopExchangedApplyTest()
        {
            var applyInfo = new ShopExchangedApplyInfo(Guid.NewGuid(),"TH0819001",_shopId,"万店联盟1",DateTime.Now,200,10,(byte)ExchangedState.CheckPending,false,"换货"); 
            IList<ShopExchangedApplyDetailInfo> applyDetailInfos = new List<ShopExchangedApplyDetailInfo>();
            applyDetailInfos.Add( new ShopExchangedApplyDetailInfo(Guid.NewGuid()
                , applyInfo.ApplyID,
                new Guid("DB195F6E-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("DB195F6E-9ED5-4AEE-9B19-6EE688255C05"), "ceshi", "goodscode",
                "", 20, 5, "盒", new Guid("52EE8255-9525-4AEE-9B19-6EE688255C05"),
                new Guid("52EE8255-9525-4AEE-9B19-6EE688255C05"), "ceshi2", "goodscode2", ""));
            applyDetailInfos.Add(new ShopExchangedApplyDetailInfo(Guid.NewGuid()
                , applyInfo.ApplyID,
                new Guid("4AEE5F6E-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("4AEE5F6E-9ED5-4AEE-9B19-6EE688255C05"), "ceshi4", "goodscode4",
                "", 20, 5, "盒", new Guid("59B19255-9525-4AEE-9B19-6EE688255C05"),
                new Guid("59B19255-9525-4AEE-9B19-6EE688255C05"), "ceshi5", "goodscode5", ""));
            string msg;
            bool actual = _target.AddShopExchangedApply(applyInfo, applyDetailInfos, out msg);
            Assert.IsTrue(string.IsNullOrEmpty(msg));
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///UpdateShopExchangedApply 的测试   换货申请更新
        ///</summary>
        [TestMethod()]
        public void UpdateShopExchangedApplyTest()
        {
            ShopExchangedApplyInfo applyInfo = null; 
            IList<ShopExchangedApplyDetailInfo> applyDetailInfos = null; 
            string msg = string.Empty; 
            string msgExpected = string.Empty; 
            bool expected = false; // 
            bool actual;
            actual = _target.UpdateShopExchangedApply(applyInfo, applyDetailInfos, out msg);
            Assert.AreEqual(msgExpected, msg);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }


        /// <summary>
        ///GetShopExchangedApplyList 的测试  换货查询
        ///</summary>
        [TestMethod()]
        public void GetShopExchangedApplyListTest()
        {
            //new Guid("FE410FFD-8BF0-4DEC-AC65-26F0FD8F0D58")
            //var goods = new GoodsManager().GetGoodsSelectList("迷卡倾城灰彩色隐形眼镜");
            //var goodsIds = goods.Select(act => new Guid(act.Key)).ToList();
            var startTime = DateTime.Now.AddMonths(-6); 
            var endTime = DateTime.Now;
            var totalList = _target.GetShopExchangedApplyList(false, string.Empty, startTime, endTime, new List<Guid>(), _shopId,
                                                                 -1).ToList();
            var tempList = totalList.Skip((1-1)*5).Take(5).ToList();
            var pageItems= new PageItems<ShopExchangedApplyInfo>(1, 5, totalList.Count,
                tempList);
            //IEnumerable<ShopExchangedApplyInfo> actual = _target.GetShopExchangedApplyList(false,
            //    string.Empty, startTime, endTime, goodsIds,Guid.Empty , -1);
            Assert.IsTrue(pageItems != null && pageItems.Items.Any());
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }


        /// <summary>
        ///UpdateShopRefundApply 的测试  退货申请更新
        ///</summary>
        [TestMethod()]
        public void UpdateShopRefundApplyTest()
        {
            var applyInfo = new ShopExchangedApplyInfo(new Guid("E777743C-DE00-4D27-91F9-BABC711F359E"), "TH-SH0820001", _shopId, "万店联盟1", DateTime.Now, 10, 100, (byte)ExchangedState.CheckPending, true, "退货");
            IList<ShopApplyDetailInfo> applyDetailInfos = new List<ShopApplyDetailInfo>();
            applyDetailInfos.Add(new ShopApplyDetailInfo(Guid.NewGuid()
                , applyInfo.ApplyID,
                new Guid("BABC711F-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("BABC711F-9ED5-4AEE-9B19-6EE688255C05"), "ceshi6", "goodscode6",
                "", 10, 5, "盒"));
            applyDetailInfos.Add(new ShopApplyDetailInfo(Guid.NewGuid()
                , applyInfo.ApplyID,
                new Guid("BABC711F-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("BABC711F-9ED5-4AEE-9B19-6EE688255C05"), "ceshi7", "goodscode7",
                "", 10, 5, "盒"));
            string msg; 
            bool actual = _target.UpdateShopRefundApply(applyInfo, applyDetailInfos, out msg);
            //Assert.AreEqual(msgExpected, msg);
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///SetShopExchangedState 的测试
        ///</summary>
        [TestMethod()]
        public void SetShopExchangedStateTest()
        {
            var applyId = new Guid("3A64377A-15BB-47F2-99EB-AC7810D7F410"); 
            byte exchangedState = 1; 
            string description = "add"; 
            string msg; 
            int actual = _target.SetShopExchangedState(applyId, exchangedState, description, out msg);
            //Assert.AreEqual(msgExpected, msg);
            Assert.IsTrue(actual>0);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetShopExchangedApplyInfo 的测试
        ///</summary>
        [TestMethod()]
        public void GetShopExchangedApplyInfoTest()
        {
            var applyId = new Guid("3A64377A-15BB-47F2-99EB-AC7810D7F410"); 
            ShopExchangedApplyInfo actual = _target.GetShopExchangedApplyInfo(applyId);
            Assert.AreEqual(applyId, actual.ApplyID);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetExchangeState 的测试
        ///</summary>
        [TestMethod()]
        public void GetExchangeStateTest()
        {
            var applyId = new Guid("91F14D78-41F8-497F-B2CF-E2E5EBE65A26");
            //TH0819001
            int actual = _target.GetExchangeState(Guid.Empty, "TH0819001");
            Assert.AreEqual(1, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///DeleteShopExchngedApply 的测试
        ///</summary>
        [TestMethod()]
        public void DeleteShopExchngedApplyTest()
        {
            var applyId = new Guid("3A64377A-15BB-47F2-99EB-AC7810D7F410"); // TODO: 初始化为适当的值
            string msg;
            bool actual = _target.DeleteShopExchngedApply(applyId, out msg);
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///AddShopRefundApply 的测试
        ///</summary>
        [TestMethod()]
        public void AddShopRefundApplyTest()
        {
            var applyInfo = new ShopExchangedApplyInfo(Guid.NewGuid(), "TH-SH0820001", _shopId, "万店联盟1", DateTime.Now, 200, 10, (byte)ExchangedState.CheckPending, true, "退货");
            IList<ShopApplyDetailInfo> applyDetailInfos = new List<ShopApplyDetailInfo>();
            applyDetailInfos.Add(new ShopApplyDetailInfo(Guid.NewGuid()
                , applyInfo.ApplyID,
                new Guid("6EE68825-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("6EE68825-9ED5-4AEE-9B19-6EE688255C05"), "ceshi6", "goodscode6",
                "", 20, 5, "盒"));
            applyDetailInfos.Add(new ShopApplyDetailInfo(Guid.NewGuid()
                , applyInfo.ApplyID,
                new Guid("88255C05-9ED5-4AEE-9B19-6EE688255C05"),
                new Guid("88255C05-9ED5-4AEE-9B19-6EE688255C05"), "ceshi7", "goodscode7",
                "", 20, 5, "盒"));
            string msg; // TODO: 初始化为适当的值
            bool actual = _target.AddShopRefundApply(applyInfo, applyDetailInfos, out msg);
            //Assert.IsTrue(string.IsNullOrEmpty(msg));
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }


        /// <summary>
        ///IsSuccessCreateCheck 的测试
        ///</summary>
        [TestMethod()]
        public void IsSuccessCreateCheckTest()
        {
            bool actual = _target.IsSuccessCreateCheck(new Guid("E995459F-C081-4B34-81A1-083AB54C9292"), "kuaidi123", "物流公司");
            Assert.IsTrue(actual);
        }

        /// <summary>
        ///GetShopExchangedApplyInfo 的测试
        ///</summary>
        [TestMethod()]
        public void GetShopExchangedApplyInfoTest1()
        {
            ShopExchangedApplyInfo actual = _target.GetShopExchangedApplyInfo(new Guid("0F75230F-A2E4-4029-8666-8BC85D0943CF"));
            Assert.IsTrue(actual.IsBarter);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        /// 商品模糊搜索
        /// </summary>
        [TestMethod]
        public void GetGoodsListByGoodsNameOrGoodsCode()
        {
            //var info = GoodsManager.GetGoodsBaseInfoByGoodsCode("3535335");
            //var list = GoodsManager.GetRealGoodsIdListByGoodsNameOrCode("迷卡倾城灰彩色隐形眼镜半年抛一片装");
            var list = new GoodsManager().GetGoodsSelectList("MK06184");
            Assert.IsTrue(list != null && list.Count>0);
        }
    }
}
