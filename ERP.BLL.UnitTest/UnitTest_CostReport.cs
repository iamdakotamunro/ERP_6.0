using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.SAL.Interface.Fakes;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Environment;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class CostReportTest
    {
        readonly StubICostReport _stubICost = new StubICostReport();
        readonly StubICostReckoning _stubICostReckoning = new StubICostReckoning();
        readonly StubIBankAccounts _stubIBankAccounts = new StubIBankAccounts();
        readonly StubIPersonnelSao _stubIPersonnelSao = new StubIPersonnelSao();
        CostReport _costReport;

        [TestInitialize]
        public void Init()
        {
            _stubICost.GetBankAccountGuidString = (guid, str) => "测试";
            _stubICost.IsRepeatSubmitGuidGuidGuidGuidGuidGuidGuidInt32Decimal = (a, b, c, d, e, f, g, h, j) => true;
            //string strs;  Parameter 'str' must be declare as 'out'
            _stubICost.InsertReportCostReportInfoStringOut = (CostReportInfo info, out string str) =>
            {
                str = "";
                return true;
            };
            _stubICost.GetReportListGuid = guid => new List<CostReportInfo>();
            //_stubICost.UpdateReportCostReportInfo = info => true;
            _stubICost.GetReportByReportIdGuid = guid => new CostReportInfo();
            _costReport = new CostReport(GlobalConfig.DB.FromType.Write);
        }

        /// <summary>
        /// 根据时间、发票类型和状态审核部门查询费用申报信息 测试完毕
        /// </summary>
        [TestMethod]
        public void TestGetReportList()
        {
            var result = _costReport.GetReportList(DateTime.Now.AddMonths(-1), DateTime.Now,
                new List<Guid>(), new List<int> { 1, 5 }, new List<int> { 0, 10 });
            Assert.IsTrue(result != null);

            var result2 = _costReport.GetReportList(DateTime.Now.AddMonths(-1), DateTime.Now,
                new List<Guid>(), new List<int> { 1, 5 }, new List<int> { 10, 0 });
            Assert.IsTrue(result2 != null);

            var result3 = _costReport.GetReportList(DateTime.Now.AddMonths(-1), DateTime.Now,
                new List<Guid>(), new List<int> { 1, 5 }, new List<int> { 2, 10, 0 });
            Assert.IsTrue(result3 != null);
        }
    }
}
