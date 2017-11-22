using ERP.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyFundReceiptInvoice
    {
        #region .对本表的维护.
        #region select data

        /// <summary>
        /// 返回lmshop_CompanyFundReceiptInvoice表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<CompanyFundReceiptInvoiceInfo> GetAlllmshop_CompanyFundReceiptInvoice();

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的receiptId字段返回数据  
        /// </summary>
        /// <param name="receiptId">receiptId</param>
        /// <returns></returns>        
        List<CompanyFundReceiptInvoiceInfo> Getlmshop_CompanyFundReceiptInvoiceByReceiptID(Guid receiptId);

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段返回数据 
        /// </summary>
        /// <param name="invoiceId">InvoiceId</param>
        /// <returns></returns>       
        CompanyFundReceiptInvoiceInfo Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(Guid invoiceId);

        /// <summary>
        /// 返回lmshop_CompanyFundReceiptInvoice表的所有数据 
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="companyId"></param>
        /// <param name="startApplyDateTime"></param>
        /// <param name="endApplyDateTime"></param>
        /// <param name="receiptStatus"></param>
        /// <param name="receiptNo"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceState"></param>
        /// <param name="startOperatingTime"></param>
        /// <param name="endOperatingTime"></param>
        /// <param name="billingUnit"></param>
        /// <param name="invoiceType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        DataTable Getlmshop_CompanyFundReceiptInvoiceByPage(Guid filialeId, Guid companyId, DateTime? startApplyDateTime,
            DateTime? endApplyDateTime, string receiptStatus, string receiptNo, string invoiceNo, int? invoiceState,
            DateTime? startOperatingTime, DateTime? endOperatingTime, string billingUnit,int? invoiceType, int pageIndex, int pageSize, out int total);
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段删除数据 
        /// </summary>
        /// <param name="invoiceId">invoiceId</param>
        /// <returns></returns>        
        bool Deletelmshop_CompanyFundReceiptInvoiceByInvoiceId(Guid invoiceId);

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的receiptId字段删除数据 
        /// </summary>
        /// <param name="receiptId">receiptId</param>
        /// <returns></returns>        
        bool Deletelmshop_CompanyFundReceiptInvoiceByReceiptID(Guid receiptId);
        #endregion
        #region update data
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段更新数据 
        /// </summary> 
        /// <param name="lmshopCompanyFundReceiptInvoice">lmshopCompanyFundReceiptInvoice</param>
        /// <returns></returns>       
        bool Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice);

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段修改数据
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="invoiceId">主键</param>
        /// <param name="invoiceState">发票状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        bool Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(string remark, Guid invoiceId, int invoiceState);

        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段修改备注信息
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="invoiceId">主键</param>
        /// <returns></returns>
        bool Updatelmshop_CompanyFundReceiptInvoiceRemarkByInvoiceId(string remark, Guid invoiceId);
        #endregion
        #region insert data
        /// <summary>
        /// 向lmshop_CompanyFundReceiptInvoice表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="lmshopCompanyFundReceiptInvoice">lmshop_CompanyFundReceiptInvoice</param>       
        /// <returns></returns>        
        bool Addlmshop_CompanyFundReceiptInvoice(CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice);

        /// <summary>
        /// 向lmshop_CompanyFundReceiptInvoice表批量插入数据
        /// </summary>
        /// <param name="lmshopCompanyFundReceiptInvoiceList"></param>
        /// <returns></returns>
        bool AddBulklmshop_CompanyFundReceiptInvoice(List<CompanyFundReceiptInvoiceInfo> lmshopCompanyFundReceiptInvoiceList);
        #endregion

        #endregion
    }
}
