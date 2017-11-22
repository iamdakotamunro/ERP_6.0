using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IMediumReckoning
    {
        /// <summary>
        /// 插入往来账
        /// </summary>
        /// <param name="info"></param>
        void Insert(MediumReckoningInfo info);

        /// <summary>
        /// 删除往来账
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="reckoningId"> </param>
        /// <param name="handleType"> </param>
        /// <param name="count"> </param>
        void Delete(Guid checkId,Guid reckoningId,int handleType,int count);

        /// <summary>
        /// 将往来账更新到
        /// </summary>
        /// <param name="checkId"></param>
        void UpdateData(Guid checkId);

        /// <summary>
        /// 将新增的往来账添加到往来账记录中
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="companyId">对账往来单位ID</param>
        void TransferData(Guid checkId,Guid companyId);


        /// <summary>获取公司异常总调账金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="isReceivable">是否应收，(true：应收；false：应付) </param>
        /// <returns></returns>
        IList<ExceptionReckoningInfo> GetAccountReceivableByFilialeId(Guid checkId, Boolean isReceivable);

        /// <summary>获取其他公司异常总调账金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="isReceivable">是否应收，(true：应收；false：应付) </param>
        /// <returns></returns>
        ExceptionReckoningInfo GetAccountReceivableByElseFilialeId(Guid checkId,Boolean isReceivable);

        /// <summary>
        /// 获取付款调账总金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        decimal GetAccountPayable(Guid checkId);

        /// <summary>
        /// 获取往来帐
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="handleType"> </param>
        /// <param name="count">查询条数</param>
        /// <returns></returns>
        IList<MediumReckoningInfo> GetList(Guid checkId,int handleType,int count);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="addList"></param>
        /// <param name="tableName"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        int BitchInsert(IList<MediumReckoningInfo> addList, string tableName, Dictionary<string, string> dics);

        /// <summary>对账完成删除临时往来帐数据（对账记录生成收付款后）  2015-04-16  陈重文
        /// </summary>
        /// <param name="checkId"></param>
        void DeleteTempReckoning(Guid checkId);
    }
}
