using System;
using System.Collections.Generic;
using AllianceShop.Common.ReturnModel;
using AllianceShop.Contract.DataTransferObject;
using KeedeGroup.GoodsManageSystem.Public.Model.B2C;

namespace ERP.SAL
{
    /// <summary>
    /// 加盟店
    /// </summary>
    public class ShopSao
    {
        #region  联盟店采购申请
        /// <summary>
        /// 联盟店修改采购订单状态
        /// </summary>
        /// <param name="saleFilialeId">店铺Id</param>
        /// <param name="applyId">采购订单/申请Id</param>
        /// <param name="stockState">状态</param>
        /// <param name="remark"> </param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool UpdatePurchaseState(Guid saleFilialeId, Guid applyId, int stockState, string remark, out string message)
        {
            message = string.Empty;
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.UpdatePurchaseState(applyId, stockState, remark);
                if (result == null || !result.IsSuccess)
                {
                    message = result == null ? "联盟店采购申请修改失败!" : result.Message;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 联盟店添加往来单位收付款
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static bool InsertCompanyFundReceipt(Guid saleFilialeId, CompanyFundReceiptTransDTO dto)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.InsertCompanyFundReceipt(dto);
                if (result == null || !result.IsSuccess)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region  联盟店商品退回检查

        /// <summary>
        /// 设置联盟店退货次数
        /// </summary>
        /// <param name="saleFilialeId"> </param>
        /// <param name="shopId"></param>
        /// <param name="isAllow"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SetCanReturnCount(Guid saleFilialeId, Guid shopId, bool isAllow, out string msg)
        {
            msg = string.Empty;
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.AddUpdatePermission(new RefundPermissionDTO
                {
                    ShopID = shopId,
                    HasRefund = isAllow
                });
                if (!result.IsSuccess)
                {
                    msg = result.Message;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region  联盟店出入库

        /// <summary>
        /// 添加联盟店入库申请
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="stockDto"></param>
        /// <param name="stockDetailDtos">入库明细</param>
        /// <param name="isPurchase">是否为采购入库</param>
        /// <param name="expressDto">物流绑定</param>
        /// <param name="totalPrice">退回金额</param>
        /// <param name="msg"> </param>
        /// <returns>生成的入库单据号</returns>
        public static string InsertStock(Guid saleFilialeId, StockDTO stockDto, IList<StockDetailDTO> stockDetailDtos,
            bool isPurchase, BindExpressDTO expressDto, decimal totalPrice, out string msg)
        {
            msg = string.Empty;
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.InsertStock(stockDto, stockDetailDtos, totalPrice, isPurchase, expressDto);
                if (result == null || !result.IsSuccess)
                {
                    msg = result == null ? "联盟店入库申请插入失败!" : result.Message;
                    return string.Empty;
                }
                return result.Data;
            }
        }

        /// <summary>
        /// 退还金额
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="shopId"></param>
        /// <param name="totalPrice"></param>
        /// <param name="purchaseId"> </param>
        /// <param name="description"> </param>
        /// <param name="msg"> </param>
        /// <returns></returns>
        public static bool ReturnMoney(Guid saleFilialeId, Guid shopId, decimal totalPrice, Guid purchaseId, string description, out string msg)
        {
            msg = string.Empty;
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.ReturnMoney(totalPrice, shopId, purchaseId, description);
                if (result == null || !result.IsSuccess)
                {
                    msg = result == null ? "联盟店退还金额失败!" : result.Message;
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region  联盟店物流

        ///// <summary>
        ///// ERP发货，审核出库绑定快递列表
        ///// </summary>
        ///// <param name="saleFilialeId"> </param>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public static IList<ExpressCodeDTO> GetExpressCodeList(Guid saleFilialeId, out string msg)
        //{
        //    msg = string.Empty;
        //    using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
        //    {
        //        var result = client.Instance.SelectExpressCodeList();
        //        if (result == null || !result.IsSuccess)
        //        {
        //            msg = result == null ? "获取快递信息列表失败!" : result.Message;
        //            return null;
        //        }
        //        return result.Data;
        //    }
        //}

        /// <summary>
        /// ERP发货,添加物流绑定
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="bindExpressDto"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool InsertBindExpress(Guid saleFilialeId, BindExpressDTO bindExpressDto, out string msg)
        {
            msg = string.Empty;
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.InsertBindExpress(bindExpressDto, false);
                if (result == null || !result.IsSuccess)
                {
                    msg = result == null ? "插入物流绑定信息失败!" : result.Message;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 获取联盟店绑定的物流信息
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="applyId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static IList<BindExpressDTO> GetBindExpressList(Guid saleFilialeId, Guid applyId, out string msg)
        {
            msg = string.Empty;
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.SelectBindExpressListById(applyId, Guid.Empty);
                if (result == null || !result.IsSuccess)
                {
                    msg = result == null ? "查询联盟店物流信息失败!" : result.Message;
                    return null;
                }
                return result.Data;
            }
        }
        #endregion

        #region 充值管理
        /// <summary>
        /// 查询充值记录
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="rechargeState"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static ReturnList<RechargeDTO> SelectRechargeList(Guid saleFilialeId, int? rechargeState, DateTime? startTime, DateTime? endTime, int pageSize, int pageIndex)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                return client.Instance.SelectRechargeList(saleFilialeId, rechargeState, startTime, endTime, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 查找充值记录-总公司
        /// modify by liangcanren at 2015-04-14  添加按照店铺类型进行搜索
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="shopId"> </param>
        /// <param name="rechargeState"></param>
        /// <param name="joinType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static ReturnPage<RechargeDTO> SelectRechargeListByParentId(Guid saleFilialeId, Guid? shopId, int rechargeState,
            int joinType, DateTime? startTime, DateTime? endTime, int? pageSize, int? pageIndex)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                return client.Instance.SelectRechargeListByParentId(shopId, rechargeState, joinType, startTime, endTime, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 获取一条充值记录
        /// </summary>
        /// <param name="rechargeId"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        public static ReturnInfo<RechargeDTO> SelectRechargeById(Guid rechargeId, Guid saleFilialeId)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                return client.Instance.SelectRechargeById(rechargeId);
            }
        }

        /// <summary>
        /// 操作充值记录
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="rechargeId"></param>
        /// <param name="rechargeState"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static ReturnBase ConfirmRechargeState(Guid saleFilialeId, Guid rechargeId, int rechargeState, string description)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                return client.Instance.ConfirmRechargeState(rechargeId, rechargeState, description);
            }
        }

        #endregion

        #region  退换货商品相关

        /// <summary>
        /// 获取同类型框架
        /// </summary>
        /// <param name="saleFilialeId"> </param>
        /// <param name="goodsID"></param>
        /// <param name="shopId"> </param>
        /// <returns></returns>
        public static IList<GoodsSaleBaseModel> GetSameClassGoodsList(Guid saleFilialeId, Guid goodsID, Guid shopId)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                return client.Instance.GetSameClassGoodsList(goodsID, shopId);
            }
        }

        /// <summary>
        /// 获取可换货商品规格
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="shopId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public static IList<RealGoodsDTO> GetExchageGoods(Guid saleFilialeId, Guid shopId, Guid goodsId)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var reutnrDics = client.Instance.GetExchageGoods(shopId, goodsId);
                if (reutnrDics == null || !reutnrDics.IsSuccess) return null;
                return reutnrDics.Data.RealGoodsDtos;
            }
        }
        #endregion

        #region  插入往来帐
        public static bool InsertReckoning(Guid saleFilialeId, ReckoningRecordDTO dto)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.InsertReckoningRecord(dto);
                if (result == null || !result.IsSuccess)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region  插入资金流
        public static bool InsertRecord(Guid saleFilialeId, Guid shopId, string orderNo, decimal sum, bool cashOrcard, int turnoverType,
            string description)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.InsertRecord(shopId, orderNo, sum, cashOrcard, turnoverType, description);
                if (result == null || !result.IsSuccess)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region 银行账户

        /// <summary>
        /// 获取店铺下银行账户
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static IList<BankBalanceDTO> GetBankBalanceList(Guid saleFilialeId, Guid shopId)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.GetBankBalanceList(shopId);
                if (result == null || !result.IsSuccess)
                {
                    return null;
                }
                return result.Data;
            }
        }

        #endregion

        #region  往来单位收付款
        public static CompanyFundReceiptDTO GetCompanyFundReceiptEntityByOriginalTradeCode(Guid saleFilialeId, string originalTradeCode)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.GetCompanyFundReceiptEntityByOriginalTradeCode(originalTradeCode);
                if (result == null || !result.IsSuccess)
                {
                    return null;
                }
                return result.Data;
            }
        }
        #endregion

        #region  门店资金流审核

        /// <summary> 门店资金流审核
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="rechargeDto"></param>
        /// <returns></returns>
        public static bool AuditingSettlement(Guid saleFilialeId, RechargeDTO rechargeDto)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.AuditingSettlement(rechargeDto);
                return result != null && result.IsSuccess;
            }
        }

        #endregion

        #region 门店直营店可用余额扣除

        /// <summary> 门店直营店可用余额扣除
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="shopId"></param>
        /// <param name="money"></param>
        /// <param name="description"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool DeductBalance(Guid saleFilialeId, Guid shopId, decimal money, string description, out string errorMsg)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.DeductBalance(shopId, money, description);
                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        errorMsg = "";
                        return true;
                    }
                    errorMsg = result.Message;
                    return false;
                }
                errorMsg = "服务连接失败!";
                return false;
            }
        }
        #endregion

        #region  插入付款记录

        /// <summary> 
        /// 插入付款记录
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="costRecordDto"></param>
        /// <returns></returns>
        /// zal 2015-09-22
        public static bool InsertCostRecord(Guid saleFilialeId, CostRecordDTO costRecordDto)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.InsertCostRecord(costRecordDto);
                return result != null && result.IsSuccess;
            }
        }

        #endregion
        
        public static IList<ShopDTO> GetAllShop(Guid saleFilialeId)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(saleFilialeId))
            {
                var result = client.Instance.SelectAllShop();
                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        return result.Data;
                    }
                }
            }
            return null;
        }

    }
}
