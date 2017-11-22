using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.SAL.Interface.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/1 17:53:02 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/6/1 17:53:02 
     * 修改人  ：  
     * 描述    ：
     */
    [TestClass]
    public class UnitTest_SalesGoodsRankingManager
    {
        static readonly StubIGoodsCenterSao _goodsCenterSao = new StubIGoodsCenterSao();
        static readonly StubISalesGoodsRanking _salesGoodsRanking = new StubISalesGoodsRanking();
        static readonly SalesGoodsRankingManager _goodsRankingManager = new SalesGoodsRankingManager(_goodsCenterSao, _salesGoodsRanking);

        [TestMethod]
        public void TestGetGoodsSalesRanking()
        {
            try
            {
                _goodsRankingManager.GetGoodsSalesRanking(0, Guid.Empty, Guid.Empty, "",Guid.Empty,
                                                                    Guid.Empty,
                                                                    Guid.Empty,
                                                                    Guid.Empty,
                                                                    Guid.Empty, DateTime.Now, DateTime.Now,
                                                                    false, 0);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }
    
    }
}
