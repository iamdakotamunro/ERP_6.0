using System;
using System.Collections.Generic;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 接   口:采购报表操作
    /// 作   者:jiangsaibiao
    /// </summary>
    public interface IFilingReport
    {
        /// <summary>
        /// 功    能:插入一条采购记录
        /// 作    者:jiangsaibiao
        /// </summary>
        /// <param name="fInfo"></param>
        void Insert(FilingReportInfo fInfo);

        /// <summary>
        /// 功  能:删除某类别下的记录(例如:进货类别)
        /// 作  者:jiangsaibiao
        /// </summary>
        /// <param name="ftype"></param>
        /// <param name="warehouseId"></param>
        void Delete(FilingType ftype, Guid warehouseId);

        /// <summary>
        /// 功  能:获取公司仓库下某类别下的数据集合
        /// 作  者:jiangsaibiao
        /// </summary>
        /// <param name="ftype"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        IList<FilingReportInfo> GetFilingReportByFilingType(FilingType ftype, Guid warehouseId);

        /// <summary>
        /// 功  能:修改公司仓库的进货数量或者采购公司
        /// 作  者:jiangsaibiao
        /// </summary>
        /// <param name="fInfo"></param>
        void Update(FilingReportInfo fInfo);

        /// <summary>
        /// 功  能:提交给仓库管理员
        /// 作  者:jiangsaibiao
        /// </summary>
        /// <param name="fInfo"></param>
        void UpdateFilingType(FilingReportInfo fInfo);

        /// <summary>
        /// 功  能:删除采购id
        /// 作  者:jiangsaibiao
        /// </summary>
        /// <param name="filingId"></param>
        void Delete(Guid filingId);
    }
}
