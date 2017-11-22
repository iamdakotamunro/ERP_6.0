using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model.SubsidyPayment;
using ERP.Model;

namespace ERP.DAL.Interface.FinanceModule
{
    /// <summary>
    /// 补贴审核、补贴打款IDAL
    /// </summary>
    public interface ISubsidyPaymentDal
    {
        bool AddSubsidyPayment(SubsidyPaymentInfo_Add model);

        bool UpdateSubsidyPayment(SubsidyPaymentInfo_Edit model);

        bool ApprovalSubsidyPayment(SubsidyPaymentInfo_Approval model);

        bool DeleteSubsidyPayment(Guid ID, string ModifyUser);

        IList<SubsidyPaymentInfo> GetSubsidyPaymentList(SubsidyPaymentInfo_SeachModel seachModel, out int Totalcount);

        SubsidyPaymentInfo GetSubsidyPaymentByID(Guid iD);
        bool CheckSubsidyPayment(SubsidyPaymentInfo_Check model);
        bool ApprovalPaymentSubsidyPayment(SubsidyPaymentInfo_Payment model);
        ArrayList GetSumList(List<string> listID);
        ResultModel IsExistSubsidyPayment(string ThirdPartyOrderNumber);
    }
}