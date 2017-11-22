using ERP.BLL.Implement.FinanceModule;
using ERP.BLL.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class UnitTest_SaleOrderGoodsSettlementManager
    {
        private ISaleOrderGoodsSettlementManager _saleOrderGoodsSettlementManager = SaleOrderGoodsSettlementManager.WriteInstance;

        [TestMethod]
        public void TestGenerate()
        {
            _saleOrderGoodsSettlementManager.Generate(new DateTime(2017, 2, 9), 100);
        }
    }
}
