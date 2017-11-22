using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model.ShopFront;

namespace ERP.DAL.Interface.IShop
{
    public interface IApplyStockDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Insert(ApplyStockInfo info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoList"></param>
        /// <returns></returns>
        int InsertDetail(IList<ApplyStockDetailInfo> infoList);

        /// <summary>
        /// 查询是否存在此采购单
        /// </summary>
        /// <param name="applyStockId"></param>
        /// <returns></returns>
        ApplyStockInfo FindById(Guid applyStockId);

        /// <summary>
        /// 获取采购明细信息
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        IList<ApplyStockDetailInfo> FindDetailList(Guid applyId);

        /// <summary>
        /// 更新采购申请单异常信息
        /// </summary>
        /// <param name="outstockno"></param>
        /// <param name="errorMessage"></param>
        bool UpdateAddApplyStockExecption(string outstockno, Dictionary<Guid, String> errorMessage);

        /// <summary>
        /// 更新申请单状态
        /// </summary>
        /// <returns></returns>
        bool UpdateApplyStockState(Guid applyId, int state);

        #region 针对新需求，采购确认给出提示信息添加  add by liangcanren at 2015-04-1
        /// <summary>
        /// 更新采购明细需确认时的商品标识与提示
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="realGoodsId"></param>
        /// <param name="content"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        bool UpdateDetailTips(Guid applyId, Guid realGoodsId, string content, bool isError);
        #endregion

        /// <summary>
        /// 获取申请采购列表
        /// 注意：这里的trackCode可以搜索申请单号和出据单号
        /// zhangfan added at 2012-July-19th
        /// </summary>
        /// <returns></returns>
        IList<ApplyStockInfo> GetList(Guid filialeId, int applyStockState, int purchaseType, string tradeCode);

        /// <summary>
        /// 专门给需求查询中的门店采购申请
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        IList<ApplyStockInfo> GetList(Guid goodsId, Guid warehouseId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        ApplyStockInfo GetApplyInfoByTradeCode(string tradeCode);

        /// <summary>
        /// 根据单号更新申请单状态
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool UpdateApplyStockStateByTradeCode(string tradeCode, int state);

        #region  联盟店新增
        /// <summary>
        /// 获取店铺某段时间内的采购数/可退换货数
        /// </summary>
        /// <param name="shopId">店铺id</param>
        /// <param name="goodsId">主商品Id</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="states"> </param>
        /// <returns></returns>
        int GetApplyStockGoodsCount(Guid shopId,Guid goodsId,DateTime startTime,DateTime endTime,IList<int> states);

        /// <summary>
        /// 获取门店采购申请列表
        /// </summary>
        /// <param name="shopId">店铺Id</param>
        /// <param name="purchaseType">采购类型</param>
        /// <param name="states">采购状态</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="searchKey">申请单号/出入库单据号</param>
        /// <param name="goodsId">商品Id列表</param>
        /// <returns></returns>
        IList<ApplyStockInfo> GetApplyStockList(Guid shopId, int purchaseType, IList<int> states,
            DateTime startTime,DateTime endTime,string searchKey,IList<Guid> goodsId);

        /// <summary>
        /// 获取特定时间段内商品采购数列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        Dictionary<Guid, Dictionary<Guid, int>> GetPurchaseGoodsQuantity(Guid shopId, DateTime startTime,
                                                                         DateTime endTime, IList<int> states);

        #endregion

        /// <summary>
        /// 通过门店采购申请编号获取明细 ADD BY LiangCanren at 2015-05-08
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        IList<ApplyStockDetailInfo> GetApplyStockDetailInfosByTradeCode(string tradeCode);

    }
}
