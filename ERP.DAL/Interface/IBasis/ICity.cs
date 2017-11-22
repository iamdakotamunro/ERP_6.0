//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��12��27��
// �ļ�������:����
// ����޸�ʱ��:2007��12��27��
// ���һ���޸���:����
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>���� 2015-05-08  ������  ���Ż����룬����ע�ͣ�ȥ����ʹ�÷�����
    /// </summary>
    public interface ICity
    {
        /// <summary>�ڱ��в��������Ϣ
        /// </summary>
        /// <param name="city">������ʵ��</param>
        int Insert(CityInfo city);

        /// <summary>���³��м�¼
        /// </summary>
        /// <param name="city">������ʵ��</param>
        int Update(CityInfo city);

        /// <summary> ɾ��ָ������
        /// </summary>
        /// <param name="cityId">����Id</param>
        int Delete(Guid cityId);

        /// <summary>��ȡ���г����б�
        /// </summary>
        /// <returns></returns>
        IList<CityInfo> GetCityList();

        /// <summary>��������ʡ��ID��ȡ�����б�
        /// </summary>
        /// <param name="provinceId">ʡ��Id</param>
        /// <returns>���س����б�</returns>
        IList<CityInfo> GetCityByProvince(Guid provinceId);

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        CityInfo GetCityInfo(Guid cityId);
    }
}
