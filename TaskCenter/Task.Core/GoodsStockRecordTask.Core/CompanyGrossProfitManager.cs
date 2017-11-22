using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using System;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.Environment;
using ERP.BLL.Implement.Inventory;
using Framework.Core.Utility;
using Config.Keede.Library;

namespace GoodsStockRecordTask.Core
{
    public class CompanyGrossProfitManager
    {
        static readonly IGoodsOrderDetail _goodsOrderDetail = new GoodsOrderDetail(GlobalConfig.DB.FromType.Write);
        static readonly ICompanyGrossProfitRecord _companyGrossProfitRecord = new CompanyGrossProfitRecordDao();
        static readonly ICompanyGrossProfitRecordDetail _companyGrossProfitRecordDetail = new CompanyGrossProfitRecordDetailDao();
        static readonly IWasteBookReport _wasteBookReport = new WasteBookDao();
        static readonly CompanyGrossProfitRecordBll _companyGrossProfitRecordBll = new CompanyGrossProfitRecordBll(_goodsOrderDetail, _companyGrossProfitRecord, _companyGrossProfitRecordDetail, _wasteBookReport);

        static int _excuteday;

        /// <summary>
        /// 生成公司毛利记录存档
        /// </summary>
        public static void RunCompanyGrossProfitTask()
        {
            try
            {
                var date = DateTime.Now;
                if (date.Day == _excuteday) return;
                var createStockHour = ConfManager.GetAppsetting("CreateStockHour");
                var hours = createStockHour.Split(',');
                if (!hours.Contains(string.Format("{0}", date.Hour))) return;
                string errorMsg;
                //6月份算4月份存档
                var result = _companyGrossProfitRecordBll.AddCompanyGrossProfitRecord(date, out errorMsg);
                if (!result)
                {
                    ERP.SAL.LogCenter.LogService.LogError("生成公司毛利记录失败", "公司毛利记录", new Exception(errorMsg));
                }
                else
                {
                    _excuteday = date.Day;
                }
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError("生成公司毛利异常", "公司毛利记录", exp);
            }
        }
    }
}
