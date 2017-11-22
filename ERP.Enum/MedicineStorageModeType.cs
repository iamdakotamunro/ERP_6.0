using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// 储存方式
    /// </summary>
    public enum MedicineStorageModeType
    {
        [Enum("常温库")]
        Normal = 1,
        [Enum("阴凉库")]
        Shade = 2,
        [Enum("冷藏库")]
        Cold = 3
    }
}
