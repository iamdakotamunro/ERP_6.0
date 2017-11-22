using System;
using System.Collections.Generic;
using ERP.DAL.Implement.FinanceModule;
using ERP.DAL.Interface.FinanceModule;
using ERP.Model.RefundsMoney;
using ERP.BLL.Interface;

namespace ERP.BLL.Implement.FinanceModule
{
    /// <summary>
    /// 退款打款BLL
    /// </summary>
    public class RefundsMoneyManager 
    {
        private static readonly IRefundsMoneyDal _refundsMoneyDal = new RefundsMoneyDal();

        public IList<RefundsMoneyInfo> GetRefundsMoneyList(RefundsMoneyInfo_SeachModel seachModel, out int Totalcount)
        {
            return _refundsMoneyDal.GetRefundsMoneyList(seachModel, out Totalcount);
        }

        public RefundsMoneyInfo GetRefundsMoneyByID(Guid ID)
        {
            return _refundsMoneyDal.GetRefundsMoneyByID(ID);
        }

        public bool ApprovalPaymentRefundsMoney(RefundsMoneyInfo_Payment model)
        {
            return _refundsMoneyDal.ApprovalPaymentRefundsMoney(model);
        }
    }
}