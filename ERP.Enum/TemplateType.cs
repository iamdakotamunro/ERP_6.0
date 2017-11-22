using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 模板类型枚举
    /// </summary>
    [Serializable]
    public enum TemplateType
    {
        /// <summary>
        /// 客服备注
        /// </summary>
        [Enum("客服备注")]
        ServiceRemark = 1,

        /// <summary>
        /// 退货短信
        /// </summary>
        [Enum("退货短信")]
        ReturnGoods = 2,
    }
}