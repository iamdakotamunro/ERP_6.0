using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 活动报备
    /// </summary>
    public interface IActivityFiling
    {
        IList<ActivityFilingInfo> SelectActivityFilings(string title, string goodsName, DateTime activityStateDate, DateTime activityEndDate, Guid saleTerrace, int purchaseState,int pageIndex,int pageSize,out int total);

        /// <summary>
        /// 获取子商品的销售统计记录
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="minDays"></param>
        /// <param name="maxDays"></param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        int GetGoodsSale(Guid goodsID, Guid warehouseId, Guid saleFilialeId, Guid salePlatformId, int minDays, int maxDays);

        /// <summary>
        /// 获得实际销量
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="warehouseId"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        int GetGoodsRealSale(Guid goodsID, Guid warehouseId, Guid saleFilialeId, Guid salePlatformId, DateTime startDateTime, DateTime endDateTime);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="activityFilingInfo"></param>
        /// <returns></returns>
        bool InsertActivityFiling(ActivityFilingInfo activityFilingInfo);

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActivityFilingInfo SelectFilingInfoById(Guid id);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool UpdateFilingState(Guid id, int state);

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="purchasePersonnelID"></param>
        /// <param name="purchasePersonnelName"></param>
        /// <param name="prospectReadyNumber"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool UpdateFilingInfo(Guid id, Guid purchasePersonnelID, string purchasePersonnelName, int prospectReadyNumber, int state);

        /// <summary>
        /// 获得统计数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="goodsName"></param>
        /// <param name="activityStateDate"></param>
        /// <param name="activityEndDate"></param>
        /// <param name="saleTerrace"></param>
        /// <param name="purchaseState"></param>
        /// <returns></returns>
        ActivityFilingTotalModel TotalNumber(string title, string goodsName, DateTime activityStateDate,
            DateTime activityEndDate, Guid saleTerrace, int purchaseState);

        /// <summary>
        /// 查询商品是否已经存在活动报备单中
        /// </summary>
        /// <param name="filingCompanyID"></param>
        /// <param name="goodsId"></param>
        /// <param name="filingTerraceID"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        bool SelectGoods(Guid filingCompanyID, Guid goodsId, Guid filingTerraceID,Guid warehouseId);

        /// <summary>
        /// 修改商品的实际销量和误差率
        /// </summary>
        /// <param name="id"></param>
        /// <param name="actualSaleNumber"></param>
        /// <param name="errorProbability"></param>
        /// <returns></returns>
        bool UpdateGoodsSaleNumber(Guid id, int actualSaleNumber, decimal errorProbability);

        /// <summary>
        /// 修改采购信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="purchasePersonnelID"></param>
        /// <param name="purchasePersonnelName"></param>
        /// <param name="prospectReadyNumber"></param>
        /// <returns></returns>
        bool UpdateFilingInfo(Guid id, Guid purchasePersonnelID, string purchasePersonnelName, int prospectReadyNumber);

        /// <summary>
        /// 修改活动报备单基本信息
        /// </summary>
        /// <param name="activityFilingInfo"></param>
        /// <returns></returns>
        bool UpdateFilingBaseInfo(ActivityFilingInfo activityFilingInfo);
    }
}
