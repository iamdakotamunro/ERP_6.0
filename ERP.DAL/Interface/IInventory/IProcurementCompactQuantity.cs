using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>▄︻┻┳═一 采购合同量接口   ADD 2014-12-18  陈重文
    /// </summary>
    public interface IProcurementCompactQuantity
    {
        /// <summary>根据公司ID和年份获取供应商采购合同量
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>Return：供应商采购合同量集合</returns>
        IList<ProcurementCompactQuantityInfo> GetProcurementCompactQuantityList(int year);
    }
}
