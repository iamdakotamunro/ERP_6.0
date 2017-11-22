using System;
using System.Collections.Generic;
using System.Data;
using Keede.Ecsoft.Model;

namespace CMS.BLL.IOrders
{
    public interface ILensProcessManager
    {
        /// <summary>
        /// 添加加工单和明细
        /// </summary>
        /// <param name="info"></param>
        /// <param name="detailsList"></param>
        bool InsertLensProcessAndDetails(LensProcessInfo info, IList<LensProcessDetailsInfo> detailsList);

        /// <summary>
        /// 添加加工单
        /// </summary>
        /// <param name="goodsOrderInfo"></param>
        /// <param name="goodsOrderDetailList"></param>
        bool InsertLensProcess(GoodsOrderInfo goodsOrderInfo, IList<GoodsOrderDetailInfo> goodsOrderDetailList);

        /// <summary>
        /// 更改加工单操作状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operateState">操作状态</param>
        /// <param name="clew"></param>
        bool UpdateLensProcessOperateState(Guid id, int operateState, string clew);

        /// <summary>
        /// 更改加工单单据状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state">单据状态</param>
        /// <param name="clew"></param>
        bool UpdateLensProcessState(Guid id, int state, string clew);

        /// <summary>
        /// 根据订单ID更改加工单单据状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="state"></param>
        /// <param name="clew"></param>
        bool UpdateLensProcessStateByOrderId(Guid orderId, int state, string clew);

        /// <summary>
        /// 获取加工单明细(用于作废)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DataTable GetLensProcessDetailsDt(Guid id);

        /// <summary>
        /// 根据加工单ID批量更新加工单操作状态
        /// </summary>
        /// <param name="idList">加工单ID集合</param>
        /// <param name="operateState">操作状态</param>
        /// <param name="clew"></param>
        bool UpateLensProcessOperateStateByIds(IList<Guid> idList, int operateState, string clew);

        /// <summary>
        /// 根据加工单ID批量更新加工单单据状态
        /// </summary>
        /// <param name="idList">加工单ID集合</param>
        /// <param name="state">单据状态</param>
        /// <param name="clew"></param>
        bool UpdateLensProcessStateByIds(IList<Guid> idList, int state, string clew);

        /// <summary>
        /// 根据配镜单ID集合，批量获取配镜单
        /// zhangfan added at 2013-June-9th
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<LensProcessInfo> GetLensProcessListByIdCollection(IEnumerable<Guid> ids);

        /// <summary>
        /// 作废加工单(未加事务TransactionScope)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="clew"></param>
        /// <returns></returns>
        bool CancelLensProcess(Guid orderId, string clew);

        /// <summary>
        /// 作废加工单(有事务TransactionScope)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="clew"></param>
        /// <returns></returns>
        bool CancelLensProcessTransactionScope(Guid orderId, string clew);

        /// <summary>
        /// 添加加工单(服务用)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderNo"></param>
        /// <param name="consignee"></param>
        /// <param name="lensProcessList"></param>
        /// <param name="lensProcessDetailsList"></param>
        /// <returns></returns>
        bool InsertLensProcessAndDetailsList(Guid orderId, string orderNo, string consignee, IList<LensProcessInfo> lensProcessList, IList<LensProcessDetailsInfo> lensProcessDetailsList);
    }
}
