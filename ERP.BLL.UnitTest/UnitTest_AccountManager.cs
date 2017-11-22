using System;
using ERP.SAL.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Model;
using ERP.BLL.Implement.Organization;
using ERP.SAL.Interface.Fakes;
using MIS.Model.View;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// UnitTest_AccountManager 的摘要说明
    /// </summary>
    [TestClass]
    public class UnitTestAccountManager
    {
        private static readonly StubIPersonnelSao _stubIPersonnelSao = new StubIPersonnelSao();
        
        [TestMethod]
        public void TestLoginByUserNameAndPassword()
        {
            _stubIPersonnelSao.GetGuid = pId => new PersonnelInfo(new LoginAccountInfo());
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimAccountSao.LoginStringString = (userName, password) => new Model.LoginResultInfo(new MIS.Model.View.LoginResultInfo("success"))
                {
                    IsSuccess = true,
                    PersonnelInfo = new PersonnelInfo(new LoginAccountInfo 
                    { 
                        PersonnelId = new Guid(), 
                        RealName ="test"
                    })
                };

                var result = AccountManager.Login("tester", "123456", _stubIPersonnelSao);
                Assert.IsNotNull(result);
            }
        }
    }
}
