using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using Keede.Ecsoft.Model;
using ERP.Model;
using ERP.SAL.MemberCenterSAL;

namespace ERP.BLL.Implement.Order
{
    public class GoodsOrder : BllInstance<GoodsOrder>
    {
        readonly IGoodsOrder _goodsOrderDao;
        readonly IGoodsOrderDetail _goodsOrderDetailDao;
        readonly IInvoice _invoiceDao;

        public GoodsOrder(GlobalConfig.DB.FromType fromType)
        {
            _goodsOrderDao = new DAL.Implement.Order.GoodsOrder(GlobalConfig.DB.FromType.Write);
            _goodsOrderDetailDao = OrderInstance.GetGoodsOrderDetailDao(fromType);
            _invoiceDao = InventoryInstance.GetInvoiceDao(fromType);
        }

        /// <summary>订单导入
        /// </summary>
        public bool AddOrderAndInvoice(GoodsOrderInfo goodsOrder, IList<GoodsOrderDetailInfo> goodsOrderDetailList, InvoiceInfo invoiceInfo, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (goodsOrderDetailList.Count == 0)
            {
                errorMessage = "订单明细没有数据！";
                return false;
            }
            Boolean isEdit = false;

            #region 验证B2C订单信息  (订单目前是走推送服务)
            var filialeList = FilialeManager.GetB2CFilialeList();
            if (filialeList.Any(act => act.ID == goodsOrder.SaleFilialeId))
            {
                try
                {
                    var info = OrderSao.GetGoodsOrderInfo(goodsOrder.SaleFilialeId, goodsOrder.OrderId);
                    if (info == null)
                    {
                        errorMessage = "获取B2C订单信息不存在";
                        return false;
                    }
                    if (info.OrderState == (int)OrderState.Cancellation)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "获取B2C订单信息异常" + ex.Message;
                    throw ex;
                }
            }
            #endregion

            var localGoodsOrderInfo = _goodsOrderDao.GetGoodsOrder(goodsOrder.OrderId);
            goodsOrder.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(goodsOrder.DeliverWarehouseId, goodsOrder.SaleFilialeId, goodsOrderDetailList.Select(ent => ent.GoodsType).Distinct());
            IList<GoodsOrderDetailInfo> localGoodsOrderDetails = new List<GoodsOrderDetailInfo>();
            if (localGoodsOrderInfo != null && localGoodsOrderInfo.OrderId != Guid.Empty)
            {
                isEdit = true;
                if (localGoodsOrderInfo.OrderState == (int)OrderState.Cancellation)
                {
                    return true;
                }
                localGoodsOrderDetails = _goodsOrderDetailDao.GetGoodsOrderDetailList(goodsOrder.OrderId);
            }

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                //1.订单导入，先删后插
                if (isEdit)
                {
                    var success = _goodsOrderDao.Delete(goodsOrder.OrderId);
                    if (!success)
                    {
                        errorMessage = "删除订单失败";
                        return false;
                    }
                }

                //2.插入订单和订单明细
                var insertSuccess = _goodsOrderDao.Insert(goodsOrder, out errorMessage);
                if (!insertSuccess)
                {
                    errorMessage = string.Format("添加订单信息失败:{0}", errorMessage);
                    return false;
                }
                var insertDetailSuccess = _goodsOrderDetailDao.Insert(goodsOrderDetailList, goodsOrder, out errorMessage);
                if (!insertDetailSuccess)
                {
                    errorMessage = string.Format("添加订单商品明细失败:{0}", errorMessage);
                    return false;
                }

                //3.记录销量
                //异步销量
                if (goodsOrder.OrderState == (int)OrderState.WaitOutbound || goodsOrder.OrderState == (int)OrderState.StockUp || goodsOrder.OrderState == (int)OrderState.RequirePurchase || goodsOrder.OrderState == (int)OrderState.Redeploy)
                {
                    var orderJsonStr = new Framework.Core.Serialize.JsonSerializer().Serialize(goodsOrder);
                    var orderDetailJsonStr = new Framework.Core.Serialize.JsonSerializer().Serialize(goodsOrderDetailList);
                    var asynGoodsDaySalesStatisticsInfo = isEdit ?
                                                                                            ASYN_GoodsDaySalesStatisticsInfo.EditGoodsDaySale(goodsOrder.OrderNo, orderJsonStr, orderDetailJsonStr, new Framework.Core.Serialize.JsonSerializer().Serialize(localGoodsOrderDetails)) :
                                                                                            ASYN_GoodsDaySalesStatisticsInfo.AddGoodsDaySale(goodsOrder.OrderNo, orderJsonStr, orderDetailJsonStr);
                    var asynResult = _goodsOrderDetailDao.InsertASYN_GoodsDaySalesStatisticsInfo(asynGoodsDaySalesStatisticsInfo);
                    if (!asynResult)
                    {
                        errorMessage = "销量记录到异步失败！";
                        return false;
                    }
                }

                //4.发票插入
                if (invoiceInfo != null && invoiceInfo.InvoiceId != Guid.Empty)
                {
                    invoiceInfo.DeliverWarehouseId = goodsOrder.DeliverWarehouseId;
                    var insertInvoiceSuccess = _invoiceDao.Insert(invoiceInfo, new Dictionary<Guid, string> { { goodsOrder.OrderId, goodsOrder.OrderNo } });
                    if (!insertInvoiceSuccess)
                    {
                        errorMessage = "插入发票失败";
                        return false;
                    }
                }
                //5.配货中，会员中心交互
                MemberCenterSao.OrderAllocateGoods(goodsOrder.SalePlatformId, goodsOrder.OrderId);
                //提交事务
                ts.Complete();
                return true;
            }
        }
    }
}
