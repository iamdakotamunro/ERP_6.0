using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.Enum;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 往来单位收付款类单元测试
    /// </summary>
    [TestClass]
    public class UnitTest_CompanyFundReceipt
    {
        readonly StubICompanyFundReceipt _companyFundReceipt = new StubICompanyFundReceipt();
        CompanyFundReceipt _companyFundReceiptBll;

        /// <summary>
        /// 初始
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            _companyFundReceipt.GetAllFundReceiptInfoListGuidReceiptPageCompanyFundReceiptStateDateTimeDateTimeStringCompanyFundReceiptTypeGuidArray = (guid, page, arg3, arg4, arg5, arg6, arg7, arg8) => new List<CompanyFundReceiptInfo>
            {
                new CompanyFundReceiptInfo
                {
                    ReceiptID = new Guid("7FD2A6F8-CBC2-4007-B159-C6F1862AEA12"),ReceiptNo = "PY05102010003",ReceiptStatus = 9,ReceiptType = 1
                },
                new CompanyFundReceiptInfo
                {
                    ReceiptID = new Guid("FF9F5494-7B4B-4F7E-BC55-52507D1CF59A"),ReceiptNo = "GT05102010003",ReceiptStatus = 3,ReceiptType = 0
                }
            };

            _companyFundReceipt.GetCompanyFundReceiptInfoGuid = guid => new CompanyFundReceiptInfo
            {
                ReceiptID = new Guid("88888888-0000-0000-0000-888888888888")
            };

            _companyFundReceiptBll = new CompanyFundReceipt(_companyFundReceipt);
        }
        
        /// <summary>
        /// 更改单据状态
        /// </summary>
        [TestMethod]
        public void TestUpdateFundReceiptState()
        {
            var result = _companyFundReceiptBll.UpdateFundReceiptState(Guid.NewGuid(), CompanyFundReceiptState.All, null);
            Assert.IsTrue(result == 1);
        }
        
        /// <summary>
        /// 更改订单状态
        /// </summary>
        [TestMethod]
        public void TestUpdateFundReceiptStateParams()
        {
            var result = _companyFundReceiptBll.UpdateFundReceiptState(Guid.NewGuid(), CompanyFundReceiptState.Audited, null);
            Assert.IsTrue(result == 0);

            var result1 = _companyFundReceiptBll.UpdateFundReceiptState(Guid.NewGuid(), CompanyFundReceiptState.HasInvoice, null, new[] { new Guid("8B6165FB-AB46-40A2-A904-D3DAF10C1010") });
            Assert.IsTrue(result1 == 1);
        }
    }
}
