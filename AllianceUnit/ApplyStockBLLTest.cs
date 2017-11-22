using System.Collections;
using System.Linq;
using System.Transactions;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement.Goods;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Implement.Shop;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Enum.ShopFront;
using ERP.Model;
using ERP.Model.ShopFront;
using ERP.SAL;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model.ShopFront;

namespace AllianceUnit
{
    /// <summary>
    ///这是 ApplyStockBLLTest 的测试类，旨在
    ///包含所有 ApplyStockBLLTest 单元测试
    ///</summary>
    [TestClass()]
    public class ApplyStockBLLTest
    {
        readonly ApplyStockBLL _target = new ApplyStockBLL();
        readonly Guid _shopId = new Guid("3C77FC44-D7A9-4DAB-87C8-50031C579214");
        readonly Guid _warehouseId = new Guid("3C77FC44-D7A9-4DAB-87C8-50031C579214");
        readonly StorageManager _storageManager=new StorageManager();
        readonly GoodsManager _goodsManager=new GoodsManager();
        private readonly GoodsStockCurrent _current = new GoodsStockCurrent();
        private readonly CompanyCussentManager _companyCussent = new CompanyCussentManager();
        private readonly ShopExchangedApplyBll _exchangedApplyBll = new ShopExchangedApplyBll();

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext { get; set; }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///UpdateStockStateErp 的测试
        ///</summary>
        [TestMethod]
        public void UpdateStockStateErpTest()
        {
            var applyId = new Guid("DE5639D4-74D1-2456-AB18-75746463F8DB"); 
            int state = (int)ApplyStockState.CheckPending; 
            string errorMsg; 
            //string errorMsgExpected = string.Empty;
            bool actual = _target.UpdateStockStateErp(applyId, state, out errorMsg);
            //Assert.AreEqual(errorMsgExpected, errorMsg);
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetApplyStockList 的测试
        ///</summary>
        [TestMethod]
        public void GetApplyStockListTest()
        {
            var list = GoodsManager.GetRealGoodsIdListByGoodsNameOrCode("4544848122");
            int purchaseType = (int)PurchaseType.FromPurchase; 
            IList<int> states = new List<int>();
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue; 
            string searchKey = string.Empty;
            IList<Guid> goodsIds = list.ToList(); 
            IList<ApplyStockInfo> actual = _target.GetApplyStockList(_shopId, purchaseType, states, startTime, endTime, searchKey, goodsIds);
            Assert.IsTrue(actual.Count>0);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///Add 的测试  添加采购申请，1，需确认，2，无需
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            var applyStockInfo = new ApplyStockInfo
                                     {
                                         ApplyId = new Guid("955B8D16-E84E-436A-B08A-2210AA6C1BBB"),
                                         ParentApplyId = Guid.Empty,
                                         FilialeId = _shopId,
                                         FilialeName = "联盟店O2O",
                                         TradeCode = "04SPH1408291440",
                                         DateCreated = DateTime.Now,
                                         WarehouseId = _warehouseId,
                                         StockState = (int) ApplyStockState.Delivering,
                                         CompanyId = new Guid("06B30857-82F5-45F5-8768-79BD4211806C"),
                                         CompanyName = "可得公司(ERP)",
                                         CompanyWarehouseId = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05"),
                                         Transactor = "111202",
                                         Description = "联盟店采购申请",
                                         PurchaseType = 2
                                     };
            IList<ApplyStockDetailInfo> applyStockDetailInfoList= new List<ApplyStockDetailInfo>
                                             {
                                                 //new ApplyStockDetailInfo
                                                 //                    {
                                                 //                        ApplyId = applyStockInfo.ApplyId,
                                                 //                        GoodsId = new Guid("2BB7E671-4F33-4BD2-BC61-F95530068F6B"),
                                                 //                        GoodsName = "盘点测试13（勿动）",
                                                 //                        CompGoodsID = new Guid("83B8CD4E-C2B7-4D7E-855A-7A9825BDBB17"),
                                                 //                        Price =55,
                                                 //                        Units = "盒",
                                                 //                        Specification = "光度:平光",
                                                 //                        Quantity = 5,
                                                 //                        ShopFilialeId = applyStockInfo.FilialeId,
                                                 //                        Description = "联盟店采购"
                                                 //                    },
                                                                     new ApplyStockDetailInfo
                                                                     {
                                                                         ApplyId = applyStockInfo.ApplyId,
                                                                         GoodsId = new Guid("B6A5294B-0C79-4A59-A314-CD239F414EAC"),
                                                                         GoodsName = "博士伦季抛",
                                                                         CompGoodsID = new Guid("DF6FCC28-A9CC-4B7D-94F1-E421C088E43A"),
                                                                         Price =10,
                                                                         Units = "盒",
                                                                         Specification = "-0.50",
                                                                         Quantity = 1,
                                                                         ShopFilialeId = applyStockInfo.FilialeId,
                                                                         Description = "联盟店采购"
                                                                     }
                                             };
            if (applyStockDetailInfoList.Count > 0)
            {
                applyStockInfo.SubtotalQuantity = applyStockDetailInfoList.Sum(act => act.Quantity);
                string msg;
                int actual = _target.Add(applyStockInfo, applyStockDetailInfoList, out msg);
                Assert.IsTrue(actual > 4);
            }
            
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetApplyStockGoodsCount 的测试
        ///</summary>
        [TestMethod()]
        public void GetApplyStockGoodsCountTest()
        {
            Guid goodsId = new Guid("57A29FCD-C12B-4BF4-BCA6-74C1C427AADA"); 
            DateTime startTime = DateTime.Now.AddMonths(-6); 
            DateTime endTime = DateTime.Now; 
            //IList<int> states = null; 
            int actual = _target.GetApplyStockGoodsCount(_shopId, goodsId, startTime, endTime, null);
            Assert.AreEqual(1,actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetPurchaseGoodsQuantity 的测试
        ///</summary>
        [TestMethod()]
        public void GetPurchaseGoodsQuantityTest()
        {
            DateTime startTime =DateTime.MinValue; 
            DateTime endTime =DateTime.MinValue; 
            IList<int> states = new List<int>(); 
            Dictionary<Guid, Dictionary<Guid, int>> actual = _target.GetPurchaseGoodsQuantity(_shopId, startTime, endTime, states);
            Assert.IsTrue(actual!=null && actual.Count>0);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///联盟店采购流程测试 的测试
        ///</summary>
        [TestMethod()]
        public void ShopPurchaseTest()
        {
            //第一步 添加采购申请  

            //第二步  修改采购状态(确认是否符合发货标准)

            //第三步  生成调拨出库

            //第四步  出库审核 填写快递物流

            //第五步  对方完成收货 出入库一致  交易完成
            //        数量不一致  采购审核  修改出入库单据
        }

        /// <summary>
        ///通过商品名称编号查询商品信息 测试
        ///</summary>
        [TestMethod()]
        public void GetGoodsInfoTest()
        {
            //var list = GoodsManager.GetRealGoodsIdListByGoodsNameOrCode("4544848122");
            //var goodsInfo = GoodsManager.GetGoodsBaseInfoByGoodsCode("盘点测试01（勿动）");
            //var goodsInfo1 = GoodsManager.GetGoodsBaseInfoByGoodsCode("4544848122");

            var shopFilialeList = FilialeManager.GetAllianceFilialeList();
            var shopList=FilialeManager.GetEntityShop();
            var info=shopFilialeList.Where(act => act.Rank == (int)MIS.Enum.FilialeRank.Partial).ToDictionary(k => k.ID, v => v.Name); 
            Assert.IsTrue(true);
        }

        /// <summary>
        ///UpdateAddApplyStockExecption 的测试  异常调拨修改
        ///</summary>
        [TestMethod]
        public void UpdateAddApplyStockExecptionTest()
        {
            string outstockno = "TSO04082116001";
            var errorMessage = new Dictionary<Guid, string>
                                                        {
                                                            {new Guid("57A29FCD-C12B-4BF4-BCA6-74C1C427AADA"),"少发" }
                                                        }; 
            bool actual = _target.UpdateAddApplyStockExecption(outstockno, errorMessage);
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        /// 出库审核单元测试 采购发货
        /// </summary>
        [TestMethod]
        public void SemistockOutAudit()
        {
            var flag = IsAudit();
            Assert.IsTrue(flag);
        }


        public bool IsAudit()
        {
            var stockId = new Guid("208CA9A1-7458-420A-93C4-5A4CE1179D81");
            StorageRecordInfo semiStock = _storageManager.GetStorageRecord(stockId);
            if (semiStock == null)
            {
                return false;
            }
            else
            {
                if (semiStock.StockState != (int)StorageState.Normal)
                {
                    return false;
                }
                IList<StorageRecordDetailInfo> goodsStockList = _storageManager.GetStorageRecordDetailListByStockId(stockId);
                List<Guid> goodsIdOrRealGoodsIdList = goodsStockList.Select(w => w.RealGoodsId).Distinct().ToList();
                Dictionary<Guid, ERP.Model.Goods.GoodsInfo> dicGoods = _goodsManager.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdOrRealGoodsIdList);
                if (dicGoods == null)
                {
                    return false;
                }
                if (goodsStockList.Select(gsi => dicGoods.ContainsKey(gsi.RealGoodsId)).Any(hasKey => hasKey == false))
                {
                    return false;
                }
                var shopFilialeInfo = CacheCollection.Filiale.Get(semiStock.RelevanceFilialeId);
                if (shopFilialeInfo != null)
                {
                    #region --> 检查加盟价
                    var msg = string.Empty;
                    foreach (StorageRecordDetailInfo storageDetailInfo in goodsStockList)
                    {
                        var goodsBaseInfo = new ERP.Model.Goods.GoodsInfo();
                        if (dicGoods.Count > 0)
                        {
                            bool hasKey = dicGoods.ContainsKey(storageDetailInfo.RealGoodsId);
                            if (hasKey)
                            {
                                goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == storageDetailInfo.RealGoodsId).Value;
                            }
                        }
                        if (goodsBaseInfo != null && goodsBaseInfo.ExpandInfo != null)
                        {
                            if (goodsBaseInfo.ExpandInfo.JoinPrice == 0)
                            {
                                if (!msg.Contains(storageDetailInfo.GoodsCode))
                                    msg += "[" + storageDetailInfo.GoodsCode + "]" + storageDetailInfo.GoodsName + "\n";
                            }
                        }
                        else
                        {
                            if (!msg.Contains(storageDetailInfo.GoodsCode))
                                msg += "[" + storageDetailInfo.GoodsCode + "]" + storageDetailInfo.GoodsName + "\n";
                        }
                    }
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return false;
                    }
                    #endregion
                }
                var goodsInfoList = new List<ERP.Model.Goods.GoodsInfo>();
                foreach (var keyValuePair in dicGoods)
                {
                    if (goodsInfoList.Any(w => w.GoodsId == keyValuePair.Value.GoodsId) == false)
                        goodsInfoList.Add(keyValuePair.Value);
                }
                string errorMsg;
                return IsSuccess(stockId, goodsStockList, semiStock, dicGoods, goodsInfoList, out errorMsg);
            }
        }


        /// <summary>
        /// 联盟店出库审核
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="goodsStockList"></param>
        /// <param name="semiStock"></param>
        /// <param name="dicGoods"></param>
        /// <param name="goodsInfoList"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool IsSuccess(Guid stockId, IList<StorageRecordDetailInfo> goodsStockList, StorageRecordInfo semiStock,
            Dictionary<Guid, ERP.Model.Goods.GoodsInfo> dicGoods, List<ERP.Model.Goods.GoodsInfo> goodsInfoList, out string errorMsg)
        {
            var isSuccess = false;
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    foreach (StorageRecordDetailInfo gsi in goodsStockList)
                    {
                        gsi.Quantity = -gsi.Quantity;

                        _current.AddToStockRequirementDetail(semiStock, new[] { gsi },
                                                            "调拨出库-[编辑]",
                                                            "操作人：111202" ,
                                                            -Math.Abs(gsi.Quantity));
                    }
                    string description = "[受理通过] 111202" 
                        + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
                    var stockInfo = semiStock;
                    stockInfo.DateCreated = DateTime.Now;
                    stockInfo.StockType = (int)StorageType.TransferStockOut;
                    stockInfo.Transactor = "111202";
                    _storageManager.Insert(stockInfo, goodsStockList, semiStock.TradeCode, dicGoods);
                    _storageManager.SetStateStorageRecord(stockId, StorageState.Pass, description);

                    //更新结算价
                    foreach (var goodsInfo in goodsInfoList)
                    {
                        _storageManager.UpdateStorageRecordDetailToSettlementPrice(stockId, goodsInfo.GoodsId,
                            goodsInfo.ExpandInfo.SettlementPrice, goodsInfo.ExpandInfo.JoinPrice);
                    }

                    //同步新增门店的入库申请
                    ApplyStockInfo shopApplyStockInfo = null;
                    ShopExchangedApplyInfo exchangedApplyInfo = null;
                    if (semiStock.OriginalCode.Contains("HH-SH")) //换货出库
                    {
                        exchangedApplyInfo =
                            _exchangedApplyBll.GetShopExchangedApplyInfoByApplyNo(semiStock.OriginalCode);
                    }
                    else  //采购出库
                    {
                        shopApplyStockInfo = _target.GetApplyInfoByTradeCode(semiStock.OriginalCode);
                    }

                    if (shopApplyStockInfo != null || exchangedApplyInfo != null)
                    {
                        if (shopApplyStockInfo != null)
                        {
                            //修改采购申请状态
                            isSuccess = _target.UpdateApplyStockState(shopApplyStockInfo.ApplyId, (int)ApplyStockState.Finishing,
                                                                 false, out errorMsg);
                        }
                        else
                        {
                            isSuccess = _exchangedApplyBll.SetShopExchangedState(exchangedApplyInfo.ApplyID,
                                                                         (byte)ExchangedState.Bartering, "换货出库",
                                                                         out errorMsg) > 0;
                        }
                        if (isSuccess)
                        {

                            //提交事务操作
                            ts.Complete();
                        }
                    }
                    else
                    {
                        errorMsg = "找不到与之相对应的采购/换货申请记录";
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = string.Format("审核失败！{0}", ex.Message);
                }
            }
            return isSuccess;
        }

        /// <summary>
        ///UpdateApplyStockState 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateApplyStockStateTest()
        {
            var applyId = new Guid("842B65D2-3892-43DA-8BE5-99B0F8F0E68D"); 
            string msg; 
            bool actual = _target.UpdateApplyStockState(applyId, (int)ApplyStockState.Delivering, false, out msg);
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void UpdateStorageRecord()
        {
            var flag = false;
            var stockList = _storageManager.GetStorageRecordListByOriginalCode("04SPH1408270940");
            var stockInfo = stockList.FirstOrDefault(act => act.StockState == (int)StorageState.Pass);
            if (stockInfo != null && stockInfo.StockId != Guid.Empty)
            {
                var stockDtailList = _storageManager.GetStorageRecordDetailListByStockId(stockInfo.StockId);
                //联盟店生成调拨出库 异常差额  已审核
                var allianceOutStorage = new StockDTO()
                                             {
                                                 CompanyID =new Guid(""),
                                                 DateCreated = DateTime.Now,
                                                 Description = "调拨异常差额出库",
                                                 OriginalTradeCode = ""
                                             };  
                var allianceOutStorageDetails = new List<StockDetailDTO>();
                //ERP生成调拨入库 异常差额  已审核
                var erpInStoreageInfo = new StorageRecordInfo();
                var erpInStorageDetails = new List<StorageRecordDetailInfo>();
                foreach (var storageRecordDetailInfo in stockDtailList)
                {
                    storageRecordDetailInfo.Quantity = storageRecordDetailInfo.Quantity + 1;
                    var detail = storageRecordDetailInfo.DeepCopy() as StorageRecordDetailInfo;
                    var detailDto = new StockDetailDTO();
                    erpInStorageDetails.Add(detail);
                    allianceOutStorageDetails.Add(detailDto);
                }
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    //修改原调拨出库单据
                    string msg;
                    _storageManager.InsertStockAndGoods(erpInStoreageInfo,erpInStorageDetails);
                    ShopSao.InsertStock(allianceOutStorage.StockID, allianceOutStorage, allianceOutStorageDetails,false,null,0,
                                        out msg);
                    var result = _target.UpdateApplyStockState(new Guid("842B65D2-3892-43DA-8BE5-99B0F8F0E68D"), (int)ApplyStockState.Pended, false, out msg);
                    if(result)
                    {
                        ts.Complete();
                    }
                }
            }
            Assert.IsTrue(flag);
        }

        /// <summary>
        ///IsAllowPurchase 的测试
        ///</summary>
        [TestMethod()]
        public void IsAllowPurchaseTest()
        {
            var goodsId = new Guid("57A29FCD-C12B-4BF4-BCA6-74C1C427AADA"); 
            bool actual = _target.IsAllowPurchase(_shopId, goodsId);
            Assert.IsTrue(actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AllianceServiceTest()
        {
            string msg;
            var list = ShopSao.GetExpressCodeList(new Guid("B6B39773-E76E-4A53-9AAC-634E7DF973EA"), out msg);
            Assert.IsTrue(string.IsNullOrEmpty(msg));
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetParentId()
        {
            var parentId = FilialeManager.GetShopHeadFilialeId(new Guid("3C77FC44-D7A9-4DAB-87C8-50031C579214"));
            var model = new Guid("B6B39773-E76E-4A53-9AAC-634E7DF973EA");
            Assert.AreEqual(model,parentId);
        }

        [TestMethod]
        public void IsExistGoods()
        {
            var dics = new GoodsManager().GetGoodsSelectList("博士伦季抛");
            //var info=GoodsManager.GetGoodsBaseInfoListByClassId(Guid.Empty, "瞳瑶蓝黑色彩色隐形眼镜-301");
            Assert.IsTrue(dics!=null && dics.Count>0);
        }

        /// <summary>
        ///ShopConfirmPurchaseOrder 的测试
        ///</summary>
        [TestMethod()]
        public void ShopConfirmPurchaseOrderTest()
        {
            var applyId = new Guid(""); 
            StorageRecordInfo storageRecord = null; 
            IList<StorageRecordDetailInfo> storageRecordDetail = null; 
            ApplyStockInfo applyStock = null; 
            IList<ApplyStockDetailInfo> stockDetailInfos = null; 
            string msg; 
            int actual = _target.ShopConfirmPurchaseOrder(applyId, storageRecord, storageRecordDetail, applyStock, stockDetailInfos, out msg);
            Assert.IsNull(actual>=0);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetConfirmErrorMsg 的测试
        ///</summary>
        [TestMethod]
        public void GetConfirmErrorMsgTest()
        {
            //A001PH14090515009
            ApplyStockInfo applyStockInfo = _target.GetApplyInfoByTradeCode("A001PH14090515004"); 
            IList<ApplyStockDetailInfo> applyStockDetailInfoList = _target.FindDetailList(applyStockInfo.ApplyId); 
            Dictionary<Guid, string> actual = _target.GetConfirmErrorMsg(applyStockInfo, applyStockDetailInfoList);
            Assert.IsTrue(actual!=null && actual.Count>0);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
