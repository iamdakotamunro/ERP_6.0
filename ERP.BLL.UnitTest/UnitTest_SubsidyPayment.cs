using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Model.SubsidyPayment;
using Config.Keede.Library;
using System.Collections.Generic;
using System.Linq;
using ERP.Environment;
using ERP.DAL.Interface.FinanceModule;
using ERP.DAL.Implement.FinanceModule;
using ERP.BLL.Interface;
using ERP.BLL.Implement.FinanceModule;
using ERP.Enum.SubsidyPayment;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 补贴审核、补贴打款
    /// </summary>
    [TestClass]
    public class UnitTest_SubsidyPayment
    {
        #region 测试IDAL

        private static readonly ISubsidyPaymentDal _SubsidyPaymentDal = new SubsidyPaymentDal();

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

            var model = new SubsidyPaymentInfo_Add()
            {
                ID = Guid.NewGuid(),
                CreateUser = "Test用户",
                OrderNumber = "订单号111",
                SaleFilialeId = Guid.Parse("43609645-97dd-4ae4-989d-f3c867969a99"),
                SalePlatformId = Guid.Parse("443a404a-91a2-409e-9b47-10af5489c360"),
                ThirdPartyAccountName = "第三方账户222",
                ThirdPartyOrderNumber = "ddd",
                BankAccountNo = "账户2",
                OrderAmount = 33,
                SubsidyAmount = 2,
                BankName = "aa",
                QuestionType = Guid.NewGuid(),
                QuestionName = "运费补贴",
                Remark = "备注",
                SubsidyType = (int)SubsidyTypeEnum.Compensate,
                UserName = "michael",
            };

            var result = _SubsidyPaymentDal.AddSubsidyPayment(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Edit()
        {
            Init();

            var model = new SubsidyPaymentInfo_Edit()
            {
                ID = Guid.Parse("C03D601C-C007-4706-A4F6-763DE5241940"),
                BankAccountNo = "1",
                BankName = "2",
                UserName = "3",
                Remark = "4",
                OrderNumber = "5",
                QuestionType = Guid.NewGuid(),
                QuestionName = "运费补贴",
                SubsidyAmount = 6.66m,
                SubsidyType = (int)SubsidyTypeEnum.Compensate,
                ThirdPartyOrderNumber = "234",
                ModifyUser = "修改的用户",
            };
            var result = _SubsidyPaymentDal.UpdateSubsidyPayment(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Approval()
        {
            Init();

            var model = new SubsidyPaymentInfo_Approval()
            {
                ID = Guid.Parse("C03D601C-C007-4706-A4F6-763DE5241940"),
                BankAccountNo = "11",
                BankName = "22",
                UserName = "33",
                ModifyUser = "审核的用户",
                IsApproved = false,
                RejectReason = "不通过"
            };
            var result = _SubsidyPaymentDal.ApprovalSubsidyPayment(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Check()
        {
            Init();

            var model = new SubsidyPaymentInfo_Check()
            {
                ID = Guid.Parse("C03D601C-C007-4706-A4F6-763DE5241940"),
                ModifyUser = "审核打款的用户",
                IsApproved = false,
                RejectReason = "不通过"
            };
            var result = _SubsidyPaymentDal.CheckSubsidyPayment(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Payment()
        {
            Init();

            var model = new SubsidyPaymentInfo_Payment()
            {
                ID = Guid.Parse("C03D601C-C007-4706-A4F6-763DE5241940"),
                AccountID = Guid.Parse("7175FA90-1214-46E7-8D15-F9053E16928C"),
                Fees = 3.33m,
                TransactionNumber = "这是交易号",
                IsPayment = true,
                ModifyUser = "打款的用户",
                RejectReason = "通过"
            };
            var result = _SubsidyPaymentDal.ApprovalPaymentSubsidyPayment(model);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Delete()
        {
            Init();

            var result = _SubsidyPaymentDal.DeleteSubsidyPayment(Guid.Parse("C03D601C-C007-4706-A4F6-763DE5241940"), "删除的用户");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_Get()
        {
            Init();

            var result = _SubsidyPaymentDal.GetSubsidyPaymentByID(Guid.Parse("C03D601C-C007-4706-A4F6-763DE5241940"));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethod_GetList()
        {
            Init();

            int totalCount = 0;
            var model = new SubsidyPaymentInfo_SeachModel()
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
            IList<SubsidyPaymentInfo> result = _SubsidyPaymentDal.GetSubsidyPaymentList(model, out totalCount);
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void TestMethod_GetSumList()
        {
            Init();
            List<string> listID = new List<string>();
            listID.Add("c03d601c-c007-4706-a4f6-763de5241940");
            listID.Add("62F5D7A3-BE37-43BF-9AEC-53A1523212D6");
            var result = _SubsidyPaymentDal.GetSumList(listID);
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public void TestMethod_IsExistSubsidyPayment()
        {
            Init();

            string ThirdPartyOrderNumber = "ddd";
            var result = _SubsidyPaymentDal.IsExistSubsidyPayment(ThirdPartyOrderNumber);
            Assert.IsNotNull(result);

        }

        #endregion 测试IDAL
    }
}