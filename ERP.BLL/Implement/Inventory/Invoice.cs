using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.SAL;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class Invoice : BllInstance<Invoice>
    {
        readonly IInvoice _invoiceDao;

        public Invoice(Environment.GlobalConfig.DB.FromType fromType)
        {
            _invoiceDao = InventoryInstance.GetInvoiceDao(fromType);
        }

        /// <summary>
        /// 添加发票
        /// </summary>
        /// <param name="invoice">发票类</param>
        public void Insert(InvoiceInfo invoice)
        {
            if (invoice.SaleFilialeId == Guid.Empty) return;
            _invoiceDao.Insert(invoice);
        }

        /// <summary>
        /// 获取指定的发票索取记录
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoice(Guid invoiceId)
        {
            if (invoiceId == Guid.Empty) return null;
            return _invoiceDao.GetInvoice(invoiceId);
        }

        /// <summary>
        /// Func : 根据订单号，获取指定的发票索取记录
        /// Code : dyy
        /// Date : 2009 Nov 26th
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoiceByGoodsOrder(Guid orderId)
        {
            if (orderId == Guid.Empty) return null;
            return _invoiceDao.GetInvoiceByGoodsOrder(orderId);
        }

        /// <summary>
        /// 根据条件查找发票
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"></param>
        /// <param name="invoiceName"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="address"></param>
        /// <param name="invoiceContent"></param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="isNeedManual">是否需要手动打印发票</param>
        /// <param name="warehouseIds"></param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="cancelPersonnel"></param>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceList(DateTime startTime, DateTime endTime, bool isOrderComplete, string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent, InvoiceState invoiceState, bool isNeedManual, IEnumerable<Guid> warehouseIds,int invoiceType,Guid saleFilialeId,string cancelPersonnel)
        {
            return _invoiceDao.GetInvoiceList(startTime, endTime, isOrderComplete, orderNo, invoiceName, invoiceNo, address, invoiceContent, invoiceState, isNeedManual, warehouseIds,invoiceType,saleFilialeId,cancelPersonnel);
        }

        /// <summary>
        /// 根据条件查找发票(分页)
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"> </param>
        /// <param name="invoiceName"> </param>
        /// <param name="invoiceNo"> </param>
        /// <param name="address"> </param>
        /// <param name="invoiceContent"> </param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="isNeedManual">是否需要手动打印发票</param>
        /// <param name="warehouseId"> </param>
        /// <param name="permissionFilialeId"> </param>
        /// <param name="permissionBranchId"> </param>
        /// <param name="permissionPositionId"> </param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeid"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="cancelPersonel"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceListByPage(DateTime startTime, DateTime endTime, bool isOrderComplete,
            string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent,
            InvoiceState invoiceState, bool isNeedManual, Guid warehouseId, Guid permissionFilialeId,
            Guid permissionBranchId, Guid permissionPositionId, int invoiceType, Guid saleFilialeid, string cancelPersonel, int pageIndex,
            int pageSize, out int recordCount)
        {
            return _invoiceDao.GetInvoiceListByPage(startTime, endTime, isOrderComplete, orderNo, invoiceName, invoiceNo, address, invoiceContent, invoiceState, isNeedManual, warehouseId,
                permissionFilialeId, permissionBranchId, permissionPositionId, invoiceType, saleFilialeid, cancelPersonel, pageIndex, pageSize, out recordCount);
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        public bool SetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel)
        {
            if (invoiceId != Guid.Empty)
                return _invoiceDao.SetInvoiceState(invoiceId, invoiceState, cancelPersonel);
            return false;
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceIdList">发票编号列表</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        /// zal 2016-12-27
        public bool BatchSetInvoiceState(List<Guid> invoiceIdList, InvoiceState invoiceState, string cancelPersonel)
        {
            if (invoiceIdList.Count > 0)
                return _invoiceDao.BatchSetInvoiceState(invoiceIdList, invoiceState, cancelPersonel);
            return false;
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        public bool UpdateSetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel)
        {
            if (invoiceId != Guid.Empty)
                return _invoiceDao.UpdateSetInvoiceState(invoiceId, invoiceState, cancelPersonel);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rollInfo"></param>
        /// <returns></returns>
        public bool AddInvoiceRoll(InvoiceRoll rollInfo)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var invoiceRollDetail = CreateRollDetail(rollInfo);
                    if (invoiceRollDetail.Count > 0)
                    {
                        _invoiceDao.InsertRoll(rollInfo);
                        foreach (var detail in invoiceRollDetail)
                        {
                            _invoiceDao.InsertRollDetail(detail);
                        }
                        tran.Complete();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rollInfo"></param>
        /// <returns></returns>
        public bool UpdateInvoiceRoll(InvoiceRoll rollInfo)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var state = _invoiceDao.SumRollDeatilState(rollInfo.Id);
                    if (state == 0)
                    {
                        var invoiceRollDetail = CreateRollDetail(rollInfo);
                        if (invoiceRollDetail.Count > 0)
                        {
                            _invoiceDao.UpdateRoll(rollInfo);
                            _invoiceDao.DeleteRollDetail(rollInfo.Id);
                            foreach (var detail in invoiceRollDetail)
                            {
                                _invoiceDao.InsertRollDetail(detail);
                            }
                            tran.Complete();
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rollInfo"></param>
        /// <returns></returns>
        private static IList<InvoiceRollDetail> CreateRollDetail(InvoiceRoll rollInfo)
        {
            IList<InvoiceRollDetail> rollDetailList = new List<InvoiceRollDetail>();
            var rollIntervalQuantity = rollInfo.InvoiceCount / rollInfo.InvoiceRollCount;
            for (int i = 0; i < rollInfo.InvoiceRollCount; i++)
            {
                var detail = new InvoiceRollDetail { RollId = rollInfo.Id, StartNo = rollInfo.InvoiceStartNo + (rollIntervalQuantity * i) };
                if (i == rollInfo.InvoiceRollCount - 1)
                {
                    detail.EndNo = rollInfo.InvoiceEndNo;
                }
                else
                {
                    detail.EndNo = rollInfo.InvoiceStartNo + (rollIntervalQuantity * (i + 1) - 1);
                }
                detail.State = InvoiceRollState.NoInvocation;
                rollDetailList.Add(detail);
            }
            return rollDetailList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<InvoiceRoll> GetInvoiceRollList()
        {
            return _invoiceDao.GetRollList();
        }

        /// <summary>
        /// 发票报送到
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="filialeId"></param>
        public void InvoiceCommit(DateTime startDate, DateTime endDate, Guid filialeId)
        {
            startDate = Convert.ToDateTime(startDate.ToString("yyyy-MM-dd") + " " + "00:00:00");
            endDate = Convert.ToDateTime(endDate.ToString("yyyy-MM-dd") + " " + "23:59:59");
            _invoiceDao.InvoiceCommit(startDate, endDate, filialeId);
        }

        /// <summary>
        /// 指定状态获取发票卷信息
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<InvoiceRollDetail> GetRollDetailListByState(InvoiceRollState state)
        {
            return _invoiceDao.GetRollDetailListByState(state);
        }

        /// <summary>
        /// 发票汇总搜索
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="pageSize"> </param>
        /// <param name="pageIndex"> </param>
        /// <param name="recordCount"> </param>
        /// <returns></returns>
        public InvoiceStatisticsInfo Search(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo, int pageSize, int pageIndex, out int recordCount)
        {
            var items = _invoiceDao.Search(startTime, endTime, filialeId, noteType, invoiceNo, pageSize, pageIndex, out recordCount);
            var invoiceNoteStatisticsInfoList = _invoiceDao.InvoiceNoteStatistics(startTime, endTime, filialeId, noteType, invoiceNo);
            return new InvoiceStatisticsInfo(invoiceNoteStatisticsInfoList, items);
        }

        /// <summary>发票汇总导出Excel专用
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceKindType"></param>
        /// <returns></returns>
        public IList<InvoiceBriefInfo> OutPutExcelInvoice(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo, byte invoiceKindType)
        {
            return _invoiceDao.OutPutExcelInvoice(startTime, endTime, filialeId, noteType, invoiceNo);
        }
    }
}
