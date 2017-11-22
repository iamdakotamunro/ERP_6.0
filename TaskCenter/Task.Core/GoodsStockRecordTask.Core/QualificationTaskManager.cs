using System;
using System.Linq;
using ERP.BLL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Framework.Core.Utility;

namespace GoodsStockRecordTask.Core
{
    /// <summary>
    /// 供应商资质是否完整、是否过期管理
    /// 每天执行一次
    /// </summary>
    public class QualificationTaskManager
    {
        static readonly int _days = ERP.Environment.GlobalConfig.Expire;
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(ERP.Environment.GlobalConfig.DB.FromType.Write);
        static readonly IQualificationManager _qualification = new ERP.DAL.Implement.Company.QualificationManager(ERP.Environment.GlobalConfig.DB.FromType.Write);
        static readonly QualificationManager _qualificationManager = new QualificationManager();
        static int _currentDay;
        /// <summary>
        /// 供应商资质完整与过期维护
        /// </summary>
        public static void RunQualificationTask()
        {
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var createStockHour = Configuration.AppSettings["CreateStockHour"];
            var hours = createStockHour.Split(',');
            if (!hours.Contains(string.Format("{0}", date.Hour))) return;
            if (_currentDay == DateTime.Now.Day) return;
            try
            {
                var dataList = _companyCussent.GetSupplierGoodsInfos(CompanyType.Suppliers, State.Enable, string.Empty, 0, 0);
                if (dataList.Count > 0)
                {
                    foreach (var supplierGoodsInfo in dataList)
                    {
                        var infoList = _qualification.GetSupplierQualificationBySupplierId(supplierGoodsInfo.ID);
                        if (infoList.Count == 0)
                        {
                            if (supplierGoodsInfo.Complete == 0 && supplierGoodsInfo.Expire == "0")
                                continue;
                            _companyCussent.UpdateQualificationCompleteState(supplierGoodsInfo.ID, 0, "0");
                        }
                        else
                        {
                            var complete = _qualificationManager.IsComplete(infoList);
                            var expire = _qualificationManager.IsExpire(infoList, _days);
                            if (supplierGoodsInfo.Complete != complete || expire != supplierGoodsInfo.Expire)
                            {
                                _companyCussent.UpdateQualificationCompleteState(supplierGoodsInfo.ID, complete, expire);
                            }
                        }
                    }
                    _currentDay = DateTime.Now.Day;
                }
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}供应商资质完整与过期维护异常!", date), "供应商资质完整与过期维护", exp);
            }
        }
    }
}
