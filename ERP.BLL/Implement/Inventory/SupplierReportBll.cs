using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Model.Report;

namespace ERP.BLL.Implement.Inventory
{
    public class SupplierReportBll
    {
        readonly ISupplierReport _reportDao;
        private readonly ICompanyCussent _companyCussent;
        private readonly IReckoning _reckoning;

        public SupplierReportBll(ISupplierReport supplierReport,ICompanyCussent companyCussent, IReckoning reckoning)
        {
            _reportDao = supplierReport;
            _companyCussent = companyCussent;
            _reckoning = reckoning;
        }

        public SupplierReportBll(ISupplierReport supplierReport,IReckoning reckoning)
        {
            _reportDao = supplierReport;
            _reckoning = reckoning;
        }

        #region  从存档的报表数据查询

        /// <summary>
        /// 应付款查询、公司对应每月的应付款金额报表数据  
        /// </summary>
        /// <param name="year"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectPaymentsReprotsGroupByFilialeId(int year,bool include)
        {
            var list = _reportDao.SelectPaymentsReprotsGroupByFilialeId(year)??new List<SupplierPaymentsReportInfo>();
            if (DateTime.Now.Year==year && include)
            {
                var result = new List<SupplierPaymentsReportInfo>();
                var filialeIds = new List<Guid>();
                int month = DateTime.Now.Month;
                var currentData = _reportDao.GetPaymentsByForFiliale();
                if (currentData!=null && currentData.Count>0)
                {
                    foreach (var info in currentData)
                    {
                        var item = list.FirstOrDefault(act => act.FilialeId == info.FilialeId) ??
                            new SupplierPaymentsReportInfo { FilialeId = info.FilialeId };
                        item.January += month == 1 ? info.TotalAmount : 0;
                        item.February += month == 2 ? info.TotalAmount : 0;
                        item.March += month == 3 ? info.TotalAmount : 0;
                        item.April += month == 4 ? info.TotalAmount : 0;
                        item.May += month == 5 ? info.TotalAmount : 0;
                        item.June += month == 6 ? info.TotalAmount : 0;
                        item.July += month == 7 ? info.TotalAmount : 0;
                        item.August += month == 8 ? info.TotalAmount : 0;
                        item.September += month == 9 ? info.TotalAmount : 0;
                        item.October += month == 10 ? info.TotalAmount : 0;
                        item.November += month == 11 ? info.TotalAmount : 0;
                        item.December += month == 12 ? info.TotalAmount : 0;

                        item.January1 += month == 1 ? info.TotalNoPayed : 0;
                        item.February2 += month == 2 ? info.TotalNoPayed : 0;
                        item.March3 += month == 3 ? info.TotalNoPayed : 0;
                        item.April4 += month == 4 ? info.TotalNoPayed : 0;
                        item.May5 += month == 5 ? info.TotalNoPayed : 0;
                        item.June6 += month == 6 ? info.TotalNoPayed : 0;
                        item.July7 += month == 7 ? info.TotalNoPayed : 0;
                        item.August8 += month == 8 ? info.TotalNoPayed : 0;
                        item.September9 += month == 9 ? info.TotalNoPayed : 0;
                        item.October10 += month == 10 ? info.TotalNoPayed : 0;
                        item.November11 += month == 11 ? info.TotalNoPayed : 0;
                        item.December12 += month == 12 ? info.TotalNoPayed : 0;
                        result.Add(item);
                        filialeIds.Add(item.FilialeId);
                    }
                    result.AddRange(list.Where(act => !filialeIds.Contains(act.FilialeId)));
                    return result;
                }
            }
            return list;
        }

        /// <summary>
        /// 应付款查询，根据公司和查询供应商的应付款明细  
        /// </summary>
        /// <param name="year"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectPaymentsReportsGroupByCompanyId(int year, Guid filialeId,string companyName,bool include)
        {
            var list= _reportDao.SelectPaymentsReportsGroupByCompanyId(year, filialeId,companyName)??new List<SupplierPaymentsReportInfo>();
            var companyList = _companyCussent.GetCompanyCussentListByCompanyName(companyName);
            foreach (var supplierPaymentsReportInfo in list)
            {
                var companyInfo = companyList.FirstOrDefault(act => act.CompanyId == supplierPaymentsReportInfo.CompanyId);
                supplierPaymentsReportInfo.CompanyName = companyInfo != null ? companyInfo.CompanyName : string.Empty;
                supplierPaymentsReportInfo.PaymentDays = companyInfo != null ? companyInfo.PaymentDays : 0;
            }
            if (DateTime.Now.Year == year && include)
            {
                int month = DateTime.Now.Month;
                var currentData = _reportDao.GetPaymentsByForCompany(filialeId);
                if (currentData != null && currentData.Count > 0)
                {
                    var result = new List<SupplierPaymentsReportInfo>();
                    var companyIds = new List<Guid>();
                    foreach (var info in currentData)
                    {
                        var companyInfo = companyList.FirstOrDefault(act => act.CompanyId == info.CompanyId);
                        if (companyInfo == null) continue;
                        var item = list.FirstOrDefault(act => act.CompanyId == info.CompanyId) ??
                                   new SupplierPaymentsReportInfo
                                   {
                                       CompanyId = info.CompanyId,
                                       CompanyName = companyInfo.CompanyName,
                                       PaymentDays = companyInfo.PaymentDays
                                   };
                        item.January += month == 1 ? info.TotalAmount : 0;
                        item.February += month == 2 ? info.TotalAmount : 0;
                        item.March += month == 3 ? info.TotalAmount : 0;
                        item.April += month == 4 ? info.TotalAmount : 0;
                        item.May += month == 5 ? info.TotalAmount : 0;
                        item.June += month == 6 ? info.TotalAmount : 0;
                        item.July += month == 7 ? info.TotalAmount : 0;
                        item.August += month == 8 ? info.TotalAmount : 0;
                        item.September += month == 9 ? info.TotalAmount : 0;
                        item.October += month == 10 ? info.TotalAmount : 0;
                        item.November += month == 11 ? info.TotalAmount : 0;
                        item.December += month == 12 ? info.TotalAmount : 0;

                        item.January1 += month == 1 ? info.TotalNoPayed : 0;
                        item.February2 += month == 2 ? info.TotalNoPayed : 0;
                        item.March3 += month == 3 ? info.TotalNoPayed : 0;
                        item.April4 += month == 4 ? info.TotalNoPayed : 0;
                        item.May5 += month == 5 ? info.TotalNoPayed : 0;
                        item.June6 += month == 6 ? info.TotalNoPayed : 0;
                        item.July7 += month == 7 ? info.TotalNoPayed : 0;
                        item.August8 += month == 8 ? info.TotalNoPayed : 0;
                        item.September9 += month == 9 ? info.TotalNoPayed : 0;
                        item.October10 += month == 10 ? info.TotalNoPayed : 0;
                        item.November11 += month == 11 ? info.TotalNoPayed : 0;
                        item.December12 += month == 12 ? info.TotalNoPayed : 0;
                        companyIds.Add(info.CompanyId);
                        result.Add(item);
                    }
                    result.AddRange(list.Where(act => !companyIds.Contains(act.CompanyId)));
                    return result;
                }
            }
            return list.Where(act=>companyList.Any(a=>a.CompanyId==act.CompanyId)).ToList();
        }

        /// <summary>
        /// 公司每月对应的采购出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectStockReprotsGroupByFilialeId(int year,bool include)
        {
            var list= _reportDao.SelectStockReprotsGroupByFilialeId(year)??new List<SupplierPaymentsReportInfo>();
            if (DateTime.Now.Year == year && include)
            {
                int month = DateTime.Now.Month;
                var currentData = _reportDao.GetPurchasingByForFiliale();
                if (currentData!=null && currentData.Count>0)
                {
                    var result = new List<SupplierPaymentsReportInfo>();
                    var filialeIds = new List<Guid>();
                    foreach (var info in currentData)
                    {
                        var item = list.FirstOrDefault(act => act.FilialeId == info.Key) ??
                                   new SupplierPaymentsReportInfo { FilialeId = info.Key };
                        item.January +=month==1?info.Value:0;
                        item.February += month==2?info.Value:0;
                        item.March += month == 3 ? info.Value : 0;
                        item.April += month==4?info.Value:0;
                        item.May += month==5?info.Value:0;
                        item.June += month==6?info.Value:0;
                        item.July += month==7?info.Value:0;
                        item.August += month==8?info.Value:0;
                        item.September += month==9?info.Value:0;
                        item.October += month==10?info.Value:0;
                        item.November += month==11?info.Value:0;
                        item.December += month==12?info.Value:0;
                        filialeIds.Add(info.Key);
                        result.Add(item);
                    }
                    result.AddRange(list.Where(act => !filialeIds.Contains(act.FilialeId)));
                    return result;
                }
            }
            return list;
        }

        /// <summary>
        /// 供应商每月应付款出入库总金额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IList<SupplierPaymentsReportInfo> SelectStockReportsGroupByCompanyId(int year, Guid filialeId,string companyName,bool include)
        {
            var list=_reportDao.SelectStockReportsGroupByCompanyId(year, filialeId,companyName)??new List<SupplierPaymentsReportInfo>();
            var companyList = _companyCussent.GetCompanyCussentListByCompanyName(companyName);
            foreach (var supplierPaymentsReportInfo in list)
            {
                var companyInfo = companyList.FirstOrDefault(act => act.CompanyId == supplierPaymentsReportInfo.CompanyId);
                supplierPaymentsReportInfo.CompanyName = companyInfo != null ? companyInfo.CompanyName : string.Empty;
                supplierPaymentsReportInfo.PaymentDays = companyInfo != null ? companyInfo.PaymentDays : 0;
            }
            if (DateTime.Now.Year == year && include)
            {
                int month = DateTime.Now.Month;
                var currentData = _reportDao.GetPurchasingByForCompany(filialeId);
                if (currentData!=null && currentData.Count>0)
                {
                    var result = new List<SupplierPaymentsReportInfo>();
                    IList<Guid> companyIds = new List<Guid>();
                    foreach (var info in currentData)
                    {
                        var companyInfo = companyList.FirstOrDefault(act => act.CompanyId == info.Key);
                        if (companyInfo == null) continue;
                        var item = list.FirstOrDefault(act => act.CompanyId == info.Key) ??
                                   new SupplierPaymentsReportInfo
                                   {
                                       FilialeId = filialeId,
                                       CompanyId = info.Key,
                                       CompanyName = companyInfo.CompanyName,
                                       PaymentDays = companyInfo.PaymentDays
                                   };
                        item.January += month == 1 ? info.Value : 0;
                        item.February += month == 2 ? info.Value : 0;
                        item.March += month == 3 ? info.Value : 0;
                        item.April += month == 4 ? info.Value : 0;
                        item.May += month == 5 ? info.Value : 0;
                        item.June += month == 6 ? info.Value : 0;
                        item.July += month == 7 ? info.Value : 0;
                        item.August += month == 8 ? info.Value : 0;
                        item.September += month == 9 ? info.Value : 0;
                        item.October += month == 10 ? info.Value : 0;
                        item.November += month == 11 ? info.Value : 0;
                        item.December += month == 12 ? info.Value : 0;
                        companyIds.Add(info.Key);
                        result.Add(item);
                    }
                    result.AddRange(list.Where(act => !companyIds.Contains(act.CompanyId)));
                    return result;
                }
            }
            return list.Where(act => companyList.Any(a => a.CompanyId == act.CompanyId)).ToList(); 
        }
        #endregion

        /// <summary>
        /// 每月一日执行
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool InsertRececkoning(DateTime dayTime)
        {
            DateTime firstDate;
            DateTime endTime;
            if (dayTime.Year == DateTime.Now.Year && dayTime.Month == DateTime.Now.Month) //当月
            {
                var reportDate = dayTime.AddDays(-1);
                firstDate = new DateTime(reportDate.Year, reportDate.Month, 1);
                endTime = new DateTime(reportDate.Year, reportDate.Month, reportDate.Day, 23, 59, 59);
            }
            else
            {
                firstDate = new DateTime(dayTime.Year, dayTime.Month, 1);
                endTime = new DateTime(dayTime.Year, dayTime.Month, DateTime.DaysInMonth(dayTime.Year, dayTime.Month), 23, 59, 59);
            }
            //查找往来帐、
            // 删除存档往来帐明细  添加明细到报表
            if (dayTime.Day == 1 && _reportDao.IsExists(firstDate)) return true;
            if (dayTime.Day != 1 && _reportDao.IsExistsRecent(firstDate)) return true;
            IList<RecordReckoningInfo> reckoningInfos=_reckoning.SelectRecordReckoningInfos(firstDate,endTime);
            if (reckoningInfos.Count==0)return false;
            return _reportDao.InsertRececkoning(dayTime.Day!=1, reckoningInfos);
        }
    }
}
