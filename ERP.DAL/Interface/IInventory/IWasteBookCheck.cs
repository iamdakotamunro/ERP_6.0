using System;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 账目操作接口
    /// </summary>
    public interface IWasteBookCheck
    {
        // End add
        /// <summary>
        /// 添加一笔账目核对记录
        /// </summary>
        /// <param name="wasteBookCheck">账目核对信息类</param>
        void Insert(WasteBookCheckInfo wasteBookCheck);
        
        ///<summary>
        ///更改一笔账目核对记录
        /// </summary>
        /// <param name="wasteBookCheck">账目核对信息类</param>
        void Update(WasteBookCheckInfo wasteBookCheck);
        
        /// <summary>
        /// 获取指定资金帐号核对记录
        /// </summary>
        /// <param name="wasteBookId">帐号操作id</param>
        /// <returns></returns>
        WasteBookCheckInfo GetWasteBookCheck(Guid wasteBookId);

        /// <summary>
        /// 删除一笔记录
        /// </summary>
        /// <param name="wasteBookId"></param>
        void DeleteWasteBookCheck(Guid wasteBookId);
    }
}
