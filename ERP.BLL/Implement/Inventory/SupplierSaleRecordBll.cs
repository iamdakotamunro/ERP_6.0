using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IStorage;
using ERP.Model.Report;

namespace ERP.BLL.Implement.Inventory
{
    public class SupplierSaleRecordBll
    {
        readonly ISupplierSaleRecord _supplierSaleRecordDao;
        readonly ICompanyCussent _companyCussent;
        private readonly IPurchaseSet _purchaseSet;
        readonly IGoodsStockRecord _goodsStockSettleRecord = new GoodsStockRecordDao();
        private readonly IStorageRecordDao _storageRecordDao;


        public SupplierSaleRecordBll(ISupplierSaleRecord supplierSaleRecord,ICompanyCussent companyCussent, IPurchaseSet purchaseSet, IGoodsStockRecord goodsStockRecord,IStorageRecordDao storageRecordDao)
        {
            _supplierSaleRecordDao = supplierSaleRecord;
            _companyCussent = companyCussent;
            _purchaseSet = purchaseSet;
            _goodsStockSettleRecord = goodsStockRecord;
            _storageRecordDao = storageRecordDao;
        }
        
        public SupplierSaleRecordBll(Environment.GlobalConfig.DB.FromType fromType = Environment.GlobalConfig.DB.FromType.Write)
        {
            _companyCussent = new CompanyCussent(fromType);
            _purchaseSet = new PurchaseSet(fromType);
            _storageRecordDao=new StorageRecordDao(fromType);
            _supplierSaleRecordDao = new SupplierSaleRecordDao();
        }

        /// <summary>
        /// 插入销售存档数据 待单元测试
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool InsertSaleRecord(DateTime dateTime)
        {
            if (_supplierSaleRecordDao.IsExists(dateTime))
            {
                return true;
            }
            var dataList = new List<SupplierSaleRecordInfo>();
            var list = _storageRecordDao.GeTempStorageRecordDetailInfos(dateTime, dateTime.AddMonths(1).AddMilliseconds(-1));
            if (list == null || list.Count == 0)
                return true;
            //获取采购设置列表
            var purchasetList = _purchaseSet.GetPurchaseSetList();
            if (purchasetList.Count == 0) return true;
            //获取最近结算价列表
            var settlePriceList = _goodsStockSettleRecord.GetGoodsSettlePriceOrPurchasePriceDicts(dateTime);
            foreach (var item in list)
            {
                var setInfo = purchasetList.FirstOrDefault(act => act.WarehouseId == item.WarehouseId && act.GoodsId == item.GoodsId);
                if (setInfo == null) continue;
                var info = dataList.FirstOrDefault(act => act.CompanyID == setInfo.CompanyId);
                var settleprice = settlePriceList.ContainsKey(item.GoodsId) ? settlePriceList[item.GoodsId] : 0;
                if (info != null)
                {
                    info.Quantity += item.Quantity;
                    info.TotalSettlePrice += item.Quantity * settleprice;
                }
                else
                {
                    dataList.Add(new SupplierSaleRecordInfo
                    {
                        CompanyID = setInfo.CompanyId,
                        DayTime = dateTime,
                        Quantity = item.Quantity,
                        TotalSettlePrice = item.Quantity * settleprice
                    });
                }
            }
            if (dataList.Count == 0) return true;
            return _supplierSaleRecordDao.InsertSaleRecord(dataList);
        }

        /// <summary>
        /// 供应商销量页面显示数据(对应公司的销量) 待单元测试
        /// </summary>
        /// <param name="year"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public IList<SupplierSaleReportInfo> SelectSupplierSaleReportInfos(int year, string companyName)
        {
            var companyList = _companyCussent.GetCompanyCussentListByCompanyName(companyName);
            if (companyList.Count > 0)
            {
                var companyIds = companyList.Select(act => act.CompanyId).Distinct().ToList();
                var list = _supplierSaleRecordDao.SelectSupplierSaleReportInfos(year);
                if (list != null && list.Count > 0)
                {
                    list = list.Where(act => companyIds.Contains(act.CompanyID)).ToList();
                    foreach (var supplierSaleReportInfo in list)
                    {
                        var info = companyList.FirstOrDefault(act => act.CompanyId == supplierSaleReportInfo.CompanyID);
                        supplierSaleReportInfo.CompanyName = info != null ? info.CompanyName : "未知公司";
                    }
                }
                return list;
            }
            return new List<SupplierSaleReportInfo>();
        }

        /// <summary>
        /// 获取当月已存在的销售数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool SelectSupplierSaleRecordInfos(DateTime dayTime)
        {
            var recordTime = new DateTime(dayTime.Year, dayTime.Month, 1);
            var dataList = new List<SupplierSaleRecordInfo>();
            var list = _storageRecordDao.GeTempStorageRecordDetailInfos(recordTime, new DateTime(dayTime.Year, dayTime.Month, dayTime.Day).AddMilliseconds(-1));
            if (list == null || list.Count == 0)
                return true;
            //获取采购设置列表
            var purchasetList = _purchaseSet.GetPurchaseSetList();
            if (purchasetList.Count == 0) return true;
            //获取最近结算价列表
            var settlePriceList = _goodsStockSettleRecord.GetGoodsSettlePriceOrPurchasePriceDicts(recordTime);
            foreach (var item in list)
            {
                var setInfo =purchasetList.FirstOrDefault(act => act.WarehouseId == item.WarehouseId && act.GoodsId == item.GoodsId);
                if (setInfo == null) continue;
                var info = dataList.FirstOrDefault(act => act.CompanyID == setInfo.CompanyId);
                var settleprice = settlePriceList.ContainsKey(item.GoodsId) ? settlePriceList[item.GoodsId] : 0;
                if (info != null)
                {
                    info.Quantity += item.Quantity;
                    info.TotalSettlePrice += item.Quantity * settleprice;
                }
                else
                {
                    dataList.Add(new SupplierSaleRecordInfo
                    {
                        CompanyID = setInfo.CompanyId,
                        DayTime = recordTime,
                        Quantity = item.Quantity,
                        TotalSettlePrice = item.Quantity * settleprice
                    });
                }
            }
            if (dataList.Count == 0) return true;
            return _supplierSaleRecordDao.SelectSupplierSaleRecordInfos(recordTime, dataList);
        }
    }
}
