using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// ������λ������Ϣ��ȡ�ӿ���
    /// </summary>
    public interface ICompanyClass
    {
        /// <summary>
        /// ���������λ��Ϣ
        /// </summary>
        /// <param name="companyClass">������λ������ʵ��</param>
        void Insert(CompanyClassInfo companyClass);

        /// <summary>
        /// ����������λ��Ϣ
        /// </summary>
        /// <param name="companyClass">������λ������ʵ��</param>
        void Update(CompanyClassInfo companyClass);

        /// <summary>
        /// ɾ��������λ����
        /// </summary>
        /// <param name="companyClassId">������λ���</param>
        void Delete(Guid companyClassId);

        /// <summary>
        /// ��ȡָ�����������λ��ʵ��
        /// </summary>
        /// <param name="companyClassId">������λ���</param>
        /// <returns></returns>
        CompanyClassInfo GetCompanyClass(Guid companyClassId);

        /// <summary>
        /// ��ȡָ����ŵ�������λ����
        /// </summary>
        /// <param name="companyClassId">������λ���</param>
        /// <returns></returns>
        CompanyClassInfo GetParentCompanyClass(Guid companyClassId);

        /// <summary>
        /// ��ȡ������λ�б�
        /// </summary>
        /// <returns></returns>
        IList<CompanyClassInfo> GetCompanyClassList();

        /// <summary>
        /// ��ȡָ��������ӷ����б�
        /// </summary>
        /// <param name="parentCompanyClassId">��������</param>
        /// <returns></returns>
        IList<CompanyClassInfo> GetChildCompanyClassList(Guid parentCompanyClassId);

        /// <summary>
        /// ��ȡ�ӷ�������
        /// </summary>
        /// <param name="companyClassId">������</param>
        /// <returns>����int��,�ӷ�������</returns>
        int GetChildCompanyClassCount(Guid companyClassId);

        /// <summary>
        /// ����ֱ�Ӱ󶨵��÷���Ĺ�˾����,�����ӷ����й�˾������
        /// </summary>
        /// <param name="companyClassId">��˾���</param>
        /// <returns>����int��,�󶨹�˾��������</returns>
        int GetFireCompanyCount(Guid companyClassId);
    }
}
