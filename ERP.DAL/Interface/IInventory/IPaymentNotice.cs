using System;
using System.Collections.Generic;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 到款通知类 --by jiang 2011-12-30
    /// </summary>
    public interface IPaymentNotice
    {
        /// <summary>
        /// 修改到款通知状态
        /// </summary>
        /// <param name="payid"></param>
        /// <param name="yon"></param>
        /// <param name="des"></param>
        void UpdatePayNoticState(Guid payid, PayState yon, string des);
        /// <summary>
        /// 获取到款通知的集合
        /// </summary>
        /// <param name="state"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        IList<PaymentNoticeInfo> GetPayNoticListByPayState(int state,string keyword);
        /// <summary>
        /// 根据订单获取到款通知集合
        /// </summary>
        /// <param name="payId"></param>
        /// <returns></returns>
        PaymentNoticeInfo GetPayNoticInfoByPayid(Guid payId);

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="payid"></param>
        void Delete(Guid payid);

        void Insert(PaymentNoticeInfo pnInfo);

        /// <summary>
        /// 是否存在这个订到款通知(待确认)
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        bool IsExistByOrderNoAndPaystate(string orderNo);

        /// <summary>
        /// 根据订单号获取支付信息记录
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        IList<PaymentNoticeInfo> GetListByOrderNo(string orderNo);

        /// <summary>
        /// 修改到款通知信息
        /// </summary>
        /// <param name="pnInfo"></param>
        void UpdatePayNoticeInfo(PaymentNoticeInfo pnInfo);

        /// <summary>
        /// 根据PAYID删除待付款确认信息
        /// </summary>
        void DeletePayNoticeByPayId(Guid payid);
    }
}
