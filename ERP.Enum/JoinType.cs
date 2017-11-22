using ERP.Enum.Attribute;

namespace ERP.Enum
{
    ///<summary>
    /// 加盟方式
    ///</summary>
    public enum JoinType
    {
        /// <summary>
        /// 直营
        /// </summary>
        [Enum("直营")]
        Self = 1,
        /// <summary>
        /// 他营
        /// </summary>
        [Enum("他营")]
        Other = 2,
    }
}
