//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年5月6日
// 文件创建人:马力
// 最后修改时间:2007年5月6日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IPaymentInterface
    {
        /// <summary>
        /// 获取指定的网络支付接口
        /// </summary>
        /// <param name="paymentInterfaceId">网络支付接口编号</param>
        /// <returns></returns>
        PaymentInterfaceInfo GetPaymentInterface(Guid paymentInterfaceId);

        /// <summary>
        /// 获取网络支付接口信息类
        /// </summary>
        /// <returns></returns>
        IList<PaymentInterfaceInfo> GetPaymentInterfaceList();
        /// <summary>
        /// 取支付宝帐户的ID
        /// </summary>
        /// <returns></returns>
        Guid GetBandAccountsId();
       
    }
}
