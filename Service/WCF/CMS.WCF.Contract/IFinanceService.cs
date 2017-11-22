using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ERP.Model.Finance;

namespace ERP.Service.Contract
{
    /// <summary>
    /// 财务API
    /// </summary>
    public partial interface IService
    {
        #region  出库单(销售出库、其它出库、出入库单据添加一个完成时间)
        /// <summary>
        /// 获取指定时间段内未导入财务API的销售出库单据(完成时间)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<StockBillDTO> GetNotAsynSellOutBillDtos(DateTime startTime,DateTime endTime);

        /// <summary>
        /// 获取指定时间段内未导入财务API的采购退货的出库单据(完成时间) 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<StockBillDTO> GetNotAsynBuyStockOutBillDtos(DateTime startTime, DateTime endTime);
        #endregion

        #region  入库单(采购入库、其它入库)
        /// <summary>
        /// 获取指定时间段内未导入财务API的采购入库单据(完成时间)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<StockBillDTO> GetNotAsynPurchasingBillDtos(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取指定时间段内未导入财务API的销售退货的入库单据(完成时间) 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<StockBillDTO> GetNotAsynSellReturnInBillDtos(DateTime startTime, DateTime endTime);
        #endregion

        #region  供应商\客户
        /// <summary>
        /// 获取未同步到财务API的供应商列表(往来单位添加一个更新时间且需要同步到U8的数据)
        /// 供应商 + 物流
        /// </summary>
        /// <returns></returns>
        List<VendorDTO> GetNotAsynVendors(DateTime startTime);

        /// <summary>
        /// 获取未同步到财务API的客户列表(往来单位添加一个更新时间)
        /// 销售商、(物流、销售公司)、快递公司
        /// </summary>
        /// <returns></returns>
        List<VendorDTO> GetNotAsynCustomer(DateTime startTime);

        #endregion

        #region 发票

        /// <summary>
        /// 获取销售发票（已开票）
        /// </summary>
        /// <returns></returns>
        List<InvoiceDTO> GetSellOutInvoiceDtos(DateTime startTime, DateTime endTime);

        #endregion
    }
}
