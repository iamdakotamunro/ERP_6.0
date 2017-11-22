using System;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using Framework.Core.Utility;

namespace GoodsStockRecordTask.Core
{
    /// <summary>
    /// 供应商应付款、采购入库存档
    /// </summary>
    public class SupplierPaymentsAndPurchasingManager
    {
        static readonly ISupplierReport _supplierReport = new SupplierReportDao();

        static readonly IReckoning _reckoning = new Reckoning(ERP.Environment.GlobalConfig.DB.FromType.Read);

        private static readonly SupplierReportBll _supplierReportBll = new SupplierReportBll(_supplierReport, _reckoning);

        private static int _day;
        /// <summary>应付款每月金额、入库统计金额存档任务
        /// </summary>
        public static void RunSupplierPaymentsAndPurchasingTask()
        {
            string title = "";
            //执行时间点  每月1号  0 - 8  每天成功运行一次就好
            try
            {
                var date = DateTime.Now;
                var createStockHour = Configuration.AppSettings["CreateStockHour"];
                var hours = createStockHour.Split(',');
                if (!hours.Contains(string.Format("{0}", date.Hour))) return;

                title = "往来帐明细存档";

                if (_day == DateTime.Now.Day) return;
                //将需要统计的往来帐记录到SupplierReckoningRecord表中
                var result = _supplierReportBll.InsertRececkoning(date);
                if (!result)
                {
                    ERP.SAL.LogCenter.LogService.LogError(string.Format("往来帐明细存档失败:{0}", date), "往来帐报表存档", null);
                }
                else
                {
                    if (date.Day == 1)
                    {
                        title = "统计应付款金额和采购入库金额";
                        var rerunResult = _supplierReport.ReRunEveryData(date);
                        if (!rerunResult)
                        {
                            ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}年{1}月{2}日数据报表存档失败", date.Year, date.Month, 1), "月初应付款金额和采购入库金额");
                            return;
                        }
                    }
                    _day = DateTime.Now.Day;
                }
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError("应付款每月金额、入库统计金额存档任务异常", title, exp);
            }
        }
    }
}
