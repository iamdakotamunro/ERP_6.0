using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model.RefundsMoney;

namespace ERP.DAL.Interface.FinanceModule
{
    public interface IRefundsMoneyDal
    {
        bool AddRefundsMoney(RefundsMoneyInfo_Add model);

        bool UpdateRefundsMoney(RefundsMoneyInfo_Edit model);

        bool ApprovalRefundsMoney(RefundsMoneyInfo_Approval model);
        bool ApprovalPaymentRefundsMoney(RefundsMoneyInfo_Payment model);

        bool DeleteRefundsMoney(Guid ID, string ModifyUser);

        IList<RefundsMoneyInfo> GetRefundsMoneyList(RefundsMoneyInfo_SeachModel seachModel, out int Totalcount);

        RefundsMoneyInfo GetRefundsMoneyByID(Guid iD);
    }
}