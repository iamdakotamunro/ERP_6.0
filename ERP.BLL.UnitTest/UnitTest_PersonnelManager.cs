using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Organization;
using ERP.Model;
using ERP.SAL.Interface.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIS.Model.View;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// UnitTest_PersonnelManager 的摘要说明
    /// </summary>
    [TestClass]
    public class UnitTestPersonnelManager
    {
        private static readonly StubIPersonnelSao _stubIPersonnelSao = new StubIPersonnelSao();
        private static readonly PersonnelManager _personnelManager = new PersonnelManager(_stubIPersonnelSao);
        
        [TestMethod]
        public void TestPersonnelManagerDefault()
        {
            var personnelManager = new PersonnelManager();
            Assert.IsNotNull(personnelManager);
        }

        [TestMethod]
        public void TestGetByAccountNo()
        {
            var account = "tester";
            //模拟返回结果
            _stubIPersonnelSao.GetString = accountNo => new PersonnelInfo(new LoginAccountInfo());
            var result = _personnelManager.Get(account);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetNameByPersonnelId()
        {
            var personnel = Guid.NewGuid();
            var expect = "tester";
            //模拟返回结果
            _stubIPersonnelSao.GetNameGuid = personnelId => "tester";
            var result = _personnelManager.GetName(personnel);
            Assert.AreEqual(expect,result);
        }

        [TestMethod]
        public void TestGetList()
        {
            //模拟返回结果
            _stubIPersonnelSao.GetList = () => new List<PersonnelInfo>();
            var result = _personnelManager.GetList();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetListByFilialeIdAndBranchId()
        {
            //模拟返回结果
            _stubIPersonnelSao.GetListGuidGuid = (filialeId, branchId) => new List<PersonnelInfo>();
            var result = _personnelManager.GetList(Guid.NewGuid(),Guid.NewGuid());
            Assert.IsNotNull(result);
        }
    }
}
