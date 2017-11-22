using System;
using System.Collections.Generic;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Order.Fakes;
using ERP.DAL.Interface.IGoods.Fakes;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.DAL.Interface.IOrder.Fakes;
using ERP.Model.ASYN;
using ERP.SAL.Interface.Fakes;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class UnitTest_AsynReckoningManager
    {
        private readonly StubIReckoning _stubIReckoning=new StubIReckoning();
        private readonly StubIGoodsOrder _stubIGoodsOrder=new StubIGoodsOrder();
        private readonly StubIGoodsOrderDetail _stubIGoodsOrderDetail=new StubIGoodsOrderDetail();
        private readonly StubIGoodsOrderDeliver _stubIGoodsOrderDeliver=new StubIGoodsOrderDeliver();
        private readonly StubICode _stubICode=new StubICode();
        private readonly StubICompanyCussent _stubICompanyCussent=new StubICompanyCussent();
        private readonly StubIExpress _stubIExpress=new StubIExpress();
        private readonly StubGoodsPriceVerificationDao _stubGoodsPriceVerificationDao=new StubGoodsPriceVerificationDao(Environment.GlobalConfig.DB.FromType.Read);
        private readonly StubIGoodsCenterSao _stubIGoodsCenterSao=new StubIGoodsCenterSao();
        private ReckoningManager _reckoningManager;

        [TestMethod]
        public void TestReckoningManager()
        {
            _reckoningManager=new ReckoningManager(Environment.GlobalConfig.DB.FromType.Read);
        }

        [TestMethod]
        public void TestRunAsynAddTask()
        {
            _stubIReckoning.GetAsynListInt32= i => new List<ERP.Model.ASYN.ASYNReckoningInfo>();
            _reckoningManager=new ReckoningManager(_stubIGoodsCenterSao,_stubIReckoning,_stubIExpress,_stubICompanyCussent,_stubGoodsPriceVerificationDao,_stubIGoodsOrder,
                _stubIGoodsOrderDetail,_stubIGoodsOrderDeliver,_stubICode);
            _reckoningManager.RunAsynAddTask(2);

            _stubIReckoning.GetAsynListInt32 = i => new List<ERP.Model.ASYN.ASYNReckoningInfo>()
            {
                new ASYNReckoningInfo()
                {
                    CreateTime = DateTime.Now,ID = Guid.NewGuid(),
                    IdentifyId = new Guid("F7B1F276-C656-4091-90AC-97AB59CD2E05"),
                    IdentifyKey = "DDA00215122309005",ReckoningFromType = "完成订单"
                }
            };
        }

        [TestMethod]
        public void TestNewCreateReckoningInfoList()
        {
            var goodsOrderInfo=new GoodsOrderInfo();
            var goodsOrderDetails = new List<GoodsOrderDetailInfo>();
            _reckoningManager = new ReckoningManager(_stubIGoodsCenterSao, _stubIReckoning, _stubIExpress, _stubICompanyCussent, _stubGoodsPriceVerificationDao, _stubIGoodsOrder,
                _stubIGoodsOrderDetail, _stubIGoodsOrderDeliver, _stubICode);

        }

        [TestMethod]
        public void TestNewCreateReckoningInfoList2()
        {
            try
            {
                _reckoningManager = new ReckoningManager(_stubIGoodsCenterSao, _stubIReckoning, _stubIExpress, _stubICompanyCussent, _stubGoodsPriceVerificationDao, _stubIGoodsOrder,
               _stubIGoodsOrderDetail, _stubIGoodsOrderDeliver, _stubICode);
                string errorMsg = "";
                _reckoningManager.NewCreateReckoningInfoList(null, null, out errorMsg);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

    }
}
