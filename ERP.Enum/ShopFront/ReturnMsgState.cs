using System;
using ERP.Enum.Attribute;
namespace ERP.Enum.ShopFront
{
    /// <summary>
    /// 联盟店退货留言申请状态
    /// </summary>
    [Serializable]
    public enum  ReturnMsgState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Enum("全部")]
        All = -1,

        /// <summary>
        /// 待审核
        /// </summary>
        [Enum("待审核")]
        CheckPending = 0,

        /// <summary>
        /// 审核通过
        /// </summary>
        [Enum("审核通过")]
        Pass = 1,

        /// <summary>
        /// 不通过
        /// </summary>
        [Enum("不通过")]
        NoPass = 2,
    }
}
