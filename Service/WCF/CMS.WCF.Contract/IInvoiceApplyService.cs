using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ERP.Model.Invoice;

namespace ERP.Service.Contract
{
    /// <summary>
    /// 发票申请接口
    /// </summary>
    public partial interface IService
    {
        /// <summary>
        /// 添加发票申请
        /// </summary>
        /// <param name="personnelId">申请人Id(操作日志需要)</param>
        /// <param name="realName">申请人(操作日志需要)</param>
        /// <param name="invoiceApply">发票申请信息</param>
        /// <param name="invoiceApplyDetailInfos">加盟店单据明细（ps:订单类型的发票申请传null）</param>
        /// <returns></returns>
        [OperationContract]
        ResultInfo InsertApply(Guid personnelId,string realName,InvoiceApplyInfo invoiceApply,List<InvoiceApplyDetailInfo> invoiceApplyDetailInfos);

        /// <summary>
        /// 更新发票申请
        /// </summary>
        /// <param name="personnelId">操作人Id(操作日志需要)</param>
        /// <param name="realName">操作人(操作日志需要)</param>
        /// <param name="invoiceApply">发票申请信息</param>
        /// <param name="invoiceApplyDetailInfos">加盟店单据明细（ps:订单类型的发票申请传null）</param>
        /// <returns></returns>
        [OperationContract]
        ResultInfo UpdateApply(Guid personnelId, string realName, InvoiceApplyInfo invoiceApply, List<InvoiceApplyDetailInfo> invoiceApplyDetailInfos);

        /// <summary>
        /// 核退重新编辑发票申请
        /// </summary>
        /// <param name="personnelId">操作人Id(操作日志需要)</param>
        /// <param name="realName">操作人(操作日志需要)</param>
        /// <param name="invoiceApply">发票申请Id</param>
        /// <param name="invoiceApplyDetailInfos">加盟店单据明细（ps:订单类型的发票申请传null）</param>
        /// <returns></returns>
        [OperationContract]
        ResultInfo AgainApply(Guid personnelId, string realName, InvoiceApplyInfo invoiceApply, List<InvoiceApplyDetailInfo> invoiceApplyDetailInfos);

        /// <summary>
        /// 发票申请作废
        /// </summary>
        /// <param name="personnelId">操作人Id(操作日志需要)</param>
        /// <param name="realName">操作人</param>
        /// <param name="applyId">发票申请Id</param>
        /// <param name="remark">作废理由</param>
        /// <returns></returns>
        [OperationContract]
        ResultInfo CancelApply(Guid personnelId,string realName, Guid applyId,string remark);

        /// <summary>
        /// 发票申请查询
        /// </summary>
        /// <param name="startTime">开始时间(没时间段使用默认值)</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="fromShop">是否来自门店(true 门店，false B2C)</param>
        /// <param name="searchKey">要货申请号/订单号</param>
        /// <param name="targetId">门店Id/销售平台Id</param>
        /// <param name="applyStates">发票申请状态列表</param>
        /// <param name="invoiceType">发票申请类型(0 全部 1 增值税专用发票 2 普通 3 收据)</param>
        /// <param name="applyKind">票据类型(0 全部  1、贷款发票 2、保证金)</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        [OperationContract]
        PageResult<InvoiceApplyGridDTO> SearchApply(DateTime startTime,DateTime endTime,bool fromShop,string searchKey,Guid targetId,IEnumerable<int> applyStates,int invoiceType,int applyKind,int pageIndex,int pageSize);

        /// <summary>
        /// 获取发票申请信息
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultInfo<InvoiceApplyInfo> GetData(Guid applyId);
    }
}
