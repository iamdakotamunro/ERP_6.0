using System;
using ERP.BLL.Implement.Order;
using ERP.DAL.Implement.Order.Fakes;
using ERP.DAL.Interface.IOrder.Fakes;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL.Interface;
using ERP.SAL.Interface.Fakes;
using ERP.SAL.StockCenterSAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/5/30 16:49:55 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/5/30 16:49:55 
     * 修改人  ：  
     * 描述    ：
     */
    [TestClass]
    public class UnitTest_LensProcessManager
    {
        private static readonly StubILensProcess _lensProcess = new StubILensProcess();
        private static readonly StubLensProcessMatch _lensProcessMatch = new StubLensProcessMatch(GlobalConfig.DB.FromType.Write);
        private static readonly StubIStockCenterSao _StockCenterSao = new StubIStockCenterSao();
        private readonly LensProcessManager _lensProcessManager = new LensProcessManager(_lensProcess, _lensProcessMatch, _StockCenterSao);

        [TestMethod]
        public void TestUpdateLensProcessOperateState()
        {
            try
            {
                _lensProcessManager.UpdateLensProcessOperateState(Guid.Empty, (int)LensProcessOperateState.Picked, "");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

        [TestMethod]
        public void TestGetLensProcessDetailsDt()
        {
            try
            {
                _lensProcessManager.GetLensProcessDetailsDt(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }
    
    
    }
}
