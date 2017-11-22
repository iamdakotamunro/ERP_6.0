//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年4月27日
// 文件创建人:马力
// 最后修改时间:2006年4月27日
// 最后一次修改人:马力
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 资金账户网络接口类
    /// </summary>
    [Serializable]
    public class PaymentInterfaceInfo
    {
        /// <summary>
        /// 支付接口编号
        /// </summary>
        public Guid PaymentInterfaceId { get; private set; }

        /// <summary>
        /// 支付接口名称
        /// </summary>
        public string PaymentInterfaceName { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PaymentInterfaceInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInterfaceId">支付接口编号</param>
        /// <param name="paymentInterfaceName">支付接口名称</param>
        public PaymentInterfaceInfo(Guid paymentInterfaceId, string paymentInterfaceName)
        {
            PaymentInterfaceId = paymentInterfaceId;
            PaymentInterfaceName = paymentInterfaceName;
        }
    }
}