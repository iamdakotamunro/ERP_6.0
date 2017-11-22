using System;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>往来账核对类接口
    /// </summary>
    public interface IReckoningCheck
    {
        /// <summary> 根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        ReckoningCheckInfo GetReckoningCheckByReckoningId(Guid reckoningId);

        /// <summary> 插入一条往来账核对信息
        /// </summary>
        /// <param name="reckoningCheckInfo"></param>
        void InsertReckoningCheck(ReckoningCheckInfo  reckoningCheckInfo);

        /// <summary>更新往来账核对信息
        /// </summary>
        /// <param name="reckoningCheckInfo"> </param>
        void UpdateReckoningCheck(ReckoningCheckInfo reckoningCheckInfo);

        /// <summary> 删除往来账核对信息
        /// </summary>
        /// <param name="reckoningId"></param>
        void DeleteReckoningCheck(Guid reckoningId);
    }
}
