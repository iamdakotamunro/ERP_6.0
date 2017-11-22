using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 系统邮件发送类型枚举
    /// </summary>
    [Serializable]
    public enum MailType
    {
        /// <summary>
        /// 注册
        /// </summary>
        [Enum("注册")]
        Register = 1,
        /// <summary>
        /// 登录
        /// </summary>
        [Enum("登录")]
        Login = 2,
        /// <summary>
        /// 获取密码
        /// </summary>
        [Enum("获取密码")]
        GetPassword = 3,
        /// <summary>
        /// 改写用户资料
        /// </summary>
        [Enum("改写用户资料")]
        RewriteMember = 4,
        /// <summary>
        /// 修改密码
        /// </summary>
        [Enum("修改密码")]
        ChangePassword = 5,
        /// <summary>
        /// 推荐给朋友
        /// </summary>
        [Enum("推荐给朋友")]
        Recommend = 6,
        /// <summary>
        /// 询问
        /// </summary>
        [Enum("询问")]
        Quaere = 7,
        /// <summary>
        /// 购物
        /// </summary>
        [Enum("购物")]
        Shopping = 10,
        /// <summary>
        /// 受理
        /// </summary>
        [Enum("受理")]
        Approved = 11,
        /// <summary>
        /// 催款
        /// </summary>
        [Enum("催款")]
        Reminder = 12,
        /// <summary>
        /// 付款确认
        /// </summary>
        [Enum("付款确认")]
        Pay = 13,
        /// <summary>
        /// 等待发货
        /// </summary>
        [Enum("等待发货")]
        WaitForConsignment = 14,
        /// <summary>
        /// 发货完成订单
        /// </summary>
        [Enum("发货完成订单")]
        Consignment = 15,
        /// <summary>
        /// 退款
        /// </summary>
        [Enum("退款")]
        Refundment = 16,
        /// <summary>
        /// 缺货通知
        /// </summary>
        [Enum("缺货通知")]
        Shortage = 17,
        /// <summary>
        /// 催款并通知
        /// </summary>
        [Enum("催款并通知")]
        NotifyAndPayment = 18,
        /// <summary>
        /// 评论推荐
        /// </summary>
        [Enum("评论推荐")]
        RemarkRecommend = 19,

        // Begin add by tianys at 2009-06-15
        /// <summary>
        /// 团购开始通知
        /// </summary>
        [Enum("团购开始通知")]
        GroupPurchaseNotify = 20,
        // End add

        /// <summary>
        /// 兑换礼券
        /// </summary>
        [Enum("兑换礼券")]
        ExchangeCoupon = 21,

        /// <summary>
        /// 好友代付
        /// </summary>
        [Enum("好友代付")]
        FriendPaid = 22,

        /// <summary>
        /// 留言回复
        /// </summary>
        [Enum("邮箱验证")]
        ValidEmail = 23,

        /// <summary>
        /// 邮件验证码
        /// </summary>
        [Enum("取消促销邮件")]
        CancelPromotionEmail = 24,

        /// <summary>
        /// 商品到货邮件
        /// </summary>
        [Enum("商品到货邮件")]
        GoodsMail = 25,

        /// <summary>
        /// 修改邮件
        /// </summary>
        [Enum("修改邮件")]
        ChangeMail = 26,

        /// <summary>
        /// App群发
        /// </summary>
        [Enum("App群发")]
        AppGroup = 27,

        /// <summary>
        /// 处方药短信通知
        /// </summary>
        [Enum("处方药短信通知")]
        PrescriptionMedicine = 28,

        /// <summary>
        /// 验光预约
        /// </summary>
        [Enum("验光预约")]
        Optometry = 29
    }
}
