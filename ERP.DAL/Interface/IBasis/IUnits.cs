//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2007��10��30��
// �ļ�������:����
// ����޸�ʱ��:2007��10��30��
// ���һ���޸���:����
//================================================

using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary> ������λ 2015-05-08  ������  ���Ż����룬����ע�ͣ�ȥ����ʹ�÷�����
    /// </summary>
    public interface IUnits
    {
        /// <summary> ��ӻ�����λ
        /// </summary>
        /// <param name="units">��λ����</param>
        int Insert(UnitsInfo units);

        /// <summary> ���»�����λ
        /// </summary>
        /// <param name="units">��λ����</param>
        int Update(UnitsInfo units);

        /// <summary> ɾ��������λ
        /// </summary>
        /// <param name="unitsId">������λ���</param>
        int Delete(Guid unitsId);

        /// <summary> ��ȡ������λ
        /// </summary>
        /// <param name="unitsId">������λId</param>
        /// <returns>���ػ�����λ��ʵ��</returns>
        UnitsInfo GetUnits(Guid unitsId);

        /// <summary> ��ȡ������λ�б�
        /// </summary>
        /// <returns>���ػ�����λ�б�</returns>
        IList<UnitsInfo> GetUnitsList();
    }
}
