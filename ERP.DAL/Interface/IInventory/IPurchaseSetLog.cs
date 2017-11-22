using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 商品采购设置
    /// </summary>
    public interface IPurchaseSetLog
    {
        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        IList<PurchaseSetLogInfo> GetPurchaseSetLogList();

        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        IList<PurchaseSetLogInfo> GetPurchaseSetLogList(Guid goodsId, Guid warehouseId, Guid hostingFilialeId);

        IList<PurchaseSetLogInfo> GetPurchaseSetLogListByPage(Guid goodsId, Guid warehouseId,Guid hostingFilialeId, int startPage,
            int pageSize, out long recordCount);

        /// <summary>
        /// 获取商品采购设置变更信息
        /// </summary>
        /// <param name="logId">日志ID</param>
        /// <returns></returns>
        PurchaseSetLogInfo GetPurchaseSetLogInfo(Guid logId);

        /// <summary>
        /// 添加商品采购设置变更信息
        /// </summary>
        /// <param name="info"></param>
        void AddPurchaseSetLog(PurchaseSetLogInfo info);

        /// <summary>
        /// 修改商品采购设置变更信息
        /// </summary>
        /// <param name="info"></param>
        void UpdatePurchaseSetLog(PurchaseSetLogInfo info);

        /// <summary>
        /// 根据商品ID删除商品采购设置变更信息
        /// </summary>
        /// <param name="goodsId"></param>
        void DeletePurchaseSetLog(Guid goodsId);

        /// <summary>
        /// 用于判断是否存在未审核的调价记录
        /// </summary>
        /// <param name="goodsIds">主商品Id列表</param>
        /// <param name="status">审核状态</param>
        /// <param name="valueType">0,调高，1,调低，-1、所有</param>
        /// <param name="logType">商品采购设置变更值类型：1、采购价，2、报备天数</param>
        /// <returns></returns>
        bool IsExistNoAuditSetLog(List<Guid> goodsIds,int status,int valueType,int logType);
    }
}
