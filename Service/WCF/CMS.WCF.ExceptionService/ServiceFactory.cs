using Framework.WCF;
using Keede.WcfAdmin.Contract;

namespace CMS.WCF.ExceptionService
{
   internal class ServiceFactory
    {
        public class WCF
        {
            public static ServiceClient<IKeedeAdmin> NewKeedeClient
            {
                get
                {
                    return new ServiceClient<IKeedeAdmin>(Config.WCF.EndPoint_NewKeede);
                }
            }

            public static ServiceClient<IKeedeAdmin> EyeseeClient
            {
                get { return new ServiceClient<IKeedeAdmin>(Config.WCF.EndPoint_Eyesee); }
            }

            public static ServiceClient<IKeedeAdmin> BaishopClient
            {
                get { return new ServiceClient<IKeedeAdmin>(Config.WCF.EndPoint_Baishop); }
            }

            public static ServiceClient<IKeedeAdmin> KeedeMisClient
            {
                get { return new ServiceClient<IKeedeAdmin>(Config.WCF.EndPoint_MIS); }
            }
        }
    }
}
