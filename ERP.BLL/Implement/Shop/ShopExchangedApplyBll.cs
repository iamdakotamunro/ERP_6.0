using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IOrder;
using ERP.DAL.Interface.IShop;
using ERP.Enum;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.Model.ShopFront;
using ERP.SAL;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Shop
{
    /// <summary>
    /// 
    /// </summary>
    public class ShopExchangedApplyBll : BllInstance<ShopExchangedApplyBll>
    {
        private readonly IShopExchangedApply _shopRefund;
        private readonly IShopExchangedApplyDetail _shopApplyDetail;
        private readonly ICheckRefund _checkRefund;

        public ShopExchangedApplyBll(GlobalConfig.DB.FromType fromType)
        {
            _shopRefund = new ShopExchangedApplyDal(fromType);
            _shopApplyDetail = new ShopExchangedApplyDetailDal(fromType);
            _checkRefund = new CheckRefund(fromType);
        }

        public ShopExchangedApplyBll(IShopExchangedApply shopExchangedApply, IShopExchangedApplyDetail shopExchangedApplyDetail,
            ICheckRefund checkRefund)
        {
            _shopRefund = shopExchangedApply;
            _shopApplyDetail = shopExchangedApplyDetail;
            _checkRefund = checkRefund;
        }

        /// <summary>
        /// 添加换货申请
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <param name="applyDetailInfos"></param>
        /// <param name="msg"> </param>
        /// <returns></returns>
        public bool AddShopExchangedApply(ShopExchangedApplyInfo applyInfo,
            IList<ShopExchangedApplyDetailInfo> applyDetailInfos, out string msg)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    _shopRefund.InsertShopExchangedApply(applyInfo);
                    foreach (var applyDetailInfo in applyDetailInfos)
                    {
                        _shopApplyDetail.InsertShopExchangedApplyDetail(applyDetailInfo);
                    }
                    tran.Complete();
                }
                msg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 添加退货申请,存在则修改原单据，反之则添加
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <param name="applyDetailInfos"></param>
        /// <param name="msg"> </param>
        /// <returns></returns>
        public bool AddShopRefundApply(ShopExchangedApplyInfo applyInfo, IList<ShopApplyDetailInfo> applyDetailInfos, out string msg)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    _shopRefund.InsertShopExchangedApply(applyInfo);
                    foreach (var applyDetailInfo in applyDetailInfos)
                    {
                        _shopApplyDetail.InsertShopdApplyDetail(applyDetailInfo);
                    }
                    tran.Complete();
                }
                msg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 修改换货申请
        /// </summary>
        /// <returns></returns>
        public bool UpdateShopExchangedApply(ShopExchangedApplyInfo applyInfo,
            IList<ShopExchangedApplyDetailInfo> applyDetailInfos, out string msg)
        {
            try
            {
                var flag = false;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var row = _shopRefund.UpdateShopExchangedApply(applyInfo);
                    if (row > 0)
                    {
                        var count = _shopApplyDetail.DeleteShopExchangedApplyDetails(applyInfo.ApplyID);
                        if (count > 0)
                        {
                            foreach (var applyDetailInfo in applyDetailInfos)
                            {
                                _shopApplyDetail.InsertShopExchangedApplyDetail(applyDetailInfo);
                            }
                            tran.Complete();
                            msg = string.Empty;
                            flag = true;
                        }
                        else if (count == 0)
                        {
                            msg = "未找到相对应的换货申请明细记录";
                        }
                        else
                        {
                            msg = "原换货申请明细删除失败!";
                        }
                    }
                    else if (row == 0)
                    {
                        msg = "更新时未找到对应的换货申请";
                    }
                    else
                    {
                        msg = "更新对应的换货申请失败";
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 修改退货申请
        /// </summary>
        /// <returns></returns>
        public bool UpdateShopRefundApply(ShopExchangedApplyInfo applyInfo, IList<ShopApplyDetailInfo> applyDetailInfos, out string msg)
        {
            try
            {
                var flag = false;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var row = _shopRefund.UpdateShopExchangedApply(applyInfo);
                    if (row > 0)
                    {
                        var count = _shopApplyDetail.DeleteShopExchangedApplyDetails(applyInfo.ApplyID);
                        if (count > 0)
                        {
                            foreach (var applyDetailInfo in applyDetailInfos)
                            {
                                _shopApplyDetail.InsertShopdApplyDetail(applyDetailInfo);
                            }
                            tran.Complete();
                            msg = string.Empty;
                            flag = true;
                        }
                        else if (count == 0)
                        {
                            msg = "未找到相对应的退货申请明细记录";
                        }
                        else
                        {
                            msg = "原退货申请明细删除失败!";
                        }
                    }
                    else if (row == 0)
                    {
                        msg = "更新时未找到对应的退货申请";
                    }
                    else
                    {
                        msg = "更新对应的退货申请失败";
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 设置退换货申请状态，追加备注
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="exchangedState"></param>
        /// <param name="description"></param>
        /// <param name="msg"> </param>
        /// <returns></returns>
        public int SetShopExchangedState(Guid applyId, int exchangedState, string description, out string msg)
        {
            try
            {
                msg = string.Empty;
                var state = _shopRefund.GetExchangeState(applyId, string.Empty);
                if (exchangedState <= state && exchangedState != 0)
                {
                    msg = "退换货申请状态已改变";
                    return -1;
                }
                return _shopRefund.UpdateExchangeState(applyId, exchangedState, description);
            }
            catch (Exception e)
            {
                msg = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 获取当月的退货记录
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-03-15
        public Dictionary<string, decimal> SelectOneMonthReturnedApplyList(Guid shopId, DateTime dateTime)
        {
            return _shopRefund.SelectOneMonthReturnedApplyList(shopId, dateTime);
        }

        /// <summary>
        /// 获取退换货申请列表
        /// </summary>
        /// <param name="isBarter"></param>
        /// <param name="applyNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsIds"></param>
        /// <param name="shopId"></param>
        /// <param name="state"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        public IEnumerable<ShopExchangedApplyInfo> GetShopExchangedApplyList(bool isBarter, string applyNo,
            DateTime startTime, DateTime endTime, IList<Guid> goodsIds, Guid shopId, int state, string goodsNameOrCode)
        {
            if (endTime <= DateTime.MinValue || startTime > endTime)
            {
                endTime = DateTime.Now;
            }
            if (startTime <= DateTime.MinValue || startTime == endTime)
            {
                startTime = endTime.AddMonths(-6);
            }
            var newDate = endTime.AddDays(1);
            endTime = new DateTime(newDate.Year, newDate.Month, newDate.Day);
            return isBarter
                       ? _shopRefund.GetShopRefundApplyList(applyNo, startTime, endTime, goodsIds, shopId, state, goodsNameOrCode)
                       : _shopRefund.GetShopBarterApplyList(applyNo, startTime, endTime, goodsIds, shopId, state, goodsNameOrCode);
        }

        /// <summary>
        /// 是否成功生成退货商品检查
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="expressNo"> </param>
        /// <param name="expressName"> </param>
        /// <returns></returns>
        public bool IsSuccessCreateCheck(Guid applyId, Guid warehouseId, string expressNo, string expressName)
        {
            //退换货原商品明细
            var applyInfo = _shopRefund.GetShopExchangedApplyInfo(applyId);
            if (applyInfo == null) return false;
            var shopApplyDetailInfos = _shopApplyDetail.GetShopApplyDetailList(applyId).ToList();
            var flag = false;
            //门店传递仓库
            var info = WarehouseManager.Get(warehouseId);
            if (info == null) return false;
            if (shopApplyDetailInfos != null && shopApplyDetailInfos.Count > 0)
            {
                var checkRefund = new CheckRefundInfo
                {
                    RefundId = Guid.NewGuid(),
                    RefundNo = applyInfo.ApplyNo,
                    OrderId = applyId,
                    ExpressNo = expressNo,
                    ExpressName = expressName,
                    CreateTime = DateTime.Now,
                    CheckState = (int)CheckState.Checking,
                    Remark = string.Empty,
                    WarehouseId = warehouseId,
                    Amount = shopApplyDetailInfos.Sum(act => act.Quantity * act.Price),
                    ReStartReason = "",
                    CheckFilialeId = info.LogisticFilialeId,
                    IsTransfer = false
                };
                IList<CheckRefundDetailInfo> checkRefundDetailInfos = (from item in shopApplyDetailInfos
                                                                       select new CheckRefundDetailInfo
                                                                       {
                                                                           RefundId = checkRefund.RefundId,
                                                                           Id = item.ID,
                                                                           GoodsId = item.GoodsID,
                                                                           RealGoodsId = item.RealGoodsID,
                                                                           GoodsName = item.GoodsName,
                                                                           GoodsCode = item.GoodsCode,
                                                                           DamageCount = 0,
                                                                           Quantity = item.Quantity,
                                                                           ReturnCount = item.Quantity,
                                                                           ReturnReason = "",
                                                                           ReturnType = applyInfo.IsBarter ? 1 : 0,
                                                                           SellPrice = item.Price,
                                                                           Specification = item.Specification
                                                                       }).ToList();
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var list = _checkRefund.GetShopCheckRefundList(applyInfo.ShopID, applyId, string.Empty);
                        if (list != null && list.Count > 0)
                        {
                            #region 门店采购退货增加了修改功能，需要二次检查信息；解决方案：删除老的“售后检查表”及“售后检查表明细”的数据，重新插入新的相应的信息;需求号1970(此操作跟伟哥确认过)  zal 2016-03-23
                            var isSuccess = _checkRefund.DeleteCheckRefundInfo(list.First().RefundId);
                            if (!isSuccess)
                                return false;
                            #endregion
                        }
                        var result = _checkRefund.InsertCheckRefundAndDetailList(checkRefund, checkRefundDetailInfos);
                        if (result)
                        {
                            var row = _shopRefund.UpdateExchangeState(applyId, (int)ExchangedState.Checking, "退回商品检查");
                            if (row > 0)
                            {
                                flag = true;
                                tran.Complete();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }
    }
}
