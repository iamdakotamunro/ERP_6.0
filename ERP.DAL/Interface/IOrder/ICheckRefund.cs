using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IOrder
{
    public interface ICheckRefund
    {
        /// <summary>
        /// 插入退回检查
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        bool InsertCheckRefund(CheckRefundInfo refundInfo);

        /// <summary>
        /// 更新退回检查
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        bool UpdateCheckRefund(CheckRefundInfo refundInfo);

        /// <summary>
        /// 删除退回检查
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        bool DeleteCheckRefund(Guid refundId);

        /// <summary>
        /// 添加退换货检查商品明细
        /// </summary>
        /// <param name="detailInfo"></param>
        /// <returns></returns>
        bool InsertCheckDetails(CheckRefundDetailInfo detailInfo);

        /// <summary>
        /// 根据条件获取退换货检查记录
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="checkState">检查状态</param>
        /// <param name="checkFilialeId">检查公司</param>
        /// <returns></returns>
        IList<CheckRefundInfo> GetCheckRefundInfo(string keywords, DateTime startTime, DateTime endTime, int checkState, Guid checkFilialeId);

        /// <summary>
        /// 根据条件获取联盟店退换货检查记录
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="checkState">检查状态</param>
        /// <param name="checkFilialeIds">检查公司</param>
        /// <returns></returns>
        IList<CheckRefundInfo> GetShopCheckRefundList(string keywords, DateTime startTime, DateTime endTime, int checkState, List<Guid> checkFilialeIds);

        /// <summary>
        /// 根据退换货号，获取退换商品详细
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        IList<CheckRefundDetailInfo> GetCheckRefundDetails(Guid refundId);

        /// <summary>
        /// 更改退回商品检查中损坏数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="damageCount">损坏数量</param>
        /// <param name="realGoodsId"></param>
        /// <param name="specification"></param>
        bool UpdateCheckRefundDetails(Guid id, int damageCount, Guid realGoodsId, string specification);

        /// <summary>
        /// 添加退换货信息以及清单信息
        /// </summary>
        /// <param name="refundInfo">退换货信息</param>
        /// <param name="refundDetailList">清单信息</param>
        /// <returns></returns>
        bool InsertCheckRefundAndDetailList(CheckRefundInfo refundInfo,
                                                   IList<CheckRefundDetailInfo> refundDetailList);


        /// <summary>
        /// 修改退换货检查信息
        /// </summary>
        /// <param name="refundInfo">退换货检查信息</param>
        /// <returns></returns>
        bool UpdateCheckRefund_Server(CheckRefundInfo refundInfo);

        /// <summary>
        /// 获取退换货检查信息
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        CheckRefundInfo GetCheckRefundInfo(Guid refundId);

        /// <summary>
        /// 删除退换货检查信息
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        bool DeleteCheckRefundInfo(Guid refundId);

        /// <summary>
        /// 供前台使用修改退换检查物流信息
        /// </summary>
        /// <param name="checkRefund"></param>
        /// <returns></returns>
        int ModifyCheckRefundExpress(CheckRefundInfo checkRefund);

        /// <summary>
        /// 获取退回商品检查信息
        /// </summary>
        /// <returns></returns>
        IList<CheckRefundInfo> GetCheckRefundList();

        /// <summary>
        /// 更改退回商品检查
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="checkState"></param>
        /// <param name="remark"></param>
        /// <param name="checkFilialeId">检查公司</param>
        int UpdateCheckRefund(Guid refundId, int checkState, string remark, Guid checkFilialeId);

        /// <summary>
        /// 获取退回商品检查明细
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        IList<CheckRefundDetailInfo> GetCheckRefundDetailList(Guid refundId); 

        /// <summary>
        /// 退回商品检查是否移交
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="isTransfer">是否移交</param>
        /// <returns></returns>
        bool UpdateCheckRefundIsTransfer(Guid refundId, bool isTransfer);

        /// <summary>
        /// 商品检查状态更改为检查未通过(售后单作废)
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="remark"></param>
        bool UpdateCheckRefundRefuse(Guid refundId, string remark);

        /// <summary>
        /// 插入退回检查快递单号
        /// add by liangcanren at 2015-04-17
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="expressNo"></param>
        /// <returns></returns>
        bool UpdateCheckRefundExpressNo(Guid refundId, string expressNo);

        #region  联盟店退货检查添加
        /// <summary>
        /// 联盟店获取退货检查信息
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        CheckRefundInfo GetShopCheckRefundInfo(Guid refundId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="applyId"></param>
        /// <param name="applyNo"></param>
        /// <returns></returns>
        IList<CheckRefundInfo> GetShopCheckRefundList(Guid shopId,Guid applyId,string applyNo);

        /// <summary>
        /// 获取联盟店退货检查明细
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="applyId"></param>
        /// <param name="checkState"></param>
        /// <returns></returns>
        IList<CheckRefundDetailInfo> GetShopCheckRefundDetailList(Guid refundId, Guid applyId, int checkState);

        #endregion
    }
}
