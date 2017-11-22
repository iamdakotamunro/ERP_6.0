using ERP.Enum.Attribute;

namespace ERP.Enum.Overseas
{  
    public enum DeliverType
    {
        /// <summary>
        /// 直邮
        /// </summary>
        [Enum("直邮")]
        Direct = 0,

        /// <summary>
        /// 转运
        /// </summary>
        [Enum("转运")]
        Transfer = 1
    }
}
