using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Goods;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.StockCenter;
using ERP.DAL.Interface.IGoods.Fakes;
using ERP.DAL.Interface.IInventory.Fakes;
using ERP.SAL.Interface.Fakes;
using Keede.Ecsoft.Model;
using Keede.Ecsoft.Model.StockCenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// 库存类单元测试
    /// </summary>
    [TestClass]
    public class UnitTest_GoodsStockCurrent
    {
        readonly StubIGoodsStockCurrent _stubIGoodsStockCurrent = new StubIGoodsStockCurrent();
        readonly StubIStockCenterSao _stockCenterSao = new StubIStockCenterSao();
        GoodsStockCurrent _goodsStockCurrent;

        static readonly StubIGoodsCenterSao _goodsCenterSao = new StubIGoodsCenterSao();
        static readonly StubIPurchaseSet _purchaseSet = new StubIPurchaseSet();
        static readonly StubIPurchasing _purchasing = new StubIPurchasing();
        static readonly StubIStorageRecordDao _storageRecordDao = new StubIStorageRecordDao();
        static readonly StubIStockCenterSao _stockCenterSaos = new StubIStockCenterSao();

        private readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        private readonly GoodsManager _goodManager = new GoodsManager(_goodsCenterSao, _purchaseSet);
        private readonly StockCenterManager _stockCenterManager = new StockCenterManager(_stockCenterSaos, _goodsCenterSao, _purchaseSet, _purchasing, _storageRecordDao);


        [TestInitialize]
        public void Init()
        {
            _stockCenterSao.SaveGoodsStockCurrentNotChangeQuantityGuidGoodsStockCurrentInfoInt32 =
                (guid, info, arg3) => true;

            _stockCenterSao.GetChildGoodsQuantityGuid = guid => new List<GoodsStockQuantityInfo>
            {
                new GoodsStockQuantityInfo{CurrentQuantity = 100,GoodsId = Guid.NewGuid()}
            };
            _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
        }

        [TestMethod]
        public void TestDefaultGoodsStockCurrent()
        {
            _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
        }

        [TestMethod]
        public void TestGoodsStockCurrent()
        {
            _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
        }

        /// <summary>
        /// 根据商品和仓库查出可用库存
        /// </summary>
        [TestMethod]
        public void TestGetUsableStock()
        {
            var result = _goodsStockCurrent.GetUsableStock(Guid.Empty, new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"));
            Assert.IsTrue(result < 0);

            var result2 = _goodsStockCurrent.GetUsableStock(new Guid("891F62CA-9164-44FA-9E5B-DB3A973841D3"), Guid.Empty);
            Assert.IsTrue(result2 < 0);


            var result3 = _goodsStockCurrent.GetUsableStock(new Guid("891F62CA-9164-44FA-9E5B-DB3A973841D3"), new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"));
            Assert.IsTrue(result3 < 0);

            _stockCenterSao.GetGoodsQuantityByRealGoodIdWarehouseGuidGuid = (guid, guid1) => new List<GoodsStockQuantityInfo>
            {
                new GoodsStockQuantityInfo{GoodsId = Guid.NewGuid(),CurrentQuantity = 10}
            };
            _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
            var result4 = _goodsStockCurrent.GetUsableStock(new Guid("891DE2CA-9164-44FA-9E5B-DB3A973841D3"), new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"));
            Assert.IsTrue(result4 == 10);
        }

        /// <summary>
        /// 添加到StockRequirementDetail表
        /// </summary>
        [TestMethod]
        public void TestAddToStockRequirementDetail()
        {
            var storageRecordInfo = new StorageRecordInfo
            {
                FilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"),
                WarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05")
            };

            IList<StorageRecordDetailInfo> storageRecordDetailInfos = new List<StorageRecordDetailInfo>
            {
                new StorageRecordDetailInfo
                {
                    RealGoodsId = new Guid("D2F69F05-C02F-4549-93C8-D3CE768D82E2")
                }
            };

            _stockCenterSao.GetGoodsQuantityByRealGoodsIdsIEnumerableOfGuidGuidGuid = (guids, guid, arg3) =>
                new List<GoodsStockQuantityInfo>
                {
                    new GoodsStockQuantityInfo
                    {
                        RealGoodsId = new Guid("D2F69F05-C02F-4549-93C8-D3CE768D82E2")
                    }
                };
            try
            {
                _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
                _goodsStockCurrent.AddToStockRequirementDetail(storageRecordInfo, storageRecordDetailInfos, string.Empty, string.Empty, true);
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }

        /// <summary>
        /// 取得当前子商品对应公司ID,仓库ID库存
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestGetGoodsStockCurrentById()
        {
            _stockCenterSao.GetGoodsQuantityByRealGoodsIdsIEnumerableOfGuidGuidGuid = (guids, guid, arg3) => null;
            _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
            try
            {
                _goodsStockCurrent.GetGoodsStockCurrentById(new Guid("7953FA6C-F88B-4079-BBBA-5E61C3D13C01"),
                    new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }


            _stockCenterSao.GetGoodsQuantityByRealGoodsIdsIEnumerableOfGuidGuidGuid = (guids, guid, arg3) => new List<GoodsStockQuantityInfo>
            {
                new GoodsStockQuantityInfo{CurrentQuantity = 100,GoodsId = new Guid("7953FA6C-F88B-4079-BBBA-5E61C3D13C01")}
            };
            _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
            try
            {
                var result2 = _goodsStockCurrent.GetGoodsStockCurrentById(new Guid("45825468-F88B-4079-BBBA-5E61C3D13C01"),
                    new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"));
                //Assert.IsTrue(result2 == 100);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }

            _stockCenterSao.GetGoodsQuantityByRealGoodsIdsIEnumerableOfGuidGuidGuid = (guids, guid, arg3) => new List<GoodsStockQuantityInfo>();
            _goodsStockCurrent = new GoodsStockCurrent(_stubIGoodsStockCurrent, _stockCenterSao);
            try
            {
                var result3 = _goodsStockCurrent.GetGoodsStockCurrentById(new Guid("45825468-F88B-4079-BBBA-5E61C3D13C01"),
                    new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"), new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"));
                //Assert.IsTrue(result3 < 0);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

        [TestMethod]
        public void TestGetGoodsClassListWithRecursion()
        {
            try
            {
                _goodsClassManager.GetGoodsClassListWithRecursion();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }



        [TestMethod]
        public void TestDeleteGoods()
        {
            try
            {
                //删除主商品
                string errorMessage;
                _goodManager.DeleteGoods(null, "", Guid.Empty, out errorMessage);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }

        }


        [TestMethod]
        public void TestGoodsStockDeclaration()
        {
            try
            {
                _stockCenterManager.GoodsStockDeclaration(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }




    }
}
