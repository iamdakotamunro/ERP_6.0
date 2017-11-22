using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.BLL.Implement.Company;
using ERP.DAL.Interface.ICompany.Fakes;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// UnitTest_PowerSet 的摘要说明
    /// </summary>
    [TestClass]
    public class UnitTestPowerSet
    {
        private static readonly StubIPowerSet _stubIPowerSet = new StubIPowerSet();
        private static readonly PowerSet _powerSet = new PowerSet(_stubIPowerSet);

        [TestInitialize]
        public void Init()
        {
            _stubIPowerSet.SetPositionWarehousePowerGuidGuidGuidGuid =
                (filialeId, branchId, positionId, warehouseId) => 1;
            _stubIPowerSet.DelPositionWarehousePowerGuidGuidGuid = (filialeId, branchId, positionId) => 1;
        }

        [TestMethod]
        public void TestPowerSet()
        {
            var powerSet = new PowerSet(_stubIPowerSet);
            Assert.IsNotNull(powerSet);
        }

        [TestMethod]
        public void TestPowerSetDefault()
        {
            var powerSet = new PowerSet(_stubIPowerSet);
            Assert.IsNotNull(powerSet);
        }

        [TestMethod]
        public void TestSetPositionWarehousePower()
        {
            var expect = 1;
            var result = _powerSet.SetPositionWarehousePower(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                Guid.NewGuid());
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        public void TestDelPositionWarehousePower()
        {
            var expect = 1;
            var result = _powerSet.DelPositionWarehousePower(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        public void TestGetPersonnelWarehouse()
        {
            //模拟返回结果
            _stubIPowerSet.GetPersonnelWarehouseGuidGuidGuid = (filialeId, branchId, positionId) => new List<Guid>
            {
                new Guid()
            };
            var result = _powerSet.GetPersonnelWarehouse(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            Assert.IsNotNull(result);
        }
    }
}
