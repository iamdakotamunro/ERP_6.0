using System;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.ICompany.Fakes;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.Model;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// zal 2015-11-02
    /// </summary>
    [TestClass()]
    public class UnitTestBankAccountManager
    {
        private readonly StubIBankAccounts _stubIBankAccounts = new StubIBankAccounts();
        private readonly StubIWasteBook _stubIWasteBook=new StubIWasteBook();
        private readonly StubIBankAccountDao _stubIBankAccountDao=new StubIBankAccountDao();
        private BankAccountManager _bankAccountManager;


        [TestMethod]
        public void DeleteTest()
        {
            try
            {
                _stubIWasteBook.GetBalanceGuid = guid => 1;
                _bankAccountManager=new BankAccountManager(_stubIBankAccounts,_stubIBankAccountDao,_stubIWasteBook,Guid.Empty);
                _bankAccountManager.Delete(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "账户中仍有余额，不允许删除！");
            }
            _stubIWasteBook.GetBalanceGuid = guid => 0;
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook,Guid.Empty);
            _bankAccountManager.Delete(Guid.NewGuid());
        }

        [TestMethod()]
        public void VirementTest()
        {
            _stubIWasteBook.GetBalanceGuid = guid => 0;
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.Virement(Guid.NewGuid(),Guid.NewGuid(),100,2,"测试","CS425154",Guid.Empty);

            _stubIBankAccounts.GetBankAccountsGuid = guid => new BankAccountInfo();
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.Virement(Guid.NewGuid(), Guid.NewGuid(), 100, 2, "测试", "CS425154", Guid.Empty);

            _stubIWasteBook.InsertWasteBookInfo = info =>
            {
                throw new Exception("测试");
            };
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            try
            {
                _bankAccountManager.Virement(Guid.NewGuid(), Guid.NewGuid(), 100, 2, "测试", "CS425154", Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "转帐同步失败！");
            } 
        }

        [TestMethod()]
        public void NewVirementTest()
        {
            _stubIWasteBook.GetBalanceGuid = guid => 0;
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.NewVirement(Guid.NewGuid(), Guid.NewGuid(), 100, 2, "测试", "CS425154", Guid.NewGuid(),Guid.NewGuid(),"Lcr");

            var id = Guid.NewGuid();
            _stubIBankAccounts.GetBankAccountsGuid = guid => new BankAccountInfo();
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.NewVirement(Guid.NewGuid(), Guid.NewGuid(), 100, 2, "测试", "CS425154", id, id, "Lcr");


            _stubIWasteBook.InsertWasteBookInfo = info =>
            {
                throw new Exception("测试");
            };
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            try
            {
                _bankAccountManager.NewVirement(Guid.NewGuid(), Guid.NewGuid(), 100, 2, "测试", "CS425154", id, id, "Lcr");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "转帐同步失败！");
            } 
        }

        [TestMethod()]
        public void UpdateBllTest()
        {
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.UpdateBll(Guid.NewGuid(),"ceshi",10,"CS68463",2,Guid.NewGuid());

            _stubIWasteBook.GetWasteBookGuid = guid => new WasteBookInfo()
            {
                Description = "[增加资金]"
            };

            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.UpdateBll(Guid.NewGuid(), "ceshi", 10, "CS68463", 2, Guid.NewGuid());

            _stubIWasteBook.GetWasteBookGuid = guid => new WasteBookInfo
            {
                Description = "[减少资金]"
            };
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.UpdateBll(Guid.NewGuid(), "ceshi", 10, "CS68463", 2, Guid.NewGuid());

            _stubIWasteBook.GetWasteBookGuid = guid => new WasteBookInfo
            {
                Description = "[付款]"
            };
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.UpdateBll(Guid.NewGuid(), "ceshi", 10, "CS68463", 2, Guid.NewGuid());

            _stubIWasteBook.GetWasteBookGuid = guid => new WasteBookInfo();
            _stubIWasteBook.GetWasteBookIdForUpdateString = s => "30AA2F62-0DA9-49CC-8948-4DDEE198072D";
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.UpdateBll(Guid.NewGuid(), "ceshi", 10, "CS68463", 2, Guid.NewGuid());

            _stubIWasteBook.GetWasteBookIdForUpdateString = s => "ce";
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            try
            {
                _bankAccountManager.UpdateBll(Guid.NewGuid(), "ceshi", 10, "CS68463", 2, Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "转帐同步失败！");
            }
        }

        [TestMethod()]
        public void InsertPoundageTest()
        {
            _stubIWasteBook.GetBalanceGuid = guid => 0;
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            _bankAccountManager.InsertPoundage(Guid.NewGuid(), "测试", 2, Guid.Empty);

            _stubIWasteBook.InsertWasteBookInfo = info =>
            {
                throw new Exception("测试");
            };
            _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
            try
            {
                _bankAccountManager.InsertPoundage(Guid.NewGuid(),"测试",2, Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "手续费新建失败！");
            } 
        }


        [TestMethod]
        public void TestAuditing()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.Auditing("");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGet()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.Get(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestDelete()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.Delete(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetBankAccountName()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.GetBankAccountName(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetBankAccounts()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.GetBankAccounts(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }



        [TestMethod]
        public void TestGetBankAccountsList()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.GetBankAccountsList();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetBankAccountsNonce()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.GetBankAccountsNonce(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetList()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.GetList(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetListByTargetId()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.GetListByTargetId(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetPermissionList()
        {
            try
            {
                _bankAccountManager = new BankAccountManager(_stubIBankAccounts, _stubIBankAccountDao, _stubIWasteBook, Guid.Empty);
                _bankAccountManager.GetPermissionList(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }
    }
}
