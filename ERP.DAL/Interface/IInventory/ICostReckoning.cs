using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICostReckoning
    {
        // Add by tianys at 2009-05-11 
        /// <summary>
        /// 按日期,账单类型获取往来账
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="receiptType"></param>
        /// <param name="auditingState"></param>
        /// <param name="filialeId"></param>
        /// <param name="branchId"></param>
        /// <param name="assumeFilialeId"></param>
        /// <returns></returns>
        IList<CostReckoningInfo> GetReckoningList(Guid companyId, DateTime startDate, DateTime endDate, int receiptType, float auditingState, Guid filialeId, Guid branchId, Guid assumeFilialeId);
        // End add




        /// <summary>
        /// 添加帐务记录数据
        /// </summary>
        /// <param name="reckoningInfo">帐务记录条目类</param>
        int Insert(CostReckoningInfo reckoningInfo);

        /// <summary>
        /// 获取指定帐务记录
        /// </summary>
        /// <param name="reckoningId">帐务记录Id</param>
        /// <returns></returns>
        CostReckoningInfo GetReckoning(Guid reckoningId);

        /// <summary>
        /// 获取往来账
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        /// <returns></returns>
        IList<CostReckoningInfo> GetReckoningList(Guid companyId);

        /// <summary>
        /// 获取指定往来单位的往来总帐
        /// </summary>
        /// <param name="companyId">往来单位编号</param>
        decimal GetTotalled(Guid companyId);

        /// <summary>
        /// 审核费用记录
        /// </summary>
        /// <param name="tradeCode"></param>
        void Auditing(string tradeCode);

        /// <summary>
        /// 修改费用记录
        /// </summary>
        /// <param name="reckoningId">ReckoningId</param>
        /// <param name="accountReceivable">金额</param>
        /// <param name="description">备注</param>
        /// <param name="dateCreated"></param>
        void Update(Guid reckoningId, decimal accountReceivable, string description, DateTime dateCreated);

        /// <summary>
        /// 更新时累加备注信息
        /// </summary>
        /// <param name="reckoningId"></param>
        /// <param name="description"></param>
        void UpdateDescription(Guid reckoningId, String description);

        /// <summary>
        /// 删除费用记录
        /// </summary>
        /// <param name="tradeCode"></param>
        void Delete(string tradeCode);
    }
}
