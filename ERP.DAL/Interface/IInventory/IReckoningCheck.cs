using System;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>�����˺˶���ӿ�
    /// </summary>
    public interface IReckoningCheck
    {
        /// <summary> �����˵���Ų�ѯ�����˺˶���Ϣ
        /// </summary>
        /// <returns></returns>
        ReckoningCheckInfo GetReckoningCheckByReckoningId(Guid reckoningId);

        /// <summary> ����һ�������˺˶���Ϣ
        /// </summary>
        /// <param name="reckoningCheckInfo"></param>
        void InsertReckoningCheck(ReckoningCheckInfo  reckoningCheckInfo);

        /// <summary>���������˺˶���Ϣ
        /// </summary>
        /// <param name="reckoningCheckInfo"> </param>
        void UpdateReckoningCheck(ReckoningCheckInfo reckoningCheckInfo);

        /// <summary> ɾ�������˺˶���Ϣ
        /// </summary>
        /// <param name="reckoningId"></param>
        void DeleteReckoningCheck(Guid reckoningId);
    }
}
