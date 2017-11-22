using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>匹配快递单业务结果返回模型（功能转移到打印程序和打包扫描合并） ADD  2016-10-14  陈重文
    /// </summary>

    [DataContract]
    public class MatchOrderResultInfo
    {
        [DataMember]
        public string OrderNo { get; private set; }

        [DataMember]
        public string ExpressNo { get; private set; }

        /// <summary>当前订单是否是使用传统面单
        /// </summary>
        [DataMember]
        public bool IsTradition { get; private set; }

        [DataMember]
        public bool IsSuccess { get; private set; }

        [DataMember]
        public string Msg { get; private set; }

        public MatchOrderResultInfo(string orderNo)
        {
            OrderNo = orderNo;
        }

        public void SetExpressNo(string expressNo)
        {
            ExpressNo = expressNo;
        }

        public void SetTradition(bool isTradition)
        {
            IsTradition = isTradition;
        }

        public void Success()
        {
            IsSuccess = true;
        }

        public void Fail(string msg)
        {
            IsSuccess = false;
            Msg = !string.IsNullOrWhiteSpace(OrderNo) ? string.Format("订单号：{0}， 错误提示：{1}", OrderNo, msg) : msg;
        }
    }
}
