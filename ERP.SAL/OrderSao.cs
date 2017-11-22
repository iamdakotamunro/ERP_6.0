using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.SAL
{
    public class OrderSao
    {
        static GoodsOrderPayInfo ConvertTo(B2C.Model.GoodsOrderPayInfo info)
        {
            if (info != null)
            {
                return new GoodsOrderPayInfo
                {
                    BankAccountId = info.BankAccountId,
                    BankTradeNo = info.BankTradeNo,
                    Content = info.Content,
                    CreationDate = info.CreationDate,
                    ExchangeRate = info.ExchangeRate,
                    OrderId = info.OrderId,
                    PaidNo = info.PaidNo,
                    PaidTime = info.PaidTime,
                    PaiSum = info.PaiSum,
                    PayState = info.PayState,
                    State = info.State
                };
            }
            return new GoodsOrderPayInfo();
        }

        /// <summary>修改订单支付状态(不插入异常数据)
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="orderId">订单id</param>
        /// <param name="payState">支付状态</param>
        /// <param name="state">GoodsOrderPayState订单状态</param>
        public static bool UpdatePayStateByOrderIdAndStateBool(Guid saleFilialeId, Guid orderId, PayState payState, GoodsOrderPayState state)
        {
            var pushDataId = Guid.NewGuid();
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                var result = client.Instance.UpdatePayStateByOrderIdAndState(pushDataId, orderId, (Int32)payState, (Int32)state);
                if (result == null || result.IsAccomplishExecuted == false)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary> 设置订单支付状态
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="orderId"></param>
        /// <param name="payState"></param>
        public static void SetOrderPayState(Guid saleFilialeId, Guid orderId, PayState payState)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                client.Instance.SetOrderPayState(Guid.NewGuid(), orderId, (Int32)payState);
            }
        }


        public static void GoodsOrderModify(Guid saleFilialeId, GoodsOrderInfo goodsOrderInfo)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                client.Instance.GoodsOrderUpdateAffirm(Guid.NewGuid(), ConvertToB2CModel(goodsOrderInfo));
            }
        }

        public static void GoodsOrderUpdateAffirm(Guid saleFilialeId, GoodsOrderInfo goodsOrderInfo)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                client.Instance.GoodsOrderUpdateAffirm(Guid.NewGuid(), ConvertToB2CModel(goodsOrderInfo));
            }
        }

        /// <summary>根据订单ID获取支付流水列表
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="orderid">订单ID</param>
        /// <returns></returns>
        public static IList<GoodsOrderPayInfo> GetGoodsOrderPayListByOrderId(Guid saleFilialeId, Guid orderid)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                var items = new List<GoodsOrderPayInfo>();
                var list = client.Instance.GetGoodsOrderPayListByOrderId(orderid);
                if (list != null && list.Count > 0)
                {
                    foreach (var info in list)
                    {
                        items.Add(ConvertTo(info));
                    }
                }
                return items;
            }
        }

        /// <summary>可以从数据库中取出订单
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static GoodsOrderInfo GetGoodsOrderInfo(Guid saleFilialeId, Guid orderId)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                var goodsOrderInfoB2C = client.Instance.GetGoodsOrderInfo(orderId);
                goodsOrderInfoB2C = goodsOrderInfoB2C ?? new B2C.Model.ERPExtensionModel.GoodsOrderInfo();
                return ConvertToERPModel(goodsOrderInfoB2C);
            }
        }


        /// <summary>根据订单号获取B2C订单信息（会员余额管理确认用）
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static B2C.Model.ERPExtensionModel.GoodsOrderInfo GetGoodsOrderInfoByOrderNo(Guid saleFilialeId, string orderNo)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                var result = client.Instance.GetGoodsOrderList(new List<string> { orderNo });
                return result.IsSuccess ? result.Data.First() : null;
            }
        }
        /// <summary>
        /// 根据第三方订单号获取订单信息
        /// </summary>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="thirdOrderNo">第三方订单号</param>
        /// <returns></returns>
        /// zal 2017-11-06
        public static List<GoodsOrderInfo> GetGoodsOrderInfoByThirdOrderNo(Guid saleFilialeId, string thirdOrderNo)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                var goodsOrderInfoList = new List<GoodsOrderInfo>();
                var result = client.Instance.GetGoodsOrderInfoByTid(thirdOrderNo);
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        goodsOrderInfoList.Add(ConvertToERPModel(item));
                    }
                }
                return goodsOrderInfoList;
            }
        }

        /// <summary>ERP订单模型转换成B2C订单模型
        /// </summary>
        /// <param name="goodsOrderInfo"></param>
        /// <returns></returns>
        public static B2C.Model.ERPExtensionModel.GoodsOrderInfo ConvertToB2CModel(GoodsOrderInfo goodsOrderInfo)
        {
            return new B2C.Model.ERPExtensionModel.GoodsOrderInfo
            {
                BankAccountsId = goodsOrderInfo.BankAccountsId,
                BankTradeNo = goodsOrderInfo.BankTradeNo,
                Carriage = goodsOrderInfo.Carriage,
                CarriageSubsidy = goodsOrderInfo.CarriageSubsidy,
                CityId = goodsOrderInfo.CityId,
                CommissionFailure = goodsOrderInfo.CommissionFailure,
                Consignee = goodsOrderInfo.Consignee,
                ConsignTime = goodsOrderInfo.ConsignTime,
                CountryId = goodsOrderInfo.CountryId,
                DeliverWarehouseId = goodsOrderInfo.DeliverWarehouseId,
                Direction = goodsOrderInfo.Direction,
                DistrictId = goodsOrderInfo.DistrictID,
                EffectiveTime = goodsOrderInfo.EffectiveTime,
                Express = goodsOrderInfo.Express,
                ExpressId = goodsOrderInfo.ExpressId,
                ExpressNo = goodsOrderInfo.ExpressNo,
                InvoiceState = goodsOrderInfo.InvoiceState,
                IsOut = goodsOrderInfo.IsOut,
                MemberId = goodsOrderInfo.MemberId,
                Memo = goodsOrderInfo.Memo,
                Mobile = goodsOrderInfo.Mobile,
                OldCustomer = goodsOrderInfo.OldCustomer,
                OrderId = goodsOrderInfo.OrderId,
                OrderNo = goodsOrderInfo.OrderNo,
                OrderState = goodsOrderInfo.OrderState,
                OrderTime = goodsOrderInfo.OrderTime,
                PaidUp = goodsOrderInfo.PaidUp,
                PaymentByBalance = goodsOrderInfo.PaymentByBalance,
                PayMode = goodsOrderInfo.PayMode,
                PayState = goodsOrderInfo.PayState,
                PayType = goodsOrderInfo.PayType,
                Phone = goodsOrderInfo.Phone,
                PickNo = goodsOrderInfo.PickNo,
                PostalCode = goodsOrderInfo.PostalCode,
                PromotionDescription = goodsOrderInfo.PromotionDescription,
                PromotionValue = goodsOrderInfo.PromotionValue,
                ProvinceId = goodsOrderInfo.ProvinceId,
                RealTotalPrice = goodsOrderInfo.RealTotalPrice,
                RefundId = goodsOrderInfo.RefundId,
                RefundmentMode = goodsOrderInfo.RefundmentMode,
                ReturnOrder = goodsOrderInfo.ReturnOrder,
                ReturnTime = goodsOrderInfo.ReturnTime,
                SaleFilialeId = goodsOrderInfo.SaleFilialeId,
                SalePlatformId = goodsOrderInfo.SalePlatformId,
                ScoreDeduction = goodsOrderInfo.ScoreDeduction,
                ScoreDeductionProportion = goodsOrderInfo.ScoreDeductionProportion,
                SendType = goodsOrderInfo.SendType,
                TotalPrice = goodsOrderInfo.TotalPrice
            };
        }

        /// <summary>B2C订单模型转换成ERP订单模型
        /// </summary>
        /// <param name="goodsOrderInfo"></param>
        /// <returns></returns>
        public static GoodsOrderInfo ConvertToERPModel(B2C.Model.ERPExtensionModel.GoodsOrderInfo goodsOrderInfo)
        {
            return new GoodsOrderInfo
            {
                BankAccountsId = goodsOrderInfo.BankAccountsId,
                BankTradeNo = goodsOrderInfo.BankTradeNo,
                Carriage = goodsOrderInfo.Carriage,
                CarriageSubsidy = goodsOrderInfo.CarriageSubsidy,
                CityId = goodsOrderInfo.CityId,
                CommissionFailure = goodsOrderInfo.CommissionFailure,
                Consignee = goodsOrderInfo.Consignee,
                ConsignTime = goodsOrderInfo.ConsignTime,
                CountryId = goodsOrderInfo.CountryId,
                DeliverWarehouseId = goodsOrderInfo.DeliverWarehouseId,
                Direction = goodsOrderInfo.Direction,
                DistrictID = goodsOrderInfo.DistrictId,
                EffectiveTime = goodsOrderInfo.EffectiveTime,
                Express = goodsOrderInfo.Express,
                ExpressId = goodsOrderInfo.ExpressId,
                ExpressNo = goodsOrderInfo.ExpressNo,
                InvoiceState = goodsOrderInfo.InvoiceState,
                //IsOut = goodsOrderInfo.IsOut,
                MemberId = goodsOrderInfo.MemberId,
                Memo = goodsOrderInfo.Memo,
                Mobile = goodsOrderInfo.Mobile,
                OldCustomer = goodsOrderInfo.OldCustomer,
                OrderId = goodsOrderInfo.OrderId,
                OrderNo = goodsOrderInfo.OrderNo,
                OrderState = goodsOrderInfo.OrderState,
                OrderTime = goodsOrderInfo.OrderTime,
                PaidUp = goodsOrderInfo.PaidUp,
                PaymentByBalance = goodsOrderInfo.PaymentByBalance,
                PayMode = goodsOrderInfo.PayMode,
                PayState = goodsOrderInfo.PayState,
                PayType = goodsOrderInfo.PayType,
                Phone = goodsOrderInfo.Phone,
                PickNo = goodsOrderInfo.PickNo,
                PostalCode = goodsOrderInfo.PostalCode,
                PromotionDescription = goodsOrderInfo.PromotionDescription,
                PromotionValue = goodsOrderInfo.PromotionValue,
                ProvinceId = goodsOrderInfo.ProvinceId,
                RealTotalPrice = goodsOrderInfo.RealTotalPrice,
                RefundId = goodsOrderInfo.RefundId,
                RefundmentMode = goodsOrderInfo.RefundmentMode,
                ReturnOrder = goodsOrderInfo.ReturnOrder,
                ReturnTime = goodsOrderInfo.ReturnTime,
                SaleFilialeId = goodsOrderInfo.SaleFilialeId,
                SalePlatformId = goodsOrderInfo.SalePlatformId,
                ScoreDeduction = goodsOrderInfo.ScoreDeduction,
                ScoreDeductionProportion = goodsOrderInfo.ScoreDeductionProportion,
                SendType = goodsOrderInfo.SendType,
                TotalPrice = goodsOrderInfo.TotalPrice
            };
        }
    }
}
