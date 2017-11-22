using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Enum;
using ERP.Model.Finance;
using ERP.DAL.Finance.Interface;
using ERP.Environment;
using ERP.DAL.Finance;

namespace ERP.Service.Implement
{
    public partial class Service
    {
        #region  出库单

        /// <summary>
        /// 获取指定时间段内未导入财务API的销售出库单据(完成时间)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StockBillDTO> GetNotAsynSellOutBillDtos(DateTime startTime, DateTime endTime)
        {
            return _storageRecordDao.GetStockBillDtos(startTime, endTime, (int)StorageRecordState.Finished, new List<int> { (int)StorageRecordType.SellStockOut,(int)StorageRecordType.InnerPurchase, (int)StorageRecordType.LendOut});
        }

        /// <summary>
        /// 获取指定时间段内未导入财务API的采购退货的出库单据(完成时间) 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StockBillDTO> GetNotAsynBuyStockOutBillDtos(DateTime startTime, DateTime endTime)
        {
            return _storageRecordDao.GetStockBillDtos(startTime, endTime, (int)StorageRecordState.Finished, new List<int> { (int)StorageRecordType.BuyStockOut,(int)StorageRecordType.AfterSaleOut });
        }
        #endregion

        #region  入库单(采购入库、其它入库)

        /// <summary>
        /// 获取指定时间段内未导入财务API的采购入库单据(完成时间)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StockBillDTO> GetNotAsynPurchasingBillDtos(DateTime startTime, DateTime endTime)
        {
            return _storageRecordDao.GetStockBillDtos(startTime, endTime, (int)StorageRecordState.Finished, new List<int> { (int)StorageRecordType.BuyStockIn, (int)StorageRecordType.LendIn, (int)StorageRecordType.BorrowIn });
        }

        /// <summary>
        /// 获取指定时间段内未导入财务API的销售退货的入库单据(完成时间) 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StockBillDTO> GetNotAsynSellReturnInBillDtos(DateTime startTime, DateTime endTime)
        {
            return _storageRecordDao.GetStockBillDtos(startTime, endTime, (int)StorageRecordState.Finished, new List<int> { (int)StorageRecordType.SellReturnIn});
        }

        #endregion

        #region  供应商

        /// <summary>
        /// 获取未同步到财务API的供应商列表(往来单位添加一个更新时间、同步到U8)
        /// </summary>
        /// <returns></returns>
        public List<VendorDTO> GetNotAsynVendors(DateTime startTime)
        {
            return null;
        }

        /// <summary>
        /// 获取未同步到财务API的客户列表(往来单位添加一个更新时间)
        /// 销售商、(物流、销售公司)、快递公司
        /// </summary>
        /// <returns></returns>
        public List<VendorDTO> GetNotAsynCustomer(DateTime startTime)
        {
            return null;
        }
        #endregion
        
        #region 发票

        /// <summary>
        /// 获取销售发票（已开票）
        /// </summary>
        /// <returns></returns>
        public List<InvoiceDTO> GetSellOutInvoiceDtos(DateTime startTime, DateTime endTime)
        {
            return _invoiceApplySerivce.GetInvoiceDtos(startTime,endTime);
        }

        #endregion

        #region  总账

        #endregion
    }
}
