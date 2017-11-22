using ERP.Model.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyGrossProfitRecordDetailChild
    {
        /// <summary>
        /// 批量插入公司毛利记录明细子表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddDataDetailChild(IList<CompanyGrossProfitRecordDetailChildInfo> list);

        /// <summary>
        /// 根据订单id或者出入库id获取订单或库存明细商品
        /// </summary>
        /// <param name="stockAndOrderIds"></param>
        /// <returns></returns>
        /// zal 2016-07-20
        IList<CompanyGrossProfitRecordDetailChildInfo> GetCompanyGrossProfitRecordDetailChildByStockAndOrderIds(List<Guid> stockAndOrderIds);
    }
}
