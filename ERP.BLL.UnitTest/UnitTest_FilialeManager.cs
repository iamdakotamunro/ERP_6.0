using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Organization;
using ERP.SAL.Fakes;
using Keede.Ecsoft.Model;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIS.Enum;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// UnitTest_FilialeManager 的摘要说明
    /// </summary>
    [TestClass]
    public class UnitTestFilialeManager
    {
        [TestMethod]
        public void TestSaveFiliale()
        {
            //using (ShimsContext.Create())
            //{
            //    ////模拟返回结果
            //    //ShimFilialeDao.SaveFilialeFilialeInfo = info => true;
            //    //var result = new FilialeManager().SaveFiliale(new FilialeInfo());
            //    //Assert.IsTrue(result);
            //}
        }

        [TestMethod]
        public void TestGetList()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo>{new FilialeInfo()};
                var result = FilialeManager.GetList();
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestGetByFilialeId()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859") } };
                var result = FilialeManager.Get(new Guid("6E677868-AE98-4932-B4DB-0000020DC859"));
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestGetNameByFilialeId()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Name = "tester" } };
                var expect1 = "tester";
                var result1 = FilialeManager.GetName(new Guid("6E677868-AE98-4932-B4DB-0000020DC859"));
                Assert.AreEqual(expect1, result1);

                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Name = "tester" } };
                var expect2 = String.Empty;
                var result2 = FilialeManager.GetName(new Guid("00000000-0000-0000-0000-000000000000"));
                Assert.AreEqual(expect2, result2);
            }
        }

        [TestMethod]
        public void TestIsEntityShopFilialeByFilialeId()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int)FilialeType.EntityShop } };
                var result1 = FilialeManager.IsEntityShopFiliale(new Guid("6E677868-AE98-4932-B4DB-0000020DC859"));
                Assert.IsTrue(result1);

                var result2 = FilialeManager.IsEntityShopFiliale(new Guid("00000000-0000-0000-0000-000000000000"));
                Assert.IsFalse(result2);
            }
        }

        [TestMethod]
        public void TestIsEntityShopFilialeByFilialeIdAndErrorMessage()
        {
            using (ShimsContext.Create())
            {
                string outMessage1;
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int)FilialeType.EntityShop } };
                var expect1 = String.Empty;
                var result1 = FilialeManager.IsEntityShopFiliale(new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), out outMessage1);
                Assert.IsTrue(result1);
                Assert.AreEqual(expect1, outMessage1);

                string outMessage2;
                var expect2 = "公司信息未获取到";
                var result2 = FilialeManager.IsEntityShopFiliale(new Guid("00000000-0000-0000-0000-000000000000"), out outMessage2);
                Assert.IsFalse(result2);
                Assert.AreEqual(expect2, outMessage2);
            }
        }

        [TestMethod]
        public void TestIsAllianceShopFilialeByFilialeIdAndErrorMessage()
        {
            using (ShimsContext.Create())
            {
                string outMessage1;
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int)FilialeType.EntityShop, IsActive = true } };
                var expect1 = String.Empty;
                var result1 = FilialeManager.IsAllianceShopFiliale(new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), out outMessage1);
                Assert.IsTrue(result1);
                Assert.AreEqual(expect1, outMessage1);

                string outMessage2;
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int)FilialeType.EntityShop, IsActive = false } };
                var expect2 = String.Empty;
                var result2 = FilialeManager.IsAllianceShopFiliale(new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), out outMessage2);
                Assert.IsFalse(result2);
                Assert.AreEqual(expect2, outMessage2);

                string outMessage3;
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo> { new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int)FilialeType.SaleCompany, IsActive = true } };
                var expect3 = String.Empty;
                var result3 = FilialeManager.IsAllianceShopFiliale(new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), out outMessage3);
                Assert.IsFalse(result3);
                Assert.AreEqual(expect3, outMessage3);

                string outMessage4;
                var expect4 = "公司信息未获取到";
                var result4 = FilialeManager.IsAllianceShopFiliale(new Guid("00000000-0000-0000-0000-000000000000"), out outMessage4);
                Assert.IsFalse(result4);
                Assert.AreEqual(expect4, outMessage4);
            }
        }

        [TestMethod]
        public void TestGetShopHeadFilialeIdByFilialeId()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo>
                {
                    new FilialeInfo { ID = new Guid("1E677868-AE98-4932-B4DB-0000020DC859"), Rank = (int)FilialeRank.Head }, 
                    new FilialeInfo { ID = new Guid("2E677868-AE98-4932-B4DB-0000020DC859"), ParentId = new Guid("1E677868-AE98-4932-B4DB-0000020DC859"),Rank = (int)FilialeRank.Partial },
                    new FilialeInfo { ID = new Guid("3E677868-AE98-4932-B4DB-0000020DC859"), ParentId = new Guid("2E677868-AE98-4932-B4DB-0000020DC859"),Rank = (int)FilialeRank.Child }
                };
                var expect1 = new Guid("1E677868-AE98-4932-B4DB-0000020DC859");
                var result1 = FilialeManager.GetShopHeadFilialeId(new Guid("3E677868-AE98-4932-B4DB-0000020DC859"));
                Assert.AreEqual(expect1, result1);

                var expect2 = new Guid("1E677868-AE98-4932-B4DB-0000020DC859");
                var result2 = FilialeManager.GetShopHeadFilialeId(new Guid("2E677868-AE98-4932-B4DB-0000020DC859"));
                Assert.AreEqual(expect2, result2);

                var expect3 = new Guid("1E677868-AE98-4932-B4DB-0000020DC859");
                var result3 = FilialeManager.GetShopHeadFilialeId(new Guid("1E677868-AE98-4932-B4DB-0000020DC859"));
                Assert.AreEqual(expect3, result3);


            }
        }

        [TestMethod]
        public void TestGetHeadList()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo>
                {
                    new FilialeInfo { ID = new Guid("1E677868-AE98-4932-B4DB-0000020DC859"), Rank = (int)FilialeRank.Head }, 
                    new FilialeInfo { ID = new Guid("2E677868-AE98-4932-B4DB-0000020DC859"), Rank = (int)FilialeRank.Partial },
                    new FilialeInfo { ID = new Guid("3E677868-AE98-4932-B4DB-0000020DC859"), Rank = (int)FilialeRank.Child }
                };
                var result = FilialeManager.GetHeadList();
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestGetEntityShop()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo>
                {
                    new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.EntityShop,IsActive = false},
                    new FilialeInfo { ID = new Guid("7E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.SaleCompany,IsActive = false },
                    new FilialeInfo { ID = new Guid("8E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.EntityShop,IsActive = true }
                };
                var result = FilialeManager.GetEntityShop();
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestGetB2CFilialeList()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo>
                {
                    new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.EntityShop,IsActive = false},
                    new FilialeInfo { ID = new Guid("7E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.SaleCompany,IsActive = false },
                    new FilialeInfo { ID = new Guid("8E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.EntityShop,IsActive = true }
                };
                var result = FilialeManager.GetB2CFilialeList();
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestGetAllianceFilialeList()
        {
            using (ShimsContext.Create())
            {
                //模拟返回结果
                ShimFilialeSao.GetAllFiliale = () => new List<FilialeInfo>
                {
                    new FilialeInfo { ID = new Guid("6E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.EntityShop,IsActive = false},
                    new FilialeInfo { ID = new Guid("7E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.SaleCompany,IsActive = false },
                    new FilialeInfo { ID = new Guid("8E677868-AE98-4932-B4DB-0000020DC859"), Type = (int) FilialeType.EntityShop,IsActive = true }
                };
                var result = FilialeManager.GetAllianceFilialeList();
                Assert.IsNotNull(result);
            }
        }
    }
}
