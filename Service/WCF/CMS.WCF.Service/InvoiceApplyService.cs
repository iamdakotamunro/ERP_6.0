using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Enum.ApplyInvocie;
using ERP.Model.Invoice;
using ERP.Service.Contract;

namespace ERP.Service.Implement
{
    public partial class Service
    {
        /// <summary>
        /// 添加发票申请
        /// </summary>
        /// <param name="personnelId">申请人Id(操作日志需要)</param>
        /// <param name="realName">申请人(操作日志需要)</param>
        /// <param name="invoiceApply">发票申请信息</param>
        /// <param name="invoiceApplyDetailInfos">加盟店单据明细（ps:订单类型的发票申请传null）</param>
        /// <returns></returns>
        public ResultInfo InsertApply(Guid personnelId,string realName, InvoiceApplyInfo invoiceApply,
            List<InvoiceApplyDetailInfo> invoiceApplyDetailInfos)
        {
            var result = _invoiceApplySerivce.Insert(personnelId,realName,invoiceApply, invoiceApplyDetailInfos);
            return result.IsSuccess?ResultInfo.Success():ResultInfo.Failure(result.Message);
        }

        /// <summary>
        /// 更新发票申请
        /// </summary>
        /// <param name="personnelId">操作人Id(操作日志需要)</param>
        /// <param name="realName">操作人(操作日志需要)</param>
        /// <param name="invoiceApply">发票申请信息</param>
        /// <param name="invoiceApplyDetailInfos">加盟店单据明细（ps:订单类型的发票申请传null）</param>
        /// <returns></returns>
        public ResultInfo UpdateApply(Guid personnelId, string realName, InvoiceApplyInfo invoiceApply,
            List<InvoiceApplyDetailInfo> invoiceApplyDetailInfos)
        {
            var result = _invoiceApplySerivce.Update(personnelId, realName, invoiceApply, invoiceApplyDetailInfos);
            return result.IsSuccess ? ResultInfo.Success() : ResultInfo.Failure(result.Message);
        }

        /// <summary>
        /// 核退重新编辑发票申请
        /// </summary>
        /// <param name="personnelId">操作人Id(操作日志需要)</param>
        /// <param name="realName">操作人(操作日志需要)</param>
        /// <param name="invoiceApply">发票申请Id</param>
        /// <param name="invoiceApplyDetailInfos">加盟店单据明细（ps:订单类型的发票申请传null）</param>
        /// <returns></returns>
        public ResultInfo AgainApply(Guid personnelId, string realName, InvoiceApplyInfo invoiceApply,
            List<InvoiceApplyDetailInfo> invoiceApplyDetailInfos)
        {
            var result = _invoiceApplySerivce.Again(personnelId, realName, invoiceApply, invoiceApplyDetailInfos);
            return result.IsSuccess ? ResultInfo.Success() : ResultInfo.Failure(result.Message);
        }

        /// <summary>
        /// 发票申请作废
        /// </summary>
        /// <param name="personnelId">操作人Id(操作日志需要)</param>
        /// <param name="realName">操作人</param>
        /// <param name="applyId">发票申请Id</param>
        /// <param name="remark">作废理由</param>
        /// <returns></returns>
        public ResultInfo CancelApply(Guid personnelId, string realName, Guid applyId, string remark)
        {
            var result = _invoiceApplySerivce.Cancel(personnelId, realName, applyId,remark);
            return result.IsSuccess ? ResultInfo.Success() : ResultInfo.Failure(result.Message);
        }

        /// <summary>
        /// 发票申请查询
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="fromShop">是否来自门店(true 门店，false B2C)</param>
        /// <param name="searchKey">要货申请号/订单号</param>
        /// <param name="targetId">门店Id/销售平台Id</param>
        /// <param name="applyStates">发票申请状态列表</param>
        /// <param name="invoiceType">发票申请类型</param>
        /// <param name="applyKind"></param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public PageResult<InvoiceApplyGridDTO> SearchApply(DateTime startTime,DateTime endTime,bool fromShop,string searchKey, Guid targetId, IEnumerable<int> applyStates, int invoiceType,int applyKind,
            int pageIndex, int pageSize)
        {
            long total;
            var result = _invoiceApplySerivce.SearchApply(startTime,endTime,searchKey, targetId, applyStates, invoiceType,fromShop?(int)ApplyInvoiceSourceType.League:(int)ApplyInvoiceSourceType.Order,applyKind, pageIndex,pageSize,out total);
            return PageResult<InvoiceApplyGridDTO>.Success(result, total);
        }

        /// <summary>
        /// 获取发票申请信息
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public ResultInfo<InvoiceApplyInfo> GetData(Guid applyId)
        {
            var result = _invoiceApplySerivce.GetData(applyId);
            return result==null? ResultInfo<InvoiceApplyInfo>.Failure("对应的发票申请已不存在！") : ResultInfo<InvoiceApplyInfo>.Success(result);
        }
    }
}
