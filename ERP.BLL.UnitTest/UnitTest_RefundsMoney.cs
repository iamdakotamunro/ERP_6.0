using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Model.RefundsMoney;
using Config.Keede.Library;
using System.Collections.Generic;
using System.Linq;
using ERP.Environment;
using ERP.DAL.Interface.FinanceModule;
using ERP.DAL.Implement.FinanceModule;
using ERP.BLL.Interface;
using ERP.BLL.Implement.FinanceModule;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 退款打款
    /// </summary>
    [TestClass]
    public class UnitTest_RefundsMoney
    {
        #region 测试IDAL

        private static readonly IRefundsMoneyDal _refundsMoneyDal = new RefundsMoneyDal();

        public void Init()
        {
            ConfManager.Init();
            // ERP 读写分离配置
            var readConnectionsOfErp = new List<string>();
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_1"));
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_2"));
            readConnectionsOfErp.Add(ConfManager.GetAppsetting("db_ERP_ReadConnections_3"));
            var writeConnectionOfErp = ConfManager.GetAppsetting("db_ERP_WriteConnection");
            Keede.DAL.RWSplitting.ConnectionContainer.AddDbConnections(GlobalConfig.ERP_DB_NAME, writeConnectionOfErp, readConnectionsOfErp.Where(m => !string.IsNullOrEmpty(m)).ToArray(), Keede.DAL.RWSplitting.EnumStrategyType.Loop);
        }

        [TestMethod]
        public void TestMethod_Add()
        {
            Init();

            var model = new RefundsMoneyInfo_Add()
            {
                ID = Guid.NewGuid(),
                AfterSalesNumber = "退换货号123",
                CreateUser = "Test用户",
                OrderNumber = "订单号111",
                RefundsAmount = 2,
                SaleFilialeId = Guid.Parse("43609645-97dd-4ae4-989d-f3c867969a99"),
                SalePlatformId = Guid.Parse("443a404a-91a2-409e-9b47-10af5489c360"),
                ThirdPartyAccountName = "第三方账户222",
                ThirdPartyOrderNumber = "ddd",
            };

            var result = _refundsMoneyDal.AddRefundsMoney(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Edit()
        {
            Init();

            var model = new RefundsMoneyInfo_Edit()
            {
                ID = Guid.Parse("05852844-E829-4443-B354-0635076CA98D"),
                BankAccountNo = "1",
                BankName = "2",
                UserName = "3",
                Remark = "4",
                ModifyUser = "修改的用户",
            };
            var result = _refundsMoneyDal.UpdateRefundsMoney(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Approval()
        {
            Init();

            var model = new RefundsMoneyInfo_Approval()
            {
                ID = Guid.Parse("05852844-E829-4443-B354-0635076CA98D"),
                BankAccountNo = "11",
                BankName = "22",
                UserName = "33",
                ModifyUser = "审核的用户",
                IsApproved = true,
                RejectReason = "通过"
            };
            var result = _refundsMoneyDal.ApprovalRefundsMoney(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_ApprovalPayment()
        {
            Init();

            var model = new RefundsMoneyInfo_Payment()
            {
                ID = Guid.Parse("05852844-E829-4443-B354-0635076CA98D"),
                AccountID = Guid.Parse("7175FA90-1214-46E7-8D15-F9053E16928C"),
                Fees = 3.33m,
                TransactionNumber = "这是交易号",
                IsPayment = true,
                ModifyUser = "审核打款的用户",
                RejectReason = "通过2"
            };
            var result = _refundsMoneyDal.ApprovalPaymentRefundsMoney(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Delete()
        {
            Init();

            var result = _refundsMoneyDal.DeleteRefundsMoney(Guid.Parse("05852844-E829-4443-B354-0635076CA98D"), "删除的用户");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Get()
        {
            Init();

            var result = _refundsMoneyDal.GetRefundsMoneyByID(Guid.Parse("05852844-E829-4443-B354-0635076CA98D"));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_GetList()
        {
            Init();

            int totalCount = 0;
            var model = new RefundsMoneyInfo_SeachModel()
            {
                //OrderNumber = "",
                //EndTime=DateTime.Now,
                //SaleFilialeId=Guid.NewGuid(),
                //SalePlatformId=Guid.NewGuid(),
                //StartTime=DateTime.Now,
                //Status=0,
                PageSize = 10,
                PageIndex = 1,
            };
            IList<RefundsMoneyInfo> result = _refundsMoneyDal.GetRefundsMoneyList(model, out totalCount);
            Assert.IsNotNull(result);
        }

        #endregion 测试IDAL
    }
}