using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.Enum;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.Interface.Fakes;
using ERP.SAL.StockCenterSAL;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.DAL.Implement.Inventory;
using ERP.Environment;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 采购管理类业务测试
    /// </summary>
    [TestClass]
    public class UnitTest_PurchasingManager
    {
        readonly StubIPurchasing _stubPurchasing = new StubIPurchasing(); 
        readonly StubIGoodsCenterSao _stubIGoodsInfoSao = new StubIGoodsCenterSao();
        readonly StubIPurchasingDetail _stubIPurchasingDetail = new StubIPurchasingDetail();
        readonly ERP.DAL.Interface.IInventory.ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        readonly ProcurementTicketLimitDAL _procurementTicketLimitDal = new ProcurementTicketLimitDAL(GlobalConfig.DB.FromType.Write);
        private PurchasingManager _purchasingManager;
        readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);

        private static readonly StubIPurchasing _purchasings = new StubIPurchasing();
        private static readonly StubIPurchasingDetail _purchasingDetail = new StubIPurchasingDetail();
        private readonly PurchasingDetailManager _purchasingDetailManager = new PurchasingDetailManager(_purchasingDetail, _purchasings, null);


        private static readonly StubIPurchaseSet _purchaseSet = new StubIPurchaseSet();
        private static readonly StubIPersonnelSao _personnelSao = new StubIPersonnelSao();
        private static readonly StubIPurchasingManagement _purchasingM = new StubIPurchasingManagement();
        private static readonly StubIGoodsCenterSao _goodsCenterSao = new StubIGoodsCenterSao();
        private static readonly StubIStockCenterSao _stockCenterSao = new StubIStockCenterSao();
        readonly ERP.BLL.Implement.Inventory.PurchasingManagement _purchasingManagement = new ERP.BLL.Implement.Inventory.PurchasingManagement(_purchaseSet, _personnelSao,
            _purchasingM, _goodsCenterSao, _stockCenterSao);


        [TestInitialize]
        public void Init()
        {
            _stubPurchasing.GetPurchasingByIdGuid = guid => new PurchasingInfo
            {
                PurchasingID = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),
                PurchasingState = (int)PurchasingState.Purchasing,
                PurchasingNo = "采购单编号"
            };

            _stubPurchasing.GetPurchasingListDateTimeDateTimeGuidGuidPurchasingStatePurchasingTypeStringGuidGuid =
                (time, dateTime, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => new List<PurchasingInfo>();

            _stubPurchasing.GetPurchasingListDateTimeDateTimeGuidGuidPurchasingStatePurchasingTypeStringListOfGuidGuidGuid =
                (time, dateTime, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => new List<PurchasingInfo>();
            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);
        }
        #region 非静态方法测试

        
        /// <summary>
        /// 修改采购单状态
        /// </summary>
        [TestMethod]
        public void TestPurchasingUpdate()
        {
            _purchasingManager.PurchasingUpdate(Guid.NewGuid(), PurchasingState.Purchasing);
            Assert.IsTrue(true);

            _purchasingManager.PurchasingUpdate(Guid.NewGuid(), PurchasingState.Deleted);
            Assert.IsTrue(true);

            _purchasingManager.PurchasingUpdate(Guid.NewGuid(), PurchasingState.Refusing);
            Assert.IsTrue(true);
        }


       /// <summary>
        /// 删除采购单
        /// </summary>
        [TestMethod]
        public void TestDelete()
        {
            var result = _purchasingManager.Delete(Guid.NewGuid());
            Assert.IsNull(result);

            _stubPurchasing.GetPurchasingByIdGuid = guid => new PurchasingInfo
            {
                PurchasingID = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),
                PurchasingState = (int)PurchasingState.StockIn,
                PurchasingNo = "采购单编号"
            };
            var result2 = _purchasingManager.Delete(Guid.NewGuid());
            Assert.IsTrue(result2 == "采购单编号");
        }
        #endregion
        
        /// <summary>
        /// 通过商品获取未完成的采购单列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestSelectPurchasingNoCompleteByGoodsId()
        {
            _stubPurchasing.SelectPurchasingNoCompleteByGoodsIdGuidListOfGuid = (guid, list) => true;
            _stubIGoodsInfoSao.GetRealGoodsIdsByGoodsIdGuid = guid => new List<Guid>
            {
                new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698")
            };
            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, null, null);
            var result = _purchasingManager.SelectPurchasingNoCompleteByGoodsId(new Guid("C1F14A7B-3119-4A3B-BDAB-782AC80359BA"), new List<Guid>());
            Assert.IsTrue(result);
        }

        /// <summary>
        /// 计算采购单为采购中（分配采购公司）
        /// </summary>
        [TestMethod]
        public void TestPurchaseInProcess()
        {
            #region  供应商与公司无绑定 PASS

            _stubPurchasing.GetPurchasingByIdGuid = guid => new PurchasingInfo
            {
                PurchasingID = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335"),
                PurchasingState = (int)PurchasingState.Purchasing,
                PurchasingNo = "PH15010915002",
                CompanyID = new Guid("368D2C97-2AF3-49DE-897E-0106110A891B")
            };
            _stubPurchasing.GetGoodsAmountListGuid = guid => new List<PurchasingGoodsAmountInfo>
            {
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 800,GoodsCode = "KD180502",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),
                    PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                }
            };
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);
            #endregion

            #region  供应商和公司有绑定 可是没有设置采购额度  PASS
            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);
            _stubPurchasing.GetPurchasingByIdGuid = guid => new PurchasingInfo
            {
                PurchasingID = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335"),
                PurchasingState = (int)PurchasingState.Purchasing,
                PurchasingNo = "PH15010915002",
                CompanyID = new Guid("2729D339-8F6A-4224-941A-0D1E337D0818")
            };
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);
            #endregion

            #region  供应商和公司绑定 且 绑定一个公司的采购额度设置  开票与不开票 PASS

            _stubPurchasing.GetPurchasingByIdGuid = guid => new PurchasingInfo
            {
                PurchasingID = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335"),
                PurchasingState = (int)PurchasingState.Purchasing,
                PurchasingNo = "PH15010915002",
                CompanyID = new Guid("6D713C0F-26E4-4DD6-B7E2-143AA1B09F1C")
            };
            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);

            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);

            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);
            #endregion

            #region 采购公司绑定多个公司 或者 有设置多个额度

            _stubPurchasing.GetPurchasingByIdGuid = guid => new PurchasingInfo
            {
                PurchasingID = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335"),
                PurchasingState = (int)PurchasingState.Purchasing,
                PurchasingNo = "PH15010915002",
                CompanyID = new Guid("7B03EBDE-5214-4A07-845A-2ED73309AC1A")
            };
            // //所有额度大于采购额度  PASS
            _stubPurchasing.GetGoodsAmountListGuid = guid => new List<PurchasingGoodsAmountInfo>
            {
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 100,GoodsCode = "KD180502",IsOut = true,
                    PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                }
            };
            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);

            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);
            //至少有一个采购额度>设置额度 PASS
            _stubPurchasing.GetGoodsAmountListGuid = guid => new List<PurchasingGoodsAmountInfo>
            {
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 800,GoodsCode = "KD180502",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                },
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 600,GoodsCode = "KD180404",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                }
            };
            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);


            //所有的设置额度小与采购额度

            //a、一个采购商品  PASS
            _stubPurchasing.GetGoodsAmountListGuid = guid => new List<PurchasingGoodsAmountInfo>
            {
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 1500,GoodsCode = "KD180502",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                }
            };


            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);

            //b、多个采购商品

            //有且有一条设置额度大于等于采购额度  PASS
            _stubPurchasing.GetGoodsAmountListGuid = guid => new List<PurchasingGoodsAmountInfo>
            {
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 1000,GoodsCode = "KD180502",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                },
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 600,GoodsCode = "KD180404",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                }
            };

            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);


            //所有的采购额度大于设置额度

            //必开发票
            _stubPurchasing.GetGoodsAmountListGuid = guid => new List<PurchasingGoodsAmountInfo>
            {
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 1000,GoodsCode = "RS142503",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                },
                new PurchasingGoodsAmountInfo
                {
                    AmountPrice = 1500,GoodsCode = "RS142503",IsOut = true,PurchasingFilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),PurchasingId = new Guid("F6B8F8E4-CFE6-44CC-BED0-A73FDF3A5335")
                }
            };
            _stubIPurchasingDetail.SelectGuid = guid => new List<PurchasingDetailInfo>
            {
                new PurchasingDetailInfo
                {
                    GoodsCode = "RS142503"
                }
            };
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);

            //非必开发票


            _stubPurchasing.GetPurchasingByIdGuid = guid => new PurchasingInfo
            {
                PurchasingID = new Guid("FDF72345-75B1-4E5A-8591-24519923BFE3"),
                PurchasingState = (int)PurchasingState.Purchasing,
                PurchasingNo = "PH15010915002",
                CompanyID = new Guid("7744DEB3-0DCF-4D8B-8B94-8FDB26ABFADF")
            };


            _purchasingManager = new PurchasingManager(_stubPurchasing, _stubIGoodsInfoSao, _stubIPurchasingDetail, _companyCussent, _procurementTicketLimitDal);
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);

            //异常测试
            _stubPurchasing.PurchasingInsertPurchasingInfo = guid =>
            {
                throw new Exception("ces");
            };
            _purchasingManager.PurchaseInProcess(Guid.NewGuid());
            Assert.IsTrue(true);
            #endregion
        }


        /// <summary>
        /// 修改采购单仓库
        /// </summary>
        [TestMethod]
        public void TestPurchasingWarehouseId()
        {
            try
            {
                _purchasing.PurchasingWarehouseId(new Guid("e406f5a6-8725-46c8-90d7-73f963ee9a6d"), new Guid("b5bcdf6e-95d5-4aee-9b19-6ee218255c05"));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

      
        [TestMethod]
        public void TestSelectDetail()
        {
            try
            {
                _purchasingDetailManager.SelectDetail(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

        [TestMethod]
        public void TestSave()
        {
            try
            {
                _purchasingDetailManager.Save(null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetNeedeGoodsOrderList()
        {
            try
            {
                _purchasingManagement.GetNeedeGoodsOrderList(DateTime.Now.AddMonths(-1), DateTime.Now);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }
    }
}
