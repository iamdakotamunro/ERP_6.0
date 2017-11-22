using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 借记单
    /// </summary>
    public interface IDebitNote
    {
        /// <summary>
        /// 获取所有借记单
        /// </summary>
        /// <returns></returns>
        IList<DebitNoteInfo> GetDebitNoteList();

        /// <summary>
        /// 根据条件筛选数据
        /// </summary>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="state">借记单状态</param>
        /// <param name="warehouseId">仓库id</param>
        /// <param name="companyId">供应商id</param>
        /// <param name="personResponsibleId">责任人id</param>
        /// <param name="activityTimeStart">活动开始时间</param>
        /// <param name="activityTimeEnd">活动结束时间</param>
        /// <param name="titleOrMemo">标题或备注</param>
        /// <param name="startPage">开始页索引</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="recordCount">查询数据的总和</param>
        /// <returns></returns>
        IList<DebitNoteInfo> GetDebitNoteList(DateTime startTime, DateTime endTime, int state, Guid warehouseId,
            Guid companyId, Guid personResponsibleId, string activityTimeStart, string activityTimeEnd,
            string titleOrMemo, int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// 根据采购单ID获取借记单
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        DebitNoteInfo GetDebitNoteInfo(Guid purchasingId);

        /// <summary>
        /// 根据新采购单ID获取借记单
        /// </summary>
        /// <param name="newPurchasingId"></param>
        /// <returns></returns>
        DebitNoteInfo GetDebitNoteInfoByNewPurchasingId(Guid newPurchasingId);

        Guid GetPurchasingIdByNewPurchasingId(Guid newPurchasingId);

        /// <summary>
        /// 添加借记单和明细
        /// </summary>
        /// <param name="info"></param>
        /// <param name="debitNoteDetailList">借记单明细</param>
        bool AddPurchaseSetAndDetail(DebitNoteInfo info, List<DebitNoteDetailInfo> debitNoteDetailList);

        /// <summary>
        /// 删除借记单
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        void DeleteDebitNote(Guid purchasingId);

        /// <summary>
        /// 根据采购单ID修改借记单状态
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="state"></param>
        void UpdateDebitNoteStateByPurchasingId(Guid purchasingId, int state);

        /// <summary>
        /// 根据新采购单ID修改借记单状态
        /// </summary>
        /// <param name="newPurchasingId"></param>
        /// <param name="state"></param>
        void UpdateDebitNoteStateByNewPurchasingId(Guid newPurchasingId, int state);

        /// <summary>
        /// 根据采购单ID修改借记单新采购单ID
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="newPurchasingId"></param>
        void UpdateDebitNoteNewPurchasingIdByPurchasingId(Guid purchasingId, Guid newPurchasingId);

        ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 获取借记单明细
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        /// <returns></returns>
        IList<DebitNoteDetailInfo> GetDebitNoteDetailList(Guid purchasingId);

        /// <summary>
        /// 根据采购单ID和商品ID更改借记单明细状态
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="goodsId"></param>
        /// <param name="state"></param>
        /// <param name="arrivalCount">实到数量</param>
        /// <param name="isUpdate">是否包含更改“实到数量”</param>
        void UpdateDebitNoteDetail(Guid purchasingId, Guid goodsId, int state, int arrivalCount, bool isUpdate);

        /// <summary>
        /// 添加借记单明细
        /// </summary>
        /// <param name="detailInfo"></param>
        void AddDebitNoteDetail(DebitNoteDetailInfo detailInfo);

        /// <summary>添加赠品借记单备注  ADD 2015-02-06  陈重文
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="memo"> </param>
        void AddDebitNoteMemo(Guid purchasingId, string memo);
    }
}
