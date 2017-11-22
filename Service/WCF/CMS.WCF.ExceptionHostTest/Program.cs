using System;
using System.Linq;
using CMS.WCF.ExceptionService;

namespace testException
{
    class Program
    {
        static void Main()
        {
            //TaskWork.Start();
            Process.ProcessNoSend(10);
            Console.ReadKey();
            
        }

        static void UpdateOrderState()
        {
            var client = new Framework.WCF.ServiceClient<Keede.WcfAdmin.Contract.IKeedeAdmin>("EyeseeEndPoint");
            var orderids = new[] { new Guid("A8351DCD-C100-47BC-AB50-5A8207F86CE1")};
            foreach (var id in orderids)
            {
                var result = client.Instance.UpdateOrderState(Guid.NewGuid(), id, Keede.Ecsoft.Enum.OrderState.PrintCompleted);
                if (result.IsSuccess)
                {
                    Console.WriteLine(id);
                }
            }
            Console.Read();
        }
    }
}
