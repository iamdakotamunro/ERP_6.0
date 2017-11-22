//Author: zal
//createtime:2016/1/12 16:37:01
//Description: 

using ERP.DAL.Interface.IInventory;
using ERP.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace ERP.BLL.Implement.Inventory
{

    /// <summary>
    /// 该类提供了一系列操作 "lmshop_CompanyFundReceiptInvoice" 表的方法;
    /// </summary>
    public class CompanyFundReceiptInvoice : BllInstance<CompanyFundReceiptInvoice>
    {
        private readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoiceDao;

        public CompanyFundReceiptInvoice(ICompanyFundReceiptInvoice companyFundReceipt)
        {
            _companyFundReceiptInvoiceDao = companyFundReceipt;
        }

        public CompanyFundReceiptInvoice(Environment.GlobalConfig.DB.FromType fromType)
        {
            _companyFundReceiptInvoiceDao = new DAL.Implement.Inventory.CompanyFundReceiptInvoice(fromType);
        }

        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回lmshop_CompanyFundReceiptInvoice表的所有数据  
        /// </summary>
        /// <returns></returns>        
        public List<CompanyFundReceiptInvoiceInfo> GetAlllmshop_CompanyFundReceiptInvoice()
        {
            return _companyFundReceiptInvoiceDao.GetAlllmshop_CompanyFundReceiptInvoice();
        }

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的receiptId字段返回数据  
        /// </summary>
        /// <param name="receiptId">receiptId</param>
        /// <returns></returns>        
        public List<CompanyFundReceiptInvoiceInfo> Getlmshop_CompanyFundReceiptInvoiceByReceiptID(Guid receiptId)
        {
            return _companyFundReceiptInvoiceDao.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(receiptId);
        }

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段返回数据  
        /// </summary>
        /// <param name="invoiceId">InvoiceId</param>
        /// <returns></returns>        
        public CompanyFundReceiptInvoiceInfo Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(System.Guid invoiceId)
        {
            return _companyFundReceiptInvoiceDao.Getlmshop_CompanyFundReceiptInvoiceByInvoiceId(invoiceId);
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段删除数据 
        /// </summary>
        /// <param name="invoiceId">invoiceId</param>
        /// <returns>返回受影响的行数</returns>
        public bool Deletelmshop_CompanyFundReceiptInvoiceByInvoiceId(System.Guid invoiceId)
        {
            return _companyFundReceiptInvoiceDao.Deletelmshop_CompanyFundReceiptInvoiceByInvoiceId(invoiceId);
        }

        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的receiptId字段删除数据 
        /// </summary>
        /// <param name="receiptId">receiptId</param>
        /// <returns></returns>        
        public bool Deletelmshop_CompanyFundReceiptInvoiceByReceiptID(Guid receiptId)
        {
            return _companyFundReceiptInvoiceDao.Deletelmshop_CompanyFundReceiptInvoiceByReceiptID(receiptId);
        }
        #endregion
        #region update data
        /// <summary>
        /// 根据lmshop_CompanyFundReceiptInvoice表的InvoiceId字段更新数据 
        /// </summary>
        /// <param name="lmshopCompanyFundReceiptInvoice">lmshopCompanyFundReceiptInvoice</param>
        /// <returns>返回受影响的行数</returns>
        public bool Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice)
        {
            return _companyFundReceiptInvoiceDao.Updatelmshop_CompanyFundReceiptInvoiceByInvoiceId(lmshopCompanyFundReceiptInvoice);
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向lmshop_CompanyFundReceiptInvoice表插入一条数据，插入成功则返回自增列数值，插入不成功则返回-1 
        /// </summary>
        /// <param name="lmshopCompanyFundReceiptInvoice">lmshop_CompanyFundReceiptInvoice</param>        
        /// <returns></returns>
        public bool Addlmshop_CompanyFundReceiptInvoice(CompanyFundReceiptInvoiceInfo lmshopCompanyFundReceiptInvoice)
        {
            return _companyFundReceiptInvoiceDao.Addlmshop_CompanyFundReceiptInvoice(lmshopCompanyFundReceiptInvoice);
        }

        /// <summary>
        /// 向lmshop_CompanyFundReceiptInvoice表批量插入数据
        /// </summary>
        /// <param name="lmshopCompanyFundReceiptInvoiceList"></param>
        /// <returns></returns>
        public bool AddBulklmshop_CompanyFundReceiptInvoice(List<CompanyFundReceiptInvoiceInfo> lmshopCompanyFundReceiptInvoiceList)
        {
            return _companyFundReceiptInvoiceDao.AddBulklmshop_CompanyFundReceiptInvoice(lmshopCompanyFundReceiptInvoiceList);
        }
        #endregion
        #endregion
    }
}