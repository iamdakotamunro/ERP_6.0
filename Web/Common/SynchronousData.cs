using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using ERP.SAL;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Common
{
    /// <summary>同步数据
    /// </summary>
    internal sealed class SynchronousData
    {
        internal static SynchronousData SyncInstance;

        internal static SynchronousData Instance
        {
            get { return SyncInstance ?? (SyncInstance = new SynchronousData()); }
        }

        /// <summary>发送短信和邮件
        /// </summary>
        internal void SyncSendMessage(GoodsOrderInfo goodsOrderInfo, MailType mailType, SMSType smsType, RefundmentMode refundmentMode)
        {
            
        }

        /// <summary>获取一条支付流水记录
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="gopstate">商品订单支付状态枚举</param>
        /// <param name="saleFilialeId"></param>
        /// <returns></returns>
        internal GoodsOrderPayInfo GetGoodsOrderPayInfoByOrderIdStatePayState(Guid saleFilialeId, Guid orderId, GoodsOrderPayState gopstate)
        {
            IList<GoodsOrderPayInfo> infos = OrderSao.GetGoodsOrderPayListByOrderId(saleFilialeId, orderId);

            IList<GoodsOrderPayInfo> gopinfos =
                infos.Where(d => d.PayState == (int)PayState.NoPay && d.State == (int)gopstate).ToList();
            if (gopinfos.Count > 0)
            {
                return gopinfos[0];
            }
            IList<GoodsOrderPayInfo> ginfos =
                infos.Where(d => d.PayState == (int)PayState.Wait && d.State == (int)gopstate).ToList();
            if (ginfos.Count > 0)
            {
                return ginfos[0];
            }

            if (infos.Count > 0)
            {
                return new GoodsOrderPayInfo();
            }
            return null;
        }

        /// <summary> 财务修改订单信息  独立后台或总后台字段区别:独---Weight,总--ReturnTime
        /// </summary>
        internal void SyncGoodsOrderUpdateAffirm(Guid saleFilialeId, GoodsOrderInfo goodsOrderInfo)
        {
            OrderSao.GoodsOrderUpdateAffirm(saleFilialeId, goodsOrderInfo);
        }

        /// <summary>修改订单信息
        /// </summary>
        /// <param name="goodsOrderInfo"></param>
        /// <param name="saleFilialeId"></param>
        internal void SyncGoodsOrderModify(Guid saleFilialeId, GoodsOrderInfo goodsOrderInfo)
        {
            OrderSao.GoodsOrderModify(saleFilialeId, goodsOrderInfo);
        }


        /// <summary>更新B2C发票状态
        /// </summary>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <param name="invoiceId">发票ID</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="flag">true 更新订单表的发票状态和发票表的发票状态，false 只更新发票表的发票状态</param>
        /// <param name="clew">备注</param>
        internal void SyncSetInvoiceState(Guid saleFilialeId, Guid invoiceId, InvoiceState invoiceState, Boolean flag, String clew)
        {
            InvoiceSao.SetInvoiceState(saleFilialeId, invoiceId, invoiceState, flag, clew);
        }

    }
}