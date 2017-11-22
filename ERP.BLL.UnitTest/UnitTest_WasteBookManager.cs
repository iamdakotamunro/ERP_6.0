using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class UnitTest_WasteBookManager
    {
        private readonly StubIWasteBook _stubIWasteBook=new StubIWasteBook();
        private WasteBookManager _wasteBookManager;

        [TestMethod]
        public void TestWasteBookManager()
        {
            _wasteBookManager = new WasteBookManager(Environment.GlobalConfig.DB.FromType.Read);
        }

        [TestMethod]
        public void TestGetFundPaymentDaysInfos()
        {
            //期末余额
            _stubIWasteBook.GetFundPaymentDaysBankInfosInt32GuidInt32String = (i, guid, arg3, arg4) => null;
            //收款列表
            _stubIWasteBook.GetFundPaymentDaysInfosInt32Int32GuidString = (i, i1, arg3, arg4) => null;
            _wasteBookManager=new WasteBookManager(_stubIWasteBook);
            var result = _wasteBookManager.GetFundPaymentDaysInfos(2, 2015, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), "");
            Assert.IsTrue(result.Count==0);

            _stubIWasteBook.GetFundPaymentDaysBankInfosInt32GuidInt32String = (i, guid, arg3, arg4) => new List<ERP.Model.FundPaymentDaysInfo>()
            {
                new FundPaymentDaysInfo
                {
                    BankName = "百秀天猫支付宝",
                    BankAccountsId = new Guid("2561BFC9-ADC8-4544-90AB-3A4B695405F8"),
                    MaxJan = 100,
                    MaxFeb = 200,
                    MaxMar = 300,
                    MaxApr = 400,
                    MaxMay = 500,
                    MaxJun = 600,
                    MaxJuly = 700,
                    MaxAug = 800,
                    MaxSept = 900,
                    MaxOct = 1000,
                    MaxNov = 1100,
                    MaxDecember = 1200
                }
            };
            _wasteBookManager = new WasteBookManager(_stubIWasteBook);
            var result1 = _wasteBookManager.GetFundPaymentDaysInfos(2, 2015, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), "");
            Assert.IsTrue(result1.Count == 1);

            _stubIWasteBook.GetFundPaymentDaysInfosInt32Int32GuidString = (i, guid, arg3, arg4) => new List<ERP.Model.FundPaymentDaysInfo>()
            {
                new FundPaymentDaysInfo
                {
                    BankName = "百秀天猫支付宝",
                    BankAccountsId = new Guid("2561BFC9-ADC8-4544-90AB-3A4B695405F8"),
                    MaxJan = 100,
                    MaxFeb = 200,
                    MaxMar = 300,
                    MaxApr = 400,
                    MaxMay = 500,
                    MaxJun = 600,
                    MaxJuly = 700,
                    MaxAug = 800,
                    MaxSept = 900,
                    MaxOct = 1000,
                    MaxNov = 1100,
                    MaxDecember = 1200
                },
                new FundPaymentDaysInfo
                {
                    BankName = "支付宝（复制）",
                    BankAccountsId = new Guid("9B381867-A499-4A8C-9C1A-95B60A4DD44F"),
                    MaxJan = 120,
                    MaxFeb = 200,
                    MaxMar = 300,
                    MaxApr = 410,
                    MaxMay = 500,
                    MaxJun = 630,
                    MaxJuly = 700,
                    MaxAug = 800,
                    MaxSept = 900,
                    MaxOct = 1000,
                    MaxNov = 1100,
                    MaxDecember = 1200
                }
            };
            _wasteBookManager = new WasteBookManager(_stubIWasteBook);
            var result2= _wasteBookManager.GetFundPaymentDaysInfos(2, 2015, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), "");
            Assert.IsTrue(result2.Count == 2);

            _stubIWasteBook.GetFundPaymentDaysBankInfosInt32GuidInt32String = (i, guid, arg3, arg4) => null;
            _wasteBookManager = new WasteBookManager(_stubIWasteBook);
            var result3 = _wasteBookManager.GetFundPaymentDaysInfos(2, 2015, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), "");
            Assert.IsTrue(result3.Count == 2);
        }
    }
}
