using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Interface;
using ERP.DAL.Interface.IInventory;
using ERP.Enum.ApplyInvocie;
using ERP.Model.Invoice;
using OperationLog.Core;
using ERP.DAL.Implement.FinanceModule;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Model;
using ERP.Model.Finance;
using ERP.DAL.Interface.FinanceModule;
using ERP.Model.SubsidyPayment;
using ERP.Model.RefundsMoney;
using System.Collections;

namespace ERP.BLL.Implement.FinanceModule
{
    /// <summary>
    /// 补贴审核、补贴打款BLL
    /// </summary>
    public class SubsidyPaymentManager 
    {
        private static readonly ISubsidyPaymentDal _subsidyPaymentDal = new SubsidyPaymentDal();

        public IList<SubsidyPaymentInfo> GetSubsidyPaymentList(SubsidyPaymentInfo_SeachModel seachModel, out int Totalcount)
        {
            return _subsidyPaymentDal.GetSubsidyPaymentList(seachModel, out Totalcount);
        }

        public SubsidyPaymentInfo GetSubsidyPaymentByID(Guid ID)
        {
            return _subsidyPaymentDal.GetSubsidyPaymentByID(ID);
        }

        public bool CheckSubsidyPayment(SubsidyPaymentInfo_Check model)
        {
            return _subsidyPaymentDal.CheckSubsidyPayment(model);
        }

        public bool ApprovalPaymentSubsidyPayment(SubsidyPaymentInfo_Payment model)
        {
            return _subsidyPaymentDal.ApprovalPaymentSubsidyPayment(model);
        }
        public ArrayList GetSumList(List<string> listID)
        {
            return _subsidyPaymentDal.GetSumList(listID);
        }
    }
}