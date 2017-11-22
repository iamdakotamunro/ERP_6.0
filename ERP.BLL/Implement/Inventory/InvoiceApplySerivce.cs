using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Interface;
using ERP.DAL.Interface.IInventory;
using ERP.Enum.ApplyInvocie;
using ERP.Model.Invoice;
using OperationLog.Core;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Model;
using ERP.Model.Finance;

namespace ERP.BLL.Implement.Inventory
{
    public class InvoiceApplySerivce
    {
        private static readonly IInvoiceApply _invoiceApply= new InvoiceApply();
        private static readonly IStorageRecordDao _storageRecord = new StorageRecordDao();
        private static readonly IGoodsOrderDetail _goodsOrderDetail = new GoodsOrderDetail();
        readonly IOperationLogManager _operationLogManager = new OperationLogManager();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="invoiceApplyInfo"></param>
        /// <param name="details"></param>
        /// <param name="personnelid"></param>
        /// <returns></returns>
        public ResultInfo Insert(Guid personnelid,string realName,InvoiceApplyInfo invoiceApplyInfo, List<InvoiceApplyDetailInfo> details)
        {
            try
            {
                if (invoiceApplyInfo.ApplySourceType == (int) ApplyInvoiceSourceType.League)
                {
                    invoiceApplyInfo.Amount = details.Sum(ent => ent.TotalPayAmount);
                    if (invoiceApplyInfo.ApplyKind==(int)ApplyInvoiceKindType.Credit)
                    {
                        var bindGoods = _storageRecord.GetStorageRecordDetailsByLinkTradeCode(details.Select(ent => ent.Tradecode));
                        foreach (var item in details)
                        {
                            var dataSource = bindGoods.Where(ent => ent.LinkTradeCode == item.Tradecode);
                            if (dataSource.Sum(ent => ent.Quantity * ent.UnitPrice) != item.TotalPayAmount)
                                return new ResultInfo(false, string.Format("发票申请插入失败！要货申请{0}和出库总金额不一致", item.Tradecode));
                            item.LinkTradeCodes = string.Join(",", dataSource.Select(ent => ent.TradeCode));
                        }
                    }
                }
                var result = _invoiceApply.Insert(invoiceApplyInfo, details);
                if (result)
                {
                    string remark = string.Format("[发票申请新增：{0} {1}]{2}", realName ,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), invoiceApplyInfo.ApplyRemark);
                    _operationLogManager.Add(personnelid, realName, invoiceApplyInfo.ApplyId,
                        invoiceApplyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1,
                        remark);
                    return new ResultInfo(true,string.Empty);
                }
                return new ResultInfo(false, "发票申请插入失败！");
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        public ResultInfo Update(Guid personnelId, string realName, InvoiceApplyInfo invoiceApplyInfo,List<InvoiceApplyDetailInfo> details)
        {
            try
            {
                var applyInfo = _invoiceApply.GetInvoiceApplyInfo(invoiceApplyInfo.ApplyId);
                if (applyInfo == null)
                    return new ResultInfo(false, "发票申请不存在!");
                if (applyInfo.ApplyState != (int)ApplyInvoiceState.WaitAudit)
                    return new ResultInfo(false, string.Format("当前发票申请状态为{0}!", Enum.Attribute.EnumAttribute.GetKeyName((ApplyInvoiceState)applyInfo.ApplyState)));
                invoiceApplyInfo.ApplyState = (int)ApplyInvoiceState.WaitAudit;
                if (applyInfo.ApplySourceType == (int)ApplyInvoiceSourceType.League)
                {
                    invoiceApplyInfo.Amount = details.Sum(ent => ent.TotalPayAmount);
                }
                var result = _invoiceApply.Modify(invoiceApplyInfo, details);
                if (result)
                {
                    string remark = string.Format("[发票申请编辑：{0} {1}]", realName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    _operationLogManager.Add(personnelId, realName, applyInfo.ApplyId, applyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1, remark);
                    return new ResultInfo(true, string.Empty);
                }
                return new ResultInfo(false, "发票申请编辑失败！");
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        public ResultInfo Again(Guid personnelId, string realName, InvoiceApplyInfo invoiceApplyInfo,List<InvoiceApplyDetailInfo> details)
        {
            try
            {
                var applyInfo = _invoiceApply.GetInvoiceApplyInfo(invoiceApplyInfo.ApplyId);
                if (applyInfo == null)
                    return new ResultInfo(false, "发票申请不存在!");
                if (applyInfo.ApplyState != (int)ApplyInvoiceState.Retreat)
                    return new ResultInfo(false, string.Format("当前发票申请状态为{0}!", Enum.Attribute.EnumAttribute.GetKeyName((ApplyInvoiceState)applyInfo.ApplyState)));
                invoiceApplyInfo.ApplyState = (int) ApplyInvoiceState.WaitAudit;
                if (applyInfo.ApplySourceType==(int)ApplyInvoiceSourceType.League)
                {
                    invoiceApplyInfo.Amount = details.Sum(ent => ent.TotalPayAmount);
                }
                var result = _invoiceApply.Modify(invoiceApplyInfo,details);
                if (result)
                {
                    string remark = string.Format("[发票申请重送：{0} {1}]", realName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    _operationLogManager.Add(personnelId, realName, applyInfo.ApplyId, applyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1, remark);
                    return new ResultInfo(true, string.Empty);
                }
                return new ResultInfo(false, "发票申请重送失败！");
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        public ResultInfo Cancel(Guid personnelId, string realName, Guid applyId,string remark)
        {
            try
            {
                var applyInfo = _invoiceApply.GetInvoiceApplyInfo(applyId);
                if (applyInfo==null)
                    return new ResultInfo(false,"发票申请不存在!");
                if(applyInfo.ApplyState!= (int)ApplyInvoiceState.WaitAudit && applyInfo.ApplyState!= (int)ApplyInvoiceState.Retreat)
                    return new ResultInfo(false, string.Format("当前发票申请状态为{0}!", Enum.Attribute.EnumAttribute.GetKeyName((ApplyInvoiceState)applyInfo.ApplyState)));
                var result = _invoiceApply.Update(applyId,(int)ApplyInvoiceState.Cancel,string.Empty,string.Empty,Guid.Empty);
                if (result)
                {
                    string description = string.Format("[发票申请作废：{0} {1}][作废理由：{2}]", realName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), remark);
                    _operationLogManager.Add(personnelId, realName, applyId,applyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1,
                        description);
                    return new ResultInfo(true, string.Empty);
                }
                return new ResultInfo(false, "发票申请作废失败！");
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        /// <summary>
        /// 发票申请查询
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="searchKey">要货申请号/订单号</param>
        /// <param name="targetId">门店Id/销售平台Id</param>
        /// <param name="applyStates">发票申请状态</param>
        /// <param name="invoiceType">发票申请类型</param>
        /// <param name="sourceType"></param>
        /// <param name="applyKind"></param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="total">总条数</param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public List<InvoiceApplyGridDTO> SearchApply(DateTime startTime,DateTime endTime,string searchKey, Guid targetId, IEnumerable<int> applyStates, int invoiceType,int sourceType,int applyKind,int pageIndex, int pageSize,out long total)
        {
            var result = _invoiceApply.GetInvoiceApplyInfosWithPage(startTime, endTime, searchKey, applyStates,
                invoiceType, sourceType, applyKind, targetId, pageIndex, pageSize, out total);
            var dics = sourceType == (int)ApplyInvoiceSourceType.Order? 
                Cache.SalePlatform.Instance.ToList().ToDictionary(k=>k.ID,v=>v.Name):
               Cache.Filiale.Instance.ToList().ToDictionary(k => k.ID, v => v.Name);
            var applyStateDics = Enum.Attribute.EnumAttribute.GetDict<ApplyInvoiceState>();
            var applyTypeDics = Enum.Attribute.EnumAttribute.GetDict<ApplyInvoiceType>();
            result.Select(ent => new InvoiceApplyGridDTO
            {
                Amount=ent.Amount,
                ApplyDateTime = ent.ApplyDateTime,
                ApplyId = ent.ApplyId,
                ApplyState = applyStateDics[ent.ApplyState],
                InvoiceType = ent.ApplyType,
                InvoiceTypeName = applyTypeDics[ent.ApplyType],
                Receiver = ent.Receiver,
                IsCanCancel = ent.ApplyState==(int)ApplyInvoiceState.WaitAudit || ent.ApplyState == (int)ApplyInvoiceState.Retreat,
                IsCanEdit = ent.ApplyState == (int)ApplyInvoiceState.WaitAudit || ent.ApplyState == (int)ApplyInvoiceState.Retreat,
                TargetName = dics.ContainsKey(ent.SalePlatformId)? dics[ent.SalePlatformId]:string.Empty
            }).ToList();
            return null;
        }

        /// <summary>
        /// 获取发票申请信息
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public InvoiceApplyInfo GetData(Guid applyId)
        {
            var invoiceApplyInfo = _invoiceApply.GetInvoiceApplyInfo(applyId);
            if (invoiceApplyInfo != null)
            {
                if (invoiceApplyInfo.ApplySourceType == (int)ApplyInvoiceSourceType.League)
                    invoiceApplyInfo.Details = _invoiceApply.GetInvoiceApplyDetailInfos(applyId);
            }
            return invoiceApplyInfo;
        }

        /// <summary>
        /// 核准
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="applyId"></param>
        /// <param name="remark"></param>
        /// <param name="personnelId"></param>
        /// <returns></returns>
        public ResultInfo Approval(Guid personnelId,string realName,Guid applyId)
        {
            try
            {
                var invoiceApplyInfo = _invoiceApply.GetInvoiceApplyInfo(applyId);
                if (invoiceApplyInfo == null)
                    return new ResultInfo(false, "未找到对应的申请单据信息！");
                if (invoiceApplyInfo.ApplyState != (int)ApplyInvoiceState.WaitAudit)
                    return new ResultInfo(false, "只有待审核状态下的发票申请才允许核准操作！");
                var result = _invoiceApply.Update(applyId, (int)ApplyInvoiceState.WaitInvoicing, string.Empty, string.Empty,Guid.Empty);
                if (result)
                {
                    string description = string.Format("[发票申请审核：{0} {1}]", realName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    _operationLogManager.Add(personnelId, realName, applyId, invoiceApplyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1,
                        description);
                    return new ResultInfo(true, string.Empty);
                }
                return new ResultInfo(false, "发票申请审核失败！");
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        /// <summary>
        /// 开票核准
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="invoiceRelations">开票总金额</param>
        /// <param name="remark"></param>
        /// <param name="personnelId"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
        public ResultInfo InvoicePass(Guid applyId, List<InvoiceRelationInfo> invoiceRelations,Guid personnelId,string realName)
        {
            try
            {
                if(invoiceRelations.All(ent=>!ent.IsCanEdit))
                    return new ResultInfo(false,"请录入需新增的发票信息！");
                if(invoiceRelations.Any(ent=>ent.UnTaxFee+ent.TaxFee!=ent.TotalFee))
                    return new ResultInfo(false, "每条发票信息未税金额+税额必须等于含税金额！");
                var invoiceApplyInfo = GetInvoiceApplyInfo(applyId);
                if (invoiceApplyInfo == null)
                    return new ResultInfo(false, "未找到对应的申请单据信息！");
                if (invoiceApplyInfo.ApplyState != (int)ApplyInvoiceState.WaitInvoicing &&
                    invoiceApplyInfo.ApplyState != (int)ApplyInvoiceState.Invoicing)
                    return new ResultInfo(false, "只有待开票/开票中状态下的发票申请才允许开票操作！");
                if (invoiceApplyInfo.ApplyType!=(int)ApplyInvoiceType.Special && Math.Abs(invoiceRelations.Sum(ent => ent.TotalFee)) > invoiceApplyInfo.Amount)
                    return new ResultInfo(false, "开票金额不能大于申请金额！");
                var isFinish = invoiceApplyInfo.ApplyType != (int)ApplyInvoiceType.Special 
                    ? Math.Abs(invoiceRelations.Sum(ent => ent.TotalFee)) == invoiceApplyInfo.Amount
                    : Math.Abs(Math.Abs(invoiceRelations.Sum(ent => ent.TotalFee)) - invoiceApplyInfo.Amount)<=decimal.Parse("0.05");
                List<SourceBindGoods> bindGoods = isFinish ? (invoiceApplyInfo.ApplyKind == (int)ApplyInvoiceSourceType.League ? _storageRecord.GetStorageRecordDetailsByLinkTradeCode(invoiceApplyInfo.Details.Select(ent => ent.Tradecode)): _goodsOrderDetail.GetSourceBindGoodsesByOrderNo(invoiceApplyInfo.TradeCode))
                    :new List<SourceBindGoods>();
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var result = _invoiceApply.Update(applyId, isFinish
                            ? (int)ApplyInvoiceState.Finished
                            : (int)ApplyInvoiceState.Invoicing, string.Empty, string.Empty, personnelId);
                    if (!result)
                        return new ResultInfo(result, "发票申请开票失败！");
                    bool success = true;
                    foreach (var invoiceGroup in invoiceRelations.GroupBy(ent=>ent.InvoiceNo))
                    {
                        if(invoiceGroup.Count()>1)
                            return new ResultInfo(false, string.Format("发票添加重复【发票号码{0}】", invoiceGroup.Key));
                        var invoice = invoiceGroup.First();
                        List<InvoiceBindGoods> dataSource = new List<InvoiceBindGoods>();
                        if (isFinish)
                        {
                            var index = 1;
                            foreach(var item in bindGoods.GroupBy(ent => ent.GoodsId))
                            {
                                var first = item.First();
                                dataSource.Add(new InvoiceBindGoods {
                                    Id =index,
                                    GoodsId =item.Key,
                                    GoodsCode = first.GoodsCode,
                                    InvoiceId = invoice.InvoiceId,
                                    Quantity =item.Sum(ent=>ent.Quantity),
                                    UnitPrice =first.UnitPrice });
                                index++;
                            }
                        }
                        success = _invoiceApply.InsertRelation(invoice, dataSource);
                        if (!success)
                        {
                            break;
                        }
                    }
                    if (success)
                    {
                        tran.Complete();

                        string description = string.Format("[发票申请开票：{0} {1}]", realName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        _operationLogManager.Add(personnelId, realName, applyId, invoiceApplyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1,
                            description);

                        return new ResultInfo(true, "");
                    }
                    return new ResultInfo(false, "插入发票失败");
                }
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        /// <summary>
        /// 开票核退
        /// </summary>
        /// <param name="personnelId"></param>
        /// <param name="realName"></param>
        /// <param name="applyId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ResultInfo InvoiceReturn(Guid personnelId,string realName,Guid applyId, string remark)
        {
            try
            {
                var invoiceApplyInfo = _invoiceApply.GetInvoiceApplyInfo(applyId);
                if (invoiceApplyInfo == null)
                    return new ResultInfo(false, "未找到对应的申请单据信息！");
                if (invoiceApplyInfo.ApplyState != (int)ApplyInvoiceState.WaitInvoicing)
                    return new ResultInfo(false, "只有待开票状态下的发票申请才允许核退操作！");
                var result = _invoiceApply.Update(applyId, (int)ApplyInvoiceState.Retreat, string.Empty, remark,Guid.Empty);
                if (result)
                {
                    string description = string.Format("[发票申请开票核退：{0} {1}]{2}", realName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), remark);
                    _operationLogManager.Add(personnelId, realName, applyId, invoiceApplyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1,
                        description);
                    return new ResultInfo(true, string.Empty);
                }
                return new ResultInfo(result, "发票申请开票核退失败！");
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        /// <summary>
        /// 核退
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="applyId"></param>
        /// <param name="remark"></param>
        /// <param name="personnelId"></param>
        /// <returns></returns>
        public ResultInfo Retreat(Guid personnelId,string realName,Guid applyId,string remark)
        {
            try
            {
                var invoiceApplyInfo = _invoiceApply.GetInvoiceApplyInfo(applyId);
                if (invoiceApplyInfo == null)
                    return new ResultInfo(false, "未找到对应的申请单据信息！");
                if (invoiceApplyInfo.ApplyState != (int)ApplyInvoiceState.WaitAudit)
                    return new ResultInfo(false, "只有待审核状态下的发票申请才允许核退操作！");
                var result = _invoiceApply.Update(applyId, (int)ApplyInvoiceState.Retreat, string.Empty, remark, Guid.Empty);
                if (result)
                {
                    string description = string.Format("[发票申请核退：{0} {1}]{2}", realName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), remark);
                    _operationLogManager.Add(personnelId, realName, applyId, invoiceApplyInfo.TradeCode, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), 1,
                        description);
                    return new ResultInfo(true, string.Empty);
                }
                return new ResultInfo(result, "发票申请核退失败！");
            }
            catch (Exception ex)
            {
                return new ResultInfo(false, ex.Message);
            }
        }

        public List<InvoiceApplyInfo> GetInvoiceApplyApprovalList(DateTime startTime, DateTime endTime, string searchKey, int applyState, int applyType, int applyKindType, Guid saleFilialeId, Guid salePlatformId)
        {
            var applyStates = applyState != (int)ApplyInvoiceState.All ? new List<int> { applyState } : new List<int>();
            return _invoiceApply.GetInvoiceApplyInfos(startTime, endTime, searchKey, applyStates, applyType, applyKindType, saleFilialeId, salePlatformId);
        }

        public List<InvoiceApplyInfo> GetInvoiceApplyMakeList(DateTime startTime, DateTime endTime, string searchKey, int applyState, int applyType, int applyKindType)
        {
            var applyStates = applyState != (int)ApplyInvoiceState.All ? new List<int> { applyState } : new List<int> { (int)ApplyInvoiceState.WaitInvoicing, (int)ApplyInvoiceState.Invoicing, (int)ApplyInvoiceState.Finished, (int)ApplyInvoiceState.Retreat };
            return _invoiceApply.GetInvoiceApplyInfos(startTime, endTime, searchKey, applyStates, applyType, applyKindType, Guid.Empty, Guid.Empty);
        }

        public InvoiceApplyInfo GetInvoiceApplyInfo(Guid applyId)
        {
            var invoiceApplyInfo = _invoiceApply.GetInvoiceApplyInfo(applyId);
            if (invoiceApplyInfo != null)
            {
                if(invoiceApplyInfo.ApplySourceType==(int)ApplyInvoiceSourceType.League)
                    invoiceApplyInfo.Details = _invoiceApply.GetInvoiceApplyDetailInfos(applyId);
                invoiceApplyInfo.RelationInfos = _invoiceApply.GetInvoiceRelationInfos(applyId);
            }
            return invoiceApplyInfo;
        }

        public List<InvoiceDTO> GetInvoiceDtos(DateTime startTime,DateTime endTime)
        {
            return _invoiceApply.GetBindGoodses(startTime, endTime);
        } 
    }
}
