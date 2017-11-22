using Microsoft.Practices.Unity;

namespace ERP.Environment
{
    public static class IocInstance
    {
       static readonly IUnityContainer _container = new UnityContainer();

        public static void Register<T>(string name,T t)
        {
            _container.RegisterInstance(name,t);
        }

        public static T Resolve<T>(string name)
        {
            if (_container.IsRegistered<T>(name))
            {
                return _container.Resolve<T>(name);
            }
            return default(T);
        }
    }
}
