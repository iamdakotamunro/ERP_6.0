using System;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// ��Ŀ�����ӿ�
    /// </summary>
    public interface IWasteBookCheck
    {
        // End add
        /// <summary>
        /// ���һ����Ŀ�˶Լ�¼
        /// </summary>
        /// <param name="wasteBookCheck">��Ŀ�˶���Ϣ��</param>
        void Insert(WasteBookCheckInfo wasteBookCheck);
        
        ///<summary>
        ///����һ����Ŀ�˶Լ�¼
        /// </summary>
        /// <param name="wasteBookCheck">��Ŀ�˶���Ϣ��</param>
        void Update(WasteBookCheckInfo wasteBookCheck);
        
        /// <summary>
        /// ��ȡָ���ʽ��ʺź˶Լ�¼
        /// </summary>
        /// <param name="wasteBookId">�ʺŲ���id</param>
        /// <returns></returns>
        WasteBookCheckInfo GetWasteBookCheck(Guid wasteBookId);

        /// <summary>
        /// ɾ��һ�ʼ�¼
        /// </summary>
        /// <param name="wasteBookId"></param>
        void DeleteWasteBookCheck(Guid wasteBookId);
    }
}
