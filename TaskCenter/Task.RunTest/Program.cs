using System;
using ReckoningTask.Core;
using System.Text;
using StorageTask.Core;
using SaleFilialeGenerateStockInAndPurchaseForm;

namespace StorageTask.RunTest
{
    class Program
    {

        //internal class JsonSerialize : Framework.Core.Serialize.IJsonSerialize
        //{
        //    public T Deserialize<T>(string json)
        //    {
        //        try
        //        {
        //            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        //        }
        //        catch (Exception ex)
        //        {
        //            //LogService.LogError("反序列化异常", "记录日志", ex, json);
        //        }
        //        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(null);
        //    }

        //    public string Serialize(object value)
        //    {
        //        try
        //        {
        //            return Newtonsoft.Json.JsonConvert.SerializeObject((value));
        //        }
        //        catch (Exception ex)
        //        {
        //            //LogService.LogError("序列化异常", "记录日志", ex, value);
        //        }
        //        return Newtonsoft.Json.JsonConvert.SerializeObject(null);
        //    }
        //}
        static void Main(string[] args)
        {
            //
            //ERP.SAL.LogCenter.LogService.LogError("message", "Tag", new Exception("error"), null);

            //初始化数据库连接对象
            //StorageTask.Core.ConfigManager.InitERPConnectionName(ERP.Environment.GlobalConfig.DB.GetConnectionName(ERP.Environment.GlobalConfig.DB.FromType.Write));
            //StorageTask.Core.ConfigManager.Init();

            //SaleFilialeGenerateStockInAndPurchaseFormTaskConfig.Init();
            //GenerateSaleFilialeStockInAndPurchaseFormTask.Generate();

            //AddStorageTaskManager.AddSellOutTask();
            //ReckoningTaskManager.AddTask();

            //Framework.Core.Log.LogInitializer.StartInitialize()
            //        .InitJsonSerialize(new JsonSerialize()) //初始化JSON序列化
            //        .InitKMQConfig(2, "OCLXUK61", "192.168.117.210:9999", "LogQueue") //初始化KMQ的配置，对记录日志中心有效
            //        .InitLocalFileConfig(20971520, Framework.Core.Log.LogLevel.Warn, Framework.Core.Log.LogLevel.Error, Framework.Core.Log.LogLevel.Fatal) //初始化本地日志记录配置，可选配置，如未配置不会对日志内容记录到本地文件上
            //        .InitSystemSourceID(new Guid("579175a5-a61b-4717-afff-db788494ae2a"));//初始化来源系统ID，对记录日志中心有效
            //var log = LogCenter.Client.Factory.CreateLogger(Framework.Core.Log.LogInitializer.Config);
            //log.Log(Framework.Core.Log.LogLevel.Error, "发生异常了", "测试异常日志", new Exception("Hello baby"), null);


            #region XXX
            Console.WriteLine("请选择运行服务：");
            //Console.WriteLine("1、应付款查询(采购入库统计) 2、结算价与销售额统计 3、商品毛利 4、公司毛利 5、自动报备 6、供应商资质检索");
            //Console.WriteLine("21、商品结算价存档 历史数据 22、供应商月销售额存档 历史数据 23、商品毛利记录存档 历史数据 24、公司毛利记录存档 历史数据 25、处理日销量表中AvgSettlePrice 历史数据");
            //Console.WriteLine("31、商品结算价存档处理(当前系统时间的上一个月的结算价) 32、供应商月销售额存档(当前系统时间的上一个月的销售额) 33、商品毛利记录存档(当前系统时间的上一个月的商品毛利) 34、公司毛利记录存档(当前系统时间的上一个月的公司毛利) 35、处理日销量表中AvgSettlePrice(当前系统时间的上一个月的数据)");
            //Console.WriteLine("36、WMS系统上线后，作废订单销量没有扣除");
            Console.WriteLine("23、商品毛利记录存档 历史数据 24、公司毛利记录存档 历史数据");
            var input = Console.ReadLine();
            Console.WriteLine("任务开始...");
            var startTime = DateTime.Now;
            try
            {
                int type = string.IsNullOrEmpty(input) ? 1 : Convert.ToInt32(input);
                switch (type)
                {
                    //case 1: //应付款查询
                    //    GoodsStockRecordTask.Core.SupplierPaymentsAndPurchasingManager.RunSupplierPaymentsAndPurchasingTask();
                    //    break;
                    //case 2: //结算价与销售额统计
                    //    GoodsStockRecordTask.Core.GoodsStockRecordTaskManager.RunGoodsStockRecordTask();
                    //    break;
                    //case 3: //商品毛利
                    //    GoodsStockRecordTask.Core.GoodsGrossProfitManager.RunGoodsGrossProfitTask();
                    //    break;
                    //case 4: //公司毛利
                    //    GoodsStockRecordTask.Core.CompanyGrossProfitManager.RunCompanyGrossProfitTask();
                    //    break;
                    //case 5: //自动报备
                    //    AutoPurchasing.Core.AutoPurchasing.RunTask();
                    //    break;
                    //case 6: //供应商资质检索
                    //    GoodsStockRecordTask.Core.QualificationTaskManager.RunQualificationTask();
                    //    break;

                    //case 21: //商品结算价存档 历史数据
                    //    HistoryData.HistorySettlePrice.RunHistorySettlePriceTask();
                    //    break;
                    //case 22: //供应商月销售额存档 历史数据
                    //    HistoryData.HistorySettlePrice.RunHistorySupplierSaleTask();
                    //    break;
                    case 23: //商品毛利记录存档 历史数据
                        HistoryData.HistorySettlePrice.RunHistoryGoodsGrossProfitTask();
                        break;
                    case 24: //公司毛利记录存档 历史数据
                        HistoryData.HistorySettlePrice.RunHistoryCompanyGrossProfitTask();
                        break;
                    //case 25: //处理日销量表中AvgSettlePrice 历史数据
                    //    HistoryData.HistorySettlePrice.RunHistoryGoodsDaySalesStatisticsTask();
                    //    break;

                    //case 31: //商品结算价存档处理(当前系统时间的上一个月的结算价)
                    //    HistoryData.HistorySettlePrice.RunHistorySettlePriceTaskForPreMonth();
                    //    break;
                    //case 32: //供应商月销售额存档(当前系统时间的上一个月的销售额)
                    //    HistoryData.HistorySettlePrice.RunHistorySupplierSaleTaskForPreMonth();
                    //    break;
                    //case 33: //商品毛利记录存档(当前系统时间的前一天的商品毛利)
                    //    HistoryData.HistorySettlePrice.RunHistoryGoodsGrossProfitTaskForPreDay();
                    //    break;
                    //case 34: //公司毛利记录存档(当前系统时间的前一天的公司毛利)
                    //    HistoryData.HistorySettlePrice.RunHistoryCompanyGrossProfitTaskForPreDay();
                    //    break;
                    //case 35: //处理日销量表中AvgSettlePrice(当前系统时间的上一个月的数据)
                    //    HistoryData.HistorySettlePrice.RunHistoryGoodsDaySalesStatisticsTaskForPreMonth();
                    //    break;

                    //case 36: //处理日销量表中AvgSettlePrice(当前系统时间的上一个月的数据)
                    //    HistoryData.HistorySettlePrice.RunHistoryGoodsDaySalesStatisticsForZuoFeiTask();
                    //    break;

                    //case 37: //处理完成时间超过一个自然月或一个自然月以上的数据
                    //    HistoryData.HistorySettlePrice.CompanyGrossProfitRecordInfosForMoreMonth();
                    //    break;
                    //case 38: //处理完成时间超过一个自然月或一个自然月以上的数据
                    //    HistoryData.HistorySettlePrice.GoodsGrossProfitInfosForMoreMonth();
                    //    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("任务结束... 总耗时：{0}", (DateTime.Now - startTime).TotalMinutes);
            Console.ReadKey();
            #endregion

            //自动报备
            //Console.WriteLine("自动报备服务运行中...");
            //AutoPurchasing.Core.AutoPurchasing.RunTask();
            //Console.WriteLine("自动报备服务运行完成...");
            //进货申报

            //Console.WriteLine("进货申报服务运行中...");
            //AutoStockDeclare.Core.StockDeclareTask.RunStockDeclare();
            //var msg = new StringBuilder();

            //StockDeclareManager.AutoDeclare(DateTime.Now, ref msg);
            //Console.WriteLine("进货申报服务运行完成... ,详细信息：");

            //完成订单
            //CompleteOrderTask.Core.CompleteSecondTask.RunWaitConsignmentOrderTask();
            //生成往来账
            //ReckoningTaskManager.AddTask();
            //订单发货率
            //Console.WriteLine("订单发货率运行中...");
            //AddStorageTaskManager.AddSellOutTask();

            //CompleteOrderTask.Core.CompleteSecondTask.RunWaitConsignmentOrderTask();
            //Console.ReadLine();
        }
    }
}
