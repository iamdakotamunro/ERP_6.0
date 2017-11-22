using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Keede.ForEyesee.Service;

namespace Keede.ForEyesee.ClientTest
{
    public class NewTest
    {
        private static EyeseeClientHelper cs;

        static void Main(string[] args)
        {
            if (cs == null)
            {
                IPAddress ip = Dns.GetHostAddresses("127.0.0.1")[0];
                cs = new EyeseeClientHelper(ip, 9541);
                //cs.Open();
            }

            OutPutMenu();

            string r = Console.ReadLine();

            bool loop = true;
            while (loop)
            {
                switch (r)
                {
                    case "1":
                        int returnValue = cs.TestMethod(100);
                        Console.WriteLine(returnValue);
                        OutPutMenu();
                        break;

                    case "10":
                        loop = false;
                        //cs.Close();
                        break;
                }
                r = Console.ReadLine();
            }
        }

        private static void OutPutMenu()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("1. CreateNewOrder.");
            Console.WriteLine();
            Console.WriteLine("2. Create Load Test .");
            Console.WriteLine();
            Console.WriteLine("10. Exit.");
        }
    }
}