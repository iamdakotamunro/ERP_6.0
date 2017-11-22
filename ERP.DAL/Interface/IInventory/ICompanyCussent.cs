using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;
using ERP.SAL.WMS;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// ������λ��Ϣ�ӿ���
    /// </summary>
    public interface ICompanyCussent
    {
        /// <summary>
        /// ����������λ�ؿ���ˮ��Ϣ
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <returns></returns>
        string GetCussentExtendInfo(Guid companyId);

        /// <summary>
        /// ����������λ�ؿ���ˮ��Ϣ
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="extend"> </param>
        /// <returns></returns>
        void UpDatetCussentExtendInfo(Guid companyId, string extend);

        /// <summary>
        /// ���������λ��Ϣ
        /// </summary>
        /// <param name="companyCussent">������λ��Ϣ��</param>
        void Insert(CompanyCussentInfo companyCussent);

        /// <summary>
        /// ����������λ
        /// </summary>
        /// <param name="companyCussent">������λ��Ϣ��</param>
        void Update(CompanyCussentInfo companyCussent);

        /// <summary>
        /// ɾ��������λ��Ϣ
        /// </summary>
        /// <param name="companyId">������λ���</param>
        void Delete(Guid companyId);

        /// <summary>
        /// ��ȡ������λ��Ϣ��
        /// </summary>
        /// <param name="companyId">������λ���</param>
        /// <returns></returns>
        CompanyCussentInfo GetCompanyCussent(Guid companyId);

        /// <summary>
        /// ��ȡ������λ��Ϣ��
        /// </summary>
        /// <param name="companyId">������λ���</param>
        /// <param name="filialeId">�󶨹�˾Id</param>
        /// <returns></returns>
        /// zal 2016-03-16
        CompanyCussentInfo GetCompanyCussentInfoByCompanyIdAndFilialeId(Guid companyId, Guid filialeId);

        /// <summary>
        /// ��ȡ������λ��Ϣ�б�
        /// </summary>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList();

        /// <summary>
        /// ��ȡָ�����͵�������λ��Ϣ�б�
        /// </summary>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType companyType);

        /// <summary>
        /// ��ȡָ�����ͣ�ָ��״̬��������λ��Ϣ�б�
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType companyType, State state);

        IList<CompanyCussentInfo> GetCompanyCussentList(CompanyType[] companyType, State state);

        /// <summary>
        /// ��ȡָ��״̬��������λ��¼
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(State state);

        /// <summary>
        /// ��ȡָ������ŵ�������λ�б�
        /// </summary>
        /// <param name="companyClassId">������λ������</param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentList(Guid companyClassId);

        /// <summary>
        /// ͨ��������λ���ƻ�ȡ������λ�б�  ģ������
        /// </summary>
        /// <param name="companyName">������λ������</param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentListByCompanyName(string companyName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyClassId"></param>
        /// <returns></returns>
        IList<Guid> GetCompanyIdList(Guid companyClassId);

        /// <summary>
        /// ��ȡ��Ա����
        /// </summary>
        /// <returns></returns>
        CompanyCussentInfo GetMemberGeneralLedger();

        /// <summary>
        /// ��ȡָ����˾���ڵ�Ӧ��������������ڱ���˾��Ӧ�տ���
        /// </summary>
        /// <param name="companyId">������˾���</param>
        /// <returns>���ضԱ���˾���Ե�ָ����˾Ӧ�տ�</returns>
        double GetNonceReckoningTotalled(Guid companyId);

        /// <summary>
        /// ��ȡָ����˾���ڵ�Ӧ��������������ڱ���˾��Ӧ�տ���
        /// </summary>
        /// <param name="companyId">������˾���</param>
        /// <param name="filialeId"> </param>
        /// <returns>���ضԱ���˾���Ե�ָ����˾Ӧ�տ�</returns>
        double GetNonceReckoningTotalled(Guid companyId, Guid filialeId);

        /// <summary>
        /// �ж��Ƿ�����ù�˾
        /// </summary>
        /// <param name="companyName">��˾����</param>
        /// <returns></returns>
        bool IsBeing(string companyName);

        /// <summary>
        /// �ж��Ƿ�Ϊ��ݹ�˾������
        /// </summary>
        /// <param name="companyId">��˾���</param>
        /// <returns></returns>
        bool IsExpress(Guid companyId);

        /// <summary>
        /// �ж��Ƿ�Ϊ��Ա���ʺ�
        /// </summary>
        /// <param name="companyId">��˾���</param>
        /// <returns></returns>
        bool IsMemberGeneralLedger(Guid companyId);

        /// <summary>
        /// ��Ա�����Ƿ�ʹ��
        /// </summary>
        /// <returns></returns>
        bool IsUseMemberGeneralLedger();

        IList<CompanyCussentInfo> GetCompanyCussentListByPersion(Guid companyClassId, Guid persionID);

        /// <summary>
        /// ����������λ�ۿ���Ϣ
        /// Add by liucaijun at 2011-August-16th
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="discountMemo">�ۿ���Ϣ</param>
        /// <returns></returns>
        void UpDatetCussentDiscountMemoInfo(Guid companyId, string discountMemo);

        /// <summary>
        /// ����������λ��ȡ�ۿ���Ϣ
        /// Add by liucaijun at 2011-August-16th
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <returns></returns>
        string GetCussentDiscountMemo(Guid companyId);

        /// <summary>
        /// ���������λ��Ӧ�ҷ������˺���Ϣ
        /// </summary>
        void InsertCompanyBankAccounts(CompanyBankAccountsInfo info);

        /// <summary>
        /// ����������λ��Ӧ�ҷ������˺���Ϣ
        /// </summary>
        void UpdateCompanyBankAccounts(CompanyBankAccountsInfo info);

        /// <summary>
        /// ��ȡ����������λ��Ӧ�ҷ������˺���Ϣ
        /// </summary>
        /// <returns></returns>
        IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList();

        IList<CompanyBankAccountsInfo> GetAllCompanyBankAccountsList(Guid companyId);

        /// <summary>
        /// ɾ��ָ��������λ��Ӧ�ҷ������˺���Ϣ
        /// </summary>
        /// <returns></returns>
        void DelCompanyBankAccounts(CompanyBankAccountsInfo info);

        /// <summary>���ݹ�˾ID�ҳ���Ӧ�İ󶨵�������λ
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyCussentByFilialeId(Guid filialeId);

        /// <summary>CompanyBalanceDetail
        /// </summary>
        /// <returns></returns>
        IList<CompanyBalanceDetailInfo> GetCompanyBalanceDetailList();

        /// <summary>
        ///  ���ҹ�Ӧ����������0��������˾ CompanyBalanceDetail
        /// </summary>
        /// <returns></returns>
        IList<Guid> GetCompanyBalanceDetailFilialeIdList(Guid companyId);

        /// <summary>CompanyBalance
        /// </summary>
        /// <returns></returns>
        IList<CompanyBalanceInfo> GetCompanyBalanceList();

        /// <summary>���湩Ӧ�̰󶨵���˾
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="filialeId">��˾ID</param>
        /// <returns></returns>
        bool SaveCompanyBindingFiliale(Guid companyId, Guid filialeId);

        /// <summary>ɾ����Ӧ�̰󶨵���˾
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="filialeId">��˾ID</param>
        /// <returns></returns>
        bool DeleteCompanyBindingFiliale(Guid companyId, Guid filialeId);

        /// <summary>��Ӧ�Ƿ�󶨸ù�˾
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="filialeId">��˾ID</param>
        /// <returns></returns>
        bool GetCompanyIsBindingFiliale(Guid companyId, Guid filialeId);

        /// <summary>��ȡ��Ӧ�̰󶨵Ĺ�˾Ids
        /// </summary>
        /// <param name="companyId">��Ӧ��Id</param>
        /// <returns></returns>
        IList<Guid> GetCompanyBindingFiliale(Guid companyId);

        /// <summary>��ȡ��Ӧ��Ӧ���� ADD 2015-03-12  ������
        /// </summary>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="companyName">��Ӧ������</param>
        /// <param name="year">���</param>
        /// <param name="initData"></param>
        /// <returns></returns>
        IList<CompanyPaymentDaysInfo> GetCompanyPaymentDaysList(Guid filialeId, string companyName, int year, bool initData);

        /// <summary>��ȡû���������ڵĹ�Ӧ�̵���Ӧ���� ADD 2015-06-11  ������
        /// </summary>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="companyName">��Ӧ������</param>
        /// <param name="initData"></param>
        /// <returns></returns>
        IList<CompanyPaymentDaysInfo> GetCompanyNotPaymentDaysList(Guid filialeId, string companyName, bool initData);


        /// <summary>  �¼ӻ�ȡ���������� add by liangcanren at 2015-08-10
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        CompanyPaymentDaysInfo GetCompanyNotPaymentDaysInfos(Guid filialeId, int year);


        /// <summary>
        /// ��ȡ�ɹ���Ӧ���������ϸ ADD 2015-08-07  ������
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        IList<CompanyPaymentDaysInfo> GetPurchasingCompanyPaymentDaysInfos(Guid filialeId, int year, string companyName);

        /// <summary>��ȡ�ɹ������ ADD 2015-08-07  ������
        /// </summary>
        /// <param name="filialeId">��˾ID</param>
        /// <param name="year">���</param>
        /// <returns></returns>
        CompanyPaymentDaysInfo GetPurchasingCompanyPaymentDaysInfo(Guid filialeId, int year);

        /// <summary>�жϴ�������λ�Ƿ���� ADD 2015-03-14 ������
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <returns></returns>
        Boolean IsExistCompanyInfo(Guid companyId);

        /// <summary>
        /// ���¹�Ӧ�������Ƿ�����
        /// </summary>
        /// <param name="companyId">������λID</param>
        /// <param name="completeState">�Ƿ�����</param>
        /// <param name="expire">�Ƿ����</param>
        Boolean UpdateQualificationCompleteState(Guid companyId, int completeState,string expire);

        /// <summary>
        /// ��Ӧ���������ݲ�ѯ
        /// </summary>
        /// <param name="companyType">������λ����</param>
        /// <param name="state">������λ״̬</param>
        /// <param name="searchKey">�����ؼ���(����)</param>
        /// <param name="complete">�Ƿ�����</param>
        /// <param name="expire">�Ƿ����</param>
        /// <returns></returns>
        IList<SupplierGoodsInfo> GetSupplierGoodsInfos(CompanyType companyType, State state,string searchKey,int complete,int expire);

        /// <summary>
        /// ��ȡ������λ�ֵ�
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        IDictionary<Guid, String> GetCompanyDic();


        /// <summary>�жϴ�������λ�Ƿ񱻸���
        /// </summary>
        /// <param name="thirdCompanyID">��������˾ID</param>
        /// <returns></returns>
        Boolean IsAbeyanced(Guid thirdCompanyID);

        /// <summary>��ȡ�ѱ������Ĺ�˾�б� 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Guid> GetRelevanceFilialeIdList();

        /// <summary>
        /// ͨ��������˾��ȡ��Ӧ��������λ
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        CompanyCussentInfo GetCompanyByRelevanceFilialeId(Guid filialeId);

        /// <summary>
        /// ���ݹ�����˾��ȡ������λID
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        Guid GetCompanyIdByRelevanceFilialeId(Guid filialeId);

        /// <summary>
        /// ���ݹ�����˾��ȡ������λID�б�
        /// </summary>
        /// <param name="filialeIds"></param>
        /// <returns></returns>
        List<PurchaseFilialeAuth> GetCompanyIdNameListByRelevanceFilialeIds(IEnumerable<Guid> filialeIds);

        /// <summary>
        /// ͨ��������λid��ȡ�����Ĺ�˾id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Guid GetRelevanceFilialeIdByCompanyId(Guid companyId);


        Dictionary<Guid, Guid> GetGoodsAndCompanyDic(IEnumerable<Guid> goodsId);

        Dictionary<Guid, String> GetAbroadCompanyList();
    }
}