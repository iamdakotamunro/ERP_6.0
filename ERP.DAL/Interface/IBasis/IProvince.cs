//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��5��30��
// �ļ�������:����
// ����޸�ʱ��:2007��5��30��
// ���һ���޸���:����
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>ʡ�� 2015-05-08  ������  ���Ż����룬����ע�ͣ�ɾ�����÷�����
    /// </summary>
    public interface IProvince
    {
        /// <summary>���ʡ����Ϣ
        /// </summary>
        /// <param name="provinceInfo">ʡ����ʵ��</param>
        int Insert(ProvinceInfo provinceInfo);

        /// <summary> ����ʡ�ݼ�¼
        /// </summary>
        /// <param name="provinceInfo">ʡ����ʵ��</param>
        int Update(ProvinceInfo provinceInfo);

        /// <summary> ɾ��ʡ����Ϣ
        /// </summary>
        /// <param name="provinceId">ʡ��Id</param>
        int Delete(Guid provinceId);
        
        /// <summary> �����ṩId��ȡʡ����ʵ��
        /// </summary>
        /// <param name="provinceId">ʡ��Id</param>
        /// <returns>����ʡ�ݶ���</returns>
        ProvinceInfo GetProvince(Guid provinceId);

        /// <summary> ��ȡ�������
        /// </summary>
        /// <returns></returns>
        IList<ProvinceInfo> GetProvinceList();

        /// <summary>�������ڹ���ID��ȡʡ���б�
        /// </summary>
        /// <param name="countryId">����Id</param>
        /// <returns>����ʡ���б�</returns>
        IList<ProvinceInfo> GetProvinceList(Guid countryId);
        
        /// <summary>��ȡʹ�ø�ʡ����Ϣ�ĳ�����
        /// </summary>
        /// <param name="provinceId">ʡ��Id</param>
        /// <returns></returns>
        int GetProvinceUseCount(Guid provinceId);
    }
}
