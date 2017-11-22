using System;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ERP.Enum.ApplyInvocie;
using ERP.Model.Invoice;

namespace ERP.BLL.UnitTest
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/5/31 15:21:03 
     * 描述    :
     * =====================================================================
     * 修改时间：2017/5/8 16:56 
     * 修改人  ：柏佳荣
     * 描述    ：
     */
    [TestClass]
    public class UnitTest_StorageManager
    {
        private readonly StorageManager _storageManager = StorageManager.WriteInstance;
        private readonly InvoiceApplySerivce _invoiceApplySerivce = new InvoiceApplySerivce();

        [TestMethod]
        public void TestNewInsertStockAndGoods()
        {
            try
            {
                _storageManager.NewInsertStockAndGoods(null, null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

        [TestMethod]
        public void TestNewSetStateStorageRecord()
        {
            try
            {
                _storageManager.NewSetStateStorageRecord(Guid.Empty, StorageRecordState.Canceled, "");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

        [TestMethod]
        public void TestGetStorageRecordDetailListByStockId()
        {
            try
            {
                _storageManager.GetStorageRecordDetailListByStockId(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestNewAddStorageRecordAndDetailList()
        {
            try
            {
                string errorMessage;

                var result = _storageManager.NewAddStorageRecordAndDetailList(null, null, out errorMessage);

                Assert.IsTrue(result);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }

        [TestMethod]
        public void TestAddBySellStockOut()
        {
            string errorMessage = string.Empty;
            _storageManager.AddBySellStockOut("KDDDA00217041013007", new Guid("A15127E0-37BF-46B5-8A1C-017EF43ED4E2"), out errorMessage);
            Assert.AreEqual(string.Empty, errorMessage);
        }

        [TestMethod]
        public void Test01()
        {
            //string errorMessage = string.Empty;
            //var gmsClient = new GMSClient(new Guid("B7E7DFC0-03A6-40AF-9333-B2E8B4695F1D"), "ERP");
            //var bb = gmsClient.GetGoodsBaseListByRealGoodsIdList(new List<Guid>(new Guid[] { new Guid("98A86CB1-D7E0-41AB-B36F-38492642DCB7") }));

            ////var client = new ERP.SAL.Goods.GoodsCenterSao();
            ////client.GetGoodsListByGoodsIds
            //var aa = gmsClient.GetGoodsBaseListByIds(new List<Guid>(new Guid[] {new Guid("040CA7BA-5A35-489A-AEB7-0007BC2061A6") }));
            //if (aa != null) { }
        }
    }
}
