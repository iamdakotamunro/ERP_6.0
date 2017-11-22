using System;
using System.Collections.Generic;
using ERP.Model.ShopFront;

namespace ERP.DAL.Interface.IShop
{
    /// <summary>
    /// 退换货申请接口
    /// </summary>
    public interface IShopExchangedApply
    {
        /// <summary>
        /// 添加退换货申请
        /// </summary>
        /// <param name="applyInfo"></param>
        void InsertShopExchangedApply(ShopExchangedApplyInfo applyInfo);

        /// <summary>
        /// 更新退换货申请
        /// </summary>
        /// <param name="applyInfo"></param>
        int UpdateShopExchangedApply(ShopExchangedApplyInfo applyInfo);

        /// <summary>
        /// 更新退换货申请状态且添加备注
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        int UpdateExchangeState(Guid applyId, int state, string description);

        /// <summary>
        /// 获取当月的退货记录
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-03-15
        Dictionary<string, decimal> SelectOneMonthReturnedApplyList(Guid shopId, DateTime dateTime);

        /// <summary>
        /// 删除退换货申请
        /// </summary>
        /// <param name="applyId"></param>
        int DeleteShopExchangedApply(Guid applyId);

        /// <summary>
        /// 获取退换货申请
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        ShopExchangedApplyInfo GetShopExchangedApplyInfo(Guid applyId);

        /// <summary>
        /// 通过退换货申请单号获取退换货申请
        /// </summary>
        /// <param name="applyNo"></param>
        /// <returns></returns>
        ShopExchangedApplyInfo GetShopExchangedApplyInfoByApplyNo(string applyNo);

        /// <summary>
        /// 根据条件获取指定退换货申请
        /// </summary>
        /// <param name="isBarter">false 换货，ture 退货</param>
        /// <param name="applyNo">退换货申请单号</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="goodsIds">商品列表</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="state">申请状态</param>
        /// <returns></returns>
        IEnumerable<ShopExchangedApplyInfo> GetShopExchangedApplyList(bool isBarter, string applyNo, DateTime startTime,
            DateTime endTime, IList<Guid> goodsIds, Guid shopId, int state);

        /// <summary>
        /// 根据条件获取指定换货申请列表
        /// </summary>
        /// <param name="applyNo">换货申请单号</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="goodsIds">商品列表</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="state">申请状态</param>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        IEnumerable<ShopExchangedApplyInfo> GetShopBarterApplyList(string applyNo, DateTime startTime,
            DateTime endTime, IList<Guid> goodsIds, Guid shopId, int state, string goodsNameOrCode);

        /// <summary>
        /// 根据条件获取指定退退货货申请
        /// </summary>
        /// <param name="applyNo">退货申请单号</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="goodsIds">商品列表</param>
        /// <param name="shopId">店铺Id</param>
        /// <param name="state">申请状态</param>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        IEnumerable<ShopExchangedApplyInfo> GetShopRefundApplyList(string applyNo, DateTime startTime,
            DateTime endTime, IList<Guid> goodsIds, Guid shopId, int state, string goodsNameOrCode);

        /// <summary>
        /// 获取退换货单的状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="applyNo"> </param>
        /// <returns></returns>
        int GetExchangeState(Guid applyId, string applyNo);
    }
}
