using System;
using System.Collections.Generic;
using ERP.Model.ShopFront;

namespace ERP.DAL.Interface.IShop
{
    /// <summary>
    /// 退货留言接口
    /// </summary>
    public interface IShopRefundMessage
    {
        /// <summary>
        /// 添加退货留言
        /// </summary>
        /// <param name="messageInfo"></param>
        void InsertShopRefundMessage(ShopRefundMessageInfo messageInfo);

        /// <summary>
        /// 修改退货留言
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        int UpdateShopRefundMessage(ShopRefundMessageInfo messageInfo);

        /// <summary>
        /// 删除退货留言
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        int DeleteShopRefundMessage(Guid msgId);

        /// <summary>
        /// 获取特定状态下的退货留言数量
        /// </summary>
        /// <param name="shopId">店铺Id</param>
        /// <param name="state"> </param>
        /// <returns></returns>
        int GetMessageCount(Guid shopId,int state);

        /// <summary>
        /// 修改退货留言状态并添加备注信息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        int SetMessageState(Guid msgId,int state,string description);

        /// <summary>
        /// 根据留言Id获取退货留言信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ShopRefundMessageInfo GetShopRefundMessageInfo(Guid id);

        /// <summary>
        /// 获取最近的退货留言Id
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="state"> </param>
        /// <returns></returns>
        Guid GetLastedRefundMsgId(Guid shopId,int state);

        /// <summary>
        /// 根据店铺、时间、留言状态查询退货留言
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="state">审核状态</param>
        /// /// <param name="shopId">店铺Id</param>
        /// <param name="ascOrDesc">升序还是降序 false 升序，true 降序</param>
        /// <returns></returns>
        IEnumerable<ShopRefundMessageInfo> GetShopRefundMesageList(DateTime startTime, DateTime endTime,
            int state, Guid shopId, bool ascOrDesc);
    }
}
