using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model.Invoice;
using ERP.Model.Finance;

namespace ERP.DAL.Interface.IInventory
{
    public interface IInvoiceApply
    {
        bool Insert(InvoiceApplyInfo invoiceApplyInfo,List<InvoiceApplyDetailInfo> detailInfos);

        bool Modify(InvoiceApplyInfo invoiceApplyInfo, List<InvoiceApplyDetailInfo> detailInfos);

        bool Update(Guid applyId,int applyState,string applyRemark,string retreatRemark,Guid personnelId);

        bool UpdateRemark(Guid applyId,string applyRemark);

        List<InvoiceApplyInfo> GetInvoiceApplyInfos(DateTime startTime,DateTime endTime,string searchKey,IEnumerable<int> applyStates,int applyType,int applySourceType,Guid saleFilialeId,Guid targetId);

        List<InvoiceApplyInfo> GetInvoiceApplyInfosWithPage(DateTime startTime, DateTime endTime, string searchKey, IEnumerable<int> applyStates, int applyType, int applySourceType,int applyKind, Guid targetId,int pageIndex,int pageSize,out long total);

        List<InvoiceApplyDetailInfo> GetInvoiceApplyDetailInfos(Guid applyId);

        InvoiceApplyInfo GetInvoiceApplyInfo(Guid applyId);

        bool IsExists(Guid applyId,string invoiceNo);

        bool InsertRelation(InvoiceRelationInfo relationInfo,List<InvoiceBindGoods> bindGoods);

        List<InvoiceRelationInfo> GetInvoiceRelationInfos(Guid applyId);

        List<InvoiceDTO> GetBindGoodses(DateTime startTime,DateTime endTime);
    }
}
