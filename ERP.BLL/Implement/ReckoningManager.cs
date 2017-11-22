using System;
using System.Collections.Generic;
using System.Linq;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement.Basis;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Goods;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IGoods;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using Keede.Ecsoft.Model;
using ERP.Enum;
using ERP.Model;
using ERP.SAL;
using GoodsOrderDetail = ERP.DAL.Implement.Order.GoodsOrderDetail;

namespace ERP.BLL.Implement
{
    /// <summary>▄︻┻┳═一 异步往来账业务层  最后修改提交  陈重文  2014-12-25  （全局优化更新）
    /// </summary>
    public class ReckoningManager : BllInstance<ReckoningManager>
    {
        private readonly CodeManager _codeManager;
        readonly IReckoning _reckoningDao;
        private readonly ICompanyCussent _companyCussent;
        private readonly IGoodsOrder _goodsOrder;
        private readonly IGoodsOrderDetail _goodsOrderDetail;
        readonly IGoodsOrderDeliver _goodsOrderDeliver;
        readonly IStorageRecordDao _storageRecordDao;

        private static readonly WMSSao _wmsSao = new WMSSao();

        public ReckoningManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _companyCussent = new CompanyCussent(fromType);
            _reckoningDao = InventoryInstance.GetReckoningDao(fromType);
            _goodsOrderDeliver = new GoodsOrderDeliver(fromType);
            _goodsOrder = new DAL.Implement.Order.GoodsOrder(fromType);
            _goodsOrderDetail = new GoodsOrderDetail(fromType);
            _codeManager = new CodeManager();
            _storageRecordDao= new StorageRecordDao(fromType);
        }

        public ReckoningManager(IReckoning reckoning, ICompanyCussent companyCussent,
            IGoodsOrder goodsOrder, IGoodsOrderDetail goodsOrderDetail, IGoodsOrderDeliver goodsOrderDeliver,
            ICode code)
        {
            _reckoningDao = reckoning;
            _companyCussent = companyCussent;
            _goodsOrder = goodsOrder;
            _goodsOrderDetail = goodsOrderDetail;
            _goodsOrderDeliver = goodsOrderDeliver;
            _codeManager = new CodeManager(code);
        }

        #region [生成对应往来账]

        /// <summary>异步完成订单生成往来账
        /// </summary>
        /// <param name="orderInfo">订单</param>
        /// <param name="orderDetailList">订单明细</param>
        /// <param name="errorMessage">异常信息</param>
        /// <returns></returns>
        public bool AddByCompleteOrder(GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> orderDetailList, out string errorMessage)
        {
            GoodsOrderDeliverInfo goodsOrderDeliverInfo;
            var reckoningList = NewCreateReckoningInfoList(orderInfo, orderDetailList, out goodsOrderDeliverInfo, out errorMessage).ToList();
            if (reckoningList.Count == 0)
            {
                errorMessage = "此订单无需生成往来帐！";
                return true;
            }

            using (var tran = new System.Transactions.TransactionScope())
            {
                //增加往来帐
                foreach (var reckoning in reckoningList)
                {
                    if (FilialeManager.IsEntityShopFiliale(reckoning.FilialeId))
                    {
                        if (!AddReckoningToEntityShop(reckoning, out errorMessage))
                        {
                            errorMessage = "插入门店往来帐失败，" + errorMessage + "\r\n数据：" + new Framework.Core.Serialize.JsonSerializer().Serialize(reckoning);
                            return false;
                        }
                    }
                    else
                    {
                        var success = _reckoningDao.Insert(reckoning, out errorMessage);
                        if (!success)
                        {
                            errorMessage = "插入往来帐失败，" + errorMessage + "\r\n数据：" + new Framework.Core.Serialize.JsonSerializer().Serialize(reckoning);
                            return false;
                        }
                    }
                    
                }
                if (goodsOrderDeliverInfo != null)
                {
                    _goodsOrderDeliver.DeleteOrderDeliver(goodsOrderDeliverInfo.OrderId);
                    var result = _goodsOrderDeliver.InsertOrderDeliver(goodsOrderDeliverInfo);
                    if (!result)
                    {
                        errorMessage = "插入订单快递运费信息失败，" + errorMessage + "\r\n数据：" + new Framework.Core.Serialize.JsonSerializer().Serialize(goodsOrderDeliverInfo);
                        return false;
                    }
                }
                tran.Complete();
                return true;
            }
        }

        /// <summary>添加往来帐到实体店系统中
        /// </summary>
        /// <param name="reckoningInfo"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool AddReckoningToEntityShop(ReckoningInfo reckoningInfo, out string errorMsg)
        {
            errorMsg = string.Empty;
            var info = new ReckoningRecordDTO
            {
                AccountReceivable = reckoningInfo.AccountReceivable,
                JoinTotalPrice = reckoningInfo.JoinTotalPrice,
                AuditingState = true,
                CompanyID = reckoningInfo.ThirdCompanyID,
                DateCreated = reckoningInfo.DateCreated,
                Description = reckoningInfo.Description,
                ShopID = reckoningInfo.FilialeId,
                NonceTotalled = reckoningInfo.NonceTotalled,
                OriginalTradeCode = reckoningInfo.LinkTradeCode,
                ID = reckoningInfo.ReckoningId,
                ReckoningType = reckoningInfo.ReckoningType,
                TradeCode = reckoningInfo.TradeCode
            };
            if (info.CompanyID == info.ShopID)
            {
                errorMsg = ("往来公司ID和账目公司ID一样，故无法插入往来帐！");
                return false;
            }
            if (!FilialeManager.IsEntityShopFiliale(info.ShopID))
            {
                errorMsg = ("往来公司ID不是门店公司，故无法插入往来帐！");
                return false;
            }
            var headFililaleId = FilialeManager.GetShopHeadFilialeId(info.ShopID);
            var result = PushManager.AddToShop(headFililaleId, "InsertReckoningWithPush", info.TradeCode, info);
            if (!result)
            {
                errorMsg = ("插入门店对ERP应付帐失败！");
                return false;
            }
            return true;
        }
        #endregion

        #region [完成订单创建所需往来账集合]

        /// <summary>新架构完成订单创建所需往来账   2015-01-19  陈重文  
        /// </summary>
        /// <param name="goodsOrderInfo">订单信息</param>
        /// <param name="goodsOrderDetailInfoList">订单明细</param>
        /// <param name="goodsOrderDeliverInfo">订单运费信息</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        public IEnumerable<ReckoningInfo> NewCreateReckoningInfoList(GoodsOrderInfo goodsOrderInfo, IList<GoodsOrderDetailInfo> goodsOrderDetailInfoList,out GoodsOrderDeliverInfo goodsOrderDeliverInfo, out string errorMsg)
        {
            if (goodsOrderInfo.HostingFilialeId == Guid.Empty)
            {
                goodsOrderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(goodsOrderInfo.DeliverWarehouseId, goodsOrderInfo.SaleFilialeId, goodsOrderDetailInfoList.Select(ent => ent.GoodsType).Distinct());
            }
            //所需添加的往来帐集合
            IList<ReckoningInfo> reckoningList = new List<ReckoningInfo>();
            goodsOrderDeliverInfo = null;
            var orderCarriageInfo = _wmsSao.GetOrderNoCarriage(goodsOrderInfo.OrderNo, out errorMsg);
            if (orderCarriageInfo == null)
            {
                return reckoningList;
            }
            var carriage = orderCarriageInfo.Carriage;
            if (carriage != 0)
            {
                goodsOrderDeliverInfo = new GoodsOrderDeliverInfo
                {
                    OrderId = goodsOrderInfo.OrderId,
                    TotalWeight = orderCarriageInfo.PackageWeight == 0 ? 0 : Convert.ToDouble(orderCarriageInfo.PackageWeight) / 1000,
                    CarriageFee = Convert.ToDouble(orderCarriageInfo.Carriage),
                    ExpressId = goodsOrderInfo.ExpressId,
                    ExpressNo = goodsOrderInfo.ExpressNo,
                    MaxWrongValue = 0,
                    ProvinceName = orderCarriageInfo.Province,
                    CityName = orderCarriageInfo.City
                };
            }

            #region [检查快递往来单位信息]

            Guid companyId = Express.Instance.Get(goodsOrderInfo.ExpressId).CompanyId;
            CompanyCussentInfo expressCompanyInfo = _companyCussent.GetCompanyCussent(companyId);
            if (expressCompanyInfo == null)
            {
                errorMsg = "快递公司的往来对账信息没有建立！";
                return new List<ReckoningInfo>();
            }

            #endregion

            //获取销售公司名称
            string saleFilialeName = FilialeManager.GetName(goodsOrderInfo.SaleFilialeId);
            string hostingFilialeName = FilialeManager.GetName(goodsOrderInfo.HostingFilialeId);
            #region [运费往来帐]
            if (carriage > 0)
            {
                #region [销售公司对快递公司应付快递运费帐]
                //销售公司对快递公司的应付快递运费帐
                var saleFilialeToCarriage = new ReckoningInfo
                {
                    ContructType = ContructType.Insert,
                    ReckoningId = Guid.NewGuid(),
                    TradeCode = _codeManager.GetCode(CodeType.PY),
                    DateCreated = DateTime.Now,
                    ReckoningType = (int)ReckoningType.Defray,
                    State = (int)ReckoningStateType.Currently,
                    IsChecked = (int)CheckType.NotCheck,
                    AuditingState = (int)AuditingState.Yes,
                    LinkTradeCode = goodsOrderInfo.ExpressNo,
                    WarehouseId = goodsOrderInfo.DeliverWarehouseId,
                    FilialeId = goodsOrderInfo.HostingFilialeId,
                    ThirdCompanyID = expressCompanyInfo.CompanyId,
                    Description = string.Format("[完成订单,{0}对快递公司{1}运费应付款]", hostingFilialeName, expressCompanyInfo.CompanyName),
                    AccountReceivable = -carriage,
                    JoinTotalPrice = -carriage,
                    ReckoningCheckType = (int)ReckoningCheckType.Carriage,
                    IsOut = goodsOrderInfo.IsOut,
                    LinkTradeType = (int)ReckoningLinkTradeType.Express,
                };
                reckoningList.Add(saleFilialeToCarriage);
                #endregion
            }
            #endregion

            #region 销售公司对快递公司的订单代收帐

            if (goodsOrderInfo.PayMode == (int)PayMode.COD || goodsOrderInfo.PayMode == (int)PayMode.COM)
            {
                #region [销售公司对快递公司的订单代收帐]

                //销售公司对快递公司的订单代收帐
                var saleFilialeToRealTotalPrice = new ReckoningInfo
                {
                    ContructType = ContructType.Insert,
                    ReckoningId = Guid.NewGuid(),
                    TradeCode = _codeManager.GetCode(CodeType.GT),
                    DateCreated = DateTime.Now,
                    ReckoningType = (int)ReckoningType.Income,
                    State = (int)ReckoningStateType.Currently,
                    IsChecked = (int)CheckType.NotCheck,
                    AuditingState = (int)AuditingState.Yes,
                    LinkTradeCode = goodsOrderInfo.ExpressNo,
                    WarehouseId = goodsOrderInfo.DeliverWarehouseId,
                    FilialeId = goodsOrderInfo.SaleFilialeId,
                    ThirdCompanyID = expressCompanyInfo.CompanyId,
                    Description = string.Format("[完成订单,{0}对快递公司{1}的订单应收货款]", saleFilialeName, expressCompanyInfo.CompanyName),
                    AccountReceivable = WebRudder.ReadInstance.CurrencyValue(goodsOrderInfo.RealTotalPrice),
                    ReckoningCheckType = (int)ReckoningCheckType.Collection,
                    IsOut = goodsOrderInfo.IsOut,
                    LinkTradeType = (int)ReckoningLinkTradeType.GoodsOrder,
                };

                reckoningList.Add(saleFilialeToRealTotalPrice);
                #endregion
            }
            #endregion

            return reckoningList;
        }

        #endregion

        #region [异步往来账操作]

        /// <summary>执行异步添加往来账
        /// </summary>
        /// <param name="readCount"></param>
        public void RunAsynAddTask(int readCount)
        {
            var asynList = _reckoningDao.GetAsynList(readCount);
            foreach (var asynInfo in asynList)
            {
                if (asynInfo.ReckoningFromType == Enum.ASYN.ASYNReckoningFromType.CompleteOrder.ToString())
                {
                    using (var tran = new System.Transactions.TransactionScope())
                    {
                        var orderInfo = _goodsOrder.GetGoodsOrder(asynInfo.IdentifyId);
                        if (orderInfo == null) continue;
                        var orderDetailList = _goodsOrderDetail.GetGoodsOrderDetailByOrderId(asynInfo.IdentifyId);
                        string errorMessage;
                        var success = AddByCompleteOrder(orderInfo, orderDetailList, out errorMessage);
                        if (!success)
                        {
                            SAL.LogCenter.LogService.LogError(string.Format("异步添加往来帐失败! IdentifyId={0} {1}", asynInfo.IdentifyId, errorMessage), "往来账管理");
                            continue;
                        }
                        var successDel = _reckoningDao.DeleteAsyn(asynInfo.ID);
                        if (!successDel)
                        {
                            SAL.LogCenter.LogService.LogError(string.Format("删除异步往来帐数据失败! ID={0}", asynInfo.ID), "往来账管理");
                            continue;
                        }
                        tran.Complete();
                    }
                }
            }
        }
        #endregion
    }
}