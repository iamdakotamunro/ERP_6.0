using System;
using ERP.DAL.Interface.IOrder.Fakes;
using ERP.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.SAL.Interface.Fakes;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.DAL.Interface.IBasis.Fakes;

namespace ERP.BLL.UnitTest
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/5/30 17:04:36 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/5/30 17:04:36 
     * 修改人  ：  
     * 描述    ：
     */
    [TestClass]
    public class UnitTest_GoodsOrder
    {
        static readonly StubIGoodsOrder _stubIGoodsOrder = new StubIGoodsOrder();
        static readonly StubIGoodsOrderDetail _stubIGoodsOrderDetail = new StubIGoodsOrderDetail();
        static readonly StubIOrderHint _stubIOrderHint = new StubIOrderHint();
        static readonly StubIStockCenterSao _stubIStockCenterSao = new StubIStockCenterSao();
        static readonly StubIInvoice _stubIInvoice = new StubIInvoice();
        static readonly StubIGoodsCenterSao _stubIGoodsCenterSao = new StubIGoodsCenterSao();
        static readonly StubIDistrict _stubIDistrict = new StubIDistrict();
        static readonly StubIExpress _stubIExpress = new StubIExpress();
        static readonly StubIShippAddress _stubIShippAddress = new StubIShippAddress();
        readonly BLL.Implement.Order.GoodsOrder _goodsOrderBll = new BLL.Implement.Order.GoodsOrder(_stubIGoodsOrder, _stubIGoodsOrderDetail, _stubIOrderHint, _stubIStockCenterSao, _stubIInvoice, _stubIGoodsCenterSao, _stubIDistrict, _stubIExpress, _stubIShippAddress);

        [TestMethod]
        public void TestSetOrderState()
        {
            try
            {
                _goodsOrderBll.SetOrderState(Guid.Empty, OrderState.WaitForConsignment);
            }
            catch (Exception ex)
            {
                Assert.IsFalse(string.IsNullOrEmpty(ex.Message));
            }
        }

    }
}
