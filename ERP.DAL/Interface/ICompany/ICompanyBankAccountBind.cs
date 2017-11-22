using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.ICompany
{
    public interface ICompanyBankAccountBind
    {
        /// <summary>
        /// 有则更新无则插入绑定数据
        /// </summary>
        /// <param name="bankAccountBindInfo"></param>
        /// <returns></returns>
        bool InsertCompanyBankAccountBind(CompanyBankAccountBindInfo bankAccountBindInfo);

        /// <summary>
        /// 有则更新无则插入绑定数据
        /// </summary>
        /// <param name="bankAccountBindInfo"></param>
        /// <returns></returns>
        bool InsertCompanyBankAccountBindWithFiliale(CompanyBankAccountBindInfo bankAccountBindInfo);

        /// <summary>
        /// 删除往来单位和公司绑定的银行账户
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        bool DeleteCompanyBankAccountBind(Guid companyId);

        /// <summary>
        /// 根据公司和往来单位获取绑定的银行账户
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        CompanyBankAccountBindInfo GetCompanyBankAccountBindInfo(Guid companyId,Guid filialeId);
        
        /// <summary>
        /// 获取往来单位绑定公司收款信息数
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        int GetBindCount(Guid companyId);

        CompanyBankAccountBindInfo GetCompanyBankAccountIdBind(Guid companyId, Guid filialeId);
    }
}
