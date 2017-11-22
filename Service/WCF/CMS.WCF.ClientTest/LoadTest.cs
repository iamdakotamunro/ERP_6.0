using System;
using System.Text;
using System.Threading;
using Keede.Ecsoft.Model;
using Keede.ForEyesee.Service;

namespace Keede.ForEyesee.ClientTest
{
    public class LoadTest
    {
        public static int SucceCount = 0;
        public static int ExecuteCount = 0;
        static void Main(string[] args)
        {
            EyeseeLog.ShowMessage = Log;

            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            StartThread(1000 * 60);
            //StartThread(1000 * 60 * 5);
            //StartThread(1000 * 60 * 10);
            //StartThread(1000 * 60 * 30);
            //StartThread(1000 * 60 * 60);

            Console.ReadLine();
        }

        public static void StartThread(long interval)
        {
            Thread t1 = new Thread(new TestLoad(interval).Run);
            t1.Start();
        }


        static void Log(string msg)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine(msg);
            Console.WriteLine();
            Console.WriteLine("------------------------------");
        }
    }

    public class TestLoad
    {
        private readonly long interval;
        private readonly int count;
        private static int gCount = 0;
        private System.Timers.Timer timer;
        private readonly IEyesee client;

        public TestLoad(long inter)
        {
            interval = inter;
            count = gCount++;
            client = new EyeseeClientHelper("NetTcpBinding_IEyesee");
        }

        private string Name
        {
            get
            {
                return ToString() + count;
            }
        }

        protected void TimerEventFunction(Object sender, System.Timers.ElapsedEventArgs e)
        {
            CreateOrder();
        }

        public void Run()
        {
            if (timer == null)
            {
                timer = new System.Timers.Timer(interval);
                timer.Elapsed += TimerEventFunction;
                timer.AutoReset = true;
                timer.Enabled = true;
                EyeseeLog.Log("Start Thread : " + Name);
            }
        }



        static GoodsOrderInfo GetOrder()
        {
            GoodsOrderInfo goi = new GoodsOrderInfo();

            goi.BankAccountsId = Guid.NewGuid();
            goi.BankTradeNo = string.Empty;

            goi.Consignee = "TestForEyesee";
            goi.CountryId = Guid.NewGuid();
            goi.CancleReason = string.Empty;
            goi.Carriage = 5;
            goi.Clew = string.Empty;
            goi.CommissionFailure = false;
            goi.ConsignTime = DateTime.MaxValue;
            goi.CouponList = null;
            goi.CityId = Guid.NewGuid();

            goi.Direction = "TestForEyesee";

            goi.EffectiveTime = DateTime.MaxValue;
            goi.Express = "";
            goi.ExpressNo = string.Empty;
            goi.ExpressId = Guid.NewGuid();

            goi.FilialeId = Guid.NewGuid();
            goi.InvoiceState = 1;
            goi.LatencyDay = 0;

            goi.MemberId = Guid.NewGuid();
            goi.Memo = string.Empty;
            goi.Mobile = "159000000000";

            goi.OrderId = Guid.NewGuid();
            goi.OrderNo = GetCode();
            goi.OrderTime = DateTime.Now;
            goi.OldCustomer = 0;
            goi.OrderTime = DateTime.Now;
            goi.OrderState = 2;

            goi.PaidUp = 0;
            goi.PaymentByBalance = 0;
            goi.Phone = string.Empty;
            goi.PickNo = "";//
            goi.PostalCode = string.Empty;
            goi.PromotionDescription = string.Empty;
            goi.PromotionValue = 0;
            goi.ProvinceId = Guid.NewGuid();
            goi.PayMode = 0;
            goi.PayState = 0;
            goi.PayType = 0;

            goi.RefundmentMode = 0;
            goi.RealTotalPrice = 500;

            goi.TotalPrice = 500;
            goi.WarehouseId = Guid.NewGuid();

            return goi;
        }

        static GoodsOrderDetailInfo[] GetOrderDetail(GoodsOrderInfo goi)
        {
            GoodsOrderDetailInfo temp = new GoodsOrderDetailInfo
            {
                CompGoodsId = Guid.Empty,
                CompIndex = 0,
                CurrencyId = Guid.NewGuid(),
                GoodsCode = "",
                GoodsId = Guid.NewGuid(),
                GoodsName = "GoodsName",
                MarketPrice = 50,
                OrderId = goi.OrderId,
                Quantity = 1,
                SellPrice = 40,
                Specification = string.Empty,
                SubScore = 40,
                Subtotal = 40
            };

            return new[] { temp };
        }

        void CreateOrder()
        {
            GoodsOrderInfo goi = GetOrder();

            Guid cmdid = Guid.NewGuid();

            EyeseeLog.Log(Name + ":" + goi.OrderId + "" + "----------SucceCount:" + LoadTest.SucceCount + "----Execute.Count:" + LoadTest.ExecuteCount);

            try
            {
                LoadTest.ExecuteCount++;
                var s = client.Insert(goi, GetOrderDetail(goi), cmdid);

                if (!s.IsSuccess)
                    EyeseeLog.Log("server error:" + s.ErrorMessage);
                else
                    LoadTest.SucceCount++;

            }
            catch (Exception ex)
            {
                EyeseeLog.Log("local error : " + ex.Message + ex.GetType().FullName);
            }
        }

        static string GetCode()
        {
            DateTime dateTime = DateTime.Now;

            StringBuilder sb = new StringBuilder();

            sb.Append("LoadTest")
                .Append(dateTime.Month.ToString("D2"))
                .Append(dateTime.Day.ToString("D2"))
                .Append(dateTime.Hour.ToString("D2"))
                .Append(dateTime.Minute.ToString("D2"))
                .Append(dateTime.Second.ToString("D2"))
                .Append(GetCount);

            return sb.ToString();
        }

        private static int c = 0;
        private static readonly object LockObject = new object();
        static int GetCount
        {
            get
            {
                int r;

                lock(LockObject)
                {
                    r = c++;
                }

                return r;
            }
        }
    }
}