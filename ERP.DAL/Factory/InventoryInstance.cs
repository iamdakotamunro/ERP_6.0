using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Goods;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IGoods;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Utilities;

namespace ERP.DAL.Factory
{
    public class InventoryInstance : InstanceBase
    {

        /// <summary>
        /// 往来单位分类信息
        /// </summary>
        public static ICompanyClass GetCompanyClassDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new CompanyClass(fromType);
        }

        /// <summary>
        /// 往来单位信息
        /// </summary>
        public static ICompanyCussent GetCompanyCussentDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new CompanyCussent(fromType);
        }

        /// <summary>
        /// 往来单位信息
        /// </summary>
        public static ICostCussent GetCostCussentDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new CostCussent(fromType); }
        }

        
        /// <summary>
        /// 资金账号信息
        /// </summary>
        public static IBankAccounts GetBankAccountsDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new BankAccounts(fromType);
        }

        /// <summary>
        /// 资金账号信息
        /// </summary>
        public static IBankAccountDao GetBankAccountDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new BankAccountDao(fromType);
        }

        /// <summary>
        /// 账目操作信息
        /// </summary>
        public static IWasteBook GetWasteBookDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new WasteBook(fromType);
        }


        /// <summary>
        /// 财务账单信息 
        /// </summary>
        public static IReckoning GetReckoningDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new Reckoning(fromType);
        }


        /// <summary>
        /// 商品当前库存信息
        /// </summary>
        public static IGoodsStockPile GetGoodsStockPileDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new GoodsStockPile(fromType); }
        }

        /// <summary>
        /// 销售排行信息
        /// </summary>
        public static ISalesGoodsRanking GetSalesGoodsRankingDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new SalesGoodsRanking(fromType); }
        }

        /// <summary>
        /// 库存预警信息
        /// </summary>
        public static IStockWarning GetStockWarningDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new StockWarning(fromType); }
        }

        /// <summary>
        /// 发票信息
        /// </summary>
        public static IInvoice GetInvoiceDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new Invoice(fromType);
        }

        /// <summary>
        /// 往来单位信息
        /// </summary>
        public static ICost GetCostDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new Cost(fromType); }
        }

        /// <summary>
        /// 账务记录数据信息
        /// </summary>
        public static ICostReckoning GetCostReckoningDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new CostReckoning(fromType);
        }
        
        /// <summary>
        /// 申报信息
        /// </summary>
        public static ICostReport GetCostReportDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new CostReport(fromType); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IPurchasingDetail GetPurchasingDetailDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new PurchasingDetail(fromType); }
        }

        /// <summary>
        /// 采购组信息
        /// </summary>
        public static IPurchasingManagement GetPurchasingManagementDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new PurchasingManagement(fromType);
        }

        /// <summary>
        /// 单据信息
        /// </summary>
        public static ICompanyFundReceipt GetCompanyFundReceipteDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new CompanyFundReceipt(fromType); }
        }

        /// <summary>
        /// 往来单位收付款审核权限信息
        /// </summary>
        public static ICompanyAuditingPower GetCompanyAuditingPowerDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new CompanyAuditingPower(fromType); }
        }

        /// <summary>
        /// 指定仓库指定状态的商品信息
        /// </summary>
        public static IWaitCheckStockGoods GetWaitCheckStockGoodsDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new WaitCheckStockGoods(fromType); }
        }
        
        /// <summary>
        /// 商品进出库存信息
        /// </summary>
        public static IStorageRecordDao GetStorageRecordDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            return new StorageRecordDao(fromType);
        }

        /// <summary>
        /// 订单快递运费信息
        /// </summary>
        public static IGoodsOrderDeliver GetGoodsOrderDeliverDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new GoodsOrderDeliver(fromType); }
        }

        public static ICheckDataRecord GetCheckDataRecordDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new CheckDataRecord(fromType); }
        }

        public static IMediumReckoning GetMediumReckoningDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new MediumReckoning(fromType); }
        }

        /// <summary>
        /// 商品采购设置，采购分组
        /// </summary>
        public static ICompanyPurchaseGoupDao GetCompanyPurchaseGoupDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new CompanyPurchaseGoupDao(fromType); }
        }

        /// <summary>
        /// 活动报备
        /// </summary>
        public static IActivityFiling GetActivityFilingDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new ActivityFiling(fromType); }
        }
        /// <summary>
        /// 操作日志
        /// </summary>
        public static IActivityOperateLog GetActivityOperateLogDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new ActivityOperateLog(fromType); }
        }

        /// <summary>
        /// 非全字段地操作某个表
        /// </summary>
        public static IUtility GetUtilityDalDao(Environment.GlobalConfig.DB.FromType fromType)
        {
            { return new UtilityDal(fromType); }
        }
    }
}
