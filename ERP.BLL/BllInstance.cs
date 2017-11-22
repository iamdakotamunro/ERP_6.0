using System;
using ERP.Environment;

namespace ERP.BLL
{
    public abstract class BllInstance<T>
    {
        public static T WriteInstance
        {
            get
            {
                const GlobalConfig.DB.FromType TYPE = GlobalConfig.DB.FromType.Write;
                var instance = IocInstance.Resolve<T>(TYPE.ToString());
                if (ReferenceEquals(instance, null))
                {
                    instance = (T)Activator.CreateInstance(typeof(T), TYPE);
                    IocInstance.Register(TYPE.ToString(), instance);
                    return instance;
                }
                return instance;
            }
        }

        public static T ReadInstance
        {
            get
            {
                const GlobalConfig.DB.FromType TYPE = GlobalConfig.DB.FromType.Read;
                var instance = IocInstance.Resolve<T>(TYPE.ToString());
                if (ReferenceEquals(instance,null))
                {
                    instance = (T)Activator.CreateInstance(typeof(T), TYPE);
                    IocInstance.Register(TYPE.ToString(), instance);
                    return instance;
                }
                return instance;
            }
        }
    }
}
