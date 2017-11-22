using System;
using System.Collections.Generic;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model.ShopFront;

namespace ERP.DAL.Interface.IShop
{
    public interface IShopActivityNoticeDal
    {
        bool InsertShopActivityNoticeInfo(ShopActivityNoticeInfo info);

        bool UpdateShopActivityNoticeInfo(ShopActivityNoticeInfo info);

        bool DeleteShopActivityNoticeInfo(Guid noteId);

        ShopActivityNoticeInfo SelectActivityNoticeInfo(Guid noteId);

        IEnumerable<ShopActivityNoticeInfo> SelectNoticeList();

        bool UpdateIsNotice(bool isNote, Guid noteId);

        bool UpdateIsShow(bool isShow, Guid noteId);

        /// <summary>
        /// 更新公告管理序号
        /// </summary>
        /// <param name="noticeId">公告管理主键ID</param>
        /// <param name="orderIndex">序号</param>
        /// <returns></returns>
        /// zal 2015-09-22
        bool UpdateOrderIndex(Guid noticeId, int orderIndex);

        /// <summary>
        /// 根据条件查询广告---分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isShow">是否显示</param>
        /// <param name="isNotice">是否是广告-显示在首页</param>
        /// <returns></returns>
        PageItems<ShopActivityNoticeInfo> SelectNoticeListByPage(int pageIndex, int pageSize, bool isShow, bool? isNotice);
    }
}
