using System;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IStorage;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using System.Linq;
using Framework.Core.Utility;

namespace GoodsStockRecordTask.Core
{
    /// <summary>
    /// 每月报表数据存档管理
    /// 包含：商品结算价、商品月库存存档、应付款和采购入库数据存档及往来帐明细存档
    /// </summary>
    public class GoodsStockRecordTaskManager
    {
        static readonly ISupplierSaleRecord _supplierSaleRecordDao = new SupplierSaleRecordDao();

        static readonly IGoodsStockRecord _goodsStockRecord = new GoodsStockRecordDao();

        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(ERP.Environment.GlobalConfig.DB.FromType.Write);

        static readonly IGoodsCenterSao _goodsInfoSao = new GoodsCenterSao();

        static readonly ICompanyCussent _companyCussent = new CompanyCussent(ERP.Environment.GlobalConfig.DB.FromType.Write);

        static readonly IPurchaseSet _purchaseSet = new PurchaseSet(ERP.Environment.GlobalConfig.DB.FromType.Write);

        private static readonly SupplierSaleRecordBll _supplierSaleRecordBll = new SupplierSaleRecordBll(_supplierSaleRecordDao, _companyCussent, _purchaseSet, _goodsStockRecord, _storageRecordDao);

        private static readonly GoodsStockSettleRecordBll _goodsStockSettleRecordBll = new GoodsStockSettleRecordBll(_goodsStockRecord, _storageRecordDao, _goodsInfoSao);

        private static int _day;

        #region  for 正式
        /// <summary>执行结算价与商品库存存档任务
        /// </summary>
        public static void RunGoodsStockRecordTask()
        {
            string title = "";
            try
            {
                var date = DateTime.Now;
                var createStockHour = Configuration.AppSettings["CreateStockHour"];
                var hours = createStockHour.Split(',');
                if (!hours.Contains(string.Format("{0}", date.Hour))) return;
                var preDate = date.AddMonths(-1);
                //10月份算9月份存档
                var recordDate = new DateTime(preDate.Year, preDate.Month, 1);

                if (_day == DateTime.Now.Day) return;

                if (date.Day == 1)
                {
                    title = "每月月末库存存档";
                    //备份月末库存
                    if (!_goodsStockSettleRecordBll.CopyMonthGoodsStockInfos(recordDate))
                    {
                        ERP.SAL.LogCenter.LogService.LogError("每月月末库存备份失败", "结算价报表存档",null);
                        return;
                    }


                    title = "商品结算价存档";
                    var result = _goodsStockSettleRecordBll.AddSettlePriceAndStockRecord(date.Year, date.Month);
                    if (!result)
                    {
                        ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}年{1}月份商品结算价报表存档失败", preDate.Year, preDate.Month), "商品库存、结算价报表存档", null);
                        return;
                    }

                    //记录销售额存档
                    if (_goodsStockRecord.IsExistsSettlePriceRecord(recordDate))
                    {
                        title = "供应商月销售额存档";
                        if (!_supplierSaleRecordBll.InsertSaleRecord(recordDate))
                        {
                            ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}年{1}月份供应商销售额报表存档失败", preDate.Year, preDate.Month), "供应商月销售额报表存档", null);
                            return;
                        }
                    }
                }
                else
                {
                    //如果是非每月1号先删除所有数据   当月数据的存储
                    var resultData = _supplierSaleRecordBll.SelectSupplierSaleRecordInfos(date);
                    if (!resultData)
                    {
                        ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}年{1}月{2}日供应商每日销售额存档失败", date.Year, date.Month, date.Day), "供应商每日销售额", null);
                        return;
                    }
                }
                _day = DateTime.Now.Day;
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError("执行结算价与商品库存存档任务", title, exp);
            }
        }

        #endregion

        /// <summary>
        /// 新品结算价
        /// </summary>
        public static void RunAddNewGoodsStockRecord(int year, int month)
        {
            if (!_goodsStockSettleRecordBll.AddNewGoodsSettlePrice(year, month))
            {
                ERP.SAL.LogCenter.LogService.LogError(string.Format("{0}年{1}月新品结算价录入失败", year, month), "结算价报表存档",null);
            }
        }
    }
}
