using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 仓库状态枚举
    /// </summary>
    public enum WarehouseState
    {
        /// <summary>
        /// 搁置
        /// </summary>
        [Enum("搁置")]
        Disable = 1,
        /// <summary>
        /// 启用
        /// </summary>
        [Enum("启用")]
        Enable = 2
    }
}
