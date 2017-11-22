using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

//================================================
// 功能：费用申报票据操作类接口
// 作者：刘彩军
// 时间：2010-February-24th
//================================================
namespace ERP.DAL.Interface.IInventory
{
    public interface ICostReportInvoice
    {
        /// <summary>
        /// 添加票据
        /// </summary>
        /// <param name="info">票据模型</param>
        void InsertInvoice(CostReportInvoiceInfo info);

        /// <summary>
        /// 获取待审核票据
        /// </summary>
        /// <returns></returns>
        IList<CostReportInvoiceInfo> GetInvoiceList();

        /// <summary>
        /// 根据票据ID获取待审核票据
        /// </summary>
        /// <param name="invoiceId">票据ID</param>
        /// <returns></returns>
        CostReportInvoiceInfo GetInvoice(Guid invoiceId);

        /// <summary>
        /// 修改票据
        /// </summary>
        /// <param name="info">票据模型</param>
        void UpdateInvoice(CostReportInvoiceInfo info);

        /// <summary>
        /// 修改票据状态
        /// </summary>
        /// <param name="info">票据模型</param>
        void UpdateInvoiceState(CostReportInvoiceInfo info);

        /// <summary>
        /// 获取所有票据
        /// </summary>
        /// <returns></returns>
        IList<CostReportInvoiceInfo> GetAllInvoiceList();

        /// <summary>
        /// 根据网页凭证id，删除网页凭证数据
        /// </summary>
        /// <param name="invoiceId">网页凭证id</param>
        /// zal 2015-12-07
        bool DelInvoice(Guid invoiceId);
    }
}
