using ERP.Enum.Attribute;

namespace ERP.Enum
{
    public enum OperateType
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Enum("新增")]
        Add = 1,

        /// <summary>
        /// 编辑
        /// </summary>
        [Enum("编辑")]
        Edit = 2,

        /// <summary>
        /// 删除
        /// </summary>
        [Enum("删除")]
        Delete = 3,
    }
}
