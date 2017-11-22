using System;
using System.Collections.Generic;
using ERP.Model.Report;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 供应商销量数据接口
    /// </summary>
    public interface ISupplierSaleRecord
    {
        /// <summary>
        /// 判断某月份是否已存档
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool IsExists(DateTime dayTime);

        /// <summary>
        /// 添加公司对应供应商销量记录
        /// </summary>
        /// <param name="supplierSaleRecordInfos"></param>
        /// <returns></returns>
        bool InsertSaleRecord(IList<SupplierSaleRecordInfo> supplierSaleRecordInfos);

        /// <summary>
        /// 供应商销量页面显示数据(对应公司的销量)
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        IList<SupplierSaleReportInfo> SelectSupplierSaleReportInfos(int year);

        /// <summary>
        /// 获取当月已存在的销售数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="supplierSaleRecordInfos"></param>
        /// <returns></returns>
        bool SelectSupplierSaleRecordInfos(DateTime dayTime, IList<SupplierSaleRecordInfo> supplierSaleRecordInfos);
    }
}
