//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��12��28��
// �ļ�������:����
// ����޸�ʱ��:2007��12��28��
// ���һ���޸���:����
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>���ҽӿ� 2015-05-08  ������  ���Ż����룬����ע�ͣ�ȥ����ʹ�÷�����
    /// </summary>
    public interface ICountry
    {
        /// <summary> ��ӹ�����Ϣ
        /// </summary>
        /// <param name="country">���Ҷ���</param>
        int Insert(CountryInfo country);

        /// <summary> ���¹�����Ϣ
        /// </summary>
        /// <param name="country"></param>
        int Update(CountryInfo country);

        /// <summary> ɾ��ָ������
        /// </summary>
        /// <param name="countryId">���ұ��</param>
        int Delete(Guid countryId);

        /// <summary> ��ȡ������Ϣ
        /// </summary>
        /// <param name="countryId">���ұ��</param>
        /// <returns></returns>
        CountryInfo GetCountry(Guid countryId);

        /// <summary>�������й���
        /// </summary>
        /// <returns></returns>
        IList<CountryInfo> GetCountryList();

        /// <summary>��ȡʹ�ù��ҵ�ʡ����
        /// </summary>
        /// <param name="countryId">���ұ��</param>
        /// <returns>����ʹ�øù��ҵ�ʡ����</returns>
        int GetCountryUseCount(Guid countryId);
    }
}
