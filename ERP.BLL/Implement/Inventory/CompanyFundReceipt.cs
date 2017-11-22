using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.ApplyInvocie;
using ERP.Model;
using ERP.Model.Invoice;
using Framework.WCF.Model;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class CompanyFundReceipt:BllInstance<CompanyFundReceipt>
    {
        private readonly ICompanyFundReceipt _companyFundReceiptDao;
        private readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice;

        public CompanyFundReceipt(ICompanyFundReceipt companyFundReceipt)
        {
            _companyFundReceiptDao = companyFundReceipt;
        }

        public CompanyFundReceipt(Environment.GlobalConfig.DB.FromType fromType)
        {
            _companyFundReceiptDao = InventoryInstance.GetCompanyFundReceipteDao(fromType);
            _companyFundReceiptInvoice= new DAL.Implement.Inventory.CompanyFundReceiptInvoice(fromType);
        }

        /// <summary>
        /// 更改订单状态
        /// </summary>
        /// <param name="receiptID"></param>
        /// <param name="state"></param>
        /// <param name="remark"></param>
        /// <param name="personnelId">审核人ID</param>
        /// <returns></returns>
        public int UpdateFundReceiptState(Guid receiptID, CompanyFundReceiptState state, string remark, params Guid[] personnelId)
        {
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    if (state == CompanyFundReceiptState.Audited)
                        _companyFundReceiptDao.UpdateFundReceiptAuditorID(receiptID, personnelId[0]);
                    if (state == CompanyFundReceiptState.HasInvoice)
                        _companyFundReceiptDao.UpdataFundReceiptInvoicesDemander(receiptID, personnelId[0]);

                    _companyFundReceiptDao.UpdateFundReceiptState(receiptID, state);
                    _companyFundReceiptDao.UpdateFundReceiptRemark(receiptID, remark);
                    ts.Complete();
                }
                catch
                {
                    return 0;
                }
            }
            return 1;
        }

        public CompanyFundReceiptInfo GetCompanyFundReceiptInfo(Guid receiptId)
        {
            return _companyFundReceiptDao.GetCompanyFundReceiptInfo(receiptId);
        }

        public List<InvoiceRelationInfo> GetInvoiceRelationInfos(Guid receiptId)
        {
            var dataSource=_companyFundReceiptInvoice.Getlmshop_CompanyFundReceiptInvoiceByReceiptID(receiptId);
            return dataSource.Select(ent=>new InvoiceRelationInfo (ent.InvoiceId,ent.ReceiptID,ent.InvoiceNo,ent.InvoiceCode,(DateTime)ent.BillingDate,ent.NoTaxAmount,ent.Tax,ent.TaxAmount,ent.Remark,false)).ToList();
        } 

        public ResultInfo InvoicePass(Guid receiptId,string invoiceUnit,List<InvoiceRelationInfo> invoiceRelations, string remark,Guid personnelId)
        {
            try
            {
                if (invoiceRelations.All(ent => !ent.IsCanEdit))
                    return new ResultInfo(false, "请录入需新增的发票信息！");
                var companyFundReceiot = _companyFundReceiptDao.GetCompanyFundReceiptInfo(receiptId);
                if (companyFundReceiot == null)
                    return new ResultInfo(false, "未找到对应的申请单据信息！");
                if (companyFundReceiot.ReceiptStatus != (int)CompanyFundReceiptState.WaitInvoice)
                    return new ResultInfo(false, "只有待开票/开票中状态下的往来账收款才允许开票操作！");
                if (Math.Abs(invoiceRelations.Sum(ent => ent.TotalFee)) > companyFundReceiot.RealityBalance)
                    return new ResultInfo(false, "开票金额不能大于收款金额！");
                bool isEqual = Math.Abs(invoiceRelations.Sum(ent => ent.TotalFee)) == companyFundReceiot.RealityBalance;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var result = _companyFundReceiptDao.UpdateInvoice(receiptId, 
                        isEqual ?(int)CompanyFundReceiptState.Audited:(int)CompanyFundReceiptState.WaitInvoice, invoiceUnit,
                            isEqual ? (int)ApplyInvoiceState.Finished
                            : (int)ApplyInvoiceState.Invoicing, remark,isEqual?personnelId:Guid.Empty);
                    if (!result)
                        return new ResultInfo(result, "往来账收款单开票失败！");
                    bool success = true;
                    foreach (var invoiceGroup in invoiceRelations.Where(ent => ent.IsCanEdit).GroupBy(ent => ent.InvoiceNo))
                    {
                        if (invoiceGroup.Count() > 1)
                            return new ResultInfo(false, string.Format("发票添加重复【票据号码{0}】", invoiceGroup.Key));
                        var item = invoiceGroup.First();
                        CompanyFundReceiptInvoiceInfo companyFundReceiptInvoiceInfo=new CompanyFundReceiptInvoiceInfo();
                        companyFundReceiptInvoiceInfo.BillingDate = item.RequestTime;
                        companyFundReceiptInvoiceInfo.BillingUnit = invoiceUnit;
                        companyFundReceiptInvoiceInfo.InvoiceCode = item.InvoiceCode;
                        companyFundReceiptInvoiceInfo.InvoiceNo = item.InvoiceNo;
                        companyFundReceiptInvoiceInfo.InvoiceId = item.InvoiceId;
                        companyFundReceiptInvoiceInfo.Remark = item.Remark;
                        companyFundReceiptInvoiceInfo.TaxAmount = item.TotalFee;
                        companyFundReceiptInvoiceInfo.ReceiptID = item.ApplyId;

                        success = _companyFundReceiptInvoice.Addlmshop_CompanyFundReceiptInvoice(companyFundReceiptInvoiceInfo);
                        if (!success)
                        {
                            break;
                        }
                    }
                    if (success)
                    {
                        tran.Complete();
                        return new ResultInfo(true, "");
                    }
                    return new ResultInfo(false, "插入发票失败");
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        public bool InvoiceBack(Guid receiptId,string remark)
        {
            return _companyFundReceiptDao.UpdateInvoice(receiptId,(int)CompanyFundReceiptState.NoAuditing,string.Empty,0, remark,Guid.Empty);
        }
    }
}
