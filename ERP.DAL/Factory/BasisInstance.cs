using ERP.DAL.Implement.Basis;
using ERP.DAL.Interface.IBasis;

namespace ERP.DAL.Factory
{
    /// <summary>
    /// 基础数据类实例化
    /// </summary>
    public class BasisInstance:InstanceBase
    {

        /// <summary>
        /// 
        /// </summary>
        public static IUnits GetUnitsDao(Environment.GlobalConfig.DB.FromType fromType)
        {
             { return new Units(fromType); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IWebRudder GetWebRudderDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new WebRudder(fromType); }
        }
    }
}
