using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Keede.Ecsoft.Enum;
using Keede.Ecsoft.Model;
using Keede.ForEyesee.Contract;
using Keede.ForEyesee.Service;

namespace Keede.ForEyesee.ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IEyesee iEyesee = new Eyesee();
            var data = iEyesee.AddShopFrontApplyStock("","");
            Console.WriteLine(data);
            Console.Read();
        }

        //OutPutMenu();

        //    while (true)
        //    {
        //        string r = Console.ReadLine();

        //        if (string.IsNullOrEmpty(r)) continue;

        //        int v;

        //        if (!int.TryParse(r, out v)) continue;

        //        if (v == -1) break;
        //        if (v == -2)
        //        {
        //            Console.Clear();
        //            OutPutMenu();
        //        }

        //        KeedeMethodName tc = (KeedeMethodName)v;
        //        Guid gid = new Guid("FC2E4B6A-7E8E-4579-8613-86140031775E");
        //        Guid cid = Guid.NewGuid();
        //        object value;
        //        List<Guid> lg = new List<Guid>();
        //        lg.Add(new Guid("1BE03AAA-B8D7-4591-AD52-629A43462F1A"));
        //        lg.Add(new Guid("26FF2C28-EF98-4369-A4FF-F5924ADBFE68"));
        //        lg.Add(new Guid("305100F4-B736-4E0C-8E11-A65ED7167EBF"));
        //        lg.Add(new Guid("9E3C8AFA-2703-419F-B838-C89CC44C424F"));
        //        lg.Add(new Guid("4D6AAA24-1153-4CCC-8446-C5C51D45D589"));
        //        lg.Add(new Guid("DF8968E4-879A-43DB-8521-D1D27C2D01A0"));
        //        lg.Add(new Guid("062DF8B8-2FA6-4883-98C6-8C2836E632C4"));

        //        Console.WriteLine();
        //        Console.WriteLine();
        //        Console.WriteLine("Command:" + tc);
        //        Console.WriteLine();
        //        Console.WriteLine();

        //        switch (tc)
        //        {
        //            //case KeedeMethodName.DeleteGoodsOrderDetail:
        //            //    value = Client.DeleteGoodsOrderDetail(gid, cid);
        //            //    Result(value);

        //            //    break;
        //            case KeedeMethodName.DemandQuantity:
        //                value = Client.DemandQuantity(gid, OrderState.UnVerify, OrderState.UnVerify, cid);
        //                Result(value);

        //                break;
        //            case KeedeMethodName.GetAllGoodsClass:
        //                value = Client.GetAllGoodsClass();
        //                Result(value);

        //                break;
        //            case KeedeMethodName.GetGoodsBaseListByClassId:
        //                value = Client.GetGoodsBaseListByClassId(new Guid("b283b4b2-bd6c-41de-b15a-c0017325589c"),
        //                                                         GoodsAffiliationClassType.Inclusive,
        //                                                         GoodsCombinationType.TopGoods
        //                                                         , null, null, SearchType.UnexactSearch);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.GetGoodsBaseListBySearch:
        //                //value  = Client.getgoods
        //                break;
        //            case KeedeMethodName.GetGoodsList:
        //                value = Client.GetGoodsList(new Guid("894FB563-FCF0-4BD9-B304-EED7F37EEC8E"),
        //                                            GoodsAffiliationClassType.Inclusive);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.GetGoodsOrderByOrderId:
        //                value = Client.GetGoodsOrderByOrderId(gid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.GetGoodOrderState:
        //                value = Client.GetGoodsOrderState(gid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.GetStockandRequestEyesee:
        //                value = Client.GetStockandRequestEyesee(gid, new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"));
        //                Result(value);
        //                break;
        //            case KeedeMethodName.GetStockByGoodIDEyesee:
        //                value = Client.GetStockByGoodIDEyesee(gid);
        //                Console.WriteLine(gid + "库存数量：" + value);

        //                break;
        //            case KeedeMethodName.Insert:
        //                CreateOrder();
        //                break;
        //            case KeedeMethodName.InsertGoodsOrderDetail:
        //                value = Client.InsertGoodsOrderDetail(GetOrderDetail(gid), cid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.InsertWastebook:
        //                WasteBookInfo wbi = new WasteBookInfo(Guid.NewGuid(),
        //                                                      new Guid("4F00D94F-C117-4904-88A6-816497F9D4F0"),
        //                                                      "testOrderid" + DateTime.Now.Millisecond, "clew", 20,
        //                                                      (int)AuditingState.Yes, (int)WasteBookType.TransferIn);

        //                value = Client.InsertWastebook(wbi, cid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.SetCancleReason:
        //                value = Client.SetCancleReason(gid, "\npengyan 财务受理--已支付受理 已核实9/5钱款到账，交易号：2011090585334733，支付宝账户：蒋莉云 395226685@qq.com[2011/9/5 10:57:46]", gid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.SetGoodsState:
        //                value = Client.SetGoodsState(new List<Guid> { gid }, 1, cid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.SetOrderClew:
        //                value = Client.SetOrderClew(gid, "clew", cid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.SetOrderPayState:
        //                value = Client.SetOrderPayState(gid, PayState.Paid, cid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.SetOrderState:
        //                value = Client.SetOrderState(gid, OrderState.UnVerify, cid);
        //                Result(value);
        //                break;
        //            case KeedeMethodName.UpdateComplete:
        //                value = Client.UpdateComplete(GetOrder(), Guid.NewGuid());
        //                Result(value);
        //                break;
        //            case KeedeMethodName.InsertGoodsOrderInvoice:
        //                InvoiceInfo info = new InvoiceInfo(Guid.NewGuid(), Guid.NewGuid(), "invoiceName",
        //                                                   "invoiceContent", "reciver", "", "address", DateTime.Now,
        //                                                   32, 1,DateTime.Now,InvoicePurchaserType.Other);
        //                var orderIds = new Guid[]
        //                                      {
        //                                          Guid.NewGuid(),
        //                                          Guid.NewGuid(),
        //                                          Guid.Empty,
        //                                      };
        //                value = Client.InsertGoodsOrderInvoice(info, orderIds, Guid.NewGuid());
                        
        //                Result(value);
        //                break;
        //            case KeedeMethodName.SetGoodsOrderInvoiceState:

        //                value = Client.SetGoodsOrderInvoiceState(new Guid("a3579a59-af87-4fa1-bd8d-544be44e6697"), InvoiceState.Cancel, "[订单修改生成新单,发票作废,生成新的发票]",Guid.NewGuid());

        //                Result(value);
        //                break;
        //            case KeedeMethodName.IsExistByOrderNoAndPaystate:
        //                value = Client.IsExistByOrderNoAndPaystate("RTEST02020615009");
        //                Result(value);
        //                break;
                                             
        //            default:
        //                OutPutMenu();
        //                break;
        //        }
        //    }
        //}

        //private static void OutPutMenu()
        //{
        //    Console.WriteLine("----------------------------------------------------------------------");
        //    Console.WriteLine("-1. Exit.");
        //    Console.WriteLine("-2. Clear Screen.");
        //    Console.WriteLine();
            
        //    Command(KeedeMethodName.InsertWastebook);
        //    Command(KeedeMethodName.GetStockandRequestEyesee);
        //    Command(KeedeMethodName.Insert);
        //    Command(KeedeMethodName.DemandQuantity);
        //    Command(KeedeMethodName.UpdateComplete);
        //    //Command(KeedeMethodName.DeleteGoodsOrderDetail);
        //    Command(KeedeMethodName.InsertGoodsOrderDetail);
        //    Command(KeedeMethodName.SetOrderState);
        //    Command(KeedeMethodName.SetCancleReason);
        //    Command(KeedeMethodName.SetOrderPayState);
        //    Command(KeedeMethodName.SetOrderClew);
        //    Command(KeedeMethodName.GetAllGoodsClass);
        //    Command(KeedeMethodName.GetGoodsBaseListBySearch); 
        //    Command(KeedeMethodName.GetGoodsBaseListByClassId);
        //    Command(KeedeMethodName.GetGoodsList);
        //    Command(KeedeMethodName.GetCombGoodsListForEyeSee);
        //    Command(KeedeMethodName.GetGoodOrderState);
        //    Command(KeedeMethodName.GetGoodsOrderByOrderId);
        //    Command(KeedeMethodName.GetStockByGoodIDEyesee);
        //    Command(KeedeMethodName.SetGoodsState);
        //    Command(KeedeMethodName.InsertGoodsOrderInvoice);
        //    Command(KeedeMethodName.SetGoodsOrderInvoiceState);
        //    Command(KeedeMethodName.SetGoodsOrderInvoiceSum);
        //    Command(KeedeMethodName.SetGoodsOrderInvoiceToNew);
        //    Command(KeedeMethodName.IsExistByOrderNoAndPaystate);
            
        //    Console.WriteLine("----------------------------------------------------------------------");
        //}

        //private static void Command(KeedeMethodName tc)
        //{
        //    Console.WriteLine(string.Format("{0:D2}. {1}.", (int)tc, tc));
        //}

        //private static void Result(object v)
        //{
        //    if (v == null)
        //    {
        //        Console.WriteLine("Reuslt : Null");
        //    }
        //    if (v is WCFReturnInfo)
        //    {
        //        var wcf = v as WCFReturnInfo;

        //        Console.WriteLine(string.Format("Reuslt : {0} {1}", wcf.IsSuccess, wcf.ErrorMessage));
        //    }
        //    else if (v is IEnumerable)
        //    {
        //        var lists = v as IEnumerable;

        //        StringBuilder sb = new StringBuilder();
        //        foreach (var list in lists)
        //        {
        //            sb.Append(list.ToString()).Append("\r\n");
        //        }
        //        Console.WriteLine(string.Format("Reuslt : {0}", sb.Length == 0 ? "EMPTY" : sb.ToString()));
        //    }
        //    else
        //    {
        //        Console.WriteLine("Reuslt : " + v);
        //    }
        //}

        //static GoodsOrderInfo GetOrder()
        //{
        //    GoodsOrderInfo goi = new GoodsOrderInfo();

        //    goi.BankAccountsId = Guid.NewGuid();
        //    goi.BankTradeNo = string.Empty;

        //    goi.Consignee = "TestForEyesee";
        //    goi.CountryId = Guid.NewGuid();
        //    goi.CancleReason = string.Empty;
        //    goi.Carriage = 5;
        //    goi.Clew = string.Empty;
        //    goi.CommissionFailure = false;
        //    goi.ConsignTime = DateTime.MaxValue;
        //    goi.CouponList = null;
        //    goi.CityId = Guid.NewGuid();

        //    goi.Direction = "TestForEyesee";

        //    goi.EffectiveTime = DateTime.MaxValue;
        //    goi.Express = "";
        //    goi.ExpressNo = string.Empty;
        //    goi.ExpressId = Guid.NewGuid();

        //    goi.FilialeId = Guid.NewGuid();
        //    goi.InvoiceState = 1;
        //    goi.LatencyDay = 0;

        //    goi.MemberId = Guid.NewGuid();
        //    goi.Memo = string.Empty;
        //    goi.Mobile = "159000000000";

        //    goi.OrderId = Guid.NewGuid();
        //    goi.OrderNo = GetCode();
        //    goi.OrderTime = DateTime.Now;
        //    goi.OldCustomer = 0;
        //    goi.OrderTime = DateTime.Now;
        //    goi.OrderState = 2;

        //    goi.PaidUp = 0;
        //    goi.PaymentByBalance = 0;
        //    goi.Phone = string.Empty;
        //    goi.PickNo = "";//
        //    goi.PostalCode = string.Empty;
        //    goi.PromotionDescription = string.Empty;
        //    goi.PromotionValue = 0;
        //    goi.ProvinceId = Guid.NewGuid();
        //    goi.PayMode = 0;
        //    goi.PayState = 0;
        //    goi.PayType = 0;

        //    goi.RefundmentMode = 0;
        //    goi.RealTotalPrice = 500;

        //    goi.TotalPrice = 500;
        //    goi.WarehouseId = Guid.NewGuid();

        //    return goi;
        //}

        //static void Record(GoodsOrderInfo goi, Guid cmdid)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.AppendFormat("OrderId:{0}\r\n", goi.OrderId)
        //        .AppendFormat("OrderNo:{0}\r\n", goi.OrderNo)
        //        .AppendFormat("MemberID:{0}\r\n", goi.MemberId)
        //        .AppendFormat("CommandID:{0}\r\n", cmdid);

        //    Console.WriteLine(sb.ToString());
        //}

        //static GoodsOrderDetailInfo[] GetOrderDetail(Guid oid)
        //{
        //    GoodsOrderDetailInfo temp = new GoodsOrderDetailInfo
        //                                    {
        //                                        CompGoodsId = Guid.Empty,
        //                                        CompIndex = 0,
        //                                        CurrencyId = Guid.NewGuid(),
        //                                        GoodsCode = "",
        //                                        GoodsId = Guid.NewGuid(),
        //                                        GoodsName = "GoodsName",
        //                                        MarketPrice = 50,
        //                                        OrderId = oid,
        //                                        Quantity = 1,
        //                                        SellPrice = 40,
        //                                        Specification = string.Empty,
        //                                        SubScore = 40,
        //                                        Subtotal = 40
        //                                    };

        //    return new[] { temp };
        //}

        //static void CreateOrder()
        //{
        //    GoodsOrderInfo goi = GetOrder();

        //    Guid cmdid = Guid.NewGuid();

        //    try
        //    {
        //        var s = Client.Insert(goi, GetOrderDetail(goi.OrderId), cmdid);

        //        Record(goi, cmdid);

        //        if (!s.IsSuccess)
        //            EyeseeLog.Log(s.ErrorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message + ex.GetType().FullName);
        //        EyeseeLog.Log(ex.Message + ex.GetType().FullName);
        //    }
        //}

        //static void CreateOrder(int i)
        //{
        //    if (i <= 0)
        //        i = 10;

        //    for (int j = 0; j < i; j++)
        //    {
        //        CreateOrder();
        //    }
        //}

        //static string GetCode()
        //{
        //    DateTime dateTime = DateTime.Now;

        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("LoadTest")
        //        .Append(dateTime.Month.ToString("D2"))
        //        .Append(dateTime.Day.ToString("D2"))
        //        .Append(dateTime.Hour.ToString("D2"))
        //        .Append(dateTime.Minute.ToString("D2"))
        //        .Append(dateTime.Second.ToString("D2"))
        //        .Append(GetCount);

        //    return sb.ToString();
        //}

        //private static int c = 0;
        //private static readonly object LockObject = new object();
        //static int GetCount
        //{
        //    get
        //    {
        //        int r;

        //        lock (LockObject)
        //        {
        //            r = c++;
        //        }

        //        return r;
        //    }
        //}

        //static IEyesee client;
        //static IEyesee Client
        //{
        //    get
        //    {
        //        if (client == null)
        //            client = new EyeseeClientHelper("NetTcpBinding_IEyesee");

        //        return client;
        //    }
        //}
    }
}