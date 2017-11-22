using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.Model.Report;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 报表存档相关业务测试
    /// </summary>
    [TestClass]
    public class UnitTest_SupplierReportBll
    {
        SupplierReportBll _supplierReportBll;
        readonly StubISupplierReport  _iSupplierReport=new StubISupplierReport();
        readonly StubICompanyCussent _stubICompanyCussent=new StubICompanyCussent();
        readonly StubIReckoning _stubIReckoning=new StubIReckoning();

        [TestInitialize]
        public void Init()
        {
            _stubIReckoning.SelectCurrentMonthPaymentsRecordsInt32Int32= (i, i1) => new List<SupplierPaymentsRecordInfo>
            {
                new SupplierPaymentsRecordInfo
                {
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),
                    FilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698")
                }
            };

            _stubICompanyCussent.GetCompanyCussentListByCompanyNameString= str => new List<CompanyCussentInfo>
            {
                new CompanyCussentInfo
                {
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),CompanyName = "测试",PaymentDays = 2
                }
            };

            _iSupplierReport.SelectPaymentsReportsGroupByCompanyIdInt32GuidString = (i, guid, arg3) => new List<SupplierPaymentsReportInfo>
            {
                new SupplierPaymentsReportInfo
                {
                    April = 400,April4 = 200,CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),FilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698")
                }
            };

            _iSupplierReport.SelectPaymentsReprotsGroupByFilialeIdInt32= i => new List<SupplierPaymentsReportInfo>
            {
                new SupplierPaymentsReportInfo
                {
                    FilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698")
                }
            };

            _stubIReckoning.SelectCurrentMonthPaymentsRecordsDetailInt32Int32GuidString = (i, i1, arg3, arg4) => new List<SupplierPaymentsRecordInfo>
            {
                new SupplierPaymentsRecordInfo
                {
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),TotalAmount = 100,TotalNoPayed = 50
                }
            };

            _iSupplierReport.SelectStockReprotsGroupByFilialeIdInt32 = i => new List<SupplierPaymentsReportInfo>
            {
                new SupplierPaymentsReportInfo
                {
                    FilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698")
                }
            };

            _stubIReckoning.SelectCurrentMonthStockRecordsInt32Int32 = (i, i1) => new List<SupplierPaymentsRecordInfo>
            {
                new SupplierPaymentsRecordInfo
                {
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),TotalAmount = 100,TotalNoPayed = 50
                    ,FilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698")
                }
            };
            _supplierReportBll = new SupplierReportBll(_iSupplierReport,_stubICompanyCussent,_stubIReckoning);
        }

        /// <summary>
        /// 插入往来帐明细存档
        /// </summary>
        [TestMethod]
        public void TestInsertRececkoning()
        {
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3);
            _iSupplierReport.IsExistsRecentDateTime = time => true;
            //_iSupplierReport.InsertRececkoningDateTimeIListOfRecordReckoningInfo = (time, list) => true;
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var result8 = _supplierReportBll.InsertRececkoning(date);
            Assert.IsTrue(result8);

            date = new DateTime(DateTime.Now.Year, 11, 1);
            _iSupplierReport.IsExistsDateTime = time => true;
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var result9 = _supplierReportBll.InsertRececkoning(date);
            Assert.IsTrue(result9);

            _iSupplierReport.IsExistsDateTime = time => false;
            _stubIReckoning.SelectRecordReckoningInfosDateTimeDateTime= (time, dateTime) => new List<ERP.Model.Report.RecordReckoningInfo>();
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var result2 = _supplierReportBll.InsertRececkoning(date);
            Assert.IsTrue(!result2);


             _stubIReckoning.SelectRecordReckoningInfosDateTimeDateTime = (time, dateTime) => new List<RecordReckoningInfo>
            {
                new RecordReckoningInfo
                {
                    ReckoningId = Guid.NewGuid()
                }
            };
            _iSupplierReport.InsertRececkoningBooleanIListOfRecordReckoningInfo = (b, list) => true;
             _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
             var result3 = _supplierReportBll.InsertRececkoning(date);
            Assert.IsTrue(result3);
        }

        /// <summary>
        /// 通过公司获取往来单位对应的应付款金额
        /// </summary>
        [TestMethod]
        public void TestSelectPaymentsReportsGroupByCompanyId()
        {
            var result = _supplierReportBll.SelectPaymentsReportsGroupByCompanyId(DateTime.Now.Year, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), string.Empty, true);
            Assert.IsNotNull(result);

            _stubICompanyCussent.GetCompanyCussentListByCompanyNameString = str => new List<CompanyCussentInfo>
            {
                new CompanyCussentInfo
                {
                    CompanyId = new Guid("32CB5565-5845-4F78-BC82-0571738FD771"),CompanyName = "测试",PaymentDays = 2
                }
            };
            var result3 = _supplierReportBll.SelectPaymentsReportsGroupByCompanyId(DateTime.Now.Year, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), string.Empty, true);
            Assert.IsNotNull(result3);

            var result2 = _supplierReportBll.SelectPaymentsReportsGroupByCompanyId(DateTime.Now.Year, Guid.NewGuid(), string.Empty, false);
            Assert.IsNotNull(result2);

            _stubIReckoning.SelectCurrentMonthPaymentsRecordsDetailInt32Int32GuidString= (i, i1, arg3, arg4) => new List<SupplierPaymentsRecordInfo>();
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);

            var result4 = _supplierReportBll.SelectPaymentsReportsGroupByCompanyId(DateTime.Now.Year, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), string.Empty, true);
            Assert.IsNotNull(result4);
        }

        /// <summary>
        /// 获取公司对应的应付款金额记录
        /// </summary>
        [TestMethod]
        public void TestSelectPaymentsReprotsGroupByFilialeId()
        {
            var result = _supplierReportBll.SelectPaymentsReprotsGroupByFilialeId(DateTime.Now.Year,true);
            Assert.IsTrue(result!=null);

            var result2 = _supplierReportBll.SelectPaymentsReprotsGroupByFilialeId(DateTime.Now.Year, false);
            Assert.IsTrue(result2 != null);

            _stubIReckoning.SelectCurrentMonthPaymentsRecordsInt32Int32 = (i, i1) => new List<SupplierPaymentsRecordInfo>();
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var result3 = _supplierReportBll.SelectPaymentsReprotsGroupByFilialeId(DateTime.Now.Year, true);
            Assert.IsTrue(result3.Count > 0);
        }

        /// <summary>
        /// 获取往来单位采购入库存档信息
        /// </summary>
        [TestMethod]
        public void TestSelectStockReportsGroupByCompanyId()
        {
            _iSupplierReport.SelectStockReportsGroupByCompanyIdInt32GuidString= (i, guid, arg3) => new List<SupplierPaymentsReportInfo>
            {
                new SupplierPaymentsReportInfo
                {
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771")
                }
            };

            _stubICompanyCussent.GetCompanyCussentListByCompanyNameString= s => new List<CompanyCussentInfo>
            {
                new CompanyCussentInfo
                {
                    CompanyId = new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),CompanyName = "测试数据",PaymentDays = 1
                }
            };

            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var result = _supplierReportBll.SelectStockReportsGroupByCompanyId(2014, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), string.Empty, true);
            Assert.IsTrue(result.Count>0);

            _iSupplierReport.GetPurchasingByForCompanyGuid= guid => new Dictionary<Guid, decimal>
            {
                {new Guid("32CB5565-6EED-4F78-BC82-0571738FD771"),100 },{new Guid("31BF01B3-C8D0-4D4F-8177-A835E3FCBF3C"),200}
            };
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var result2 = _supplierReportBll.SelectStockReportsGroupByCompanyId(DateTime.Now.Year, Guid.Empty, string.Empty, true);
            Assert.IsTrue(result2.Count > 0);
        }

        /// <summary>
        /// 获取公司采购入库存档信息
        /// </summary>
        [TestMethod]
        public void TestSelectStockReprotsGroupByFilialeId()
        {
            var result = _supplierReportBll.SelectStockReprotsGroupByFilialeId(DateTime.Now.Year,true);
            Assert.IsNotNull(result);

            var result2 = _supplierReportBll.SelectStockReprotsGroupByFilialeId(DateTime.Now.Year, false);
            Assert.IsNotNull(result2);

            _stubIReckoning.SelectCurrentMonthStockRecordsInt32Int32 = (i, i1) => new List<SupplierPaymentsRecordInfo>();
            _supplierReportBll = new SupplierReportBll(_iSupplierReport, _stubICompanyCussent, _stubIReckoning);
            var result3 = _supplierReportBll.SelectStockReprotsGroupByFilialeId(DateTime.Now.Year, true);
            Assert.IsTrue(result3.Count>0);

        }
    }
}
